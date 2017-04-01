#ifndef CHRYSLERCCDSCIPROTOCOL_H
#define CHRYSLERCCDSCIPROTOCOL_H

#include <stdint.h>
#include <avr/io.h>
#include <stdbool.h>
#include "main.h"


// CCD-bus related variables
#define CCD_SOM		0x40	// UART flag for start of CCD-bus message
#define CCD_EOM		0x20	// UART flag for end of CCD-bus message
#define DIAG_REQ	0xB2	// Diagnostic request ID-byte for CCD-bus
#define DIAG_RESP	0xF2	// Diagnostic response ID-byte for CCD-bus


// SCI-bus related variables
// Baudrate prescaler calculation: UBRR = (F_CPU / (16 * BAUDRATE)) - 1
#define SCI_CCD_LO_SPEED	127  // prescaler for   7812.5 baud communication speed (CCD-SCI / default Low-Speed diagnostic command mode)
#define SCI_HI_SPEED		15   // prescaler for  62500   baud communication speed (SCI / High-Speed parameter interrogation command mode)
#define SCI_EH_SPEED		7    // prescaler for 125000   baud communication speed (SCI / Extra-High-Speed mode) - NOT used

#define SCI_INTERFRAME_RESPONSE_DELAY   100  // ms
#define SCI_INTERMESSAGE_RESPONSE_DELAY 50   // ms
#define SCI_INTERMESSAGE_REQUEST_DELAY  50   // ms

#define SCI_BUS_PORT	PORTD
#define SCI_BUS_PCM_PIN PORTD4
#define SCI_BUS_TCM_PIN PORTD5

#define NON 0x00
#define PCM 0x01
#define TCM 0x02

// Function prototypes
extern void start_clock_generator(void);
extern void stop_clock_generator(void);

#endif