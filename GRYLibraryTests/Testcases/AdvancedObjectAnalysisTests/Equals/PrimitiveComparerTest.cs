using GRYLibrary.Core.AdvancedObjectAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.Testcases.AdvancedObjectAnalysisTests.Equals
{
    [TestClass]
    public class PrimitiveComparerTest
    {

        [TestMethod]
        public void PrimitiveEqualsTestString()
        {
            string testString = "test";
            PropertyEqualsCalculator comparer = new PropertyEqualsCalculator();
            Assert.IsTrue(comparer.Equals(testString, testString));
        }

        [TestMethod]
        public void PrimitiveEqualsTestInt()
        {
            int testInt = 4;
            PropertyEqualsCalculator comparer = new PropertyEqualsCalculator();
            Assert.IsTrue(comparer.Equals(testInt, testInt));
        }
    }
}
