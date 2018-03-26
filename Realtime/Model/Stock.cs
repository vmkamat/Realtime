using GalaSoft.MvvmLight;

namespace Realtime.Model
{
    public class Stock : ViewModelBase
    {
        private double _price;
        private int _qty;

        public Stock(string symbol, int qty, double px)
        {
            Symbol = symbol;
            Quantity = qty;
            Price = px;
        }

        public string Symbol { get; }

        public int Quantity
        {
            get => _qty;
            set
            {
                _qty = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Value");
            }
        }

        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Value");
            }
        }

        public double Value => Quantity * Price;

    }
}