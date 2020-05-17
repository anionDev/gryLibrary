using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.TestData.TestTypes.CyclicDataStructure
{
    public class CycleC
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public IList<CycleA> A { get; set; } = new List<CycleA>();
        public override bool Equals(object obj)
        {
            return new PropertyEqualsCalculator().Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return new PropertyEqualsCalculator().GetHashCode(this);
        }
        public override string ToString()
        {            
            return GenericToString.Instance.ToString(this);
        }
    }
}
