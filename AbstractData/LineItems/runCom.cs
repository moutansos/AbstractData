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

        public void parseString(adScript script, ref adScript.Output output)
        {
            //Eventually use this for returning the string when it is rewritten
            //scriptFile = StringUtils.returnStringInside(lineString, '\"', '\"');

            //For now use this
            scriptFile = StringUtils.returnStringInside(lineString, '(', ')').TrimStart('\"').TrimEnd('\"');

            //TODO: Add Regex Validation here
            output = null;
        }

        public void execute(adScript script, ref adScript.Output output)
        {
            //Perform Checks
            FileInfo info = new FileInfo(scriptFile);
            if (!info.Exists)
            {
                scriptFile = null;
                output = new adScript.Output("The specified file does not exist.", true);
                if (line > 0)
                {
                    output.lineNumber = line;
                }
            }

            if (scriptFile != null)
            {
                adScript newScript = new adScript(scriptFile);
                newScript.output = script.output;

                //TODO: Add try statement to this to catch the potential IO errors reading the stream.
                newScript.runScript();
                GC.Collect();
            }
        }

        public string generateString()
        {
            lineString = "run(\"" + scriptFile + "\")";
            return originalString;
        }

        public static bool isARunCom(string line)
        {
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
