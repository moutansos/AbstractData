using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractData;

namespace adTests
{
    [TestClass]
    public class ObjectEntryTests
    {

        [TestMethod]
        public void ObjectEntryTest1()
        {
            string tv1 = "testVal1";
            string tv2 = "testVal2";
            string tv3 = "testVal3";

            string[] path1 = new string[] { "test", "value", "one" };
            string[] path2 = new string[] { "test", "value", "two" };
            string[] path3 = new string[] { "test", "value3" };

            ContainerObject cont = new ContainerObject();
            cont[path1] = tv1;
            cont[path2] = tv2;
            cont[path3] = tv3;

            Assert.AreEqual(tv1, cont[path1]);
            Assert.AreEqual(tv2, cont[path2]);
            Assert.AreEqual(tv3, cont[path3]);
        }
    }
}
