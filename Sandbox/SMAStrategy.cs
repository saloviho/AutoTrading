using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using QuotesHistory.Models;
using Sandbox;

namespace Sandbox
{
    public class SmaStrategy : IStrategy
    {
        public int SlowPeriod { get; set; }
        public int FastPeriod { get; set; }
        public decimal SlowSma { get; set; }
        public decimal FastSma { get; set; }
        
        public List<Candle> History { get; set; }
        public List<Lot> Lots { get; set; }
        
        public decimal Money = 5000;
        public decimal TakeProfit;
        public SmaStrategy(int slowPeriod, int fastPeriod, decimal takeProfit)
        {
            SlowPeriod = slowPeriod;
            FastPeriod = fastPeriod;
            TakeProfit = takeProfit;
            Lots = new List<Lot>();
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
            decimal test1 = FastSma - SlowSma;
            
            SlowSma = SlowSma - History[^SlowPeriod].CloseValue / SlowPeriod + tick.CloseValue / SlowPeriod;
            FastSma = FastSma - History[^FastPeriod].CloseValue / FastPeriod + tick.CloseValue / FastPeriod;
            
            decimal test2 = FastSma - SlowSma;

            if (test1 * test2 < 0)
            {
                if (test1 > 0)
                {
                    var sl = Math.Min(LastMax(History), tick.CloseValue + TakeProfit * 5);
                    var l = new SellLot(tick.CloseValue, sl, tick.CloseValue - TakeProfit, 1);
                    Console.WriteLine(l);
                    Lots.Add(l);
                }
                else
                {
                    var sl = Math.Max(LastMin(History), tick.CloseValue - TakeProfit * 5);
                    var l = new BuyLot(tick.CloseValue, sl, tick.CloseValue + TakeProfit, 1);
                    Console.WriteLine(l);
                    Lots.Add(l);
                }
            }
        }
    }
}