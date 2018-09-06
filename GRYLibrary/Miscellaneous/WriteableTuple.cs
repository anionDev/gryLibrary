using System;

namespace GRYLibrary.Miscellaneous
{
    public class WriteableTuple<T1, T2> : Tuple<T1, T2>
    {
        public WriteableTuple(T1 item1, T2 item2) : base(item1, item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public new T1 Item1 { get; set; }
        public new T2 Item2 { get; set; }
    }
}
