using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentMonitor.Domain.Model;

namespace InstrumentMonitor.Domain.Services
{
    public interface IInstrumentFactory
    {
        IEnumerable<InstrumentData> CreateInstruments();
        InstrumentData GetInstrument(string ticker);
    }

    public class InstrumentsProvider : IInstrumentFactory
    {
        private readonly List<InstrumentData> _instruments;

        public InstrumentsProvider()
        {
            _instruments = new List<InstrumentData>
            {
                new InstrumentData("AAPL", 175),
                new InstrumentData("MSFT", 115),
                new InstrumentData("GOOG", 1200),
                new InstrumentData("FB", 160)
            };;
        }

        public InstrumentData GetInstrument(string ticker)
        {
            return _instruments.FirstOrDefault(i => i.Ticker == ticker);
        }
        
        public IEnumerable<InstrumentData> CreateInstruments()
        {
            return _instruments;
        }
    }
}
