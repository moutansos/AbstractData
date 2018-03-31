using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class tableRef : ILine, IEquatable<tableRef>
    {
        const char TABLE_SEPERATOR = '>';

        private int line;
        private string lineString;

        private string readDb;
        private string writeDb;
        private string readTable;

        #region Constructors
        public tableRef(string originalString)
        {
            this.originalString = originalString;
        }

        public tableRef(string readDbName, string readTableName, string writeDbName, string writeTableName)
        {
            ReadDb = readDbName;
            ReadTable = readTableName;
            WriteDb = writeDbName;
            writeTableText = writeTableName;
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

        public IDatabase ReadDatabase { get; private set; }

        public IDatabase WriteDatabase { get; private set; }

        public string readDbText
        {
            get { return ReadDb; }
        }

        public string readTableText
        {
            get { return ReadTable; }
        }

        public string writeDbText
        {
            get { return WriteDb; }
        }

        public string writeTableText { get; private set; }

        public string tableRefKey => ReadDb + TABLE_SEPERATOR + ReadTable + " => " + WriteDb + TABLE_SEPERATOR + writeTableText;

        public string ReadDb { get => readDb; set => readDb = value; }
        public string WriteDb { get => writeDb; set => writeDb = value; }
        public string ReadTable { get => readTable; set => readTable = value; }
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
                ReadDb = readTextSplit[0].Trim();
                ReadTable = readTextSplit[1].Trim();
            }
            else
            {
                ReadDb = readText;
            }

            if (writeText.Contains('>'))
            {
                string[] writeTextSplit = writeText.Split('>');
                WriteDb = writeTextSplit[0].Trim();
                writeTableText = writeTextSplit[1].Trim();
            }
            else
            {
                WriteDb = writeText;
            }

            //TODO: Add RegEx validation

            output = null;
        }

        public void execute(adScript script, ref adScript.Output output)
        {
            script.ClearDataRefs();
            ReadDatabase = script.getDatabase(ReadDb);
            //TODO: Validate readTable is in database
            WriteDatabase = script.getDatabase(WriteDb);
            if(ReadDatabase == null ||
               WriteDatabase == null)
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
            string readRef = ReadDb;
            string writeRef = WriteDb;
            if(ReadTable != null)
            {
                readRef = readRef + ">" + ReadTable;
            }
            if(writeTableText != null)
            {
                writeRef = writeRef + ">" + writeTableText;
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

        public override int GetHashCode()
        {
            return tableRefKey.GetHashCode();
        }

        public bool Equals(tableRef other)
        {
            return tableRefKey == other.tableRefKey;
        }
    }
}
