using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GRYLibraryTest
{
    [TestClass]
    public class ConcurrentTest
    {
        [TestMethod]
        public void TestConcurrentFunctions1()
        {
            ISet<Tuple<int, int>> testFunctionDetails = new HashSet<Tuple<int, int>>();
            int resultValueOfFirstResult = 42;
            testFunctionDetails.Add(new Tuple<int, int>(7, resultValueOfFirstResult + 1));
            testFunctionDetails.Add(new Tuple<int, int>(1, resultValueOfFirstResult));
            testFunctionDetails.Add(new Tuple<int, int>(2, resultValueOfFirstResult - 1));
            ISet<Func<int>> input = GetTestFunction(testFunctionDetails);
            int result = Utilities.RunAllConcurrentAndReturnFirstResult(input);
            Assert.AreEqual(resultValueOfFirstResult, result);
        }

        private ISet<Func<T>> GetTestFunction<T>(ISet<Tuple<int, T>> functionDetails)
        {
            ISet<Func<T>> result = new HashSet<Func<T>>();
            foreach (Tuple<int, T> item in functionDetails)
            {
                System.Threading.Thread.Sleep(item.Item1 * 1000);
                result.Add(() => { return item.Item2; });
            }
            return result;
        }
    }
}