using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AbstractData
{
    public class GoogleSheets : IDatabase
    {
        public const string idInScript = "GoogleSheets";
        private const int cacheLimit = 5000;

        private string ID;
        private string refStr;
        private string tableRange;

        private string sheetUrl; //The url of the sheet
        private string sheetId; //The id for the spreadsheet
        private UserCredential credential;
        private string clientSecretFile;
        private string clientCredentialFile;
        private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private const string ApplicationName = "AbstractData";

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
            get { return tableRange; }
            set
            {
                writeCache();
                tableRange = value;
            }
        }

        public dbType type
        {
            get { return dbType.GoogleSheets; }
        }
        #endregion

        public moveResult getData(Action<DataEntry> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            UserCredential credential;

            using (var stream =
                new FileStream(clientSecretFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(clientCredentialFile, true)).Result;
                Console.WriteLine("Credential file saved to: " + clientCredentialFile);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(sheetId, tableRange);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0) //If there is any data to read
            {
                foreach (var row in values)
                {
                    DataEntry newEntry = new DataEntry();
                    foreach(var column in readColumns)
                    {
                        string dataToGet = "";
                        try
                        {
                            dataToGet = row[convertColumnToIndex(column)].ToString();
                        }
                        catch(IndexOutOfRangeException) // If the value doesn't exist then leave it blank
                        {
                            //PASS
                        }
                        newEntry.addField(column, dataToGet);
                    }
                    //Add the entry to the database
                    newEntry.convertToWriteEntry(dRefs, script, ref output);
                    addData(newEntry);

                    //Increment counters
                    result.incrementTraversalCounter();
                    result.incrementMovedCounter(); //TODO: Change this when implementing conditionals
                }
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
            if(dataEntryCache.Count > 0)
            {
                throw new NotImplementedException();
            }
        }

        public void close()
        {
            writeCache();
        }

        private static int convertColumnToIndex(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("The column name is invalid");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }
    }
}
