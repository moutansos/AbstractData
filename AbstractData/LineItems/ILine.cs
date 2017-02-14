using System;

namespace AbstractData
{
    public interface ILine
    {
        void execute(adScript script);
        void parseString();
        string generateString();

        Type type { get; }
        int lineNumber { get; set; }
        string originalString { get; set; }
        bool hasError { get; }
    }
}
