using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class Comment : ILine
    {
        private string commentText;
        private string original;
        private int lineNum;

        #region Constructors
        public Comment(string originalString)
        {
            this.originalString = originalString;
        }
        #endregion

        #region Properties
        public Type type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int lineNumber
        {
            get { return lineNum; }
            set
            {
                if (value > 0)
                {
                    lineNum = value;
                }
                else
                {
                    lineNum = 0;
                }

            }
        }

        public string originalString
        {
            get
            {
                if (original == null)
                {
                    generateString();
                }
                return original;
            }
            set
            {
                original = value;
                parseString();
            }
        }

        public bool hasError
        {
            get
            {
                return false;
            }
        }
        
        #endregion

        private void parseComment()
        {
            commentText = original.TrimStart('#');
        }

        public string generateComment()
        {
            original = "# " + commentText;
            return original;
        }

        #region Static Utils
        public static bool isComment(string originalString)
        {
            if (originalString.StartsWith("#"))
            {
                return true;
            }
            return false;
        }
        #endregion

        public void execute(adScript script)
        {
            throw new NotImplementedException();
        }

        public void parseString()
        {
            throw new NotImplementedException();
        }

        public string generateString()
        {
            throw new NotImplementedException();
        }
    }
}
