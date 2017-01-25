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
        private string fileName;
        private int numberOfColumns;

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
            throw new NotImplementedException();
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
    }
}
