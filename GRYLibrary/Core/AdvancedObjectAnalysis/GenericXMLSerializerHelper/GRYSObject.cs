using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.FlatObject;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper
{
    /// <summary>
    /// Represents a GRYSerializedObject.
    /// </summary>
    public class GRYSObject
    {
        public Guid RootObjectId { get; set; }
        public HashSet<FlatObject> Objects { get; set; }
        public static GRYSObject Create(object @object, SerializationConfiguration serializationConfiguration)
        {
            Dictionary<object, FlatObject> dictionary = new Dictionary<object, FlatObject>(new ReferenceEqualsComparer());
            FillDictionary(dictionary, @object, serializationConfiguration);
            GRYSObject result = new GRYSObject
            {
                Objects = new HashSet<FlatObject>(dictionary.Values),
                RootObjectId = dictionary[@object].ObjectId
            };
            return result;
        }

        private static Guid FillDictionary(Dictionary<object, FlatObject> dictionary, object @object, SerializationConfiguration serializationConfiguration)
        {
            if (Utilities.IsDefault(@object))
            {
                return default;
            }
            Type typeOfObject = @object.GetType();
            if (Utilities.ObjectIsKeyValuePair(@object))
            {
                System.Collections.Generic.KeyValuePair<object, object> kvp = Utilities.ObjectToKeyValuePair<object,object>(@object);
                @object = new XMLSerializer.KeyValuePair<object, object>(kvp.Key, kvp.Value);
            }
            if (dictionary.ContainsKey(@object))
            {
                return dictionary[@object].ObjectId;
            }
            else
            {
                FlatObject simplification;
                if (PrimitiveComparer.TypeIsTreatedAsPrimitive(typeOfObject))
                {
                    simplification = new FlatPrimitive();
                }
                else if (Utilities.ObjectIsEnumerable(@object))
                {
                    simplification = new FlatEnumerable();
                }
                else
                {
                    simplification = new FlatComplexObject();
                }
                simplification.ObjectId = Guid.NewGuid();
                simplification.TypeName = @object.GetType().AssemblyQualifiedName;
                dictionary.Add(@object, simplification);
                simplification.Accept(new SerializeVisitor(@object, dictionary, serializationConfiguration));

                return simplification.ObjectId;
            }
        }

        private class DeserializeVisitor : IFlatObjectVisitor
        {
            private readonly IDictionary<Guid, object> _DeserializedObjects;
            public DeserializeVisitor(IDictionary<Guid, object> deserializedObjects)
            {
                this._DeserializedObjects = deserializedObjects;
            }
            public void Handle(FlatComplexObject simplifiedObject)
            {
                object @object = GetDeserialisedObjectOrDefault(simplifiedObject.ObjectId);
                Type typeOfObject = @object.GetType();
                foreach (FlatAttribute attribute in simplifiedObject.Attributes)
                {
                    PropertyInfo property = typeOfObject.GetProperty(attribute.Name);
                    if (property != null)
                    {
                        if (attribute.ObjectId.Equals(default))
                        {
                            property.SetValue(@object, Utilities.GetDefault(property.PropertyType));
                        }
                        else
                        {
                            property.SetValue(@object, this._DeserializedObjects[attribute.ObjectId]);
                        }
                        continue;
                    }

                    FieldInfo field = typeOfObject.GetField(attribute.Name);
                    if (field != null)
                    {
                        if (attribute.ObjectId.Equals(default))
                        {
                            field.SetValue(@object, Utilities.GetDefault(field.FieldType));
                        }
                        else
                        {
                            field.SetValue(@object, this._DeserializedObjects[attribute.ObjectId]);
                        }
                        continue;
                    }

                    throw new KeyNotFoundException($"Can not find attribute '{attribute.Name}' in type '{typeOfObject}'.");
                }
            }

            public void Handle(FlatEnumerable simplifiedEnumerable)
            {
                object enumerable = GetDeserialisedObjectOrDefault(simplifiedEnumerable.ObjectId);
                if (Utilities.IsDefault(enumerable))
                {
                    return;
                }
                Type enumerableType = Type.GetType(simplifiedEnumerable.TypeName);
                bool isDictionaryNotGeneric = Utilities.TypeIsDictionaryNotGeneric(enumerableType);
                bool isDictionaryGeneric = Utilities.TypeIsDictionaryGeneric(enumerableType);
                foreach (Guid itemId in simplifiedEnumerable.Items)
                {
                    object itemForEnumerable;
                    itemForEnumerable = GetDeserialisedObjectOrDefault(itemId);
                    object[] arguments;
                    if (isDictionaryGeneric)
                    {
                        XMLSerializer.KeyValuePair<object, object> gkvp =(XMLSerializer.KeyValuePair<object, object>) itemForEnumerable;
                        arguments = new object[] { gkvp.Key, gkvp.Value };
                    }
                    else if (isDictionaryNotGeneric)
                    {
                        DictionaryEntry keyValuePair = Utilities.ObjectToDictionaryEntry(itemForEnumerable);
                        arguments = new object[] { keyValuePair.Key, keyValuePair.Value };
                    }
                    else
                    {
                        arguments = new object[] { itemForEnumerable };
                    }
                    Utilities.AddItemToEnumerable(enumerable, arguments);
                }
            }
            public void Handle(FlatPrimitive simplifiedPrimitive)
            {
                Utilities.NoOperation();
            }
            private object GetDeserialisedObjectOrDefault(Guid id)
            {
                if (default(Guid).Equals(id))
                {
                    return default;
                }
                else
                {
                    return this._DeserializedObjects[id];
                }
            }
        }
        private class CreateObjectVisitor : IFlatObjectVisitor<object>
        {
            public object Handle(FlatComplexObject simplifiedObject)
            {
                Type type = Type.GetType(simplifiedObject.TypeName);
                return Activator.CreateInstance(type);
            }

            public object Handle(FlatEnumerable simplifiedEnumerable)
            {
                Type typeOfSimplifiedEnumerable = Type.GetType(simplifiedEnumerable.TypeName);
                Type concreteTypeOfEnumerable;
                if (Utilities.TypeIsListGeneric(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(List<>);
                }
                else if (Utilities.TypeIsListNotGeneric(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(ArrayList);
                }
                else if (Utilities.TypeIsSet(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(HashSet<>);
                }
                else if (Utilities.TypeIsDictionaryGeneric(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(Dictionary<,>);
                }
                else if (Utilities.TypeIsDictionaryNotGeneric(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(Hashtable);
                }
                else if (Utilities.TypeIsEnumerable(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(List<>);
                }
                else if (Utilities.TypeIsKeyValuePair(typeOfSimplifiedEnumerable))
                {
                    concreteTypeOfEnumerable = typeof(List<>);
                }
                else
                {
                    throw new ArgumentException($"Unknown type of enumerable: {typeOfSimplifiedEnumerable}");
                }
                if (typeOfSimplifiedEnumerable.GenericTypeArguments.Length > 0)
                {
                    concreteTypeOfEnumerable = concreteTypeOfEnumerable.MakeGenericType(typeOfSimplifiedEnumerable.GenericTypeArguments);
                }
                return Activator.CreateInstance(concreteTypeOfEnumerable);
            }

            public object Handle(FlatPrimitive simplifiedPrimitive)
            {
                return simplifiedPrimitive.Value;
            }
        }
        private class SerializeVisitor : IFlatObjectVisitor
        {
            private readonly object _Object;
            private readonly Dictionary<object, FlatObject> _Dictionary;
            private readonly SerializationConfiguration _SerializationConfiguration;

            public SerializeVisitor(object @object, Dictionary<object, FlatObject> dictionary, SerializationConfiguration serializationConfiguration)
            {
                this._Object = @object;
                this._Dictionary = dictionary;
                this._SerializationConfiguration = serializationConfiguration;
            }

            public void Handle(FlatComplexObject simplifiedObject)
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

            public void Handle(FlatEnumerable simplifiedEnumerable)
            {
                simplifiedEnumerable.Items = new List<Guid>();
                foreach (object @object in Utilities.ObjectToEnumerable<object>(this._Object))
                {
                    simplifiedEnumerable.Items.Add(FillDictionary(this._Dictionary, @object, this._SerializationConfiguration));
                }
            }

            public void Handle(FlatPrimitive simplifiedPrimitive)
            {
                simplifiedPrimitive.Value = this._Object;
            }
        }
        private static void AddSimplifiedAttribute(FlatComplexObject container, string attributeName, Type attributeType, object attributeValue, Dictionary<object, FlatObject> dictionary, SerializationConfiguration serializationConfiguration)
        {
            FlatAttribute attribute = new FlatAttribute
            {
                ObjectId = FillDictionary(dictionary, attributeValue, serializationConfiguration),
                Name = attributeName,
                TypeName = attributeType.AssemblyQualifiedName
            };
            container.Attributes.Add(attribute);
        }

        internal object Get()
        {
            Dictionary<Guid, object> deserializedObjects = new Dictionary<Guid, object>();
            IList<FlatObject> sorted = this.Objects.ToList();
            sorted = sorted.OrderBy((item)=> {
                if (item.TypeName.StartsWith("GRYLibrary.Core.XMLSerializer.KeyValuePair"))
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }).ToList();
            foreach (FlatObject simplified in sorted)
            {
                deserializedObjects.Add(simplified.ObjectId, simplified.Accept(new CreateObjectVisitor()));
            }
            foreach (FlatObject simplified in sorted)
            {
                simplified.Accept(new DeserializeVisitor(deserializedObjects));
            }
            return deserializedObjects[this.RootObjectId];
        }
    }
}
