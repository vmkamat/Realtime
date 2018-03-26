namespace Realtime.Model
{
    public class SymbolPrice
    {
        public SymbolPrice(string symbol, double price)
        {
            Symbol = symbol;
            Price = price;
        }
        public string Symbol { get; }
        public double Price { get; }
    }
}
