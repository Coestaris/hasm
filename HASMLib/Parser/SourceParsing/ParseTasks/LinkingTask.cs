using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    class LinkingTask : ParseTask
    {
        public override string Name => "Linking structure";

        protected override void InnerReset() { }

        private List<Class> PlainClassesList;
        private List<Field> PlainFiledsList;
        private List<Function> PlainFunctionsList;
        private int classID = 0;
        private int fieldID = 0;
        private int functionID = 0;

        private ParseError ReferenceCheck()
        {
            foreach (var function in PlainFunctionsList)
            {
                foreach (var parameter in function.Parameters)
                    if (!parameter.Type.CheckClassType(PlainClassesList, source.Assembly))
                        return new ParseError(ParseErrorType.Directives_WrongTypeReference,
                            function.Directive.LineIndex, function.Directive.FileName);

                if (!function.RetType.CheckClassType(PlainClassesList, source.Assembly))
                    return new ParseError(ParseErrorType.Directives_WrongTypeReference,
                        function.Directive.LineIndex, function.Directive.FileName);
            }

            foreach (var field in PlainFiledsList)
                if (!field.Type.CheckClassType(PlainClassesList, source.Assembly))
                    return new ParseError(ParseErrorType.Directives_WrongTypeReference,
                        field.Directive.LineIndex, field.Directive.FileName);

            return null;
        }

        private ParseError CheckNames()
        {
            List<string> funcNames = new List<string>();
            List<string> fieldNames = new List<string>();
            List<string> classNames = new List<string>();

            foreach (var function in PlainFunctionsList)
            {
                if (funcNames.Contains(function.Signature))
                {
                    return new ParseError(ParseErrorType.Directives_FunctionWithThatNameAlreadyExists,
                        function.Directive.LineIndex, function.Directive.FileName);
                }
                funcNames.Add(function.Signature);

                if (function.IsEntryPoint)
                {
                    if (source.Assembly._entryPoint != null)
                        return new ParseError(ParseErrorType.Directives_MoreThanOneEntryPointDeclared,
                        function.Directive.LineIndex, function.Directive.FileName);

                    source.Assembly._entryPoint = function;
                }
            }

            foreach (var field in PlainFiledsList)
            {
                if (fieldNames.Contains(field.FullName))
                {
                    return new ParseError(ParseErrorType.Directives_FieldWithThatNameAlreadyExists,
                        field.Directive.LineIndex, field.Directive.FileName);
                }
                fieldNames.Add(field.FullName);
            }

            foreach (var Class in PlainClassesList)
            {
                if (classNames.Contains(Class.FullName))
                {
                    return new ParseError(ParseErrorType.Directives_ClassWithThatNameAlreadyExists,
                        Class.Directive.LineIndex, Class.Directive.FileName);
                }
                classNames.Add(Class.FullName);
            }

            return null;
        }

        private void BuildPlainLists(Class baseClass)
        {
            PlainClassesList.Add(baseClass);
            baseClass.UniqueID = classID++;

            foreach (var function in baseClass.Constructors)
            {
                PlainFunctionsList.Add(function);
                function.UniqueID = functionID++;
            }

            foreach (var function in baseClass.Functions)
            {
                PlainFunctionsList.Add(function);
                function.UniqueID = functionID++;
            }

            foreach (var field in baseClass.Fields)
            {
                PlainFiledsList.Add(field);
                field.UniqueID = fieldID++;
            }

            foreach (var innerClass in baseClass.InnerClasses)
            {
                BuildPlainLists(innerClass);
            }
        }

        protected override void InnerRun()
        {
            classID = 1;
            fieldID = 0;
            functionID = 0;
            PlainClassesList = new List<Class>();
            PlainFiledsList = new List<Field>();
            PlainFunctionsList = new List<Function>();

            foreach (var item in source.Assembly.Classes)
                BuildPlainLists(item as Class);

            var parseError = CheckNames();
            if (parseError != null)
            {
                InnerEnd(parseError);
                return;
            }

            parseError = ReferenceCheck();
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

            source.Assembly.AllFields = PlainFiledsList;
            source.Assembly.AllClasses = PlainClassesList;
            source.Assembly.AllFunctions = PlainFunctionsList;
            InnerEnd();
        }
    }
}
