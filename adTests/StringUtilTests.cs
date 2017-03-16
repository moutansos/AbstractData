using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;
using System.Collections.Generic;

namespace adTests
{
    [TestClass]
    public class StringUtilTests
    {
        [TestMethod]
        public void StringReturnAllButInsideTest()
        {
            string cleanString = StringUtils.returnAllButInside("\"this \\\"is\\\"\", a test, \"to see\"", '\"', '\"');

            Assert.AreEqual("\"           \", a test, \"      \"", cleanString);
        }

        [TestMethod]
        public void splitOnAllButInsideTest()
        {
            string masterString = "test = \"This is a test 1\", test1 = \"This is a test 2\", test3 = \"This is a test 3\"";

            List<string> split = StringUtils.splitOnAllButInside(masterString, ',', '\"', '\"');

            Assert.AreEqual("test = \"This is a test 1\"", split[0]);
            Assert.AreEqual(" test1 = \"This is a test 2\"", split[1]);
            Assert.AreEqual(" test3 = \"This is a test 3\"", split[2]);
        }
    }
}
