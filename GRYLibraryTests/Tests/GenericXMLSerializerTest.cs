using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [Ignore]
        [TestMethod]
        public void SerializeSimpleObjects()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.GetDefaultInstance();
            Console.WriteLine(serializer.Serialize(SimpleDataStructure1.GetTestObject()));
        }
    }
}
