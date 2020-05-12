using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class PropertyEqualsCalculator : GRYEqualityComparer<object>
    {

        private readonly Dictionary<object, int> _HashCodes = new Dictionary<object, int>(ReferenceEqualsComparer.Instance);
        public static GRYEqualityComparer<object> DefaultInstance { get; } = new PropertyEqualsCalculator();
        public List<AbstractCustomComparer> CustomComparer { get; set; } = new List<AbstractCustomComparer>() {
            PrimitiveComparer.DefaultInstance,
            KeyValuePairComparer.DefaultInstance,
            ListComparer.DefaultInstance,
            SetComparer.DefaultInstance,
            DictionaryComparer.DefaultInstance,
            EnumerableComparer.DefaultInstance,
        };
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
        {
            return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
        };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
        {
            return false;
        };
        public PropertyEqualsCalculator()
        {
        }
        public static IEqualityComparer<T> GetDefaultInstance<T>()
        {
            return PropertyEqualsCalculator<T>.Instance;
        }

        /// <remarks>This function assumes that 2 objects are not equal if their types are not equal.</remarks>
        public override bool Equals(object object1, object object2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            bool object1IsDefault = Utilities.IsDefault(object1);
            bool object2IsDefault = Utilities.IsDefault(object2);
            if (object1IsDefault == true && object2IsDefault == true)
            {
                return true;
            }
            if (object1IsDefault == false && object2IsDefault == false)
            {
                Type object1Type = object1.GetType();
                Type object2Type = object2.GetType();
                if (visitedObjects.Contains(new PropertyEqualsCalculatorTuple(object1, object2)))
                {
                    return true;
                }
                if (object1Type.Equals(object2Type))
                {
                    if (this.CustomComparerShouldBeApplied(object1Type, out AbstractCustomComparer customComparer))
                    {
                        //use custom comparer
                        bool result = customComparer.Equals(object1, object2, visitedObjects);
                        if (result)
                        {
                            visitedObjects.Add(new PropertyEqualsCalculatorTuple(object1, object2));
                        }
                        return result;
                    }
                    else
                    {
                        //use default comparer
                        Type type = object1Type;
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
                                visitedObjects.Add(new PropertyEqualsCalculatorTuple(object1, object2));
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool CustomComparerShouldBeApplied(Type object1Type, out AbstractCustomComparer customComparer)
        {
            foreach (AbstractCustomComparer comparer in this.CustomComparer)
            {
                if (comparer.IsApplicable(object1Type))
                {
                    customComparer = comparer;
                    return true;
                }
            }
            customComparer = null;
            return false;
        }
        public override int GetHashCode(object @object)
        {
            if (!this._HashCodes.ContainsKey(@object))
            {
                this._HashCodes.Add(@object, RuntimeHelpers.GetHashCode(@object));
            }
            return this._HashCodes[@object];
        }

    }
    public class PropertyEqualsCalculator<T> : IEqualityComparer<T>
    {
        internal static PropertyEqualsCalculator<T> Instance { get; } = new PropertyEqualsCalculator<T>();
        public bool Equals(T x, T y)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return PropertyEqualsCalculator.DefaultInstance.GetHashCode(obj);
        }
    }
}
