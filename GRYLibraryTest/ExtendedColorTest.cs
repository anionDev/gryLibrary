using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace GRYLibraryTest
{
    [TestClass]
   public class ExtendedColorTest
    {
        [TestMethod]
        public void ExtendedColorTest1()
        {
            var color = new ExtendedColor();
            Assert.AreEqual(0, color.ColorCode);
        }
        [TestMethod]
        public void ExtendedColorTest2()
        {
            var color1 = new ExtendedColor(42);
            var color2 = new ExtendedColor(42);
            Assert.AreEqual(color1, color2);
            Assert.AreEqual(color1.GetHashCode(), color2.GetHashCode());
        }
        [TestMethod]
        public void ExtendedColorTest3()
        {
            byte testValueA = 200;
            byte testValueR = 255;
            byte testValueG = 0;
            byte testValueB = 10;

            var color = new ExtendedColor(testValueA,testValueR,testValueG,testValueB);
            Assert.AreEqual(0, color.ColorCode);
            Assert.AreEqual(testValueA, color.A);
            Assert.AreEqual(testValueR, color.R);
            Assert.AreEqual(testValueG, color.G);
            Assert.AreEqual(testValueB, color.B);
            Assert.AreEqual(System.Drawing.Color.FromArgb(testValueA,testValueR,testValueG,testValueB), color.DrawingColor);
            Assert.AreEqual("", color.GetARGBString());
            Assert.AreEqual("", color.GetRGBString());
            Assert.AreEqual(Color.FromArgb(testValueA, testValueR, testValueG, testValueB), color.MediaColor);
            Assert.AreEqual("", color.ToString());
        }
    }
}
