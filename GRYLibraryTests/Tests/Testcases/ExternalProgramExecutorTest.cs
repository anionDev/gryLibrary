using GRYLibrary.Core;
using GRYLibrary.Core.OperatingSystem;
using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Semaphore = GRYLibrary.Core.Semaphore;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class ExternalProgramExecutorTest
    {
        [TestMethod]
        public void TestSimpleEcho()
        {
            string testStdOut = "test";
            ExternalProgramExecutor e = new ExternalProgramExecutor("echo", testStdOut);
            int result = e.StartSynchronously();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestAsyncExecution()
        {
            ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor(GetTimeoutTool(), 2.ToString());
            Semaphore semaphore = new Semaphore();
            semaphore.Increment();
            externalProgramExecutor.ExecutionFinishedEvent += (ExternalProgramExecutor sender, int exitCode) =>
            {
                Assert.AreEqual(0, exitCode);
                semaphore.Decrement();
            };
             externalProgramExecutor.StartAsynchronously();
            Assert.AreNotEqual(0, externalProgramExecutor.ProcessId);
            while (semaphore.Value != 0)
            {
                Thread.Sleep(200);
            }
        }
        public string GetTimeoutTool()
        {
            return OperatingSystem.GetCurrentOperatingSystem().Accept(GetTimeoutToolVisitor.Instance);
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
