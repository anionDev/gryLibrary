using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class SetComparer : AbstractCustomComparer
    {
        private SetComparer() { }
        public static SetComparer DefaultInstance { get; } = new SetComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return this.EqualsTyped(Utilities.ObjectToSet<object>(x), Utilities.ObjectToSet<object>(y), visitedObjects);
        }
        public bool EqualsTyped<T>(ISet<T> set1, ISet<T> set2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            if (set1.Count != set2.Count)
            {
                return false;
            }
            foreach (T obj in set1)
            {
                if (!this.Contains(set2, obj, visitedObjects))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Contains<T>(ISet<T> set, T obj, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            foreach (ISet<T> item in set)
            {
                if (PropertyEqualsCalculator.DefaultInstance.Equals(item, obj, visitedObjects))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsSet(type);
        }
    }
}
