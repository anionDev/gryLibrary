using GRYLibrary.Core;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class PropertyTest
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void SimplePropertyTests()
        {
            object object1 = new object();
            object object2 = new object();
            string name = "propertyName";
            Core.Miscellaneous.Property<object> property = new Core.Miscellaneous.Property<object>(object1, name, true);
            Assert.AreEqual(1, property.History.Count);
            Assert.AreEqual(object1, property.History.Pop().Value);
            Assert.AreEqual(name, property.PropertyName);
            Assert.AreEqual(object1, property.Value);
            property.Value = object2;
            Assert.AreEqual(object2, property.Value);
            property.AllowNullAsValue = false;
            try
            {
                property.Value = null;
            }
            catch (ArgumentException)
            {
                //expected
            }
            Assert.AreEqual(object2, property.Value);
            System.Collections.Generic.Stack<System.Collections.Generic.KeyValuePair<DateTime, object>> currentHistoy = property.History;
            Assert.AreEqual(2, currentHistoy.Count);
            Assert.AreEqual(object2, currentHistoy.Pop().Value);
            Assert.AreEqual(object1, currentHistoy.Pop().Value);
        }
    }
}
