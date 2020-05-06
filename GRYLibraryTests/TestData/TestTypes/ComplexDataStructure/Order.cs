using GRYLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class Order
    {
        public ISet<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public PaymentMethod PaymentMethod { get; set; }
        public ICustomer Buyer { get; set; }
        public Address BillingAddress { get; set; }

        internal static Order GetRandom(ISet<Product> products)
        {
            Order result = new Order();
            Random random = new Random();
            foreach (Product product in products)
            {
                Enumerable.Range(0, random.Next(4)).ForEach(index => result.OrderItems.Add(OrderItem.GetRandom(product)));
            }
            result.PaymentMethod = PaymentMethod.GetRandom();
            if (random.NextDouble() < 0.5)
            {
                result.Buyer = PrivateCustomer.GetRandom();
            }
            else
            {
                result.Buyer = BusinessCustomer.GetRandom();
            }
            return result;
        }
    }
}
