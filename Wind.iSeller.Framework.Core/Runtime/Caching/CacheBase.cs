using System;
using Castle.Core.Logging;

namespace Wind.iSeller.Framework.Core.Runtime.Caching
{
    /// <summary>
    /// Base class for caches.
    /// It's used to simplify implementing <see cref="ICache"/>.
    /// </summary>
    public abstract class CacheBase : ICache
    {
        public ILogger Logger { get; set; }

        public string Name { get; private set; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly object SyncObj = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);

            Logger = NullLogger.Instance;
        }

        public virtual object Get(string key, Func<string, object> factory)
        {
            object item = null;

            try
            {
                item = GetOrDefault(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (item == null)
            {
                lock (SyncObj)
                {
                    try
                    {
                        item = GetOrDefault(key);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    if (item == null)
                    {
                        item = factory(key);

                        if (item == null)
                        {
                            return null;
                        }

                        try
                        {
                            Set(key, item);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return item;
        }

        public abstract object GetOrDefault(string key);

        public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public abstract void Remove(string key);

        public abstract void Clear();

        public virtual void Dispose()
        {

        }
    }
}
