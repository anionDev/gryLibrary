using System;
using System.Collections.Generic;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class DirectedGraph : Graph
    {
        public static DirectedGraph CreateByAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            DirectedGraph result = new DirectedGraph();
            Tuple<IList<Edge>, IList<Vertex>> items = result.ParseAdjacencyMatrix(adjacencyMatrix);
            foreach (Vertex item in items.Item2)
            {
                result._Vertices.Add(item);
            }
            foreach (Edge item in items.Item1)
            {
                result._Edges.Add(item);
            }
            return result;
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

    }
}
