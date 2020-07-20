using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using GRYLibrary.Core.AdvancedXMLSerialysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class Generic
    {
        private static readonly IdGenerator<int> _IdGenerator = IdGenerator.GetDefaultIntIdGenerator();
        private static readonly Dictionary<object, int> _ObjectReferenceHashCodeCache = new Dictionary<object, int>(new ReferenceEqualsComparer());
        public static int GenericGetHashCode(object @object)
        {
            if (@object == null)
            {
                return 684835431;
            }
            Type type = @object.GetType();
            if (PrimitiveComparer.TypeIsTreatedAsPrimitive(type))
            {
                Utilities.NoOperation();
            }
            else if (Utilities.TypeIsSet(type))
            {
                type = typeof(ISet<>);
            }
            else if (Utilities.TypeIsList(type))
            {
                type = typeof(IList<>);
            }
            else if (Utilities.TypeIsDictionary(type))
            {
                type = typeof(IDictionary<,>);
            }
            else if (Utilities.TypeIsEnumerable(type))
            {
                type = typeof(IEnumerable);
            }
            else
            {
                Utilities.NoOperation();
            }
            if (!_ObjectReferenceHashCodeCache.ContainsKey(type))
            {
                _ObjectReferenceHashCodeCache.Add(type, _IdGenerator.GenerateNewId());
            }
            return _ObjectReferenceHashCodeCache[type];
        }

        public static bool GenericEquals(object object1, object object2)
        {
            return new PropertyEqualsCalculator().Equals(object1, object2);
        }

        public static string GenericToString(object @object)
        {
            return AdvancedObjectAnalysis.GenericToString.Instance.ToString(@object);
        }

#pragma warning disable IDE0060 // Suppress "Remove unused parameter 'object'"
        public static XmlSchema GenericGetSchema(object @object)
#pragma warning restore IDE0060
        {
            return null;
        }

        public static void GenericWriteXml(object @object, XmlWriter writer)
        {
            GenericXMLSerializer.DefaultInstance.Serialize(@object, writer);
        }

        public static void GenericReadXml(object @object, XmlReader reader)
        {
            reader.ReadStartElement(@object.GetType().Name);
            GenericXMLSerializer.DefaultInstance.CopyContent(@object, GenericXMLSerializer.DefaultInstance.Deserialize(reader));
        }
    }
}
