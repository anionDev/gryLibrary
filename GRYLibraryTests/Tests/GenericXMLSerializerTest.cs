using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [TestMethod]
        public void SerializeSimpleObjects()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.GetDefaultInstance();
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetTestObject();
            string serialized = serializer.Serialize(testObject);
            Console.WriteLine(serialized);
            Assert.AreEqual(testObject, serializer.Deserialize(serialized));
        }
    }
}
