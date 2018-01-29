using System;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Runtime.Caching.Configuration;

namespace Wind.iSeller.Framework.Core.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with MemoryCache.
    /// </summary>
    public class WindMemoryCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindMemoryCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterIfNot<WindMemoryCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<WindMemoryCache>(new { name });
        }
    }
}
