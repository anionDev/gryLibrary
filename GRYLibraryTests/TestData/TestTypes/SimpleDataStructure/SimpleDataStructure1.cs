using GRYLibrary.Core;
using System;
using System.Collections.Generic;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure1
    {
        public IList<SimpleDataStructure3> Property1 { get; set; }
        public SimpleDataStructure2 Property2 { get; set; }
        public int Property3 { get; set; }

        public static SimpleDataStructure1 GetTestObject()
        {
            SimpleDataStructure1 result = new SimpleDataStructure1();

            result.Property1 = new List<SimpleDataStructure3>();
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property2 = new SimpleDataStructure2() { Guid = Guid.Parse("3735ece2-942f-4380-aec4-27aaa4021ed5") };
            result.Property3 = 21;
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleDataStructure1 structure &&
                   this.Property1.SequanceEqual(structure.Property1) &&
                   EqualityComparer<SimpleDataStructure2>.Default.Equals(this.Property2, structure.Property2) &&
                   this.Property3 == structure.Property3;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Property1, this.Property2, this.Property3);
        }
        public override string ToString()
        {
            return $"{nameof(SimpleDataStructure1)}{{{nameof(Property1)}=(List with {Property1.Count} item(s)),{nameof(Property2)}={Property2},{nameof(Property3)}={Property3}}}";
        }
    }
}
