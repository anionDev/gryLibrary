using System.Text;
using System.Xml;

namespace GRYLibrary
{
    /// <summary>
    /// Represents a simple Manager for persisting on the file-system and reloading an object.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be persisted.</typeparam>
    public sealed class SimpleObjectPersistence<T> where T : new()
    {
        public T Object { get; set; }
        public string File { get; set; }
        private readonly SimpleGenericXMLSerializer<T> _Serializer = null;
        public XmlWriterSettings XMLWriterSettings { get { return this._Serializer.XMLWriterSettings; } set { this._Serializer.XMLWriterSettings = value; } }
        /// <summary>
        /// Loads an object from <paramref name="file"/> which will be stored in <see cref="Object"/>. UTF-8 will be used as encoding.
        /// </summary>
        /// <param name="file">filename with full path</param>
        public SimpleObjectPersistence(string file) : this(file, new UTF8Encoding(false))
        {
        }
        /// <summary>
        /// Loads an object from <paramref name="file"/> which will be stored in <see cref="Object"/>.
        /// </summary>
        /// <param name="file">filename with full path</param>
        /// <param name="encoding">Encoding which should be used to load <paramref name="file"/></param>
        public SimpleObjectPersistence(string file, Encoding encoding)
        {
            this.File = file;
            this._Serializer = new SimpleGenericXMLSerializer<T>();
            this.LoadObject(encoding);
        }
        /// <summary>
        /// Stores <paramref name="object"/> in <see cref="Object"/> and in <paramref name="file"/>. UTF-8 will be used as encoding.
        /// </summary>
        /// <param name="file">filename with full path</param>
        /// <param name="object">object which should be saved</param>
        public SimpleObjectPersistence(string file, T @object) : this(file,new UTF8Encoding(false), @object)
        {
        }
        /// <summary>
        /// Stores <paramref name="object"/> in <see cref="Object"/> and in <paramref name="file"/>.
        /// </summary>
        /// <param name="file">filename with full path</param>
        /// <param name="encoding">Encoding which should be used to save <paramref name="file"/></param>
        /// <param name="object">object which should be saved</param>
        public SimpleObjectPersistence(string file,Encoding encoding, T @object) : this(file, @object, new XmlWriterSettings() { Indent = true, Encoding = encoding })
        {
        }
        /// <summary>
        /// Stores <paramref name="object"/> in <see cref="Object"/> and in <paramref name="file"/>.
        /// </summary>
        /// <param name="file">filename with full path</param>
        /// <param name="object">object which should be saved</param>
        /// <param name="xmlWriterSettings">settings for writing <paramref name="object"/> to the disk</param>
        public SimpleObjectPersistence(string file, T @object, XmlWriterSettings xmlWriterSettings) : this(file)
        {
            this.Object = @object;
            this.XMLWriterSettings = xmlWriterSettings;
            this.SaveObject();
        }
        public void LoadObject()
        {
            this.LoadObject(new UTF8Encoding(false));
        }
        public void LoadObject(Encoding encoding)
        {
            if (!System.IO.File.Exists(this.File))
            {
                this.ResetObject();
            }
            this.Object = this._Serializer.Deserialize(System.IO.File.ReadAllText(this.File, encoding));
        }

        public void ResetObject()
        {
            this.Object = new T();
            this.SaveObject();
        }

        public void SaveObject()
        {
            Utilities.EnsureFileExists(this.File);
            System.IO.File.WriteAllText(this.File, this._Serializer.Serialize(this.Object), this.XMLWriterSettings.Encoding);
        }
    }
}
