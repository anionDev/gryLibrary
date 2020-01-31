using GRYLibrary.Core.XMLSerializer.SerializationInfos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.Core.XMLSerializer
{
    public class CustomizableXMLSerializer
    {
        public CustomizableXMLSerializer()
        {
            this.StandardSerializationInfos = new List<CustomXMLSerializer>{
            new StringSerializer(this),
            new ArraySerializer(this),
            new DictionarySerializer(this),
            new ListSerializer(this),
            new SetSerializer(this),
            new EnumerableSerializer(this),
            new KeyValuePairSerializer(this),
        };
        }
        public XmlSchema GenericGetXMLSchema(Type type)
        {
            return null;
        }
        public bool HandlePropertiesWithPublicGetter { get; set; } = true;
        public bool HandlePropertiesWithNonPublicGetter { get; set; } = false;
        public IList<CustomXMLSerializer> StandardSerializationInfos { get; set; }

        public void GenericXMLSerializer(object @object, XmlWriter writer)
        {
            this.GenericXMLSerializer(@object, writer, this.StandardSerializationInfos);
        }
        public void GenericXMLSerializer(object @object, XmlWriter writer, IList<CustomXMLSerializer> serializationInfos)
        {
            GenericXMLSerializer(@object, writer, serializationInfos, @object.GetType());
        }
        public void GenericXMLSerializer(object @object, XmlWriter writer, IList<CustomXMLSerializer> serializationInfos, Type allowedType)
        {
            if (@object == null)
            {
                return;
            }
            Type typeOfObject = @object.GetType();
            if (typeOfObject.IsPrimitive || typeOfObject.IsEnum)
            {
                new SimpleXMLSerializer().SerializeToWriter(@object, writer);
            }
            else
            {
                bool customSerializationInfoHasSerializedObject = false;
                foreach (CustomXMLSerializer serializationInfo in serializationInfos)
                {
                    if (serializationInfo.IsApplicable(@object, allowedType))
                    {
                        serializationInfo.Serialize(@object, writer);
                        writer.Flush();
                        customSerializationInfoHasSerializedObject = true;
                        break;
                    }
                }
                if (!customSerializationInfoHasSerializedObject)
                {
                    writer.WriteStartElement(@object.GetType().Name);//todo handle generic names (e. g. "List`1")
                    foreach (PropertyInfo property in typeOfObject.GetProperties())
                    {
                        if (this.PropertyShouldBeHandled(property))
                        {
                            writer.WriteStartElement(property.Name);
                            this.GenericXMLSerializer(property.GetValue(@object), writer, serializationInfos, property.PropertyType);
                            writer.WriteEndElement();
                            writer.Flush();
                        }
                    }
                    writer.WriteEndElement();
                }
            }
        }

        private bool PropertyShouldBeHandled(PropertyInfo property)
        {
            if (property.GetSetMethod().IsPublic)
            {
                return this.HandlePropertiesWithPublicGetter;
            }
            else
            {
                return this.HandlePropertiesWithNonPublicGetter;
            }
        }

        public void GenericXMLDeserializer(object @object, XmlReader reader)
        {
            this.GenericXMLDeserializer(@object, reader, this.StandardSerializationInfos);
        }
        public void GenericXMLDeserializer(object @object, XmlReader reader, IList<CustomXMLSerializer> serializationInfos)
        {
            foreach (CustomXMLSerializer serializationInfo in serializationInfos)
            {
                if (serializationInfo.IsApplicable(@object,@object.GetType()))
                {
                    serializationInfo.Deserialize(@object, reader);
                    return;
                }
            }
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == @object.GetType().ToString())
            {
                foreach (PropertyInfo property in @object.GetType().GetProperties())
                {
                    if (this.PropertyShouldBeHandled(property))
                    {
                        object propertyValue = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(@object, propertyValue);
                        this.GenericXMLDeserializer(propertyValue, reader, serializationInfos);
                    }
                }
            }
        }
    }
}
