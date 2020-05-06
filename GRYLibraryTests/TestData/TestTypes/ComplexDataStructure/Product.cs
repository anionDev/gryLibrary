using System;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public class Product
    {
        public string Name { get; set; }

        internal static Product GetRandom()
        {
            Product result = new Product();
            result.Name = GetRandomName();
            return result;
        }

        private static string GetRandomName()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
