using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class dbRef : ILine
    {
        private string errorText;
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
                parseString();
            }
        }

        public string referenceID
        {
            get { return refID; }
            set { refID = value; }
        }
        #endregion

        public void execute(adScript script)
        {
            throw new NotImplementedException();
        }

        public string generateString()
        {
            if (db == null ||
               refID == null)
            {
                return null;
            }

            //TODO: Generate the string
            return "";
        }

        public void parseString()
        {
            string line = originalString;
            //DB Type
            int posOfFirstSpace = line.IndexOf(' ');
            string typeString = line.Substring(0, posOfFirstSpace).Trim(); //Verify the math on this one
            dbType type = getDbType(typeString);

            //Ref ID
            int posOfEquals = line.IndexOf('=');
            refID = line.Substring(posOfFirstSpace, posOfEquals - posOfFirstSpace).Trim(); //This one too

            //Ref String
            refString = line.Substring(posOfEquals + 1, line.Length - (posOfEquals + 1)).Trim(); //Again, this one too
            cleanRefString = refString.Trim('\"');

            //Get the database
            db = getDatabase(type);
        }

        public static dbType getDbType(string type)
        {
            if(type == ExcelFile.idInScript)
            {
                return dbType.ExcelFile;
            }
            else if(type == "CSVFile")
            {
                return dbType.CSVFile;
            }
            else if(type == "AccessDB")
            {
                return dbType.AccessDB;
            }
            else if(type == SQLServerDB.idInScript)
            {
                return dbType.SQLServerDB;
            }
            else if(type == "PostgreSqlDB")
            {
                return dbType.PostgreSqlDB;
            }
            else if(type == "MariaDB")
            {
                return dbType.MariaDB;
            }
            else if(type == "SQLiteDB")
            {
                return dbType.SQLiteDB;
            }
            else
            {
                throw new ArgumentException("The input database type of: " + type + " was not valid");
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
                return "CSVFile";
            }
            else if(type == dbType.AccessDB)
            {
                return "AccessDB";
            }
            else if(type == dbType.SQLServerDB)
            {
                return SQLServerDB.idInScript;
            }
            else if(type == dbType.PostgreSqlDB)
            {
                return "PostgreSqlDB";
            }
            else if(type == dbType.MariaDB)
            {
                return "MariaDB";
            }
            else if(type == dbType.SQLiteDB)
            {
                return "SQLiteDB";
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
                throw new NotImplementedException("CSVFile Database Not Implemented");
            }
            else if(type == dbType.AccessDB)
            {
                throw new NotImplementedException("AccessDB Database Not Implemented");
            }
            else if(type == dbType.SQLServerDB)
            {
                return new SQLServerDB(cleanRefString);
            }
            else if(type == dbType.PostgreSqlDB)
            {
                throw new NotImplementedException("PostgreSqlDB Database Not Implemented");
            }
            else if(type == dbType.MariaDB)
            {
                throw new NotImplementedException("MariaDB Database Not Implemented");
            }
            else if(type == dbType.SQLiteDB)
            {
                throw new NotImplementedException("SQLiteDB Database Not Implemented");
            }
            else
            {
                throw new Exception("Fatal Exception: Internal error reading database type");
            }

        }
    }
}
