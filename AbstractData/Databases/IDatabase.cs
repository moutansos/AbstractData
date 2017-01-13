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
        void addData(DataEntry data);
        void getData(Func<DataEntry> addData, List<string> columns);
        void close();
    }

    public enum dbType
    {
        ExcelFile,
        CSVFile,
        AccessDB,
        SQLServerDB,
        PostgreSqlDB,
        MariaDB,
        SQLiteDB,
        Unknown
    }
}
