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
        // SerialPortStream object to transmit and receive byte-based packets
        SerialPortStream Serial;

        // Packet Reception handler object
        PacketReceptor Pr;

        // Packet objects
        PacketManager.PacketStructure PacketRx; // for incoming packets
        PacketManager.PacketStructure PacketTx; // for outgoing packets

        // CCDSCI Packet command list object
        //CCDSCIPKT_Commands ccdscipkt_commands = new CCDSCIPKT_Commands();

        // Diagnostic Trouble Code (DTC) lookup table object
        //DTC_LookupTable ccdscipkt_lookuptable = new DTC_LookupTable();

        // Circular buffer objects (FIFO - First In First Out) for easier data management
        CircularBuffer<byte> SerialRxBuffer = new CircularBuffer<byte>(4096);
        CircularBuffer<byte> SerialTxBuffer = new CircularBuffer<byte>(4096);

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

        public bool scanner_connected = false;
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
            
            TimeoutTimer.Elapsed += new ElapsedEventHandler(TimeoutReached);
            TimeoutTimer.Interval = 500; // 500 ms timeout
            TimeoutTimer.Enabled = false;

            ModuleListComboBox.SelectedIndex = 0;
            PCMTCMSelectorComboBox.SelectedIndex = 0;

            LogEnabled = PacketLogEnabledCheckbox.Enabled;

            Pr = new PacketReceptor();
            PacketRx = new PacketManager.PacketStructure();
            PacketTx = new PacketManager.PacketStructure();

            Pr.PropertyChanged += new PropertyChangedEventHandler(PacketReceived);
            PCMTCMSelectorComboBox.MouseWheel += new MouseEventHandler(PCMTCMSelectorComboBox_MouseWheel);

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
            ReadDTCGroupbox.Enabled = false;
            ReadAllDTCGroupbox.Enabled = false;
            MiscGroupbox.Enabled = false;
            EEPROMGroupbox.Enabled = false;
            DTCListGroupbox.Enabled = false;
        }

        public void ComponentsEnabled()
        {
            // Group boxes
            PacketLogGroupbox.Enabled = true;
            CCDBusMessagesGroupbox.Enabled = true;
            SCIBusMessagesGroupbox.Enabled = true;
            SensorDataGroupbox.Enabled = true;
            RealTimeDiagnosticsGroupbox.Enabled = true;
            ReadDTCGroupbox.Enabled = true;
            ReadAllDTCGroupbox.Enabled = true;
            MiscGroupbox.Enabled = true;
            EEPROMGroupbox.Enabled = true;
            DTCListGroupbox.Enabled = true;
        }

        public void TimeoutReached(object source, ElapsedEventArgs e)
        {
            timeout = true;
        }

        // This method gets called everytime when something arrives on the COM-port
        // It searches for valid packets (even if there's multiple packets in one reception) and discards the garbage bytes
        void PacketReceived(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PacketReceived")
            {
                int bytes_to_read = 0;
                bool repeat = true;
                byte[] temp;

                while (repeat)
                {
                    // Find the first byte with value 0x33
                    Here: // This is a goto label
                    while ((SerialRxBuffer.Array[SerialRxBuffer.Start] != PacketManager.SYNC_BYTE) && (SerialRxBuffer.ReadLength > 0))
                    {
                        // If the byte at the "Start" of the circular buffer is not 0x33 then remove it,
                        // the "Start" address advances automatically when the Pop method is called,
                        // so the while loop looks at the next byte.
                        SerialRxBuffer.Pop();
                    }

                    // If all the bytes are consumed then exit from this method (nothing to do anymore, wait for another packet)
                    if (SerialRxBuffer.ReadLength == 0) return;

                    bytes_to_read = ((SerialRxBuffer.Array[SerialRxBuffer.Start + LENGTH_POS] << 8) | SerialRxBuffer.Array[SerialRxBuffer.Start + LENGTH_POS + 1]) + 4;
                    if (bytes_to_read > 2042)
                    {
                        SerialRxBuffer.Pop(); // Pop this byte so we can search for another packet since this is too big (>2042 bytes)
                        goto Here; // Jump back to the while loop to repeat
                    }

                    // Copy the circular buffer array to a temporary array
                    // This is needed because the circular buffer doesn't necessarily start at zero index
                    temp = new byte[bytes_to_read];
                    Array.Copy(SerialRxBuffer.Array, SerialRxBuffer.Start, temp, 0, bytes_to_read);

                    // Try to convert the remainder bytes into a packet
                    if (PacketRx.FromBytes(temp))
                    {
                        // If the conversion is OK then remove them from the circular buffer
                        for (int i = 0; i < bytes_to_read; i++)
                        {
                            SerialRxBuffer.Pop();
                        }
                        ProcessData(temp); // Get these bytes processed by another method
                    }

                    // Don't loop again if there are no bytes left
                    if (SerialRxBuffer.ReadLength == 0)
                    {
                        repeat = false;
                        SerialRxBuffer.Reset();
                        Array.Clear(SerialRxBuffer.Array, 0, SerialRxBuffer.Array.Length);
                    }
                    else // if there are more bytes then loop again
                    {
                        repeat = true;
                    }

                    // Update the misc. values
                    // Packet numbers are updated in the WritePacketTextBox method
                    BufferStartLabel.Text = "Buffer Start: " + SerialRxBuffer.Start;
                    BufferEndLabel.Text = "Buffer End: " + SerialRxBuffer.End;
                    BufferReadlengthLabel.Text = "Buffer ReadLength: " + SerialRxBuffer.ReadLength + " byte(s)";
                    BufferWritelengthLabel.Text = "Buffer WriteLength: " + SerialRxBuffer.WriteLength + " byte(s)";
                }
            }
        }

        private bool GetHandshake()
        {
            try
            {
                Serial.Open(); // the current serial port is previously configured
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.handshake, PacketManager.ok, null);
                WriteSerialData(PacketTx.ToBytes());
                WritePacketTextBox("TX", "REQUEST HANDSHAKE (" + Serial.PortName + ")", PacketTx.ToBytes());

                timeout = false;
                TimeoutTimer.Enabled = true;
                while ((Serial.BytesToRead < 27) && (!timeout))
                {
                    // Wait here until all bytes are received (we know that 27 bytes should be received) or timeout occurs.
                }
                TimeoutTimer.Enabled = false;
                if (timeout)
                {
                    timeout = false;

                    // Manually save the received bytes from the scanner
                    int bytes = Serial.BytesToRead;
                    byte[] data = new byte[bytes];
                    Serial.Read(data, 0, bytes);
                    WritePacketTextBox("RX", "TIMEOUT (" + Serial.PortName + ")", null);
                    Serial.Close();
                    GC.Collect();
                    return false;
                }
                else
                {
                    // Manually save the received bytes from the scanner
                    byte[] data = new byte[27];
                    Serial.Read(data, 0, Serial.BytesToRead);
                    if (Encoding.ASCII.GetString(data, 5, 21) == "CHRYSLERCCDSCISCANNER")
                    {
                        ProcessData(data);
                        return true;
                    }
                    else
                    {
                        Serial.Close();
                        return false;
                    }
                }
            }
            catch
            {
                WriteCommandHistory("Can't open " + Serial.PortName + "!");
                Serial.Close();
                GC.Collect();
                return false;
            }
        }

        public void WriteCommandHistory(string message)
        {

            CommandHistoryTextBox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] ");
            CommandHistoryTextBox.AppendText(message + Environment.NewLine);
            // Scroll down
            CommandHistoryTextBox.SelectionStart = CommandHistoryTextBox.TextLength;
            CommandHistoryTextBox.ScrollToCaret();
        }

        public void WritePacketTextBox(string direction, string description, byte[] message)
        {
            // Get all the lines out as an arry
            string[] lines = PacketTextBox.Lines;

            // If there are "too much lines" then remove some of them from the beginning
            if (lines.Length > 50)
            {
                var newlineslist = lines.ToList();
                newlineslist.RemoveRange(0, 30);

                // And put back what's left
                PacketTextBox.Lines = newlineslist.ToArray();
            }

            // Build the new text separately to avoid heavy textbox flickering
            StringBuilder temp = new StringBuilder();

            // Add stuff
            temp.Append(direction + ": " + description + Environment.NewLine);

            if (message != null)
            {
                // Add the bytes of the message
                foreach (byte bytes in message)
                {
                    temp.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
                }

                // Add two new lines
                temp.Append(Environment.NewLine + Environment.NewLine);
            }
            else 
            {
                // Add one new line
                temp.Append(Environment.NewLine);
            }


            // Add the built string to the textbox in one go
            //PacketTextBox.AppendText(newstuff.ToString());
            PacketTextBox.SelectionStart = PacketTextBox.TextLength;
            PacketTextBox.SelectedText = temp.ToString();

            // Scroll down to the end of the textbox
            //PacketTextBox.SelectionStart = PacketTextBox.TextLength;
            PacketTextBox.ScrollToCaret();

            // Update User Interface
            if (direction == "RX") packet_count_rx++;
            if (direction == "TX") packet_count_tx++;
            PacketCountRxLabel.Text = "Packets received: " + packet_count_rx;
            PacketCountTxLabel.Text = "Packets sent: " + packet_count_tx;

            // Save data to log files
            if (!Directory.Exists("LOG")) Directory.CreateDirectory("LOG");
            File.AppendAllText(TextLogFilename, temp.ToString());

            using (BinaryWriter writer = new BinaryWriter(File.Open(BinaryLogFilename, FileMode.Append)))
            {
                writer.Write(message);
                writer.Close();
            }

            // Discard the temporary string builder
            temp = null;
        }

        public void WriteCCDSCIBusTextBox(TextBox textBox, byte[] message)
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
        public void WriteSendPacketTextBox(byte[] message)
        {
            StringBuilder newstuff = new StringBuilder();

            foreach (byte bytes in message)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            PacketSendTextBox.Text = newstuff.ToString();
            PacketSendTextBox.Refresh();
        }

        public async void ReadSerialData()
        {
            // Put this in an endless loop so it's monitoring the serial port all the time
            for(;;)
            {
                int actual_length = await Serial.ReadAsync(buffer, 0, buffer.Length);
                if (actual_length > 0)
                {
                    SerialRxBuffer.Append(buffer, 0, actual_length); // put whatever arrived in the circular buffer
                    Array.Clear(buffer, 0, buffer.Length); // then clear the source array
                    Pr.PacketReceived = true; // Let the program know first that we have something, added to the circular buffer
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
                await Serial.WriteAsync(message, 0, message.Length);
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
            if (PacketRx.FromBytes(data))
            {
                // Find out what the source and the target of the packet is by examining the high nibble (4 bits)
                byte source = (byte)((PacketRx.datacode[0] >> 6) & 0x03);
                byte target = (byte)((PacketRx.datacode[0] >> 4) & 0x03); // Must be the laptop (0x00)

                // Extract DC command value from the low nibble (4 bits)
                byte dc_command = (byte)(PacketRx.datacode[0] & 0x0F);

                // Get SUB-DATA CODE ready
                byte subdatacode = PacketRx.subdatacode[0];

                switch (source)
                {
                    case PacketManager.from_laptop:
                        {
                            // Packet echoed back
                            WritePacketTextBox("RX", "PACKET ECHO", data);
                            break;
                        }

                    case PacketManager.from_scanner:
                        {
                            switch (dc_command)
                            {
                                case PacketManager.reboot: // 0x00
                                    {
                                        // This is just a reboot confirmation before the actual reboot
                                        WritePacketTextBox("RX", "SCANNER REBOOT COMPLETE", data);
                                        WriteCommandHistory("Scanner reboot complete!");
                                        break;
                                    }
                                case PacketManager.handshake: // 0x01
                                    {
                                        // This is a handshake response, the payload contains an ASCII-encoded string
                                        string received_handshake = Encoding.ASCII.GetString(PacketRx.payload);
                                        WritePacketTextBox("RX", "HANDSHAKE (" + Serial.PortName + ")", data);
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
                                case PacketManager.status: // 0x02
                                    {
                                        // Write out to packet textbox
                                        WritePacketTextBox("RX", "STATUS PACKET", data);
                                        WriteCommandHistory("Status packet received!");

                                        break;
                                    }
                                case PacketManager.settings: // 0x03 - It doesn't seem to be happening too much around here... 
                                    {
                                        switch (subdatacode)
                                        {
                                            case PacketManager.enable_ccd_bus:
                                                {
                                                    if (PacketRx.payload[0] == 0x00) WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    else WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                                    break;
                                                }
                                            case PacketManager.disable_ccd_bus:
                                                {
                                                    if (PacketRx.payload[0] == 0x00) WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    else WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                                    break;
                                                }
                                            case PacketManager.enable_sci_bus:
                                                {
                                                    if (PacketRx.payload[0] == 0x01) WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    else if (PacketRx.payload[0] == 0x02) WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    else WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    break;
                                                }
                                            case PacketManager.disable_sci_bus:
                                                {
                                                    if (PacketRx.payload[0] == 0x00) WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    else WritePacketTextBox("RX", "SETTINGS / OK", data);
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case PacketManager.response: // 0x05; 0x04 is skipped because that's the request datacode from the laptop
                                    {
                                        switch (subdatacode)
                                        {
                                            case PacketManager.free_ram_available:
                                                {
                                                    WritePacketTextBox("RX", "FREE RAM", data);
                                                    WriteCommandHistory("Free RAM value received!");
                                                    break;
                                                }
                                            case PacketManager.mcu_counter_value:
                                                {
                                                    WritePacketTextBox("RX", "MCU COUNTER", data);
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
                                case PacketManager.ok_error: // 0x0F
                                    {
                                        switch (subdatacode)
                                        {
                                            case PacketManager.ok:
                                                {
                                                    WritePacketTextBox("RX", "COMMAND OK", data);
                                                    WriteCommandHistory("Command acknowledged!");
                                                    break;
                                                }

                                        }
                                        break;
                                    }
                                default:
                                    {
                                        WritePacketTextBox("RX", "RECEIVED BYTES", data);
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
                                        WritePacketTextBox("RX", "CCD-BUS MESSAGE", data);

                                        // If filtering is active by ID-bytes (first byte of every message)
                                        if (ccd_filter_bytes != null)
                                        {
                                            // Write only if the ID byte is on the filter list
                                            if (ccd_filter_bytes.Contains(PacketRx.payload[0]))
                                            {
                                                WriteCCDSCIBusTextBox(CCDBusMsgTextBox, PacketRx.payload);
                                            }
                                            else
                                            {
                                                // Ignore this message
                                            }
                                        }
                                        else // Filtering disabled, show everything
                                        {
                                            WriteCCDSCIBusTextBox(CCDBusMsgTextBox, PacketRx.payload);
                                        }

                                        break;
                                    }
                                default:
                                    {
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
                                        WritePacketTextBox("RX", "SCI-BUS MESSAGE", data);

                                        // If filtering is active by ID-bytes (first byte of every message)
                                        if (sci_filter_bytes != null)
                                        {
                                            if (sci_filter_bytes.Contains(PacketRx.payload[0]))
                                            {
                                                WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload);
                                            }
                                            else
                                            {
                                                // Ignore this message
                                            }
                                        }
                                        else // Filtering disabled, show everything
                                        {
                                            WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload);
                                        }

                                        break;
                                    }
                                default:
                                    {
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
                        Serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                        // Try 5 times
                        for (int i = 0; i < 5; i++)
                        {
                            WriteCommandHistory("Connecting...");
                            //serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                            if (GetHandshake())
                            {
                                scanner_connected = true;
                                WriteCommandHistory("Scanner found and connected!");
                                DisconnectButton.Text = "Disconnect (" + Serial.PortName + ")";

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
                                scanner_connected = false;
                                WriteCommandHistory("No scanner found!");
                                ConnectButton.Enabled = true;
                                Serial.Close();
                            }
                        }

                        if (scanner_connected) break;
                        else Serial.Dispose();
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
            if (Serial.IsOpen)
            {
                Serial.Close();
                DisconnectButton.Text = "Disconnect";

                WriteCommandHistory("Scanner disconnected!");

                ComponentsDisabled();

                SerialRxBuffer.Reset();
                SerialTxBuffer.Reset();

                GC.Collect();

                scanner_connected = false;
                //scanner_connected = false;
                ConnectButton.Enabled = true;
                DisconnectButton.Enabled = false;
                StatusButton.Enabled = false;
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.status, PacketManager.ok, null);
            WriteSerialData(PacketTx.ToBytes());
            WritePacketTextBox("TX", "REQUEST STATUS PACKET", PacketTx.ToBytes());
        }

        private void SCIBusHsButton_Click(object sender, EventArgs e)
        {

        }

        private void PacketClearButton_Click(object sender, EventArgs e)
        {
            PacketTextBox.Clear();
            CommandHistoryTextBox.Clear();
        }

        private void CCDBusLogClearButton_Click(object sender, EventArgs e)
        {
            CCDBusMsgTextBox.Clear();
        }

        private void SCIBusLogClearButton_Click(object sender, EventArgs e)
        {
            SCIBusMsgTextBox.Clear();
        }

        private void RealTimeDiagnosticsButton_Click(object sender, EventArgs e)
        {
            DiagnosticsForm diagnostics = new DiagnosticsForm();
            diagnostics.Show();
            //diagnostics.Dispose();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void PacketSendButton_Click(object sender, EventArgs e)
        {
            if (PacketSendTextBox.Text != "") // don't do anything if the textbox is empty!
            {
                byte[] bytes = Util.GetBytes(PacketSendTextBox.Text);
                if ((bytes.Length >= 6) && (bytes[0] != 0x00))
                {
                    PacketTx.FromBytes(bytes); // can't use the GeneratePacket method here because we don't know right away what the user wants
                    byte[] temp = PacketTx.ToBytes(); // the previous FromBytes method corrects the size and checksum errors so this ToBytes method returns with a valid packet
                    WriteSerialData(temp);
                    WritePacketTextBox("TX", "CUSTOM PACKET", temp);
                }
            }
        }

        private void CCDBusSendMsgButton_Click(object sender, EventArgs e)
        {
            if (CCDBusSendMsgTextBox.Text != "")
            {
                byte[] bytes = Util.GetBytes(CCDBusSendMsgTextBox.Text);
                if ((bytes.Length > 0) && (bytes[0] != 0x00))
                {
                    PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_ccd_bus, PacketManager.send_msg, PacketManager.ok, bytes);
                    byte[] temp = PacketTx.ToBytes();
                    WriteSerialData(temp);
                    WritePacketTextBox("TX", "CUSTOM CCD-MESSAGE", temp);
                }
            }
        }

        private void SCIBusSendMsgButton_Click(object sender, EventArgs e)
        {
            if (SCIBusSendMsgTextBox.Text != "")
            {
                byte[] bytes = Util.GetBytes(SCIBusSendMsgTextBox.Text);
                if ((bytes.Length > 0) && (bytes[0] != 0x00))
                {
                    PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_sci_bus, PacketManager.send_msg, PacketManager.ok, bytes);
                    byte[] temp = PacketTx.ToBytes();
                    WriteSerialData(temp);
                    WritePacketTextBox("TX", "CUSTOM SCI-MESSAGE", temp);
                }
            }
        }

        private void CCDBusMsgFilterApplyButton_Click(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterTextBox.Text != "")
            {
                byte[] temp = Util.GetBytes(CCDBusMsgFilterTextBox.Text);
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
            CCDBusMsgFilterTextBox.Clear();
        }

        private void SCIBusMsgFilterApplyButton_Click(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterTextBox.Text != "")
            {
                byte[] temp = Util.GetBytes(SCIBusMsgFilterTextBox.Text);
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
            SCIBusMsgFilterTextBox.Clear();
        }

        private void RebootScannerButton_Click(object sender, EventArgs e)
        {
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.reboot, PacketManager.ok, null);
            WriteSerialData(PacketTx.ToBytes());
            WritePacketTextBox("TX", "REQUEST SCANNER REBOOT", PacketTx.ToBytes());
        }

        private void PacketTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PacketTextBox.Text != "")
            {
                PacketClearButton.Enabled = true;
            }
            else
            {
                PacketClearButton.Enabled = false;
            }
        }

        private void CCDBusMessagesTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextBox.Text != "")
            {
                CCDBusClearMsgButton.Enabled = true;
            }
            else
            {
                CCDBusClearMsgButton.Enabled = false;
            }
        }

        private void SCIBusMessagesTextBox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextBox.Text != "")
            {
                SCIBusClearMsgButton.Enabled = true;
            }
            else
            {
                SCIBusClearMsgButton.Enabled = false;
            }
        }

        private void PacketSendTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                PacketSendButton.PerformClick();
                e.Handled = true;
            }
        }

        private void CCDBusSendMsgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CCDBusSendMsgButton.PerformClick();
                e.Handled = true;
            }
        }

        private void SCIBusSendMsgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SCIBusSendMsgButton.PerformClick();
                e.Handled = true;
            }
        }

        private void CCDBusMsgTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgTextBox.Text != "")
            {
                CCDBusClearMsgButton.Enabled = true;
            }
            else
            {
                CCDBusClearMsgButton.Enabled = false;
            }
        }

        private void SCIBusMsgTextBox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgTextBox.Text != "")
            {
                SCIBusClearMsgButton.Enabled = true;
            }
            else
            {
                SCIBusClearMsgButton.Enabled = false;
            }
        }

        private void CCDMessageFilteringTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterTextBox.Text != "")
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

        private void SCIMessageFilteringTextBox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterTextBox.Text != "")
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

        private void PacketSendTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PacketSendTextBox.Text != "")
            {
                PacketSendButton.Enabled = true;
            }
            else
            {
                PacketSendButton.Enabled = false;
            }
        }

        private void CCDBusSendMsgTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CCDBusSendMsgTextBox.Text != "")
            {
                CCDBusSendMsgButton.Enabled = true;
            }
            else
            {
                CCDBusSendMsgButton.Enabled = false;
            }
        }

        private void SCIBusSendMsgTextBox_TextChanged(object sender, EventArgs e)
        {
            if (SCIBusSendMsgTextBox.Text != "")
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
                CCDBusMsgTextBox.Enabled = true;
                CCDBusSendMsgTextBox.Enabled = true;
                CCDEnabled = true;

                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_ccd_bus, null);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                WritePacketTextBox("TX", "SETTINGS / CCD-BUS ON", temp);
            }
            else
            {
                CCDBusMsgTextBox.Enabled = false;
                CCDBusSendMsgTextBox.Enabled = false;
                CCDBusSendMsgButton.Enabled = false;
                CCDBusClearMsgButton.Enabled = false;
                CCDEnabled = false;

                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.disable_ccd_bus, null);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                WritePacketTextBox("TX", "SETTINGS / CCD-BUS OFF", temp);
            }
        }

        private void SCIBusEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SCIBusEnabledCheckbox.Checked)
            {
                SCIBusMsgTextBox.Enabled = true;
                SCIBusSendMsgTextBox.Enabled = true;
                SCIEnabled = true;

                byte[] SelectedSCIbus = new byte[] { (byte)(PCMTCMSelectorComboBox.SelectedIndex + 1) }; // add 1 because of zero based indexing
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_sci_bus, SelectedSCIbus);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                if (SelectedSCIbus[0] == 0x01) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (PCM)", temp);
                else if (SelectedSCIbus[0] == 0x02) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (TCM)", temp);
            }
            else
            {
                SCIBusMsgTextBox.Enabled = false;
                SCIBusSendMsgTextBox.Enabled = false;
                SCIBusSendMsgButton.Enabled = false;
                SCIBusClearMsgButton.Enabled = false;
                SCIEnabled = false;

                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.disable_sci_bus, new byte[] { 0x00 });
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                WritePacketTextBox("TX", "SETTINGS / SCI-BUS OFF", temp);
            }
        }

        private void PacketLogEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (PacketLogEnabledCheckbox.Checked)
            {
                PacketTextBox.Enabled = true;
                PacketSendTextBox.Enabled = true;
                LogEnabled = true;
            }
            else
            {
                PacketTextBox.Enabled = false;
                PacketSendTextBox.Enabled = false;
                LogEnabled = false;
            }
        }

        private void CCDBusMsgFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CCDBusMsgFilterCheckbox.Checked)
            {
                CCDBusMsgFilterTextBox.Enabled = true;
                if (CCDBusMsgFilterTextBox.Text != "") CCDBusMsgFilterApplyButton.Enabled = true;
                if (CCDBusMsgFilterTextBox.Text != "") CCDBusMsgFilterClearButton.Enabled = true;
            }
            else
            {
                CCDBusMsgFilterTextBox.Enabled = false;
                CCDBusMsgFilterApplyButton.Enabled = false;
                CCDBusMsgFilterClearButton.Enabled = false;
            }
        }

        private void SCIBusMsgFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterCheckbox.Checked)
            {
                SCIBusMsgFilterTextBox.Enabled = true;
                if (SCIBusMsgFilterTextBox.Text != "") SCIBusMsgFilterApplyButton.Enabled = true;
                if (SCIBusMsgFilterTextBox.Text != "") SCIBusMsgFilterClearButton.Enabled = true;
            }
            else
            {
                SCIBusMsgFilterTextBox.Enabled = false;
                SCIBusMsgFilterApplyButton.Enabled = false;
                SCIBusMsgFilterClearButton.Enabled = false;
            }
        }

        private void packetGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketGenerator packetgenerator = new PacketGenerator(this);
            packetgenerator.Show(); // open a new window
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm(this);
            about.Show();
        }

        private void superCardReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "d:\\Dokumentumok\\Chrysler\\CCD\\LOG\\";
            dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;

            // Here is a file selected from the dialog box
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        scReader = new SimpleBinaryReader(fs);
                    }

                    int pos = 0;
                    bool done = false;
                    byte[] temp;
                    byte[] file_start_signature = new byte[] { 0x4E, 0x75, 0x26, 0x28, 0x23, 0x29 };

                    // Search for "file start signatures" using the raw byte field inside SimpleBinaryReader
                    while (!done)
                    {
                        pos = Util.SearchBytes(scReader.rawDB, pos, file_start_signature);
                        if (pos != -1) // the SearchBytes method returns -1 if there's nothing more to find
                        {
                            temp = scReader.ReadBytes(ref pos, 30);
                            SensorDataTextBox.AppendText(Encoding.ASCII.GetString(temp, 7, 23) + "\n");
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
            GC.Collect();
        }

        private void PCMTCMSelectorComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            byte[] SelectedSCIbus = new byte[] { (byte)(PCMTCMSelectorComboBox.SelectedIndex + 1) }; // add 1 because of zero based indexing
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_sci_bus, SelectedSCIbus);
            byte[] temp = PacketTx.ToBytes();
            WriteSerialData(temp);
            if (SelectedSCIbus[0] == 0x01) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (PCM)", temp);
            else if (SelectedSCIbus[0] == 0x02) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (TCM)", temp);
            
        }
        
        // Prevent mousewheel scrolling through this combobox
        void PCMTCMSelectorComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }
}
