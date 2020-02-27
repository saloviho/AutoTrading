using QuotesHistory.Models;

namespace Sandbox.Indicators
{
    public interface IIndicator
    {
        public Lot? OnTick(Candle tick);
    }
}