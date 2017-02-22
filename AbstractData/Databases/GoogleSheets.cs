using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbstractData
{
    public class GoogleSheets : IDatabase
    {
        public const string idInScript = "GoogleSheets";
        private const int cacheLimit = 5000;

        private string ID;
        private string refStr;
        private string tableName;

        private string apiKey; //The api key for the google sheets user
        private string sheetUrl; //The shared url of the sheet
        private string sheetId; //The id for the spreadsheet


        private List<DataEntry> dataEntryCache;

        #region Constructors
        public GoogleSheets(string refStr)
        {
            dataEntryCache = new List<DataEntry>();
            this.refStr = refStr;

            //Setup credential path
            string credPath = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-abstractdata.json");

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
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();
            string jsonString = string.Empty;
            string url = buildReadRequestURL();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }

            SheetsData data = JsonConvert.DeserializeObject<SheetsData>(jsonString);

            foreach(List<string> row in data.values)
            {
                DataEntry newEntry = new DataEntry();
                foreach (string column in readColumns)
                {
                    string dataToGet = row[getColumn(column)];
                    newEntry.addField(column, dataToGet);
                }
                //Add the data to the database
                newEntry.convertToWriteEntry(dRefs);
                addData(newEntry);

                //Increment counters
                result.incrementTraversalCounter();
                result.incrementMovedCounter(); //TODO: Change this when implementing conditionals
            }

            return result;
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
            writeCache();
        }

        #region Parsing Methods
        private void parseRefStr()
        {
            throw new NotImplementedException();
        }

        private void parseUrlString()
        {
            throw new NotImplementedException();
        }
        #endregion

        public string getReadRange()
        {
            //TODO: Check to make sure table name is valid
            return tableName + "!A:Z";
        }

        public string buildReadRequestURL()
        {
            //TODO: Check sheetId, readrange and apikey for validity
            return @"https://sheets.googleapis.com/v4/spreadsheets/" + sheetId + @"/values/" + getReadRange() + @"?key=" + apiKey;
        }

        public int getColumn(string c)
        {
            c = c.Trim();
            return getColumn(c[0]);
        }

        public int getColumn(char c)
        {
            return char.ToUpper(c) - 65; //startIndex == 0
        }

        public class SheetsData
        {
            public string range { get; set; }
            public string majorDimension { get; set; }
            public List<List<string>> values { get; set; }
        }
    }
}
