using GRYLibrary.Core;
using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibrary.Tests.Testcases.AdvancedObjectAnalysisTests.Equals
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
            CycleA testObject2 = Core.Utilities.DeepClone(testObject1);
            testObject2.B.Id = Guid.NewGuid();
            TestUtilities.AssertNotEqual(testObject1, testObject2);
        }
        [TestMethod]
        public void EqualCyclicTestObject()
        {
            CycleA testObject1 = CycleA.GetRandom();
            CycleA testObject2 = Core.Utilities.DeepClone(testObject1);
            TestUtilities.AssertEqual(testObject1, testObject2);
        }

        [TestMethod]
        public void PrimitiveEqualsTestObjectWithSameObject()
        {
            object testObject = new object();
            PropertyEqualsCalculator comparer = new PropertyEqualsCalculator();
            Assert.IsTrue(comparer.Equals(testObject, testObject));
        }

        [TestMethod]
        public void PrimitiveEqualsTestObjectWithEqualObject()
        {
            PropertyEqualsCalculator comparer = new PropertyEqualsCalculator();
            Assert.IsTrue(comparer.Equals(new object(), new object()));
        }
    }
}
