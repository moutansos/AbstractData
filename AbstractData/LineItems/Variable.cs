using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class Variable
    {
        private string varID;
        private string varValue; //Switch to a reference
        private string typeID;

        private string originalString;

        #region Constructors
        public Variable(string originalString)
        {
            this.originalString = originalString;
        }

        public Variable(string varID, string value, string type)
        {
            this.id = varID;
            this.value = value;
            this.type = type;
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

        public string type
        {
            get { return typeID; }
            set
            {
                validateType(value);
                typeID = value;
            }
        }
        #endregion

        #region Validation Methods
        public void validateType(string type)
        {
            if ((type != "Local") ||
                (type != "Global"))
            {
                throw new ArgumentException("The type being set to the variable is not valid. Must be a Local or Global type.");
            }
        }
        #endregion

        private void parseAndSet()
        {
            int posOfFirstSpace = originalString.IndexOf(' ');
            type = originalString.Substring(0, posOfFirstSpace); //Double check this to see if space is taken care of
            string varAndId = originalString.Substring(posOfFirstSpace, originalString.Length); //this too
            varID = varAndId.Split('=')[0].Trim();
            value = varAndId.Split('=')[1].Trim();
        }

        private string generateVariableString()
        {
            originalString = type + " " + id + " = " + value;
            return originalString;
        }

        #region Static Utils
        /*
        public static bool isVar(adLine line)
        {
            string originalString = line.getOriginalString();
            //TODO: Add Regex Validation
            if (originalString.StartsWith("Global") ||
               originalString.StartsWith("Local"))
            {
                return true;
            }
            else
            {
                return false;
            }
        } */
        #endregion
    }
}
