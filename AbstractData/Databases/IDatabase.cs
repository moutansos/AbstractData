using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public interface IDatabase
    {
        dbType getType();
        bool isMultiTable();
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
