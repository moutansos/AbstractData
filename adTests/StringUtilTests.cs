using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

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
    }
}
