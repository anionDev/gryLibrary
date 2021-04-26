using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.CryptoSystems.ConcreteHashAlgorithms;
using GRYLibrary.Core.OperatingSystem;
using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GRYLibrary.Tests.Utilities
{
    public static class TestUtilities
    {
        internal static void AssertEqual(object expectedObject, object actualObject, bool addDefaultEqualAssertion = true)
        {
            bool expectedObjectIsNull = expectedObject == null;
            bool actualObjectIsNull = actualObject == null;
            if (expectedObjectIsNull && actualObjectIsNull)
            {
                Core.Miscellaneous.Utilities.NoOperation();
            }
            if (expectedObjectIsNull && !actualObjectIsNull)
            {
                Assert.Fail("actual object is not null");
            }
            if (!expectedObjectIsNull && actualObjectIsNull)
            {
                Assert.Fail("actual object is null");
            }
            if (!expectedObjectIsNull && !actualObjectIsNull)
            {
                Assert.IsTrue(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                Assert.AreEqual(Generic.GenericGetHashCode(expectedObject), Generic.GenericGetHashCode(actualObject));

                if (addDefaultEqualAssertion)
                {
                    if (Core.Miscellaneous.EnumerableTools.ObjectIsSet(expectedObject))
                    {
                        Assert.IsTrue(Core.Miscellaneous.EnumerableTools.ObjectToSet<object>(expectedObject).SetEquals(Core.Miscellaneous.EnumerableTools.ObjectToSet<object>(actualObject)), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                    }
                    else if (Core.Miscellaneous.EnumerableTools.ObjectIsList(expectedObject))
                    {
                        Assert.IsTrue(Core.Miscellaneous.EnumerableTools.ObjectToList<object>(expectedObject).SequenceEqual(Core.Miscellaneous.EnumerableTools.ObjectToList<object>(actualObject)), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                    }
                    else if (!Core.Miscellaneous.EnumerableTools.ObjectIsEnumerable(expectedObject))
                    {
                        Assert.AreEqual(expectedObject, actualObject, Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                        Assert.AreEqual(expectedObject.GetHashCode(), actualObject.GetHashCode());
                    }
                }
            }
        }
        internal static void AssertNotEqual(object expectedObject, object actualObject)
        {
            bool expectedObjectIsNull = expectedObject == null;
            bool actualObjectIsNull = actualObject == null;
            if (expectedObjectIsNull && actualObjectIsNull)
            {
                Assert.Fail("Both objects are equal");
            }
            if (expectedObjectIsNull && !actualObjectIsNull)
            {
                Core.Miscellaneous.Utilities.NoOperation();
            }
            if (!expectedObjectIsNull && actualObjectIsNull)
            {
                Core.Miscellaneous.Utilities.NoOperation();
            }
            if (!expectedObjectIsNull && !actualObjectIsNull)
            {
                Assert.IsFalse(Generic.GenericEquals(expectedObject, actualObject), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                Assert.AreNotEqual(expectedObject, actualObject, Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
            }
        }
        internal static void AssertPureSHA256ValueIsEqualsToDotNetImplementation(string input)
        {
            AssertSHA256ValueIsEqualsToDotNetImplementation(new SHA256PureCSharp(), input);
        }
        internal static void AssertSHA256ValueIsEqualsToDotNetImplementation(Core.CryptoSystems.HashAlgorithm algorithmUnderTest, string input)
        {
            AssertHashValueIsEqualsToDotNetImplementation(algorithmUnderTest, new Core.CryptoSystems.ConcreteHashAlgorithms.SHA256(), input);
        }
        internal static void AssertHashValueIsEqualsToDotNetImplementation(Core.CryptoSystems.HashAlgorithm algorithmUnderTest, Core.CryptoSystems.HashAlgorithm verificationAlgorithm, string input)
        {
            // arrange
            byte[] inputAsByteArray = Core.Miscellaneous.Utilities.StringToByteArray(input);
            byte[] expectedResult = verificationAlgorithm.Hash(inputAsByteArray);

            // act
            byte[] actualResult = algorithmUnderTest.Hash(inputAsByteArray);

            // assert
            Assert.IsTrue(expectedResult.SequenceEqual(actualResult));
        }
        public static string GetTimeoutTool()
        {
            return Core.OperatingSystem.OperatingSystem.GetCurrentOperatingSystem().Accept(GetTimeoutToolVisitor.Instance);
        }
        private class GetTimeoutToolVisitor : IOperatingSystemVisitor<string>
        {
            public static IOperatingSystemVisitor<string> Instance { get; set; } = new GetTimeoutToolVisitor();

            public string Handle(OSX operatingSystem)
            {
                return "sleep";
            }

            public string Handle(Windows operatingSystem)
            {
                return "timeout";
            }

            public string Handle(Linux operatingSystem)
            {
                return "sleep";
            }
        }
    }
}
