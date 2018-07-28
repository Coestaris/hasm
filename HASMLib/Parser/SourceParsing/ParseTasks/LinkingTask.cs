using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Runtime.Structures;
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

        private bool IsTypeOk(TypeReference Type)
        {
            if (Type.IsVoid) return true;
            if (Type.IsBaseInteger) return true;
            if (Type.IsClass) return true;

            Class Class = PlainClassesList.Find(p => p.FullName == source.Assembly.Name + Class.NameSeparator + Type.Name);
            if (Class == null) return false;

            Type.IsClass = true;
            Type.ClassType = Class;
            return true;
        }

        private ParseError ReferenceCheck()
        {
            foreach (var function in PlainFunctionsList)
            {
                foreach (var parameter in function.Parameters)
                    if (!IsTypeOk(parameter.Type))
                        return new ParseError(ParseErrorType.Directives_WrongTypeReference,
                            function.Directive.LineIndex, function.Directive.FileName);

                if (!IsTypeOk(function.RetType))
                    return new ParseError(ParseErrorType.Directives_WrongTypeReference,
                        function.Directive.LineIndex, function.Directive.FileName);
            }

            return null;
        }

        private ParseError RecursiveAdd(Class baseClass)
        {
            PlainClassesList.Add(baseClass);
            List<string> Names = new List<string>();
            foreach (var function in baseClass.Functions)
            {
                if (Names.Contains(function.Signature))
                {
                    return new ParseError(ParseErrorType.Directives_FunctionWithThatNameAlreadyExists,
                        function.Directive.LineIndex, function.Directive.FileName);
                }
                Names.Add(function.Signature);

                if(function.IsEntryPoint)
                {
                    if(source.Assembly._entryPoint != null)
                        return new ParseError(ParseErrorType.Directives_MoreThanOneEntryPointDeclared,
                        function.Directive.LineIndex, function.Directive.FileName);

                    source.Assembly._entryPoint = function;
                }

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
            PlainClassesList = new List<Class>();
            PlainFunctionsList = new List<Function>();
            List<string> Names = new List<string>();
            foreach (var item in source.Assembly.Classes)
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

            var parseError = ReferenceCheck();
            if (parseError != null)
            {
                InnerEnd(parseError);
                return;
            }

            if (source.Assembly._entryPoint == null)
            {
                InnerEnd(new ParseError(ParseErrorType.Directives_NoEntryPointFound,
                        source.Assembly.Directive.LineIndex, source.Assembly.Directive.FileName));
                return;
            }

            source.Assembly.AllClasses = PlainClassesList;
            source.Assembly.AllFunctions = PlainFunctionsList;
            InnerEnd();
        }
    }
}
