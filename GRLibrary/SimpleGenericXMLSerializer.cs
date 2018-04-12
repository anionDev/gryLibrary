using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace GRLibrary
{
    public static class SimpleGenericXMLSerializer
    {
        public static string Serialize<T>(T @object)
        {
            return Serialize(@object, new XmlWriterSettings() { Encoding = SimpleGenericXMLSerializer.Encoding });
        }
        public static string SerializeWithIndent<T>(T @object)
        {
            return Serialize(@object, new XmlWriterSettings { Indent = true, Encoding = SimpleGenericXMLSerializer.Encoding });
        }
        public static string Serialize<T>(T obj, XmlWriterSettings settings)
        {
            using (Stream stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                {
                    serializer.WriteObject(xmlWriter, Encoding);
                }
                stream.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
        public static System.Text.Encoding Encoding = System.Text.Encoding.UTF8;
        public static T Deserialize<T>(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = SimpleGenericXMLSerializer.Encoding.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }
    }

}


