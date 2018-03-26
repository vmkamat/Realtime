using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.Model
{
    public class Position
    {
        public Position( string symbol, int qty)
        {
            Symbol = symbol;
            Qty = qty;
        }

        public string Symbol { get; }
        public int Qty { get; }
    }
}
