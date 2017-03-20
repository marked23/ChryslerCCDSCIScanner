/******************************************************************************
Project:  CHRYSLER CCD/SCI SCANNER
Author:   László Dániel
Email:    laszlodaniel@gmail.com
Date:     2016-10-09
Revision: V1.30
Purpose:  receive and transmit messages from and to the legacy CCD/SCI-network
Software: Atmel Studio 7.0
Hardware: ATmega2560 + ATmega8U2 + CDP68HC68S1E + CD4066 + MCP23017
License:  GNU General Public License

Based on original program by Juha Niinikoski:
http://www.kolumbus.fi/juha.niinikoski/CCD_bus/ccd_display.htm

UART code is based on original library by Andy Gock:
https://github.com/andygock/avr-uart

They helped me a lot:
Sergey Tsinin with BCM firmware reverse engineering and sharing general knowledge
of diagnostic request messages.
Kyle Repinski with understanding the DRB-III's database file.
Many others with little things.

Supported OBD-II pin layout:
3:  CCD+
4:  Chassis Ground
5:  Signal Ground
6:  SCI Receive  (RX) (PCM)
7:  SCI Transmit (TX) (PCM, TCM)
11: CCD-
14: SCI Receive  (RX) (TCM)
16: +12V

Supported cars: all Chrysler vehicles (manual or auto transmission) equipped 
with CCD- and/or SCI-bus with above pin configuration.

The program uses the 2560's hardware UARTs to communicate with both buses:
- UART0: CCD-bus communication,
- UART1: SCI-bus communication (exclusive for PCM and TCM),
- UART2: Packet-based communication between scanner and laptop,
- UART3: 1-wire communication with DS18B20 temperature sensor.

Note: this scanner can not be used to re-flash the PCM and/or TCM,
it doesn't have the necessary circuitry to achieve that!
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


/*
          SYSTEM INFO          
-------------------------------
   MCU: ATmega2560-16AU
 SPEED: 16MHz
 FLASH: 256kB
   RAM: 8kB
EEPROM: 4kB
-------------------------------
   USB: ATmega8U2-AU
 SPEED: 16MHz
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

// Task variables in which every bit position has a different meaning
uint16_t volatile sci_bus_tasks = 0x0000;
uint16_t volatile ccd_bus_tasks = 0x0000;
uint16_t lcd_tasks     = 0x0000;
uint16_t button_tasks  = 0x0000;

uint8_t sci_active_bus = 0x00;

uint32_t previous_millis = 0;
uint32_t interval = 500;

volatile uint32_t ccd_messages_count;
volatile uint32_t sci_messages_count;

bool ccd_bus_present = false;
bool sci_bus_present = false;

// Timeout values for packets
uint8_t command_timeout = 100; // milliseconds
uint16_t command_purge_timeout = 1000; // milliseconds

const uint8_t handshake_progmem[] PROGMEM = { 0x43, 0x48, 0x52, 0x59, 0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44, 0x53, 0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E, 0x45, 0x52 };
//											    "C     H     R     Y     S     L     E     R     C     C     D     S     C     I     S     C     A     N     N     E     R"

uint8_t handshake_array[21];
bool dummy_ok = false;

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



/******************************************************************************
                                   Functions                                   
******************************************************************************/


/*****************************************************************************
Function: read_avr_signature()
Purpose:  read signature bytes
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
Notes:    UART2 is used to send and receive packets.
          Here are the tasks created/assigned.
**************************************************************************/
void check_commands(void)
{
	// Proceed only if the packet receive buffer (UART2) contains at least 5 bytes.
	// Two dummy bytes are always there in the beginning (caused by the ATmega8U2)
	if (uart2_available_rx() > 4)
	{
		// Make some local variables, they will disappear after the function ends
		uint8_t sync, length_hb, length_lb, datacode, subdatacode, checksum;
		bool payload_bytes = false;
		uint16_t bytes_to_read = 0;
		uint16_t payload_length = 0;
		uint8_t calculated_checksum = 0;

		uint32_t command_timeout_start = 0;
		bool command_timeout_reached = false;

		// Find the first SYNC byte (0x33)
		command_timeout_reached = false;
		command_timeout_start = millis_get();
		while ( ((uart2_peek() & 0xFF) != SYNC_BYTE) && !command_timeout_reached)
		{
			// If it's not the SYNC byte then get rid of it
			uart2_getc();
			if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true;
		}
		command_timeout_reached = false;

		if (uart2_available_rx() == 0) return; // exit this function right here if there are no bytes left

		// Read 3 bytes (one SYNC and two LENGTH byte).
		// All UART reads are masked with 0xFF because the ring buffer
		// contains words (two bytes). MSB contains flags, LSB contains
		// the actual data byte. The "& 0xFF" mask gets rid of the MSB.

		sync      = uart2_getc() & 0xFF;
		length_hb = uart2_getc() & 0xFF;
		length_lb = uart2_getc() & 0xFF;

		// Calculate how much more bytes should we read.
		bytes_to_read = (length_hb << 8 | length_lb) + 1; // +1 CHECKSUM byte

		// Max packet length is 2048 bytes. 3 bytes are already read and the checksum byte is not read yet.
		// Can't accept larger packets so if that's the case, the function needs to exit after sending an error packet back to the laptop.
		if ((bytes_to_read - 1) > 2044)
		{
			send_packet(from_scanner, to_laptop, ok_error, error_length_invalid_value, (uint8_t*)ok_error, 0);
			return;
		}

		// Calculate the exact size of the payload
		payload_length = bytes_to_read - 3;

		// Save us some headache
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
				// Read all the PAYLOAD bytes (max 2042 (!) bytes)
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
			calculated_checksum += length_hb;
			calculated_checksum += length_lb;
			calculated_checksum += datacode;
			calculated_checksum += subdatacode;

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
				// Find out what is the source and the target of the packet by examining the high nibble (4 bits)
				uint8_t source = (datacode >> 6) & 0x03; // keep the upper two bits
				uint8_t target = (datacode >> 4) & 0x03; // keep the lower two bits

				// Extract DC command value from the low nibble (4 bits)
				uint8_t dc_command = datacode & 0x0F;

				if (source == from_laptop) // 0x00 - Proceed only if the packet is coming from the laptop.
				{
					switch (target) // Evaluate target value.
					{
						case to_laptop: // 0x00 - If the target is the laptop itself then simply return the received packet as is
						{
							if (payload_bytes) send_packet(from_laptop, to_laptop, datacode, subdatacode, cmd_payload, payload_length);
							else send_packet(from_laptop, to_laptop, datacode, subdatacode, (uint8_t*)ok, 0);
							break;
						}
						case to_scanner: // 0x01 - Scanner is the target.
						{
							switch (dc_command) // Evaluate DC command
							{
								case reboot: // 0x00 - Reboot scanner.
								{
									// Send REBOOT packet back first to acknowledge the scanner has received the command
									send_packet(from_scanner, to_laptop, reboot, ok, (uint8_t*)ok, 0);

									// Wait for 200 milliseconds to finish all operations
									_delay_ms(200);

									// Then call the reboot function which activates the watchdog timer and waits for the reboot to happen
									wdt_init();
									avr8_software_reset();
									break;
								}
								case handshake: // 0x01 - Handshake request.
								{
									// Create a local array to save memory in the long run
									uint8_t handshake_array[21];

									// Read flash memory for pre-stored handshake bytes
									for (uint8_t i = 0; i < 21; i++)
									{
										handshake_array[i] = pgm_read_byte(&handshake_progmem[i]);
									}

									// Send it back to the laptop
									send_packet(from_scanner, to_laptop, handshake, ok, handshake_array, 21);

									// Reset tasks
									stop_clock_generator();
									select_sci_bus_target(PCM);

									break;
								}
								case status: // 0x02 - Scanner status report request
								{
									// Gather status data and send it back
									send_packet(from_scanner, to_laptop, status, ok, (uint8_t*)ok, 0);
									break;
								}
								case settings: // 0x03 - Change scanner settings
								{
									switch (subdatacode) // Evaluate sub-data code byte
									{
										case 0x00:
										{
											// Sub-data code missing, not enough information is given
											send_packet(from_scanner, to_laptop, ok_error, error_subdatacode_not_enough_info, (uint8_t*)ok, 0);
											break;
										}
										case read_settings: // 0x01 - Read scanner settings directly from EEPROM
										{
											send_packet(from_scanner, to_laptop, ok_error, ok, (uint8_t*)ok, 0); // acknowledge
											dummy_ok = true;
											interval = 50;
											break;
										}
										case write_settings: // 0x02 - Write scanner settings directly to EEPROM (values in PAYLOAD) (warning!)
										{
											send_packet(from_scanner, to_laptop, ok_error, ok, (uint8_t*)ok, 0); // acknowledge
											dummy_ok = false;
											interval = 500;
											break;
										}
										case enable_ccd_bus: // 0x03 - Enable CCD-bus communication
										{
											// First make sure the CCD-buffers are empty
											while (uart0_available() > 0) uart0_getc();

											// Reset pending message buffer too
											ccd_bus_bytes_buffer_ptr = 0;
											ccd_bus_msg_pending = false;
											ccd_bus_msg_to_send_ptr = 0;
											
											ccd_enabled = true;
											uint8_t ack[1] = { 0x00 };
											start_clock_generator();
											send_packet(from_scanner, to_laptop, settings, enable_ccd_bus, ack, 1); // acknowledge with zero byte
											break;
										}
										case disable_ccd_bus: // 0x04 - Disable CCD-bus communication
										{
											ccd_enabled = false;
											uint8_t ack[1] = { 0x00 };
											stop_clock_generator();
											send_packet(from_scanner, to_laptop, settings, disable_ccd_bus, ack, 1); // acknowledge with zero byte
											break;
										}
										case enable_sci_bus: // 0x05 - Enable SCI-bus communication
										{
											// First make sure the SCI-buffers are empty
											while (uart1_available() > 0) uart1_getc();

											// Reset pending message buffer too
											sci_bus_bytes_buffer_ptr = 0;
											sci_bus_msg_pending = false;
											sci_bus_msg_to_send_ptr = 0;
											
											sci_enabled = true;
											select_sci_bus_target(cmd_payload[0]); // 0x01 if PCM, 0x02 if TCM
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
											break;
										}
										case enable_lcd_bl_pwm: // 0x08 - Enable LCD backlight dimming (0-100% PWM value in PAYLOAD)
										{
											break;
										}
										case disable_lcd_bl: // 0x09 - Disable LCD backlight (0%)
										{
											break;
										}
										case metric_units: // 0x0A - Use metric units
										{
											break;
										}
										case imperial_units: // 0x0B - Use imperial units
										{
											break;
										}
										case ext_eeprom_wp_on: // 0x0C - Enable external EEPROM write protection (read only)
										{
											break;
										}
										case ext_eeprom_wp_off: // 0x0D - Disable external EEPROM write protection(read/write)
										{
											break;
										}
										case set_ccd_interframe_response: // 0x0E - Set CCD-bus interframe response delay (ms in PAYLOAD)
										{
											break;
										}
										case set_sci_interframe_response: // 0x0F - Set SCI-bus interframe response delay (ms in PAYLOAD)
										{
											break;
										}
										case set_sci_intermsg_response: // 0x10 - Set SCI-bus intermessage response delay (ms in PAYLOAD)
										{
											break;
										}
										case set_sci_intermsg_request: // 0x11 - Set SCI-bus intermessage request delay (ms in PAYLOAD)
										{
											break;
										}
										case set_packet_intermsg_response: // 0x12 - Set PACKET intermessage response delay (ms in PAYLOAD)
										{
											break;
										}
										case enable_sci_bus_high_speed: // 0x13 - Enable SCI-bus high speed mode (62500 baud)
										{
											break;
										}
										case disable_sci_bus_high_speed: // 0x14 - Disable SCI-bus high speed mode (7812.5 baud)
										{
											break;
										}
										case enable_ccd_bus_filtering: // 0x15 - Enable CCD-bus message filtering (ID bytes in PAYLOAD)
										{
											break;
										}
										case disable_ccd_bus_filtering: // 0x16 - Disable CCD-bus message filtering
										{
											break;
										}
										case enable_sci_bus_filtering: // 0x17 - Enable SCI-bus message filtering (ID bytes in PAYLOAD)
										{
											break;
										}
										case disable_sci_bus_filtering: // 0x18 - Disable SCI-bus message filtering
										{
											break;
										}
										case set_sci_bus_target: // 0x19 - Set SCI-bus target (PCM, TCM, none in PAYLOAD)
										{
											break;
										}
										case enable_buzzer: // 0x1A - Enable buzzer
										{
											break;
										}
										case disable_buzzer: // 0x1B - Disable buzzer
										{
											break;
										}
										case enable_button_hold: // 0x1C - Enable button hold down sensing
										{
											break;
										}
										case disable_button_hold: // 0x1D - Disable button hold down sensing
										{
											break;
										}
										case enable_act_led: // 0x1E - Enable ACT LED (action, blue color)
										{
											break;
										}
										case disable_act_led: // 0x1F - Disable ACT LED (action, blue color)
										{
											break;
										}
										default: // These values are not used
										{
											send_packet(from_scanner, to_laptop, ok_error, error_subdatacode_invalid_value, (uint8_t*)ok, 0);
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
											send_packet(from_scanner, to_laptop, ok_error, error_subdatacode_not_enough_info, (uint8_t*)ok, 0);
											break;
										}
										case scanner_firmware_version: // 0x01 - Scanner firmware version
										{
											break;
										}
										case read_int_eeprom: // 0x02 - Read internal EEPROM in chunks (size in PAYLOAD)
										{
											break;
										}
										case read_ext_eeprom: // 0x03 - Read external EEPROM in chunks (size in PAYLOAD)
										{
											break;
										}
										case write_int_eeprom: // 0x04 - Write internal EEPROM in chunks (value(s) in PAYLOAD)
										{
											break;
										}
										case write_ext_eeprom: // 0x05 - Write external EEPROM in chunks (value(s) in PAYLOAD)
										{
											break;
										}
										case scan_ccd_bus_modules: // 0x06 - Scan CCD-bus modules
										{
											break;
										}
										case free_ram_value: // 0x07 - Free RAM available
										{
											// Read the actual free ram value
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
										default: // These values are not used
										{
											send_packet(from_scanner, to_laptop, ok_error, error_subdatacode_invalid_value, (uint8_t*)ok, 0);
											break;
										}
									}
									break;
								}
								case self_diag: // 0x0A - Run self-diagnostics
								{
									break;
								}
								case make_backup: // 0x0B - Create scanner settings backup packet (int. EEPROM dump)
								{
									break;
								}
								case restore_backup: // 0x0C - Restore scanner settings from backup
								{
									break;
								}
								case restore_default: // 0x0D - Restore default scanner settings (factory reset)
								{
									break;
								}
								case debug: // 0x0E - Debug
								{
									// 5 valid packets are hidden in this mess
									uint8_t dummy_packet[168] = {	0x33, 0x67, 0x23, 0x64, 0x33, 0x34, 0x77, 0xAA,
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
																				
									for (uint8_t i = 0; i < 167; i++)
									{
										uart2_putc(dummy_packet[i]);
									}
									break;
								}
								case ok_error: // 0x0F - OK/ERROR message
								default: // These values are not used.
								{
									send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_dc_command, (uint8_t*)ok, 0);
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
									break;
								}
								case send_rep_msg: // 0x07 - Send message(s) repeatedly to the CCD-bus
								{
									break;
								}
								case stop_msg_flow: // 0x08 - Stop message flow to the CCD-bus
								{
									break;
								}
								default: // These values are not used.
								{
									send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_dc_command, (uint8_t*)ok, 0);
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
									break;
								}
								case send_rep_msg: // 0x07 - Send message(s) repeatedly to the SCI-bus
								{
									break;
								}
								case stop_msg_flow: // 0x08 - Stop message flow to the SCI-bus
								{
									break;
								}
								default: // These values are not used.
								{
									send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_dc_command, (uint8_t*)ok, 0);
									break;
								}
							}
							break;
						}
						default: // These values are not used.
						{
							send_packet(from_scanner, to_laptop, ok_error, error_datacode_invalid_target, (uint8_t*)ok, 0);
							break;						
						}
					} // switch (target)
				} // if (source == laptop)
				else
				{
					// Packet is coming from an unknown source
					send_packet(from_scanner, to_laptop, ok_error, error_packet_unknown_source, (uint8_t*)ok, 0);
				}
			} // SYNC and CHECKSUM is OK
			else // CHECKSUM byte and/or SYNC byte error! Something is wrong with this packet
			{
				if ( calculated_checksum != checksum )
				{
					send_packet(from_scanner, to_laptop, ok_error, error_checksum_invalid_value, (uint8_t*)ok, 0);
				}

				if ( sync != SYNC_BYTE )
				{
					send_packet(from_scanner, to_laptop, ok_error, error_sync_invalid_value, (uint8_t*)ok, 0);
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
			send_packet(from_scanner, to_laptop, ok_error, error_packet_timeout_occured, (uint8_t*)ok, 0);

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
		// TODO: check here if the maximum number of 3 bytes are stayed long enough to consider them junk and get rid of them
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
		// Don't care what message was received, the point is that something more was received than a command echo
	}
	else
	{
		cbi(ret, 1);
	}

	return ret;
}


/*************************************************************************
Function: scan_ccd_bus_modules()
Purpose:  discovers all the modules on the CCD-bus.
**************************************************************************/
void scan_ccd_bus_mods(void)
{
	
}


/*************************************************************************
Function: select_sci_bus()
Purpose:  PCM and TCM share the same SCI-bus, different RX-lines, but same
          TX-line and this function created a route for one module.
Params:   bus number (see switch statement)
Returns:  true if success,
          false if wrong bus number was selected (PCM is selected by default).
Note:     PD4 pin (SCI_BUS_PCM_PIN) selects the PCM (Powertrain Control Module),
          PD5 pin (SCI_BUS_TCM_PIN) selects the TCM (Transmission Control Module).
		  They can't be HIGH at the same time.
**************************************************************************/
bool select_sci_bus_target(uint8_t bus)
{
    sbi(DDRD, DDD4); // Set PD4 pin to output
    sbi(DDRD, DDD5); // Set PD5 pin to output

	switch (bus)
	{
		case 0x00: // No SCI-module selected
		{
			cbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 LOW
			cbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 LOW
			return true;
			break;

		}
		case 0x01: // PCM selected
		{
			sbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 HIGH
			cbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 LOW
			return true;
			break;
		}
		case 0x02: // TCM selected
		{
			cbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 LOW
			sbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 HIGH
			return true;
			break;
		}
		default: // PCM is selected automatically
		{
			sbi(SCI_BUS_PORT, SCI_BUS_PCM_PIN); // Set PD4 HIGH
			cbi(SCI_BUS_PORT, SCI_BUS_TCM_PIN); // Set PD5 LOW
			return false;
			break;
		}
	}

	return false;
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
	ccd_idle = true;
}


// CCD-bus active byte detector on INT5 pin
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
	sbi(DDRE, DDE3);						// Data Direction Register E, set Data Direction on Port E pin 3 to 1 => output
	sbi(LCD_BGLIGHT_PORT, LCD_BGLIGHT_PIN);	// Set LCD backlight (ON)
	sbi(DDRD, DDD6);						// Data Direction Register D, set Data Direction on PORT D pin 6 to 1 => output
	cbi(BUZZER_PORT, BUZZER_PIN);			// Clear BUZZER (OFF)
	sbi(DDRD, DDD7);						// Data Direction Register D, set Data Direction on PORT D pin 7 to 1 => output
	cbi(ACT_LED_PORT, ACT_LED_PIN);			// Clear Activity LED (blue) (OFF)

	select_sci_bus_target(NON);				// Default SCI-bus target is none.
	
	// Read scanner settings from internal EEPROM.
	// ...

	// Make changes according to saved settings.
	// ...
	//start_clock_generator(); // CCD-bus ON!

	// Enable serial communication.
	uart0_init(SCI_CCD_LO_SPEED);	// CCD-bus default (and only) speed is 7812.5 baud
	uart1_init(SCI_CCD_LO_SPEED);	// SCI-bus default speed is 7812.5 baud
	uart2_init(3);					// USB communication takes place at 250000 baud speed
//	uart3_init(3);					// DS18B20 custom UART protocol

	// Enable LCD.
	LCD_init(); // Init
	LCD_clr();  // Clear screen
	LCD_setCursorXY(0, 0); // Put cursor at the top left corner
	//LCD_drawFullBMP(flanders_bmp); // Draw some picture
	//LCD_drawFullBMP(chrysler_logo_bmp); // Draw some picture
	LCD_drawFullBMP(chrysler_keyboard_01);

	// Clear packet buffer in case some garbage appears from nowhere
	bool command_timeout_reached = false;
	uint32_t command_timeout_start = millis_get();
	while ((uart2_available_rx() > 0) && !command_timeout_reached)
	{
		uart2_getc();
		if (millis_get() - command_timeout_start > command_purge_timeout) command_timeout_reached = true;
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
		if (ccd_enabled)
		{
			// Check if there are any data bytes in the CCD-ringbuffer
			if (uart0_available() > 0)
			{
				// Peek one byte (don't remove it from the buffer yet)
				uint16_t dummy_read = uart0_peek();

				// If it's an ID-byte then send the previous bytes to the laptop
				if (((dummy_read >> 8) & 0xFF) == 0x40)
				{
					//              where        to         what      ok/error flag     buffer          length
					send_packet(from_ccd_bus, to_laptop, receive_msg, ok, ccd_bus_bytes_buffer, ccd_bus_bytes_buffer_ptr);
					ccd_bus_bytes_buffer_ptr = 0; // reset pointer

					// And save this byte as the first one in the buffer
					ccd_bus_bytes_buffer[ccd_bus_bytes_buffer_ptr] = uart0_getc() & 0xFF;
					ccd_bus_bytes_buffer_ptr++;
				}
				else // get this byte and save to the temporary buffer
				{
					ccd_bus_bytes_buffer[ccd_bus_bytes_buffer_ptr] = uart0_getc() & 0xFF;
					ccd_bus_bytes_buffer_ptr++;
				}
			}

			// If there's a message to be sent to the CCD-bus and the bus happens to be idling then send it here and now
			if (ccd_bus_msg_pending && ccd_idle)
			{
				for (uint8_t i = 0; i < ccd_bus_msg_to_send_ptr; i++)
				{
					uart0_putc(ccd_bus_msg_to_send[i]);
				}

				ccd_bus_msg_to_send_ptr = 0; // reset pointer
				ccd_bus_msg_pending = false; // re-arm
			}
			
			switch (ccd_bus_tasks)
			{
				case 0x00: // Check if there's a full message in the receive buffer
				{
					
					break;
				}
				case 0x01: // At least one CCD-message is ready to be transmitted through USB
				{

					break;
				}
				default: // Anything else is discarded like it was 0x00
				{
					
					break;
				}
			}
		}

		// Do SCI-bus things here.
		if (sci_enabled)
		{
			// Check if there are any data bytes in the SCI-ringbuffer
			if (uart1_available() > 0)
			{
				// Check how much bytes do we have
				uint8_t sci_bytes_available = uart1_available();

				// Save all of them
				for (uint8_t i = 0; i < sci_bytes_available; i++)
				{
					sci_bus_bytes_buffer[sci_bus_bytes_buffer_ptr] = uart1_getc();
					sci_bus_bytes_buffer_ptr++;
				}

				// Save the current time
				last_sci_byte_received = millis_get();
				sci_bus_active_bytes = true;
			}

			// If there's no data coming in after a while then consider the previous message to be completed
			// Make sure there's data in the buffer before sending empty packets... been there done that...
			if ((millis_get() - last_sci_byte_received > SCI_INTERMESSAGE_RESPONSE_DELAY) && (sci_bus_bytes_buffer_ptr > 0) )
			{
				send_packet(from_sci_bus, to_laptop, receive_msg, ok, sci_bus_bytes_buffer, sci_bus_bytes_buffer_ptr); // send sci-bus msg
				sci_bus_bytes_buffer_ptr = 0; // reset pointer
				sci_bus_active_bytes = false;
			}

			// If there's a message to be sent to the SCI-bus then send it here and now
			// Check if there's activity on the SCI-bus before sending, if there's activity then wait!
			if (sci_bus_msg_pending && !sci_bus_active_bytes)
			{
				for (uint8_t i = 0; i < sci_bus_msg_to_send_ptr; i++)
				{
					uart1_putc(sci_bus_msg_to_send[i]);
				}

				sci_bus_msg_to_send_ptr = 0; // reset pointer
				sci_bus_msg_pending = false; // re-arm
				sci_bus_active_bytes = true;
			}
			
			switch (sci_bus_tasks)
			{
				case 0x00: // Don't do anything!
				{
					break;
				}
				case 0x01: // At least one SCI-message is ready to be transmitted through USB
				{

					break;
				}
				default: // Anything else is discarded like it was 0x00
				{
					
					break;
				}
			}
		}

		// Do LCD things here.
		if (lcd_enabled)
		{
			switch (lcd_tasks)
			{
				case 0x00: // Don't do anything!
				{
					break;
				}
				case 0x01: // Do something LCD related.
				{
					if (lcd_enabled)
					{
						lcd_enabled = false;
					}
					break;
				}
				default: // Anything else is discarded like it was 0x00
				{
					break;
				}
			}
		}

		// Do button things here (process button presses).
		if (button_pressed)
		{
			// Figure out which button was pressed and proceed evaluating it
			button_pressed = false; // Re-arm button press detection

			switch (button_tasks)
			{
				case 0x00: // Don't do anything!
				{
					break;
				}
				case 0x01: // Do something button related.
				{
					break;
				}
				default: // Anything else is discarded like it was 0x00
				{
					break;
				}
			}

		}

		// Blink activity LED to show everything is okay
		uint32_t current_millis = millis_get();
		if (current_millis - previous_millis >= interval)
		{
			previous_millis = current_millis;
			ibi(ACT_LED_PORT, ACT_LED_PIN);
			
			if (dummy_ok)
			{
				// Send it back to the laptop
				send_packet(from_scanner, to_laptop, handshake, ok, handshake_array, 21);
			}
		}
    }

	return 0; // The program will never reach this point but formally the main function has to return an integer number (zero).
}
