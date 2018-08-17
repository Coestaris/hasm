using HASMLib.Parser.SyntaxTokens.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class PreprocessorTask : ParseTask
    {
        public override string Name => "Resolving preprocessor";

        protected override void InnerReset() { }

        private const char MultilineSymbol = '\\';
        private static Regex multipleSpaceRegex = new Regex(@"[ \t]{1,}");
        private static Regex commaSpaceRegex = new Regex(@",[ \t]{1,}");
        private const string PrepareSourceSpaceReplace = " ";
        private const string PrepareSourceMultiCommaReplace = ",";

        private List<List<string>> BasePrepareLines(string absoluteFileName)
        {
            string input = File.ReadAllText(absoluteFileName);

            input = multipleSpaceRegex.Replace(input, PrepareSourceSpaceReplace);
            input = commaSpaceRegex.Replace(input, PrepareSourceMultiCommaReplace);

            List<List<string>> result = new List<List<string>>();
            var rawLines = input.Split('\n').Select(p => p.Trim(SourceLine.StringCleanUpChars));

            foreach (var line in rawLines)
            {
                if (line.Last() == MultilineSymbol)
                {
                    result.Last().Add(line.TrimEnd(MultilineSymbol));
                }
                else
                {
                    result.Add(new List<string>()
                    {
                        line
                    });
                }
            }

            return result;
        }

        protected override void InnerRun()
        {
            List<SourceLine> lines = PreprocessorDirective.RecursiveParse(source.BaseFilename,
                source.WorkingDirectory, out ParseError parseError, BasePrepareLines, source.Machine.UserDefinedDefines);

            source._lines = lines;

            if (parseError != null)
                InnerEnd(parseError);
            else InnerEnd();
        }
    }
}
