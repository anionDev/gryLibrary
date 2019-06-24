using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class UndirectedGraph : Graph
    {
        public static UndirectedGraph CreateByAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            UndirectedGraph result = new UndirectedGraph();
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


        public override bool TryGetConnectionBetween(Vertex vertex1, Vertex vertex2, out Edge connection)
        {
            foreach (Edge edge in this._Edges)
            {
                if ((edge.Source.Equals(vertex1) && edge.Target.Equals(vertex2)) || (edge.Target.Equals(vertex1) && edge.Source.Equals(vertex2)))
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
