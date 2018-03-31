using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class moveCom : ILine
    {
        private string lineString;
        private int line;
        private string moveParams;

        #region Constructor
        public moveCom(string line)
        {
            originalString = line;
        }
        #endregion

        #region Properties
        public int lineNumber
        {
            get { return line; }
            set
            {
                if (value > 0)
                {
                    line = value;
                }
                else
                {
                    line = 0;
                }
            }
        }

        public string originalString
        {
            get
            {
                if (lineString == null)
                {
                    generateString();
                }
                return lineString;
            }
            set
            {
                lineString = value;
            }
        }

        public Type type
        {
            get { return typeof(moveCom); }
        }
        #endregion

        public void parseString(adScript script, ref adScript.Output output)
        {
            moveParams = StringUtils.returnStringInside(lineString, '(', ')');

            //TODO: Add RegEx Validation
            output = null;
        }

        public void execute(adScript script, ref adScript.Output output)
        {
            Dictionary<tableRef, List<dataRef>> moveSchema = new Dictionary<tableRef, List<dataRef>>();

            //Setup the schema that defines what data is moved where
            foreach(KeyValuePair<string, dataRef> kv in script.currentDataRefs)
            {
                dataRef dataReference = kv.Value;
                if(!moveSchema.ContainsKey(dataReference.tableReference))
                {
                    moveSchema[dataReference.tableReference] = new List<dataRef>();
                }

                moveSchema[dataReference.tableReference].Add(dataReference);
            }

            //Execute each movePack
            foreach(KeyValuePair<tableRef, List<dataRef>> kv in moveSchema)
            {
                tableRef tRef = kv.Key;
                List<dataRef> dataRefs = kv.Value;

                tRef.ReadDatabase.table = tRef.readTableText;
                tRef.WriteDatabase.table = tRef.writeTableText;
                moveResult result = tRef.ReadDatabase.getData(tRef.WriteDatabase.addData, dataRefs, script, ref output);
                tRef.ReadDatabase.close();
                tRef.WriteDatabase.close();
                script.output?.Invoke(result.resultText);
            }

            //Check for errors
            output = null;
        }

        public string generateString()
        {
            lineString = "move(" + moveParams + ")";
            return lineString;
        }

        public static bool IsMoveCom(string line)
        {
            if (line.StartsWith("move"))
            {
                return true;
            }
            return false;
        }

        private static movePackage MatchRefInMovePack(List<movePackage> packs, dataRef dataReference)
        {
            foreach(var pack in packs)
            {
                if(dataReference.tableReference == pack.tableReference)
                {
                    return pack;
                }
            }

            return new movePackage { isEmpty = true };
        }

        public struct movePackage
        {
            public bool isEmpty;
            public tableRef tableReference;
            public List<dataRef> references;
        }
    }
}
