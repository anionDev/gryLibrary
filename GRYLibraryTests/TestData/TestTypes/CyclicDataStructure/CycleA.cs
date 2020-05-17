using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GRYLibrary.TestData.TestTypes.CyclicDataStructure
{
    public class CycleA
    {
         public Guid Id { get; set; } = Guid.NewGuid();
        public CycleB B { get; set; }

        internal static CycleA GetRandom()
        {
            CycleA a1 = new CycleA();
            CycleB b1 = new CycleB();
            CycleC c1 = new CycleC();
            CycleA a2 = new CycleA();
            CycleB b2 = new CycleB();
            CycleC c2 = new CycleC();

            a1.B = b1;
            b1.C = c1;
            c1.A.Add(a1);
            c1.A.Add(a2);
            a2.B = b2;
            b2.C = c2;
            c2.A.Add(a1);

            return a1;
        }

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
