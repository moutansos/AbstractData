using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Linq;
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
        private string tableRange;

        private reference sheetIdRef;
        private string sheetId; //The id for the spreadsheet
        private reference clientSecretFile;
        private string secretFile;
        private reference clientCredentialFile;
        private string credFile;
        private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private const string ApplicationName = "AbstractData";

        private List<DataEntry> dataEntryCache;

        #region Constructors
        public GoogleSheets(reference sheetIdRef, reference credPath, reference secretPath)
        {
            dataEntryCache = new List<DataEntry>();
            clientSecretFile = secretPath;
            clientCredentialFile = credPath;
            this.sheetIdRef = sheetIdRef;
        }
        #endregion

        #region Properites
        public string id
        {
            get { return ID; }
            set { ID = value; }
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

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            evalInputRefs(script);
            List<string> readColumns = dataRef.getColumnsForRefs(dRefs);
            moveResult result = new moveResult();

            UserCredential credential;

            using (var stream = new FileStream(secretFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credFile, true)).Result;
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
                    addData(newEntry, script);

                    //Increment counters
                    result.incrementTraversalCounter();
                    result.incrementMovedCounter(); //TODO: Change this when implementing conditionals
                }
            }
            return result;
        }

        public void addData(DataEntry data,
                            adScript script)
        {
            if(sheetId == null || credFile == null || secretFile == null)
            {
                evalInputRefs(script);
            }

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
                moveResult result = new moveResult();

                UserCredential credential;

                using (var stream = new FileStream(secretFile, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credFile, true)).Result;
                    Console.WriteLine("Credential file saved to: " + clientCredentialFile);
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define request parameters.
                ValueRange data = convertToValueRange(dataEntryCache);
                SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(data, sheetId, data.Range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                AppendValuesResponse response = request.Execute();
                dataEntryCache.Clear();
            }
        }

        private ValueRange convertToValueRange(IEnumerable<DataEntry> entries)
        {
            //Unfinished. Does not take into account the actual reference position. 
            ValueRange newRange = new ValueRange();
            char firstColumn = '\0';
            char lastColumn = '\0';
            IList<IList<object>> newData = new List<IList<object>>();
            foreach(DataEntry entry in entries)
            {
                List<DataEntry.Field> fields = entry.getFields();
                var sortedFields = fields.OrderBy(x => x.column).ToList();
                DataEntry.Field last = sortedFields.Last();
                if(last != null)
                {
                    int lastIndex = convertColumnToIndex(last.column);
                    object[] newAra = new object[lastIndex + 1];
                    foreach (DataEntry.Field field in fields)
                    {
                        int fieldIndex = convertColumnToIndex(field.column);
                        newAra[fieldIndex] = field.data;
                        
                    }
                    IList<object> newList = newAra.ToList();
                    newData.Add(newList);

                    //Find the first and last column char
                    char entryFirstCol = sortedFields.First().column[0];
                    char entryLastCol = last.column[0];
                    if(firstColumn != '\0' && entryFirstCol < firstColumn)
                    {
                        firstColumn = entryFirstCol;
                    }
                    if (lastColumn != '\0' && entryLastCol > lastColumn)
                    {
                        lastColumn = entryLastCol;
                    }
                }
            }
            /*
            if(firstColumn != '\0' && lastColumn != '\0')
            {
                newRange.Range = tableRange + "!" + firstColumn + ":" + lastColumn;
            }
            else
            {
                newRange.Range = tableRange;
            }*/

            //TODO: Start on the last used row like the other spreadsheets
            newRange.Range = tableRange + "!A:A";

            newRange.Values = newData;
            return newRange;
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
                sum += (columnName[i] - 'A');
            }

            return sum;
        }

        private void evalInputRefs(adScript script)
        {
            adScript.Output output = null;
            secretFile = clientSecretFile.evalReference(null, script, ref output);
            credFile = clientCredentialFile.evalReference(null, script, ref output);
            sheetId = sheetIdRef.evalReference(null, script, ref output);
            sheetId = parseSheetIdFromURL(sheetId);
        }

        private static string parseSheetIdFromURL(string input)
        {
            if(Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                Uri url = new Uri(input);
                string id = url.Segments[3];
                return id.TrimEnd('/');
            }
            return input;
        }
    }
}
