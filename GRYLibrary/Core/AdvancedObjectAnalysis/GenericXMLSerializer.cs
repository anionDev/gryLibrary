using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        XmlSerializer xmlSerializer { get; set; } = new XmlSerializer(typeof(T));
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public bool Indent { get; set; } = true;
        public IList<CustomSerializer> CustomSerializer { get; set; } = new List<CustomSerializer>();
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
        };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
            {
                return false;
            };
        private XmlWriterSettings GetXmlWriterSettings()
        {
            XmlWriterSettings result = new XmlWriterSettings();
            result.Encoding = this.Encoding;
            result.Indent = this.Indent;
            result.IndentChars = "    ";
            return result;
        }
        public string Serialize(T @object)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.GetXmlWriterSettings()))
            {
                this.Serialize(@object, xmlWriter);
            }
            return this.Encoding.GetString(memoryStream.ToArray());
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            xmlSerializer.Serialize(writer, GenericallySerializedObject.Create(@object, PropertySelector, FieldSelector));
        }
        public T Deserialize(string serializedObject)
        {
            using StringReader stringReader = new StringReader(serializedObject);
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return this.Deserialize(xmlReader);
        }
        public T Deserialize(XmlReader reader)
        {
            throw new NotImplementedException();
        }
        public void AddDefaultCustomSerializer()
        {
            this.CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.ISetOfTSerializer);
            this.CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IListOfTSerializer);
            this.CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IDictionaryOfT2Serializer);
            this.CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IEnumerableOfTSerializer);
            this.CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.KeyValuePairfTSerializer);
        }
    }
    public class GenericXMLSerializer
    {
        public static GenericXMLSerializer<object> GetDefaultInstance()
        {
            GenericXMLSerializer<object> result = new GenericXMLSerializer<object>();
            result.AddDefaultCustomSerializer();
            return result;
        }
    }
    #region internal Helper types for serialized content
    public class GenericallySerializedObject
    {
        public HashSet<SimplifiedObject> SimplifiedObject { get; set; }
        public Guid RootObjectId { get; set; }
        public static GenericallySerializedObject Create(object @object, Func<PropertyInfo, bool> propertySelector, Func<FieldInfo, bool> fieldSelector)
        {
            Dictionary<object, SimplifiedObject> dictionary = new Dictionary<object, SimplifiedObject>(ReferenceEqualsComparer.Instance);
            FillDictionary(dictionary, @object, propertySelector, fieldSelector);
            GenericallySerializedObject result = new GenericallySerializedObject();
            result.SimplifiedObject = new HashSet<SimplifiedObject>(dictionary.Values);
            result.RootObjectId = dictionary[@object].ObjectId;
            return result;
        }

        private static void FillDictionary(Dictionary<object, SimplifiedObject> dictionary, object @object, Func<PropertyInfo, bool> propertySelector, Func<FieldInfo, bool> fieldSelector)
        {
            if (!dictionary.ContainsKey(@object))
            {
                SimplifiedObject simplification = new SimplifiedObject();
                simplification.ObjectId = Guid.NewGuid();
                dictionary.Add(@object, simplification);
                foreach (PropertyInfo property in @object.GetType().GetProperties())
                {
                    if (propertySelector(property))
                    {
                        AddSimplifiedAttribute(simplification, property.Name, property.GetValue(@object), property.PropertyType, dictionary);
                    }
                }
                foreach (FieldInfo field in @object.GetType().GetFields())
                {
                    if (fieldSelector(field))
                    {
                        AddSimplifiedAttribute(simplification, field.Name, field.GetValue(@object), field.FieldType, dictionary);
                    }
                }
            }
        }

        private static void AddSimplifiedAttribute(SimplifiedObject parent, string name, object @object, Type propertyType, Dictionary<object, SimplifiedObject> dictionary)
        {
            throw new NotImplementedException();
        }
    }
    public class SimplifiedObject
    {
        public Guid ObjectId { get; set; }
        public Type Type { get; set; }
        public List<Attribute> Attribute { get; set; }
        public override bool Equals(object obj)
        {
            SimplifiedObject typedObject = obj as SimplifiedObject;
            if (typedObject == null)
            {
                return false;
            }
            else
            {
                return this.ObjectId.Equals(typedObject.ObjectId);
            }
        }
        public override int GetHashCode()
        {
            return this.ObjectId.GetHashCode();
        }
    }
    public abstract class Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
    public abstract class PrimitiveTarget : Attribute
    {
        public object Value { get; set; }
    }
    public abstract class ComplexTarget : Attribute
    {
        public Guid ObjectId { get; set; }
    }

    #endregion
    public class CustomSerializer
    {
        public Func<object, string> Serialize { get; set; }
        public Func<string, object> Deserialize { get; set; }
        public Func<PropertyInfo, bool> IsApplicable { get; set; }
        public CustomSerializer(Func<PropertyInfo, bool> isApplicable, Func<object, string> serialize, Func<string, object> deserialize)
        {
            this.IsApplicable = isApplicable;
            this.Serialize = serialize;
            this.Deserialize = deserialize;
        }
        #region ISetOfTSerializer
        public static CustomSerializer ISetOfTSerializer { get; } = new CustomSerializer(
            (info) =>/*IsApplicable*/
            {
                throw new NotImplementedException();
            }, (@object) =>/*Serialize*/
            {
                throw new NotImplementedException();
            }, (serializedObject) =>/*Deserialize*/
            {
                throw new NotImplementedException();
            });
        #endregion
        #region IListOfTSerializer
        public static CustomSerializer IListOfTSerializer { get; } = new CustomSerializer(
            (info) =>/*IsApplicable*/
            {
                throw new NotImplementedException();
            }, (@object) =>/*Serialize*/
            {
                throw new NotImplementedException();
            }, (serializedObject) =>/*Deserialize*/
            {
                throw new NotImplementedException();
            });
        #endregion
        #region IDictionaryOfT2Serializer
        public static CustomSerializer IDictionaryOfT2Serializer { get; } = new CustomSerializer(
            (info) =>/*IsApplicable*/
            {
                throw new NotImplementedException();
            }, (@object) =>/*Serialize*/
            {
                throw new NotImplementedException();
            }, (serializedObject) =>/*Deserialize*/
            {
                throw new NotImplementedException();
            });
        #endregion
        #region IEnumerableOfTSerializer
        public static CustomSerializer IEnumerableOfTSerializer { get; } = new CustomSerializer(
            (info) =>/*IsApplicable*/
            {
                throw new NotImplementedException();
            }, (@object) =>/*Serialize*/
            {
                throw new NotImplementedException();
            }, (serializedObject) =>/*Deserialize*/
            {
                throw new NotImplementedException();
            });
        #endregion
        #region KeyValuePairfTSerializer
        public static CustomSerializer KeyValuePairfTSerializer { get; } = new CustomSerializer(
            (info) =>/*IsApplicable*/
            {
                throw new NotImplementedException();
            }, (@object) =>/*Serialize*/
            {
                throw new NotImplementedException();
            }, (serializedObject) =>/*Deserialize*/
            {
                throw new NotImplementedException();
            });
        #endregion
    }
}
