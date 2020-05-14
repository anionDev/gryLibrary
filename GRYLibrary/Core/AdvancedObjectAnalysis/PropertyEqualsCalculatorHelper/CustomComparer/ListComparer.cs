using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class ListComparer : AbstractCustomComparer
    {
        private ListComparer() { }
        public static ListComparer DefaultInstance { get; } = new ListComparer();
    
        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return this.EqualsTyped(Utilities.ObjectToList<object>(x), Utilities.ObjectToList<object>(y), visitedObjects);
        }

        public bool EqualsTyped<T>(IList<T> x, IList<T> y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            for (int i = 0; i < x.Count; i++)
            {
                if (!PropertyEqualsCalculator.DefaultInstance.Equals(x, y, visitedObjects))
                {
                    return false;
                }
            }
            return true;
        }


        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsList(type);
        }
    }
}
