#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#include <util/delay.h>
#include <inttypes.h>
#include <stdbool.h>
#include "ccdsci.h"
#include "uart.h"

// Variables

// CCD-bus related variables

bool  bcm_memory_request = false;

uint8_t  ccd_msg_array[16];
uint8_t  ccd_msg_array_ptr = 0;

uint8_t  sci_msg_array[16];
uint8_t  sci_msg_array_ptr = 0;

uint8_t  pending_sci_bytes[16];
uint8_t  pending_sci_bytes_ptr = 0;
bool  pending_sci_msg = false;

uint8_t  pending_ccd_bytes[16];
uint8_t  pending_ccd_bytes_ptr = 0;
bool  pending_ccd_msg = false;

uint8_t  pcm_memory_26_request[4] = { 0x26, 0x00, 0x00, 0x00 };
uint8_t  pcm_memory_26_request_ptr = 0;

uint8_t  pcm_memory_28_request[3] = { 0x28, 0x00, 0x00 };
uint8_t  pcm_memory_28_request_ptr = 0;

bool  log_enabled = true;
bool  pcm_memory_26_read = false;
bool  pcm_memory_28_read = false;

bool  pcm_ram_read = false;
uint8_t  pcm_ram_area = 0;

bool  pcm_o2_volts_read = false;
bool  pcm_map_volts_read = false;
bool  pcm_read_all_parameters = false;

bool  sci_highspeed_enabled = false;
uint8_t  uart_settings[5];

uint8_t  exteeprom_chunk[128];
uint8_t  exteeprom_chunk_ptr = 0;
bool  exteeprom_chunk_ready = false;

bool  packet_waiting = false;


volatile bool     sci_idle = false;               // Set when EOM (End of Message) condition detected, cleared automatically
volatile uint32_t total_sci_msg_count = 0;        // Total SCI-messages received since last restart
volatile uint8_t  sci_buff_ptr;                   // CCD internal receive buffer pointer
volatile uint8_t  sci_message_length;             // CCD message pointer / length
volatile uint16_t sci_checksum_check;             // Adding CCD messages bytes together in here (except the last one)
volatile uint8_t  calculated_sci_checksum;        // The lower byte of the "ccd_checksum_check" is stored here
volatile bool  sci_err;                        // True when the calculated CRC byte is not equal to the last byte of the CCD-message

uint8_t  sci_timeout = 50; // ms
uint32_t sci_timeout_start = 0;
bool  sci_timeout_reached = false;
bool  sci_no_response = false;
uint8_t  sci_id_byte = 0;
uint32_t sci_tx_sent_time = 0;
uint16_t sci_tx_timeout = 500;
bool  sci_tx_buffer_loaded = false;
uint16_t counter = 0;
uint8_t  sci_byte_to_send[8];
uint8_t  sci_tx_length = 0;
uint8_t  sci_req_counter = 0;
uint8_t  sci_req_counter_2 = 0;
bool  sci_highspeed = false;
bool  sci_tx_handled = true;


// CCD related variables

volatile bool  ccd_idle = false;				// Set when EOM (End of Message) condition detected, cleared automatically
volatile bool ccd_active_byte;					// Set when there's a byte being communicated on the CCD-bus
volatile uint32_t total_ccd_msg_count = 0;		// Total CCD-messages received since last restart
volatile uint8_t  ccd_buff_ptr;					// CCD internal receive buffer pointer
volatile uint8_t  ccd_rx_message_length;		// CCD message pointer / length
volatile uint16_t ccd_checksum_check;			// Adding CCD messages bytes together in here (except the last one)
volatile uint8_t  calculated_ccd_checksum;		// The lower byte of the "ccd_checksum_check" is stored here
volatile bool  ccd_err;							// True when the calculated CRC byte is not equal to the last byte of the CCD-message
volatile bool  ccd_rx_message_processed = false;

uint8_t		ccd_id_byte           = 0;		// Message Identification (ID) byte
uint32_t	ccd_rx_message_starttime = 0;	// Message start time (us) since last restart
uint32_t	ccd_rx_message_endtime   = 0;	// Message end time (us) since last restart
uint16_t	ccd_rx_message_duration  = 0;	// Message length in microseconds (us)
uint32_t	ccd_dist_vehicle_raw  = 0;		// Vehicle Distance (raw data)
float		ccd_dist_vehicle_mi   = 0;		// Vehicle Distance (mi)
float		ccd_dist_vehicle_km   = 0;		// Vehicle Distance (km)
uint32_t	ccd_dist_trip_raw     = 0;		// Trip Distance (raw data)
float		ccd_dist_trip_mi      = 0;		// Trip Distance (mi)
float		ccd_dist_trip_km      = 0;		// Trip Distance (km)
uint16_t	ccd_spd_raw    = 0;				// Speed (raw, mph + km/h)
float		ccd_spd_mph    = 0;				// Speed (mph)
float		ccd_spd_kmh    = 0;				// Speed (km/h)
uint8_t		ccd_rpm_raw    = 0;				// RPM (raw)
uint16_t	ccd_rpm        = 0;				// RPM (1/min)
uint8_t		ccd_map_raw    = 0;				// MAP-Sensor (raw)
float		ccd_map        = 0;				// MAP-Sensor (kPa)
float		ccd_fuel       = 0;				// Fuel Efficiency
uint8_t		ccd_tps_raw    = 0;				// Throttle Position Sensor (raw)
float		ccd_tps        = 0;				// Throttle Position Sensor (%)
uint8_t		ccd_cruise     = 0;				// Cruise settings
uint8_t		ccd_vs_raw     = 0;				// Battery Voltage (raw)
float		ccd_vs         = 0;				// Battery Voltage (V)
uint8_t		ccd_vc_raw     = 0;				// Target Charging Voltage (raw)
float		ccd_vc         = 0;				// Target Charging Voltage (V)
uint8_t		ccd_ect_raw    = 0;				// Engine Coolant Temperature (raw)
bool		ccd_ect_pos    = true;			// Positive or negative temperature
uint8_t		ccd_ect        = 0;				// Engine Coolant Temperature (°C)
uint8_t		ccd_bt_raw     = 0;				// Battery Temperature (raw)
bool		ccd_bt_pos     = true;			// Positive or negative temperature
uint8_t		ccd_bt         = 0;				// Battery Temperature (°C)
uint8_t		ccd_vin[17];					// Vehicle Identification Number (VIN)
uint32_t	ccd_vin_counter  = 0;
bool		ccd_vin_complete = false;
uint8_t		ccd_vin_wmi[3];					// World Manufacturer Identifier (VIN 1-3)
uint8_t		ccd_vin_vds[6];					// Vehicle Descriptor Section    (VIN 4-9)
uint8_t		ccd_vin_vis[8];					// Vehicle Identifier Section    (VIN 10-17)
char		*ccd_vin_string[17];			// Decoded string of the VIN
bool		ccd_tx_buffer_loaded = false;
uint32_t	ccd_tx_sent_time = 0;
uint16_t	ccd_tx_timeout = 300;
uint8_t		ccd_req_counter = 0;
uint8_t		ccd_req_counter_2 = 0;


// Functions

/*****************************************************************************
Function: ccd_eom()
Purpose:  set flag for "Start of Message" condition
******************************************************************************/
void ccd_eom(void)
{
	ccd_idle = true;  // set flag so the main loop knows, simple as that
	//total_ccd_msg_count++;  // increment message counter for statistic purposes
	
} /* ccd_eom */


/*************************************************************************
Function: start_clock_generator()
Purpose:  generates 1 MHz clock signal for the CDP68HC68S1 chip,
          effectively turning it on
**************************************************************************/
void start_clock_generator(void)
{
    TCCR1A = 0;
    TCCR1B = 0;
    TCNT1  = 0;
    DDRB   = (1<<DDB5);                // set OC1A/PB5 as output
    TCCR1A = (1<<COM1A0);              // toggle OC1A on compare match
    OCR1A  = 7;                        // top value for counter, toggle after counting to 8 (0->7) = 2 MHz interrupt ( = 16 MHz clock frequency / 8)
    TCCR1B = (1<<WGM12) | (1<<CS10);   // CTC mode, prescaler clock/1 (no prescaler)
    
} // end of start_clock_generator


/*************************************************************************
Function: stop_clock_generator()
Purpose:  stops generating 1 MHz clock signal for the CDP68HC68S1 chip,
          effectively turning it off
**************************************************************************/
void stop_clock_generator(void)
{
    TCCR1A = 0;
    TCCR1B = 0;
    TCNT1  = 0;
	OCR1A  = 0;
    
} // end of stop_clock_generator


/*************************************************************************
Function: handle_ccd_rx_bytes()
Purpose:  as the name says
**************************************************************************/
void handle_ccd_rx_bytes(void)
{
    uint16_t dummy8 = uart0_getc(); // try to grab a byte from CCD receive buffer as frequently as possible! 
    if ( !(dummy8 & UART_NO_DATA) ) // if there is at least one byte...
    {
        //digitalWrite(DATAIN_LED, HIGH); // turn on led
        if (dummy8 & CCD_SOM) // if it's the ID byte... (Start of Message)
        {
            // If there's a complete message waiting to be transferred...
            if (ccd_msg_array_ptr > 0)
            {
                // send a CCDSCI Packet
                if (ccd_msg_array[0] == 0xF2) // If it's a diagnostic response CCD-message
                {
                    //send_packet(dc_from_ccd_bus, 0x01, ccd_msg_array, ccd_msg_array_ptr);
                }
                else // If it's a general CCD-message
                {
                    //send_packet(dc_from_ccd_bus, 0x00, ccd_msg_array, ccd_msg_array_ptr);
                }

                //process_ccd_rx_message(temp_msg_array);
            }

            // Reset pointer to zero...
            ccd_msg_array_ptr = 0;
        }
        // And start filling the buffer again with a message
        ccd_msg_array[ccd_msg_array_ptr] = dummy8 & 0xFF;

        // Increase message pointer by one so the next byte gets saved to the right position
        ccd_msg_array_ptr++;
        //digitalWrite(DATAIN_LED, LOW); // turn off led
    }
            
    // If it's okay to broadcast a message to the CCD-bus then proceed doing so
    if (ccd_idle && !ccd_tx_buffer_loaded)
    {
        // If the command tells to request BCM memory locations
        if (pending_ccd_msg)
        {
            
        } 
    }

} // end of handle_ccd_rx_bytes


/*************************************************************************
Function: handle_sci_rx_bytes()
Purpose:  as the name says
**************************************************************************/
void handle_sci_rx_bytes(void)
{
    // If there's a new message waiting to be send to the SCI-bus
    if (pending_sci_msg)
    {
        //digitalWrite(DATAIN_LED, HIGH);
        
        // If SCI-bus is in low speed mode (7812.5 baud) then echo is excepted for every byte sent
//        if (!sci_highspeed_enabled)
//        {
            // Cycle through all the message bytes
            for (uint8_t i = 0; i < pending_sci_bytes_ptr; i++)
            {
            
                uart1_putc(pending_sci_bytes[i]);
                sci_timeout_start = millis_get();
                while ( (uart1_available() == 0) && !sci_timeout_reached )
                { 
                    /* wait for echo or exit when timeout reached */ 
                    if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
                }
                
                // If there's a response in time then save it
                if (!sci_timeout_reached)
                {
                    uint8_t response_byte = uart1_getc() & 0xFF;
                    sci_msg_array[sci_msg_array_ptr] = response_byte;
                    sci_msg_array_ptr++;
                }
                else // timeout reached
                {
                    //send_packet(dc_from_scanner, sdc_no_sci_echo, (uint8_t*)empty_payload, 0);
                    sci_timeout_reached = false;
                }
            }
            pending_sci_bytes_ptr = 0; // revert settings to default
            pending_sci_msg = false;   // clear flag

            //digitalWrite(DATAIN_LED, LOW);

            packet_waiting = true;
            
//        } /* if (sci_highspeed_enabled == false) */

        // If SCI-bus is in high speed mode (62500 baud) then no echo is excepted just the response from the PCM
        if (sci_highspeed_enabled)
        {
            // Do nothing for now
        }
        
    } /* if (pending_sci_msg) */

    // If PCM memory 26 read is selected: 26 00 00 00
    // Memory address is 3 bytes long (128 kilobytes)
    if (pcm_memory_26_read)
    {
        uint8_t dummy20 = 0;
        
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        
        // Put first byte
        uart1_putc(pcm_memory_26_request[0]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;
        
        // Put second byte
        uart1_putc(pcm_memory_26_request[1]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;
        
        // Put third byte
        uart1_putc(pcm_memory_26_request[2]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;
        
        // Put fourth byte
        uart1_putc(pcm_memory_26_request[3]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;

        // Wait for PCM to return memory byte
        //uart1_putc(0xAB); // debug (send dummy byte as response
        while (uart1_available() == 0) { /* wait for response */ }
        dummy20 = uart1_getc() & 0xFF;

        // Write this byte to the temporary eeprom array
        exteeprom_chunk[exteeprom_chunk_ptr] = dummy20;
        exteeprom_chunk_ptr++;

        //digitalWrite(DATAIN_LED, LOW);  // turn off led

        // If enough bytes are gathered then reset pointer in the array
        if (exteeprom_chunk_ptr > 127)
        {
            exteeprom_chunk_ptr = 0;
            exteeprom_chunk_ready = true;
        }

        // And write the full chunk to the EEPROM
        if (exteeprom_chunk_ready)
        {
            exteeprom_chunk_ready = false;
            
            uint8_t pcm_memory_request_final[4 + 128];
            uint32_t eeprom_address = (((uint32_t)pcm_memory_26_request[1] << 16) | ((uint32_t)pcm_memory_26_request[2] << 8) | ((uint32_t)pcm_memory_26_request[3])) - 127;
            pcm_memory_request_final[0] = (eeprom_address >> 24) & 0xFF;
            pcm_memory_request_final[1] = (eeprom_address >> 16) & 0xFF;
            pcm_memory_request_final[2] = (eeprom_address >> 8) & 0xFF;
            pcm_memory_request_final[3] = eeprom_address & 0xFF;

            for (uint8_t i = 0; i < 128; i++)
            {
                pcm_memory_request_final[4 + i] = exteeprom_chunk[i];
            }

            // Send the chunk back to the computer
            //send_packet(dc_from_sci_bus, sdc_pcm_dump, pcm_memory_request_final, 4 + 128);

            // Save it to the external EEPROM memory too
            //eep.write(eeprom_address, exteeprom_chunk, 128);
        }

        // Increment memory address in the command
        pcm_memory_26_request[3]++;
        if (pcm_memory_26_request[3] == 0) pcm_memory_26_request[2]++;
        if ((pcm_memory_26_request[2] == 0) && (pcm_memory_26_request[3] == 0)) pcm_memory_26_request[1]++;

        // If 128k bytes are reached then stop requesting
        if ( (pcm_memory_26_request[1] == 0x02) && (pcm_memory_26_request[2] == 0x00) && (pcm_memory_26_request[3] == 0x00))
        {
            pcm_memory_26_read = false;
            //send_packet(dc_from_scanner, sdc_pcm_memory_26_read_off, (uint8_t*)empty_payload, 0);
        }
    }

    // If PCM memory 28 read is selected: 28 00 00
    // Memory address is 2 bytes long (512 bytes)
    if (pcm_memory_28_read)
    {
        uint8_t dummy20 = 0;
        
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        
        // Put first byte
        uart1_putc(pcm_memory_28_request[0]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;
        
        // Put second byte
        uart1_putc(pcm_memory_28_request[1]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;
        
        // Put third byte
        uart1_putc(pcm_memory_28_request[2]);
        while (uart1_available() == 0) { /* wait for echo */ }
        dummy20 = uart1_getc() & 0xFF;

        // Wait for PCM to return memory byte
        //uart1_putc(0xAB); // debug (send dummy byte as response
        while (uart1_available() == 0) { /* wait for response */ }
        dummy20 = uart1_getc() & 0xFF;

        // Write this byte to the temporary eeprom array
        exteeprom_chunk[exteeprom_chunk_ptr] = dummy20;
        exteeprom_chunk_ptr++;

        //digitalWrite(DATAIN_LED, LOW);  // turn off led

        // If enough bytes are gathered then reset pointer in the array
        if (exteeprom_chunk_ptr > 127)
        {
            exteeprom_chunk_ptr = 0;
            exteeprom_chunk_ready = true;
        }

        // And write the full chunk to the EEPROM
        if (exteeprom_chunk_ready)
        {
            exteeprom_chunk_ready = false;

            uint8_t pcm_memory_request_final[4 + 128];
            uint32_t eeprom_address = (((uint32_t)pcm_memory_28_request[1] << 8) | ((uint32_t)pcm_memory_28_request[2])) - 127;
            pcm_memory_request_final[0] = (eeprom_address >> 24) & 0xFF;
            pcm_memory_request_final[1] = (eeprom_address >> 16) & 0xFF;
            pcm_memory_request_final[2] = (eeprom_address >> 8) & 0xFF;
            pcm_memory_request_final[3] = eeprom_address & 0xFF;

            for (uint8_t i = 0; i < 128; i++)
            {
                pcm_memory_request_final[4 + i] = exteeprom_chunk[i];
            }

            // Send the chunk back to the computer
            //send_packet(dc_from_sci_bus, sdc_pcm_dump, pcm_memory_request_final, 4 + 128);

            // Save it to the external EEPROM memory too
            //eep.write(eeprom_address, exteeprom_chunk, 128);
        }
        
        // Increment memory address in the command
        pcm_memory_28_request[2]++;
        if (pcm_memory_28_request[2] == 0) pcm_memory_28_request[1]++;

        // If 512 bytes are reached then stop requesting
        if ( (pcm_memory_28_request[1] == 0x02) && (pcm_memory_28_request[2] == 0x00))
        {
            pcm_memory_28_read = false;
            //send_packet(dc_from_scanner, sdc_pcm_memory_28_read_off, (uint8_t*)empty_payload, 0);
        }
    }

    // If PCM RAM address read is selected
    if (pcm_ram_read && sci_highspeed_enabled)
    {       
        uint8_t  pcm_ram[256];

        uart1_putc(pcm_ram_area);
        while ( (uart1_available() == 0) ) { /* Wait for echo */ }
        uint8_t dummy23 = uart1_getc() & 0xFF;
        
        for (uint8_t i = 0; i < 240; i++)
        {
            // Put RAM address byte
            uart1_putc(i);
            sci_timeout_start = millis_get();
            while ( (uart1_available() == 0) && !sci_timeout_reached )
            { 
                /* wait for RAM data or exit when timeout reached */ 
                if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
            }
                
            // If there's a response in time then save it
            if (!sci_timeout_reached)
            {
                uint8_t dummy21 = uart1_getc() & 0xFF;
                pcm_ram[i] = dummy21;
            }
            else // timeout reached
            {
                //send_packet(dc_from_scanner, sdc_no_sci_echo, (uint8_t*)i, 1);
                sci_timeout_reached = false;
            }
        }

        for (uint8_t j = 0; j < 16; j++)
        {
            pcm_ram[240 + j] = 0xFF;
        }
        //send_packet(dc_from_scanner, sdc_pcm_ram_low_dump, pcm_ram, 256);
    }

    if (pcm_o2_volts_read && !sci_highspeed_enabled)
    {
        uint8_t aat_volts_data[3];
        uint8_t o2_volts_data[3];
        uint8_t tps_volts_data[3];
        uint8_t min_tps_volts_data[3];
        uint8_t map_volts_data[3];
        uint8_t aff_data[3];
        uint8_t baro_data[3];
        uint8_t rpm_data[3];
        uint8_t adv_data[3];
        uint8_t iat_sensor_data[3]; 
        
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        
        // Put first byte
        if (!sci_timeout_reached) uart1_putc(0x14);
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_timeout_reached )
        { 
            /* wait for echo */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
        }
        if (!sci_timeout_reached) // save the byte if it's received before timeout
        {
            o2_volts_data[0] = uart1_getc() & 0xFF;
        }
        
        // Put second byte
        if (!sci_timeout_reached) uart1_putc(0x42);
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_timeout_reached )
        {
            /* wait for echo */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
        }
        if (!sci_timeout_reached) // save the byte if it's received before timeout
        {
            o2_volts_data[1] = uart1_getc() & 0xFF;
        }

        // Wait for PCM to return O2 sensor data
        //uart1_putc(0xAB); // debug (send dummy byte as response)
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_no_response )
        {
            /* wait for response */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_no_response = true;
        }
        if (!sci_timeout_reached && !sci_no_response) // save the byte if it's received before timeout
        {
            o2_volts_data[2] = uart1_getc() & 0xFF;
        }
        
        //digitalWrite(DATAIN_LED, LOW);  // turn off led

        _delay_ms(40);
/*
        // Send back 3 bytes (2 command + 1 data) if timeout was never reached
        if (!sci_timeout_reached && !sci_no_response) send_packet(dc_from_sci_bus, sdc_pcm_o2_volts_data, o2_volts_data, 3);
        else // if at least 1 timeout has been reached then abort operation and send back "no sci echo" packet
        {
            if (sci_timeout_reached) send_packet(dc_from_scanner, sdc_no_sci_echo, (uint8_t*)empty_payload, 0);
            if (sci_no_response) send_packet(dc_from_scanner, sdc_no_sci_response, (uint8_t*)empty_payload, 0);
            sci_timeout_reached = false; // reset timeout flag so sub-function can have a fresh start next time in the loop
            sci_no_response = false; // same as above
        }
*/
    }

    if (pcm_map_volts_read && !sci_highspeed_enabled)
    {
        uint8_t map_volts_data[3];
        
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        
        // Put first byte
        if (!sci_timeout_reached) uart1_putc(0x14);
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_timeout_reached )
        { 
            /* wait for echo */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
        }
        if (!sci_timeout_reached) // save the byte if it's received before timeout
        {
            map_volts_data[0] = uart1_getc() & 0xFF;
        }
        
        // Put second byte
        if (!sci_timeout_reached) uart1_putc(0x40);
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_timeout_reached )
        {
            /* wait for echo */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
        }
        if (!sci_timeout_reached) // save the byte if it's received before timeout
        {
            map_volts_data[1] = uart1_getc() & 0xFF;
        }

        // Wait for PCM to return MAP sensor data
        //sci_putc(0xAB); // debug (send dummy byte as response)
        sci_timeout_start = millis_get();
        while ( (uart1_available() == 0) && !sci_no_response )
        {
            /* wait for response */
            if (millis_get() - sci_timeout_start > sci_timeout) sci_no_response = true;
        }
        if (!sci_timeout_reached && !sci_no_response) // save the byte if it's received before timeout
        {
            map_volts_data[2] = uart1_getc() & 0xFF;
        }
        
        //digitalWrite(DATAIN_LED, LOW);  // turn off led

        _delay_ms(40);
/*
        // Send back 3 bytes (2 command + 1 data) if timeout was never reached
        if (!sci_timeout_reached && !sci_no_response) send_packet(dc_from_sci_bus, sdc_pcm_map_volts_data, map_volts_data, 3);
        else // if at least 1 timeout has been reached then abort operation and send back "no sci echo" packet
        {
            if (sci_timeout_reached) send_packet(dc_from_scanner, sdc_no_sci_echo, (uint8_t*)empty_payload, 0);
            if (sci_no_response) send_packet(dc_from_scanner, sdc_no_sci_response, (uint8_t*)empty_payload, 0);
            sci_timeout_reached = false; // reset timeout flag so sub-function can have a fresh start next time in the loop
            sci_no_response = false; // same as above
        }
*/
    }

    if (pcm_read_all_parameters && !sci_highspeed_enabled)
    {
        uint8_t pcm_parameters_data[80];
        
        for (uint16_t i = 0; i < 0x50; i++)
        {
            bool repeat = true; // initial condition is to repeat the while loop at least once
            while (repeat)
            {
                if ((i == 0x00) ||
                    (i == 0x03) ||
                    (i == 0x04) ||
                    (i == 0x0D) ||
                    (i == 0x14) ||
                    (i == 0x16) ||
                    (i == 0x17) ||
                    (i == 0x18) ||
                    (i == 0x19) ||
                    (i == 0x1A) ||
                    (i == 0x25) ||
                    (i == 0x26) ||
                    (i == 0x28) ||
                    (i == 0x2B) ||
                    (i == 0x2E) ||
                    (i == 0x36) ||
                    (i == 0x37) ||
                    (i == 0x38) ||
                    (i == 0x39) ||
                    (i == 0x3A) ||
                    (i == 0x3C) ||
                    (i == 0x3D) ||
                    (i == 0x43) ||
                    (i == 0x44) ||
                    (i == 0x48) ||
                    (i == 0x4A) ||
                    (i == 0x4B) ||
                    (i == 0x4C) ||
                    (i == 0x4D))
                {
                    pcm_parameters_data[i] = 0xFF;
                    i++; // 29 bytes out of 80 bytes are skipped in this loop
                }
                else // if the address is not in the condition list
                {
                    repeat = false; // break the while loop and continue by requesting the sensor data
                }
            }
            
            // Put first byte (command)
            uart1_putc(0x14);
            sci_timeout_start = millis_get();
            while ( (uart1_available() == 0) && !sci_timeout_reached )
            { 
                /* wait for echo */
                if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
            }
            if (sci_timeout_reached)
            {
                sci_timeout_reached = false;
            }
            uart1_getc(); // dummy get, this byte is not interesting
        
            // Put second byte (parameter to get)
            uart1_putc(i);
            sci_timeout_start = millis_get();
            while ( (uart1_available() == 0) && !sci_timeout_reached )
            {
                /* wait for echo */
                if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
            }
            if (sci_timeout_reached)
            {
                sci_timeout_reached = false;
            }
            uart1_getc(); // dummy get, this byte is not interesting

            // Wait for PCM to return sensor data
            //sci_putc(0xAB); // debug (send dummy byte as response)
            sci_timeout_start = millis_get();
            while ( (uart1_available() == 0) && !sci_timeout_reached )
            {
                /* wait for response */
                if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
            }
            if (sci_timeout_reached)
            {
                //sci_timeout_reached = false;
                pcm_parameters_data[i] = 0xFF; // save current iteration result as 0xFF is no response is detected
            }
            else
            {
                pcm_parameters_data[i] = uart1_getc() & 0xFF; // save current iteration result
            }
            sci_timeout_reached = false;
        }
        
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        // Send back complete packet
        //send_packet(dc_from_sci_bus, sdc_pcm_parameters_data, pcm_parameters_data, sizeof pcm_parameters_data);
        //digitalWrite(DATAIN_LED, LOW);  // turn off led
    }
    

    if ((uart1_available() > 0) || packet_waiting)
    {
        //digitalWrite(DATAIN_LED, HIGH);  // turn on led
        sci_timeout_start = millis_get();
        while ( !sci_timeout_reached )
        {
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;
            if ( !(uart1_peek() & UART_NO_DATA) ) 
            {
                sci_msg_array[sci_msg_array_ptr] = uart1_getc() & 0xFF;
                sci_msg_array_ptr++;
                sci_timeout_start = millis_get(); // restart timeout
                packet_waiting = false;
            }
        }
        sci_timeout_reached = false;

        // Assemble a CCDSCI Packet
        //send_packet(dc_from_sci_bus, 0x01, sci_msg_array, sci_msg_array_ptr);

        if (packet_waiting)
        {
            packet_waiting = false;
            //send_packet(dc_from_scanner, sdc_no_sci_response, (uint8_t*)empty_payload, 0);
        }

        // Reset pointer to zero...
        sci_msg_array_ptr = 0;
        
        //digitalWrite(DATAIN_LED, LOW); // turn off led
    }

/*
    // Try to grab a byte from the SCI receive buffer as frequently as possible! 
    uint16_t dummy9 = sci_getc();

    // If there is at least 1 byte present
    if ( !(dummy9 & SCI_RX_NO_DATA) )
    {
        digitalWrite(DATAIN_LED, HIGH);  // turn on led

        // Save the first byte that got us here
        sci_msg_array[sci_msg_array_ptr] = dummy9 & 0xFF;
        sci_msg_array_ptr++;
      
        // Wait for another bytes or exit when timeout reached
        sci_timeout_start = millis_get();
        while ( (sci_rx_available() > 0) && !sci_timeout_reached )
        { 
            // wait for RAM data or exit when timeout reached
            if (millis_get() - sci_timeout_start > sci_timeout) sci_timeout_reached = true;

            if (sci_rx_available() > 0)
            {
                for (uint8_t i = 0; i < sci_rx_available(); i++)
                {
                    sci_msg_array[sci_msg_array_ptr] = sci_getc() & 0xFF;
                    sci_msg_array_ptr++;
                }
            }
        }
        
        sci_timeout_reached = false;
  
        // Assemble a CCDSCI Packet
        send_packet(dc_from_sci_bus, 0x01, sci_msg_array, sci_msg_array_ptr);

        // Reset pointer to zero...
        sci_msg_array_ptr = 0;
        digitalWrite(DATAIN_LED, LOW); // turn off led

        // Put dummy bytes to test single command timeout
        sci_putc(0xAB);
        sci_putc(0xCD);
        sci_putc(0xEF);
    }
*/

} // end of handle_sci_rx_bytes, see you next time


