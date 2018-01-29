using System;
using System.Collections.Generic;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Framework.Core.Runtime.Caching.Configuration;

namespace Wind.iSeller.Framework.Core.Configuration.Startup
{
    /// <summary>
    /// This class is used to configure Wind and modules on startup.
    /// </summary>
    public class WindStartupConfiguration : DictionaryBasedConfig, IWindStartupConfiguration
    {
        /// <summary>
        /// Reference to the IocManager.
        /// </summary>
        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// Used to configure authorization.
        /// </summary>
        //public IAuthorizationConfiguration Authorization { get; private set; }

        /// <summary>
        /// Used to configure validation.
        /// </summary>
        //public IValidationConfiguration Validation { get; private set; }

        /// <summary>
        /// Used to configure settings.
        /// </summary>
        //public ISettingsConfiguration Settings { get; private set; }

        /// <summary>
        /// Gets/sets default connection string used by ORM module.
        /// It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="ModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        /// <summary>
        /// Used to configure unit of work defaults.
        /// </summary>
        public IUnitOfWorkDefaultOptions UnitOfWork { get; private set; }

        /// <summary>
        /// Used to configure <see cref="IEventBus"/>.
        /// </summary>
        public IEventBusConfiguration EventBus { get; private set; }

        /// <summary>
        /// Used to configure auditing.
        /// </summary>
        //public IAuditingConfiguration Auditing { get; private set; }

        public ICachingConfiguration Caching { get; private set; }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public WindStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        public void Initialize()
        {
            Modules = IocManager.Resolve<IModuleConfigurations>();
            Caching = IocManager.Resolve<ICachingConfiguration>();

            ServiceReplaceActions = new Dictionary<Type, Action>();
        }

        public void ReplaceService(Type type, Action replaceAction)
        {
            ServiceReplaceActions[type] = replaceAction;
        }

        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, () => IocManager.Resolve<T>());
        }
    }
}
