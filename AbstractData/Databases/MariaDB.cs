using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace AbstractData
{
    class MariaDB : IDatabase
    {
        //Constants
        public const string idInScript = "MariaDB";
        private const int cacheLimit = 5000;

        private string ID;

        private string tableName;
        private reference connectionString;
        private string connStr;

        List<DataEntry> dataEntryCache;

        #region Constructors
        public MariaDB(reference connectionString)
        {
            dataEntryCache = new List<DataEntry>();
            this.connectionString = connectionString;
        }
        #endregion

        #region Properties
        public string id
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isMultiTable
        {
            get { return false; }
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
            }
        }

        public dbType type
        {
            get { return dbType.MariaDB; }
        }
        #endregion

        public void addData(DataEntry data, adScript script)
        {
            throw new NotImplementedException();
        }

        public void writeCache()
        {
            if(dataEntryCache.Count > 0 && connectionString != null)
            {
                DataTable schemaTable = getSchemaTable(connStr);
                Dictionary<string, Type> columnTypes = getColumnTypes(schemaTable);

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    con.Open();
                    MySqlTransaction transaction = con.BeginTransaction();

                    foreach (DataEntry data in dataEntryCache)
                    {
                        using (MySqlCommand cmd = buildInsertCommand(data, columnTypes))
                        {
                            cmd.Connection = con;
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
            throw new NotImplementedException();
        }

        public moveResult getData(Action<DataEntry, adScript> addData,
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            throw new NotImplementedException();
        }

        private DataTable getSchemaTable(string conStr)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + tableName);
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                con.Open();
                cmd.Connection = con;
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
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

        private MySqlCommand buildInsertCommand(DataEntry dataEntry, Dictionary<string, Type> columnTypes)
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
            for(int i = 0; i < fields.Count(); i++)
            {
                insertString = insertString + "?, ";
            }
            insertString = insertString.Remove(insertString.Length - 2) + ")";
            MySqlCommand cmd = new MySqlCommand(insertString);
            foreach (var field in fields)
            {
                Type columnType = columnTypes[field.column];
                if(columnType == typeof(DateTime))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.dataAsDate);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(string))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.data);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(int))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.dataAsInt);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(double))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.dataAsInt);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(float))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.dataAsFloat);
                    cmd.Parameters.Add(param);
                }
                else if (columnType == typeof(Guid))
                {
                    MySqlParameter param = new MySqlParameter("@" + field.column, field.dataAsGuid);
                    cmd.Parameters.Add(param);
                }
                else
                {
                    throw new ArgumentException("The type " + columnType + " of the column " + field.column + " is invald.");
                }
            }
            return cmd;
        }
    }
}
