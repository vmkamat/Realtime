using Autofac;
using Realtime.ViewModel;

namespace Realtime.Modules
{
    public class ViewModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MainViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
