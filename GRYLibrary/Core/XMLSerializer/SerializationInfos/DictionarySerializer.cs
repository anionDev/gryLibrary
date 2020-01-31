using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer.SerializationInfos
{
    public class DictionarySerializer : CustomXMLSerializer<IDictionary<dynamic, dynamic>>
    {
        private readonly CustomizableXMLSerializer _CustomizableXMLSerializer;

        public DictionarySerializer(CustomizableXMLSerializer customizableXMLSerializer)
        {
            this._CustomizableXMLSerializer = customizableXMLSerializer;
        }

        public override bool IsApplicable(object @object, Type allowedType)
        {
            return Utilities.InheritsFromIDictionaryOfTKeyTValue(allowedType);
        }
        
        protected internal override IDictionary<dynamic, dynamic> Cast(object @object)
        {
            IEnumerable objectAsEnumerable = @object as IEnumerable;
            Dictionary<dynamic, dynamic> result = new Dictionary<dynamic, dynamic>();
            foreach (object item in objectAsEnumerable)
            {
                result.Add(((dynamic)item).Key, ((dynamic)item).Value);
            }
            return result;

        }
        protected override void Deserialize(IDictionary<dynamic, dynamic> @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(IDictionary<dynamic, dynamic> @object, XmlWriter writer)
        {
            new ListSerializer(this._CustomizableXMLSerializer).Serialize(@object.ToList(), writer);
        }

    }
}
