using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    internal class EquivalenceClass
    {
        public Guid Id { get; }
        private readonly object _ReferenceElement;
        public ISet<object> ContainedObjects { get; }
        public EquivalenceClass(object referenceElement)
        {
            this.Id = Guid.NewGuid();
            this._ReferenceElement = referenceElement;
            this.ContainedObjects = new HashSet<object>(GetComparer());
        }

        private IEqualityComparer<object> GetComparer()
        {
            throw new NotImplementedException();
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
            //todo special treatments for enumerables and complexobjects
            return _ReferenceElement.Equals(@object);
        }
    }
}
