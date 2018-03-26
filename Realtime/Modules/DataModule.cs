using Autofac;
using Realtime.Model;
using Realtime.repository;
using Realtime.Services;

namespace Realtime.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().SingleInstance();
            builder.RegisterType<PricingService>().As<IPricingService>().SingleInstance();
            builder.RegisterType<PositionService>().As<IPositionService>().SingleInstance();
            builder.RegisterType<ValuationService>().As<IValuationService>().SingleInstance();
        }
    }
}
