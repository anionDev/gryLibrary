using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class CustomComparer
    {
        private readonly Func<Type, bool> _IsApplicable;
        private readonly Func<object, object, bool> _EqualsFunction;
        public CustomComparer(Func<Type, bool> isApplicable, Func<object, object, bool> equalsFunction)
        {
            this._IsApplicable = isApplicable;
            this._EqualsFunction = equalsFunction;
        }
        public bool IsApplicable(Type objectType)
        {
            return this._IsApplicable(objectType);
        }
        public bool ObjectsAreEqual(object object1, object object2)
        {
            return this._EqualsFunction(object1, object2);
        }
    }

}
