using System;
using System.Collections.Concurrent;
using Wind.iSeller.Framework.Core.Collections.Extensions;
using Castle.Core.Logging;

namespace Wind.iSeller.Framework.Core.Runtime
{
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        public ILogger Logger { get; set; }

        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        private readonly IAmbientDataContext _dataContext;

        public DataContextAmbientScopeProvider(IAmbientDataContext dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            _dataContext = dataContext;

            Logger = NullLogger.Instance;
        }

        public T GetValue(string contextKey)
        {
            var item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default(T);
            }

            return item.Value;
        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new WindException("Can not add item! ScopeDictionary.TryAdd returns false!");
            }

            _dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = _dataContext.GetData(contextKey) as string;
            return objKey != null ? ScopeDictionary.GetOrDefault(objKey) : null;
        }

        private class ScopeItem
        {
            public string Id { get; private set; }

            public ScopeItem Outer { get; private set; }

            public T Value { get; private set; }

            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }
    }
}