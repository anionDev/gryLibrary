using GRYLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class ExternalProgramExecutorTest
    {
        [TestMethod]
        public void TestSimpleEcho()
        {
            string testStdOut = "test";
            ExternalProgramExecutor e = ExternalProgramExecutor.Create("echo", testStdOut);
            int result = e.StartConsoleApplicationInCurrentConsoleWindow();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestSimpleEcho2()
        {
            string testStdOut = "test othertest";
            ExternalProgramExecutor e = ExternalProgramExecutor.Create("echo", testStdOut);
            int result = e.StartConsoleApplicationInCurrentConsoleWindow();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestSimpleEcho3_SomeSpecialCharacterAnd1DoubleQuote()
        {
            string testStdOut = "test /othertest\" c+\\_-";
            ExternalProgramExecutor e = ExternalProgramExecutor.Create("echo", testStdOut);
            int exitCode = e.StartConsoleApplicationInCurrentConsoleWindow();
            Assert.AreEqual(0, exitCode);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestSimpleEcho3_SomeSpecialCharacterAnd2DoubleQuote()
        {
            string testStdOut = "test /o\"thertest\" c+\\_-";
            ExternalProgramExecutor e = ExternalProgramExecutor.Create("echo", testStdOut);
            int result = e.StartConsoleApplicationInCurrentConsoleWindow();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }

    }
}
