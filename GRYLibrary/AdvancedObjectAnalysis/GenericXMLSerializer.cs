using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper;
using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer
    {
#pragma warning disable CA1822 // Mark members as static
        public SerializationConfiguration SerializationConfiguration { get; set; }
        private readonly Type _T;
        public GenericXMLSerializer() : this(typeof(object))
        {
        }
        public GenericXMLSerializer(Type type)
        {
            this._T = type;
            this.SerializationConfiguration = new SerializationConfiguration
            {
                PropertySelector = (PropertyInfo propertyInfo) => { return propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.GetMethod.IsPublic; },
                FieldSelector = (FieldInfo fieldInfo) => { return false; },
                Encoding = new UTF8Encoding(false)
            };
        }

        public static GenericXMLSerializer<object> DefaultInstance()
        {
            return new GenericXMLSerializer<object>();
        }
        internal static GenericXMLSerializer CreateForObject(object @object)
        {
            return new GenericXMLSerializer(@object.GetType());
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
        public string Serialize(object/*T*/ @object)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.GetXmlWriterSettings()))
            {
                this.Serialize(@object, xmlWriter);
            }
            return this.SerializationConfiguration.Encoding.GetString(memoryStream.ToArray());
        }
        public void Serialize(object/*T*/ @object, XmlWriter writer)
        {
            if (@object == null)
            {
                //TODO
            }
            if (!Utilities.IsAssignableFrom(@object, _T))
            {
                throw new ArgumentException($"Can only serilize objects of type {@object.GetType().FullName} but the given object has the type {_T.FullName}");
            }
            object objectForRealSerialization = GRYSObject.Create(@object, this.SerializationConfiguration);
            IEnumerable<(object, Type)> allReferencedObjects = new PropertyIterator().IterateOverObjectTransitively(objectForRealSerialization);
            HashSet<Type> extraTypes = new HashSet<Type>();
            foreach ((object, Type) referencedObject in allReferencedObjects)
            {
                if (referencedObject.Item1 != null && referencedObject.Item1 is IGRYSerializable extraTypesProvider)
                {
                    extraTypes.UnionWith(extraTypesProvider.GetExtraTypesWhichAreRequiredForSerialization());
                }
            }
            GetSerializer().Serialize(writer, objectForRealSerialization);
        }


        public U Deserialize<U>(string serializedObject)
        {
            return (U)this.Deserialize(serializedObject);
        }
        public object/*T*/ Deserialize(string serializedObject)
        {
            using StringReader stringReader = new StringReader(serializedObject);
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return this.Deserialize(xmlReader);
        }
        public object/*T*/ Deserialize(XmlReader reader)
        {
            object result = this.GetSerializer().Deserialize(reader);
            GRYSObject gRYSObject = (GRYSObject)result;
            return gRYSObject.Get();
        }

        private XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(GRYSObject), typeof(GRYSObject).Name);//TODO use extra types
        }

        /// <summary>
        /// Sets the values of all properties of <paramref name="thisObject"/> to the value of the equal property of <paramref name="deserializedObject"/>.
        /// </summary>
        /// <remarks>
        /// This function does not create a deep copy of the property-values. It reassignes only the property-target-objects of <paramref name="thisObject"/>.
        /// If <paramref name="thisObject"/> is an <see cref="IEnumerable"/> then only the references of the items of the enumeration will be copied, no property-values.
        /// </remarks>
        internal void CopyContentOfObject(object thisObject, object deserializedObject)
        {
            bool thisIsNull = thisObject == null;
            bool deserializedObjectIsNull = deserializedObject == null;
            if (thisIsNull && deserializedObjectIsNull)
            {
                return;
            }
            if (thisIsNull && !deserializedObjectIsNull)
            {
                throw new NullReferenceException();
            }
            if (!thisIsNull && deserializedObjectIsNull)
            {
                throw new NullReferenceException();
            }
            if (!thisIsNull && !deserializedObjectIsNull)
            {
                Type type = thisObject.GetType();
                if (Utilities.TypeIsEnumerable(type))
                {
                    foreach (object item in deserializedObject as IEnumerable)
                    {
                        Utilities.AddItemToEnumerable(thisObject, new object[] { item });
                    }
                }
                else
                {
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
        }
#pragma warning restore CA1822 // Mark members as static
    }
    public class GenericXMLSerializer<T> : GenericXMLSerializer
    {
        public T DeserializeTyped(string serializedObject)
        {
            return (T)this.Deserialize(serializedObject);
        }
        public T DeserializeTyped(XmlReader reader)
        {
            return (T)this.Deserialize(reader);
        }
    }
}
