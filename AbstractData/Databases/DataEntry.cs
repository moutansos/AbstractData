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
                    else if(getField(field) == null)
                    {
                        continue;
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

        #region CSVFile Methods
        public void csvFormatData()
        {
            List<Field> newFields = new List<Field>();
            foreach (Field field in fields)
            {
                if (field.data.Contains(","))
                {
                    field.data = "\"" + field.data + "\"";
                }
                newFields.Add(field);
            }
        }
        #endregion

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

            #region Conversion Properties
            public DateTime dataAsDate
            {
                get { return DateTime.Parse(dataString); }
            }

            public int dataAsInt
            {
                get { return int.Parse(dataString); }
            }

            public double dataAsDouble
            {
                get { return double.Parse(dataString); }
            }

            public float dataAsFloat
            {
                get { return float.Parse(dataString); }
            }

            public bool dataAsBool
            {
                get { return bool.Parse(dataString); }
            }

            public decimal dataAsDecimal
            {
                get { return decimal.Parse(dataString); }
            }

            public Guid dataAsGuid
            {
                get { return Guid.Parse(dataString); }
            }
            #endregion
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
