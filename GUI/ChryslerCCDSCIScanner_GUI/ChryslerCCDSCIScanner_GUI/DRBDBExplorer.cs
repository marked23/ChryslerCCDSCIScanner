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
            CreateNodes();
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
            //checkDB();
            //TreeNode[] ModuleListTreeNode = new TreeNode[] { };
            //int pointer = 0;
            
            //for (ushort k = 0x0000; k < 0x2000; k++)
            //{

            //        string temp = db.getModule(k);
            //        if (temp != null)
            //        {
            //            ModuleListTreeNode[pointer] = new TreeNode(temp);
            //            pointer++;
            //        }
                

            //}

            //TreeNode abc = new TreeNode("Module list", ModuleListTreeNode);
            //TreeNode abc = new TreeNode(pointer.ToString());
            //DBTreeView.Nodes.Add(abc);

            TreeNode treeNode1 = new TreeNode("CCD");
            TreeNode treeNode2 = new TreeNode("SCI");
            TreeNode treeNode3 = new TreeNode("J1850");
            TreeNode treeNode4 = new TreeNode("ISO");
            TreeNode treeNode5 = new TreeNode("Multimeter");
            TreeNode treeNode6 = new TreeNode("J2190");
            TreeNode treeNode7 = new TreeNode("Module list", new TreeNode[]
            {
                treeNode1,
                treeNode2,
                treeNode3,
                treeNode4,
                treeNode5,
                treeNode6
            });

            DBTreeView.Nodes.Add(treeNode7);

            DBTreeView.SelectedNode = treeNode7;
            DBTreeView.SelectedNode.Expand();
        }

        private void DRBDBExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.db = null;
            GC.Collect();
        }

        private void DBTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label1.Text = DBTreeView.SelectedNode.ToString();
        }
    }
}
