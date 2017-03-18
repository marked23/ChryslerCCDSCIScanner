using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ChryslerCCDSCIScanner_GUI
{
    public partial class MainForm : Form
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        public struct DevBroadcastVolume
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public int Mask;
            public Int16 Flags;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            command_history_textbox.AppendText("New Device Arrived" + Environment.NewLine);

                            // Scroll down
                            command_history_textbox.SelectionStart = command_history_textbox.TextLength;
                            command_history_textbox.ScrollToCaret();

                            int devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DevBroadcastVolume vol;
                                vol = (DevBroadcastVolume)
                                   Marshal.PtrToStructure(m.LParam,
                                   typeof(DevBroadcastVolume));
                                command_history_textbox.AppendText("Mask is " + vol.Mask + Environment.NewLine);

                                // Scroll down
                                command_history_textbox.SelectionStart = command_history_textbox.TextLength;
                                command_history_textbox.ScrollToCaret();
                            }

                            break;

                        case DBT_DEVICEREMOVECOMPLETE:
                            command_history_textbox.AppendText("Device Removed" + Environment.NewLine);

                            // Scroll down
                            command_history_textbox.SelectionStart = command_history_textbox.TextLength;
                            command_history_textbox.ScrollToCaret();
                            break;
                    }
                    break;
            }
        }
    }
}
