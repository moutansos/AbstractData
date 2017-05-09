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

            adScript.Output output = null;
            var.parseString(new adScript(), ref output);

            //Check
            Assert.IsTrue(output == null || !output.isError);
            Assert.AreEqual("staticVar", var.id);
            Assert.AreEqual("\"Static Variable\"", var.value);
            Assert.AreEqual("Global", var.varType);
        }
    }
}
