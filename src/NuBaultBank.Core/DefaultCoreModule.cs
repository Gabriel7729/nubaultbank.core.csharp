using Autofac;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Services;

namespace NuBaultBank.Core;
public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<LogService>()
        .As<ILogService>().InstancePerLifetimeScope();

    builder.RegisterType<ProductService>()
        .As<IProductService>().InstancePerLifetimeScope();

    builder.RegisterType<LoanService>()
        .As<ILoanService>().InstancePerLifetimeScope();
  }
}
