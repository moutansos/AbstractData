using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class MongoDB : IDatabase
    {
        const string idInScript = "MongoDB";
        const int cacheLimit = 5000;

        private string ID;
        private string collectionName;

        private static IMongoClient client;
        private List<DataEntry> dataEntryCache;

        //Script References
        reference connectionString;
        string conStr;

        #region Constructors
        public MongoDB(reference connectionString)
        {
            this.connectionString = connectionString;
            dataEntryCache = new List<DataEntry>();
        }
        #endregion

        #region Properties
        public dbType type => dbType.MongoDB;

        public string id
        {
            get => ID;
            set => ID = value;
        }

        public string table
        {
            get => collectionName;
            set
            {
                writeCache();
                collectionName = value;
            }
        }
        #endregion

        public void addData(DataEntry data, adScript script)
        {
            if (conStr == null)
            {
                evalReferences(script);
            }

            dataEntryCache.Add(data);
            if (dataEntryCache.Count > cacheLimit)
            {
                writeCache();
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

        private void writeCache()
        {
            if (dataEntryCache.Count > 0 && conStr != null)
            {
                foreach(DataEntry entry in dataEntryCache)
                {
                    //BsonDocument doc = getDocumentForEntry(entry);

                    
                }
            }
        }

        private void evalReferences(adScript script)
        {
            adScript.Output output = null; //TODO: Fix this. 
            conStr = connectionString.evalReference(null, script, ref output);
        }

        /*
        private BsonDocument getDocumentForEntry(DataEntry entry)
        {
            BsonDocument doc = new BsonDocument();
            
        }
        */
    }
}
