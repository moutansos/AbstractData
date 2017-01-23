﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class moveCom : ILine
    {
        private string errorText;
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
        public bool hasError
        {
            get
            {
                if (errorText != null) return true;
                else return false;
            }
        }

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
                parseString();
            }
        }

        public Type type
        {
            get { return typeof(moveCom); }
        }
        #endregion

        public void parseString()
        {
            moveParams = StringUtils.returnStringInside(lineString, '(', ')');
        }

        public void execute(adScript script)
        {
            List<dataRef> currentDataRefs = script.currentDataRefs;
            List<movePackage> movePacks = new List<movePackage>();

            //Setup Move Packages
            foreach (dataRef data in currentDataRefs)
            {
                movePackage pack = matchRefInMovePack(movePacks, data);
                if (pack.isEmpty) //No package found. Make a new one
                {
                    pack = new movePackage {
                        isEmpty = false,
                        tableReference = data.tableReference,
                        references = new List<dataRef>()
                    };
                    pack.references.Add(data);
                    movePacks.Add(pack);
                }
                else
                {
                    movePacks.Remove(pack); //Remove from collection
                    pack.references.Add(data); //Change data
                    movePacks.Add(pack); //Add it back to collection
                }
            }

            //Execute each movePack
            foreach(var pack in movePacks)
            {
                tableRef tRef = pack.tableReference;
                tRef.readDatabase.table = tRef.readTableText;
                tRef.writeDatabase.table = tRef.writeTableText;
                tRef.readDatabase.getData(tRef.writeDatabase.addData, pack.references);
                tRef.readDatabase.close();
                tRef.writeDatabase.close();
            }
        }

        public string generateString()
        {
            lineString = "move(" + moveParams + ")";
            return lineString;
        }

        public static bool isMoveCom(string line)
        {
            if (line.StartsWith("move"))
            {
                return true;
            }
            return false;
        }

        private static movePackage matchRefInMovePack(List<movePackage> packs, dataRef dataReference)
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
