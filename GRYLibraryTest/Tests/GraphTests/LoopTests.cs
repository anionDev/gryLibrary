using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GRYLibraryTest.Tests.GraphTests
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
            DirectedGraph graph = this.GetTestGraphWithSimpleLoop();
            Assert.IsTrue(graph.ContainsOneOrMoreCycles());
            Assert.IsFalse(graph.ContainsOneOrMoreSelfLoops());
            Assert.IsTrue(graph.HasHamiltonianCycle(out _));
            ISet<Cycle> cycles = graph.GetAllCycles();
            Assert.AreEqual(1, cycles.Count);
        }
        [TestMethod]
        public void IsConnectedTest1()
        {
            Assert.IsTrue(this.GetTestGraphWithSimpleLoop().ToUndirectedGraph().IsConnected());
        }
            /// <returns>
            /// Returns a graph with the following structure:
            /// v0->v1->v2->v3->v4->v5
            /// ^                   |
            /// └-------------------┘
            /// </returns>
            private DirectedGraph GetTestGraphWithSimpleLoop()
        {
            DirectedGraph graph = new DirectedGraph();
            Vertex v0 = new Vertex("v0");
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Vertex v3 = new Vertex("v3");
            graph.AddEdge(new Edge(v0, v0, "e1"));
            graph.AddEdge(new Edge(v0, v1, "e2", 0.8));
            graph.AddEdge(new Edge(v1, v2, "e3"));
            graph.AddEdge(new Edge(v1, v3, "e4"));
            graph.AddEdge(new Edge(v2, v0, "e5", 0.2));
            graph.AddEdge(new Edge(v2, v3, "e6"));
            return graph;
        }
    }
}
