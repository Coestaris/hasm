namespace HASMLib.Parser
{
    public enum ParseErrorType
    {
        Constant_WrongFormat,
        Constant_TooLong,
        Constant_BaseOverflow,
        Constant_UnknownConstantLengthQualifier,
        Instruction_WrongParameterCount,
        Instruction_UnknownInstruction,
        Other_OutOfFlash,
		Other_UnknownError,
		Syntax_AmbiguityBetweenVarAndConst,
		Syntax_UnknownVariableName,
		Syntax_ExpectedVar,
		Syntax_ExpectedСonst
    }
}