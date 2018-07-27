using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.IO;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    class LinkingTask : ParseTask
    {
        public override string Name => "Linking structure";

        protected override void InnerReset() { }

        private List<Class> PlainClassesList;
        private List<Function> PlainFunctionsList;

        private ParseError RecursiveAdd(Class baseClass)
        {
            //PlainClassesList.Add(baseClass);

            List<string> Names = new List<string>();
            foreach (var function in baseClass.Functions)
            {
                if (Names.Contains(function.Signature))
                {
                    return new ParseError(ParseErrorType.Directives_ClassWithThatNameAlreadyExists,
                        function.Directive.LineIndex, function.Directive.FileName);
                }
                Names.Add(function.Signature);

                PlainFunctionsList.Add(function);
            }

            Names = new List<string>();
            foreach (var innerClass in baseClass.InnerClasses)
            {
                if (Names.Contains(innerClass.Name))
                {
                    return new ParseError(ParseErrorType.Directives_ClassWithThatNameAlreadyExists,
                        innerClass.Directive.LineIndex, innerClass.Directive.FileName);
                }

                Names.Add(innerClass.Name);
                var error = RecursiveAdd(innerClass);
                if (error != null) return error;
            }
            return null;
        }

        protected override void InnerRun()
        {
            PlainFunctionsList = new List<Function>();
            List<string> Names = new List<string>();
            foreach (var item in source._structures)
            {
                if(Names.Contains(item.Name))
                {
                    InnerEnd(new ParseError(ParseErrorType.Directives_ClassWithThatNameAlreadyExists,
                        item.Directive.LineIndex, item.Directive.FileName));
                    return;
                }

                Names.Add(item.Name);
                var error = RecursiveAdd(item as Class);
                if(error != null)
                {
                    InnerEnd(error);
                    return;
                }
            }

            source._functions = PlainFunctionsList;
            InnerEnd();
        }
    }
}
