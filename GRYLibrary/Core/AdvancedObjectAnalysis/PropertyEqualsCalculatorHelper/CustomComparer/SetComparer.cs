using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class SetComparer : AbstractCustomComparer
    {
        internal SetComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToSet<object>(item1), Utilities.ObjectToSet<object>(item2));
            return result;
        }
        internal bool EqualsTyped<T>(ISet<T> set1, ISet<T> set2)
        {
            if (set1.Count != set2.Count)
            {
                return false;
            }
            foreach (T obj in set1)
            {
                if (!this.Contains(set2, obj))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Contains<T>(ISet<T> set, T obj)
        {
            foreach (ISet<T> item in set)
            {
                if (new PropertyEqualsCalculator(Configuration).Equals(item, obj))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsSet(type);
        }

        public override int DefaultGetHashCode(object obj)
        {
            return Configuration.GetRuntimeHashCode(obj);
        }
    }
}
