using System.ComponentModel;

namespace ChryslerCCDSCIScanner_GUI
{
    public class PacketReceptor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Class constructor
        public PacketReceptor()
        {
            // empty
        }

        public bool PacketReceived
        {
            set { SendPropertyChanged("PacketReceived"); }
        }

        private void SendPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
