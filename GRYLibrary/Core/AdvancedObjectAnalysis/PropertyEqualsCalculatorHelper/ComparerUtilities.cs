using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class ComparerUtilities
    {
        public static readonly CustomComparer DefaultPrimitiveComparer = new CustomComparer(
            (Type objType) => objType.IsPrimitive || objType.Equals(typeof(string)) || objType.IsValueType,
            (object object1, object object2) =>
            {
                return object1.Equals(object2);
            });

        public static readonly CustomComparer DefaultListComparer = new CustomComparer(
            (Type objType) => objType.TypeIsList(),
            (object object1, object object2) =>
            {
                IList<object> object1AsList = object1.ObjectToList<object>();
                IList<object> object2AsList = object1.ObjectToList<object>();
                return object1AsList.SequenceEqual(object2AsList, PropertyEqualsCalculator.DefaultInstance);
            });

        public static readonly CustomComparer DefaultSetComparer = new CustomComparer(
            (Type objType) => objType.TypeIsSet(),
            (object object1, object object2) =>
            {
                ISet<object> object1AsSet = object1.ObjectToSet<object>();
                ISet<object> object2AsSet = object2.ObjectToSet<object>();
                return object1AsSet.SetEquals(object2AsSet, PropertyEqualsCalculator.DefaultInstance);
            });

        public static readonly CustomComparer DefaultDictionaryComparer = new CustomComparer(
            (Type objType) => objType.TypeIsDictionary(),
            (object object1, object object2) =>
            {
                IDictionary<object, object> object1AsDictionary = object1.ObjectToDictionary<object, object>();
                IDictionary<object, object> object2AsDictionary = object1.ObjectToDictionary<object, object>();
                return object1AsDictionary.DictionaryEquals(object2AsDictionary, KeyValuePairComparer.DefaultInstance);
            });

        public static readonly CustomComparer DefaultKeyValuePairComparer = new CustomComparer(
               (Type objType) => objType.TypeIsKeyValuePair(),
               (object object1, object object2) =>
               {
                   KeyValuePair<object, object> object1AsKeyValuePair = object1.ObjectToKeyValuePair<object, object>();
                   KeyValuePair<object, object> object2AsKeyValuePair = object2.ObjectToKeyValuePair<object, object>();
                   return KeyValuePairComparer.DefaultInstance.Equals(object1AsKeyValuePair, object2AsKeyValuePair);
               });

        public static readonly CustomComparer DefaultEnumerableComparer = new CustomComparer(
            (Type objType) => objType.TypeIsEnumerable(),
            (object object1, object object2) =>
            {
                IEnumerable<object> object1AsEnumerable = object1.ObjectToEnumerableGeneric<object>();
                IEnumerable<object> object2AsEnumerable = object1.ObjectToEnumerableGeneric<object>();
                return object1AsEnumerable.EnumerableEquals(object2AsEnumerable, PropertyEqualsCalculator.DefaultInstance);
            });


    }

}
