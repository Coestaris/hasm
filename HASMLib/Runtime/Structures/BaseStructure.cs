using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures
{
    public class BaseStructure
    {
        public virtual string FullName => "";
        public virtual string Signature => "";

        public override string ToString()
        {
            return Signature;
        }

        public RuleTarget Target;
        public List<SourceLine> RawLines;

        public string Name;
        public List<Modifier> Modifiers;
        public AccessModifier AccessModifier;
        public List<BaseStructure> Childs;

        public BaseStructure() { }

        public BaseStructure(string name, List<Modifier> modifiers, AccessModifier accessModifier, List<BaseStructure> childs)
        {
            Name = name;
            Modifiers = modifiers;
            AccessModifier = accessModifier;
            Childs = childs;
        }

        public BaseStructure Cast()
        {
            switch (Target)
            {
                case RuleTarget.Class:
                    return new Class(this);
                case RuleTarget.Method:
                    return new Function(this)
                    {
                        RawLines = RawLines
                    };
                case RuleTarget.Field:
                    return new Field(this);
                default:
                    return null;
            }
        }

        protected Modifier GetModifier(string name)
        {
            return Modifiers.Find(p => p.Name == name);
        }
    }
}
