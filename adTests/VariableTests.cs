using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

namespace adTests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void variableParseTest1()
        {
            string testOriginalString = "Global {staticVar} = \"Static Variable\"";

            //Begin test
            Variable var = new Variable(testOriginalString);

            //Check
            Assert.AreEqual("staticVar", var.id);
            Assert.AreEqual("\"Static Variable\"", var.value);
            Assert.AreEqual("Global", var.varType);
        }
    }
}
