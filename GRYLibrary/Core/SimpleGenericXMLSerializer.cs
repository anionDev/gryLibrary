using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Core
{
    /// <summary>
    /// Represents a very easy usable XML-Serializer.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be serialized.</typeparam>
    public class SimpleGenericXMLSerializer<T> where T : new()
    {
        private readonly ISet<Type> _AllTypes;
        public Encoding Encoding { get; set; }
        public XmlWriterSettings XMLWriterSettings { get; set; }
        public ISet<Type> KnownTypes { get; set; }
        public SimpleGenericXMLSerializer()
        {
            this._AllTypes = new HashSet<Type>(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()));
            this.Encoding = new UTF8Encoding(false);
            this.XMLWriterSettings = new XmlWriterSettings() { Indent = true, Encoding = Encoding };
            this.KnownTypes = new HashSet<Type>();
        }
        public string Serialize(T @object)
        {
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T), this.GetExtraTypes());
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, this.XMLWriterSettings))
                {
                    serializer.Serialize(xmlWriter, @object);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public T Deserialize(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = this.Encoding.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                XmlSerializer deserializer = new XmlSerializer(typeof(T), this.GetExtraTypes());
                return (T)deserializer.Deserialize(stream);
            }
        }

        private Type[] GetExtraTypes()
        {
            return this.KnownTypes.Union(this._AllTypes).Where(t => typeof(T).IsAssignableFrom(t)).ToArray();
        }
    }
}


