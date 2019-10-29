using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GRYLibraryTest.Tests.GraphTests
{

    [TestClass]
    public class BreadthFirstSearchTest
    {
        [TestMethod]
        public void TestSimpleSearch()
        {
            int i = 0;
            List<int> order = new List<int>();
            Graph g = TestGraphs.GetTestGraphWithoutLoop();
            g.BreadthFirstSearch((v, edges) =>
            {
                i = i + 1;
                order.Add(i);
            });
            Assert.AreEqual(1, order[0]);
            Assert.IsTrue(new HashSet<int>(new int[] { 2, 3, 4 }).SetEquals(new int[] { order[1], order[2], order[3] }));
            Assert.IsTrue(new HashSet<int>(new int[] { 5, 6, 7, 8 }).SetEquals(new int[] { order[4], order[5], order[6], order[7] }));
            Assert.IsTrue(new HashSet<int>(new int[] { 9, 10 }).SetEquals(new int[] { order[8], order[9] }));
        }
    }
}
