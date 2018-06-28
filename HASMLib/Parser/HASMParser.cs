﻿using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Instructions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System.Collections.Generic;
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
        private List<SourceLine> PrepareSource(string input, string fileName, out ParseError error)
        {
            input = multipleSpaceRegex.Replace(input, PrepareSourceSpaceReplace);
            input = commaSpaceRegex.Replace(input, PrepareSourceMultiCommaReplace);

            var actualLines = input.Split('\n').ToList();
            var lines = new List<SourceLine>();
            int index = 0;

            foreach (string line in actualLines)
            {
                //TODO: чето блять нормальное
                if(line.StartsWith("#"))
                {
                    lines.Add(new SourceLinePreprocessor());
                }
                else
                {
                    var a = new SourceLineInstruction()
                    {
                        LineIndex = index++,
                        FileName = fileName
                    };

                    error = a.Parse(line);

                    if (error != null)
                        return null;

                    lines.Add(a);
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
            _namedConsts = new List<NamedConstant>();
        }

        private List<MemZoneFlashElement> SetupRegisters(HASMMachine machine)
        {
            var result = new List<MemZoneFlashElement>();
            machine.GetRegisterNames().ForEach(p =>
            {
                Variables.Add(new Variable(p, LengthQualifier.Single));
                result.Add(new MemZoneFlashElementVariable((UInt12)(_varIndex++), LengthQualifier.Single));
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
                        info.memZoneFlashElementConstant.UpdateValue((UInt12)value, info.ConstIndex);
                        break;
                    case LengthQualifier.Double:
                        info.memZoneFlashElementConstant.UpdateValue((UInt24)value, info.ConstIndex);
                        break;
                    case LengthQualifier.Quad:
                        info.memZoneFlashElementConstant.UpdateValue((UInt48)value, info.ConstIndex);
                        break;
                }

                //Тут какбы нечего возвращать, все уже задано!
                return null;
            }

            //Расчет нового индекса константы	
            int constIndex = ++_constIndex;
            //Заносим данную констатнту в список именных констант
            _namedConsts.Add(new NamedConstant(name, (UInt24)constIndex, new Constant(value, LengthQualifier.Double)));
            //Записываем его во флеш память
            return new MemZoneFlashElementConstantUInt24((UInt24)value, constIndex);
        }

        private List<MemZoneFlashElement> ProceedInstruction(SourceLineInstruction line, out ParseError error)
        {
            if (line.Parameters == null) return GetFlashElementsNoArguents(line, out error);
            else return GetFlashElementsWithArguents(line, out error);
        }

        private List<MemZoneFlashElement> GetFlashElementsNoArguents(SourceLineInstruction line, out ParseError error)
        {
            UInt24 currentInstructionProgramIndex = (UInt24)(_instructionIndex++);
            
            var result = new List<MemZoneFlashElement>();
            error = null;

            //Если инструкция не предполагает отсутсвие параметров то ошибка
            if (line.Instruction.ParameterCount != 0)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
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

            //Закидываем во флеш нашу инструкцию без параметров
            result.Add(new MemZoneFlashElementInstruction(line.Instruction, null, currentInstructionProgramIndex));
            return result;
        }

        private List<MemZoneFlashElement> GetFlashElementsWithArguents(SourceLineInstruction line, out ParseError error)
        {
            UInt24 currentInstructionProgramIndex = (UInt24)(_instructionIndex++);
            var result = new List<MemZoneFlashElement>();

            //Если кол-во параметров не совпадает с предполагаемым
            if (line.Parameters.Length != line.Instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
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
                var constError = Constant.Parse(argument, out Constant constant);

                //Грубое определние типа нашего аргумента
                var isConst = constant != null;
                var isVar = Variables.Select(p => p.Name).Contains(argument);


                //Если допустимо и константа и переменная, то выходит неоднозначность
                if (isVar && isConst && line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.ConstantOrRegister)
                {
                    error = NewParseError(ParseErrorType.Syntax_AmbiguityBetweenVarAndConst, line, argIndex);
                    return null;
                }

                //Если мы определили, что это может быть как переменная, так и коснтанта, 
                //то смотрим что от нас хочет инструкция
                if (isVar && isConst)
                {
                    //Если необходима коснтанта
                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
                    {
                        //Запоминаем индекс константы
                        usedIndexes.Add(new ObjectReference((UInt24)(++_constIndex), ReferenceType.Constant));
                        //Записываем во флеш константу
                        result.Add(constant.ToFlashElement(_constIndex));
                    };

                    //Если необходима переменная
                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.Register)
                    {
                        //Получаем индекс переменной со списка переменных
                        int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(argument);
                        //Запоминаем индекс переменной
                        usedIndexes.Add(new ObjectReference((UInt24)varIndex, ReferenceType.Variable));
                    }
                }

                //Если это однозначно константа, не переменная
                if (isConst)
                {
                    //А ожидалась константа, то ошибка
                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.Register)
                    {
                        error = NewParseError(ParseErrorType.Syntax_ExpectedVar, line, argIndex);
                        return null;
                    }

                    //Запоминаем индекс константы
                    usedIndexes.Add(new ObjectReference((UInt24)(++_constIndex), ReferenceType.Constant));
                    //Заносим константу во флеш
                    result.Add(constant.ToFlashElement(_constIndex));
                }
                else //Если это не константа... 
                {
                    //Если это однозначно переменная...
                    if (isVar)
                    {
                        //А ожидалась константа, то ошибка
                        if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
                        {
                            error = NewParseError(ParseErrorType.Syntax_ExpectedСonst, line, argIndex);
                            return null;
                        }

                        //Получаем индекс переменной со списка переменных 
                        int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(argument);
                        //Запоминаем индекс переменной
                        usedIndexes.Add(new ObjectReference((UInt24)varIndex, ReferenceType.Variable));
                    }
                    else //Если это не переменная, а просили константу
                    
                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.ConstantOrRegister ||
                        line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
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
                                usedIndexes.Add(new ObjectReference((UInt24)item.ConstIndex, ReferenceType.Constant));
                            }
                            else
                            {

                                int constIndex = ++_constIndex;
                                NamedConstant dummyNamedConstant = new NamedConstant(argument, (UInt24)constIndex, new Constant());
                                MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy(constIndex);

                                UnknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                    argument,
                                    NewParseError(ParseErrorType.Syntax_UnknownConstName, line, argIndex),
                                    constIndex,
                                    dummyNamedConstant, dummyConstant));

                                _namedConsts.Add(dummyNamedConstant);
                                //Записываем его во флеш память
                                result.Add(dummyConstant);

                                usedIndexes.Add(new ObjectReference((UInt24)constIndex, ReferenceType.Constant));
                            }

                            //error = NewParseError (ParseErrorType.Syntax_UnknownConstName, label, stringParts, argIndex, index);


                        }
                    }
                    else //Если удалось частично пропарсить константу, но были переполнения и тд...
                    if (constError.Type == ParseErrorType.Constant_BaseOverflow || constError.Type == ParseErrorType.Constant_TooLong)
                    {
                        //Вернуть новую ошибку с типо старой
                        error = NewParseError(constError.Type, line, argIndex);
                        return null;
                    }
                    else //Если ничего не известно, то вернем что неивесное имя переменной
                    {
                        error = NewParseError(ParseErrorType.Syntax_UnknownVariableName, line, argIndex);
                        return null;
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
            // OPT        REQ       OPT            OPT
            //label: instruction a1, a2, a3 ... ; comment

            //Examples
            //       instruction a1, a2, a3 ... ; comment
            //label: instruction                ; comment
            //etc

            //Обнуляем глобальные переменые
            ResetGLobals();

            List<MemZoneFlashElement> result = new List<MemZoneFlashElement>();

            //Заносим регистры в список переменных
            result.AddRange(SetupRegisters(machine));

            //Очистка строк от лишних пробелов, табов
            List<SourceLine> lines = PrepareSource(Source, "onlyFile.hasm", out parseError);

            if (parseError != null)
                return null;

            foreach (SourceLineInstruction line in lines)
            {
                if (line.IsEmpty)
                    continue;

                var res = ProceedInstruction(line, out parseError);
                if(parseError != null)
                {
                    return null;
                }

                result.AddRange(res);
            }

            //Просматриваем все наши "отложенные" константы
            //Если среди них есть пустые, то бьем тревогу!
            foreach (var item in UnknownLabelNameErrorList)
            {
                if(item.memZoneFlashElementConstant.isEmpty)
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


