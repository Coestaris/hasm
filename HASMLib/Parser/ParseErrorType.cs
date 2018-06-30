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
		Syntax_UnknownConstName,
		Syntax_ExpectedVar,
		Syntax_ExpectedСonst,
		Syntax_UnExpectedToken,

        Expression_Parse_UnclosedBracket,
        Expression_Parse_UnknownFunction,
        Expression_Parse_UnknownOperator,
        Expression_Parse_CantParse,
        Expression_Parse_WrongOperatorCount
    }
}