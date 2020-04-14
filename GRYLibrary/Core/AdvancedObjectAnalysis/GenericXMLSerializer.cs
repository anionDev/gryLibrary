using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace GRYLibrary.Core.AdvancedXMLSerialysis
{
    public class GenericXMLSerializer<T>
    {
        public IList<CustomSerializer> CustomSerializer { get; set; } = new List<CustomSerializer>();
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
        };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
        {
            return false;
        };

        public string Serialize(T @object)
        {
            throw new NotImplementedException();
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            throw new NotImplementedException();
        }
        public T Deserialize(string serializedObject)
        {
            throw new NotImplementedException();
        }
        public void Deserialize(string serializedObject, XmlReader reader)
        {
            throw new NotImplementedException();
        }
        public void AddDefaultCustomSerializer()
        {
            CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.ISetOfTSerializer);
            CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IListOfTSerializer);
            CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IDictionaryOfT2Serializer);
            CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.IEnumerableOfTSerializer);
            CustomSerializer.Add(AdvancedXMLSerialysis.CustomSerializer.KeyValuePairfTSerializer);
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
}
