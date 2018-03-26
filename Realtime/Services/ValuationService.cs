using Realtime.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Realtime.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IPositionService _positionSvc;
        private readonly IPricingService _pricingSvc;

        private readonly ConcurrentDictionary<string, Stock> _htStocks;
        private bool _isRunning;

        public event EventHandler<Stock> ValueChanged;

        public ValuationService(IPositionService positionSvc, IPricingService pxSvc)
        {
            _positionSvc = positionSvc;
            _pricingSvc = pxSvc;
            _htStocks = new ConcurrentDictionary<string, Stock>();
        }

        public void Start()
        {
            if (_isRunning) return;

            var positions = _positionSvc.GetAllPositions();

            foreach (var position in positions)
            {
                _htStocks[position.Symbol] = new Stock(position.Symbol, position.Qty, 0.0);
            }

            var prices = _pricingSvc.GetAllPrices();

            foreach (var price in prices)
            {
                var stock = _htStocks[price.Symbol];
                stock.Price = price.Price;
            }

            _positionSvc.PositionChanged += (s, p) => UpdatePositionChange(p);
            _pricingSvc.PriceChanged += (s, p) => UpdatePriceChange(p);

            _isRunning = true;
        }

        public void Stop()
        {
            _positionSvc.PositionChanged -= ((s, p) => UpdatePositionChange(p));
            _pricingSvc.PriceChanged -= ((s, p) => UpdatePriceChange(p));

            _positionSvc.Stop();
            _pricingSvc.Stop();
        }

        public Task<IList<Stock>> GetAllStocksAsync()
        {
            if (!_isRunning) Start();

            return GetStocksAsync(_htStocks.Keys);
        }

        public Task<IList<Stock>> GetStocksAsync(IEnumerable<string> requestedSymbols)
        {
            if (!_isRunning) Start();

            return Task.FromResult<IList<Stock>>(requestedSymbols
                .Where(s => _htStocks.ContainsKey(s))
                .Select(s => _htStocks[s])
                .OrderBy(kv => kv.Symbol)
                .ToList());
        }

        public Task<Stock> GetStockAsync(string symbol)
        {
            if (!_isRunning) Start();

            return Task.FromResult(_htStocks.ContainsKey(symbol) ? _htStocks[symbol] : null);
        }

        private void UpdatePositionChange(Position position)
        {
            var symbol = position.Symbol;
            if (!_htStocks.ContainsKey(symbol)) return;
            var stock = _htStocks[symbol];
            _htStocks[symbol] = new Stock(symbol, position.Qty, stock.Price);
            OnValueChanged(stock);
        }

        private void UpdatePriceChange(SymbolPrice sp)
        {
            var symbol = sp.Symbol;
            if (!_htStocks.ContainsKey(symbol)) return;
            var stock = _htStocks[symbol];
            _htStocks[symbol] = new Stock(symbol, stock.Quantity, sp.Price);
            OnValueChanged(stock);
        }

        private void OnValueChanged(Stock stock)
        {
            var handler = ValueChanged;
            handler?.Invoke(null, stock);
        }

    }
}
