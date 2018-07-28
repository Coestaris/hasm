using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Runtime.Structures.Units
{
    public class Function : BaseStructure
    {
        public const string StaticKeyword = "static";
        public const string EntryPointKeyword = "entrypoint";

        public const string ReturnKeyword = "ret";
        public const string ParameterKeyword = "param";
        public const string NoReturnableValueKeyword = "void";

        internal List<UnknownLabelNameError> _unknownLabelNameErrorList;
        internal List<Variable> _variables;
        internal List<NamedConstant> _namedConsts;
        internal int _constIndex;
        internal int _expressionIndex;
        internal int _varIndex;
        internal int _instructionIndex;

        public Class BaseClass;
        public bool HasNoRetValue { get; private set; }
        public TypeReference RetType { get; internal set; }
        public List<FunctionParameter> Parameters { get; private set; }

        public bool IsConstuctor { get; private set; }
        public bool IsStatic { get; private set; }
        public bool IsEntryPoint { get; private set; }

        public List<MemZoneFlashElement> Compiled;

        public override string FullName
        {
            get => BaseClass.FullName + Class.NameSeparator + Name;
        }

        public override string Signature
        {
            get => $"{(HasNoRetValue ? NoReturnableValueKeyword : RetType.ToString())} {FullName}" +
                (Parameters.Count == 0 ? "" : "(" + string.Join(", ", Parameters.Select(p => p.Type + " " + p.Name)) + ")");
        }

        public Function(BaseStructure Base, bool asConstructor) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Directive = Base.Directive;

            _unknownLabelNameErrorList = new List<UnknownLabelNameError>();
            _variables = new List<Variable>();
            _namedConsts = new List<NamedConstant>();
            Parameters = new List<FunctionParameter>();

            if(asConstructor)
            {
                Target = RuleTarget.Constructor;
                IsConstuctor = true;

            }
            else
            {
                Target = RuleTarget.Method;

                Modifier retModifier = GetModifier(ReturnKeyword);
                if (retModifier.Value == NoReturnableValueKeyword)
                {
                    RetType = TypeReference.Void;
                    HasNoRetValue = true;
                }
                else RetType = new TypeReference(retModifier.Value);

                if (GetModifier(StaticKeyword) != null) IsStatic = true;
                if (GetModifier(EntryPointKeyword) != null) IsEntryPoint = true;
            }

            foreach (var parameter in Modifiers.FindAll(p => p.Name == ParameterKeyword))
            {
                var parts = parameter.Value.Split(':');

                var type = parts[0];
                var name = parts[1];

                Parameters.Add(new FunctionParameter(
                    new TypeReference(type), name));
            }
        }

        public Function(BaseStructure Base) : this(Base, false)
        {
            
        }
    }
}
