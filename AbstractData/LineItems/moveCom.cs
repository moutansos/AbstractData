using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData.LineItems
{
    class moveCom : ILine
    {
        private string errorText;
        private string lineString;
        private int line;
        private string moveParams;

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
                parseString();
            }
        }

        public Type type
        {
            get { return typeof(moveCom); }
        }
        #endregion

        public void parseString()
        {
            moveParams = StringUtils.returnStringInside(lineString, '(', ')');
        }

        public void execute(adScript script)
        {
            List<dataRef> currentDataRefs = script.currentDataRefs;
            foreach(var dataRef in currentDataRefs)
            {

            }
        }

        public string generateString()
        {
            lineString = "move(" + moveParams + ")";
            return lineString;
        }

        public static bool isMoveCom(string line)
        {
            if (line.StartsWith("move"))
            {
                return true;
            }
            return false;
        }
    }
}
