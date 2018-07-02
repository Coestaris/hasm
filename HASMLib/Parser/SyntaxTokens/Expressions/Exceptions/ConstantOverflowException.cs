using System;

namespace HASMLib.Parser.SyntaxTokens.Expressions.Exceptions
{
    internal class ConstantOverflowException : Exception
    {
        public ConstantOverflowException(string constnant, ParseErrorType type)
        {
            Constnant = constnant;
            Type = type;
        }

        public string Constnant { get; set; }
        public ParseErrorType Type { get; set; }

    }
}
