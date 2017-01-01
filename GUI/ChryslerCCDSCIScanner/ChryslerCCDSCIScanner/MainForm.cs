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
using RJCP.IO.Ports;
using RJCP.Datastructures;

namespace ChryslerCCDSCIScanner
{
    public partial class MainForm : Form
    {
        private DiagnosticsForm diagnostics;
        private AboutForm about;

        // SerialPortStream object to transmit and receive byte-based messages
        SerialPortStream serial = new SerialPortStream();
        //SerialPort serial = new SerialPort();

        DataReception dr = new DataReception();

        // CCDSCI Packet objects to parse byte arrays
        CCDSCIPKT.CCDSCI_PACKET ccdscipkt_rx = new CCDSCIPKT.CCDSCI_PACKET();
        CCDSCIPKT.CCDSCI_PACKET ccdscipkt_tx = new CCDSCIPKT.CCDSCI_PACKET();

        // CCDSCI Packet command list object
        CCDSCIPKT_Commands ccdscipkt_commands = new CCDSCIPKT_Commands();

        // CCDSCI Packet lookup table object
        CCDSCIPKT_LookupTable ccdscipkt_lookuptable = new CCDSCIPKT_LookupTable();

        // Circular buffer objects (FIFO - First In First Out) for easier data management
        CircularBuffer<byte> serial_rx_buffer = new CircularBuffer<byte>(4096);
        CircularBuffer<byte> serial_tx_buffer = new CircularBuffer<byte>(4096);

        System.Timers.Timer timeout_timer = new System.Timers.Timer();
        
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

        //public byte bcm_hb = 0;
        //public byte bcm_lb = 0;
        //public byte bcm_cs = 0;

        public bool ccd_enabled = true;
        public bool sci_enabled = true;
        public bool log_enabled = true;

        public bool bcm_memory_read = false;
        public bool pcm_memory_read = false;
        
        public bool sci_highspeed_enabled = false;
        public bool pcm_ram_read = false;
        public bool pcm_o2_sensor_read = false;
        public bool pcm_map_sensor_read = false;
        public bool pcm_read_all_parameters = false;

        public bool scanner_found = false;
        public bool scanner_connected = false;
        public bool timeout = false;
        public bool start2 = false;

        byte[] buffer = new byte[4096];

        StringBuilder packet_log_textbox_string = new StringBuilder();

        public DateTime uc_time;

        public MainForm()
        {
            InitializeComponent();
            default_button_states();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            //serial.DiscardInBuffer();

            dr.PropertyChanged += new PropertyChangedEventHandler(dr_PropertyChanged);

            timeout_timer.Elapsed += new ElapsedEventHandler(timeout_reached);
            timeout_timer.Interval = 200;
            timeout_timer.Enabled = false;
        }

        public void default_button_states()
        {
            connect_button.Enabled = true;
            disconnect_button.Enabled = false;
            status_button.Enabled = false;
            exit_button.Enabled = true;

            packet_log_save_button.Enabled = false;
            packet_log_clear_button.Enabled = false;
            packet_send_button.Enabled = false;
            packet_send_textbox.Enabled = false;
            packet_log_textbox.Enabled = true;

            ccd_bus_log_save_button.Enabled = false;
            ccd_bus_log_clear_button.Enabled = false;
            ccd_bus_send_msg_button.Enabled = false;
            ccd_bus_send_msg_textbox.Enabled = false;
            ccd_bus_msg_richtextbox.Enabled = true;

            sci_bus_log_save_button.Enabled = false;
            sci_bus_log_clear_button.Enabled = false;
            sci_bus_send_msg_button.Enabled = false;
            sci_bus_send_msg_textbox.Enabled = false;
            sci_bus_msg_richtextbox.Enabled = true;

            read_inteeprom_button.Enabled = false;
            write_inteeprom_button.Enabled = false;
            save_inteeprom_button.Enabled = false;
            erase_inteeprom_button.Enabled = false;

            read_exteeprom_button.Enabled = false;
            write_exteeprom_button.Enabled = false;
            save_exteeprom_button.Enabled = false;
            erase_exteeprom_button.Enabled = false;
        }

        public void timeout_reached(object source, ElapsedEventArgs e)
        {
            timeout = true;
        }

        /// <summary>
        /// Serial data reception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (dr.DataReceived && e.PropertyName == "DataReceived")
            {
                int bytes_to_read = 0;
                int available_packets = 0;
                int result = 0;
                bool first_real_sync_found = false;
                dr.DataReceived = false; // re-arm detection

                // Find the first valid packet
                while (!first_real_sync_found)
                {
                    // Find the first byte with value 0x33
                    while ((serial_rx_buffer.Array[serial_rx_buffer.Start] != CCDSCIPKT.SYNC_BYTE) && (serial_rx_buffer.ReadLength > 0))
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
                        while ((serial_rx_buffer.Array[current_pointer] != CCDSCIPKT.SYNC_BYTE) && (current_pointer <= current_bytes_in_buffer))
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
                            current_pointer++; // pop this false sync byte out of existence!
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
                            while ((serial_rx_buffer.Array[serial_rx_buffer.Start] != CCDSCIPKT.SYNC_BYTE) && (serial_rx_buffer.ReadLength > 0))
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
                        process_data(temp_read_buffer); // TODO

                        first_real_sync_found = false;
                    }

                    // Flush buffer (if there are remaining bytes they are 100% useless)
                    // TODO: some kind of timeout to detect half packets received
                    serial_rx_buffer.Reset();
                    Array.Clear(serial_rx_buffer.Array, 0, serial_rx_buffer.Array.Length);
                }
            }
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            connect_button.Enabled = false;
            try
            {
                // Get available COM ports
                string[] ports = SerialPortStream.GetPortNames();
                //string[] ports = SerialPort.GetPortNames();

                // Cycle through all of them and try to get a handshake (don't be shy!)
                foreach (string port in ports)
		        {
                    // Try 10 times, nothing to lose
                    for (int i = 0; i < 5; i++)
                    {
                        serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                        //serial = new SerialPort(port, 250000, Parity.None, 8, StopBits.One);
                        if (get_scanner_handshake())
                        {
                            scanner_found = true;
                            read_serial_data(); // Initiate automatic packet reception
                            connect_button.Enabled = false;
                            status_button.Enabled = true;
                            status_button.PerformClick();
                            break;
                        }
                        else
                        {
                            scanner_found = false;
                            connect_button.Enabled = true;
                        }
                    }

                    if (scanner_found) break;
                }

                //serial = new SerialPortStream("COM11", 250000, 8, Parity.None, StopBits.One);
                //get_scanner_handshake();

                if (ports.Length == 0)
                {
                    //write_packet_textbox(new byte[1] { 0xFF }, 2, "NO COM PORTS AVAILABLE!");
                    write_command_history("Error: no COM ports are available!");
                    connect_button.Enabled = true;
                }

                textBox3.Text = ccdscipkt_lookuptable.lookup_pcm_dtc(0x08).ToString();
            }
            catch (Exception ex)
            {
                connect_button.Enabled = true;
            }

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private bool get_scanner_handshake()
        {
            try
            {
                // Manually save the received bytes from the scanner
                serial.Open();
                write_serial_data(ccdscipkt_commands.request_scanner_handshake);
                write_packet_textbox("TX", "REQUEST HANDSHAKE (" + serial.PortName + ")", ccdscipkt_commands.request_scanner_handshake);

                timeout = false;
                timeout_timer.Enabled = true;
                while ((serial.BytesToRead < 27) && (!timeout))
                {
                    // Wait here until all bytes are received (we know that 27 bytes should be received) or timeout occurs.
                }
                timeout_timer.Enabled = false;
                if (timeout)
                {
                    timeout = false;
                    int bytes = serial.BytesToRead;
                    byte[] data = new byte[bytes];
                    serial.Read(data, 0, bytes);
                    write_packet_textbox("RX", "ERROR (" + serial.PortName + ")", data);
                    serial.Close();
                    return false;
                }
                else
                {
                    byte[] data = new byte[27];
                    serial.Read(data, 0, serial.BytesToRead);

                    if (data[5]  == 0x43 &&   // C
                        data[6]  == 0x48 &&   // H
                        data[7]  == 0x52 &&   // R
                        data[8]  == 0x59 &&   // Y
                        data[9]  == 0x53 &&   // S
                        data[10] == 0x4C &&   // L
                        data[11] == 0x45 &&   // E
                        data[12] == 0x52 &&   // R
                        data[13] == 0x43 &&   // C
                        data[14] == 0x43 &&   // C
                        data[15] == 0x44 &&   // D
                        data[16] == 0x53 &&   // S
                        data[17] == 0x43 &&   // C
                        data[18] == 0x49 &&   // I
                        data[19] == 0x53 &&   // S
                        data[20] == 0x43 &&   // C
                        data[21] == 0x41 &&   // A
                        data[22] == 0x4E &&   // N
                        data[23] == 0x4E &&   // N
                        data[24] == 0x45 &&   // E
                        data[25] == 0x52)     // R
                    {
                        write_packet_textbox("RX", "HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                        return true;
                    }
                    else
                    {
                        write_packet_textbox("RX", "NO VALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                        serial.Close();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                serial.Close();
                return false;
            }
        }

        public void write_command_history(string message)
        {
            command_history_textbox.AppendText(message + Environment.NewLine);
            // Scroll down
            command_history_textbox.SelectionStart = command_history_textbox.TextLength;
            command_history_textbox.ScrollToCaret();
        }

        public void write_packet_textbox(string rxtx, string description, byte[] message)
        {
            // Suspend Layout
            packet_log_textbox.SuspendLayout();
            
            // Get all the lines out as an arry
            string[] lines = packet_log_textbox.Lines;
            
            // If there are "too much lines" then remove some of them from the beginning
            if (lines.Length > 50)
            {
                var newlineslist = lines.ToList();
                newlineslist.RemoveRange(0, 30);

                // And put back what's left
                packet_log_textbox.Lines = newlineslist.ToArray();
            }

            // Build the new text separately to avoid heavy textbox flickering
            StringBuilder newstuff = new StringBuilder();

            // Add stuff
            newstuff.Append(rxtx + ": " + description + Environment.NewLine);

            // Add the bytes of the message
            foreach (byte bytes in message)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            // Add two new lines
            newstuff.Append(Environment.NewLine + Environment.NewLine);

            // Add the built string to the textbox in one go
            packet_log_textbox.AppendText(newstuff.ToString());

            // Discard the temporary string builder
            newstuff = null;

            // Scroll down to the end of the textbox
            packet_log_textbox.SelectionStart = packet_log_textbox.TextLength;
            packet_log_textbox.ScrollToCaret();

            // Refresh
            packet_log_textbox.Refresh();

            // Resume Layout
            packet_log_textbox.ResumeLayout();

            // Force the Garbage Collector to clean up the mess I just made
            //GC.Collect();

            // Update User Interface
            if (rxtx == "RX") packet_count_rx++;
            if (rxtx == "TX") packet_count_tx++;
            packet_count_rx_label.Text = "Packets received: " + packet_count_rx;
            packet_count_tx_label.Text = "Packets sent: " + packet_count_tx;

        }


        /// <summary>
        /// A simple async serial transmitter.
        /// </summary>
        /// <param name="message">Message to send (byte array)</param>
        public async void write_serial_data(byte[] message)
        {
            try
            {
                await serial.WriteAsync(message, 0, message.Length);
                //await serial.BaseStream.WriteAsync(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                write_command_history("Error: " + ex.ToString() + "\n");
            }
        }

        /// <summary>
        /// A simple async serial receiver that monitors the serial port all the time.
        /// </summary>
        public async void read_serial_data()
        {
            // Put this in an endless loop so it's monitoring the serial port all the time
            for(;;)
            {
                int actual_length = await serial.ReadAsync(buffer, 0, buffer.Length);
                //int actual_length = await serial.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                if (actual_length > 0)
                {
                    serial_rx_buffer.Append(buffer, 0, actual_length);
                    Array.Clear(buffer, 0, buffer.Length);
                    actual_length = 0;
                    dr.DataReceived = true; // let the program know that we have something

                    buffer_start_label.Text = "Buffer Start: " + serial_rx_buffer.Start;
                    buffer_end_label.Text = "Buffer End: " + serial_rx_buffer.End;
                    buffer_readlength_label.Text = "Buffer ReadLength: " + serial_rx_buffer.ReadLength + " byte(s)";
                    buffer_writelength_label.Text = "Buffer WriteLength: " + serial_rx_buffer.WriteLength + " byte(s)";
                }
                Application.DoEvents();
            }
        }


        /// <summary>
        /// Data processing function.
        /// </summary>
        /// <param name="data">Message to be analized.</param>
        public void process_data(byte[] data)
        {
            // Make sure it's a valid CCDSCI Packet (the "FromBytes" function must return a true value)
            if (ccdscipkt_rx.FromBytes(data))
            {
                // Find out what the source and the target of the packet is by examining the high nibble (4 bits)
                byte source = (byte)((ccdscipkt_rx.datacode[0] >> 6) & 0x03);
                byte target = (byte)((ccdscipkt_rx.datacode[0] >> 4) & 0x03); // Must be the laptop (0x00)

                // Extract DC command value from the low nibble (4 bits)
                byte dc_command = (byte)(ccdscipkt_rx.datacode[0] & 0x0F);

                // Proceed if the source is the scanner / ccd-bus / sci-bus and the target is the laptop (0x00)
                if (((source == 0x00) || (source == 0x01) || (source == 0x02) || (source == 0x03)) && (target == 0x00))
                {
                    switch (dc_command)
                    {
                        case 0x00:
                            {
                                // Reboot confirmation received
                                write_packet_textbox("RX", "SCANNER REBOOTED SUCCESSFULLY", data);
                                break;
                            }
                        case 0x01:
                            {
                                // Handshake response is coming (again)
                                if (ccdscipkt_rx.payload[0]  == 0x43 &&   // C
                                    ccdscipkt_rx.payload[1]  == 0x48 &&   // H
                                    ccdscipkt_rx.payload[2]  == 0x52 &&   // R
                                    ccdscipkt_rx.payload[3]  == 0x59 &&   // Y
                                    ccdscipkt_rx.payload[4]  == 0x53 &&   // S
                                    ccdscipkt_rx.payload[5]  == 0x4C &&   // L
                                    ccdscipkt_rx.payload[6]  == 0x45 &&   // E
                                    ccdscipkt_rx.payload[7]  == 0x52 &&   // R
                                    ccdscipkt_rx.payload[8]  == 0x43 &&   // C
                                    ccdscipkt_rx.payload[9]  == 0x43 &&   // C
                                    ccdscipkt_rx.payload[10] == 0x44 &&   // D
                                    ccdscipkt_rx.payload[11] == 0x53 &&   // S
                                    ccdscipkt_rx.payload[12] == 0x43 &&   // C
                                    ccdscipkt_rx.payload[13] == 0x49 &&   // I
                                    ccdscipkt_rx.payload[14] == 0x53 &&   // S
                                    ccdscipkt_rx.payload[15] == 0x43 &&   // C
                                    ccdscipkt_rx.payload[16] == 0x41 &&   // A
                                    ccdscipkt_rx.payload[17] == 0x4E &&   // N
                                    ccdscipkt_rx.payload[18] == 0x4E &&   // N
                                    ccdscipkt_rx.payload[19] == 0x45 &&   // E
                                    ccdscipkt_rx.payload[20] == 0x52)     // R
                                {
                                    write_packet_textbox("RX", "HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                                }
                                else
                                {
                                    write_packet_textbox("RX", "NO VALID HANDSHAKE RECEIVED (" + serial.PortName + ")", data);
                                }
                                break;
                            }
                        case 0x02:
                            {
                                // Write out to packet textbox
                                write_packet_textbox("RX", "STATUS PACKET RECEIVED", data);

                                // If this is the first status report received then do some extra work
                                if (!scanner_connected)
                                {
                                    scanner_connected = true;
                                    disconnect_button.Enabled = true;
                                    status_button.Enabled = true;

                                    write_command_history("Scanner connected" + Environment.NewLine);

                                    if (sci_enabled)
                                    {
                                        sci_bus_send_msg_button.Enabled = true;
                                        sci_bus_send_msg_textbox.Enabled = true;
                                    }

                                    if (ccd_enabled)
                                    {
                                        ccd_bus_send_msg_button.Enabled = true;
                                        ccd_bus_send_msg_textbox.Enabled = true;
                                    }

                                    if (log_enabled)
                                    {
                                        packet_send_button.Enabled = true;
                                        packet_send_textbox.Enabled = true;
                                    }

                                    communication_packet_log_enabled_checkbox.Enabled = true;
                                    ccd_bus_listen_checkbox.Enabled = true;
                                    sci_bus_listen_checkbox.Enabled = true;

                                    read_inteeprom_button.Enabled = true;
                                    write_inteeprom_button.Enabled = true;
                                    save_inteeprom_button.Enabled = true;

                                    read_exteeprom_button.Enabled = true;
                                    write_exteeprom_button.Enabled = true;
                                    save_exteeprom_button.Enabled = true;

                                    packet_send_textbox.Enabled = true;
                                }
                                break;
                            }
                        case 0x05: // General request responses
                            {
                                switch(ccdscipkt_rx.subdatacode[0])
                                {
                                    case 0x07:
                                        {
                                            write_packet_textbox("RX", "FREE RAM VALUE RESPONSE RECEIVED", data);

                                            break;
                                        }
                                    case 0x08:
                                        {
                                            write_packet_textbox("RX", "MCU COUNTER VALUE RESPONSE RECEIVED", data);

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
                                write_packet_textbox("RX", "RECEIVED BYTES", data);
                                break;
                            }
                    }
                }
            }
        }

        private void dump_pcm_Click(object sender, EventArgs e)
        {
            if (pcm_memory_read)
            {
                pcm_memory_read = false;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_memory_26_read_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                pcm_memory_read = true;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_memory_26_read_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }

        }

        private void read_exteeprom_button_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_exteeprom_content);
            write_serial_data(ccdscipkt_tx.ToBytes());

            byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
            temp = ccdscipkt_tx.ToBytes();

            StringBuilder temp_string = new StringBuilder(temp.Length * 3);
            foreach (byte status_byte in temp)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            string temp_string_final = temp_string.ToString().ToUpper();
            packet_log_textbox.AppendText("TX:\n");
            packet_log_textbox.AppendText(temp_string_final + "\n\n");
            packet_log_textbox.ScrollToCaret();
        }

        private void erase_exteeprom_button_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.FromBytes(ccdscipkt_commands.erase_exteeprom_content);
            write_serial_data(ccdscipkt_tx.ToBytes());
        }

        private void read_inteeprom_button_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_inteeprom_content);
            write_serial_data(ccdscipkt_tx.ToBytes());

            byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
            temp = ccdscipkt_tx.ToBytes();

            StringBuilder temp_string = new StringBuilder(temp.Length * 3);
            foreach (byte status_byte in temp)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            string temp_string_final = temp_string.ToString().ToUpper();
            packet_log_textbox.AppendText("TX:\n");
            packet_log_textbox.AppendText(temp_string_final + "\n\n");
            packet_log_textbox.ScrollToCaret();
        }

        private void status_button_Click(object sender, EventArgs e)
        {
            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_status);
            write_serial_data(ccdscipkt_tx.ToBytes());
            write_packet_textbox("TX", "REQUEST STATUS PACKET", ccdscipkt_tx.ToBytes());
        }

        private void send_sci_msg_button_Click(object sender, EventArgs e)
        {
            if (sci_enabled)
            {
                if (sci_bus_send_msg_textbox.Text != "")
                {
                    int calculated_checksum = 0;

                    byte[] bytes = GetBytes(sci_bus_send_msg_textbox.Text);
                    byte[] transmit_sci_msg = new byte[6 + bytes.Length];

                    transmit_sci_msg[0] = CCDSCIPKT.SYNC_BYTE; // 0x33
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
                    write_serial_data(ccdscipkt_tx.ToBytes());

                    if (log_enabled)
                    {
                        write_packet_textbox("TX", "MESSAGE TO SCI-BUS", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void sci_bus_hs_button_Click(object sender, EventArgs e)
        {
            if (sci_highspeed_enabled)
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_high_speed_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_high_speed_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void pcm_ram_button_Click(object sender, EventArgs e)
        {
            if (pcm_ram_area_textbox.Text != "")
            {
                byte[] bytes = GetBytes(pcm_ram_area_textbox.Text);
                if (pcm_ram_read)
                {
                    switch (bytes[0])
                    {
                        case 0xF2:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F2_off);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        case 0xF4:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F4_off);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        case 0xF6:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F6_off);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        default:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F2_off);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;
                    }
                }
                else
                {
                    switch (bytes[0])
                    {
                        case 0xF2:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F2_on);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        case 0xF4:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F4_on);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        case 0xF6:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F6_on);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;

                        default:
                            ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_ram_F2_on);
                            write_serial_data(ccdscipkt_tx.ToBytes());
                            break;
                    }
                }
            }
        }

        private void o2_sensor_button_Click(object sender, EventArgs e)
        {
            if (pcm_o2_sensor_read)
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_o2_sensor_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_o2_sensor_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }


        // Returns true if the given bit is 1 in "b" at "bitNumber" position
        public bool get_bit(byte b, int bitNumber)
        {
            bool bit = (b & (1 << bitNumber)) != 0;
            if (bit) return true;
            else return false;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = str.Split().Select(s => byte.Parse(s, NumberStyles.HexNumber)).ToArray();
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private void convert_o2_button_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            double o2_time = 0;
            double o2_voltage = 0;
            byte[] o2_temp = new byte[17];

            openFileDialog1.InitialDirectory = "d:\\Dokumentumok\\VS_Projects\\ChryslerCCDSCIScanner\\ChryslerCCDSCIScanner\\bin\\Debug\\";
            //openFileDialog1.Filter = "Binary files (*.bin)";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (var o2_data = new BinaryReader(myStream))
                        {
                            while (o2_data.BaseStream.Position != o2_data.BaseStream.Length)
                            {
                                o2_data.Read(o2_temp, 0, 17);
                                o2_time = (double)((double)(o2_temp[6] << 24 | o2_temp[7] << 16 | o2_temp[8] << 8 | o2_temp[9]) / 1000);
                                o2_voltage = ((double)o2_temp[14] / 255 * 5);
                                String temp = o2_time.ToString("0.00") + " " + o2_voltage.ToString("0.00");

                                StreamWriter file = new StreamWriter("o2_sensor.txt", true);
                                file.WriteLine(temp);
                                file.Close();
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            }

        }

        private void convert_ram_button_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            byte[] ram_temp = new byte[256 + 14];
            byte[] ram_values = new byte[256];
            double ram_time = 0;

            openFileDialog1.InitialDirectory = "d:\\Dokumentumok\\VS_Projects\\ChryslerCCDSCIScanner\\ChryslerCCDSCIScanner\\bin\\Debug\\";
            //openFileDialog1.Filter = "Binary files (*.bin)";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (var ram_data = new BinaryReader(myStream))
                        {
                            while (ram_data.BaseStream.Position != ram_data.BaseStream.Length)
                            {
                                ram_data.Read(ram_temp, 0, 256 + 14);
                                Array.Copy(ram_temp, 12, ram_values, 0, 256);
                                ram_time = (double)((double)(ram_temp[6] << 24 | ram_temp[7] << 16 | ram_temp[8] << 8 | ram_temp[9]) / 1000);

                                StreamWriter writer_20 = new StreamWriter("ram_values.txt", true);
                                try
                                {
                                    int linebreaker = 0;
                                    StringBuilder hex = new StringBuilder(ram_values.Length * 2);
                                    foreach (byte b in ram_values)
                                    {
                                        hex.AppendFormat("{0:x2}", b);
                                        hex.Append(" ");
                                        linebreaker++;
                                        if (linebreaker > 15)
                                        {
                                            hex.AppendLine();
                                            linebreaker = 0;
                                        }
                                    }
                                    writer_20.WriteLine(ram_time.ToString("0.00"));
                                    writer_20.WriteLine(hex.ToString().ToUpper());
                                    writer_20.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            }
        }

        private void communication_packets_richtextbox_TextChanged(object sender, EventArgs e)
        {
            if (packet_log_textbox.Text != "")
            {
                packet_log_save_button.Enabled = true;
                packet_log_clear_button.Enabled = true;
            }
            else
            {
                packet_log_save_button.Enabled = false;
                packet_log_clear_button.Enabled = false;
            }
        }

        private void ccd_bus_messages_richtextbox_TextChanged(object sender, EventArgs e)
        {
            if (ccd_bus_msg_richtextbox.Text != "")
            {
                ccd_bus_log_save_button.Enabled = true;
                ccd_bus_log_clear_button.Enabled = true;
            }
            else
            {
                ccd_bus_log_save_button.Enabled = false;
                ccd_bus_log_clear_button.Enabled = false;
            }
        }

        private void sci_bus_messages_richtextbox_TextChanged(object sender, EventArgs e)
        {
            if (sci_bus_msg_richtextbox.Text != "")
            {
                sci_bus_log_save_button.Enabled = true;
                sci_bus_log_clear_button.Enabled = true;
            }
            else
            {
                sci_bus_log_save_button.Enabled = false;
                sci_bus_log_clear_button.Enabled = false;
            }
        }

        private void communication_log_clear_button_Click(object sender, EventArgs e)
        {
            packet_log_textbox.Clear();
        }

        private void ccd_bus_log_clear_button_Click(object sender, EventArgs e)
        {
            ccd_bus_msg_richtextbox.Clear();
        }

        private void sci_bus_log_clear_button_Click(object sender, EventArgs e)
        {
            sci_bus_msg_richtextbox.Clear();
        }

        private void communication_log_enabled_checkbox_Click(object sender, EventArgs e)
        {
            if (log_enabled)
            {
                log_enabled = false;
                communication_packet_log_enabled_checkbox.Checked = false;
                //packet_send_button.Enabled = false;
                //packet_send_textbox.Clear();
                //packet_send_textbox.Enabled = false;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.log_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                log_enabled = true;
                communication_packet_log_enabled_checkbox.Checked = true;
                communication_packet_log_enabled_checkbox.Enabled = true;
                //packet_send_button.Enabled = true;
                //packet_send_textbox.Enabled = true;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.log_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void sci_bus_enabled_checkbox_Click(object sender, EventArgs e)
        {
            if (sci_enabled)
            {
                sci_enabled = false;
                sci_bus_listen_checkbox.Checked = false;
                sci_bus_send_msg_button.Enabled = false;
                sci_bus_send_msg_textbox.Clear();
                sci_bus_send_msg_textbox.Enabled = false;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                sci_enabled = true;
                sci_bus_listen_checkbox.Checked = true;
                sci_bus_listen_checkbox.Enabled = true;
                sci_bus_send_msg_button.Enabled = true;
                sci_bus_send_msg_textbox.Enabled = true;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.sci_bus_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void ccd_bus_enabled_checkbox_Click(object sender, EventArgs e)
        {
            if (ccd_enabled)
            {
                ccd_enabled = false;
                ccd_bus_listen_checkbox.Checked = false;
                ccd_bus_listen_checkbox.Text = "Disabled";
                ccd_bus_send_msg_button.Enabled = false;
                ccd_bus_send_msg_textbox.Clear();
                ccd_bus_send_msg_textbox.Enabled = false;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.ccd_bus_off);
                write_serial_data(ccdscipkt_tx.ToBytes());

            }
            else
            {
                ccd_enabled = true;
                ccd_bus_listen_checkbox.Checked = true;
                ccd_bus_listen_checkbox.Text = "Enabled";
                ccd_bus_listen_checkbox.Enabled = true;
                ccd_bus_send_msg_button.Enabled = true;
                ccd_bus_send_msg_textbox.Enabled = true;
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.ccd_bus_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void real_time_diagnostics_button_Click(object sender, EventArgs e)
        {
            diagnostics = new DiagnosticsForm();
            diagnostics.Show();
            //diagnostics.Dispose();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (AboutForm about = new AboutForm())
            {
                about.Show();
            }
            
            //about.BringToFront();
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            Environment.Exit(0);
        }

        private void read_dtc_pcm_button_Click(object sender, EventArgs e)
        {
            if (!sci_highspeed_enabled)
            {
                int calculated_checksum = 0;

                byte[] transmit_sci_msg = new byte[7];

                transmit_sci_msg[0] = CCDSCIPKT.SYNC_BYTE; // 0x33
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
                write_serial_data(ccdscipkt_tx.ToBytes());

                byte[] temp = new byte[ccdscipkt_tx.ToBytes().Length];
                temp = ccdscipkt_tx.ToBytes();

                StringBuilder temp_string = new StringBuilder(temp.Length * 3);
                foreach (byte status_byte in temp)
                {
                    temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
                }

                string temp_string_final = temp_string.ToString().ToUpper();
                packet_log_textbox.AppendText("TX:\n");
                packet_log_textbox.AppendText(temp_string_final + "\n\n");
                packet_log_textbox.ScrollToCaret();
            }
        }

        private void packet_send_button_Click(object sender, EventArgs e)
        {
            if (packet_send_textbox.Text != "")
            {
                byte[] bytes = GetBytes(packet_send_textbox.Text);
                if (bytes.Length > 1)
                {
                    byte[] transmit_sci_msg;

                    ccdscipkt_tx.payload = null; // causes a bug if it's not here

                    // DATA CODE byte, SUB-DATA CODE byte, PAYLOAD byte(s) only (Partial packet)
                    if (bytes.Length < 6)
                    {
                        transmit_sci_msg = new byte[bytes.Length + 4];

                        for (int i = 0; i < 3; i++)
                        {
                            transmit_sci_msg[i] = 0x00; // SYNC byte and LENGTH bytes are zeros, let the packetizer calculate them
                        }

                        for (int i = 0; i < bytes.Length; i++)
                        {
                            transmit_sci_msg[3 + i] = bytes[i]; // DATA CODE and SUB-DATA CODE and PAYLOAD bytes
                        }

                        transmit_sci_msg[3 + bytes.Length] = 0x00; // CHECKSUM byte is zero, let the packetizer calculate it
                    }
                    else // SYNC + LENGTH + DATA CODE + SUB-DATA CODE + PAYLOAD + CHECKSUM (Whole packet + error correction)
                    {
                        transmit_sci_msg = new byte[bytes.Length];
                        for (int i = 0; i < transmit_sci_msg.Length; i++)
                        {
                            transmit_sci_msg[i] = bytes[i];
                        }
                    }

                    ccdscipkt_tx.FromBytes(transmit_sci_msg);
                    write_serial_data(ccdscipkt_tx.ToBytes());

                    if (log_enabled)
                    {
                        write_packet_textbox("TX", "CUSTOM PACKET", ccdscipkt_tx.ToBytes());
                    }
                }
            }
        }

        private void packet_send_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                packet_send_button.PerformClick();
                e.Handled = true;
            }
        }

        private void ccd_bus_send_msg_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                ccd_bus_send_msg_button.PerformClick();
                e.Handled = true;
            }
        }

        private void sci_bus_send_msg_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                sci_bus_send_msg_button.PerformClick();
                e.Handled = true;
            }
        }

        private void disconnect_button_Click(object sender, EventArgs e)
        {
            if (serial.IsOpen)
            {
                serial.Close();

                write_command_history("Scanner disconnected");

                default_button_states();

                serial_rx_buffer.Reset();
                serial_tx_buffer.Reset();

                scanner_found = false;
                scanner_connected = false;
            }

            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
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

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.textBox2.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.textBox2.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                this.textBox2.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                this.textBox2.Text = "Done!";
            }
        }

        private void map_sensor_button_Click(object sender, EventArgs e)
        {
            if (pcm_map_sensor_read)
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_map_sensor_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_map_sensor_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void pcm_read_all_Click(object sender, EventArgs e)
        {
            if (pcm_read_all_parameters)
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_read_all_off);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
            else
            {
                ccdscipkt_tx.FromBytes(ccdscipkt_commands.request_pcm_read_all_on);
                write_serial_data(ccdscipkt_tx.ToBytes());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder temp_string = new StringBuilder(serial_rx_buffer.Array.Length * 3);
            foreach (byte status_byte in serial_rx_buffer.Array)
            {
                temp_string.Append(Convert.ToString(status_byte, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            string temp_string_final = temp_string.ToString().ToUpper();
            pcm_parameters_richtextbox.AppendText(temp_string_final + "\n\n");
            pcm_parameters_richtextbox.ScrollToCaret();
        }

        private void reboot_scanner_button_Click(object sender, EventArgs e)
        {
            write_packet_textbox("TX", "REQUEST SCANNER REBOOT", ccdscipkt_commands.reboot_scanner);
            ccdscipkt_tx.FromBytes(ccdscipkt_commands.reboot_scanner);
            write_serial_data(ccdscipkt_tx.ToBytes());
        }

        private void packet_log_textbox_TextChanged(object sender, EventArgs e)
        {
            if (packet_log_textbox.Text != "")
            {
                packet_log_save_button.Enabled = true;
                packet_log_clear_button.Enabled = true;
            }
            else
            {
                packet_log_save_button.Enabled = false;
                packet_log_clear_button.Enabled = false;
            }
        }

        private void flush_memory_button_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void packetGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketGenerator packetgenerator = new PacketGenerator();
            packetgenerator.Show();
        }
    }

    public class DataReception : INotifyPropertyChanged
    {
        private bool _datareceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool DataReceived
        {
            get { return _datareceived; }
            set
            {
                if (_datareceived != value)
                {
                    _datareceived = value;
                    SendPropertyChanged("DataReceived");
                }
            }
        }

        private void SendPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public DataReception()
        {
        }
    }
}
