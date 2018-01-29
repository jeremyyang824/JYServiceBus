using System;

namespace Wind.iSeller.Framework.Core.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ITypedCache{TKey,TValue}"/>.
    /// </summary>
    public static class TypedCacheExtensions
    {
        public static TValue Get<TKey, TValue>(this ITypedCache<TKey, TValue> cache, TKey key, Func<TValue> factory)
        {
            return cache.Get(key, k => factory());
        }

    }
}
