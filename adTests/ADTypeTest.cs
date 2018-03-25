using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

namespace adTests
{
    /// <summary>
    /// Summary description for ADTypeTestx
    /// </summary>
    [TestClass]
    public class ADTypeTest
    {
        [TestMethod]
        public void TestADTypeParsing()
        {
            // Input type, Actual Type, is list
            Tuple<string, Type, bool>[] testTypes = new Tuple<string, Type, bool>[]
            {
                new Tuple<string, Type, bool>("String", typeof(string), false),
                new Tuple<string, Type, bool>("lIsT<sTriNG>", typeof(string), true),
                new Tuple<string, Type, bool>("int", typeof(int), false)
            };

            foreach(var testCase in testTypes)
            {
                ADType type = new ADType(testCase.Item1);
                type.Parse();
                Assert.AreEqual(testCase.Item2, type.NativeType);
                Assert.AreEqual(testCase.Item3, type.IsList);
            }
        }
    }
}
