using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace GRYLibrary
{
    /// <summary>
    /// Represents a very easy usable XML-Serializer.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be serialized.</typeparam>
    public class SimpleGenericXMLSerializer<T>
    {
        public string Serialize(T @object)
        {
            return this.Serialize(@object, new XmlWriterSettings() { Encoding = this.Encoding });
        }
        public string SerializeWithIndent(T @object)
        {
            return this.Serialize(@object, new XmlWriterSettings { Indent = true, Encoding = this.Encoding });
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
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
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


