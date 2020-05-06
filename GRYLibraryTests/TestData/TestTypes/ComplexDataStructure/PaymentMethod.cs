using GRYLibrary.TestData.TestTypes.ComplexDataStructure.PaymentMethods;
using System;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public abstract class PaymentMethod
    {
        internal static PaymentMethod GetRandom()
        {
            Random random = new Random();
            double randomValue = random.NextDouble();
            if (randomValue < 0.3)
            {
                return new DebitCard();
            }
            if (randomValue < 0.6)
            {
                return new CreditCard();
            }
            return new Bitcoin();
        }
    }
}