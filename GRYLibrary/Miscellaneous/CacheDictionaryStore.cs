using System;
using System.Collections.Generic;

namespace GRYLibrary.Miscellaneous
{
    public class CacheDictionaryStore<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _Cache = new Dictionary<TKey, TValue>();
        private readonly Func<TKey, TValue> _GetFunction = null;

        public CacheDictionaryStore(Func<TKey, TValue> getFunction)
        {
            this._GetFunction = getFunction;
        }

        public TValue GetValue(TKey key)
        {
            if (!this._Cache.ContainsKey(key))
            {
                this._Cache.Add(key, this._GetFunction(key));
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
}
