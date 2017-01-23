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

        public void convertToWriteEntry(List<dataRef> dRefs)
        {
            List<Field> newFields = new List<Field>();
            foreach(dataRef dRef in dRefs)
            {
                string dataVal = "";
                foreach(string field in dRef.readReference.allFields)
                {
                    if(field.StartsWith("\"") && field.EndsWith("\""))
                    {
                        dataVal = dataVal + field.TrimStart('\"').TrimEnd('\"');
                    }
                    else
                    {
                        dataVal = dataVal + getField(field).data;
                    }
                }
                Field newField = new Field(dRef.writeAssignmentText, dataVal);
                newFields.Add(newField);
            }
            fields = newFields;
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

        public Field getField(string columnName)
        {
            foreach(Field field in fields)
            {
                if(field.column == columnName)
                {
                    return field;
                }
            }

            return null;
        }
    }
}
