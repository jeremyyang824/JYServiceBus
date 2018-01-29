using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wind.iSeller.Framework.Core.Modules;

namespace Wind.iSeller.Framework.Core.Reflection
{
    public class WindAssemblyFinder : IAssemblyFinder
    {
        private readonly IWindModuleManager _moduleManager;

        public WindAssemblyFinder(IWindModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}