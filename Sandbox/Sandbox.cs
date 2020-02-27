using System;
using System.Collections.Generic;
using System.Dynamic;
using QuotesHistory.Models;

namespace Sandbox
{
    public class Sandbox
    {
        private List<Candle> History { get; set; }
        private List<Candle> Real { get; set; }
        public void LoadHistoryData(List<Candle> history) { History = history; }
        public void LoadRealData(List<Candle> real) { Real = real; }

        public void Test(IStrategy strategy)
        {
            strategy.LoadHistory(History);
            foreach (var tick in Real) strategy.OnTick(tick);
        }
    }
}