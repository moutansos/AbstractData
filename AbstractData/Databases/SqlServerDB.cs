using System;
using System.Collections.Generic;
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

                }
            }
        }

        public void close()
        {
            writeCache();
        }
    }
}
