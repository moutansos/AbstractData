using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class GoogleSheet : IDatabase
    {
        public const string idInScript = "GoogleSheet";
        private const int cacheLimit = 5000;

        private string ID;
        private string refStr;
        private string tableName;

        private List<DataEntry> dataEntryCache;

        #region Constructors
        public GoogleSheet(string refStr)
        {
            dataEntryCache = new List<DataEntry>();
            this.refStr = refStr;
        }
        #endregion

        #region Properites
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
            get { return refStr; }
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
            get { return dbType.GoogleSheet; }
        }
        #endregion

        public moveResult getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        public void close()
        {
            throw new NotImplementedException();
        }
    }
}
