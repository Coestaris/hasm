using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    public class SourceLineInstruction : SourceLine
    {
        public string Label;
        public Instruction Instruction;
        public string[] Parameters;

        private const string LabelReplaceChar = "";
        private const char LabelTrimChar = ':';
        private static Regex LabelRegex = new Regex(@"^\w{1,100}:");

        private string input;

        private const int ArgumentInstructionIndex = 0;
        private const int ArgumentArgumentsIndex = 1;
        private const char ArgumentSplitChar = ',';
        private const char GetStringPartsSplitChar = ' ';

        public bool IsEmpty => Instruction == null;

        public override string ToString()
        {
            if(Instruction != null)
            {
                return $"{Instruction.NameString} {string.Join(" ", Parameters)}";
            }
            return $"No instruction line.{(string.IsNullOrEmpty(Label) ? "" : " [has label]")} {(Comment == null ? "" : " [has comment]")}";
        }

        public static List<Instruction> Instructions
        {
            get
            {
                if (_instructions != null)
                    return _instructions;

                var type = typeof(Instruction);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p.FullName != type.FullName);

                int index = 0;

                _instructions = new List<Instruction>();
                _instructions.AddRange(types.Select(p => (Instruction)Activator.CreateInstance(p, index++)));
                return _instructions;
            }

        }

        public SourceLineInstruction(string input)
        {
            this.input = input;
        }

        private static List<Instruction> _instructions;
        
        private void FindAndDeleteLabel(ref string input)
        {
            Label = "";

            //Поиск вхождений указателя в строке
            Match label = LabelRegex.Match(input);

            //Если в строке был найден указатель, то запомнить его,
            //удалив со строки
            if (label.Success)
            {
                input = LabelRegex.Replace(input, LabelReplaceChar);
                Label = label.Value.TrimEnd(LabelTrimChar);
            }
        }

        private string[] GetStringParts(string input)
        {
            var parts = input.Split(GetStringPartsSplitChar);
            if (parts.Length == 1) return parts;
            else
            {
                return new string[2]
                {
                    parts[0],
                    string.Join("", parts.Skip(1))
                };
            }
        }

        private void SplitLineIntoArguments(string[] stringParts, out string instruction, out string[] argumentList)
        {
            instruction = stringParts[ArgumentInstructionIndex];
            argumentList = stringParts[ArgumentArgumentsIndex].Split(ArgumentSplitChar);
        }

        public ParseError Parse()
        {
            FindAndDeleteComment(ref input);
            FindAndDeleteLabel(ref input);
            CleanUpLine(ref input);

            if (string.IsNullOrWhiteSpace(input))
                return null;

            foreach (var instruction in Instructions)
            {
                //Если регулярка инструкции присутсвует в строке
                //Все обязаны иметь в себе "^" (начало строки), чтобы
                //избегать некоректный поиск в строке!
                if (instruction.Name.Match(input).Success)
                {
                    //Делим строку спейсом, молясь о том, что это сработает!
                    string[] stringParts = GetStringParts(input);

                    //Если слишком много частей строки
                    if (stringParts.Length > 2)
                    {
                        return new ParseError(
                             ParseErrorType.Syntax_UnExpectedToken,
                             LineIndex,
                             Label.Length + 2 + stringParts.Take(2).Sum(p => p.Length),
                             FileName);
                    }

                    if(stringParts.Length == 2)
                    {
                        //Выделяем со строки параметры в отдельный массив
                        SplitLineIntoArguments(stringParts, out var instructionName, out Parameters);
                    }

                    Instruction = instruction;
                    return null;
                }
            }

            //Если не было найдено то ошибка
            return new ParseError(
                ParseErrorType.Syntax_Instruction_UnknownInstruction,
                LineIndex,
                Label.Length + 2,
                FileName);
        }
    }
}
