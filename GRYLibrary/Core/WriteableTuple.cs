using System;
using System.Collections.Generic;

namespace GRYLibrary.Core
{
    public class WriteableTuple<T1, T2> : Tuple<T1, T2>
    {
        public WriteableTuple(T1 item1, T2 item2) : base(item1, item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public new T1 Item1 { get; set; }
        public new T2 Item2 { get; set; }
        public override int GetHashCode()
        {
            if (EqualityComparer<T1>.Default.Equals(this.Item1, default(T1)))
            {
                return 986987671;
            }
            else
            {
                return this.Item1.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            WriteableTuple<T1, T2> typedObject = obj as WriteableTuple<T1, T2>;
            if (typedObject == null)
            {
                return false;
            }
            if (!this.Item1.NullSafeEquals(typedObject.Item1))
            {
                return false;
            }
            if (!this.Item2.NullSafeEquals(typedObject.Item2))
            {
                return false;
            }
            return true;
        }
    }
}
