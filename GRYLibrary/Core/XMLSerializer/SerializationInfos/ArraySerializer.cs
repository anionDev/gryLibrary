using System;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class ArraySerializer : CustomXMLSerializer<dynamic[]>
    {
        public ArraySerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            return allowedType.IsArray;
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
            new ListSerializer(this.CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }
        
    }
}
