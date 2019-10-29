using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibraryTest.Tests.GraphTests
{
    [TestClass]
    public class CycleTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            Vertex v1 = new Vertex(nameof(v1));
            Edge edge1 = new Edge(null, null, "e1");
            Edge edge2 = new Edge(null, v1, "e2");
            Edge edge3 = new Edge(v1, null, "e3");
            Edge edge4 = new Edge(null, null, "e4");

            List<Edge> cycleItems = new List<Edge>();
            cycleItems.Add(edge3);
            cycleItems.Add(edge4);
            cycleItems.Add(edge1);
            cycleItems.Add(edge2);

            Cycle cycle = new Cycle(cycleItems);

            List<Edge> cycleInternalOrder = new List<Edge>();
            cycleInternalOrder.Add(edge1);
            cycleInternalOrder.Add(edge2);
            cycleInternalOrder.Add(edge3);
            cycleInternalOrder.Add(edge4);

            Assert.IsTrue(cycle.Edges.SequenceEqual(cycleInternalOrder));
        }
        }
    }
