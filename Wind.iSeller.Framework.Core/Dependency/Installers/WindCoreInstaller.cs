using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.PlugIns;
using Wind.iSeller.Framework.Core.Reflection;
using Wind.iSeller.Framework.Core.Runtime.Caching.Configuration;

namespace Wind.iSeller.Framework.Core.Dependency.Installers
{
    internal class WindCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAssemblyFinder, WindAssemblyFinder>().ImplementedBy<WindAssemblyFinder>().LifestyleSingleton(),
                Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<IWindStartupConfiguration, WindStartupConfiguration>().ImplementedBy<WindStartupConfiguration>().LifestyleSingleton(),
                Component.For<IWindModuleManager, WindModuleManager>().ImplementedBy<WindModuleManager>().LifestyleSingleton(),
                Component.For<IModuleConfigurations, ModuleConfigurations>().ImplementedBy<ModuleConfigurations>().LifestyleSingleton(),
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<IEventBusConfiguration, EventBusConfiguration>().ImplementedBy<EventBusConfiguration>().LifestyleSingleton(),
                Component.For<IWindPlugInManager, WindPlugInManager>().ImplementedBy<WindPlugInManager>().LifestyleSingleton(),
                Component.For<ICachingConfiguration, CachingConfiguration>().ImplementedBy<CachingConfiguration>().LifestyleSingleton()

                //Component.For<INavigationConfiguration, NavigationConfiguration>().ImplementedBy<NavigationConfiguration>().LifestyleSingleton(),
                //Component.For<ILocalizationConfiguration, LocalizationConfiguration>().ImplementedBy<LocalizationConfiguration>().LifestyleSingleton(),
                //Component.For<IAuthorizationConfiguration, AuthorizationConfiguration>().ImplementedBy<AuthorizationConfiguration>().LifestyleSingleton(),
                //Component.For<IValidationConfiguration, ValidationConfiguration>().ImplementedBy<ValidationConfiguration>().LifestyleSingleton(),
                //Component.For<IFeatureConfiguration, FeatureConfiguration>().ImplementedBy<FeatureConfiguration>().LifestyleSingleton(),
                //Component.For<ISettingsConfiguration, SettingsConfiguration>().ImplementedBy<SettingsConfiguration>().LifestyleSingleton(),
                //Component.For<IMultiTenancyConfig, MultiTenancyConfig>().ImplementedBy<MultiTenancyConfig>().LifestyleSingleton(),
                //Component.For<IAuditingConfiguration, AuditingConfiguration>().ImplementedBy<AuditingConfiguration>().LifestyleSingleton(),
                //Component.For<IBackgroundJobConfiguration, BackgroundJobConfiguration>().ImplementedBy<BackgroundJobConfiguration>().LifestyleSingleton(),
                //Component.For<INotificationConfiguration, NotificationConfiguration>().ImplementedBy<NotificationConfiguration>().LifestyleSingleton(),
                //Component.For<IEmbeddedResourcesConfiguration, EmbeddedResourcesConfiguration>().ImplementedBy<EmbeddedResourcesConfiguration>().LifestyleSingleton(),
                );
        }
    }
}
