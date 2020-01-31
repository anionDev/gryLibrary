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
        public override bool IsApplicable(object @object)
        {
            return @object.GetType().IsGenericType && @object.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
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
            this.CommonSerializationImplementation(@object, writer, this._CustomizableXMLSerializer);
        }

        public override string GetXMLFriendlyNameOfType(IList<dynamic> @object)
        {
            return "List";
        }
    }
}
