using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public class PriceGenerator : IGeneratePrice
    {
        private readonly List<IObserver<InstrumentData>> _observers;
        private readonly Timer _timer;
        private readonly Random _random;
        private readonly string _sourceName;

        private readonly List<InstrumentData> _instruments; 

        public PriceGenerator(IInstrumentFactory instrumentFactory, string name)
        {
            _sourceName = name;
            _instruments = instrumentFactory.CreateInstruments().ToList();            
            _observers = new List<IObserver<InstrumentData>>();         
            _timer = new Timer(PublishPrices, null, -1, -1);
            _random = new Random();
        }

        public void Start()
        {
            if (IsRunning)
                return;
            _timer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100));
            IsRunning = true;
        }

        public void Stop()
        {           
            _timer.Change(-1, -1);
            IsRunning = false;

            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public bool IsRunning { get; private set; }

        private void PublishPrices(object state)
        {
            Parallel.ForEach(_instruments, (instrument) =>
            {
                var max = instrument.Open * 1.025;
                var min = instrument.Open * 0.975;

                instrument.CurrentPrice = _random.NextDouble() * (max - min) + min;
                instrument.TimeStamp = DateTime.Now;
                instrument.SourceName = _sourceName;

                //var data = new InstrumentData
                //{
                //    Ticker = instrument.Ticker,
                //    SourceName = _sourceName,
                //    Open = instrument.Open,
                //    TimeStamp = DateTime.Now,
                //    CurrentPrice = _random.NextDouble() * (max - min) + min
                //};

                foreach (var observer in _observers)
                {
                    observer.OnNext(instrument);
                    //observer.OnNext(data);
                }
            });
        }

        public IDisposable Subscribe(IObserver<InstrumentData> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<InstrumentData>> _observers;
            private readonly IObserver<InstrumentData> _observer;

            public Unsubscriber(List<IObserver<InstrumentData>> observers, IObserver<InstrumentData> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }

    //public struct PriceInfo
    //{
    //    public string Ticker { get; set; }
    //    public double CurrentPrice { get; set; }
    //}
}
