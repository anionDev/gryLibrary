using GRYLibrary.Core;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.Equals
{
    [TestClass]
    public class EqualsTest
    {
        [TestMethod]
        public void NotEqualCyclicTestObject1()
        {
            CycleA testObject1 = CycleA.GetRandom();
            CycleA testObject2 = CycleA.GetRandom();
            TestUtilities.AssertNotEqual(testObject1, testObject2);
        }
        [TestMethod]
        public void NotEqualCyclicTestObject2()
        {
            CycleA testObject1 = CycleA.GetRandom();
            CycleA testObject2 = Utilities.DeepClone(testObject1);
            testObject2.B.Id = Guid.NewGuid();
            TestUtilities.AssertNotEqual(testObject1, testObject2);
        }
        [TestMethod]
        public void EqualCyclicTestObject()
        {
            CycleA testObject1 = CycleA.GetRandom();
            CycleA testObject2 = Utilities.DeepClone(testObject1);
            TestUtilities.AssertEqual(testObject1, testObject2);
        }
    }
}
