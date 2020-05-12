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
        public override bool Equals(object obj)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return PropertyEqualsCalculator.DefaultInstance.GetHashCode(this);
        }
    }
}
