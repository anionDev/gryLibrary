using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.TestData.TestTypes.CyclicDataStructure
{
    public class CycleB
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CycleC C { get; set; }

        private readonly PropertyEqualsCalculator _PropertyEqualsCalculator = new PropertyEqualsCalculator();//TODO: avoid such an field
        public override bool Equals(object obj)
        {
            return _PropertyEqualsCalculator.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return _PropertyEqualsCalculator.GetHashCode(this);
        }
        public override string ToString()
        {
            return GenericToString.Instance.ToString(this);
        }
    }
}
