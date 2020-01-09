using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibraryTest.Tests.GraphTests
{
    //bookmark: ⭠⭢⭡⭣⭦⭧⭨⭩
    [TestClass]
    public class CycleFinderTest
    {
        [TestMethod]
        public void TestSimpleGraph()
        {
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

            Edge e01 = new Edge(a, b, nameof(e01)); graph.AddEdge(e01);
            Edge e02 = new Edge(b, c, nameof(e02)); graph.AddEdge(e02);
            Edge e03 = new Edge(c, d, nameof(e03)); graph.AddEdge(e03);
            Edge e04 = new Edge(d, e, nameof(e04)); graph.AddEdge(e04);
            Edge e05 = new Edge(e, f, nameof(e05)); graph.AddEdge(e05);
            Edge e06 = new Edge(f, g, nameof(e06)); graph.AddEdge(e06);
            Edge e07 = new Edge(g, h, nameof(e07)); graph.AddEdge(e07);
            Edge e08 = new Edge(h, a, nameof(e08)); graph.AddEdge(e08);
            Edge e09 = new Edge(b, g, nameof(e09)); graph.AddEdge(e09);
            Edge e10 = new Edge(f, c, nameof(e10)); graph.AddEdge(e10);

            ISet<Cycle> expectedCycles = new HashSet<Cycle>();
            expectedCycles.Add(new Cycle(new Edge[] { e01, e02, e03, e04, e05, e06, e07, e08 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e01, e09, e07, e08 }.ToList()));
            expectedCycles.Add(new Cycle(new Edge[] { e03, e04, e05, e10 }.ToList()));

            ISet<Cycle> foundCycles = graph.GetAllCycles();

            Assert.IsTrue(foundCycles.SetEquals(expectedCycles));
        }
    }
}
