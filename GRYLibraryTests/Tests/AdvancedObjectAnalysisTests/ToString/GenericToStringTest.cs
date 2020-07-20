using GRYLibrary.TestData.TestTypes.ComplexDataStructure;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GRYLibrary.Tests.AdvancedObjectAnalysisTests.ToString
{
    [TestClass]
    public class GenericToStringTest
    {
        [TestMethod]
        public void SimpleDataStructureTestObjectToString()
        {
            // arrange
            SimpleDataStructure1 testObject = null;//todo
            string expectedString = @"TODO";

            // act
            string actualString = testObject.ToString();

            // assert
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void ComplexTestObjectToString()
        {
            // arrange
            Company testObject = null;//todo
            string expectedString = @"TODO";

            // act
            string actualString = testObject.ToString();

            // assert
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void CyclicTestObjectToString()
        {
            // arrange
            CycleA testObject = null;//todo
            string expectedString = @"TODO";

            // act
            string actualString = testObject.ToString();

            // assert
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
