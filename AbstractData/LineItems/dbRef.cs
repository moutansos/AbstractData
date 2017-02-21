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
        private string refString;
        private string cleanRefString; //No quotes

        private IDatabase db;
        private string refID;

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
            get { return refString; }
        }

        public string cleanReferenceString
        {
            get { return cleanRefString; }
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
            cleanRefString = db.refString;
            refString = "\"" + cleanRefString + "\"";

            originalString = type + " " + refID + " = " + refString;

            return originalString;
        }

        public void parseString(ref adScript.Output output)
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
            refString = line.Substring(posOfEquals + 1, line.Length - (posOfEquals + 1)).Trim();
            cleanRefString = refString.Trim('\"');

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
            else if(!refString.StartsWith("\"") ||
                    !refString.EndsWith("\""))
            {
                //Perhaps change this later to accept variables. Not sure if that's needed though.
                output = new adScript.Output("The database assignment only accepts a string value.", true);
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
               candidateString.StartsWith(SQLiteDB.idInScript))
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
            else
            {
                throw new Exception("Fatal Exception: Internal error reading database type");
            }
        }

        public IDatabase getDatabase(dbType type)
        {
            if(type == dbType.ExcelFile)
            {
                return new ExcelFile(cleanRefString);
            }
            else if(type == dbType.CSVFile)
            {
                return new CSVFile(cleanRefString);
            }
            else if(type == dbType.AccessDB)
            {
                return new AccessDB(cleanRefString);
            }
            else if(type == dbType.SQLServerDB)
            {
                return new SQLServerDB(cleanRefString);
            }
            else if(type == dbType.PostgreSqlDB)
            {
                return new PostgreSqlDB(cleanRefString);
            }
            else if(type == dbType.MariaDB)
            {
                throw new NotImplementedException("MariaDB Database Not Implemented");
            }
            else if(type == dbType.SQLiteDB)
            {
                return new SQLiteDB(cleanRefString);
            }
            else
            {
                throw new Exception("Fatal Exception: Internal error reading database type");
            }

        }
    }
}
