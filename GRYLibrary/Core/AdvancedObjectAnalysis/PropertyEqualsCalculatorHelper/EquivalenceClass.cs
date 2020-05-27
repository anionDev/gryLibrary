using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    internal class EquivalenceClass
    {
        public Guid Id { get; }
        public object ReferenceItem { get; }
        public ISet<object> ContainedObjects { get { return new HashSet<object>(this._ContainedObjects, new ReferenceEqualsComparer()); } }
        private readonly ISet<object> _ContainedObjects;
        public EquivalenceClass(object @object)
        {
            this.Id = Guid.NewGuid();
            this.ReferenceItem = @object;
            this._ContainedObjects = new HashSet<object>(new ReferenceEqualsComparer());
            this._ContainedObjects.Add(this.ReferenceItem);
        }
        public override bool Equals(object obj)
        {
            return obj is EquivalenceClass @class &&
                   this.Id.Equals(@class.Id);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }

        public bool AlreadyBelongsToThisEquivalenceClass(object @object)
        {
            bool result = this.ContainedObjects.Contains(@object);
            //TODO must also return true if @object equals ReferenceItem. problem here: how to calculate if this 2 objects are equal at this prosition?
            return result;
        }

        internal void Add(object @object)
        {
            this._ContainedObjects.Add(@object);
        }
    }
}
