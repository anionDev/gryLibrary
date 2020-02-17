using GRYLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class XMLTests
    {
        private readonly Encoding _FileEncoding = new UTF8Encoding(false);
        [TestMethod]
        public void TestXSDValidator_Test1()
        {
            string testDataFolder = Path.Combine("TestData", "XSDValidator", "Test1");
            string testXSD1File = Path.Combine(testDataFolder, "TestXSD1.xsd");
            string testXML1_MatchsXSD1 = Path.Combine(testDataFolder, "TestXML1_MatchsXSD1.xml");
            string testXML2_MatchsXSD1Not = Path.Combine(testDataFolder, "TestXML2_MatchsXSD1Not.xml");

            string xmlWorking = File.ReadAllText(testXML1_MatchsXSD1, this._FileEncoding);
            string xmlNotWorking = File.ReadAllText(testXML2_MatchsXSD1Not, this._FileEncoding);
            string xsd = File.ReadAllText(testXSD1File, this._FileEncoding);
            Assert.IsTrue(Utilities.ValidateXMLAgainstXSD(xmlWorking, xsd, out _));
            Assert.IsFalse(Utilities.ValidateXMLAgainstXSD(xmlNotWorking, xsd, out _));
        }
        [TestMethod]
        public void TestXSLTValidator_Test1()
        {
            string testDataFolder = Path.Combine("TestData", "XSLTTransformator", "Test1");
            string testSource = Path.Combine(testDataFolder, "TestTransformationSource.xml");
            string testTarget = Path.Combine(testDataFolder, "TestTransformationTarget.xml");
            string testXSLT = Path.Combine(testDataFolder, "TestXSLT1.xslt");
            string xmlSource = File.ReadAllText(testSource, this._FileEncoding);
            string xmlTarget = File.ReadAllText(testTarget, this._FileEncoding);
            string xslt = File.ReadAllText(testXSLT, this._FileEncoding);
            Assert.AreEqual(xmlTarget, Utilities.ApplyXSLTToXML(xmlSource, xslt));
        }
    }
}