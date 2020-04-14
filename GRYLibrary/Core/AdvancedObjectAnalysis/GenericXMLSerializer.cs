using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
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
            var result = new XmlWriterSettings();
            result.Encoding = this.Encoding;
            result.Indent = this.Indent;
            result.IndentChars = "    ";
            return result;
        }
        public string Serialize(T @object)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.GetXmlWriterSettings()))
                {
                    this.Serialize(@object, xmlWriter);
                }
                return this.Encoding.GetString(memoryStream.ToArray());
            }
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            writer.WriteStartElement("Object");

            writer.WriteStartElement("Type");
            writer.WriteString(@object.GetType().ToString());
            writer.WriteEndElement();

            Guid rootObjectId = default;//todo
            writer.WriteStartElement("RootObjectId");
            writer.WriteString(rootObjectId.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ReferencedObjects");
            //TODO
            writer.WriteEndElement();

            writer.WriteStartElement("AttributeGraph");
            //TODO
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
        public T Deserialize(string serializedObject)
        {
            using (StringReader stringReader = new StringReader(serializedObject))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return this.Deserialize(xmlReader);
                }
            }
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
    public class GenericXMLSerializer
    {
        public static GenericXMLSerializer<object> GetDefaultInstance()
        {
            GenericXMLSerializer<object> result = new GenericXMLSerializer<object>();
            result.AddDefaultCustomSerializer();
            return result;
        }
    }
}
