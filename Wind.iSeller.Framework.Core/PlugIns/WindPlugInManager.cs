using System;
using System.Linq;

namespace Wind.iSeller.Framework.Core.PlugIns
{
    public class WindPlugInManager : IWindPlugInManager
    {
        public PlugInSourceList PlugInSources { get; private set; }

        private static readonly object syncObj = new object();
        private static bool _isRegisteredToAssemblyResolve;

        public WindPlugInManager()
        {
            PlugInSources = new PlugInSourceList();
            RegisterToAssemblyResolve(PlugInSources);
        }


        private static void RegisterToAssemblyResolve(PlugInSourceList plugInSources)
        {
            if (_isRegisteredToAssemblyResolve)
            {
                return;
            }

            lock (syncObj)
            {
                if (_isRegisteredToAssemblyResolve)
                {
                    return;
                }

                _isRegisteredToAssemblyResolve = true;

                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    return plugInSources.GetAllAssemblies().FirstOrDefault(a => a.FullName == args.Name);
                };
            }
        }

    }
}