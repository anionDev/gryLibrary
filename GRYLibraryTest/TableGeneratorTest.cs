using GRYLibrary.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static GRYLibrary.Miscellaneous.TableGenerator;

namespace GRYLibraryTest
{
    [TestClass]
    public class TableGeneratorTest
    {
        [TestMethod]
        public void TestSimpleTable1()
        {
            string[,] items = new string[3, 4];
            items[0, 0] = "firstline0";
            items[0, 1] = "firstline1_012";
            items[0, 2] = "firstline2";
            items[0, 3] = "firstline3";
            items[1, 0] = "secondline0";
            items[1, 1] = "secondline1";
            items[1, 2] = "secondline2_01";
            items[1, 3] = "secondline3";
            items[2, 0] = "lastline0";
            items[2, 1] = "lastline1";
            items[2, 2] = "lastline2";
            items[2, 3] = "lastline3_012345";
            string title = "sample";
            string[] table = TableGenerator.Generate(items, title, true, new TableGenerator.ASCIITable(), 100);
            Assert.AreEqual(7, table.Length);
            Assert.AreEqual(title, table[0]);
            Assert.AreEqual("┌───────────┬──────────────┬──────────────┬────────────────┐", table[1]);
            Assert.AreEqual("│firstline0 │firstline1_012│firstline2    │firstline3      │", table[2]);
            Assert.AreEqual("├───────────┼──────────────┼──────────────┼────────────────┤", table[3]);
            Assert.AreEqual("│secondline0│secondline1   │secondline2_01│secondline3     │", table[4]);
            Assert.AreEqual("│lastline0  │lastline1     │lastline2     │lastline3_012345│", table[5]);
            Assert.AreEqual("└───────────┴──────────────┴──────────────┴────────────────┘", table[6]);
        }
        [TestMethod]
        public void TestSimpleTable2()
        {
            string[,] items = new string[3, 4];
            items[0, 0] = "firstline0";
            items[0, 1] = "firstline1_012";
            items[0, 2] = "firstline2";
            items[0, 3] = "firstline3";
            items[1, 0] = "secondline";
            items[1, 1] = "secondline1";
            items[1, 2] = "secondline2_01";
            items[1, 3] = "secondline3";
            items[2, 0] = "lastline0";
            items[2, 1] = "lastline1";
            items[2, 2] = "lastline2";
            items[2, 3] = "lastline3_012345";
            string title = "sample";
            string[] table = TableGenerator.Generate(items, title, false, new TableGenerator.ASCIITable() { Characters = new DoubleLineTableCharacter()}, 11);
            Assert.AreEqual(6, table.Length);
            Assert.AreEqual(title, table[0]);
            Assert.AreEqual("╔══════════╦═══════════╦═══════════╦═══════════╗", table[1]);
            Assert.AreEqual("║firstline0║firstlin...║firstline2 ║firstline3 ║", table[2]);
            Assert.AreEqual("║secondline║secondline1║secondli...║secondline3║", table[3]);
            Assert.AreEqual("║lastline0 ║lastline1  ║lastline2  ║lastline...║", table[4]);
            Assert.AreEqual("╚══════════╩═══════════╩═══════════╩═══════════╝", table[5]);
        }
    }
}
