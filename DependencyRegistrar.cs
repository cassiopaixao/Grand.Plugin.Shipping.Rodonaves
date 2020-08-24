using Autofac;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Plugin.Shipping.Rodonaves.Services;

namespace Grand.Plugin.Shipping.Rodonaves
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, GrandConfig grandConfig)
        {
            builder.RegisterType<RodonavesComputationMethod>().InstancePerLifetimeScope();
            builder.RegisterType<RodonavesService>().As<IRodonavesService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
