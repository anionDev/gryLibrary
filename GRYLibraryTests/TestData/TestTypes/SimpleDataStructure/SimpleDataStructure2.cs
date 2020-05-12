using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure2
    {
        public Guid Guid { get; set; }

        public override bool Equals(object obj)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Guid);
        }
        public override string ToString()
        {
            return $"{nameof(SimpleDataStructure2)}{{{nameof(this.Guid)}={this.Guid}}}";
        }
    }

}
