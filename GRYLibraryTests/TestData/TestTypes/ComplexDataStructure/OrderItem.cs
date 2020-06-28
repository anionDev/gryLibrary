using System;
using GRYLibrary.Core.AdvancedObjectAnalysis;


namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class OrderItem
    {
        public Product Product { get; set; }
        public MoneyAmount Price { get; set; }
        public string NotesFromCustomer { get; set; }
        public string UsedVoucherCode { get; set; }
        public Address DeliveryAddress { get; set; }

        internal static OrderItem GetRandom(Product product)
        {
            OrderItem result = new OrderItem();
            result.Product = product;
            result.Price = MoneyAmount.GetRandom();
            result.NotesFromCustomer = Utilities.GetRandomNotes();
            if (new Random().NextDouble() < 0)
            {
                result.UsedVoucherCode = Utilities.GetRandomName();
            }
            else
            {
                result.UsedVoucherCode = null;
            }
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
