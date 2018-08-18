using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    class ConstantStringFormat : ConstantFormat
    {
        public override Regex Regex => new Regex("\\\".*\\\"");

        public Dictionary<char, Integer> EscapeSymbols = new Dictionary<char, Integer>()
        {
            { '0', new Integer(0, BaseIntegerType.CommonCharType) },
            { 't', new Integer(9, BaseIntegerType.CommonCharType) },
            { 'b', new Integer(8, BaseIntegerType.CommonCharType) },
            { 'a', new Integer(7, BaseIntegerType.CommonCharType) },
            { 'n', new Integer(10, BaseIntegerType.CommonCharType) },
            { 'v', new Integer(11, BaseIntegerType.CommonCharType) },
            { 'f', new Integer(12, BaseIntegerType.CommonCharType) },
            { 'r', new Integer(13, BaseIntegerType.CommonCharType) },
            { '"',  new Integer(34, BaseIntegerType.CommonCharType) },
            { '\'', new Integer(39, BaseIntegerType.CommonCharType) },
            { '\\', new Integer(92, BaseIntegerType.CommonCharType) }
        };

        private const char EscapeChar = '\\';

        public override ParseError Parse(string str, out Constant constant)
        {
            if(str.First() != '"'  || str.Last() != '"')
            {
                constant = null;
                return new ParseError(ParseErrorType.Syntax_Constant_WrongFormat);
            }

            str = str.Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);

            bool lastSybmolIsEscapeChar = false;
            List<Integer> integers = new List<Integer>();
            
            for(int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == EscapeChar)
                {
                    if (!EscapeSymbols.ContainsKey(str[i + 1]))
                    {
                        constant = null;
                        return new ParseError(ParseErrorType.Syntax_Constant_WrongEscapeSymbol);
                    }
                    integers.Add(EscapeSymbols[str[i + 1]]);
                    lastSybmolIsEscapeChar = true;
                }
                else
                {
                    if(!lastSybmolIsEscapeChar)
                        integers.Add(new Integer(str[i], BaseIntegerType.CommonCharType));

                    lastSybmolIsEscapeChar = false;
                }
            }

            if (!lastSybmolIsEscapeChar && str.Length != 0)
                integers.Add(new Integer(str[str.Length - 1], BaseIntegerType.CommonCharType));

            constant = new Constant(new Array(StringType.DefaultStringType));
            constant.ArrayValue.Collection = integers.Select(p => new Object(p, null)).ToList();
            return null;
        }
    }
}
