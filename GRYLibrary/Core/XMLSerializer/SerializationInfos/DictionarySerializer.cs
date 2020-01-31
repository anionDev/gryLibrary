using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class DictionarySerializer : CustomXMLSerializer<IDictionary<dynamic, dynamic>>
    {
        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public DictionarySerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }

        public override bool IsApplicable(object @object)
        {
            Type[] interfaces = @object.GetType().GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (IsIDictionaryOfKeyValue(@interface))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsIDictionaryOfKeyValue(Type @interface)
        {
            if (@interface.Name != "IDictionary`2")
            {
                return false;
            }
            if (@interface.Namespace != "System.Collections.Generic")
            {
                return false;
            }
            return true;
        }

        protected internal override IDictionary<dynamic, dynamic> Cast(object @object)
        {
            IEnumerable objectAsEnumerable = @object as IEnumerable;
            Dictionary<dynamic, dynamic> result = new Dictionary<dynamic, dynamic>();
            foreach (object item in objectAsEnumerable)
            {
                result.Add(((dynamic)item).Key, ((dynamic)item).Value);
            }
            return result;

        }
        protected override void Deserialize(IDictionary<dynamic, dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(IDictionary<dynamic, dynamic> @object, XmlWriter writer)
        {
            foreach (KeyValuePair<dynamic, dynamic> kvp in @object)
            {
                writer.WriteStartElement("Entry");
                writer.WriteStartElement("Key");
                this._CustomizableXMLSerializer.GenericXMLSerializer(kvp.Key, writer);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                this._CustomizableXMLSerializer.GenericXMLSerializer(kvp.Value, writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public override string GetXMLFriendlyNameOfType(IDictionary<dynamic, dynamic> @object)
        {
            return "Dictionary";
        }

    }
}
