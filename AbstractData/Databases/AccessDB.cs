using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace AbstractData.Databases
{
    class AccessDB : IDatabase
    {
        public const string idInScript = "AccessDB";
        private const int cacheLimit = 5000;

        private string ID;

        private string fileName;
        private string tableName;

        List<DataEntry> dataEntryCache;

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
            get { return dbType.AccessDB; }
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
            /*
            string connectionString = getConnectionString(fileName);

            if (dataEntryCache.Count > 0)
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "[" + table + "]";
                        DataTable cacheTable = SQLServerDB.convertRecordsToTable(dataEntryCache);
                        foreach (DataColumn column in cacheTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName); //Map each to the field of its name
                        }
                        bulkCopy.WriteToServer(cacheTable);
                    }
                    //Reset the cache
                    dataEntryCache.Clear();
                }
            } */
            throw new NotImplementedException();
        }

        public void getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            string connectionString = getConnectionString(fileName);
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
                        newEntry.convertToWriteEntry(dRefs);
                        addData(newEntry);
                    }
                    reader.Close();
                }
            }
        }

        public void close()
        {
            throw new NotImplementedException();
        }

        private static string getConnectionString(string file)
        {
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
    }
}
