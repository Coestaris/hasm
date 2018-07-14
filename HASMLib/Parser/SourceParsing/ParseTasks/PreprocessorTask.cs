using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class PreprocessorTask : ParseTask
    {
        public override string Name => "Resolving preprocessor";

        protected override void InnerReset() { }

        private static Regex multipleSpaceRegex = new Regex(@"[ \t]{1,}");
        private static Regex commaSpaceRegex = new Regex(@",[ \t]{1,}");
        private const string PrepareSourceSpaceReplace = " ";
        private const string PrepareSourceMultiCommaReplace = ",";

        private List<string> BasePrepareLines(string absoluteFileName)
        {
            string input = File.ReadAllText(absoluteFileName);

            input = multipleSpaceRegex.Replace(input, PrepareSourceSpaceReplace);
            input = commaSpaceRegex.Replace(input, PrepareSourceMultiCommaReplace);

            return input.Split('\n').Select(p => p.Trim('\r', '\t')).ToList();
        }

        protected override void InnerRun()
        {
            List<SourceLine> lines = PreprocessorDirective.RecursiveParse(source.BaseFilename, 
                source.WorkingDirectory, out ParseError parseError, BasePrepareLines, source.Machine.UserDefinedDefines);

            source._lines = lines;

            if (parseError != null)
                InnerEnd(true, parseError);
            else InnerEnd(false, null);
        }
    }
}
