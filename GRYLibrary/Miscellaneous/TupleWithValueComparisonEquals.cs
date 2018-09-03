using System;

namespace GRYLibrary.Miscellaneous
{
    public class TupleWithValueComparisonEquals : Tuple<string, string>
    {
        public TupleWithValueComparisonEquals(string item1, string item2) : base(item1, item2)
        {
        }

        public override bool Equals(object @object)
        {
            return this.Item2.Equals(((TupleWithValueComparisonEquals)@object).Item2);
        }

        public override int GetHashCode()
        {
            return this.Item2.GetHashCode();
        }
    }
}
