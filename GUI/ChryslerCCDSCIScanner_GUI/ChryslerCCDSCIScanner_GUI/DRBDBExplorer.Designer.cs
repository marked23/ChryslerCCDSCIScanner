namespace ChryslerCCDSCIScanner_GUI
{
    partial class DRBDBExplorer
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
            this.DBTreeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DBTreeView
            // 
            this.DBTreeView.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DBTreeView.Location = new System.Drawing.Point(12, 12);
            this.DBTreeView.Name = "DBTreeView";
            this.DBTreeView.Size = new System.Drawing.Size(450, 331);
            this.DBTreeView.TabIndex = 0;
            this.DBTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DBTreeView_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 378);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "0";
            // 
            // DRBDBExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 521);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DBTreeView);
            this.Name = "DRBDBExplorer";
            this.Text = "DRB-III Database Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DRBDBExplorer_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView DBTreeView;
        private System.Windows.Forms.Label label1;
    }
}