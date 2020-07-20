using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using GRYLibrary.TestData.TestTypes.XMLSerializableType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            writer.Flush();
            string objectAsXmlString = writer.ToString();

            Assert.AreNotEqual(string.Empty, objectAsXmlString);
            using StringReader textReader = new StringReader(objectAsXmlString);
            using XmlReader xmlReader = XmlReader.Create(textReader);
            XMLSerializableType actualObject = (XMLSerializableType)serializer.Deserialize(xmlReader);

            // assert
            Assert.IsNotNull(actualObject);
            Assert.AreEqual(expectedObject, actualObject);
            Assert.AreEqual(expectedObject.GetHashCode(), actualObject.GetHashCode());
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationListGeneric()
        {
            // arrange
            List<string> expectedObject = new List<string>() { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            List<string> actualObject = new List<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationListNotGeneric()
        {
            // arrange
            ArrayList expectedObject = new ArrayList() { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            ArrayList actualObject = new ArrayList();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationSetGeneric()
        {
            // arrange
            ISet<string> expectedObject = new HashSet<string>() { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            HashSet<string> actualObject = new HashSet<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationDictionaryGeneric()
        {
            // arrange
            Dictionary<string, string> expectedObject = new Dictionary<string, string>() { { "aö", "b" }, { "\\", "<" }, { "<test>x</test", "&" }, { "nulltest:", null } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            Dictionary<string, string> actualObject = new Dictionary<string, string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationDictionaryNotGeneric()
        {
            // arrange
            Hashtable expectedObject = new Hashtable() { { "aö", "b" }, { "\\", "<" }, { "<test>x</test", "&" }, { "nulltest:", null } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            Hashtable actualObject = new Hashtable();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationEnumerableGeneric()
        {
            // arrange
            IOrderedEnumerable<string> expectedObject = new List<string> { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null }.OrderBy(key => key);
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            List<string> actualObject = new List<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationKeyValuePair()
        {
            // arrange
            KeyValuePair<string, int> expectedObject = new KeyValuePair<string, int>("test", 5);
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            KeyValuePair<string, int> actualObject = new KeyValuePair<string, int>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(stringWriter.ToString())));

            // assert
            Assert.AreEqual(expectedObject, actualObject);
        }
    }
}
