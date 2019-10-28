using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class Cycle
    {
        public IList<Edge> Edges { get; private set; } = new List<Edge>();
        public Cycle(IList<Edge> edges)
        {
            if (edges.Count == 0)
            {
                throw new System.Exception("A cycle can not be empty.");
            }
            if (new HashSet<Edge>(edges).Count < edges.Count)
            {
                throw new System.Exception("A cycle can not contain two equal edges.");
            }
            if (!edges.First().Source.Equals(edges.Last().Target))
            {
                throw new System.Exception("Edge-list does not represent a cycle.");
            }
            int indexOfLowestEdge = 0;
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] < edges[indexOfLowestEdge])
                {
                    indexOfLowestEdge = i;
                }
            }
            for (int i = 0; i < edges.Count; i++)
            {
                this.Edges.Add(edges[(i + indexOfLowestEdge) % edges.Count]);
            }
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

        private bool CheckOrder(IList<Edge> list1, IList<Edge> list2)
        {
            int indexOfFirstElementInFirstListInSecondList = list2.IndexOf(list1.First());
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[(i + indexOfFirstElementInFirstListInSecondList) % list1.Count]))
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
    }
}
