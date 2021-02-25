using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.Tests.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.SimpleDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.XMLSerializableType;
using GRYLibrary.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace GRYLibrary.Tests.Testcases.AdvancedObjectAnalysisTests.Serializer
{
    [TestClass]
    public class GenericXMLSerializerTest
    {
        [TestMethod]
        public void SerializeSimpleTestObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance();
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetRandom();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure1 actualObject = serializer.Deserialize<SimpleDataStructure1>(serialized);
            Assert.AreEqual(testObject, actualObject);
        }
        [TestMethod]
        public void SerializeComplexTestObject1()
        {
            // arrange
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance();
            object expectedObject = CycleA.GetRandom();

            // act
            string serialized = serializer.Serialize(expectedObject);
            CycleA actualObject = serializer.Deserialize<CycleA>(serialized);

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serialized));
            TestUtilities.AssertEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void SerializeCyclicTestObject3()
        {
            // arrange
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance();
            object expectedObject = CycleA.GetRandom();

            // act
            string serialized = serializer.Serialize(expectedObject);
            CycleA actualObject = serializer.Deserialize<CycleA>(serialized);

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serialized));
            TestUtilities.AssertEqual(expectedObject, actualObject);
        }
        [Ignore]
        [TestMethod]
        public void SerializeComplexTestObject2()
        {
            // arrange
            object expectedObject = Company.GetRandom();

            // act
            string serialized = Generic.GenericSerialize(expectedObject);
            Company actualObject = Generic.GenericDeserialize<Company>(serialized);

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serialized));
            TestUtilities.AssertEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void SerializeCyclicTestObject4()
        {
            // arrange
            object expectedObject = CycleA.GetRandom();

            // act
            string serialized = Generic.GenericSerialize(expectedObject);
            CycleA actualObject = Generic.GenericDeserialize<CycleA>(serialized);

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serialized));
            TestUtilities.AssertEqual(expectedObject, actualObject);
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serializedObject));
            Assert.AreEqual(8, actualObject.Count);
            TestUtilities.AssertEqual(expectedObject, actualObject);
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(8, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(8, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }


        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithList()
        {
            // arrange
            TypeWithList expectedObject = new TypeWithList() { List = new List<string> { "a", "b" } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithList actualObject = new TypeWithList();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.List.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithListGeneric()
        {
            // arrange
            TypeWithListGeneric<string> expectedObject = new TypeWithListGeneric<string>() { List = new List<string> { "a", "b" } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithListGeneric<string> actualObject = new TypeWithListGeneric<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.List.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithEnumerable()
        {
            // arrange
            TypeWithEnumerable expectedObject = new TypeWithEnumerable() { Enumerable = new List<string> { "a", "b" } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithEnumerable actualObject = new TypeWithEnumerable();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, Core.Miscellaneous.Utilities.Count(actualObject.Enumerable));
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithEnumerableGeneric()
        {
            // arrange
            TypeWithEnumerableGeneric<string> expectedObject = new TypeWithEnumerableGeneric<string>() { Enumerable = new HashSet<string> { "a", "b" } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithEnumerableGeneric<string> actualObject = new TypeWithEnumerableGeneric<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Enumerable.Count());
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithSetGeneric()
        {
            // arrange
            TypeWithSetGeneric<string> expectedObject = new TypeWithSetGeneric<string>() { Set = new HashSet<string> { "a", "b" } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithSetGeneric<string> actualObject = new TypeWithSetGeneric<string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Set.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [Ignore]
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithDictionaryGeneric()
        {
            // arrange
            TypeWithDictionaryGeneric<int, string> expectedObject = new TypeWithDictionaryGeneric<int, string>() { Dictionary = new Dictionary<int, string>() { { 5, "b" }, { 6, "d" } } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithDictionaryGeneric<int, string> actualObject = new TypeWithDictionaryGeneric<int, string>();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Dictionary.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
    }
}
