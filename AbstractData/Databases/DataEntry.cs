using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class DataEntry
    {
        private List<Field> fields = new List<Field>();

        public void addField(string column, string data)
        {
            Field newField = new Field(column, data);
            fields.Add(newField);
        }

        public IEnumerable<Field> getFields()
        {
            return fields;
        }

        public class Field
        {
            private string columnName;
            private string dataString;

            public Field(string column, string data)
            {
                this.column = column;
                this.data = data;
            }

            public string column
            {
                get { return columnName; }
                set { columnName = value; }
            }

            public string data
            {
                get { return dataString; }
                set { dataString = value; }
            }
        }
    }
}
