using GRYLibrary.Miscellaneous.GraphOperations;

namespace GRYLibraryTest.Tests.GraphTests
{
    internal class TestGraphs
    {
        /// <returns>
        /// Returns a graph with the following structure:
        /// v0->v1->v2->v3->v4->v5
        /// ^                   |
        /// └-------------------┘
        /// </returns>
        internal static DirectedGraph GetTestGraphWithSimpleLoop()
        {
            DirectedGraph graph = new DirectedGraph();
            Vertex v0 = new Vertex("v0");
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Vertex v3 = new Vertex("v3");
            Vertex v4 = new Vertex("v4");
            Vertex v5 = new Vertex("v5");
            graph.AddEdge(new Edge(v0, v1, "e1"));
            graph.AddEdge(new Edge(v1, v2, "e2"));
            graph.AddEdge(new Edge(v2, v3, "e3"));
            graph.AddEdge(new Edge(v3, v4, "e4"));
            graph.AddEdge(new Edge(v4, v5, "e5"));
            graph.AddEdge(new Edge(v5, v0, "e6"));
            return graph;
        }
        /// <returns>
        /// Returns a graph with the following structure:
        /// v0->v1->v2->v3->v4->v5
        /// </returns>
        internal static DirectedGraph GetTestGraphWithoutLoop2()
        {
            DirectedGraph graph = new DirectedGraph();
            Vertex v0 = new Vertex("v0");
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Vertex v3 = new Vertex("v3");
            Vertex v4 = new Vertex("v4");
            Vertex v5 = new Vertex("v5");
            graph.AddEdge(new Edge(v0, v1, "e1"));
            graph.AddEdge(new Edge(v1, v2, "e2"));
            graph.AddEdge(new Edge(v2, v3, "e3"));
            graph.AddEdge(new Edge(v3, v4, "e4"));
            graph.AddEdge(new Edge(v4, v5, "e5"));
            return graph;
        }

        internal static UndirectedGraph GetTestGraphWithoutLoop()
        {
            UndirectedGraph graph = new UndirectedGraph();
            Vertex v1 = new Vertex(nameof(v1)); graph.AddVertex(v1);
            Vertex v2 = new Vertex(nameof(v2)); graph.AddVertex(v2);
            Vertex v3 = new Vertex(nameof(v3)); graph.AddVertex(v3);
            Vertex v4 = new Vertex(nameof(v4)); graph.AddVertex(v4);
            Vertex v5 = new Vertex(nameof(v5)); graph.AddVertex(v5);
            Vertex v6 = new Vertex(nameof(v6)); graph.AddVertex(v6);
            Vertex v7 = new Vertex(nameof(v7)); graph.AddVertex(v7);
            Vertex v8 = new Vertex(nameof(v8)); graph.AddVertex(v8);
            Vertex v9 = new Vertex(nameof(v9)); graph.AddVertex(v9);
            Vertex v10 = new Vertex(nameof(v10)); graph.AddVertex(v10);

            Edge e1 = new Edge(v1, v2, nameof(e1)); graph.AddEdge(e1);
            Edge e2 = new Edge(v1, v3, nameof(e2)); graph.AddEdge(e2);
            Edge e3 = new Edge(v1, v4, nameof(e3)); graph.AddEdge(e3);
            Edge e4 = new Edge(v2, v5, nameof(e4)); graph.AddEdge(e4);
            Edge e5 = new Edge(v3, v6, nameof(e5)); graph.AddEdge(e5);
            Edge e6 = new Edge(v3, v7, nameof(e6)); graph.AddEdge(e6);
            Edge e7 = new Edge(v4, v8, nameof(e7)); graph.AddEdge(e7);
            Edge e8 = new Edge(v5, v9, nameof(e8)); graph.AddEdge(e8);
            Edge e9 = new Edge(v6, v10, nameof(e9)); graph.AddEdge(e9);

            return graph;

        }
    }
}
