﻿using System;
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

            adScript.Output output = null;
            dbref.parseString(new adScript(), ref output);

            //Check
            Assert.IsTrue(output == null || !output.isError);
            Assert.AreEqual(dbType.SQLServerDB, dbref.databaseType);
            Assert.AreEqual("sqlDb", dbref.referenceID);
            Assert.AreEqual("\"<<connectionString>>\"", dbref.referenceString);
        }

        /* TODO: write a gen db ref string method
        [TestMethod]
        public void dbRefStringGenerationTest1()
        {
            //Begin test
            ExcelFile excelFile = new ExcelFile(new reference("\"<<TestPath>>\""));
            dbRef dbref = new dbRef(excelFile, "excelFile");

            //Check
            Assert.AreEqual("ExcelFile excelFile = \"<<TestPath>>\"", dbref.generateString());
        }*/
    }
}
