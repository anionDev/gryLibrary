using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class SetComparer: AbstractCustomComparer
    {
        private SetComparer() { }
        public static AbstractCustomComparer DefaultInstance { get; } = new SetComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }
        public bool EqualsTyped(ISet<object> x, ISet<object> y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }


        public override int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicable(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
