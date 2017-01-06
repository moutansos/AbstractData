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
            List<adLine> scriptLines = new List<adLine>();
            while ((line = dataStream.ReadLine()) != null) //Parse in file and set up structures
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    scriptLines.Add(new adLine(lineCounter, line));
                }
                lineCounter++;
            }


        }

        public class adLine
        {
            //Line Type
            private bool isDbRef;
            private bool isVarAssgn;
            private bool isTableRef;
            private bool isDataRef;
            private bool isCommand;
            private bool isComment;
            private bool isLastHeaderComment;

            //Error found
            private bool hasError;

            //Line String
            private string originalString;
            private int lineNumber;

            //Database Reference
            private dbRef dbRef;
            private dbObject db;

            #region Constructors
            public adLine(int lineNumber, string originalString)
            {
                this.originalString = originalString;
                this.lineNumber = lineNumber;

                //Run checks to see what kind of command it is
                isVarAssgn = Variable.isVar(this);
                parseString();
            }
            #endregion

            #region Set Methods
            #endregion

            #region Get Methods
            public string getOriginalString()
            {
                return originalString;
            }
            #endregion

            private void parseString()
            {
                originalString = originalString.Trim();

                //Assign all the type flags
                isDbRef = dbRef.isDbRef(originalString);

                //TODO: Check for conflicts here

                //Parse the strings
                if (isDbRef)
                {
                    dbRef = new dbRef(this);
                    db = dbRef.getDbObject();
                }
            }

        }

        //TODO: add all variable parsing logic to this class
        public class Variable
        {
            private string varID;
            private string varValue; //Switch to a reference
            private string typeID;
            private adLine line;

            private string originalString;

            #region Constructors
            public Variable()
            {

            }

            public Variable(adLine line)
            {
                this.line = line;
                originalString = line.getOriginalString();
                parseAndSet();
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
            }
            #endregion
        }

        public class dbRef
        {
            private string dbName;
            private string dbType;
            private string connectionString;
            private dbObject db;
            private adLine line;

            private string originalString;

            #region Constructors
            public dbRef(string dbName, dbObject db)
            {
                setDbName(dbName);
                setDbType(db.getDbType());
                setConnectionString(db.getConnectionString());

                originalString = generateDbRef();
            }

            public dbRef(string dbName, string dbType, string connectionString)
            {
                setDbName(dbName);
                setDbType(dbType);
                setConnectionString(connectionString);

                originalString = generateDbRef();
            }

            public dbRef(adLine line)
            {
                this.line = line;
                originalString = line.getOriginalString();
                parseDbRef();
            }
            #endregion

            private void parseDbRef()
            {
                int posOfFirstSpace = originalString.IndexOf(' ');
                dbType = originalString.Substring(0, posOfFirstSpace);
                connectionString = StringUtils.returnStringInside(originalString, '"', '"');
            }

            public string generateDbRef()
            {
                originalString = dbType + " " + dbName + " = \"" + connectionString + "\"";
                return originalString;
            }

            #region Set Methods
            public void setDbType(string dbType)
            {
                if ((dbType != "SqlServerDb") &&
                    (dbType != "PostgreSqlDb") &&
                    (dbType != "AccessDb") &&
                    (dbType != "CSVFile") &&
                    (dbType != "ExcelFile"))
                {
                    this.dbType = dbType;
                }
                else
                {
                    throw new UnknownDatabaseTypeException();
                }
            }

            public void setDbName(string dbName)
            {
                this.dbName = dbName;
            }

            public void setConnectionString(string connectionString)
            {
                this.connectionString = connectionString;
            }
            #endregion

            #region Get Methods
            public string getDbType()
            {
                return dbType;
            }

            public dbObject getDbObject()
            {
                return db;
            }
            #endregion

            #region Static Utils
            public static bool isDbRef(string originalString)
            {
                //TODO: Add REGEX validation here
                if (originalString.StartsWith("SqlServerDb") ||
                   originalString.StartsWith("PostgreSqlDb") ||
                   originalString.StartsWith("AccessDb") ||
                   originalString.StartsWith("CSVFile") ||
                   originalString.StartsWith("ExcelFile"))
                {
                    return true;
                }
                return false;
            }
            #endregion
        }

        public class comment
        {
            private string commentText;
            private adLine line;

            private string originalString;

            #region Constructors
            public comment()
            {

            }

            public comment(adLine line)
            {
                this.line = line;
                this.originalString = line.getOriginalString();
            }

            public comment(string commentString)
            {
                originalString = commentString;
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

            public void setLine(adLine line)
            {
                this.line = line;
            }
            #endregion

            #region Get Methods
            public string getCommentString()
            {
                return commentText;
            }

            public adLine getLine()
            {
                return line;
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
