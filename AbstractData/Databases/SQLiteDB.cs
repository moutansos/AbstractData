using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace AbstractData
{
    public class SQLiteDB : IDatabase
    {
        public const string idInScript = "SQLiteDB";
        private const int cacheLimit = 5000;

        private string ID;
        private reference fileName;
        private string connectionString;
        private string tableName;

        private List<DataEntry> dataEntryCache;

        #region  Constructors
        public SQLiteDB(reference file)
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
            get { return dbType.SQLiteDB; }
        }
        #endregion

        public void addData(DataEntry data,
                            adScript script)
        {
            if(fileName == null)
            {
                evalConnectionString(script);
            }

            dataEntryCache.Add(data);
            if (dataEntryCache.Count > cacheLimit)
            {
                writeCache();
            }
        }

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            evalConnectionString(script);
            string conStr = connectionString;
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            //Open a Sql Connection
            using (SQLiteConnection conn = new SQLiteConnection(conStr))
            {
                conn.Open();
                string sqlCommandText = "SELECT  ";
                foreach (string column in readColumns)
                {
                    sqlCommandText = sqlCommandText + column + ",";
                }
                sqlCommandText = sqlCommandText.Remove(sqlCommandText.Length - 1);
                sqlCommandText = sqlCommandText + " FROM " + table;
                using (SQLiteCommand cmd = new SQLiteCommand(sqlCommandText, conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
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

        public void writeCache()
        {
            if (dataEntryCache.Count > 0 && connectionString != null)
            {
                DataTable schemaTable = getSchemaTable(connectionString);
                Dictionary<string, Type> columnTypes = getColumnTypes(schemaTable);

                using (SQLiteConnection conn = new SQLiteConnection(getSQLiteConnectionString(connectionString)))
                {
                    conn.Open();
                    SQLiteTransaction transaction = conn.BeginTransaction();

                    foreach (DataEntry data in dataEntryCache)
                    {
                        using (SQLiteCommand cmd = buildInsertCommand(data, columnTypes))
                        {
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }

                //Reset the cache
                dataEntryCache.Clear();
            }
        }

        public void close()
        {
            writeCache();
        }

        private string getSQLiteConnectionString(string file)
        {
            return "Data Source=" + file + ";Version=3;";
        }

        private DataTable getSchemaTable(string conStr)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + tableName);
            using (SQLiteConnection newConn = new SQLiteConnection(conStr))
            {
                newConn.Open();
                cmd.Connection = newConn;
                SQLiteDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable schemaTable = reader.GetSchemaTable();
                return schemaTable;
            }
        }

        private SQLiteCommand buildInsertCommand(DataEntry dataEntry, Dictionary<string, Type> columnTypes)
        {
            string insertString = "INSERT INTO " + tableName + "(";

            IEnumerable<DataEntry.Field> fields = dataEntry.getFields();

            //Add columns to insert statement
            foreach (var field in fields)
            {
                insertString = insertString + field.column + ", ";
            }

            insertString = insertString.Remove(insertString.Length - 2) + ") VALUES (";
            //Add values placeholders
            for (int i = 0; i < fields.Count(); i++)
            {
                insertString = insertString + "?, ";
            }
            insertString = insertString.Remove(insertString.Length - 2) + ")";
            SQLiteCommand cmd = new SQLiteCommand(insertString);
            foreach (var field in fields)
            {
                Type columnType = columnTypes[field.column];
                if (columnType == typeof(DateTime))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsDate);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(string))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.data);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(int))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsInt);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(double))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsDouble);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(float))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsFloat);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(bool))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsBool);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(decimal))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsDecimal);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(Guid))
                {
                    SQLiteParameter param = new SQLiteParameter("@" + field.column, field.dataAsGuid);
                    cmd.Parameters.Add(param);
                }
                else
                {
                    throw new ArgumentException("The type " + columnType + " of the column " + field.column + " is invalid.");
                }
            }
            return cmd;
        }

        private Dictionary<string, Type> getColumnTypes(DataTable schemaTable)
        {
            Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

            foreach (DataRow row in schemaTable.Rows)
            {
                string columnName = row["ColumnName"].ToString();
                Type columnType = Type.GetType(row["DataType"].ToString());
                typeDict.Add(columnName, columnType);
            }

            return typeDict;
        }

        private void evalConnectionString(adScript script)
        {
            //TODO: Check if the file exists
            adScript.Output output = null;
            connectionString = getSQLiteConnectionString(fileName.evalReference(null, script, ref output));
        }
    }
}