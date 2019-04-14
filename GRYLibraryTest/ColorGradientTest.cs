using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibraryTest
{
    [TestClass]
    public class ColorGradientTest
    {
        [TestMethod]
        public void ColorGradientTest1()
        {
            var startColor = new ExtendedColor(255, 142, 0);
            var endColor = new ExtendedColor(100, 100, 100);
            var colorGradient = new ColorGradient(startColor, endColor);
            Assert.AreEqual(startColor, colorGradient.GetColorGradientValue(0));
            Assert.AreEqual(null, colorGradient.GetColorGradientValue(1));
            Assert.AreEqual(null, colorGradient.GetColorGradientValue(2));
            Assert.AreEqual(null, colorGradient.GetColorGradientValue(4.6));

        }
    }
}
