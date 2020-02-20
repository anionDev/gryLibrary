using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.XMLSerializer
{
    public class SerializableDictionary<TKey, TValue>
    {
        public IDictionary<TKey, TValue> Dictionary { get; }
        /// <summary>
        /// Represents the items as list. This property is required for the serializer and is not intended to be used directly.
        /// </summary>
        public List<KeyValuePair<TKey, TValue>> Items
        {
            get
            {
                List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>();
                foreach(System.Collections.Generic.KeyValuePair<TKey, TValue> item in Dictionary)
                {
                    result.Add(new KeyValuePair<TKey, TValue>(item.Key, item.Value));
                }
                return result;
            }
            set
            {
                Dictionary.Clear();
                foreach (KeyValuePair<TKey, TValue> item in value)
                {
                    Dictionary.Add(item.Key, item.Value);
                }
            }
        }
        public SerializableDictionary()
        {
            Dictionary = new Dictionary<TKey, TValue>();
            Items = new List<KeyValuePair<TKey, TValue>>();
        }
    }
}
