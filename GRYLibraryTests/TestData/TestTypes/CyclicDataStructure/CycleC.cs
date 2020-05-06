using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.TestData.TestTypes.CyclicDataStructure
{
    public class CycleC
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IList<CycleA> A { get; set; } = new List<CycleA>(); public override bool Equals(object obj)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return PropertyEqualsCalculator.DefaultInstance.GetHashCode(this);
        }
        public override string ToString()
        {
          return GenericToString.Instance.ToString(this);
        }
    }
}
