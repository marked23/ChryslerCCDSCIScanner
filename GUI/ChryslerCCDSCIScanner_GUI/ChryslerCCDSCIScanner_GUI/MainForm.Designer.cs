namespace ChryslerCCDSCIScanner_GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ConnectButton = new System.Windows.Forms.Button();
            this.PacketCountRxLabel = new System.Windows.Forms.Label();
            this.BufferWritelengthLabel = new System.Windows.Forms.Label();
            this.BufferStartLabel = new System.Windows.Forms.Label();
            this.BufferEndLabel = new System.Windows.Forms.Label();
            this.BufferReadlengthLabel = new System.Windows.Forms.Label();
            this.read_exteeprom_button = new System.Windows.Forms.Button();
            this.read_inteeprom_button = new System.Windows.Forms.Button();
            this.StatusButton = new System.Windows.Forms.Button();
            this.SCIBusSendMsgButton = new System.Windows.Forms.Button();
            this.SCIBusSendMsgTextBox = new System.Windows.Forms.TextBox();
            this.erase_exteeprom_button = new System.Windows.Forms.Button();
            this.PacketLogGroupbox = new System.Windows.Forms.GroupBox();
            this.PacketTextBox = new System.Windows.Forms.TextBox();
            this.TXLabel = new System.Windows.Forms.Label();
            this.RXLabel = new System.Windows.Forms.Label();
            this.PacketSendTextBox = new System.Windows.Forms.TextBox();
            this.PacketSendButton = new System.Windows.Forms.Button();
            this.PacketClearButton = new System.Windows.Forms.Button();
            this.PacketLogEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.superCardReaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CCDBusMessagesGroupbox = new System.Windows.Forms.GroupBox();
            this.CCDBusSpeedLabel = new System.Windows.Forms.Label();
            this.CCDBusMsgTextBox = new System.Windows.Forms.TextBox();
            this.CCDBusMsgFilterClearButton = new System.Windows.Forms.Button();
            this.CCDBusMsgFilterApplyButton = new System.Windows.Forms.Button();
            this.CCDBusMsgFilterTextBox = new System.Windows.Forms.TextBox();
            this.CCDBusMsgFilterCheckbox = new System.Windows.Forms.CheckBox();
            this.CCDBusSendMsgTextBox = new System.Windows.Forms.TextBox();
            this.CCDBusSendMsgButton = new System.Windows.Forms.Button();
            this.CCDBusEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.CCDBusClearMsgButton = new System.Windows.Forms.Button();
            this.SCIBusMessagesGroupbox = new System.Windows.Forms.GroupBox();
            this.SCIBusHSLSButton = new System.Windows.Forms.Button();
            this.SCIBusSpeedLabel = new System.Windows.Forms.Label();
            this.PCMTCMSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.SCIBusMsgTextBox = new System.Windows.Forms.TextBox();
            this.SCIBusMsgFilterClearButton = new System.Windows.Forms.Button();
            this.SCIBusMsgFilterApplyButton = new System.Windows.Forms.Button();
            this.SCIBusEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.SCIBusMsgFilterTextBox = new System.Windows.Forms.TextBox();
            this.SCIBusMsgFilterCheckbox = new System.Windows.Forms.CheckBox();
            this.SCIBusClearMsgButton = new System.Windows.Forms.Button();
            this.command_history_groupbox = new System.Windows.Forms.GroupBox();
            this.CommandHistoryTextBox = new System.Windows.Forms.TextBox();
            this.MiscGroupbox = new System.Windows.Forms.GroupBox();
            this.CounterLabel = new System.Windows.Forms.Label();
            this.PacketCountTxLabel = new System.Windows.Forms.Label();
            this.RebootScannerButton = new System.Windows.Forms.Button();
            this.SCIHSModeScanButton = new System.Windows.Forms.Button();
            this.SCIHSModeLabel = new System.Windows.Forms.Label();
            this.SCIHSModeComboBox = new System.Windows.Forms.ComboBox();
            this.ControlPanelGroupbox = new System.Windows.Forms.GroupBox();
            this.ExitButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.EEPROMGroupbox = new System.Windows.Forms.GroupBox();
            this.write_exteeprom_button = new System.Windows.Forms.Button();
            this.save_exteeprom_button = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.write_inteeprom_button = new System.Windows.Forms.Button();
            this.erase_inteeprom_button = new System.Windows.Forms.Button();
            this.save_inteeprom_button = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.RealTimeDiagnosticsGroupbox = new System.Windows.Forms.GroupBox();
            this.real_time_diagnostics_button = new System.Windows.Forms.Button();
            this.ReadDTCGroupbox = new System.Windows.Forms.GroupBox();
            this.ScanModulesButton = new System.Windows.Forms.Button();
            this.ModuleListComboBox = new System.Windows.Forms.ComboBox();
            this.ReadDTCByModuleButton = new System.Windows.Forms.Button();
            this.SensorDataGroupbox = new System.Windows.Forms.GroupBox();
            this.SensorDataTextbox = new System.Windows.Forms.TextBox();
            this.DTCListGroupbox = new System.Windows.Forms.GroupBox();
            this.DTCListTextbox = new System.Windows.Forms.TextBox();
            this.DRBDBExplorerButton = new System.Windows.Forms.Button();
            this.DRB3ToolsGroupbox = new System.Windows.Forms.GroupBox();
            this.PCMRAMGroupbox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.RepeatTextbox = new System.Windows.Forms.TextBox();
            this.PCMRAMRepeatCheckbox = new System.Windows.Forms.CheckBox();
            this.PacketLogGroupbox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.CCDBusMessagesGroupbox.SuspendLayout();
            this.SCIBusMessagesGroupbox.SuspendLayout();
            this.command_history_groupbox.SuspendLayout();
            this.MiscGroupbox.SuspendLayout();
            this.ControlPanelGroupbox.SuspendLayout();
            this.EEPROMGroupbox.SuspendLayout();
            this.RealTimeDiagnosticsGroupbox.SuspendLayout();
            this.ReadDTCGroupbox.SuspendLayout();
            this.SensorDataGroupbox.SuspendLayout();
            this.DTCListGroupbox.SuspendLayout();
            this.DRB3ToolsGroupbox.SuspendLayout();
            this.PCMRAMGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(6, 19);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(132, 23);
            this.ConnectButton.TabIndex = 1;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // PacketCountRxLabel
            // 
            this.PacketCountRxLabel.AutoSize = true;
            this.PacketCountRxLabel.Location = new System.Drawing.Point(6, 22);
            this.PacketCountRxLabel.Name = "PacketCountRxLabel";
            this.PacketCountRxLabel.Size = new System.Drawing.Size(96, 13);
            this.PacketCountRxLabel.TabIndex = 2;
            this.PacketCountRxLabel.Text = "Packets received: ";
            // 
            // BufferWritelengthLabel
            // 
            this.BufferWritelengthLabel.AutoSize = true;
            this.BufferWritelengthLabel.Location = new System.Drawing.Point(6, 87);
            this.BufferWritelengthLabel.Name = "BufferWritelengthLabel";
            this.BufferWritelengthLabel.Size = new System.Drawing.Size(99, 13);
            this.BufferWritelengthLabel.TabIndex = 3;
            this.BufferWritelengthLabel.Text = "Buffer WriteLength:";
            // 
            // BufferStartLabel
            // 
            this.BufferStartLabel.AutoSize = true;
            this.BufferStartLabel.Location = new System.Drawing.Point(6, 48);
            this.BufferStartLabel.Name = "BufferStartLabel";
            this.BufferStartLabel.Size = new System.Drawing.Size(63, 13);
            this.BufferStartLabel.TabIndex = 6;
            this.BufferStartLabel.Text = "Buffer Start:";
            // 
            // BufferEndLabel
            // 
            this.BufferEndLabel.AutoSize = true;
            this.BufferEndLabel.Location = new System.Drawing.Point(6, 61);
            this.BufferEndLabel.Name = "BufferEndLabel";
            this.BufferEndLabel.Size = new System.Drawing.Size(60, 13);
            this.BufferEndLabel.TabIndex = 7;
            this.BufferEndLabel.Text = "Buffer End:";
            // 
            // BufferReadlengthLabel
            // 
            this.BufferReadlengthLabel.AutoSize = true;
            this.BufferReadlengthLabel.Location = new System.Drawing.Point(6, 74);
            this.BufferReadlengthLabel.Name = "BufferReadlengthLabel";
            this.BufferReadlengthLabel.Size = new System.Drawing.Size(100, 13);
            this.BufferReadlengthLabel.TabIndex = 8;
            this.BufferReadlengthLabel.Text = "Buffer ReadLength:";
            // 
            // read_exteeprom_button
            // 
            this.read_exteeprom_button.Enabled = false;
            this.read_exteeprom_button.Location = new System.Drawing.Point(103, 47);
            this.read_exteeprom_button.Name = "read_exteeprom_button";
            this.read_exteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.read_exteeprom_button.TabIndex = 18;
            this.read_exteeprom_button.Text = "Read";
            this.read_exteeprom_button.UseVisualStyleBackColor = true;
            // 
            // read_inteeprom_button
            // 
            this.read_inteeprom_button.Enabled = false;
            this.read_inteeprom_button.Location = new System.Drawing.Point(103, 18);
            this.read_inteeprom_button.Name = "read_inteeprom_button";
            this.read_inteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.read_inteeprom_button.TabIndex = 20;
            this.read_inteeprom_button.Text = "Read";
            this.read_inteeprom_button.UseVisualStyleBackColor = true;
            // 
            // StatusButton
            // 
            this.StatusButton.Enabled = false;
            this.StatusButton.Location = new System.Drawing.Point(6, 77);
            this.StatusButton.Name = "StatusButton";
            this.StatusButton.Size = new System.Drawing.Size(132, 23);
            this.StatusButton.TabIndex = 21;
            this.StatusButton.Text = "Status";
            this.StatusButton.UseVisualStyleBackColor = true;
            this.StatusButton.Click += new System.EventHandler(this.StatusButton_Click);
            // 
            // SCIBusSendMsgButton
            // 
            this.SCIBusSendMsgButton.Enabled = false;
            this.SCIBusSendMsgButton.Location = new System.Drawing.Point(153, 254);
            this.SCIBusSendMsgButton.Name = "SCIBusSendMsgButton";
            this.SCIBusSendMsgButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusSendMsgButton.TabIndex = 26;
            this.SCIBusSendMsgButton.Text = "Send";
            this.SCIBusSendMsgButton.UseVisualStyleBackColor = true;
            this.SCIBusSendMsgButton.Click += new System.EventHandler(this.SCIBusSendMsgButton_Click);
            // 
            // SCIBusSendMsgTextBox
            // 
            this.SCIBusSendMsgTextBox.Enabled = false;
            this.SCIBusSendMsgTextBox.Font = new System.Drawing.Font("Courier New", 9F);
            this.SCIBusSendMsgTextBox.Location = new System.Drawing.Point(3, 256);
            this.SCIBusSendMsgTextBox.Name = "SCIBusSendMsgTextBox";
            this.SCIBusSendMsgTextBox.Size = new System.Drawing.Size(143, 21);
            this.SCIBusSendMsgTextBox.TabIndex = 27;
            this.SCIBusSendMsgTextBox.TextChanged += new System.EventHandler(this.SCIBusSendMsgTextBox_TextChanged);
            this.SCIBusSendMsgTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SCIBusSendMsgTextBox_KeyPress);
            // 
            // erase_exteeprom_button
            // 
            this.erase_exteeprom_button.Enabled = false;
            this.erase_exteeprom_button.Location = new System.Drawing.Point(301, 47);
            this.erase_exteeprom_button.Name = "erase_exteeprom_button";
            this.erase_exteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.erase_exteeprom_button.TabIndex = 30;
            this.erase_exteeprom_button.Text = "Erase";
            this.erase_exteeprom_button.UseVisualStyleBackColor = true;
            // 
            // PacketLogGroupbox
            // 
            this.PacketLogGroupbox.Controls.Add(this.PacketTextBox);
            this.PacketLogGroupbox.Controls.Add(this.TXLabel);
            this.PacketLogGroupbox.Controls.Add(this.RXLabel);
            this.PacketLogGroupbox.Controls.Add(this.PacketSendTextBox);
            this.PacketLogGroupbox.Controls.Add(this.PacketSendButton);
            this.PacketLogGroupbox.Controls.Add(this.PacketClearButton);
            this.PacketLogGroupbox.Controls.Add(this.PacketLogEnabledCheckbox);
            this.PacketLogGroupbox.Location = new System.Drawing.Point(12, 27);
            this.PacketLogGroupbox.Name = "PacketLogGroupbox";
            this.PacketLogGroupbox.Size = new System.Drawing.Size(365, 315);
            this.PacketLogGroupbox.TabIndex = 41;
            this.PacketLogGroupbox.TabStop = false;
            this.PacketLogGroupbox.Text = "COM packets";
            // 
            // PacketTextBox
            // 
            this.PacketTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PacketTextBox.Location = new System.Drawing.Point(3, 16);
            this.PacketTextBox.MaxLength = 0;
            this.PacketTextBox.Multiline = true;
            this.PacketTextBox.Name = "PacketTextBox";
            this.PacketTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PacketTextBox.Size = new System.Drawing.Size(359, 234);
            this.PacketTextBox.TabIndex = 65;
            this.PacketTextBox.TextChanged += new System.EventHandler(this.PacketTextBox_TextChanged);
            // 
            // TXLabel
            // 
            this.TXLabel.AutoSize = true;
            this.TXLabel.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.TXLabel.Location = new System.Drawing.Point(183, 289);
            this.TXLabel.Name = "TXLabel";
            this.TXLabel.Size = new System.Drawing.Size(78, 13);
            this.TXLabel.TabIndex = 56;
            this.TXLabel.Text = "TX: transmitted";
            // 
            // RXLabel
            // 
            this.RXLabel.AutoSize = true;
            this.RXLabel.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.RXLabel.Location = new System.Drawing.Point(108, 289);
            this.RXLabel.Name = "RXLabel";
            this.RXLabel.Size = new System.Drawing.Size(69, 13);
            this.RXLabel.TabIndex = 52;
            this.RXLabel.Text = "RX: received";
            // 
            // PacketSendTextBox
            // 
            this.PacketSendTextBox.Font = new System.Drawing.Font("Courier New", 9F);
            this.PacketSendTextBox.Location = new System.Drawing.Point(3, 256);
            this.PacketSendTextBox.Name = "PacketSendTextBox";
            this.PacketSendTextBox.Size = new System.Drawing.Size(289, 21);
            this.PacketSendTextBox.TabIndex = 55;
            this.PacketSendTextBox.TextChanged += new System.EventHandler(this.PacketSendTextBox_TextChanged);
            this.PacketSendTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PacketSendTextBox_KeyPress);
            // 
            // PacketSendButton
            // 
            this.PacketSendButton.Enabled = false;
            this.PacketSendButton.Location = new System.Drawing.Point(298, 254);
            this.PacketSendButton.Name = "PacketSendButton";
            this.PacketSendButton.Size = new System.Drawing.Size(60, 23);
            this.PacketSendButton.TabIndex = 54;
            this.PacketSendButton.Text = "Send";
            this.PacketSendButton.UseVisualStyleBackColor = true;
            this.PacketSendButton.Click += new System.EventHandler(this.PacketSendButton_Click);
            // 
            // PacketClearButton
            // 
            this.PacketClearButton.Enabled = false;
            this.PacketClearButton.Location = new System.Drawing.Point(298, 283);
            this.PacketClearButton.Name = "PacketClearButton";
            this.PacketClearButton.Size = new System.Drawing.Size(60, 23);
            this.PacketClearButton.TabIndex = 46;
            this.PacketClearButton.Text = "Clear";
            this.PacketClearButton.UseVisualStyleBackColor = true;
            this.PacketClearButton.Click += new System.EventHandler(this.PacketClearButton_Click);
            // 
            // PacketLogEnabledCheckbox
            // 
            this.PacketLogEnabledCheckbox.AutoSize = true;
            this.PacketLogEnabledCheckbox.Checked = true;
            this.PacketLogEnabledCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PacketLogEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.PacketLogEnabledCheckbox.Name = "PacketLogEnabledCheckbox";
            this.PacketLogEnabledCheckbox.Size = new System.Drawing.Size(77, 17);
            this.PacketLogEnabledCheckbox.TabIndex = 1;
            this.PacketLogEnabledCheckbox.Text = "Packet log";
            this.PacketLogEnabledCheckbox.UseVisualStyleBackColor = true;
            this.PacketLogEnabledCheckbox.CheckedChanged += new System.EventHandler(this.PacketLogEnabledCheckbox_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem1,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1224, 24);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packetGeneratorToolStripMenuItem,
            this.superCardReaderToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // packetGeneratorToolStripMenuItem
            // 
            this.packetGeneratorToolStripMenuItem.Name = "packetGeneratorToolStripMenuItem";
            this.packetGeneratorToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.packetGeneratorToolStripMenuItem.Text = "Packet Generator";
            this.packetGeneratorToolStripMenuItem.Click += new System.EventHandler(this.packetGeneratorToolStripMenuItem_Click);
            // 
            // superCardReaderToolStripMenuItem
            // 
            this.superCardReaderToolStripMenuItem.Name = "superCardReaderToolStripMenuItem";
            this.superCardReaderToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.superCardReaderToolStripMenuItem.Text = "SuperCard Reader";
            this.superCardReaderToolStripMenuItem.Click += new System.EventHandler(this.superCardReaderToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 20);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // CCDBusMessagesGroupbox
            // 
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusSpeedLabel);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgTextBox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterClearButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterApplyButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterTextBox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterCheckbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusSendMsgTextBox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusSendMsgButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusEnabledCheckbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusClearMsgButton);
            this.CCDBusMessagesGroupbox.Location = new System.Drawing.Point(383, 27);
            this.CCDBusMessagesGroupbox.Name = "CCDBusMessagesGroupbox";
            this.CCDBusMessagesGroupbox.Size = new System.Drawing.Size(220, 431);
            this.CCDBusMessagesGroupbox.TabIndex = 43;
            this.CCDBusMessagesGroupbox.TabStop = false;
            this.CCDBusMessagesGroupbox.Text = "CCD-bus messages";
            // 
            // CCDBusSpeedLabel
            // 
            this.CCDBusSpeedLabel.AutoSize = true;
            this.CCDBusSpeedLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CCDBusSpeedLabel.Location = new System.Drawing.Point(150, 312);
            this.CCDBusSpeedLabel.Name = "CCDBusSpeedLabel";
            this.CCDBusSpeedLabel.Size = new System.Drawing.Size(66, 13);
            this.CCDBusSpeedLabel.TabIndex = 73;
            this.CCDBusSpeedLabel.Text = "7812.5 kbps";
            // 
            // CCDBusMsgTextBox
            // 
            this.CCDBusMsgTextBox.Enabled = false;
            this.CCDBusMsgTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CCDBusMsgTextBox.Location = new System.Drawing.Point(3, 16);
            this.CCDBusMsgTextBox.MaxLength = 0;
            this.CCDBusMsgTextBox.Multiline = true;
            this.CCDBusMsgTextBox.Name = "CCDBusMsgTextBox";
            this.CCDBusMsgTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CCDBusMsgTextBox.Size = new System.Drawing.Size(214, 234);
            this.CCDBusMsgTextBox.TabIndex = 66;
            this.CCDBusMsgTextBox.TextChanged += new System.EventHandler(this.CCDBusMsgTextBox_TextChanged);
            // 
            // CCDBusMsgFilterClearButton
            // 
            this.CCDBusMsgFilterClearButton.Enabled = false;
            this.CCDBusMsgFilterClearButton.Location = new System.Drawing.Point(72, 402);
            this.CCDBusMsgFilterClearButton.Name = "CCDBusMsgFilterClearButton";
            this.CCDBusMsgFilterClearButton.Size = new System.Drawing.Size(60, 23);
            this.CCDBusMsgFilterClearButton.TabIndex = 68;
            this.CCDBusMsgFilterClearButton.Text = "Clear";
            this.CCDBusMsgFilterClearButton.UseVisualStyleBackColor = true;
            this.CCDBusMsgFilterClearButton.Click += new System.EventHandler(this.CCDBusMsgFilterClearButton_Click);
            // 
            // CCDBusMsgFilterApplyButton
            // 
            this.CCDBusMsgFilterApplyButton.Enabled = false;
            this.CCDBusMsgFilterApplyButton.Location = new System.Drawing.Point(6, 402);
            this.CCDBusMsgFilterApplyButton.Name = "CCDBusMsgFilterApplyButton";
            this.CCDBusMsgFilterApplyButton.Size = new System.Drawing.Size(60, 23);
            this.CCDBusMsgFilterApplyButton.TabIndex = 67;
            this.CCDBusMsgFilterApplyButton.Text = "Apply";
            this.CCDBusMsgFilterApplyButton.UseVisualStyleBackColor = true;
            this.CCDBusMsgFilterApplyButton.Click += new System.EventHandler(this.CCDBusMsgFilterApplyButton_Click);
            // 
            // CCDBusMsgFilterTextBox
            // 
            this.CCDBusMsgFilterTextBox.Enabled = false;
            this.CCDBusMsgFilterTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CCDBusMsgFilterTextBox.Location = new System.Drawing.Point(3, 338);
            this.CCDBusMsgFilterTextBox.MaxLength = 0;
            this.CCDBusMsgFilterTextBox.Multiline = true;
            this.CCDBusMsgFilterTextBox.Name = "CCDBusMsgFilterTextBox";
            this.CCDBusMsgFilterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CCDBusMsgFilterTextBox.Size = new System.Drawing.Size(213, 59);
            this.CCDBusMsgFilterTextBox.TabIndex = 66;
            this.CCDBusMsgFilterTextBox.TextChanged += new System.EventHandler(this.CCDMessageFilteringTextBox_TextChanged);
            // 
            // CCDBusMsgFilterCheckbox
            // 
            this.CCDBusMsgFilterCheckbox.AutoSize = true;
            this.CCDBusMsgFilterCheckbox.Location = new System.Drawing.Point(6, 311);
            this.CCDBusMsgFilterCheckbox.Name = "CCDBusMsgFilterCheckbox";
            this.CCDBusMsgFilterCheckbox.Size = new System.Drawing.Size(135, 17);
            this.CCDBusMsgFilterCheckbox.TabIndex = 54;
            this.CCDBusMsgFilterCheckbox.Text = "CCD-bus message filter";
            this.CCDBusMsgFilterCheckbox.UseVisualStyleBackColor = true;
            this.CCDBusMsgFilterCheckbox.CheckedChanged += new System.EventHandler(this.CCDBusMsgFilterCheckbox_CheckedChanged);
            // 
            // CCDBusSendMsgTextBox
            // 
            this.CCDBusSendMsgTextBox.Enabled = false;
            this.CCDBusSendMsgTextBox.Font = new System.Drawing.Font("Courier New", 9F);
            this.CCDBusSendMsgTextBox.Location = new System.Drawing.Point(3, 256);
            this.CCDBusSendMsgTextBox.Name = "CCDBusSendMsgTextBox";
            this.CCDBusSendMsgTextBox.Size = new System.Drawing.Size(143, 21);
            this.CCDBusSendMsgTextBox.TabIndex = 53;
            this.CCDBusSendMsgTextBox.TextChanged += new System.EventHandler(this.CCDBusSendMsgTextBox_TextChanged);
            this.CCDBusSendMsgTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CCDBusSendMsgTextBox_KeyPress);
            // 
            // CCDBusSendMsgButton
            // 
            this.CCDBusSendMsgButton.Enabled = false;
            this.CCDBusSendMsgButton.Location = new System.Drawing.Point(153, 254);
            this.CCDBusSendMsgButton.Name = "CCDBusSendMsgButton";
            this.CCDBusSendMsgButton.Size = new System.Drawing.Size(60, 23);
            this.CCDBusSendMsgButton.TabIndex = 52;
            this.CCDBusSendMsgButton.Text = "Send";
            this.CCDBusSendMsgButton.UseVisualStyleBackColor = true;
            this.CCDBusSendMsgButton.Click += new System.EventHandler(this.CCDBusSendMsgButton_Click);
            // 
            // CCDBusEnabledCheckbox
            // 
            this.CCDBusEnabledCheckbox.AutoSize = true;
            this.CCDBusEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.CCDBusEnabledCheckbox.Name = "CCDBusEnabledCheckbox";
            this.CCDBusEnabledCheckbox.Size = new System.Drawing.Size(68, 17);
            this.CCDBusEnabledCheckbox.TabIndex = 47;
            this.CCDBusEnabledCheckbox.Text = "CCD-bus";
            this.CCDBusEnabledCheckbox.UseVisualStyleBackColor = true;
            this.CCDBusEnabledCheckbox.CheckedChanged += new System.EventHandler(this.CCDBusEnabledCheckbox_CheckedChanged);
            // 
            // CCDBusClearMsgButton
            // 
            this.CCDBusClearMsgButton.Enabled = false;
            this.CCDBusClearMsgButton.Location = new System.Drawing.Point(153, 283);
            this.CCDBusClearMsgButton.Name = "CCDBusClearMsgButton";
            this.CCDBusClearMsgButton.Size = new System.Drawing.Size(60, 23);
            this.CCDBusClearMsgButton.TabIndex = 48;
            this.CCDBusClearMsgButton.Text = "Clear";
            this.CCDBusClearMsgButton.UseVisualStyleBackColor = true;
            this.CCDBusClearMsgButton.Click += new System.EventHandler(this.CCDBusLogClearButton_Click);
            // 
            // SCIBusMessagesGroupbox
            // 
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusHSLSButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusSpeedLabel);
            this.SCIBusMessagesGroupbox.Controls.Add(this.PCMTCMSelectorComboBox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgTextBox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterClearButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterApplyButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusEnabledCheckbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterTextBox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterCheckbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusClearMsgButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusSendMsgTextBox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusSendMsgButton);
            this.SCIBusMessagesGroupbox.Location = new System.Drawing.Point(609, 27);
            this.SCIBusMessagesGroupbox.Name = "SCIBusMessagesGroupbox";
            this.SCIBusMessagesGroupbox.Size = new System.Drawing.Size(220, 431);
            this.SCIBusMessagesGroupbox.TabIndex = 44;
            this.SCIBusMessagesGroupbox.TabStop = false;
            this.SCIBusMessagesGroupbox.Text = "SCI-bus messages";
            // 
            // SCIBusHSLSButton
            // 
            this.SCIBusHSLSButton.Enabled = false;
            this.SCIBusHSLSButton.Location = new System.Drawing.Point(153, 402);
            this.SCIBusHSLSButton.Name = "SCIBusHSLSButton";
            this.SCIBusHSLSButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusHSLSButton.TabIndex = 73;
            this.SCIBusHSLSButton.Text = "HS/LS";
            this.SCIBusHSLSButton.UseVisualStyleBackColor = true;
            this.SCIBusHSLSButton.Click += new System.EventHandler(this.SCIBusHSLSButton_Click);
            // 
            // SCIBusSpeedLabel
            // 
            this.SCIBusSpeedLabel.AutoSize = true;
            this.SCIBusSpeedLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SCIBusSpeedLabel.Location = new System.Drawing.Point(148, 312);
            this.SCIBusSpeedLabel.Name = "SCIBusSpeedLabel";
            this.SCIBusSpeedLabel.Size = new System.Drawing.Size(66, 13);
            this.SCIBusSpeedLabel.TabIndex = 66;
            this.SCIBusSpeedLabel.Text = "7812.5 kbps";
            // 
            // PCMTCMSelectorComboBox
            // 
            this.PCMTCMSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PCMTCMSelectorComboBox.FormattingEnabled = true;
            this.PCMTCMSelectorComboBox.Items.AddRange(new object[] {
            "NON",
            "PCM",
            "TCM"});
            this.PCMTCMSelectorComboBox.Location = new System.Drawing.Point(72, 285);
            this.PCMTCMSelectorComboBox.Name = "PCMTCMSelectorComboBox";
            this.PCMTCMSelectorComboBox.Size = new System.Drawing.Size(48, 21);
            this.PCMTCMSelectorComboBox.TabIndex = 55;
            this.PCMTCMSelectorComboBox.SelectionChangeCommitted += new System.EventHandler(this.PCMTCMSelectorComboBox_SelectionChangeCommitted);
            // 
            // SCIBusMsgTextBox
            // 
            this.SCIBusMsgTextBox.Enabled = false;
            this.SCIBusMsgTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SCIBusMsgTextBox.Location = new System.Drawing.Point(3, 16);
            this.SCIBusMsgTextBox.MaxLength = 0;
            this.SCIBusMsgTextBox.Multiline = true;
            this.SCIBusMsgTextBox.Name = "SCIBusMsgTextBox";
            this.SCIBusMsgTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SCIBusMsgTextBox.Size = new System.Drawing.Size(214, 234);
            this.SCIBusMsgTextBox.TabIndex = 69;
            this.SCIBusMsgTextBox.TextChanged += new System.EventHandler(this.SCIBusMsgTextBox_TextChanged);
            // 
            // SCIBusMsgFilterClearButton
            // 
            this.SCIBusMsgFilterClearButton.Enabled = false;
            this.SCIBusMsgFilterClearButton.Location = new System.Drawing.Point(72, 402);
            this.SCIBusMsgFilterClearButton.Name = "SCIBusMsgFilterClearButton";
            this.SCIBusMsgFilterClearButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusMsgFilterClearButton.TabIndex = 72;
            this.SCIBusMsgFilterClearButton.Text = "Clear";
            this.SCIBusMsgFilterClearButton.UseVisualStyleBackColor = true;
            this.SCIBusMsgFilterClearButton.Click += new System.EventHandler(this.SCIBusMsgFilterClearButton_Click);
            // 
            // SCIBusMsgFilterApplyButton
            // 
            this.SCIBusMsgFilterApplyButton.Enabled = false;
            this.SCIBusMsgFilterApplyButton.Location = new System.Drawing.Point(6, 402);
            this.SCIBusMsgFilterApplyButton.Name = "SCIBusMsgFilterApplyButton";
            this.SCIBusMsgFilterApplyButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusMsgFilterApplyButton.TabIndex = 71;
            this.SCIBusMsgFilterApplyButton.Text = "Apply";
            this.SCIBusMsgFilterApplyButton.UseVisualStyleBackColor = true;
            this.SCIBusMsgFilterApplyButton.Click += new System.EventHandler(this.SCIBusMsgFilterApplyButton_Click);
            // 
            // SCIBusEnabledCheckbox
            // 
            this.SCIBusEnabledCheckbox.AutoSize = true;
            this.SCIBusEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.SCIBusEnabledCheckbox.Name = "SCIBusEnabledCheckbox";
            this.SCIBusEnabledCheckbox.Size = new System.Drawing.Size(66, 17);
            this.SCIBusEnabledCheckbox.TabIndex = 49;
            this.SCIBusEnabledCheckbox.Text = "SCI-bus:";
            this.SCIBusEnabledCheckbox.UseVisualStyleBackColor = true;
            this.SCIBusEnabledCheckbox.CheckedChanged += new System.EventHandler(this.SCIBusEnabledCheckbox_CheckedChanged);
            // 
            // SCIBusMsgFilterTextBox
            // 
            this.SCIBusMsgFilterTextBox.Enabled = false;
            this.SCIBusMsgFilterTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SCIBusMsgFilterTextBox.Location = new System.Drawing.Point(3, 338);
            this.SCIBusMsgFilterTextBox.MaxLength = 0;
            this.SCIBusMsgFilterTextBox.Multiline = true;
            this.SCIBusMsgFilterTextBox.Name = "SCIBusMsgFilterTextBox";
            this.SCIBusMsgFilterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SCIBusMsgFilterTextBox.Size = new System.Drawing.Size(213, 59);
            this.SCIBusMsgFilterTextBox.TabIndex = 70;
            this.SCIBusMsgFilterTextBox.TextChanged += new System.EventHandler(this.SCIMessageFilteringTextBox_TextChanged);
            // 
            // SCIBusMsgFilterCheckbox
            // 
            this.SCIBusMsgFilterCheckbox.AutoSize = true;
            this.SCIBusMsgFilterCheckbox.Location = new System.Drawing.Point(6, 311);
            this.SCIBusMsgFilterCheckbox.Name = "SCIBusMsgFilterCheckbox";
            this.SCIBusMsgFilterCheckbox.Size = new System.Drawing.Size(130, 17);
            this.SCIBusMsgFilterCheckbox.TabIndex = 69;
            this.SCIBusMsgFilterCheckbox.Text = "SCI-bus message filter";
            this.SCIBusMsgFilterCheckbox.UseVisualStyleBackColor = true;
            this.SCIBusMsgFilterCheckbox.CheckedChanged += new System.EventHandler(this.SCIBusMsgFilterCheckbox_CheckedChanged);
            // 
            // SCIBusClearMsgButton
            // 
            this.SCIBusClearMsgButton.Enabled = false;
            this.SCIBusClearMsgButton.Location = new System.Drawing.Point(153, 283);
            this.SCIBusClearMsgButton.Name = "SCIBusClearMsgButton";
            this.SCIBusClearMsgButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusClearMsgButton.TabIndex = 51;
            this.SCIBusClearMsgButton.Text = "Clear";
            this.SCIBusClearMsgButton.UseVisualStyleBackColor = true;
            this.SCIBusClearMsgButton.Click += new System.EventHandler(this.SCIBusLogClearButton_Click);
            // 
            // command_history_groupbox
            // 
            this.command_history_groupbox.Controls.Add(this.CommandHistoryTextBox);
            this.command_history_groupbox.Location = new System.Drawing.Point(12, 348);
            this.command_history_groupbox.Name = "command_history_groupbox";
            this.command_history_groupbox.Size = new System.Drawing.Size(365, 110);
            this.command_history_groupbox.TabIndex = 45;
            this.command_history_groupbox.TabStop = false;
            this.command_history_groupbox.Text = "Command history";
            // 
            // CommandHistoryTextBox
            // 
            this.CommandHistoryTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CommandHistoryTextBox.Location = new System.Drawing.Point(3, 17);
            this.CommandHistoryTextBox.MaxLength = 0;
            this.CommandHistoryTextBox.Multiline = true;
            this.CommandHistoryTextBox.Name = "CommandHistoryTextBox";
            this.CommandHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CommandHistoryTextBox.Size = new System.Drawing.Size(358, 87);
            this.CommandHistoryTextBox.TabIndex = 69;
            // 
            // MiscGroupbox
            // 
            this.MiscGroupbox.Controls.Add(this.CounterLabel);
            this.MiscGroupbox.Controls.Add(this.PacketCountTxLabel);
            this.MiscGroupbox.Controls.Add(this.RebootScannerButton);
            this.MiscGroupbox.Controls.Add(this.PacketCountRxLabel);
            this.MiscGroupbox.Controls.Add(this.BufferStartLabel);
            this.MiscGroupbox.Controls.Add(this.BufferEndLabel);
            this.MiscGroupbox.Controls.Add(this.BufferReadlengthLabel);
            this.MiscGroupbox.Controls.Add(this.BufferWritelengthLabel);
            this.MiscGroupbox.Location = new System.Drawing.Point(162, 464);
            this.MiscGroupbox.Name = "MiscGroupbox";
            this.MiscGroupbox.Size = new System.Drawing.Size(215, 136);
            this.MiscGroupbox.TabIndex = 46;
            this.MiscGroupbox.TabStop = false;
            this.MiscGroupbox.Text = "Misc.";
            // 
            // CounterLabel
            // 
            this.CounterLabel.AutoSize = true;
            this.CounterLabel.Location = new System.Drawing.Point(181, 117);
            this.CounterLabel.Name = "CounterLabel";
            this.CounterLabel.Size = new System.Drawing.Size(13, 13);
            this.CounterLabel.TabIndex = 25;
            this.CounterLabel.Text = "0";
            // 
            // PacketCountTxLabel
            // 
            this.PacketCountTxLabel.AutoSize = true;
            this.PacketCountTxLabel.Location = new System.Drawing.Point(6, 35);
            this.PacketCountTxLabel.Name = "PacketCountTxLabel";
            this.PacketCountTxLabel.Size = new System.Drawing.Size(75, 13);
            this.PacketCountTxLabel.TabIndex = 24;
            this.PacketCountTxLabel.Text = "Packets sent: ";
            // 
            // RebootScannerButton
            // 
            this.RebootScannerButton.Location = new System.Drawing.Point(6, 106);
            this.RebootScannerButton.Name = "RebootScannerButton";
            this.RebootScannerButton.Size = new System.Drawing.Size(107, 23);
            this.RebootScannerButton.TabIndex = 23;
            this.RebootScannerButton.Text = "Reboot scanner";
            this.RebootScannerButton.UseVisualStyleBackColor = true;
            this.RebootScannerButton.Click += new System.EventHandler(this.RebootScannerButton_Click);
            // 
            // SCIHSModeScanButton
            // 
            this.SCIHSModeScanButton.Location = new System.Drawing.Point(5, 57);
            this.SCIHSModeScanButton.Name = "SCIHSModeScanButton";
            this.SCIHSModeScanButton.Size = new System.Drawing.Size(63, 23);
            this.SCIHSModeScanButton.TabIndex = 76;
            this.SCIHSModeScanButton.Text = "Scan";
            this.SCIHSModeScanButton.UseVisualStyleBackColor = true;
            this.SCIHSModeScanButton.Click += new System.EventHandler(this.SCIHSModeScanButton_Click);
            // 
            // SCIHSModeLabel
            // 
            this.SCIHSModeLabel.AutoSize = true;
            this.SCIHSModeLabel.Location = new System.Drawing.Point(3, 16);
            this.SCIHSModeLabel.Name = "SCIHSModeLabel";
            this.SCIHSModeLabel.Size = new System.Drawing.Size(37, 13);
            this.SCIHSModeLabel.TabIndex = 75;
            this.SCIHSModeLabel.Text = "Mode:";
            // 
            // SCIHSModeComboBox
            // 
            this.SCIHSModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SCIHSModeComboBox.FormattingEnabled = true;
            this.SCIHSModeComboBox.Items.AddRange(new object[] {
            "F0",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "FA",
            "FB",
            "FC",
            "FD",
            "FE",
            "FF"});
            this.SCIHSModeComboBox.Location = new System.Drawing.Point(6, 30);
            this.SCIHSModeComboBox.Name = "SCIHSModeComboBox";
            this.SCIHSModeComboBox.Size = new System.Drawing.Size(61, 21);
            this.SCIHSModeComboBox.TabIndex = 74;
            this.SCIHSModeComboBox.SelectionChangeCommitted += new System.EventHandler(this.SCIHSModeComboBox_SelectionChangeCommitted);
            // 
            // ControlPanelGroupbox
            // 
            this.ControlPanelGroupbox.Controls.Add(this.ExitButton);
            this.ControlPanelGroupbox.Controls.Add(this.DisconnectButton);
            this.ControlPanelGroupbox.Controls.Add(this.ConnectButton);
            this.ControlPanelGroupbox.Controls.Add(this.StatusButton);
            this.ControlPanelGroupbox.Location = new System.Drawing.Point(12, 464);
            this.ControlPanelGroupbox.Name = "ControlPanelGroupbox";
            this.ControlPanelGroupbox.Size = new System.Drawing.Size(144, 136);
            this.ControlPanelGroupbox.TabIndex = 47;
            this.ControlPanelGroupbox.TabStop = false;
            this.ControlPanelGroupbox.Text = "Control panel";
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(6, 106);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(132, 23);
            this.ExitButton.TabIndex = 22;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Enabled = false;
            this.DisconnectButton.Location = new System.Drawing.Point(6, 48);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(132, 23);
            this.DisconnectButton.TabIndex = 2;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // EEPROMGroupbox
            // 
            this.EEPROMGroupbox.Controls.Add(this.write_exteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.save_exteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.label17);
            this.EEPROMGroupbox.Controls.Add(this.write_inteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.erase_inteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.save_inteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.label16);
            this.EEPROMGroupbox.Controls.Add(this.read_inteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.read_exteeprom_button);
            this.EEPROMGroupbox.Controls.Add(this.erase_exteeprom_button);
            this.EEPROMGroupbox.Location = new System.Drawing.Point(12, 606);
            this.EEPROMGroupbox.Name = "EEPROMGroupbox";
            this.EEPROMGroupbox.Size = new System.Drawing.Size(365, 82);
            this.EEPROMGroupbox.TabIndex = 48;
            this.EEPROMGroupbox.TabStop = false;
            this.EEPROMGroupbox.Text = "EEPROM (scanner)";
            // 
            // write_exteeprom_button
            // 
            this.write_exteeprom_button.Enabled = false;
            this.write_exteeprom_button.Location = new System.Drawing.Point(169, 47);
            this.write_exteeprom_button.Name = "write_exteeprom_button";
            this.write_exteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.write_exteeprom_button.TabIndex = 55;
            this.write_exteeprom_button.Text = "Write";
            this.write_exteeprom_button.UseVisualStyleBackColor = true;
            // 
            // save_exteeprom_button
            // 
            this.save_exteeprom_button.Enabled = false;
            this.save_exteeprom_button.Location = new System.Drawing.Point(235, 47);
            this.save_exteeprom_button.Name = "save_exteeprom_button";
            this.save_exteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.save_exteeprom_button.TabIndex = 54;
            this.save_exteeprom_button.Text = "Save";
            this.save_exteeprom_button.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 52);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(97, 13);
            this.label17.TabIndex = 53;
            this.label17.Text = "External EEPROM:";
            // 
            // write_inteeprom_button
            // 
            this.write_inteeprom_button.Enabled = false;
            this.write_inteeprom_button.Location = new System.Drawing.Point(169, 18);
            this.write_inteeprom_button.Name = "write_inteeprom_button";
            this.write_inteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.write_inteeprom_button.TabIndex = 52;
            this.write_inteeprom_button.Text = "Write";
            this.write_inteeprom_button.UseVisualStyleBackColor = true;
            // 
            // erase_inteeprom_button
            // 
            this.erase_inteeprom_button.Enabled = false;
            this.erase_inteeprom_button.Location = new System.Drawing.Point(301, 18);
            this.erase_inteeprom_button.Name = "erase_inteeprom_button";
            this.erase_inteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.erase_inteeprom_button.TabIndex = 51;
            this.erase_inteeprom_button.Text = "Erase";
            this.erase_inteeprom_button.UseVisualStyleBackColor = true;
            // 
            // save_inteeprom_button
            // 
            this.save_inteeprom_button.Enabled = false;
            this.save_inteeprom_button.Location = new System.Drawing.Point(235, 18);
            this.save_inteeprom_button.Name = "save_inteeprom_button";
            this.save_inteeprom_button.Size = new System.Drawing.Size(60, 23);
            this.save_inteeprom_button.TabIndex = 50;
            this.save_inteeprom_button.Text = "Save";
            this.save_inteeprom_button.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 49;
            this.label16.Text = "Internal EEPROM:";
            // 
            // RealTimeDiagnosticsGroupbox
            // 
            this.RealTimeDiagnosticsGroupbox.Controls.Add(this.real_time_diagnostics_button);
            this.RealTimeDiagnosticsGroupbox.Location = new System.Drawing.Point(383, 535);
            this.RealTimeDiagnosticsGroupbox.Name = "RealTimeDiagnosticsGroupbox";
            this.RealTimeDiagnosticsGroupbox.Size = new System.Drawing.Size(141, 65);
            this.RealTimeDiagnosticsGroupbox.TabIndex = 50;
            this.RealTimeDiagnosticsGroupbox.TabStop = false;
            this.RealTimeDiagnosticsGroupbox.Text = "Real Time Diagnostics";
            // 
            // real_time_diagnostics_button
            // 
            this.real_time_diagnostics_button.Location = new System.Drawing.Point(6, 19);
            this.real_time_diagnostics_button.Name = "real_time_diagnostics_button";
            this.real_time_diagnostics_button.Size = new System.Drawing.Size(126, 40);
            this.real_time_diagnostics_button.TabIndex = 51;
            this.real_time_diagnostics_button.Text = "Open in new window";
            this.real_time_diagnostics_button.UseVisualStyleBackColor = true;
            this.real_time_diagnostics_button.Click += new System.EventHandler(this.RealTimeDiagnosticsButton_Click);
            // 
            // ReadDTCGroupbox
            // 
            this.ReadDTCGroupbox.Controls.Add(this.ScanModulesButton);
            this.ReadDTCGroupbox.Controls.Add(this.ModuleListComboBox);
            this.ReadDTCGroupbox.Controls.Add(this.ReadDTCByModuleButton);
            this.ReadDTCGroupbox.Location = new System.Drawing.Point(383, 606);
            this.ReadDTCGroupbox.Name = "ReadDTCGroupbox";
            this.ReadDTCGroupbox.Size = new System.Drawing.Size(220, 82);
            this.ReadDTCGroupbox.TabIndex = 51;
            this.ReadDTCGroupbox.TabStop = false;
            this.ReadDTCGroupbox.Text = "Read DTC by modules";
            // 
            // ScanModulesButton
            // 
            this.ScanModulesButton.Location = new System.Drawing.Point(112, 46);
            this.ScanModulesButton.Name = "ScanModulesButton";
            this.ScanModulesButton.Size = new System.Drawing.Size(101, 23);
            this.ScanModulesButton.TabIndex = 54;
            this.ScanModulesButton.Text = "Scan";
            this.ScanModulesButton.UseVisualStyleBackColor = true;
            this.ScanModulesButton.Click += new System.EventHandler(this.ScanModulesButton_Click);
            // 
            // ModuleListComboBox
            // 
            this.ModuleListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModuleListComboBox.FormattingEnabled = true;
            this.ModuleListComboBox.Items.AddRange(new object[] {
            "Scanning needed"});
            this.ModuleListComboBox.Location = new System.Drawing.Point(6, 18);
            this.ModuleListComboBox.Name = "ModuleListComboBox";
            this.ModuleListComboBox.Size = new System.Drawing.Size(207, 21);
            this.ModuleListComboBox.TabIndex = 53;
            // 
            // ReadDTCByModuleButton
            // 
            this.ReadDTCByModuleButton.Enabled = false;
            this.ReadDTCByModuleButton.Location = new System.Drawing.Point(6, 46);
            this.ReadDTCByModuleButton.Name = "ReadDTCByModuleButton";
            this.ReadDTCByModuleButton.Size = new System.Drawing.Size(101, 23);
            this.ReadDTCByModuleButton.TabIndex = 52;
            this.ReadDTCByModuleButton.Text = "Read DTC";
            this.ReadDTCByModuleButton.UseVisualStyleBackColor = true;
            // 
            // SensorDataGroupbox
            // 
            this.SensorDataGroupbox.Controls.Add(this.SensorDataTextbox);
            this.SensorDataGroupbox.Location = new System.Drawing.Point(835, 27);
            this.SensorDataGroupbox.Name = "SensorDataGroupbox";
            this.SensorDataGroupbox.Size = new System.Drawing.Size(377, 661);
            this.SensorDataGroupbox.TabIndex = 52;
            this.SensorDataGroupbox.TabStop = false;
            this.SensorDataGroupbox.Text = "Sensor data";
            // 
            // SensorDataTextbox
            // 
            this.SensorDataTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SensorDataTextbox.Location = new System.Drawing.Point(3, 16);
            this.SensorDataTextbox.MaxLength = 0;
            this.SensorDataTextbox.Multiline = true;
            this.SensorDataTextbox.Name = "SensorDataTextbox";
            this.SensorDataTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SensorDataTextbox.Size = new System.Drawing.Size(371, 639);
            this.SensorDataTextbox.TabIndex = 73;
            // 
            // DTCListGroupbox
            // 
            this.DTCListGroupbox.Controls.Add(this.DTCListTextbox);
            this.DTCListGroupbox.Location = new System.Drawing.Point(609, 464);
            this.DTCListGroupbox.Name = "DTCListGroupbox";
            this.DTCListGroupbox.Size = new System.Drawing.Size(220, 224);
            this.DTCListGroupbox.TabIndex = 52;
            this.DTCListGroupbox.TabStop = false;
            this.DTCListGroupbox.Text = "Diagnostic Trouble Code (DTC) list";
            // 
            // DTCListTextbox
            // 
            this.DTCListTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DTCListTextbox.Location = new System.Drawing.Point(3, 16);
            this.DTCListTextbox.MaxLength = 0;
            this.DTCListTextbox.Multiline = true;
            this.DTCListTextbox.Name = "DTCListTextbox";
            this.DTCListTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DTCListTextbox.Size = new System.Drawing.Size(214, 202);
            this.DTCListTextbox.TabIndex = 70;
            // 
            // DRBDBExplorerButton
            // 
            this.DRBDBExplorerButton.Location = new System.Drawing.Point(6, 19);
            this.DRBDBExplorerButton.Name = "DRBDBExplorerButton";
            this.DRBDBExplorerButton.Size = new System.Drawing.Size(126, 40);
            this.DRBDBExplorerButton.TabIndex = 77;
            this.DRBDBExplorerButton.Text = "DRB-III Database Explorer";
            this.DRBDBExplorerButton.UseVisualStyleBackColor = true;
            this.DRBDBExplorerButton.Click += new System.EventHandler(this.DRBDBExplorerButton_Click);
            // 
            // DRB3ToolsGroupbox
            // 
            this.DRB3ToolsGroupbox.Controls.Add(this.DRBDBExplorerButton);
            this.DRB3ToolsGroupbox.Location = new System.Drawing.Point(383, 464);
            this.DRB3ToolsGroupbox.Name = "DRB3ToolsGroupbox";
            this.DRB3ToolsGroupbox.Size = new System.Drawing.Size(141, 65);
            this.DRB3ToolsGroupbox.TabIndex = 52;
            this.DRB3ToolsGroupbox.TabStop = false;
            this.DRB3ToolsGroupbox.Text = "DRB-III Tools";
            // 
            // PCMRAMGroupbox
            // 
            this.PCMRAMGroupbox.Controls.Add(this.label1);
            this.PCMRAMGroupbox.Controls.Add(this.RepeatTextbox);
            this.PCMRAMGroupbox.Controls.Add(this.PCMRAMRepeatCheckbox);
            this.PCMRAMGroupbox.Controls.Add(this.SCIHSModeLabel);
            this.PCMRAMGroupbox.Controls.Add(this.SCIHSModeScanButton);
            this.PCMRAMGroupbox.Controls.Add(this.SCIHSModeComboBox);
            this.PCMRAMGroupbox.Location = new System.Drawing.Point(530, 464);
            this.PCMRAMGroupbox.Name = "PCMRAMGroupbox";
            this.PCMRAMGroupbox.Size = new System.Drawing.Size(73, 136);
            this.PCMRAMGroupbox.TabIndex = 52;
            this.PCMRAMGroupbox.TabStop = false;
            this.PCMRAMGroupbox.Text = "PCM RAM";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "s";
            // 
            // RepeatTextbox
            // 
            this.RepeatTextbox.Enabled = false;
            this.RepeatTextbox.Location = new System.Drawing.Point(5, 106);
            this.RepeatTextbox.Name = "RepeatTextbox";
            this.RepeatTextbox.Size = new System.Drawing.Size(46, 20);
            this.RepeatTextbox.TabIndex = 78;
            this.RepeatTextbox.Text = "0,1";
            // 
            // PCMRAMRepeatCheckbox
            // 
            this.PCMRAMRepeatCheckbox.AutoSize = true;
            this.PCMRAMRepeatCheckbox.Location = new System.Drawing.Point(6, 86);
            this.PCMRAMRepeatCheckbox.Name = "PCMRAMRepeatCheckbox";
            this.PCMRAMRepeatCheckbox.Size = new System.Drawing.Size(61, 17);
            this.PCMRAMRepeatCheckbox.TabIndex = 77;
            this.PCMRAMRepeatCheckbox.Text = "Repeat";
            this.PCMRAMRepeatCheckbox.UseVisualStyleBackColor = true;
            this.PCMRAMRepeatCheckbox.CheckedChanged += new System.EventHandler(this.PCMRAMRepeatCheckbox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 692);
            this.Controls.Add(this.PCMRAMGroupbox);
            this.Controls.Add(this.DRB3ToolsGroupbox);
            this.Controls.Add(this.DTCListGroupbox);
            this.Controls.Add(this.SensorDataGroupbox);
            this.Controls.Add(this.ReadDTCGroupbox);
            this.Controls.Add(this.RealTimeDiagnosticsGroupbox);
            this.Controls.Add(this.EEPROMGroupbox);
            this.Controls.Add(this.ControlPanelGroupbox);
            this.Controls.Add(this.MiscGroupbox);
            this.Controls.Add(this.command_history_groupbox);
            this.Controls.Add(this.SCIBusMessagesGroupbox);
            this.Controls.Add(this.CCDBusMessagesGroupbox);
            this.Controls.Add(this.PacketLogGroupbox);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chrysler CCD/SCI Scanner GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.PacketLogGroupbox.ResumeLayout(false);
            this.PacketLogGroupbox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.CCDBusMessagesGroupbox.ResumeLayout(false);
            this.CCDBusMessagesGroupbox.PerformLayout();
            this.SCIBusMessagesGroupbox.ResumeLayout(false);
            this.SCIBusMessagesGroupbox.PerformLayout();
            this.command_history_groupbox.ResumeLayout(false);
            this.command_history_groupbox.PerformLayout();
            this.MiscGroupbox.ResumeLayout(false);
            this.MiscGroupbox.PerformLayout();
            this.ControlPanelGroupbox.ResumeLayout(false);
            this.EEPROMGroupbox.ResumeLayout(false);
            this.EEPROMGroupbox.PerformLayout();
            this.RealTimeDiagnosticsGroupbox.ResumeLayout(false);
            this.ReadDTCGroupbox.ResumeLayout(false);
            this.SensorDataGroupbox.ResumeLayout(false);
            this.SensorDataGroupbox.PerformLayout();
            this.DTCListGroupbox.ResumeLayout(false);
            this.DTCListGroupbox.PerformLayout();
            this.DRB3ToolsGroupbox.ResumeLayout(false);
            this.PCMRAMGroupbox.ResumeLayout(false);
            this.PCMRAMGroupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label PacketCountRxLabel;
        private System.Windows.Forms.Label BufferWritelengthLabel;
        private System.Windows.Forms.Label BufferStartLabel;
        private System.Windows.Forms.Label BufferEndLabel;
        private System.Windows.Forms.Label BufferReadlengthLabel;
        private System.Windows.Forms.Button read_exteeprom_button;
        private System.Windows.Forms.Button read_inteeprom_button;
        private System.Windows.Forms.Button StatusButton;
        private System.Windows.Forms.TextBox SCIBusSendMsgTextBox;
        private System.Windows.Forms.Button erase_exteeprom_button;
        private System.Windows.Forms.GroupBox PacketLogGroupbox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.CheckBox PacketLogEnabledCheckbox;
        private System.Windows.Forms.GroupBox CCDBusMessagesGroupbox;
        private System.Windows.Forms.GroupBox SCIBusMessagesGroupbox;
        private System.Windows.Forms.Button PacketClearButton;
        private System.Windows.Forms.GroupBox command_history_groupbox;
        private System.Windows.Forms.CheckBox CCDBusEnabledCheckbox;
        private System.Windows.Forms.Button CCDBusClearMsgButton;
        private System.Windows.Forms.CheckBox SCIBusEnabledCheckbox;
        private System.Windows.Forms.Button SCIBusClearMsgButton;
        private System.Windows.Forms.GroupBox MiscGroupbox;
        private System.Windows.Forms.GroupBox ControlPanelGroupbox;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button SCIBusSendMsgButton;
        private System.Windows.Forms.TextBox CCDBusSendMsgTextBox;
        private System.Windows.Forms.Button CCDBusSendMsgButton;
        private System.Windows.Forms.GroupBox EEPROMGroupbox;
        private System.Windows.Forms.Button erase_inteeprom_button;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button write_inteeprom_button;
        private System.Windows.Forms.Button write_exteeprom_button;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox RealTimeDiagnosticsGroupbox;
        private System.Windows.Forms.Button real_time_diagnostics_button;
        private System.Windows.Forms.TextBox PacketSendTextBox;
        private System.Windows.Forms.Button PacketSendButton;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox ReadDTCGroupbox;
        private System.Windows.Forms.Button ReadDTCByModuleButton;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Label RXLabel;
        private System.Windows.Forms.Label TXLabel;
        private System.Windows.Forms.GroupBox SensorDataGroupbox;
        private System.Windows.Forms.Button RebootScannerButton;
        private System.Windows.Forms.TextBox PacketTextBox;
        private System.Windows.Forms.Button CCDBusMsgFilterClearButton;
        private System.Windows.Forms.Button CCDBusMsgFilterApplyButton;
        private System.Windows.Forms.TextBox CCDBusMsgFilterTextBox;
        private System.Windows.Forms.CheckBox CCDBusMsgFilterCheckbox;
        private System.Windows.Forms.Button SCIBusMsgFilterClearButton;
        private System.Windows.Forms.Button SCIBusMsgFilterApplyButton;
        private System.Windows.Forms.TextBox SCIBusMsgFilterTextBox;
        private System.Windows.Forms.CheckBox SCIBusMsgFilterCheckbox;
        private System.Windows.Forms.TextBox CommandHistoryTextBox;
        private System.Windows.Forms.Label PacketCountTxLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetGeneratorToolStripMenuItem;
        private System.Windows.Forms.TextBox CCDBusMsgTextBox;
        private System.Windows.Forms.TextBox SCIBusMsgTextBox;
        private System.Windows.Forms.TextBox SensorDataTextbox;
        private System.Windows.Forms.GroupBox DTCListGroupbox;
        private System.Windows.Forms.TextBox DTCListTextbox;
        private System.Windows.Forms.ToolStripMenuItem superCardReaderToolStripMenuItem;
        private System.Windows.Forms.ComboBox ModuleListComboBox;
        private System.Windows.Forms.Button ScanModulesButton;
        private System.Windows.Forms.ComboBox PCMTCMSelectorComboBox;
        private System.Windows.Forms.Label SCIBusSpeedLabel;
        private System.Windows.Forms.Label CCDBusSpeedLabel;
        private System.Windows.Forms.Button SCIBusHSLSButton;
        private System.Windows.Forms.ComboBox SCIHSModeComboBox;
        private System.Windows.Forms.Button SCIHSModeScanButton;
        private System.Windows.Forms.Label SCIHSModeLabel;
        private System.Windows.Forms.Button DRBDBExplorerButton;
        private System.Windows.Forms.GroupBox DRB3ToolsGroupbox;
        private System.Windows.Forms.GroupBox PCMRAMGroupbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox RepeatTextbox;
        private System.Windows.Forms.CheckBox PCMRAMRepeatCheckbox;
        private System.Windows.Forms.Button save_exteeprom_button;
        private System.Windows.Forms.Button save_inteeprom_button;
        private System.Windows.Forms.Label CounterLabel;
    }
}

