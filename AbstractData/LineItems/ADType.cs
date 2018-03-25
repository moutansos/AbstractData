using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class ADType
    {
        Type type;
        bool isList;

        string text;

        public ADType(string text)
        {
            this.text = text;
        }

        public void Parse()
        {
            string clean = text.Trim().ToLower();
            if(clean.Contains("list") && clean.Contains('<') && clean.Contains('>'))
            {
                isList = true;
                int indexOfOpen = clean.IndexOf('<');
                int indexOfClose = clean.IndexOf('>');
                clean = clean.Substring(indexOfOpen + 1, indexOfClose - (indexOfOpen + 1));
            }

            type = ParseType(clean);
        }

        public Type NativeType => type;
        public bool IsList => isList;

        private static Type ParseType(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "string":
                case "text":
                    return typeof(string);
                case "int":
                case "integer":
                    return typeof(int);
                case "float":
                    return typeof(float);
                case "decimal":
                    return typeof(decimal);
                case "uint":
                    return typeof(uint);
                case "bool":
                case "boolean":
                    return typeof(bool);
                default:
                    throw new InvalidTypeException("The data type \"" + input + "\" was unrecognized");
            }
        }

    }
}
