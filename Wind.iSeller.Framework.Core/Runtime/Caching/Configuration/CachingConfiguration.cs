using System;
using System.Collections.Generic;
using Wind.iSeller.Framework.Core.Configuration.Startup;

namespace Wind.iSeller.Framework.Core.Runtime.Caching.Configuration
{
    internal class CachingConfiguration : ICachingConfiguration
    {
        public IWindStartupConfiguration WindConfiguration { get; private set; }

        public IList<ICacheConfigurator> Configurators
        {
            get { return _configurators; }
        }
        private readonly List<ICacheConfigurator> _configurators;

        public CachingConfiguration(IWindStartupConfiguration windConfiguration)
        {
            WindConfiguration = windConfiguration;

            _configurators = new List<ICacheConfigurator>();
        }

        public void ConfigureAll(Action<ICache> initAction)
        {
            _configurators.Add(new CacheConfigurator(initAction));
        }

        public void Configure(string cacheName, Action<ICache> initAction)
        {
            _configurators.Add(new CacheConfigurator(cacheName, initAction));
        }
    }
}
