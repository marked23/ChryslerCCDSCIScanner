#include <avr/io.h>
#include <stdbool.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include "packet.h"
#include "main.h"
#include "millis.h"

bool	handshake_requested = false;
//const uint8_t handshake_array[21] = { 0x43, 0x48, 0x52, 0x59, 0x53, 0x4C, 0x45, 0x52, 0x43, 0x43, 0x44, 0x53, 0x43, 0x49, 0x53, 0x43, 0x41, 0x4E, 0x4E, 0x45, 0x52 };




/*************************************************************************
Function: send_packet()
Purpose:  assemble and send data packet through serial link
Inputs:   - one source byte,
          - one target byte,
		  - one datacode value byte, these three are used to calculate the DATA CODE byte
          - one SUB-DATA CODE byte,
          - pointer to the PAYLOAD bytes array (name of the array),
            (it must be previously filled with data)
          - PAYLOAD length
Returns:  0 if OK
          1 if ERROR
Note:     SYNC, LENGTH and CHECKSUM bytes are calculated automatically
**************************************************************************/
uint8_t send_packet(uint8_t source, uint8_t target, uint8_t dc_command, uint8_t subdatacode, uint8_t *payloadbuff, uint16_t payloadbufflen)
{
    // Calculate the length of the packet
    // PAYLOAD length + 1 SYNC byte + 2 LENGTH bytes + 1 DATA CODE byte
    // + 1 SUB-DATA CODE byte + 1 CHECKSUM byte
    uint16_t temp_packet_length = payloadbufflen + 6;
	uint8_t  temp_packet[temp_packet_length];
	bool payload_bytes = false;
	uint8_t calculated_checksum = 0;

	if (payloadbufflen == 0) payload_bytes = false;
	else payload_bytes = true;

	uint8_t datacode = 0;
	datacode += (source << 6);
	datacode += (target << 4);
	datacode += dc_command;
    
    temp_packet[0] = SYNC_BYTE; // Add SYNC byte (0x33)
    temp_packet[1] = ((payloadbufflen + 2) >> 8) & 0xFF; // Add LENGTH high byte
    temp_packet[2] =  (payloadbufflen + 2) & 0xFF;       // Add LENGTH low byte
    temp_packet[3] = datacode;    // Add DATA CODE byte
    temp_packet[4] = subdatacode; // Add SUB-DATA CODE byte
    
    // If there are payload bytes add them too
    if (payload_bytes)
    {
        for (uint16_t i = 0; i < payloadbufflen; i++)
        {
            temp_packet[5 + i] = payloadbuff[i]; // Add message bytes to the PAYLOAD bytes
        }
    }

    // Calculate checksum, skip SYNC byte by starting at index 1.
    for (uint16_t j = 1; j < temp_packet_length - 1; j++)
	{
		calculated_checksum += temp_packet[j];
	}

    // Place checksum byte
    temp_packet[temp_packet_length - 1] = calculated_checksum & 0xFF;

    // Send the packet through serial link
    for (uint16_t k = 0; k < temp_packet_length; k++)
    {
        uart2_putc(temp_packet[k]); // Write every byte in the packet to the serial port
    }
                     
    return 0; // return zero as success
    
} /* send_packet */