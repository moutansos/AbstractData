using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class reference
    {
        //TODO: Add query functionality
        private string lineSubString;
        private List<string> fields;

        #region Constructors
        public reference(string refString)
        {
            fields = new List<string>();
            originalString = refString;
        }
        #endregion

        #region Properites
        public string originalString
        {
            get { return lineSubString; }
            set
            {
                lineSubString = value;
                parseString();
            }
        }

        public IEnumerable<string> refFields
        {
            get { return fields.Where(f => !(f.StartsWith("\"") && f.EndsWith("\""))); }
        }

        public IEnumerable<string> allFields
        {
            get { return fields; }
        }
        #endregion

        public void parseString()
        { //TODO: Implement with return all but inside method to avoid things within the strings.
            if (lineSubString.Contains("+"))
            {
                string[] splitItems = lineSubString.Split('+');
                for(int i = 0; i < splitItems.Length; i++)
                {
                    string item = splitItems[i];
                    item = item.Trim();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        fields.Add(item);
                    }
                }
            }
            else
            {
                fields.Add(lineSubString.Trim());
            }
        }

        public string evalReference(DataEntry dataIn, adScript script, ref adScript.Output output)
        {
            string dataVal = "";
            foreach (string field in allFields)
            {
                string fieldData = evalField(field, dataIn, script, ref output);
                if(fieldData != null)
                {
                    dataVal = dataVal + fieldData;
                }
                else if (output != null && output.isError)
                {
                    return null; //Break execution on error
                }
            }

            return dataVal;
        }

        public string evalField(string field, DataEntry dataIn, adScript script, ref adScript.Output output)
        {
            string dataVal = null;
            if (field.StartsWith("\"") && field.EndsWith("\"")) //This is a static value
            {
                dataVal = dataVal + field.TrimStart('\"').TrimEnd('\"');
            }
            else if (field.StartsWith("{") && field.EndsWith("}")) //This is a variable reference
            {
                string varID = field.TrimStart('{').TrimEnd('}');
                string globalResult = script.getGlobalVariable(varID).ToString();
                string localResult = script.getLocalVariable(varID).ToString();

                string varVal = "";

                if(localResult != null)
                {
                    varVal = localResult;
                }
                else if(globalResult != null)
                {
                    varVal = globalResult;
                }
                else
                {
                    output = new adScript.Output("The variable name " + varID + " is undefined.", true);
                    return null; //Break if error
                }

                dataVal = dataVal + evalField(varVal, dataIn, script, ref output);
            }
            else if (dataIn == null ||
                     dataIn.getField(field) == null) //Pass if there is no data found
            {
                return null;
            }
            else //The data is a valid field
            {
                dataVal = dataVal + dataIn.getField(field).data;
            }

            return dataVal;
        }
    }
}
