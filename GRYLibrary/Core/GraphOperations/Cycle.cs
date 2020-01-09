using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.GraphOperations
{
    [Serializable]
    public class Cycle
    {
        public IList<Edge> Edges { get; private set; } = new List<Edge>();
        public Cycle(IList<Edge> edges)
        {
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
            return this.Edges.SequenceEqual(cycle.Edges);
        }
        public override int GetHashCode()
        {
            return this.Edges.Count.GetHashCode();
        }

        public static bool RepresentsCycle(IList<Edge> edges)
        {
            if (edges.Count > 0)
            {
                List<Edge> reversedList = edges.ToList();
                reversedList.Reverse();
                return CheckIfEdgesAreCyclic(edges) || CheckIfEdgesAreCyclic(reversedList);
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
                    try
                    {
  if (!edges[i].Source.Equals(edges[i - 1].Target))
                    {
                        return false;
                    }
                    }
                    catch
                    {

                    }
                  
                }
            }
            return true;
        }
    }
}
