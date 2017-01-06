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
        bool isMultiTable { get; }
        string table { get; set; }
        void addData(DataEntry data);
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
    }
}
