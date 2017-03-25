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

        // Class constructor
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
            PayloadTextbox.Enabled = false;
            BrowseButton.Enabled = false;
            UpdatePacketPreview();
            SourceComboBox.Focus();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (SendCheckBox.Checked)
            {
                originalForm.WritePacketTextBox("TX", "CUSTOM PACKET", PacketPreview);
                originalForm.WriteSerialData(PacketPreview);
            }
            else
            {
                originalForm.WriteSendPacketTextBox(PacketPreview);
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
            TargetComboBox.Items.Clear();
            TargetComboBox.Items.AddRange(new object[]
            {
                "($00) ---NOT USED---",
                "($01) Scanner",
                "($02) CCD - bus",
                "($03) SCI - bus"
            });
            TargetComboBox.SelectedIndex = 1;

            if (TargetComboBox.SelectedIndex == 0x00) UpdatePacketPreview();
        }

        private void TargetComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (TargetComboBox.SelectedIndex != 0x00) UpdatePacketPreview();
        }

        private void CommandComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (CommandComboBox.SelectedIndex)
            {
                case 0x00:
                    {
                        ModifierComboBox.Items.Clear(); // Always clear first then add new items
                        ModifierComboBox.Items.AddRange(new object[] 
                        {
                            "($00) Not available"
                        });
                        ModifierComboBox.SelectedIndex = 0;
                        PayloadTextbox.Enabled = false;
                        BrowseButton.Enabled = false;
                        break;
                    }
                case 0x01:
                    {
                        ModifierComboBox.Items.Clear();
                        ModifierComboBox.Items.AddRange(new object[]
                        {
                            "($00) Not available"
                        });
                        ModifierComboBox.SelectedIndex = 0;
                        PayloadTextbox.Enabled = false;
                        BrowseButton.Enabled = false;
                        break;
                    }
                case 0x02:
                    {
                        ModifierComboBox.Items.Clear();
                        ModifierComboBox.Items.AddRange(new object[]
                        {
                            "($00) Not available"
                        });
                        ModifierComboBox.SelectedIndex = 0;
                        PayloadTextbox.Enabled = false;
                        BrowseButton.Enabled = false;
                        break;
                    }
                case 0x03:
                    {
                        ModifierComboBox.Items.Clear();
                        ModifierComboBox.Items.AddRange(new object[]
                        {
                            "($00) ---NOT USED---",
                            "($01) Read scanner settings from EEPROM",
                            "($02) Write scanner settings to EEPROM",
                            "($03) Enable CCD-bus communication",
                            "($04) Disable CCD-bus communication",
                            "($05) Enable SCI-bus communication",
                            "($06) Disable SCI-bus communication",
                            "($07) Enable LCD backlight",
                            "($08) Enable LCD backlight dimming",
                            "($09) Disable LCD backlight",
                            "($0A) Use metric units",
                            "($0B) Use imperial units",
                            "($0C) Enable external EEPROM write protection",
                            "($0D) Disable external EEPROM write protection",
                            "($0E) Set CCD-bus interframe response delay",
                            "($0F) Set SCI-bus interframe response delay",
                            "($10) Set SCI-bus intermessage response delay",
                            "($11) Set SCI-bus intermessage request delay",
                            "($12) Set PACKET intermessage response delay",
                            "($13) Enable SCI-bus high speed mode",
                            "($14) Disable SCI-bus high speed mode",
                            "($15) Enable CCD-bus message filtering",
                            "($16) Disable CCD-bus message filtering",
                            "($17) Enable SCI-bus message filtering",
                            "($18) Disable SCI-bus message filtering",
                            "($19) Set SCI-bus target",
                            "($1A) Enable buzzer",
                            "($1B) Disable buzzer",
                            "($1C) Enable button hold down sensing",
                            "($1D) Disable button hold down sensing",
                            "($1E) Enable ACT LED",
                            "($1F) Disable ACT LED"
                        });
                        ModifierComboBox.SelectedIndex = 1;
                        PayloadTextbox.Enabled = true;
                        BrowseButton.Enabled = true;
                        break;
                    }
                case 0x04:
                    {
                        ModifierComboBox.Items.Clear();
                        ModifierComboBox.Items.AddRange(new object[]
                        {
                            "($00) ---NOT USED---",
                            "($01) Scanner firmware version",
                            "($02) Read internal EEPROM in chunks",
                            "($03) Read external EEPROM in chunks",
                            "($04) Write internal EEPROM in chunks",
                            "($05) Write external EEPROM in chunks",
                            "($06) Scan CCD/SCI-bus modules",
                            "($07) Free RAM available",
                            "($08) MCU counter value"
                        });
                        ModifierComboBox.SelectedIndex = 1;
                        PayloadTextbox.Enabled = true;
                        BrowseButton.Enabled = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            UpdatePacketPreview();
        }

        private void SendCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SendCheckBox.Checked)
            {
                OkButton.Text = "OK";
            }
            else
            {
                OkButton.Text = "Paste";
            }
        }

        private void ModifierComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePacketPreview();
        }

        private void PayloadTextbox_TextChanged(object sender, EventArgs e)
        {
            UpdatePacketPreview();
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

            // Drop hints
            switch (command)
            {
                case 0x00:
                    {
                        HintTextBox.Text = "The scanner will reboot.";
                        break;
                    }
                case 0x01:
                    {
                        HintTextBox.Text = "A handshake request packet will be sent to any USB-connected device to discover if a scanner had been connected.";
                        break;
                    }
                case 0x02:
                    {
                        HintTextBox.Text = "Request a status report packet from the scanner.";
                        break;
                    }
                case 0x03:
                    {
                        switch (modifier)
                        {
                            case 0x00:
                                {
                                    HintTextBox.Text = "This modifier is not is use.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x01:
                                {
                                    HintTextBox.Text = "Save the scanner's internal EEPROM content into a file (scanner_inteeprom_yyyymmddhhmmss.bin).";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x02:
                                {
                                    HintTextBox.Text = "Write the content of the Payload textbox to the scanner's internal EEPROM. You can also load the payload from a .bin file.";
                                    PayloadTextbox.Enabled = true;
                                    BrowseButton.Enabled = true;
                                    break;
                                }
                            case 0x03:
                                {
                                    HintTextBox.Text = "Enable CCD-bus message collection.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x04:
                                {
                                    HintTextBox.Text = "Disable CCD-bus message collection.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x05:
                                {
                                    HintTextBox.Text = "Enable SCI-bus message collection.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x06:
                                {
                                    HintTextBox.Text = "Disable SCI-bus message collection.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x07:
                                {
                                    HintTextBox.Text = "Turn on LCD backlight (100%).";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x08:
                                {
                                    HintTextBox.Text = "Turn on LCD backlight to a desired brightness level. Payload contains the brightness level $00 being fully OFF and $64 fully ON.";
                                    PayloadTextbox.Enabled = true;
                                    BrowseButton.Enabled = true;
                                    break;
                                }
                            case 0x09:
                                {
                                    HintTextBox.Text = "Turn off LCD backlight (0%).";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            default:
                                {
                                    HintTextBox.Text = "N/A";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                        }
                        break;
                    }
                case 0x04:
                    {
                        switch (modifier)
                        {
                            case 0x00:
                                {
                                    HintTextBox.Text = "This modifier is not is use.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x01:
                                {
                                    HintTextBox.Text = "Get scanner firmware version.";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x02:
                                {
                                    HintTextBox.Text = "Read scanner's internal EEPROM in chunks into a file (scanner_inteeprom_addr_aaaa-bbbb_yyyymmddhhmmss.bin";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x03:
                                {
                                    HintTextBox.Text = "Read scanner's external EEPROM in chunks into a file (scanner_exteeprom_addr_aaaaa-bbbbb_yyyymmddhhmmss.bin";
                                    PayloadTextbox.Enabled = false;
                                    BrowseButton.Enabled = false;
                                    break;
                                }
                            case 0x04:
                                {
                                    HintTextBox.Text = "Write the content of the Payload textbox to the scanner's internal EEPROM. The first 2 bytes have to be the starting address. You can also load the payload from a .bin file.";
                                    PayloadTextbox.Enabled = true;
                                    BrowseButton.Enabled = true;
                                    break;
                                }
                            case 0x05:
                                {
                                    HintTextBox.Text = "Write the content of the Payload textbox to the scanner's external EEPROM. The first 3 bytes have to be the starting address. You can also load the payload from a .bin file.";
                                    PayloadTextbox.Enabled = true;
                                    BrowseButton.Enabled = true;
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
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
