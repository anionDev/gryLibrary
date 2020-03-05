using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Graph
{
    public class Cycle
    {
        public IList<Edge> Edges { get { return new List<Edge>(_Edges); } }
        private IList<Edge> _Edges = new List<Edge>();
        public Cycle(IList<Edge> edgesList)
        {
            List<Edge> edges = edgesList.ToList();
            if (edges.Count == 0)
            {
                throw new Exception("A cycle can not be empty.");
            }
            if (new HashSet<Edge>(edges).Count < edges.Count)
            {
                throw new Exception("A cycle can not contain two equal edges.");
            }
            if (!RepresentsCycle(edges))
            {
                throw new Exception("The given edge-list is not cyclic.");
            }
            this._Edges = edges;
        }
        public override bool Equals(object obj)
        {
            Cycle cycle = obj as Cycle;
            if (cycle == null)
            {
                return false;
            }
            int edgesCount = this._Edges.Count;
            if (this._Edges.Count != cycle._Edges.Count)
            {
                return false;
            }
            int indexOfStartItemInCycle2;
            if (cycle._Edges.Contains(this._Edges[0]))
            {
                indexOfStartItemInCycle2 = cycle._Edges.IndexOf(this._Edges[0]);
            }
            else
            {
                return false;
            }
            for (int indexOfStartItemInCycle1 = 0; indexOfStartItemInCycle1 < edgesCount; indexOfStartItemInCycle1++)
            {
                Edge edgeInCycle1 = this._Edges[indexOfStartItemInCycle1];
                Edge edgeInCycle2 = cycle._Edges[(indexOfStartItemInCycle2 + indexOfStartItemInCycle1) % edgesCount];
                if (!edgeInCycle1.Equals(edgeInCycle2))
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this._Edges.Count.GetHashCode();
        }

        public static bool RepresentsCycle(IList<Edge> edges)
        {
            if (edges.Count > 0)
            {
                if (new HashSet<Edge>(edges).Count != edges.Count)
                {
                    return false;
                }
                return EdgesAreCyclic(edges);
            }
            else
            {
                return false;
            }
        }

        private static bool EdgesAreCyclic(IList<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Edge edge1 = edges[i];
                Edge edge2 = edges[(i + 1) % edges.Count];
                if (!AtLeastOneOutputOfEdge1LeadsToInputOfEdge1(edge1, edge2))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AtLeastOneOutputOfEdge1LeadsToInputOfEdge1(Edge edge1, Edge edge2)
        {
            return edge1.GetOutputs().Intersect(edge2.GetInputs()).Count() > 0;
        }
        public override string ToString()
        {
            return nameof(Cycle) + "(" + string.Join("->", this._Edges) + ")";
        }
    }
}
