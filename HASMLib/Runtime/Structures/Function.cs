using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime.Structures
{
    public struct Parameter
    {
        public string Type;
        public string Name;

        public Parameter(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public class Function : BaseStructure
    {
        public const string ReturnKeyword = "ret";
        public const string ParameterKeyword = "param";
        public const string NoReturnableValueKeyword = "_none_";

        public Class BaseClass;
        public bool HasNoRetValue { get; private set; }
        public string RetType { get; private set; }
        public List<Parameter> Parameters { get; private set; }

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
            Target = RuleTarget.Method;
            Parameters = new List<Parameter>();
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

                Parameters.Add(new Parameter(type, name));
            }
        }
    }
}
