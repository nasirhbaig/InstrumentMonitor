namespace InstrumentMonitor.Domain.Services
{
    public class PriceGeneratorProvider : IProvidePriceGenerator
    {
        private readonly IInstrumentFactory _instrumentFactory;

        public PriceGeneratorProvider(IInstrumentFactory instrumentFactory)
        {
            _instrumentFactory = instrumentFactory;
        }

        public IGeneratePrice CreatePriceGenerator(string name)
        {
            return new PriceGenerator(_instrumentFactory, name);
        }
    }
}
