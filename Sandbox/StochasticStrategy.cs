using System;
using System.Collections.Generic;
using System.Linq;
using QuotesHistory.Models;

namespace Sandbox
{
    public class StochasticStrategy : IStrategy
    {
        public decimal Stochastic = 50;
        public decimal Money = 5000;
        public List<Candle> History { get; set; }
        public List<Lot> Lots { get; set; }
        public int Period { get; set; }

        public StochasticStrategy(int period)
        {
            Period = period;
            Lots = new List<Lot>();
        }

        void CalculateStochastic(decimal currentPrice)
        {
            var min = History.Take(History.Count - Period).Select(candle => candle.CloseValue).Min();
            var max = History.Skip(History.Count - Period).Select(candle => candle.CloseValue).Max();
            if(min != max) Stochastic = (currentPrice - min) / (max - min) * 100;
        }
        
        public void LoadHistory(List<Candle> history)
        {
            History = history;
            CalculateStochastic(history.Last().CloseValue);
        }
        
        public void OnTick(Candle tick)
        {
            List<Lot> tmp = new List<Lot>();
            foreach (var lot in Lots)
            {
                var check = lot.ShouldClose(tick.CloseValue); 
                if (check != 0)
                {
                    var reason = check == 1 ? "Profit" : "Loss";
                    var change = 0M;
                    
                    if (lot is SellLot) change = lot.Price - tick.CloseValue;
                    else change = tick.CloseValue - lot.Price;

                    change = change * lot.Volume;
                    Money += change;
                    Console.WriteLine($"[LOT CLOSED] Reason: {reason}, balance change: {change}, current amount: {Money}");
                }
                else tmp.Add(lot);
            }

            Lots = tmp;
            
            History.Add(tick);

            var prevSt = Stochastic;
            CalculateStochastic(tick.CloseValue);
            
            if(Stochastic < 20 && prevSt > 20) 
                Lots.Add(new BuyLot(tick.CloseValue, tick.CloseValue - 0.1M, tick.CloseValue + 0.1M, 100));
            if (Stochastic > 80 && prevSt < 80)
                Lots.Add(new SellLot(tick.CloseValue, tick.CloseValue + 0.1M, tick.CloseValue - 0.1M, 100));
        }
    }
}