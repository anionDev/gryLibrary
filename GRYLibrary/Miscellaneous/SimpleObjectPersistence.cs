using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary
{
    /// <summary>
    /// Represents a simple Manager for persisting on the file-system and reloading an object.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be persisted.</typeparam>
    /// <remarks>The types of the objects which should be serialized must be stated as class-types (not interface-types for example).
    /// If an object is not serializeable then the type can be "wrapped" in another class which correctly implements <see cref="IXmlSerializable"/>.</remarks>
    public sealed class SimpleObjectPersistence<T> where T : new()
    {
        public T Object { get; set; }
        public string File { get; set; }
        private readonly SimpleGenericXMLSerializer<T> _Serializer = new SimpleGenericXMLSerializer<T>();
        public XmlWriterSettings XMLWriterSettings { get { return this._Serializer.XMLWriterSettings; } set { this._Serializer.XMLWriterSettings = value; } }
        public Encoding Encoding = new UTF8Encoding(false);
        public static SimpleObjectPersistence<T> CreateByFile(string file)
        {
            SimpleObjectPersistence<T> result = new SimpleObjectPersistence<T>();
            result.File = file;
            return result;
        }
        public static SimpleObjectPersistence<T> CreateByObjectAndFile(T @object, string file)
        {
            SimpleObjectPersistence<T> result = new SimpleObjectPersistence<T>();
            result.File = file;
            result.Object = @object;
            return result;
        }
        public void LoadObjectFromFile()
        {
            if (!System.IO.File.Exists(this.File))
            {
                this.ResetObject();
            }
            this.Object = this._Serializer.Deserialize(System.IO.File.ReadAllText(this.File, this.Encoding));
        }

        public void ResetObject()
        {
            this.Object = new T();
            this.SaveObjectToFile();
        }

        public void SaveObjectToFile()
        {
            Utilities.EnsureFileExists(this.File);
            System.IO.File.WriteAllText(this.File, this._Serializer.Serialize(this.Object), this.XMLWriterSettings.Encoding);
        }
    }
}
