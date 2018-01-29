using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wind.iSeller.Framework.Core.PlugIns
{
    public interface IPlugInSource
    {
        List<Assembly> GetAssemblies();

        List<Type> GetModules();
    }
}