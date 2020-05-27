using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    internal class EquivalenceClass
    {
        public Guid Id { get; }
        public ISet<object> ContainedObjects { get; }
        public EquivalenceClass()
        {
            this.Id = Guid.NewGuid();
            this.ContainedObjects = new HashSet<object>(new ReferenceEqualsComparer()/*ReferenceEqualsComparer is not correct here. reason: it does not work for objects which are treated as equal but are not the same (for example 2 HashSet-objects which contain the same elements).*/);
        }

        public override bool Equals(object obj)
        {
            return obj is EquivalenceClass @class &&
                   this.Id.Equals(@class.Id);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        public bool BelongsToThisEquivalenceClass(object @object)
        {
            bool result = ContainedObjects.Contains(@object);
            return result;
        }

    }
}
