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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChryslerCCDSCIScanner_GUI
{
    public partial class AboutForm : Form
    {
        // Link back to the parent form
        MainForm originalForm;

        public AboutForm(MainForm incomingForm)
        {
            this.CenterToParent();
            originalForm = incomingForm;

            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
