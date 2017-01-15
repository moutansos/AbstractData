using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class tableRef : ILine
    {
        private string errorText;
        private int line;
        private string lineString;

        private string readDb;
        private string writeDb;
        private string readTable;
        private string writeTable;

        private IDatabase readData;
        private IDatabase writeData;

        #region Constructors
        public tableRef(string originalString)
        {
            this.originalString = originalString;
        }

        public tableRef(string readDbName, string readTableName, string writeDbName, string writeTableName)
        {
            readDb = readDbName;
            readTable = readTableName;
            writeDb = writeDbName;
            writeTable = writeTableName;
        }
        #endregion

        #region Fields
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

        public Type type
        {
            get { return typeof(tableRef); }
        }

        public IDatabase readDatabase
        {
            get { return readData; }
        }

        public IDatabase writeDatabase
        {
            get { return writeData; }
        }
        #endregion

        public void parseString()
        {
            string innerTableRef = StringUtils.returnStringInside(lineString, '(', ')');
            string readText = string.Empty;
            string writeText = string.Empty;
            if (innerTableRef.Contains("=>"))
            {
                int indexOfSplit = innerTableRef.IndexOf("=>");
                int indexOfSplitEnd = indexOfSplit + 1;
                readText = innerTableRef.Substring(0, indexOfSplit);
                writeText = innerTableRef.Substring(indexOfSplitEnd, innerTableRef.Length - indexOfSplitEnd);
            }
            else if (innerTableRef.Contains("<="))
            {
                int indexOfSplit = innerTableRef.IndexOf("<=");
                int indexOfSplitEnd = indexOfSplit + 1;
                writeText = innerTableRef.Substring(0, indexOfSplit);
                readText = innerTableRef.Substring(indexOfSplitEnd, innerTableRef.Length - indexOfSplitEnd);
            }
            else
            {
                throw new ArgumentException("There is no directional operator in the tableRef");
            }

            if (readText.Contains('.'))
            {
                string[] readTextSplit = readText.Split('.');
                readDb = readTextSplit[0];
                readTable = readTextSplit[1];
            }
            else
            {
                readDb = readText;
            }

            if (writeText.Contains('.'))
            {
                string[] writeTextSplit = writeText.Split('.');
                writeDb = writeTextSplit[0];
                writeTable = writeTextSplit[1];
            }
            else
            {
                writeDb = writeText;
            }
        }

        public void execute(adScript script)
        {
            readData = script.getDatabase(readDb);
            //TODO: Validate readTable is in database
            writeData = script.getDatabase(writeDb);
            if(readData == null ||
               writeData == null)
            {
                throw new ArgumentException("Invalid database name. That database hasn't been initialied yet.");
            }

            script.currentTableRef = this;
        }

        public string generateString()
        {
            string readRef = readDb;
            string writeRef = writeDb;
            if(readTable != null)
            {
                readRef = readRef + "." + readTable;
            }
            if(writeTable != null)
            {
                writeRef = writeRef + "." + writeTable;
            }
            lineString = "tableRefernce(" + readRef + " => " + writeRef + ")";
            return originalString;
        }

        public static bool isTableRef(string line)
        {
            //TODO: Add RegEx validation
            if (line.StartsWith("tableReference"))
            {
                return true;
            }
            return false;
        }
    }
}
