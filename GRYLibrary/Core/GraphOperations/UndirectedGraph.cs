using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class UndirectedGraph : Graph
    {
        public static UndirectedGraph CreateByAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            UndirectedGraph result = new UndirectedGraph();
            return (UndirectedGraph)FillByAdjacencyMatrix(result, adjacencyMatrix);
        }
        public override void Accept(IGraphVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IGraphVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        /// <returns>Returns a set of all vertices which have a connection to this vertex in this graph.</returns>
        /// <remarks>
        /// If this graph contains a selfloop with this vertex then the result-set will also contain this vertex.
        /// The runtime of this function is &lt;=O(|this.Edges|).
        /// </remarks>
        public override ISet<Vertex> GetDirectSuccessors(Vertex vertex)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                bool sourceIsThis = edge.Source.Equals(vertex);
                bool targetIsThis = edge.Target.Equals(vertex);
                if (sourceIsThis && targetIsThis)
                {
                    result.Add(vertex);
                }
                else
                {
                    if (sourceIsThis)
                    {
                        result.Add(edge.Target);
                    }
                    if (targetIsThis)
                    {
                        result.Add(edge.Source);
                    }
                }
            }
            return result;
        }
        public override bool TryGetEdge(Vertex source, Vertex target, out Edge result)
        {
            foreach (Edge edge in this.Edges)
            {
                if ((edge.Source.Equals(source) && edge.Target.Equals(target)) || (edge.Source.Equals(target) && edge.Target.Equals(source)))
                {
                    result = edge;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public override ISet<Edge> GetDirectSuccessorEdges(Vertex vertex)
        {
            HashSet<Edge> result = new HashSet<Edge>();
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                bool sourceIsThis = edge.Source.Equals(vertex);
                bool targetIsThis = edge.Target.Equals(vertex);
                if (sourceIsThis && targetIsThis)
                {
                    result.Add(edge);
                }
                else
                {
                    if (sourceIsThis)
                    {
                        result.Add(edge);
                    }
                    if (targetIsThis)
                    {
                        result.Add(edge);
                    }
                }
            }
            return result;
        }
    }
}
