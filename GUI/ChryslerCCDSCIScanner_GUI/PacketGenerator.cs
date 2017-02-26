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
    public partial class PacketGenerator : Form
    {
        MainForm originalForm;

        byte source, target, command, modifier, checksum; // source + target + command = data code; modifier = sub-data code
        int calculated_length;
        int calculated_checksum;
        int PacketLength;
        byte[] length;
        byte[] payload;

        byte[] PacketPreview;
        bool SendPacket;

        public PacketGenerator(MainForm incomingForm)
        {
            this.CenterToParent();
            originalForm = incomingForm;

            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            DefaultComponentState();
            UpdatePacketPreview();
        }

        private void DefaultComponentState()
        {
            SourceComboBox.SelectedIndex = 0;
            TargetComboBox.SelectedIndex = 1;
            CommandComboBox.SelectedIndex = 1;
            ModifierComboBox.SelectedIndex = 0;
            PayloadTextbox.Clear();
            UpdatePacketPreview();
            SourceComboBox.Focus();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (SendPacket)
            {
                originalForm.WritePacketTextbox("TX", "CUSTOM PACKET", PacketPreview);
                originalForm.WriteSerialData(PacketPreview);
            }
            else
            {
                originalForm.WriteSendPacketTextbox(PacketPreview);
            }
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            DefaultComponentState();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SourceComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void TargetComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void CommandComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void ModifierComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void PayloadTextbox_TextChanged(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void SendCheckBox_Click(object sender, EventArgs e)
        {
            SendPacket = SendCheckBox.Checked;
        }

        private void UpdatePacketPreview()
        {
            source = (byte)SourceComboBox.SelectedIndex;
            target = (byte)TargetComboBox.SelectedIndex;
            command = (byte)CommandComboBox.SelectedIndex;
            modifier = (byte)ModifierComboBox.SelectedIndex;

            try
            {
                payload = Util.GetBytes(PayloadTextbox.Text);
            }
            catch
            {
                payload = null;
            }

            if (payload != null)
            {
                calculated_length = 2 + payload.Length;
            }
            else
            {
                calculated_length = 2;
            }

            length = new byte[2];

            length[0] = (byte)((calculated_length >> 8) & 0xFF);
            length[1] = (byte)((calculated_length) & 0xFF);

            PacketLength = calculated_length + 4;

            PacketPreview = new byte[PacketLength];
            PacketPreview[0] = PacketManager.SYNC_BYTE; // 0x33
            PacketPreview[1] = length[0];
            PacketPreview[2] = length[1];
            PacketPreview[3] = (byte)((source << 6) | (target << 4) | command);
            PacketPreview[4] = modifier;

            if (payload != null)
            {
                for (int i = 0; i < payload.Length; i++)
                {
                    PacketPreview[5 + i] = payload[i];
                }
            }

            calculated_checksum = 0;

            for (int j = 1; j < PacketLength - 1; j++)
            {
                calculated_checksum += PacketPreview[j];
            }

            checksum = (byte)(calculated_checksum & 0xFF);

            PacketPreview[PacketLength - 1] = checksum;

            StringBuilder newstuff = new StringBuilder();

            foreach (byte bytes in PacketPreview)
            {
                newstuff.Append(Convert.ToString(bytes, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper());
            }

            PacketPreviewTextBox.Text = newstuff.ToString();
        }

    }
}
