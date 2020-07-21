using System;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class PrimitiveComparer : AbstractCustomComparer
    {
        internal PrimitiveComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        internal override bool DefaultEquals(object item1, object item2)
        {
            bool result = item1.Equals(item2);
            return result;
        }

        internal override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return typeOfObject1.Equals(typeOfObject2) && TypeIsTreatedAsPrimitive(typeOfObject1);
        }
        public static bool TypeIsTreatedAsPrimitive(Type type)
        {
            return type.IsPrimitive || typeof(string).Equals(type);
        }
    }
}
