/*
 * mcp23017.c
 *
 * Created: 12/23/2011 5:40:33 PM
 *  Author: Owner
 */ 

#include "mcp23017.h"
#include "i2c_master.h"
#include <util/delay.h>

#define MCP23017_BASE_ADDRESS 0x20 // default address when all three address pins are grounded on the chip

void mcp23017_init(MCP23017 *obj, uint8_t address)
{
	obj->address = MCP23017_BASE_ADDRESS + address;
	
	mcp23017_write_register(obj, MCP23017_IOCON, 0x00); // Set addressing style
	
	obj->data = 0xFFFF;
	mcp23017_write_word(obj, MCP23017_IODIRA);	// Set all pins to input

	obj->data = 0xFFFF;
	mcp23017_write_word(obj, MCP23017_GPPUA);	// Set 100k pullup on all pins

	//obj->data = 0xFFFF;
	//mcp23017_write_word(obj, MCP23017_IPOLA);	// Invert polarity of button signal, if taken to ground, GPIO shows logical 1

	obj->data = 0x6060;
	mcp23017_write_word(obj, MCP23017_IOCON);   // Configure the interrupt system (mirror INTA/B, INTA/B not floating, interrupt signaled with LOW) 

	//obj->data = 0xFFFF;
	//mcp23017_write_word(obj, MCP23017_INTCONA); // Compare interrupt to the default value
	
	obj->data = 0xFFFF;
	mcp23017_write_word(obj, MCP23017_GPINTENA); // Enable interrupts on all pins

}
void mcp23017_write(MCP23017 *obj)
{
	mcp23017_write_word(obj,MCP23017_GPIOA);
}

void mcp23017_write_register( MCP23017 *obj, uint8_t reg, uint8_t data)
{
	i2c_start_wait(obj->address + I2C_WRITE);
	i2c_write(reg);
	i2c_write(data);
	i2c_stop();
}

uint8_t mcp23017_read_register( MCP23017 *obj, uint8_t reg)
{
	i2c_start_wait(obj->address + I2C_WRITE);
	i2c_write( reg );
	i2c_rep_start( obj->address + I2C_READ);
	uint8_t data = i2c_readNak();
	i2c_stop();
	return data;
}

void mcp23017_write_word( MCP23017 *obj, uint8_t reg )
{
	i2c_start_wait(obj->address + I2C_WRITE);
	i2c_write(reg);
	uint16_t data = obj->data;		//	access our object's 16 bits of data
	i2c_write((uint8_t)data);		//	cast and write the lower byte
	i2c_write((uint8_t)(data>>8));	//	automatically adv address pointer and write upper
	i2c_stop();
}