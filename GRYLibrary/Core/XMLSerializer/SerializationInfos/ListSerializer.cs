using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class ListSerializer : CustomXMLSerializer<IList<dynamic>>
    {
        public ListSerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            return Utilities.InheritsFrom(allowedType,typeof(IList<>))
                || Utilities.InheritsFrom(allowedType, typeof(IList));
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
                this.CustomizableXMLSerializer.GenericXMLSerializer(item, writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

    }
}
