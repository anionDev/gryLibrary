using GRYLibrary.Core.CryptoSystems;
using GRYLibrary.Core.CryptoSystems.ConcreteHashAlgorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GRYLibrary.Tests.Testcases.CryptoSystemsTests
{
    [TestClass]
    public class SHA256PureCSharpTests
    {
        [TestMethod]
        public void Test()
        {
            AssertSHA256ValueIsEqualsToDotNetImplementation(string.Empty);
            AssertSHA256ValueIsEqualsToDotNetImplementation("Franz jagt im komplett verwahrlosten Taxi quer durch Bayern");
            AssertSHA256ValueIsEqualsToDotNetImplementation("Simple ASCII input");
            AssertSHA256ValueIsEqualsToDotNetImplementation("Long input test test test test test test test test test test test test test test test test test test test test test test test test test test test test test");
            AssertSHA256ValueIsEqualsToDotNetImplementation("Specialcharacter-test: äöüß 您好 Здравствуйте नमस्कार");
        }

        private void AssertSHA256ValueIsEqualsToDotNetImplementation(string input)
        {
            // arrange
            byte[] inputAsByteArray = Core.Miscellaneous.Utilities.StringToByteArray(input);
            HashAlgorithm sha256PureCSharp = new SHA256PureCSharp();
            HashAlgorithm verificationAlgorithm = new SHA256();
            byte[] expectedResult = verificationAlgorithm.Hash(inputAsByteArray);

            // act
            byte[] actualResult = sha256PureCSharp.Hash(inputAsByteArray);

            // assert
            Assert.IsTrue(expectedResult.SequenceEqual(actualResult));
        }
    }
}
