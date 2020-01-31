using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer
{
    public abstract class CustomXMLSerializer
    {
        public abstract bool IsApplicable(object @object);
        public abstract string GetXMLFriendlyNameOfType(object @object);
        public abstract void Serialize(object @object, XmlWriter writer);
        public abstract void Deserialize(object @object, XmlReader reader);
    }
    public abstract class CustomXMLSerializer<T> : CustomXMLSerializer
    {
        protected internal abstract T Cast(object @object);
        protected abstract void Serialize(T @object, XmlWriter writer);
        protected abstract void Deserialize(T @object, XmlReader reader);
        public abstract string GetXMLFriendlyNameOfType(T @object);
        public sealed override void Serialize(object @object, XmlWriter writer)
        {
            this.Serialize(this.Cast(@object), writer);
        }
        public sealed override void Deserialize(object @object, XmlReader reader)
        {
            this.Deserialize(this.Cast(@object), reader);
        }
        public override string GetXMLFriendlyNameOfType(object @object)
        {
            return this.GetXMLFriendlyNameOfType(this.Cast(@object));
        }
        protected void CommonSerializationImplementation(IEnumerable<dynamic> @object, XmlWriter writer, CustomizableXMLSerializer customizableXMLSerializer)
        {
            foreach (dynamic item in @object)
            {
                writer.WriteStartElement("Item");
                customizableXMLSerializer.GenericXMLSerializer(item, writer);
                writer.WriteEndElement();
            }
        }
        protected IList<dynamic> CommonEnumerableCastImplementation(object @object)
        {
            List<dynamic> result = new List<dynamic>();
            foreach (object item in @object as IEnumerable)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
