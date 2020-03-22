using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.Tree
{
    public class Container<ContainedItemType> : TreeItem<ContainedItemType>
    {
        public List<TreeItem<ContainedItemType>> Children { get; set; }
        public Container()
        {
            this.Children = new List<TreeItem<ContainedItemType>>();
        }
        public IList<TreeItem<ContainedItemType>> GetDirectChildren()
        {
            return new List<TreeItem<ContainedItemType>>(this.Children);
        }
        public IEnumerable<TreeItem<ContainedItemType>> GetDirectAndTransitiveChildren()
        {
            List<TreeItem<ContainedItemType>> result = new List<TreeItem<ContainedItemType>>();
            result.AddRange(this.Children);
            foreach (var child in this.Children)
            {
                if (child is Container<ContainedItemType>)
                {
                    result.AddRange(((Container<ContainedItemType>)child).GetDirectAndTransitiveChildren());
                }
            }
            return result;
        }
        public override void Accept(ITreeItemVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ITreeItemVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
