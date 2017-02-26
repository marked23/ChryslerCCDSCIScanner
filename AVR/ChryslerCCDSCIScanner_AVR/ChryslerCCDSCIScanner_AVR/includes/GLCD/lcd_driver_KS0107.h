//*****************************************************************************
//
// Work File 	: lcd_driver_KS0107.h
// Description	: Driver for KS0107 driven LCD display.
//
// Author       : Sami Varjo
// Created      : 2008-06-23
// Revised      : 2008-06-29
// Version      : 0.03
// Target MCU	: Atmel AVR - ATMEGA 5815 (or equivalent)
// Uses         : avr-libc
//
// This code is distributed under the GNU Public License
// which can be found at http://www.gnu.org/licenses/gpl.txt
//
// Change history:  
//       ver 0.03: Cleaned up
//       ver 0.02: Added several functions
//       ver 0.01: Project started - basic definitions
//
//*****************************************************************************

#ifndef LCD_DRIVER_KS0107
#define LCD_DRIVER_KS0107

#include <inttypes.h>
#include <avr/io.h>

//MCU definitions
#ifndef F_CPU
//#define F_CPU 1000000UL //1M CPU speed (for <util/delay.h>) 
#define F_CPU  7372800UL //Atmega5815  7.3728MHz
//#define F_CPU 14745600UL //Atmega8    14.7456MHz
#endif

//MCU connection definitions  (bits for pins and ports)
//Control port pins
#define LCD_CONTROLPORT   PORTA   //port to send control signals (PC1-PC7) 
#define LCD_CPDIRECTION   DDRA    //direction register for port to LCD control
                                  //input 0, output 0b11111111 (0xff)
#define LCD_CS1  (1<<PIN1) // Chip select 1
#define LCD_CS2  (1<<PIN2) // Chip select 2
#define LCD_RST  (1<<PIN5) // RESET bit - IF 0 IT STARTS INITIALIZATION
#define LCD_RS   (1<<PIN3) //MCU pin to LCD RS  0 == command  | 1 == DDRAM data 
#define LCD_RW   (1<<PIN4) //MCU pin to LCD R/W 0 == write to 0|1 == read from  
#define LCD_E    (1<<PIN0) //MCU pin to LCD E   1 == Enable operation - execute

//Data port definitions (actual commands and data)
#define LCD_DATAPORT		PORTC // MCU port connected to LCD data pins
#define LCD_DATA_IN			PINC  // Place to read data in
#define LCD_DPDIRECTION		DDRC  // direction reg for data/command port data 1=out 0=in (A0)
#define LCD_D0 (1<<PIN0) // MCU pin to LCD  : data bit 0
#define LCD_D1 (1<<PIN1) // MCU pin to LCD  : data bit 1
#define LCD_D2 (1<<PIN2) // MCU pin to LCD  : data bit 2
#define LCD_D3 (1<<PIN3) // MCU pin to LCD  : data bit 3
#define LCD_D4 (1<<PIN4) // MCU pin to LCD  : data bit 4
#define LCD_D5 (1<<PIN5) // MCU pin to LCD  : data bit 5
#define LCD_D6 (1<<PIN6) // MCU pin to LCD  : data bit 6
#define LCD_D7 (1<<PIN7) // MCU pin to LCD  : data bit 7

//LCD spec definitions
#define LCD_PAGE_HEIGHT 8   //8 lines per page
#define LCD_LINES       64
#define LCD_LINE_LENGTH 128
#define LCD_NUM_CHIPS   2

#define FALSE   0x00
#define TRUE    0x01

#define LEFT  0
#define RIGHT 1

//LCD binary instructions
#define SET_DISPLAY_OFF             0b00111110       
#define SET_DISPLAY_ON              0b00111111
#define SET_COLUMN_ADDRESS(col)     0b01000000|col  //0-63
#define SET_INIT_DISPLAY_LINE(line) 0b11000000|line //0-63
#define SET_PAGE_ADDRESS(page)      0b10111000|page //0-7

//Function declarations for low level operations
void    LCD_sendCommand(uint8_t command);
void    LCD_writeByte(uint8_t data);
uint8_t LCD_readByte(void);
uint8_t LCD_readStatus(void);
void    LCD_selectChip(void);
void    LCD_selectLeftChip(void);
void    LCD_selectRightChip(void);

//API function declarations
void LCD_init(void);   //do reset and int LCD
void LCD_on(void);     //turn on  LCD (not backlight)
void LCD_off(void);    //turn off LCD (not backlight)

void LCD_clr(void);
void LCD_allPagesOn(uint8_t byte);

void LCD_setCursorXY(uint8_t x, uint8_t y);
void LCD_pixelOn(uint8_t x, uint8_t y);
void LCD_pixelOff(uint8_t x, uint8_t y);
void LCD_invertPixel(uint8_t x, uint8_t y);

void LCD_invertPage(uint8_t page, uint8_t x1, uint8_t x2);
void LCD_onPage(uint8_t page,uint8_t x1,uint8_t x2);
void LCD_offPage(uint8_t page,uint8_t x1,uint8_t x2);

void LCD_putchar(uint8_t c);
void LCD_puts(uint8_t* string);
void LCD_putsp(uint8_t* string, uint8_t page, uint8_t x);

uint8_t LCD_isBusy(void);
uint8_t LCD_isOff(void);
uint8_t LCD_isReseting(void);

//Functions to set RAM addresses for read and write operations
void LCD_setPageAddress(uint8_t page);
void LCD_setColumnAddress(uint8_t col);
void LCD_setInitialLineAddress(uint8_t line);

//help functions
void LCD_wait_execution(void);
void LCD_wait_busy(void);
void LCD_selectSide(uint8_t side);
uint8_t _LCD_readByte(void);

#endif //Define LCD_DRIVER

