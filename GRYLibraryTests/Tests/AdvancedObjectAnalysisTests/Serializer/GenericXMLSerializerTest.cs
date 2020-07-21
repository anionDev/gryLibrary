﻿using GRYLibrary.Core.AdvancedObjectAnalysis;
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
        [Ignore]
        [TestMethod]
        public void SerializeCyclicTestObject()
        {
            GenericXMLSerializer<object> serializer = GenericXMLSerializer.DefaultInstance;
            object testObject = CycleA.GetRandom();
            string serialized = serializer.Serialize(testObject);
            CycleA actualObject = serializer.Deserialize<CycleA>(serialized);
            Assert.IsTrue(Generic.GenericEquals(testObject, actualObject), Core.Utilities.GetAssertionFailMessage(testObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(testObject), Generic.GenericGetHashCode(actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            Assert.AreEqual(8, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(6, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(4, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(8, actualObject.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
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
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(expectedObject.GetHashCode(), actualObject.GetHashCode());
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
        [TestMethod]
        public void IXmlSerializableDefaultImplementationTypeWithDictionary()
        {
            // arrange
            TypeWithDictionary expectedObject = new TypeWithDictionary() { Dictionary = new Hashtable { { "a", "b" }, { "c", "d" } } };
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter);
            TypeWithDictionary actualObject = new TypeWithDictionary();

            // act
            Generic.GenericWriteXml(expectedObject, writer);
            string serializedObject = stringWriter.ToString();
            Generic.GenericReadXml(actualObject, XmlReader.Create(new StringReader(serializedObject)));

            // assert
            Assert.AreEqual(2, actualObject.Dictionary.Count);
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.AreEqual(2,Core.Utilities.Count(actualObject.Enumerable));
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
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
            Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));
        }
    }
}
