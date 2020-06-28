using System;
using GRYLibrary.Core.AdvancedObjectAnalysis;


namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class MoneyAmount
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        internal static MoneyAmount GetRandom()
        {
            MoneyAmount result = new MoneyAmount();
            result.Amount = (decimal)new Random().NextDouble() * 500;
            result.Currency = "USD";
            return result;
        }
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }
        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }
        public override string ToString()
        {
            return Generic.GenericToString(this);
        }
        #endregion
    }
}
