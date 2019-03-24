using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InstrumentMonitor.Client.Annotations;

namespace InstrumentMonitor.Client
{
    public class Instrument : INotifyPropertyChanged
    {
        public string Ticker { get; private set; }
        public double Open { get; set; }

        private double _currentPrice;

        public double CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                _currentPrice = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentPrice"));
            }
        }

        private string _exchange;

        public string Exchange
        {
            get { return _exchange; }
            set
            {
                _exchange = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Exchange"));
            }
        }

        private DateTime _timeStamp;

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TimeStamp"));
            }
        }

        public Instrument(string ticker, double open)
        {
            Ticker = ticker;
            Open = open;
            CurrentPrice = open;
        }
        
        
        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
