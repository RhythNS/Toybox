using System;
namespace Modularity.Scene
{
    public class ParserException : Exception
    {
        public ParserException(int lineNumber, string message) : base("Error on line " + lineNumber + ": " + message)
        {
        }
    }
}
