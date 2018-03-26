using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Realtime.Model;

namespace Realtime.Services
{
    public class PricingService : IPricingService
    {
        private readonly ConcurrentDictionary<string, SymbolPrice> _htPrices;
        private readonly ISymbolRepository _symbolRepository;

        private IList<string> _allSymbols;
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public event EventHandler<SymbolPrice> PriceChanged;

        
        public PricingService(ISymbolRepository sr)
        {
            _allSymbols = new List<string>();
            _symbolRepository = sr;
            _htPrices = new ConcurrentDictionary<string, SymbolPrice>();
        }

        public void Start()
        {
            if (_isRunning) return;

            _cts = new CancellationTokenSource();

            _allSymbols = _symbolRepository.GetAllSymbols();

            foreach (var symbol in _allSymbols)
                _htPrices.TryAdd(symbol, new SymbolPrice(symbol, 0));

            for (var i = 0; i < 10; i++)
            {
                var counter = i;
                SimulateChanges(new Random(counter));
            }

            _isRunning = true;
        }

        public void Stop()
        {
            _cts.Cancel();
            _isRunning = false;
        }

        public IList<SymbolPrice> GetAllPrices()
        {
            if (!_isRunning) Start();

            return GetPrices(_allSymbols);
        }

        public IList<SymbolPrice> GetPrices(IEnumerable<string> requestedSymbols)
        {
            if (!_isRunning) Start();

            return requestedSymbols.Where(s => _htPrices.ContainsKey(s)).Select(s => _htPrices[s]).ToList();
        }

        public SymbolPrice GetPrice(string symbol)
        {
            if (!_isRunning) Start();

            return _htPrices.ContainsKey(symbol) ? _htPrices[symbol] : null;
        }

        private async void SimulateChanges(Random random)
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(20);

                for (var i = 0; i < 2; i++)
                {
                    var index = random.Next(0, _allSymbols.Count);
                    var symbol = _allSymbols[index];
                    var price = random.NextDouble() * 100;

                    var symbolPrice = _htPrices[symbol] = new SymbolPrice(symbol, price);

                    var handler = PriceChanged;
                    handler?.Invoke(null, symbolPrice);                              
                }
            }
        }
    }
}