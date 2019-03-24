using System;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public class InstrumentPriceObserver : IObserver<InstrumentData>
    {
        public event EventHandler<InstrumentData> OnPriceReceived;

        public void OnNext(InstrumentData value)
        {
            var del = OnPriceReceived;
            if (del != null)
            {
                OnPriceReceived(this, value);
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
