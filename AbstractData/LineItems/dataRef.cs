using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class dataRef : ILine
    {
        private string errorText;
        private int line;
        private string lineString;

        private tableRef tableRef;
        private reference readField;
        private string writeField; //TODO: Change to an assignment

        #region Constructors
        public dataRef(string originalString)
        {
            this.originalString = originalString;
        }

        public dataRef(string readRefText, string writeRefText)
        {
            readField = new reference(readRefText);
            writeField = writeRefText;
        }
        #endregion

        #region Properties
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
            }
        }

        public Type type
        {
            get { return typeof(dataRef); }
        }

        public reference readReference
        {
            get { return readField; }
        }

        public tableRef tableReference
        {
            get { return tableRef; }
        }

        public string readReferenceText
        {
            get { return readField.originalString; }
        }

        public string writeAssignment
        {
            get { return writeAssignment; }
        }

        public string writeAssignmentText
        {
            get { return writeField; }
        }
        #endregion

        public void parseString(ref string output)
        {
            if (lineString.Contains("=>")) //TODO: Move this into string utils and implement it here and in tableRef
            {
                int indexOfSplit = lineString.IndexOf("=>");
                int indexOfSplitEnd = indexOfSplit + 2;
                string readText = lineString.Substring(0, indexOfSplit).Trim();
                string writeText = lineString.Substring(indexOfSplitEnd, lineString.Length - indexOfSplitEnd).Trim();

                readField = new reference(readText);
                writeField = writeText;
            }
            else if (lineString.Contains("<="))
            {
                int indexOfSplit = lineString.IndexOf("<=");
                int indexOfSplitEnd = indexOfSplit + 2;
                string writeText = lineString.Substring(0, indexOfSplit).Trim();
                string readText = lineString.Substring(indexOfSplitEnd, lineString.Length - indexOfSplitEnd).Trim();

                readField = new reference(readText);
                writeField = writeText;
            }
            else
            {
                throw new ArgumentException("Error: Line was parsed as a dataRef but no reference operator was used.");
            }
        }

        public void execute(adScript script, ref string output)
        {
            //TODO: Add checks to see if there is any table referenced currently. Throw error if otherwise
            tableRef = script.currentTableRef; //Grab the table reference

            script.addDataRef(this);
        }

        public string generateString()
        {
            lineString = readField.originalString + " => " + writeField;
            return originalString;
        }

        public static bool isDataRef(string line)
        {
            //TODO: Add regex validation
            //TODO: Convert to an all but inside quotes and then check.
            if(line.Contains("<=") ||
               line.Contains("=>"))
            {
                return true;
            }
            return false;
        }

        public static List<string> getColumnsForRefs(IEnumerable<dataRef> dRefs)
        {
            //Find the needed columns
            List<string> readColumns = new List<string>();
            foreach (dataRef dRef in dRefs)
            {
                readColumns.AddRange(dRef.readReference.refFields);
            }
            return readColumns;
        }
    }
}
