#ifndef MAIN_H_
#define MAIN_H_

#include <avr/io.h>
#include <avr/wdt.h>


/******************************************************************************
                                     Masks                                     
******************************************************************************/

// Set (1), clear (0) and invert (1->0; 0->1) bit in a register or variable easily
#define sbi(port, bit) (port) |=  (1 << (bit))
#define cbi(port, bit) (port) &= ~(1 << (bit))
#define ibi(port, bit) (port) ^=  (1 << (bit))

#ifndef F_CPU
#define F_CPU 16000000UL // The ATmega2560 has a 16 MHz crystal external oscillator
#endif

// Buzzer
#define BUZZER_PORT		PORTD
#define BUZZER_PIN		PORTD6

// Activity LED (blue)
#define ACT_LED_PORT	PORTD
#define ACT_LED_PIN		PORTD7

// LCD backlight control
#define LCD_BGLIGHT_PORT PORTE
#define LCD_BGLIGHT_PIN  PORTE3


/******************************************************************************
                           Externally Used Variables                           
******************************************************************************/
extern volatile uint32_t ccd_messages_count;
extern volatile uint16_t sci_bus_tasks;
extern volatile uint16_t ccd_bus_tasks;
extern volatile bool ccd_idle;
extern volatile bool ccd_active_byte;
extern uint16_t lcd_tasks;
extern uint16_t button_tasks;


/******************************************************************************
                              Function Prototypes                              
******************************************************************************/
extern void init_lcd(void);
extern void init_ccd_bus(void);
extern void init_sci_bus(void);
extern void read_avr_signature(void);
extern void avr8_software_reset(void);
extern void wdt_init(void) __attribute__((naked)) __attribute__((section(".init3")));
extern void check_commands(void);
extern uint16_t free_ram(void);
extern void select_sci_bus_target(uint8_t bus);


#endif /* MAIN_H_ */