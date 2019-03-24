using System;

namespace InstrumentMonitor.Domain.Model
{
    public class InstrumentData : IEquatable<InstrumentData>
    {
        public string Ticker { get; set; }
        public double Open { get; set; }
        public double CurrentPrice { get; set; }
        public DateTime TimeStamp { get; set; }
        public string SourceName { get; set; }

        public InstrumentData(){}

        public InstrumentData(string ticker, double open)
        {
            if(string.IsNullOrEmpty(ticker))
                throw new ArgumentException("Invalide ticker. Ticker can't be null or empty.");

            if(open == 0)
                throw new ArgumentException("Open can't be null.");

            Ticker = ticker;
            Open = open;
            CurrentPrice = open;
            TimeStamp = DateTime.Now;
        }

        public bool Equals(InstrumentData other)
        {
            if (other == null)
                return false;

            return Ticker.Equals(other.Ticker);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstrumentData);
        }

        public override int GetHashCode()
        {
            return Ticker.GetHashCode();
        }
    }
}
