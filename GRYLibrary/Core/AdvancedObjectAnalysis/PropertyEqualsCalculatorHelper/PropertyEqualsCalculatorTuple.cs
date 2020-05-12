using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class PropertyEqualsCalculatorTuple
    {
        public PropertyEqualsCalculatorTuple(object item1, object item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public object Item1 { get; set; }
        public object Item2 { get; set; }
        public override bool Equals(object obj)
        {
            PropertyEqualsCalculatorTuple tuple = obj as PropertyEqualsCalculatorTuple;
            if (tuple == null)
            {
                return false;
            }
            if (!ReferenceEquals(this.Item1, tuple.Item1))
            {
                return false;
            }
            if (!ReferenceEquals(this.Item2, tuple.Item2))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return 6843;
        }
    }
}
