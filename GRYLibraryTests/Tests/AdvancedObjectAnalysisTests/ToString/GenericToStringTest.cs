using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.ToString
{
    [TestClass]
    public class GenericToStringTest
    {
        [Ignore]
        [TestMethod]
        public void SimpleDataStructureTestObjectToString()
        {
            Assert.AreEqual(@"TODO", SimpleDataStructure1.GetTestObject().ToString());
        }

        [Ignore]
        [TestMethod]
        public void ComplexTestObjectToString()
        {
            Assert.AreEqual(@"TODO", Company.GetRandom().ToString());
        }

        [Ignore]
        [TestMethod]
        public void CyclicTestObjectToString()
        {
            Assert.AreEqual(@"TODO", CycleA.GetRandom().ToString());
        }
    }
}
