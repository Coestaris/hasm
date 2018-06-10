using HASMLib.Core.MemoryZone;
using HASMLib.Core;
using HASMLib.SyntaxTokens;
using HASMLib.SyntaxTokens.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib
{
    public class HASMSource
    {
        public string Source { get; }

        public HASMSource(FileStream fs)
        {
        }

        public HASMSource(string source)
        {
            
        }

        public List<Instruction> instructions = new List<Instruction>()
        {
            new InstructionADD(0x1),
            new InstructionJMP(0x2),
            new InstructionMOV(0x3),
            new InstructionNOP(0x4)
        };

        private List<string> PrepareSource(string input)
        {

            input = multipleSpace.Replace(input, " ");
            input = commaSpace.Replace(input, ",");
            return input.Split('\n').ToList();
        }

        private Regex LabelRegex = new Regex(@"^\w{1,100}:");
        private Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");
        private Regex multipleSpace = new Regex(@"[ \t]{1,}");
        private Regex commaSpace = new Regex(@",[ \t]{1,}");

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
                input = LabelRegex.Replace(input, "");
                labelStr = label.Value.TrimEnd(':');
            }

            //Если в строке был найден коментарий, то запомнить его,
            //удалив со строки
            if (comment.Success)
            {
                input = CommentRegex.Replace(input, "");
                commentStr = comment.Value.TrimStart(';');
            }
        }

        public List<MemZoneFlashElement> GetFlashElementsNoArguents(Instruction instruction, string label, string line, int index, out ParseError error)
        {
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

            //Если указан указатель, то наносим его простой переменной во флеш память
            if (!string.IsNullOrEmpty(label))
                result.Add(new MemZoneFlashElementVariableUInt24((UInt24)index, label));

            //Закидываем во флеш нашу инструкцию без параметров
            result.Add(new MemZoneFlashElementInstruction(instruction));
            return result;
        }

        public List<MemZoneFlashElement> GetFlashElementsWithArguents(Instruction instruction, string label, string line, int index, out ParseError error)
        {
            //Выделяем со строки параметры в отдельный массив
            string argumentPart = stringParts[1];
            string[] arguments = argumentPart.Split(',');

            //Если кол-во параметров не совпадает с предполагаемым
            if (arguments.Length != instruction.ParameterCount)
            {
                error = new ParseError(
                    ParseErrorType.Instruction_WrongParameterCount,
                    index,
                    instruction.Name.Match(line).Index);
                return null;
            }

            //Проверяем типы аргументов
            foreach (var argument in arguments)
            {

            }
        }

        public List<MemZoneFlashElement> Parse(HASMMachine machine, out ParseError parseError)
        {
            // OPT        REQ       OPT            OPT
            //label: instruction a1, a2, a3 ... ; comment

            //Examples
            //       instruction a1, a2, a3 ... ; comment
            //label: instruction                ; comment
            //etc

            List<MemZoneFlashElement> result = new List<MemZoneFlashElement>();

            //Очистка строк от лишних пробелов, табов
            List<string> lines = PrepareSource(Source);

            for (var index = 0; index < lines.Count; index++)
            {
                string line = lines[index];

                FindCommentAndLabel(ref line, out string comment, out string label);

                //Дополнительно удаляем лишние символы со строки
                line = line.Trim(' ', '\t');
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
                        string[] stringParts = line.Split(' ');

                        //Если нету аргументов
                        if (stringParts.Length == 1)
                        {
                            var flashElements = GetFlashElementsNoArguents(instruction, label, line, index, out var error);
                            if(flashElements == null)
                            {
                                parseError = error;
                                return null;
                            }
                        }
                        else
                        {
                            var flashElements = GetFlashElementsWithArguents(instruction, label, line, index, out var error);
                            if (flashElements == null)
                            {
                                parseError = error;
                                return null;
                            }
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

            int totalFlashSize = result.Sum(p => p.FixedSize);

            if(totalFlashSize > machine.Flash)
            {
                parseError = new ParseError(ParseErrorType.Other_OutOfFlash, 0, 0);
                return null;
            }

            parseError = null;
            return null;
        }
    }
}