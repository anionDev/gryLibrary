using GRYLibrary.Core.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class SimilarityTest
    {
        #region Combined
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCalculateCombinedSimilarity1()
        {
            TestRange(0.8.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCombinedSimilarity("123a5678", "123b5678"), 0.99.ToPercentValue());
            TestRange(0.01.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCombinedSimilarity("12345678", "a2cdefgh"), 0.2.ToPercentValue());
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCombinedSimilarity("1234", "56789").Value);
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateCombinedSimilarity("1234", "1234").Value);
        }
        #endregion 
        #region Jaccard
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCalculateJaccardSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity(testString, testString).Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestJaccardIndexWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0.5, Core.Miscellaneous.Utilities.CalculateJaccardIndex(testString, testString));
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestJaccardSimilarityWithDifferentValues()
        {
            Assert.AreEqual(4 / (decimal)5, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("test1", "test2").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("test", string.Empty).Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity(string.Empty, "test").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("test", "a").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("a", "test").Value);
            Assert.AreEqual((decimal)1 / 4, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("test", "aeaa").Value);
            Assert.AreEqual((decimal)1 / 4, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("aeaa", "test").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("abcd", "efgh").Value);
            Assert.AreEqual((decimal)3 / 8, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("xx345xxx", "yy345yyy").Value);
            Assert.AreEqual((decimal)5 / 8, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("12xxx678", "12yyy678").Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestJaccardIndexSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateJaccardSimilarity("abcd", "efgh").Value);
        }
        #endregion
        #region Levenshtein
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestLevenshteinSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity(testString, testString).Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestLevenshteinDistanceWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance(testString, testString));
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestLevenshteinDistanceWithDifferentValues()
        {
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("test", "test2"));
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("test1", "test2"));
            Assert.AreEqual(4, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("test", string.Empty));
            Assert.AreEqual(4, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance(string.Empty, "test"));
            Assert.AreEqual(4, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("test", "a"));
            Assert.AreEqual(4, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("a", "test"));
            Assert.AreEqual(3, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("test", "aeaa"));
            Assert.AreEqual(3, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("aeaa", "test"));
            Assert.AreEqual(4, Core.Miscellaneous.Utilities.CalculateLevenshteinDistance("abcd", "efgh"));
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestLevenshteinSimilarityWithDifferentValues()
        {
            Assert.AreEqual((decimal)4 / 5, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("test", "test2").Value);
            Assert.AreEqual((decimal)4 / 5, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("test1", "test2").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("test", string.Empty).Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity(string.Empty, "test").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("test", "a").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("a", "test").Value);
            Assert.AreEqual((decimal)1 / 4, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("test", "aeaa").Value);
            Assert.AreEqual((decimal)1 / 4, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("aeaa", "test").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("abcd", "efgh").Value);
            Assert.AreEqual((decimal)3 / 8, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("xx345xxx", "yy345yyy").Value);
            Assert.AreEqual((decimal)5 / 8, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("12xxx678", "12yyy678").Value);
            Assert.AreEqual((decimal)7 / 10, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("12345678", "1x34567890").Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestLevenshteinSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateLevenshteinSimilarity("abcd", "efgh").Value);
        }

        #endregion
        #region Cosine
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCosineSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Miscellaneous.Utilities.CalculateCosineSimilarity(testString, testString).Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCosineSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity("abcd", "efgh").Value);
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCosineSimilarityWithDifferentValues()
        {
            TestRange(0.9.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("test", "test2"), 1.ToPercentValue());
            TestRange(0.8.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("test1", "test2"), 0.9.ToPercentValue());
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity("test", string.Empty).Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity(string.Empty, "test").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity("test", "a").Value);
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity("a", "test").Value);
            TestRange(0.1.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("test", "aeaa"), 0.2.ToPercentValue());
            TestRange(0.1.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("aeaa", "test"), 0.2.ToPercentValue());
            Assert.AreEqual(0, Core.Miscellaneous.Utilities.CalculateCosineSimilarity("abcd", "efgh").Value);
            TestRange(0.1.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("xx345xxx", "yy345yyy"), 0.2.ToPercentValue());
            TestRange(0.3.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("12xxx678", "12yyy678"), 0.4.ToPercentValue());
            TestRange(0.7.ToPercentValue(), Core.Miscellaneous.Utilities.CalculateCosineSimilarity("12345678", "1x34567890"), 0.8.ToPercentValue());
        }

        #endregion
        #region Helper
        private void TestRange(PercentValue minimumValue, PercentValue actualValue, PercentValue maximalValue)
        {
            Assert.IsTrue(minimumValue < actualValue, $"Expected {minimumValue}<{actualValue}");
            Assert.IsTrue(actualValue < maximalValue, $"Expected {actualValue}<{maximalValue}");

            Assert.IsTrue(minimumValue.Value < actualValue.Value, $"Expected {minimumValue.Value}<{actualValue.Value}");
            Assert.IsTrue(actualValue.Value < maximalValue.Value, $"Expected {actualValue.Value}<{maximalValue.Value}");
        }
        #endregion
    }
}
