﻿using GRYLibrary.Core.Graph;
using GRYLibrary.Core.Graph.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibrary.Tests.Testcases.GraphTests
{
    [TestClass]
    public class IsConnectedTest
    {
        [TestMethod]
        public void TestGraphWithOneVertex()
        {
            Graph graph = new UndirectedGraph();
            try
            {
                graph.IsConnected();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                //test passed
            }
            Vertex v1 = new("v1");
            graph.AddVertex(v1);
            Assert.IsTrue(graph.IsConnected());
            Vertex v2 = new("v2");
            graph.AddVertex(v2);
            Assert.IsFalse(graph.IsConnected());
            graph.AddEdge(new UndirectedEdge(new Vertex[] { v1, v2 }, "e1"));
            Assert.IsTrue(graph.IsConnected());
            Assert.ThrowsException<InvalidGraphStructureException>(() => graph.AddEdge(new UndirectedEdge(new Vertex[] { v2, v1 }, "e2")));
            Assert.IsTrue(graph.IsConnected());
        }
    }
}
