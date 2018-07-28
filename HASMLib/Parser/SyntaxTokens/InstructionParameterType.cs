using System;

namespace HASMLib.Parser.SyntaxTokens
{
    [Flags]
    public enum InstructionParameterType
    {
        Variable = 1,
        Constant = 2,
        Expression = 4,
        ClassName = 8,
        NewVariable = 16,
        FunctionName = 32,
        FieldName = 64,
    }
}
