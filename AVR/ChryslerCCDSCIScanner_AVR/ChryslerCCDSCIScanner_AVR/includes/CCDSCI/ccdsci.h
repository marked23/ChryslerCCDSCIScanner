#ifndef CHRYSLERCCDSCIPROTOCOL_H
#define CHRYSLERCCDSCIPROTOCOL_H

#include <stdint.h>
#include <avr/io.h>
#include <stdbool.h>
#include "main.h"

#define SCI_INTERFRAME_RESPONSE_DELAY   100  // ms
#define SCI_INTERMESSAGE_RESPONSE_DELAY 50   // ms
#define SCI_INTERMESSAGE_REQUEST_DELAY  50   // ms
#define SCI_HI_BAUD 62500
#define SCI_LO_BAUD 7812.5

// SCI-bus related variables
// Baudrate prescaler calculation: UBRR = (F_CPU / (16 * BAUDRATE)) - 1
#define SCI_CCD_LO_SPEED	127  // prescaler for   7812.5 baud communication speed (CCD-SCI / default Low-Speed diagnostic command mode)
#define SCI_HI_SPEED		15   // prescaler for  62500   baud communication speed (SCI / High-Speed parameter interrogation command mode)
#define SCI_EH_SPEED		7    // prescaler for 125000   baud communication speed (SCI / Extra-High-Speed mode)

extern bool ccd_enabled;
extern bool sci_enabled;
//extern volatile bool ccd_idle;
//extern volatile bool ccd_active_byte;
//extern uint32_t sci_timeout_start;
//extern bool sci_timeout_reached;
//extern uint8_t sci_timeout;

// Function prototypes
extern void ccd_eom(void);
extern void start_clock_generator(void);
extern void stop_clock_generator(void);

#endif