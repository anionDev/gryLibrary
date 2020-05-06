using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public static class Utilities
    {
        internal static string GetRandomPhoneNumber()
        {
            return Guid.NewGuid().ToString();
        }
        internal static DateTime GetRandomDateOfBirth()
        {
            Random random = new Random();
            DateTime start = new DateTime(1960, 1, 1);
            DateTime end = new DateTime(2010, 1, 1);
            return start.AddDays(random.Next((int)Math.Round((end - start).TotalDays)));
        }

        internal static string GetRandomWebsite()
        {
            return $"https://{GetRandomName()}.com";
        }

        internal static string GetRandomNotes()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomName()
        {
            return Guid.NewGuid().ToString();
        }
        internal static string GetRandomCountry()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomStateProvince()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomPostalCode()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomFloorLevel()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomCountryRegion()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomCity()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomBuilding()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomAddressLine2()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetRandomAddressLine1()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
