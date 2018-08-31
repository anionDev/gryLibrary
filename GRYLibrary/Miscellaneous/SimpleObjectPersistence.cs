namespace GRYLibrary
{
    public sealed class SimpleObjectPersistence<T> where T : new()
    {
        public T Object { get; private set; }
        public System.Text.Encoding Encoding { get; private set; }
        public string File { get; set; }
        private readonly SimpleGenericXMLSerializer<T> _Serializer = null;
        public SimpleObjectPersistence(string file, System.Text.Encoding encoding)
        {
            this.File = file;
            this.Encoding = encoding;
            this._Serializer = new SimpleGenericXMLSerializer<T>();
            this._Serializer.Encoding = this.Encoding;
        }
        public void LoadObject()
        {
            if (!System.IO.File.Exists(this.File))
            {
                ResetObject();
            }
            this.Object = this._Serializer.Deserialize(System.IO.File.ReadAllText(this.File, this.Encoding));
        }

        public void ResetObject()
        {
            this.Object = new T();
            Utilities.EnsureFileExists(this.File);
            SaveObject();
        }

        public void SaveObject()
        {
            this._Serializer.SerializeWithIndent(this.Object);
        }
    }
}
