using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using Realtime.Model;
using System.Collections.Concurrent;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;

namespace Realtime.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IValuationService _vsvc;
        private ObservableCollection<Stock> _stocks;

        private ConcurrentDictionary<string, Stock> _htUpdates;
        private Timer _updateTimer;
        private int _uiUpdateInterval;
        private bool _flashUpdates;
        private bool _throttleEnabled;

        public MainViewModel(IValuationService vsvc )
        {
            if (IsInDesignMode) return;

            _htUpdates = new ConcurrentDictionary<string, Stock>();
            _vsvc = vsvc;

            var initializeTask = Initialize();
        }

        private async Task Initialize()
        {
            var stocks = await _vsvc.GetAllStocksAsync();
            Stocks = new ObservableCollection<Stock>(stocks);

            _vsvc.ValueChanged += (s, p) => FoldUpdates(p);

            UIupdateInterval = 250;
            FlashUpdates = false;
            _throttleEnabled = true;
            ThrottleUpdates();
        }

        public ObservableCollection<Stock> Stocks
        {
            get { return _stocks; }
            set
            {
                _stocks = value;
                RaisePropertyChanged("Stocks");
            }
        }

        public int UIupdateInterval
        {
            get { return _uiUpdateInterval; }
            set
            {
                _uiUpdateInterval = value;
                RaisePropertyChanged();
            }
        }

        public bool FlashUpdates
        {
            get { return _flashUpdates; }
            set
            {
                _flashUpdates = value;
                RaisePropertyChanged();
            }
        }

        private void UpdateDispatcherTimer()
        {
            if (_updateTimer == null)
            {
                var dispatcher = Application.Current.Dispatcher;
                _updateTimer = new Timer((o) => dispatcher.Invoke((Action)ThrottleUpdates, 
                    DispatcherPriority.Normal, null), null, 250, UIupdateInterval);
            }
            else
            {
                _updateTimer.Change(0, UIupdateInterval);
            }
        }

        private async void ThrottleUpdates()
        {
            while (true)
            {
                if (UIupdateInterval == 0) break;

                await Task.Delay(UIupdateInterval);

                ProcessFoldedUpdates();

                RaisePropertyChanged("Stocks");  
            }
        }

        private void ProcessFoldedUpdates()
        {
            var updates = _htUpdates.Values.ToList();
            _htUpdates.Clear();

            updates.ForEach(UpdateStock);
        }

        private void FoldUpdates(Stock update)
        {
            if (UIupdateInterval == 0)
            {
                _throttleEnabled = false;

                if (_htUpdates.Count > 0)
                {
                    ProcessFoldedUpdates();
                }
                UpdateStock(update);
            }
            else
            {
                if (!_throttleEnabled)
                {
                    _throttleEnabled = true;

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>ThrottleUpdates());
                }
                _htUpdates[update.Symbol] = update;
            }
        }

        private void UpdateStock(Stock update)
        {
            var stock = _stocks.Where(s => s.Symbol == update.Symbol).Select(s => s).FirstOrDefault();
            if (stock != null)
            {
                stock.Quantity = update.Quantity;
                stock.Price = update.Price;
            }
        }
    }
}