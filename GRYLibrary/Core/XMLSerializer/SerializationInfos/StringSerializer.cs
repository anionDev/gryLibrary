using System;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class StringSerializer : CustomXMLSerializer<string>
    {
        public StringSerializer(CustomizableXMLSerializer customizableXMLSerializer) : base(customizableXMLSerializer)
        {
        }

        public override bool IsApplicable(object @object, Type allowedType)
        {
            return typeof(string).Equals(@object.GetType());
        }

        protected internal override string Cast(object @object)
        {
            return (string)@object;
        }

        protected override void Deserialize(string @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(string @object, XmlWriter writer)
        {
            writer.WriteCData(@object);
        }

    }
}
