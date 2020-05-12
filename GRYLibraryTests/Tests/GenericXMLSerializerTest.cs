using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [Ignore]
        [TestMethod]
        public void SerializeSimpleObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.GetDefaultInstance();
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetTestObject();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure1 actualObject = serializer.Deserialize<SimpleDataStructure1>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
        [Ignore]
        [TestMethod]
        public void SerializeComplexObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.GetDefaultInstance();
            object testObject = Company.GetRandom();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure1 actualObject = serializer.Deserialize<SimpleDataStructure1>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
        [Ignore]
        [TestMethod]
        public void SerializeCyclicObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.GetDefaultInstance();
            object testObject = CycleA.GetRandom();
            string serialized = serializer.Serialize(testObject);
            CycleA actualObject = serializer.Deserialize<CycleA>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
    }
}
