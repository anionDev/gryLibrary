﻿using System;
using System.Collections.Generic;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class DirectedGraph : Graph
    {
        public static DirectedGraph CreateByAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            DirectedGraph result = new DirectedGraph();
            return (DirectedGraph)FillByAdjacencyMatrix(result, adjacencyMatrix);
        }

        public override void Accept(IGraphVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IGraphVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public UndirectedGraph ToUndirectedGraph()
        {
            UndirectedGraph result = new UndirectedGraph();
            foreach (Vertex vertex in this._Vertices)
            {
                result.AddVertex(vertex.DeepClone());
            }
            foreach (Edge edge in this._Edges)
            {
                result.AddEdge(edge.DeepClone());
            }
            return result;
        }
        public override bool TryGetConnectionBetween(Vertex vertex1, Vertex vertex2, out Edge connection)
        {
            foreach (Edge edge in this._Edges)
            {
                if (edge.Source.Equals(vertex1) && edge.Target.Equals(vertex2))
                {
                    connection = edge;
                    return true;
                }
            }
            connection = null;
            return false;
        }
        /// <returns>Returns a set of all vertices which are a direct successor of this vertex in this graph.</returns>
        /// <remarks>If this graph contains a selfloop with this vertex then the result-set will also contain this vertex.
        /// The runtime of this function is &lt;=O(|this.Edges|).
        /// </remarks>
        public override ISet<Vertex> GetDirectSuccessors(Vertex vertex)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                if (edge.Source.Equals(vertex))
                {
                    result.Add(edge.Target);
                }
            }
            return result;
        }

        /// <returns>Returns a set of all vertices which are a direct predecessor of this vertex in this graph.</returns>
        /// <remarks>
        /// If this graph contains a selfloop with this vertex then the result-set will also contain this vertex.
        /// The runtime of this function is &lt;=O(|this.Edges|).
        /// </remarks>
        public ISet<Vertex> GetDirectPredecessors(Vertex vertex)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            foreach (Edge edge in vertex.ConnectedEdges)
            {
                if (edge.Target.Equals(vertex))
                {
                    result.Add(edge.Source);
                }
            }
            return result;
        }

    }
}