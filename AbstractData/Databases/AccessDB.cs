using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace AbstractData
{
    class AccessDB : IDatabase
    {
        public const string idInScript = "AccessDB";
        private const int cacheLimit = 5000;

        private string ID;
        private reference fileName;
        private string connectionString;
        private string tableName;

        List<DataEntry> dataEntryCache;

        #region Constructors
        public AccessDB(reference file)
        {
            dataEntryCache = new List<DataEntry>();
            fileName = file;
        }
        #endregion

        #region Properties
        public string id
        {
            get { return ID; }
            set { ID = value; }
        }

        public bool isMultiTable
        {
            get { return true; }
        }

        public string refString
        {
            get { return fileName.originalString; }
        }

        public string table
        {
            get { return tableName; }
            set
            {
                writeCache();
                tableName = value;
            }
        }

        public dbType type
        {
            get { return dbType.AccessDB; }
        }
        #endregion

        public void addData(DataEntry data,
                            adScript script)
        {
            dataEntryCache.Add(data);

            if(connectionString != null)
            {
                evaluateConnectionString(script);
            }

            if (dataEntryCache.Count > cacheLimit)
            {
                writeCache();
            }
        }

        public void writeCache()
        {
            if (dataEntryCache.Count > 0 && connectionString != null)
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    DataTable schemaTable = getSchemaTable(conn);
                    Dictionary<string, Type> columnTypes = getColumnTypes(schemaTable);

                    conn.Open();

                    foreach(DataEntry data in dataEntryCache)
                    {
                        using(OleDbCommand cmd = buildInsertCommand(data, columnTypes))
                        {
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    //Reset the cache
                    dataEntryCache.Clear();
                }
            }
            //throw new NotImplementedException();
        }

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs, 
                                  adScript script, 
                                  ref adScript.Output output)
        {
            evaluateConnectionString(script);
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            if(connectionString == null)
            {
                throw new ArgumentException("The provided access file name was invalid");
            }

            //Open a Ole Connection
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sqlCommandText = "SELECT  ";
                foreach (string column in readColumns)
                {
                    sqlCommandText = sqlCommandText + column + ",";
                }
                sqlCommandText = sqlCommandText.Remove(sqlCommandText.Length - 1);
                sqlCommandText = sqlCommandText + " FROM [" + table + "]";
                using (OleDbCommand cmd = new OleDbCommand(sqlCommandText, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataEntry newEntry = new DataEntry();
                        foreach (string column in readColumns)
                        {
                            string dataToGet = reader.GetValue(readColumns.IndexOf(column)).ToString();
                            newEntry.addField(column, dataToGet);
                        }
                        //Add the data to the database
                        newEntry.convertToWriteEntry(dRefs, script, ref output);
                        addData(newEntry, script);

                        //Increment counters
                        result.incrementTraversalCounter();
                        result.incrementMovedCounter(); //TODO: Change this when implementing conditionals
                    }
                    reader.Close();
                }
            }

            return result;
        }

        public void close()
        {
            writeCache();
        }

        private static string getConnectionString(string file)
        {
            //TODO: Move the file exists logic to evaluateConnectionString method as to allow for output
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + file + "; Persist Security Info=False;";
            }
            else
            {
                return null;
            }
        }

        private OleDbCommand buildInsertCommand(DataEntry dataEntry, Dictionary<string, Type> columnTypes)
        {
            string insertString = "INSERT INTO " + tableName + "([";

            IEnumerable<DataEntry.Field> fields = dataEntry.getFields();

            //Add columns to insert statement
            foreach(var field in fields)
            {
                insertString = insertString + field.column + "], [";
            }

            insertString = insertString.Remove(insertString.Length - 3) + ") VALUES (";
            //Add values placeholders
            for (int i = 0; i < fields.Count(); i++)
            {
                insertString = insertString + "?, ";
            }
            insertString = insertString.Remove(insertString.Length - 2) + ")";
            OleDbCommand cmd = new OleDbCommand(insertString);
            foreach (var field in fields)
            {
                Type columnType = columnTypes[field.column];
                if (columnType == typeof(DateTime))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsDate);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(string))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.data);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(int))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsInt);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(double))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsDouble);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(float))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsFloat);
                    cmd.Parameters.Add(param);
                }
                else if(columnType == typeof(bool))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsBool);
                    cmd.Parameters.Add(param);
                }
                else if(columnType == typeof(decimal))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column, field.dataAsDecimal);
                    cmd.Parameters.Add(param);
                }
                else if(columnType == typeof(Guid))
                {
                    OleDbParameter param = new OleDbParameter("@" + field.column,  field.dataAsGuid);
                    cmd.Parameters.Add(param);
                }
                else
                {
                    throw new ArgumentException("The type " + columnType + " of the column " + field.column + " is invalid.");
                }
            }
            return cmd;
        }

        private DataTable getSchemaTable(OleDbConnection conn)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM " + tableName);
            cmd.Connection = conn;
            OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
            DataTable schemaTable = reader.GetSchemaTable();
            return schemaTable;
        }

        private Dictionary<string, Type> getColumnTypes(DataTable schemaTable)
        {
            Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

            foreach(DataRow row in schemaTable.Rows)
            {
                string columnName = row["ColumnName"].ToString();
                Type columnType = Type.GetType(row["DataType"].ToString());
                typeDict.Add(columnName, columnType);
            }

            return typeDict;
        }

        private void evaluateConnectionString(adScript script)
        {
            adScript.Output output = null;
            connectionString = getConnectionString(fileName.evalReference(null, script, ref output));
        }
    }
}
