using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

namespace adTests
{
    [TestClass]
    public class dbRefTests
    {
        [TestMethod]
        public void dbRefParseTest1()
        {
            string testOriginalString = "SQLServerDB sqlDb = \"<<connectionString>>\"";

            //Begin test
            dbRef dbref = new dbRef(testOriginalString);

            //Check
            Assert.AreEqual(dbType.SQLServerDB, dbref.databaseType);
            Assert.AreEqual("sqlDb", dbref.referenceID);
            Assert.AreEqual("\"<<connectionString>>\"", dbref.referenceString);
            Assert.AreEqual("<<connectionString>>", dbref.cleanReferenceString);
        }

        [TestMethod]
        public void dbRefStringGenerationTest1()
        {

            //Begin test
            ExcelFile excelFile = new ExcelFile("<<TestPath>>");
            dbRef dbref = new dbRef(excelFile, "excelFile");

            //Check
            Assert.AreEqual("ExcelFile excelFile = \"<<TestPath>>\"", dbref.generateString());
        }
    }
}
