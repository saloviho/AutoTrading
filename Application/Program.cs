using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QuotesHistory;
using QuotesHistory.Models;
using Sandbox;
using TestApp;

namespace Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO use autofac
            var historyProvider = new FinamQuotesHistoryProvider(
                new FinamTicksParser(), 
                new FinamCandlesParser(),
                new FinamQuotesHistoryClient(
                    new HistoryConfiguration()));
            
            /*
            var ticksHistory = await historyProvider.GetTicksHistory(
                "GAZP",
                new DateTime(2020, 2, 20, 0, 0, 0),
                new DateTime(2020, 2, 21, 0, 0, 0),
                CancellationToken.None);
            */
            var candlesHistory = await historyProvider.GetCandlesHistory(
                "SBER",
                TimeInterval.Minutes1,
                new DateTime(2019, 2, 25, 0, 0, 0),
                new DateTime(2020, 2, 25, 0, 0, 0),
                CancellationToken.None);

            //var SMA = new SmaStrategy(60, 5, 0.1M);
            var ST = new StochasticStrategy(60);
            var sandbox = new Sandbox.Sandbox();
            sandbox.LoadHistoryData(candlesHistory.Candles.Take(500).ToList());
            sandbox.LoadRealData(candlesHistory.Candles.Skip(500).ToList());
            sandbox.Test(ST);
            
            Console.WriteLine("===========================");
        }
    }
}