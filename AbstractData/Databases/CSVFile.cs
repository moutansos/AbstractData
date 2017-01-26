using Microsoft.VisualBasic.FileIO;
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
        private string fileName;

        #region Constructors
        public CSVFile(string fileName)
        {
            this.fileName = fileName;
        }
        #endregion

        #region Properties
        public string id
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isMultiTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string refString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string table
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public dbType type
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public void addData(DataEntry data)
        {
            data.csvFormatData();
            int maxColumn = getBiggestFieldOrdinal(data);
            string[] dataVals = new string[maxColumn];
            foreach(DataEntry.Field field in data.getFields())
            {
                int ordinal = int.Parse(field.data);
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
                    CSVLine = CSVLine + "," + dataVals;
                }
            }

            using(TextWriter writer = new StreamWriter(fileName, true))
            {
                writer.WriteLine(CSVLine);
                writer.Close();
            }
            
        }

        public void getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
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
                    newEntry.convertToWriteEntry(dRefs);
                    addData(newEntry);
                }
            }
        }

        public void close()
        {
            throw new NotImplementedException();
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
    }
}
