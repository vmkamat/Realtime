using System.Threading.Tasks;
using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using Realtime.Model;
using Realtime.Modules;

namespace Realtime
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new ViewModule());
            var container = builder.Build();

            DispatcherHelper.Initialize();

            Task.Run(() => container.Resolve<IPositionService>().Start());
            Task.Run(() => container.Resolve<IPricingService>().Start());
            Task.Run(() => container.Resolve<IValuationService>().Start());

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}