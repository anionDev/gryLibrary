using System;
using System.Xml.Serialization;

namespace GRYLibrary.Core.XMLSerializer
{
    [Serializable]
    [XmlType(TypeName = "KeyValuePair")]
    public struct KeyValuePair<TKey, TValue>
    {
        public KeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public TKey Key { get; set; }

        public TValue Value { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is KeyValuePair<TKey, TValue>))
            {
                return false;
            }
            KeyValuePair<TKey, TValue> typedObject = new KeyValuePair<TKey, TValue>();
            return this.Key.Equals(typedObject.Key) && this.Value.Equals(typedObject.Value);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Key);
        }
    }
}
