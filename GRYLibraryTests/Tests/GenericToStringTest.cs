using GRYLibrary.Core.AdvancedObjectAnalysis;
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
        public void SimpleDataStructureToString()
        {
            Assert.AreEqual(@"TODO", GenericToString.Instance.ToString(SimpleDataStructure1.GetTestObject()));
        }
    }
}
