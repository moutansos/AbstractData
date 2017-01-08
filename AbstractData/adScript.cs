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
        private List<IDatabase> databaseReferenceList;

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
            databaseReferenceList = new List<IDatabase>();
            #endregion

            //Parse Script
            int lineCounter = 1;

            string line;
            List<ILine> scriptLines = new List<ILine>();
            while ((line = dataStream.ReadLine()) != null) //Parse in file and set up structures
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    ILine lineObject = getLineObjectForLine(line, lineCounter);

                    if(lineObject != null)
                    {
                        scriptLines.Add(lineObject);
                    }
                }
                lineCounter++;
            }

            //Execute Script
            foreach(ILine lineObj in scriptLines)
            {
                lineObj.execute(this);
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

        private ILine getLineObjectForLine(string line, int lineNumber)
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

            if(lineObject != null) //Set the line number
            {
                lineObject.lineNumber = lineNumber;
            }

            return lineObject;
        }

        public void addDatabaseReference(IDatabase db)
        {
            if (databaseReferenceList.Where(a => a.id == db.id).Count() == 0)
            {
                databaseReferenceList.Add(db);
            }
        }
    }

    public interface ILine
    {
        void execute(adScript script);
        void parseString();
        string generateString();

        Type type { get; }
        int lineNumber { get; set; }
        string originalString { get; set; }
        bool hasError { get; }
    }
}
