﻿using HASMLib.Core.BaseTypes;
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

        private List<FlashElement> SetupRegisters()
        {
            /*var result = new List<MemZoneFlashElement>();
            source.Machine.GetRegisterNames().ForEach(p =>
            {
                var a = new MemZoneFlashElementVariable((Integer)(source._varIndex++));
                source._variables.Add(new Variable(p, BaseIntegerType.CommonType.Base)
                {
                    FEReference = a
                });
                result.Add(a);
            });
            return result;*/
            return new List<FlashElement>();
        }

        protected override void InnerRun()
        {
            //source._lines = new List<SourceLine>();
            //source._unknownLabelNameErrorList = new List<UnknownLabelNameError>();
            //source._variables = new List<Variable>();
            //source._namedConsts = new List<NamedConstant>();

            if(!string.IsNullOrEmpty(source.Source) && string.IsNullOrEmpty(source.BaseFilename))
            {
                source.BaseFilename = Path.GetTempFileName();
                File.WriteAllText(source.BaseFilename, source.Source);
            }

            if (!File.Exists(source.BaseFilename))
            {
                var parseError = new ParseError(ParseErrorType.IO_UnabletoFindSpecifiedFile);
                InnerEnd(parseError);
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
                    InnerEnd(parseError);
                    return;
                }
            }

            //source.ParseResult = new List<MemZoneFlashElement>();

            //Заносим регистры в список переменных
            //source.ParseResult.AddRange(SetupRegisters());

            InnerEnd();
        }
    }
}
