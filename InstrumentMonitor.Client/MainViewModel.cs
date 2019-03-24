using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using InstrumentMonitor.Domain.Model;
using InstrumentMonitor.Domain.Services;

namespace InstrumentMonitor.Client
{
    public class MainViewModel
    {
        private readonly ISourcePrice _priceSource;
        private readonly Dictionary<string, Subscription> _subscriptions; 
        public ObservableCollection<Instrument> Instruments { get; private set; }
        private readonly Dictionary<string, Instrument> _instrumentMap;
        private bool _isPriceSourceRunning;

        public MainViewModel()
        {
            Instruments = new ObservableCollection<Instrument>
            {
                new Instrument("AAPL", 175),
                new Instrument("MSFT", 115),
                new Instrument("GOOG", 1200),
                new Instrument("FB", 160)
            };

            _subscriptions = new Dictionary<string, Subscription>();
            _instrumentMap = new Dictionary<string, Instrument>();

            foreach (var instrument in Instruments)
            {
                _instrumentMap.Add(instrument.Ticker, instrument);
            }

            IInstrumentFactory instrumentFactory = new InstrumentsProvider();
            IProvidePriceGenerator priceGeneratorProvider = new PriceGeneratorProvider(instrumentFactory);
            _priceSource = new CompositePriceSource(instrumentFactory, priceGeneratorProvider);

            // commands
            StartPriceSourceCommand = new RelayCommand(StartPriceSource, CanStartPriceSource);
            StopPriceSourceCommand = new RelayCommand(StopPriceSource, CanStopPriceSource);
            SubscribeCommand = new RelayCommand<Instrument>(SubscribePrice, CanSubscribePrice);
            UnsubscribeCommand = new RelayCommand<Instrument>(UnsubscribePrice, CanUnubscribePrice);
            SubscribeAllCommand = new RelayCommand(SubscribeAll, CanSubscribeAll);
            UnsubscribeAllCommand = new RelayCommand(UnsubscribeAll, CanUnsubscribeAll);       
        }     

        public void StartPriceSource()
        {
            if (_isPriceSourceRunning)
                return;

            _priceSource.Start();
            _isPriceSourceRunning = true;

            RaiseCommandExecutes();
        }

        public void StopPriceSource()
        {
            if (_subscriptions.Any())
            {
                foreach (var pair in _subscriptions)
                {
                    pair.Value.Dispose();
                }
                _subscriptions.Clear();
            }

            if (_isPriceSourceRunning)
            {
                _priceSource.Stop();
                _isPriceSourceRunning = false;
            }

            RaiseCommandExecutes();
        }

        private void OnPriceReceived(object sender, InstrumentData instrumentData)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _instrumentMap[instrumentData.Ticker].CurrentPrice = instrumentData.CurrentPrice;
                _instrumentMap[instrumentData.Ticker].Exchange = instrumentData.SourceName;
                _instrumentMap[instrumentData.Ticker].TimeStamp = instrumentData.TimeStamp;
            }));
        }
        
        // Commands

        public RelayCommand StartPriceSourceCommand { get; private set; }

        private bool CanStartPriceSource()
        {
            return !_isPriceSourceRunning;
        }

        public RelayCommand StopPriceSourceCommand { get; private set; }

        private bool CanStopPriceSource()
        {
            return _isPriceSourceRunning;
        }

        public RelayCommand<Instrument> SubscribeCommand { get; private set; }

        private void SubscribePrice(Instrument instrument)
        {
            if (!_subscriptions.ContainsKey(instrument.Ticker))
            {
                Subscribe(instrument.Ticker);
            }
            RaiseCommandExecutes();
        }

        private bool CanSubscribePrice(Instrument instrument)
        {
            if (instrument == null)
                return false;
            
            return _isPriceSourceRunning && !_subscriptions.ContainsKey(instrument.Ticker);
        }

        public RelayCommand<Instrument> UnsubscribeCommand { get; private set; }

        private void UnsubscribePrice(Instrument instrument)
        {
            if (_subscriptions.ContainsKey(instrument.Ticker))
            {
                Unsubscribe(instrument.Ticker);
            }
            RaiseCommandExecutes();
        }

        private bool CanUnubscribePrice(Instrument instrument)
        {
            if (instrument == null)
                return false;
            return _isPriceSourceRunning && _subscriptions.ContainsKey(instrument.Ticker);
        }

        public RelayCommand SubscribeAllCommand { get; private set; }

        private void SubscribeAll()
        {
            foreach (var instrument in Instruments)
            {
                Subscribe(instrument.Ticker);
            }
            RaiseCommandExecutes();
        }

        private bool CanSubscribeAll()
        {
            return _isPriceSourceRunning;
        }

        public RelayCommand UnsubscribeAllCommand { get; private set; }

        private void UnsubscribeAll()
        {
            foreach (var pair in _subscriptions)
            {
                pair.Value.Dispose();
            }
            _subscriptions.Clear();
            RaiseCommandExecutes();
        }

        private bool CanUnsubscribeAll()
        {
            return _isPriceSourceRunning;
        }

        private void Subscribe(string ticker)
        {
            if (!_subscriptions.ContainsKey(ticker))
            {
                var observer = new InstrumentPriceObserver();
                observer.OnPriceReceived += OnPriceReceived;
                var subscription = _priceSource.Subscribe(ticker, observer);
                _subscriptions.Add(ticker, subscription);
            }
        }

        private void Unsubscribe(string ticker)
        {
            if (_subscriptions.ContainsKey(ticker))
            {
                _subscriptions[ticker].Dispose();
                _subscriptions.Remove(ticker);
            }
        }

        private void RaiseCommandExecutes()
        {
            StopPriceSourceCommand.RaiseCanExecuteChanged();
            StartPriceSourceCommand.RaiseCanExecuteChanged();
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            SubscribeAllCommand.RaiseCanExecuteChanged();
            UnsubscribeAllCommand.RaiseCanExecuteChanged();
        }
    }
}
