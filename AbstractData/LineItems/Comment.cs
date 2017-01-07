using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class Comment
    {
        private string commentText;

        private string originalString;

        #region Constructors
        public Comment(string originalString)
        {
            this.originalString = originalString;
        }
        #endregion

        private void parseComment()
        {
            commentText = originalString.TrimStart('#');
        }

        public string generateComment()
        {
            originalString = "# " + commentText;
            return originalString;
        }

        #region Set Methods
        public void setCommentString(string commentString)
        {
            commentText = commentString;
        }
        #endregion

        #region Get Methods
        public string getCommentString()
        {
            return commentText;
        }
        #endregion

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
    }
}
