using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class SetSerializer : CustomXMLSerializer<ISet<dynamic>>
    {

        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public SetSerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }
        public override bool IsApplicable(object @object, Type allowedType)
        {
            Type[] interfaces = allowedType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (IsISetOfT(@interface))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsISetOfT(Type @interface)
        {
            if (@interface.Name != "ISet`1")
            {
                return false;
            }
            if (@interface.Namespace != "System.Collections.Generic")
            {
                return false;
            }
            return true;
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
            new ListSerializer(this._CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }
    }
}
