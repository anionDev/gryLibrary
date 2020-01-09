﻿using GRYLibrary.Core.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Tests.GraphTests
{
    [TestClass]
    public class DepthFirstSearchTest
    {
        [TestMethod]
        public void TestSimpleSearch()
        {
            IList<Tuple<int, IList<Edge>>> order = new List<Tuple<int, IList<Edge>>>();
            Graph g = TestGraphs.GetTestGraphWithoutLoop();
            g.DepthFirstSearch((v, edges) =>
            {
                order.Add(new Tuple<int, IList<Edge>>(int.Parse(v.Name.Replace("v", string.Empty)), edges));
            });
            int i1 = GetIndexOfTupleWithSpeicficFirstvalue(order, 1);
            int i2 = GetIndexOfTupleWithSpeicficFirstvalue(order, 2);
            int i3 = GetIndexOfTupleWithSpeicficFirstvalue(order, 3);
            int i4 = GetIndexOfTupleWithSpeicficFirstvalue(order, 4);
            int i5 = GetIndexOfTupleWithSpeicficFirstvalue(order, 5);
            int i6 = GetIndexOfTupleWithSpeicficFirstvalue(order, 6);
            int i7 = GetIndexOfTupleWithSpeicficFirstvalue(order, 7);
            int i8 = GetIndexOfTupleWithSpeicficFirstvalue(order, 8);
            int i9 = GetIndexOfTupleWithSpeicficFirstvalue(order, 9);
            int i10 = GetIndexOfTupleWithSpeicficFirstvalue(order, 10);
            Assert.AreEqual(0, i1);
            Assert.IsTrue(i1 < i4);
            Assert.IsTrue(i4 < i8);
            Assert.IsTrue(i1 < i3);
            Assert.IsTrue(i3 < i7);
            Assert.IsTrue(i3 < i6);
            Assert.IsTrue(i6 < i10);
            Assert.IsTrue(i1 < i2);
            Assert.IsTrue(i2 < i5);
            Assert.IsTrue(i5 < i9);
        }

        private int GetIndexOfTupleWithSpeicficFirstvalue(IList<Tuple<int, IList<Edge>>> l, int index)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].Item1.Equals(index))
                {
                    return i;
                }
            }
            throw new KeyNotFoundException();
        }
    }
}