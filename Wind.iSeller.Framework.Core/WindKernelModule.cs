using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.Runtime;
using Wind.iSeller.Framework.Core.Reflection.Extensions;
using Wind.iSeller.Framework.Core.Events.Bus;

namespace Wind.iSeller.Framework.Core
{
    /// <summary>
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class WindKernelModule : WindModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            AddAuditingSelectors();
            AddLocalizationSources();
            AddSettingProviders();
            AddUnitOfWorkFilters();
            ConfigureCaches();
        }

        public override void Initialize()
        {
            foreach (var replaceAction in ((WindStartupConfiguration)Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }
            
            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(typeof(WindKernelModule).GetAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }

        public override void PostInitialize()
        {
            RegisterMissingComponents();

            //IocManager.Resolve<SettingDefinitionManager>().Initialize();
        }

        public override void Shutdown()
        {
            
        }

        private void AddUnitOfWorkFilters()
        {
            //Configuration.UnitOfWork.RegisterFilter(WindDataFilters.SoftDelete, true);
            //Configuration.UnitOfWork.RegisterFilter(WindDataFilters.MustHaveTenant, true);
            //Configuration.UnitOfWork.RegisterFilter(WindDataFilters.MayHaveTenant, true);
        }

        private void AddSettingProviders()
        {

        }

        private void AddAuditingSelectors()
        {
            //Configuration.Auditing.Selectors.Add(
            //    new NamedTypeSelector(
            //        "Wind.ApplicationServices",
            //        type => typeof(IApplicationService).IsAssignableFrom(type)
            //    )
            //);
        }

        private void AddLocalizationSources()
        {
            
        }

        private void ConfigureCaches()
        {
            //Configuration.Caching.Configure(WindCacheNames.ApplicationSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            //});

            //Configuration.Caching.Configure(WindCacheNames.TenantSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            //});

            //Configuration.Caching.Configure(WindCacheNames.UserSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            //});
        }

        private void RegisterMissingComponents()
        {
            //if (!IocManager.IsRegistered<IGuidGenerator>())
            //{
            //    IocManager.IocContainer.Register(
            //        Component
            //            .For<IGuidGenerator, SequentialGuidGenerator>()
            //            .Instance(SequentialGuidGenerator.Instance)
            //    );
            //}

            //IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            //IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Singleton);
            //IocManager.RegisterIfNot<IPermissionChecker, NullPermissionChecker>(DependencyLifeStyle.Singleton);
            //IocManager.RegisterIfNot<IRealTimeNotifier, NullRealTimeNotifier>(DependencyLifeStyle.Singleton);
            //IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>(DependencyLifeStyle.Singleton);
            //IocManager.RegisterIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>(DependencyLifeStyle.Singleton);
            //IocManager.RegisterIfNot<IClientInfoProvider, NullClientInfoProvider>(DependencyLifeStyle.Singleton);
                       
        }
    }
}
