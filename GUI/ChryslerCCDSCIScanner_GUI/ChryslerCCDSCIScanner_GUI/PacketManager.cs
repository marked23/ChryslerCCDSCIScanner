/*
 * ChryslerCCDSCIScanner_GUI
 * Copyright (C) 2016-2017, László Dániel
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.IO;
using System;
using System.Collections.Generic;

namespace ChryslerCCDSCIScanner_GUI
{
    public class PacketManager
    {
        // Fundamental constants
        public const byte SYNC_BYTE         = 0x33;
        public const int MAX_PAYLOAD_LENGTH = 2042;

        // DATA CODE byte building blocks
        // Source and Target masks (high nibble (4 bits))
        public const byte from_laptop       = 0x00;
        public const byte from_scanner      = 0x01;
        public const byte from_ccd_bus      = 0x02;
        public const byte from_sci_bus      = 0x03;
        public const byte to_laptop         = 0x00;
        public const byte to_scanner        = 0x01;
        public const byte to_ccd_bus        = 0x02;
        public const byte to_sci_bus        = 0x03;
        // DC commands (low nibble (4 bits))
        public const byte reboot            = 0x00;
        public const byte handshake         = 0x01;
        public const byte status            = 0x02;
        public const byte settings          = 0x03;
        public const byte request           = 0x04;
        public const byte response          = 0x05;
        public const byte send_msg          = 0x06;
        public const byte send_rep_msg      = 0x07;
        public const byte stop_msg_flow     = 0x08;
        public const byte receive_msg       = 0x09;
        public const byte self_diag         = 0x0A;
        public const byte make_backup       = 0x0B;
        public const byte restore_backup    = 0x0C;
        public const byte restore_default   = 0x0D;
        public const byte debug             = 0x0E;
        public const byte ok_error          = 0x0F;

        // SUB-DATA CODE bytes

        // DC command 0x03 (Settings)
        public const byte read_settings     = 0x01;
        public const byte write_settings    = 0x02;
        public const byte enable_ccd_bus    = 0x03;
        public const byte disable_ccd_bus   = 0x04;
        public const byte enable_sci_bus    = 0x05;
        public const byte disable_sci_bus   = 0x06;
        public const byte enable_lcd_bl     = 0x07;
        public const byte enable_lcd_bl_pwm = 0x08;
        public const byte disable_lcd_bl    = 0x09;
        public const byte metric_units      = 0x0A;
        public const byte imperial_units    = 0x0B;
        public const byte ext_eeprom_wp_on  = 0x0C;
        public const byte ext_eeprom_wp_off = 0x0D;
        public const byte set_ccd_interframe_response   = 0x0E;
        public const byte set_sci_interframe_response   = 0x0F;
        public const byte set_sci_intermsg_response     = 0x10;
        public const byte set_sci_intermsg_request      = 0x11;
        public const byte set_packet_intermsg_response  = 0x12;
        public const byte enable_sci_hi_speed           = 0x13;
        public const byte disable_sci_hi_speed          = 0x14;
        public const byte enable_ccd_bus_filtering      = 0x15;
        public const byte disable_ccd_bus_filtering     = 0x16;
        public const byte enable_sci_bus_filtering      = 0x17;
        public const byte disable_sci_bus_filtering     = 0x18;
        public const byte set_sci_bus_target    = 0x19;
        public const byte enable_buzzer         = 0x1A;
        public const byte disable_buzzer        = 0x1B;
        public const byte enable_button_hold    = 0x1C;
        public const byte disable_button_hold   = 0x1D;
        public const byte enable_act_led        = 0x1E;
        public const byte disable_act_led       = 0x1F;

        // DC command 0x04 (General request)
        public const byte scanner_firmware_ver      = 0x01;
        public const byte read_int_eeprom           = 0x02;
        public const byte read_ext_eeprom           = 0x03;
        public const byte write_int_eeprom          = 0x04;
        public const byte write_ext_eeprom          = 0x05;
        public const byte scan_vehicle_modules      = 0x06;
        public const byte free_ram_available        = 0x07;
        public const byte mcu_counter_value         = 0x08;

        // DC command 0x0E (Debug)
        public const byte high_speed_sci_bus_dump   = 0x01;

        // DC command 0x0F (OK/ERROR)
        public const byte ok                                    = 0x00;
        public const byte error_sync_invalid_value              = 0x01;
        public const byte error_length_invalid_value            = 0x02;
        public const byte error_datacode_same_source_target     = 0x03;
        public const byte error_datacode_source_target_conflict = 0x04;
        public const byte error_datacode_invalid_target         = 0x05;
        public const byte error_datacode_invalid_dc_command     = 0x06;
        public const byte error_subdatacode_invalid_value       = 0x07;
        public const byte error_subdatacode_not_enough_info     = 0x08;
        public const byte error_payload_missing_values          = 0x09;
        public const byte error_payload_invalid_values          = 0x0A;
        public const byte error_checksum_invalid_value          = 0x0B;
        public const byte error_packet_invalid_frame_format     = 0x0C;
        public const byte error_packet_timeout_occured          = 0x0D;
        public const byte error_packet_unknown_source           = 0x0E;
        public const byte error_scanner_internal_error          = 0x0F;
        public const byte error_general_invalid_address         = 0x10;
        public const byte error_general_invalid_address_range   = 0x11;

        // USB Packet structure
        public struct PacketStructure
        {
            public byte[] sync;         // SYNC byte          [1]     { 0x33 }
            public byte[] length;       // LENGTH bytes       [2]     { 0x__, 0x__ } (number of PAYLOAD bytes)
            public byte[] datacode;     // DATA CODE byte     [1]     { 0x__ }
            public byte[] subdatacode;  // SUB-DATA CODE byte [1]     { 0x__ }
            public byte[] payload;      // PAYLOAD byte(s)    [VAR]   { 0x__, 0x__, 0x__, ..., 0x__ }
            public byte[] checksum;     // CHECKSUM byte      [1]     { 0x__ } (LENGTH + DATA CODE + SUB-DATA CODE + PAYLOAD bytes summed then MSB byte masked out)

            // Create a byte-array for serial transmission or other purposes
            // Note, this method tolerates length and checksum byte errors so they can be zeros if you manually send packets
            // Another note, it takes the structure variables and transforms them into a byte-array, so they need to be previously filled with data
            public byte[] ToBytes()
            {
                // Instantiate a new MemoryStream object called "stream", it gets automatically disposed (using...)
                using (var stream = new MemoryStream())
                {
                    // Some variables that we need along the way
                    int calculated_checksum = 0;
                    int calculated_length = 0;
                    bool payload_bytes = false;

                    // Decide if there are PAYLOAD bytes present
                    if (payload == null) calculated_length = 2;
                    else
                    {
                        calculated_length = 2 + payload.Length;
                        payload_bytes = true;
                    }

                    length = new byte[2];
                    length[0] = (byte)((calculated_length >> 8) & 0xFF);
                    length[1] = (byte)(calculated_length & 0xFF);

                    // Start writing to the MemoryStream
                    // Write one SYNC byte
                    sync = new byte[1] { SYNC_BYTE };
                    stream.Write(sync, 0, sync.Length);

                    // Write two LENGTH bytes
                    stream.Write(length, 0, length.Length);

                    // Write one DATA CODE byte
                    stream.Write(datacode, 0, datacode.Length);

                    // Write one SUB-DATA CODE byte
                    stream.Write(subdatacode, 0, subdatacode.Length);

                    // Write PAYLOAD bytes if available
                    if (payload_bytes) stream.Write(payload, 0, payload.Length);

                    // Calculate CHECKSUM-8 value from the LENGTH, DATA CODE, SUB-DATA CODE and PAYLOAD bytes
                    // Add the DATA CODE byte and SUB-DATA CODE byte to the temporary CHECKSUM-16 variable
                    calculated_checksum += length[0] + length[1] + datacode[0] + subdatacode[0];

                    // Continue adding bytes to it if there are PAYLOAD bytes
                    if (payload_bytes)
                    {
                        for (int i = 0; i < payload.Length; i++)
                        {
                            calculated_checksum += payload[i];
                        }
                    }

                    // CHECKSUM, LSB byte of the sum of the previous bytes (except SYNC byte)
                    checksum = new byte[1];
                    checksum[0] = (byte)(calculated_checksum & 0xFF);

                    // Write final CHECKSUM byte to the MemoryStream
                    stream.Write(checksum, 0, checksum.Length);

                    // Reset payload
                    payload = null;

                    // Return the whole MemoryStream as a byte-array
                    return stream.ToArray();
                }
            }

            // Create USB Packet from byte-array and error-check the packet
            // The method returns true if the byte array contains a valid packet
            public bool FromBytes(byte[] bytes)
            {
                int payload_length = 0;

                // Instantiate a new MemoryStream object inside of a BinaryReader object called "reader", it gets automatically disposed (using...)
                // The MemoryStream is filled with the input "bytes" array
                using (var reader = new BinaryReader(new MemoryStream(bytes)))
                {
                    // Some variables
                    int calculated_checksum = 0;
                    bool payload_bytes = true;

                    // Let the SYNC bytes equal to the first byte of the MemoryStream {0x33}
                    sync = new byte[1];
                    sync = reader.ReadBytes(1);

                    // Let the LENGTH bytes equal to the next two bytes
                    length = new byte[2];
                    length = reader.ReadBytes(2);

                    // Let the DATA CODE byte equal to the next byte
                    datacode = new byte[1];
                    datacode = reader.ReadBytes(1);

                    // Let the SUB-DATA CODE byte equal to the next byte
                    subdatacode = new byte[1];
                    subdatacode = reader.ReadBytes(1);

                    // Decide if there are PAYLOAD bytes present in the MemoryStream
                    payload_length = (length[0] << 8 | length[1]) - 2;
                    if ( payload_length > 0) payload_bytes = true;
                    else payload_bytes = false;

                    // Continue adding PAYLOAD bytes to the packet if available
                    if (payload_bytes)
                    {
                        try
                        {
                            // Let the PAYLOAD bytes equal to the next coming bytes
                            payload = new byte[payload_length];
                            payload = reader.ReadBytes(payload_length);
                        }
                        catch { return false; }
                    }
                    else
                    {
                        payload = null;
                    }

                    // Let the CHECKSUM byte equal to the last byte of the MemoryStream
                    checksum = new byte[1];
                    checksum = reader.ReadBytes(1);

                    // Calculate checksum
                    try
                    {
                        calculated_checksum = length[0] + length[1] + datacode[0] + subdatacode[0];
                    }
                    catch { return false; }

                    if (payload_bytes)
                    {
                        for (int i = 0; i < payload.Length; i++)
                        {
                            calculated_checksum += payload[i];
                        }
                    }

                    if (sync[0] != SYNC_BYTE) return false;
                    if (payload_length + 6 != bytes.Length) return false;

                    try
                    {
                        if ((calculated_checksum & 0xFF) == checksum[0]) { return true; }
                        else { return false; }
                    }
                    catch { return false; }
                }
            }

            // This method takes the input values and fills the structure variables with the appropriate values
            // Note, practically this method needs to be used with the ToBytes() method after the packet has been "generated"
            // Side note, packets are easier to be generated with the help of this method than with the ToBytes() alone
            public void GeneratePacket(byte source, byte target, byte dc_command, byte subdatacode_value, byte[] payloadbuff)
            {
                payload = null;
                sync = new byte[1] { SYNC_BYTE };

                if (payloadbuff != null)
                {
                    length = new byte[2]
                    {
                        (byte)(((payloadbuff.Length + 2) >> 8) & 0xFF),
                        (byte)((payloadbuff.Length + 2) & 0xFF)
                    };
                }
                else
                {
                    length = new byte[2] { 0x00, 0x02 }; // If there's no payload then the length is always 2.
                }

                datacode = new byte[1] { (byte)((source << 6) | (target << 4) | dc_command) }; // Assemble datacode byte using 3 different values
                subdatacode = new byte[1] { subdatacode_value };

                if (payloadbuff != null)
                {
                    payload = new byte[payloadbuff.Length];
                    for (int i = 0; i < payloadbuff.Length; i++)
                    {
                        payload[i] = payloadbuff[i];
                    }
                }
                else { payload = null; }

                int calculated_checksum = length[0] + length[1] + datacode[0] + subdatacode[0];
                if (payloadbuff != null)
                {
                    for (int i = 0; i < payloadbuff.Length; i++)
                    {
                        calculated_checksum += payloadbuff[i];
                    }
                }

                checksum = new byte[1] { (byte)(calculated_checksum & 0xFF) };
            }
        }
    }

    class DTC_LookupTable
    {
        public Dictionary<byte, string> PCM_DTC_Table { get; }
        public HashSet<byte> ReservedKeys { get; }

        public DTC_LookupTable()
        {
            PCM_DTC_Table = new Dictionary<byte, string>();
            ReservedKeys = new HashSet<byte>();

            // The first byte argument is the table offset of the DTC,
            // and the second string is the meaning of this byte

            //PCM_DTC_Table.Add(0x00, "P1684, "BATTERY DISCONNECT");
            PCM_DTC_Table.Add(0x00, "UNRECOGNIZED DTC");
            PCM_DTC_Table.Add(0x01, "NO CAM SIGNAL AT PCM");
            PCM_DTC_Table.Add(0x02, "INTERNAL CONTROLLER FAILURE");
            PCM_DTC_Table.Add(0x03, "O2 SENSOR STAYS ABOVE CENTER (RICH)");
            PCM_DTC_Table.Add(0x04, "O2 SENSOR SIGNAL BELOW CENTER (LEAN)");
            PCM_DTC_Table.Add(0x05, "CHARGING SYSTEM VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x06, "CHARGING SYSTEM VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x07, "TURBO BOOST LIMIT EXCEEDED");
            PCM_DTC_Table.Add(0x08, "RIGHT O2 SENSOR STAYS ABOVE CENTER (RICH)");
            PCM_DTC_Table.Add(0x09, "RIGHT O2 SENSOR STAYS BELOW CENTER (LEAN)");
            PCM_DTC_Table.Add(0x0A, "AUTO SHUTDOWN RELAY CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x0B, "GENERATOR FIELD NOT SWITCHING PROPERLY");
            PCM_DTC_Table.Add(0x0C, "TORQUE CONVERTER CLUTCH SOLENOID/TRANS RELAY CIRCUITS");
            PCM_DTC_Table.Add(0x0D, "TC WASTEGATE SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x0E, "LOW SPEED FAN CTRL RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x0F, "SPEED CONTROL SOLENOID CIRCUITS");
            PCM_DTC_Table.Add(0x10, "A/C CLUTCH RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x11, "EGR SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x12, "EVAP PURGE SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x13, "INJECTOR #3 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x14, "INJECTOR #2 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x15, "INJECTOR #1 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x16, "INJECTOR #3 PEAK CURRENT NOT REACHED");
            PCM_DTC_Table.Add(0x17, "INJECTOR #2 PEAK CURRENT NOT REACHED");
            PCM_DTC_Table.Add(0x18, "INJECTOR #1 PEAK CURRENT NOT REACHED");
            PCM_DTC_Table.Add(0x19, "IDLE AIR CONTROL MOTOR CIRCUITS");
            PCM_DTC_Table.Add(0x1A, "THROTTLE POSITION SENSOR VOLTAGE LOW");
            PCM_DTC_Table.Add(0x1B, "THROTTLE POSITION SENSOR VOLTAGE HIGH");
            PCM_DTC_Table.Add(0x1C, "THROTTLE BODY TEMP SENSOR VOLTAGE LOW");
            PCM_DTC_Table.Add(0x1D, "THROTTLE BODY TEMP SENSOR VOLTAGE HIGH");
            PCM_DTC_Table.Add(0x1E, "ECT SENSOR VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x1F, "ECT SENSOR VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x20, "UPSTREAM O2 SENSOR STAYS AT CENTER");
            PCM_DTC_Table.Add(0x21, "ENGINE IS COLD TOO LONG");
            PCM_DTC_Table.Add(0x22, "SKIP SHIFT SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x23, "NO VEHICLE SPEED SENSOR SIGNAL");
            PCM_DTC_Table.Add(0x24, "MAP SENSOR VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x25, "MAP SENSOR VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x26, "SLOW CHANGE IN IDLE MAP SENSOR SIGNAL");
            PCM_DTC_Table.Add(0x27, "NO CHANGE IN MAP FROM START TO RUN");
            PCM_DTC_Table.Add(0x28, "NO CRANK REFERENCE SIGNAL AT PCM");
            PCM_DTC_Table.Add(0x29, "IGNITION COIL #3 PRIMARY CIRCUIT");
            PCM_DTC_Table.Add(0x2A, "IGNITION COIL #2 PRIMARY CIRCUIT");
            PCM_DTC_Table.Add(0x2B, "IGNITION COIL #1 PRIMARY CIRCUIT");
            PCM_DTC_Table.Add(0x2C, "NO ASD RELAY OUTPUT VOLTAGE AT PCM");
            PCM_DTC_Table.Add(0x2D, "SYSTEM RICH, IDLE ADAP AT LEAN LIMIT");
            PCM_DTC_Table.Add(0x2E, "EGR SYSTEM FAILURE");
            PCM_DTC_Table.Add(0x2F, "BARO READ SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x30, "PCM FAILURE SRI MILE NOT STORED");
            PCM_DTC_Table.Add(0x31, "PCM FAILURE EEPROM WRITE DENIED");
            PCM_DTC_Table.Add(0x32, "TRANS 3 - 4 SHIFT SOL / TRANS RELAY CIRCUITS");
            PCM_DTC_Table.Add(0x33, "SECONDARY AIR SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x34, "IDLE SWITCH SHORTED LOW");
            PCM_DTC_Table.Add(0x35, "IDLE SWITCH OPEN CIRCUIT");
            PCM_DTC_Table.Add(0x36, "SURGE VALVE SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x37, "INJECTOR #9 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x38, "INJECTOR #10 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x39, "INTAKE AIR TEMP SENSOR VOLTAGE LOW");
            PCM_DTC_Table.Add(0x3A, "INTAKE AIR TEMP SENSOR VOLTAGE HIGH");
            PCM_DTC_Table.Add(0x3B, "KNOCK SENSOR CIRCUIT");
            PCM_DTC_Table.Add(0x3C, "BAROMETRIC PRESSURE OUT OF RANGE");
            PCM_DTC_Table.Add(0x3D, "INJECTOR #4 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x3E, "UPSTREAM O2 SENSOR SHORTED TO VOLTAGE");
            PCM_DTC_Table.Add(0x3F, "SYSTEM RICH, R-IDLE ADAP AT LEAN LIMIT");
            PCM_DTC_Table.Add(0x40, "WASTEGATE #2 CIRCUIT");
            PCM_DTC_Table.Add(0x41, "RIGHT O2 SENSOR STAYS AT CENTER");
            PCM_DTC_Table.Add(0x42, "RIGHT O2 SENSOR SHORTED TO VOLTAGE");
            PCM_DTC_Table.Add(0x43, "SYSTEM LEAN, R-IDLE ADAP AT RICH LIMIT");
            PCM_DTC_Table.Add(0x44, "PCM FAILURE SPI COMMUNICATIONS");
            PCM_DTC_Table.Add(0x45, "INJECTOR #5 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x46, "INJECTOR #6 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x47, "BATTERY TEMP SENSOR VOLTS OUT OF LIMIT");
            PCM_DTC_Table.Add(0x48, "NO CMP AT IGNITION/ INJ DRIVER MODULE");
            PCM_DTC_Table.Add(0x49, "NO CKP AT IGNITION/ INJ DRIVER MODULE");
            PCM_DTC_Table.Add(0x4A, "TRANS TEMP SENSOR VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x4B, "TRANS TEMP SENSOR VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x4C, "IGNITION COIL #4 PRIMARY CIRCUIT");
            PCM_DTC_Table.Add(0x4D, "IGNITION COIL #5 PRIMARY CIRCUIT");
            PCM_DTC_Table.Add(0x4E, "SYSTEM LEAN, IDLE ADAP AT RICH LIMIT");
            PCM_DTC_Table.Add(0x4F, "INJECTOR #7 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x50, "INJECTOR #8 CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x51, "FUEL PUMP RESISTOR BYPASS RELAY CKT");
            PCM_DTC_Table.Add(0x52, "SPD CTRL PWR RLY; OR S/ C 12V DRIVER CKT");
            PCM_DTC_Table.Add(0x53, "KNOCK SENSOR #2 CIRCUIT");
            PCM_DTC_Table.Add(0x54, "FLEX FUEL SENSOR VOLTS TOO HIGH");
            PCM_DTC_Table.Add(0x55, "FLEX FUEL SENSOR VOLTS TOO LOW");
            PCM_DTC_Table.Add(0x56, "SPEED CONTROL SWITCH ALWAYS HIGH");
            PCM_DTC_Table.Add(0x57, "SPEED CONTROL SWITCH ALWAYS LOW");
            PCM_DTC_Table.Add(0x58, "MANIFOLD TUNE VALVE SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x59, "NO BUS MESSAGES");
            PCM_DTC_Table.Add(0x5A, "A/C PRESSURE SENSOR VOLTS TOO HIGH");
            PCM_DTC_Table.Add(0x5B, "A/C PRESSURE SENSOR VOLTS TOO LOW");
            PCM_DTC_Table.Add(0x5C, "LOW SPEED FAN CTRL RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x5D, "HIGH SPEED CONDENSER FAN CTRL RELAY CKT");
            PCM_DTC_Table.Add(0x5E, "CNG TEMP SENSOR VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x5F, "CNG TEMP SENSOR VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x60, "NO CCD/ J1850 MESSAGES FROM TCM");
            PCM_DTC_Table.Add(0x61, "NO CCD/ PCI BUS MESSAGE FROM BODY CONTROL MODULE");
            PCM_DTC_Table.Add(0x62, "CNG PRESSURE SENSOR VOLTAGE TOO HIGH");
            PCM_DTC_Table.Add(0x63, "CNG PRESSURE SENSOR VOLTAGE TOO LOW");
            PCM_DTC_Table.Add(0x64, "LOSS OF FLEX FUEL CALIBRATION SIGNAL");
            PCM_DTC_Table.Add(0x65, "FUEL PUMP RELAY CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x66, "UPSTREAM O2 SENSOR SLOW RESPONSE");
            PCM_DTC_Table.Add(0x67, "UPSTREAM O2 SENSOR HEATER FAILURE");
            PCM_DTC_Table.Add(0x68, "DOWNSTRM O2S UNABLE TO SWITCH RICH/LEAN");
            PCM_DTC_Table.Add(0x69, "DOWNSTREAM O2 SENSOR HEATER FAILURE");
            PCM_DTC_Table.Add(0x6A, "MULTIPLE CYLINDER MIS - FIRE");
            PCM_DTC_Table.Add(0x6B, "CYLINDER #1 MIS-FIRE");
            PCM_DTC_Table.Add(0x6C, "CYLINDER #2 MIS-FIRE");
            PCM_DTC_Table.Add(0x6D, "CYLINDER #3 MIS-FIRE");
            PCM_DTC_Table.Add(0x6E, "CYLINDER #4 MIS-FIRE");
            PCM_DTC_Table.Add(0x6F, "TOO LITTLE SECONDARY AIR");
            PCM_DTC_Table.Add(0x70, "CATALYTIC CONVERTER EFFICIENCY FAILURE");
            PCM_DTC_Table.Add(0x71, "EVAP PURGE FLOW MONITOR FAILURE");
            PCM_DTC_Table.Add(0x72, "P / N SWITCH STUCK IN PARK OR IN GEAR");
            PCM_DTC_Table.Add(0x73, "POWER STEERING SWITCH FAILURE");
            PCM_DTC_Table.Add(0x74, "DESIRED FUEL TIMING ADVANCE NOT REACHED");
            PCM_DTC_Table.Add(0x75, "LOST FUEL INJECTION TIMING SIGNAL");
            PCM_DTC_Table.Add(0x76, "FUEL SYSTEM RICH");
            PCM_DTC_Table.Add(0x77, "FUEL SYSTEM LEAN");
            PCM_DTC_Table.Add(0x78, "RIGHT BANK FUEL SYSTEM RICH");
            PCM_DTC_Table.Add(0x79, "RIGHT BANK FUEL SYSTEM LEAN");
            PCM_DTC_Table.Add(0x7A, "RIGHT BANK UPSTREAM O2S SLOW RESPONSE");
            PCM_DTC_Table.Add(0x7B, "RIGHT DOWNSTREAM O2 SENSOR SLOW RESPONSE");
            PCM_DTC_Table.Add(0x7C, "RIGHT BANK UPSTREAM O2S HEATER FAILURE");
            PCM_DTC_Table.Add(0x7D, "RIGHT BANK DOWNSTREAM O2S HEATER FAILURE");
            PCM_DTC_Table.Add(0x7E, "DOWNSTREAM O2 SENSOR SHORTED TO VOLTAGE");
            PCM_DTC_Table.Add(0x7F, "RIGHT BNK DNSTRM O2S SHORTED TO VOLTAGE");
            PCM_DTC_Table.Add(0x80, "CLOSED LOOP TEMP NOT REACHED");
            PCM_DTC_Table.Add(0x81, "DOWNSTREAM O2 SENSOR STAYS AT CENTER");
            PCM_DTC_Table.Add(0x82, "RIGHT BANK DOWNSTRM O2S STAYS AT CENTER");
            PCM_DTC_Table.Add(0x83, "LEAN OPERATION AT WIDE OPEN THROTTLE");
            PCM_DTC_Table.Add(0x84, "TPS VOLTAGE DOES NOT AGREE WITH MAP");
            PCM_DTC_Table.Add(0x85, "TIMING BELT SKIPPED 1 TOOTH OR MORE");
            PCM_DTC_Table.Add(0x86, "NO 5 VOLTS TO A / C PRESSURE SENSOR");
            PCM_DTC_Table.Add(0x87, "NO 5 VOLTS TO MAP SENSOR");
            PCM_DTC_Table.Add(0x88, "NO 5 VOLTS TO TP SENSOR");
            PCM_DTC_Table.Add(0x89, "EATX CONTROLLER DTC PRESENT / Read Transmission DTCs");
            PCM_DTC_Table.Add(0x8A, "TARGET IDLE NOT REACHED");
            PCM_DTC_Table.Add(0x8B, "HI SPEED RAD FAN CTRL RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x8C, "DIESEL EGR SYSTEM FAILURE");
            PCM_DTC_Table.Add(0x8D, "GOV PRESS NOT EQUAL TO TARGET @ 15 - 20PSI");
            PCM_DTC_Table.Add(0x8E, "GOV PRES ABOVE 3 PSI IN GEAR WITH 0 MPH");
            PCM_DTC_Table.Add(0x8F, "STARTER RELAY CONTROL CIRCUIT");
            PCM_DTC_Table.Add(0x90, "POST - CATALYST O2 SEN SHORTED TO GROUND");
            PCM_DTC_Table.Add(0x91, "VACUUM LEAK FOUND(IAC FULLY SEATED)");
            PCM_DTC_Table.Add(0x92, "5 VOLT SUPPLY, OUTPUT TOO LOW");
            PCM_DTC_Table.Add(0x93, "POST - CATALYST O2 SEN SHORTED TO VOLTAGE");
            PCM_DTC_Table.Add(0x94, "TORQ CONV CLU, NO RPM DROP AT LOCKUP");
            PCM_DTC_Table.Add(0x95, "FUEL LEVEL SENDING UNIT VOLTS TOO LOW");
            PCM_DTC_Table.Add(0x96, "FUEL LEVEL SENDING UNIT VOLTS TOO HIGH");
            PCM_DTC_Table.Add(0x97, "FUEL LEVEL UNIT NO CHANGE OVER MILES");
            PCM_DTC_Table.Add(0x98, "BRAKE SWITCH STUCK PRESSED OR RELEASED");
            PCM_DTC_Table.Add(0x99, "AMBIENT / BATT TEMP SEN VOLTS TOO LOW");
            PCM_DTC_Table.Add(0x9A, "AMBIENT / BATT TEMP SEN VOLTS TOO HIGH");
            PCM_DTC_Table.Add(0x9B, "UPSTREAM O2 SENSOR SHORTED TO GROUND");
            PCM_DTC_Table.Add(0x9C, "DOWNSTREAM O2 SENSOR SHORTED TO GROUND");
            PCM_DTC_Table.Add(0x9D, "INTERMITTENT LOSS OF CMP OR CKP");
            PCM_DTC_Table.Add(0x9E, "TOO MUCH SECONDARY AIR");
            PCM_DTC_Table.Add(0x9F, "POST - CATALYST O2S SLOW RESPONSE");
            PCM_DTC_Table.Add(0xA0, "EVAP LEAK MONITOR SMALL LEAK DETECTED");
            PCM_DTC_Table.Add(0xA1, "EVAP LEAK MONITOR LARGE LEAK DETECTED");
            PCM_DTC_Table.Add(0xA2, "NO TEMP RISE SEEN FROM INTAKE HEATERS");
            PCM_DTC_Table.Add(0xA3, "WAIT TO START LAMP CIRCUIT");
            PCM_DTC_Table.Add(0xA4, "TRANS TEMP SENSR, NO TEMP RISE AFTR START");
            PCM_DTC_Table.Add(0xA5, "3 - 4 SHIFT SOL, NO RPM DROP @ 3 - 4 SHIFT");
            PCM_DTC_Table.Add(0xA6, "LOW OUTPUT SPD SENSR RPM, ABOVE 15 MPH");
            PCM_DTC_Table.Add(0xA7, "GOVERNOR PRESSURE SENSOR VOLTS TOO LOW");
            PCM_DTC_Table.Add(0xA8, "GOVERNOR PRESSURE SENSOR VOLTS TOO HI");
            PCM_DTC_Table.Add(0xA9, "GOV PRESS SEN OFFSET VOLTS TOO LO OR HIGH");
            PCM_DTC_Table.Add(0xAB, "GOVERNOR PRESSURE SOL CONTROL/ TRANS RELAY CIRCUITS");
            PCM_DTC_Table.Add(0xAC, "POST - CATALYST O2S STUCK IN THE MIDDLE");
            PCM_DTC_Table.Add(0xAD, "TRANS 12 VOLT SUPPLY RELAY CTRL CIRCUIT");
            PCM_DTC_Table.Add(0xAE, "CYLINDER #5 MIS-FIRE");
            PCM_DTC_Table.Add(0xAF, "CYLINDER #6 MIS-FIRE");
            PCM_DTC_Table.Add(0xB0, "CYLINDER #7 MIS-FIRE");
            PCM_DTC_Table.Add(0xB1, "CYLINDER #8 MIS-FIRE");
            PCM_DTC_Table.Add(0xB2, "CYLINDER #9 MIS-FIRE");
            PCM_DTC_Table.Add(0xB3, "CYLINDER #10 MIS-FIRE");
            PCM_DTC_Table.Add(0xB4, "RIGHT BANK CATALYST EFFICIENCY FAILURE");
            PCM_DTC_Table.Add(0xB5, "REAR BANK UPSTRM O2S SHORTED TO GROUND");
            PCM_DTC_Table.Add(0xB6, "REAR BANK DWNSTR O2S SHORTED TO GROUND");
            PCM_DTC_Table.Add(0xB7, "LEAK DETECTION PUMP SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0xB8, "LEAK DETECT PUMP SW OR MECHANICAL FAULT");
            PCM_DTC_Table.Add(0xB9, "AUXILIARY 5 VOLT SUPPLY OUTPUT TOO LOW");
            PCM_DTC_Table.Add(0xBA, "MIS - FIRE ADAPTIVE NUMERATOR AT LIMIT");
            PCM_DTC_Table.Add(0xBB, "EVAP LEAK MONITOR PINCHED HOSE FOUND");
            PCM_DTC_Table.Add(0xBC, "O / D SWITCH PRESSED (LO)MORE THAN 5 MIN");
            PCM_DTC_Table.Add(0xBD, "POST - CATALYST O2S HEATER FAILURE");
            PCM_DTC_Table.Add(0xC5, "HIGH SPEED RAD FAN GROUND CTRL RLY CKT");
            PCM_DTC_Table.Add(0xC6, "ONE OF THE IGN COILS DRAWS TOO MUCH CURRENT");
            PCM_DTC_Table.Add(0xFE, "END OF DTC LIST");

            //ReservedKeys.Add(0x08);
            //ReservedKeys.Add(0x09);
        }

        public string lookup_pcm_dtc(byte key)
        {
            return (ReservedKeys.Contains(key)) ? null : PCM_DTC_Table[key];
        }
    }
}
