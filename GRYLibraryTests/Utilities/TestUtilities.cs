using GRYLibrary.Core.AdvancedObjectAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GRYLibrary.Tests.Utilities
{
    public static class TestUtilities
    {
        public static void AssertEqual(object expectedObject, object actualObject, bool addDefaultEqualAssertion = true)
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
                    if (Core.Miscellaneous.Utilities.ObjectIsSet(expectedObject))
                    {
                        Assert.IsTrue(Core.Miscellaneous.Utilities.ObjectToSet<object>(expectedObject).SetEquals(Core.Miscellaneous.Utilities.ObjectToSet<object>(actualObject)), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                    }
                    else if (Core.Miscellaneous.Utilities.ObjectIsList(expectedObject))
                    {
                        Assert.IsTrue(Core.Miscellaneous.Utilities.ObjectToList<object>(expectedObject).SequenceEqual(Core.Miscellaneous.Utilities.ObjectToList<object>(actualObject)), Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                    }
                    else if (!Core.Miscellaneous.Utilities.ObjectIsEnumerable(expectedObject))
                    {
                        Assert.AreEqual(expectedObject, actualObject, Core.Miscellaneous.Utilities.GetAssertionFailMessage(expectedObject, actualObject));
                        Assert.AreEqual(expectedObject.GetHashCode(), actualObject.GetHashCode());
                    }
                }
            }
        }
        public static void AssertNotEqual(object expectedObject, object actualObject)
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
    }
}
