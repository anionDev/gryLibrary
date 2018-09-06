using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace GRYLibrary
{
    public class SimpleGenericXMLSerializer<T>
    {
        public string Serialize(T @object)
        {
            return Serialize(@object, new XmlWriterSettings() { Encoding = this.Encoding });
        }
        public string SerializeWithIndent(T @object)
        {
            return Serialize(@object, new XmlWriterSettings { Indent = true, Encoding = this.Encoding });
        }
        public string Serialize(T @object, XmlWriterSettings settings)
        {
            using (Stream stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                {
                    serializer.WriteObject(xmlWriter, @object);
                }
                stream.Seek(0, SeekOrigin.Begin);
                StreamReader streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
        public System.Text.Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;
        public T Deserialize(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = this.Encoding.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }
    }
}


