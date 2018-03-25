using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

namespace adTests
{
    [TestClass]
    public class dataRefTests
    {
        [TestMethod]
        public void dataRefParseTest1()
        {
            //Setup Variables
            string testOriginalString = "\"Test: \" + Column1 => B";

            //Setup Object
            dataRef dataRef = new dataRef(testOriginalString);

            adScript.Output output = null;
            dataRef.parseString(new adScript(), ref output);

            //Check Results
            Assert.IsTrue(output == null || !output.isError);
            Assert.AreEqual("\"Test: \" + Column1", dataRef.readReferenceText);
            Assert.AreEqual("B", dataRef.writeAssignmentText);
        }

        [TestMethod]
        public void dataRefParseTest2()
        {
            //Setup Variables
            string testOriginalString = "\"Test: \" + Column1 => B( List<String>   )";

            //Setup Object
            dataRef dataRef = new dataRef(testOriginalString);

            adScript.Output output = null;
            dataRef.parseString(new adScript(), ref output);

            //Check Results
            Assert.IsTrue(output == null || !output.isError);
            Assert.AreEqual("\"Test: \" + Column1", dataRef.readReferenceText);
            Assert.AreEqual("B", dataRef.writeAssignmentText);
            Assert.AreEqual(typeof(string), dataRef.writeAssignmentType.NativeType);
            Assert.IsTrue(dataRef.writeAssignmentType.IsList);
        }

        [TestMethod]
        public void dataRefGenTest1()
        {
            //Setup Object
            dataRef dataRef = new dataRef("A + B + C", "Field1");

            //Check Results
            Assert.AreEqual("A + B + C => Field1", dataRef.generateString());
        }
    }
}
