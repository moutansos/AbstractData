using System;

namespace AbstractData
{
    public interface ILine
    {
        void execute(adScript script, ref adScript.Output output);
        void parseString(ref adScript.Output output);
        string generateString();

        Type type { get; }
        int lineNumber { get; set; }
        string originalString { get; set; }
    }
}
