using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class runCom : ILine
    {
        private string errorText;
        private int line;
        private string lineString;

        private string scriptFile;

        #region Constructors
        public runCom(string originalString)
        {
            this.originalString = originalString;
        }

        public runCom(FileInfo scriptFile)
        {
            this.scriptFile = scriptFile.FullName;
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
                parseString();
            }
        }

        public Type type
        {
            get {  return typeof(runCom); }
        }

        public string scriptFileName
        {
            set { scriptFile = value; }
            get { return scriptFile; }
        }

        public bool scriptExists
        {
            get
            {
                FileInfo file = new FileInfo(scriptFile);
                if (scriptFile != null && file.Exists)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        public void parseString()
        {
            //Eventually use this for returning the string when it is rewritten
            //scriptFile = StringUtils.returnStringInside(lineString, '\"', '\"');

            //For now use this
            scriptFile = StringUtils.returnStringInside(lineString, '(', ')').TrimStart('\"').TrimEnd('\"');
        }

        public void execute(adScript script)
        {
            adScript newScript = new adScript(scriptFile);
            newScript.runScript();
            GC.Collect();
        }

        public string generateString()
        {
            lineString = "run(\"" + scriptFile + "\")";
            return originalString;
        }

        public static bool isARunCom(string line)
        {
            //Add Regex Validation here
            if (line.StartsWith("run("))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
