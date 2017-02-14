using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbstractData;

namespace adTests
{
    [TestClass]
    public class CommentTests
    {
        [TestMethod]
        public void CommentParseTest1()
        {
            //Setup Variables
            string testOriginalString = "# <<TestText>>";

            //Setup Object
            Comment comment = new Comment(testOriginalString);
            string output = null;
            comment.parseString(ref output);

            //Check Results
            Assert.AreEqual(" <<TestText>>", comment.commentTextString);
        }

        [TestMethod]
        public void CommentStringGeneration1()
        {
            //Setup Variables
            string commentTest = " <<TestText>>";

            //Setup Object
            Comment comment = new Comment(commentTest, true);

            //Check Results
            Assert.AreEqual("# <<TestText>>", comment.generateComment());
        }
    }
}
