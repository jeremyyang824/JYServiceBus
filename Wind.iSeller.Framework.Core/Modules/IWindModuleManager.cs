using System;
using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Modules
{
    public interface IWindModuleManager
    {
        WindModuleInfo StartupModule { get; }

        IList<WindModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}
