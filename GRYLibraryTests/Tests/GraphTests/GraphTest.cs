using System.Collections.Generic;
using System.Linq;
using GRYLibrary.Core;
using GRYLibrary.Core.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.GraphTests
{
    /// <summary>
    /// Contains simple graph tests
    /// </summary>
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void SimpleVertexTest()
        {
            Vertex v1 = new Vertex("v1");
            Assert.AreEqual(0, v1.ConnectedEdges.Count());
            Assert.AreEqual("v1", v1.Name);
            Assert.AreEqual(v1, v1);
        }
        [TestMethod]
        public void VertexEqual()
        {
            Vertex v1 = new Vertex("v");
            Vertex v2 = new Vertex("v");
            Assert.AreEqual(v1, v2);
        }
        [TestMethod]
        public void SimpleEdgeTest()
        {
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Edge e1 = new Edge(v1, v2, "e", 1.5);
            Assert.AreEqual(v1, e1.Source);
            Assert.AreEqual(v2, e1.Target);
            Assert.AreEqual("e", e1.Name);
            Assert.AreEqual(1.5, e1.Weight);
            Edge e2 = new Edge(v1, v2, "e", 1.5);
            Assert.AreEqual(e1, e2);
            Edge e3 = new Edge(v1, v2, "e", 1.6);
            Assert.AreEqual(e1, e3);
            Edge e4 = new Edge(v2, v1, "e", 1.5);
            Assert.AreEqual(e1, e4);
            Edge e5 = new Edge(v1, new Vertex("v3"), "e", 1.5);
            Assert.AreEqual(e1, e5);

        }
        [TestMethod]
        public void EdgeEquals()
        {
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Edge e1 = new Edge(v1, v2, "e1");
            Edge e21 = new Edge(v1, v2, "e2");
            Edge e22 = new Edge(v2, v1, "e3");
            Assert.AreEqual(e1, e1);
            Assert.AreNotEqual(e1, e21);
            Assert.AreNotEqual(e1, e22);
        }
        [TestMethod]
        public void SimpleDirectedGraphTest()
        {
            DirectedGraph g = new DirectedGraph();
            Assert.IsTrue(g.SelfLoopIsAllowed);
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Vertex v3 = new Vertex("v3");
            Vertex v4 = new Vertex("v4");
            Vertex v5 = new Vertex("v5");

            Edge e1 = new Edge(v1, v2, "e1");
            Edge e2 = new Edge(v2, v3, "e2");
            Edge e3 = new Edge(v3, v4, "e3");
            Edge e4 = new Edge(v4, v5, "e4");
            Edge e5 = new Edge(v5, v1, "e5");

            g.AddVertex(v1);
            g.AddVertex(v2);
            g.AddVertex(v3);
            g.AddVertex(v4);
            g.AddVertex(v5);
            g.AddEdge(e1);
            g.AddEdge(e2);
            g.AddEdge(e3);
            g.AddEdge(e4);
            g.AddEdge(e5);

            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(5, g.Edges.Count());
            Assert.AreEqual(2, v1.ConnectedEdges.Count());

            //test TryGetConnectionBetween:
            Edge e1Reloaded;
            Assert.IsTrue(g.TryGetEdge(v1, v2, out e1Reloaded));
            Assert.AreEqual(e1, e1Reloaded);

            Edge e45 = new Edge(v4, v5, "e45");
            try
            {
                g.AddEdge(e45);//edge this this source and target does already exist
                Assert.Fail();
            }
            catch
            {
            }

            //test TryGetConnectionBetween with selfloop:
            Edge eSelfLoop = new Edge(v1, v1, "e11");
            g.AddEdge(eSelfLoop);
            Edge eSelfLoopReloaded;
            g.TryGetEdge(v1, v1, out eSelfLoopReloaded);
            Assert.AreEqual(eSelfLoop, eSelfLoopReloaded);
            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(6, g.Edges.Count());
            Assert.AreEqual(2, g.GetMinimumDegree());
            Assert.AreEqual(3, g.GetMaximumDegree());

            try
            {
                g.SelfLoopIsAllowed = false;//g does already have a selfloop
                Assert.Fail();
            }
            catch
            {
            }
            Assert.IsTrue(g.SelfLoopIsAllowed);

            g.RemoveEdge(eSelfLoop);
            Assert.IsFalse(g.TryGetEdge(v1, v1, out _));
            g.SelfLoopIsAllowed = false;
            Assert.IsFalse(g.SelfLoopIsAllowed);
            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(5, g.Edges.Count());

            ISet<Vertex> successorsOfv5 = g.GetDirectSuccessors(v5);
            Assert.AreEqual(1, successorsOfv5.Count);
            Assert.AreEqual(v1, successorsOfv5.First());

            Edge e43 = new Edge(v4, v3, "e43");
            g.AddEdge(e43);
            ISet<Vertex> successorsOfv4 = g.GetDirectSuccessors(v4);
            Assert.AreEqual(2, successorsOfv4.Count);
            Assert.IsTrue(new HashSet<Vertex>() { v3, v5 }.SetEquals(successorsOfv4));
        }
        [TestMethod]
        public void SimpleUndrectedGraphTest()
        {
            UndirectedGraph g = new UndirectedGraph();
            Assert.IsTrue(g.SelfLoopIsAllowed);
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Vertex v3 = new Vertex("v3");
            Vertex v4 = new Vertex("v4");
            Vertex v5 = new Vertex("v5");

            Edge e1 = new Edge(v1, v2, "e1");
            Edge e2 = new Edge(v2, v3, "e2");
            Edge e3 = new Edge(v3, v4, "e3");
            Edge e4 = new Edge(v4, v5, "e4");
            Edge e5 = new Edge(v5, v1, "e5");

            g.AddVertex(v1);
            g.AddVertex(v2);
            g.AddVertex(v3);
            g.AddVertex(v4);
            g.AddVertex(v5);
            g.AddEdge(e1);
            g.AddEdge(e2);
            g.AddEdge(e3);
            g.AddEdge(e4);
            g.AddEdge(e5);

            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(5, g.Edges.Count());
            Assert.AreEqual(2, v1.ConnectedEdges.Count());

            //test TryGetEdge:
            Edge e1Reloaded1;
            Assert.IsTrue(g.TryGetEdge(v1, v2, out e1Reloaded1));
            Assert.AreEqual(e1, e1Reloaded1);
            Edge e1Reloaded2;
            Assert.IsTrue(g.TryGetEdge(v2, v1, out e1Reloaded2));
            Assert.AreEqual(e1, e1Reloaded2);

            Assert.AreEqual(e1Reloaded1, e1Reloaded2);

            Edge e54 = new Edge(v5, v4, "e54");
            try
            {
                g.AddEdge(e54);//edge this this source and target does already exist
                Assert.Fail();
            }
            catch
            {
            }

            //test TryGetConnectionBetween with selfloop:
            Edge eSelfLoop = new Edge(v1, v1, "e11");
            g.AddEdge(eSelfLoop);
            Edge eSelfLoopReloaded;
            g.TryGetEdge(v1, v1, out eSelfLoopReloaded);
            Assert.AreEqual(eSelfLoop, eSelfLoopReloaded);
            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(6, g.Edges.Count());

            try
            {
                g.SelfLoopIsAllowed = false;//g does already have a selfloop
                Assert.Fail();
            }
            catch
            {
            }
            Assert.IsTrue(g.SelfLoopIsAllowed);

            g.RemoveEdge(eSelfLoop);
            Assert.IsFalse(g.TryGetEdge(v1, v1, out _));
            g.SelfLoopIsAllowed = false;
            Assert.IsFalse(g.SelfLoopIsAllowed);
            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(5, g.Edges.Count());

            ISet<Vertex> successorsOfv4 = g.GetDirectSuccessors(v4);
            Assert.AreEqual(2, successorsOfv4.Count);
            Assert.IsTrue(new HashSet<Vertex>() { v3, v5 }.SetEquals(successorsOfv4));
        }
        [TestMethod]
        public void GraphTpAdjacencyMatrixTest()
        {
            DirectedGraph graph = this.GetTestGraph();
            double[,] adjacencyMatrix = this.GetTestAdjacencyMatrix();
            double[,] toAdjacencyMatrixResult = graph.ToAdjacencyMatrix();
            Assert.IsTrue(Utilities.TwoDimensionalArrayEquals(adjacencyMatrix, toAdjacencyMatrixResult), $"Expected {Utilities.TwoDimensionalArrayToString(adjacencyMatrix)} but was {Utilities.TwoDimensionalArrayToString(toAdjacencyMatrixResult)}");
        }

        [TestMethod]
        public void AdjacencyMatrixToGraphTest()
        {
            DirectedGraph graph = this.GetTestGraph();
            DirectedGraph createByAdjacencyMatrix = DirectedGraph.CreateByAdjacencyMatrix(this.GetTestAdjacencyMatrix());
            Assert.IsTrue(graph.Equals(createByAdjacencyMatrix), $"Expected <{Utilities.TwoDimensionalArrayToString(graph.ToAdjacencyMatrix())}> but was <{Utilities.TwoDimensionalArrayToString(createByAdjacencyMatrix.ToAdjacencyMatrix())}>");
        }
        private double[,] GetTestAdjacencyMatrix()
        {
            double[,] adjacencyMatrix = new double[4, 4];
            adjacencyMatrix[0, 0] = 1;
            adjacencyMatrix[0, 1] = 0.8;
            adjacencyMatrix[0, 2] = 0;
            adjacencyMatrix[0, 3] = 0;
            adjacencyMatrix[1, 0] = 0;
            adjacencyMatrix[1, 1] = 0;
            adjacencyMatrix[1, 2] = 1;
            adjacencyMatrix[1, 3] = 1;
            adjacencyMatrix[2, 0] = 0.2;
            adjacencyMatrix[2, 1] = 0;
            adjacencyMatrix[2, 2] = 0;
            adjacencyMatrix[2, 3] = 1;
            adjacencyMatrix[3, 0] = 0;
            adjacencyMatrix[3, 1] = 0;
            adjacencyMatrix[3, 2] = 0;
            adjacencyMatrix[3, 3] = 0;
            return adjacencyMatrix;
        }

        private DirectedGraph GetTestGraph()
        {
            DirectedGraph graph = new DirectedGraph();
            Vertex v0 = new Vertex("Vertex_1");
            Vertex v1 = new Vertex("Vertex_2");
            Vertex v2 = new Vertex("Vertex_3");
            Vertex v3 = new Vertex("Vertex_4");
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
