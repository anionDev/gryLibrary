using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure2
    {
        public Guid Guid { get; set; }

        private readonly PropertyEqualsCalculator _PropertyEqualsCalculator = new PropertyEqualsCalculator();//TODO: avoid such an field
        public override bool Equals(object obj)
        {
            return this._PropertyEqualsCalculator.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return this._PropertyEqualsCalculator.GetHashCode(this);
        }
        public override string ToString()
        {
            return GenericToString.Instance.ToString(this);
        }
    }

}
