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
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Threading;
using RJCP.IO.Ports;
using RJCP.Datastructures;
using DRBDBReader.DB;

namespace ChryslerCCDSCIScanner_GUI
{
    public partial class MainForm : Form
    {
        SerialPortStream Serial;
        PacketReceptor Pr;
        PacketGenerator packetgenerator;
        AboutForm about;
        DiagnosticsForm diagnostics;
        DRBDBExplorer drbdbexplorer;
        PacketManager.PacketStructure PacketRx;
        PacketManager.PacketStructure PacketTx;
        DTC_LookupTable pcmdtctable;
        CircularBuffer<byte> SerialRxBuffer;
        CircularBuffer<byte> SerialTxBuffer;
        SimpleBinaryReader scReader;

        System.Timers.Timer TimeoutTimer = new System.Timers.Timer();

        public const byte SYNC_POS = 0;
        public const byte LENGTH_POS = 1;
        public const byte DATACODE_POS = 3;
        public const byte SUBDATACODE_POS = 4;

        public int packet_count_rx = 0;
        public int packet_count_tx = 0;

        public bool CCDEnabled = false;
        public bool SCIEnabled = false;
        public bool LogEnabled = true;

        public bool SCIHiSpeed = false;

        public bool scanner_connected = false;
        public bool timeout = false;

        public byte[] buffer = new byte[4096];

        public string DateTimeNow;
        public string TextLogFilename;
        public string BinaryLogFilename;
        public string CCDLogFilename;
        public string SCILogFilename;

        public byte[] ccd_filter_bytes;
        public bool ccd_filtering_active = false;

        public byte[] sci_filter_bytes;
        public bool sci_filtering_active = false;


        // SCI-bus low speed parameters (Mode 14)
        public float PCM_AMB_TEMP_VOLTS = 0;        // 14 01
        public float PCM_O2_VOLTS1 = 0;             // 14 02
        //
        public float PCM_COOLANT_TEMP = 0;          // 14 05
        public float PCM_COOLANT_TEMP_VOLTS = 0;    // 14 06
        public float PCM_TPS_VOLTS = 0;             // 14 07
        public float PCM_TPS_MIN_VOLTS = 0;         // 14 08
        public float PCM_KNOCK_VOLTS = 0;           // 14 09
        public float PCM_BATTERY_VOLTS = 0;         // 14 0A
        public float PCM_MAP_VOLTS1 = 0;            // 14 0B
        public float PCM_TARGET_IAC_POS = 0;        // 14 0C
        //
        public float PCM_ADAPT_FUEL_FACT = 0;       // 14 0E
        public float PCM_BARO_PRESSURE = 0;         // 14 0F
        public float PCM_RPM1 = 0;                  // 14 10
        public float PCM_RPM2 = 0;                  // 14 11
        //
        public float PCM_KEY_ON_CYCLES_ERROR1 = 0;  // 14 13
        //
        public float PCM_SPARK_ADVANCE = 0;         // 14 15
        public float PCM_CYL1_RETARD = 0;           // 14 16
        public float PCM_CYL2_RETARD = 0;           // 14 17
        public float PCM_CYL3_RETARD = 0;           // 14 18
        public float PCM_CYL4_RETARD = 0;           // 14 19
        public float PCM_TARGET_BOOST = 0;          // 14 1A
        public float PCM_CHARGE_TEMP = 0;           // 14 1B
        public float PCM_CHARGE_TEMP_VOLTS = 0;     // 14 1C
        public float PCM_CRUISE_TARGET_SPEED = 0;   // 14 1D
        public float PCM_KEY_ON_CYCLES_ERROR2 = 0;  // 14 1E
        public float PCM_KEY_ON_CYCLES_ERROR3 = 0;  // 14 1F
        public float PCM_CRUISE_STATUS = 0;         // 14 20
        //
        public float PCM_TARGET_BATT_CHARGE = 0;    // 14 24
        public float PCM_WASTEGATE_DUTY_CYCLE = 0;  // 14 26
        public float PCM_THEFT_ALARM_STATUS = 0;    // 14 27
        //
        public float PCM_MAP_VOLTS2 = 0;            // 14 40
        public float PCM_VEHICLE_SPEED = 0;         // 14 41
        public float PCM_O2_VOLTS2 = 0;             // 14 42
        //
        public float PCM_TOTAL_SPARK_RETARD = 0;    // 14 46
        public float PCM_TOTAL_SPARK_ADVANCE = 0;   // 14 47


        // SCI-bus high speed parameters (F4)       // Command                  Description                     Scaling
        public byte PCMRAMMode = 0;
        public float F4_ENG_RPM = 0;                // F4 0A 0B	            - engine rpm			        - scaling: RAW * 0.125 [rpm]
        public float F4_TARGET_IDLE = 0;            // F4 35 36	            - target idle rpm		        - scaling: RAW * 0.125 [rpm]
        public float F4_SPEED = 0;                  // F4 0C 0D	            - speed					        - scaling: RAW * 0.0251057664 [km/h]
        public float F4_BATT_VOLTS = 0;             // F4 0F	            - battery voltage		        - scaling: RAW * 0.0618 [V]
        public float F4_CHARGE_VOLTS = 0;           // F4 3A	            - charging voltage		        - scaling: RAW * 0.0618 [V]
        public float F4_TPS_VOLTS = 0;              // F4 12	            - tps volts				        - scaling: RAW * 0.0196 [V]
        public float F4_MIN_TPS_VOLTS = 0;          // F4 13	            - minimum tps volts		        - scaling: RAW * 0.0196 [V]
        public float F4_CALC_TPS_VOLTS = 0;         // F4 14	            - calculated tps volts          - scaling: RAW * 0.0196 [V]
        public float F4_MAP_VOLTS = 0;              // F4 17	            - map volts				        - scaling: RAW * 0.0196 [V]
        public float F4_MAP_VACUUM = 0;             // F4 DA	            - map vacuum			        - scaling: RAW * 0.412054294 [kPa]
        public float F4_O2_VOLTS = 0;               // F4 1B	            - o2 volts				        - scaling: RAW * 0.0196 [V]
        public float F4_INJ_PW = 0;                 // F4 27 28	            - injector pulsewidth	        - scaling: RAW * 0.004 [ms]
        public byte F4_IAC_STEPS = 0;               // F4 3E		        - iac steps				        - scaling: RAW * 1
        public byte F4_TARGET_IAC_STEPS = 0;        // F4 37		        - target iac steps		        - scaling: RAW * 1
        public float F4_SPARK_ADV = 0;              // F4 2F		        - spark advance			        - scaling: RAW * 0.5 [°]
        public float F4_CYL_1_RETARD = 0;           // F4 31		        - cylinder 1 retard		        - scaling: RAW * 0.5 [°]
        public float F4_CYL_2_RETARD = 0;           // F4 32		        - cylinder 2 retard		        - scaling: RAW * 0.5 [°]
        public float F4_CYL_3_RETARD = 0;           // F4 33		        - cylinder 3 retard		        - scaling: RAW * 0.5 [°]
        public float F4_CYL_4_RETARD = 0;           // F4 34		        - cylinder 4 retard		        - scaling: RAW * 0.5 [°]
        public float F4_KNOCK_VOLTS = 0;            // F4 1F		        - knock volts			        - scaling: RAW * 0.0196 [V]
        public float F4_KNOCK_RETARD = 0;           // F4 30		        - overall knock retard	        - scaling: RAW * 0.5 [°]
        public byte F4_DIS_SIGNAL = 0;              // F4 41		        - distributor signal	        - masking: 0x01: sync history - true: in-sync; false: out-of-sync
                                                    //                                                                 0x10: current sync state - true: in-sync; false: out-of-sync
                                                    //                                                                 0x20: ckp state - true: present; false: lost
                                                    //                                                                 0x40: cmp state - true: present; false: lost
        public byte F4_FUEL_SYSTEM_STATUS = 0;      // F4 42		        - fuel system status	        - masking: 0x01: open loop
                                                    //                                                                 0x02: closed loop
                                                    //                                                                 0x04: open loop drive
                                                    //                                                                 0x08: open loop dtc
                                                    //                                                                 0x10: closed loop dtc
                                                    //                                                                 0x20: o2 sensor is updating adaptive memory
                                                    //                                                                 0x40: closed loop
                                                    //                                                                 0x80: ready for closed loop
        public float F4_CLOSED_LOOP_TIMER = 0;      // F4 4A		        - closed loop timer		        - scaling: RAW * 0.0535 [min]
        public byte F4_CURRENT_ADAPTIVE_CELL = 0;   // F4 43		        - current adaptive cell	        - scaling: RAW * 1
        public float F4_STFT = 0;                   // F4 D6		        - short-term adaptive	        - scaling: RAW * 0.1953 [%]
        public float F4_LTFT = 0;                   // F4 D7		        - long-term adaptive	        - scaling: RAW * 0.1953 [%]
        public float F4_FUEL_LEVEL_VOLTS = 0;       // F4 DE		        - fuel level volts		        - scaling: RAW * 0.0196 [V]
        public float F4_ENGINE_LOAD = 0;            // F4 E3		        - engine load			        - scaling: RAW * 0.3922 [%]
        public float F4_AMB_TEMP_VOLTS = 0;         // F4 10		        - ambient temp volts	        - scaling: RAW * 0.0196 [V]
        public short F4_AMB_TEMP_DEG = 0;           // F4 11		        - ambient temp deg		        - scaling: RAW - 128 [°C]
        public float F4_ECT_VOLTS = 0;              // F4 15		        - engine coolant volts	        - scaling: RAW * 0.0196 [V]
        public short F4_ECT_DEG = 0;                // F4 16		        - engine coolant deg	        - scaling: RAW - 128 [°C]
        public float F4_IAT_VOLTS = 0;              // F4 1D		        - intake air temp volts	        - scaling: RAW * 0.0196 [V]
        public short F4_IAT_DEG = 0;                // F4 1E		        - intake air temp deg	        - scaling: RAW - 128 [°C]
        public float F4_AC_PRESSURE_VOLTS = 0;      // F4 25		        - a/c pressure volts	        - scaling: RAW * 0.0196 [V]
        public float F4_AC_PRESSURE_KPA = 0;        // F4 26		        - a/c pressure			        - scaling: RAW * 13.52061848 [kPa]
        public float F4_BARO_PRESSURE = 0;          // F4 19		        - barometric pressure	        - scaling: RAW * 0.412054294 [kPa]
        public byte F4_LIMP_REASON = 0;             // F4 73		        - limp-in reason		        - masking: 0x08: IAT DTC (intake air temperature sensor malfunction)
                                                    //                                                                 0x10: TPS DTC (throttle pedal sensor malfunction)
                                                    //                                                                 0x20: MAP EL DTC (manifold absolute pressure sensor malfunction)
                                                    //                                                                 0x40: MAP VA DTC (manifold absolute pressure sensor malfunction)
                                                    //                                                                 0x80: ECT DTC (engine coolant temperature sensor malfunction)
        public float F4_RUNTIME = 0;                // F4 4B 4C	            - time from start/run           - scaling: RAW * 0.0002 [min]
        public float F4_RUNTIME_STALL = 0;          // F4 4D		        - runtime at stall		        - scaling: RAW * 0.0535 [min]
        public byte F4_DES_RELAY_STATES = 0;        // F4 3C		        - desired relay states	        - masking: 0x10: asd relay - true: off; false: on
        public byte F4_ACT_RELAY_STATES = 0;        // F4 7A		        - actual relay states	        - masking: 0x10: asd relay - true: on; false: off

        // CCD-bus parameters
        public uint CCD_SPEED = 0;                  // 24 AA BB CS;         - vehicle speed                         AA [mph], BB [km/h] 
        public uint CCD_TIMER = 0;                  // 29 AA BB CS;         - minutes since last engine shutdown    AABB [min] 
        public float CCD_TPS_PERC = 0;              // 42 AA BB CS;         - throttle pedal position               AA*0,66 [%] 
        public float CCD_AC_PRESSURE = 0;           // 75 AA BB CS;         - A/C high side pressure                AA*1,961*6,894757 [kPa] 
        public int CCD_ECT_TEMP = 0;                // 8C AA BB CS;         - engine coolant temperature            AA-128 [°C]
        public int CCD_AMB_TEMP = 0;                //                      - ambient temperature                   BB-128 [°C] 
        public float CCD_ODOMETER = 0;              // CE AA BB CC DD CS;   - odometer                              AABBCCDD / 8000 [mi], AABBCCDD / 4971 [km] 
        public float CCD_BATTERY_VOLTS = 0;         // D4 AA BB CS;         - battery voltage                       AA*0,0592 [V]
        public float CCD_CHARGING_VOLTS = 0;        //                      - charging voltage                      BB*0,0592 [V] 
        public int CCD_RPM = 0;                     // E4 AA BB CS;         - engine speed                          AA*32 [RPM]; 
        public float CCD_MAP = 0;                   //                      - air intake manifold vacuum            BB*0,1217 [inHg], BB*0,1217*25,4 [mmHg] -> BB*0,1217*25,4*0,1333 [kPa] 
        public float CCD_TRIPMETER = 0;             // EE AA BB CC CS;      - tripmeter                             AABBCC*(128/8000) [mi], AABBCC*(128/4971) [km] 

        string CCDSensorList = "               N/A" + Environment.NewLine;
        string SCISensorList = "               N/A" + Environment.NewLine;

        // Class constructor
        public MainForm()
        {
            InitializeComponent();
            ComponentsDisabled();

            // Assign application icon
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            
            // Setup timeout timer and disable it, assign timeout event handler
            TimeoutTimer.Elapsed += new ElapsedEventHandler(TimeoutReached);
            TimeoutTimer.Interval = 500; // 500 ms timeout
            TimeoutTimer.Enabled = false;

            // Set combobox states
            ModuleListComboBox.SelectedIndex = 0;
            PCMTCMSelectorComboBox.SelectedIndex = 1;
            SCIHSModeComboBox.SelectedIndex = 0;

            // Set checkbox states
            LogEnabled = PacketLogEnabledCheckbox.Enabled;

            // Instantiate objects in memory
            Pr = new PacketReceptor();
            PacketRx = new PacketManager.PacketStructure();
            PacketTx = new PacketManager.PacketStructure();
            SerialRxBuffer = new CircularBuffer<byte>(4096);
            SerialTxBuffer = new CircularBuffer<byte>(4096);
            pcmdtctable = new DTC_LookupTable();

            // Assign event handler for USB packet reception
            Pr.PropertyChanged += new PropertyChangedEventHandler(PacketReceived);

            // Assign mouse event handler for a combobox
            PCMTCMSelectorComboBox.MouseWheel += new MouseEventHandler(PCMTCMSelectorComboBox_MouseWheel);

            // Assign logfile names
            DateTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            TextLogFilename = @"LOG/scannerlog_" + DateTimeNow + ".txt";
            BinaryLogFilename = @"LOG/scannerlog_" + DateTimeNow + ".bin";
            CCDLogFilename = @"LOG/ccdlog_" + DateTimeNow + ".txt";
            SCILogFilename = @"LOG/scilog_" + DateTimeNow + ".txt";

            // Setup complete, welcome
            WriteCommandHistory("Welcome!");
        }

        private void ComponentsDisabled()
        {
            // Group boxes
            PacketLogGroupbox.Enabled = false;
            CCDBusMessagesGroupbox.Enabled = false;
            SCIBusMessagesGroupbox.Enabled = false;
            SensorDataGroupbox.Enabled = false;
            RealTimeDiagnosticsGroupbox.Enabled = false;
            ReadDTCGroupbox.Enabled = false;
            MiscGroupbox.Enabled = false;
            EEPROMGroupbox.Enabled = false;
            DTCListGroupbox.Enabled = false;
            //DRB3ToolsGroupbox.Enabled = false;
            PCMRAMGroupbox.Enabled = false;

        }

        private void ComponentsEnabled()
        {
            // Group boxes
            PacketLogGroupbox.Enabled = true;
            CCDBusMessagesGroupbox.Enabled = true;
            SCIBusMessagesGroupbox.Enabled = true;
            SensorDataGroupbox.Enabled = true;
            RealTimeDiagnosticsGroupbox.Enabled = true;
            ReadDTCGroupbox.Enabled = true;
            MiscGroupbox.Enabled = true;
            EEPROMGroupbox.Enabled = true;
            DTCListGroupbox.Enabled = true;
            DRB3ToolsGroupbox.Enabled = true;
            //PCMRAMGroupbox.Enabled = true;
        }

        // Timer timout event handler method
        public void TimeoutReached(object source, ElapsedEventArgs e)
        {
            timeout = true; // set flag, nothing more
        }

        // This method gets called everytime when something arrives on the COM-port
        // It searches for valid packets (even if there are multiple packets in one reception) and discards the garbage bytes
        private void PacketReceived(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PacketReceived")
            {
                int PacketSize = 0;
                bool repeat = true;
                byte[] temp;

                // Loop at least once
                while (repeat)
                {
                    // Find the first byte with value 0x33
                    Here: // This is a goto label, never mind now
                    while ((SerialRxBuffer.Array[SerialRxBuffer.Start] != PacketManager.SYNC_BYTE) && (SerialRxBuffer.ReadLength > 0))
                    {
                        // If the byte at the "Start" of the circular buffer is not 0x33 then remove it,
                        // the "Start" address advances automatically when the Pop method is called,
                        // so the while loop will look at the next byte.
                        SerialRxBuffer.Pop();

                        // Check if the buffer is empty and update the misc groupbox before exiting
                        if (SerialRxBuffer.ReadLength == 0)
                        {
                            SerialRxBuffer.Reset();
                            BufferStartLabel.Text = "Buffer Start: " + SerialRxBuffer.Start;
                            BufferEndLabel.Text = "Buffer End: " + SerialRxBuffer.End;
                            BufferReadlengthLabel.Text = "Buffer ReadLength: " + SerialRxBuffer.ReadLength + " byte(s)";
                            BufferWritelengthLabel.Text = "Buffer WriteLength: " + SerialRxBuffer.WriteLength + " byte(s)";
                            return;
                        }
                    }

                    // If we're still here chances are there's a packet ahead
                    // Get the length of the packet
                    if (SerialRxBuffer.ReadLength > 2)
                    {
                        PacketSize = ((SerialRxBuffer.Array[SerialRxBuffer.Start + LENGTH_POS] << 8) | SerialRxBuffer.Array[SerialRxBuffer.Start + LENGTH_POS + 1]) + 4;
                    }
                    else { return; } // If there are clearly not enough bytes then don't do anything

                    // If the size is bigger than 2044 bytes (payload only, + 4 bytes = 2048) then it's most likely garbage data 
                    if (PacketSize > 2044)
                    {
                        if (SerialRxBuffer.ReadLength != 0) SerialRxBuffer.Pop(); // Pop this byte that lead us here so we can search for another packet
                        goto Here; // Jump back to the while loop to repeat
                    }

                    // Copy the data in the circular buffer array to a temporary linear array
                    // This is needed because the data doesn't necessarily start at zero index and this is confusing for my dummy methods
                    temp = new byte[PacketSize];
                    Array.Copy(SerialRxBuffer.Array, SerialRxBuffer.Start, temp, 0, PacketSize);

                    // Try to convert the data into a packet
                    if (PacketRx.FromBytes(temp))
                    {
                        // If the conversion is OK then remove the bytes from the circular buffer
                        for (int i = 0; i < PacketSize; i++)
                        {
                            if (SerialRxBuffer.ReadLength != 0) SerialRxBuffer.Pop();
                        }
                        ProcessData(temp); // Get these bytes processed by another method
                    }
                    else
                    {
                        // If something goes wrong (checksum error...)
                        if (SerialRxBuffer.ReadLength != 0) SerialRxBuffer.Pop(); // discard this byte, run the loop again and search for another packet
                        goto Here;
                    }

                    // Don't loop again if there are no bytes left
                    if (SerialRxBuffer.ReadLength == 0)
                    {
                        repeat = false; // clear flag
                        SerialRxBuffer.Reset();
                        Array.Clear(SerialRxBuffer.Array, 0, SerialRxBuffer.Array.Length);
                    }
                    else // if there are more bytes available then loop again
                    {
                        repeat = true; // set flag
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

        private void WriteCommandHistory(string message)
        {

            CommandHistoryTextBox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] ");
            CommandHistoryTextBox.AppendText(message + Environment.NewLine);
            // Scroll down
            CommandHistoryTextBox.SelectionStart = CommandHistoryTextBox.TextLength;
            CommandHistoryTextBox.ScrollToCaret();
        }

        public void WritePacketTextBox(string direction, string description, byte[] message)
        {
            if (PacketLogEnabledCheckbox.Checked)
            {
                // Get all the lines out as an arry
                string[] lines = PacketTextBox.Lines;

                // Remove some lines from the beginning because too much lines freeze the application
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
        }

        private void WriteCCDSCIBusTextBox(TextBox textBox, byte[] message)
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

            // Add the bytes of the message and convert the numbers into text
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

            // Save text to log files
            if (textBox.Name == "CCDBusMsgTextBox")
            {
                if (!Directory.Exists("LOG")) Directory.CreateDirectory("LOG");
                File.AppendAllText(CCDLogFilename, newstuff.ToString());
            }
            else if (textBox.Name == "SCIBusMsgTextBox")
            {
                if (!Directory.Exists("LOG")) Directory.CreateDirectory("LOG");
                File.AppendAllText(SCILogFilename, newstuff.ToString());
            }

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

        private void ProcessData(byte[] data)
        {
            // Make sure it's a valid USB Packet (the "FromBytes" method must return a true value)
            // This method also assigns the internal variables of the packet structure
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
                        // Packet echoed back
                        WritePacketTextBox("RX", "PACKET ECHO", data);
                        break;

                    case PacketManager.from_scanner:
                        switch (dc_command)
                        {
                            case PacketManager.reboot: // 0x00
                                // This is just a reboot confirmation before the actual reboot
                                WritePacketTextBox("RX", "SCANNER REBOOT COMPLETE", data);
                                WriteCommandHistory("Scanner reboot complete!");
                                break;

                            case PacketManager.handshake: // 0x01
                                // This is a handshake response, the payload contains an ASCII-encoded string
                                string received_handshake = Encoding.ASCII.GetString(PacketRx.payload);
                                WritePacketTextBox("RX", "HANDSHAKE (" + Serial.PortName + ")", data);
                                if (received_handshake == "CHRYSLERCCDSCISCANNER")
                                {
                                    WriteCommandHistory("Handshake OK: " + received_handshake);
                                    CCDBusEnabledCheckbox.Checked = false;
                                    SCIBusEnabledCheckbox.Checked = false;
                                }
                                else
                                {
                                    WriteCommandHistory("Handshake ER: " + received_handshake);
                                }
                                break;

                            case PacketManager.status: // 0x02
                                // Write out to packet textbox
                                WritePacketTextBox("RX", "STATUS", data);
                                WriteCommandHistory("Status packet received!");
                                break;
                                
                            case PacketManager.settings: // 0x03 - It doesn't seem to be happening too much around here... 
                                switch (subdatacode)
                                {
                                    case PacketManager.read_settings:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK", data);
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                        break;
                                        
                                    case PacketManager.write_settings:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK", data);
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                        break;
                                        
                                    case PacketManager.enable_ccd_bus:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK", data);
                                            CCDBusEnabledCheckbox.Checked = true;
                                            CCDEnabled = true;
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                        break;
                                        
                                    case PacketManager.disable_ccd_bus:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK", data);
                                            CCDBusEnabledCheckbox.Checked = false;
                                            CCDEnabled = false;
                                        }
                                        else WritePacketTextBox("RX", "SETTINGS / ERROR", data);
                                        break;
                                        
                                    case PacketManager.enable_sci_bus:
                                        if (PacketRx.payload[0] == 0x01)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK (PCM)", data);
                                            SCIBusEnabledCheckbox.Checked = true; // TODO: disable event firing... somehow
                                            PCMTCMSelectorComboBox.SelectedIndex = 1;
                                            SCIEnabled = true;
                                        }
                                        else if (PacketRx.payload[0] == 0x02)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK (TCM)", data);
                                            SCIBusEnabledCheckbox.Checked = true; // TODO: disable event firing... somehow
                                            PCMTCMSelectorComboBox.SelectedIndex = 2;
                                            SCIEnabled = true;
                                        }
                                        else if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK (NON)", data);
                                            PCMTCMSelectorComboBox.SelectedIndex = 0;
                                            SCIEnabled = true;
                                        }
                                        break;
                                        
                                    case PacketManager.disable_sci_bus:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK", data);
                                            SCIEnabled = false;
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ER", data);
                                        break;
                                        
                                    case PacketManager.enable_sci_hi_speed:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK (SCI-BUS HIGH SPEED ON)", data);
                                            SCIBusSpeedLabel.Text = "62500 kbps";
                                            SCIHiSpeed = true;
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ER", data);
                                        break;
                                        
                                    case PacketManager.disable_sci_hi_speed:
                                        if (PacketRx.payload[0] == 0x00)
                                        {
                                            WritePacketTextBox("RX", "SETTINGS / OK (SCI-BUS HIGH SPEED OFF)", data);
                                            SCIBusSpeedLabel.Text = "7812.5 kbps";
                                            SCIHiSpeed = false;
                                        }
                                        else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SETTINGS / ER", data);
                                        break;
                                        
                                    default:
                                        WritePacketTextBox("RX", "SETTINGS / ER", data);
                                        break;
                                        
                                }
                                break;

                            case PacketManager.response: // 0x05; 0x04 is skipped because that's the request datacode from the laptop
                                switch (subdatacode)
                                {
                                    case PacketManager.free_ram_available:
                                        WritePacketTextBox("RX", "FREE RAM", data);
                                        break;
                                        
                                    case PacketManager.mcu_counter_value:
                                        WritePacketTextBox("RX", "MCU COUNTER", data);
                                        break;
                                        
                                    case PacketManager.scan_vehicle_modules:
                                        if (PacketRx.payload.Length == 1)
                                        {
                                            if (PacketRx.payload[0] == 0x00)
                                            {
                                                WritePacketTextBox("RX", "CCD-BUS MODULE SCAN START", data);
                                                DTCListTextbox.AppendText("Scanning CCD-bus for modules...\n\n");
                                            }
                                            else if (PacketRx.payload[0] == 0xFF)
                                            {
                                                WritePacketTextBox("RX", "CCD-BUS MODULE SCAN ERROR", data);
                                                DTCListTextbox.AppendText("Error, scan already done!\n");
                                            }
                                        }
                                        else
                                        {
                                            WritePacketTextBox("RX", "CCD-BUS MODULE SCAN END", data);
                                            ScanModulesButton.Enabled = true;

                                            bool ccd_modules_found = false;

                                            for (int i = 0; i < 32; i++)
                                            {
                                                if (PacketRx.payload[i] != 0x00) ccd_modules_found = true;
                                            }

                                            if (!ccd_modules_found) DTCListTextbox.AppendText("No CCD-bus modules found!\n\n");

                                        }
                                        break;
                                        
                                    default:
                                        WritePacketTextBox("RX", "RESPONSE / ER", data);
                                        break;
                                }
                                break;

                            case PacketManager.debug: // 0x0E
                                WritePacketTextBox("RX", "DEBUG PACKET(S)", data);
                                byte counter = 0;

                                StringBuilder newstuff = new StringBuilder();
                                newstuff.Append(Convert.ToString(PacketRx.subdatacode[0], 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
                                newstuff.Append(Environment.NewLine);

                                foreach (byte bytes in PacketRx.payload)
                                {
                                    newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());

                                    counter++;
                                    if (counter > 15)
                                    {
                                        newstuff.Append(Environment.NewLine); // Add new line after every 16 bytes
                                        counter = 0;
                                    }
                                }

                                newstuff.Append(Environment.NewLine);
                                SensorDataTextbox.Text = newstuff.ToString();

                                // save this text to a textfile
                                string dummyfilename = @"LOG/scihslog_" + DateTimeNow + ".txt";
                                if (!Directory.Exists("LOG")) Directory.CreateDirectory("LOG");
                                File.AppendAllText(dummyfilename, newstuff.ToString());

                                break;

                            case PacketManager.ok_error: // 0x0F
                                switch (subdatacode)
                                {
                                    case PacketManager.ok:
                                        WritePacketTextBox("RX", "COMMAND OK", data);
                                        break;
                                        
                                    default:
                                        WritePacketTextBox("RX", "COMMAND ER", data);
                                        break;
                                }
                                break;
                                
                            default:
                                WritePacketTextBox("RX", "UNKNOWN BYTES", data);
                                break;
                        }
                        break;
                        
                    case PacketManager.from_ccd_bus:
                        switch (dc_command)
                        {
                            case PacketManager.send_msg:
                                if (PacketRx.payload[0] == 0x00) WritePacketTextBox("RX", "CCD-BUS MESSAGE RECEIVED", data);
                                else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "CCD-BUS MESSAGE ERROR", data);
                                break;
                                
                            case PacketManager.receive_msg:
                                WritePacketTextBox("RX", "CCD-BUS MESSAGE", data);

                                // If filtering is active by ID-bytes (first byte of every message)
                                if (ccd_filter_bytes != null)
                                {
                                    // Write only if the ID byte is on the filter list
                                    if (ccd_filter_bytes.Contains(PacketRx.payload[0]))
                                    {
                                        try { WriteCCDSCIBusTextBox(CCDBusMsgTextBox, PacketRx.payload); }
                                        catch {  }
                                    }
                                    else { /* Ignore this message */ }
                                }
                                else // Filtering disabled, show everything
                                {
                                    try { WriteCCDSCIBusTextBox(CCDBusMsgTextBox, PacketRx.payload); }
                                    catch {  }
                                }

                                switch (PacketRx.payload[0])
                                {
                                    case 0x24: // speed [km/h]
                                        CCD_SPEED = PacketRx.payload[2];
                                        break;
                                            
                                    case 0x29: // timer since last engine shutdown [min]
                                        CCD_TIMER = (uint)((PacketRx.payload[1] << 8) + PacketRx.payload[2]);
                                        break;

                                    case 0x42: // throttle position sensor
                                        CCD_TPS_PERC = PacketRx.payload[1] * 0.6666F;
                                        break;

                                    case 0x75: // A/C high side pressure [kPa]
                                        CCD_AC_PRESSURE = PacketRx.payload[1] * 1.961F * 6.894757F;
                                        break;

                                    case 0x8C: // ECT, ambient temperature
                                        CCD_ECT_TEMP = PacketRx.payload[1] - 128;
                                        CCD_AMB_TEMP = PacketRx.payload[2] - 128;
                                        break;

                                    case 0xCE: // odometer value [km]
                                        CCD_ODOMETER = ((PacketRx.payload[1] << 24) + (PacketRx.payload[2] << 16) + (PacketRx.payload[3] << 8) + PacketRx.payload[4]) / 4971F;
                                        break;
                                            
                                    case 0xD4: // battery volts [V]
                                        CCD_BATTERY_VOLTS = PacketRx.payload[1] * 0.0618F;
                                        CCD_CHARGING_VOLTS = PacketRx.payload[2] * 0.0618F;
                                        break;
                                            
                                    case 0xE4: // rpm & map
                                        CCD_RPM = PacketRx.payload[1] * 32;
                                        CCD_MAP = PacketRx.payload[2] * 0.1217F * 25.4F * 0.1333F; // kPa
                                        break;
                                            
                                    case 0xEE: // tripmeter value [km]
                                        CCD_TRIPMETER = ((PacketRx.payload[1] << 16) + (PacketRx.payload[2] << 8) + PacketRx.payload[3]) * (128F / 4971F);
                                        break;
                                            
                                    default:
                                        break;
                                }

                                StringBuilder ccd_sensor_data = new StringBuilder();
                                ccd_sensor_data.Append("   ENGINE SPEED : " + Convert.ToString(CCD_RPM) + " rpm" + Environment.NewLine);
                                ccd_sensor_data.Append("  VEHICLE SPEED : " + Convert.ToString(CCD_SPEED) + " km/h" + Environment.NewLine);
                                ccd_sensor_data.Append("     MAP SENSOR : " + Convert.ToString(Math.Round(CCD_MAP, 1)) + " kPa" + Environment.NewLine);
                                ccd_sensor_data.Append(" THROTTLE PEDAL : " + Convert.ToString(Math.Round(CCD_TPS_PERC, 1)) + " %" + Environment.NewLine);
                                ccd_sensor_data.Append("   COOLANT TEMP : " + Convert.ToString(CCD_ECT_TEMP) + " °C" + Environment.NewLine);
                                ccd_sensor_data.Append("   AMBIENT TEMP : " + Convert.ToString(CCD_AMB_TEMP) + " °C" + Environment.NewLine);
                                ccd_sensor_data.Append("  BATTERY VOLTS : " + Convert.ToString(Math.Round(CCD_BATTERY_VOLTS, 1)) + " V" + Environment.NewLine);
                                ccd_sensor_data.Append(" CHARGING VOLTS : " + Convert.ToString(Math.Round(CCD_CHARGING_VOLTS, 1)) + " V" + Environment.NewLine);
                                ccd_sensor_data.Append("       ODOMETER : " + Convert.ToString(Math.Round(CCD_ODOMETER, 1)) + " km" + Environment.NewLine);
                                ccd_sensor_data.Append("      TRIPMETER : " + Convert.ToString(Math.Round(CCD_TRIPMETER, 1)) + " km" + Environment.NewLine);
                                ccd_sensor_data.Append("  LAST SHUTDOWN : " + Convert.ToString(CCD_TIMER) + " min(s)" + Environment.NewLine);
                                ccd_sensor_data.Append("   A/C PRESSURE : " + Convert.ToString(Math.Round(CCD_AC_PRESSURE, 1)) + " kPa" + Environment.NewLine);
                                CCDSensorList = ccd_sensor_data.ToString();
                                UpdateSensorTextbox();
                                break;
                                
                            default:
                                WritePacketTextBox("RX", "UNKNOWN BYTES", data);
                                break;
                        }
                        break;
                        
                    case PacketManager.from_sci_bus:
                        switch (dc_command)
                        {
                            case PacketManager.send_msg:
                                {
                                    if (PacketRx.payload[0] == 0x00) WritePacketTextBox("RX", "SCI-BUS MESSAGE RECEIVED", data);
                                    else if (PacketRx.payload[0] == 0xFF) WritePacketTextBox("RX", "SCI-BUS MESSAGE ERROR", data);
                                    break;
                                }
                            case PacketManager.receive_msg:
                                {
                                    WritePacketTextBox("RX", "SCI-BUS MESSAGE", data);

                                    // If filtering is active by ID-bytes (first byte of every message)
                                    if (sci_filter_bytes != null)
                                    {
                                        if (sci_filter_bytes.Contains(PacketRx.payload[0]))
                                        {
                                            try { WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload); }
                                            catch {  }
                                        }
                                        else { /* Ignore this message */ }
                                    }
                                    else // Filtering disabled, show everything
                                    {
                                        try { WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload); }
                                        catch {  }
                                    }

                                    // Write things to other textboxes
                                    // SCI -bus low speed mode
                                    if (!SCIHiSpeed)
                                    {
                                        // PCM DTC List
                                        if ((PacketRx.payload[0] == 0x10) && (PacketRx.payload.Length > 2))
                                        {
                                            DTCListTextbox.Clear();
                                            DTCListTextbox.AppendText("Source: SCI-bus" + Environment.NewLine + Environment.NewLine);

                                            if (PCMTCMSelectorComboBox.SelectedIndex == 1) DTCListTextbox.AppendText("$10: PCM DTC List (" + Convert.ToString(PacketRx.payload.Length - 3) + " DTC)" + Environment.NewLine);
                                            if (PCMTCMSelectorComboBox.SelectedIndex == 2) DTCListTextbox.AppendText("$10: TCM DTC List (" + Convert.ToString(PacketRx.payload.Length - 3) + " DTC)" + Environment.NewLine);

                                            for (int i = 0; i < (PacketRx.payload.Length - 2); i++)
                                            {
                                                string tempstring;

                                                try
                                                {
                                                    tempstring = pcmdtctable.lookup_pcm_dtc(PacketRx.payload[i + 1]).ToString();
                                                }
                                                catch
                                                {
                                                    tempstring = "UNRECOGNIZED DTC";
                                                }
                                                DTCListTextbox.AppendText("$" + Convert.ToString(PacketRx.payload[i + 1], 16).PadLeft(2, '0').ToUpper() + ": " + tempstring + Environment.NewLine);
                                            }

                                            // Validate checksum
                                            int calculated_checksum = 0;
                                            for (int i = 0; i < (PacketRx.payload.Length - 1); i++)
                                            {
                                                calculated_checksum += PacketRx.payload[i];
                                            }
                                            calculated_checksum = calculated_checksum & 0xFF;
                                            if (calculated_checksum == PacketRx.payload[PacketRx.payload.Length - 1])
                                            {
                                                DTCListTextbox.AppendText("$" + Convert.ToString(PacketRx.payload[PacketRx.payload.Length - 1], 16).PadLeft(2, '0').ToUpper() + ": CHECKSUM OK! (" + "=" + Convert.ToString(calculated_checksum, 16).PadLeft(2, '0').ToUpper() + ")" + Environment.NewLine);
                                            }
                                            else
                                            {
                                                DTCListTextbox.AppendText("$" + Convert.ToString(PacketRx.payload[PacketRx.payload.Length - 1], 16).PadLeft(2, '0').ToUpper() + ": CHECKSUM ERROR! (" + "!=" + Convert.ToString(calculated_checksum, 16).PadLeft(2, '0').ToUpper() + ")" + Environment.NewLine);
                                            }

                                            if ((PacketRx.payload.Length == 3) && (PacketRx.payload[1] == 0xFE))
                                            {
                                                DTCListTextbox.AppendText("No DTC found!" + Environment.NewLine);
                                            }

                                            DTCListTextbox.AppendText(Environment.NewLine);
                                        }

                                        // PCM DTC clear
                                        if ((PacketRx.payload[0] == 0x17) && (PacketRx.payload.Length > 1))
                                        {
                                            if (PacketRx.payload[1] == 0xE0) DTCListTextbox.AppendText("PCM DTC list erased!" + Environment.NewLine);
                                        }

                                        // Diagnostic data
                                        if ((PacketRx.payload[0] == 0x14) && (PacketRx.payload.Length > 1))
                                        {
                                            switch (PacketRx.payload[1])
                                            {
                                                case 0x01:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_AMB_TEMP_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_AMB_TEMP_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x02:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_O2_VOLTS1 = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_O2_VOLTS1 = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x05:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_COOLANT_TEMP = PacketRx.payload[2] - 128; // °C
                                                    }
                                                    else
                                                    {
                                                        PCM_COOLANT_TEMP = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x06:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_COOLANT_TEMP_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_COOLANT_TEMP_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x07:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_TPS_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_TPS_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x08:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_TPS_MIN_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_TPS_MIN_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x09:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_KNOCK_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_KNOCK_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x0A:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_BATTERY_VOLTS = (float)Math.Round(PacketRx.payload[2] * 0.0618F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_BATTERY_VOLTS = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x0B:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_MAP_VOLTS1 = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_MAP_VOLTS1 = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x0C:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_TARGET_IAC_POS = PacketRx.payload[2];
                                                    }
                                                    else
                                                    {
                                                        PCM_TARGET_IAC_POS = 0;
                                                    }
                                                    break;

                                                case 0x11:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_RPM1 = PacketRx.payload[2] * 32;
                                                    }
                                                    else
                                                    {
                                                        PCM_RPM1 = 0;
                                                    }
                                                    break;
                                                    
                                                case 0x40:
                                                    if (PacketRx.payload.Length > 2)
                                                    {
                                                        PCM_MAP_VOLTS2 = (float)Math.Round(PacketRx.payload[2] * 0.0196F, 4);
                                                    }
                                                    else
                                                    {
                                                        PCM_MAP_VOLTS2 = 0;
                                                    }
                                                    break;
                                                    
                                                default:
                                                    break;
                                            }
                                        }

                                        StringBuilder sci_sensor_data = new StringBuilder();
                                        sci_sensor_data.Append(" AMB TEMP VOLTS : " + Convert.ToString(PCM_AMB_TEMP_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("       O2 VOLTS : " + Convert.ToString(PCM_O2_VOLTS1) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("   COOLANT TEMP : " + Convert.ToString(PCM_COOLANT_TEMP) + " °C" + Environment.NewLine);
                                        sci_sensor_data.Append("CLNT TEMP VOLTS : " + Convert.ToString(PCM_COOLANT_TEMP_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("      TPS VOLTS : " + Convert.ToString(PCM_TPS_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("  TPS MIN VOLTS : " + Convert.ToString(PCM_TPS_MIN_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("    KNOCK VOLTS : " + Convert.ToString(PCM_KNOCK_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("  BATTERY VOLTS : " + Convert.ToString(PCM_BATTERY_VOLTS) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append("   MAP VOLTS #1 : " + Convert.ToString(PCM_MAP_VOLTS1) + " V" + Environment.NewLine);
                                        sci_sensor_data.Append(" IAC TARGET POS : " + Convert.ToString(PCM_TARGET_IAC_POS) + Environment.NewLine);
                                        sci_sensor_data.Append("   ENGINE SPEED : " + Convert.ToString(PCM_RPM1) + " rpm" + Environment.NewLine);
                                        sci_sensor_data.Append("   MAP VOLTS #2 : " + Convert.ToString(PCM_MAP_VOLTS2) + " V" + Environment.NewLine);
                                        SCISensorList = sci_sensor_data.ToString();
                                        UpdateSensorTextbox();
                                    }
                                    else // SCI-bus high speed mode
                                    {
                                        
                                    }
                                   
                                    break;
                                }
                            case PacketManager.send_rep_msg:
                                WritePacketTextBox("RX", "REP BYTES", data);

                                // If filtering is active by ID-bytes (first byte of every message)
                                if (sci_filter_bytes != null)
                                {
                                    if (sci_filter_bytes.Contains(PacketRx.payload[0]))
                                    {
                                        try { WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload); }
                                        catch { }
                                    }
                                    else { /* Ignore this message */ }
                                }
                                else // Filtering disabled, show everything
                                {
                                    try { WriteCCDSCIBusTextBox(SCIBusMsgTextBox, PacketRx.payload); }
                                    catch { }
                                }
                                
                                if (!SCIHiSpeed)
                                {
                                    // low-speed mode
                                }
                                else
                                {
                                    // high-speed mode
                                    switch (subdatacode)
                                    {
                                        case 0xF4: // SCI-bus high speed mode memory area selected
                                                   // The first section of the payload is the original request command (including the F4 pointer and the selected address)
                                                   // The second section is the same but the addresses are replaced with the response byte(s)
                                            if (PacketRx.payload[0] == 0xF4) // make sure that the first byte is indeed 0xF4
                                            {
                                                switch (PacketRx.payload[1]) // take a look at the second byte and decide what to do
                                                {
                                                    case 0x0A: // F4 0A 0B	- engine rpm			- scaling: RAW * 0.125 [rpm]
                                                        F4_ENG_RPM = ((PacketRx.payload[4] << 8) + PacketRx.payload[5]) / 8F;
                                                        break;

                                                    case 0x35: // F4 35 36	- target idle rpm		- scaling: RAW * 0.125 [rpm]
                                                        F4_TARGET_IDLE = ((PacketRx.payload[4] << 8) + PacketRx.payload[5]) / 8F;
                                                        break;

                                                    case 0x0C: // F4 0C 0D	- speed					- scaling: RAW * 0.0251057664 [km/h]
                                                        F4_SPEED = ((PacketRx.payload[4] << 8) + PacketRx.payload[5]) * 0.0251057664F;
                                                        break;

                                                    case 0x0F: // F4 0F		- battery voltage		- scaling: RAW * 0.0618 [V]
                                                        F4_BATT_VOLTS = PacketRx.payload[3] * 0.0618F;
                                                        break;

                                                    case 0x3A: // F4 3A		- charging voltage		- scaling: RAW * 0.0618 [V]
                                                        F4_CHARGE_VOLTS = PacketRx.payload[3] * 0.0618F;
                                                        break;

                                                    case 0x12: // F4 12		- tps volts				- scaling: RAW * 0.0196 [V]
                                                        F4_TPS_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x13: // F4 13		- minimum tps volts		- scaling: RAW * 0.0196 [V]
                                                        F4_MIN_TPS_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x14: // F4 14		- calculated tps volts	- scaling: RAW * 0.0196 [V]
                                                        F4_CALC_TPS_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x17: // F4 17		- map volts				- scaling: RAW * 0.0196 [V]
                                                        F4_MAP_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0xDA: // F4 DA		- map vacuum			- scaling: RAW * 0.412054294 [kPa]
                                                        F4_MAP_VACUUM = PacketRx.payload[3] * 0.412054294F;
                                                        break;

                                                    case 0x1B: // F4 1B     - o2 volts              - scaling: RAW * 0.0196[V]
                                                        F4_O2_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x27: // F4 27 28  - injector pulsewidth   - scaling: RAW * 0.004[ms]
                                                        F4_INJ_PW = ((PacketRx.payload[4] << 8) + PacketRx.payload[5]) * 0.004F;
                                                        break;

                                                    case 0x3E: // F4 3E     - iac steps             - scaling: RAW * 1
                                                        F4_IAC_STEPS = PacketRx.payload[3];
                                                        break;

                                                    case 0x37: // F4 37     - target iac steps      - scaling: RAW * 1
                                                        F4_TARGET_IAC_STEPS = PacketRx.payload[3];
                                                        break;

                                                    case 0x2F: // F4 2F     - spark advance         - scaling: RAW * 0.5[°]
                                                        F4_SPARK_ADV = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x31: // F4 31     - cylinder 1 retard     - scaling: RAW * 0.5[°]
                                                        F4_CYL_1_RETARD = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x32: // F4 32     - cylinder 2 retard     - scaling: RAW * 0.5[°]
                                                        F4_CYL_2_RETARD = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x33: // F4 33     - cylinder 3 retard     - scaling: RAW * 0.5[°]
                                                        F4_CYL_3_RETARD = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x34: // F4 34     - cylinder 4 retard     - scaling: RAW * 0.5[°]
                                                        F4_CYL_4_RETARD = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x1F: // F4 1F     - knock volts           - scaling: RAW * 0.0196[V]
                                                        F4_KNOCK_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x30: // F4 30     - overall knock retard  - scaling: RAW * 0.5[°]
                                                        F4_KNOCK_RETARD = PacketRx.payload[3] * 0.5F;
                                                        break;

                                                    case 0x41: // F4 41     - dis signal            - masking: 0x01: sync history -true: in-sync; false: out-of - sync
                                                               //                                              0x10: current sync state - true: in-sync; false: out-of - sync
                                                               //                                              0x20: ckp state -true: present; false: lost
                                                               //                                              0x40: cmp state -true: present; false: lost
                                                        F4_DIS_SIGNAL = PacketRx.payload[3];
                                                        break;

                                                    case 0x42: // F4 42     - fuel system status    - masking: 0x01: open loop
                                                               //                                              0x02: closed loop
                                                               //                                              0x04: open loop drive
                                                               //                                              0x08: open loop dtc
                                                               //                                              0x10: closed loop dtc
                                                               //                                              0x20: o2 sensor is updating adaptive memory
                                                               //                                              0x40: closed loop
                                                               //                                              0x80: ready for closed loop
                                                        F4_FUEL_SYSTEM_STATUS = PacketRx.payload[3];
                                                        break;

                                                    case 0x4A: // F4 4A     - closed loop timer     - scaling: RAW * 0.0535[min]
                                                        F4_CLOSED_LOOP_TIMER = PacketRx.payload[3] * 0.0535F;
                                                        break;

                                                    case 0x43: // F4 43     - current adaptive cell - scaling: RAW * 1
                                                        F4_CURRENT_ADAPTIVE_CELL = PacketRx.payload[3];
                                                        break;

                                                    case 0xD6: // F4 D6     - short-term adaptive   - scaling: RAW * 0.1953[%]
                                                        F4_STFT = PacketRx.payload[3] * 0.1953F;
                                                        break;

                                                    case 0xD7: // F4 D7     - long-term adaptive    - scaling: RAW * 0.1953[%]
                                                        F4_LTFT = PacketRx.payload[3] * 0.1953F;
                                                        break;

                                                    case 0xDE: // F4 DE     - fuel level volts      - scaling: RAW * 0.0196[V]
                                                        F4_FUEL_LEVEL_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0xE3: // F4 E3     - engine load           - scaling: RAW * 0.3922[%]
                                                        F4_ENGINE_LOAD = PacketRx.payload[3] * 0.3922F;
                                                        break;

                                                    case 0x10: // F4 10     - ambient temp volts    - scaling: RAW * 0.0196[V]
                                                        F4_AMB_TEMP_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x11: // F4 11     - ambient temp deg      - scaling: RAW - 128[°C]
                                                        F4_AMB_TEMP_DEG = (short)(PacketRx.payload[3] - 128);
                                                        break;

                                                    case 0x15: // F4 15     - engine coolant volts  - scaling: RAW * 0.0196[V]
                                                        F4_ECT_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x16: // F4 16     - engine coolant deg    - scaling: RAW - 128[°C]
                                                        F4_ECT_DEG = (short)(PacketRx.payload[3] - 128);
                                                        break;

                                                    case 0x1D: // F4 1D     - intake air temp volts - scaling: RAW * 0.0196[V]
                                                        F4_IAT_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x1E: // F4 1E     - intake air temp deg   - scaling: RAW - 128[°C]
                                                        F4_IAT_DEG = (short)(PacketRx.payload[3] - 128);
                                                        break;

                                                    case 0x25: // F4 25     - a / c pressure volts  - scaling: RAW * 0.0196[V]
                                                        F4_AC_PRESSURE_VOLTS = PacketRx.payload[3] * 0.0196F;
                                                        break;

                                                    case 0x26: // F4 26     - a / c pressure        - scaling: RAW * 13.52061848[kPa]
                                                        F4_AC_PRESSURE_KPA = PacketRx.payload[3] * 13.52061848F;
                                                        break;

                                                    case 0x19: // F4 19     - barometric pressure   - scaling: RAW * 0.412054294[kPa]
                                                        F4_BARO_PRESSURE = PacketRx.payload[3] * 0.412054294F;
                                                        break;

                                                    case 0x73: // F4 73     - limp-in reason        - masking: 0x08: IAT DTC (intake air temperature sensor malfunction)
                                                               //                                              0x10: TPS DTC (throttle pedal sensor malfunction)
                                                               //                                              0x20: MAP EL DTC(manifold absolute pressure sensor malfunction)
                                                               //                                              0x40: MAP VA DTC(manifold absolute pressure sensor malfunction)
                                                               //                                              0x80: ECT DTC (engine coolant temperature sensor malfunction)
                                                        F4_LIMP_REASON = PacketRx.payload[3];
                                                        break;

                                                    case 0x4B: // F4 4B 4C - time from start/ run   - scaling: RAW * 0.0002[min]
                                                        F4_RUNTIME = ((PacketRx.payload[4] << 8) + PacketRx.payload[5]) * 0.0002F;
                                                        break;

                                                    case 0x4D: // F4 4D     - runtime at stall      - scaling: RAW * 0.0535[min]
                                                        F4_RUNTIME_STALL = PacketRx.payload[3] * 0.0535F;
                                                        break;

                                                    case 0x3C: // F4 3C     - desired relay states  - masking: 0x10: asd relay -true: off; false: on
                                                        F4_DES_RELAY_STATES = PacketRx.payload[3];
                                                        break;

                                                    case 0x7A: // F4 7A - actual relay states   -masking: 0x10: asd relay -true: on; false: off
                                                        F4_ACT_RELAY_STATES = PacketRx.payload[3];
                                                        break;
                                                }
                                            }
                                            break;

                                        default:
                                            break;
                                    }

                                    StringBuilder sci_sensor_data = new StringBuilder();
                                    sci_sensor_data.Append("     ENGINE RPM : " + Convert.ToString(Math.Round(F4_ENG_RPM, 0)) + " rpm" + Environment.NewLine);
                                    sci_sensor_data.Append("    TARGET IDLE : " + Convert.ToString(Math.Round(F4_TARGET_IDLE, 0)) + " rpm" + Environment.NewLine);
                                    sci_sensor_data.Append("          SPEED : " + Convert.ToString(Math.Round(F4_SPEED, 1)) + " km/h" + Environment.NewLine);
                                    sci_sensor_data.Append("  BATTERY VOLTS : " + Convert.ToString(Math.Round(F4_BATT_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append(" CHARGING VOLTS : " + Convert.ToString(Math.Round(F4_CHARGE_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("      TPS VOLTS : " + Convert.ToString(Math.Round(F4_TPS_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("  MIN TPS VOLTS : " + Convert.ToString(Math.Round(F4_MIN_TPS_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append(" CALC TPS VOLTS : " + Convert.ToString(Math.Round(F4_CALC_TPS_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("      MAP VOLTS : " + Convert.ToString(Math.Round(F4_MAP_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("     MAP VACUUM : " + Convert.ToString(Math.Round(F4_MAP_VACUUM, 1)) + " kPa" + Environment.NewLine);
                                    sci_sensor_data.Append("       O2 VOLTS : " + Convert.ToString(Math.Round(F4_O2_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("    INJECTOR PW : " + Convert.ToString(Math.Round(F4_INJ_PW, 1)) + " ms" + Environment.NewLine);
                                    sci_sensor_data.Append("      IAC STEPS : " + Convert.ToString(F4_IAC_STEPS) + Environment.NewLine);
                                    sci_sensor_data.Append("     TARGET IAC : " + Convert.ToString(F4_TARGET_IAC_STEPS) + Environment.NewLine);
                                    sci_sensor_data.Append("  SPARK ADVANCE : " + Convert.ToString(Math.Round(F4_SPARK_ADV, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("   CYL 1 RETARD : " + Convert.ToString(Math.Round(F4_CYL_1_RETARD, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("   CYL 2 RETARD : " + Convert.ToString(Math.Round(F4_CYL_2_RETARD, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("   CYL 3 RETARD : " + Convert.ToString(Math.Round(F4_CYL_3_RETARD, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("   CYL 4 RETARD : " + Convert.ToString(Math.Round(F4_CYL_4_RETARD, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("    KNOCK VOLTS : " + Convert.ToString(Math.Round(F4_KNOCK_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("   KNOCK RETARD : " + Convert.ToString(Math.Round(F4_KNOCK_RETARD, 1)) + "°" + Environment.NewLine);
                                    sci_sensor_data.Append("     DIS SIGNAL : " + Convert.ToString(F4_DIS_SIGNAL, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    sci_sensor_data.Append("FUEL SYS STATUS : " + Convert.ToString(F4_FUEL_SYSTEM_STATUS, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    sci_sensor_data.Append("      C-L TIMER : " + Convert.ToString(Math.Round(F4_CLOSED_LOOP_TIMER, 3)) + " min" + Environment.NewLine);
                                    sci_sensor_data.Append("  ADAPTIVE CELL : " + Convert.ToString(F4_CURRENT_ADAPTIVE_CELL, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    sci_sensor_data.Append("SHORT-TERM ADAP : " + Convert.ToString(Math.Round(F4_STFT, 2)) + "%" + Environment.NewLine);
                                    sci_sensor_data.Append(" LONG-TERM ADAP : " + Convert.ToString(Math.Round(F4_LTFT, 2)) + "%" + Environment.NewLine);
                                    sci_sensor_data.Append(" FUEL LVL VOLTS : " + Convert.ToString(Math.Round(F4_FUEL_LEVEL_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("    ENGINE LOAD : " + Convert.ToString(Math.Round(F4_ENGINE_LOAD, 2)) + "%" + Environment.NewLine);
                                    sci_sensor_data.Append(" AMB TEMP VOLTS : " + Convert.ToString(Math.Round(F4_AMB_TEMP_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("AMB TEMP DEGREE : " + Convert.ToString(F4_AMB_TEMP_DEG) + " °C" + Environment.NewLine);
                                    sci_sensor_data.Append("      ECT VOLTS : " + Convert.ToString(Math.Round(F4_ECT_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("     ECT DEGREE : " + Convert.ToString(F4_ECT_DEG) + " °C" + Environment.NewLine);
                                    sci_sensor_data.Append("      IAT VOLTS : " + Convert.ToString(Math.Round(F4_IAT_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("     IAT DEGREE : " + Convert.ToString(F4_IAT_DEG) + " °C" + Environment.NewLine);
                                    sci_sensor_data.Append("A/C PRESS VOLTS : " + Convert.ToString(Math.Round(F4_AC_PRESSURE_VOLTS, 3)) + " V" + Environment.NewLine);
                                    sci_sensor_data.Append("   A/C PRESSURE : " + Convert.ToString(Math.Round(F4_AC_PRESSURE_KPA, 1)) + " kPa" + Environment.NewLine);
                                    sci_sensor_data.Append("  BARO PRESSURE : " + Convert.ToString(Math.Round(F4_BARO_PRESSURE, 1)) + " kPa" + Environment.NewLine);
                                    sci_sensor_data.Append(" LIMP-IN REASON : " + Convert.ToString(F4_LIMP_REASON, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    sci_sensor_data.Append("        RUNTIME : " + Convert.ToString(Math.Round(F4_RUNTIME, 3)) + " min" + Environment.NewLine);
                                    sci_sensor_data.Append("RUNTIME @ STALL : " + Convert.ToString(Math.Round(F4_RUNTIME_STALL, 3)) + " min" + Environment.NewLine);
                                    sci_sensor_data.Append("DES RELAY STATE : " + Convert.ToString(F4_DES_RELAY_STATES, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    sci_sensor_data.Append("ACT RELAY STATE : " + Convert.ToString(F4_ACT_RELAY_STATES, 16).PadLeft(1, '0').ToUpper() + Environment.NewLine);
                                    SCISensorList = sci_sensor_data.ToString();
                                    UpdateSensorTextbox();
                                }

                                break;

                            default:
                                WritePacketTextBox("RX", "UNKNOWN BYTES", data);
                                break;
                        }
                        break;
                        
                    default:
                        WritePacketTextBox("RX", "UNKNOWN BYTES", data);
                        break;
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
                    string temp = "";
                    char[] charsToTrim = { ',', ' ' };
                    for (int i = 0; i < ports.Length; i++)
                    {
                        temp += ports[i] + ", ";
                    }
                    temp = temp.TrimEnd(charsToTrim);

                    WriteCommandHistory("Available COM-ports: " + temp);

                    // Iterate through all available ports until one responds correctly
                    foreach (string port in ports)
                    {
                        Serial = new SerialPortStream(port, 250000, 8, Parity.None, StopBits.One);
                        WriteCommandHistory("Connecting to " + Serial.PortName + "...");

                        // Try 5 times
                        for (int i = 0; i < 5; i++)
                        {
                            // If this method returns true then a valid handshake has been received
                            if (GetHandshake())
                            {
                                scanner_connected = true;
                                WriteCommandHistory("Scanner found and connected!");
                                DisconnectButton.Text = "Disconnect (" + Serial.PortName + ")";

                                // Enable visual components
                                ComponentsEnabled();

                                // Kick off serial monitor for receiving data
                                ReadSerialData();

                                ConnectButton.Enabled = false;
                                DisconnectButton.Enabled = true;
                                StatusButton.Enabled = true;
                                //StatusButton.PerformClick();
                                break;
                            }
                            else // None of the available ports are connected to the scanner
                            {
                                scanner_connected = false;
                                ConnectButton.Enabled = true;
                                Serial.Close();
                            }
                        }

                        if (scanner_connected) break;
                        else
                        {
                            WriteCommandHistory("No scanner found on " + Serial.PortName + "!");
                            Serial.Dispose();
                        }
                    }
                }
            }
            catch
            {
                ConnectButton.Enabled = true;
                WriteCommandHistory("Scanner not found!");
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
                    // Wait here until all bytes are received (we know that the handshake packet is 27 bytes long) or timeout occurs.
                }
                TimeoutTimer.Enabled = false;
                if (timeout)
                {
                    timeout = false;
                    WritePacketTextBox("RX", "TIMEOUT (" + Serial.PortName + ")", null);
                    Serial.Close();
                    GC.Collect(); // Get rid of garbage in the RAM
                    return false;
                }
                else
                {
                    // Manually save the received bytes from the scanner (automatic serial monitor is not started yet)
                    byte[] data = new byte[27];
                    Serial.Read(data, 0, Serial.BytesToRead);

                    // Convert the packet's payload section (5-21) into a string
                    if (Encoding.ASCII.GetString(data, 5, 21) == "CHRYSLERCCDSCISCANNER")
                    {
                        ProcessData(data); // Get this packet processed by a method
                        BufferStartLabel.Text = "Buffer Start: " + SerialRxBuffer.Start;
                        BufferEndLabel.Text = "Buffer End: " + SerialRxBuffer.End;
                        BufferReadlengthLabel.Text = "Buffer ReadLength: " + SerialRxBuffer.ReadLength + " byte(s)";
                        BufferWritelengthLabel.Text = "Buffer WriteLength: " + SerialRxBuffer.WriteLength + " byte(s)";
                        return true;
                    }
                    else // The handshake is incorrect
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

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (Serial.IsOpen)
            {
                CCDBusEnabledCheckbox.Checked = false;
                CCDBusMsgFilterCheckbox.Checked = false;
                SCIBusEnabledCheckbox.Checked = false;
                SCIBusMsgFilterCheckbox.Checked = false;

                ccd_filtering_active = false;
                sci_filtering_active = false;

                ccd_filter_bytes = null;
                sci_filter_bytes = null;

                Serial.Close();
                DisconnectButton.Text = "Disconnect";
                WriteCommandHistory("Scanner disconnected!");
                ComponentsDisabled();

                SerialRxBuffer.Reset();
                SerialTxBuffer.Reset();

                scanner_connected = false;
                ConnectButton.Enabled = true;
                DisconnectButton.Enabled = false;
                StatusButton.Enabled = false;

                GC.Collect();
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.status, PacketManager.ok, null);
            WriteSerialData(PacketTx.ToBytes());
            WritePacketTextBox("TX", "REQUEST STATUS", PacketTx.ToBytes());
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
                // Convert text to bytes
                byte[] bytes = Util.GetBytes(CCDBusSendMsgTextBox.Text);

                // Check if the last checksum byte if correct, if not fix it
                int temp2 = 0;
                for (int i = 0; i < bytes.Length - 1; i++)
                {
                    temp2 += bytes[i];
                }

                // Keep the lower byte
                temp2 &= 0xFF;

                // Overwrite last byte (doesn't matter what was manually entered)
                bytes[bytes.Length - 1] = (byte)temp2;

                // Don't mess with the odometer value yet (0x84)
                if (bytes.Length > 0 && (bytes[0] != 0x84))
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

                if (bytes.Length > 0)
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

                if (CCDBusSendMsgTextBox.Text != "") CCDBusSendMsgButton.Enabled = true;
                if (CCDBusMsgTextBox.Text != "") CCDBusClearMsgButton.Enabled = true;

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
                SCIBusHSLSButton.Enabled = true;
                SCIEnabled = true;
                PCMRAMGroupbox.Enabled = true;

                if (SCIBusSendMsgTextBox.Text != "") SCIBusSendMsgButton.Enabled = true;
                if (SCIBusMsgTextBox.Text != "") SCIBusClearMsgButton.Enabled = true;

                byte[] SelectedSCIbus = new byte[] { (byte)(PCMTCMSelectorComboBox.SelectedIndex) };
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_sci_bus, SelectedSCIbus);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                if (SelectedSCIbus[0] == 0x01) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (PCM)", temp);
                else if (SelectedSCIbus[0] == 0x02) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (TCM)", temp);
            }
            else
            {
                if (SCIHiSpeed)
                {
                    SCIBusHSLSButton.PerformClick();
                }

                SCIBusMsgTextBox.Enabled = false;
                SCIBusSendMsgTextBox.Enabled = false;
                SCIBusSendMsgButton.Enabled = false;
                SCIBusClearMsgButton.Enabled = false;
                SCIBusHSLSButton.Enabled = false;
                SCIEnabled = false;
                PCMRAMGroupbox.Enabled = false;

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
                if (CCDBusMsgFilterTextBox.Text != "")
                {
                    CCDBusMsgFilterApplyButton.Enabled = true;
                    CCDBusMsgFilterClearButton.Enabled = true;

                    // Re-activate filtering here 
                    byte[] temp = Util.GetBytes(CCDBusMsgFilterTextBox.Text);
                    ccd_filter_bytes = new byte[temp.Length];
                    Array.Copy(temp, ccd_filter_bytes, temp.Length);
                }
            }
            else
            {
                CCDBusMsgFilterTextBox.Enabled = false;
                CCDBusMsgFilterApplyButton.Enabled = false;
                CCDBusMsgFilterClearButton.Enabled = false;
                ccd_filter_bytes = null;
            }
        }

        private void SCIBusMsgFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SCIBusMsgFilterCheckbox.Checked)
            {
                SCIBusMsgFilterTextBox.Enabled = true;
                if (SCIBusMsgFilterTextBox.Text != "")
                {
                    SCIBusMsgFilterApplyButton.Enabled = true;
                    SCIBusMsgFilterClearButton.Enabled = true;

                    // Re-activate filtering here 
                    byte[] temp = Util.GetBytes(SCIBusMsgFilterTextBox.Text);
                    sci_filter_bytes = new byte[temp.Length];
                    Array.Copy(temp, sci_filter_bytes, temp.Length);
                }
            }
            else
            {
                SCIBusMsgFilterTextBox.Enabled = false;
                SCIBusMsgFilterApplyButton.Enabled = false;
                SCIBusMsgFilterClearButton.Enabled = false;
                sci_filter_bytes = null;
            }
        }

        private void packetGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (packetgenerator == null || !packetgenerator.Visible)
            {
                packetgenerator = new PacketGenerator(this);
                packetgenerator.Show();
            }
            else
            {
                packetgenerator.BringToFront();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (about == null || !about.Visible)
            {
                about = new AboutForm(this);
                about.Show();
            }
            else
            {
                about.BringToFront();
            }
        }

        //  Open new window or bring it to front if already opened
        private void RealTimeDiagnosticsButton_Click(object sender, EventArgs e)
        {
            if (diagnostics == null || !diagnostics.Visible)
            {
                diagnostics = new DiagnosticsForm();
                diagnostics.Show();
            }
            else
            {
                diagnostics.BringToFront();
            }
        }

        private void DRBDBExplorerButton_Click(object sender, EventArgs e)
        {
            if (drbdbexplorer == null || !drbdbexplorer.Visible)
            {
                drbdbexplorer = new DRBDBExplorer();
                drbdbexplorer.Show();
            }
            else
            {
                drbdbexplorer.BringToFront();
            }
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
                    byte[] supercard2_header = new byte[] { 0x63, 0x61, 0x72, 0x64, 0x00, 0x00, 0x73, 0x70, 0x72, 0x32 };

                    temp = scReader.ReadBytes(ref pos, 10);

                    if (temp == supercard2_header)
                    {
                        // Search for "file start signatures" using the raw byte field inside SimpleBinaryReader
                        while (!done)
                        {
                            pos = Util.SearchBytes(scReader.rawDB, pos, file_start_signature);
                            if (pos != -1) // the SearchBytes method returns -1 if there's nothing more to find
                            {
                                temp = scReader.ReadBytes(ref pos, 30);
                                SensorDataTextbox.AppendText(Encoding.ASCII.GetString(temp, 7, 23) + "\n");
                            }
                            else
                            {
                                done = true;
                            }
                        }
                    }
                    else
                    {
                        SensorDataTextbox.AppendText("The loaded file is not a SuperCard binary!\n");
                        SensorDataTextbox.AppendText(Encoding.ASCII.GetString(temp, 0, 10) + "\n");

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
            if (SCIBusEnabledCheckbox.Checked)
            {
                if (SCIHiSpeed) SCIBusHSLSButton.PerformClick(); // in case of High Speed Mode return to normal low speed before changin SCI-bus target

                byte[] SelectedSCIbus = new byte[] { (byte)(PCMTCMSelectorComboBox.SelectedIndex) }; // add 1 because of zero based indexing
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_sci_bus, SelectedSCIbus);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                if (SelectedSCIbus[0] == 0x01) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (PCM)", temp);
                else if (SelectedSCIbus[0] == 0x02) WritePacketTextBox("TX", "SETTINGS / SCI-BUS ON (TCM)", temp);
            }
        }
        
        // Prevent mousewheel scrolling through this combobox
        void PCMTCMSelectorComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void ScanModulesButton_Click(object sender, EventArgs e)
        {
            ScanModulesButton.Enabled = false;
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.request, PacketManager.scan_vehicle_modules, null);
            byte[] temp = PacketTx.ToBytes();
            WriteSerialData(temp);
            WritePacketTextBox("TX", "REQUEST / CCD-BUS MODULE LIST", temp);
        }

        private void SCIBusHSLSButton_Click(object sender, EventArgs e)
        {
            if (SCIHiSpeed)
            {
                // Exit high speed mode
                // Fill textbox with the SCI-bus command as if someone typed manually
                SCIBusSendMsgTextBox.Text = "FE";

                // Click the "Send" button
                SCIBusSendMsgButton.PerformClick();

                // Clear textbox
                SCIBusSendMsgTextBox.Clear();

                // Clear flag
                SCIHiSpeed = false;

                // Generate a valid packet inside PacketManager
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.disable_sci_hi_speed, null);

                // Convert the packet structure into a byte-array
                byte[] temp = PacketTx.ToBytes();

                // Send it through serial link
                WriteSerialData(temp);

                // Notify the user that something happened
                WritePacketTextBox("TX", "SETTINGS / SCI-BUS HIGH SPEED OFF", temp);
            }
            else
            {
                // Enter high speed mode
                SCIBusSendMsgTextBox.Text = "12";
                SCIBusSendMsgButton.PerformClick();
                SCIBusSendMsgTextBox.Clear();

                SCIHiSpeed = true;
                PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.settings, PacketManager.enable_sci_hi_speed, null);
                byte[] temp = PacketTx.ToBytes();
                WriteSerialData(temp);
                WritePacketTextBox("TX", "SETTINGS / SCI-BUS HIGH SPEED ON", temp);
            }
        }

        private void SCIHSModeScanButton_Click(object sender, EventArgs e)
        {
            // Check if PCM has been selected as SCI-bus target
            if (PCMTCMSelectorComboBox.SelectedIndex != 0x01)
            {
                PCMTCMSelectorComboBox.SelectedIndex = 0x01;
                SCIBusEnabledCheckbox.Checked = false;
                SCIBusEnabledCheckbox.Checked = true;
            }

            if (!SCIHiSpeed)
            {
                SCIBusHSLSButton.PerformClick();
                SCIHiSpeed = true;
            }
            // 33 00 03 14 FB F4 17 
            byte[] dummy = new byte[1] { (byte)(SCIHSModeComboBox.SelectedIndex + 0xF0) };
            PacketTx.GeneratePacket(PacketManager.from_laptop, PacketManager.to_scanner, PacketManager.request, 0xFB, dummy);
            byte[] temp = PacketTx.ToBytes();
            WriteSerialData(temp);
            WritePacketTextBox("TX", "REQUEST / SCI-BUS HIGH SPEED MEMORY DUMP", temp);
        }

        private void PCMRAMRepeatCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (PCMRAMRepeatCheckbox.Checked)
            {
                RepeatTextbox.Enabled = true;
            }
            else
            {
                RepeatTextbox.Enabled = false;
            }
        }

        private void SCIHSModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PCMRAMMode = (byte)SCIHSModeComboBox.SelectedIndex;
            CounterLabel.Text = PCMRAMMode.ToString();
        }

        // Take measures before exiting the application
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close opened forms
            if (drbdbexplorer != null) drbdbexplorer.Close();
            if (packetgenerator != null) packetgenerator.Close();
            if (about != null) about.Close();
            if (diagnostics != null) diagnostics.Close();

            // Close every communication channel before closing
            CCDBusMsgFilterCheckbox.Checked = false;
            CCDBusEnabledCheckbox.Checked = false;
            SCIBusHSLSButton.PerformClick(); // return to low speed mode (SCI-bus)
            SCIBusMsgFilterCheckbox.Checked = false;
            SCIBusEnabledCheckbox.Checked = false;
            DisconnectButton.PerformClick();
            Thread.Sleep(100); // wait for a little bit
            Environment.Exit(0);
        }

        private void UpdateSensorTextbox()
        {
            SensorDataTextbox.Text = "             CCD-BUS" + Environment.NewLine + CCDSensorList + Environment.NewLine + "             SCI-BUS" + Environment.NewLine + SCISensorList;
        }
    }
}
