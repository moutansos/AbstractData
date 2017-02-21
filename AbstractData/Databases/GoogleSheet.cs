using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    class GoogleSheet : IDatabase
    {
        #region Properites
        public string id
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isMultiTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string refString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string table
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public dbType type
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public void addData(DataEntry data)
        {
            throw new NotImplementedException();
        }

        public void close()
        {
            throw new NotImplementedException();
        }

        public moveResult getData(Action<DataEntry> addData, List<dataRef> dRefs)
        {
            throw new NotImplementedException();
        }
    }
}
