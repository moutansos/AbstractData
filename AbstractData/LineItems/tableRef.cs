using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class tableRef : ILine
    {
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

        #region Properties
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

        public string readDbText
        {
            get { return readDb; }
        }

        public string readTableText
        {
            get { return readTable; }
        }

        public string writeDbText
        {
            get { return writeDb; }
        }

        public string writeTableText
        {
            get { return writeTable; }
        }
        #endregion

        public void parseString(adScript script, ref adScript.Output output)
        {
            string innerTableRef = StringUtils.returnStringInside(lineString, '(', ')');
            string readText = string.Empty;
            string writeText = string.Empty;
            if (innerTableRef.Contains("=>"))
            {
                int indexOfSplit = innerTableRef.IndexOf("=>");
                int indexOfSplitEnd = indexOfSplit + 2;
                readText = innerTableRef.Substring(0, indexOfSplit).Trim();
                writeText = innerTableRef.Substring(indexOfSplitEnd, innerTableRef.Length - indexOfSplitEnd).Trim();
            }
            else if (innerTableRef.Contains("<="))
            {
                int indexOfSplit = innerTableRef.IndexOf("<=");
                int indexOfSplitEnd = indexOfSplit + 2;
                writeText = innerTableRef.Substring(0, indexOfSplit).Trim();
                readText = innerTableRef.Substring(indexOfSplitEnd, innerTableRef.Length - indexOfSplitEnd).Trim();
            }
            else
            {
                output = new adScript.Output("There is no directional operator in the tableRef", true);
                if (line > 0)
                {
                    output.lineNumber = line;
                }
            }

            if (readText.Contains('>'))
            {
                string[] readTextSplit = readText.Split('>');
                readDb = readTextSplit[0].Trim();
                readTable = readTextSplit[1].Trim();
            }
            else
            {
                readDb = readText;
            }

            if (writeText.Contains('>'))
            {
                string[] writeTextSplit = writeText.Split('>');
                writeDb = writeTextSplit[0].Trim();
                writeTable = writeTextSplit[1].Trim();
            }
            else
            {
                writeDb = writeText;
            }

            //TODO: Add RegEx validation

            output = null;
        }

        public void execute(adScript script, ref adScript.Output output)
        {
            script.clearDataRefs();
            readData = script.getDatabase(readDb);
            //TODO: Validate readTable is in database
            writeData = script.getDatabase(writeDb);
            if(readData == null ||
               writeData == null)
            {
                output = new adScript.Output("Invalid database name. That database hasn't been initialied yet.", true);
                if (line > 0)
                {
                    output.lineNumber = line;
                }
            }

            script.currentTableRef = this;
        }

        public string generateString()
        {
            string readRef = readDb;
            string writeRef = writeDb;
            if(readTable != null)
            {
                readRef = readRef + ">" + readTable;
            }
            if(writeTable != null)
            {
                writeRef = writeRef + ">" + writeTable;
            }
            lineString = "tableReference(" + readRef + " => " + writeRef + ")";
            return originalString;
        }

        public static bool isTableRef(string line)
        {
            if (line.StartsWith("tableReference"))
            {
                return true;
            }
            return false;
        }
    }
}
