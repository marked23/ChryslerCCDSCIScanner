/*-----------------------------------------------------------------------------*
 * ext_eeprom.h - Arduino library to support external I2C EEPROMs.             *
 *                                                                             *
 * This library will work with most I2C serial EEPROM chips between 2k bits    *
 * and 2048k bits (2M bits) in size. Multiple EEPROMs on the bus are supported *
 * as a single address space. I/O across block, page and device boundaries     *
 * is supported. Certain assumptions are made regarding the EEPROM             *
 * device addressing. These assumptions should be true for most EEPROMs        *
 * but there are exceptions, so read the datasheet and know your hardware.     *
 *                                                                             *
 * The library should also work for EEPROMs smaller than 2k bits, assuming     *
 * that there is only one EEPROM on the bus and also that the user is careful  *
 * to not exceed the maximum address for the EEPROM.                           *
 *                                                                             *
 * Library tested with:                                                        *
 *   Microchip 24AA02E48 (2k bit)                                              *
 *   24xx32 (32k bit, thanks to Richard M)                                     *
 *   Microchip 24LC256 (256k bit)                                              *
 *   Microchip 24FC1026 (1M bit, thanks to Gabriele B on the Arduino forum)    *
 *   ST Micro M24M02 (2M bit)                                                  *
 *                                                                             *
 * Library will NOT work with Microchip 24xx1025 as its control byte does not  *
 * conform to the following assumptions.                                       *
 *                                                                             *
 * Device addressing assumptions:                                              *
 * 1. The I2C address sequence consists of a control byte followed by one      *
 *    address byte (for EEPROMs <= 16k bits) or two address bytes (for         *
 *    EEPROMs > 16k bits).                                                     *
 * 2. The three least-significant bits in the control byte (excluding the R/W  *
 *    bit) comprise the three most-significant bits for the entire address     *
 *    space, i.e. all chips on the bus. As such, these may be chip-select      *
 *    bits or block-select bits (for individual chips that have an internal    *
 *    block organization), or a combination of both (in which case the         *
 *    block-select bits must be of lesser significance than the chip-select    *
 *    bits).                                                                   *
 * 3. Regardless of the number of bits needed to address the entire address    *
 *    space, the three most-significant bits always go in the control byte.    *
 *    Depending on EEPROM device size, this may result in one or more of the   *
 *    most significant bits in the I2C address bytes being unused (or "don't   *
 *    care").                                                                  *
 * 4. An EEPROM contains an integral number of pages.                          *
 *                                                                             *
 * To use the extEEPROM library, the Arduino Wire library must also            *
 * be included.                                                                *
 *                                                                             *
 * Jack Christensen 23Mar2013 v1                                               *
 * 29Mar2013 v2 - Updated to span page boundaries (and therefore also          *
 * device boundaries, assuming an integral number of pages per device)         *
 * 08Jul2014 v3 - Generalized for 2kb - 2Mb EEPROMs.                           *
 *                                                                             *
 * External EEPROM Library by Jack Christensen is licensed under CC BY-SA 4.0, *
 * http://creativecommons.org/licenses/by-sa/4.0/                              *
 *-----------------------------------------------------------------------------*/

#ifndef EXTEEPROM_H_
#define EXTEEPROM_H_

#define BUFFER_LENGTH 32

#include <inttypes.h>
#include <stdint-gcc.h>

//EEPROM size in kilobits. EEPROM part numbers are usually designated in k-bits.

#define kbits_2     2
#define kbits_4     4
#define kbits_8     8
#define kbits_16    16
#define kbits_32    32
#define kbits_64    64
#define kbits_128   128
#define kbits_256   256
#define kbits_512   512
#define kbits_1024  1024
#define kbits_2048  2048

#define twiClock100kHz  100000
#define twiClock400kHz  400000

//EEPROM addressing error, returned by write() or read() if upper address bound is exceeded
#define EEPROM_ADDR_ERR  9

/** \struct     LC512
 *  \brief      Used to define a 24LC512 device on the I2C bus
 *  \details    LC512 type is used to hold the device address and the data to 
 *              be written to the device (or read from the device)
 */
typedef struct
{
	uint8_t address;	//	the address of the device
	uint16_t data;		//	data to/from device
} _24LC512;

extern void    exteeprom_init(uint16_t deviceCapacity, uint8_t nDevice, uint16_t pageSize, uint8_t eepromAddr);
//extern uint8_t extEEPROM_begin(uint32_t twiFreq);
extern uint8_t exteeprom_begin();
extern uint8_t exteeprom_write_bytes(uint32_t addr, uint8_t *values, uint16_t nBytes);
extern uint8_t exteeprom_write(uint32_t addr, uint8_t value);
extern uint8_t exteeprom_read_bytes(uint32_t addr, uint8_t *values, uint16_t nBytes);
extern int16_t exteeprom_read(uint32_t addr);

uint8_t  _eepromAddr;    //eeprom i2c address
uint16_t _dvcCapacity;   //capacity of one EEPROM device, in kbits
uint8_t  _nDevice;       //number of devices on the bus
uint16_t _pageSize;      //page size in bytes
uint8_t  _csShift;       //number of bits to shift address for chip select bits in control byte
uint16_t _nAddrBytes;    //number of address bytes (1 or 2)
uint32_t _totalCapacity; //capacity of all EEPROM devices on the bus, in bytes

#endif /* EXTEEPROM_H_ */
