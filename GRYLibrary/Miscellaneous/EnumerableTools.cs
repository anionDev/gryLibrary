using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static GRYLibrary.Core.Miscellaneous.Utilities;

namespace GRYLibrary.Core.Miscellaneous
{
    public static class EnumerableTools
    {

        #region IsEnumerable
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IEnumerable"/>.</returns>
        public static bool ObjectIsEnumerable(this object @object)
        {
            return @object is IEnumerable;
        }
        public static bool TypeIsEnumerable(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IEnumerable)) && !typeof(string).Equals(type);
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="ISet{T}"/>.</returns>
        public static bool ObjectIsSet(this object @object)
        {
            return TypeIsSet(@object.GetType());
        }
        public static bool TypeIsSet(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(ISet<>));
        }
        public static bool ObjectIsList(this object @object)
        {
            return TypeIsList(@object.GetType());
        }
        public static bool TypeIsList(this Type type)
        {
            return TypeIsListNotGeneric(type) || TypeIsListGeneric(type);
        }
        public static bool TypeIsListNotGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IList));
        }
        public static bool TypeIsListGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IList<>));
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IDictionary{TKey, TValue}"/> or <see cref="IDictionary"/>.</returns>
        public static bool ObjectIsDictionary(this object @object)
        {
            return TypeIsDictionary(@object.GetType());
        }
        public static void AddItemToEnumerable(object enumerable, object[] addMethodArgument)
        {
            enumerable.GetType().GetMethod("Add").Invoke(enumerable, addMethodArgument);
        }
        public static bool TypeIsDictionary(this Type type)
        {
            return TypeIsDictionaryNotGeneric(type) || TypeIsDictionaryGeneric(type);
        }
        public static bool TypeIsDictionaryNotGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IDictionary));
        }
        public static bool TypeIsDictionaryGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IDictionary<,>));
        }
        public static bool ObjectIsKeyValuePair(this object @object)
        {
            return TypeIsKeyValuePair(@object.GetType());
        }
        public static bool TypeIsKeyValuePair(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(System.Collections.Generic.KeyValuePair<,>)) || TypeIsAssignableFrom(type, typeof(XMLSerializer.KeyValuePair<object, object>));
        }
        public static bool ObjectIsDictionaryEntry(this object @object)
        {
            return TypeIsDictionaryEntry(@object.GetType());
        }
        public static bool TypeIsDictionaryEntry(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(DictionaryEntry));
        }
        public static bool ObjectIsTuple(this object @object)
        {
            return TypeIsTuple(@object.GetType());
        }
        public static bool TypeIsTuple(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(Tuple<,>));
        }

        #endregion
        #region ToEnumerable
        public static IEnumerable ObjectToEnumerable(this object @object)
        {
            if (!ObjectIsEnumerable(@object))
            {
                throw new InvalidCastException();
            }
            return @object as IEnumerable;
        }
        public static IEnumerable<T> ObjectToEnumerable<T>(this object @object)
        {
            if (!ObjectIsEnumerable(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            List<T> result = new List<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        public static ISet<T> ObjectToSet<T>(this object @object)
        {
            if (!ObjectIsSet(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            HashSet<T> result = new HashSet<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IList{T}"/> or <see cref="IList"/>.</returns>
        public static IList ObjectToList(this object @object)
        {
            return ObjectToList<object>(@object).ToList();
        }
        public static IList<T> ObjectToList<T>(this object @object)
        {
            if (!ObjectIsList(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            List<T> result = new List<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        public static IDictionary ObjectToDictionary(this object @object)
        {
            IDictionary result = new Hashtable();
            foreach (System.Collections.Generic.KeyValuePair<object, object> item in ObjectToDictionary<object, object>(@object))
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
        public static IDictionary<TKey, TValue> ObjectToDictionary<TKey, TValue>(this object @object)
        {
            if (!ObjectIsDictionary(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable<object> objects = ObjectToEnumerable<object>(@object);
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            foreach (object obj in objects)
            {
                System.Collections.Generic.KeyValuePair<TKey, TValue> kvp = ObjectToKeyValuePair<TKey, TValue>(obj);
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
        public static System.Collections.Generic.KeyValuePair<TKey, TValue> ObjectToKeyValuePair<TKey, TValue>(this object @object)
        {
            if (!ObjectIsKeyValuePair(@object))
            {
                throw new InvalidCastException();
            }
            return ObjectToKeyValuePairUnsafe<TKey, TValue>(@object);
        }

        internal static System.Collections.Generic.KeyValuePair<TKey, TValue> ObjectToKeyValuePairUnsafe<TKey, TValue>(object @object)
        {
            object key = ((dynamic)@object).Key;
            object value = ((dynamic)@object).Value;
            TKey tKey;
            TValue tValue;

            if (key is TKey key1)
            {
                tKey = key1;
            }
            else if (IsDefault(key))
            {
                tKey = default;
            }
            else
            {
                throw new InvalidCastException();
            }
            if (value is TValue value1)
            {
                tValue = value1;
            }
            else if (IsDefault(value))
            {
                tValue = default;
            }
            else
            {
                throw new InvalidCastException();
            }
            return new System.Collections.Generic.KeyValuePair<TKey, TValue>(tKey, tValue);
        }

        public static DictionaryEntry ObjectToDictionaryEntry(object @object)
        {
            if (!ObjectIsDictionaryEntry(@object))
            {
                throw new InvalidCastException();
            }
            object key = ((dynamic)@object).Key;
            object value = ((dynamic)@object).Value;
            return new DictionaryEntry(key, value);
        }
        public static Tuple<T1, T2> ObjectToTuple<T1, T2>(this object @object)
        {
            if (!ObjectIsTuple(@object))
            {
                throw new InvalidCastException();
            }
            object item1 = ((dynamic)@object).Item1;
            object item2 = ((dynamic)@object).Item2;
            if (item1 is T1 t1 && item2 is T2 t2)
            {
                return new Tuple<T1, T2>(t1, t2);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        #endregion
        #region EqualsEnumerable
        public static bool EnumerableEquals(this IEnumerable enumerable1, IEnumerable enumerable2)
        {
            return new EnumerableComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(enumerable1, enumerable2);
        }
        /// <returns>Returns true if and only if the items in <paramref name="list1"/> and <paramref name="list2"/> are equal (ignoring the order) using the GRYLibrary-AdvancedObjectAnalysis for object-comparison.</returns>
        public static bool SetEquals<T>(this ISet<T> set1, ISet<T> set2)
        {
            return new SetComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(set1, set2);
        }
        public static bool ListEquals(this IList list1, IList list2)
        {
            return new ListComparer(new PropertyEqualsCalculatorConfiguration()).Equals(list1, list2);
        }
        /// <returns>Returns true if and only if the items in <paramref name="list1"/> and <paramref name="list2"/> are equal using the GRYLibrary-AdvancedObjectAnalysis for object-comparison.</returns>
        public static bool ListEquals<T>(this IList<T> list1, IList<T> list2)
        {
            return new ListComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(list1, list2);
        }
        public static bool DictionaryEquals(this IDictionary dictionary1, IDictionary dictionary2)
        {
            return new DictionaryComparer(new PropertyEqualsCalculatorConfiguration()).Equals(dictionary1, dictionary2);
        }
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            return new DictionaryComparer(new PropertyEqualsCalculatorConfiguration()).DefaultEquals(dictionary1, dictionary2);
        }
        public static bool KeyValuePairEquals<TKey, TValue>(this System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair1, System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair2)
        {
            return new KeyValuePairComparer(new PropertyEqualsCalculatorConfiguration()).Equals(keyValuePair1, keyValuePair2);
        }
        public static bool TupleEquals<TKey, TValue>(this Tuple<TKey, TValue> tuple1, Tuple<TKey, TValue> tuple2)
        {
            return new TupleComparer(new PropertyEqualsCalculatorConfiguration()).Equals(tuple1, tuple2);
        }

        #endregion
    }
}
