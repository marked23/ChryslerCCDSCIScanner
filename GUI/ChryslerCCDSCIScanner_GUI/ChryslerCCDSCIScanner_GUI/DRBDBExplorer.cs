/*
 * DRBDBReader
 * Copyright (C) 2016-2017, Kyle Repinski
 * 
 * DRBDBExplorer (derived from DRBDBReader)
 * Copyright (C) 2017, László Dániel
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using DRBDBReader;
using DRBDBReader.DB;
using DRBDBReader.DB.Records;
using DRBDBReader.DB.Converters;

namespace ChryslerCCDSCIScanner_GUI
{
    public partial class DRBDBExplorer : Form
    {
        FileInfo fi = new FileInfo("database.mem");
        Database db;

        public DRBDBExplorer()
        {
            InitializeComponent();
            checkDB();
            if (DBTreeView.Nodes.Count == 0)
            {
                CreateNodes();
            }
        }

        private void checkDB()
        {
            if (this.db == null)
            {
                this.db = new Database(this.fi);
            }
        }

        private void CreateNodes()
        {
            DBTreeView.BeginUpdate();
            TreeNode moduleRoot = new TreeNode("Modules");
            //TreeNode txRoot = new TreeNode("TxRecords");

            for (ushort k = 0x0000; k < 0x2000; k++)
            {

                var moduleRecord = db.GetModuleRecord(k);
                if (moduleRecord != null)
                {
                    string key = $"{moduleRecord.id:X4}";
                    string text = $"{key} - {moduleRecord.scname,-30} - {moduleRecord.name}";
                    moduleRoot.Nodes.Add(key, text);
                }
                


            }

            DBTreeView.Nodes.Add(moduleRoot);
            //DBTreeView.Nodes.Add(txRoot);
            DBTreeView.EndUpdate();
            DBTreeView.SelectedNode = moduleRoot;
            DBTreeView.SelectedNode.Expand();

        }

        private void DRBDBExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.db = null;
            GC.Collect();
        }

        private void DBTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string key = DBTreeView.SelectedNode.Name;
            ushort selectedKey;
            if (ushort.TryParse(key, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture , out selectedKey))
            {
                StringBuilder txDescription = new StringBuilder();
                var moduleRecord = db.GetModuleRecord(selectedKey);
                foreach (var txRecord in moduleRecord.dataelements)
                {

                    var converter =  txRecord.converter;
                    txDescription.AppendLine($"{txRecord.name,-30} {converter.type}");
                    
                    if (converter.type == Converter.Types.BINARY_STATE)
                    {
                        foreach (var entry in ((StateConverter)converter).entries)
                        {
                            txDescription.AppendLine($"   {entry.Key,20} {entry.Value}");
                        }
                    }
                }
                textBox1.Text = txDescription.ToString();


            }
            else
            {
                // Do nothing
            }
            
        }


    }
}
