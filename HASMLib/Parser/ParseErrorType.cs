﻿namespace HASMLib.Parser
{
    public enum ParseErrorType
    {
        Syntax_Expression_UnclosedBracket,
        Syntax_Expression_UnknownFunction,
        Syntax_Expression_UnknownOperator,
        Syntax_Expression_CantParse,
        Syntax_Expression_WrongOperatorCount,
        Syntax_Constant_WrongFormat,
        Syntax_Constant_TooLong,
        Syntax_Constant_BaseOverflow,
        Syntax_Constant_UnknownConstantLengthQualifier,
        Syntax_Instruction_WrongParameterCount,
        Syntax_Instruction_UnknownInstruction,
        Other_OutOfFlash,
        Other_UnknownError,
        Syntax_AmbiguityBetweenVarAndConst,
        Syntax_UnknownVariableName,
        Syntax_UnknownConstName,
        Syntax_ExpectedVar,
        Syntax_ExpectedСonst,
        Syntax_UnExpectedToken,
        Preprocessor_UnexpectedPreprocessorLine,
        Preprocessor_WrongPreprocessorFormat,
        IO_UnabletoFindSpecifiedFile,
        IO_UnabletoFindSpecifiedWorkingDirectory,
        Preprocessor_UnknownDirective,
        Preprocessor_WrongParameterCount,
        Preprocessor_NameExpected,
        Preprocessor_WrongParametricDefineFormat,
        Preprocessor_ParametricDefineWithoutExpression,
        Preprocessor_WrongDefineName,
        Preprocessor_ElseWithoutPreviousConditionals,
        Preprocessor_EndifWithoutPreviousConditionals,
        Preprocessor_DefineNameAlreadyExists,
        Preprocessor_UnknownDefineName,
        Syntax_Expression_NotAllowToUseDefinedFunction,
        Preprocessor_ReferenceToEmptyDefine,
        Preprocessor_WrongNameFormat,
        Preprocessor_UserDefinedError,
        Syntax_Expression_UnknownToken,
        Syntax_Constant_WrongType,
    }
}