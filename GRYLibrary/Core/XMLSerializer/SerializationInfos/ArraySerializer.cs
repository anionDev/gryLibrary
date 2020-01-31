using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class ArraySerializer : CustomXMLSerializer<dynamic[]>
    {
        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public ArraySerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }

        public override bool IsApplicable(object @object)
        {
            return @object.GetType().IsArray;
        }

        protected internal override dynamic[] Cast(object @object)
        {
            return this.CommonEnumerableCastImplementation(@object).ToArray();
        }

        protected override void Deserialize(dynamic[] @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(dynamic[] @object, XmlWriter writer)
        {
            this.CommonSerializationImplementation(@object, writer, this._CustomizableXMLSerializer);
        }

        public override string GetXMLFriendlyNameOfType(dynamic[] @object)
        {
            return "Array";
        }

    }
}
