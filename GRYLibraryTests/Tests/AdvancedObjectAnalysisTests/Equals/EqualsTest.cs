using GRYLibrary.Core;
using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.Equals
{
    [TestClass]
    public class EqualsTest
    {
        [TestMethod]
        public void NotEqualCyclicTestObject1()
        {
            object testObject1 = CycleA.GetRandom();
            object testObject2 = CycleA.GetRandom();
            Assert.IsFalse(Generic.GenericEquals(testObject1, testObject2), Utilities.GetAssertionFailMessage(testObject1, testObject2));
            Assert.IsFalse(testObject1.Equals(testObject2));
        }
        [Ignore]
        [TestMethod]
        public void NotEqualCyclicTestObject2()
        {
            object testObject1 = CycleA.GetRandom();
            object testObject2 = null;//todo reproduce [and fix] stackoverflow caused by not equal objects. See GRYLogTests.SerializeAndDeserialize()
            Assert.IsFalse(testObject1.Equals(testObject2));
            Assert.IsFalse(Generic.GenericEquals(testObject1, testObject2), Utilities.GetAssertionFailMessage(testObject1, testObject2));
        }
        [Ignore]
        [TestMethod]
        public void EqualCyclicTestObject()
        {
            object testObject1 = CycleA.GetRandom();
            object testObject2 = Utilities.DeepClone(testObject1);
            Assert.IsTrue(testObject1.Equals(testObject2));
            Assert.IsTrue(Generic.GenericEquals(testObject1, testObject2), Utilities.GetAssertionFailMessage(testObject1, testObject2));
            Assert.AreEqual(testObject1.GetHashCode(), testObject2.GetHashCode());
            Assert.AreEqual(Generic.GenericGetHashCode(testObject1), Generic.GenericGetHashCode(testObject2));
        }
    }
}
