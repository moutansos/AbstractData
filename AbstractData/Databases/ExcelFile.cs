using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML;
using System.IO;

namespace AbstractData
{
    public class ExcelFile : IDatabase
    {
        //Constants
        public const string idInScript = "ExcelFile";

        private string tableName;
        private string fullFilePath;
        //IDEA: What if file paths could be downloadable from http/s?
        //      as long as it is not an assignment file. If it is then 
        //      add to the cached file and warn the user.

        private ClosedXML.Excel.XLWorkbook currentWorkbook;
        private ClosedXML.Excel.IXLWorksheet currentWorksheet;

        #region Constructors
        public ExcelFile(string filePath)
        {
            fileName = filePath;
        }
        #endregion

        #region Properties
        public dbType type
        {
            get { return dbType.ExcelFile; }
        }

        public bool isMultiTable
        {
            get { return true; }
        }
        
        public string table
        {
            get { return tableName; }
            set { tableName = value; } //TODO: Add validation that table is in this excel file
        }

        public string fileName
        {
            get { return fullFilePath; }
            set { fullFilePath = value; } //TODO: Add RegEx validation for windows and linux file paths that end in xlsx
                                          //In validation method, check for machine OS before checking the file path. Also
                                          //check to see if the file exists. After all this, run createFileIfNotExist()
        }
        #endregion

        public void addData(DataEntry data)
        {
            if(currentWorkbook == null)
            {
                currentWorkbook = new ClosedXML.Excel.XLWorkbook(fileName);
            }
            if(currentWorksheet == null || currentWorksheet.Name != table)
            {
                if (!currentWorkbook.Worksheets.TryGetWorksheet(table, out currentWorksheet))
                {
                    //If the sheet didn't exist then add a new sheet with that name
                    currentWorkbook.AddWorksheet(table);
                    currentWorksheet = currentWorkbook.Worksheet(table);
                }
            }
            else
            {
                ClosedXML.Excel.IXLRow currentRow = currentWorksheet.LastCellUsed().WorksheetRow().RowBelow();
                var dataFields = data.getFields();
                foreach(var field in dataFields)
                {
                    ClosedXML.Excel.IXLCell cell = currentRow.Cell(field.column);
                    cell.Value = field.data;
                }
            }
        }

        public void close()
        {
            if(currentWorkbook != null)
            {
                currentWorkbook.Save();
            }
        }

        public static void createFileIfNotExist(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                ClosedXML.Excel.XLWorkbook newWorkbook = new ClosedXML.Excel.XLWorkbook();
                newWorkbook.SaveAs(path);
            }
        }
    }
}
