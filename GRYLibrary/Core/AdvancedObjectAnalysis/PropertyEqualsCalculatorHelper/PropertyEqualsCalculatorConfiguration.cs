using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    internal class PropertyEqualsCalculatorConfiguration
    {
        internal ISet<EquivalenceClass> EquivalenceClasses { get; } = new HashSet<EquivalenceClass>();
        public Func<PropertyInfo, bool> PropertySelector { get; set; } = (PropertyInfo propertyInfo) =>
           {
               return propertyInfo.CanWrite && propertyInfo.GetMethod.IsPublic;
           };
        public Func<FieldInfo, bool> FieldSelector { get; set; } = (FieldInfo propertyInfo) =>
        {
            return false;
        };
        public List<AbstractCustomComparer> CustomComparer { get; set; }
        public PropertyEqualsCalculatorConfiguration()
        {
            CustomComparer = new List<AbstractCustomComparer>() {
              new PrimitiveComparer(this),
              new KeyValuePairComparer(this),
              new TupleComparer(this),
              new ListComparer(this),
              new SetComparer(this),
              new DictionaryComparer(this),
              new EnumerableComparer(this),
            };
        }
        public int GetRuntimeHashCode(object @object)
        {
            return GetEquivalenceClassOfObject(@object).GetHashCode();
        }

        private EquivalenceClass GetEquivalenceClassOfObject(object @object)
        {
            foreach (EquivalenceClass equivalenceClass in EquivalenceClasses)
            {
                if (equivalenceClass.BelongsToThisEquivalenceClass(@object))
                {
                    equivalenceClass.ContainedObjects.Add(@object);
                    return equivalenceClass;
                }
            }
            EquivalenceClass newEquivalenceClass = new EquivalenceClass();
            newEquivalenceClass.ContainedObjects.Add(@object);
            this.EquivalenceClasses.Add(newEquivalenceClass);
            return newEquivalenceClass;
        }

        public bool AreInSameEquivalenceClass(object object1, object object2)
        {
            return GetEquivalenceClassOfObject(object1).Equals(GetEquivalenceClassOfObject(object2));
        }

        internal void AddEqualObjects(object object1, object object2)
        {
            foreach (EquivalenceClass e in this.EquivalenceClasses)
            {
                if (e.BelongsToThisEquivalenceClass(object1))
                {
                    e.ContainedObjects.Add(object1);
                    e.ContainedObjects.Add(object2);
                }
            }
        }
    }
}
