using System;
using System.Collections.Generic;

namespace GRYLibrary.Miscellaneous
{
    public class CachDictionaryStore<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _Cache = new Dictionary<TKey, TValue>();
        private readonly Func<TKey, TValue> _GetFunction = null;

        public CachDictionaryStore(Func<TKey, TValue> getFunction)
        {
            this._GetFunction = getFunction;
        }

        public TValue GetValue(TKey key)
        {
            if (!_Cache.ContainsKey(key))
            {
                _Cache.Add(key, _GetFunction(key));
            }
            return _Cache[key];
        }
        public void ResetCache()
        {
            _Cache.Clear();
        }
        public void ResetCache(TKey item)
        {
            _Cache.Remove(item);
        }
    }
}
