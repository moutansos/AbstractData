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
        //TODO: Add a script command for printing to the output/log

        private List<Variable> globalVariablesList;
        private List<Variable> localVariablesList;
        private List<IDatabase> databaseReferenceList;
        private tableRef curTableRef;
        private List<dataRef> curDataReferences;

        //An action for handling output - Set by the user of the library
        public Action<string> output;

        #region Constructors
        public adScript()
        {

        }

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
            databaseReferenceList = new List<IDatabase>();
            #endregion

            bool errorFound = false;

            List<ILine> scriptLines = parseLines(dataStream, ref errorFound);

            if (!errorFound)
            {
                //Execute Script
                foreach (ILine lineObj in scriptLines)
                {
                    Output outputObj = null;
                    lineObj.execute(this, ref outputObj);

                    if (outputObj != null &&
                       output != null)
                    {
                        output(outputObj.value);
                    }

                    if(outputObj != null && outputObj.isError)
                    {
                        errorFound = true;
                        break;
                    }
                }
            }

            dataStream.Close();
        }

        public void runLine(string line)
        {
            if(globalVariablesList == null)
            {
                globalVariablesList = new List<Variable>();
            }
            if(localVariablesList == null)
            {
                localVariablesList = new List<Variable>();
            }
            if(databaseReferenceList == null)
            {
                databaseReferenceList = new List<IDatabase>();
            }

            ILine lineObj = getLineObjectForLine(line, -1);
            if(lineObj != null)
            {
                Output outputObj = null;
                
                //Parse
                lineObj.parseString(this, ref outputObj);
                if (outputObj != null &&
                    output != null)
                {
                    output(outputObj.value); //Send the message to the action
                }

                //Execute if no error
                if(outputObj == null ||
                   !outputObj.isError)
                {
                    lineObj.execute(this, ref outputObj);
                }

                if (outputObj != null &&
                    output != null)
                {
                    output(outputObj.value); //Send the message to the action
                }
            }
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
            Variable var = getGlobalVariable(varID);
            if (var != null)
            {
                return var.value;
            }
            return null;
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
            Variable var = getLocalVariable(varID);
            if(var != null)
            {
                return var.value;
            }
            return null;
        }
        #endregion

        private static ILine getLineObjectForLine(string line, int lineNumber)
        {
            #region Check and clean the string
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            line = line.Trim();
            #endregion

            ILine lineObject = null;
            if (dbRef.isDbRef(line))
            {
                lineObject = new dbRef(line);
                //TODO: Check for error in line or implement an events system?
            }
            else if (Comment.isComment(line))
            {
                lineObject = new Comment(line);
            }
            else if (Variable.isVar(line))
            {
                lineObject = new Variable(line);
            }
            else if (tableRef.isTableRef(line))
            {
                lineObject = new tableRef(line);
            }
            else if (dataRef.isDataRef(line))
            {
                lineObject = new dataRef(line);
            }
            else if (moveCom.isMoveCom(line))
            {
                lineObject = new moveCom(line);
            }
            else if (runCom.isARunCom(line))
            {
                lineObject = new runCom(line);
            }

            if(lineObject != null) //Set the line number
            {
                lineObject.lineNumber = lineNumber;
            }

            return lineObject;
        }

        private List<ILine> parseLines(System.IO.StreamReader stream, ref bool errorFound)
        {
            //Parse Script
            int lineCounter = 1;

            string line;
            List<ILine> scriptLines = new List<ILine>();
            while ((line = stream.ReadLine()) != null &&
                   !errorFound) //Parse in file and set up structures as long as there isn't an error
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    ILine lineObject = getLineObjectForLine(line, lineCounter);

                    Output outputObj = null;
                    lineObject.parseString(this, ref outputObj);

                    if(output != null && 
                       outputObj != null && 
                       outputObj.isError) //Check for an error
                    {
                        errorFound = true;
                    }
                    else
                    {
                        if (outputObj != null &&
                            output != null)
                        {
                            output(outputObj.value); //Send the message to the action
                        }

                        if (lineObject != null)
                        {
                            scriptLines.Add(lineObject);
                        }
                    }
                }
                lineCounter++;
            }

            return scriptLines;
        }

        #region Database Methods
        public void addDatabaseReference(IDatabase db)
        {
            if (databaseReferenceList.Where(a => a.id == db.id).Count() == 0)
            {
                databaseReferenceList.Add(db);
            }
        }

        public IDatabase getDatabase(string dbName)
        {
            foreach(IDatabase database in databaseReferenceList)
            {
                if(database.id == dbName)
                {
                    return database;
                }
            }
            return null;
        }
        #endregion

        #region Data Reference Methods
        public void addDataRef(dataRef newRef)
        {
            if(curDataReferences == null)
            {
                curDataReferences = new List<dataRef>();
            }
            curDataReferences.Add(newRef);
        }

        public void clearDataRefs()
        {
            if(curDataReferences != null)
            {
                curDataReferences.Clear();
            }
        }
        #endregion

        #region Properties
        public tableRef currentTableRef
        {
            get { return curTableRef; }
            set { curTableRef = value; }
        }

        public List<dataRef> currentDataRefs
        {
            get { return curDataReferences; }
        }
        #endregion

        public class Output
        {
            private bool error;
            private string outputString;
            private int line;

            #region Constructors
            public Output()
            {
                isError = false;
                value = "";
                line = -1;
            }

            public Output(string message)
            {
                isError = false;
                value = message;
                line = -1;
            }

            public Output(string message, bool isError)
            {
                this.isError = isError;
                value = message;
                line = -1;
            }
            #endregion

            #region Properties
            public bool isError
            {
                get { return error; }
                set { error = value; }
            }

            public bool isEmpty
            {
                get { return (error = false && String.IsNullOrWhiteSpace(outputString)); }
            }

            public int lineNumber
            {
                get { return line; }
                set { line = value; }
            }

            public string value
            {
                get { return generateValue(); }
                set { outputString = value; }
            }
            #endregion

            private string generateValue()
            {
                if (error && line == -1)
                {
                    return "Error: " + outputString;
                }
                else if(error && line != -1)
                {
                    return "Error on line " + line + ": " + outputString;
                }
                else
                {
                    return outputString;
                }
            }
        }
    }
}