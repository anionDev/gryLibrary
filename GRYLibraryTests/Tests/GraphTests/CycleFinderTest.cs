using GRYLibrary.Core.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Tests.GraphTests
{
    //bookmark: ⭠⭢⭡⭣⭦⭧⭨⭩
    [TestClass]
    public class CycleFinderTest
    {
            [TestMethod]
        public void TestSimpleGraph()
        {
            throw new System.Exception("caution: endless-loop due to bug in GetAllCyclesThroughASpecificVertex");
            /*
             * Graph:
             * A⭢B⭢C⭢D
             * ⭡ ⭣  ⭡  ⭣
             * H⭠G⭠F⭠E
             */
            DirectedGraph graph = new DirectedGraph();
            Vertex a = new Vertex(nameof(a)); graph.AddVertex(a);
            Vertex b = new Vertex(nameof(b)); graph.AddVertex(b);
            Vertex c = new Vertex(nameof(c)); graph.AddVertex(c);
            Vertex d = new Vertex(nameof(d)); graph.AddVertex(d);
            Vertex e = new Vertex(nameof(e)); graph.AddVertex(e);
            Vertex f = new Vertex(nameof(f)); graph.AddVertex(f);
            Vertex g = new Vertex(nameof(g)); graph.AddVertex(g);
            Vertex h = new Vertex(nameof(h)); graph.AddVertex(h);

            DirectedEdge e01 = new DirectedEdge(a, b, nameof(e01)); graph.AddEdge(e01);
            DirectedEdge e02 = new DirectedEdge(b, c, nameof(e02)); graph.AddEdge(e02);
            DirectedEdge e03 = new DirectedEdge(c, d, nameof(e03)); graph.AddEdge(e03);
            DirectedEdge e04 = new DirectedEdge(d, e, nameof(e04)); graph.AddEdge(e04);
            DirectedEdge e05 = new DirectedEdge(e, f, nameof(e05)); graph.AddEdge(e05);
            DirectedEdge e06 = new DirectedEdge(f, g, nameof(e06)); graph.AddEdge(e06);
            DirectedEdge e07 = new DirectedEdge(g, h, nameof(e07)); graph.AddEdge(e07);
            DirectedEdge e08 = new DirectedEdge(h, a, nameof(e08)); graph.AddEdge(e08);
            DirectedEdge e09 = new DirectedEdge(b, g, nameof(e09)); graph.AddEdge(e09);
            DirectedEdge e10 = new DirectedEdge(f, c, nameof(e10)); graph.AddEdge(e10);

            ISet<Cycle> expectedCycles = new HashSet<Cycle>();
            expectedCycles.Add(new Cycle(new Edge[] { e01, e02, e03, e04, e05, e06, e07, e08 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e01, e09, e07, e08 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e03, e04, e05, e10 }.ToList()));

            ISet<Cycle> foundCycles = graph.GetAllCycles();

            Assert.IsTrue(foundCycles.SetEquals(expectedCycles));
        }
        [TestMethod]
        public void TestSimpleGraph2()
        {
            throw new System.Exception("caution: endless-loop due to bug in GetAllCyclesThroughASpecificVertex");
            DirectedGraph graph = new DirectedGraph();
            Vertex a = new Vertex(nameof(a)); graph.AddVertex(a);
            Vertex b = new Vertex(nameof(b)); graph.AddVertex(b);
            Vertex c = new Vertex(nameof(c)); graph.AddVertex(c);
            Vertex d = new Vertex(nameof(d)); graph.AddVertex(d);
            Vertex e = new Vertex(nameof(e)); graph.AddVertex(e);

            DirectedEdge e01 = new DirectedEdge(a, b, nameof(e01)); graph.AddEdge(e01);
            DirectedEdge e02 = new DirectedEdge(b, c, nameof(e02)); graph.AddEdge(e02);
            DirectedEdge e03 = new DirectedEdge(c, d, nameof(e03)); graph.AddEdge(e03);
            DirectedEdge e04 = new DirectedEdge(d, e, nameof(e04)); graph.AddEdge(e04);
            DirectedEdge e05 = new DirectedEdge(e, a, nameof(e05)); graph.AddEdge(e05);
            DirectedEdge e06 = new DirectedEdge(a, c, nameof(e06)); graph.AddEdge(e06);
            DirectedEdge e07 = new DirectedEdge(b, e, nameof(e07)); graph.AddEdge(e07);
            DirectedEdge e08 = new DirectedEdge(d, b, nameof(e08)); graph.AddEdge(e08);

            ISet<Cycle> expectedCycles = new HashSet<Cycle>();
            expectedCycles.Add(new Cycle(new Edge[] { e01, e02, e03, e04, e05 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e06, e03, e08, e07,e05 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e02, e03, e08 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e01, e07, e05 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e06, e03, e04, e05 }.ToList()));

            ISet<Cycle> foundCycles = graph.GetAllCycles();

            Assert.IsTrue(foundCycles.SetEquals(expectedCycles));

        }
    }
}
