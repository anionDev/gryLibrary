using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        public SerializationConfiguration SerializationConfiguration { get; set; }
        public GenericXMLSerializer()
        {
            this.SerializationConfiguration = new SerializationConfiguration();
            this.SerializationConfiguration.XmlSerializer = new XmlSerializer(typeof(GRYSObject));
            this.SerializationConfiguration.PropertySelector = (PropertyInfo propertyInfo) => { return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic; };
            this.SerializationConfiguration.FieldSelector = (FieldInfo propertyInfo) => { return false; };
            this.SerializationConfiguration.Encoding = new UTF8Encoding(false);
        }

        private XmlWriterSettings GetXmlWriterSettings()
        {
            XmlWriterSettings result = new XmlWriterSettings();
            result.Encoding = this.SerializationConfiguration.Encoding;
            result.Indent = this.SerializationConfiguration.Indent;
            result.IndentChars = "    ";
            return result;
        }
        public string Serialize(T @object)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.GetXmlWriterSettings()))
            {
                this.Serialize(@object, xmlWriter);
            }
            return this.SerializationConfiguration.Encoding.GetString(memoryStream.ToArray());
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            GRYSObject genericallySerializedObject = GRYSObject.Create(@object, this.SerializationConfiguration);
            this.SerializationConfiguration.XmlSerializer.Serialize(writer, genericallySerializedObject);
        }
        public U Deserialize<U>(string serializedObject)
        {
            return (U)(object)this.Deserialize(serializedObject);
        }
        public T Deserialize(string serializedObject)
        {
            using StringReader stringReader = new StringReader(serializedObject);
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return this.Deserialize(xmlReader);
        }
        public T Deserialize(XmlReader reader)
        {
            GRYSObject grySerializedObject = (GRYSObject)this.SerializationConfiguration.XmlSerializer.Deserialize(reader);
            return (T)grySerializedObject.Get();
        }
    }
    public static class GenericXMLSerializer
    {
        public static GenericXMLSerializer<object> GetDefaultInstance()
        {
            GenericXMLSerializer<object> result = new GenericXMLSerializer<object>();
            return result;
        }
    }
}
