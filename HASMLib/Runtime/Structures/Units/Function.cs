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
        public const string SelfParameter = "self";

        public const string ReturnKeyword = "ret";
        public const string ParameterKeyword = "param";
        public const string NoReturnableValueKeyword = "void";

        public Class BaseClass;
        public bool HasNoRetValue { get; private set; }
        public TypeReference RetType { get; internal set; }
        public List<FunctionParameter> Parameters { get; private set; }

        public bool IsConstuctor { get; private set; }
        public bool IsStatic { get; private set; }
        public bool IsEntryPoint { get; private set; }

        internal FunctionCompileCache CompileCache;
        internal FunctionRuntimeCache RuntimeCache;

        public override string FullName
        {
            get => BaseClass.ToAbsoluteName(Name);
        }

        public override string Signature
        {
            get => $"{(HasNoRetValue ? NoReturnableValueKeyword : RetType.ToString())} {FullName}" +
                (Parameters.Count == 0 ? "" : "(" + string.Join(", ", Parameters.Select(p => p.Type + " " + p.Name)) + ")");
        }

        public Function(BaseStructure Base, bool asConstructor) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Directive = Base.Directive;

            CompileCache.UnknownLabelNameErrorList = new List<ConstantErrorMark>();
            CompileCache.Variables = new List<VariableMark>();
            CompileCache.NamedConsts = new List<ConstantMark>();
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
                else RetType = new TypeReference(retModifier.Value, null);

                if (GetModifier(StaticKeyword) != null) IsStatic = true;
                if (GetModifier(EntryPointKeyword) != null) IsEntryPoint = true;
            }

            foreach (var parameter in Modifiers.FindAll(p => p.Name == ParameterKeyword))
            {
                var parts = parameter.Value.Split(':');

                var type = parts[0];
                var name = parts[1];

                Parameters.Add(new FunctionParameter(
                    new TypeReference(type, null), name));
            }
        }

        public static Function GetInstance(string name, Assembly assembly, Class baseClass)
        {
            var rel = baseClass.Functions.Find(p => p.Name == name);
            if (rel != null) return rel;

            name = assembly.ToAbsoluteName(name);
            var abs = assembly.AllFunctions.Find(p => p.FullName == name);
            return abs;
        }

        public Function(BaseStructure Base) : this(Base, false)
        {
            
        }
    }
}
