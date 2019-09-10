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
        public XmlWriterSettings XMLWriterSettings { get; set; } = new XmlWriterSettings() { Indent = true, Encoding = new UTF8Encoding(false) };
        public string Serialize(T @object)
        {
            using (Stream stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, this.XMLWriterSettings))
                {
                    serializer.WriteObject(xmlWriter, @object);
                }
                stream.Seek(0, SeekOrigin.Begin);
                StreamReader streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
        public T Deserialize(string xml)
        {
            return this.Deserialize(xml, new UTF8Encoding(false));
        }
        public T Deserialize(string xml, Encoding encoding)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = encoding.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }
    }
}


