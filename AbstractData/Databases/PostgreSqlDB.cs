using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace AbstractData
{
    class PostgreSqlDB : IDatabase
    {
        public const string idInScript = "PostgreSqlDB";
        private const int cacheLimit = 5000;

        private string ID;
        private reference connectionString;
        private string conStr;
        private string tableName;

        private List<DataEntry> dataEntryCache;

        #region Constructors
        public PostgreSqlDB(reference connectionString)
        {
            dataEntryCache = new List<DataEntry>();
            this.connectionString = connectionString;
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
            get { return connectionString.originalString; }
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
            get { return dbType.PostgreSqlDB; }
        }
        #endregion

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            conStr = connectionString.evalReference(null, script, ref output);

            //Open a Sql Connection
            using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
            {
                conn.Open();
                string sqlCommandText = "SELECT  ";
                foreach (string column in readColumns)
                {
                    sqlCommandText = sqlCommandText + "\"" + column + "\",";
                }
                sqlCommandText = sqlCommandText.Remove(sqlCommandText.Length - 1);
                sqlCommandText = sqlCommandText + " FROM \"" + table + "\"";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlCommandText, conn))
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
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

        public void addData(DataEntry data,
                            adScript script)
        {
            if(conStr == null)
            {
                adScript.Output output = null;
                conStr = connectionString.evalReference(null, script, ref output);
            }
            dataEntryCache.Add(data);
            if (dataEntryCache.Count > cacheLimit)
            {
                adScript.Output output = null;
                conStr = connectionString.evalReference(null, script, ref output);
                writeCache();
            }
        }

        public void writeCache()
        {
            if (dataEntryCache.Count > 0 && conStr != null)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(conStr))
                {
                    DataTable schemaTable = getSchemaTable(conStr);
                    Dictionary<string, Type> columnTypes = getColumnTypes(schemaTable);

                    conn.Open();

                    foreach (DataEntry data in dataEntryCache)
                    {
                        using (NpgsqlCommand cmd = buildInsertCommand(data, columnTypes))
                        {
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                //Reset the cache
                dataEntryCache.Clear();
            }
        }

        public void close()
        {
            writeCache();
        }

        private DataTable getSchemaTable(string conStr)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"" + tableName + "\"");
            using (NpgsqlConnection newConn = new NpgsqlConnection(conStr))
            {
                newConn.Open();
                cmd.Connection = newConn;
                NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable schemaTable = reader.GetSchemaTable();
                return schemaTable;
            }
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

        private NpgsqlCommand buildInsertCommand(DataEntry dataEntry, Dictionary<string, Type> columnTypes)
        {
            string insertString = "INSERT INTO \"" + tableName + "\"(";

            IEnumerable<DataEntry.Field> fields = dataEntry.getFields();

            //Add columns to insert statement
            foreach (var field in fields)
            {
                insertString = insertString + "\"" + field.column + "\"" + ", ";
            }

            insertString = insertString.Remove(insertString.Length - 2) + ") VALUES (";
            //Add values placeholders
            foreach (var field in fields)
            {
                insertString = insertString + "@"+ field.column + ", ";
            }
            insertString = insertString.Remove(insertString.Length - 2) + ")";
            NpgsqlCommand cmd = new NpgsqlCommand(insertString);
            foreach (var field in fields)
            {
                Type columnType = columnTypes[field.column];
                if (columnType == typeof(DateTime))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsDate);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(string))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.data);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(int))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsInt);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(double))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsDouble);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(float))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsFloat);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(bool))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsBool);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(decimal))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsDecimal);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(Guid))
                {
                    NpgsqlParameter param = new NpgsqlParameter("@" + field.column, field.dataAsGuid);
                    cmd.Parameters.Add(param);
                }
                else
                {
                    throw new ArgumentException("The type " + columnType + " of the column " + field.column + " is invalid.");
                }
            }
            return cmd;
        }
    }
}
