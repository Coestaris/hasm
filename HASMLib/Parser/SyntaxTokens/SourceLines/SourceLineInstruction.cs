using HASMLib.Runtime.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    public class SourceLineInstruction : SourceLine
    {
        private static Regex LabelRegex = new Regex(@"^\w{1,100}:");
        private static List<Instruction> _instructions;

        private const string LabelReplaceChar = "";
        private const int ArgumentInstructionIndex = 0;
        private const int ArgumentArgumentsIndex = 1;
        private const char ArgumentSplitChar = ',';
        private const char GetStringPartsSplitChar = ' ';
        private const char LabelTrimChar = ':';

        public string Label { get; private set; }
        public Instruction Instruction { get; private set; }
        public string[] Parameters { get; private set; }
        public override bool IsEmpty => Instruction == null;

        public override string ToString()
        {
            if (Instruction != null)
            {
                return $"{Instruction.NameString} {string.Join(" ", Parameters)}";
            }
            else if (Instruction == null && string.IsNullOrEmpty(Label) && string.IsNullOrEmpty(Comment))
            {
                return $"EL: {Input}";
            }
            else return $"No instruction line.{(string.IsNullOrEmpty(Label) ? "" : " [has label]")} {(Comment == null ? "" : " [has comment]")}";
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

        public SourceLineInstruction(string input, int index = -1, string filename = null) : base(input, index, filename) { }

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
            FindAndDeleteComment(ref Input);
            FindAndDeleteLabel(ref Input);
            CleanUpLine(ref Input);

            if (string.IsNullOrWhiteSpace(Input))
                return null;

            foreach (var instruction in Instructions)
            {
                //Если регулярка инструкции присутсвует в строке
                //Все обязаны иметь в себе "^" (начало строки), чтобы
                //избегать некоректный поиск в строке!
                if (instruction.Name.Match(Input).Success)
                {
                    //Делим строку спейсом, молясь о том, что это сработает!
                    string[] stringParts = GetStringParts(Input);

                    //Если слишком много частей строки
                    if (stringParts.Length > 2)
                    {
                        return new ParseError(
                             ParseErrorType.Syntax_UnExpectedToken,
                             LineIndex,
                             Label.Length + 2 + stringParts.Take(2).Sum(p => p.Length),
                             FileName);
                    }

                    if (stringParts.Length == 2)
                    {
                        //Выделяем со строки параметры в отдельный массив
                        SplitLineIntoArguments(stringParts, out var instructionName, out var parameters);
                        Parameters = parameters;
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
