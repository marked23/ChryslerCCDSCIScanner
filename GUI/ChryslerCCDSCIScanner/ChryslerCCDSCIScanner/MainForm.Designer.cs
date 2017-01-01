namespace ChryslerCCDSCIScanner
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
            this.connect_button = new System.Windows.Forms.Button();
            this.packet_count_rx_label = new System.Windows.Forms.Label();
            this.buffer_writelength_label = new System.Windows.Forms.Label();
            this.dump_pcm = new System.Windows.Forms.Button();
            this.buffer_start_label = new System.Windows.Forms.Label();
            this.buffer_end_label = new System.Windows.Forms.Label();
            this.buffer_readlength_label = new System.Windows.Forms.Label();
            this.read_exteeprom_button = new System.Windows.Forms.Button();
            this.read_inteeprom_button = new System.Windows.Forms.Button();
            this.status_button = new System.Windows.Forms.Button();
            this.sci_bus_send_msg_button = new System.Windows.Forms.Button();
            this.sci_bus_send_msg_textbox = new System.Windows.Forms.TextBox();
            this.sci_bus_hs_button = new System.Windows.Forms.Button();
            this.sci_bus_speed_label = new System.Windows.Forms.Label();
            this.erase_exteeprom_button = new System.Windows.Forms.Button();
            this.pcm_ram_button = new System.Windows.Forms.Button();
            this.pcm_ram_values_richtextbox = new System.Windows.Forms.RichTextBox();
            this.o2_sensor_button = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.convert_o2_button = new System.Windows.Forms.Button();
            this.convert_ram_button = new System.Windows.Forms.Button();
            this.log_groupbox = new System.Windows.Forms.GroupBox();
            this.packet_log_textbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.packet_send_textbox = new System.Windows.Forms.TextBox();
            this.packet_send_button = new System.Windows.Forms.Button();
            this.packet_log_clear_button = new System.Windows.Forms.Button();
            this.packet_log_save_button = new System.Windows.Forms.Button();
            this.communication_packet_log_enabled_checkbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ccd_bus_messages_groupbox = new System.Windows.Forms.GroupBox();
            this.ccd_filter_clear_button = new System.Windows.Forms.Button();
            this.ccd_filtering_apply_button = new System.Windows.Forms.Button();
            this.ccd_message_filtering_textbox = new System.Windows.Forms.TextBox();
            this.ccd_filtering_checkbox = new System.Windows.Forms.CheckBox();
            this.ccd_bus_msg_richtextbox = new System.Windows.Forms.RichTextBox();
            this.ccd_bus_send_msg_textbox = new System.Windows.Forms.TextBox();
            this.ccd_bus_send_msg_button = new System.Windows.Forms.Button();
            this.ccd_bus_listen_checkbox = new System.Windows.Forms.CheckBox();
            this.ccd_bus_log_clear_button = new System.Windows.Forms.Button();
            this.ccd_bus_log_save_button = new System.Windows.Forms.Button();
            this.sci_bus_messages_groupbox = new System.Windows.Forms.GroupBox();
            this.sci_filtering_clear_button = new System.Windows.Forms.Button();
            this.sci_bus_msg_richtextbox = new System.Windows.Forms.RichTextBox();
            this.sci_filtering_apply_button = new System.Windows.Forms.Button();
            this.sci_bus_listen_checkbox = new System.Windows.Forms.CheckBox();
            this.sci_message_filtering_textbox = new System.Windows.Forms.TextBox();
            this.sci_filtering_checkbox = new System.Windows.Forms.CheckBox();
            this.sci_bus_log_clear_button = new System.Windows.Forms.Button();
            this.sci_bus_log_save_button = new System.Windows.Forms.Button();
            this.command_history_groupbox = new System.Windows.Forms.GroupBox();
            this.command_history_textbox = new System.Windows.Forms.TextBox();
            this.misc_groupbox = new System.Windows.Forms.GroupBox();
            this.packet_count_tx_label = new System.Windows.Forms.Label();
            this.reboot_scanner_button = new System.Windows.Forms.Button();
            this.scanner_connection_groupbox = new System.Windows.Forms.GroupBox();
            this.exit_button = new System.Windows.Forms.Button();
            this.disconnect_button = new System.Windows.Forms.Button();
            this.eeprom_groupbox = new System.Windows.Forms.GroupBox();
            this.write_exteeprom_button = new System.Windows.Forms.Button();
            this.save_exteeprom_button = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.write_inteeprom_button = new System.Windows.Forms.Button();
            this.erase_inteeprom_button = new System.Windows.Forms.Button();
            this.save_inteeprom_button = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.pcm_ram_map_groupbox = new System.Windows.Forms.GroupBox();
            this.real_time_diagnostics_groupbox = new System.Windows.Forms.GroupBox();
            this.real_time_diagnostics_button = new System.Windows.Forms.Button();
            this.read_dtcs_groupbox = new System.Windows.Forms.GroupBox();
            this.read_dtc_tcm_button = new System.Windows.Forms.Button();
            this.read_dtc_mic_button = new System.Windows.Forms.Button();
            this.read_dtc_abs_button = new System.Windows.Forms.Button();
            this.read_dtc_acm_button = new System.Windows.Forms.Button();
            this.read_dtc_bcm_button = new System.Windows.Forms.Button();
            this.read_dtc_pcm_button = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pcm_ram_area_textbox = new System.Windows.Forms.TextBox();
            this.sci_bus_speed_groupbox = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.map_sensor_button = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.pcm_read_all = new System.Windows.Forms.Button();
            this.pcm_parameters_richtextbox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.flush_memory_button = new System.Windows.Forms.Button();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.log_groupbox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.ccd_bus_messages_groupbox.SuspendLayout();
            this.sci_bus_messages_groupbox.SuspendLayout();
            this.command_history_groupbox.SuspendLayout();
            this.misc_groupbox.SuspendLayout();
            this.scanner_connection_groupbox.SuspendLayout();
            this.eeprom_groupbox.SuspendLayout();
            this.pcm_ram_map_groupbox.SuspendLayout();
            this.real_time_diagnostics_groupbox.SuspendLayout();
            this.read_dtcs_groupbox.SuspendLayout();
            this.sci_bus_speed_groupbox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(6, 19);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(132, 23);
            this.connect_button.TabIndex = 1;
            this.connect_button.Text = "Connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
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
            // dump_pcm
            // 
            this.dump_pcm.Location = new System.Drawing.Point(838, 317);
            this.dump_pcm.Name = "dump_pcm";
            this.dump_pcm.Size = new System.Drawing.Size(75, 23);
            this.dump_pcm.TabIndex = 4;
            this.dump_pcm.Text = "Dump PCM";
            this.dump_pcm.UseVisualStyleBackColor = true;
            this.dump_pcm.Click += new System.EventHandler(this.dump_pcm_Click);
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
            // status_button
            // 
            this.status_button.Enabled = false;
            this.status_button.Location = new System.Drawing.Point(6, 77);
            this.status_button.Name = "status_button";
            this.status_button.Size = new System.Drawing.Size(132, 23);
            this.status_button.TabIndex = 21;
            this.status_button.Text = "Status";
            this.status_button.UseVisualStyleBackColor = true;
            this.status_button.Click += new System.EventHandler(this.status_button_Click);
            // 
            // sci_bus_send_msg_button
            // 
            this.sci_bus_send_msg_button.Enabled = false;
            this.sci_bus_send_msg_button.Location = new System.Drawing.Point(153, 254);
            this.sci_bus_send_msg_button.Name = "sci_bus_send_msg_button";
            this.sci_bus_send_msg_button.Size = new System.Drawing.Size(60, 23);
            this.sci_bus_send_msg_button.TabIndex = 26;
            this.sci_bus_send_msg_button.Text = "Send";
            this.sci_bus_send_msg_button.UseVisualStyleBackColor = true;
            this.sci_bus_send_msg_button.Click += new System.EventHandler(this.send_sci_msg_button_Click);
            // 
            // sci_bus_send_msg_textbox
            // 
            this.sci_bus_send_msg_textbox.Enabled = false;
            this.sci_bus_send_msg_textbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.sci_bus_send_msg_textbox.Location = new System.Drawing.Point(3, 256);
            this.sci_bus_send_msg_textbox.Name = "sci_bus_send_msg_textbox";
            this.sci_bus_send_msg_textbox.Size = new System.Drawing.Size(143, 21);
            this.sci_bus_send_msg_textbox.TabIndex = 27;
            this.sci_bus_send_msg_textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sci_bus_send_msg_textbox_KeyPress);
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
            // pcm_ram_button
            // 
            this.pcm_ram_button.Location = new System.Drawing.Point(838, 346);
            this.pcm_ram_button.Name = "pcm_ram_button";
            this.pcm_ram_button.Size = new System.Drawing.Size(69, 23);
            this.pcm_ram_button.TabIndex = 31;
            this.pcm_ram_button.Text = "PCM RAM";
            this.pcm_ram_button.UseVisualStyleBackColor = true;
            this.pcm_ram_button.Click += new System.EventHandler(this.pcm_ram_button_Click);
            // 
            // pcm_ram_values_richtextbox
            // 
            this.pcm_ram_values_richtextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcm_ram_values_richtextbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pcm_ram_values_richtextbox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pcm_ram_values_richtextbox.Location = new System.Drawing.Point(6, 19);
            this.pcm_ram_values_richtextbox.Name = "pcm_ram_values_richtextbox";
            this.pcm_ram_values_richtextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.pcm_ram_values_richtextbox.Size = new System.Drawing.Size(352, 199);
            this.pcm_ram_values_richtextbox.TabIndex = 34;
            this.pcm_ram_values_richtextbox.Text = "";
            // 
            // o2_sensor_button
            // 
            this.o2_sensor_button.Location = new System.Drawing.Point(6, 607);
            this.o2_sensor_button.Name = "o2_sensor_button";
            this.o2_sensor_button.Size = new System.Drawing.Size(75, 23);
            this.o2_sensor_button.TabIndex = 35;
            this.o2_sensor_button.Text = "O2 Sensor";
            this.o2_sensor_button.UseVisualStyleBackColor = true;
            this.o2_sensor_button.Click += new System.EventHandler(this.o2_sensor_button_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(33, 630);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(29, 20);
            this.textBox1.TabIndex = 36;
            this.textBox1.Text = "0,00";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 633);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "O2:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(65, 633);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(14, 13);
            this.label15.TabIndex = 38;
            this.label15.Text = "V";
            // 
            // convert_o2_button
            // 
            this.convert_o2_button.Location = new System.Drawing.Point(87, 607);
            this.convert_o2_button.Name = "convert_o2_button";
            this.convert_o2_button.Size = new System.Drawing.Size(75, 23);
            this.convert_o2_button.TabIndex = 39;
            this.convert_o2_button.Text = "Convert O2";
            this.convert_o2_button.UseVisualStyleBackColor = true;
            this.convert_o2_button.Click += new System.EventHandler(this.convert_o2_button_Click);
            // 
            // convert_ram_button
            // 
            this.convert_ram_button.Location = new System.Drawing.Point(837, 374);
            this.convert_ram_button.Name = "convert_ram_button";
            this.convert_ram_button.Size = new System.Drawing.Size(106, 23);
            this.convert_ram_button.TabIndex = 40;
            this.convert_ram_button.Text = "Convert RAM";
            this.convert_ram_button.UseVisualStyleBackColor = true;
            this.convert_ram_button.Click += new System.EventHandler(this.convert_ram_button_Click);
            // 
            // log_groupbox
            // 
            this.log_groupbox.Controls.Add(this.packet_log_textbox);
            this.log_groupbox.Controls.Add(this.label9);
            this.log_groupbox.Controls.Add(this.label8);
            this.log_groupbox.Controls.Add(this.packet_send_textbox);
            this.log_groupbox.Controls.Add(this.packet_send_button);
            this.log_groupbox.Controls.Add(this.packet_log_clear_button);
            this.log_groupbox.Controls.Add(this.packet_log_save_button);
            this.log_groupbox.Controls.Add(this.communication_packet_log_enabled_checkbox);
            this.log_groupbox.Location = new System.Drawing.Point(12, 27);
            this.log_groupbox.Name = "log_groupbox";
            this.log_groupbox.Size = new System.Drawing.Size(365, 315);
            this.log_groupbox.TabIndex = 41;
            this.log_groupbox.TabStop = false;
            this.log_groupbox.Text = "Communication Packets";
            // 
            // packet_log_textbox
            // 
            this.packet_log_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.packet_log_textbox.Location = new System.Drawing.Point(3, 16);
            this.packet_log_textbox.MaxLength = 0;
            this.packet_log_textbox.Multiline = true;
            this.packet_log_textbox.Name = "packet_log_textbox";
            this.packet_log_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.packet_log_textbox.Size = new System.Drawing.Size(358, 234);
            this.packet_log_textbox.TabIndex = 65;
            this.packet_log_textbox.TextChanged += new System.EventHandler(this.packet_log_textbox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label9.Location = new System.Drawing.Point(249, 295);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 56;
            this.label9.Text = "TX: transmitted";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label8.Location = new System.Drawing.Point(248, 281);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 52;
            this.label8.Text = "RX: received";
            // 
            // packet_send_textbox
            // 
            this.packet_send_textbox.Enabled = false;
            this.packet_send_textbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.packet_send_textbox.Location = new System.Drawing.Point(3, 256);
            this.packet_send_textbox.Name = "packet_send_textbox";
            this.packet_send_textbox.Size = new System.Drawing.Size(289, 21);
            this.packet_send_textbox.TabIndex = 55;
            this.packet_send_textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.packet_send_textbox_KeyPress);
            // 
            // packet_send_button
            // 
            this.packet_send_button.Enabled = false;
            this.packet_send_button.Location = new System.Drawing.Point(298, 254);
            this.packet_send_button.Name = "packet_send_button";
            this.packet_send_button.Size = new System.Drawing.Size(60, 23);
            this.packet_send_button.TabIndex = 54;
            this.packet_send_button.Text = "Send";
            this.packet_send_button.UseVisualStyleBackColor = true;
            this.packet_send_button.Click += new System.EventHandler(this.packet_send_button_Click);
            // 
            // packet_log_clear_button
            // 
            this.packet_log_clear_button.Enabled = false;
            this.packet_log_clear_button.Location = new System.Drawing.Point(153, 284);
            this.packet_log_clear_button.Name = "packet_log_clear_button";
            this.packet_log_clear_button.Size = new System.Drawing.Size(60, 23);
            this.packet_log_clear_button.TabIndex = 46;
            this.packet_log_clear_button.Text = "Clear";
            this.packet_log_clear_button.UseVisualStyleBackColor = true;
            this.packet_log_clear_button.Click += new System.EventHandler(this.communication_log_clear_button_Click);
            // 
            // packet_log_save_button
            // 
            this.packet_log_save_button.Enabled = false;
            this.packet_log_save_button.Location = new System.Drawing.Point(87, 284);
            this.packet_log_save_button.Name = "packet_log_save_button";
            this.packet_log_save_button.Size = new System.Drawing.Size(60, 23);
            this.packet_log_save_button.TabIndex = 45;
            this.packet_log_save_button.Text = "Save";
            this.packet_log_save_button.UseVisualStyleBackColor = true;
            // 
            // communication_packet_log_enabled_checkbox
            // 
            this.communication_packet_log_enabled_checkbox.AutoSize = true;
            this.communication_packet_log_enabled_checkbox.Enabled = false;
            this.communication_packet_log_enabled_checkbox.Location = new System.Drawing.Point(6, 288);
            this.communication_packet_log_enabled_checkbox.Name = "communication_packet_log_enabled_checkbox";
            this.communication_packet_log_enabled_checkbox.Size = new System.Drawing.Size(77, 17);
            this.communication_packet_log_enabled_checkbox.TabIndex = 1;
            this.communication_packet_log_enabled_checkbox.Text = "Packet log";
            this.communication_packet_log_enabled_checkbox.UseVisualStyleBackColor = true;
            this.communication_packet_log_enabled_checkbox.Click += new System.EventHandler(this.communication_log_enabled_checkbox_Click);
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
            this.menuStrip1.Size = new System.Drawing.Size(1334, 24);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem1.Text = "File";
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
            // ccd_bus_messages_groupbox
            // 
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_filter_clear_button);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_filtering_apply_button);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_message_filtering_textbox);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_filtering_checkbox);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_msg_richtextbox);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_send_msg_textbox);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_send_msg_button);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_listen_checkbox);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_log_clear_button);
            this.ccd_bus_messages_groupbox.Controls.Add(this.ccd_bus_log_save_button);
            this.ccd_bus_messages_groupbox.Location = new System.Drawing.Point(383, 27);
            this.ccd_bus_messages_groupbox.Name = "ccd_bus_messages_groupbox";
            this.ccd_bus_messages_groupbox.Size = new System.Drawing.Size(220, 431);
            this.ccd_bus_messages_groupbox.TabIndex = 43;
            this.ccd_bus_messages_groupbox.TabStop = false;
            this.ccd_bus_messages_groupbox.Text = "CCD-bus Messages";
            // 
            // ccd_filter_clear_button
            // 
            this.ccd_filter_clear_button.Enabled = false;
            this.ccd_filter_clear_button.Location = new System.Drawing.Point(72, 402);
            this.ccd_filter_clear_button.Name = "ccd_filter_clear_button";
            this.ccd_filter_clear_button.Size = new System.Drawing.Size(60, 23);
            this.ccd_filter_clear_button.TabIndex = 68;
            this.ccd_filter_clear_button.Text = "Clear";
            this.ccd_filter_clear_button.UseVisualStyleBackColor = true;
            // 
            // ccd_filtering_apply_button
            // 
            this.ccd_filtering_apply_button.Enabled = false;
            this.ccd_filtering_apply_button.Location = new System.Drawing.Point(6, 402);
            this.ccd_filtering_apply_button.Name = "ccd_filtering_apply_button";
            this.ccd_filtering_apply_button.Size = new System.Drawing.Size(60, 23);
            this.ccd_filtering_apply_button.TabIndex = 67;
            this.ccd_filtering_apply_button.Text = "Apply";
            this.ccd_filtering_apply_button.UseVisualStyleBackColor = true;
            // 
            // ccd_message_filtering_textbox
            // 
            this.ccd_message_filtering_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ccd_message_filtering_textbox.Location = new System.Drawing.Point(3, 334);
            this.ccd_message_filtering_textbox.MaxLength = 0;
            this.ccd_message_filtering_textbox.Multiline = true;
            this.ccd_message_filtering_textbox.Name = "ccd_message_filtering_textbox";
            this.ccd_message_filtering_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ccd_message_filtering_textbox.Size = new System.Drawing.Size(213, 63);
            this.ccd_message_filtering_textbox.TabIndex = 66;
            // 
            // ccd_filtering_checkbox
            // 
            this.ccd_filtering_checkbox.AutoSize = true;
            this.ccd_filtering_checkbox.Enabled = false;
            this.ccd_filtering_checkbox.Location = new System.Drawing.Point(6, 311);
            this.ccd_filtering_checkbox.Name = "ccd_filtering_checkbox";
            this.ccd_filtering_checkbox.Size = new System.Drawing.Size(149, 17);
            this.ccd_filtering_checkbox.TabIndex = 54;
            this.ccd_filtering_checkbox.Text = "CCD-bus message filtering";
            this.ccd_filtering_checkbox.UseVisualStyleBackColor = true;
            // 
            // ccd_bus_msg_richtextbox
            // 
            this.ccd_bus_msg_richtextbox.BackColor = System.Drawing.SystemColors.Window;
            this.ccd_bus_msg_richtextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ccd_bus_msg_richtextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.ccd_bus_msg_richtextbox.Location = new System.Drawing.Point(3, 16);
            this.ccd_bus_msg_richtextbox.Name = "ccd_bus_msg_richtextbox";
            this.ccd_bus_msg_richtextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ccd_bus_msg_richtextbox.Size = new System.Drawing.Size(214, 234);
            this.ccd_bus_msg_richtextbox.TabIndex = 47;
            this.ccd_bus_msg_richtextbox.Text = "";
            this.ccd_bus_msg_richtextbox.TextChanged += new System.EventHandler(this.ccd_bus_messages_richtextbox_TextChanged);
            // 
            // ccd_bus_send_msg_textbox
            // 
            this.ccd_bus_send_msg_textbox.Enabled = false;
            this.ccd_bus_send_msg_textbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.ccd_bus_send_msg_textbox.Location = new System.Drawing.Point(3, 256);
            this.ccd_bus_send_msg_textbox.Name = "ccd_bus_send_msg_textbox";
            this.ccd_bus_send_msg_textbox.Size = new System.Drawing.Size(143, 21);
            this.ccd_bus_send_msg_textbox.TabIndex = 53;
            this.ccd_bus_send_msg_textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ccd_bus_send_msg_textbox_KeyPress);
            // 
            // ccd_bus_send_msg_button
            // 
            this.ccd_bus_send_msg_button.Enabled = false;
            this.ccd_bus_send_msg_button.Location = new System.Drawing.Point(153, 254);
            this.ccd_bus_send_msg_button.Name = "ccd_bus_send_msg_button";
            this.ccd_bus_send_msg_button.Size = new System.Drawing.Size(60, 23);
            this.ccd_bus_send_msg_button.TabIndex = 52;
            this.ccd_bus_send_msg_button.Text = "Send";
            this.ccd_bus_send_msg_button.UseVisualStyleBackColor = true;
            // 
            // ccd_bus_listen_checkbox
            // 
            this.ccd_bus_listen_checkbox.AutoSize = true;
            this.ccd_bus_listen_checkbox.Enabled = false;
            this.ccd_bus_listen_checkbox.Location = new System.Drawing.Point(6, 288);
            this.ccd_bus_listen_checkbox.Name = "ccd_bus_listen_checkbox";
            this.ccd_bus_listen_checkbox.Size = new System.Drawing.Size(67, 17);
            this.ccd_bus_listen_checkbox.TabIndex = 47;
            this.ccd_bus_listen_checkbox.Text = "Disabled";
            this.ccd_bus_listen_checkbox.UseVisualStyleBackColor = true;
            this.ccd_bus_listen_checkbox.Click += new System.EventHandler(this.ccd_bus_enabled_checkbox_Click);
            // 
            // ccd_bus_log_clear_button
            // 
            this.ccd_bus_log_clear_button.Enabled = false;
            this.ccd_bus_log_clear_button.Location = new System.Drawing.Point(153, 284);
            this.ccd_bus_log_clear_button.Name = "ccd_bus_log_clear_button";
            this.ccd_bus_log_clear_button.Size = new System.Drawing.Size(60, 23);
            this.ccd_bus_log_clear_button.TabIndex = 48;
            this.ccd_bus_log_clear_button.Text = "Clear";
            this.ccd_bus_log_clear_button.UseVisualStyleBackColor = true;
            this.ccd_bus_log_clear_button.Click += new System.EventHandler(this.ccd_bus_log_clear_button_Click);
            // 
            // ccd_bus_log_save_button
            // 
            this.ccd_bus_log_save_button.Enabled = false;
            this.ccd_bus_log_save_button.Location = new System.Drawing.Point(87, 284);
            this.ccd_bus_log_save_button.Name = "ccd_bus_log_save_button";
            this.ccd_bus_log_save_button.Size = new System.Drawing.Size(60, 23);
            this.ccd_bus_log_save_button.TabIndex = 47;
            this.ccd_bus_log_save_button.Text = "Save";
            this.ccd_bus_log_save_button.UseVisualStyleBackColor = true;
            // 
            // sci_bus_messages_groupbox
            // 
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_filtering_clear_button);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_msg_richtextbox);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_filtering_apply_button);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_listen_checkbox);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_message_filtering_textbox);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_filtering_checkbox);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_log_clear_button);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_log_save_button);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_send_msg_textbox);
            this.sci_bus_messages_groupbox.Controls.Add(this.sci_bus_send_msg_button);
            this.sci_bus_messages_groupbox.Location = new System.Drawing.Point(609, 27);
            this.sci_bus_messages_groupbox.Name = "sci_bus_messages_groupbox";
            this.sci_bus_messages_groupbox.Size = new System.Drawing.Size(220, 431);
            this.sci_bus_messages_groupbox.TabIndex = 44;
            this.sci_bus_messages_groupbox.TabStop = false;
            this.sci_bus_messages_groupbox.Text = "SCI-bus Messages";
            // 
            // sci_filtering_clear_button
            // 
            this.sci_filtering_clear_button.Enabled = false;
            this.sci_filtering_clear_button.Location = new System.Drawing.Point(72, 402);
            this.sci_filtering_clear_button.Name = "sci_filtering_clear_button";
            this.sci_filtering_clear_button.Size = new System.Drawing.Size(60, 23);
            this.sci_filtering_clear_button.TabIndex = 72;
            this.sci_filtering_clear_button.Text = "Clear";
            this.sci_filtering_clear_button.UseVisualStyleBackColor = true;
            // 
            // sci_bus_msg_richtextbox
            // 
            this.sci_bus_msg_richtextbox.BackColor = System.Drawing.SystemColors.Window;
            this.sci_bus_msg_richtextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sci_bus_msg_richtextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.sci_bus_msg_richtextbox.Location = new System.Drawing.Point(3, 16);
            this.sci_bus_msg_richtextbox.Name = "sci_bus_msg_richtextbox";
            this.sci_bus_msg_richtextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.sci_bus_msg_richtextbox.Size = new System.Drawing.Size(214, 234);
            this.sci_bus_msg_richtextbox.TabIndex = 54;
            this.sci_bus_msg_richtextbox.Text = "";
            this.sci_bus_msg_richtextbox.TextChanged += new System.EventHandler(this.sci_bus_messages_richtextbox_TextChanged);
            // 
            // sci_filtering_apply_button
            // 
            this.sci_filtering_apply_button.Enabled = false;
            this.sci_filtering_apply_button.Location = new System.Drawing.Point(6, 402);
            this.sci_filtering_apply_button.Name = "sci_filtering_apply_button";
            this.sci_filtering_apply_button.Size = new System.Drawing.Size(60, 23);
            this.sci_filtering_apply_button.TabIndex = 71;
            this.sci_filtering_apply_button.Text = "Apply";
            this.sci_filtering_apply_button.UseVisualStyleBackColor = true;
            // 
            // sci_bus_listen_checkbox
            // 
            this.sci_bus_listen_checkbox.AutoSize = true;
            this.sci_bus_listen_checkbox.Enabled = false;
            this.sci_bus_listen_checkbox.Location = new System.Drawing.Point(6, 288);
            this.sci_bus_listen_checkbox.Name = "sci_bus_listen_checkbox";
            this.sci_bus_listen_checkbox.Size = new System.Drawing.Size(67, 17);
            this.sci_bus_listen_checkbox.TabIndex = 49;
            this.sci_bus_listen_checkbox.Text = "Disabled";
            this.sci_bus_listen_checkbox.UseVisualStyleBackColor = true;
            this.sci_bus_listen_checkbox.Click += new System.EventHandler(this.sci_bus_enabled_checkbox_Click);
            // 
            // sci_message_filtering_textbox
            // 
            this.sci_message_filtering_textbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sci_message_filtering_textbox.Location = new System.Drawing.Point(3, 334);
            this.sci_message_filtering_textbox.MaxLength = 0;
            this.sci_message_filtering_textbox.Multiline = true;
            this.sci_message_filtering_textbox.Name = "sci_message_filtering_textbox";
            this.sci_message_filtering_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sci_message_filtering_textbox.Size = new System.Drawing.Size(213, 63);
            this.sci_message_filtering_textbox.TabIndex = 70;
            // 
            // sci_filtering_checkbox
            // 
            this.sci_filtering_checkbox.AutoSize = true;
            this.sci_filtering_checkbox.Enabled = false;
            this.sci_filtering_checkbox.Location = new System.Drawing.Point(6, 311);
            this.sci_filtering_checkbox.Name = "sci_filtering_checkbox";
            this.sci_filtering_checkbox.Size = new System.Drawing.Size(144, 17);
            this.sci_filtering_checkbox.TabIndex = 69;
            this.sci_filtering_checkbox.Text = "SCI-bus message filtering";
            this.sci_filtering_checkbox.UseVisualStyleBackColor = true;
            // 
            // sci_bus_log_clear_button
            // 
            this.sci_bus_log_clear_button.Enabled = false;
            this.sci_bus_log_clear_button.Location = new System.Drawing.Point(153, 284);
            this.sci_bus_log_clear_button.Name = "sci_bus_log_clear_button";
            this.sci_bus_log_clear_button.Size = new System.Drawing.Size(60, 23);
            this.sci_bus_log_clear_button.TabIndex = 51;
            this.sci_bus_log_clear_button.Text = "Clear";
            this.sci_bus_log_clear_button.UseVisualStyleBackColor = true;
            this.sci_bus_log_clear_button.Click += new System.EventHandler(this.sci_bus_log_clear_button_Click);
            // 
            // sci_bus_log_save_button
            // 
            this.sci_bus_log_save_button.Enabled = false;
            this.sci_bus_log_save_button.Location = new System.Drawing.Point(87, 284);
            this.sci_bus_log_save_button.Name = "sci_bus_log_save_button";
            this.sci_bus_log_save_button.Size = new System.Drawing.Size(60, 23);
            this.sci_bus_log_save_button.TabIndex = 50;
            this.sci_bus_log_save_button.Text = "Save";
            this.sci_bus_log_save_button.UseVisualStyleBackColor = true;
            // 
            // command_history_groupbox
            // 
            this.command_history_groupbox.Controls.Add(this.command_history_textbox);
            this.command_history_groupbox.Location = new System.Drawing.Point(12, 348);
            this.command_history_groupbox.Name = "command_history_groupbox";
            this.command_history_groupbox.Size = new System.Drawing.Size(365, 110);
            this.command_history_groupbox.TabIndex = 45;
            this.command_history_groupbox.TabStop = false;
            this.command_history_groupbox.Text = "Command History";
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
            // misc_groupbox
            // 
            this.misc_groupbox.Controls.Add(this.packet_count_tx_label);
            this.misc_groupbox.Controls.Add(this.reboot_scanner_button);
            this.misc_groupbox.Controls.Add(this.packet_count_rx_label);
            this.misc_groupbox.Controls.Add(this.buffer_start_label);
            this.misc_groupbox.Controls.Add(this.buffer_end_label);
            this.misc_groupbox.Controls.Add(this.buffer_readlength_label);
            this.misc_groupbox.Controls.Add(this.buffer_writelength_label);
            this.misc_groupbox.Location = new System.Drawing.Point(162, 464);
            this.misc_groupbox.Name = "misc_groupbox";
            this.misc_groupbox.Size = new System.Drawing.Size(215, 136);
            this.misc_groupbox.TabIndex = 46;
            this.misc_groupbox.TabStop = false;
            this.misc_groupbox.Text = "Misc.";
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
            // reboot_scanner_button
            // 
            this.reboot_scanner_button.Location = new System.Drawing.Point(6, 106);
            this.reboot_scanner_button.Name = "reboot_scanner_button";
            this.reboot_scanner_button.Size = new System.Drawing.Size(132, 23);
            this.reboot_scanner_button.TabIndex = 23;
            this.reboot_scanner_button.Text = "Reboot scanner";
            this.reboot_scanner_button.UseVisualStyleBackColor = true;
            this.reboot_scanner_button.Click += new System.EventHandler(this.reboot_scanner_button_Click);
            // 
            // scanner_connection_groupbox
            // 
            this.scanner_connection_groupbox.Controls.Add(this.exit_button);
            this.scanner_connection_groupbox.Controls.Add(this.disconnect_button);
            this.scanner_connection_groupbox.Controls.Add(this.connect_button);
            this.scanner_connection_groupbox.Controls.Add(this.status_button);
            this.scanner_connection_groupbox.Location = new System.Drawing.Point(12, 464);
            this.scanner_connection_groupbox.Name = "scanner_connection_groupbox";
            this.scanner_connection_groupbox.Size = new System.Drawing.Size(144, 136);
            this.scanner_connection_groupbox.TabIndex = 47;
            this.scanner_connection_groupbox.TabStop = false;
            this.scanner_connection_groupbox.Text = "Scanner Connection";
            // 
            // exit_button
            // 
            this.exit_button.Location = new System.Drawing.Point(6, 106);
            this.exit_button.Name = "exit_button";
            this.exit_button.Size = new System.Drawing.Size(132, 23);
            this.exit_button.TabIndex = 22;
            this.exit_button.Text = "Exit";
            this.exit_button.UseVisualStyleBackColor = true;
            this.exit_button.Click += new System.EventHandler(this.exit_button_Click);
            // 
            // disconnect_button
            // 
            this.disconnect_button.Enabled = false;
            this.disconnect_button.Location = new System.Drawing.Point(6, 48);
            this.disconnect_button.Name = "disconnect_button";
            this.disconnect_button.Size = new System.Drawing.Size(132, 23);
            this.disconnect_button.TabIndex = 2;
            this.disconnect_button.Text = "Disconnect";
            this.disconnect_button.UseVisualStyleBackColor = true;
            this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
            // 
            // eeprom_groupbox
            // 
            this.eeprom_groupbox.Controls.Add(this.write_exteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.save_exteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.label17);
            this.eeprom_groupbox.Controls.Add(this.write_inteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.erase_inteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.save_inteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.label16);
            this.eeprom_groupbox.Controls.Add(this.read_inteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.read_exteeprom_button);
            this.eeprom_groupbox.Controls.Add(this.erase_exteeprom_button);
            this.eeprom_groupbox.Location = new System.Drawing.Point(12, 606);
            this.eeprom_groupbox.Name = "eeprom_groupbox";
            this.eeprom_groupbox.Size = new System.Drawing.Size(365, 82);
            this.eeprom_groupbox.TabIndex = 48;
            this.eeprom_groupbox.TabStop = false;
            this.eeprom_groupbox.Text = "EEPROM";
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
            // pcm_ram_map_groupbox
            // 
            this.pcm_ram_map_groupbox.Controls.Add(this.pcm_ram_values_richtextbox);
            this.pcm_ram_map_groupbox.Location = new System.Drawing.Point(383, 464);
            this.pcm_ram_map_groupbox.Name = "pcm_ram_map_groupbox";
            this.pcm_ram_map_groupbox.Size = new System.Drawing.Size(446, 224);
            this.pcm_ram_map_groupbox.TabIndex = 49;
            this.pcm_ram_map_groupbox.TabStop = false;
            this.pcm_ram_map_groupbox.Text = "PCM/ECU RAM Values";
            // 
            // real_time_diagnostics_groupbox
            // 
            this.real_time_diagnostics_groupbox.Controls.Add(this.real_time_diagnostics_button);
            this.real_time_diagnostics_groupbox.Location = new System.Drawing.Point(835, 27);
            this.real_time_diagnostics_groupbox.Name = "real_time_diagnostics_groupbox";
            this.real_time_diagnostics_groupbox.Size = new System.Drawing.Size(140, 43);
            this.real_time_diagnostics_groupbox.TabIndex = 50;
            this.real_time_diagnostics_groupbox.TabStop = false;
            this.real_time_diagnostics_groupbox.Text = "Real Time Diagnostics";
            // 
            // real_time_diagnostics_button
            // 
            this.real_time_diagnostics_button.Dock = System.Windows.Forms.DockStyle.Top;
            this.real_time_diagnostics_button.Location = new System.Drawing.Point(3, 16);
            this.real_time_diagnostics_button.Name = "real_time_diagnostics_button";
            this.real_time_diagnostics_button.Size = new System.Drawing.Size(134, 23);
            this.real_time_diagnostics_button.TabIndex = 51;
            this.real_time_diagnostics_button.Text = "Open RTD window";
            this.real_time_diagnostics_button.UseVisualStyleBackColor = true;
            this.real_time_diagnostics_button.Click += new System.EventHandler(this.real_time_diagnostics_button_Click);
            // 
            // read_dtcs_groupbox
            // 
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_tcm_button);
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_mic_button);
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_abs_button);
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_acm_button);
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_bcm_button);
            this.read_dtcs_groupbox.Controls.Add(this.read_dtc_pcm_button);
            this.read_dtcs_groupbox.Location = new System.Drawing.Point(835, 76);
            this.read_dtcs_groupbox.Name = "read_dtcs_groupbox";
            this.read_dtcs_groupbox.Size = new System.Drawing.Size(140, 106);
            this.read_dtcs_groupbox.TabIndex = 51;
            this.read_dtcs_groupbox.TabStop = false;
            this.read_dtcs_groupbox.Text = "Read DTCs";
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
            // pcm_ram_area_textbox
            // 
            this.pcm_ram_area_textbox.Location = new System.Drawing.Point(913, 348);
            this.pcm_ram_area_textbox.Name = "pcm_ram_area_textbox";
            this.pcm_ram_area_textbox.Size = new System.Drawing.Size(30, 20);
            this.pcm_ram_area_textbox.TabIndex = 52;
            this.pcm_ram_area_textbox.Text = "F2";
            // 
            // sci_bus_speed_groupbox
            // 
            this.sci_bus_speed_groupbox.Controls.Add(this.sci_bus_hs_button);
            this.sci_bus_speed_groupbox.Controls.Add(this.sci_bus_speed_label);
            this.sci_bus_speed_groupbox.Location = new System.Drawing.Point(835, 188);
            this.sci_bus_speed_groupbox.Name = "sci_bus_speed_groupbox";
            this.sci_bus_speed_groupbox.Size = new System.Drawing.Size(140, 42);
            this.sci_bus_speed_groupbox.TabIndex = 56;
            this.sci_bus_speed_groupbox.TabStop = false;
            this.sci_bus_speed_groupbox.Text = "SCI-bus Speed";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(859, 236);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 57;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(859, 262);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 58;
            // 
            // map_sensor_button
            // 
            this.map_sensor_button.Location = new System.Drawing.Point(168, 606);
            this.map_sensor_button.Name = "map_sensor_button";
            this.map_sensor_button.Size = new System.Drawing.Size(75, 23);
            this.map_sensor_button.TabIndex = 59;
            this.map_sensor_button.Text = "MAP Sensor";
            this.map_sensor_button.UseVisualStyleBackColor = true;
            this.map_sensor_button.Click += new System.EventHandler(this.map_sensor_button_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(231, 632);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 62;
            this.label10.Text = "V";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(166, 632);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 13);
            this.label12.TabIndex = 61;
            this.label12.Text = "MAP:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(199, 629);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(29, 20);
            this.textBox4.TabIndex = 60;
            this.textBox4.Text = "0,00";
            // 
            // pcm_read_all
            // 
            this.pcm_read_all.Location = new System.Drawing.Point(838, 288);
            this.pcm_read_all.Name = "pcm_read_all";
            this.pcm_read_all.Size = new System.Drawing.Size(90, 23);
            this.pcm_read_all.TabIndex = 63;
            this.pcm_read_all.Text = "PCM Read All";
            this.pcm_read_all.UseVisualStyleBackColor = true;
            this.pcm_read_all.Click += new System.EventHandler(this.pcm_read_all_Click);
            // 
            // pcm_parameters_richtextbox
            // 
            this.pcm_parameters_richtextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcm_parameters_richtextbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pcm_parameters_richtextbox.Font = new System.Drawing.Font("Courier New", 9F);
            this.pcm_parameters_richtextbox.Location = new System.Drawing.Point(6, 16);
            this.pcm_parameters_richtextbox.Name = "pcm_parameters_richtextbox";
            this.pcm_parameters_richtextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.pcm_parameters_richtextbox.Size = new System.Drawing.Size(329, 585);
            this.pcm_parameters_richtextbox.TabIndex = 35;
            this.pcm_parameters_richtextbox.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pcm_parameters_richtextbox);
            this.groupBox1.Controls.Add(this.o2_sensor_button);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.map_sensor_button);
            this.groupBox1.Controls.Add(this.convert_o2_button);
            this.groupBox1.Location = new System.Drawing.Point(981, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 663);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sensor Data";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(868, 483);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 64;
            this.button1.Text = "View buffer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // flush_memory_button
            // 
            this.flush_memory_button.Location = new System.Drawing.Point(868, 507);
            this.flush_memory_button.Name = "flush_memory_button";
            this.flush_memory_button.Size = new System.Drawing.Size(91, 23);
            this.flush_memory_button.TabIndex = 65;
            this.flush_memory_button.Text = "Flush Memory";
            this.flush_memory_button.UseVisualStyleBackColor = true;
            this.flush_memory_button.Click += new System.EventHandler(this.flush_memory_button_Click);
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
            this.packetGeneratorToolStripMenuItem.Click += new System.EventHandler(this.packetGeneratorToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 702);
            this.Controls.Add(this.flush_memory_button);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pcm_read_all);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.sci_bus_speed_groupbox);
            this.Controls.Add(this.pcm_ram_area_textbox);
            this.Controls.Add(this.read_dtcs_groupbox);
            this.Controls.Add(this.real_time_diagnostics_groupbox);
            this.Controls.Add(this.pcm_ram_map_groupbox);
            this.Controls.Add(this.eeprom_groupbox);
            this.Controls.Add(this.scanner_connection_groupbox);
            this.Controls.Add(this.misc_groupbox);
            this.Controls.Add(this.command_history_groupbox);
            this.Controls.Add(this.sci_bus_messages_groupbox);
            this.Controls.Add(this.ccd_bus_messages_groupbox);
            this.Controls.Add(this.log_groupbox);
            this.Controls.Add(this.convert_ram_button);
            this.Controls.Add(this.pcm_ram_button);
            this.Controls.Add(this.dump_pcm);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chrysler CCD/SCI Scanner";
            this.log_groupbox.ResumeLayout(false);
            this.log_groupbox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ccd_bus_messages_groupbox.ResumeLayout(false);
            this.ccd_bus_messages_groupbox.PerformLayout();
            this.sci_bus_messages_groupbox.ResumeLayout(false);
            this.sci_bus_messages_groupbox.PerformLayout();
            this.command_history_groupbox.ResumeLayout(false);
            this.command_history_groupbox.PerformLayout();
            this.misc_groupbox.ResumeLayout(false);
            this.misc_groupbox.PerformLayout();
            this.scanner_connection_groupbox.ResumeLayout(false);
            this.eeprom_groupbox.ResumeLayout(false);
            this.eeprom_groupbox.PerformLayout();
            this.pcm_ram_map_groupbox.ResumeLayout(false);
            this.real_time_diagnostics_groupbox.ResumeLayout(false);
            this.read_dtcs_groupbox.ResumeLayout(false);
            this.sci_bus_speed_groupbox.ResumeLayout(false);
            this.sci_bus_speed_groupbox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Label packet_count_rx_label;
        private System.Windows.Forms.Label buffer_writelength_label;
        private System.Windows.Forms.Button dump_pcm;
        private System.Windows.Forms.Label buffer_start_label;
        private System.Windows.Forms.Label buffer_end_label;
        private System.Windows.Forms.Label buffer_readlength_label;
        private System.Windows.Forms.Button read_exteeprom_button;
        private System.Windows.Forms.Button read_inteeprom_button;
        private System.Windows.Forms.Button status_button;
        private System.Windows.Forms.TextBox sci_bus_send_msg_textbox;
        private System.Windows.Forms.Button sci_bus_hs_button;
        private System.Windows.Forms.Label sci_bus_speed_label;
        private System.Windows.Forms.Button erase_exteeprom_button;
        private System.Windows.Forms.Button pcm_ram_button;
        private System.Windows.Forms.RichTextBox pcm_ram_values_richtextbox;
        private System.Windows.Forms.Button o2_sensor_button;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button convert_o2_button;
        private System.Windows.Forms.Button convert_ram_button;
        private System.Windows.Forms.GroupBox log_groupbox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.CheckBox communication_packet_log_enabled_checkbox;
        private System.Windows.Forms.GroupBox ccd_bus_messages_groupbox;
        private System.Windows.Forms.GroupBox sci_bus_messages_groupbox;
        private System.Windows.Forms.Button packet_log_clear_button;
        private System.Windows.Forms.Button packet_log_save_button;
        private System.Windows.Forms.GroupBox command_history_groupbox;
        private System.Windows.Forms.CheckBox ccd_bus_listen_checkbox;
        private System.Windows.Forms.Button ccd_bus_log_clear_button;
        private System.Windows.Forms.Button ccd_bus_log_save_button;
        private System.Windows.Forms.CheckBox sci_bus_listen_checkbox;
        private System.Windows.Forms.Button sci_bus_log_clear_button;
        private System.Windows.Forms.Button sci_bus_log_save_button;
        private System.Windows.Forms.GroupBox misc_groupbox;
        private System.Windows.Forms.GroupBox scanner_connection_groupbox;
        private System.Windows.Forms.Button disconnect_button;
        private System.Windows.Forms.Button exit_button;
        private System.Windows.Forms.Button sci_bus_send_msg_button;
        private System.Windows.Forms.TextBox ccd_bus_send_msg_textbox;
        private System.Windows.Forms.Button ccd_bus_send_msg_button;
        private System.Windows.Forms.GroupBox eeprom_groupbox;
        private System.Windows.Forms.Button erase_inteeprom_button;
        private System.Windows.Forms.Button save_inteeprom_button;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button write_inteeprom_button;
        private System.Windows.Forms.Button write_exteeprom_button;
        private System.Windows.Forms.Button save_exteeprom_button;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox pcm_ram_map_groupbox;
        private System.Windows.Forms.RichTextBox ccd_bus_msg_richtextbox;
        private System.Windows.Forms.RichTextBox sci_bus_msg_richtextbox;
        private System.Windows.Forms.GroupBox real_time_diagnostics_groupbox;
        private System.Windows.Forms.Button real_time_diagnostics_button;
        private System.Windows.Forms.TextBox packet_send_textbox;
        private System.Windows.Forms.Button packet_send_button;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox read_dtcs_groupbox;
        private System.Windows.Forms.Button read_dtc_abs_button;
        private System.Windows.Forms.Button read_dtc_acm_button;
        private System.Windows.Forms.Button read_dtc_bcm_button;
        private System.Windows.Forms.Button read_dtc_pcm_button;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox pcm_ram_area_textbox;
        private System.Windows.Forms.GroupBox sci_bus_speed_groupbox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button read_dtc_mic_button;
        private System.Windows.Forms.Button map_sensor_button;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button pcm_read_all;
        private System.Windows.Forms.RichTextBox pcm_parameters_richtextbox;
        private System.Windows.Forms.Button read_dtc_tcm_button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button reboot_scanner_button;
        private System.Windows.Forms.TextBox packet_log_textbox;
        private System.Windows.Forms.Button ccd_filter_clear_button;
        private System.Windows.Forms.Button ccd_filtering_apply_button;
        private System.Windows.Forms.TextBox ccd_message_filtering_textbox;
        private System.Windows.Forms.CheckBox ccd_filtering_checkbox;
        private System.Windows.Forms.Button sci_filtering_clear_button;
        private System.Windows.Forms.Button sci_filtering_apply_button;
        private System.Windows.Forms.TextBox sci_message_filtering_textbox;
        private System.Windows.Forms.CheckBox sci_filtering_checkbox;
        private System.Windows.Forms.TextBox command_history_textbox;
        private System.Windows.Forms.Button flush_memory_button;
        private System.Windows.Forms.Label packet_count_tx_label;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetGeneratorToolStripMenuItem;
    }
}

