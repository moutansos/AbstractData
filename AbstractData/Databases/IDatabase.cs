using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public interface IDatabase
    {
        dbType type { get; }
        string id { get; set; }
        bool isMultiTable { get; }
        string table { get; set; }
        string refString { get; }
        void addData(DataEntry data,
                     adScript script);
        moveResult getData(Action<DataEntry, adScript> addData, 
                           List<dataRef> dRefs,
                           adScript script,
                           ref adScript.Output output);
        void close();
    }

    public enum dbType
    {
        ExcelFile,
        CSVFile,
        GoogleSheets,
        AccessDB,
        SQLServerDB,
        PostgreSqlDB,
        MariaDB,
        SQLiteDB,
        Unknown
    }
}
