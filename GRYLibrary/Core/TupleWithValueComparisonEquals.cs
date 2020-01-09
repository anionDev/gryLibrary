using System;

namespace GRYLibrary.Core
{
    public class TupleWithValueComparisonEquals<T1, T2> : Tuple<T1, T2>
    {
        public TupleWithValueComparisonEquals(T1 item1, T2 item2) : base(item1, item2)
        {
        }

        public override bool Equals(object @object)
        {
            return this.Item2.Equals(((TupleWithValueComparisonEquals<T1, T2>)@object).Item2) && this.Item1.Equals(((TupleWithValueComparisonEquals<T1, T2>)@object).Item1);
        }

        public override int GetHashCode()
        {
            return this.Item2.GetHashCode();
        }
    }
}
