using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Instructions;
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

        public static List<Instruction> instructions = new List<Instruction>()
        {
            new InstructionADD(0x0),
            new InstructionJMP(0x1),
            new InstructionMOV(0x2),
            new InstructionNOP(0x3),
            new InstructionOUT(0x4),
            new InstructionLDI(0x5),
            new InstructionCMP(0x6),
            new InstructionBREQ(0x7)
        };
        #endregion


        #region Regex
        private Regex LabelRegex = new Regex(@"^\w{1,100}:");
        private Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");
        private Regex multipleSpaceRegex = new Regex(@"[ \t]{1,}");
        private Regex commaSpaceRegex = new Regex(@",[ \t]{1,}");
        #endregion


        #region Constants
        private const string LabelReplaceChar = "";
        private const char LabelTrimChar = ':';
        private const string CommentReplaceChar = "";
        private const char CommentTrimChar = ':';
        private const string PrepareSourceSpaceReplace = " ";
        private const string PrepareSourceMultiCommaReplace = ",";
        private const int ArgumentInstructionIndex = 0;
        private const int ArgumentArgumentsIndex = 1;
        private const char ArgumentSplitChar = ',';
        private readonly char[] StringCleanUpChars = { ' ', '\t' };
        private const char GetStringPartsSplitChar = ' ';
        #endregion


        #region Text Processing Methods
        private List<string> PrepareSource(string input)
        {
            input = multipleSpaceRegex.Replace(input, PrepareSourceSpaceReplace);
            input = commaSpaceRegex.Replace(input, PrepareSourceMultiCommaReplace);
            return input.Split('\n').ToList().FindAll(p => !string.IsNullOrEmpty(p)).ToList();
        }

        private string CleanUpLine(string input)
        {
            return input.Trim(StringCleanUpChars);
        }

        private string[] GetStringParts(string input)
        {
            return input.Split(GetStringPartsSplitChar);
        }

        private void SplitLineIntoArguments(string[] stringParts, out string instruction, out string[] argumentList)
        {
            instruction = stringParts[ArgumentInstructionIndex];
            argumentList = stringParts[ArgumentArgumentsIndex].Split(ArgumentSplitChar);
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

        private ParseError NewParseError(ParseErrorType error, string label, string[] stringParts, int argIndex, int index)
        {
            return new ParseError(
                error,
                index,
                label.Length + 2 + stringParts.Take(argIndex).Sum(p => p.Length));
        }
        #endregion


        #region Core Parse Methods
        private void FindCommentAndLabel(ref string input, out string commentStr, out string labelStr)
        {
            commentStr = "";
            labelStr = "";

            //Поиск вхождений указателя в строке
            Match label = LabelRegex.Match(input);
            //Поиск вхождений коментария в строке
            Match comment = CommentRegex.Match(input);

            //Если в строке был найден указатель, то запомнить его,
            //удалив со строки
            if (label.Success)
            {
                input = LabelRegex.Replace(input, LabelReplaceChar);
                labelStr = label.Value.TrimEnd(LabelTrimChar);
            }

            //Если в строке был найден коментарий, то запомнить его,
            //удалив со строки
            if (comment.Success)
            {
                input = CommentRegex.Replace(input, CommentReplaceChar);
                commentStr = comment.Value.TrimStart(CommentTrimChar);
            }
        }

        public List<MemZoneFlashElement> GetFlashElementsNoArguents(Instruction instruction, string label, string line, int index, out ParseError error)
        {
            UInt24 currentInstructionProgramIndex = (UInt24)(_instructionIndex++);
            
            var result = new List<MemZoneFlashElement>();
            error = null;

            //Если инструкция не предполагает отсутсвие параметров то ошибка
            if (instruction.ParameterCount != 0)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
                    index,
                    instruction.Name.Match(line).Index);
                return null;
            }

            //Если указан указатель
            if (!string.IsNullOrEmpty(label))
            {
                //Регистируем эту константу
                RegisterConstant(label, index, LengthQualifier.Double);
            }

            //Закидываем во флеш нашу инструкцию без параметров
            result.Add(new MemZoneFlashElementInstruction(instruction, null, currentInstructionProgramIndex));
            return result;
        }

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

        public List<MemZoneFlashElement> GetFlashElementsWithArguents(Instruction instruction, string label, string[] stringParts, int index, out ParseError error)
        {
            string instructionName = "";
            string[] arguments;

            UInt24 currentInstructionProgramIndex = (UInt24)(_instructionIndex++);

            var result = new List<MemZoneFlashElement>();

            //Выделяем со строки параметры в отдельный массив
            SplitLineIntoArguments(stringParts, out instructionName, out arguments);


            //Если кол-во параметров не совпадает с предполагаемым
            if (arguments.Length != instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
                    index,
                    instruction.Name.Match(instructionName).Index);
                return null;
            }

            //Если указан указатель
            if (!string.IsNullOrEmpty(label))
            {

                //Регистируем эту константу
                RegisterConstant(label, currentInstructionProgramIndex, LengthQualifier.Double);
            }

            //Индекс текущего аргумента
            int argIndex = 0;

            //Список последовательных индексов, что используются в инструкции
            var usedIndexes = new List<ObjectReference>();

            //Проверяем типы аргументов
            foreach (var argument in arguments)
            {
                Constant constant = null;

                //Попытка пропарсить константу
                var constError = Constant.Parse(argument, out constant);

                //Грубое определние типа нашего аргумента
                var isConst = constant != null;
                var isVar = Variables.Select(p => p.Name).Contains(argument);


                //Если допустимо и константа и переменная, то выходит неоднозначность
                if (isVar && isConst && instruction.ParameterTypes[argIndex] == InstructionParameterType.ConstantOrRegister)
                {
                    error = NewParseError(ParseErrorType.Syntax_AmbiguityBetweenVarAndConst, label, stringParts, argIndex, index);
                    return null;
                }

                //Если мы определили, что это может быть как переменная, так и коснтанта, 
                //то смотрим что от нас хочет инструкция
                if (isVar && isConst)
                {
                    //Если необходима коснтанта
                    if (instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
                    {
                        //Запоминаем индекс константы
                        usedIndexes.Add(new ObjectReference((UInt24)(++_constIndex), ReferenceType.Constant));
                        //Записываем во флеш константу
                        result.Add(constant.ToFlashElement(_constIndex));
                    };

                    //Если необходима переменная
                    if (instruction.ParameterTypes[argIndex] == InstructionParameterType.Register)
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
                    if (instruction.ParameterTypes[argIndex] == InstructionParameterType.Register)
                    {
                        error = NewParseError(ParseErrorType.Syntax_ExpectedVar, label, stringParts, argIndex, index);
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
                        if (instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
                        {
                            error = NewParseError(ParseErrorType.Syntax_ExpectedСonst, label, stringParts, argIndex, index);
                            return null;
                        }

                        //Получаем индекс переменной со списка переменных 
                        int varIndex = Variables.Select(p => p.Name).ToList().IndexOf(argument);
                        //Запоминаем индекс переменной
                        usedIndexes.Add(new ObjectReference((UInt24)varIndex, ReferenceType.Variable));
                    }
                    else //Если это не переменная, а просили константу
                        if (instruction.ParameterTypes[argIndex] == InstructionParameterType.ConstantOrRegister ||
                            instruction.ParameterTypes[argIndex] == InstructionParameterType.Constant)
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
                                    NewParseError(ParseErrorType.Syntax_UnknownConstName, label, stringParts, argIndex, index),
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
                        error = NewParseError(constError.Type, label, stringParts, argIndex, index);
                        return null;
                    }
                    else //Если ничего не известно, то вернем что неивесное имя переменной
                    {
                        error = NewParseError(ParseErrorType.Syntax_UnknownVariableName, label, stringParts, argIndex, index);
                        return null;
                    }
                }
                argIndex++;
            }

            result.Add(new MemZoneFlashElementInstruction(instruction, usedIndexes, currentInstructionProgramIndex));
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

            try
            {
                //Обнуляем глобальные переменые
                ResetGLobals();

                List<MemZoneFlashElement> result = new List<MemZoneFlashElement>();

                //Заносим регистры в список переменных
                result.AddRange(SetupRegisters(machine));

                //Очистка строк от лишних пробелов, табов
                List<string> lines = PrepareSource(Source);

                for (var index = 0; index < lines.Count; index++)
                {
                    string line = lines[index];
                    string comment = "";
                    string label = "";

                    FindCommentAndLabel(ref line, out comment, out label);

                    //Дополнительно удаляем лишние символы со строки
                    line = CleanUpLine(line);

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    //Переменная для отслеживания успеха поиска инструкции
                    bool found = false;

                    foreach (var instruction in instructions)
                    {
                        //Если регулярка инструкции присутсвует в строке
                        //Все обязаны иметь в себе "^" (начало строки), чтобы
                        //избегать некоректный поиск в строке!
                        if (instruction.Name.Match(line).Success)
                        {
                            //Делим строку спейсом, молясь о том, что это сработает!
                            string[] stringParts = GetStringParts(line);

                            //Если нету аргументов
                            if (stringParts.Length == 1)
                            {
                                ParseError error;
                                //Попытка пропарсить строку
                                var flashElements = GetFlashElementsNoArguents(instruction, label, line, index, out error);
                                if (flashElements == null)
                                {
                                    parseError = error;
                                    return null;
                                }
                                result.AddRange(flashElements);
                            }
                            else if (stringParts.Length == 2)
                            {
                                ParseError error;
                                //Попытка пропарсить строку
                                var flashElements = GetFlashElementsWithArguents(instruction, label, stringParts, index, out error);
                                if (flashElements == null)
                                {
                                    parseError = error;
                                    return null;
                                }
                                result.AddRange(flashElements);
                            }
                            else
                            {
                                parseError = NewParseError(ParseErrorType.Syntax_UnExpectedToken, label, stringParts, 2, index);
                                return null;
                            }
                            found = true;
                            break;
                        }

                    }

                    //Если не было найдено то ошибка
                    if (!found)
                    {
                        parseError = new ParseError(
                            ParseErrorType.Instruction_UnknownInstruction,
                            index,
                            label.Length + 2);
                        return null;
                    }
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
            catch
            {
                parseError = null;
                return null;
            }
        }
    }
}


