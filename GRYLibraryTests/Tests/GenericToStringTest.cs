using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class GenericToStringTest
    {
        [TestMethod]
        public void SimpleDataStructureTestObjectToString()
        {
            Assert.AreEqual(@"TODO", SimpleDataStructure1.GetTestObject().ToString());
        }
        [TestMethod]
        public void ComplexTestObjectToString()
        {
            Assert.AreEqual(@"TODO", Company.GetRandom().ToString());
        }
        [TestMethod]
        public void CyclicTestObjectToString()
        {
            Assert.AreEqual(@"TODO", CycleA.GetRandom().ToString());
        }
    }
}
