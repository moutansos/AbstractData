﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class CSVFile : IDatabase
    {
        public const string idInScript = "CSVFile";
        private const int cacheLimit = 5000;
        private reference fileName;
        private string file;
        private string ID;

        private List<string> dataCache;

        #region Constructors
        public CSVFile(reference fileName)
        {
            this.fileName = fileName;
            dataCache = new List<string>();
        }
        #endregion

        #region Properties
        public string id
        {
            get { return ID; }
            set { ID = value; }
        }

        public bool isMultiTable
        {
            get { return false; }
        }

        public string refString
        {
            get { return fileName.originalString; }
        }

        public string table
        {
            get
            {
                return null;
            }

            set
            {
                //PASS
            }
        }

        public dbType type
        {
            get { return dbType.CSVFile; }
        }
        #endregion

        public void addData(DataEntry data,
                            adScript script)
        {
            data.csvFormatData();
            int maxColumn = getBiggestFieldOrdinal(data);
            string[] dataVals = new string[maxColumn + 1];
            foreach(DataEntry.Field field in data.getFields())
            {
                int ordinal = int.Parse(field.column);
                dataVals[ordinal] = field.data;
            }
            string CSVLine = "";
            for(int i = 0; i < dataVals.Length; i++)
            {
                if (i == 0)
                {
                    CSVLine = dataVals[i];
                }
                else
                {
                    CSVLine = CSVLine + "," + dataVals[i];
                }
            }
            dataCache.Add(CSVLine);

            if(file == null)
            {
                evalFileName(script);
            }

            if(dataCache.Count > cacheLimit)
            {
                writeCache();
            }
            
        }

        public void writeCache()
        {
            if(dataCache.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(file, true))
                {
                    foreach (string line in dataCache)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Close();
                }

                dataCache.Clear();
            }
        }

        public moveResult getData(Action<DataEntry, adScript> addData, 
                                  List<dataRef> dRefs,
                                  adScript script,
                                  ref adScript.Output output)
        {
            moveResult result = new moveResult();
            evalFileName(script);

            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    DataEntry newEntry = new DataEntry();
                    string[] fields = parser.ReadFields();
                    for(int i = 0; i < fields.Length; i++)
                    {
                        newEntry.addField(i.ToString(), fields[i]);
                    }
                    newEntry.convertToWriteEntry(dRefs, script, ref output);
                    addData(newEntry, script);

                    //Increment counters
                    result.incrementTraversalCounter();
                    result.incrementMovedCounter(); //TODO: Change this when implementing conditionals
                }
            }

            return result;
        }

        public void close()
        {
            writeCache();
        }

        private int getBiggestFieldOrdinal(DataEntry data)
        {
            int biggestNum = 0;
            foreach(DataEntry.Field field in data.getFields())
            {
                int parseField = -1;
                if(!int.TryParse(field.column, out parseField))
                {
                    throw new ArgumentException("The database is CSV, but the field name isn't a number.");
                }

                if(parseField > biggestNum)
                {
                    biggestNum = parseField;
                }
            }
            return biggestNum;
        }

        private void evalFileName(adScript script)
        {
            adScript.Output output = null;
            file = fileName.evalReference(null, script, ref output);
        }
    }
}
