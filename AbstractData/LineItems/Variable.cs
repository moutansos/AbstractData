using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class Variable : ILine
    {
        private string varID;
        private string varValue;
        private string typeID;
        private int line;

        private string lineString;

        #region Constructors
        public Variable(string originalString)
        {
            this.originalString = originalString;
        }

        public Variable(string varID, string value, string type)
        {
            this.id = varID;
            this.value = value;
            this.varType = type;
        }
        #endregion

        #region Properties
        public string id
        {
            get { return varID; }
            set { varID = value; }
        }

        public string value
        {
            get { return varValue; }
            set { varValue = value; }
        }

        public string varType
        {
            get { return typeID; }
            set
            {
                validateType(value);
                typeID = value;
            }
        }

        public Type type
        {
            get { return typeof(Variable); }
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
        #endregion

        #region Validation Methods
        public void validateType(string type)
        {
            if ((type != "Local") &&
                (type != "Global"))
            {
                throw new ArgumentException("The type being set to the variable is not valid. Must be a Local or Global type.");
            }
        }
        #endregion

        public void execute(adScript script, ref adScript.Output output)
        {
            if(varType == "Local")
            {
                script.setLocalVariable(this);
            }
            else if(varType == "Global")
            {
                script.setGlobalVariable(this);
            }
            else
            {
                output = new adScript.Output("Invalid variable type was set.", true);
                if (line > 0)
                {
                    output.lineNumber = line;
                }
            }
        }

        public void parseString(ref adScript.Output output)
        {
            int posOfFirstSpace = originalString.IndexOf(' ');
            varType = originalString.Substring(0, posOfFirstSpace);
            string varAndId = originalString.Substring(posOfFirstSpace, originalString.Length - posOfFirstSpace); //this too
            varID = varAndId.Split('=')[0].Trim();
            value = varAndId.Split('=')[1].Trim();
            varID = StringUtils.returnStringInside(varID, '{', '}');

            //TODO: Add RegEx Validation
            output = null;
        }

        public string generateString()
        {
            originalString = type + " {" + id + "} = " + value;
            return originalString;
        }

        public static bool isVar(string line)
        {
            if (line.StartsWith("Global") ||
                line.StartsWith("Local"))
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