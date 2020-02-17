using GRYLibrary.Core.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.GraphTests
{
    [TestClass]
    public class EdgeTest
    {
        [TestMethod]
        public void TestOperators()
        {
            Edge edge1 = new Edge(null, null, "e1");
            Edge edge2 = new Edge(null, null, "e2");
            Edge edge3 = new Edge(null, null, "e3");
            Edge edge4 = new Edge(null, null, "e4");
            Assert.IsTrue(edge1 < edge2);
            Assert.IsTrue(edge1 < edge3);
            Assert.IsTrue(edge1 < edge4);
            Assert.IsTrue(edge2 < edge3);
            Assert.IsTrue(edge3 < edge4);
            Assert.IsFalse(edge1 > edge2);
            Assert.IsFalse(edge2 > edge3);
            Assert.IsFalse(edge1 > edge4);
            Assert.IsFalse(edge2 > edge4);
            Assert.IsFalse(edge3 > edge4);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(edge1 == edge1);
            Assert.IsFalse(edge1 != edge1);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.IsTrue(edge1 != edge2);
            Assert.IsFalse(edge1 == edge2);
        }
    }
}
