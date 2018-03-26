using System.Collections.Generic;
using System.Linq;
using Realtime.Model;

namespace Realtime.repository
{
    public class SymbolRepository : ISymbolRepository
    {
        private IList<string> _allSymbols;

        public SymbolRepository()
        {
            _allSymbols = new List<string>();
        }

        public IList<string> GetAllSymbols()
        {
            if (_allSymbols != null && _allSymbols.Any()) return _allSymbols;

            _allSymbols = getAllSymbols();

            return _allSymbols;

            IList<string> getAllSymbols()
            {
                var allSymbols = new List<string>();
                var alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				
                foreach (var c in alphabets)
                    for (var i = 0; i < 100; i++)
                    {
                        var symbol = c + i.ToString("D3");
                        allSymbols.Add(symbol);
                    }
                return allSymbols;
            }
        }
    }
}