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
        private string fileName;
        private string tableName;

        private List<DataEntry> dataEntryCache;

        #region  Constructors
        public SQLiteDB(string file)
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
            get { return fileName; }
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

        public void addData(DataEntry data)
        {
            dataEntryCache.Add(data);
            if (dataEntryCache.Count > cacheLimit)
            {
                writeCache();
            }
        }

        public void writeCache()
        {
            if (dataEntryCache.Count > 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(getSQLiteConnectionString()))
                {
                    DataTable schemaTable = getSchemaTable(conn);
                    Dictionary<string, Type> columnTypes = getColumnTypes(schemaTable);

                    conn.Open();

                    foreach (DataEntry data in dataEntryCache)
                    {
                        using (SQLiteCommand cmd = buildInsertCommand(data, columnTypes))
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

        public void getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);

            //Open a Sql Connection
            using (SQLiteConnection conn = new SQLiteConnection(getSQLiteConnectionString()))
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
                        newEntry.convertToWriteEntry(dRefs);
                        addData(newEntry);
                    }
                    reader.Close();
                }
            }

        }

        public void close()
        {
            writeCache();
        }

        private string getSQLiteConnectionString()
        {
            return "Data Source=" + fileName + ";Version=3;";
        }

        private DataTable getSchemaTable(SQLiteConnection conn)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + tableName);
            using (SQLiteConnection newConn = new SQLiteConnection(getSQLiteConnectionString()))
            {
                newConn.Open();
                cmd.Connection = new SQLiteConnection(newConn);
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
    }
}
