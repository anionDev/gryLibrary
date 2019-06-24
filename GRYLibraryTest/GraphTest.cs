using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibraryTest
{
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
            Assert.AreNotEqual(v1, v2);
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
        }
        [TestMethod]
        public void EdgeEquals()
        {
            Vertex v1 = new Vertex("v1");
            Vertex v2 = new Vertex("v2");
            Edge e1 = new Edge(v1, v2);
            Edge e21 = new Edge(v1, v2);
            Edge e22 = new Edge(v2, v1);
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
            Assert.IsTrue(g.TryGetConnectionBetween(v1, v2, out e1Reloaded));
            Assert.AreEqual(e1, e1Reloaded);

            Edge e42 = new Edge(v4, v5);
            try
            {
                g.AddEdge(e42);//edge this this source and target does already exist
                Assert.Fail();
            }
            catch
            {
            }

            //test TryGetConnectionBetween with selfloop:
            Edge eSelfLoop = new Edge(v1, v1);
            g.AddEdge(eSelfLoop);
            Edge eSelfLoopReloaded;
            g.TryGetConnectionBetween(v1, v1, out eSelfLoopReloaded);
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
            Assert.IsFalse(g.TryGetConnectionBetween(v1, v1, out _));
            g.SelfLoopIsAllowed = false;
            Assert.IsFalse(g.SelfLoopIsAllowed);
            Assert.AreEqual(5, g.Vertices.Count());
            Assert.AreEqual(5, g.Edges.Count());

            ISet<Vertex> successorsOfv5 = v5.GetSuccessors(g);
            Assert.AreEqual(1, successorsOfv5.Count);
            Assert.AreEqual(v1, successorsOfv5.First());

        }

    }

}
