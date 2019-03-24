using System;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public interface IGeneratePrice : IObservable<InstrumentData>
    {
        void Start();
        void Stop();
    }
}
