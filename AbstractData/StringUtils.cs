using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class StringUtils
    {
        public static string returnStringInside(string masterString, char start, char end)
        {
            //TODO: Rewrite to make end iterate from the end of the string and return the inside
            //      This will elimenate issue with giving the value inside quotes and also resove 
            //      issues with matching on characters inside the quotes.

            int startPos = -1;
            int endPos = -1;
            for (int i = 0; i < masterString.Length; i++)
            {
                if (startPos == -1 &&
                    endPos == -1 &&
                    masterString[i] == start)
                {
                    startPos = i;
                }

                if (endPos == -1 &&
                   startPos != -1 &&
                   masterString[i] == end)
                {
                    endPos = i;
                }
            }

            if (startPos == -1 &&
               endPos == -1)
            {
                return null;
            }
            else
            {
                return masterString.Substring(startPos + 1, endPos - startPos - 1);
            }

        }

        public static string returnAllButInside(string masterString, char start, char end)
        {
            //Replace escaped quotes
            while (masterString.Contains("\\\""))
            {
                masterString = masterString.Replace("\\\"", "  ");
            }

            string newString = "";
            bool spaceChars = false;
            for(int i = 0; i < masterString.Length; i++)
            {
                if (spaceChars == false &&
                    masterString[i] == start)
                {
                    newString = newString + masterString[i];
                    spaceChars = true;
                }
                else if(spaceChars == true &&
                          masterString[i] != end)
                {
                    newString = newString + ' ';
                }
                else if(spaceChars == true &&
                          masterString[i] == end)
                {
                    newString = newString + masterString[i];
                    spaceChars = false;
                }
                else
                {
                    newString = newString + masterString[i];
                }
            }

            return newString;

        }

        public static List<string> splitOnAllButInside(string masterString, char charToSplitOn, char startOfExlusion, char endOfExclusion)
        {
            List<string> returnSplit = new List<string>();
            string cleanMasterString = returnAllButInside(masterString, startOfExlusion, endOfExclusion);
            int numberOfSplitChars = cleanMasterString.Where(a => a == charToSplitOn).Count(); //Get the total number of split characters
            int numberOfTerms = numberOfSplitChars + 1;
            int indexOfPreviousSplitVar = 0;
            for (int i = 0; i < numberOfTerms; i++)
            {
                int indexOfNextSplitVar = cleanMasterString.IndexOf(charToSplitOn, indexOfPreviousSplitVar);
                if(indexOfNextSplitVar == -1)
                {
                    returnSplit.Add(masterString.Substring(indexOfPreviousSplitVar, masterString.Length - indexOfPreviousSplitVar));
                }
                else
                {
                    string newString = masterString.Substring(indexOfPreviousSplitVar, indexOfNextSplitVar - indexOfPreviousSplitVar);
                    returnSplit.Add(newString);
                    indexOfPreviousSplitVar = indexOfNextSplitVar + 1;
                }
            }
            return returnSplit;
        }

        public class constructorVals
        {
            private Dictionary<string, reference> vals;

            public constructorVals()
            {
                vals = new Dictionary<string, reference>();
            }

            public void addVal(string value, string textFromScript)
            {
                addVal(value, new reference(textFromScript));
            }

            public void addVal(string value, reference dataref)
            {
                vals.Add(value, dataref); 
            }

            public void removeVal(string value)
            {
                vals.Remove(value);
            }

            public string evaluate(string value)
            {
                reference data = vals[value];
                return null;
            }
        }
    }
}
