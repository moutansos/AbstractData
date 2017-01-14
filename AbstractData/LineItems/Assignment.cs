using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData.LineItems
{
    class Assignment : ILine
    {
        public bool hasError
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int lineNumber
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

        public string originalString
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

        public Type type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void execute(adScript script)
        {
            throw new NotImplementedException();
        }

        public string generateString()
        {
            throw new NotImplementedException();
        }

        public void parseString()
        {
            throw new NotImplementedException();
        }
    }
}
