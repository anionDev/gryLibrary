using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using System;
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
            if (!_ObjectReferenceHashCodeCache.ContainsKey(@object))
            {
                _ObjectReferenceHashCodeCache.Add(@object, _IdGenerator.GenerateNewId());
            }
            return _ObjectReferenceHashCodeCache[@object];
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
        internal static XmlSchema GenericGetSchema(object @object)
#pragma warning restore IDE0060
        {
            return null;
        }

        internal static void GenericWriteXml(object @object, XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        internal static void GenericReadXml(object @object, XmlReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
