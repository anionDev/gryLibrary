using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace GRYLibraryTest
{
    [TestClass]
    public class SimpleObjectPersistenceTest
    {
        [TestMethod]
        public void SimpleObjectPersistenceTestTest1()
        {
            string file = "file";
            Encoding encoding = new UTF8Encoding(false);
            SimpleObjectPersistence<SerializeTestClass> sop1 = new SimpleObjectPersistence<SerializeTestClass>(file, encoding);

            SerializeTestClass testObject = SerializeTestClass.Create();
            sop1.Object = testObject;
            sop1.SaveObject();

            string content = System.IO.File.ReadAllText(file, encoding);
            //TODO compare content with testvalue

            SimpleObjectPersistence<SerializeTestClass> sop2 = new SimpleObjectPersistence<SerializeTestClass>(file, encoding);

            Assert.AreEqual(TestProperty1String, sop2.Object.TestProperty1String);
            Assert.AreEqual(TestProperty2Bool, sop2.Object.TestProperty2Bool);
            Assert.AreEqual(TestProperty3Double, sop2.Object.TestProperty3Double);
            Assert.IsNull(sop2.Object.NullTest);
            Assert.AreEqual(TestProperty3Double, sop2.Object.TestPropertyTestClass2.TestProperty3Double);
            Assert.AreEqual(3, sop2.Object.TestPropertyTestClass3.ListTest.Count);

            Assert.AreEqual("TODO", content);
        }
        internal static string TestProperty1String = "testäéß";
        internal static bool TestProperty2Bool = true;
        internal static double TestProperty3Double = 22 / 7;
        internal static List<bool> TestList = new List<bool>() { true, false, true };
        internal static string NullTestValue = null;
        private interface TestInterface
        {
            string TestProperty1String { get; set; }
        }
        private class TestClass1
        {
            public bool TestProperty2Bool { get; set; }
            public double TestProperty3Double { get; set; }
        }
        private class TestClass2 : TestClass1
        {
            internal TestClass2 TestPropertyTestClass2 { get; set; }
            internal TestClass3 TestPropertyTestClass3_1 { get; set; }
        }
        private class TestClass3
        {
            public List<bool> ListTest { get; set; }
        }
        private class SerializeTestClass : TestClass2, TestInterface
        {
            public string TestProperty1String { get; set; }
            internal TestClass3 TestPropertyTestClass3 { get; set; }
            internal TestClass3 NullTest { get; set; }
            public static SerializeTestClass Create()
            {
                SerializeTestClass result = new SerializeTestClass();
                result.NullTest = null;
                result.TestProperty1String = SimpleObjectPersistenceTest.TestProperty1String;
                result.TestProperty2Bool = SimpleObjectPersistenceTest.TestProperty2Bool;
                result.TestProperty3Double = SimpleObjectPersistenceTest.TestProperty3Double;
                result.TestPropertyTestClass3 = new TestClass3();
                result.TestPropertyTestClass3.ListTest = TestList;
                result.TestPropertyTestClass2 = new TestClass2();
                result.TestPropertyTestClass2.TestProperty2Bool = SimpleObjectPersistenceTest.TestProperty2Bool;
                result.TestPropertyTestClass2.TestProperty3Double = SimpleObjectPersistenceTest.TestProperty3Double;
                return result;
            }
        }
    }
}
