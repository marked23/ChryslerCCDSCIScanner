using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Globalization;
using System.Collections;
using RJCP.IO.Ports;
using RJCP.Datastructures;

namespace ChryslerCCDSCIScanner_GUI
{
    public partial class MainForm : Form
    {
        // SerialPortStream object to transmit and receive byte-based messages
        SerialPortStream serial = new SerialPortStream();

        // Packet Reception object
        PacketReceptor pr = new PacketReceptor();

        // CCDSCI Packet objects to parse byte arrays
        PacketManager.PacketStructure ccdscipkt_rx = new PacketManager.PacketStructure();
        PacketManager.PacketStructure ccdscipkt_tx = new PacketManager.PacketStructure();

        // CCDSCI Packet command list object
        //CCDSCIPKT_Commands ccdscipkt_commands = new CCDSCIPKT_Commands();

        // Diagnostic Trouble Code (DTC) lookup table object
        DTC_LookupTable ccdscipkt_lookuptable = new DTC_LookupTable();

        // Circular buffer objects (FIFO - First In First Out) for easier data management
        CircularBuffer<byte> serial_rx_buffer = new CircularBuffer<byte>(4096);
        CircularBuffer<byte> serial_tx_buffer = new CircularBuffer<byte>(4096);

        System.Timers.Timer TimeoutTimer = new System.Timers.Timer();
        
        public const byte SYNC_POS = 0;
        public const byte LENGTH_POS = 1;
        public const byte DATACODE_POS = 3;
        public const byte SUBDATACODE_POS = 4;

        public bool serial_packet_complete = true;
        public int serial_packet_length = 0;
        public int serial_packet_ptr = 0;
        public int serial_bytes_needed = 0;

        ulong packet_count_rx = 0;
        ulong packet_count_tx = 0;

        public bool packet_arrived = false;

        public bool ccd_enabled = true;
        public bool sci_enabled = true;
        public bool log_enabled = true;

        public bool scanner_found = false;
        public bool scanner_connected = false;
        public bool timeout = false;
        public bool start2 = false;

        byte[] buffer = new byte[4096];

        public string DateTimeNow;
        public string TextLogFilename;
        public string BinaryLogFilename;

        public byte[] ccd_filter_bytes;
        public bool ccd_filtering_active = false;

        public MainForm()
        {
            InitializeComponent();
            ComponentsDisabled();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            pr.PropertyChanged += new PropertyChangedEventHandler(PacketReceived);
            TimeoutTimer.Elapsed += new ElapsedEventHandler(TimeoutReached);
            TimeoutTimer.Interval = 200;
            TimeoutTimer.Enabled = false;

            // Create logfile names
            DateTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            TextLogFilename = @"LOG/scannerlog_" + DateTimeNow + ".txt";
            BinaryLogFilename = @"LOG/scannerlog_" + DateTimeNow + ".bin";

            // Welcome
            WriteCommandHistory("Welcome!");
        }

        public void ComponentsDisabled()
        {
            // Group boxes
            PacketLogGroupbox.Enabled = false;
            CCDBusMessagesGroupbox.Enabled = false;
            SCIBusMessagesGroupbox.Enabled = false;
            SensorDataGroupbox.Enabled = false;
            RealTimeDiagnosticsGroupbox.Enabled = false;
            ReadDTCsGroupbox.Enabled = false;
            SCIBusSpeedGroupbox.Enabled = false;
            MiscGroupbox.Enabled = false;
            EEPROMGroupbox.Enabled = false;
            ETCGroupbox.Enabled = false;
        }

        public void ComponentsEnabled()
        {
            // Group boxes
            PacketLogGroupbox.Enabled = true;
            CCDBusMessagesGroupbox.Enabled = true;
            SCIBusMessagesGroupbox.Enabled = true;
            SensorDataGroupbox.Enabled = true;
            RealTimeDiagnosticsGroupbox.Enabled = true;
            ReadDTCsGroupbox.Enabled = true;
            SCIBusSpeedGroupbox.Enabled = true;
            MiscGroupbox.Enabled = true;
            EEPROMGroupbox.Enabled = true;
            ETCGroupbox.Enabled = false;
        }

        public void TimeoutReached(object source, ElapsedEventArgs e)
        {
            timeout = true;
        }

        void PacketReceived(object sender, PropertyChangedEventArgs e)
        {
            if (pr.PacketReceived && e.PropertyName == "PacketReceived")
            {
                int bytes_to_read = 0;
                int available_packets = 0;
                int result = 0;
                bool first_real_sync_found = false;
                pr.PacketReceived = false; // re-arm detection

                // Find the first valid packet
                while (!first_real_sync_found)
                {
                    // Find the first byte with value 0x33
                    while ((serial_rx_buffer.Array[serial_rx_buffer.Start] != PacketManager.SYNC_BYTE) && (serial_rx_buffer.ReadLength > 0))
                    {
                        // If it's not 0x33 then remove it (the pointer advances automatically)
                        serial_rx_buffer.Pop();
                    }

                    // If all the bytes are consumed then exit from this function
                    if (serial_rx_buffer.ReadLength == 0) return;

                    // Determine if this is really a SYNC byte (not just random byte with the value 0x33)
                    bytes_to_read = ((serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS] << 8) | serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS + 1]) + 4;

                    // A valid packet's length must be less or equal than 2042
                    if (bytes_to_read <= 2042)
                    {
                        byte[] temp_buffer = new byte[bytes_to_read];
                        Array.Copy(serial_rx_buffer.Array, serial_rx_buffer.Start, temp_buffer, 0, bytes_to_read);

                        // Try to convert the bytes into a packet
                        if (ccdscipkt_rx.FromBytes(temp_buffer))
                        {
                            first_real_sync_found = true; // success
                        }
                        else
                        {
                            first_real_sync_found = false; // failure
                            serial_rx_buffer.Pop(); // pop this fake sync byte out of existence!
                        }
                    }
                    else
                    {
                        first_real_sync_found = false; // failure
                        serial_rx_buffer.Pop(); // pop this false sync byte out of existence!
                    }
                }

                // Read the length of the first available packet finally
                bytes_to_read = ((serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS] << 8) | serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS + 1]) + 4;

                // If there are more bytes in the circular buffer then calculate how many more packets there may be
                if (serial_rx_buffer.ReadLength > bytes_to_read)
                {
                    bool last_packet_found = false;
                    int current_pointer = serial_rx_buffer.Start;
                    int current_bytes_in_buffer = serial_rx_buffer.ReadLength;

                    // This is the first packet's length again temporarly
                    int temp_bytes_to_read = ((serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS] << 8) | serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS + 1]) + 4;

                    // Put the pointer at the end of the first known packet
                    current_pointer += temp_bytes_to_read;

                    result++; // 1 packet is always preset at this point

                    // Find another packet in the mess only by peeking (not removing anything from the buffer)
                    while (!last_packet_found)
                    {
                        // Find the first byte with value 0x33
                        while ((serial_rx_buffer.Array[current_pointer] != PacketManager.SYNC_BYTE) && (current_pointer <= current_bytes_in_buffer))
                        {
                            // If it's not 0x33 then remove it (the pointer advances automatically)
                            current_pointer++;
                        }

                        // If all the bytes are consumed then exit this function
                        if (current_pointer >= current_bytes_in_buffer) last_packet_found = true;

                        // TODO: Determine if this is really a SYNC byte (not just random byte with the value 0x33)
                        temp_bytes_to_read = ((serial_rx_buffer.Array[current_pointer + LENGTH_POS] << 8) | serial_rx_buffer.Array[current_pointer + LENGTH_POS + 1]) + 4;
                        if (temp_bytes_to_read <= 2042)
                        {
                            byte[] temp_buffer = new byte[temp_bytes_to_read];
                            Array.Copy(serial_rx_buffer.Array, current_pointer, temp_buffer, 0, temp_bytes_to_read);

                            if (ccdscipkt_rx.FromBytes(temp_buffer))
                            {
                                result++;
                                current_pointer += temp_bytes_to_read;
                                last_packet_found = false;
                            }
                        }
                        else
                        {
                            current_pointer++;
                            last_packet_found = false;
                        }
                    }
                    available_packets = result;
                }
                else if (serial_rx_buffer.ReadLength == bytes_to_read) // There's only 1 packet in the circular buffer
                {
                    available_packets = 1;
                }
                else // There are not enough bytes to make a whole packet, wait for another data reception
                {
                    available_packets = 0;
                }

                if (available_packets > 0)
                {
                    for (int i = 0; i < available_packets; i++)
                    {
                        // Find the first valid packet
                        while (!first_real_sync_found)
                        {
                            // Find the first byte with value 0x33
                            while ((serial_rx_buffer.Array[serial_rx_buffer.Start] != PacketManager.SYNC_BYTE) && (serial_rx_buffer.ReadLength > 0))
                            {
                                // If it's not 0x33 then remove it (the pointer advances automatically)
                                serial_rx_buffer.Pop();
                            }

                            // If all the bytes are consumed then exit from this function
                            if (serial_rx_buffer.ReadLength == 0) return;

                            // Determine if this is really a SYNC byte (not just random byte with the value 0x33)
                            bytes_to_read = ((serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS] << 8) | serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS + 1]) + 4;

                            // A valid packet's length must be lesser or equal than 2042
                            if (bytes_to_read <= 2042)
                            {
                                byte[] temp_buffer = new byte[bytes_to_read];
                                Array.Copy(serial_rx_buffer.Array, serial_rx_buffer.Start, temp_buffer, 0, bytes_to_read);

                                // Try to convert the bytes into a packet
                                if (ccdscipkt_rx.FromBytes(temp_buffer))
                                {
                                    first_real_sync_found = true; // success
                                }
                                else
                                {
                                    first_real_sync_found = false; // failure
                                    serial_rx_buffer.Pop(); // pop this fake sync byte out of existence!
                                }
                            }
                            else
                            {
                                first_real_sync_found = false;
                                serial_rx_buffer.Pop(); // pop this false sync byte out of existence!
                            }
                        }

                        // Read actual length again
                        bytes_to_read = ((serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS] << 8) | serial_rx_buffer.Array[serial_rx_buffer.Start + LENGTH_POS + 1]) + 4;

                        byte[] temp_read_buffer = new byte[bytes_to_read];
                        for (int k = 0; k < bytes_to_read; k++)
                        {
                            temp_read_buffer[k] = serial_rx_buffer.Pop();
                        }
                        ProcessData(temp_read_buffer); // TODO

                        first_real_sync_found = false;
                    }

                    // Flush buffer (if there are remaining bytes they are 100% useless)
                    // TODO: some kind of timeout to detect half packets received
                    serial_rx_buffer.Reset();
                    Array.Clear(serial_rx_buffer.Array, 0, serial_rx_buffer.Array.Length);
                }
            }
        }

        private bool GetHandshake()
        {
            try
            {
                serial.Open(); // the current serial port is previously configured
                ccdscipkt_tx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.handshake, PacketManager.ok, null);
                WriteSerialData(ccdscipkt_tx.ToBytes());
                WritePacketTextbox("TX", "REQUEST HANDSHAKE FROM " + serial.PortName, ccdscipkt_tx.ToBytes());

                timeout = false;
                TimeoutTimer.Enabled = true;
                while ((serial.BytesToRead < 27) && (!timeout))
                {
                    // Wait here until all bytes are received (we know that 27 bytes should be received) or timeout occurs.
                }
                TimeoutTimer.Enabled = false;
                if (timeout)
                {
                    timeout = false;

                    // Manually save the received bytes from the scanner
                    int bytes = serial.BytesToRead;
                    byte[] data = new byte[bytes];
                    serial.Read(data, 0, bytes);
                    WritePacketTextbox("RX", "TIMEOUT (" + serial.PortName + ")", null);
                    serial.Close();
                    return false;
                }
                else
                {
                    // Manually save the received bytes from the scanner
                    byte[] data = new byte[27];
                    serial.Read(data, 0, serial.BytesToRead);
                    if (Encoding.ASCII.GetString(data, 5, 21) == "CHRYSLERCCDSCISCANNER")
                    {
                        WritePacketTextbox("RX", "VALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                        DisconnectButton.Text = "Disconnect (" + serial.PortName + ")";
                        return true;
                    }
                    else
                    {
                        WritePacketTextbox("RX", "INVALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                        serial.Close();
                        return false;
                    }
                }
            }
            catch
            {
                WriteCommandHistory("Can't open " + serial.PortName + "!");
                serial.Close();
                return false;
            }
        }

        public void WriteCommandHistory(string message)
        {

            command_history_textbox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] ");
            command_history_textbox.AppendText(message + Environment.NewLine);
            // Scroll down
            command_history_textbox.SelectionStart = command_history_textbox.TextLength;
            command_history_textbox.ScrollToCaret();
        }

        public void WritePacketTextbox(string rxtx, string description, byte[] message)
        {

            // Suspend Layout
            PacketTextbox.SuspendLayout();

            // Get all the lines out as an arry
            string[] lines = PacketTextbox.Lines;

            // If there are "too much lines" then remove some of them from the beginning
            if (lines.Length > 50)
            {
                var newlineslist = lines.ToList();
                newlineslist.RemoveRange(0, 30);

                // And put back what's left
                PacketTextbox.Lines = newlineslist.ToArray();
            }

            // Build the new text separately to avoid heavy textbox flickering
            StringBuilder newstuff = new StringBuilder();

            // Add stuff
            newstuff.Append(rxtx + ": " + description + Environment.NewLine);

            if (message != null)
            {
                // Add the bytes of the message
                foreach (byte bytes in message)
                {
                    newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
                }
            }

            // Add two new lines
            newstuff.Append(Environment.NewLine + Environment.NewLine);

            // Add the built string to the textbox in one go
            PacketTextbox.AppendText(newstuff.ToString());

            // Scroll down to the end of the textbox
            PacketTextbox.SelectionStart = PacketTextbox.TextLength;
            PacketTextbox.ScrollToCaret();

            // Refresh
            PacketTextbox.Refresh();

            // Resume Layout
            PacketTextbox.ResumeLayout();

            // Force the Garbage Collector to clean up the mess I just made
            //GC.Collect();

            // Update User Interface
            if (rxtx == "RX") packet_count_rx++;
            if (rxtx == "TX") packet_count_tx++;
            packet_count_rx_label.Text = "Packets received: " + packet_count_rx;
            packet_count_tx_label.Text = "Packets sent: " + packet_count_tx;

            // Save data to log files
            if (!Directory.Exists("LOG")) Directory.CreateDirectory("LOG");
            File.AppendAllText(TextLogFilename, newstuff.ToString());

            using (BinaryWriter writer = new BinaryWriter(File.Open(BinaryLogFilename, FileMode.Append)))
            {
                writer.Write(message);
            }

            // Discard the temporary string builder
            newstuff = null;
        }

        public void WriteSendPacketTextbox(byte[] message)
        {
            StringBuilder newstuff = new StringBuilder();

            foreach (byte bytes in message)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            PacketSendTextbox.Text = newstuff.ToString();
            PacketSendTextbox.Refresh();
        }

        public void WriteCCDBusTextbox(byte[] message)
        {
            // Suspend Layout
            CCDBusMsgTextbox.SuspendLayout();

            // Get all the lines out as an arry
            string[] lines = CCDBusMsgTextbox.Lines;

            // If there are "too much lines" then remove some of them from the beginning
            if (lines.Length > 50)
            {
                var newlineslist = lines.ToList();
                newlineslist.RemoveRange(0, 30);

                // And put back what's left
                CCDBusMsgTextbox.Lines = newlineslist.ToArray();
            }

            // Build the new text separately to avoid heavy textbox flickering
            StringBuilder newstuff = new StringBuilder();

            // Add the bytes of the message
            foreach (byte bytes in message)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            // Add two new lines
            newstuff.Append(Environment.NewLine);

            // Add the built string to the textbox in one go
            CCDBusMsgTextbox.AppendText(newstuff.ToString());

            // Scroll down to the end of the textbox
            CCDBusMsgTextbox.SelectionStart = CCDBusMsgTextbox.TextLength;
            CCDBusMsgTextbox.ScrollToCaret();

            // Refresh
            CCDBusMsgTextbox.Refresh();

            // Resume Layout
            CCDBusMsgTextbox.ResumeLayout();

            // Force the Garbage Collector to clean up the mess I just made
            //GC.Collect();

            // Discard the temporary string builder
            newstuff = null;
        }

        public async void ReadSerialData()
        {
            // Put this in an endless loop so it's monitoring the serial port all the time
            for(;;)
            {
                int actual_length = await serial.ReadAsync(buffer, 0, buffer.Length);
                if (actual_length > 0)
                {
                    serial_rx_buffer.Append(buffer, 0, actual_length);
                    Array.Clear(buffer, 0, buffer.Length);

                    pr.PacketReceived = true; // Let the program know first that we have something

                    // Then proceed to update some values
                    buffer_start_label.Text = "Buffer Start: " + serial_rx_buffer.Start;
                    buffer_end_label.Text = "Buffer End: " + serial_rx_buffer.End;
                    buffer_readlength_label.Text = "Buffer ReadLength: " + serial_rx_buffer.ReadLength + " byte(s)";
                    buffer_writelength_label.Text = "Buffer WriteLength: " + serial_rx_buffer.WriteLength + " byte(s)";
                }
                if (scanner_connected == false) break; // exit this loop when serial port is closed (high cpu usage!)
                //Application.DoEvents();
                //GC.Collect();
            }
        }

        public async void WriteSerialData(byte[] message)
        {
            try
            {
                await serial.WriteAsync(message, 0, message.Length);
            }
            catch
            {
                WriteCommandHistory("Can't write to the serial port!");
            }
        }

        public void ProcessData(byte[] data)
        {
            // Make sure it's a valid CCDSCI Packet (the "FromBytes" function must return a true value)
            // This function also assigns the internal variables of the packet structure
            if (ccdscipkt_rx.FromBytes(data))
            {
                // Find out what the source and the target of the packet is by examining the high nibble (4 bits)
                byte source = (byte)((ccdscipkt_rx.datacode[0] >> 6) & 0x03);
                byte target = (byte)((ccdscipkt_rx.datacode[0] >> 4) & 0x03); // Must be the laptop (0x00)

                // Extract DC command value from the low nibble (4 bits)
                byte dc_command = (byte)(ccdscipkt_rx.datacode[0] & 0x0F);

                switch (source)
                {
                    case PacketManager.from_laptop:
                        {
                            // Packet echoed back
                            break;
                        }

                    case PacketManager.from_scanner:
                        {
                            switch (dc_command)
                            {
                                case PacketManager.reboot:
                                    {
                                        // Reboot confirmation received
                                        WritePacketTextbox("RX", "SCANNER REBOOT COMPLETE", data);
                                        break;
                                    }
                                case PacketManager.handshake:
                                    {
                                        string received_handshake = Encoding.ASCII.GetString(ccdscipkt_rx.payload);
                                        if (received_handshake == "CHRYSLERCCDSCISCANNER")
                                        {
                                            WritePacketTextbox("RX", "VALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                                        }
                                        else
                                        {
                                            WritePacketTextbox("RX", "INVALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                                        }
                                        break;
                                    }
                                case PacketManager.status:
                                    {
                                        // Write out to packet textbox
                                        WritePacketTextbox("RX", "STATUS PACKET RECEIVED", data);

                                        // If this is the first status report received then do some extra work
                                        if (!scanner_connected)
                                        {
                                            scanner_connected = true;
                                            DisconnectButton.Enabled = true;
                                            StatusButton.Enabled = true;

                                            WriteCommandHistory("Scanner connected!");
                                            //WriteCommandHistory("Waiting for commands...");

                                            PacketLogEnabledCheckbox.Enabled = true;
                                            CCDBusEnabledCheckbox.Enabled = true;
                                            SCIBusEnabledCheckbox.Enabled = true;

                                            read_inteeprom_button.Enabled = true;
                                            write_inteeprom_button.Enabled = true;
                                            save_inteeprom_button.Enabled = true;

                                            read_exteeprom_button.Enabled = true;
                                            write_exteeprom_button.Enabled = true;
                                            save_exteeprom_button.Enabled = true;

                                            PacketSendTextbox.Enabled = true;
                                        }

                                        if (sci_enabled)
                                        {
                                            SCIBusSendMsgButton.Enabled = true;
                                            SCIBusSendMsgTextbox.Enabled = true;
                                        }

                                        if (ccd_enabled)
                                        {
                                            CCDBusSendMsgButton.Enabled = true;
                                            CCDBusSendMsgTextbox.Enabled = true;
                                        }

                                        if (log_enabled)
                                        {
                                            PacketSendButton.Enabled = true;
                                            PacketSendTextbox.Enabled = true;
                                        }

                                        break;
                                    }
                                case PacketManager.response: // General request responses
                                    {
                                        switch (ccdscipkt_rx.subdatacode[0])
                                        {
                                            case 0x07:
                                                {
                                                    WritePacketTextbox("RX", "FREE RAM VALUE RESPONSE RECEIVED", data);

                                                    break;
                                                }
                                            case 0x08:
                                                {
                                                    WritePacketTextbox("RX", "MCU COUNTER VALUE RESPONSE RECEIVED", data);

                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        // Write out to packet textbox


                                        break;
                                    }
                                default:
                                    {
                                        WritePacketTextbox("RX", "RECEIVED BYTES", data);
                                        break;
                                    }
                            }
                            break;
                        }
                    case PacketManager.from_ccd_bus:
                        {
                            switch (dc_command)
                            {
                                case PacketManager.receive_msg:
                                    {
                                        WritePacketTextbox("RX", "CCD-BUS MESSAGE", data);

                                        // If filtering is active
                                        if (ccd_filter_bytes != null)
                                        {
                                            if (ccd_filter_bytes.Contains(ccdscipkt_rx.payload[0])) WriteCCDBusTextbox(ccdscipkt_rx.payload);
                                        }
                                        else // Filtering disabled
                                        {
                                            WriteCCDBusTextbox(ccdscipkt_rx.payload);
                                        }

                                        break;
                                    }
                            }
                            break;
                        }
                    case PacketManager.from_sci_bus:
                        {
                            // Do something
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            try
            {
                // Get available COM ports
                string[] ports = SerialPortStream.GetPortNames();

                if (ports.Length == 0)
                {
                    WriteCommandHistory("Scanner not found!");
                    ConnectButton.Enabled = true;
                }
                else
                {
                    foreach (string port in ports)
                    {
                        // Try 5 times
                        for (int i = 0; i < 5; i++)
                        {
                            serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                            if (GetHandshake())
                            {
                                scanner_found = true;
                                ComponentsEnabled();
                                ReadSerialData();
                                ConnectButton.Enabled = false;
                                StatusButton.Enabled = true;
                                StatusButton.PerformClick();
                                break;
                            }
                            else
                            {
                                scanner_found = false;
                                ConnectButton.Enabled = true;
                                serial.Close();
                            }
                        }
                        if (scanner_found) break;
                    }
                }

                //textBox3.Text = ccdscipkt_lookuptable.lookup_pcm_dtc(0x08).ToString();
            }
            catch
            {
                ConnectButton.Enabled = true;
                WriteCommandHistory("Scanner not found!");
            }
        }

        private void read_exteeprom_button_Click(object sender, EventArgs e)
        {
            //ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_exteeprom_content);
            //write_serial_data(ccdscipkt_tx.ToBytes());

            byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
            temp = ccdscipkt_tx.ToBytes();

            StringBuilder temp_string = new StringBuilder(temp.Length * 3);
            foreach (byte status_byte in temp)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            string temp_string_final = temp_string.ToString().ToUpper();
            PacketTextbox.AppendText("TX:\n");
            PacketTextbox.AppendText(temp_string_final + "\n\n");
            PacketTextbox.ScrollToCaret();
        }

        private void erase_exteeprom_button_Click(object sender, EventArgs e)
        {
            //ccdscipkt_tx.FromBytes(ccdscipkt_commands.erase_exteeprom_content);
            //write_serial_data(ccdscipkt_tx.ToBytes());
        }

        private void read_inteeprom_button_Click(object sender, EventArgs e)
        {
            //ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_inteeprom_content);
            //write_serial_data(ccdscipkt_tx.ToBytes());

            byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
            temp = ccdscipkt_tx.ToBytes();

            StringBuilder temp_string = new StringBuilder(temp.Length * 3);
            foreach (byte status_byte in temp)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            string temp_string_final = temp_string.ToString().ToUpper();
            PacketTextbox.AppendText("TX:\n");
            PacketTextbox.AppendText(temp_string_final + "\n\n");
            PacketTextbox.ScrollToCaret();
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.status, PacketManager.ok, null);
            WriteSerialData(ccdscipkt_tx.ToBytes());
            WritePacketTextbox("TX", "REQUEST STATUS PACKET", ccdscipkt_tx.ToBytes());
        }

        private void send_sci_msg_button_Click(object sender, EventArgs e)
        {
            if (sci_enabled)
            {
                if (SCIBusSendMsgTextbox.Text != "")
                {
                    int calculated_checksum = 0;

                    byte[] bytes = Util.GetBytes(SCIBusSendMsgTextbox.Text);
                    byte[] transmit_sci_msg = new byte[6 + bytes.Length];

                    transmit_sci_msg[0] = PacketManager.SYNC_BYTE; // 0x33
                    transmit_sci_msg[1] = (byte)(((bytes.Length + 2) >> 8) & 0xFF);
                    transmit_sci_msg[2] = (byte)((bytes.Length + 2) & 0xFF);
                    transmit_sci_msg[3] = 0x8C;
                    transmit_sci_msg[4] = 0x23;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        transmit_sci_msg[5 + i] = bytes[i];
                    }

                    for (int j = 0; j < bytes.Length + 4; j++)
                    {
                        calculated_checksum += transmit_sci_msg[1 + j];
                    }

                    transmit_sci_msg[5 + bytes.Length] = (byte)(calculated_checksum & 0xFF);

                    ccdscipkt_tx.FromBytes(transmit_sci_msg);
                    WriteSerialData(ccdscipkt_tx.ToBytes());

                    if (log_enabled)
                    {
                        WritePacketTextbox("TX", "MESSAGE TO SCI-BUS", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void sci_bus_hs_button_Click(object sender, EventArgs e)
        {

        }

        private void communication_packets_textbox_TextChanged(object sender, EventArgs e)
        {
            if (PacketTextbox.Text != "")
            {
                PacketClearButton.Enabled = true;
            }
            else
            {
                PacketClearButton.Enabled = false;
            }
        }

        private void ccd_bus_messages_textbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextbox.Text != "")
            {
                CCDBusClearButton.Enabled = true;
            }
            else
            {
                CCDBusClearButton.Enabled = false;
            }
        }

        private void sci_bus_messages_textbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextbox.Text != "")
            {
                SCIBusClearButton.Enabled = true;
            }
            else
            {
                SCIBusClearButton.Enabled = false;
            }
        }

        private void PacketClearButton_Click(object sender, EventArgs e)
        {
            PacketTextbox.Clear();
        }

        private void ccd_bus_log_clear_button_Click(object sender, EventArgs e)
        {
            CCDBusMsgTextbox.Clear();
        }

        private void sci_bus_log_clear_button_Click(object sender, EventArgs e)
        {
            SCIBusMsgTextbox.Clear();
        }

        private void sci_bus_enabled_checkbox_Click(object sender, EventArgs e)
        {
            if (sci_enabled)
            {
                sci_enabled = false;
                SCIBusEnabledCheckbox.Checked = false;
                SCIBusSendMsgButton.Enabled = false;
                SCIBusSendMsgTextbox.Clear();
                SCIBusSendMsgTextbox.Enabled = false;
                //ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_off);
                //write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                sci_enabled = true;
                SCIBusEnabledCheckbox.Checked = true;
                SCIBusEnabledCheckbox.Enabled = true;
                SCIBusSendMsgButton.Enabled = true;
                SCIBusSendMsgTextbox.Enabled = true;
                //ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_on);
                //write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void ccd_bus_enabled_checkbox_Click(object sender, EventArgs e)
        {
            if (ccd_enabled)
            {
                ccd_enabled = false;
                CCDBusEnabledCheckbox.Checked = false;
                CCDBusEnabledCheckbox.Text = "CCD-bus disabled";
                CCDBusSendMsgButton.Enabled = false;
                CCDBusSendMsgTextbox.Clear();
                CCDBusSendMsgTextbox.Enabled = false;
                //ccdscipkt_tx.FromBytes(ccdscipkt_commands.ccd_bus_off);
                //write_serial_data(ccdscipkt_tx.ToBytes());

            }
            else
            {
                ccd_enabled = true;
                CCDBusEnabledCheckbox.Checked = true;
                CCDBusEnabledCheckbox.Text = "CCD-bus enabled";
                CCDBusEnabledCheckbox.Enabled = true;
                CCDBusSendMsgButton.Enabled = true;
                CCDBusSendMsgTextbox.Enabled = true;
                //ccdscipkt_tx.FromBytes(ccdscipkt_commands.ccd_bus_on);
                //write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void real_time_diagnostics_button_Click(object sender, EventArgs e)
        {
            DiagnosticsForm diagnostics = new DiagnosticsForm();
            diagnostics.Show();
            //diagnostics.Dispose();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm(this);
            about.Show();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            Environment.Exit(0);
        }

        private void read_dtc_pcm_button_Click(object sender, EventArgs e)
        {
            if (true)
            {
                int calculated_checksum = 0;

                byte[] transmit_sci_msg = new byte[7];

                transmit_sci_msg[0] = PacketManager.SYNC_BYTE; // 0x33
                transmit_sci_msg[1] = 0x00;
                transmit_sci_msg[2] = 0x03;
                transmit_sci_msg[3] = 0x8C;
                transmit_sci_msg[4] = 0x23;
                transmit_sci_msg[5] = 0x10;

                for (int j = 0; j < 5; j++)
                {
                    calculated_checksum += transmit_sci_msg[1 + j];
                }

                transmit_sci_msg[6] = (byte)(calculated_checksum & 0xFF);

                ccdscipkt_tx.FromBytes(transmit_sci_msg);
                WriteSerialData(ccdscipkt_tx.ToBytes());

                byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
                temp = ccdscipkt_tx.ToBytes();

                StringBuilder temp_string = new StringBuilder(temp.Length * 3);
                foreach (byte status_byte in temp)
                {
                    temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
                }

                string temp_string_final = temp_string.ToString().ToUpper();
                PacketTextbox.AppendText("TX:\n");
                PacketTextbox.AppendText(temp_string_final + "\n\n");
                PacketTextbox.ScrollToCaret();
            }
        }

        private void PacketSendButton_Click(object sender, EventArgs e)
        {
            if (PacketSendTextbox.Text != "")
            {
                byte[] bytes = Util.GetBytes(PacketSendTextbox.Text);
                if (bytes.Length > 1)
                {
                    byte[] transmit_packet;

                    ccdscipkt_tx.payload = null; // causes a bug if it's not here

                    // DATA CODE byte, SUB-DATA CODE byte, PAYLOAD byte(s) only (Partial packet)
                    if (bytes.Length < 6)
                    {
                        transmit_packet = new byte[bytes.Length + 4];

                        for (int i = 0; i < 3; i++)
                        {
                            transmit_packet[i] = 0x00; // SYNC byte and LENGTH bytes are zeros, let the packetizer calculate them
                        }

                        for (int i = 0; i < bytes.Length; i++)
                        {
                            transmit_packet[3 + i] = bytes[i]; // DATA CODE and SUB-DATA CODE and PAYLOAD bytes
                        }

                        transmit_packet[3 + bytes.Length] = 0x00; // CHECKSUM byte is zero, let the packetizer calculate it
                    }
                    else // SYNC + LENGTH + DATA CODE + SUB-DATA CODE + PAYLOAD + CHECKSUM (Whole packet + error correction)
                    {
                        transmit_packet = new byte[bytes.Length];
                        for (int i = 0; i < transmit_packet.Length; i++)
                        {
                            transmit_packet[i] = bytes[i];
                        }
                    }

                    ccdscipkt_tx.FromBytes(transmit_packet);
                    WriteSerialData(ccdscipkt_tx.ToBytes());

                    if (log_enabled)
                    {
                        WritePacketTextbox("TX", "CUSTOM PACKET", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void PacketSendTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                PacketSendButton.PerformClick();
                e.Handled = true;
            }
        }

        private void ccd_bus_send_msg_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CCDBusSendMsgButton.PerformClick();
                e.Handled = true;
            }
        }

        private void sci_bus_send_msg_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SCIBusSendMsgButton.PerformClick();
                e.Handled = true;
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (serial.IsOpen)
            {
                serial.Close();
                DisconnectButton.Text = "Disconnect";

                WriteCommandHistory("Scanner disconnected!");

                ComponentsDisabled();

                serial_rx_buffer.Reset();
                serial_tx_buffer.Reset();

                GC.Collect();

                scanner_found = false;
                scanner_connected = false;
                ConnectButton.Enabled = true;
                DisconnectButton.Enabled = false;
                StatusButton.Enabled = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 1; (i <= 10); i++)
            //while (true)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(100);
                    worker.ReportProgress((i * 10));
                }
            }
        }

        private void RebootScannerButton_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.reboot, PacketManager.ok, null);
            WriteSerialData(ccdscipkt_tx.ToBytes());
            WritePacketTextbox("TX", "REQUEST SCANNER REBOOT", ccdscipkt_tx.ToBytes());
        }

        private void PacketTextbox_TextChanged(object sender, EventArgs e)
        {
            if (PacketTextbox.Text != "")
            {
                PacketClearButton.Enabled = true;
            }
            else
            {
                PacketClearButton.Enabled = false;
            }
        }

        private void PacketLogEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (PacketLogEnabledCheckbox.Checked)
            {
                PacketLogEnabledCheckbox.Text = "Packet log enabled";
                log_enabled = true;
            }
            else
            {
                PacketLogEnabledCheckbox.Text = "Packet log disabled";
                log_enabled = false;
            }
        }

        private void CCDBusMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextbox.Text != "")
            {
                CCDBusClearButton.Enabled = true;
            }
            else
            {
                CCDBusClearButton.Enabled = false;
            }
        }

        private void SCIBusMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextbox.Text != "")
            {
                SCIBusClearButton.Enabled = true;
            }
            else
            {
                SCIBusClearButton.Enabled = false;
            }
        }

        private void CCDMessageFilteringTextbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterTextbox.Text != "")
            {
                CCDBusMsgFilterClearButton.Enabled = true;
                CCDBusMsgFilterApplyButton.Enabled = true;
            }
            else
            {
                CCDBusMsgFilterClearButton.Enabled = false;
                CCDBusMsgFilterApplyButton.Enabled = false;
            }
        }

        private void SCIMessageFilteringTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterTextbox.Text != "")
            {
                SCIBusMsgFilterClearButton.Enabled = true;
                SCIBusMsgFilterApplyButton.Enabled = true;
            }
            else
            {
                SCIBusMsgFilterClearButton.Enabled = false;
                SCIBusMsgFilterClearButton.Enabled = false;
            }
        }

        private void read_dtc_mic_button_Click(object sender, EventArgs e)
        {
            StringBuilder temp_string = new StringBuilder(serial_rx_buffer.Array.Length * 3);
            foreach (byte status_byte in serial_rx_buffer.Array)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            string temp_string_final = temp_string.ToString().ToUpper();

            SensorDataTextbox.Text = temp_string_final;
        }

        private void CCDBusSendMsgButton_Click(object sender, EventArgs e)
        {
            if (CCDBusSendMsgTextbox.Text != "")
            {
                // Convert text to bytes
                byte[] payload = Util.GetBytes(CCDBusSendMsgTextbox.Text);

                if (payload.Length > 0)
                {
                    ccdscipkt_tx.payload = null; // causes a bug if it's not here

                    // Generate packet
                    ccdscipkt_tx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_ccd_bus, PacketManager.send_msg, PacketManager.ok, payload);

                    // Send packet
                    WriteSerialData(ccdscipkt_tx.ToBytes());

                    if (log_enabled)
                    {
                        WritePacketTextbox("TX", "CUSTOM CCD-MESSAGE", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void CCDBusMsgFilterApplyButton_Click(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterTextbox.Text != "")
            {
                byte[] temp = Util.GetBytes(CCDBusMsgFilterTextbox.Text);
                ccd_filter_bytes = new byte[temp.Length];
                Array.Copy(temp, ccd_filter_bytes, temp.Length);
                //if (temp.Length > 0) ccd_filtering_active = true;
            }
            else
            {
                ccd_filter_bytes = null;
                //ccd_filtering_active = false;
            }
        }

        private void CCDBusMsgFilterClearButton_Click(object sender, EventArgs e)
        {
            ccd_filter_bytes = null;
            CCDBusMsgFilterTextbox.Clear();
            //ccd_filtering_active = false;
        }

        private void PacketGeneratorButton_Click(object sender, EventArgs e)
        {
            PacketGenerator packetgenerator = new PacketGenerator(this);
            packetgenerator.Show();
        }
    }
}
