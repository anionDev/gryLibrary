using GRYLibrary.Core.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GRYLibrary.Tests.GraphTests
{
    /// <summary>
    /// Contains complex graph tests
    /// </summary>
    [TestClass]
    public class LoopTests
    {
        [TestMethod]
        public void HasOneOrMoreCycleTest1()
        {
            DirectedGraph graph = TestGraphs.GetTestGraphWithSimpleLoop();
            Assert.IsTrue(graph.ContainsOneOrMoreCycles());
            Assert.IsFalse(graph.ContainsOneOrMoreSelfLoops());
            Assert.IsTrue(graph.HasHamiltonianCycle(out _));
            ISet<Cycle> cycles = graph.GetAllCycles();
            Assert.AreEqual(1, cycles.Count);
        }
        [TestMethod]
        public void HasOneOrMoreCycleTest()
        {
            DirectedGraph graph = TestGraphs.GetTestGraphWithoutLoop2();
            Assert.IsFalse(graph.ContainsOneOrMoreCycles());
            Assert.IsFalse(graph.ContainsOneOrMoreSelfLoops());
        }
        [TestMethod]
        public void IsConnectedTest1()
        {
            Assert.IsTrue(TestGraphs.GetTestGraphWithSimpleLoop().ToUndirectedGraph().IsConnected());
        }
    }
}
