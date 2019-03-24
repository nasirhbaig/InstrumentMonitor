using System;
using System.Collections.Generic;
using System.Linq;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public class CompositePriceSource : ISourcePrice, IObserver<InstrumentData>
    {
        private List<InstrumentData> _instruments; 
        private readonly object _lock = new object();
        private bool _isRunning;

        private readonly IGeneratePrice _nsdqPriceGenerator;
        private IDisposable _nsdqSubscriber;

        private readonly IGeneratePrice _arcaPriceGenerator;
        private IDisposable _arcaSubscriber;

        private readonly Dictionary<string, List<Subscription>> _subscriptions;

        public CompositePriceSource(IInstrumentFactory instrumentFactory, 
                                    IProvidePriceGenerator priceGeneratorProvider)
        {
            _instruments = instrumentFactory.CreateInstruments().ToList();
            _subscriptions = new Dictionary<string, List<Subscription>>();

            // creating two price generator           
            _arcaPriceGenerator = priceGeneratorProvider.CreatePriceGenerator("ARCA");
            _nsdqPriceGenerator = priceGeneratorProvider.CreatePriceGenerator("NSDQ");
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _arcaPriceGenerator.Start();
                _arcaSubscriber = _arcaPriceGenerator.Subscribe(this);

                _nsdqPriceGenerator.Start();
                _nsdqSubscriber = _nsdqPriceGenerator.Subscribe(this);

                _isRunning = true;
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                    return;

                _nsdqPriceGenerator.Stop();
                _nsdqSubscriber.Dispose();

                _arcaPriceGenerator.Stop();
                _arcaSubscriber.Dispose();

                _isRunning = false;
            }
        }

        public Subscription Subscribe(string ticker, IObserver<InstrumentData> observer)
        {
            //if (_instruments.FirstOrDefault(i => i.Ticker.Equals(ticker)) == null)
            //{
            //    return null;
            //}

            List<Subscription> subscriptions = null;

            if (!_subscriptions.TryGetValue(ticker, out subscriptions))
            {
                subscriptions = new List<Subscription>();
                _subscriptions.Add(ticker, subscriptions);
            }

            var existingSubscription = subscriptions.FirstOrDefault(x => x.Ticker == ticker);

            if (existingSubscription == null)
            {
                var subscription = new Subscription(ticker, this, observer);
                subscriptions.Add(subscription);
                return subscription;
            }
            
            return existingSubscription;
        }

        public void Unsubscribe(Subscription subscription)
        {
            List<Subscription> subscriptions = null;
            if (_subscriptions.TryGetValue(subscription.Ticker, out subscriptions))
            {
                subscriptions.Remove(subscription);
            }
        }

        public void OnNext(InstrumentData value)
        {
            List<Subscription> subscriptions = null;
            if (_subscriptions.TryGetValue(value.Ticker, out subscriptions))
            {
                foreach (var subscription in subscriptions)
                {
                    subscription.Observer.OnNext(value);
                }
            }
        }

        public void OnError(Exception error)
        {
            foreach (var pair in _subscriptions)
            {
                foreach (var subscription in pair.Value)
                {
                    subscription.Observer.OnError(error);
                }
            }
        }

        public void OnCompleted()
        {
            foreach (var pair in _subscriptions)
            {
                foreach (var subscription in pair.Value)
                {
                    subscription.Observer.OnCompleted();
                }
            }
        }
    }
}
