//*****************************************************************************
//
// Work File		: lcd_driver_KS0107.c
// Description		: Driver for KS0107 driven LCD display
//            		  Implementations for lcd_driver_KS0107.h
//
// Author		: Sami Varjo
// Created		: 2008-23-06
// Revised		: 2008-29-06
// Version		: 0.04
// Target MCU	        : Atmel AVR series - ATMEGA5815 (or equivalent)
// Uses                 : avr-libc lcd_driver_KS0107.h font5x7.h
//
// This code is distributed under the GNU Public License
// which can be found at http://www.gnu.org/licenses/gpl.txt
//
// Change history:  
//       ver 0.04: Fixed LCD_readStatus() + changed LCD_allPixelsOn() to 
//                 LCD_allPagesOn(byte) + added starting x to LCD_putsp()
//       ver 0.03: Bugs fixed
//       ver 0.02: Added several functions
//       ver 0.01: Project started - basic definition
//
//*****************************************************************************

#include "lcd_driver_KS0107.h"
#include <util/delay.h>
#include <avr/pgmspace.h>
#include <avr/io.h>
#include "font5x7.h"

//------------------------------------------------------------------------------
//Global variables used by driver
uint8_t LCD_currentX, LCD_currentY;

//------------------------------------------------------------------------------
//Send a binary command to LCD 
// Target is currently selected controller chip
void LCD_sendCommand(uint8_t command)
{
  LCD_CPDIRECTION  = 0xff;     //send data out
  LCD_DATAPORT     = command;  //load command to port

  LCD_CONTROLPORT |= LCD_E;    //execute  (LCD_RS and LCD_RW by default low)
  LCD_wait_execution();            
  LCD_CONTROLPORT &= ~LCD_E;   //stop execition  
  LCD_wait_execution();
}

//------------------------------------------------------------------------------
//Write a byte to LCD.  Single 8 bit segment is send to current cursor position
//
void LCD_writeByte(uint8_t data)
{
  LCD_setCursorXY(LCD_currentX,LCD_currentY); //address and chip selected 

  LCD_DPDIRECTION  = 0xff;     //send data out 
  LCD_DATAPORT     = data;     //load data to port
  LCD_CONTROLPORT |= (LCD_RS | LCD_E); 
  LCD_wait_execution();            

  LCD_CONTROLPORT &= ~LCD_E;
  LCD_CONTROLPORT &= ~LCD_RS;  //stop and go back to command mode
  LCD_wait_execution();            

  LCD_currentX++;
}

//-----------------------------------------------------------------------------
//Read a byte from LCD DDRAM (using modify_read mode)
//
uint8_t _LCD_readByte(void)
{
  uint8_t data;
  LCD_setCursorXY(LCD_currentX,LCD_currentY);    //address and chip selected
  LCD_DPDIRECTION &= 0x00;                       //set read data from port

  LCD_CONTROLPORT |= ( LCD_E | LCD_RS | LCD_RW); //select read data mode and execute
  LCD_wait_execution();
  data=LCD_DATA_IN;
  LCD_CONTROLPORT &= ~( LCD_E | LCD_RS | LCD_RW); //select read data mode and execute
  LCD_wait_execution();

  LCD_DPDIRECTION |= 0xff;                    
  LCD_setCursorXY(LCD_currentX,LCD_currentY);   //move back to where was red

  return data;
}

uint8_t LCD_readByte(){
  _LCD_readByte();       //dummy read needed
  return _LCD_readByte();
}

//------------------------------------------------------------------------------
//Functions to inquiry the status of S6B0724
uint8_t LCD_readStatus(void)
{
  uint8_t status;
  LCD_selectChip();
  LCD_DPDIRECTION  =  0x00;     //read data from port
  LCD_CONTROLPORT &= ~LCD_RS; //=0
  LCD_CONTROLPORT |= (LCD_RW | LCD_E); //=1
  LCD_wait_execution();
  status=LCD_DATA_IN;
  LCD_DPDIRECTION |= 0xff;                    
  LCD_CONTROLPORT &= ~(LCD_E | LCD_RW);  

  return status;
}

//------------------------------------------------------------------------------
//Initialize LCD 
//
void LCD_init(void)
{ 
  LCD_CPDIRECTION |=0xff;      // controlport pins for output (needed for reset)
  LCD_DPDIRECTION |=0xff;      // dataport pins for output    

  LCD_CONTROLPORT &= ~LCD_RST; //0 -reset the chip
  _delay_ms(20);               //  -wait for reset
  LCD_CONTROLPORT |= LCD_RST;  //1 
  
  while(LCD_isReseting()) LCD_wait_execution();  //This might be omitted 
  //  _delay_ms(50);           //  -wait for reset 

  LCD_on();
  LCD_clr();                   //clear LCD memory
  LCD_setCursorXY(0,0);        //set cursor to "home"

}


//------------------------------------------------------------------------------
//clear LCD DDRAM 
void LCD_clr(void)
{
    LCD_allPagesOn(0x00);
}
//-----------------------------------------------------------------------
//fill all pages with selected byte
void LCD_allPagesOn(uint8_t byte)
{
  uint8_t i,j;
  for (i=0;i<LCD_LINES/LCD_PAGE_HEIGHT;i++){
    LCD_setCursorXY(0,i*LCD_PAGE_HEIGHT);
    for(j=0;j<LCD_LINE_LENGTH;j++){
      LCD_writeByte(byte);
    }
  }
}

//------------------------------------------------------------------------------
//Move cursor to position x y
void LCD_setCursorXY(uint8_t x, uint8_t y)
{
  LCD_currentX=x;
  LCD_currentY=y;
  LCD_selectChip();
  LCD_setColumnAddress(x);
  LCD_setPageAddress(y/LCD_PAGE_HEIGHT); 
}

//------------------------------------------------------------------------------
//Draw pixel at x y
void LCD_pixelOn(uint8_t x, uint8_t y)
{  
  uint8_t mask=0x00; 
  LCD_setCursorXY(x,y);  
  mask=LCD_readByte();
  LCD_writeByte( mask | (1<< (y&0b111))); //shift to selected bit on page
}
//------------------------------------------------------------------------------
//Dim pixel at x y
void LCD_pixelOff(uint8_t x, uint8_t y)
{
  uint8_t mask;
  LCD_setCursorXY(x,y);
  mask=LCD_readByte();
  LCD_writeByte(mask & ~(1<<(y&0b111)));  //select bit on page to operate on
}
//------------------------------------------------------------------------------
//Invert pixel value at x y
void LCD_invertPixel(uint8_t x, uint8_t y){
  uint8_t mask;
  LCD_setCursorXY(x,y);
  mask=LCD_readByte();
  if (mask &(1<<(y&0b111))) mask &= ~(1<<(y&0b111));
  else  mask |= (1<<(y&0b111));
  LCD_writeByte(mask);
}
//------------------------------------------------------------------------------
//invert pixels on a page from x1 to x2
// x1 must be smaller than x2
void LCD_invertPage(uint8_t page, uint8_t x1, uint8_t x2)
{
  uint8_t mask,i;
  LCD_setCursorXY(x1,page*LCD_PAGE_HEIGHT);
  for (i=x1;i<=x2;i++){
    mask=LCD_readByte();
    LCD_writeByte(~mask);
  }
}

//------------------------------------------------------------------------------
//clear a single page in LCD DDRAM from x1 to x2
void LCD_offPage(uint8_t p,uint8_t x1, uint8_t x2){
  uint8_t i;
  LCD_setCursorXY(x1,p*LCD_PAGE_HEIGHT);
  for(i=x1;i<x2;i++){
      LCD_writeByte(0x00);
    }
}
//------------------------------------------------------------------------------
//Turn pixels on in a single page in LCD DDRAM from x1 to x2
void LCD_onPage(uint8_t p,uint8_t x1, uint8_t x2){
  uint8_t i;
  LCD_setCursorXY(x1,p*LCD_PAGE_HEIGHT);
  for(i=x1;i<=x2;i++){
      LCD_writeByte(0xff);
    }
}

//------------------------------------------------------------------------------
//Put a single char to LCD on line at current cursor position
// Note that page is used not exact coordinates 
void LCD_putchar(uint8_t c){
  uint8_t i = 0;  
  for(i=0; i<5; i++){         
    if (LCD_currentX>=LCD_LINE_LENGTH){
      if (LCD_currentY<LCD_LINES+LCD_PAGE_HEIGHT){
	LCD_setCursorXY(0,LCD_currentY+LCD_PAGE_HEIGHT);
	}
      else {
	LCD_setCursorXY(0,0);	  
      }
    }
    LCD_writeByte(pgm_read_byte(&Font5x7[(c - 0x20)*5 + i])); 
  }    
  LCD_writeByte(0x00);  
}

//---------------------------------------------------------------------------------
//Put a string on display - auto for next line is default (from LCD_putchar)
//null terminated strings are expected
// /n  for new line 
void LCD_puts(uint8_t* pString)
{    	
  uint8_t i=0;
  while (pString[i] != '\0'){
    if (pString[i]=='\n'){           
      LCD_currentX=0;
      LCD_currentY+=LCD_PAGE_HEIGHT;    
    }
    else {
      LCD_putchar(pString[i]);
    }
    i++;      
  }
}

//-----------------------------------------------------------------------------
//Put a string on selected page (line) starting at x
//
void LCD_putsp(uint8_t* pString, uint8_t page, uint8_t x)
{
  LCD_currentX=x;
  LCD_currentY=page*LCD_PAGE_HEIGHT;
  LCD_selectChip();
  LCD_setColumnAddress(x);
  LCD_setPageAddress(page); 
  LCD_puts(pString);
}

//-----------------------------------------------------------------------------
uint8_t LCD_isBusy(void)
{
  uint8_t status=LCD_readStatus();
  if (status & LCD_D7) return TRUE;
  else return FALSE; 
}
//-----------------------------------------------------------------------------
uint8_t LCD_isOff(void)
{
  uint8_t status=LCD_readStatus();
  if (status & LCD_D5) return TRUE;
  else return FALSE;
}
//-----------------------------------------------------------------------------
uint8_t LCD_isReseting(void)    //by RESETB signal
{
  uint8_t status=LCD_readStatus();
  if (status & LCD_D4) return TRUE;
  else return FALSE;
}

//------------------------------------------------------------------------------
//Functions to set display DDRAM page addresses
void LCD_setPageAddress(uint8_t page) 
{
  LCD_sendCommand(SET_PAGE_ADDRESS(page));
}
//------------------------------------------------------------------------------
//Select x-coordinate in  LCD memory 
//
void LCD_setColumnAddress(uint8_t col)
{  
  if (col<LCD_LINE_LENGTH/LCD_NUM_CHIPS){ 
    LCD_selectLeftChip();
    LCD_sendCommand(SET_COLUMN_ADDRESS(col));
  }
  else {
    LCD_selectRightChip();
    LCD_sendCommand(SET_COLUMN_ADDRESS( (col - (LCD_LINE_LENGTH/LCD_NUM_CHIPS)) ));
  }	
}

//------------------------------------------------------------------------------
//Select display memory line for current chip 
//can be used to scroll data up and down (LCD memory is looped)
//
void LCD_setInitialLineAddress(uint8_t line){
  LCD_sendCommand(SET_INIT_DISPLAY_LINE(line));
}

//------------------------------------------------------------------------------
//function to be used with low level commands
// be sure to have long enough waittime for your target MCU
void LCD_wait_execution(void){ 
  asm("nop"); asm("nop"); 
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
  //asm("nop"); asm("nop"); asm("nop"); asm("nop");
} 
  
//------------------------------------------------------------------------------
//function to wait for LCD_operations (wait_execution() is faster!)
void LCD_wait_busy(void){ while(LCD_isBusy()) LCD_wait_execution(); }

//------------------------------------------------------------------------------
//Select the right controller chip depending on LCD_currentX value 
//
void LCD_selectChip(void){
  if (LCD_currentX<LCD_LINE_LENGTH/LCD_NUM_CHIPS){
    LCD_selectLeftChip();
  }
  else {
    LCD_selectRightChip();
  }
} 
//------------------------------------------------------------------------------
//Enable left controller && disable right controller
void LCD_selectLeftChip(void){   
    LCD_CONTROLPORT &= ~LCD_CS2; 
    LCD_CONTROLPORT |=  LCD_CS1;   
}
//------------------------------------------------------------------------------
//Enable right controller && disable left controller
void LCD_selectRightChip(void){
    LCD_CONTROLPORT &= ~LCD_CS1; 
    LCD_CONTROLPORT |=  LCD_CS2; 
}

//------------------------------------------------------------------------------
//Turn display on 
void LCD_on(void)
{
  LCD_selectLeftChip();        
  LCD_sendCommand(SET_DISPLAY_ON);
  LCD_selectRightChip();
  LCD_sendCommand(SET_DISPLAY_ON);
}

//------------------------------------------------------------------------------
//Turn display off
void LCD_off(void)
{
  LCD_selectLeftChip();        //turn whole display on
  LCD_sendCommand(SET_DISPLAY_OFF);
  LCD_selectRightChip();
  LCD_sendCommand(SET_DISPLAY_OFF);
}

 
