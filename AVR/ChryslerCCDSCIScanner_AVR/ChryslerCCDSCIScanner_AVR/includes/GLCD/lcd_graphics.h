//*****************************************************************************
//
// Work File    : lcd_graphics.h
// Description	: Several basic functions for LCD-displays  
//
// Author       : Sami Varjo 
// Created      : 2008-06-08
// Revised      : 2008-07-06
// Version      : 0.5
// Uses         : avr-libc + suitable lowlevel driver eg lcd_driver_S6B0724.h or
//		  lcd_driver_SED1520.h or lcd_driver_KS0107.h
//
// This code is distributed under the GNU Public License
// which can be found at http://www.gnu.org/licenses/gpl.txt
//
// Change history:  
//       ver 0,05: added LCD_drawGlyph()
//       ver 0.04: added LCD_drawFullBMP
//       ver 0.03: Changed shape functions to general ones using func pointers
//       ver 0.02: Added several functions
//       ver 0.01: Project started - basic definitions
//
//
//*****************************************************************************

#ifndef LCD_GRAPHICS
#define LCD_GRAPHICS

#ifndef uint8_t
#include <inttypes.h>
#endif

//Graphics functions
void LCD_drawLine (uint8_t x0,uint8_t y0,uint8_t x1,uint8_t y1);
void LCD_clearLine(uint8_t x0,uint8_t y0,uint8_t x1,uint8_t y1);
void LCD_invertLine(uint8_t x0,uint8_t y0,uint8_t x1,uint8_t y1);

void LCD_drawCirc  (uint8_t x1, uint8_t y1, uint8_t radius);
void LCD_fillCirc  (uint8_t x1, uint8_t y1, uint8_t radius);
void LCD_clearCirc (uint8_t x1, uint8_t y1, uint8_t radius);
void LCD_invertCirc(uint8_t x1, uint8_t y1, uint8_t radius);

void LCD_drawFullBMP(uint8_t* bitmap);
void LCD_drawBMP(uint8_t* bitmap, uint8_t x, uint8_t y, uint8_t width, uint8_t height);
void LCD_putgchar(uint8_t c, uint8_t* font);
void LCD_putgstr(uint8_t* string, uint8_t* font, uint8_t x, uint8_t y);

void LCD_writeByteXY(uint8_t data, uint8_t x, uint8_t y);
void LCD_copyPixelTo(uint8_t* source, uint8_t swidth, uint8_t sx, uint8_t sy, uint8_t tx, uint8_t ty);
void LCD_copyPageTo (uint8_t* source, uint8_t swidth, uint8_t spage,
		     uint8_t sx1, uint8_t sx2,uint8_t tx1, uint8_t ty);

void LCD_drawRect  (uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2);
void LCD_clearRect (uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2);
void LCD_fillRect  (uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2);

//helper functions:
uint8_t _LCD_absDiff(uint8_t a,uint8_t b);
void _LCD_doRect(uint8_t x1,uint8_t y1,uint8_t x2,uint8_t y2,
		 void (*pPixFun)(uint8_t, uint8_t ),
		 void (*pPageFun)(uint8_t, uint8_t, uint8_t ) );
void _LCD_doLine (uint8_t x0,uint8_t y0,uint8_t x1,uint8_t y1,
		  void (*pPixFun)(uint8_t, uint8_t ) );
void _LCD_doCirc(uint8_t x1, uint8_t y1, uint8_t radius,
		 void (*pLineFun)(uint8_t,uint8_t,uint8_t,uint8_t) );

#endif //LCD_GRAPHICS
