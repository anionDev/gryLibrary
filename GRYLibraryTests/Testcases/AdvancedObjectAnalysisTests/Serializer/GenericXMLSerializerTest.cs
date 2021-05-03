using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.AdvancedXMLSerialysis;
using GRYLibrary.Tests.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.SimpleDataStructure;
using GRYLibrary.Tests.TestData.TestTypes.XMLSerializableType;
using GRYLibrary.Tests.TestData.TypeWithCommonInterfaces;
using GRYLibrary.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            List<string> expectedObject = new() { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            List<string> actualObject = new();

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
            ArrayList expectedObject = new() { "aö", "b", "\\", "<", "<test>x</test", "&", "nulltest:", null };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            ArrayList actualObject = new();

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
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            HashSet<string> actualObject = new();

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
            TypeWithList expectedObject = new() { List = new List<string> { "a", "b" } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithList actualObject = new();

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
            TypeWithListGeneric<string> expectedObject = new() { List = new List<string> { "a", "b" } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithListGeneric<string> actualObject = new();

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
            TypeWithEnumerable expectedObject = new() { Enumerable = new List<string> { "a", "b" } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithEnumerable actualObject = new();

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
            TypeWithEnumerableGeneric<string> expectedObject = new() { Enumerable = new HashSet<string> { "a", "b" } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithEnumerableGeneric<string> actualObject = new();

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
            TypeWithSetGeneric<string> expectedObject = new() { Set = new HashSet<string> { "a", "b" } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithSetGeneric<string> actualObject = new();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Set.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithDictionaryGeneric()
        {
            // arrange
            TypeWithDictionaryGeneric<int, string> expectedObject = new() { Dictionary = new Dictionary<int, string>() { { 5, "b" }, { 6, "d" } } };
            StringWriter stringWriter = new();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithDictionaryGeneric<int, string> actualObject = new();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Dictionary.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [Ignore]
        [TestMethod]
        public void SerializeTypeWithCommonInterfaces1()
        {
            // arrange
            TypeWithCommonInterfaces expectedObject = new()
            {
                List = new List<object>() { 2, 4, 3 },
                Enumerable = new List<int>() { 2, 4, 3 },
                Set = new HashSet<int>() { 2, 4, 3 },
                Dictionary = new Dictionary<int, int>() { { 2, 20 }, { 4, 40 }, { 3, 30 } }
            };
            MemoryStream stream = new();
            XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Encoding = new UTF8Encoding(false) });
            XmlReader xmlReader = XmlReader.Create(stream);

            // act
            expectedObject.WriteXml(xmlWriter);
            TypeWithCommonInterfaces actualObject = new();
            actualObject.ReadXml(xmlReader);

            // assert
            Assert.AreEqual(expectedObject, actualObject);
        }
        [TestMethod]
        public void SerializeTypeWithCommonInterfaces2()
        {
            // arrange
            TypeWithCommonInterfaces expectedObject = new()
            {
                List = new List<object>() { 2, 4, 3 },
                Enumerable = new List<int>() { 2, 4, 3 },
                Set = new HashSet<int>() { 2, 4, 3 },
                Dictionary = new Dictionary<int, int>() { { 2, 20 }, { 4, 40 }, { 3, 30 } }
            };
            expectedObject.List.Add(expectedObject);

            // act
            string serialized = Generic.GenericSerialize(expectedObject);
            TypeWithCommonInterfaces actualObject = Generic.GenericDeserialize<TypeWithCommonInterfaces>(serialized);

            // assert
            Assert.IsTrue(Core.Miscellaneous.Utilities.IsValidXML(serialized));
            TestUtilities.AssertEqual(expectedObject, actualObject);
        }
    }
}
