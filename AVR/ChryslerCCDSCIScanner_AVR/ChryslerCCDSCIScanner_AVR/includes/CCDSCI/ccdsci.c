#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#include <util/delay.h>
#include <inttypes.h>
#include <stdbool.h>
#include "ccdsci.h"
#include "uart.h"


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

