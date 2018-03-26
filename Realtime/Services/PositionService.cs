using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Realtime.Model;

namespace Realtime.Services
{
    public class PositionService : IPositionService
    {
        private readonly ConcurrentDictionary<string, Position> _htPositions;
        private readonly ISymbolRepository _symbolRepository;

        private IList<string> _allSymbols;
        private bool _isRunning;
        private int _symbolCount;
        private CancellationTokenSource _cts;

        public event EventHandler<Position> PositionChanged;

        public PositionService(ISymbolRepository symbolRepository)
        {
            _symbolRepository = symbolRepository;
            _htPositions = new ConcurrentDictionary<string, Position>();
        }

        public void Start()
        {
            if (_isRunning) return;

            _cts = new CancellationTokenSource();

            _allSymbols = _symbolRepository.GetAllSymbols();
            _symbolCount = _allSymbols.Count;
            foreach (var symbol in _allSymbols)
                _htPositions.TryAdd(symbol, new Position(symbol, 0));

            _isRunning = true;


            for (var i = 0; i < 10; i++)
            {
                var counter = i;
                SimulateUpdates(new Random(counter));
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _isRunning = false;
        }

        public IList<Position> GetAllPositions()
        {
            if (!_isRunning) Start();

            return GetPositions(_allSymbols);
        }

        public IList<Position> GetPositions(IEnumerable<string> requestedSymbols)
        {
            if (!_isRunning) Start();

            return _allSymbols.Where(s => _htPositions.ContainsKey(s)).Select(s => _htPositions[s]).ToList();
        }

        public Position GetPosition(string symbol)
        {
            if (!_isRunning) Start();

            return _htPositions.ContainsKey(symbol) ? _htPositions[symbol] : null;
        }

        private async void SimulateUpdates(Random random)
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(20);

                for (var i = 0; i < 2; i++)
                {
                    var index = random.Next(0, _symbolCount);
                    var symbol = _allSymbols[index];
                    var qty = random.Next(0, 501);
                    var position = _htPositions[symbol] = new Position(symbol, qty);

                    var handler = PositionChanged;
                    handler?.Invoke(null, position);                    
                }
            }
        }
    }
}