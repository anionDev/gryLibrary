using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
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
            this.SerializationConfiguration = new SerializationConfiguration();
            this.SerializationConfiguration.XmlSerializer = new XmlSerializer(typeof(GRYSerializedObject));
            this.SerializationConfiguration.PropertySelector = (PropertyInfo propertyInfo) => { return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic; };
            this.SerializationConfiguration.FieldSelector = (FieldInfo propertyInfo) => { return false; };
            this.SerializationConfiguration.Encoding = new UTF8Encoding(false);
        }

        private XmlWriterSettings GetXmlWriterSettings()
        {
            XmlWriterSettings result = new XmlWriterSettings();
            result.Encoding = this.SerializationConfiguration.Encoding;
            result.Indent = this.SerializationConfiguration.Indent;
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
            return this.SerializationConfiguration.Encoding.GetString(memoryStream.ToArray());
        }
        public void Serialize(T @object, XmlWriter writer)
        {
            GRYSerializedObject genericallySerializedObject = GRYSerializedObject.Create(@object, this.SerializationConfiguration);
            this.SerializationConfiguration.XmlSerializer.Serialize(writer, genericallySerializedObject);
        }
        public T Deserialize(string serializedObject)
        {
            using StringReader stringReader = new StringReader(serializedObject);
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return this.Deserialize(xmlReader);
        }
        public T Deserialize(XmlReader reader)
        {
            GRYSerializedObject grySerializedObject = (GRYSerializedObject)this.SerializationConfiguration.XmlSerializer.Deserialize(reader);
            return (T)grySerializedObject.Get(this.SerializationConfiguration);
        }
    }
    public class GenericXMLSerializer
    {
        public static GenericXMLSerializer<object> GetDefaultInstance()
        {
            GenericXMLSerializer<object> result = new GenericXMLSerializer<object>();
            return result;
        }
    }
    #region internal Helper types for serialized content
    public class GRYSerializedObject
    {
        public Guid RootObjectId { get; set; }
        public HashSet<Simplified> Objects { get; set; }
        public static GRYSerializedObject Create(object @object, SerializationConfiguration serializationConfiguration)
        {
            Dictionary<object, Simplified> dictionary = new Dictionary<object, Simplified>(ReferenceEqualsComparer.Instance);
            FillDictionary(dictionary, @object, serializationConfiguration);
            GRYSerializedObject result = new GRYSerializedObject();
            result.Objects = new HashSet<Simplified>(dictionary.Values);
            result.RootObjectId = dictionary[@object].ObjectId;
            return result;
        }

        private static Guid FillDictionary(Dictionary<object, Simplified> dictionary, object @object, SerializationConfiguration serializationConfiguration)
        {
            if (@object == null)
            {
                return default;
            }
            Type typeOfObject = @object.GetType();
            if (!typeOfObject.IsPublic)
            {
                throw new SerializationException($"Object of type {typeOfObject} can not be serialized because the type is not pubilc");
            }
            if (dictionary.ContainsKey(@object))
            {
                return dictionary[@object].ObjectId;
            }
            else
            {
                Simplified simplification;
                if (IsPrimitiveType(typeOfObject, serializationConfiguration))
                {
                    simplification = new SimplifiedPrimitive();
                }
                else if (Utilities.ObjectIsEnumerable(@object))
                {
                    simplification = new SimplifiedEnumerable();
                }
                else
                {
                    simplification = new SimplifiedObject();
                }
                simplification.ObjectId = Guid.NewGuid();
                simplification.TypeName = @object.GetType().AssemblyQualifiedName;
                dictionary.Add(@object, simplification);
                simplification.Accept(new SerializeVisitor(@object, dictionary, serializationConfiguration));

                return simplification.ObjectId;
            }
        }

        private static bool IsPrimitiveType(Type typeOfObject, SerializationConfiguration serializationConfiguration)
        {
            return typeOfObject.IsPrimitive || typeOfObject.Equals(typeof(string));
        }

        private class DeserializeVisitor : Simplified.ISimplifiedVisitor
        {
            private readonly SerializationConfiguration _SerializationConfiguration;
            private readonly Dictionary<Guid, object> _DeserializedObjects;
            private readonly GRYSerializedObject _GRYSerializedObject;
            public DeserializeVisitor(Dictionary<Guid, object> deserializedObjects, GRYSerializedObject gRYSerializedObject, SerializationConfiguration serializationConfiguration)
            {
                this._SerializationConfiguration = serializationConfiguration;
                this._DeserializedObjects = deserializedObjects;
                this._GRYSerializedObject = gRYSerializedObject;
            }
            public void Handle(SimplifiedObject simplifiedObject)
            {
                object @object = _DeserializedObjects[simplifiedObject.ObjectId];
                Type typeOfObject = @object.GetType();
                foreach (SimplifiedAttribute attribute in simplifiedObject.Attributes)
                {
                    PropertyInfo property = typeOfObject.GetProperty(attribute.Name);
                    if (property != null)
                    {
                        property.SetValue(@object, _DeserializedObjects[attribute.ObjectId]);
                        return;
                    }
                    FieldInfo field = typeOfObject.GetField(attribute.Name);
                    if (field == null)
                    {
                        field.SetValue(@object, _DeserializedObjects[attribute.ObjectId]);
                        return;
                    }
                    throw new KeyNotFoundException($"Can not find attribute {attribute.Name} in type {typeOfObject}");
                }
            }

            public void Handle(SimplifiedEnumerable simplifiedEnumerable)
            {
                object enumerable = _DeserializedObjects[simplifiedEnumerable.ObjectId];
                MethodInfo addOperation = enumerable.GetType().GetMethod("Add");
                foreach (Guid item in simplifiedEnumerable.Items)
                {
                    addOperation.Invoke(enumerable, new object[] { _DeserializedObjects[item] });
                }
            }

            public void Handle(SimplifiedPrimitive simplifiedPrimitive)
            {
                Utilities.NoOperation();
            }
        }
        private class CreateObjectVisitor : Simplified.ISimplifiedVisitor<object>
        {
            private readonly SerializationConfiguration _SerializationConfiguration;
            public CreateObjectVisitor(SerializationConfiguration serializationConfiguration)
            {
                this._SerializationConfiguration = serializationConfiguration;
            }
            public object Handle(SimplifiedObject simplifiedObject)
            {
                Type type = Type.GetType(simplifiedObject.TypeName);
                return Activator.CreateInstance(type);
            }

            public object Handle(SimplifiedEnumerable simplifiedEnumerable)
            {
                Type typeOfSimplifiedEnumerable = Type.GetType(simplifiedEnumerable.TypeName);
                Type ConcreteTypeOfEnumerable = null;
                if (Utilities.TypeIsList(typeOfSimplifiedEnumerable))
                {
                    ConcreteTypeOfEnumerable = typeof(List<>);
                }
                else if (Utilities.TypeIsSet(typeOfSimplifiedEnumerable))
                {
                    ConcreteTypeOfEnumerable = typeof(HashSet<>);
                }
                else if (Utilities.TypeIsDictionary(typeOfSimplifiedEnumerable))
                {
                    ConcreteTypeOfEnumerable = typeof(Dictionary<,>);
                }
                else if (Utilities.TypeIsEnumerable(typeOfSimplifiedEnumerable))
                {
                    ConcreteTypeOfEnumerable = typeof(List<>);
                }
                else
                {
                    throw new NotImplementedException();
                }
                ConcreteTypeOfEnumerable = ConcreteTypeOfEnumerable.MakeGenericType(typeOfSimplifiedEnumerable.GenericTypeArguments);
                return Activator.CreateInstance(ConcreteTypeOfEnumerable);
            }

            public object Handle(SimplifiedPrimitive simplifiedPrimitive)
            {
                return simplifiedPrimitive.Value;
            }
        }
        private class SerializeVisitor : Simplified.ISimplifiedVisitor
        {
            private readonly object _Object;
            private readonly Dictionary<object, Simplified> _Dictionary;
            private readonly SerializationConfiguration _SerializationConfiguration;

            public SerializeVisitor(object @object, Dictionary<object, Simplified> dictionary, SerializationConfiguration serializationConfiguration)
            {
                this._Object = @object;
                this._Dictionary = dictionary;
                this._SerializationConfiguration = serializationConfiguration;
            }

            public void Handle(SimplifiedObject simplifiedObject)
            {
                foreach (PropertyInfo property in this._Object.GetType().GetProperties())
                {
                    if (this._SerializationConfiguration.PropertySelector(property))
                    {
                        AddSimplifiedAttribute(simplifiedObject, property.Name, property.PropertyType, property.GetValue(this._Object), this._Dictionary, this._SerializationConfiguration);
                    }
                }
                foreach (FieldInfo field in this._Object.GetType().GetFields())
                {
                    if (this._SerializationConfiguration.FieldSelector(field))
                    {
                        AddSimplifiedAttribute(simplifiedObject, field.Name, field.FieldType, field.GetValue(this._Object), this._Dictionary, this._SerializationConfiguration);
                    }
                }
            }

            public void Handle(SimplifiedEnumerable simplifiedEnumerable)
            {
                simplifiedEnumerable.Items = new List<Guid>();
                foreach (object @object in Utilities.ObjectToEnumerableGeneric<object>(this._Object))
                {
                    simplifiedEnumerable.Items.Add(FillDictionary(this._Dictionary, @object, this._SerializationConfiguration));
                }
            }

            public void Handle(SimplifiedPrimitive simplifiedPrimitive)
            {
                simplifiedPrimitive.Value = this._Object;
            }
        }
        private static void AddSimplifiedAttribute(SimplifiedObject container, string attributeName, Type attributeType, object attributeValue, Dictionary<object, Simplified> dictionary, SerializationConfiguration serializationConfiguration)
        {
            SimplifiedAttribute attribute = new SimplifiedAttribute();
            attribute.ObjectId = FillDictionary(dictionary, attributeValue, serializationConfiguration);
            attribute.Name = attributeName;
            attribute.TypeName = attributeType.AssemblyQualifiedName;
            container.Attributes.Add(attribute);
        }

        internal object Get(SerializationConfiguration serializationConfiguration)
        {
            Dictionary<Guid, object> deserializedObjects = new Dictionary<Guid, object>();
            foreach (Simplified simplified in this.Objects)
            {
                deserializedObjects.Add(simplified.ObjectId, simplified.Accept(new CreateObjectVisitor(serializationConfiguration)));
            }
            foreach (Simplified simplified in this.Objects)
            {
                simplified.Accept(new DeserializeVisitor(deserializedObjects, this, serializationConfiguration));
            }
            return deserializedObjects[this.RootObjectId];
        }


    }
    public class SimplifiedObject : Simplified
    {
        public List<SimplifiedAttribute> Attributes { get; set; } = new List<SimplifiedAttribute>();

        public override void Accept(ISimplifiedVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISimplifiedVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
    public class SimplifiedEnumerable : Simplified
    {
        public List<Guid> Items { get; set; }

        public override void Accept(ISimplifiedVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISimplifiedVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }

    public class SimplifiedPrimitive : Simplified
    {
        public object Value { get; set; }
        public override void Accept(ISimplifiedVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISimplifiedVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
    [XmlInclude(typeof(SimplifiedEnumerable))]
    [XmlInclude(typeof(SimplifiedObject))]
    [XmlInclude(typeof(SimplifiedPrimitive))]
    public abstract class Simplified
    {
        public Guid ObjectId { get; set; }
        public string TypeName { get; set; }
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
            return HashCode.Combine(this.ObjectId);
        }
        public abstract void Accept(ISimplifiedVisitor visitor);
        public abstract T Accept<T>(ISimplifiedVisitor<T> visitor);
        public interface ISimplifiedVisitor
        {
            void Handle(SimplifiedObject simplifiedObject);
            void Handle(SimplifiedEnumerable simplifiedEnumerable);
            void Handle(SimplifiedPrimitive simplifiedPrimitive);
        }
        public interface ISimplifiedVisitor<T>
        {
            T Handle(SimplifiedObject simplifiedObject);
            T Handle(SimplifiedEnumerable simplifiedEnumerable);
            T Handle(SimplifiedPrimitive simplifiedPrimitive);
        }
    }
    public class SimplifiedAttribute
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public Guid ObjectId { get; set; }
    }

    #endregion
    public class SerializationConfiguration
    {
        public Func<PropertyInfo, bool> PropertySelector { get; set; }
        public Func<FieldInfo, bool> FieldSelector { get; set; }
        public XmlSerializer XmlSerializer { get; set; }
        public Encoding Encoding { get; set; }
        public bool Indent { get; set; } = true;
    }
}
