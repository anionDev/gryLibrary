using GRYLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class SimilarityTest
    {
        #region Jaccard
        [TestMethod]
        public void TestCalculateJaccardSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Utilities.CalculateJaccardSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestJaccardIndexWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0.5, Core.Utilities.CalculateJaccardIndex(testString, testString));
        }
        [TestMethod]
        public void TestJaccardSimilarityWithDifferentValues()
        {
            Assert.AreEqual(8 / (double)9, Core.Utilities.CalculateJaccardSimilarity("test", "test2"));
            Assert.AreEqual(4 / (double)5, Core.Utilities.CalculateJaccardSimilarity("test1", "test2"));
            Assert.AreEqual(0,Core. Utilities.CalculateJaccardSimilarity("test", string.Empty));
            Assert.AreEqual(0,Core. Utilities.CalculateJaccardSimilarity(string.Empty, "test"));
            Assert.AreEqual(0,Core. Utilities.CalculateJaccardSimilarity("test", "a"));
            Assert.AreEqual(0, Core.Utilities.CalculateJaccardSimilarity("a", "test"));
            Assert.AreEqual(1 / (double)4, Core.Utilities.CalculateJaccardSimilarity("test", "aeaa"));
            Assert.AreEqual(1 / (double)4, Core.Utilities.CalculateJaccardSimilarity("aeaa", "test"));
            Assert.AreEqual(0, Core.Utilities.CalculateJaccardSimilarity("abcd", "efgh"));
            Assert.AreEqual(3 / (double)8, Core.Utilities.CalculateJaccardSimilarity("xx345xxx", "yy345yyy"));
            Assert.AreEqual(5 / (double)8, Core.Utilities.CalculateJaccardSimilarity("12xxx678", "12yyy678"));
            Assert.AreEqual(7 / (double)9, Core.Utilities.CalculateJaccardSimilarity("12345678", "1x34567890"));
        }
        [TestMethod]
        public void TestJaccardIndexSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Utilities.CalculateJaccardSimilarity("abcd", "efgh"));
        }
        #endregion
        #region Levenshtein
        [TestMethod]
        public void TestLevenshteinSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Utilities.CalculateLevenshteinSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestLevenshteinDistanceWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0, Core.Utilities.CalculateLevenshteinDistance(testString, testString));
        }
        [TestMethod]
        public void TestLevenshteinDistanceWithDifferentValues()
        {
            Assert.AreEqual(1, Core.Utilities.CalculateLevenshteinDistance("test", "test2"));
            Assert.AreEqual(1,Core.Utilities.CalculateLevenshteinDistance("test1", "test2"));
            Assert.AreEqual(4,Core.Utilities.CalculateLevenshteinDistance("test", string.Empty));
            Assert.AreEqual(4,Core.Utilities.CalculateLevenshteinDistance(string.Empty, "test"));
            Assert.AreEqual(4,Core.Utilities.CalculateLevenshteinDistance("test", "a"));
            Assert.AreEqual(4,Core.Utilities.CalculateLevenshteinDistance("a", "test"));
            Assert.AreEqual(3,Core.Utilities.CalculateLevenshteinDistance("test", "aeaa"));
            Assert.AreEqual(3,Core.Utilities.CalculateLevenshteinDistance("aeaa", "test"));
            Assert.AreEqual(4,Core.Utilities.CalculateLevenshteinDistance("abcd", "efgh"));
        }
        [TestMethod]
        public void TestLevenshteinSimilarityWithDifferentValues()
        {
            Assert.AreEqual(4 / (double)5, Core.Utilities.CalculateLevenshteinSimilarity("test", "test2"));
            Assert.AreEqual(4 / (double)5,Core. Utilities.CalculateLevenshteinSimilarity("test1", "test2"));
            Assert.AreEqual(0, Core.Utilities.CalculateLevenshteinSimilarity("test", string.Empty));
            Assert.AreEqual(0,Core. Utilities.CalculateLevenshteinSimilarity(string.Empty, "test"));
            Assert.AreEqual(0,Core. Utilities.CalculateLevenshteinSimilarity("test", "a"));
            Assert.AreEqual(0,Core. Utilities.CalculateLevenshteinSimilarity("a", "test"));
            Assert.AreEqual(1 / (double)4, Core.Utilities.CalculateLevenshteinSimilarity("test", "aeaa"));
            Assert.AreEqual(1 / (double)4, Core.Utilities.CalculateLevenshteinSimilarity("aeaa", "test"));
            Assert.AreEqual(0, Core.Utilities.CalculateLevenshteinSimilarity("abcd", "efgh"));
            Assert.AreEqual(3 / (double)8, Core.Utilities.CalculateLevenshteinSimilarity("xx345xxx", "yy345yyy"));
            Assert.AreEqual(5 / (double)8, Core.Utilities.CalculateLevenshteinSimilarity("12xxx678", "12yyy678"));
            Assert.AreEqual(7 / (double)10, Core.Utilities.CalculateLevenshteinSimilarity("12345678", "1x34567890"));
        }
        [TestMethod]
        public void TestLevenshteinSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Utilities.CalculateLevenshteinSimilarity("abcd", "efgh"));
        }

        #endregion
        #region Cosine
        [TestMethod]
        public void TestCosineSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Core.Utilities.CalculateCosineSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestCosineSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Core.Utilities.CalculateCosineSimilarity("abcd", "efgh"));
        }
        [TestMethod]
        public void TestCosineSimilarityWithDifferentValues()
        {
            TestRange(0.9, Core.Utilities.CalculateCosineSimilarity("test", "test2"), 1);
            TestRange(0.8, Core.Utilities.CalculateCosineSimilarity("test1", "test2"), 0.9);
            Assert.AreEqual(0,Core. Utilities.CalculateCosineSimilarity("test", string.Empty));
            Assert.AreEqual(0,Core. Utilities.CalculateCosineSimilarity(string.Empty, "test"));
            Assert.AreEqual(0,Core. Utilities.CalculateCosineSimilarity("test", "a"));
            Assert.AreEqual(0, Core.Utilities.CalculateCosineSimilarity("a", "test"));
            TestRange(0.1, Core.Utilities.CalculateCosineSimilarity("test", "aeaa"), 0.2);
            TestRange(0.1, Core.Utilities.CalculateCosineSimilarity("aeaa", "test"), 0.2);
            Assert.AreEqual(0, Core.Utilities.CalculateCosineSimilarity("abcd", "efgh"));
            TestRange(0.1, Core.Utilities.CalculateCosineSimilarity("xx345xxx", "yy345yyy"), 0.2);
            TestRange(0.3, Core.Utilities.CalculateCosineSimilarity("12xxx678", "12yyy678"), 0.4);
            TestRange(0.7, Core.Utilities.CalculateCosineSimilarity("12345678", "1x34567890"), 0.8);
        }

        #endregion
        private void TestRange(double minimumValue, double actualValue, double maximalValue)
        {
            Assert.IsTrue(minimumValue < actualValue, $"Expected {minimumValue}<{actualValue}");
            Assert.IsTrue(actualValue < maximalValue, $"Expected {actualValue}<{maximalValue}");
        }
    }
}
