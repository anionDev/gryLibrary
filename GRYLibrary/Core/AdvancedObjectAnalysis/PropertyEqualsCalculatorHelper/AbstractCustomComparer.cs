using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public abstract class AbstractCustomComparer : GRYEqualityComparer<object>
    {      
        public abstract bool IsApplicable(Type type);
    }

}
