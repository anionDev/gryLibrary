using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class EnumerableSerializer : CustomXMLSerializer<IEnumerable<dynamic>>
    {
        public EnumerableSerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            return Utilities.InheritsFromIEnumerable(allowedType);
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
            new ListSerializer(this.CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }

    }
}
