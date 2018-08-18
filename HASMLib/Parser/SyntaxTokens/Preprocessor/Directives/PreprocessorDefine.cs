using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
{
    internal class PreprocessorDefine : PreprocessorDirective
    {
        public PreprocessorDefine()
        {
            Name = "define";
            CanAddNewLines = false;
        }

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            string firstLine = ClearInput(input.AsSingleLine());
            if (string.IsNullOrEmpty(firstLine))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            string[] parts = firstLine.Split(' ');
            string name = parts[0];

            if (name.Contains('(') || name.Contains(')') || name.Contains(','))
            {
                try
                {
                    if (!ParametricDefine.ParametricDefineRegex.IsMatch(name))
                    {
                        error = new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat);
                        return;
                    }
                }
                catch
                {
                    error = new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat);
                    return;
                }

                if (parts.Length == 1 && input.IsSingleLine)
                {
                    error = new ParseError(ParseErrorType.Preprocessor_ParametricDefineWithoutExpression);
                    return;
                }

                var firstValueLine = string.Join(")", firstLine.Split(')').Skip(1).ToArray());
                var group = new StringGroup(firstValueLine);
                group.Strings.AddRange(input.Strings.Skip(1));

                var newDef = new ParametricDefine(name, group);

                if (defines.Exists(p => p.Name == newDef.Name))
                {
                    error = new ParseError(ParseErrorType.Preprocessor_DefineNameAlreadyExists);
                    return;
                }

                var value = newDef.Value;
                Define.ResolveDefines(defines, ref value, -1, null);
                newDef.Value = value;
                defines.Add(newDef);
            }
            else
            {

                if (!Define.GeneralDefineNameRegex.IsMatch(name))
                {
                    error = new ParseError(ParseErrorType.Preprocessor_WrongDefineName);
                    return;
                }

                if (defines.Exists(p => p.Name == Name))
                {
                    error = new ParseError(ParseErrorType.Preprocessor_DefineNameAlreadyExists);
                    return;
                }

                if (parts.Length == 1)
                {
                    defines.Add(new Define(name));
                }
                else
                {
                    var firstValueLine = string.Join(" ", parts.Skip(1).ToArray());
                    var group = new StringGroup(firstValueLine);
                    group.Strings.AddRange(input.Strings.Skip(1));

                    var newDef = new Define(name, group);
                    var value = newDef.Value;
                    Define.ResolveDefines(defines, ref value, -1, null);
                    newDef.Value = value;
                    defines.Add(newDef);
                }
            }
            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}