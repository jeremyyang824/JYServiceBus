using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Wind.iSeller.Framework.Core.Configuration.Startup;
using Wind.iSeller.Framework.Core.Dependency;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.PlugIns;

namespace Wind.iSeller.Framework.Core.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class WindModuleManager : IWindModuleManager
    {
        public WindModuleInfo StartupModule { get; private set; }

        public IList<WindModuleInfo> Modules { get { return _modules; } }   // to immutable list

        public ILogger Logger { get; set; }

        private WindModuleCollection _modules;

        private readonly IIocManager _iocManager;
        private readonly IWindPlugInManager _windPlugInManager;

        public WindModuleManager(IIocManager iocManager, IWindPlugInManager windPlugInManager)
        {
            _iocManager = iocManager;
            _windPlugInManager = windPlugInManager;

            Logger = NullLogger.Instance;
        }

        public virtual void Initialize(Type startupModule)
        {
            _modules = new WindModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.Debug("Shutting down has been started");

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.Debug("Loading Wind modules...");

            List<Type> plugInModuleTypes;
            var moduleTypes = FindAllModuleTypes(out plugInModuleTypes).Distinct().ToList();

            Logger.Debug("Found " + moduleTypes.Count + " Wind modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = WindModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);

            foreach (var plugInModuleType in _windPlugInManager.PlugInSources.GetAllModules())
            {
                if (modules.AddIfNotContains(plugInModuleType))
                {
                    plugInModuleTypes.Add(plugInModuleType);
                }
            }

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _iocManager.Resolve(moduleType) as WindModule;
                if (moduleObject == null)
                {
                    throw new WindException("This type is not an Wind module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IWindStartupConfiguration>();

                var moduleInfo = new WindModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                _iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in WindModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new WindException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
