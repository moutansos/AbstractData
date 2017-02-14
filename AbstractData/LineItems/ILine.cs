using System;

namespace AbstractData
{
    public interface ILine
    {
        void execute(adScript script, ref string output);
        void parseString(ref string output);
        string generateString();

        Type type { get; }
        int lineNumber { get; set; }
        string originalString { get; set; }
        bool hasError { get; }
    }
}
