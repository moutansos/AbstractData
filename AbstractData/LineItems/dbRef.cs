using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class dbRef : ILine
    {
        private int line;
        private string lineString;
        private reference refString;

        private IDatabase db;
        private string refID;
        StringUtils.constructorVals constructorVals;

        #region Constructor
        public dbRef(string original)
        {
            originalString = original;
        }

        public dbRef(IDatabase db, string refId)
        {
            this.db = db;
            referenceID = refId;
        }
        #endregion

        #region Properties
        public Type type
        {
            get { return typeof(dbRef); }
        }

        public dbType databaseType
        {
            get
            {
                if (db != null) { return db.type; }
                else { return dbType.Unknown; }
            }
        }

        public string referenceString
        {
            get { return refString.originalString; }
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

        public string referenceID
        {
            get { return refID; }
            set { refID = value; }
        }
        #endregion

        public void execute(adScript script, ref adScript.Output output)
        {
            db.id = refID;
            script.addDatabaseReference(db);
        }

        public string generateString()
        {
            if (db == null ||
                refID == null)
            {
                return null;
            }

            string type = getDbType(db.type);

            originalString = type + " " + refID + " = " + refString.originalString;

            return originalString;
        }

        public void parseString(adScript script, ref adScript.Output output)
        {
            string line = originalString;
            //DB Type
            int posOfFirstSpace = line.IndexOf(' ');
            string typeString = line.Substring(0, posOfFirstSpace).Trim();
            dbType type = getDbType(typeString);

            //Ref ID
            int posOfEquals = line.IndexOf('=');
            refID = line.Substring(posOfFirstSpace, posOfEquals - posOfFirstSpace).Trim();

            //Ref String
            string referenceString = line.Substring(posOfEquals + 1, line.Length - (posOfEquals + 1)).Trim();
            if(referenceString.StartsWith("\"") || referenceString.EndsWith("\""))
            {
                refString = new reference(referenceString);
            }
            else if (referenceString.ToLower().StartsWith("new"))
            {
                parseConstructorRef(referenceString, script, ref output); // Provides parsing for the constructor syntax
            }
            else
            {
                output = new adScript.Output("The reference string for the database: " + refID + " is invalid", true);
            }

            //Get the database
            db = getDatabase(type);

            //Error Checking
            //TODO: Add RegEx validation
            if (type == dbType.Unknown)
            {
                output = new adScript.Output("The input database type of: " + type + " was not valid", true);
                if (this.line > 0)
                {
                    output.lineNumber = this.line;
                }
            }
            else if(!(referenceString.StartsWith("\"") && referenceString.EndsWith("\"")) &&
                    !referenceString.StartsWith("new"))
            {
                //Perhaps change this later to accept variables. Not sure if that's needed though.
                output = new adScript.Output("The database assignment only accepts a string value or a constructor.", true);
                if (this.line > 0)
                {
                    output.lineNumber = this.line;
                }
            }
        }

        public static bool isDbRef(string candidateString)
        {
            if(candidateString.StartsWith(ExcelFile.idInScript) ||
               candidateString.StartsWith(CSVFile.idInScript) ||
               candidateString.StartsWith(AccessDB.idInScript) ||
               candidateString.StartsWith(SQLServerDB.idInScript) ||
               candidateString.StartsWith(PostgreSqlDB.idInScript) ||
               candidateString.StartsWith("MariaDB") ||
               candidateString.StartsWith(SQLiteDB.idInScript) ||
               candidateString.StartsWith(GoogleSheets.idInScript))
            {
                return true;
            }

            return false;
        }

        public static dbType getDbType(string type)
        {
            if(type == ExcelFile.idInScript)
            {
                return dbType.ExcelFile;
            }
            else if(type == CSVFile.idInScript)
            {
                return dbType.CSVFile;
            }
            else if(type == AccessDB.idInScript)
            {
                return dbType.AccessDB;
            }
            else if(type == SQLServerDB.idInScript)
            {
                return dbType.SQLServerDB;
            }
            else if(type == PostgreSqlDB.idInScript)
            {
                return dbType.PostgreSqlDB;
            }
            else if(type == "MariaDB")
            {
                return dbType.MariaDB;
            }
            else if(type == SQLiteDB.idInScript)
            {
                return dbType.SQLiteDB;
            }
            else if(type == GoogleSheets.idInScript)
            {
                return dbType.GoogleSheets;
            }
            else
            {
                return dbType.Unknown;
            }
        }

        public static string getDbType(dbType type)
        {
            if(type == dbType.ExcelFile)
            {
                return ExcelFile.idInScript;
            }
            else if(type == dbType.CSVFile)
            {
                return CSVFile.idInScript;
            }
            else if(type == dbType.AccessDB)
            {
                return AccessDB.idInScript;
            }
            else if(type == dbType.SQLServerDB)
            {
                return SQLServerDB.idInScript;
            }
            else if(type == dbType.PostgreSqlDB)
            {
                return PostgreSqlDB.idInScript;
            }
            else if(type == dbType.MariaDB)
            {
                return "MariaDB";
            }
            else if(type == dbType.SQLiteDB)
            {
                return SQLiteDB.idInScript;
            }
            else if(type == dbType.GoogleSheets)
            {
                return GoogleSheets.idInScript;
            }
            else
            {
                throw new Exception("Fatal Exception: Internal error reading database type");
            }
        }

        public IDatabase getDatabase(dbType type)
        {
            if(type == dbType.ExcelFile)
            {
                //TODO: Handle errors on reference creation and constructor vals reference
                if(constructorVals != null)
                {
                    reference fileName = constructorVals["fileName"];
                    return new ExcelFile(fileName);
                }
                else
                {
                    return new ExcelFile(refString);
                }
            }
            else if(type == dbType.CSVFile)
            {
                //TODO: Handle errors on reference creation and constructor vals reference
                if(constructorVals != null)
                {
                    reference fileName = constructorVals["fileName"];
                    return new CSVFile(fileName);
                }
                else
                {
                    return new CSVFile(refString);
                }
            }
            else if(type == dbType.AccessDB)
            {
                //TODO: Handle errors on reference creation and constructor vals reference
                if(constructorVals != null)
                {
                    reference fileName = constructorVals["fileName"];
                    return new AccessDB(fileName);
                }
                else
                {
                    return new AccessDB(refString);
                }
            }
            else if(type == dbType.SQLServerDB)
            {
                //TODO: Handle errors on reference creation and constructor vals reference
                if (constructorVals != null)
                {
                    reference fileName = constructorVals["fileName"];
                    return new SQLiteDB(fileName);
                }
                else
                {
                    return new SQLServerDB(refString);
                }
            }
            else if(type == dbType.PostgreSqlDB)
            {
                //TODO: Handle errors on reference creation and constructor vals reference
                if(constructorVals != null)
                {
                    reference conStr = constructorVals["connectionString"];
                    return new PostgreSqlDB(conStr);
                }
                else
                {
                    return new PostgreSqlDB(refString);
                }
            }
            else if(type == dbType.MariaDB)
            {
                throw new NotImplementedException("MariaDB Database Not Implemented");
            }
            else if(type == dbType.SQLiteDB)
            {
                if(constructorVals != null)
                {
                    reference fileName = constructorVals["fileName"];
                    return new SQLiteDB(fileName);
                }
                else
                {
                    return new SQLiteDB(refString);
                }
            }
            else if(type == dbType.GoogleSheets)
            {
                //TODO: Figure out how to manage if these are in error.
                if(constructorVals != null)
                {
                    reference clientSecretPath = constructorVals["secretPath"];
                    reference credPath = constructorVals["credPath"];
                    reference id = constructorVals["id"];
                    return new GoogleSheets(id, credPath, clientSecretPath); //Return the unclean one for google sheets
                }
                else
                {
                    //TODO: Throw an error because google sheets requires constructor syntax
                    return null;
                }
            }
            else
            {
                throw new Exception("Fatal Exception: Internal error reading database type");
            }

        }

        public void parseConstructorRef(string refString, adScript script, ref adScript.Output output)
        {
            int posOfFirstSpace = refString.IndexOf(' ');
            int posOfFirstParenth = refString.IndexOf('(');
            int posOfLastParenth = refString.LastIndexOf(')');
            string constructorType = refString.Substring(posOfFirstSpace + 1, refString.Length - (posOfLastParenth - posOfFirstParenth) - posOfFirstSpace - 2); //TODO: Check if this is right
            string innerVars = refString.Substring(posOfFirstParenth + 1, posOfLastParenth - posOfFirstParenth - 1); //TODO: Check this too
            if (getDbType(constructorType) == dbType.Unknown)
            {
                output = new adScript.Output("The database types in the dbRef do not match");
            }
            else
            {
                //Continue with the parsing
                constructorVals = new StringUtils.constructorVals(innerVars); //Only initialize constructorvals when the constructor syntax is used
            }
        }
    }
}
