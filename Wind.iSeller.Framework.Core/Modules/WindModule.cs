using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.Framework.Core.Modules
{
    /// <summary>
    /// 模块定义
    /// </summary>
    public abstract class WindModule
    {
        /// <summary>
        /// Gets a reference to the IOC manager.
        /// </summary>
        protected internal IIocManager IocManager { get; internal set; }

        /// <summary>
        /// Gets a reference to the Wind configuration.
        /// </summary>
        protected internal IWindStartupConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        protected WindModule()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// This is the first event called on application startup. 
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public virtual void PreInitialize()
        {

        }

        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// This method is called lastly on application startup.
        /// </summary>
        public virtual void PostInitialize()
        {

        }

        /// <summary>
        /// This method is called when the application is being shutdown.
        /// </summary>
        public virtual void Shutdown()
        {

        }

        public virtual Assembly[] GetAdditionalAssemblies()
        {
            return new Assembly[0];
        }

        /// <summary>
        /// Checks if given type is an Wind module class.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsWindModule(Type type)
        {
            var typeInfo = type;
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(WindModule).IsAssignableFrom(type);
        }

        /// <summary>
        /// Finds direct depended modules of a module (excluding given module).
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsWindModule(moduleType))
            {
                throw new WindException("This type is not an Wind module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }

            return list;
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesRecursively(list, moduleType);
            list.AddIfNotContains(typeof(WindKernelModule));
            return list;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> modules, Type module)
        {
            if (!IsWindModule(module))
            {
                throw new WindException("This type is not an Wind module: " + module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(modules, dependedModule);
            }
        }
    }
}
