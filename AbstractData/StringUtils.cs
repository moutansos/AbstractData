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
        { //TODO: Finish this
            string returnString = "";
            bool replaceInside = false;
            for (int i = 0; i < masterString.Length; i++)
            {
                if (!replaceInside && masterString[i] == start)
                {
                    replaceInside = true;
                }
                else if (replaceInside && masterString[i] == end)
                {

                }
            }
            return "";
        }
    }
}
