using System;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public interface ISourcePrice 
    {
        void Start();
        void Stop();
        Subscription Subscribe(string instrument, IObserver<InstrumentData> observer );
        void Unsubscribe(Subscription subscription);
    }
}
