using GRYLibrary.Core.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Tests.GraphTests
{
    [TestClass]
    public class CycleTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Vertex v1 = new Vertex(nameof(v1));
            Vertex v2 = new Vertex(nameof(v2));
            Vertex v3 = new Vertex(nameof(v3));
            Vertex v4 = new Vertex(nameof(v4));
            Edge edge1 = new Edge(v3, v4, "e1");
            Edge edge2 = new Edge(v4, v1, "e2");
            Edge edge3 = new Edge(v1, v2, "e3");
            Edge edge4 = new Edge(v2, v3, "e4");

            List<Edge> cycleItems = new List<Edge>();
            cycleItems.Add(edge3);
            cycleItems.Add(edge4);
            cycleItems.Add(edge1);
            cycleItems.Add(edge2);

            Cycle cycle = new Cycle(cycleItems);

            List<Edge> cycleInternalOrder = new List<Edge>();
            cycleInternalOrder.Add(edge3);
            cycleInternalOrder.Add(edge4);
            cycleInternalOrder.Add(edge1);
            cycleInternalOrder.Add(edge2);

            Assert.IsTrue(cycle.Edges.SequenceEqual(cycleInternalOrder));
        }
        [TestMethod]
        public void TestRepresentsCycle()
        {
            Vertex v1 = new Vertex(nameof(v1));
            Vertex v2 = new Vertex(nameof(v2));
            Vertex v3 = new Vertex(nameof(v3));
            Vertex v4 = new Vertex(nameof(v3));
            Edge edge1 = new Edge(v1, v2, "e1");
            Edge edge2 = new Edge(v2, v3, "e2");
            Edge edge3 = new Edge(v3, v1, "e3");
            Edge edge4 = new Edge(v3, v4, "e4");
            Edge edge5 = new Edge(v4, v3, "e5");

            List<Edge> cycleItems = new List<Edge>();
            cycleItems.Add(edge1);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));
            cycleItems.Add(edge2);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));
            cycleItems.Add(edge3);
            Assert.IsTrue(Cycle.RepresentsCycle(cycleItems));
            cycleItems.Add(edge4);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));
            cycleItems.Add(edge5);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));


        }
    }
}
