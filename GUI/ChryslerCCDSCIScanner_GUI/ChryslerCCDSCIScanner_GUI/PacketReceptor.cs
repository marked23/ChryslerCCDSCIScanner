using System.ComponentModel;

namespace ChryslerCCDSCIScanner_GUI
{
    public class PacketReceptor : INotifyPropertyChanged
    {
        private bool _datareceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool PacketReceived
        {
            get { return _datareceived; }
            set
            {
                if (_datareceived != value)
                {
                    _datareceived = value;
                    SendPropertyChanged("PacketReceived");
                }
            }
        }

        private void SendPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public PacketReceptor()
        {
        }
    }
}
