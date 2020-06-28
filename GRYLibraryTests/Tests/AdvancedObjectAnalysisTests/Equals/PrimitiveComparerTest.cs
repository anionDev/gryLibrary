using GRYLibrary.Core.AdvancedObjectAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.Equals
{
    [TestClass]
  public  class PrimitiveComparerTest
    {

        [TestMethod]
        public void PrimitiveEqualsTest()
        {
            string testString = "test";
            PropertyEqualsCalculator comparer = new PropertyEqualsCalculator();
            Assert.IsTrue(comparer.Equals(testString, testString));
        }
    }
}
