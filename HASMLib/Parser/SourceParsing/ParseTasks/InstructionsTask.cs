using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
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

        private MemZoneFlashElementConstant RegisterConstant(Runtime.Structures.Units.Function function, string name, ulong value)
        {
            if (function._unknownLabelNameErrorList.Exists(p => p.Name == name))
            {
                var info = function._unknownLabelNameErrorList.Find(p => p.Name == name);

                info.namedConstant.Constant = new Constant(value);
                info.memZoneFlashElementConstant.UpdateValue((Integer)value, info.ConstIndex);
                return null;
            }

            int constIndex = ++function._constIndex;

            var constant = new MemZoneFlashElementConstant((Integer)value, (Integer)constIndex);
            function._namedConsts.Add(new NamedConstant(name, (Integer)constIndex, new Constant(value))
            {
                FEReference = constant
            });

            return constant;
        }

        private List<MemZoneFlashElement> ProceedInstruction(HASMLib.Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            if (line.Parameters == null) return GetFlashElementsNoArguents(function, line, out error);
            else return GetFlashElementsWithArguents(function, line, out error);
        }

        private List<MemZoneFlashElement> GetFlashElementsNoArguents(HASMLib.Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)function._instructionIndex++;

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
                RegisterConstant(function, line.Label, (ulong)currentInstructionProgramIndex);
            }

            result.Add(new MemZoneFlashElementInstruction(line.Instruction, null, currentInstructionProgramIndex));
            return result;
        }

        private TypeReference ParseType(string type, out ParseError error)
        {
            TypeReference Type = new TypeReference(type);
            error = null;

            if (Type.IsVoid) return Type;
            if (Type.IsBaseInteger) return Type;
            if (Type.IsClass) return Type;

            Class Class = source.Assembly.AllClasses.Find(p => p.FullName == source.Assembly.Name + Class.NameSeparator + Type.Name);
            if (Class == null) 
            {
                error = new ParseError(ParseErrorType.Syntax_Instruction_UnknownType);
                return null;
            }

            Type.IsClass = true;
            Type.ClassType = Class;
            return Type;
        }

        private List<MemZoneFlashElement> GetFlashElementsWithArguents(Runtime.Structures.Units.Function function, SourceLineInstruction line, out ParseError error)
        {
            Integer currentInstructionProgramIndex = (Integer)(function._instructionIndex++);
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
                bool isVar = function._variables.Select(p => p.Name).Contains(argument);

                //Если допустимо выражение и если это не просто выражение
                if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Expression) && expressionError == null &&
                    (!expression.TokenTree.IsSimple || expression.TokenTree.UnaryFunction != null || expression.TokenTree.UnaryOperator != null))
                {
                    try
                    {
                        Expression.Precompile(expression.TokenTree,
                            (token) =>
                            {

                                var variable = function._variables.Find(p => p.Name == token.RawValue);
                                if (variable != null)
                                {

                                    int varIndex = function._variables.Select(p => p.Name).ToList().IndexOf(token.RawValue);
                                    return new ObjectReference((Integer)varIndex, ReferenceType.Variable)
                                    {
                                        Object = function._variables[varIndex].FEReference
                                    };

                                }

                                //То, возможно, это именная константа...
                                if (function._namedConsts.Select(p => p.Name).Contains(token.RawValue))
                                {
                                    //Получения индекса константы со списка
                                    int constantIndex = function._namedConsts.Select(p => p.Name).ToList().IndexOf(token.RawValue);

                                    return new ObjectReference(function._namedConsts[constantIndex].Index, ReferenceType.Constant)
                                    {
                                        Object = function._namedConsts[constantIndex].FEReference
                                    };
                                }
                                else
                                {

                                    //Константа не найдена! Логично было бы выкинуть ошибку
                                    //Но жива еще надежда на то, что она будет объявлена чуть позже.
                                    //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                                    //И если она таки будет найдена, то подменим ее настоящим значением


                                    //Елси на эту неведомую херню уже ссылались, то сошлемся на нее же
                                    if (function._unknownLabelNameErrorList.Exists(p => p.Name == token.RawValue))
                                    {
                                        var item = function._unknownLabelNameErrorList.Find(p => p.Name == token.RawValue);
                                        //После такой "грязной" хуйни мне хочется сходить с душ!
                                        return new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant)
                                        {
                                            Object = function._unknownLabelNameErrorList.Find(p => p.Name == token.RawValue).memZoneFlashElementConstant
                                        };
                                    }
                                    else
                                    {

                                        int constIndex = ++function._constIndex;
                                        MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy((Integer)constIndex);
                                        NamedConstant dummyNamedConstant = new NamedConstant(token.RawValue, (Integer)constIndex, new Constant())
                                        {
                                            FEReference = dummyConstant
                                        };

                                        function._unknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                            token.RawValue,
                                            NewParseError(ParseErrorType.Syntax_UnknownConstName, line, argIndex),
                                            (Integer)constIndex,
                                            dummyNamedConstant, dummyConstant));

                                        function._namedConsts.Add(dummyNamedConstant);
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

                        Integer index = (Integer)(++function._expressionIndex);
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
                    if(line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.NewVariable)
                    {
                        function._variables.Add(new Variable(argument));
                        function._varIndex++;
                        usedIndexes.Add(new ObjectReference((Integer)function._varIndex, ReferenceType.Variable));
                        continue;
                    }

                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.ClassName)
                    {
                        TypeReference type = ParseType(argument, out error);
                        if (error != null)
                        {
                            error = NewParseError(error.Type, line, argIndex);
                            return null;
                        }

                        //var _const = RegisterConstant(function, $"__classReference{type.UniqueID}_", (ulong)type.UniqueID);
                        //result.Add(_const);                        
                        usedIndexes.Add(new ObjectReference((Integer)type.UniqueID, ReferenceType.Type));
                        continue;
                    }

                    if (line.Instruction.ParameterTypes[argIndex] == InstructionParameterType.FieldName)
                    {
                        Field field = source.Assembly.AllFields.Find(p => p.FullName == 
                            source.Assembly.Name + BaseStructure.NameSeparator + argument);

                        if(field == null)
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
                        Runtime.Structures.Units.Function func = source.Assembly.AllFunctions.Find(p => p.FullName ==
                            source.Assembly.Name + BaseStructure.NameSeparator + argument);

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
                            usedIndexes.Add(new ObjectReference((Integer)(++function._constIndex), ReferenceType.Constant));
                            //Записываем во флеш константу
                            result.Add(constant.ToFlashElement((Integer)function._constIndex));
                        };

                        //Если необходима переменная, а не константа
                        if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Variable) &&
                            !line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                        {
                            //Получаем индекс переменной со списка переменных
                            int varIndex = function._variables.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминаем индекс переменной
                            usedIndexes.Add(new ObjectReference((Integer)varIndex, ReferenceType.Variable));
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
                        usedIndexes.Add(new ObjectReference((Integer)(++function._constIndex), ReferenceType.Constant));
                        //Заносим константу во флеш
                        result.Add(constant.ToFlashElement((Integer)function._constIndex));
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
                        int varIndex = function._variables.Select(p => p.Name).ToList().IndexOf(argument);
                        //Запоминаем индекс переменной
                        usedIndexes.Add(new ObjectReference((Integer)varIndex, ReferenceType.Variable));
                        continue;
                    }

                    //Если это не переменная, а просили константу, не переменную
                    if (line.Instruction.ParameterTypes[argIndex].HasFlag(InstructionParameterType.Constant))
                    {
                        //То, возможно, это именная константа...
                        if (function._namedConsts.Select(p => p.Name).Contains(argument))
                        {
                            //Получения индекса константы со списка
                            int constantIndex = function._namedConsts.Select(p => p.Name).ToList().IndexOf(argument);
                            //Запоминания индекса
                            usedIndexes.Add(new ObjectReference(function._namedConsts[constantIndex].Index, ReferenceType.Constant));
                            //Запись константы во флеш
                            result.Add(function._namedConsts[constantIndex].Constant.ToFlashElement(function._namedConsts[constantIndex].Index));
                        }
                        else
                        {

                            //Константа не найдена! Логично было бы выкинуть ошику
                            //Но жива еще надежда на то, что она будет объявлена чуть позже.
                            //Потому сейчас внесем ее сюда. Создадим новую "пустую" константу во флеше,
                            //И если она таки будет найдена, то подменим ее настоящим значением


                            //Елси на эту неведомую херню уже ссылались, то сослемся на нее же
                            if (function._unknownLabelNameErrorList.Exists(p => p.Name == argument))
                            {
                                var item = function._unknownLabelNameErrorList.Find(p => p.Name == argument);

                                //После такой "грязной" хуйни мне хочется сходить с душ!
                                usedIndexes.Add(new ObjectReference((Integer)item.ConstIndex, ReferenceType.Constant));
                            }
                            else
                            {

                                int constIndex = ++function._constIndex;
                                MemZoneFlashElementConstantDummy dummyConstant = new MemZoneFlashElementConstantDummy((Integer)constIndex);
                                NamedConstant dummyNamedConstant = new NamedConstant(argument, (Integer)constIndex, new Constant())
                                {
                                    FEReference = dummyConstant
                                };

                                function._unknownLabelNameErrorList.Add(new UnknownLabelNameError(
                                    argument,
                                    NewParseError(expressionError == null ? ParseErrorType.Syntax_UnknownConstName : expressionError.Type, line, argIndex),
                                    (Integer)constIndex,
                                    dummyNamedConstant, dummyConstant));

                                function._namedConsts.Add(dummyNamedConstant);
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

            result.Add(new MemZoneFlashElementInstruction(line.Instruction, usedIndexes, currentInstructionProgramIndex));
            error = null;
            return result;
        }

        private ParseError ParseFunction(Runtime.Structures.Units.Function function)
        {
            function.Compiled = new List<MemZoneFlashElement>();
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

                function.Compiled.AddRange(res);
            }

            //Просматриваем все наши "отложенные" константы
            //Если среди них есть пустые, то бьем тревогу!
            foreach (var item in function._unknownLabelNameErrorList)
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
            foreach (var function in source.Assembly.AllFunctions)
            {
                if (!function.IsStatic)
                {
                    function._varIndex++;
                    function._variables.Add(new Variable(Runtime.Structures.Units.Function.SelfParameter,
                        new TypeReference(function.BaseClass)));
                }

                foreach (var parameter in function.Parameters)
                {
                    function._varIndex++;
                    function._variables.Add(new Variable(parameter.Name, parameter.Type));
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
