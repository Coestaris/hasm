using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;
using System.IO;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    class PrepareTask : ParseTask
    {
        public override string Name => "Preparing data";

        protected override void InnerReset() { }

        private List<MemZoneFlashElement> SetupRegisters()
        {
            var result = new List<MemZoneFlashElement>();
            source.Machine.GetRegisterNames().ForEach(p =>
            {
                var a = new MemZoneFlashElementVariable((Integer)(source._varIndex++));
                source._variables.Add(new Variable(p, BaseIntegerType.CommonType.Base)
                {
                    FEReference = a
                });
                result.Add(a);
            });
            return result;
        }

        protected override void InnerRun()
        {
            source._lines = new List<SourceLine>();
            source._unknownLabelNameErrorList = new List<UnknownLabelNameError>();
            source._variables = new List<Variable>();
            source._namedConsts = new List<NamedConstant>();

            if (!File.Exists(source.BaseFilename))
            {
                var parseError = new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedFile);
                InnerEnd(true, parseError);
                return;
            }

            //Проверка валидности указанного имени рабочей директории
            if (source.WorkingDirectory == null)
            {
                source.WorkingDirectory = new FileInfo(source.BaseFilename).DirectoryName;
            }
            else
            {
                if (!Directory.Exists(source.WorkingDirectory))
                {
                    var parseError = new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedWorkingDirectory);
                    InnerEnd(true, parseError);
                    return;
                }
            }

            source.ParseResult = new List<MemZoneFlashElement>();

            //Заносим регистры в список переменных
            source.ParseResult.AddRange(SetupRegisters());

            InnerEnd(false, null);
        }
    }
}
