using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Runtime.Structures.Units
{
    public class Function : BaseStructure
    {
        public const string ReturnKeyword = "ret";
        public const string ParameterKeyword = "param";
        public const string NoReturnableValueKeyword = "_none_";

        internal List<UnknownLabelNameError> _unknownLabelNameErrorList;
        internal List<Variable> _variables;
        internal List<NamedConstant> _namedConsts;
        internal int _constIndex;
        internal int _expressionIndex;
        internal int _varIndex;
        internal int _instructionIndex;

        public Class BaseClass;
        public bool HasNoRetValue { get; private set; }
        public string RetType { get; private set; }
        public List<FunctionParameter> Parameters { get; private set; }

        public List<MemZoneFlashElement> Compiled;

        public override string FullName
        {
            get => BaseClass.FullName + Class.NameSeparator + Name;
        }

        public override string Signature
        {
            get => $"{(HasNoRetValue ? NoReturnableValueKeyword : RetType)} {FullName}" +
                (Parameters.Count == 0 ? "" : "(" + string.Join(", ", Parameters.Select(p => p.Type + " " + p.Name)) + ")");
        }

        public Function(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            _unknownLabelNameErrorList = new List<UnknownLabelNameError>();
            _variables = new List<Variable>();
            _namedConsts = new List<NamedConstant>();

            Target = RuleTarget.Method;
            Directive = Base.Directive;

            Parameters = new List<FunctionParameter>();
            Modifier retModifier = GetModifier(ReturnKeyword);
            if (retModifier.Value == NoReturnableValueKeyword)
            {
                HasNoRetValue = true;
            } else RetType = retModifier.Value;

            foreach (var parameter in Modifiers.FindAll(p => p.Name == ParameterKeyword))
            {
                var parts = parameter.Value.Split(':');

                var type = parts[0];
                var name = parts[1];

                Parameters.Add(new FunctionParameter(
                    new TypeReference(type), name));
            }
        }
    }
}
