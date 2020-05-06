using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper
{
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
                if (IsPrimitiveType(typeOfObject))
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

        private static bool IsPrimitiveType(Type typeOfObject)
        {
            return ComparerUtilities.DefaultPrimitiveComparer.IsApplicable(typeOfObject);
        }

        private class DeserializeVisitor : Simplified.ISimplifiedVisitor
        {
            private readonly Dictionary<Guid, object> _DeserializedObjects;
            public DeserializeVisitor(Dictionary<Guid, object> deserializedObjects)
            {
                this._DeserializedObjects = deserializedObjects;
            }
            public void Handle(SimplifiedObject simplifiedObject)
            {
                object @object = this._DeserializedObjects[simplifiedObject.ObjectId];
                Type typeOfObject = @object.GetType();
                foreach (SimplifiedAttribute attribute in simplifiedObject.Attributes)
                {
                    PropertyInfo property = typeOfObject.GetProperty(attribute.Name);
                    if (property != null)
                    {
                        property.SetValue(@object, this._DeserializedObjects[attribute.ObjectId]);
                        continue;
                    }
                    FieldInfo field = typeOfObject.GetField(attribute.Name);
                    if (field == null)
                    {
                        field.SetValue(@object, this._DeserializedObjects[attribute.ObjectId]);
                        continue;
                    }
                    throw new KeyNotFoundException($"Can not find attribute {attribute.Name} in type {typeOfObject}");
                }
            }

            public void Handle(SimplifiedEnumerable simplifiedEnumerable)
            {
                object enumerable = this._DeserializedObjects[simplifiedEnumerable.ObjectId];
                MethodInfo addOperation = enumerable.GetType().GetMethod("Add");
                foreach (Guid item in simplifiedEnumerable.Items)
                {
                    addOperation.Invoke(enumerable, new object[] { this._DeserializedObjects[item] });
                }
            }

            public void Handle(SimplifiedPrimitive simplifiedPrimitive)
            {
                Utilities.NoOperation();
            }
        }
        private class CreateObjectVisitor : Simplified.ISimplifiedVisitor<object>
        {
            public object Handle(SimplifiedObject simplifiedObject)
            {
                Type type = Type.GetType(simplifiedObject.TypeName);
                return Activator.CreateInstance(type);
            }

            public object Handle(SimplifiedEnumerable simplifiedEnumerable)
            {
                Type typeOfSimplifiedEnumerable = Type.GetType(simplifiedEnumerable.TypeName);
                Type ConcreteTypeOfEnumerable;
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

        internal object Get()
        {
            Dictionary<Guid, object> deserializedObjects = new Dictionary<Guid, object>();
            foreach (Simplified simplified in this.Objects)
            {
                deserializedObjects.Add(simplified.ObjectId, simplified.Accept(new CreateObjectVisitor()));
            }
            foreach (Simplified simplified in this.Objects)
            {
                simplified.Accept(new DeserializeVisitor(deserializedObjects));
            }
            return deserializedObjects[this.RootObjectId];
        }


    }

}
