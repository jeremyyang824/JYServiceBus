using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Facilities.Logging;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.PlugIns;
using Wind.iSeller.Framework.Log4Net.Castle;

namespace Wind.iSeller.NServiceBus.Host
{
    public class WindServiceBusApplication<TStartupModule>
        where TStartupModule : WindModule
    {
        public static WindBootstrapper WindBootstrapper { get; private set; }

        static WindServiceBusApplication()
        {
            WindBootstrapper = WindBootstrapper.Create<TStartupModule>();

            WindBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseWindLog4Net().WithConfig("log4net.config")
            );
        }

        public void Start()
        {
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PluginModules");
            if (!Directory.Exists(pluginFolder))
            {
                Directory.CreateDirectory(pluginFolder);
            }
            WindBootstrapper.PlugInSources.AddFolder(pluginFolder);

            WindBootstrapper.Initialize();


        }

        public void Stop()
        {
            WindBootstrapper.Dispose();
        }
    }
}
