using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class adScript
    {
        System.IO.StreamReader dataStream;
        //TODO: Setup an output stream for sending ouptut data from the scripts
        //TODO: Add a script command for printing to the output/log

        private List<Variable> globalVariablesList;
        private List<Variable> localVariablesList;

        #region Constructors
        public adScript(string fileName)
        {
            dataStream = new System.IO.StreamReader(fileName);
        }

        public adScript(System.IO.StreamReader reader)
        {
            dataStream = reader;
        }
        #endregion

        public void runScript()
        {
            #region Data Checks
            //Check for null values
            if (dataStream == null)
            {
                throw new NullReferenceException("There was no data stream or file specified.");
            }

            //Reset storage structures
            globalVariablesList = new List<Variable>();
            localVariablesList = new List<Variable>();
            #endregion

            int lineCounter = 1;

            string line;
            List<ILine> scriptLines = new List<ILine>();
            while ((line = dataStream.ReadLine()) != null) //Parse in file and set up structures
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    ILine lineObject = null;
                    //TODO: Add type detection here

                    if(lineObject != null)
                    {
                        scriptLines.Add(lineObject);
                    }
                }
                lineCounter++;
            }


        }

        //TODO: add all variable parsing logic to this class
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

        public class dbRef : ILine
        {
            private string errorText;
            private int line;
            private string lineString;

            #region Constructor
            public dbRef()
            {

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
                    if(value > 0)
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
                    if(lineString == null)
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
            #endregion

            public void execute(adScript script)
            {
                throw new NotImplementedException();
            }

            public string generateString()
            {
                throw new NotImplementedException();
            }

            public void parseString()
            {
                //TODO: Add parsing logic
            }
        }

        public class comment
        {
            private string commentText;

            private string originalString;

            #region Constructors
            public comment(string originalString)
            {
                this.originalString = originalString;
            }
            #endregion

            private void parseComment()
            {
                commentText = originalString.TrimStart('#');
            }

            public string generateComment()
            {
                originalString = "# " + commentText;
                return originalString;
            }

            #region Set Methods
            public void setCommentString(string commentString)
            {
                commentText = commentString;
            }
            #endregion

            #region Get Methods
            public string getCommentString()
            {
                return commentText;
            }
            #endregion

            #region Static Utils
            public static bool isComment(string originalString)
            {
                if (originalString.StartsWith("#"))
                {
                    return true;
                }
                return false;
            }
            #endregion

        }

        public interface ILine
        {
            void execute(adScript script);
            void parseString();
            string generateString();

            int lineNumber { get; set; }
            string originalString { get; set; }
            bool hasError { get; }
        }

        #region Variable Management
        public void setGlobalVariable(Variable newVar)
        {
            string varID = newVar.id;
            Variable foundValue = null;
            foreach (Variable var in globalVariablesList)
            {
                if (var.id == varID)
                {
                    foundValue = var;
                    break;
                }
            }

            if (foundValue != null)
            {
                globalVariablesList.Remove(foundValue);
            }

            globalVariablesList.Add(newVar);
        }

        public void setGlobalVariable(string varID, string value)
        {
            setGlobalVariable(new Variable(varID, value, "Global"));
        }

        public void setLocalVariable(Variable newVar)
        {
            string varID = newVar.id;
            Variable foundValue = null;
            foreach (Variable var in localVariablesList)
            {
                if (var.id == varID)
                {
                    foundValue = var;
                    break;
                }
            }

            if (foundValue != null)
            {
                localVariablesList.Remove(foundValue);
            }

            localVariablesList.Add(newVar);
        }

        public void setLocalVariable(string varID, string value)
        {
            setLocalVariable(new Variable(varID, value, "Local"));
        }

        public Variable getGlobalVariable(string varID)
        {
            foreach (Variable var in globalVariablesList)
            {
                if (var.id == varID)
                {
                    return var;
                }
            }

            return null;
        }

        public string getGlobalVarString(string varID)
        {
            return getGlobalVariable(varID).value;
        }

        public Variable getLocalVariable(string varID)
        {
            foreach (Variable var in localVariablesList)
            {
                if (var.id == varID)
                {
                    return var;
                }
            }

            return null;
        }

        public string getLocalVarString(string varID)
        {
            return getLocalVariable(varID).value;
        }
        #endregion
    }
}
