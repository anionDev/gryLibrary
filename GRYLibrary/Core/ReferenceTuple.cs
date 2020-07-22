using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core
{
    public class ReferenceTuple : IEquatable<ReferenceTuple>
    {
        public object Item1 { get; set; }
        public object Item2 { get; set; }
        public ReferenceTuple(object item1, object item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj);
        }

        public override int GetHashCode()
        {
            return nameof(ReferenceTuple).GetHashCode();
        }

        public bool Equals(ReferenceTuple other)
        {
            return other != null
                && object.ReferenceEquals(this.Item1, other.Item1)
                && object.ReferenceEquals(this.Item2, other.Item2);
        }
    }
}
