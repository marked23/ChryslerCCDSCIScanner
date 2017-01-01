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
            this.ok_send_button = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ok_send_button
            // 
            this.ok_send_button.Location = new System.Drawing.Point(548, 357);
            this.ok_send_button.Name = "ok_send_button";
            this.ok_send_button.Size = new System.Drawing.Size(79, 23);
            this.ok_send_button.TabIndex = 0;
            this.ok_send_button.Text = "OK / Send";
            this.ok_send_button.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(458, 215);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(265, 119);
            this.textBox1.TabIndex = 1;
            // 
            // PacketGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 392);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ok_send_button);
            this.Name = "PacketGenerator";
            this.Text = "PacketGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ok_send_button;
        private System.Windows.Forms.TextBox textBox1;
    }
}