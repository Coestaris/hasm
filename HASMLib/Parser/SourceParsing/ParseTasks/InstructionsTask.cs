using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using HASMLib.Runtime.Instructions;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class InstructionsTask : ParseTask
    {
        public override string Name => "Parsing instruction";

        protected override void InnerReset() { }
        
        private ParseError NewParseError(ParseErrorType error, SourceLineInstruction line, int argIndex)
        {
            return new ParseError(
                error,
                line.LineIndex,
                line.Label.Length + 2 + line.Parameters.Take(argIndex).Sum(p => p.Length),
                line.FileName);
        }

        private FlashElementConstant RegisterConstant(Runtime.Structures.Units.Function function, string name, ulong value)
        {
            if (function.CompileCache.UnknownLabelNameErrorList.Exists(p => p.Name == name))
            {
                var info = function.CompileCache.UnknownLabelNameErrorList.Find(p => p.Name == name);

                info.namedConstant.Constant = new Constant(value);
                info.memZoneFlashElementConstant.UpdateValue((Integer)value, info.ConstIndex);
                return null;
            }

            int constIndex = ++function.CompileCache.ConstIndex;

            var constant = new FlashElementConstant((Integer)value, (Integer)constIndex);
            function.CompileCache.NamedConsts.Add(new ConstantMark(name, (Integer)constIndex, new Constant(value))
            {
                FEReference = constant
            });

            return constant;
        }

        private List<FlashElement> ProceedInstruction(Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            if (line.Parameters == null) return GetFlashElementsNoArguents(function, line, out error);
            else return GetFlashElementsWithArguents(function, line, out error);
        }

        private List<FlashElement> GetFlashElementsNoArguents(Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)function.CompileCache.InstructionIndex++;

            var result = new List<FlashElement>();
            error = null;

            if (line.Instruction.ParameterCount != 0)
            {
                error = new ParseError(
                    ParseErrorType.Syntax_Instruction_WrongParameterCount,
                    line.LineIndex,
                    (line.Comment != null ? line.Comment.Length : 0) + line.Instruction.NameString.Length,
                    line.FileName);
                return null;
            }

            if (!string.IsNullOrEmpty(line.Label))
            {
                RegisterConstant(function, line.Label, (ulong)currentInstructionProgramIndex);
            }

            result.Add(new FlashElementInstruction(line.Instruction, null, currentInstructionProgramIndex));
            return result;
        }

        private List<FlashElement> GetFlashElementsWithArguents(Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)(function.CompileCache.InstructionIndex++);
            var result = new List<FlashElement>();

            if (line.Parameters.Length != line.Instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Syntax_Instruction_WrongParameterCount,
                    line.LineIndex,
                    line.Comment.Length + line.Instruction.NameString.Length,
                    line.FileName);
                return null;
            }

            if (!string.IsNullOrEmpty(line.Label))
            {
                RegisterConstant(function, line.Label, (ulong)currentInstructionProgramIndex);
            }

            int argIndex = -1;
            var usedIndexes = new List<ObjectReference>();

            foreach (var argument in line.Parameters)
            {
                argIndex++;
                
                //Попытка пропарсить константу
                ParseError constError = Constant.Parse(argument, out Constant constant);

                //Попытка пропарсить выражение
                ParseError expressionError = expressionError = Expression.Parse(argument, out Expression expression);

                //Грубое определние типа нашего аргумента
                bool isConst = constant != null;
                bool isVar = function.CompileCache.Variables.Select(p => p.Name).Contains(argument);

                //Если допустимо выражение и если это не просто выражение
                if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression) && expressionError == null &&
                    (!expression.TokenTree.IsSimple || expression.TokenTree.UnaryFunction != null || expression.TokenTree.UnaryOperator != null))
                {
                    try
                    {
                        Expression.Precompile(expression.TokenTree,
                            (token) =>
                            {

                                var variable = function.CompileCache.Variables.Find(p => p.Name == token.RawValue);
                                if (variable != null)
                                {

                                    int varIndex = function.CompileCache.Variables.Select(p => p.Name).ToList().IndexOf(token.RawValue);
                                    return new ObjectReference((Integer)varIndex, ReferenceType.Variable)
                                    {
                                        Object = null
                                    };

                                }

                                //То, возможно, это именная константа...
                                if (function.CompileCache.NamedConsts.Select(p => p.Name).Contains(token.RawValue))
                                {
                                    //Получения индекса константы со списка
                                    int constantIndex = function.CompileCache.NamedConsts.Select(p => p.Name).ToList().IndexOf(token.RawValue);

                                    return new ObjectReference(function.CompileCache.NamedConsts[constantIndex].Index, ReferenceType.Constant)
                                    {
                                        Object = function.CompileCache.NamedConsts[constantIndex].FEReference
                                    };
                                }
                                else
                                {

                                    //Константа не найдена! Логично было бы выкинуть ошибку
                                    //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                    //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                    //И если она таки будет найдена, то подменим ее настоящим значением


                                    //Елси на эту неведомую херню уже ссылались, то сошлемся на нее же
                                    if (function.CompileCache.UnknownLabelNameErrorList.Exists(p => p.Name == token.RawValue))
                                    {
                                        var item = function.CompileCache.UnknownLabelNameErrorList.Find(p => p.Name == token.RawValue);
                                        //После такой "грязной" хуйни мне хочется сходить с душ!
                                        return new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant)
                                        {
                                            Object = function.CompileCache.UnknownLabelNameErrorList.Find(p => p.Name == token.RawValue).memZoneFlashElementConstant
                                        };
                                    }
                                    else
                                    {

                                        int constIndex = ++function.CompileCache.ConstIndex;
                                        FlashElementConstantDummy dummyConstant = new FlashElementConstantDummy((Integer)constIndex);
                                        ConstantMark dummyNamedConstant = new ConstantMark(token.RawValue, (Integer)constIndex, new Constant())
                                        {
                                            FEReference = dummyConstant
                                        };

                                        function.CompileCache.UnknownLabelNameErrorList.Add(new ConstantErrorMark(
                                            token.RawValue,
                                            NewParseError(ParseErrorType.Syntax_UnknownConstName, line, argIndex),
                                            (Integer)constIndex,
                                            dummyNamedConstant, dummyConstant));

                                        function.CompileCache.NamedConsts.Add(dummyNamedConstant);
                                        //Записываем его во флеш память
                                        result.Add(dummyConstant);

                                        return new ObjectReference((Integer)constIndex, ReferenceType.Constant)
                                        {
                                            Object = dummyConstant
                                        };
                                    }
                                }
                            }
                            /*,(c) =>
                            {
                                //Расчет нового индекса константы	
                                int constIndex = ++source._constIndex;
                                //Записываем его во флеш память
                                var flashElement = c.ToFlashElement((Integer)constIndex);

                                result.Add(flashElement);

                                return new ObjectReference((Integer)constIndex, ReferenceType.Constant)
                                {
                                    Object = flashElement
                                };

                            }*/);

                        Integer index = (Integer)(++function.CompileCache.ExpressionIndex);
                        result.Add(new FlashElementExpression(expression, index));
                        usedIndexes.Add(new ObjectReference(index, ReferenceType.Expression));
                    }
                    catch (ConstantOverflowException ex)
                    {
                        error = NewParseError(ex.Type, line, argIndex);
                        return null;
                    }
                }
                else
                {
                    if(line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.NewVariable)
                    {
                        function.CompileCache.VarIndex++;
                        function.CompileCache.Variables.Add(new VariableMark(argument) { Index = (Integer)function.CompileCache.VarIndex });
                        usedIndexes.Add(new ObjectReference((Integer)function.CompileCache.VarIndex, ReferenceType.Variable));
                        continue;
                    }

                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.ClassName)
                    {
                        TypeReference type = new TypeReference(argument, source.Assembly);
                        if (!type.CheckClassType(source.Assembly.AllClasses, source.Assembly))
                        {
                            error = NewParseError(ParseErrorType.Syntax_Instruction_UnknownType, line, argIndex);
                            return null;
                        }

                        //var _const = RegisterConstant(function, $"__classReference{type.UniqueID}_", (ulong)type.UniqueID);
                        //result.Add(_const);                        
                        usedIndexes.Add(new ObjectReference((Integer)type.UniqueID, ReferenceType.Type));
                        continue;
                    }

                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.FieldName)
                    {
                        var field = BaseStructure.GetInstance<Field>(argument, source.Assembly, function.BaseClass);

                        if (field == null)
                        {
                            error = NewParseError(ParseErrorType.Syntax_Instruction_UnknownFieldName,
                                line, argIndex);
                            return null;
                        }

                        //var _const = RegisterConstant(function, $"__fieldRference{field.UniqueID}_", (ulong)field.UniqueID);
                        //result.Add(_const);
                        usedIndexes.Add(new ObjectReference((Integer)field.UniqueID, ReferenceType.Field));
                        continue;
                    }

                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.FunctionName)
                    {
                        var func = BaseStructure.GetInstance<Runtime.Structures.Units.Function>(argument, source.Assembly,
                            function.BaseClass);

                        if (func == null)
                        {
                            error = NewParseError(ParseErrorType.Syntax_Instruction_UnknownFuncName,
                                line, argIndex);
                            return null;
                        }

                        //var _const = RegisterConstant(function, $"__funcRference{func.UniqueID}_", (ulong)func.UniqueID);
                        //result.Add(_const);
                        usedIndexes.Add(new ObjectReference((Integer)func.UniqueID, ReferenceType.Function));
                        continue;
                    }


                    //Если это не выражение, то просто разбираем его дальше по типам...
                    //Если допустимо и константа и переменная, то выходит неоднозначность
                    if (isVar && isConst &&
                        line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Variable) &&
                        line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                    {
                        error = NewParseError(ParseErrorType.Syntax_AmbiguityBetweenVarAndConst, line, argIndex);
                        return null;
                    }

                    //Если мы определили, что это может быть как переменная, так и константа, 
                    //то смотрим что от нас хочет инструкция
                    if (isVar && isConst)
                    {
                        //Если необходима коснтанта, а не переменная
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Variable))
                        {
                            //Запоминаем индекс константы
                            usedIndexes.Add(new ObjectReference((Integer)(++function.CompileCache.ConstIndex), ReferenceType.Constant));
                            //Записываем во флеш константу
                            result.Add(constant.ToFlashElement((Integer)function.CompileCache.ConstIndex));
                        };

                        //Если необходима переменная, а не константа
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Variable) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //Получаем индекс переменной со списка переменных
                            Integer varIndex = function.CompileCache.Variables.Find(p => p.Name == argument).Index;
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference(varIndex, ReferenceType.Variable));
                        }
                        continue;
                    }

                    //Если это однозначно константа, не переменная
                    if (isConst)
                    {
                        //А ожидалась не константа, то ошибка
                        if (!line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            error = NewParseError(ParseErrorType.Syntax_ExpectedVar, line, argIndex);
                            return null;
                        }

                        //Если константу он то пропарсил, но было переполение
                        if (constError != null && (
                            constError.Type == ParseErrorType.Syntax_Constant_BaseOverflow ||
                            constError.Type == ParseErrorType.Syntax_Constant_WrongType ||
                            constError.Type == ParseErrorType.Syntax_Constant_TooLong))
                        {
                            error = NewParseError(constError.Type, line, argIndex);
                            return null;
                        }

                        //Запоминаем индекс константы
                        usedIndexes.Add(new ObjectReference((Integer)(++function.CompileCache.ConstIndex), ReferenceType.Constant));
                        //Заносим константу во флеш
                        result.Add(constant.ToFlashElement((Integer)function.CompileCache.ConstIndex));
                        continue;
                    }

                    //Если это не константа... 
                        //Если это однозначно переменная...
                    if (isVar)
                    {
                        //А ожидалась не переменная, то ошибка
                        if (!line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Variable))
                        {
                            error = NewParseError(ParseErrorType.Syntax_ExpectedСonst, line, argIndex);
                            return null;
                        }

                        //Получаем индекс переменной со списка переменных 
                        Integer varIndex = function.CompileCache.Variables.Find(p => p.Name == argument).Index;
                        //Запоминаем индекс переменной
                        usedIndexes.Add(new ObjectReference(varIndex, ReferenceType.Variable));
                        continue;
                    }

                    //Если это не переменная, а просили константу, не переменную
                    if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                    {
                        //То, возможно, это именная константа...
                        if (function.CompileCache.NamedConsts.Select(p => p.Name).Contains(argument))
                        {
                            //Получения индекса константы со списка
                            int constantIndex = function.CompileCache.NamedConsts.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминания индекса
                            usedIndexes.Add(new ObjectReference(function.CompileCache.NamedConsts[constantIndex].Index, ReferenceType.Constant));
                            //Запись константы во флеш
                            result.Add(function.CompileCache.NamedConsts[constantIndex].Constant.ToFlashElement(function.CompileCache.NamedConsts[constantIndex].Index));
                        }
                        else
                        {

                            //Константа не найдена! Логично было бы выкинуть ошику
                            //Но жива еще надежда на то, что она будет объявлена чуть позже.
                            //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                            //И если она таки будет найдена, то подменим ее настоящим значением


                            //Елси на эту неведомую херню уже ссылались, то сослемся на нее же
                            if (function.CompileCache.UnknownLabelNameErrorList.Exists(p => p.Name == argument))
                            {
                                var item = function.CompileCache.UnknownLabelNameErrorList.Find(p => p.Name == argument);

                                //После такой "грязной" хуйни мне хочется сходить с душ!
                                usedIndexes.Add(new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant));
                            }
                            else
                            {

                                int constIndex = ++function.CompileCache.ConstIndex;
                                FlashElementConstantDummy dummyConstant = new FlashElementConstantDummy((Integer)constIndex);
                                ConstantMark dummyNamedConstant = new ConstantMark(argument, (Integer)constIndex, new Constant())
                                {
                                    FEReference = dummyConstant
                                };

                                function.CompileCache.UnknownLabelNameErrorList.Add(new ConstantErrorMark(
                                    argument,
                                    NewParseError(expressionError == null ? ParseErrorType.Syntax_UnknownConstName : expressionError.Type, line, argIndex),
                                    (Integer)constIndex,
                                    dummyNamedConstant, dummyConstant));

                                function.CompileCache.NamedConsts.Add(dummyNamedConstant);
                                //Записываем его во флеш память
                                result.Add(dummyConstant);

                                usedIndexes.Add(new ObjectReference((Integer)constIndex, ReferenceType.Constant));
                            }

                            //error = NewParseError (ParseErrorType.Syntax_UnknownConstName, label, stringParts, argIndex, index);
                        }
                        continue;
                    }

                    //Если удалось частично пропарсить константу, но были переполнения и тд...
                    if (constError.Type == ParseErrorType.Syntax_Constant_BaseOverflow ||
                        constError.Type == ParseErrorType.Syntax_Constant_WrongType ||
                        constError.Type == ParseErrorType.Syntax_Constant_TooLong)
                    {
                        //Вернуть новую ошибку с типо старой
                        error = NewParseError(constError.Type, line, argIndex);
                        return null;
                    }

                    //Если ничего не известно, то вернем что неизвестное имя переменной
                    if (expressionError != null && line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression))
                    {
                        error = NewParseError(expressionError.Type, line, argIndex);
                        return null;
                    }

                    error = NewParseError(ParseErrorType.Syntax_UnknownVariableName, line, argIndex);
                    return null;
                }
            }

            result.Add(new FlashElementInstruction(line.Instruction, usedIndexes, currentInstructionProgramIndex)
            {
                Line = line
            });
            error = null;
            return result;
        }

        private ParseError ParseFunction(Runtime.Structures.Units.Function function)
        {
            function.CompileCache.Compiled = new List<FlashElement>();
            for (int i = 0; i < function.RawLines.Count; i++)
            {
                SourceLineInstruction instruction = new SourceLineInstruction(
                    function.RawLines[i].Input,
                    function.RawLines[i].LineIndex,
                    function.RawLines[i].FileName);
                ParseError error = instruction.Parse();

                function.RawLines[i] = instruction;

                if (error != null)
                {
                    return error;
                }
            }

            //Разбираем логику типов и пр.
            foreach (SourceLineInstruction line in function.RawLines)
            {
                if (line.IsEmpty) continue;

                var res = ProceedInstruction(function, line, out ParseError parseError);
                if (parseError != null)
                {
                    return parseError;
                }

                function.CompileCache.Compiled.AddRange(res);
            }

            //Просматриваем все наши "отложенные" константы
            //Если среди них есть пустые, то бьем тревогу!
            foreach (var item in function.CompileCache.UnknownLabelNameErrorList)
            {
                if (item.memZoneFlashElementConstant.isEmpty)
                {
                    return item.ParseError;
                }
            }

            return null;

        }

        protected override void InnerRun()
        {
            if (source.Assembly == null)
            {
                InnerEnd();
                return;
            }

            foreach (var function in source.Assembly.AllFunctions)
            {
                if (!function.IsStatic)
                {
                    function.CompileCache.VarIndex++;

                    TypeReference type = new TypeReference(function.BaseClass, source.Assembly);
                    type.CheckClassType(source.Assembly.AllClasses, source.Assembly);

                    function.CompileCache.Variables.Add(
                        new VariableMark(Runtime.Structures.Units.Function.SelfParameter, type)
                        {
                            Index = (Integer)function.CompileCache.VarIndex
                        });
                }

                foreach (var parameter in function.Parameters)
                {
                    function.CompileCache.VarIndex++;
                    function.CompileCache.Variables.Add(new VariableMark(parameter.Name, parameter.Type)
                    {
                        Index = (Integer)function.CompileCache.VarIndex
                    });
                } 

                var error = ParseFunction(function);
                if (error != null)
                {
                    InnerEnd(error);
                    return;
                }
            }

            InnerEnd();
        }
    }
}
