using GRYLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class SimilarityTest
    {
        #region Jaccard
        [TestMethod]
        public void TestCalculateJaccardSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Utilities.CalculateJaccardSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestJaccardIndexWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0.5, Utilities.CalculateJaccardIndex(testString, testString));
        }
        [TestMethod]
        public void TestJaccardSimilarityWithDifferentValues()
        {
            Assert.AreEqual(8 / (double)9, Utilities.CalculateJaccardSimilarity("test", "test2"));
            Assert.AreEqual(4 / (double)5, Utilities.CalculateJaccardSimilarity("test1", "test2"));
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity("test", string.Empty));
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity(string.Empty, "test"));
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity("test", "a"));
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity("a", "test"));
            Assert.AreEqual(1 / (double)4, Utilities.CalculateJaccardSimilarity("test", "aeaa"));
            Assert.AreEqual(1 / (double)4, Utilities.CalculateJaccardSimilarity("aeaa", "test"));
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity("abcd", "efgh"));
            Assert.AreEqual(3 / (double)8, Utilities.CalculateJaccardSimilarity("xx345xxx", "yy345yyy"));
            Assert.AreEqual(5 / (double)8, Utilities.CalculateJaccardSimilarity("12xxx678", "12yyy678"));
            Assert.AreEqual(7 / (double)9, Utilities.CalculateJaccardSimilarity("12345678", "1x34567890"));
        }
        [TestMethod]
        public void TestJaccardIndexSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Utilities.CalculateJaccardSimilarity("abcd", "efgh"));
        }
        #endregion
        #region Levenshtein
        [TestMethod]
        public void TestLevenshteinSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Utilities.CalculateLevenshteinSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestLevenshteinDistanceWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(0, Utilities.CalculateLevenshteinDistance(testString, testString));
        }
        [TestMethod]
        public void TestLevenshteinDistanceWithDifferentValues()
        {
            Assert.AreEqual(1, Utilities.CalculateLevenshteinDistance("test", "test2"));
            Assert.AreEqual(1, Utilities.CalculateLevenshteinDistance("test1", "test2"));
            Assert.AreEqual(4, Utilities.CalculateLevenshteinDistance("test", string.Empty));
            Assert.AreEqual(4, Utilities.CalculateLevenshteinDistance(string.Empty, "test"));
            Assert.AreEqual(4, Utilities.CalculateLevenshteinDistance("test", "a"));
            Assert.AreEqual(4, Utilities.CalculateLevenshteinDistance("a", "test"));
            Assert.AreEqual(3, Utilities.CalculateLevenshteinDistance("test", "aeaa"));
            Assert.AreEqual(3, Utilities.CalculateLevenshteinDistance("aeaa", "test"));
            Assert.AreEqual(4, Utilities.CalculateLevenshteinDistance("abcd", "efgh"));
        }
        [TestMethod]
        public void TestLevenshteinSimilarityWithDifferentValues()
        {
            Assert.AreEqual(4 / (double)5, Utilities.CalculateLevenshteinSimilarity("test", "test2"));
            Assert.AreEqual(4 / (double)5, Utilities.CalculateLevenshteinSimilarity("test1", "test2"));
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity("test", string.Empty));
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity(string.Empty, "test"));
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity("test", "a"));
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity("a", "test"));
            Assert.AreEqual(1 / (double)4, Utilities.CalculateLevenshteinSimilarity("test", "aeaa"));
            Assert.AreEqual(1 / (double)4, Utilities.CalculateLevenshteinSimilarity("aeaa", "test"));
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity("abcd", "efgh"));
            Assert.AreEqual(3 / (double)8, Utilities.CalculateLevenshteinSimilarity("xx345xxx", "yy345yyy"));
            Assert.AreEqual(5 / (double)8, Utilities.CalculateLevenshteinSimilarity("12xxx678", "12yyy678"));
            Assert.AreEqual(7 / (double)10, Utilities.CalculateLevenshteinSimilarity("12345678", "1x34567890"));
        }
        [TestMethod]
        public void TestLevenshteinSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Utilities.CalculateLevenshteinSimilarity("abcd", "efgh"));
        }

        #endregion
        #region Cosine
        [TestMethod]
        public void TestCosineSimilarityWithEqualStrings()
        {
            string testString = "test";
            Assert.AreEqual(1, Utilities.CalculateCosineSimilarity(testString, testString));
        }
        [TestMethod]
        public void TestCosineSimilarityWithCompletelyDifferentStrings()
        {
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity("abcd", "efgh"));
        }
        [TestMethod]
        public void TestCosineSimilarityWithDifferentValues()
        {
            TestRange(0.9, Utilities.CalculateCosineSimilarity("test", "test2"), 1);
            TestRange(0.8, Utilities.CalculateCosineSimilarity("test1", "test2"), 0.9);
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity("test", string.Empty));
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity(string.Empty, "test"));
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity("test", "a"));
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity("a", "test"));
            TestRange(0.1, Utilities.CalculateCosineSimilarity("test", "aeaa"), 0.2);
            TestRange(0.1, Utilities.CalculateCosineSimilarity("aeaa", "test"), 0.2);
            Assert.AreEqual(0, Utilities.CalculateCosineSimilarity("abcd", "efgh"));
            TestRange(0.1, Utilities.CalculateCosineSimilarity("xx345xxx", "yy345yyy"), 0.2);
            TestRange(0.3, Utilities.CalculateCosineSimilarity("12xxx678", "12yyy678"), 0.4);
            TestRange(0.7, Utilities.CalculateCosineSimilarity("12345678", "1x34567890"), 0.8);
        }

        #endregion
        private void TestRange(double minimumValue, double actualValue, double maximalValue)
        {
            Assert.IsTrue(minimumValue < actualValue, $"Expected {minimumValue}<{actualValue}");
            Assert.IsTrue(actualValue < maximalValue, $"Expected {actualValue}<{maximalValue}");
        }
    }
}
