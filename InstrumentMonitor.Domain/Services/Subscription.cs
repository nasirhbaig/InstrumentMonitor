using System;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public class Subscription : IDisposable
    {
        public IObserver<InstrumentData> Observer { get; set; }
        private readonly string _ticker;
        private readonly ISourcePrice _sourcePrice;

        public Subscription(string ticker, ISourcePrice sourcePrice, IObserver<InstrumentData> observer)
        {
            Observer = observer;
            _ticker = ticker;
            _sourcePrice = sourcePrice;
        }

        public string Ticker
        {
            get { return _ticker; }
        }

        public void Dispose()
        {
            _sourcePrice.Unsubscribe(this);
        }
    }
}
