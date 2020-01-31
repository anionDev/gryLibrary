using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class EnumerableSerializer : CustomXMLSerializer<IEnumerable<dynamic>>
    {
        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public EnumerableSerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }
        public override bool IsApplicable(object @object)
        {
            return @object is IEnumerable;
        }

        protected internal override IEnumerable<dynamic> Cast(object @object)
        {
            return this.CommonEnumerableCastImplementation(@object);
        }

        protected override void Deserialize(IEnumerable<dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(IEnumerable<dynamic> @object, XmlWriter writer)
        {
            this.CommonSerializationImplementation(@object, writer, this._CustomizableXMLSerializer);
        }

        public override string GetXMLFriendlyNameOfType(IEnumerable<dynamic> @object)
        {
            return "Enumerable";
        }
    }
}
