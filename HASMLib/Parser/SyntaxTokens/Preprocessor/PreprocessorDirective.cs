﻿using HASMLib.Core;
using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using HASMLib.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    public abstract class PreprocessorDirective
    {
        private const char DirectiveBeginChar = '#';

        private static Regex GeneralPreprocessorRegex = new Regex(@"^#[^#]{1,}$");
        private static List<PreprocessorDirective> _preprocessorDirectives;
        private static Func<string, List<StringGroup>> getLinesFunc;
        private static string workingDirectory;
        private static Stack<bool> enableStack;
        internal static List<Define> defines;
        private static Cache Cache;

        public bool CanAddNewLines { get; protected set; }
        public string Name { get; protected set; }

        public string ClearInput(string input)
        {
            var match = SourceLine.CommentRegex.Match(input);

            if (match.Success)
                input = input.Remove(match.Index, match.Length);

            return input.TrimStart(DirectiveBeginChar).Remove(0, Name.Length).Trim(
                SourceLine.StringCleanUpChars);
        }

        public static bool IsPreprocessorLine(string line)
        {
            return line.StartsWith(DirectiveBeginChar.ToString());
        }

        public static List<SourceLine> RecursiveParse(string fileName, string WorkingDirectory, out ParseError error, Func<string, List<StringGroup>> GetLinesFunc, List<Define> defines, Cache cache)
        {
            Cache = cache;

			if (GetLinesFunc == null)
				new ArgumentNullException (nameof (GetLinesFunc));
			
			getLinesFunc = GetLinesFunc;
            workingDirectory = WorkingDirectory;
            enableStack = new Stack<bool>();
            PreprocessorDirective.defines = new List<Define>();

            if (defines != null)
                PreprocessorDirective.defines.AddRange(defines);

            //TODO: USER DEFINED CONSTS!
            PreprocessorDirective.defines.Add(new Define("__base__", new StringGroup(HASMBase.Base.ToString())));

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
            input = input.TrimStart(DirectiveBeginChar);
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
                fileName = new FileInfo(Path.Combine(workingDirectory, fileName)).FullName;

            if (!File.Exists(fileName))
            {
                return new PreprocessorParseResult(
                    null,
                    new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedFile, -1, fileName));
            }

            if(Cache.FileCache.ContainsKey(fileName))
            {
                FileCache cache = Cache.FileCache[fileName];
                if (cache.CompiledDefines != null)
                    defines.AddRange(cache.CompiledDefines);

                return new PreprocessorParseResult(new List<SourceLine>(), null);
            }

            List<StringGroup> stringGroups = getLinesFunc(fileName);

            int index = -1;
            for(int i = 0; i < stringGroups.Count; i++)
            {
                StringGroup group = stringGroups[i];

                if(!group.IsSingleLine && !group.IsMultilineDefine)
                    return new PreprocessorParseResult(null, new ParseError(ParseErrorType.Preprcessor_MultilineNonDefinesAreNotAllowed, index, fileName));

                if(group.IsMultilineDefine)
                {
					ParseError error;
                    PreprocessorDirective directive = GetDirective(group.Strings.First(), index, fileName, out error);
                    if (error != null) return new PreprocessorParseResult(null, error);

					ParseError parseError;
                    //Preprocessor.Directives.PreprocessorDefine.
                    directive.Apply(group, enableStack, defines, out parseError);
                    if (parseError != null) return new PreprocessorParseResult(null, new ParseError(parseError.Type, index, fileName));

                    index += group.Strings.Count;
                }

                if(group.IsSingleLine)
                {
                    index++;

                    string line = group.AsSingleLine();
                    if (IsPreprocessorLine(line))
                    {
						ParseError error;
                        PreprocessorDirective directive = GetDirective(line, index, fileName, out error);
                        if (error != null) return new PreprocessorParseResult(null, error);

                        if (directive.CanAddNewLines)
                        {
							ParseError parseError;
                            var newLines = directive.Apply(new StringGroup(line), enableStack, defines, out parseError, RecursiveParse);
                            if (parseError != null) return new PreprocessorParseResult(null, parseError);
                            result.AddRange(newLines);
                        }
                        else
                        {
							ParseError parseError;
                            directive.Apply(new StringGroup(line), enableStack, defines, out parseError);
                            if (parseError != null) return new PreprocessorParseResult(null, new ParseError(parseError.Type, index, fileName));
                        }
                    }
                    else
                    {
                        if (!enableStack.Contains(false))
                        {
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            ParseError parseError = Define.ResolveDefines(defines, ref group, index, fileName);
                            if (parseError != null) return new PreprocessorParseResult(null, parseError);

                            group.Strings.ForEach(p =>
                            {
                                result.Add(new SourceLine(p, index, fileName));
                            });
                        }
                    }
                }
                else
                {
                    //STUB
                }
            }
            return new PreprocessorParseResult(result, null);
        }

        internal abstract void Apply(StringGroup input, Stack<bool> enableList, List<Define> defines, out ParseError error);

        internal abstract List<SourceLine> Apply(StringGroup input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc);
    }
}
