using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.Core.Modules;

namespace Wind.iSeller.Framework.Core.PlugIns
{
    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(WindModule.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}