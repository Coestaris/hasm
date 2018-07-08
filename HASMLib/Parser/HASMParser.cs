using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.Instructions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser
{
    public partial class HASMParser
    {
        #region Globals
        private List<Variable> Variables;
        private List<NamedConstant> _namedConsts;
        private int _constIndex;
        private int _expressionIndex;
        private int _varIndex;
        private int _instructionIndex;

        private struct UnknownLabelNameError
        {
            public string Name;
            public int ConstIndex;
            public ParseError ParseError;

            //Ссылки на обьекты во флеше!
            public NamedConstant namedConstant;
            public MemZoneFlashElementConstantDummy memZoneFlashElementConstant;

            public UnknownLabelNameError(string name, ParseError pe, int constIndex, NamedConstant namedConstant, MemZoneFlashElementConstantDummy memZoneFlashElementConstant)
            {
                ConstIndex = constIndex;
                Name = name;
                ParseError = pe;
                this.namedConstant = namedConstant;
                this.memZoneFlashElementConstant = memZoneFlashElementConstant;
            }
        }

        private List<UnknownLabelNameError> UnknownLabelNameErrorList;
        #endregion


        #region Regex
        private Regex multipleSpaceRegex = new Regex(@"[ \t]{1,}");
        private Regex commaSpaceRegex = new Regex(@",[ \t]{1,}");
        #endregion


        #region Constants
      
        private const string PrepareSourceSpaceReplace = " ";
        private const string PrepareSourceMultiCommaReplace = ",";


        #endregion


        #region Text Processing Methods

        private List<string> BasePrepareLines(string absoluteFileName)
        {
            string input = File.ReadAllText(absoluteFileName);

            input = multipleSpaceRegex.Replace(input, PrepareSourceSpaceReplace);
            input = commaSpaceRegex.Replace(input, PrepareSourceMultiCommaReplace);

            return input.Split('\n').Select(p => p.Trim('\r', '\t')).ToList();
        }

        private List<SourceLine> InstructionPhase(List<SourceLine> lines, out ParseError error)
        {
            foreach (SourceLine line in lines)
            {
                if(line.GetType() == typeof(SourceLineInstruction))
                {
                    error = (line as SourceLineInstruction).Parse();
                    if (error != null) return null;
                }
            }

            error = null;
            return lines;
        }
        #endregion


        #region Help Methods
        private void ResetGLobals()
        {
            UnknownLabelNameErrorList = new List<UnknownLabelNameError>();
            Variables = new List<Variable>();
            _constIndex = 0;
            _expressionIndex = 0;
            _varIndex = 0;
            _namedConsts = new List<NamedConstant>();
        }

        private List<MemZoneFlashElement> SetupRegisters(HASMMachine machine)
        {
            var result = new List<MemZoneFlashElement>();
            machine.GetRegisterNames().ForEach(p =>
            {
                var a = new MemZoneFlashElementVariable((UIntSingle)(_varIndex++), LengthQualifier.Single);
                Variables.Add(new Variable(p, LengthQualifier.Single)
                {
                    variable = a
                });
                result.Add(a);
            });
            return result;
        }

        private ParseError NewParseError(ParseErrorType error, SourceLineInstruction line, int argIndex)
        {
            return new ParseError(
                error,
                line.LineIndex,
                line.Label.Length + 2 + line.Parameters.Take(argIndex).Sum(p => p.Length));
        }
        #endregion


        #region Core Parse Methods
        private MemZoneFlashElementConstant RegisterConstant(string name, long value, LengthQualifier lq)
        {
            //Если эта константа требовалась ранее, то подменяем ее!
            if (UnknownLabelNameErrorList.Exists(p => p.Name == name))
            {
                var info = UnknownLabelNameErrorList.Find(p => p.Name == name);

                info.namedConstant.Constant = new Constant(value, lq);

                switch (lq)
                {
                    case LengthQualifier.Single:
                        info.memZoneFlashElementConstant.UpdateValue((UIntSingle)value, info.ConstIndex);
                        break;
                    case LengthQualifier.Double:
                        info.memZoneFlashElementConstant.UpdateValue((UIntDouble)value, info.ConstIndex);
                        break;
                    case LengthQualifier.Quad:
                        info.memZoneFlashElementConstant.UpdateValue((UIntQuad)value, info.ConstIndex);
                        break;
                }

                //Тут какбы нечего возвращать, все уже задано!
                return null;
            }

            //Расчет нового индекса константы	
            int constIndex = ++_constIndex;

            var constant = new MemZoneFlashElementConstantUInt24((UIntDouble)value, constIndex);
            
            //Заносим данную констатнту в список именных констант
            _namedConsts.Add(new NamedConstant(name, (UIntDouble)constIndex, new Constant(value, LengthQualifier.Double))
            {
                constant = constant
            });
            //Записываем его во флеш память
            return constant;
        }

        private List<MemZoneFlashElement> ProceedInstruction(SourceLineInstruction line, out ParseError error)
        {
            if (line.Parameters == null) return GetFlashElementsNoArguents(line, out error);
            else return GetFlashElementsWithArguents(line, out error);
        }

        private List<MemZoneFlashElement> GetFlashElementsNoArguents(SourceLineInstruction line, out ParseError error)
        {
            UIntDouble currentInstructionProgramIndex = (UIntDouble)(_instructionIndex++);
            
            var result = new List<MemZoneFlashElement>();
            error = null;

            //Если инструкция не предполагает отсутсвие параметров то ошибка
            if (line.Instruction.ParameterCount != 0)
            {
                error = new ParseError(
                    ParseErrorType.Syntax_Instruction_WrongParameterCount,
                    line.LineIndex,
                    (line.Comment != null ? line.Comment.Length : 0) + line.Instruction.NameString.Length,
                    line.FileName);
                return null;
            }

            //Если указан указатель
            if (!string.IsNullOrEmpty(line.Label))
            {
                //Регистируем эту константу
                RegisterConstant(line.Label, currentInstructionProgramIndex, LengthQualifier.Double);
            }

            //Закидываем во флеш нашу инструкцию без параметров
            result.Add(new MemZoneFlashElementInstruction(line.Instruction, null, currentInstructionProgramIndex));
            return result;
        }

        private List<MemZoneFlashElement> GetFlashElementsWithArguents(SourceLineInstruction line, out ParseError error)
        {
            UIntDouble currentInstructionProgramIndex = (UIntDouble)(_instructionIndex++);
            var result = new List<MemZoneFlashElement>();

            //Если кол-во параметров не совпадает с предполагаемым
            if (line.Parameters.Length != line.Instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Syntax_Instruction_WrongParameterCount,
                    line.LineIndex,
                    line.Comment.Length + line.Instruction.NameString.Length,
                    line.FileName);
                return null;
            }

            //Если указан указатель
            if (!string.IsNullOrEmpty(line.Label))
            {
                //Регистируем эту константу
                RegisterConstant(line.Label, currentInstructionProgramIndex, LengthQualifier.Double);
            }

            //Индекс текущего аргумента
            int argIndex = 0;

            //Список последовательных индексов, что используются в инструкции
            var usedIndexes = new List<ObjectReference>();

            //Проверяем типы аргументов
            foreach (var argument in line.Parameters)
            {
                //Попытка пропарсить константу
                ParseError constError = Constant.Parse(argument, out Constant constant);

                //Попытка пропарсить выражение
                ParseError expressionError = expressionError = Expression.Parse(argument, out Expression expression);


                //Грубое определние типа нашего аргумента
                bool isConst = constant != null;
                bool isVar = Variables.Select(p => p.Name).Contains(argument);

                //Если допустимо выражение и если это не просто выражение
                if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression) &&
                    expressionError == null  && 
                    (!expression.TokenTree.IsSimple || expression.TokenTree.UnaryFunction != null || expression.TokenTree.UnaryOperator != null))
                {
                    try
                    {
                        Expression.Precompile(expression.TokenTree,
                            (token) =>
                            {

                                var variable = Variables.Find(p => p.Name == token.RawValue);
                                if (variable != null)
                                {
                                //Получаем индекс переменной со списка переменных
                                int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(token.RawValue);
                                //Запоминаем индекс переменной
                                return new ObjectReference((UIntDouble)varIndex, ReferenceType.Variable)
                                    {
                                        Object = Variables[varIndex].variable
                                    };
                                }

                            //То, возможно, это именная константа...
                            if (_namedConsts.Select(p => p.Name).Contains(token.RawValue))
                                {
                                //Получения индекса константы со списка
                                int constantIndex = _namedConsts.Select(p => p.Name).ToList().IndexOf(token.RawValue);

                                    return new ObjectReference(_namedConsts[constantIndex].Index, ReferenceType.Constant)
                                    {
                                        Object = _namedConsts[constantIndex].constant
                                    };
                                }
                                else
                                {

                                //Константа не найдена! Логично было бы выкинуть ошибку
                                //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                //И если она таки будет найдена, то подменим ее настоящим значением


                                //Елси на эту неведомую херню уже ссылались, то сошлемся на нее же
                                if (UnknownLabelNameErrorList.Exists(p => p.Name == token.RawValue))
                                    {
                                        var item = UnknownLabelNameErrorList.Find(p => p.Name == token.RawValue);
                                    //После такой "грязной" хуйни мне хочется сходить с душ!
                                    return new ObjectReference((UIntDouble)item.ConstIndex, ReferenceType.Constant)
                                        {
                                            Object = UnknownLabelNameErrorList.Find(p => p.Name == token.RawValue).memZoneFlashElementConstant
                                        };
                                    }
                                    else
                                    {

                                        int constIndex = ++_constIndex;
                                        MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy(constIndex);
                                        NamedConstant dummyNamedConstant = new NamedConstant(token.RawValue, (UIntDouble)constIndex, new Constant())
                                        {
                                            constant = dummyConstant
                                        };

                                        UnknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                            token.RawValue,
                                            NewParseError(ParseErrorType.Syntax_UnknownConstName, line, argIndex),
                                            constIndex,
                                            dummyNamedConstant, dummyConstant));

                                        _namedConsts.Add(dummyNamedConstant);
                                    //Записываем его во флеш память
                                    result.Add(dummyConstant);

                                        return new ObjectReference((UIntDouble)constIndex, ReferenceType.Constant)
                                        {
                                            Object = dummyConstant
                                        };
                                    }
                                }
                            },
                            (c) =>
                            {
                            //Расчет нового индекса константы	
                            int constIndex = ++_constIndex;
                            //Записываем его во флеш память
                            var flashElement = c.ToFlashElement(constIndex);

                                result.Add(flashElement);

                                return new ObjectReference((UIntDouble)constIndex, ReferenceType.Constant)
                                {
                                    Object = flashElement
                                };

                            });

                        UIntDouble index = (UIntDouble)(++_expressionIndex);
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
                        if ( line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register))
                        {
                            //Запоминаем индекс константы
                            usedIndexes.Add(new ObjectReference((UIntDouble)(++_constIndex), ReferenceType.Constant));
                            //Записываем во флеш константу
                            result.Add(constant.ToFlashElement(_constIndex));
                        };

                        //Если необходима переменная, а не константа
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Register) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //Получаем индекс переменной со списка переменных
                            int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference((UIntDouble)varIndex, ReferenceType.Variable));
                        }
                    } else

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
                        if(constError != null && (constError.Type == ParseErrorType.Syntax_Constant_BaseOverflow || constError.Type == ParseErrorType.Syntax_Constant_TooLong))
                        {
                            error = NewParseError(constError.Type, line, argIndex);
                            return null;
                        }

                        //Запоминаем индекс константы
                        usedIndexes.Add(new ObjectReference((UIntDouble)(++_constIndex), ReferenceType.Constant));
                        //Заносим константу во флеш
                        result.Add(constant.ToFlashElement(_constIndex));
                    } else 
                    
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
                            int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference((UIntDouble)varIndex, ReferenceType.Variable));
                        } else 
                        
                        //Если это не переменная, а просили константу, не переменную
                        if ( line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //То, возможно, это именная константа...
                            if (_namedConsts.Select(p => p.Name).Contains(argument))
                            {
                                //Получения индекса константы со списка
                                int constantIndex = _namedConsts.Select(p => p.Name).ToList().IndexOf(argument);
                                //Запоминания индекса
                                usedIndexes.Add(new ObjectReference(_namedConsts[constantIndex].Index, ReferenceType.Constant));
                                //Запись константы во флеш
                                result.Add(_namedConsts[constantIndex].Constant.ToFlashElement(_namedConsts[constantIndex].Index));
                            }
                            else
                            {

                                //Константа не найдена! Логично было бы выкинуть ошику
                                //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                //И если она таки будет найдена, то подменим ее настоящим значением


                                //Елси на эту неведомую херню уже ссылались, то сослемся на нее же
                                if (UnknownLabelNameErrorList.Exists(p => p.Name == argument))
                                {
                                    var item = UnknownLabelNameErrorList.Find(p => p.Name == argument);

                                    //После такой "грязной" хуйни мне хочется сходить с душ!
                                    usedIndexes.Add(new ObjectReference((UIntDouble)item.ConstIndex, ReferenceType.Constant));
                                }
                                else
                                {

                                    int constIndex = ++_constIndex;
                                    MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy(constIndex);
                                    NamedConstant dummyNamedConstant = new NamedConstant(argument, (UIntDouble)constIndex, new Constant())
                                    {
                                        constant = dummyConstant
                                    };

                                    UnknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                        argument,
                                        NewParseError(expressionError == null ? ParseErrorType.Syntax_UnknownConstName : expressionError.Type, line, argIndex),
                                        constIndex,
                                        dummyNamedConstant, dummyConstant));

                                    _namedConsts.Add(dummyNamedConstant);
                                    //Записываем его во флеш память
                                    result.Add(dummyConstant);

                                    usedIndexes.Add(new ObjectReference((UIntDouble)constIndex, ReferenceType.Constant));
                                }

                                //error = NewParseError (ParseErrorType.Syntax_UnknownConstName, label, stringParts, argIndex, index);


                            }
                        } else 
                        
                        //Если удалось частично пропарсить константу, но были переполнения и тд...
                        if (constError.Type == ParseErrorType.Syntax_Constant_BaseOverflow || constError.Type == ParseErrorType.Syntax_Constant_TooLong)
                        {
                            //Вернуть новую ошибку с типо старой
                            error = NewParseError(constError.Type, line, argIndex);
                            return null;
                        }
                        else 
                        
                        //Если ничего не известно, то вернем что неизвестное имя переменной
                        {
                            if(expressionError != null && line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression))
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
        #endregion

        internal List<MemZoneFlashElement> Parse(HASMMachine machine, out ParseError parseError, string Source)
        {
            var filename = Path.GetTempFileName();
            File.WriteAllText(filename, Source);

            return Parse(machine, out parseError, filename, new FileInfo(filename).DirectoryName);

        }
        
        // OPT        REQ       OPT            OPT
        //label: instruction a1, a2, a3 ... ; comment

        //Examples
        //       instruction a1, a2, a3 ... ; comment
        //label: instruction                ; comment
        //etc
        internal List<MemZoneFlashElement> Parse(HASMMachine machine, out ParseError parseError, string filename, string workingDirectory, List<Define> defines = null)
        {
            //Задаем глобальные переменные выражений
            Expression.InitGlobals();

            //Обнуляем глобальные переменные
            ResetGLobals();

            //Проверка валидности указанного имени файла
            if(!File.Exists(filename))
            {
                parseError = new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedFile);
                return null;
            }

            //Проверка валидности указанного имени рабочей директории
            if (workingDirectory == null)
            {
                workingDirectory = new FileInfo(filename).DirectoryName;
            }
            else
            {
                if(!Directory.Exists(workingDirectory))
                {
                    parseError = new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedWorkingDirectory);
                    return null;
                }
            }

            List<MemZoneFlashElement> result = new List<MemZoneFlashElement>();

            //Заносим регистры в список переменных
            result.AddRange(SetupRegisters(machine));

            //Запускаем рекурсивный метод обработки препроцессором
            List<SourceLine> lines = PreprocessorDirective.RecursiveParse(filename, workingDirectory, out parseError, BasePrepareLines, defines);
            if (parseError != null) return null;

            //Обрабатываем данные о инструкциях
            lines = InstructionPhase(lines, out parseError);
            if (parseError != null) return null;

            //Разбираем логику типов и пр.
            foreach (SourceLineInstruction line in lines)
            { 
                if (line.IsEmpty) continue;

                var res = ProceedInstruction(line, out parseError);
                if (parseError != null)
                {
                    return null;
                }

                result.AddRange(res);
            }

            //Просматриваем все наши "отложенные" константы
            //Если среди них есть пустые, то бьем тревогу!
            foreach (var item in UnknownLabelNameErrorList)
            {
                if (item.memZoneFlashElementConstant.isEmpty)
                {
                    parseError = item.ParseError;
                    return null;
                }
            }


            //Помещаем все константы в начало флеша для удобства дебага
            //Выбираем с массива константы
            var constants = result.FindAll(p => p.Type == MemZoneFlashElementType.Constant).ToList();
            //Удаляем их из коллекции
            result.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);
            //Пихаем в ее начало
            result.InsertRange(0, constants);

            //Если размер программы превышает максимально допустимый для этой машины
            int totalFlashSize = result.Sum(p => p.FixedSize);
            if (totalFlashSize > machine.Flash)
            {
                parseError = new ParseError(ParseErrorType.Other_OutOfFlash);
                return null;
            }

            parseError = null;
            return result;

        }
    }
}


