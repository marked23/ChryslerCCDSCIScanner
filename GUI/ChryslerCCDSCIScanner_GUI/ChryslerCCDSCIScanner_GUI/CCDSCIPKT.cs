using System.IO;
using System;
using System.Collections.Generic;

namespace ChryslerCCDSCIScanner_GUI
{
    public class CCDSCIPKT
    {
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

        // CCDSCI Packet structure
        public struct CCDSCI_PACKET
        {
            // Basic CCDSCI Packet Structure variables
            public byte[] sync;         // SYNC bytes         [1]     { 0x33 }
            public byte[] length;       // LENGTH bytes       [2]     { 0x__, 0x__ } (number of PAYLOAD bytes)
            public byte[] datacode;     // DATA CODE byte     [1]     { 0x__ }
            public byte[] subdatacode;  // SUB-DATA CODE byte [1]     { 0x__ }
            public byte[] payload;      // PAYLOAD bytes      [VAR]   { 0x__, 0x__, 0x__, ..., 0x__ }
            public byte[] checksum;     // CHECKSUM bytes     [1]     { 0x__ } (LENGTH + DATA CODE + SUB-DATA CODE + PAYLOAD bytes summed and LSB byte taken)
            // Note: if the LENGTH is 2 then there's only 1 DATA CODE byte, 1 SUB-DATACODE byte and no PAYLOAD bytes!

            // Create a byte-array for serial transmission or other purposes
            // Note: it tolerates length and checksum byte errors so they can be zeros if you manually send packets
            // Another note: it takes the structure variables and transforms them into a byte-array, so they need to be previously filled with data
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
                    // Write two SYNC bytes (they are constant)
                    sync = new byte[1] { SYNC_BYTE };
                    stream.Write(sync, 0, sync.Length);

                    // Write two LENGTH bytes
                    stream.Write(length, 0, length.Length);

                    // Write one DATA CODE byte
                    stream.Write(datacode, 0, datacode.Length);

                    // Write one SUB-DATA CODE byte
                    stream.Write(subdatacode, 0, subdatacode.Length);

                    // Write PAYLOAD bytes if the length is greater than 1
                    if (payload_bytes) stream.Write(payload, 0, payload.Length);

                    // Calculate CHECKSUM-16 value from the LENGTH, DATA CODE, SUB-DATA CODE and PAYLOAD bytes
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

                    // CHECKSUM: LSB byte of the sum of the previous bytes (except SYNC byte)
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

            // Create CCDSCI Packet from byte-array and error-check the packet
            // The method returns true if the byte array contains a valid CCDSCI Packet
            public bool FromBytes(byte[] bytes)
            {
                int payload_length = 0;
                //int calculated_length = 0;

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
                    try
                    {
                        payload_length = (length[0] << 8 | length[1]) - 2;
                        if (payload_length > 2) payload_bytes = true;
                        else payload_bytes = true;
                    }
                    catch { return false; }

                    // Continue adding PAYLOAD bytes to the packet if the length is greater than 1
                    if (payload_bytes)
                    {
                        // Calculate PAYLOAD length
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

            // This function takes the input values and fills the structure variables with the appropriate values
            // Note: practically this function needs to be used with the ToBytes() function after the packet has been "generated"
            // Side note: packets are easier to be generated with the help of this function than with the ToBytes() function alone
            public void GeneratePacket(byte source, byte target, byte dc_command, byte subdatacode_value, byte[] payloadbuff)
            {
                sync = new byte[1] { SYNC_BYTE };

                if (payloadbuff != null) { length = new byte[2] { (byte)(((payloadbuff.Length + 2) >> 8) & 0xFF), (byte)((payloadbuff.Length + 2) & 0xFF) }; }
                else { length = new byte[2] { 0x00, 0x02 }; }

                datacode = new byte[1] { (byte)((source << 6) | (target << 4) | dc_command) };
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

    class CCDSCIPKT_Commands
    {
        public byte[] ccd_bus_on = new byte[6]                      { 0x33, 0x00, 0x02, 0x8C, 0x04, 0x92 };
        public byte[] ccd_bus_off = new byte[6]                     { 0x33, 0x00, 0x02, 0x8C, 0x05, 0x93 };
        public byte[] sci_bus_on = new byte[6]                      { 0x33, 0x00, 0x02, 0x8C, 0x06, 0x94 };
        public byte[] sci_bus_off = new byte[6]                     { 0x33, 0x00, 0x02, 0x8C, 0x07, 0x95 };
        public byte[] request_bcm_memory_read_on = new byte[6]      { 0x33, 0x00, 0x02, 0x94, 0x08, 0x9E };
        public byte[] request_bcm_memory_read_off = new byte[6]     { 0x33, 0x00, 0x02, 0x94, 0x09, 0x9F };
        public byte[] request_exteeprom_content = new byte[6]       { 0x33, 0x00, 0x02, 0x8C, 0x15, 0xA3 };
        public byte[] request_inteeprom_content = new byte[6]       { 0x33, 0x00, 0x02, 0x8C, 0x16, 0xA4 };
        public byte[] request_status = new byte[6]                  { 0x33, 0x00, 0x02, 0x12, 0x00, 0x14 };
        public byte[] request_dummy_packets = new byte[6]           { 0x33, 0x00, 0x02, 0x8C, 0x20, 0xAE };
        public byte[] request_pcm_memory_26_read_on = new byte[6]   { 0x33, 0x00, 0x02, 0x8C, 0x21, 0xAF };
        public byte[] request_pcm_memory_26_read_off = new byte[6]  { 0x33, 0x00, 0x02, 0x8C, 0x22, 0xB0 };
        public byte[] sci_bus_high_speed_on = new byte[6]           { 0x33, 0x00, 0x02, 0x8C, 0x30, 0xBE };
        public byte[] sci_bus_high_speed_off = new byte[6]          { 0x33, 0x00, 0x02, 0x8C, 0x31, 0xBF };
        public byte[] erase_exteeprom_content = new byte[6]         { 0x33, 0x00, 0x02, 0x8C, 0x32, 0xC0 };
        public byte[] log_on = new byte[6]                          { 0x33, 0x00, 0x02, 0x8C, 0x33, 0xC1 };
        public byte[] log_off = new byte[6]                         { 0x33, 0x00, 0x02, 0x8C, 0x34, 0xC2 };
        public byte[] request_pcm_memory_28_read_on = new byte[6]   { 0x33, 0x00, 0x02, 0x8C, 0x35, 0xC3 };
        public byte[] request_pcm_memory_28_read_off = new byte[6]  { 0x33, 0x00, 0x02, 0x8C, 0x36, 0xC4 };
        public byte[] request_pcm_ram_F2_on = new byte[7]           { 0x33, 0x00, 0x03, 0x8C, 0x37, 0xF2, 0xB8 };
        public byte[] request_pcm_ram_F2_off = new byte[7]          { 0x33, 0x00, 0x03, 0x8C, 0x38, 0xF2, 0xB9 };
        public byte[] request_pcm_ram_F4_on = new byte[7]           { 0x33, 0x00, 0x03, 0x8C, 0x37, 0xF4, 0xBA };
        public byte[] request_pcm_ram_F4_off = new byte[7]          { 0x33, 0x00, 0x03, 0x8C, 0x38, 0xF4, 0xBB };
        public byte[] request_pcm_ram_F6_on = new byte[7]           { 0x33, 0x00, 0x03, 0x8C, 0x37, 0xF6, 0xBC };
        public byte[] request_pcm_ram_F6_off = new byte[7]          { 0x33, 0x00, 0x03, 0x8C, 0x38, 0xF6, 0xBD };
        public byte[] request_o2_sensor_on = new byte[6]            { 0x33, 0x00, 0x02, 0x8C, 0x39, 0xC7 };
        public byte[] request_o2_sensor_off = new byte[6]           { 0x33, 0x00, 0x02, 0x8C, 0x3A, 0xC8 };
        public byte[] request_map_sensor_on = new byte[6]           { 0x33, 0x00, 0x02, 0x8C, 0x3B, 0xC9 };
        public byte[] request_map_sensor_off = new byte[6]          { 0x33, 0x00, 0x02, 0x8C, 0x3C, 0xCA };
        public byte[] request_pcm_read_all_on = new byte[6]         { 0x33, 0x00, 0x02, 0x8C, 0x3D, 0xCB };
        public byte[] request_pcm_read_all_off = new byte[6]        { 0x33, 0x00, 0x02, 0x8C, 0x3E, 0xCC };
        public byte[] request_scanner_handshake = new byte[6]       { 0x33, 0x00, 0x02, 0x11, 0x00, 0x13 };
        public byte[] reboot_scanner = new byte[6]                  { 0x33, 0x00, 0x02, 0x10, 0x00, 0x12 };
    }

    class CCDSCIPKT_LookupTable
    {
        public Dictionary<byte, string> PCM_DTC_Table { get; }
        public HashSet<byte> ReservedKeys { get; }

        public CCDSCIPKT_LookupTable()
        {
            PCM_DTC_Table = new Dictionary<byte, string>();
            ReservedKeys = new HashSet<byte>();

            // The first byte argument is the table offset of the DTC,
            // and the second string is the meaning of this byte, including OBD-II P-code

            //PCM_DTC_Table.Add(0x00, "P1684: BATTERY DISCONNECT");
            PCM_DTC_Table.Add(0x00, "P0000: DTC ERROR");
            PCM_DTC_Table.Add(0x01, "P0340: CAMSHAFT SENSOR CIRCUIT");
            PCM_DTC_Table.Add(0x02, "P0601: INTERNAL SELF-TEST");
            PCM_DTC_Table.Add(0x03, "P0031: O2 SENSOR 1/1 HEATER CKT LOW");
            PCM_DTC_Table.Add(0x04, "P0032: O2 SENSOR 1/1 HEATER CKT HIGH");
            PCM_DTC_Table.Add(0x05, "P1682: CHARGING OUTPUT LOW");
            PCM_DTC_Table.Add(0x06, "P1594: BATTERY VOLTAGE HIGH");
            PCM_DTC_Table.Add(0x07, "P0234: BOOST LIMIT EXCEEDED");
            PCM_DTC_Table.Add(0x08, "P0000: N/A");
            PCM_DTC_Table.Add(0x09, "P0000: N/A");
            PCM_DTC_Table.Add(0x0A, "P1388: ASD RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x0B, "P0622: GENERATOR FIELD CIRCUIT");
            PCM_DTC_Table.Add(0x0C, "P0743: TCC SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x0D, "P0243: WASTEGATE SOLENOID CIRCUIT");
            PCM_DTC_Table.Add(0x0E, "P1491: RAD FAN RELAY CIRCUIT");
            PCM_DTC_Table.Add(0x0F, "P1595: S/C SERVO SOLENOID CKTS");
            PCM_DTC_Table.Add(0x10, "P0645: A/C CLUTCH RELAY CIRCUIT");

            //ReservedKeys.Add(0x08);
            //ReservedKeys.Add(0x09);
        }

        public string lookup_pcm_dtc(byte key)
        {
            return (ReservedKeys.Contains(key))
               ? null
               : PCM_DTC_Table[key];
        }
    }
}
