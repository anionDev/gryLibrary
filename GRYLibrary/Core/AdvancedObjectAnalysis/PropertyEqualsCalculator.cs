using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class PropertyEqualsCalculator : IEqualityComparer<object>
    {
        private readonly Dictionary<object, int> _HashCodes = new Dictionary<object, int>(new ReferenceEqualsComparer());

        public System.Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
        };
        public System.Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
        {
            return false;
        };
        public PropertyEqualsCalculator()
        {
        }
        public new bool Equals(object object1, object object2)
        {
            return this.Equals(object1, object2, new HashSet<Tuple>());
        }
        private bool Equals(object object1, object object2, ISet<Tuple> visitedObjects)
        {
            bool object1IsDefault = Utilities.IsDefault(object1);
            bool object2IsDefault = Utilities.IsDefault(object2);
            if (object1IsDefault == false && object2IsDefault == false)
            {
                System.Type object1Type = object1.GetType();
                System.Type object2Type = object2.GetType();
                if (visitedObjects.Contains(new Tuple(object1, object2)))
                {
                    return true;
                }
                if (this.SpecialComparerShouldBeApplied(object1Type, object2Type, out System.Func<object, object, bool> specialComparer))
                {
                    bool result = specialComparer(object1, object2);
                    if (result)
                    {
                        visitedObjects.Add(new Tuple(object1, object2));
                    }
                    return result;
                }
                if (object1Type.Equals(object2Type))
                {
                    System.Type type = object1Type;
                    List<WriteableTuple<object, object>> propertyValues = new List<WriteableTuple<object, object>>();
                    foreach (FieldInfo field in type.GetFields())
                    {
                        if (this.FieldSelector(field))
                        {
                            propertyValues.Add(new WriteableTuple<object, object>(field.GetValue(object1), field.GetValue(object2)));
                        }
                    }
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (this.PropertySelector(property))
                        {
                            propertyValues.Add(new WriteableTuple<object, object>(property.GetValue(object1), property.GetValue(object2)));
                        }
                    }
                    foreach (WriteableTuple<object, object> entry in propertyValues)
                    {
                        if (this.Equals(entry.Item1, entry.Item2, visitedObjects))
                        {
                            visitedObjects.Add(new Tuple(object1, object2));
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (object1IsDefault == true && object2IsDefault == true)
            {
                return true;
            }
            return false;
        }

        private bool SpecialComparerShouldBeApplied(System.Type object1Type, System.Type object2Type, out System.Func<object, object, bool> specialComparer)
        {
            if (object1Type.ObjectIsSet())
            {
                specialComparer = (object object1, object object2) =>
                {
                    ISet<object> object1AsSet = object1.ObjectToSet();
                    ISet<object> object2AsSet = object1.ObjectToSet();
                    return object1AsSet.SetEquals(object2AsSet, this);
                };
                return true;

            }
            if (object1Type.ObjectIsDictionary())
            {
                specialComparer = (object object1, object object2) =>
                {
                    IDictionary<object, object> object1AsDictionary = object1.ObjectToDictionary();
                    IDictionary<object, object> object2AsDictionary = object1.ObjectToDictionary();
                    return object1AsDictionary.DictionaryEquals(object2AsDictionary, this, this);
                };
                return true;

            }
            if (object1Type.ObjectIsList())
            {
                specialComparer = (object object1, object object2) =>
                {
                    IList<object> object1AsList = object1.ObjectToList();
                    IList<object> object2AsList = object1.ObjectToList();
                    return object1AsList.SequenceEqual(object2AsList, this);
                };
                return true;

            }
            if (object1Type.ObjectIsEnumerable())
            {
                specialComparer = (object object1, object object2) =>
                {
                    IEnumerable<object> object1AsEnumerable = object1.ObjectToEnumerable();
                    IEnumerable<object> object2AsEnumerable = object1.ObjectToEnumerable();
                    return object1AsEnumerable.EnumerableEquals(object2AsEnumerable, this);
                };
                return true;
            }
            if (object1Type.IsPrimitive)
            {
                specialComparer = (object object1, object object2) =>
                {
                    return object1.Equals(object2);
                };
                return true;
            }

            specialComparer = null;
            return false;
        }


        public int GetHashCode(object @object)
        {
            if (!this._HashCodes.ContainsKey(@object))
            {
                this._HashCodes.Add(@object, RuntimeHelpers.GetHashCode(@object));
            }
            return this._HashCodes[@object];
        }
        private class Tuple
        {
            public Tuple(object item1, object item2)
            {
                this.Item1 = item1;
                this.Item2 = item2;
            }

            public object Item1 { get; set; }
            public object Item2 { get; set; }
            public override bool Equals(object obj)
            {
                Tuple tuple = obj as Tuple;
                if (tuple == null)
                {
                    return false;
                }
                if (!ReferenceEquals(this.Item1, tuple.Item1))
                {
                    return false;
                }
                if (!ReferenceEquals(this.Item2, tuple.Item2))
                {
                    return false;
                }
                return true;
            }
            public override int GetHashCode()
            {
                return 6843;
            }
        }
    }
    public class ReferenceEqualsComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
