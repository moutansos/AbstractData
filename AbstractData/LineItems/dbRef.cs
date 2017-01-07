using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class dbRef : ILine
    {
        private string errorText;
        private int line;
        private string lineString;

        private IDatabase db;
        private string refID;

        #region Constructor
        public dbRef()
        {

        }

        #endregion

        #region Properties
        public Type type
        {
            get { return typeof(dbRef); }
        }

        public bool hasError
        {
            get
            {
                if (errorText != null) return true;
                else return false;
            }
        }

        public int lineNumber
        {
            get { return line; }
            set
            {
                if (value > 0)
                {
                    line = value;
                }
                else
                {
                    line = 0;
                }

            }
        }

        public string originalString
        {
            get
            {
                if (lineString == null)
                {
                    generateString();
                }
                return lineString;
            }
            set
            {
                lineString = value;
                parseString();
            }
        }

        public string referenceID
        {
            get { return refID; }
            set { refID = value; }
        }
        #endregion

        public void execute(adScript script)
        {
            throw new NotImplementedException();
        }

        public string generateString()
        {
            if (db == null ||
               refID == null)
            {
                return null;
            }

            //TODO: Generate the string
            return "";
        }

        public void parseString()
        {
            string line = originalString;

        }
    }
}
