using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure1
    {
        public IList<SimpleDataStructure3> Property1 { get; set; }
        public SimpleDataStructure2 Property2 { get; set; }
        public int Property3 { get; set; }

        public static SimpleDataStructure1 GetRandom()
        {
            SimpleDataStructure1 result = new SimpleDataStructure1();

            result.Property1 = new List<SimpleDataStructure3>();
            result.Property1.Add(SimpleDataStructure3.GetRandom());
            result.Property1.Add(SimpleDataStructure3.GetRandom());
            result.Property1.Add(SimpleDataStructure3.GetRandom());
            result.Property2 = new SimpleDataStructure2() { Guid = Guid.Parse("3735ece2-942f-4380-aec4-27aaa4021ed5") };
            result.Property3 = 21;
            return result;
        }


        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }
        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }
        public override string ToString()
        {
            return Generic.GenericToString(this);
        }
        #endregion
    }
}
