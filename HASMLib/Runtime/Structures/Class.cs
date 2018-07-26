using HASMLib.Parser.SyntaxTokens.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime.Structures
{
    public class Class : BaseStructure
    {
        public const string Abstract = "abstract";
        public const string Sealed = "sealed";

        private static string GetName(Class _class, string separator, string result)
        {
            if (_class.IsInner)
            {
                result = GetName(_class.InnerParent, separator, result + separator + _class.Name);
                return result;
            }
            else return _class.Name;
        }

        public string FullName => GetName(this, ".", "");
        public bool IsInner { get; private set; }
        public Class InnerParent { get; private set; }

        public Class(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Target = RuleTarget.Class;
            InnerClasses = new List<Class>();
            Functions = new List<Function>();

            foreach (var child in Base.Childs)
            {
                if (child.Target == RuleTarget.Class)
                {
                    (child as Class).IsInner = true;
                    (child as Class).InnerParent = this;
                    InnerClasses.Add(child as Class);
                }

                if (child.Target == RuleTarget.Method)
                {
                    (child as Function).BaseClass = this;
                    Functions.Add(child as Function);
                }
            }
        }

        public override string ToString()
        {
            return $"class({AccessModifier.ToString().ToLower()}) {FullName}";
        }

        public bool IsSealed => Modifiers.Exists(p => p.Name == Abstract);
        public bool IsAbstact => Modifiers.Exists(p => p.Name == Sealed);

        public List<Class> InnerClasses { get; private set; }
        public List<Function> Functions { get; private set; }
    }
}
