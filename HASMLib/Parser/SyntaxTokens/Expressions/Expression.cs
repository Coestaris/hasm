using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    public class Expression
    {
        public static List<char> OperatorCharaters;

        public static List<Operator> Operators = new List<Operator>()
        {
            //Unary 
            new Operator(14, "!", false, (a) => a.Value == 1 ? 1 : 0),
            new Operator(14, "~", false, (a) => ~ a.Value),
            new Operator(14, "-", false, (a) => - a.Value),

            //Binnary
            new Operator(13, "*", (a, b) => a.Value * b.Value),
            new Operator(13, "/", (a, b) => a.Value / b.Value),

            new Operator(12, "+", (a, b) => a.Value + b.Value),
            new Operator(12, "-", (a, b) => a.Value - b.Value),

            new Operator(11, "<<", (a, b) => a.Value << (int)b.Value),
            new Operator(11, ">>", (a, b) => a.Value >> (int)b.Value),

            new Operator(10, "<", (a, b) => a.Value < b.Value ? 1 : 0),
            new Operator(10, "<=", (a, b) => a.Value <= b.Value ? 1 : 0),
            new Operator(10, ">", (a, b) => a.Value > b.Value ? 1 : 0),
            new Operator(10, ">=", (a, b) => a.Value >= b.Value ? 1 : 0),

            new Operator(9, "!=", (a, b) => a.Value != b.Value ? 1 : 0),
            new Operator(9, "==", (a, b) => a.Value == b.Value ? 1 : 0),

            new Operator(8, "&", (a, b) => a.Value & b.Value),
            new Operator(7, "^", (a, b) => a.Value ^ b.Value),
            new Operator(6, "|", (a, b) => a.Value | b.Value),
            new Operator(5, "&&", (a, b) => a.AsBool && b.AsBool ? 1 : 0),
            new Operator(4, "||", (a, b) => a.AsBool || b.AsBool ? 1 : 0)
        };

        public string Value;

        public Token TokenTree;
        
        private List<Token> CreateTokenTree(string input, Token parentToken)
        {
            if (parentToken == null)
                parentToken = new Token(input);

            string currentToken = "";
            string currentOperator = "";
            bool lastCharWasOperator = false;
            List<Token> tokens = new List<Token>();
            List<string> operators = new List<string>();

            int bracketCount = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    if (lastCharWasOperator)
                    {
                        operators.Add(currentOperator);
                        currentOperator = "";
                        lastCharWasOperator = false;
                    }

                    bracketCount++;
                }
                if (input[i] == ')')
                {
                    bracketCount--;

                    if (bracketCount == 0)
                    {
                        if (currentToken != "")
                        {
                            tokens.Add(new Token(AccurateBracketTrim(currentToken + ")")));
                            currentToken = "";
                        }
                        continue;
                    }
                }

                if (bracketCount == 0)
                {
                    if (OperatorCharaters.Contains(input[i]))
                    {
                        currentOperator += input[i];

                        if (!lastCharWasOperator && currentToken != "")
                        {
                            tokens.Add(new Token(AccurateBracketTrim(currentToken)));
                            currentToken = "";
                        }
                        lastCharWasOperator = true;
                    }
                    else
                    {
                        if (currentOperator != "")
                            operators.Add(currentOperator);
                        currentOperator = "";

                        currentToken += input[i];
                        lastCharWasOperator = false;
                    }
                }
                else currentToken += input[i];
            }

            if (currentToken != "")
                tokens.Add(new Token(currentToken));

            Console.WriteLine("============");
            Console.WriteLine("Input    : {0}", input);
            Console.WriteLine("Tokens   : {0}", string.Join(",", tokens.Select(p => p.RawValue)));
            Console.WriteLine("Operators: {0}",string.Join(",", operators));

            foreach (var item in tokens)
            {
                if (!item.IsSimple)
                    item.Subtokens = CreateTokenTree(AccurateBracketTrim(item.RawValue), item);
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                //Самый левый токен
                if (i == 0 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = null;
                    tokens[i].LeftSideToken = null;
                    tokens[i].RightSideOperator = FindOperator(operators[i]);
                    tokens[i].RightSideToken = tokens[i + 1];
                }
                //Самый правый токен
                else if(i != 0 && i == tokens.Count - 1 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1]);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = null;
                    tokens[i].RightSideToken = null;
                }
                else if(i != 0 && tokens.Count != 1)
                {
                    tokens[i].LeftSideOperator = FindOperator(operators[i - 1]);
                    tokens[i].LeftSideToken = tokens[i - 1];
                    tokens[i].RightSideOperator = FindOperator(operators[i]);
                    tokens[i].RightSideToken = tokens[i + 1];
                } else if (i == 0 && tokens.Count == 1)
                {
                    tokens[i].LeftSideOperator = null;
                    tokens[i].LeftSideToken = null;
                    tokens[i].RightSideOperator = null;
                    tokens[i].RightSideToken = null;
                }
            }

            parentToken.Subtokens = tokens;

            return tokens;
        }

        private Token ParseToken(string input)
        {
            Token token = new Token(input);
            CreateTokenTree(input, token);
            return token;
        }

        private Operator FindOperator(string name)
        {
            var a = Operators.Find(p => p.OperatorString == name);
            return a ?? throw new Exception("Unknown operator");
        }

        private string AccurateBracketTrim(string input)
        {
            while ((input[0] == '(' && input[input.Length - 1] == ')'))
                input = input.Remove(0, 1).Remove(input.Length - 2, 1);

            return input;
        }

        private void Calculate(Token token)
        {
            if (token.CanBeCalculated)
            {
                token.Calculate();
                return;
            }

            foreach (Token subToken in token.Subtokens)
            {
                Calculate(subToken);
            }
        }

        public long Calculate()
        {
            Calculate(TokenTree);

            if (!TokenTree.CanBeCalculated)
                throw new Exception("Still cant caclulate tree");

            return TokenTree.Calculate();
        }

        public Expression(string input)
        {
            OperatorCharaters = new List<char>();
            foreach (Operator op in Operators)
            {
                foreach (char c in op.OperatorString)
                {
                    if (!OperatorCharaters.Contains(c))
                        OperatorCharaters.Add(c);
                }
            }
            input = input.Replace(" ", "");

            TokenTree = ParseToken(input);
        }
    }
}
