﻿using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens
{
    /*
        #define name[(arg, ...)] [value]
        #undef name
        #ifdef name
        #ifndef name
        #if <conditional>
        #elif <conditional>
        
        #else 
        #endif
        
        #error
        #warning 
        #message
        
        #include

        # (empty directive)
    */

    public abstract class PreprocessorDirective
    {
        private static Regex GeneralPreprocessorRegex = new Regex(@"^#[^#]{1,}$");

        public static bool IsPreprocessorLine(string line)
        {
            return line.StartsWith("#");
        }

        public string ClearInput(string input)
        {
            return input.TrimStart('#').Remove(0, Name.Length).Trim();
        }

        private static Func<string, List<string>> getLinesFunc;
        private static string workingDirectory;
        private static Stack<bool> enableStack;
        private static List<Define> defines;

        public static List<SourceLine> RecursiveParse(string fileName, string WorkingDirectory, out ParseError error, Func<string, List<string>> GetLinesFunc)
        {
            getLinesFunc = GetLinesFunc ?? throw new ArgumentNullException(nameof(GetLinesFunc));
            workingDirectory = WorkingDirectory;
            enableStack = new Stack<bool>();
            defines = new List<Define>();

            var result = RecursiveParse(fileName);

            error = result.error;
            return result.sourceLines;
        }

        protected struct RecursiveParseResult
        {
            public List<SourceLine> sourceLines;
            public ParseError error;

            public RecursiveParseResult(List<SourceLine> sourceLines, ParseError error)
            {
                this.sourceLines = sourceLines;
                this.error = error;
            }
        }

        private static RecursiveParseResult RecursiveParse(string fileName)
        {
            var result = new List<SourceLine>();

            if (!File.Exists(fileName))
                fileName = Path.Combine(workingDirectory, fileName);

            if(!File.Exists(fileName))
            {
                return new RecursiveParseResult(
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
                    if (error != null) return new RecursiveParseResult(null, error);

                    if(directive.CanAddNewLines)
                    {
                        var newLines = directive.Apply(line, enableStack, defines, out ParseError parseError, RecursiveParse);
                        if (parseError != null) return new RecursiveParseResult(null, new ParseError(parseError.Type, index, fileName));
                        result.AddRange(newLines);
                    }
                    else
                    {
                        directive.Apply(line, enableStack, defines, out ParseError parseError);
                        if (parseError != null) return new RecursiveParseResult(null, new ParseError(parseError.Type, index, fileName));
                    }
                }
                else
                {
                    foreach (Define define in defines)
                    {
                        if(line.Contains(define.Name) && line.IndexOf(define.Name) < line.IndexOf(';'))
                        {
                            if(define.IsEmpty)
                            {
                                return new RecursiveParseResult(null, new ParseError(ParseErrorType.Preprocessor_ReferenceToEmptyDefine, index, fileName));
                            }

                            if (define.IsParametric)
                            {
                                Match match = ParametricDefine.ParametricUsageRegex.Match(line);
                                if (match.Success)
                                {
                                    var subStr = line.Substring(match.Index, match.Length);
                                    line = line.Remove(match.Index, match.Length);

                                    var newStr = (define as ParametricDefine).Expand(subStr, out ParseError parseError);
                                    if(parseError != null) return new RecursiveParseResult(null, new ParseError(parseError.Type, index, fileName));

                                    line = line.Insert(match.Index, newStr);
                                }
                                else
                                {
                                    return new RecursiveParseResult(null, new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat, index, fileName));
                                }
                            }
                            else line = line.Replace(define.Name, define.Value);
                        }
                    }

                    if (!enableStack.Contains(false))
                    {
                        result.Add(new SourceLineInstruction(line)
                        {
                            LineIndex = index,
                            FileName = fileName,
                        });
                    }
                }

            }

            return new RecursiveParseResult(result, null); 
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

        public bool CanAddNewLines;

        public string Name;

        private static List<PreprocessorDirective> _preprocessorDirectives;

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

        protected abstract void Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error);
        protected abstract List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc);
    }
}
