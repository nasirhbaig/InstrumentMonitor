namespace InstrumentMonitor.Domain.Services
{
    public interface IProvidePriceGenerator
    {
        IGeneratePrice CreatePriceGenerator(string name);
    }
}
