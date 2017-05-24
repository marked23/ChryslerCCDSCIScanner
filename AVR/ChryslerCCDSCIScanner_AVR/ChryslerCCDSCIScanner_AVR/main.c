/*
 * ChryslerCCDSCIScanner_AVR
 * Copyright (C) 2016-2017, László Dániel
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/******************************************************************************
Project:  CHRYSLER CCD/SCI SCANNER
Author:   László Dániel
Email:    laszlodaniel@gmail.com
Date:     2017-03-21
Version:  V1.30
Purpose:  receive and transmit messages from and to Chrysler's legacy CCD/SCI-bus
Software: Atmel Studio 7.0
Hardware: CHRYSLER CCD/SCI SCANNER V1.30 PCB (ATmega2560 + ATmega8U2 + CDP68HC68S1E + CD4066 + MCP23017)
License:  GNU General Public License

Based on original program by Juha Niinikoski:
http://www.kolumbus.fi/juha.niinikoski/CCD_bus/ccd_display.htm

UART code is based on original library by Andy Gock:
https://github.com/andygock/avr-uart

They helped me a lot:
Sergey Tsinin with BCM firmware reverse engineering and sharing general knowledge of diagnostic request messages.
Kyle Repinski with understanding the DRB-III's database file.
Many others with little things.

Supported OBD-II pin layout:
3:  CCD+
4:  Chassis Ground (not used)
5:  Signal Ground (used)
6:  SCI Receive  (RX) (PCM)
7:  SCI Transmit (TX) (PCM, TCM)
11: CCD-
14: SCI Receive  (RX) (TCM)
16: +12V

Supported cars: all Chrysler vehicles (manual or auto transmission) equipped 
with CCD- and/or SCI-bus with above pin configuration (typical years between 1989-2004).

The program uses the 2560's hardware UARTs to communicate with both buses:
- UART0: CCD-bus communication,
- UART1: SCI-bus communication (exclusive for PCM and TCM),
- UART2: Packet-based USB communication between scanner and laptop,
- UART3: 1-wire communication with DS18B20 temperature sensor (independent from scanner).

Note: this scanner cannot be used to re-flash the PCM and/or TCM,
it doesn't have the necessary circuitry to achieve that!
If you figure out a way to do it anyway using this hardware and software I cannot
be held responsible for the damages.
******************************************************************************/


/******************************************************************************
                                   Includes                                    
******************************************************************************/

// AVR libraries
#include <avr/io.h>
#include <avr/boot.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include <util/twi.h>
#include <util/atomic.h>
#include <util/delay.h>
#include <inttypes.h>
#include <stdint-gcc.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <avr/wdt.h>

// Custom libraries
#include "uart.h"				// UART serial communication
#include "ccdsci.h"				// CCD/SCI protocols
#include "i2cmaster.h"			// I2C standard library
#include "exteeprom.h"			// External EEPROM driver (using I2C)
#include "mcp23017.h"			// I/O expander for buttons (using I2C)
#include "packet.h"				// Serial Packet Communication protocols
#include "millis.h"             // Measure elapsed time for timeout

#include "lcd_driver_KS0107.h"	// KS0107 LCD driver
#include "lcd_graphics.h"		// Extended graphics for KS0107
#include "flanders_bmp.h"		// Bitmap file (example)
#include "chrysler_logo_bmp.h"
#include "chrysler_keyboard_01.h"
#include "doge_wow.h"


/*
          SYSTEM INFO          
-------------------------------
   MCU: ATmega2560-16AU
 CLOCK: 16MHz
 FLASH: 256kB
   RAM: 8kB
EEPROM: 4kB
-------------------------------
   USB: ATmega8U2-AU
 CLOCK: 16MHz
 FLASH: 8kB
   RAM: 0.5kB
EEPROM: 0.5kB
-------------------------------
   CCD: CDP68HC68S1E @ 1 MHz
   SCI: CD4066
   BTN: MCP23017
-------------------------------
EEPROM: 24LC512-I/P
  SIZE: 2x64kB
-------------------------------
   LCD: 128x64 pixels
DRIVER: KS0107/KS0108

*/



/******************************************************************************
                                   Variables                                   
******************************************************************************/

bool ccd_enabled = false;
bool sci_enabled = false;
bool lcd_enabled = false;
bool button_pressed = false;

// Create I/O expander object
MCP23017 mcp;

// Create EEPROM object
// TODO: implement object oriented source in exteeprom.c/h
_24LC512 eep;

uint32_t previous_millis = 0;
uint32_t interval = 500;

volatile uint32_t ccd_messages_count;
volatile uint32_t sci_messages_count;

bool ccd_bus_present = false;
bool sci_bus_present = false;

// Timeout values for packets
uint8_t command_timeout = 100; // milliseconds
uint16_t command_purge_timeout = 1000; // milliseconds

// Store handshake in the PROGram MEMory (flash)
const uint8_t handshake_progmem[] PROGMEM = { 0x43, 0x48, 0x52, 0x59, 0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44, 0x53, 0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E, 0x45, 0x52 };
//											    "C     H     R     Y     S     L     E     R     C     C     D     S     C     I     S     C     A     N     N     E     R"

uint8_t handshake_array[21];

uint8_t ccd_msg_count = 0;
uint8_t sci_msg_count = 0;

volatile bool ccd_idle;
volatile bool ccd_active_byte;

uint8_t ccd_bus_bytes_buffer[32];
uint8_t ccd_bus_bytes_buffer_ptr = 0;

bool ccd_bus_msg_pending = false;
uint8_t ccd_bus_msg_to_send[16];
uint8_t ccd_bus_msg_to_send_ptr = 0;

uint8_t sci_bus_bytes_buffer[32];
uint8_t sci_bus_bytes_buffer_ptr = 0;

bool sci_bus_msg_pending = false;
uint8_t sci_bus_msg_to_send[16];
uint8_t sci_bus_msg_to_send_ptr = 0;

uint32_t last_sci_byte_received = 0;
bool sci_bus_active_bytes = false;

uint8_t ccd_bus_module_addr[32];
uint8_t ccd_bus_module_num = 0;

uint8_t avr_signature[32];

uint8_t ccd_mod_addr[32];
uint8_t ccd_mod_addr_ptr = 0;
//uint8_t ccd_addr_offset = 0;
uint8_t ccd_scan_msg[6];
bool scan_ccd = false;
bool scan_ccd_done = false;
bool next_ccd_addr = true;
uint32_t last_ccd_scan_request = 0;
uint16_t ccd_scan_request_timeout = 200; // ms
uint8_t ccd_scan_start_addr = 0x00;
uint8_t ccd_scan_end_addr = 0xFE;

bool sci_bus_msg_rep = false;
bool ccd_bus_msg_rep = false;

bool sci_command_14 = false; // request sensor data from SCI-bus (low speed)
uint8_t sci_command_14_buffer[4] = { 0x14, 0x00, 0x00, 0x00 }; // Probably 3 bytes are enough...

bool sci_bus_high_speed = false;
bool sci_hs_mode_f4 = false;

// SCI-bus high speed parameters (Mode F4)
// Command location(s)
uint8_t mode_f4_parameters_loc[64]; // max 64 location bytes!

// Command length(s)
uint8_t mode_f4_parameters_length[64]; // max 64 length bytes!

// Commands each after another (starting with 0xF4)
uint8_t mode_f4_parameters[128]; // max 128 parameter bytes!

// Pointer value for navigating inside the location, length and parameters arrays
uint8_t mode_f4_ptr = 0;
uint8_t mode_f4_ptr_length = 0;
uint8_t mode_f4_length = 0;

// Mode F4 parameters explained
/***************************************************************************************************************
F4 0A 0B	- engine rpm			- scaling: RAW * 0.125 [rpm]
F4 35 36	- target idle rpm		- scaling: RAW * 0.125 [rpm]
F4 0C 0D	- speed					- scaling: RAW * 0.0251057664 [km/h]
F4 0F		- battery voltage		- scaling: RAW * 0.0618 [V]
F4 3A		- charging voltage		- scaling: RAW * 0.0618 [V]
F4 12		- tps volts				- scaling: RAW * 0.0196 [V]
F4 13		- minimum tps volts		- scaling: RAW * 0.0196 [V]
F4 14		- calculated tps volts	- scaling: RAW * 0.0196 [V]
F4 17		- map volts				- scaling: RAW * 0.0196 [V]
F4 DA		- map vacuum			- scaling: RAW * 0.412054294 [kPa]
F4 1B		- o2 volts				- scaling: RAW * 0.0196 [V]
F4 27 28	- injector pulsewidth	- scaling: RAW * 0.004 [ms]
F4 3E		- iac steps				- scaling: RAW * 1
F4 37		- target iac steps		- scaling: RAW * 1
F4 2F		- spark advance			- scaling: RAW * 0.5 [°]
F4 XX (31)	- cylinder 1 retard		- scaling: RAW * 0.5 [°]
F4 XX (32)	- cylinder 2 retard		- scaling: RAW * 0.5 [°]
F4 XX (33)	- cylinder 3 retard		- scaling: RAW * 0.5 [°]
F4 XX (34)	- cylinder 4 retard		- scaling: RAW * 0.5 [°]
F4 1F		- knock volts			- scaling: RAW * 0.0196 [V]
F4 30		- overall knock retard	- scaling: RAW * 0.5 [°]
F4 41		- dis signal			- masking: 0x01: sync history - true: in-sync; false: out-of-sync
											   0x10: current sync state - true: in-sync; false: out-of-sync
											   0x20: ckp state - true: present; false: lost
											   0x40: cmp state - true: present; false: lost
F4 42		- fuel system status	- masking: 0x01: open loop
											   0x02: closed loop
											   0x04: open loop drive
											   0x08: open loop dtc
											   0x10: closed loop dtc
											   0x20: o2 sensor is updating adaptive memory
											   0x40: closed loop
											   0x80: ready for closed loop
F4 4A		- closed loop timer		- scaling: RAW * 0.0535 [min]
F4 43		- current adaptive cell	- scaling: RAW * 1
F4 D6		- short-term adaptive	- scaling: RAW * 0.1953 [%]
F4 D7		- long-term adaptive	- scaling: RAW * 0.1953 [%]
F4 DE		- fuel level volts		- scaling: RAW * 0.0196 [V]
F4 XX (E3)	- engine load			- scaling: RAW * 0.3922 [%]
F4 10		- ambient temp volts	- scaling: RAW * 0.0196 [V]
F4 11		- ambient temp deg		- scaling: RAW - 128 [°C]
F4 15		- engine coolant volts	- scaling: RAW * 0.0196 [V]
F4 16		- engine coolant deg	- scaling: RAW - 128 [°C]
F4 1D		- intake air temp volts	- scaling: RAW * 0.0196 [V]
F4 1E		- intake air temp deg	- scaling: RAW - 128 [°C]
F4 25		- a/c pressure volts	- scaling: RAW * 0.0196 [V]
F4 26		- a/c pressure			- scaling: RAW * 13.52061848 [kPa]
F4 19		- barometric pressure	- scaling: RAW * 0.412054294 [kPa]
F4 73		- limp-in reason		- masking: 0x08: IAT DTC (intake air temperature sensor malfunction)
											   0x10: TPS DTC (throttle pedal sensor malfunction)
											   0x20: MAP EL DTC (manifold absolute pressure sensor malfunction)
											   0x40: MAP VA DTC (manifold absolute pressure sensor malfunction)
											   0x80: ECT DTC (engine coolant temperature sensor malfunction)
F4 4B 4C	- time from start/run	- scaling: RAW * 0.0002 [min]
F4 4D		- runtime at stall		- scaling: RAW * 0.0535 [min]
F4 3C		- desired relay states	- masking: 0x10: asd relay - true: off; false: on
F4 7A		- actual relay states	- masking: 0x10: asd relay - true: on; false: off
***************************************************************************************************************/


/******************************************************************************
                                   Functions                                   
******************************************************************************/


/*****************************************************************************
Function: read_avr_signature()
Purpose:  read signature bytes into a global array
******************************************************************************/
void read_avr_signature(void)
{
	for (uint8_t i = 0; i < 32; i++)
	{
		avr_signature[i] = boot_signature_byte_get(i);
	}

} /* read_avr_signature */


/*****************************************************************************
Function: avr8_software_reset()
Purpose:  force software reset by watchdog timer timeout.
Note:     this is an elegant way to reset the avr purely by software
          because all registers return to they default value
		  and the execution jumps to the beginning address. Just call this
		  function anytime to force a software reset.
******************************************************************************/
void avr8_software_reset(void)
{
	wdt_enable(WDTO_15MS);
	for(;;)
	{
		// wait for the reset for 15 milliseconds (should be fast enough)
	}
}


/*****************************************************************************
Function: wdt_init()
Purpose:  initialize software reset.
******************************************************************************/
void wdt_init(void)
{
	MCUSR = 0;
	wdt_disable();
	return;
}


/*************************************************************************
Function: check_commands()
Purpose:  check if a command has been received from the laptop.
Notes:    UART2 is used to send and receive packets through USB.
**************************************************************************/
void check_commands(void)
{
	// Proceed only if the packet receive buffer (UART2) contains at least 5 bytes.
	// Two dummy bytes are always there in the beginning (caused by the ATmega8U2)!!!
	if (uart2_available_rx() > 4)
	{
		// Make some local variables, they will disappear after the function ends
		uint8_t sync, length_hb, length_lb, datacode, subdatacode, checksum;
		bool payload_bytes = false;
		uint16_t bytes_to_read = 0;
		uint16_t payload_length = 0;
		uint8_t calculated_checksum = 0;

		uint8_t ack[1] = { ACK }; // acknowledge payload array (0x00)
		uint8_t err[1] = { ERR }; // error payload array (0xFF)

		uint32_t command_timeout_start = 0;
		bool command_timeout_reached = false;

		// Find the first SYNC byte (0x33)
		command_timeout_reached = false;
		command_timeout_start = millis_get();
		while ( ((uart2_peek() & 0xFF) != SYNC_BYTE) && !command_timeout_reached)
		{
			// If it's not the SYNC byte then get rid of it
			uart2_getc();

			// Determine if timeout has been reached
			if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true;
		}
		command_timeout_reached = false;

		if (uart2_available_rx() == 0) return; // exit this function right here if there are no bytes left

		// Read 3 bytes (one SYNC and two LENGTH byte).
		// All UART reads are masked with 0xFF because the ring buffer
		// contains words (two bytes). MSB contains UART flags, LSB contains
		// the actual data byte. The "& 0xFF" mask gets rid of the MSB, effectively zeroing them out.
		sync      = uart2_getc() & 0xFF;
		length_hb = uart2_getc() & 0xFF;
		length_lb = uart2_getc() & 0xFF;

		// Calculate how much more bytes should we read.
		bytes_to_read = (length_hb << 8 | length_lb) + 1; // +1 CHECKSUM byte

		// Max packet length is 2048 bytes. 3 bytes are already read and the checksum byte is not read yet.
		// Can't accept larger packets so if that's the case, the function needs to exit after sending an error packet back to the laptop.
		if (bytes_to_read > 2045)
		{
			send_packet(from_scanner, to_laptop, ok_error, error_length_invalid_value, err, 1);
			return;
		}

		// Let's continue...
		// Calculate the exact size of the payload
		payload_length = bytes_to_read - 3;

		// Save us some headache by not letting this variable sink below zero
		if (payload_length < 0) payload_length = 0;

		// Make some space for the payload bytes
		uint8_t cmd_payload[payload_length];

		// Wait here until all of the expected bytes are received or timeout occurs
		command_timeout_start = millis_get();
		while ( (uart2_available_rx() < bytes_to_read) && !command_timeout_reached ) 
		{
			if (millis_get() - command_timeout_start > command_timeout) command_timeout_reached = true;
		}

		// Check if timeout has been reached, if not then continue processing stuff, cool!
		if (!command_timeout_reached)
		{
			// There's at least one full command in the buffer
			// Go ahead and read one DATA CODE byte.
			datacode = uart2_getc() & 0xFF;

			// Read one SUB-DATA CODE byte.
			subdatacode = uart2_getc() & 0xFF;

			// If the payload length is greater than zero then read those bytes too
			if (payload_length > 0)
			{
				// Read all the PAYLOAD bytes
				for (uint16_t i = 0; i < payload_length; i++)
				{
					cmd_payload[i] = uart2_getc() & 0xFF;
				}
				// And set flag so the rest of the code knows.
				payload_bytes = true;
			}
			// Set flag if there are no PAYLOAD bytes available.
			else payload_bytes = false;

			// Read last CHECKSUM byte.
			checksum = uart2_getc() & 0xFF;

			// Verify the received command packet by calculating what the checksum byte should be.
			calculated_checksum = 0;
			calculated_checksum = length_hb + length_lb + datacode + subdatacode;

			if (payload_bytes)
			{
				for (uint16_t j = 0; j < payload_length; j++)
				{
					calculated_checksum += cmd_payload[j];
				}
			}

			// Keep the low byte of the result
			calculated_checksum = calculated_checksum & 0xFF;

			// Compare calculated checksum to the received CHECKSUM byte
			// and ensure the first byte is the correct SYNC byte.
			// Here it goes...
			if ( (sync == SYNC_BYTE) && (calculated_checksum  == checksum) )
			{
				// If everything is good then continue processing the packet...
				// Find out what is the source and the target of the packet by examining the DATA CODE byte's high nibble (upper 4 bits)
				uint8_t source = (datacode >> 6) & 0x03; // keep the upper two bits
				uint8_t target = (datacode >> 4) & 0x03; // keep the lower two bits

				// Extract DC command value from the low nibble (lower 4 bits)
				uint8_t dc_command = datacode & 0x0F;

				if (source == from_laptop) // 0x00 - Proceed only if the packet is coming from the laptop.
				{
					switch (target) // Evaluate target value.
					{
						case to_laptop: // 0x00 - If the target is the laptop itself then simply return the received packet as is
						{
							if (payload_bytes) send_packet(from_laptop, to_laptop, datacode, subdatacode, cmd_payload, payload_length);
							else send_packet(from_laptop, to_laptop, datacode, subdatacode, (uint8_t*)ok, 0); // this is the only time something is returned without payload
							break;
						}
						case to_scanner: // 0x01 - Scanner is the target.
						{
							switch (dc_command) // Evaluate DC command
							{
								case reboot: // 0x00 - Reboot scanner.
								{
									reset_diagnostic_comms();

									// Send REBOOT packet back first to acknowledge the scanner has received the command
									send_packet(from_scanner, to_laptop, reboot, ok, ack, 1);

									// Wait for 200 milliseconds to finish all operations
									_delay_ms(200);

									// Then call the reboot function which activates the watchdog timer and waits for the reboot to happen
									wdt_init();
									avr8_software_reset();
									break;
								}
								case handshake: // 0x01 - Handshake request.
								{
									reset_diagnostic_comms();

									// The handshake was previously read in the main() before the loop
									send_packet(from_scanner, to_laptop, handshake, ok, handshake_array, 21);
									break;
								}
								case status: // 0x02 - Scanner status report request
								{
									// Gather status data and send it back... not yet implemented
									send_packet(from_scanner, to_laptop, status, ok, ack, 1); // the payload should contain all information but now it's just an ACK byte (0x00)
									break;
								}
								case settings: // 0x03 - Change scanner settings
								{
									switch (subdatacode) // Evaluate sub-data code byte
									{
										case 0x00:
										{
											// Sub-data code missing, not enough information is given
											send_packet(from_scanner, to_laptop, settings, error_subdatacode_not_enough_info, err, 1);
											break;
										}
										case read_settings: // 0x01 - Read scanner settings directly from EEPROM
										{
											send_packet(from_scanner, to_laptop, settings, read_settings, ack, 1); // acknowledge
											interval = 50; // dummy variable
											break;
										}
										case write_settings: // 0x02 - Write scanner settings directly to EEPROM (values in PAYLOAD) (warning!)
										{
											send_packet(from_scanner, to_laptop, settings, write_settings, ack, 1); // acknowledge with a zero byte in payload
											interval = 500; // dummy variable
											break;
										}
										case enable_ccd_bus: // 0x03 - Enable CCD-bus communication
										{
											init_ccd_bus();
											send_packet(from_scanner, to_laptop, settings, enable_ccd_bus, ack, 1); // acknowledge with a zero byte in payload
											break;
										}
										case disable_ccd_bus: // 0x04 - Disable CCD-bus communication
										{
											ccd_enabled = false;
											stop_clock_generator();

											send_packet(from_scanner, to_laptop, settings, disable_ccd_bus, ack, 1); // acknowledge with a zero byte in payload
											break;
										}
										case enable_sci_bus: // 0x05 - Enable SCI-bus communication
										{
											init_sci_bus();
											select_sci_bus_target(cmd_payload[0]); // payload contains which module to connect by default: 0x01 if PCM, 0x02 if TCM
											send_packet(from_scanner, to_laptop, settings, enable_sci_bus, cmd_payload, 1); // acknowledge with 1 byte payload, that is the selected bus
											break;
										}
										case disable_sci_bus: // 0x06 - Disable SCI-bus communication
										{
											sci_enabled = false;
											select_sci_bus_target(NON);
											send_packet(from_scanner, to_laptop, settings, disable_sci_bus, cmd_payload, 1); // acknowledge with 1 byte payload, that is none of the buses
											break;
										}
										case enable_lcd_bl: // 0x07 - Enable LCD backlight (100%)
										{
											send_packet(from_scanner, to_laptop, settings, enable_lcd_bl, ack, 1);
											break;
										}
										case enable_lcd_bl_pwm: // 0x08 - Enable LCD backlight dimming (0-100% PWM value in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, enable_lcd_bl_pwm, ack, 1);
											break;
										}
										case disable_lcd_bl: // 0x09 - Disable LCD backlight (0%)
										{
											send_packet(from_scanner, to_laptop, settings, disable_lcd_bl, ack, 1);
											break;
										}
										case metric_units: // 0x0A - Use metric units
										{
											send_packet(from_scanner, to_laptop, settings, metric_units, ack, 1);
											break;
										}
										case imperial_units: // 0x0B - Use imperial units
										{
											send_packet(from_scanner, to_laptop, settings, imperial_units, ack, 1);
											break;
										}
										case ext_eeprom_wp_on: // 0x0C - Enable external EEPROM write protection (read only)
										{
											send_packet(from_scanner, to_laptop, settings, ext_eeprom_wp_on, ack, 1);
											break;
										}
										case ext_eeprom_wp_off: // 0x0D - Disable external EEPROM write protection(read/write)
										{
											send_packet(from_scanner, to_laptop, settings, ext_eeprom_wp_off, ack, 1);
											break;
										}
										case set_ccd_interframe_response: // 0x0E - Set CCD-bus interframe response delay (ms in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_ccd_interframe_response, ack, 1);
											break;
										}
										case set_sci_interframe_response: // 0x0F - Set SCI-bus interframe response delay (ms in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_sci_interframe_response, ack, 1);
											break;
										}
										case set_sci_intermsg_response: // 0x10 - Set SCI-bus intermessage response delay (ms in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_sci_intermsg_response, ack, 1);
											break;
										}
										case set_sci_intermsg_request: // 0x11 - Set SCI-bus intermessage request delay (ms in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_sci_intermsg_request, ack, 1);
											break;
										}
										case set_packet_intermsg_response: // 0x12 - Set PACKET intermessage response delay (ms in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_packet_intermsg_response, ack, 1);
											break;
										}
										case enable_sci_bus_high_speed: // 0x13 - Enable SCI-bus high speed mode (62500 baud)
										{
											while (uart1_available() > 0) uart1_getc(); // clear buffer before entering high speed mode
											uart1_init(SCI_HI_SPEED); // configure UART1 to 62500 baud speed (62500 kbps high speed mode)
											sci_bus_high_speed = true; // set flag
											send_packet(from_scanner, to_laptop, settings, enable_sci_bus_high_speed, ack, 1); // acknowledge
											break;
										}
										case disable_sci_bus_high_speed: // 0x14 - Disable SCI-bus high speed mode (7812.5 baud)
										{
											while (uart1_available() > 0) uart1_getc(); // clear buffer before exiting high speed mode
											uart1_init(SCI_CCD_LO_SPEED); // configure UART1 to 7812.5 baud speed (7812.5 kbps default SCI-bus speed)
											sci_bus_high_speed = false; // set flag
											send_packet(from_scanner, to_laptop, settings, disable_sci_bus_high_speed, ack, 1); // acknowledge
											break;
										}
										case enable_ccd_bus_filtering: // 0x15 - Enable CCD-bus message filtering (ID bytes in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, enable_ccd_bus_filtering, ack, 1);
											break;
										}
										case disable_ccd_bus_filtering: // 0x16 - Disable CCD-bus message filtering
										{
											send_packet(from_scanner, to_laptop, settings, disable_ccd_bus_filtering, ack, 1);
											break;
										}
										case enable_sci_bus_filtering: // 0x17 - Enable SCI-bus message filtering (ID bytes in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, enable_sci_bus_filtering, ack, 1);
											break;
										}
										case disable_sci_bus_filtering: // 0x18 - Disable SCI-bus message filtering
										{
											send_packet(from_scanner, to_laptop, settings, disable_sci_bus_filtering, ack, 1);
											break;
										}
										case set_sci_bus_target: // 0x19 - Set SCI-bus target (PCM, TCM, none in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, settings, set_sci_bus_target, ack, 1);
											break;
										}
										case enable_buzzer: // 0x1A - Enable buzzer
										{
											send_packet(from_scanner, to_laptop, settings, enable_buzzer, ack, 1);
											break;
										}
										case disable_buzzer: // 0x1B - Disable buzzer
										{
											send_packet(from_scanner, to_laptop, settings, disable_buzzer, ack, 1);
											break;
										}
										case enable_button_hold: // 0x1C - Enable button hold down sensing
										{
											send_packet(from_scanner, to_laptop, settings, enable_button_hold, ack, 1);
											break;
										}
										case disable_button_hold: // 0x1D - Disable button hold down sensing
										{
											send_packet(from_scanner, to_laptop, settings, disable_button_hold, ack, 1);
											break;
										}
										case enable_act_led: // 0x1E - Enable ACT LED (action, blue color)
										{
											send_packet(from_scanner, to_laptop, settings, enable_act_led, ack, 1);
											break;
										}
										case disable_act_led: // 0x1F - Disable ACT LED (action, blue color)
										{
											send_packet(from_scanner, to_laptop, settings, disable_act_led, ack, 1);
											break;
										}
										default: // These values are not used
										{
											send_packet(from_scanner, to_laptop, settings, error_subdatacode_invalid_value, err, 1);
											break;
										}
									}
									break;
								}
								case request: // 0x04 - General request from the scanner
								{
									switch (subdatacode)  // Evaluate sub-data code byte
									{
										case 0x00:
										{
											// Sub-data code missing, not enough information is given
											send_packet(from_scanner, to_laptop, request, error_subdatacode_not_enough_info, err, 1);
											break;
										}
										case scanner_firmware_version: // 0x01 - Scanner firmware version
										{
											send_packet(from_scanner, to_laptop, request, scanner_firmware_version, ack, 1);
											break;
										}
										case read_int_eeprom: // 0x02 - Read internal EEPROM in chunks (size in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, request, read_int_eeprom, ack, 1);
											break;
										}
										case read_ext_eeprom: // 0x03 - Read external EEPROM in chunks (size in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, request, read_ext_eeprom, ack, 1);
											break;
										}
										case write_int_eeprom: // 0x04 - Write internal EEPROM in chunks (value(s) in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, request, write_int_eeprom, ack, 1);
											break;
										}
										case write_ext_eeprom: // 0x05 - Write external EEPROM in chunks (value(s) in PAYLOAD)
										{
											send_packet(from_scanner, to_laptop, request, write_ext_eeprom, ack, 1);
											break;
										}
										case scan_ccd_bus_modules: // 0x06 - Scan CCD-bus modules
										{
											scan_ccd = true;

											if (payload_length == 2) // start-end addresses in payload, default timeout (200 ms)
											{
												ccd_scan_start_addr = cmd_payload[0];
												ccd_scan_end_addr = cmd_payload[1];
												ccd_scan_request_timeout = 200;
											}
											else if (payload_length == 4) // start-end addresses and timeout in payload
											{
												ccd_scan_start_addr = cmd_payload[0];
												ccd_scan_end_addr = cmd_payload[1];
												ccd_scan_request_timeout = (cmd_payload[2] << 8) | cmd_payload[3];
											}
											else // no payload, default settings
											{
												ccd_scan_start_addr = 0x00;
												ccd_scan_end_addr = 0xFE;
												ccd_scan_request_timeout = 200;
											}

											send_packet(from_scanner, to_laptop, request, scan_ccd_bus_modules, ack, 1);
											break;
										}
										case free_ram_value: // 0x07 - Free RAM available
										{
											// Read the actual free ram value into this local variable (it exists only in this "case")
											uint16_t free_ram_available = free_ram();

											// Create a local array of two bytes
											uint8_t free_ram_array[2];

											// Separate the first 16-bit value (word) into two 8-bit values (bytes)
											free_ram_array[0] = (free_ram_available >> 8) & 0xFF;
											free_ram_array[1] = free_ram_available & 0xFF;

											// Send the packet back to the laptop
											send_packet(from_scanner, to_laptop, response, free_ram_value, free_ram_array, 2);
											break;
										}
										case mcu_counter_value: // 0x08 - MCU counter value (milliseconds elapsed)
										{
											// Read the actual millis value (how many milliseconds passed since start)
											uint32_t mcu_millis = millis_get();

											// Create a local array of four bytes
											uint8_t mcu_millis_array[4];

											// Separate the first 32-bit value (qword) into four 8-bit values (bytes)
											mcu_millis_array[0] = (mcu_millis >> 24) & 0xFF;
											mcu_millis_array[1] = (mcu_millis >> 16) & 0xFF;
											mcu_millis_array[2] = (mcu_millis >> 8) & 0xFF;
											mcu_millis_array[3] = mcu_millis & 0xFF;

											// Send the packet back to the laptop
											send_packet(from_scanner, to_laptop, response, mcu_counter_value, mcu_millis_array, 4);
											break;
										}
										case 0xFB: // debug: high speed sci-bus memory area dump, WARNING: this is not loop-safe, the program will freeze until this "case" is done
										{
											// prepare result array
											uint8_t result[256];
											bool sci_addr_accepted = false;

											// make sure the address selector command is accepted by the PCM
											// it has to echo back the address byte ($F2, $F3, $F4...) that is the first payload byte
											while (!sci_addr_accepted)
											{
												// Save the current number of bytes in the receive buffer (most likely 0)
												uint8_t numbytes = uart1_available();

												// Put the address selector byte to the SCI-bus (first byte of payload section)
												uart1_putc(cmd_payload[0]);

												// Wait for answer or timeout
												bool timeout_reached = false;
												uint32_t timeout_start = millis_get();
												while ((numbytes >= uart1_available()) && !timeout_reached)
												{
													// Check the timeout condition only, the received byte is stored automatically in the ringbuffer
													if (millis_get() - timeout_start > SCI_INTERMESSAGE_RESPONSE_DELAY) timeout_reached = true;
												}

												if (timeout_reached)
												{
													timeout_reached = false; // re-arm, don't care if true or false
													break; // exit from this "case" if the SCI-bus won't respond within 3 seconds
												}

												uint8_t dummy2 = uart1_getc() & 0xFF; // get the echo from the SCI-bus
												if (dummy2 == cmd_payload[0]) sci_addr_accepted = true; // exit this loop if the proper byte has been received
											}

											// now cycle through every possible address one time only!
											// skip the last 16 bytes (they are most likely area selector bytes)
											for (uint8_t i = 0; i < 0xF0; i++)
											{
												// Save the current number of bytes in the receive buffer (most likely 0)
												uint8_t numbytes = uart1_available();

												// Put one/next byte to the SCI-bus
												uart1_putc(i);

												// Wait for answer or timeout
												bool timeout_reached = false;
												uint32_t timeout_start = millis_get();

												// Unlike CCD-bus, SCI-bus needs a little bit of delay between bytes,
												// so we check here if the PCM/TCM has echoed the byte back.
												while ((numbytes >= uart1_available()) && !timeout_reached)
												{
													// Check the timeout condition only, the received byte is stored automatically in the ringbuffer
													if (millis_get() - timeout_start > SCI_INTERMESSAGE_RESPONSE_DELAY) timeout_reached = true;
												}

												// If the SCI-bus doesn't respond in the given timeframe then save an $FF byte in the array
												if (timeout_reached)
												{
													timeout_reached = false; // re-arm, don't care if true or false
													result[i] = 0xFF;
												}
												else
												{
													result[i] = uart1_getc() & 0xFF;
												}
											}

											// add the last 16 bytes to the result array
											for (uint8_t k = 0; k < 16; k++)
											{
												result[0xF0 + k] = 0xFF;
											}

											// send result back to laptop, note that the subdatacode is the actual SCI-bus memory area address
											send_packet(from_scanner, to_laptop, debug, cmd_payload[0], result, 256);
											break;
										}
										case 0xFC: // enable sci-bus command 14, sensor request
										{
											sci_command_14 = true; 
											send_packet(from_sci_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
											break;
										}
										case 0xFD: // disable sci-bus command 14, sensor request
										{
											sci_command_14 = false; 
											send_packet(from_sci_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
											break;
										}
										case 0xFE: // dummy sensor request
										{
											// Fill the pending buffer with the message to be sent
											for (uint8_t i = 0; i < payload_length; i++)
											{
												sci_bus_msg_to_send[i] = cmd_payload[i];
											}
											sci_bus_msg_to_send_ptr = payload_length;

											// Set flag so the main loop knows there's something to do
											sci_bus_msg_pending = true;

											send_packet(from_sci_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
											break;
										}
										default: // These values are not used
										{
											send_packet(from_scanner, to_laptop, request, error_subdatacode_invalid_value, err, 1);
											break;
										}
									}
									break;
								}
								case self_diag: // 0x0A - Run self-diagnostics
								{
									send_packet(from_scanner, to_laptop, self_diag, ok, ack, 1); // acknowledge
									break;
								}
								case make_backup: // 0x0B - Create scanner settings backup packet (int. EEPROM dump)
								{
									send_packet(from_scanner, to_laptop, make_backup, ok, ack, 1); // acknowledge
									break;
								}
								case restore_backup: // 0x0C - Restore scanner settings from backup
								{
									send_packet(from_scanner, to_laptop, restore_backup, ok, ack, 1); // acknowledge
									break;
								}
								case restore_default: // 0x0D - Restore default scanner settings (factory reset)
								{
									send_packet(from_scanner, to_laptop, restore_default, ok, ack, 1); // acknowledge
									break;
								}
								case debug: // 0x0E - Debug
								{
									// 5 valid packets are hidden in this mess
									uint8_t dummy_packet[168] = 
									{	
										0x33, 0x67, 0x23, 0x64, 0x33, 0x34, 0x77, 0xAA,
										0x33, 0x33, 0x33, 0x33, 0xAA, 0x33, 0x00, 0x17,
										0x41, 0x00, 0x43, 0x48, 0x52, 0x59, 0x53, 0x4C,
										0x45, 0x52, 0x43, 0x43, 0x44, 0x53, 0x43, 0x49,
										0x53, 0x43, 0x41, 0x4E, 0x4E, 0x45, 0x52, 0x77,
										0xBB, 0xAA, 0x00, 0x45, 0x33, 0x00, 0x17, 0x41,
										0x00, 0x43, 0x48, 0x52, 0x59, 0x53, 0x4C, 0x45,
										0x52, 0x43, 0x43, 0x44, 0x53, 0x43, 0x49, 0x53,
										0x43, 0x41, 0x4E, 0x4E, 0x45, 0x52, 0x77, 0x33,
										0x00, 0x17, 0x41, 0x00, 0x43, 0x48, 0x52, 0x59,
										0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44, 0x53,
										0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E, 0x45,
										0x52, 0x77, 0xBB, 0x00, 0x00, 0x45, 0x23, 0x98,
										0x33, 0x00, 0x17, 0x41, 0x00, 0x43, 0x48, 0x52,
										0x59, 0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44,
										0x53, 0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E,
										0x45, 0x52, 0x77, 0x33, 0x66, 0x33, 0x22, 0x00,
										0x33, 0x00, 0x17, 0x41, 0x00, 0x43, 0x48, 0x52,
										0x59, 0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44,
										0x53, 0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E,
										0x45, 0x52, 0x77, 0x88, 0xAA, 0xBB, 0xCC, 0x33
									};
									//33
									//67
									//23
									//64
									//33
									//34
									//77
									//AA
									//33
									//33
									//33
									//33
									//AA
									//33 00 17 41 00 43 48 52 59 53 4C 45 52 43 43 44 53 43 49 53 43 41 4E 4E 45 52 77
									//BB
									//AA
									//00
									//45
									//33 00 17 41 00 43 48 52 59 53 4C 45 52 43 43 44 53 43 49 53 43 41 4E 4E 45 52 77
									//33 00 17 41 00 43 48 52 59 53 4C 45 52 43 43 44 53 43 49 53 43 41 4E 4E 45 52 77
									//BB
									//00
									//00
									//45
									//23
									//98
									//33 00 17 41 00 43 48 52 59 53 4C 45 52 43 43 44 53 43 49 53 43 41 4E 4E 45 52 77
									//33
									//66
									//33
									//22
									//00
									//33 00 17 41 00 43 48 52 59 53 4C 45 52 43 43 44 53 43 49 53 43 41 4E 4E 45 52 77
									//88
									//AA
									//BB
									//CC
									//33

									// Send bytes directly with uart2_putc command (unsafe in most situations)
									for (uint8_t i = 0; i < 167; i++)
									{
										uart2_putc(dummy_packet[i]);
									}

									// Send ack byte back
									send_packet(from_scanner, to_laptop, debug, ok, ack, 1); // acknowledge
									break;
								}
								case ok_error: // 0x0F - OK/ERROR message
								{
									send_packet(from_scanner, to_laptop, ok_error, ok, ack, 1); // acknowledge
									break;
								}
								default: // These values are not used.
								{
									send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_dc_command, err, 1);
									break;
								}
							}
							break;
						}
						case to_ccd_bus: // 0x02 - CCD-bus is the target.
						{
							switch (dc_command) // Evaluate data code low nibble
							{
								case send_msg: // 0x06 - Send message to the CCD-bus
								{
									// Fill the pending buffer with the message to be sent
									for (uint8_t i = 0; i < payload_length; i++)
									{
										ccd_bus_msg_to_send[i] = cmd_payload[i];
									}
									ccd_bus_msg_to_send_ptr = payload_length;

									// Set flag so the main loop knows there's something to do
									ccd_bus_msg_pending = true;

									send_packet(from_ccd_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
									break;
								}
								case send_rep_msg: // 0x07 - Send message(s) repeatedly to the CCD-bus
								{
									send_packet(from_ccd_bus, to_laptop, send_rep_msg, ok, ack, 1); // acknowledge
									break;
								}
								case stop_msg_flow: // 0x08 - Stop message flow to the CCD-bus
								{
									ccd_bus_msg_rep = false;
									ccd_bus_msg_to_send_ptr = 0;

									send_packet(from_ccd_bus, to_laptop, stop_msg_flow, ok, ack, 1); // acknowledge
									break;
								}
								default: // These values are not used.
								{
									send_packet(from_ccd_bus, to_laptop, ok_error, error_datacode_invalid_dc_command, err, 1);
									break;
								}
							}
							break;
						}
						case to_sci_bus: // 0x03 - SCI-bus is the target.
						{
							switch (dc_command) // Evaluate data code low nibble
							{
								case send_msg: // 0x06 - Send message to the SCI-bus
								{
									// Fill the pending buffer with the message to be sent
									for (uint8_t i = 0; i < payload_length; i++)
									{
										sci_bus_msg_to_send[i] = cmd_payload[i];
									}
									sci_bus_msg_to_send_ptr = payload_length;

									// Set flag so the main loop knows there's something to do
									sci_bus_msg_pending = true;

									send_packet(from_sci_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
									break;
								}
								case send_rep_msg: // 0x07 - Send message(s) repeatedly to the SCI-bus
								{
									/**********************************************************************
									Frame format:
									33 00 11 37 F4 03 07 00 02 04 02 02 03 F4 1A F4 1B F4 1C 1D 9D 

									$33: SYNC byte
									$00 $11: LENGTH bytes
									$37: DATA CODE byte (from laptop, to sci-bus, send repeated messages)
									$F4: SUB-DATA CODE byte (sci-bus high speed memory area target)
										$03: length of parameter location and length arrays
										$07: length of parameters array
										$00 $02 $04: 3 location bytes 
										$02 $02 $03: 3 length bytes
										$F4 $1A: first parameter (starts at relative address 0)
										$F4 $1B: second parameter (starts at relative address 2)
										$F4 $1C $1D: third parameter (starts at relative address 4)
									$9D: CHECKSUM byte

									**********************************************************************/
									
									switch (subdatacode)
									{
										case 0xF4: // High speed mode memory area
										{
											// Read payload for parameter location(s) and command(s) 
											if (payload_bytes)
											{
												// The first byte in the payload refers to the length of the parameter locations and length
												if (cmd_payload[0] <= 64)
												{
													// Add these bytes to two separate array with for-loops
													for (uint8_t m = 0; m < cmd_payload[0]; m++)
													{
														mode_f4_parameters_loc[m] = cmd_payload[2 + m]; // First two bytes have to be ignored
													}

													for (uint8_t m = 0; m < cmd_payload[0]; m++)
													{
														mode_f4_parameters_length[m] = cmd_payload[2 + cmd_payload[0] + m];
													}
												}

												// The second byte in the payload refers to the length of the parameter command list
												if (cmd_payload[1] <= 128)
												{
													// Add these bytes to a separate array
													for (uint8_t n = 0; n < cmd_payload[1]; n++)
													{
														mode_f4_parameters[n] = cmd_payload[2 + (2 * cmd_payload[0]) + n]; // First two bytes and the parameter locations and lengths have to be ignored
													}
												}

												mode_f4_ptr_length = cmd_payload[0];
												mode_f4_length = cmd_payload[1];
												mode_f4_ptr = 0;

												// Set flag so the SCI-bus routine begins execution as soon as possible
												sci_hs_mode_f4 = true;
												//send_packet(from_sci_bus, to_laptop, send_rep_msg, ok, ack, 1); // acknowledge

												// Debug: send back these three arrays separately
												send_packet(from_scanner, to_laptop, send_rep_msg, 0x00, mode_f4_parameters_loc, mode_f4_ptr_length);
												send_packet(from_scanner, to_laptop, send_rep_msg, 0x01, mode_f4_parameters_length, mode_f4_ptr_length);
												send_packet(from_scanner, to_laptop, send_rep_msg, 0x02, mode_f4_parameters, mode_f4_length);
											}
											break;
										}
										default:
										{
											break;
										}
									}
									break;
								}
								case stop_msg_flow: // 0x08 - Stop message flow to the SCI-bus
								{
									//sci_bus_msg_rep = false;
									sci_hs_mode_f4 = false;
									mode_f4_ptr_length = 0;
									mode_f4_length = 0;
									mode_f4_ptr = 0;

									sci_bus_msg_to_send_ptr = 0;

									send_packet(from_sci_bus, to_laptop, send_msg, ok, ack, 1); // acknowledge
									break;
								}
								default: // These values are not used.
								{
									send_packet(from_sci_bus, to_laptop, ok_error, error_datacode_invalid_dc_command, err, 1);
									break;
								}
							}
							break;
						}
						default: // These values are not used.
						{
							send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_target, err, 1);
							break;						
						}
					} // switch (target)
				} // if (source == laptop)
				else
				{
					// Packet is coming from an unknown source
					send_packet(from_scanner, to_laptop, ok_error, error_packet_unknown_source, err, 1);
				}
			} // SYNC and CHECKSUM is OK
			else // CHECKSUM byte and/or SYNC byte error! Something is wrong with this packet
			{
				if ( calculated_checksum != checksum )
				{
					send_packet(from_scanner, to_laptop, ok_error, error_checksum_invalid_value, err, 1);
				}

				if ( sync != SYNC_BYTE )
				{
					send_packet(from_scanner, to_laptop, ok_error, error_sync_invalid_value, err, 1);
				}

				// Let's see if there's something left behind
				// Find the next SYNC byte (0x33) until there's enough data in the receive buffer or timeout occurs
				command_timeout_reached = false;
				command_timeout_start = millis_get();
				while ( ((uart2_peek() & 0xFF) != SYNC_BYTE) && (uart2_available_rx() > 0) && (!command_timeout_reached) )
				{
					if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true;
					uart2_getc();
				}
				command_timeout_reached = false;
			}
		} // if (!command_timeout_reached)
		else // If we're here then command read timeout occurred and there are most probably junk data in the buffer
		{
			// Let them know
			send_packet(from_scanner, to_laptop, ok_error, error_packet_timeout_occured, err, 1);

			// Let's see if there's something left behind
			// Find the next SYNC byte (0x33) until there's enough data in the receive buffer or timeout occurs
			command_timeout_reached = false;
			command_timeout_start = millis_get();
			while ( ((uart2_peek() & 0xFF) != SYNC_BYTE) && (uart2_available_rx() > 0) && (!command_timeout_reached) )
			{
				if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true;
				uart2_getc();
			}
			command_timeout_reached = false;
		}								
	}
	else
	{
		// TODO: check here if the minimum number of 3 bytes stayed long enough to consider getting rid of them
	}
}


/*************************************************************************
Function: free_ram()
Purpose:  returns how many bytes exists between the end of the heap and 
          the last allocated memory on the stack, so it is effectively 
          how much the stack/heap can grow before they collide.
**************************************************************************/
uint16_t free_ram(void)
{
    extern int  __bss_end; 
    extern int  *__brkval; 
    uint16_t free_memory; 
    
    if((int)__brkval == 0)
    {
        free_memory = ((int)&free_memory) - ((int)&__bss_end); 
    }
    else 
    {
        free_memory = ((int)&free_memory) - ((int)__brkval); 
    }
    return free_memory; 

}


/*************************************************************************
Function: available_buses()
Purpose:  determines the vehicle's bus-configuration.
Note:     the lowest bit of the returned bytes indicates the CCD-bus
          00000000 - no CCD/SCI-bus present
		  00000001 - CCD-bus present, no SCI-bus present
		  00000010 - SCI-bus present, no CCD-bus present
		  00000011 - CCD/SCI-bus present
**************************************************************************/
uint8_t available_buses(void)
{
	uint32_t timeout_start = 0;
	uint16_t timeout_value = 1000;
	bool timeout_reached = false;
	uint32_t ccd_messages_count_before, ccd_messages_count_after;
	bool ccd_bus_state, sci_bus_state;
	uint8_t ret = 0;
	
	// First determine if there are messages flowing on the CCD-bus
	// Save the current state of the CCD-bus (enabled / disabled)
	ccd_bus_state = ccd_enabled;

	// If the CCD-bus was previously turned off then turn it back on
	if (!ccd_enabled) ccd_enabled = true;

	// Do some measurements
	// Save the current number of full CCD-messages received
	ATOMIC_BLOCK(ATOMIC_RESTORESTATE)
	{
		ccd_messages_count_before = ccd_messages_count;
	}

	// Save the time when the timeout started
	timeout_start = millis_get();
	while (!timeout_reached)
	{
		// Wait here for the timeout to occur
		if (millis_get() - timeout_start > timeout_value) timeout_reached = true;
	}
	timeout_reached = false; // Re-arm timeout detection for the next time

	// Save the current number of full CCD-messages received again
	ATOMIC_BLOCK(ATOMIC_RESTORESTATE)
	{
		ccd_messages_count_after = ccd_messages_count;
	}
	
	// If the after value is bigger then the CCD-bus is active
	if (ccd_messages_count_after - ccd_messages_count_before > 0)
	{
		sbi(ret, 0);
	}
	else
	{
		cbi(ret, 0);
	}

	// Restore the saved state of the CCD-bus (enabled / disabled)
	ccd_enabled = ccd_bus_state;


	// Now do the same with the SCI-bus
	sci_bus_state = sci_enabled;
	if (!sci_enabled) sci_enabled = true;
	select_sci_bus_target(PCM); // Select PCM by default

	// Clear SCI-bus UART buffer
	while (uart1_available() > 0) uart1_getc();

	// Request PCM DTC as a bus test
	uart1_putc(0x10);

	timeout_start = millis_get();
	while (!timeout_reached)
	{
		// Wait here for the timeout to occur
		if (millis_get() - timeout_start > timeout_value) timeout_reached = true;
	}
	timeout_reached = false; // Re-arm timeout detection for the next time

	if (uart1_available() > 1) // The command is echoed back so the received message should be greater than 1 byte
	{
		sbi(ret, 1);
		while (uart1_available() > 0) uart1_getc(); // Erase the buffer
	}
	else
	{
		cbi(ret, 1);
	}

	// Restore the saved state of the CCD-bus (enabled / disabled)
	sci_enabled = sci_bus_state;

	return ret;
}


/*************************************************************************
Function: scan_ccd_bus_mods()
Purpose:  discovers all the modules on the CCD-bus.
**************************************************************************/
void scan_ccd_bus_mods(void)
{

}


/*************************************************************************
Function: init_ccd_bus()
Purpose:  enables CCD-bus communication and clears buffers
**************************************************************************/
void init_ccd_bus()
{
	// Make sure the CCD-buffers are empty
	while (uart0_available() > 0) uart0_getc();

	// Reset pending message buffer too
	ccd_bus_bytes_buffer_ptr = 0;
	ccd_bus_msg_to_send_ptr = 0;
	ccd_bus_msg_pending = false;

	// 7812.5 kbps default (and only) CCD-bus speed
	uart0_init(SCI_CCD_LO_SPEED);

	// Start 1 MHz clock generator
	start_clock_generator();

	// Set flag
	ccd_enabled = true;
}


/*************************************************************************
Function: init_sci_bus()
Purpose:  enables SCI-bus communication and clears buffers
**************************************************************************/
void init_sci_bus()
{
	// Make sure the SCI-buffers are empty
	while (uart1_available() > 0) uart1_getc();

	// Reset pending message buffer too
	sci_bus_bytes_buffer_ptr = 0;
	sci_bus_msg_pending = false;
	sci_bus_msg_to_send_ptr = 0;

	// 7812.5 kbps default SCI-bus speed
	uart1_init(SCI_CCD_LO_SPEED);

	// Set flag
	sci_enabled = true;
}


/*************************************************************************
Function: reset_diagnostic_comms()
Purpose:  resets diagnostic communication as if the scanner was restarted
**************************************************************************/
void reset_diagnostic_comms()
{
	// Reset bus states to avoid previous communication interference
	ccd_enabled = false;
	sci_enabled = false;
	
	//uart0_stop(); // These don't do what they're told...
	//uart1_stop();
	while (uart0_available() > 0) uart0_getc(); // Clear CCD-bus buffer
	while (uart1_available() > 0) uart1_getc(); // Clear SCI-bus buffer

	// Stop clock generator so the CCD-bus interpreter chip stops working
	stop_clock_generator();

	// SCI-bus target is set to none
	select_sci_bus_target(NON);

	// Reset message buffers (CCD/SCI-bus)
	ccd_bus_bytes_buffer_ptr = 0;
	ccd_bus_msg_pending = false;
	ccd_bus_msg_to_send_ptr = 0;

	sci_bus_bytes_buffer_ptr = 0;
	sci_bus_msg_pending = false;
	sci_bus_msg_to_send_ptr = 0;
}


/*************************************************************************
Function: handle_ccd_data()
Purpose:  handles CCD-bus messages
**************************************************************************/
void handle_ccd_data(void)
{
	// #1 - Collecting bytes from the CCD-bus:
	// Check if there are any data bytes in the CCD-ringbuffer
	if (uart0_available() > 0)
	{
		// Peek one byte (don't remove it from the buffer yet)
		uint16_t dummy_read = uart0_peek();

		// #2 - Detecting end of message condition
		// If it's an ID-byte then send the previous bytes to the laptop
		if ((((dummy_read >> 8) & 0xFF) == CCD_SOM) && (ccd_bus_bytes_buffer_ptr > 0))
		{
			// But before that we decide if a CCD-bus scan request has received a valid response
			if (scan_ccd)
			{
				// Make sure that the first byte is a diagnostic response byte (0xF2) and the second byte is the current address offset
				if ((ccd_bus_bytes_buffer[0] == DIAG_RESP) && (ccd_bus_bytes_buffer[1] == ccd_scan_start_addr))
				{
					// Save this address in the array at the next available position
					ccd_mod_addr[ccd_mod_addr_ptr] = ccd_scan_start_addr;
					ccd_mod_addr_ptr++; // increment next available position


					if (ccd_scan_start_addr != ccd_scan_end_addr)
					{
						// Prepare the next
						ccd_scan_start_addr++;   // increment global address by one
						next_ccd_addr = true; // allow next address to be requested
					}
					else
					{
						next_ccd_addr = false;
						scan_ccd_done = true;
					}
				}
				else
				{
					// Check here if an address has been waiting too long (timeout)
					if ((millis_get() - last_ccd_scan_request) > ccd_scan_request_timeout)
					{
						if (ccd_scan_start_addr != ccd_scan_end_addr)
						{
							// Manually increment address
							ccd_scan_start_addr++;   // increment global address by one
							next_ccd_addr = true; // allow next address to be requested
						}
						else
						{
							next_ccd_addr = false;
							scan_ccd_done = true;
						}						
					}
				}
			}
			
			// Send CCD-bus message back to the laptop
			//              where        to         what      ok/error flag     buffer          length
			send_packet(from_ccd_bus, to_laptop, receive_msg, ok, ccd_bus_bytes_buffer, ccd_bus_bytes_buffer_ptr);
			//process_ccd_msg();
			ccd_bus_bytes_buffer_ptr = 0; // reset pointer

			// And save this byte as the first one in the buffer
			ccd_bus_bytes_buffer[ccd_bus_bytes_buffer_ptr] = uart0_getc() & 0xFF;
			ccd_bus_bytes_buffer_ptr++;
		}
		else // get this byte and save to the temporary buffer in the next available position
		{
			ccd_bus_bytes_buffer[ccd_bus_bytes_buffer_ptr] = uart0_getc() & 0xFF;
			ccd_bus_bytes_buffer_ptr++; // increment position by one
		}
	}

	// #3 - Sending bytes to the CCD-bus:
	// If there's a message to be sent to the CCD-bus and the bus happens to be idling then send it here and now
	if (ccd_bus_msg_pending && ccd_idle)
	{
		for (uint8_t i = 0; i < ccd_bus_msg_to_send_ptr; i++)
		{
			uart0_putc(ccd_bus_msg_to_send[i]);
			// There's no need to delay between bytes because of the UART hardware's 
			// inherent delay while loading a byte into its own transmit buffer.
		}

		ccd_bus_msg_to_send_ptr = 0; // reset pointer
		ccd_bus_msg_pending = false; // re-arm
	}

	// #4 - Misc functions regarding CCD-bus communication:
	// Scan CCD-bus modules
	// Obsolete... you can just send B2 FF 00 00 00 B1 to reset all modules on the CCD-bus 
	// and force them to send their address to the bus.
	if (scan_ccd && next_ccd_addr && ccd_idle)
	{
		next_ccd_addr = false; // don't execute this again unless a valid response has been received or timeout occurs
		
		// Create new request message
		ccd_scan_msg[0] = DIAG_REQ;				// diagnostic request (0xB2)
		ccd_scan_msg[1] = ccd_scan_start_addr;	// module address (0-255)
		ccd_scan_msg[2] = 0x00;					// reset command
		ccd_scan_msg[3] = 0x00;					// parameter 1
		ccd_scan_msg[4] = 0x00;					// parameter 2
		ccd_scan_msg[5] = (DIAG_REQ + ccd_scan_start_addr) & 0xFF;	// checksum

		// Send it to the CCD-bus
		for (uint8_t i = 0; i < 6; i++)
		{
			uart0_putc(ccd_scan_msg[i]);
		}

		// Save the time
		last_ccd_scan_request = millis_get();
	}

	if (scan_ccd_done)
	{
		// Send the address list here back to the laptop
		scan_ccd_done = false; // re-arm
		scan_ccd = false; // re-arm
		send_packet(from_scanner, to_laptop, response, scan_ccd_bus_modules, ccd_mod_addr, 32);
		ccd_mod_addr_ptr = 0;
	}
}


/*************************************************************************
Function: handle_sci_data()
Purpose:  handles SCI-bus messages
**************************************************************************/
void handle_sci_data(void)
{
	// #1 - Collecting bytes from the SCI-bus:
	// Check if there are any data bytes in the SCI-ringbuffer
	Here: // goto label
	if (uart1_available() > 0)
	{
		// Check how much bytes do we have
		uint8_t sci_bytes_available = uart1_available();

		// Save all of them
		for (uint8_t i = 0; i < sci_bytes_available; i++)
		{
			sci_bus_bytes_buffer[sci_bus_bytes_buffer_ptr] = uart1_getc() & 0xFF;
			sci_bus_bytes_buffer_ptr++;
		}

		// Save the current time so we can measure if the next byte is the start of a new message (timeout)
		last_sci_byte_received = millis_get();
		sci_bus_active_bytes = true; // set this flag for now and wait
	}

	// #2 - Detecting end of message condition with timeout
	// If there's no data coming in after a while then consider the previous message (if there is) complete
	// Alternatively send message if length is 16 or greater
	if ((millis_get() - last_sci_byte_received > SCI_INTERMESSAGE_RESPONSE_DELAY) || sci_bus_bytes_buffer_ptr > 15)
	{
		// Make sure there's data in the buffer before sending empty packets...
		if (sci_bus_bytes_buffer_ptr > 0)
		{
			// This packet has to be labeled specifically to show which parameter's response is in there (in high speed mode the pcm doesn't echo back the ram-address)
			if (sci_hs_mode_f4)
			{
				// construct a custom local array that contains the source parameter and the response
				// length of the original request
				uint8_t temp9 = mode_f4_parameters_length[mode_f4_ptr];

				// final packet array should be double of the original size of the request
				uint8_t temp10 = 2 * temp9;

				// allocate local array (temp11) with the calculated size (temp10)
				uint8_t temp11[temp10];

				// get the current parameter request location inside the parameters array
				uint8_t temp12 = mode_f4_parameters_loc[mode_f4_ptr];

				// put the original request at the first half of the temporary array
				for (uint8_t k = 0; k < mode_f4_parameters_length[mode_f4_ptr]; k++)
				{
					// copy all the bytes from the parameters array
					temp11[k] = mode_f4_parameters[k + temp12];
				}

				// now copy response data from the sci_bus_bytes_buffer at the second half of the temporary array
				for (uint8_t p = 0; p < sci_bus_bytes_buffer_ptr; p++)
				{
					temp11[temp9 + p] = sci_bus_bytes_buffer[p];
				}
				
				send_packet(from_sci_bus, to_laptop, send_rep_msg, 0xF4, temp11, temp10);
				sci_bus_bytes_buffer_ptr = 0; // reset sci-bus receive buffer pointer

				if (mode_f4_ptr != mode_f4_ptr_length - 1)
				{
					mode_f4_ptr++; // increment location and length pointer at the same time
				}
				else
				{
					mode_f4_ptr = 0; // at the end reset the pointer to zero and loop again
				}
			}
			else // normal mode here (everything is echoed back + data if available, so we know in one go what this message is all about)
			{
				send_packet(from_sci_bus, to_laptop, receive_msg, ok, sci_bus_bytes_buffer, sci_bus_bytes_buffer_ptr);
				//process_sci_msg();
				sci_bus_bytes_buffer_ptr = 0;
			}
		}
		sci_bus_active_bytes = false; // clear active bytes flag here so we can move on with a new message or something else
	}

	// #3 - Sending bytes to the SCI-bus:
	// Check if there's activity on the SCI-bus before sending!
	if (sci_bus_msg_pending && !sci_command_14 && !sci_hs_mode_f4 && !sci_bus_active_bytes)
	{
		// Send every byte with a for-loop and wait for echo and data
		for (uint8_t i = 0; i < sci_bus_msg_to_send_ptr; i++)
		{
			// Save the current number of bytes in the receive buffer (most likely 0)
			uint8_t numbytes = uart1_available();

			// Put one/next byte to the SCI-bus
			uart1_putc(sci_bus_msg_to_send[i]);

			// Wait for answer or timeout
			bool timeout_reached = false;
			uint32_t timeout_start = millis_get();

			// Unlike CCD-bus, SCI-bus needs a little bit of delay between bytes,
			// so we check here if the PCM/TCM has echoed the byte back.
			while ((numbytes >= uart1_available()) && !timeout_reached)
			{
				// Check the timeout condition only, the received byte is stored automatically in the ringbuffer
				if (millis_get() - timeout_start > SCI_INTERMESSAGE_RESPONSE_DELAY) timeout_reached = true;
			}
			timeout_reached = false; // re-arm, don't care if true or false
		}

		sci_bus_msg_pending = false; // re-arm pending msg

		last_sci_byte_received = millis_get(); // save the last time a byte was received
		sci_bus_active_bytes = true; // data is being transferred to the scanner...
		goto Here; // force data processing one time only
	}

	// #4 - Get low speed sensor data from SCI-bus
	// Enter only if there's no active byte on the bus and there's no message to be sent
	if (sci_command_14 && !sci_bus_active_bytes && !sci_bus_msg_pending && !sci_bus_high_speed)
	{
		// Enter for-loop, cycle for every byte to be sent
		for (uint8_t i = 0; i < 1; i++)
		{
			// Save the current number of bytes in the receive buffer
			uint8_t numbytes = uart1_available();

			// Put one/next byte to the SCI-bus
			uart1_putc(sci_command_14_buffer[i]); // pointer is 0 by default

			// Wait for answer or timeout
			bool timeout_reached = false;
			uint32_t timeout_start = millis_get();

			// Unlike CCD-bus, SCI-bus needs a little bit of delay between bytes,
			// so we check here if the PCM/TCM has echoed the byte back.
			// We can't just bang the SCI-bus with bytes without checking the echo.
			// TODO: there's no echo while in high speed mode!!!
			while ((numbytes >= uart1_available()) && !timeout_reached)
			{
				// Check the timeout condition only, the received byte is stored automatically in the ringbuffer
				if (millis_get() - timeout_start > SCI_INTERMESSAGE_RESPONSE_DELAY) timeout_reached = true;
			}
			timeout_reached = false; // re-arm, don't care if true or false
		}

		// Keep increasing the command parameter until its value is 254
		if (sci_command_14_buffer[1] != 0xFF)
		{
			sci_command_14_buffer[1]++;	// increase the command parameter (finally to 255)
			last_sci_byte_received = millis_get(); // save the last time a byte was received
			sci_bus_active_bytes = true; // data is being transferred to the scanner...
			goto Here; // force data processing one time only
		}
		else // if the command parameter reaches 255 then reset it to zero.
		{
			//sci_command_14 = false;
			sci_command_14_buffer[1] = 0x00; // reset command parameter to zero and start again...
		}
	}

	// #5 - Get high speed data from SCI-bus
	// Mode F4
	// Conditions: bus in high speed mode, mode f4 flag, no active bytes on the bus, no messages pending in the transmit buffer
	if (sci_bus_high_speed && sci_hs_mode_f4 && !sci_bus_active_bytes && !sci_bus_msg_pending)
	{
		// Send every byte with a for-loop and wait for echo and data
		for (uint8_t i = 0; i < mode_f4_parameters_length[mode_f4_ptr]; i++)
		{
			// Save the current number of bytes in the receive buffer (most likely 0)
			uint8_t numbytes = uart1_available();

			// Current location lookup routine (start address)
			uint8_t temp8 = mode_f4_parameters_loc[mode_f4_ptr];

			// Put one/next byte to the SCI-bus // array inside array... arrayception
			uart1_putc(mode_f4_parameters[temp8 + i]);

			// Wait for answer or timeout
			bool timeout_reached = false;
			uint32_t timeout_start = millis_get();

			// Unlike CCD-bus, SCI-bus needs a little bit of delay between bytes,
			// so we check here if the PCM/TCM has echoed the byte back.
			while ((numbytes >= uart1_available()) && !timeout_reached)
			{
				// Check the timeout condition only, the received byte is stored automatically in the ringbuffer
				if (millis_get() - timeout_start > SCI_INTERMESSAGE_RESPONSE_DELAY) timeout_reached = true;
			}
			timeout_reached = false; // re-arm, don't care if true or false
		}
		
		last_sci_byte_received = millis_get(); // save the last time a byte was received
		sci_bus_active_bytes = true; // data is being transferred to the scanner...
		goto Here; // force data processing one time only
	}
}


/*************************************************************************
Function: select_sci_bus()
Purpose:  PCM and TCM share the same SCI-bus, different RX-lines, but same
          TX-line. This function decides which module to talk to.
		  There's an electronic switch on the PCB to do this (CD4066N).
Params:   bus number (see switch statement)
Returns:  none
Note:     PD4 pin (SCI_BUS_PCM_PIN) selects the PCM (Powertrain Control Module),
          PD5 pin (SCI_BUS_TCM_PIN) selects the TCM (Transmission Control Module).
		  They can't be HIGH at the same time.
**************************************************************************/
void select_sci_bus_target(uint8_t bus)
{
    sbi(DDRD, DDD4); // Set PD4 pin to output
    sbi(DDRD, DDD5); // Set PD5 pin to output

	switch (bus)
	{
		case 0x00: // No SCI-module selected
		{
			select_sci_non();
			break;
		}
		case 0x01: // PCM selected
		{
			// First disable all SCI-bus connection
			select_sci_non();
			
			// Then set the appropriate pin
			select_sci_pcm();
			break;
		}
		case 0x02: // TCM selected
		{
			// First disable all SCI-bus connection
			select_sci_non();
			
			// Then set the appropriate pin
			select_sci_tcm();
			break;
		}
		default: // PCM is selected if error
		{
			// First disable all SCI-bus connection
			select_sci_non();
			
			// Then set the appropriate pin
			select_sci_pcm();
			break;
		}
	}
}


void select_sci_pcm(void)
{
	sbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 HIGH
	cbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 LOW
}


void select_sci_tcm(void)
{
	cbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 LOW
	sbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 HIGH
}


void select_sci_non(void)
{
	cbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 LOW
	cbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 LOW
}


void init_interrupt()
{
	EICRB |= (1 << ISC41) | (1 << ISC51);
	EIMSK |= (1 << INT4) | (1 << INT5);

	sei(); // Enable interrupts in SREG;
}


// Interrupt Service Routines (ISRs)
// CCD-bus idle detector on INT4 pin
ISR(INT4_vect)
{
	// Set flag so the next received byte can be identified as SOM (Start of Message byte)
	// This flag must be cleared as soon as possible
	ccd_idle = true;
}


// CCD-bus active byte detector on INT5 pin (not in use... yet)
ISR(INT5_vect)
{
	// Set flag
	ccd_active_byte = true;
}


/******************************************************************************
                                     MAIN                                      
******************************************************************************/

int main(void)
{
	// ------------------------------------------------------
	// This would be the setup() function of the Arduino IDE.
	// ------------------------------------------------------

	// Disable global interrupts until setup is complete
	cli();

	// Enable timer that counts elapsed time in milliseconds (Arduino style).
	millis_init();

	// Init I2C-bus
	i2c_init();

	// Enable external EEPROM memories.
	// I2C is enabled here for the first time.
	//exteeprom_init(kbits_512, 2, 128, 0x50);
	//exteeprom_begin(twiClock400kHz);
	//exteeprom_init(&eep, 0);

	// Enable buttons.
	//mcp23017_init(&mcp, 0);

	// Configure I/O ports.
	sbi(DDRE, DDE3);						// PORTE D3 output
	sbi(DDRD, DDD6);						// PORTD D6 output
	sbi(DDRD, DDD7);						// PORTD D7 output
	sbi(LCD_BGLIGHT_PORT, LCD_BGLIGHT_PIN);	// Set LCD backlight (ON)
	cbi(BUZZER_PORT, BUZZER_PIN);			// Clear buzzer (OFF)
	cbi(ACT_LED_PORT, ACT_LED_PIN);			// Clear activity LED (blue) (OFF)

	select_sci_bus_target(NON);				// Default SCI-bus target is none.

	// Enable serial communication.
	uart0_init(SCI_CCD_LO_SPEED);	// CCD-bus default (and only) speed is 7812.5 baud
	uart1_init(SCI_CCD_LO_SPEED);	// SCI-bus default speed is 7812.5 baud
	uart2_init(3);					// USB communication takes place at 250000 baud speed
	//uart3_init(3);					// DS18B20 custom UART protocol

	// Enable LCD.
	LCD_init(); // Init
	LCD_clr();  // Clear screen
	LCD_setCursorXY(0, 0); // Put cursor at the top left corner
	//LCD_drawFullBMP(flanders_bmp); // Draw some picture
	//LCD_drawFullBMP(chrysler_logo_bmp); // Draw some picture
	//LCD_drawFullBMP(chrysler_keyboard_01);
	LCD_drawFullBMP(doge_wow);

	// Clear USB packet buffer in case some garbage appears from nowhere
	bool command_timeout_reached = false;
	uint32_t command_timeout_start = millis_get();
	while ((uart2_available_rx() > 0) && !command_timeout_reached)
	{
		uart2_getc(); // read one byte into oblivion
		if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true; // check if it takes too much time
	}
	command_timeout_reached = false;
				
	// Read flash memory for pre-stored handshake bytes
	for (uint8_t i = 0; i < 21; i++)
	{
		handshake_array[i] = pgm_read_byte(&handshake_progmem[i]);
	}

	// Enable global interrupts.
	init_interrupt();


	// -----------------------------------------------------
	// This would be the loop() function of the Arduino IDE.
	// -----------------------------------------------------

	// Loop forever
	for(;;)
    {
		// Check if a command has been received from the laptop
		check_commands();
		
		// Do CCD-bus things here.
		if (ccd_enabled) { handle_ccd_data(); }

		// Do SCI-bus things here.
		if (sci_enabled) { handle_sci_data(); }

		// Do LCD things here.
		if (lcd_enabled) { /* coming soon...*/ }

		// Do button things here.
		if (button_pressed) { /* coming soon...*/ }

		// Blink activity LED to show looping is OK and didn't freeze somewhere.
		uint32_t current_millis = millis_get(); // check current time

		// Check if enough time has elapsed (interval) to invert LED state
		if (current_millis - previous_millis >= interval)
		{
			previous_millis = current_millis; // save current time
			ibi(ACT_LED_PORT, ACT_LED_PIN); // invert LED state (on-off-on-off...)
		}
    }

	return 0; // The program will never reach this point but formally the main function has to return an integer number (zero).
}
