using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    public class Operator
    {
        public int Priority;
        public string OperatorString;
        public bool IsUnary;
        public bool IsBracketsRequider;

        public Func<Operand, Operand, long> BinaryFunc;
        public Func<Operand, long> UnaryFunc;

        public Regex FindRegex;

        public override string ToString()
        {
            return OperatorString;
        }

        public Operator(int priority, string operatorString, bool isBracketsRequider, Func<Operand, long> function)
        {
            Priority = priority;
            OperatorString = operatorString;
            IsUnary = true;
            IsBracketsRequider = isBracketsRequider;

            //Экранируем любой символ для регулярок
            string regexString = "";
            foreach (char c in operatorString)
                regexString += "\\" + c; 

            FindRegex = new Regex(regexString, RegexOptions.IgnoreCase);

            UnaryFunc = function;
        }

        public Operator(int priority, string operatorString, Func<Operand, Operand, long> function)
        {
            Priority = priority;
            OperatorString = operatorString;
            IsUnary = false;
            IsBracketsRequider = false;

            //Экранируем любой символ для регулярок
            string regexString = "";
            foreach (char c in operatorString)
                regexString += "\\" + c;

            FindRegex = new Regex(regexString, RegexOptions.IgnoreCase);

            BinaryFunc = function;
        }
    }
}
