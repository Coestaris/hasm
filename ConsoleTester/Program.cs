using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
    class Program
    {

        public struct ExpressionTest
        {
            public TimeSpan Parsed;

            public Expression expression;
            public long Result;

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
            if (span == TimeSpan.Zero) return "0 minutes";

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
            List<ExpressionTest> expressions = new List<ExpressionTest>()
            {
                new ExpressionTest(new Expression(@"2"), 2),
                new ExpressionTest(new Expression(@"3 << 1"), 6),
                new ExpressionTest(new Expression(@"3 || 1"), 1),
                new ExpressionTest(new Expression(@"3 || 6"), 0),
                new ExpressionTest(new Expression(@"1 && (1 || 0 || 0 || 1)"), 1),
                new ExpressionTest(new Expression(@"0 && (1 || 0 || 0 || 1)"), 1),
                new ExpressionTest(new Expression(@"2 * (2 + 2)"), 8),
                new ExpressionTest(new Expression(@"4 - 2 + 2 / 2 * 4 - 1- 2 - 4 - 5"), -6),
                new ExpressionTest(new Expression(@"2 + (3 - 3 * ( 3 + 4 ) / 3)"), -2),
            };

            foreach (var item in expressions)
            {
                DateTime now = DateTime.Now;
                var result = item.expression.Calculate();
                var calculated = TimeSpan.FromMilliseconds((DateTime.Now - now).TotalMilliseconds);

                Console.WriteLine("Test: {0}. Expected: {1}. Result: {2}. Parsed: {3}. Calculated: {4}", 
                    item.expression.Value, item.Result, result, ToPrettyFormat(item.Parsed), ToPrettyFormat(calculated));
            }

            Console.ReadKey();
        }
    }
}
