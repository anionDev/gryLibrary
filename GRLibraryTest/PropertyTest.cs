using GRLibrary.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRLibraryTest
{
    [TestClass]
    public class PropertyTest
    {
        [TestMethod]
        public void SimplePropertyTests()
        {
            object object1 = new object();
            object object2 = new object();
            object object3 = new object();
            string name = "propertyName";
            Property<object> property = new Property<object>(object1, name, true);
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
            catch (InvalidArgumentException)
            {
                //expected
            }
            Assert.AreEqual(object2, property.Value);
            System.Collections.Generic.Stack<System.Collections.Generic.KeyValuePair<System.DateTime, object>> currentHistoy = property.History;
            Assert.AreEqual(2, currentHistoy.Count);
            Assert.AreEqual(object2, currentHistoy.Pop().Value);
            Assert.AreEqual(object1, currentHistoy.Pop().Value);
        }
    }
}
