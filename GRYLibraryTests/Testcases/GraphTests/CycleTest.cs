using GRYLibrary.Core.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Tests.Testcases.GraphTests
{
    [TestClass]
    public class CycleTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Vertex v1 = new(nameof(v1));
            Vertex v2 = new(nameof(v2));
            Vertex v3 = new(nameof(v3));
            Vertex v4 = new(nameof(v4));
            DirectedEdge edge1 = new(v3, v4, "e1");
            DirectedEdge edge2 = new(v4, v1, "e2");
            DirectedEdge edge3 = new(v1, v2, "e3");
            DirectedEdge edge4 = new(v2, v3, "e4");

            List<Edge> cycleItems = new();
            cycleItems.Add(edge3);
            cycleItems.Add(edge4);
            cycleItems.Add(edge1);
            cycleItems.Add(edge2);

            Cycle cycle = new(cycleItems);

            List<DirectedEdge> cycleInternalOrder = new();
            cycleInternalOrder.Add(edge3);
            cycleInternalOrder.Add(edge4);
            cycleInternalOrder.Add(edge1);
            cycleInternalOrder.Add(edge2);

            Assert.IsTrue(cycle.Edges.SequenceEqual(cycleInternalOrder));
        }
        [TestMethod]
        public void TestRepresentsCycle()
        {
            Vertex v1 = new(nameof(v1));
            Vertex v2 = new(nameof(v2));
            Vertex v3 = new(nameof(v3));
            Vertex v4 = new(nameof(v3));
            DirectedEdge edge1 = new(v1, v2, "e1");
            DirectedEdge edge2 = new(v2, v3, "e2");
            DirectedEdge edge3 = new(v3, v1, "e3");
            DirectedEdge edge4 = new(v3, v4, "e4");
            DirectedEdge edge5 = new(v4, v3, "e5");

            List<Edge> cycleItems = new();

            cycleItems.Add(edge1);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));

            cycleItems.Add(edge2);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));

            cycleItems.Add(edge3);
            Assert.IsTrue(Cycle.RepresentsCycle(cycleItems));
            new Cycle(cycleItems);

            cycleItems.Add(edge4);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));

            cycleItems.Add(edge5);
            Assert.IsFalse(Cycle.RepresentsCycle(cycleItems));

        }
        public void TestErrorsOfCycleConstructorsDueToEmptyEdgesList()
        {
            Assert.ThrowsException<Exception>(() => new List<DirectedEdge>(new DirectedEdge[] { }));
        }
        public void TestErrorsOfCycleConstructorsDueToUncyclicEdges()
        {
            Vertex v1 = new(nameof(v1));
            Vertex v2 = new(nameof(v2));
            Vertex v3 = new(nameof(v3));
            Vertex v4 = new(nameof(v4));
            Vertex v5 = new(nameof(v5));
            DirectedEdge edge1 = new(v1, v2, "e1");
            DirectedEdge edge2 = new(v2, v3, "e2");
            DirectedEdge edge3 = new(v3, v1, "e3");
            Assert.ThrowsException<Exception>(() => new List<DirectedEdge>(new DirectedEdge[] { edge1, edge2 }));
        }
        public void TestErrorsOfCycleConstructorsDueToDuplicatedEdges()
        {
            Vertex v1 = new(nameof(v1));
            Vertex v2 = new(nameof(v2));
            Vertex v3 = new(nameof(v3));
            Vertex v4 = new(nameof(v4));
            Vertex v5 = new(nameof(v5));
            DirectedEdge edge1 = new(v1, v2, "e1");
            DirectedEdge edge2 = new(v2, v3, "e2");
            DirectedEdge edge3 = new(v3, v1, "e3");
            DirectedEdge edge4 = new(v1, v4, "e1");
            DirectedEdge edge5 = new(v4, v5, "e2");
            DirectedEdge edge6 = new(v5, v1, "e3");
            Assert.ThrowsException<Exception>(() => new List<DirectedEdge>(new DirectedEdge[] { edge1, edge2, edge3, edge4, edge5, edge6, }));
        }
    }
}
