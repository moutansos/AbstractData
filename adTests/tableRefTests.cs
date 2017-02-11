using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;


namespace adTests
{
    [TestClass]
    public class tableRefTests
    {
        [TestMethod]
        public void tableRefParseTest1()
        {
            string testOriginalString = "tableReference(sqlDb>Table1 => excelFile>Sheet1)";

            //Begin test
            tableRef tableRef = new tableRef(testOriginalString);

            //Check
            Assert.AreEqual("sqlDb", tableRef.readDbText);
            Assert.AreEqual("Table1", tableRef.readTableText);
            Assert.AreEqual("excelFile", tableRef.writeDbText);
            Assert.AreEqual("Sheet1", tableRef.writeTableText);
        }

        [TestMethod]
        public void tableRefGenTest1()
        {
            //Initialize Object
            tableRef tableRef = new tableRef("sqlDb", "Table1", "excelFile", "Sheet1");

            //Check
            string tableRefTest = tableRef.generateString();
            Assert.AreEqual("tableReference(sqlDb>Table1 => excelFile>Sheet1)", tableRef.generateString());
        }
    }
}
