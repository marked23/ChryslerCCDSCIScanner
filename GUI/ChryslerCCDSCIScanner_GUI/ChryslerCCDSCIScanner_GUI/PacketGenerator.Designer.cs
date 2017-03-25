namespace ChryslerCCDSCIScanner_GUI
{
    partial class PacketGenerator
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
            this.OkButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SourceLabel = new System.Windows.Forms.Label();
            this.SourceComboBox = new System.Windows.Forms.ComboBox();
            this.TargetComboBox = new System.Windows.Forms.ComboBox();
            this.TargetLabel = new System.Windows.Forms.Label();
            this.CommandComboBox = new System.Windows.Forms.ComboBox();
            this.CommandLabel = new System.Windows.Forms.Label();
            this.PacketPreviewLabel = new System.Windows.Forms.Label();
            this.PacketPreviewTextBox = new System.Windows.Forms.TextBox();
            this.ModifierComboBox = new System.Windows.Forms.ComboBox();
            this.ModifierLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PayloadTextbox = new System.Windows.Forms.TextBox();
            this.DefaultButton = new System.Windows.Forms.Button();
            this.SendCheckBox = new System.Windows.Forms.CheckBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.HintLabel = new System.Windows.Forms.Label();
            this.HintTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(75, 208);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(94, 23);
            this.OkButton.TabIndex = 7;
            this.OkButton.Text = "Paste";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(275, 208);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(94, 23);
            this.CloseButton.TabIndex = 9;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // SourceLabel
            // 
            this.SourceLabel.AutoSize = true;
            this.SourceLabel.Location = new System.Drawing.Point(25, 9);
            this.SourceLabel.Name = "SourceLabel";
            this.SourceLabel.Size = new System.Drawing.Size(44, 13);
            this.SourceLabel.TabIndex = 0;
            this.SourceLabel.Text = "Source:";
            // 
            // SourceComboBox
            // 
            this.SourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SourceComboBox.FormattingEnabled = true;
            this.SourceComboBox.Items.AddRange(new object[] {
            "($00) Laptop",
            "($01) ---NOT USED---",
            "($02) ---NOT USED---",
            "($03) ---NOT USED---"});
            this.SourceComboBox.Location = new System.Drawing.Point(75, 6);
            this.SourceComboBox.MaxDropDownItems = 4;
            this.SourceComboBox.Name = "SourceComboBox";
            this.SourceComboBox.Size = new System.Drawing.Size(118, 21);
            this.SourceComboBox.TabIndex = 0;
            this.SourceComboBox.SelectionChangeCommitted += new System.EventHandler(this.SourceComboBox_SelectionChangeCommitted);
            // 
            // TargetComboBox
            // 
            this.TargetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TargetComboBox.FormattingEnabled = true;
            this.TargetComboBox.Items.AddRange(new object[] {
            "($00) ---NOT USED---",
            "($01) Scanner",
            "($02) CCD-bus",
            "($03) SCI-bus"});
            this.TargetComboBox.Location = new System.Drawing.Point(254, 6);
            this.TargetComboBox.MaxDropDownItems = 4;
            this.TargetComboBox.Name = "TargetComboBox";
            this.TargetComboBox.Size = new System.Drawing.Size(118, 21);
            this.TargetComboBox.TabIndex = 1;
            this.TargetComboBox.SelectionChangeCommitted += new System.EventHandler(this.TargetComboBox_SelectionChangeCommitted);
            // 
            // TargetLabel
            // 
            this.TargetLabel.AutoSize = true;
            this.TargetLabel.Location = new System.Drawing.Point(207, 9);
            this.TargetLabel.Name = "TargetLabel";
            this.TargetLabel.Size = new System.Drawing.Size(41, 13);
            this.TargetLabel.TabIndex = 0;
            this.TargetLabel.Text = "Target:";
            // 
            // CommandComboBox
            // 
            this.CommandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CommandComboBox.FormattingEnabled = true;
            this.CommandComboBox.Items.AddRange(new object[] {
            "($00) Reboot scanner",
            "($01) Handshake request",
            "($02) Scanner status report request",
            "($03) Change scanner settings",
            "($04) General request",
            "($05) General response",
            "($06) Send message to the CCD/SCI-bus",
            "($07) Send message(s) repeatedly to the CCD/SCI-bus",
            "($08) Stop message flow to the CCD/SCI-bus",
            "($09) Message forwarded from the CCD/SCI-bus",
            "($0A) Run self-diagnostics",
            "($0B) Create scanner settings backup",
            "($0C) Restore scanner settings",
            "($0D) Restore default scanner settings",
            "($0E) Debug",
            "($0F) OK/ERROR"});
            this.CommandComboBox.Location = new System.Drawing.Point(75, 33);
            this.CommandComboBox.MaxDropDownItems = 16;
            this.CommandComboBox.Name = "CommandComboBox";
            this.CommandComboBox.Size = new System.Drawing.Size(297, 21);
            this.CommandComboBox.TabIndex = 2;
            this.CommandComboBox.SelectionChangeCommitted += new System.EventHandler(this.CommandComboBox_SelectionChangeCommitted);
            // 
            // CommandLabel
            // 
            this.CommandLabel.AutoSize = true;
            this.CommandLabel.Location = new System.Drawing.Point(12, 36);
            this.CommandLabel.Name = "CommandLabel";
            this.CommandLabel.Size = new System.Drawing.Size(57, 13);
            this.CommandLabel.TabIndex = 0;
            this.CommandLabel.Text = "Command:";
            // 
            // PacketPreviewLabel
            // 
            this.PacketPreviewLabel.AutoSize = true;
            this.PacketPreviewLabel.Location = new System.Drawing.Point(21, 116);
            this.PacketPreviewLabel.Name = "PacketPreviewLabel";
            this.PacketPreviewLabel.Size = new System.Drawing.Size(48, 13);
            this.PacketPreviewLabel.TabIndex = 0;
            this.PacketPreviewLabel.Text = "Preview:";
            // 
            // PacketPreviewTextBox
            // 
            this.PacketPreviewTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PacketPreviewTextBox.Location = new System.Drawing.Point(75, 113);
            this.PacketPreviewTextBox.Name = "PacketPreviewTextBox";
            this.PacketPreviewTextBox.ReadOnly = true;
            this.PacketPreviewTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PacketPreviewTextBox.Size = new System.Drawing.Size(297, 21);
            this.PacketPreviewTextBox.TabIndex = 10;
            // 
            // ModifierComboBox
            // 
            this.ModifierComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierComboBox.FormattingEnabled = true;
            this.ModifierComboBox.Items.AddRange(new object[] {
            "($00) Not available"});
            this.ModifierComboBox.Location = new System.Drawing.Point(75, 60);
            this.ModifierComboBox.MaxDropDownItems = 16;
            this.ModifierComboBox.Name = "ModifierComboBox";
            this.ModifierComboBox.Size = new System.Drawing.Size(297, 21);
            this.ModifierComboBox.TabIndex = 3;
            this.ModifierComboBox.SelectionChangeCommitted += new System.EventHandler(this.ModifierComboBox_SelectionChangeCommitted);
            // 
            // ModifierLabel
            // 
            this.ModifierLabel.AutoSize = true;
            this.ModifierLabel.Location = new System.Drawing.Point(22, 63);
            this.ModifierLabel.Name = "ModifierLabel";
            this.ModifierLabel.Size = new System.Drawing.Size(47, 13);
            this.ModifierLabel.TabIndex = 0;
            this.ModifierLabel.Text = "Modifier:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Payload:";
            // 
            // PayloadTextbox
            // 
            this.PayloadTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PayloadTextbox.Location = new System.Drawing.Point(75, 87);
            this.PayloadTextbox.Name = "PayloadTextbox";
            this.PayloadTextbox.Size = new System.Drawing.Size(216, 21);
            this.PayloadTextbox.TabIndex = 4;
            this.PayloadTextbox.TextChanged += new System.EventHandler(this.PayloadTextbox_TextChanged);
            // 
            // DefaultButton
            // 
            this.DefaultButton.Location = new System.Drawing.Point(175, 208);
            this.DefaultButton.Name = "DefaultButton";
            this.DefaultButton.Size = new System.Drawing.Size(94, 23);
            this.DefaultButton.TabIndex = 8;
            this.DefaultButton.Text = "Default";
            this.DefaultButton.UseVisualStyleBackColor = true;
            this.DefaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // SendCheckBox
            // 
            this.SendCheckBox.AutoSize = true;
            this.SendCheckBox.Location = new System.Drawing.Point(18, 212);
            this.SendCheckBox.Name = "SendCheckBox";
            this.SendCheckBox.Size = new System.Drawing.Size(51, 17);
            this.SendCheckBox.TabIndex = 6;
            this.SendCheckBox.Text = "Send";
            this.SendCheckBox.UseVisualStyleBackColor = true;
            this.SendCheckBox.CheckedChanged += new System.EventHandler(this.SendCheckBox_CheckedChanged);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(297, 85);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 5;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            // 
            // HintLabel
            // 
            this.HintLabel.AutoSize = true;
            this.HintLabel.Location = new System.Drawing.Point(41, 143);
            this.HintLabel.Name = "HintLabel";
            this.HintLabel.Size = new System.Drawing.Size(29, 13);
            this.HintLabel.TabIndex = 11;
            this.HintLabel.Text = "Hint:";
            // 
            // HintTextBox
            // 
            this.HintTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.HintTextBox.Location = new System.Drawing.Point(75, 139);
            this.HintTextBox.Multiline = true;
            this.HintTextBox.Name = "HintTextBox";
            this.HintTextBox.ReadOnly = true;
            this.HintTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.HintTextBox.Size = new System.Drawing.Size(297, 63);
            this.HintTextBox.TabIndex = 12;
            // 
            // PacketGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 242);
            this.Controls.Add(this.HintTextBox);
            this.Controls.Add(this.HintLabel);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.SendCheckBox);
            this.Controls.Add(this.DefaultButton);
            this.Controls.Add(this.PayloadTextbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ModifierComboBox);
            this.Controls.Add(this.ModifierLabel);
            this.Controls.Add(this.PacketPreviewTextBox);
            this.Controls.Add(this.PacketPreviewLabel);
            this.Controls.Add(this.CommandComboBox);
            this.Controls.Add(this.CommandLabel);
            this.Controls.Add(this.TargetComboBox);
            this.Controls.Add(this.TargetLabel);
            this.Controls.Add(this.SourceComboBox);
            this.Controls.Add(this.SourceLabel);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.OkButton);
            this.Name = "PacketGenerator";
            this.Text = "Packet Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label SourceLabel;
        private System.Windows.Forms.ComboBox SourceComboBox;
        private System.Windows.Forms.ComboBox TargetComboBox;
        private System.Windows.Forms.Label TargetLabel;
        private System.Windows.Forms.ComboBox CommandComboBox;
        private System.Windows.Forms.Label CommandLabel;
        private System.Windows.Forms.Label PacketPreviewLabel;
        private System.Windows.Forms.TextBox PacketPreviewTextBox;
        private System.Windows.Forms.ComboBox ModifierComboBox;
        private System.Windows.Forms.Label ModifierLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PayloadTextbox;
        private System.Windows.Forms.Button DefaultButton;
        private System.Windows.Forms.CheckBox SendCheckBox;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label HintLabel;
        private System.Windows.Forms.TextBox HintTextBox;
    }
}