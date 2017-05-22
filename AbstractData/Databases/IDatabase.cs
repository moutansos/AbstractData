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
        string table { get; set; }

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
        AccessDB,
        CSVFile,
        ExcelFile,
        GoogleSheets,
        MariaDB,
        MongoDB,
        PostgreSqlDB,
        SQLiteDB,
        SQLServerDB,
        Unknown
    }
}
