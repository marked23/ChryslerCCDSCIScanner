using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChryslerCCDSCIScanner
{
    public partial class AboutForm : Form
    {
#pragma warning disable CS0414 // The field 'AboutForm.about' is assigned but its value is never used
        private AboutForm about;
#pragma warning restore CS0414 // The field 'AboutForm.about' is assigned but its value is never used

        public AboutForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            about = null;
        }
    }
}
