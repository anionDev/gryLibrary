using GRYLibrary.TestData.TestTypes.AdvancedObjectAnalysisUsageFull;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests
{
    [TestClass]
    public class SimpleFullTest
    {
        [Ignore]
        [TestMethod]
        public void TestSimpleFullTest()
        {
            TestType testType1 = TestType.GetRandom();
            TestType testType2 = TestType.GetRandom();
            Assert.AreEqual(testType1, testType2);
            Assert.AreEqual(testType1.AttributeA, testType2.AttributeA);
            Assert.AreEqual("TODO", testType1.ToString());
            Assert.AreEqual(testType1.GetHashCode(), testType2.GetHashCode());

        }
    }
}
