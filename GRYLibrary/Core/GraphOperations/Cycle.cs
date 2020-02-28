using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class Cycle
    {
        public IList<Edge> Edges { get; private set; } = new List<Edge>();
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
            this.Edges = edges;
        }
        public override bool Equals(object obj)
        {
            Cycle cycle = obj as Cycle;
            if (cycle == null)
            {
                return false;
            }
            int edgesCount = this.Edges.Count;
            if (this.Edges.Count != cycle.Edges.Count)
            {
                return false;
            }
            int indexOfStartItemInCycle2;
            if (cycle.Edges.Contains(this.Edges[0]))
            {
                indexOfStartItemInCycle2 = cycle.Edges.IndexOf(this.Edges[0]);
            }
            else
            {
                return false;
            }
            for (int indexOfStartItemInCycle1 = 0; indexOfStartItemInCycle1 < edgesCount; indexOfStartItemInCycle1++)
            {
                Edge edgeInCycle1 = this.Edges[indexOfStartItemInCycle1];
                Edge edgeInCycle2 = cycle.Edges[(indexOfStartItemInCycle2 + indexOfStartItemInCycle1) % edgesCount];
                if (!edgeInCycle1.Equals(edgeInCycle2))
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this.Edges.Count.GetHashCode();
        }

        public static bool RepresentsCycle(IList<Edge> edges)
        {
            if (edges.Count > 0)
            {
                if (new HashSet<Edge>(edges).Count != edges.Count)
                {
                    return false;
                }
                return CheckIfEdgesAreCyclic(edges);
            }
            else
            {
                return false;
            }
        }

        private static bool CheckIfEdgesAreCyclic(IList<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (i == 0)
                {
                    if (!edges[0].Source.Equals(edges[edges.Count - 1].Target))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!edges[i].Source.Equals(edges[i - 1].Target))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override string ToString()
        {
            return nameof(Cycle) + "(" + string.Join("->", this.Edges) + ")";
        }
    }
}
