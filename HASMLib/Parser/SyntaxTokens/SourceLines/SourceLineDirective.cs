using HASMLib.Parser.SyntaxTokens.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    class SourceLineDirective : SourceLine
    {
        public class SourceLineDirectiveType
        {
            public string Name;
            public Type Type;

            public SourceLineDirectiveType(string name, Type type)
            {
                Name = name;
                Type = type;
            }
        }

        public const char DirectiveStartChar = '.';
        private static Regex DirectiveRegex = new Regex(@"^\.\w+(\([^,]+?(,([^,]+?,\s?)+[^,]+)?\))?\s+\w+$");

        private static List<SourceLineDirectiveType> _sourceLineTypes;

        public static List<SourceLineDirectiveType> SourceLineTypes
        {
            get
            {
                if (_sourceLineTypes != null)
                    return _sourceLineTypes;

                var type = typeof(SourceLineDirective);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p.FullName != type.FullName);

                _sourceLineTypes = new List<SourceLineDirectiveType>();
                _sourceLineTypes.AddRange(
                    types.Select(p => new SourceLineDirectiveType(
                        p.GetField("FindName", BindingFlags.Static | BindingFlags.Public).GetValue(null) as string,
                        p)));

                return _sourceLineTypes;
            }
        }

        public bool RequireCodeBlock;
        public CodeBlock CodeBlock;

        public string Directive;
        public string Name;
        public List<string> Parameters;

        public SourceLineDirective(SourceLine sourceLine) : base(sourceLine.Input, sourceLine.LineIndex, sourceLine.FileName) { }

        public static bool IsDirective(string input)
        {
            string Input = input.Trim(StringCleanUpChars);
            return !string.IsNullOrWhiteSpace(Input) && Input[0] == DirectiveStartChar;
        }

        public ParseError Parse()
        {
            string input = Input;

            FindAndDeleteComment(ref input);
            CleanUpLine(ref input);

            if (!DirectiveRegex.IsMatch(input))
            {
                return new ParseError(ParseErrorType.Directives_WrongDirectiveFormat,
                    LineIndex, FileName);
            }

            input = input.TrimStart(DirectiveStartChar);
            string[] parts = input.Split(' ');
            Name = parts.Last();

            if (input.Contains('('))
            {
                if (input.Count(p => p == '(') != input.Count(p => p == ')'))
                {
                    return new ParseError(ParseErrorType.Directives_WrongBracketCount,
                        LineIndex, FileName);
                }

                parts = new string[2]
                {
                    string.Join(" ", parts.Take(parts.Length - 1)),
                    parts.Last()
                };

                var nameParamPart = parts[0].Remove(parts[0].Length - 1, 1);
                var nameParamParts = nameParamPart.Split('(');

                Directive = nameParamParts[0];
                Parameters = string.Join("(", nameParamParts.Skip(1)).Split(',').ToList();
            }
            else
            {
                Directive = parts[0];
            }

            return null;
        }

        internal SourceLineDirective Cast(out ParseError error)
        {
            var types = SourceLineTypes;
            var type = types.Find(p => p.Name == Directive);

            if(type == null)
            {
                error = new ParseError(ParseErrorType.Directives_WrongDirective,
                    LineIndex, FileName);
                return null;
            }

            error = null;
            return (SourceLineDirective)Activator.CreateInstance(type.Type, (object)this);

        }
    }
}
