using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibraryTest.Tests.GraphTests
{
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
            ISet<Cycle> cycles = graph.GetCycles();
            Assert.AreEqual(1, cycles.Count);
        }
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
