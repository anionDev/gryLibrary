using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Xml;
using System.Xml.Schema;
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

        public System.Collections.Generic.KeyValuePair<object, object> ToDotNetKeyValuePair()
        {
            return new System.Collections.Generic.KeyValuePair<object, object>(this.Key, this.Value);
        }
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }

        public XmlSchema GetSchema()
        {
            return Generic.GenericGetSchema(this);
        }

        public void ReadXml(XmlReader reader)
        {
            Generic.GenericReadXml(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Generic.GenericWriteXml(this, writer);
        }
        #endregion
    }
}
