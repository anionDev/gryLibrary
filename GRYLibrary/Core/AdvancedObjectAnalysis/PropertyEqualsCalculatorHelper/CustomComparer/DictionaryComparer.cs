using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
   public class DictionaryComparer : AbstractCustomComparer
    {
        private DictionaryComparer() { }
        public static AbstractCustomComparer DefaultInstance { get; } = new DictionaryComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }
        public bool EqualsTyped(IDictionary<object,object> x, IDictionary<object, object> y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
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
