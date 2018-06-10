namespace HASMLib.SyntaxTokens
{
    public enum ParseErrorType
    {
        Constant_WrongFormat,
        Constant_TooLong,
        Constant_BaseOverflow,
        Constant_UnknownConstantLengthQualifier,
        Instruction_WrongParameterCount,
        Instruction_UnknownInstruction,
        Other_OutOfFlash
    }
}