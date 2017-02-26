#ifndef PACKET_H_
#define PACKET_H_

#define SYNC_BYTE 0x33
#define MAX_PAYLOAD_LENGTH 2042

// DATA CODE byte building blocks
// Source and Target masks (high nibble (4 bits))
#define from_laptop		0x00
#define from_scanner	0x01
#define from_ccd_bus	0x02
#define from_sci_bus	0x03
#define to_laptop		0x00
#define to_scanner		0x01
#define to_ccd_bus		0x02
#define to_sci_bus		0x03
// DC commands (low nibble (4 bits))
#define reboot			0x00
#define handshake		0x01
#define status			0x02
#define settings		0x03
#define request			0x04
#define response		0x05
#define send_msg		0x06
#define send_rep_msg	0x07
#define stop_msg_flow	0x08
#define receive_msg		0x09
#define self_diag		0x0A
#define make_backup		0x0B
#define restore_backup	0x0C
#define restore_default	0x0D
#define debug			0x0E
#define ok_error		0x0F

// SUB-DATA CODE byte
// DC command 0x03 (settings)
#define read_settings					0x01
#define write_settings					0x02
#define enable_ccd_bus					0x03
#define disable_ccd_bus					0x04
#define enable_sci_bus					0x05
#define disable_sci_bus					0x06
#define enable_lcd_bl					0x07
#define enable_lcd_bl_pwm				0x08
#define disable_lcd_bl					0x09
#define metric_units					0x0A
#define imperial_units					0x0B
#define ext_eeprom_wp_on				0x0C
#define ext_eeprom_wp_off				0x0D
#define set_ccd_interframe_response		0x0E
#define set_sci_interframe_response		0x0F
#define set_sci_intermsg_response		0x10
#define set_sci_intermsg_request		0x11
#define set_packet_intermsg_response	0x12
#define enable_sci_bus_high_speed		0x13
#define disable_sci_bus_high_speed		0x14
#define enable_ccd_bus_filtering		0x15
#define disable_ccd_bus_filtering		0x16
#define enable_sci_bus_filtering		0x17
#define disable_sci_bus_filtering		0x18
#define set_sci_bus_target				0x19
#define enable_buzzer					0x1A
#define disable_buzzer					0x1B
#define enable_button_hold				0x1C
#define disable_button_hold				0x1D
#define enable_act_led					0x1E
#define disable_act_led					0x1F

// DC command 0x04-0x05 (request)
#define scanner_firmware_version		0x01
#define read_int_eeprom					0x02
#define read_ext_eeprom					0x03
#define write_int_eeprom				0x04
#define write_ext_eeprom				0x05
#define scan_ccd_bus_modules			0x06
#define free_ram_value					0x07
#define mcu_counter_value				0x08
//...
#define button_pressed_down				0xFE
#define button_released					0xFF


// DC command 0x0F (OK/ERROR)
#define ok										0x00
#define error_sync_invalid_value				0x01
#define error_length_invalid_value				0x02
#define error_datacode_same_source_target		0x03
#define error_datacode_source_target_conflict	0x04
#define error_datacode_invalid_target			0x05
#define error_datacode_invalid_dc_command		0x06
#define error_subdatacode_invalid_value			0x07
#define error_subdatacode_not_enough_info		0x08
#define error_payload_missing_values			0x09
#define error_payload_invalid_values			0x0A
#define error_checksum_invalid_value			0x0B
#define error_packet_invalid_frame_format		0x0C
#define error_packet_timeout_occured			0x0D
#define error_packet_unknown_source				0x0E
#define error_scanner_internal_error			0x0F
#define error_general_invalid_address			0x10
#define error_general_invalid_address_range		0x11
#define error_incomplete_ccd_msg				0x20
#define error_incomplete_sci_msg				0x21


extern uint8_t send_packet(uint8_t source, uint8_t target, uint8_t dc_command, uint8_t subdatacode, uint8_t *payloadbuff, uint16_t payloadbufflen);

#endif /* PACKET_H_ */