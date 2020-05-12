using System;

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
    }
}
