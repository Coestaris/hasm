using HASMLib.Core;
using HASMLib.Parser.Parser;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class PreprocessorDirective
    {
        private static Regex GeneralPreprocessorRegex = new Regex(@"^#[^#]{1,}$");
        private static List<PreprocessorDirective> _preprocessorDirectives;
        private static Func<string, List<string>> getLinesFunc;
        private static string workingDirectory;
        private static Stack<bool> enableStack;
        private static List<Define> defines;

        public bool CanAddNewLines { get; protected set; }
        public string Name { get; protected set; }
        
        public string ClearInput(string input)
        {
            var match = SourceLine.CommentRegex.Match(input);

            if (match.Success)
                input = input.Remove(match.Index, match.Length);

            return input.TrimStart('#').Remove(0, Name.Length).Trim(' ', '\t', '\r');
        }
       
        public static bool IsPreprocessorLine(string line)
        {
            return line.StartsWith("#");
        }

        public static List<SourceLine> RecursiveParse(string fileName, string WorkingDirectory, out ParseError error, Func<string, List<string>> GetLinesFunc, List<Define> defines)
        {
            getLinesFunc = GetLinesFunc ?? throw new ArgumentNullException(nameof(GetLinesFunc));
            workingDirectory = WorkingDirectory;
            enableStack = new Stack<bool>();
            PreprocessorDirective.defines = new List<Define>();

            if (defines != null)
                PreprocessorDirective.defines.AddRange(defines);

            PreprocessorDirective.defines.Add(new Define("__base__", HASMBase.Base.ToString()));

            var result = RecursiveParse(fileName);

            error = result.error;
            return result.sourceLines;
        }

        public static PreprocessorDirective GetDirective(string input, int index, string filename, out ParseError error)
        {
            if (!GeneralPreprocessorRegex.IsMatch(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_WrongPreprocessorFormat, index, filename);
                return null;
            }

            //Удаляем шарп с начала строки
            input = input.TrimStart('#');
            string directiveName = input.Split(' ')[0];

            foreach (var item in PreprocessorDirectives)
            {
                if (item.Name == directiveName)
                {
                    error = null;
                    return item;
                }
            }

            error = new ParseError(ParseErrorType.Preprocessor_UnknownDirective, index, filename);
            return null;

        }

        public static List<PreprocessorDirective> PreprocessorDirectives
        {
            get
            {
                if (_preprocessorDirectives != null)
                    return _preprocessorDirectives;

                var type = typeof(PreprocessorDirective);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p.FullName != type.FullName);

                _preprocessorDirectives = new List<PreprocessorDirective>();
                _preprocessorDirectives.AddRange(types.Select(p => (PreprocessorDirective)Activator.CreateInstance(p)));
                return _preprocessorDirectives;
            }
        }

        private static PreprocessorParseResult RecursiveParse(string fileName)
        {
            var result = new List<SourceLine>();

            if (!File.Exists(fileName))
                fileName = Path.Combine(workingDirectory, fileName);

            if(!File.Exists(fileName))
            {
                return new PreprocessorParseResult(
                    null,
                    new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedFile, -1, fileName));
            }

            List<string> lines = getLinesFunc(fileName);

            for(int index = 0; index < lines.Count; index++)
            {
                string line = lines[index];

                if (IsPreprocessorLine(line))
                {
                    PreprocessorDirective directive = GetDirective(line, index, fileName, out ParseError error);
                    if (error != null) return new PreprocessorParseResult(null, error);

                    if(directive.CanAddNewLines)
                    {
                        var newLines = directive.Apply(line, enableStack, defines, out ParseError parseError, RecursiveParse);
                        if (parseError != null) return new PreprocessorParseResult(null, parseError);
                        result.AddRange(newLines);
                    }
                    else
                    {
                        directive.Apply(line, enableStack, defines, out ParseError parseError);
                        if (parseError != null) return new PreprocessorParseResult(null, new ParseError(parseError.Type, index, fileName));
                    }
                }
                else
                {
                    if (!enableStack.Contains(false))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        ParseError parseError = Define.ResolveDefines(defines, ref line, index, fileName);
                        if(parseError != null) return new PreprocessorParseResult(null, parseError);
                        result.Add(new SourceLineInstruction(line, index, fileName));
                    }
                }
            }
            return new PreprocessorParseResult(result, null); 
        }

        internal abstract void Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error);

        internal abstract List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc);
    }
}
