using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GRYLibraryTest
{
    [TestClass]
    public class SimpleObjectPersistenceTest
    {
        [TestMethod]
        public void SimpleObjectPersistenceTestTest1()
        {
            string file = "file.xml";
            try
            {
                Utilities.EnsureFileDoesNotExist(file);

                Encoding encoding = new UTF8Encoding(false);
                SimpleObjectPersistence<SerializeTestClass> sop1 = new SimpleObjectPersistence<SerializeTestClass>(file, encoding);

                SerializeTestClass testObject = SerializeTestClass.CreateTestObject();
                sop1.Object = testObject;
                sop1.SaveObject();

                string content = System.IO.File.ReadAllText(file, encoding);

                SimpleObjectPersistence<SerializeTestClass> sop2 = new SimpleObjectPersistence<SerializeTestClass>(file, encoding);


                Assert.AreEqual(3, sop2.Object.ListTest.Count);
                Assert.AreEqual(true, sop2.Object.ListTest[0]);
                Assert.AreEqual(false, sop2.Object.ListTest[1]);
                Assert.AreEqual(true, sop2.Object.ListTest[2]);
                Assert.AreEqual(null, sop2.Object.TestAttribute.TestAttribute1.TestAttribute1);
                Assert.AreEqual("x", sop2.Object.TestAttribute.TestAttribute1.TestString1);
                Assert.AreEqual("encodingtest: áä?<👍你好", sop2.Object.TestAttribute.TestString1);
                Assert.AreEqual(22 / (double)7, sop2.Object.TestDouble);
                Assert.AreEqual(5, sop2.Object.TestDouble2);
                Assert.AreEqual("y", sop2.Object.TestStringFromInterface);
                Assert.AreEqual(BigInteger.Parse("29996224275833"), sop2.Object.TestBigInteger);

                Assert.AreEqual(testObject, sop2.Object);
                string expectedXMLValue = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SimpleObjectPersistenceTest.SerializeTestClass xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/GRYLibraryTest"">
  <ListTest xmlns:d2p1=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
    <d2p1:boolean>true</d2p1:boolean>
    <d2p1:boolean>false</d2p1:boolean>
    <d2p1:boolean>true</d2p1:boolean>
  </ListTest>
  <TestAttribute>
    <TestAttribute1>
      <TestAttribute1 i:nil=""true"" />
      <TestString1>x</TestString1>
    </TestAttribute1>
    <TestString1>encodingtest: áä?&lt;👍你好</TestString1>
  </TestAttribute>
  <TestBigInteger xmlns:d2p1=""http://schemas.datacontract.org/2004/07/System.Numerics"">
    <d2p1:_bits xmlns:d3p1=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
      <d3p1:unsignedInt>172680569</d3p1:unsignedInt>
      <d3p1:unsignedInt>6984</d3p1:unsignedInt>
    </d2p1:_bits>
    <d2p1:_sign>1</d2p1:_sign>
  </TestBigInteger>
  <TestDouble>3.1428571428571428</TestDouble>
  <TestDouble2>5</TestDouble2>
  <TestStringFromInterface>y</TestStringFromInterface>
</SimpleObjectPersistenceTest.SerializeTestClass>";
                Assert.AreEqual(expectedXMLValue, content);
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(file);
            }
        }
        #region TestClass
        public class SerializeTestClass : SerializeTestBaseClass, SerializeTestInterface
        {
            public string TestStringFromInterface { get; set; }
            public double TestDouble { get; set; }
            public double? TestDouble2 { get; set; }
            public BigInteger TestBigInteger { get; set; }

            public static SerializeTestClass CreateTestObject()
            {
                SerializeTestClass result = new SerializeTestClass();

                result.ListTest.Add(true);
                result.ListTest.Add(false);
                result.ListTest.Add(true);
                result.TestAttribute = new SerializeTestAttributeClass();
                result.TestAttribute.TestAttribute1 = new SerializeTestAttributeClass();
                result.TestAttribute.TestAttribute1.TestAttribute1 = null;
                result.TestAttribute.TestAttribute1.TestString1 = "x";
                result.TestAttribute.TestString1 = "encodingtest: áä?<👍你好";
                result.TestDouble = 22 / (double)7;
                result.TestDouble2 = 5;
                result.TestStringFromInterface = "y";
                result.TestBigInteger = BigInteger.Parse("29996224275833");

                return result;
            }
            public override bool Equals(object obj)
            {
                if (!base.Equals(obj))
                {
                    return false;
                }
                SerializeTestClass typedObject = obj as SerializeTestClass;
                return this.TestStringFromInterface.Equals(typedObject.TestStringFromInterface)
                    && this.TestDouble.Equals(typedObject.TestDouble)
                    && this.TestBigInteger.Equals(typedObject.TestBigInteger)
                    && this.TestDouble2.Equals(typedObject.TestDouble2);
            }
            public override int GetHashCode()
            {
                return TestDouble.GetHashCode();
            }
        }
        public interface SerializeTestInterface
        {
            string TestStringFromInterface { get; set; }
        }
        public class SerializeTestBaseClass
        {
            public IList<bool> ListTest { get; set; }
            public SerializeTestAttributeClass TestAttribute { get; set; }
            public SerializeTestBaseClass()
            {
                this.ListTest = new List<bool>();
            }
            public override bool Equals(object obj)
            {
                SerializeTestBaseClass typedObject = obj as SerializeTestBaseClass;
                return this.TestAttribute.Equals(typedObject.TestAttribute)
                    && this.ListTest.SequenceEqual(typedObject.ListTest);
            }
            public override int GetHashCode()
            {
                return TestAttribute.GetHashCode();
            }
        }
        public class SerializeTestAttributeClass
        {
            public string TestString1 { get; set; }
            public SerializeTestAttributeClass TestAttribute1 { get; set; }

            public override bool Equals(object obj)
            {
                SerializeTestAttributeClass typedObject = obj as SerializeTestAttributeClass;
                bool testAttribute1IsNull1 = this.TestAttribute1 == null;
                bool testAttribute1IsNull2 = typedObject.TestAttribute1 == null;

                if (testAttribute1IsNull1 == testAttribute1IsNull2)
                {
                    if (!testAttribute1IsNull1 && !this.TestAttribute1.Equals(typedObject.TestAttribute1))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return this.TestString1.Equals(typedObject.TestString1);
            }
            public override int GetHashCode()
            {
                return TestString1.GetHashCode();
            }

        }
        #endregion 
    }
}
