using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class reference
    {
        //TODO: Add query functionality
        private dbRef db;
        private string lineSubString;
        private List<string> fields;

        #region Constructors
        public reference(string refString)
        {
            fields = new List<string>();
            originalString = refString;
        }
        #endregion

        #region Properites
        public string originalString
        {
            get { return lineSubString; }
            set
            {
                lineSubString = value;
                parseString();
            }
        }
        #endregion

        public void parseString()
        { //TODO: Implement with return all but inside method to avoid things within the strings.
            if (lineSubString.Contains("+"))
            {
                string[] splitItems = lineSubString.Split('+');
                for(int i = 0; i < splitItems.Length; i++)
                {
                    string item = splitItems[i];
                    item = item.Trim();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        fields.Add(item);
                    }
                }
            }
            else
            {
                fields.Add(lineSubString.Trim());
            }
        }

    }
}
