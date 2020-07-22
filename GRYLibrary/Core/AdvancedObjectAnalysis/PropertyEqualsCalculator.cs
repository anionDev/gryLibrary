using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using System;

namespace GRYLibrary.Core.AdvancedObjectAnalysis
{
    public class PropertyEqualsCalculator : GRYEqualityComparer<object>
    {

        public PropertyEqualsCalculator() : this(new PropertyEqualsCalculatorConfiguration())
        {
        }
        internal PropertyEqualsCalculator(PropertyEqualsCalculatorConfiguration configuration) : base(configuration)
        {
        }

        /// <remarks>This function assumes that 2 objects which are not implementing <see cref="System.Collections.IEnumerable"/> are not equal if their types are not equal (except they are enumerables).</remarks>
        public override bool DefaultEquals(object object1, object object2)
        {
            bool object1IsDefault = Utilities.IsDefault(object1);
            bool object2IsDefault = Utilities.IsDefault(object2);
            if (object1IsDefault && object2IsDefault)
            {
                return true;
            }
            else if (!object1IsDefault && !object2IsDefault)
            {
                if (this.Configuration.AreInSameEquivalenceClass(object1, object2))
                {
                    //objects where already compared and it was determined that they are equal
                    return true;
                }
                else if (this.CustomComparerShouldBeApplied(this.Configuration, object1.GetType(), object2.GetType(), out AbstractCustomComparer customComparer))
                {
                    //use custom comparer
                    return customComparer.Equals(object1, object2);
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

        private bool CustomComparerShouldBeApplied(PropertyEqualsCalculatorConfiguration configurationAndCache, Type object1Type, Type object2Type, out AbstractCustomComparer customComparer)
        {
            foreach (AbstractCustomComparer comparer in configurationAndCache.CustomComparer)
            {
                if (comparer.IsApplicable(object1Type, object2Type))
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
            return this.Configuration.GetHashCode(obj);
        }
    }
}
