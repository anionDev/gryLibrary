using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    internal class PropertyEqualsCalculatorConfiguration
    {
        private static readonly IdGenerator<int> _IdGenerator = IdGenerator.GetDefaultIntIdGenerator();
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
            this.CustomComparer = new List<AbstractCustomComparer>() {
              new PrimitiveComparer(this),
              new KeyValuePairComparer(this),
              new TupleComparer(this),
              new ListComparer(this),
              new SetComparer(this),
              new DictionaryComparer(this),
              new EnumerableComparer(this),
            };
        }
        public int GetHashCode(object @object)
        {
            return Generic.GenericGetHashCode(@object);
        }
        /// <remarks>This function requires that <paramref name="object"/> was already assigned to an <see cref="EquivalenceClass"/>.</remarks>
        private EquivalenceClass GetEquivalenceClassOfObject(object @object)
        {
            foreach (EquivalenceClass equivalenceClass in this.EquivalenceClasses)
            {
                if (this.BelongsToEquivalenceClass(equivalenceClass, @object))
                {
                    equivalenceClass.Add(@object);
                    return equivalenceClass;
                }
            }
            throw new KeyNotFoundException($"Object '{@object}' was not assigned to an {nameof(EquivalenceClass)} yet.");
        }

        private bool BelongsToEquivalenceClass(EquivalenceClass equivalenceClass, object @object)
        {
            return equivalenceClass.ContainedObjects.Contains(@object);
        }

        public bool AreInSameEquivalenceClass(object object1, object object2)
        {
            if (!this.HasEquivalenceClass(object1))
            {
                return false;
            }
            if (!this.HasEquivalenceClass(object2))
            {
                return false;
            }
            return this.GetEquivalenceClassOfObject(object1).Equals(this.GetEquivalenceClassOfObject(object2));
        }

        private bool HasEquivalenceClass(object @object)
        {
            foreach (EquivalenceClass equivalenceClass in this.EquivalenceClasses)
            {
                if (this.BelongsToEquivalenceClass(equivalenceClass, @object))
                {
                    return true;
                }
            }
            return false;
        }

        internal void AddEqualObjectsToEquivalenceClasses(object object1, object object2)
        {
            foreach (EquivalenceClass loopEquivalenceClass in this.EquivalenceClasses)
            {
                if (this.BelongsToEquivalenceClass(loopEquivalenceClass, object1))
                {
                    loopEquivalenceClass.Add(object2);
                    return;
                }
            }
            foreach (EquivalenceClass loopEquivalenceClass in this.EquivalenceClasses)
            {
                if (this.BelongsToEquivalenceClass(loopEquivalenceClass, object2))
                {
                    loopEquivalenceClass.Add(object1);
                    return;
                }
            }
            EquivalenceClass equivalenceClass = new EquivalenceClass(object1, _IdGenerator.GenerateNewId());
            equivalenceClass.Add(object2);
            this.EquivalenceClasses.Add(equivalenceClass);
        }
    }
}
