using System;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class KeyValuePairSerializer : CustomXMLSerializer<KeyValuePair<dynamic, dynamic>>
    {
        public KeyValuePairSerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }

        public override bool IsApplicable(object @object, Type allowedType)
        {
            return this.IsIKeyValuePair(allowedType);
        }
        private bool IsIKeyValuePair(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || !type.IsValueType)
            {
                return false;
            }
            if (type.Name != "KeyValuePair`2")
            {
                return false;
            }
            if (type.Namespace != "System.Collections.Generic")
            {
                return false;
            }
            return true;
        }

        protected override void Deserialize(KeyValuePair<dynamic, dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(KeyValuePair<dynamic, dynamic> @object, XmlWriter writer)
        {
            writer.WriteStartElement("Key");
            this.CustomizableXMLSerializer.GenericXMLSerializer(@object.Key, writer);
            writer.WriteEndElement();
            writer.WriteStartElement("Value");
            this.CustomizableXMLSerializer.GenericXMLSerializer(@object.Value, writer);
            writer.WriteEndElement();
        }

        protected internal override KeyValuePair<dynamic, dynamic> Cast(object @object)
        {
            return new KeyValuePair<dynamic, dynamic>(((dynamic)@object).Key, ((dynamic)@object).Value);
        }
    }
}
