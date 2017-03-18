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
using ChryslerCCDSCIScanner_GUI;

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

        public SimpleBinaryReader scReader; // store SuperCard data here

        public const byte SYNC_POS = 0;
        public const byte LENGTH_POS = 1;
        public const byte DATACODE_POS = 3;
        public const byte SUBDATACODE_POS = 4;

        int packet_count_rx = 0;
        int packet_count_tx = 0;

        public bool CCDEnabled = false;
        public bool SCIEnabled = false;
        public bool LogEnabled = true;

        public bool scanner_found = false;
        public bool timeout = false;

        byte[] buffer = new byte[4096];

        public string DateTimeNow;
        public string TextLogFilename;
        public string BinaryLogFilename;

        public byte[] ccd_filter_bytes;
        public bool ccd_filtering_active = false;

        public byte[] sci_filter_bytes;
        public bool sci_filtering_active = false;

        // Class constructor
        public MainForm()
        {
            InitializeComponent();

            ComponentsDisabled();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            pr.PropertyChanged += new PropertyChangedEventHandler(PacketReceived);
            TimeoutTimer.Elapsed += new ElapsedEventHandler(TimeoutReached);
            TimeoutTimer.Interval = 200;
            TimeoutTimer.Enabled = false;

            SCIEnabled = PacketLogEnabledCheckbox.Enabled;

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
                        Array.Copy(serial_rx_buffer.Array, serial_rx_buffer.Start, temp_buffer, 0, bytes_to_read); // get a copy of some bytes

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
                WritePacketTextbox("TX", "REQUEST HANDSHAKE (" + serial.PortName + ")", ccdscipkt_tx.ToBytes());

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
                    GC.Collect();
                    return false;
                }
                else
                {
                    // Manually save the received bytes from the scanner
                    byte[] data = new byte[27];
                    serial.Read(data, 0, serial.BytesToRead);
                    if (Encoding.ASCII.GetString(data, 5, 21) == "CHRYSLERCCDSCISCANNER")
                    {
                        ProcessData(data);
                        return true;
                    }
                    else
                    {
                        serial.Close();
                        return false;
                    }
                }
            }
            catch
            {
                WriteCommandHistory("Can't open " + serial.PortName + "!");
                serial.Close();
                GC.Collect();
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

        public void WritePacketTextbox(string direction, string description, byte[] message)
        {
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
            newstuff.Append(direction + ": " + description + Environment.NewLine);

            if (message != null)
            {
                // Add the bytes of the message
                foreach (byte bytes in message)
                {
                    newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
                }

                // Add two new lines
                newstuff.Append(Environment.NewLine + Environment.NewLine);
            }
            else 
            {
                // Add one new line
                newstuff.Append(Environment.NewLine);
            }


            // Add the built string to the textbox in one go
            PacketTextbox.AppendText(newstuff.ToString());

            // Scroll down to the end of the textbox
            PacketTextbox.SelectionStart = PacketTextbox.TextLength;
            PacketTextbox.ScrollToCaret();

            // Update User Interface
            if (direction == "RX") packet_count_rx++;
            if (direction == "TX") packet_count_tx++;
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

        public void WriteCCDSCIBusTextbox(TextBox textBox, byte[] message)
        {
            // Get all the lines out as an arry
            string[] lines = textBox.Lines;

            // If there are "too much lines" then remove some of them from the beginning
            if (lines.Length > 50)
            {
                var newlineslist = lines.ToList();
                newlineslist.RemoveRange(0, 30);

                // And put back what's left
                textBox.Lines = newlineslist.ToArray();
            }

            // Build the new text separately to avoid heavy textbox flickering
            StringBuilder newstuff = new StringBuilder();

            // Add the bytes of the message
            foreach (byte bytes in message)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            // Add new line
            newstuff.Append(Environment.NewLine);

            // Add the built string to the textbox in one go
            textBox.AppendText(newstuff.ToString());

            // Scroll down to the end of the textbox
            textBox.SelectionStart = textBox.TextLength;
            textBox.ScrollToCaret();

            // Discard the temporary string builder
            newstuff = null;
        }

        // Used for crossform communication only
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

        public async void ReadSerialData()
        {
            // Put this in an endless loop so it's monitoring the serial port all the time
            for(;;)
            {
                int actual_length = await serial.ReadAsync(buffer, 0, buffer.Length);
                if (actual_length > 0)
                {
                    serial_rx_buffer.Append(buffer, 0, actual_length); // put whatever arrived in the circular buffer
                    Array.Clear(buffer, 0, buffer.Length); // then clear the source array

                    pr.PacketReceived = true; // Let the program know first that we have something, handle it

                    // Then proceed to update some values
                    buffer_start_label.Text = "Buffer Start: " + serial_rx_buffer.Start;
                    buffer_end_label.Text = "Buffer End: " + serial_rx_buffer.End;
                    buffer_readlength_label.Text = "Buffer ReadLength: " + serial_rx_buffer.ReadLength + " byte(s)";
                    buffer_writelength_label.Text = "Buffer WriteLength: " + serial_rx_buffer.WriteLength + " byte(s)";
                }
                if (scanner_found == false) break; // exit this loop when serial port is closed (high cpu usage!)
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
                            WritePacketTextbox("RX", "PACKET ECHO", data);
                            break;
                        }

                    case PacketManager.from_scanner:
                        {
                            switch (dc_command)
                            {
                                case PacketManager.reboot:
                                    {
                                        // This is just a reboot confirmation before the actual reboot
                                        WritePacketTextbox("RX", "SCANNER REBOOT COMPLETE", data);
                                        WriteCommandHistory("Scanner reboot complete!");
                                        break;
                                    }
                                case PacketManager.handshake:
                                    {
                                        // This is a handshake response, the payload contains an ASCII-encoded string
                                        string received_handshake = Encoding.ASCII.GetString(ccdscipkt_rx.payload);
                                        WritePacketTextbox("RX", "HANDSHAKE (" + serial.PortName + ")", data);
                                        if (received_handshake == "CHRYSLERCCDSCISCANNER")
                                        {
                                            WriteCommandHistory("Handshake OK: " + received_handshake);
                                        }
                                        else
                                        {
                                            WriteCommandHistory("Handshake ER: " + received_handshake);
                                        }
                                        break;
                                    }
                                case PacketManager.status:
                                    {
                                        // Write out to packet textbox
                                        WritePacketTextbox("RX", "STATUS PACKET", data);
                                        WriteCommandHistory("Status packet received!");

                                        break;
                                    }
                                case PacketManager.response: // General request responses
                                    {
                                        switch (ccdscipkt_rx.subdatacode[0])
                                        {
                                            case 0x07:
                                                {
                                                    WritePacketTextbox("RX", "FREE RAM", data);
                                                    WriteCommandHistory("Free RAM value received!");
                                                    break;
                                                }
                                            case 0x08:
                                                {
                                                    WritePacketTextbox("RX", "MCU COUNTER", data);
                                                    WriteCommandHistory("MCU counter value received!");
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
                                        WriteCommandHistory("Unknown bytes received!");
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

                                        // If filtering is active by ID-bytes (first byte of every message)
                                        if (ccd_filter_bytes != null)
                                        {
                                            // Write only if the ID byte is on the filter list
                                            if (ccd_filter_bytes.Contains(ccdscipkt_rx.payload[0]))
                                            {
                                                WriteCCDSCIBusTextbox(CCDBusMsgTextbox, ccdscipkt_rx.payload);
                                            }
                                            else
                                            {
                                                // Ignore this message
                                            }
                                        }
                                        else // Filtering disabled, show everything
                                        {
                                            WriteCCDSCIBusTextbox(CCDBusMsgTextbox, ccdscipkt_rx.payload);
                                        }

                                        break;
                                    }
                            }
                            break;
                        }
                    case PacketManager.from_sci_bus:
                        {
                            switch (dc_command)
                            {
                                case PacketManager.receive_msg:
                                    {
                                        WritePacketTextbox("RX", "SCI-BUS MESSAGE", data);

                                        // If filtering is active by ID-bytes (first byte of every message)
                                        if (sci_filter_bytes != null)
                                        {
                                            if (sci_filter_bytes.Contains(ccdscipkt_rx.payload[0]))
                                            {
                                                WriteCCDSCIBusTextbox(SCIBusMsgTextbox, ccdscipkt_rx.payload);
                                            }
                                            else
                                            {
                                                // Ignore this message
                                            }
                                        }
                                        else // Filtering disabled, show everything
                                        {
                                            WriteCCDSCIBusTextbox(SCIBusMsgTextbox, ccdscipkt_rx.payload);
                                        }

                                        break;
                                    }
                            }
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
                    WriteCommandHistory("No scanner available!");
                    ConnectButton.Enabled = true;
                }
                else
                {
                    foreach (string port in ports)
                    {
                        serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                        // Try 5 times
                        for (int i = 0; i < 5; i++)
                        {
                            WriteCommandHistory("Connecting...");
                            //serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                            if (GetHandshake())
                            {
                                scanner_found = true;
                                WriteCommandHistory("Scanner found and connected!");
                                DisconnectButton.Text = "Disconnect (" + serial.PortName + ")";

                                ComponentsEnabled();

                                ReadSerialData();
                                ConnectButton.Enabled = false;
                                DisconnectButton.Enabled = true;
                                StatusButton.Enabled = true;
                                StatusButton.PerformClick();
                                break;
                            }
                            else
                            {
                                scanner_found = false;
                                WriteCommandHistory("No scanner found!");
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
                //scanner_connected = false;
                ConnectButton.Enabled = true;
                DisconnectButton.Enabled = false;
                StatusButton.Enabled = false;
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.status, PacketManager.ok, null);
            WriteSerialData(ccdscipkt_tx.ToBytes());
            WritePacketTextbox("TX", "REQUEST STATUS PACKET", ccdscipkt_tx.ToBytes());
        }

        private void SCIBusSendMsgButton_Click(object sender, EventArgs e)
        {
            if (SCIEnabled)
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

                    if (SCIEnabled)
                    {
                        WritePacketTextbox("TX", "MESSAGE TO SCI-BUS", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void SCIBusHsButton_Click(object sender, EventArgs e)
        {

        }

        private void PacketClearButton_Click(object sender, EventArgs e)
        {
            PacketTextbox.Clear();
        }

        private void CCDBusLogClearButton_Click(object sender, EventArgs e)
        {
            CCDBusMsgTextbox.Clear();
        }

        private void SCIBusLogClearButton_Click(object sender, EventArgs e)
        {
            SCIBusMsgTextbox.Clear();
        }

        private void RealTimeDiagnosticsButton_Click(object sender, EventArgs e)
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
            Environment.Exit(0);
        }

        private void PacketSendButton_Click(object sender, EventArgs e)
        {
            if (PacketSendTextbox.Text != "")
            {
                byte[] bytes = Util.GetBytes(PacketSendTextbox.Text);
                if (bytes.Length >= 6)
                {
                    ccdscipkt_tx.payload = null; // causes a bug if it's not here
                    ccdscipkt_tx.FromBytes(bytes); // this method corrects the checksum mismatch
                    byte[] temp = ccdscipkt_tx.ToBytes();
                    WriteSerialData(temp);
                    WritePacketTextbox("TX", "CUSTOM PACKET", temp);
                }
            }
        }

        private void ReadDTCPCMButton_Click(object sender, EventArgs e)
        {

        }

        private void ReadDTCBCMButton_Click(object sender, EventArgs e)
        {

        }

        private void ReadDTCACMButton_Click(object sender, EventArgs e)
        {

        }

        private void ReadDTCABSButton_Click(object sender, EventArgs e)
        {

        }

        private void ReadDTCTCMButton_Click(object sender, EventArgs e)
        {

        }

        private void ReadDTCMICButton_Click(object sender, EventArgs e)
        {

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
                    byte[] temp = ccdscipkt_tx.ToBytes();
                    WriteSerialData(temp);
                    WritePacketTextbox("TX", "CUSTOM CCD-MESSAGE", temp);
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
            }
            else
            {
                ccd_filter_bytes = null;
            }
        }

        private void CCDBusMsgFilterClearButton_Click(object sender, EventArgs e)
        {
            ccd_filter_bytes = null;
            CCDBusMsgFilterTextbox.Clear();
        }

        private void SCIBusMsgFilterApplyButton_Click(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterTextbox.Text != "")
            {
                byte[] temp = Util.GetBytes(SCIBusMsgFilterTextbox.Text);
                sci_filter_bytes = new byte[temp.Length];
                Array.Copy(temp, sci_filter_bytes, temp.Length);
            }
            else
            {
                ccd_filter_bytes = null;
            }
        }

        private void SCIBusMsgFilterClearButton_Click(object sender, EventArgs e)
        {
            sci_filter_bytes = null;
            SCIBusMsgFilterTextbox.Clear();
        }

        private void PacketGeneratorButton_Click(object sender, EventArgs e)
        {
            PacketGenerator packetgenerator = new PacketGenerator(this);
            packetgenerator.Show(); // open a new window
        }

        private void SuperCardButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            byte[] first_line;

            //openFileDialog1.InitialDirectory = "d:\\Dokumentumok\\Chrysler\\CCD\\LOG\\";
            dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;


            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Read the file into memory but only once!
                    if (scReader == null)
                    {
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            scReader = new SimpleBinaryReader(fs);
                        }
                    }

                    int pos = 0;
                    bool done = false;

                    // Search for "file start signatures" using the raw byte field inside SimpleBinaryReader
                    while (!done)
                    {
                        pos = Util.SearchBytes(scReader.rawDB, pos, new byte[] { 0x4E, 0x75, 0x26, 0x28, 0x23, 0x29 });
                        //pos = Util.SearchBytes(scReader.rawDB, pos, new byte[] { 0x4E, 0x75 });
                        if (pos != -1)
                        {
                            first_line = scReader.ReadBytes(ref pos, 30);
                            SensorDataTextbox.AppendText(Encoding.ASCII.GetString(first_line, 7, 23) + "\n");
                        }
                        else
                        {
                            done = true;
                        }
                    }

                } // end try

                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
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

        private void CCDBusMessagesTextbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextbox.Text != "")
            {
                CCDBusClearMsgButton.Enabled = true;
            }
            else
            {
                CCDBusClearMsgButton.Enabled = false;
            }
        }

        private void SCIBusMessagesTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextbox.Text != "")
            {
                SCIBusClearMsgButton.Enabled = true;
            }
            else
            {
                SCIBusClearMsgButton.Enabled = false;
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

        private void CCDBusSendMsgTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CCDBusSendMsgButton.PerformClick();
                e.Handled = true;
            }
        }

        private void SCIBusSendMsgTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SCIBusSendMsgButton.PerformClick();
                e.Handled = true;
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

        private void CCDBusMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextbox.Text != "")
            {
                CCDBusClearMsgButton.Enabled = true;
            }
            else
            {
                CCDBusClearMsgButton.Enabled = false;
            }
        }

        private void SCIBusMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextbox.Text != "")
            {
                SCIBusClearMsgButton.Enabled = true;
            }
            else
            {
                SCIBusClearMsgButton.Enabled = false;
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
                SCIBusMsgFilterApplyButton.Enabled = false;
            }
        }

        private void PacketSendTextbox_TextChanged(object sender, EventArgs e)
        {
            if (PacketSendTextbox.Text != "")
            {
                PacketSendButton.Enabled = true;
            }
            else
            {
                PacketSendButton.Enabled = false;
            }
        }

        private void CCDBusSendMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusSendMsgTextbox.Text != "")
            {
                CCDBusSendMsgButton.Enabled = true;
            }
            else
            {
                CCDBusSendMsgButton.Enabled = false;
            }
        }

        private void SCIBusSendMsgTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusSendMsgTextbox.Text != "")
            {
                SCIBusSendMsgButton.Enabled = true;
            }
            else
            {
                SCIBusSendMsgButton.Enabled = false;
            }
        }

        private void CCDBusEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CCDBusEnabledCheckbox.Checked)
            {
                CCDBusEnabledCheckbox.Text = "CCD-bus enabled";
                CCDBusMsgTextbox.Enabled = true;
                CCDBusSendMsgTextbox.Enabled = true;
                CCDEnabled = true;
            }
            else
            {
                CCDBusEnabledCheckbox.Text = "CCD-bus disabled";
                CCDBusMsgTextbox.Enabled = false;
                CCDBusSendMsgTextbox.Enabled = false;
                CCDBusSendMsgButton.Enabled = false;
                CCDBusClearMsgButton.Enabled = false;
                CCDEnabled = false;
            }
        }

        private void SCIBusEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SCIBusEnabledCheckbox.Checked)
            {
                SCIBusEnabledCheckbox.Text = "SCI-bus enabled";
                SCIBusMsgTextbox.Enabled = true;
                SCIBusSendMsgTextbox.Enabled = true;
                SCIEnabled = true;
            }
            else
            {
                SCIBusEnabledCheckbox.Text = "SCI-bus disabled";
                SCIBusMsgTextbox.Enabled = false;
                SCIBusSendMsgTextbox.Enabled = false;
                SCIBusSendMsgButton.Enabled = false;
                SCIBusClearMsgButton.Enabled = false;
                SCIEnabled = false;
            }
        }

        private void PacketLogEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (PacketLogEnabledCheckbox.Checked)
            {
                PacketLogEnabledCheckbox.Text = "Packet log enabled";
                PacketTextbox.Enabled = true;
                PacketSendTextbox.Enabled = true;
                LogEnabled = true;
            }
            else
            {
                PacketLogEnabledCheckbox.Text = "Packet log disabled";
                PacketTextbox.Enabled = false;
                PacketSendTextbox.Enabled = false;
                LogEnabled = false;
            }
        }

        private void CCDBusMsgFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterCheckbox.Checked)
            {
                CCDBusMsgFilterTextbox.Enabled = true;
                if (CCDBusMsgFilterTextbox.Text != "") CCDBusMsgFilterApplyButton.Enabled = true;
                if (CCDBusMsgFilterTextbox.Text != "") CCDBusMsgFilterClearButton.Enabled = true;
            }
            else
            {
                CCDBusMsgFilterTextbox.Enabled = false;
                CCDBusMsgFilterApplyButton.Enabled = false;
                CCDBusMsgFilterClearButton.Enabled = false;
            }
        }

        private void SCIBusMsgFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterCheckbox.Checked)
            {
                SCIBusMsgFilterTextbox.Enabled = true;
                if (SCIBusMsgFilterTextbox.Text != "") SCIBusMsgFilterApplyButton.Enabled = true;
                if (SCIBusMsgFilterTextbox.Text != "") SCIBusMsgFilterClearButton.Enabled = true;
            }
            else
            {
                SCIBusMsgFilterTextbox.Enabled = false;
                SCIBusMsgFilterApplyButton.Enabled = false;
                SCIBusMsgFilterClearButton.Enabled = false;
            }
        }
    }
}
