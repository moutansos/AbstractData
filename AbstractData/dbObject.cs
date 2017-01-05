using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class dbObject
    {
        private string dbType;
        private string connectionString;

        #region Constructors
        public dbObject(string dbType, string connectionString)
        {
            this.dbType = dbType;
            this.connectionString = connectionString;
        }
        #endregion

        #region Get Methods
        public string getDbType()
        {
            return dbType;
        }

        public string getConnectionString()
        {
            return connectionString;
        }
        #endregion

        #region Set Methods
        #endregion
    }
}
