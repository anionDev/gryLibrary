using System;
using System.Collections.Generic;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure1
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

    }
}
