namespace Sandbox
{
    public abstract class Lot
    {
        protected Lot(decimal price, decimal stopLoss, decimal takeProfit, decimal volume)
        {
            Price = price;
            StopLoss = stopLoss;
            TakeProfit = takeProfit;
            Volume = volume;
        }
        public decimal Price { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal Volume { get; set; }
        public abstract int ShouldClose(decimal currentPrice);
    }
}