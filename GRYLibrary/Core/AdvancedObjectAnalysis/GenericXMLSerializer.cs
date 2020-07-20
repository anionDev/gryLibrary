﻿using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        public SerializationConfiguration SerializationConfiguration { get; set; }
        public GenericXMLSerializer()
        {
            this.SerializationConfiguration = new SerializationConfiguration
            {
                XmlSerializer = new XmlSerializer(typeof(GRYSObject)),
                PropertySelector = (PropertyInfo propertyInfo) => { return propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.GetMethod.IsPublic; },
                FieldSelector = (FieldInfo fieldInfo) => { return false; },
                Encoding = new UTF8Encoding(false)
            };
        }

        private XmlWriterSettings GetXmlWriterSettings()
        {
            XmlWriterSettings result = new XmlWriterSettings
            {
                Encoding = this.SerializationConfiguration.Encoding,
                Indent = this.SerializationConfiguration.Indent,
                IndentChars = "    "
            };
            return result;
        }
        public string Serialize(T @object)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.GetXmlWriterSettings()))
            {
                this.Serialize(@object, xmlWriter);
            }
            return this.SerializationConfiguration.Encoding.GetString(memoryStream.ToArray());
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            GRYSObject genericallySerializedObject = GRYSObject.Create(@object, this.SerializationConfiguration);
            this.SerializationConfiguration.XmlSerializer.Serialize(writer, genericallySerializedObject);
        }
        public U Deserialize<U>(string serializedObject)
        {
            return (U)(object)this.Deserialize(serializedObject);
        }
        public T Deserialize(string serializedObject)
        {
            using StringReader stringReader = new StringReader(serializedObject);
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return this.Deserialize(xmlReader);
        }
        public T Deserialize(XmlReader reader)
        {
            GRYSObject grySerializedObject = (GRYSObject)this.SerializationConfiguration.XmlSerializer.Deserialize(reader);
            return (T)grySerializedObject.Get();
        }
        /// <summary>
        /// Sets the values of all properties of <paramref name="thisObject"/> to the value of the equal property of <paramref name="deserializedObject"/>.
        /// </summary>
        /// <remarks>This function does not create a deep copy of the property-values. It reassignes only the property-target-objects of <paramref name="thisObject"/>.</remarks>
        internal void CopyContent(object thisObject, object deserializedObject)
        {
            Type type = thisObject.GetType();
            foreach (FieldInfo field in type.GetFields().Where((field) => this.SerializationConfiguration.FieldSelector(field)))
            {
                field.SetValue(thisObject, field.GetValue(deserializedObject));
            }
            foreach (PropertyInfo property in type.GetProperties().Where((property) => this.SerializationConfiguration.PropertySelector(property)))
            {
                property.SetValue(thisObject, property.GetValue(deserializedObject));
            }
        }
    }
    public static class GenericXMLSerializer
    {
        public static GenericXMLSerializer<object> DefaultInstance { get; } = new GenericXMLSerializer<object>();
    }
}
