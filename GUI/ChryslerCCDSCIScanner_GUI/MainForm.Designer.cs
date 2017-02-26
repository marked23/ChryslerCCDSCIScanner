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
            this.components = new System.ComponentModel.Container();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.packet_count_rx_label = new System.Windows.Forms.Label();
            this.buffer_writelength_label = new System.Windows.Forms.Label();
            this.buffer_start_label = new System.Windows.Forms.Label();
            this.buffer_end_label = new System.Windows.Forms.Label();
            this.buffer_readlength_label = new System.Windows.Forms.Label();
            this.read_exteeprom_button = new System.Windows.Forms.Button();
            this.read_inteeprom_button = new System.Windows.Forms.Button();
            this.StatusButton = new System.Windows.Forms.Button();
            this.SCIBusSendMsgButton = new System.Windows.Forms.Button();
            this.SCIBusSendMsgTextbox = new System.Windows.Forms.TextBox();
            this.sci_bus_hs_button = new System.Windows.Forms.Button();
            this.sci_bus_speed_label = new System.Windows.Forms.Label();
            this.erase_exteeprom_button = new System.Windows.Forms.Button();
            this.PacketLogGroupbox = new System.Windows.Forms.GroupBox();
            this.PacketGeneratorButton = new System.Windows.Forms.Button();
            this.PacketTextbox = new System.Windows.Forms.TextBox();
            this.TXLabel = new System.Windows.Forms.Label();
            this.RXLabel = new System.Windows.Forms.Label();
            this.PacketSendTextbox = new System.Windows.Forms.TextBox();
            this.PacketSendButton = new System.Windows.Forms.Button();
            this.PacketClearButton = new System.Windows.Forms.Button();
            this.PacketLogEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CCDBusMessagesGroupbox = new System.Windows.Forms.GroupBox();
            this.CCDBusMsgStackingCheckbox = new System.Windows.Forms.CheckBox();
            this.CCDBusMsgTextbox = new System.Windows.Forms.TextBox();
            this.CCDBusMsgFilterClearButton = new System.Windows.Forms.Button();
            this.CCDBusMsgFilterApplyButton = new System.Windows.Forms.Button();
            this.CCDBusMsgFilterTextbox = new System.Windows.Forms.TextBox();
            this.CCDBusMsgFilterCheckbox = new System.Windows.Forms.CheckBox();
            this.CCDBusSendMsgTextbox = new System.Windows.Forms.TextBox();
            this.CCDBusSendMsgButton = new System.Windows.Forms.Button();
            this.CCDBusEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.CCDBusClearButton = new System.Windows.Forms.Button();
            this.SCIBusMessagesGroupbox = new System.Windows.Forms.GroupBox();
            this.SCIBusMsgStackingCheckbox = new System.Windows.Forms.CheckBox();
            this.SCIBusMsgTextbox = new System.Windows.Forms.TextBox();
            this.SCIBusMsgFilterClearButton = new System.Windows.Forms.Button();
            this.SCIBusMsgFilterApplyButton = new System.Windows.Forms.Button();
            this.SCIBusEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.SCIBusMsgFilterTextbox = new System.Windows.Forms.TextBox();
            this.SCIBusMsgFilterCheckbox = new System.Windows.Forms.CheckBox();
            this.SCIBusClearButton = new System.Windows.Forms.Button();
            this.command_history_groupbox = new System.Windows.Forms.GroupBox();
            this.command_history_textbox = new System.Windows.Forms.TextBox();
            this.MiscGroupbox = new System.Windows.Forms.GroupBox();
            this.packet_count_tx_label = new System.Windows.Forms.Label();
            this.RebootScannerButton = new System.Windows.Forms.Button();
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
            this.ReadDTCsGroupbox = new System.Windows.Forms.GroupBox();
            this.read_dtc_tcm_button = new System.Windows.Forms.Button();
            this.read_dtc_mic_button = new System.Windows.Forms.Button();
            this.read_dtc_abs_button = new System.Windows.Forms.Button();
            this.read_dtc_acm_button = new System.Windows.Forms.Button();
            this.read_dtc_bcm_button = new System.Windows.Forms.Button();
            this.read_dtc_pcm_button = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SCIBusSpeedGroupbox = new System.Windows.Forms.GroupBox();
            this.SensorDataGroupbox = new System.Windows.Forms.GroupBox();
            this.SensorDataTextbox = new System.Windows.Forms.TextBox();
            this.ETCGroupbox = new System.Windows.Forms.GroupBox();
            this.PacketLogGroupbox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.CCDBusMessagesGroupbox.SuspendLayout();
            this.SCIBusMessagesGroupbox.SuspendLayout();
            this.command_history_groupbox.SuspendLayout();
            this.MiscGroupbox.SuspendLayout();
            this.ControlPanelGroupbox.SuspendLayout();
            this.EEPROMGroupbox.SuspendLayout();
            this.RealTimeDiagnosticsGroupbox.SuspendLayout();
            this.ReadDTCsGroupbox.SuspendLayout();
            this.SCIBusSpeedGroupbox.SuspendLayout();
            this.SensorDataGroupbox.SuspendLayout();
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
            // packet_count_rx_label
            // 
            this.packet_count_rx_label.AutoSize = true;
            this.packet_count_rx_label.Location = new System.Drawing.Point(6, 22);
            this.packet_count_rx_label.Name = "packet_count_rx_label";
            this.packet_count_rx_label.Size = new System.Drawing.Size(96, 13);
            this.packet_count_rx_label.TabIndex = 2;
            this.packet_count_rx_label.Text = "Packets received: ";
            // 
            // buffer_writelength_label
            // 
            this.buffer_writelength_label.AutoSize = true;
            this.buffer_writelength_label.Location = new System.Drawing.Point(6, 87);
            this.buffer_writelength_label.Name = "buffer_writelength_label";
            this.buffer_writelength_label.Size = new System.Drawing.Size(99, 13);
            this.buffer_writelength_label.TabIndex = 3;
            this.buffer_writelength_label.Text = "Buffer WriteLength:";
            // 
            // buffer_start_label
            // 
            this.buffer_start_label.AutoSize = true;
            this.buffer_start_label.Location = new System.Drawing.Point(6, 48);
            this.buffer_start_label.Name = "buffer_start_label";
            this.buffer_start_label.Size = new System.Drawing.Size(63, 13);
            this.buffer_start_label.TabIndex = 6;
            this.buffer_start_label.Text = "Buffer Start:";
            // 
            // buffer_end_label
            // 
            this.buffer_end_label.AutoSize = true;
            this.buffer_end_label.Location = new System.Drawing.Point(6, 61);
            this.buffer_end_label.Name = "buffer_end_label";
            this.buffer_end_label.Size = new System.Drawing.Size(60, 13);
            this.buffer_end_label.TabIndex = 7;
            this.buffer_end_label.Text = "Buffer End:";
            // 
            // buffer_readlength_label
            // 
            this.buffer_readlength_label.AutoSize = true;
            this.buffer_readlength_label.Location = new System.Drawing.Point(6, 74);
            this.buffer_readlength_label.Name = "buffer_readlength_label";
            this.buffer_readlength_label.Size = new System.Drawing.Size(100, 13);
            this.buffer_readlength_label.TabIndex = 8;
            this.buffer_readlength_label.Text = "Buffer ReadLength:";
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
            this.read_exteeprom_button.Click += new System.EventHandler(this.read_exteeprom_button_Click);
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
            this.read_inteeprom_button.Click += new System.EventHandler(this.read_inteeprom_button_Click);
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
            this.SCIBusSendMsgButton.Click += new System.EventHandler(this.send_sci_msg_button_Click);
            // 
            // SCIBusSendMsgTextbox
            // 
            this.SCIBusSendMsgTextbox.Enabled = false;
            this.SCIBusSendMsgTextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.SCIBusSendMsgTextbox.Location = new System.Drawing.Point(3, 256);
            this.SCIBusSendMsgTextbox.Name = "SCIBusSendMsgTextbox";
            this.SCIBusSendMsgTextbox.Size = new System.Drawing.Size(143, 21);
            this.SCIBusSendMsgTextbox.TabIndex = 27;
            this.SCIBusSendMsgTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sci_bus_send_msg_textbox_KeyPress);
            // 
            // sci_bus_hs_button
            // 
            this.sci_bus_hs_button.Dock = System.Windows.Forms.DockStyle.Left;
            this.sci_bus_hs_button.Location = new System.Drawing.Point(3, 16);
            this.sci_bus_hs_button.Name = "sci_bus_hs_button";
            this.sci_bus_hs_button.Size = new System.Drawing.Size(48, 23);
            this.sci_bus_hs_button.TabIndex = 28;
            this.sci_bus_hs_button.Text = "Switch";
            this.sci_bus_hs_button.UseVisualStyleBackColor = true;
            this.sci_bus_hs_button.Click += new System.EventHandler(this.sci_bus_hs_button_Click);
            // 
            // sci_bus_speed_label
            // 
            this.sci_bus_speed_label.AutoSize = true;
            this.sci_bus_speed_label.Location = new System.Drawing.Point(57, 21);
            this.sci_bus_speed_label.Name = "sci_bus_speed_label";
            this.sci_bus_speed_label.Size = new System.Drawing.Size(67, 13);
            this.sci_bus_speed_label.TabIndex = 29;
            this.sci_bus_speed_label.Text = "7812.5 baud";
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
            this.erase_exteeprom_button.Click += new System.EventHandler(this.erase_exteeprom_button_Click);
            // 
            // PacketLogGroupbox
            // 
            this.PacketLogGroupbox.Controls.Add(this.PacketTextbox);
            this.PacketLogGroupbox.Controls.Add(this.TXLabel);
            this.PacketLogGroupbox.Controls.Add(this.RXLabel);
            this.PacketLogGroupbox.Controls.Add(this.PacketSendTextbox);
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
            // PacketGeneratorButton
            // 
            this.PacketGeneratorButton.Location = new System.Drawing.Point(536, 476);
            this.PacketGeneratorButton.Name = "PacketGeneratorButton";
            this.PacketGeneratorButton.Size = new System.Drawing.Size(79, 23);
            this.PacketGeneratorButton.TabIndex = 66;
            this.PacketGeneratorButton.Text = "Generator";
            this.PacketGeneratorButton.UseVisualStyleBackColor = true;
            this.PacketGeneratorButton.Click += new System.EventHandler(this.PacketGeneratorButton_Click);
            // 
            // PacketTextbox
            // 
            this.PacketTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PacketTextbox.Location = new System.Drawing.Point(3, 16);
            this.PacketTextbox.MaxLength = 0;
            this.PacketTextbox.Multiline = true;
            this.PacketTextbox.Name = "PacketTextbox";
            this.PacketTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PacketTextbox.Size = new System.Drawing.Size(359, 234);
            this.PacketTextbox.TabIndex = 65;
            this.PacketTextbox.TextChanged += new System.EventHandler(this.PacketTextbox_TextChanged);
            // 
            // TXLabel
            // 
            this.TXLabel.AutoSize = true;
            this.TXLabel.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.TXLabel.Location = new System.Drawing.Point(127, 294);
            this.TXLabel.Name = "TXLabel";
            this.TXLabel.Size = new System.Drawing.Size(78, 13);
            this.TXLabel.TabIndex = 56;
            this.TXLabel.Text = "TX: transmitted";
            // 
            // RXLabel
            // 
            this.RXLabel.AutoSize = true;
            this.RXLabel.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.RXLabel.Location = new System.Drawing.Point(126, 280);
            this.RXLabel.Name = "RXLabel";
            this.RXLabel.Size = new System.Drawing.Size(69, 13);
            this.RXLabel.TabIndex = 52;
            this.RXLabel.Text = "RX: received";
            // 
            // PacketSendTextbox
            // 
            this.PacketSendTextbox.Enabled = false;
            this.PacketSendTextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.PacketSendTextbox.Location = new System.Drawing.Point(3, 256);
            this.PacketSendTextbox.Name = "PacketSendTextbox";
            this.PacketSendTextbox.Size = new System.Drawing.Size(289, 21);
            this.PacketSendTextbox.TabIndex = 55;
            this.PacketSendTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PacketSendTextbox_KeyPress);
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
            this.PacketLogEnabledCheckbox.Enabled = false;
            this.PacketLogEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.PacketLogEnabledCheckbox.Name = "PacketLogEnabledCheckbox";
            this.PacketLogEnabledCheckbox.Size = new System.Drawing.Size(118, 17);
            this.PacketLogEnabledCheckbox.TabIndex = 1;
            this.PacketLogEnabledCheckbox.Text = "Packet log enabled";
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
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
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
            this.packetGeneratorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // packetGeneratorToolStripMenuItem
            // 
            this.packetGeneratorToolStripMenuItem.Name = "packetGeneratorToolStripMenuItem";
            this.packetGeneratorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.packetGeneratorToolStripMenuItem.Text = "Packet Generator";
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
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgStackingCheckbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgTextbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterClearButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterApplyButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterTextbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusMsgFilterCheckbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusSendMsgTextbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusSendMsgButton);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusEnabledCheckbox);
            this.CCDBusMessagesGroupbox.Controls.Add(this.CCDBusClearButton);
            this.CCDBusMessagesGroupbox.Location = new System.Drawing.Point(383, 27);
            this.CCDBusMessagesGroupbox.Name = "CCDBusMessagesGroupbox";
            this.CCDBusMessagesGroupbox.Size = new System.Drawing.Size(220, 431);
            this.CCDBusMessagesGroupbox.TabIndex = 43;
            this.CCDBusMessagesGroupbox.TabStop = false;
            this.CCDBusMessagesGroupbox.Text = "CCD-bus messages";
            // 
            // CCDBusMsgStackingCheckbox
            // 
            this.CCDBusMsgStackingCheckbox.AutoSize = true;
            this.CCDBusMsgStackingCheckbox.Enabled = false;
            this.CCDBusMsgStackingCheckbox.Location = new System.Drawing.Point(146, 406);
            this.CCDBusMsgStackingCheckbox.Name = "CCDBusMsgStackingCheckbox";
            this.CCDBusMsgStackingCheckbox.Size = new System.Drawing.Size(68, 17);
            this.CCDBusMsgStackingCheckbox.TabIndex = 69;
            this.CCDBusMsgStackingCheckbox.Text = "Stacking";
            this.CCDBusMsgStackingCheckbox.UseVisualStyleBackColor = true;
            // 
            // CCDBusMsgTextbox
            // 
            this.CCDBusMsgTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CCDBusMsgTextbox.Location = new System.Drawing.Point(3, 16);
            this.CCDBusMsgTextbox.MaxLength = 0;
            this.CCDBusMsgTextbox.Multiline = true;
            this.CCDBusMsgTextbox.Name = "CCDBusMsgTextbox";
            this.CCDBusMsgTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CCDBusMsgTextbox.Size = new System.Drawing.Size(214, 234);
            this.CCDBusMsgTextbox.TabIndex = 66;
            this.CCDBusMsgTextbox.TextChanged += new System.EventHandler(this.CCDBusMsgTextbox_TextChanged);
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
            // CCDBusMsgFilterTextbox
            // 
            this.CCDBusMsgFilterTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CCDBusMsgFilterTextbox.Location = new System.Drawing.Point(3, 338);
            this.CCDBusMsgFilterTextbox.MaxLength = 0;
            this.CCDBusMsgFilterTextbox.Multiline = true;
            this.CCDBusMsgFilterTextbox.Name = "CCDBusMsgFilterTextbox";
            this.CCDBusMsgFilterTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CCDBusMsgFilterTextbox.Size = new System.Drawing.Size(213, 59);
            this.CCDBusMsgFilterTextbox.TabIndex = 66;
            this.CCDBusMsgFilterTextbox.TextChanged += new System.EventHandler(this.CCDMessageFilteringTextbox_TextChanged);
            // 
            // CCDBusMsgFilterCheckbox
            // 
            this.CCDBusMsgFilterCheckbox.AutoSize = true;
            this.CCDBusMsgFilterCheckbox.Enabled = false;
            this.CCDBusMsgFilterCheckbox.Location = new System.Drawing.Point(6, 311);
            this.CCDBusMsgFilterCheckbox.Name = "CCDBusMsgFilterCheckbox";
            this.CCDBusMsgFilterCheckbox.Size = new System.Drawing.Size(149, 17);
            this.CCDBusMsgFilterCheckbox.TabIndex = 54;
            this.CCDBusMsgFilterCheckbox.Text = "CCD-bus message filtering";
            this.CCDBusMsgFilterCheckbox.UseVisualStyleBackColor = true;
            // 
            // CCDBusSendMsgTextbox
            // 
            this.CCDBusSendMsgTextbox.Enabled = false;
            this.CCDBusSendMsgTextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.CCDBusSendMsgTextbox.Location = new System.Drawing.Point(3, 256);
            this.CCDBusSendMsgTextbox.Name = "CCDBusSendMsgTextbox";
            this.CCDBusSendMsgTextbox.Size = new System.Drawing.Size(143, 21);
            this.CCDBusSendMsgTextbox.TabIndex = 53;
            this.CCDBusSendMsgTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ccd_bus_send_msg_textbox_KeyPress);
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
            this.CCDBusEnabledCheckbox.Enabled = false;
            this.CCDBusEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.CCDBusEnabledCheckbox.Name = "CCDBusEnabledCheckbox";
            this.CCDBusEnabledCheckbox.Size = new System.Drawing.Size(110, 17);
            this.CCDBusEnabledCheckbox.TabIndex = 47;
            this.CCDBusEnabledCheckbox.Text = "CCD-bus disabled";
            this.CCDBusEnabledCheckbox.UseVisualStyleBackColor = true;
            this.CCDBusEnabledCheckbox.Click += new System.EventHandler(this.ccd_bus_enabled_checkbox_Click);
            // 
            // CCDBusClearButton
            // 
            this.CCDBusClearButton.Enabled = false;
            this.CCDBusClearButton.Location = new System.Drawing.Point(153, 283);
            this.CCDBusClearButton.Name = "CCDBusClearButton";
            this.CCDBusClearButton.Size = new System.Drawing.Size(60, 23);
            this.CCDBusClearButton.TabIndex = 48;
            this.CCDBusClearButton.Text = "Clear";
            this.CCDBusClearButton.UseVisualStyleBackColor = true;
            this.CCDBusClearButton.Click += new System.EventHandler(this.ccd_bus_log_clear_button_Click);
            // 
            // SCIBusMessagesGroupbox
            // 
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgStackingCheckbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgTextbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterClearButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterApplyButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusEnabledCheckbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterTextbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusMsgFilterCheckbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusClearButton);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusSendMsgTextbox);
            this.SCIBusMessagesGroupbox.Controls.Add(this.SCIBusSendMsgButton);
            this.SCIBusMessagesGroupbox.Location = new System.Drawing.Point(609, 27);
            this.SCIBusMessagesGroupbox.Name = "SCIBusMessagesGroupbox";
            this.SCIBusMessagesGroupbox.Size = new System.Drawing.Size(220, 431);
            this.SCIBusMessagesGroupbox.TabIndex = 44;
            this.SCIBusMessagesGroupbox.TabStop = false;
            this.SCIBusMessagesGroupbox.Text = "SCI-bus messages";
            // 
            // SCIBusMsgStackingCheckbox
            // 
            this.SCIBusMsgStackingCheckbox.AutoSize = true;
            this.SCIBusMsgStackingCheckbox.Enabled = false;
            this.SCIBusMsgStackingCheckbox.Location = new System.Drawing.Point(146, 406);
            this.SCIBusMsgStackingCheckbox.Name = "SCIBusMsgStackingCheckbox";
            this.SCIBusMsgStackingCheckbox.Size = new System.Drawing.Size(68, 17);
            this.SCIBusMsgStackingCheckbox.TabIndex = 70;
            this.SCIBusMsgStackingCheckbox.Text = "Stacking";
            this.SCIBusMsgStackingCheckbox.UseVisualStyleBackColor = true;
            // 
            // SCIBusMsgTextbox
            // 
            this.SCIBusMsgTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SCIBusMsgTextbox.Location = new System.Drawing.Point(3, 16);
            this.SCIBusMsgTextbox.MaxLength = 0;
            this.SCIBusMsgTextbox.Multiline = true;
            this.SCIBusMsgTextbox.Name = "SCIBusMsgTextbox";
            this.SCIBusMsgTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SCIBusMsgTextbox.Size = new System.Drawing.Size(214, 234);
            this.SCIBusMsgTextbox.TabIndex = 69;
            this.SCIBusMsgTextbox.TextChanged += new System.EventHandler(this.SCIBusMsgTextbox_TextChanged);
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
            // 
            // SCIBusEnabledCheckbox
            // 
            this.SCIBusEnabledCheckbox.AutoSize = true;
            this.SCIBusEnabledCheckbox.Enabled = false;
            this.SCIBusEnabledCheckbox.Location = new System.Drawing.Point(6, 288);
            this.SCIBusEnabledCheckbox.Name = "SCIBusEnabledCheckbox";
            this.SCIBusEnabledCheckbox.Size = new System.Drawing.Size(105, 17);
            this.SCIBusEnabledCheckbox.TabIndex = 49;
            this.SCIBusEnabledCheckbox.Text = "SCI-bus disabled";
            this.SCIBusEnabledCheckbox.UseVisualStyleBackColor = true;
            this.SCIBusEnabledCheckbox.Click += new System.EventHandler(this.sci_bus_enabled_checkbox_Click);
            // 
            // SCIBusMsgFilterTextbox
            // 
            this.SCIBusMsgFilterTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SCIBusMsgFilterTextbox.Location = new System.Drawing.Point(3, 338);
            this.SCIBusMsgFilterTextbox.MaxLength = 0;
            this.SCIBusMsgFilterTextbox.Multiline = true;
            this.SCIBusMsgFilterTextbox.Name = "SCIBusMsgFilterTextbox";
            this.SCIBusMsgFilterTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SCIBusMsgFilterTextbox.Size = new System.Drawing.Size(213, 59);
            this.SCIBusMsgFilterTextbox.TabIndex = 70;
            this.SCIBusMsgFilterTextbox.TextChanged += new System.EventHandler(this.SCIMessageFilteringTextbox_TextChanged);
            // 
            // SCIBusMsgFilterCheckbox
            // 
            this.SCIBusMsgFilterCheckbox.AutoSize = true;
            this.SCIBusMsgFilterCheckbox.Enabled = false;
            this.SCIBusMsgFilterCheckbox.Location = new System.Drawing.Point(6, 311);
            this.SCIBusMsgFilterCheckbox.Name = "SCIBusMsgFilterCheckbox";
            this.SCIBusMsgFilterCheckbox.Size = new System.Drawing.Size(144, 17);
            this.SCIBusMsgFilterCheckbox.TabIndex = 69;
            this.SCIBusMsgFilterCheckbox.Text = "SCI-bus message filtering";
            this.SCIBusMsgFilterCheckbox.UseVisualStyleBackColor = true;
            // 
            // SCIBusClearButton
            // 
            this.SCIBusClearButton.Enabled = false;
            this.SCIBusClearButton.Location = new System.Drawing.Point(153, 283);
            this.SCIBusClearButton.Name = "SCIBusClearButton";
            this.SCIBusClearButton.Size = new System.Drawing.Size(60, 23);
            this.SCIBusClearButton.TabIndex = 51;
            this.SCIBusClearButton.Text = "Clear";
            this.SCIBusClearButton.UseVisualStyleBackColor = true;
            this.SCIBusClearButton.Click += new System.EventHandler(this.sci_bus_log_clear_button_Click);
            // 
            // command_history_groupbox
            // 
            this.command_history_groupbox.Controls.Add(this.command_history_textbox);
            this.command_history_groupbox.Location = new System.Drawing.Point(12, 348);
            this.command_history_groupbox.Name = "command_history_groupbox";
            this.command_history_groupbox.Size = new System.Drawing.Size(365, 110);
            this.command_history_groupbox.TabIndex = 45;
            this.command_history_groupbox.TabStop = false;
            this.command_history_groupbox.Text = "Command history";
            // 
            // command_history_textbox
            // 
            this.command_history_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.command_history_textbox.Location = new System.Drawing.Point(3, 17);
            this.command_history_textbox.MaxLength = 0;
            this.command_history_textbox.Multiline = true;
            this.command_history_textbox.Name = "command_history_textbox";
            this.command_history_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.command_history_textbox.Size = new System.Drawing.Size(358, 87);
            this.command_history_textbox.TabIndex = 69;
            // 
            // MiscGroupbox
            // 
            this.MiscGroupbox.Controls.Add(this.packet_count_tx_label);
            this.MiscGroupbox.Controls.Add(this.RebootScannerButton);
            this.MiscGroupbox.Controls.Add(this.packet_count_rx_label);
            this.MiscGroupbox.Controls.Add(this.buffer_start_label);
            this.MiscGroupbox.Controls.Add(this.buffer_end_label);
            this.MiscGroupbox.Controls.Add(this.buffer_readlength_label);
            this.MiscGroupbox.Controls.Add(this.buffer_writelength_label);
            this.MiscGroupbox.Location = new System.Drawing.Point(162, 464);
            this.MiscGroupbox.Name = "MiscGroupbox";
            this.MiscGroupbox.Size = new System.Drawing.Size(215, 136);
            this.MiscGroupbox.TabIndex = 46;
            this.MiscGroupbox.TabStop = false;
            this.MiscGroupbox.Text = "Misc.";
            // 
            // packet_count_tx_label
            // 
            this.packet_count_tx_label.AutoSize = true;
            this.packet_count_tx_label.Location = new System.Drawing.Point(6, 35);
            this.packet_count_tx_label.Name = "packet_count_tx_label";
            this.packet_count_tx_label.Size = new System.Drawing.Size(75, 13);
            this.packet_count_tx_label.TabIndex = 24;
            this.packet_count_tx_label.Text = "Packets sent: ";
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
            this.EEPROMGroupbox.Text = "EEPROM";
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
            this.label16.Location = new System.Drawing.Point(6, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 49;
            this.label16.Text = "Internal EEPROM:";
            // 
            // RealTimeDiagnosticsGroupbox
            // 
            this.RealTimeDiagnosticsGroupbox.Controls.Add(this.real_time_diagnostics_button);
            this.RealTimeDiagnosticsGroupbox.Location = new System.Drawing.Point(383, 464);
            this.RealTimeDiagnosticsGroupbox.Name = "RealTimeDiagnosticsGroupbox";
            this.RealTimeDiagnosticsGroupbox.Size = new System.Drawing.Size(140, 66);
            this.RealTimeDiagnosticsGroupbox.TabIndex = 50;
            this.RealTimeDiagnosticsGroupbox.TabStop = false;
            this.RealTimeDiagnosticsGroupbox.Text = "Real Time Diagnostics";
            // 
            // real_time_diagnostics_button
            // 
            this.real_time_diagnostics_button.Dock = System.Windows.Forms.DockStyle.Top;
            this.real_time_diagnostics_button.Location = new System.Drawing.Point(3, 16);
            this.real_time_diagnostics_button.Name = "real_time_diagnostics_button";
            this.real_time_diagnostics_button.Size = new System.Drawing.Size(134, 44);
            this.real_time_diagnostics_button.TabIndex = 51;
            this.real_time_diagnostics_button.Text = "Open";
            this.real_time_diagnostics_button.UseVisualStyleBackColor = true;
            this.real_time_diagnostics_button.Click += new System.EventHandler(this.real_time_diagnostics_button_Click);
            // 
            // ReadDTCsGroupbox
            // 
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_tcm_button);
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_mic_button);
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_abs_button);
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_acm_button);
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_bcm_button);
            this.ReadDTCsGroupbox.Controls.Add(this.read_dtc_pcm_button);
            this.ReadDTCsGroupbox.Location = new System.Drawing.Point(383, 534);
            this.ReadDTCsGroupbox.Name = "ReadDTCsGroupbox";
            this.ReadDTCsGroupbox.Size = new System.Drawing.Size(140, 106);
            this.ReadDTCsGroupbox.TabIndex = 51;
            this.ReadDTCsGroupbox.TabStop = false;
            this.ReadDTCsGroupbox.Text = "Read DTCs";
            // 
            // read_dtc_tcm_button
            // 
            this.read_dtc_tcm_button.Location = new System.Drawing.Point(6, 77);
            this.read_dtc_tcm_button.Name = "read_dtc_tcm_button";
            this.read_dtc_tcm_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_tcm_button.TabIndex = 57;
            this.read_dtc_tcm_button.Text = "TCM";
            this.toolTip1.SetToolTip(this.read_dtc_tcm_button, "Transmission Control Module");
            this.read_dtc_tcm_button.UseVisualStyleBackColor = true;
            // 
            // read_dtc_mic_button
            // 
            this.read_dtc_mic_button.Location = new System.Drawing.Point(73, 77);
            this.read_dtc_mic_button.Name = "read_dtc_mic_button";
            this.read_dtc_mic_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_mic_button.TabIndex = 56;
            this.read_dtc_mic_button.Text = "MIC";
            this.toolTip1.SetToolTip(this.read_dtc_mic_button, "Mechanical Instrument Cluster");
            this.read_dtc_mic_button.UseVisualStyleBackColor = true;
            this.read_dtc_mic_button.Click += new System.EventHandler(this.read_dtc_mic_button_Click);
            // 
            // read_dtc_abs_button
            // 
            this.read_dtc_abs_button.Location = new System.Drawing.Point(73, 48);
            this.read_dtc_abs_button.Name = "read_dtc_abs_button";
            this.read_dtc_abs_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_abs_button.TabIndex = 55;
            this.read_dtc_abs_button.Text = "ABS";
            this.toolTip1.SetToolTip(this.read_dtc_abs_button, "Antilock Brake System");
            this.read_dtc_abs_button.UseVisualStyleBackColor = true;
            // 
            // read_dtc_acm_button
            // 
            this.read_dtc_acm_button.Location = new System.Drawing.Point(6, 48);
            this.read_dtc_acm_button.Name = "read_dtc_acm_button";
            this.read_dtc_acm_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_acm_button.TabIndex = 54;
            this.read_dtc_acm_button.Text = "ACM";
            this.toolTip1.SetToolTip(this.read_dtc_acm_button, "Airbag Control Module");
            this.read_dtc_acm_button.UseVisualStyleBackColor = true;
            // 
            // read_dtc_bcm_button
            // 
            this.read_dtc_bcm_button.Location = new System.Drawing.Point(73, 19);
            this.read_dtc_bcm_button.Name = "read_dtc_bcm_button";
            this.read_dtc_bcm_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_bcm_button.TabIndex = 53;
            this.read_dtc_bcm_button.Text = "BCM";
            this.toolTip1.SetToolTip(this.read_dtc_bcm_button, "Body Control Module");
            this.read_dtc_bcm_button.UseVisualStyleBackColor = true;
            // 
            // read_dtc_pcm_button
            // 
            this.read_dtc_pcm_button.Location = new System.Drawing.Point(6, 19);
            this.read_dtc_pcm_button.Name = "read_dtc_pcm_button";
            this.read_dtc_pcm_button.Size = new System.Drawing.Size(61, 23);
            this.read_dtc_pcm_button.TabIndex = 52;
            this.read_dtc_pcm_button.Text = "PCM";
            this.toolTip1.SetToolTip(this.read_dtc_pcm_button, "Powertrain Control Module");
            this.read_dtc_pcm_button.UseVisualStyleBackColor = true;
            this.read_dtc_pcm_button.Click += new System.EventHandler(this.read_dtc_pcm_button_Click);
            // 
            // SCIBusSpeedGroupbox
            // 
            this.SCIBusSpeedGroupbox.Controls.Add(this.sci_bus_hs_button);
            this.SCIBusSpeedGroupbox.Controls.Add(this.sci_bus_speed_label);
            this.SCIBusSpeedGroupbox.Location = new System.Drawing.Point(383, 646);
            this.SCIBusSpeedGroupbox.Name = "SCIBusSpeedGroupbox";
            this.SCIBusSpeedGroupbox.Size = new System.Drawing.Size(140, 42);
            this.SCIBusSpeedGroupbox.TabIndex = 56;
            this.SCIBusSpeedGroupbox.TabStop = false;
            this.SCIBusSpeedGroupbox.Text = "SCI-bus speed";
            // 
            // SensorDataGroupbox
            // 
            this.SensorDataGroupbox.Controls.Add(this.SensorDataTextbox);
            this.SensorDataGroupbox.Location = new System.Drawing.Point(835, 27);
            this.SensorDataGroupbox.Name = "SensorDataGroupbox";
            this.SensorDataGroupbox.Size = new System.Drawing.Size(341, 663);
            this.SensorDataGroupbox.TabIndex = 52;
            this.SensorDataGroupbox.TabStop = false;
            this.SensorDataGroupbox.Text = "Sensor data";
            // 
            // SensorDataTextbox
            // 
            this.SensorDataTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SensorDataTextbox.Location = new System.Drawing.Point(6, 16);
            this.SensorDataTextbox.MaxLength = 0;
            this.SensorDataTextbox.Multiline = true;
            this.SensorDataTextbox.Name = "SensorDataTextbox";
            this.SensorDataTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SensorDataTextbox.Size = new System.Drawing.Size(329, 641);
            this.SensorDataTextbox.TabIndex = 73;
            // 
            // ETCGroupbox
            // 
            this.ETCGroupbox.Location = new System.Drawing.Point(529, 525);
            this.ETCGroupbox.Name = "ETCGroupbox";
            this.ETCGroupbox.Size = new System.Drawing.Size(300, 163);
            this.ETCGroupbox.TabIndex = 52;
            this.ETCGroupbox.TabStop = false;
            this.ETCGroupbox.Text = "etc...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 692);
            this.Controls.Add(this.PacketGeneratorButton);
            this.Controls.Add(this.ETCGroupbox);
            this.Controls.Add(this.SensorDataGroupbox);
            this.Controls.Add(this.SCIBusSpeedGroupbox);
            this.Controls.Add(this.ReadDTCsGroupbox);
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
            this.ReadDTCsGroupbox.ResumeLayout(false);
            this.SCIBusSpeedGroupbox.ResumeLayout(false);
            this.SCIBusSpeedGroupbox.PerformLayout();
            this.SensorDataGroupbox.ResumeLayout(false);
            this.SensorDataGroupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label packet_count_rx_label;
        private System.Windows.Forms.Label buffer_writelength_label;
        private System.Windows.Forms.Label buffer_start_label;
        private System.Windows.Forms.Label buffer_end_label;
        private System.Windows.Forms.Label buffer_readlength_label;
        private System.Windows.Forms.Button read_exteeprom_button;
        private System.Windows.Forms.Button read_inteeprom_button;
        private System.Windows.Forms.Button StatusButton;
        private System.Windows.Forms.TextBox SCIBusSendMsgTextbox;
        private System.Windows.Forms.Button sci_bus_hs_button;
        private System.Windows.Forms.Label sci_bus_speed_label;
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
        private System.Windows.Forms.Button CCDBusClearButton;
        private System.Windows.Forms.CheckBox SCIBusEnabledCheckbox;
        private System.Windows.Forms.Button SCIBusClearButton;
        private System.Windows.Forms.GroupBox MiscGroupbox;
        private System.Windows.Forms.GroupBox ControlPanelGroupbox;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button SCIBusSendMsgButton;
        private System.Windows.Forms.TextBox CCDBusSendMsgTextbox;
        private System.Windows.Forms.Button CCDBusSendMsgButton;
        private System.Windows.Forms.GroupBox EEPROMGroupbox;
        private System.Windows.Forms.Button erase_inteeprom_button;
        private System.Windows.Forms.Button save_inteeprom_button;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button write_inteeprom_button;
        private System.Windows.Forms.Button write_exteeprom_button;
        private System.Windows.Forms.Button save_exteeprom_button;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox RealTimeDiagnosticsGroupbox;
        private System.Windows.Forms.Button real_time_diagnostics_button;
        private System.Windows.Forms.TextBox PacketSendTextbox;
        private System.Windows.Forms.Button PacketSendButton;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox ReadDTCsGroupbox;
        private System.Windows.Forms.Button read_dtc_abs_button;
        private System.Windows.Forms.Button read_dtc_acm_button;
        private System.Windows.Forms.Button read_dtc_bcm_button;
        private System.Windows.Forms.Button read_dtc_pcm_button;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Label RXLabel;
        private System.Windows.Forms.Label TXLabel;
        private System.Windows.Forms.GroupBox SCIBusSpeedGroupbox;
        private System.Windows.Forms.Button read_dtc_mic_button;
        private System.Windows.Forms.Button read_dtc_tcm_button;
        private System.Windows.Forms.GroupBox SensorDataGroupbox;
        private System.Windows.Forms.Button RebootScannerButton;
        private System.Windows.Forms.TextBox PacketTextbox;
        private System.Windows.Forms.Button CCDBusMsgFilterClearButton;
        private System.Windows.Forms.Button CCDBusMsgFilterApplyButton;
        private System.Windows.Forms.TextBox CCDBusMsgFilterTextbox;
        private System.Windows.Forms.CheckBox CCDBusMsgFilterCheckbox;
        private System.Windows.Forms.Button SCIBusMsgFilterClearButton;
        private System.Windows.Forms.Button SCIBusMsgFilterApplyButton;
        private System.Windows.Forms.TextBox SCIBusMsgFilterTextbox;
        private System.Windows.Forms.CheckBox SCIBusMsgFilterCheckbox;
        private System.Windows.Forms.TextBox command_history_textbox;
        private System.Windows.Forms.Label packet_count_tx_label;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetGeneratorToolStripMenuItem;
        private System.Windows.Forms.TextBox CCDBusMsgTextbox;
        private System.Windows.Forms.TextBox SCIBusMsgTextbox;
        private System.Windows.Forms.TextBox SensorDataTextbox;
        private System.Windows.Forms.CheckBox CCDBusMsgStackingCheckbox;
        private System.Windows.Forms.CheckBox SCIBusMsgStackingCheckbox;
        private System.Windows.Forms.GroupBox ETCGroupbox;
        private System.Windows.Forms.Button PacketGeneratorButton;
    }
}

