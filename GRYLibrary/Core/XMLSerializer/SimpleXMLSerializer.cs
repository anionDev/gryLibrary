using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer
{
    public class SimpleXMLSerializer
    {

        public Encoding Encoding { get; set; }
        public XmlWriterSettings XMLWriterSettings { get; set; }
        public ISet<Type> KnownTypes { get; set; }
        public SimpleXMLSerializer()
        {
            this.Encoding = new UTF8Encoding(false);
            this.XMLWriterSettings = new XmlWriterSettings() { Indent = true, Encoding = Encoding };
            this.KnownTypes = new HashSet<Type>();
        }
        public void SerializeToWriter(object @object, XmlWriter xmlWriter)
        {
            object serializer = this.GetSerializerForType(@object.GetType());
            serializer.GetType().GetMethod(nameof(SimpleXMLSerializer.SerializeToWriter)).Invoke(serializer, new object[] { @object, xmlWriter });
        }

        public string Serialize(object @object)
        {
            object serializer = this.GetSerializerForType(@object.GetType());
            return (string)serializer.GetType().GetMethod(nameof(SimpleXMLSerializer.Serialize)).Invoke(serializer, new object[] { @object });
        }

        public dynamic DeserializeFromReader(XmlReader xmlReader)
        {
            object serializer = this.GetSerializerForType(this.GetOutermostType(xmlReader));
            return serializer.GetType().GetMethod(nameof(SimpleXMLSerializer.DeserializeFromReader)).Invoke(serializer, new object[] { xmlReader });
        }

        public dynamic Deserialize(string xml)
        {
            object serializer = this.GetSerializerForType(this.GetOutermostType(xml));
            return serializer.GetType().GetMethod(nameof(SimpleXMLSerializer.Deserialize)).Invoke(serializer, new object[] { xml });
        }

        private object GetSerializerForType(Type type)
        {
            return Activator.CreateInstance(typeof(SimpleGenericXMLSerializer<>).MakeGenericType(new Type[] { type }));
        }
        private Type GetOutermostType(XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }
        private Type GetOutermostType(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xml);
            return Type.GetType(xmlDoc.DocumentElement.Name);
        }
    }
}
