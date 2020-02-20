using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.XMLSerializer
{
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private readonly XmlSerializer _KeySerializer;
        private readonly XmlSerializer _ValueSerializer;
        public SerializableDictionary()
        {
            _KeySerializer = new XmlSerializer(typeof(TKey));
            _ValueSerializer = new XmlSerializer(typeof(TValue));
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            reader.Read();
            if (isEmptyElement)
            {
                return;
            }
            else
            {
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("Item");
                    reader.ReadStartElement("Key");
                    TKey key = (TKey)_KeySerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("Value");
                    TValue value = (TValue)_ValueSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    this.Add(key, value);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("Item");

                writer.WriteStartElement("Key");
                _KeySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("Value");
                _ValueSerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}
