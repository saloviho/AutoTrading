namespace Sandbox
{
    public class BuyLot : Lot
    {
        
        public BuyLot(decimal price, decimal stopLoss, decimal takeProfit, decimal volume) : base(price, stopLoss, takeProfit, volume)
        {
        }
        public override int ShouldClose(decimal currentPrice)
        {
            if (currentPrice >= TakeProfit) return 1;
            if (currentPrice <= StopLoss) return -1;
            return 0;
        }
        
        public override string ToString()
        {
            return $"[BUY] Price: {Price}, TakeProfit: {TakeProfit}, StopLoss: {StopLoss}, Volume: {Volume}";
        }
    }
}