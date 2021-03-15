using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous
{
    public class CacheDictionaryStore<TKey, TValue, THelper>
    {
        internal readonly IDictionary<TKey, TValue> _Cache = new Dictionary<TKey, TValue>();
        internal readonly Func<TKey, THelper, TValue> _GetFunction = null;

        public CacheDictionaryStore(Func<TKey, THelper, TValue> getFunction)
        {
            this._GetFunction = getFunction;
        }

        public TValue GetValue(TKey key, THelper helper)
        {
            if (!this._Cache.ContainsKey(key))
            {
                this._Cache.Add(key, this._GetFunction(key, helper));
            }
            return this._Cache[key];
        }
        public void ResetCache()
        {
            this._Cache.Clear();
        }
        public void ResetCache(TKey item)
        {
            this._Cache.Remove(item);
        }
        public bool ContainsKey(TKey key)
        {
            return this._Cache.ContainsKey(key);
        }
    }
    public class CacheDictionaryStore<TKey, TValue>
    {
        private readonly CacheDictionaryStore<TKey, TValue, object> _CacheDictionaryStore;
        public CacheDictionaryStore(Func<TKey, TValue> getFunction)
        {
            this._CacheDictionaryStore = new CacheDictionaryStore<TKey, TValue, object>((key, _) => getFunction(key));
        }
        public TValue GetValue(TKey key)
        {
            return _CacheDictionaryStore.GetValue(key, default);
        }
        public void ResetCache()
        {
            _CacheDictionaryStore.ResetCache();
        }
        public void ResetCache(TKey item)
        {
            _CacheDictionaryStore.ResetCache(item);
        }
        public bool ContainsKey(TKey key)
        {
            return this._CacheDictionaryStore.ContainsKey(key);
        }
    }
}
