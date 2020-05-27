using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class PropertyEqualsCalculator : GRYEqualityComparer<object>
    {

        public PropertyEqualsCalculator() : this(new PropertyEqualsCalculatorConfiguration())
        {
        }
        internal PropertyEqualsCalculator(PropertyEqualsCalculatorConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <remarks>This function assumes that 2 objects which are not implementing <see cref="System.Collections.IEnumerable"/>are not equal if their types are not equal.</remarks>
        public override bool DefaultEquals(object object1, object object2)
        {
            bool object1IsDefault = Utilities.IsDefault(object1);
            bool object2IsDefault = Utilities.IsDefault(object2);
            if (object1IsDefault == true && object2IsDefault == true)
            {
                return true;
            }
            else if (object1IsDefault == false && object2IsDefault == false)
            {
                Type object1Type = object1.GetType();
                Type object2Type = object2.GetType();
                if (this.Configuration.AreInSameEquivalenceClass(object1, object2))
                {
                    //objects where already compared and it was determined that they are equal
                    return true;
                }
                else if (this.CustomComparerShouldBeApplied(this.Configuration, object1Type, out AbstractCustomComparer customComparer))
                {
                    //use custom comparer
                    bool result = customComparer.Equals(object1, object2);
                    if (result)
                    {
                        Configuration.AddEqualObjects(object1, object2);
                    }
                    else
                    {
                        Utilities.NoOperation();
                    }
                    return result;
                }
                else if (object1Type.Equals(object2Type))
                {
                    //use default comparer
                    Type type = object1Type;
                    List<WriteableTuple<object, object>> propertyValues = new List<WriteableTuple<object, object>>();
                    foreach (FieldInfo field in type.GetFields())
                    {
                        if (this.Configuration.FieldSelector(field))
                        {
                            propertyValues.Add(new WriteableTuple<object, object>(field.GetValue(object1), field.GetValue(object2)));
                        }
                    }
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (this.Configuration.PropertySelector(property))
                        {
                            propertyValues.Add(new WriteableTuple<object, object>(property.GetValue(object1), property.GetValue(object2)));
                        }
                    }
                    foreach (WriteableTuple<object, object> entry in propertyValues)
                    {
                        if (this.Equals(entry.Item1, entry.Item2))
                        {
                            Configuration.AddEqualObjects(object1, object2);
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
            else
            {
                return false;
            }
        }

        private bool CustomComparerShouldBeApplied(PropertyEqualsCalculatorConfiguration configurationAndCache, Type object1Type, out AbstractCustomComparer customComparer)
        {
            foreach (AbstractCustomComparer comparer in configurationAndCache.CustomComparer)
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
        public override int DefaultGetHashCode(object obj)
        {
            return Configuration.GetRuntimeHashCode(obj);
        }
    }
}
