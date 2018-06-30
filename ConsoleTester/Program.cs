using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTester
{
    class Program
    {

        public class ExpressionTest
        {
            public TimeSpan Parsed;

            public Expression expression;
            public long Result;

            public string ExpressionString;

            public ParseError Parse()
            {
                return Expression.Parse(ExpressionString, out expression);
            }

            public ExpressionTest(string expression)
            {
                Result = 0;
                Parsed = TimeSpan.Zero;

                ExpressionString = expression;
            }

            public ExpressionTest(Expression expression, long result)
            {
                DateTime now = DateTime.Now;

                this.expression = expression;

                Parsed = TimeSpan.FromMilliseconds((DateTime.Now - now).TotalMilliseconds);
                Result = result;
            }
        }

        public static string ToPrettyFormat(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return "< 0 ms";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            if (span.Seconds > 0)
                sb.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);
            if (span.Milliseconds > 0)
                sb.AppendFormat("{0} ms", span.Milliseconds);
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            /*
            List<ExpressionTest> expressions = new List<ExpressionTest>()
            {
                //Primitive
                new ExpressionTest(new Expression(@"2"), 2),
                new ExpressionTest(new Expression(@"(((1234567_q)))"), 1234567),

                //Unary operators
                new ExpressionTest(new Expression(@"-2"), -2),
                new ExpressionTest(new Expression(@"2 + (-2)"), 0),
                new ExpressionTest(new Expression(@"~2"), -3),
                new ExpressionTest(new Expression(@"!(3 && 1)"), 1),
                new ExpressionTest(new Expression(@"!(1 && (1 || 0 || 0 || 1))"), 0),
                new ExpressionTest(new Expression(@"5 + ~2"), 2),
                new ExpressionTest(new Expression(@"~(~2)"), ~(~2)),
                new ExpressionTest(new Expression(@"~(~1234 - 123)"), ~(~1234 - 123)),

                //Binary Operators
                new ExpressionTest(new Expression(@"3 << 1"), 6),
                new ExpressionTest(new Expression(@"3 || 1"), 1),
                new ExpressionTest(new Expression(@"3 || 6"), 0),
                new ExpressionTest(new Expression(@"1 && (1 || 0 || 0 || 1)"), 1),
                new ExpressionTest(new Expression(@"0 && (1 || 0 || 0 || 1)"), 0),
                new ExpressionTest(new Expression(@"2 * (2 + 2)"), 8),
                new ExpressionTest(new Expression(@"4 - 2 + 2 / 2 * 4 - 1- 2 - 4 - 5"), -6),
                new ExpressionTest(new Expression(@"2 + (3 - 3 * ( 3 + 4 ) / 3)"), -2),
                new ExpressionTest(new Expression(@"4 & 2 ^ 4 - 2 + 1"), 3),
                
                //Conditional
                new ExpressionTest(new Expression(@"1 ? 2 : 3"), 2),
                new ExpressionTest(new Expression(@"0 ? 2 : 3"), 3),
                new ExpressionTest(new Expression(@"0 ? 2 + 4 : 3 - 1"), 2),
                new ExpressionTest(new Expression(@"4 % 2 == 0 ? 2 + 4 : 3 - 1"), 6),
                
                //Functions
                new ExpressionTest(new Expression(@"low(323)"), 67),
                new ExpressionTest(new Expression(@"double(double(2))"), 8),
                new ExpressionTest(new Expression(@"double(double(double(2)))"), 16),
                
                //Mixed
                new ExpressionTest(new Expression(@"~double(2)"), ~4),
                new ExpressionTest(new Expression(@"double(~2)"), (~2) * 2),
                new ExpressionTest(new Expression(@"double(double(2) + 1)"), 10),
                new ExpressionTest(new Expression(@"double(double(2 + 1))"), 12),
                new ExpressionTest(new Expression(@"double(4) + double(5 * 2) - 1"), 27),
                new ExpressionTest(new Expression(@"double(2+double(2+double(2+double(2+double(2)))))"), 124),
                new ExpressionTest(new Expression(@"(~2- ~2 + ~2 - ~2) || 1"), 1),
                new ExpressionTest(new Expression(@"1 && (1 || 0 || 0 || 1) ? double(3) : ~3 << 1"), 6),
                new ExpressionTest(new Expression(@"0 && (1 || 0 || 0 || 1) ? double(3) : ~3 << 1"), ~3 << 1),
            };
            foreach (var item in expressions)
            {
                DateTime now = DateTime.Now;
                var result = item.expression.Calculate();
                var calculated = TimeSpan.FromMilliseconds((DateTime.Now - now).TotalMilliseconds);

                if(result.Value == item.Result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("PASSED");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" :: Test: \"{0}\". Result: {1}. Parsed: {2}. Calculated: {3}",
                        item.expression.Value, result, ToPrettyFormat(item.Parsed), ToPrettyFormat(calculated));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FAILED");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" :: Test: \"{0}\". Expected: {1}. Result: {2}. Parsed: {3}. Calculated: {4}", 
                        item.expression.Value, item.Result, result, ToPrettyFormat(item.Parsed), ToPrettyFormat(calculated));
                }

            }*/

            Expression.InitGlobals();
            var a = Expression.Parse("2 + 2 + 10", out Expression expression);
            if(a == null)
            {
                Console.WriteLine(expression.Calculate(true));
                Console.WriteLine(expression.Calculate(false));
                Console.WriteLine(expression.Calculate(true));

            }

            Console.ReadKey();
        }
    }
}
