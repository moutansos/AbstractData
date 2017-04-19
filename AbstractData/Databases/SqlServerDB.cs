using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class SQLServerDB : IDatabase
    {
        //Constants
        public const string idInScript = "SQLServerDB";
        private const int cacheLimit = 5000;

        private string ID;

        private string tableName;
        private reference connectionString;
        private string connStr;

        List<DataEntry> dataEntryCache;

        #region Constructors
        public SQLServerDB(reference connectionString)
        {
            dataEntryCache = new List<DataEntry>();
            this.connectionString = connectionString;
        }
        #endregion

        #region Properties
        public bool isMultiTable
        {
            get { return true; }
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
            get { return dbType.SQLServerDB; }
        }

        public string refString
        {
            get { return connectionString.originalString; }
        }

        public string id
        {
            get { return ID; }
            set { ID = value; }
        }
        #endregion

        public void addData(DataEntry data,
                            adScript script)
        {
            dataEntryCache.Add(data);
            if(dataEntryCache.Count > cacheLimit)
            {
                evalConnectionString(script);
                writeCache();
            }
        }

        public void writeCache()
        {
            if(dataEntryCache.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "dbo.[" + table + "]";
                        DataTable cacheTable = convertRecordsToTable(dataEntryCache);
                        foreach (DataColumn column in cacheTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName); //Map each to the field of its name
                        }
                        bulkCopy.WriteToServer(cacheTable);
                    }
                    //Reset the cache
                    dataEntryCache.Clear();
                }
            }
        }

        public void close()
        {
            writeCache();
        }

        public static DataTable convertRecordsToTable(List<DataEntry> dataList)
        {
            DataTable newData = new DataTable();
            foreach(DataEntry entry in dataList)
            {
                IEnumerable<DataEntry.Field> fields = entry.getFields();
                //Add Needed Columns
                foreach(DataEntry.Field field in fields)
                {
                    if (!newData.Columns.Contains(field.column))
                    {
                        newData.Columns.Add(field.column);
                    }
                }

                //Add the data to the table
                DataRow newRow = newData.NewRow();
                foreach(DataEntry.Field field in fields)
                {
                    newRow[field.column] = field.data;
                }
                newData.Rows.Add(newRow);
            }

            return newData;
        }

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            evalConnectionString(script);
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            //Open a Sql Connection
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sqlCommandText = "SELECT  ";
                foreach (string column in readColumns)
                {
                    sqlCommandText = sqlCommandText + column + ",";
                }
                sqlCommandText = sqlCommandText.Remove(sqlCommandText.Length - 1);
                if (table.Contains('.'))
                {
                    /*
                    string[] tableNameArray = table.Split('.');
                    for(int i = 0; i < tableNameArray.Length; i++)
                    {
                        if(i == 0)
                        {
                            sqlCommandText = sqlCommandText + " FROM dbo.[" + tableNameArray[i] + "]";
                        }
                        else
                        {
                            sqlCommandText = sqlCommandText + ".[" + tableNameArray[i] + "]";
                        }
                    }*/
                    sqlCommandText = sqlCommandText + " FROM " + table;
                }else
                {
                    sqlCommandText = sqlCommandText + " FROM dbo.[" + table + "]";
                }
                using (SqlCommand cmd = new SqlCommand(sqlCommandText, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
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

        private void evalConnectionString(adScript script)
        {
            adScript.Output output = null;
            connStr = connectionString.evalReference(null, script, ref output);
        }
    } 
}