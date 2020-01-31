using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class ListSerializer : CustomXMLSerializer<IList<dynamic>>
    {
        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public ListSerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            Type[] interfaces = allowedType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (IsIListOfT(@interface))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsIListOfT(Type @interface)
        {
            if (@interface.Name != "IList`1")
            {
                return false;
            }
            if (@interface.Namespace != "System.Collections")
            {
                return false;
            }
            return true;
        }

        protected internal override IList<dynamic> Cast(object @object)
        {
            return this.CommonEnumerableCastImplementation(@object);
        }

        protected override void Deserialize(IList<dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(IList<dynamic> @object, XmlWriter writer)
        {
            writer.WriteStartElement("List");
            foreach (dynamic item in @object)
            {
                writer.WriteStartElement("Item");
                _CustomizableXMLSerializer.GenericXMLSerializer(item, writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

    }
}
