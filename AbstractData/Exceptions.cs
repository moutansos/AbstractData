using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class UnknownDatabaseTypeException : Exception
    {
        public UnknownDatabaseTypeException()
        {
        }

        public UnknownDatabaseTypeException(string message)
            : base(message)
        {
        }

        public UnknownDatabaseTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class adLibException : Exception
    {
        public adLibException()
        {
        }

        public adLibException(string message)
            : base(message)
        {
        }

        public adLibException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DataNotFoundException : Exception
    {
        public DataNotFoundException()
        {
        }

        public DataNotFoundException(string message)
            : base(message)
        {
        }

        public DataNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class InvalidTypeException : Exception
    {
        public InvalidTypeException()
        {
        }

        public InvalidTypeException(string message)
            : base(message)
        {
        }

        public InvalidTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
