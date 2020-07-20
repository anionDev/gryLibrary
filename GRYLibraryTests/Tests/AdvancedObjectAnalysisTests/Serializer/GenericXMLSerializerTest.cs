using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using GRYLibrary.TestData.TestTypes.XMLSerializableType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.Serializer
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [TestMethod]
        public void SerializeSimpleTestObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance;
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetRandom();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure1 actualObject = serializer.Deserialize<SimpleDataStructure1>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
        [TestMethod]
        public void SerializeComplexTestObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance;
            object testObject = Company.GetRandom();
            string serialized = serializer.Serialize(testObject);
            Company actualObject = serializer.Deserialize<Company>(serialized);
            string o1 = testObject.ToString();
            string o2 = actualObject.ToString();
            Assert.AreEqual(o1, o2);
            Assert.AreEqual(testObject, actualObject);
        }
        [TestMethod]
        public void SerializeCyclicTestObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance;
            object testObject = CycleA.GetRandom();
            string serialized = serializer.Serialize(testObject);
            CycleA actualObject = serializer.Deserialize<CycleA>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTest()
        {
            // arrange
            XMLSerializableType expectedObject = XMLSerializableType.GetRandom();

            // act
            XmlSerializer serializer = new XmlSerializer(typeof(XMLSerializableType), new XmlRootAttribute(nameof(XMLSerializableType)));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, expectedObject);

            string objectAsXmlString = writer.ToString();

            using StringReader textReader = new StringReader(objectAsXmlString);
            using XmlReader xmlReader = XmlReader.Create(textReader);
            XMLSerializableType actualObject = (XMLSerializableType)serializer.Deserialize(xmlReader);

            // assert
            Assert.IsNotNull(actualObject);
            Assert.AreEqual(expectedObject, actualObject);
            Assert.AreEqual(expectedObject.GetHashCode(), actualObject.GetHashCode());
        }
    }
}
