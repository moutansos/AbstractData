﻿using System;
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
        public Comment(string inputString)
        {
            this.originalString = inputString;
        }

        public Comment(string commentText, bool isNotOriginal)
        {
            this.commentText = commentText;
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
            }
        }
        
        public string commentTextString
        {
            get { return commentText; }
        }
        #endregion

        public string generateComment()
        {
            original = "#" + commentText;
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

        public void execute(adScript script, ref adScript.Output output)
        {
            output = null;
        }

        public void parseString(adScript script, ref adScript.Output output)
        {
            commentText = original.TrimStart('#');
            output = null;
        }

        public string generateString()
        {
            originalString = "#" + commentText;
            return originalString;
        }
    }
}
