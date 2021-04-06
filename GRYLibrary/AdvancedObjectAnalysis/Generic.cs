﻿using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class Generic
    {
        private static readonly IdGenerator<int> _IdGenerator = IdGenerator.GetDefaultIntIdGenerator();
        private static readonly Dictionary<object, int> _ObjectReferenceHashCodeCache = new(new ReferenceEqualsComparer());
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
            else if (EnumerableTools.TypeIsSet(type))
            {
                type = typeof(ISet<>);
            }
            else if (EnumerableTools.TypeIsList(type))
            {
                type = typeof(IList<>);
            }
            else if (EnumerableTools.TypeIsDictionary(type))
            {
                type = typeof(IDictionary<,>);
            }
            else if (EnumerableTools.TypeIsEnumerable(type))
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
            return new PropertyEqualsCalculator().DefaultEquals(object1, object2);
        }

        public static string GenericToString(object @object, int maxOutputLength = int.MaxValue)
        {
            return AdvancedObjectAnalysis.GenericToString.Instance.ToString(@object, maxOutputLength);
        }

#pragma warning disable IDE0060 // Suppress "Remove unused parameter 'object'"
        public static XmlSchema GenericGetSchema(object @object)
#pragma warning restore IDE0060
        {
            return null;
        }

        public static void GenericWriteXml(object @object, XmlWriter writer)
        {
            GenericXMLSerializer.CreateForObject(@object).Serialize(@object, writer);
        }

        public static void GenericReadXml(object @object, XmlReader reader)
        {
            GenericXMLSerializer genericXMLSerializer = GenericXMLSerializer.CreateForObject(@object);
            genericXMLSerializer.CopyContentOfObject(@object, genericXMLSerializer.Deserialize(reader));
        }
        public static IEnumerable<(object, Type)> IterateOverObjectTransitively(object @object)
        {
            return new PropertyIterator().IterateOverObjectTransitively(@object);
        }

        public static string GenericSerialize(object @object)
        {
            using StringWriter stringWriter = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
            {
                GenericWriteXml(@object, xmlWriter);
            }
            return stringWriter.ToString();
        }
        public static T GenericDeserialize<T>(string serializedObject)
        {
            using XmlReader xmlReader = XmlReader.Create(new StringReader(serializedObject));
            T result = (T)Activator.CreateInstance(typeof(T));
            GenericReadXml(result, xmlReader);
            return result;
        }
    }
}
