using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
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

        private MemZoneFlashElementConstant RegisterConstant(string name, ulong value)
        {
            if (source._unknownLabelNameErrorList.Exists(p => p.Name == name))
            {
                var info = source._unknownLabelNameErrorList.Find(p => p.Name == name);

                info.namedConstant.Constant = new Constant(value);
                info.memZoneFlashElementConstant.UpdateValue((Integer)value, info.ConstIndex);
                return null;
            }

            int constIndex = ++source._constIndex;

            var constant = new MemZoneFlashElementConstant((Integer)value, (Integer)constIndex);
            source._namedConsts.Add(new NamedConstant(name, (Integer)constIndex, new Constant(value))
            {
                FEReference = constant
            });

            return constant;
        }

        private List<MemZoneFlashElement> ProceedInstruction(SourceLineInstruction line, out ParseError error)
        {
            if (line.Parameters == null) return GetFlashElementsNoArguents(line, out error);
            else return GetFlashElementsWithArguents(line, out error);
        }

        private List<MemZoneFlashElement> GetFlashElementsNoArguents(SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)source._instructionIndex++;

            var result = new List<MemZoneFlashElement>();
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
                RegisterConstant(line.Label, (ulong)currentInstructionProgramIndex);
            }

            result.Add(new MemZoneFlashElementInstruction(line.Instruction, null, currentInstructionProgramIndex));
            return result;
        }

        private List<MemZoneFlashElement> GetFlashElementsWithArguents(SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)(source._instructionIndex++);
            var result = new List<MemZoneFlashElement>();

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
                RegisterConstant(line.Label, (ulong)currentInstructionProgramIndex);
            }

            int argIndex = 0;
            var usedIndexes = new List<ObjectReference>();

            foreach (var argument in line.Parameters)
            {
                //Попытка пропарсить константу
                ParseError constError = Constant.Parse(argument, out Constant constant);

                //Попытка пропарсить выражение
                ParseError expressionError = expressionError = Expression.Parse(argument, out Expression expression);

                //Грубое определние типа нашего аргумента
                bool isConst = constant != null;
                bool isVar = source._variables.Select(p => p.Name).Contains(argument);

                //Если допустимо выражение и если это не просто выражение
                if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression) &&
                    expressionError == null &&
                    (!expression.TokenTree.IsSimple || expression.TokenTree.UnaryFunction != null || expression.TokenTree.UnaryOperator != null))
                {
                    try
                    {
                        Expression.Precompile(expression.TokenTree,
                            (token) =>
                            {

                                var variable = source._variables.Find(p => p.Name == token.RawValue);
                                if (variable != null)
                                {

                                    int varIndex = source._variables.Select(p => p.Name).ToList().IndexOf(token.RawValue);
                                    return new ObjectReference((Integer)varIndex, ReferenceType.Variable)
                                    {
                                        Object = source._variables[varIndex].FEReference
                                    };

                                }

                                //То, возможно, это именная константа...
                                if (source._namedConsts.Select(p => p.Name).Contains(token.RawValue))
                                {
                                    //Получения индекса константы со списка
                                    int constantIndex = source._namedConsts.Select(p => p.Name).ToList().IndexOf(token.RawValue);

                                    return new ObjectReference(source._namedConsts[constantIndex].Index, ReferenceType.Constant)
                                    {
                                        Object = source._namedConsts[constantIndex].FEReference
                                    };
                                }
                                else
                                {

                                    //Константа не найдена! Логично было бы выкинуть ошибку
                                    //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                    //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                    //И если она таки будет найдена, то подменим ее настоящим значением


                                    //Елси на эту неведомую херню уже ссылались, то сошлемся на нее же
                                    if (source._unknownLabelNameErrorList.Exists(p => p.Name == token.RawValue))
                                    {
                                        var item = source._unknownLabelNameErrorList.Find(p => p.Name == token.RawValue);
                                        //После такой "грязной" хуйни мне хочется сходить с душ!
                                        return new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant)
                                        {
                                            Object = source._unknownLabelNameErrorList.Find(p => p.Name == token.RawValue).memZoneFlashElementConstant
                                        };
                                    }
                                    else
                                    {

                                        int constIndex = ++source._constIndex;
                                        MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy((Integer)constIndex);
                                        NamedConstant dummyNamedConstant = new NamedConstant(token.RawValue, (Integer)constIndex, new Constant())
                                        {
                                            FEReference = dummyConstant
                                        };

                                        source._unknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                            token.RawValue,
                                            NewParseError(ParseErrorType.Syntax_UnknownConstName, line, argIndex),
                                            (Integer)constIndex,
                                            dummyNamedConstant, dummyConstant));

                                        source._namedConsts.Add(dummyNamedConstant);
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

                        Integer index = (Integer)(++source._expressionIndex);
                        result.Add(new MemZoneFlashElementExpression(expression, index));
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
                    //Если это не выражение, то просто разбираем его дальше по типам...

                    //Если допустимо и константа и переменная, то выходит неоднозначность
                    if (isVar && isConst &&
                        line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register) &&
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
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register))
                        {
                            //Запоминаем индекс константы
                            usedIndexes.Add(new ObjectReference((Integer)(++source._constIndex), ReferenceType.Constant));
                            //Записываем во флеш константу
                            result.Add(constant.ToFlashElement((Integer)source._constIndex));
                        };

                        //Если необходима переменная, а не константа
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //Получаем индекс переменной со списка переменных
                            int varIndex = source._variables.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference((Integer)varIndex, ReferenceType.Variable));
                        }
                    }
                    else

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
                        usedIndexes.Add(new ObjectReference((Integer)(++source._constIndex), ReferenceType.Constant));
                        //Заносим константу во флеш
                        result.Add(constant.ToFlashElement((Integer)source._constIndex));
                    }
                    else

                    //Если это не константа... 
                    {
                        //Если это однозначно переменная...
                        if (isVar)
                        {
                            //А ожидалась не переменная, то ошибка
                            if (!line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register))
                            {
                                error = NewParseError(ParseErrorType.Syntax_ExpectedСonst, line, argIndex);
                                return null;
                            }

                            //Получаем индекс переменной со списка переменных 
                            int varIndex = source._variables.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference((Integer)varIndex, ReferenceType.Variable));
                        }
                        else

                        //Если это не переменная, а просили константу, не переменную
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //То, возможно, это именная константа...
                            if (source._namedConsts.Select(p => p.Name).Contains(argument))
                            {
                                //Получения индекса константы со списка
                                int constantIndex = source._namedConsts.Select(p => p.Name).ToList().IndexOf(argument);
                                //Запоминания индекса
                                usedIndexes.Add(new ObjectReference(source._namedConsts[constantIndex].Index, ReferenceType.Constant));
                                //Запись константы во флеш
                                result.Add(source._namedConsts[constantIndex].Constant.ToFlashElement(source._namedConsts[constantIndex].Index));
                            }
                            else
                            {

                                //Константа не найдена! Логично было бы выкинуть ошику
                                //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                //И если она таки будет найдена, то подменим ее настоящим значением


                                //Елси на эту неведомую херню уже ссылались, то сослемся на нее же
                                if (source._unknownLabelNameErrorList.Exists(p => p.Name == argument))
                                {
                                    var item = source._unknownLabelNameErrorList.Find(p => p.Name == argument);

                                    //После такой "грязной" хуйни мне хочется сходить с душ!
                                    usedIndexes.Add(new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant));
                                }
                                else
                                {

                                    int constIndex = ++source._constIndex;
                                    MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy((Integer)constIndex);
                                    NamedConstant dummyNamedConstant = new NamedConstant(argument, (Integer)constIndex, new Constant())
                                    {
                                        FEReference = dummyConstant
                                    };

                                    source._unknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                        argument,
                                        NewParseError(expressionError == null ? ParseErrorType.Syntax_UnknownConstName : expressionError.Type, line, argIndex),
                                        (Integer)constIndex,
                                        dummyNamedConstant, dummyConstant));

                                    source._namedConsts.Add(dummyNamedConstant);
                                    //Записываем его во флеш память
                                    result.Add(dummyConstant);

                                    usedIndexes.Add(new ObjectReference((Integer)constIndex, ReferenceType.Constant));
                                }

                                //error = NewParseError (ParseErrorType.Syntax_UnknownConstName, label, stringParts, argIndex, index);


                            }
                        }
                        else

                        //Если удалось частично пропарсить константу, но были переполнения и тд...
                        if (constError.Type == ParseErrorType.Syntax_Constant_BaseOverflow ||
                            constError.Type == ParseErrorType.Syntax_Constant_WrongType ||
                            constError.Type == ParseErrorType.Syntax_Constant_TooLong)
                        {
                            //Вернуть новую ошибку с типо старой
                            error = NewParseError(constError.Type, line, argIndex);
                            return null;
                        }
                        else

                        //Если ничего не известно, то вернем что неизвестное имя переменной
                        {
                            if (expressionError != null && line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression))
                            {
                                error = NewParseError(expressionError.Type, line, argIndex);
                                return null;
                            }

                            error = NewParseError(ParseErrorType.Syntax_UnknownVariableName, line, argIndex);
                            return null;
                        }
                    }
                }
                argIndex++;
            }

            result.Add(new MemZoneFlashElementInstruction(line.Instruction, usedIndexes, currentInstructionProgramIndex));
            error = null;
            return result;
        }

        protected override void InnerRun()
        {
            foreach (SourceLine line in source._lines)
            {
                if (line.GetType() == typeof(SourceLineInstruction))
                {
                    ParseError error = (line as SourceLineInstruction).Parse();
                    if (error != null)
                    {
                        InnerEnd(true, error);
                        return;
                    }
                }
            }

            //Разбираем логику типов и пр.
            foreach (SourceLineInstruction line in source._lines)
            {
                if (line.IsEmpty) continue;

                var res = ProceedInstruction(line, out ParseError parseError);
                if (parseError != null)
                {
                    InnerEnd(true, parseError);
                    return;
                }

                source.ParseResult.AddRange(res);
            }

            //Просматриваем все наши "отложенные" константы
            //Если среди них есть пустые, то бьем тревогу!
            foreach (var item in source._unknownLabelNameErrorList)
            {
                if (item.memZoneFlashElementConstant.isEmpty)
                {
                    InnerEnd(true, item.ParseError);
                    return;
                }
            }

            InnerEnd(false, null);
        }
    }
}
