using System;
using System.Threading;
using InstrumentMonitor.Domain.Model;
using InstrumentMonitor.Domain.Services;

namespace InstrumentMonitor
{
    class Program 
    {
        static void Main(string[] args)
        {
            IInstrumentFactory instrumentFactory = new InstrumentsProvider();
            IProvidePriceGenerator priceGeneratorProvider = new PriceGeneratorProvider(instrumentFactory);
            ISourcePrice priceSource = new CompositePriceSource(instrumentFactory, priceGeneratorProvider);

            // start source
            priceSource.Start();

            var priceObserver = new InstrumentPriceObserver();
            priceObserver.OnPriceReceived += OnPriceReceived;
            
            var subscription1 = priceSource.Subscribe("AAPL", priceObserver);
            var subscription2 = priceSource.Subscribe("MSFT", priceObserver);
            var subscription3 = priceSource.Subscribe("GOOG", priceObserver);

            Console.ReadKey();
            subscription1.Dispose();
            subscription2.Dispose();
            subscription3.Dispose();
            priceSource.Stop();
        }

        private static void OnPriceReceived(object sender, InstrumentData instrumentData)
        {
            Console.WriteLine("{0} - {1} at {2} from {3} on {4}", instrumentData.Ticker, instrumentData.CurrentPrice,
                                instrumentData.TimeStamp, instrumentData.SourceName, Thread.CurrentThread.ManagedThreadId);
        }
    }

    
}
