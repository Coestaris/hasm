using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorDefine: PreprocessorDirective
    {
        public PreprocessorDefine()
        {
            Name = "define";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error)
        {
            input = ClearInput(input);
            if(string.IsNullOrEmpty(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            string[] parts = input.Split(' ');
            string name = parts[0];

            if(name.Contains('(') || name.Contains(')') || name.Contains(','))
            {
                if (!ParametricDefine.ParametricDefineRegex.IsMatch(name))
                {
                    error = new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat);
                    return;
                }

                if(parts.Length == 1)
                {
                    error = new ParseError(ParseErrorType.Preprocessor_ParametricDefineWithoutExpression);
                    return;
                }

                var newDef = new ParametricDefine(name, string.Join(" ", parts.Skip(1)));

                if(defines.Exists(p => p.Name == newDef.Name))
                {
                    error = new ParseError(ParseErrorType.Preprocessor_DefineNameAlreadyExists);
                    return;
                }

                Define.ResolveDefines(defines, ref newDef.Value, -1, null);
                defines.Add(newDef);
                
                error = null;
                return;
            }

            if(!Define.GeneralDefineNameRegex.IsMatch(name))
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
                var newDef = new Define(name, string.Join(" ", parts.Skip(1)));
                Define.ResolveDefines(defines, ref newDef.Value, -1, null);
                defines.Add(newDef);
            }

            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
