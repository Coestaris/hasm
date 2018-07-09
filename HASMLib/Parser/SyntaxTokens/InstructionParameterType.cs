using System;

namespace HASMLib.Parser.SyntaxTokens
{
    [Flags]
    public enum InstructionParameterType
    {
        Register = 1,
        Constant = 2,
        Expression = 4,
    }
}
