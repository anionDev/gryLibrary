using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
         public override bool IsApplicable(object @object, Type allowedType)
        {
            Type[] interfaces = allowedType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (IsIEnumerable(@interface))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsIEnumerable(Type @interface)
        {
            if (@interface.Name != "IEnumerable")
            {
                return false;
            }
            if (@interface.Namespace != "System.Collections")
            {
                return false;
            }
            return true;
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
            new ListSerializer(this._CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }

    }
}
