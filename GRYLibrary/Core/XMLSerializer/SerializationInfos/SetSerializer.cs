using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class SetSerializer : CustomXMLSerializer<ISet<dynamic>>
    {

        public SetSerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            return Utilities.InheritsFromISetOfT(allowedType);
        }
        protected internal override ISet<dynamic> Cast(object @object)
        {
            return new HashSet<dynamic>(this.CommonEnumerableCastImplementation(@object));
        }

        protected override void Deserialize(ISet<dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(ISet<dynamic> @object, XmlWriter writer)
        {
            new ListSerializer(this.CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }
    }
}
