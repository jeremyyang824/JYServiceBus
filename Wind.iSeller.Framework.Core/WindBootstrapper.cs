using System;
using System.Reflection;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Dependency.Installers;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.PlugIns;

namespace Wind.iSeller.Framework.Core
{
    /// <summary>
    /// This is the main class that is responsible to start entire Wind system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class WindBootstrapper : IDisposable
    {
        /// <summary>
        /// Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; private set; }

        /// <summary>
        /// A list of plug in folders.
        /// </summary>
        public PlugInSourceList PlugInSources { get; private set; }

        /// <summary>
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private WindModuleManager _moduleManager;
        private ILogger _logger;

        private WindBootstrapper(Type startupModule)
            : this(startupModule, Dependency.IocManager.Instance)
        {

        }

        private WindBootstrapper(Type startupModule, IIocManager iocManager)
        {
            if (startupModule == null)
                throw new ArgumentNullException("startupModule");
            if (iocManager == null)
                throw new ArgumentNullException("iocManager");

            if (!typeof(WindModule).IsAssignableFrom(startupModule))
            {
                throw new ArgumentException(startupModule + " should be derived from WindModule.");
            }

            StartupModule = startupModule;
            IocManager = iocManager;

            PlugInSources = new PlugInSourceList();
            _logger = NullLogger.Instance;

            AddInterceptorRegistrars();
        }

        /// <summary>
        /// Creates a new <see cref="WindBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="WindModule"/>.</typeparam>
        public static WindBootstrapper Create<TStartupModule>()
            where TStartupModule : WindModule
        {
            return new WindBootstrapper(typeof(TStartupModule));
        }

        /// <summary>
        /// Creates a new <see cref="WindBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="WindModule"/>.</typeparam>
        /// <param name="iocManager">IIocManager that is used to bootstrap the Wind system</param>
        public static WindBootstrapper Create<TStartupModule>(IIocManager iocManager)
            where TStartupModule : WindModule
        {
            return new WindBootstrapper(typeof(TStartupModule), iocManager);
        }

        /// <summary>
        /// Creates a new <see cref="WindBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="WindModule"/>.</param>
        public static WindBootstrapper Create(Type startupModule)
        {
            return new WindBootstrapper(startupModule);
        }

        /// <summary>
        /// Creates a new <see cref="WindBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="WindModule"/>.</param>
        /// <param name="iocManager">IIocManager that is used to bootstrap the Wind system</param>
        public static WindBootstrapper Create(Type startupModule, IIocManager iocManager)
        {
            return new WindBootstrapper(startupModule, iocManager);
        }

        private void AddInterceptorRegistrars()
        {
            //TODO: Add Interceptors
            //ValidationInterceptorRegistrar.Initialize(IocManager);
            //AuditingInterceptorRegistrar.Initialize(IocManager);
            UnitOfWorkRegistrar.Initialize(IocManager);
            //AuthorizationInterceptorRegistrar.Initialize(IocManager);
        }

        /// <summary>
        /// Initializes the Wind system.
        /// </summary>
        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();
                IocManager.IocContainer.Install(new WindCoreInstaller());

                IocManager.Resolve<WindPlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<WindStartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<WindModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(WindBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<WindBootstrapper>())
            {
                IocManager.IocContainer.Register(Component.For<WindBootstrapper>().Instance(this));
            }
        }

        /// <summary>
        /// Disposes the Wind system.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (_moduleManager != null)
            {
                _moduleManager.ShutdownModules();
            }
        }
    }
}
