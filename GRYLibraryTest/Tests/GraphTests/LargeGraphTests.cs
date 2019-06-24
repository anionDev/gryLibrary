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
    public class LargeGraphTests
    {
        public readonly UndirectedGraph NotDirectedConnectedCyclicGraph = GetNotDirectedConnectedCyclicGraph();
        public readonly UndirectedGraph NotDirectedConnectedNotCyclicGraph = GetNotDirectedConnectedNotCyclicGraph();
        public readonly UndirectedGraph NotDirectedNotConnectedCyclicGraph = GetNotDirectedNotConnectedCyclicGraph();
        public readonly UndirectedGraph NotDirectedNotConnectedNotCyclicGraph = GetNotDirectedNotConnectedNotCyclicGraph();
        public readonly DirectedGraph DirectedConnectedCyclicGraph = GetDirectedConnectedCyclicGraph();
        public readonly DirectedGraph DirectedConnectedNotCyclicGraph = GetDirectedConnectedNotCyclicGraph();
        public readonly DirectedGraph DirectedNotConnectedCyclicGraph = GetDirectedNotConnectedCyclicGraph();
        public readonly DirectedGraph DirectedNotConnectedNotCyclicGraph = GetDirectedNotConnectedNotCyclicGraph();

        private static UndirectedGraph CreateUndirectedGraph(IEnumerable<Edge> bigStructure)
        {
            UndirectedGraph result = new UndirectedGraph();
            foreach (Edge edge in bigStructure)
            {
                result.AddEdge(edge);
            }
            return result;
        }

        private static void MakeUnconnected(IList<Edge> existingStructure)
        {
            foreach (Edge item in CreateRandomConnectedNotCyclicEdgesWithNewVertices())
            {
                existingStructure.Add(item);
            }
        }

        private static IEnumerable<Edge> CreateRandomConnectedNotCyclicEdgesWithNewVertices()
        {
            throw new NotImplementedException();
        }

        private static IList<Edge> GetLargeUndirectedConnectedNotCyclicGraphStructure()
        {
            throw new NotImplementedException();
        }

        private static void AddCycle(IList<Edge> existingStructure)
        {
            throw new NotImplementedException();
        }
        private static DirectedGraph CreateDirectedGraph(IEnumerable<Edge> bigStructure)
        {
            var result = new DirectedGraph();
            foreach (var edge in bigStructure)
            {
                result.AddEdge(edge);
            }
            return result;
        }

        private static UndirectedGraph GetNotDirectedConnectedCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            AddCycle(bigStructure);
            return CreateUndirectedGraph(bigStructure);
        }

        private static UndirectedGraph GetNotDirectedConnectedNotCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            return CreateUndirectedGraph(bigStructure);
        }

        private static UndirectedGraph GetNotDirectedNotConnectedCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            MakeUnconnected(bigStructure);
            AddCycle(bigStructure);
            return CreateUndirectedGraph(bigStructure);
        }

        private static UndirectedGraph GetNotDirectedNotConnectedNotCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            MakeUnconnected(bigStructure);
            return CreateUndirectedGraph(bigStructure);
        }

        private static DirectedGraph GetDirectedConnectedCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            MakeUnconnected(bigStructure);
            AddCycle(bigStructure);
            return CreateDirectedGraph(bigStructure);
        }

        private static DirectedGraph GetDirectedConnectedNotCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            return CreateDirectedGraph(bigStructure);
        }

        private static DirectedGraph GetDirectedNotConnectedCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            MakeUnconnected(bigStructure);
            AddCycle(bigStructure);
            return CreateDirectedGraph(bigStructure);
        }


        private static DirectedGraph GetDirectedNotConnectedNotCyclicGraph()
        {
            IList<Edge> bigStructure = GetLargeUndirectedConnectedNotCyclicGraphStructure();
            MakeUnconnected(bigStructure);
            return CreateDirectedGraph(bigStructure);
        }
    }
}
