using HASMLib.Runtime.Structures;
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
                        return new ParseError(ParseErrorType.Directives_WrongTypeReference, function.Directive);

                if (!function.RetType.CheckClassType(PlainClassesList, source.Assembly))
                    return new ParseError(ParseErrorType.Directives_WrongTypeReference, function.Directive);
            }

            foreach (var field in PlainFiledsList)
                if (!field.Type.CheckClassType(PlainClassesList, source.Assembly))
                    return new ParseError(ParseErrorType.Directives_WrongTypeReference, field.Directive);

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
                    return new ParseError(ParseErrorType.Directives_FunctionWithThatNameAlreadyExists, function.Directive);
                }

                funcNames.Add(function.Signature);

                if (function.IsEntryPoint)
                {
                    if (source.Assembly._entryPoint != null)
                        return new ParseError(ParseErrorType.Directives_MoreThanOneEntryPointDeclared, function.Directive);

                    source.Assembly._entryPoint = function;
                }
            }

            foreach (var field in PlainFiledsList)
            {
                if (fieldNames.Contains(field.Signature))
                {
                    return new ParseError(ParseErrorType.Directives_FieldWithThatNameAlreadyExists, field.Directive);
                }

                fieldNames.Add(field.Signature);

                if (field.IsStatic)
                    field.BaseClass.StaticFields.Add(field.UniqueID,
                        new Object(field.Type));
            }

            foreach (var Class in PlainClassesList)
            {
                if (classNames.Contains(Class.FullName))
                {
                    return new ParseError(ParseErrorType.Directives_ClassWithThatNameAlreadyExists, Class.Directive);
                }
                classNames.Add(Class.FullName);

                var extends = Class.Modifiers.FindAll(p => p.Name == Class.ExtendsKeyword);
                foreach(var extend in extends)
                {
                    var type = new TypeReference(extend.Value, source.Assembly);
                    if (!type.CheckClassType(PlainClassesList, source.Assembly))
                        return new ParseError(ParseErrorType.Directives_WrongTypeReference, Class.Directive);

                    if (type.Type != TypeReferenceType.Class)
                        return new ParseError(ParseErrorType.Directives_ClassNameExpected, Class.Directive);

                    Class.Extends.Add(type.ClassType);
                    Class.Functions.AddRange(type.ClassType.Functions);
                    Class.Fields.AddRange(type.ClassType.Fields);
                }
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
            if(source.Assembly == null)
            {
                InnerEnd();
                return;
            }

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
                InnerEnd(new ParseError(ParseErrorType.Directives_NoEntryPointFound, source.Assembly.Directive));
                return;
            }

            source.Assembly.AllFields = PlainFiledsList;
            source.Assembly.AllClasses = PlainClassesList;
            source.Assembly.AllFunctions = PlainFunctionsList;
            InnerEnd();
        }
    }
}
