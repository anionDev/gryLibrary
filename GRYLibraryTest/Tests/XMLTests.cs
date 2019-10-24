using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace GRYLibraryTest.Tests
{
    [TestClass]
    public class XMLTests
    {
        private static readonly Encoding _FileEncoding = new UTF8Encoding(false);
        private static readonly string _TestDataFolder = Path.Combine("TestData", "XMLValidator", "Test1");
        private static readonly string _TestXSD1File = Path.Combine(_TestDataFolder, "TestXSD1.xsd");
        private static readonly string _TestXML1_MatchsXSD1 = Path.Combine(_TestDataFolder, "TestXML1_MatchsXSD1.xml");
        private static readonly string _TestXML2_MatchsXSD1Not = Path.Combine(_TestDataFolder, "TestXML2_MatchsXSD1Not.xml");
        private static readonly string _TestTargetNamespace = "mynamespace";
        [TestMethod]
        public void TestXSDValidator_Test1()
        {
            string xmlWorking = File.ReadAllText(_TestXML1_MatchsXSD1, _FileEncoding);
            string xmlNotWorking = File.ReadAllText(_TestXML2_MatchsXSD1Not, _FileEncoding);
            string xsd = File.ReadAllText(_TestXSD1File, _FileEncoding);
            Assert.IsTrue(GRYLibrary.Utilities.ValidateXMLAgainstXSD(xmlWorking, xsd, _TestTargetNamespace, out _));
            Assert.IsFalse(GRYLibrary.Utilities.ValidateXMLAgainstXSD(xmlNotWorking, xsd, _TestTargetNamespace, out _));
        }
        [TestMethod]
        public void TestXSLTValidator_Test1()
        {
            throw new System.NotImplementedException();
        }
    }
}