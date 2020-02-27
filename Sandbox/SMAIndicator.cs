using System;
using System.Collections.Generic;
using System.Linq;
using QuotesHistory.Models;

namespace Sandbox.Indicators
{
    public class SMAIndicator : IIndicator
    {
        public int SlowPeriod { get; set; }
        public int FastPeriod { get; set; }
        public decimal SlowSma { get; set; }
        public decimal FastSma { get; set; }
        public List<Candle> History { get; set; }
        public decimal TakeProfit { get; set; } 
        
        public SMAIndicator(int slowPeriod, int fastPeriod, decimal takeProfit)
        {
            SlowPeriod = slowPeriod;
            FastPeriod = fastPeriod;
            TakeProfit = takeProfit;
        }

        decimal CalculateSMA(List<Candle> history, int n)
        {
            if (history.Count - n < 0) throw new Exception("Not enough data");
            return history.Skip(history.Count - n).Aggregate(0M, (sum, item) => sum + item.CloseValue) / n;
        }
        
        decimal LastMin(List<Candle> history)
        {
            if (history.Count < 2) throw new Exception("Not enough data");
            
            var min = history[history.Count - 2].CloseValue;
            for (int i = history.Count - 2; i >= 0; i--)
            {
                if (history[i].CloseValue < min) min = history[i].CloseValue;
                if (history[i].CloseValue > min) return min;
            }

            return min;
        }
        
        decimal LastMax(List<Candle> history)
        {
            if (history.Count < 2) throw new Exception("Not enough data");
            
            var max = history[history.Count - 2].CloseValue;
            for (int i = history.Count - 2; i >= 0; i--)
            {
                if (history[i].CloseValue > max) max = history[i].CloseValue;
                if (history[i].CloseValue < max) return max;
            }

            return max;
        }

        public void LoadHistory(List<Candle> history)
        {
            History = history;
            SlowSma = CalculateSMA(History, SlowPeriod);
            FastSma = CalculateSMA(History, FastPeriod);
        }

        public Lot? OnTick(Candle tick)
        {
            History.Add(tick);
            decimal test1 = FastSma - SlowSma;
            
            SlowSma = SlowSma - History[^SlowPeriod].CloseValue / SlowPeriod + tick.CloseValue / SlowPeriod;
            FastSma = FastSma - History[^FastPeriod].CloseValue / FastPeriod + tick.CloseValue / FastPeriod;
            
            decimal test2 = FastSma - SlowSma;

            if (test1 * test2 < 0)
            {
                if (test1 > 0)
                {
                    var stopLoss = LastMin(History);
                    return new SellLot(tick.CloseValue, stopLoss, tick.CloseValue - TakeProfit, 10);
                }
                else
                {
                    var stopLoss = LastMax(History);
                    return new BuyLot(tick.CloseValue, stopLoss, tick.CloseValue + TakeProfit, 10);
                }
            }

            return null;
        }
    }
}