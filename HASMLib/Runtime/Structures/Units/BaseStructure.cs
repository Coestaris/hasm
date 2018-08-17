using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures.Units
{
    public class BaseStructure
    {
        public const string StaticKeyword = "static";

        public string ToAbsoluteName(string baseName)
        {
            return baseName.StartsWith(FullName) ? 
                baseName :
                FullName + NameSeparator + baseName;
        }
        
        public const string NameSeparator = ".";

        public Assembly ParentAssembly;
        public virtual string FullName => "";
        public virtual string Signature => "";
        public int UniqueID;

        public override string ToString()
        {
            return Signature;
        }

        public RuleTarget Target;
        public List<SourceLine> RawLines;

        public string Name;
        public AccessModifier AccessModifier;

        internal SourceLineDirective Directive;
        internal List<Modifier> Modifiers;
        internal List<BaseStructure> Childs;

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
                case RuleTarget.Constructor:
                    return new Function(this, true)
                    {
                        RawLines = RawLines
                    };
                case RuleTarget.Class:
                    return new Class(this);
                case RuleTarget.Method:
                    return new Function(this)
                    {
                        RawLines = RawLines
                    };
                case RuleTarget.Assembly:
                    return new Assembly(this);
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

        public static BaseStructure GetInstance<FindType>(string name, Assembly assembly, Class baseClass) where FindType : BaseStructure
        {
            if (typeof(FindType) == typeof(Function))
            {
                Function rel = baseClass.Functions.Find(p => p.Name == name);
                if (rel != null) return rel;

                name = assembly.ToAbsoluteName(name);
                Function abs = assembly.AllFunctions.Find(p => p.FullName == name);
                return abs;
            }
            else if (typeof(FindType) == typeof(Class))
            {
                Class rel = baseClass.InnerClasses.Find(p => p.Name == name);
                if (rel != null) return rel;

                name = assembly.ToAbsoluteName(name);
                Class abs = assembly.AllClasses.Find(p => p.FullName == name);
                return abs;
            }
            else if (typeof(FindType) == typeof(Field))
            {
                Field rel = baseClass.Fields.Find(p => p.Name == name);
                if (rel != null) return rel;

                name = assembly.ToAbsoluteName(name);
                Field abs = assembly.AllFields.Find(p => p.FullName == name);
                return abs;
            }
            else throw new System.ArgumentException();
        }
    }
}
