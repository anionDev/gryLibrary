using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GraphOperations
{
    public class UndirectedGraph : Graph
    {
        public override ISet<Edge> Edges { get { return new HashSet<Edge>(_UndirectedEdges); } }
        public ISet<Edge> UndirectedEdges { get { return new HashSet<Edge>(_UndirectedEdges); } }
        private ISet<UndirectedEdge> _UndirectedEdges = new HashSet<UndirectedEdge>();
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
            foreach (DirectedEdge edge in vertex.ConnectedEdges)
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
        public override bool TryGetEdge(Vertex source, Vertex target, out DirectedEdge result)
        {
            foreach (DirectedEdge edge in this.Edges)
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

        public override ISet<DirectedEdge> GetDirectSuccessorEdges(Vertex vertex)
        {
            HashSet<DirectedEdge> result = new HashSet<DirectedEdge>();
            foreach (DirectedEdge edge in vertex.ConnectedEdges)
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

        public override void AddEdge(Edge edge)
        {
            if (!(edge is UndirectedEdge))
            {
                throw new Exception($"{nameof(UndirectedGraph)}-objects can only have edges of type {nameof(UndirectedEdge)}");
            }
            UndirectedEdge undirectedEdge = (UndirectedEdge)edge;
            var connectedVertices = undirectedEdge.ConnectedVertices.ToList();
            this.AddCheck(edge,connectedVertices[0],connectedVertices[1]);
            this._UndirectedEdges.Add((UndirectedEdge)edge);
        }

        public override void RemoveEdge(Edge edge)
        {
            if (edge is UndirectedEdge)
            {
                this._UndirectedEdges.Remove((UndirectedEdge)edge);
            }
        }
    }
}
