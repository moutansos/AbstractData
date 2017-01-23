﻿using System;
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

        private string ID;

        private string tableName;
        private string connectionString;

        List<DataEntry> dataEntryCache;

        #region Constructors
        public SQLServerDB(string connectionString)
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
            get { return connectionString; }
        }

        public string id
        {
            get { return ID; }
            set { ID = value; }
        }
        #endregion

        public void addData(DataEntry data)
        {
            dataEntryCache.Add(data);
            if(dataEntryCache.Count > 5000)
            {
                writeCache();
            }
        }

        public void writeCache()
        {
            if(dataEntryCache.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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

        public DataTable convertRecordsToTable(List<DataEntry> dataList)
        {
            DataTable newData = new DataTable();
            foreach(DataEntry entry in dataEntryCache)
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

        public void getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);

            //Open a Sql Connection
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sqlCommandText = "SELECT  ";
                foreach (string column in readColumns)
                {
                    sqlCommandText = sqlCommandText + column + ",";
                }
                sqlCommandText = sqlCommandText.Remove(sqlCommandText.Length - 1);
                sqlCommandText = sqlCommandText + " FROM dbo.[" + table + "]";
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
                        newEntry.convertToWriteEntry(dRefs);
                        addData(newEntry);
                    }
                    reader.Close();
                }
            }
            
        }
    }
}

