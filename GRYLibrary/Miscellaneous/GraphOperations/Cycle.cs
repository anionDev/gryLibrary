using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Miscellaneous.GraphOperations
{
    public class Cycle
    {
        public IList<Edge> Edges { get; private set; } = new List<Edge>();
        public override bool Equals(object obj)
        {
            Cycle cycle = obj as Cycle;
            if (cycle == null)
            {
                return false;
            }
            if (!this.Edges.Count.Equals(cycle.Edges.Count))
            {
                return false;
            }
            if (this.Edges.Count == 0)
            {
                return true;
            }
            if (!cycle.Edges.Contains(this.Edges.First()))
            {
                return false;
            }
            return this.CheckOrder(this.Edges, cycle.Edges) || this.CheckOrder(this.Edges, cycle.Edges.Reverse().ToList());
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
