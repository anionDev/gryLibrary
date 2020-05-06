using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [TestMethod]
        public void SerializeSimpleObjects()
        {
            GenericXMLSerializer<SimpleDataStructure1> serializer = new GenericXMLSerializer<SimpleDataStructure1>();
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetTestObject();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure1 actualObject = serializer.Deserialize(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
    }
}
