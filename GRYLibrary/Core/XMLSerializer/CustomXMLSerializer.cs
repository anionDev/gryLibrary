using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer
{
    public abstract class CustomXMLSerializer
    {
        public abstract bool IsApplicable(object @object, Type allowedType);
        public abstract void Serialize(object @object, XmlWriter writer);
        public abstract void Deserialize(object @object, XmlReader reader);
    }
    public abstract class CustomXMLSerializer<T> : CustomXMLSerializer
    {
        protected CustomizableXMLSerializer CustomizableXMLSerializer { get; set; }
        protected internal abstract T Cast(object @object);
        protected abstract void Serialize(T @object, XmlWriter writer);
        protected abstract void Deserialize(T @object, XmlReader reader);
        public CustomXMLSerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this.CustomizableXMLSerializer = customizableXMLSerializer;
        }
        public sealed override void Serialize(object @object, XmlWriter writer)
        {
            this.Serialize(this.Cast(@object), writer);
        }
        public sealed override void Deserialize(object @object, XmlReader reader)
        {
            this.Deserialize(this.Cast(@object), reader);
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
