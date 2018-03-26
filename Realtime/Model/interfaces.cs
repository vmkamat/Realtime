using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Realtime.Model
{
    public interface IService
    {
        void Start();
        void Stop();
    }

    public interface IPositionService : IService
    {
        IList<Position> GetAllPositions();
        IList<Position> GetPositions(IEnumerable<string> requestedSymbols);
        Position GetPosition(string symbol);
        event EventHandler<Position> PositionChanged;
    }

    public interface IPricingService : IService
    {
        IList<SymbolPrice> GetAllPrices();
        IList<SymbolPrice> GetPrices(IEnumerable<string> requestedSymbols);
        SymbolPrice GetPrice(string symbol);
        event EventHandler<SymbolPrice> PriceChanged;
    }

    public interface ISymbolRepository
    {
        IList<string> GetAllSymbols();
    }

    public interface IValuationService : IService
    {
        Task<IList<Stock>> GetAllStocksAsync();
        Task<IList<Stock>> GetStocksAsync(IEnumerable<string> requestedSymbols);
        Task<Stock> GetStockAsync(string symbol);
        event EventHandler<Stock> ValueChanged;
    }

}
