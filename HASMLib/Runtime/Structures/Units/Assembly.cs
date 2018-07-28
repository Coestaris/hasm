using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures.Units
{
    public class Assembly : BaseStructure
    {
        internal Function _entryPoint;

        public List<Class> Classes;

        public Function GetEntryPointFunction()
        {
            return null;
        }

        public Assembly(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Classes = new List<Class>();
            ParentAssembly = null;
            Target = RuleTarget.Assembly;
            Directive = Base.Directive;

            SetParentAssembly(Base.Childs);

            foreach (var item in Base.Childs)
            {
                Classes.Add(item as Class);
            }
        }

        private void SetParentAssembly(List<BaseStructure> childs)
        {
            foreach (var child in childs)
            {
                if (child.Target == RuleTarget.Class)
                    (child as Class)._fullName = null;

                child.ParentAssembly = this;
                if(child.Childs != null)
                    SetParentAssembly(child.Childs);
            }
        }

        public override string FullName
        {
            get => Name;
        }

        public override string Signature
        {
            get => $"Assembly: {FullName}";
        }

        public List<Field> AllFields { get; internal set; }
        public List<Class> AllClasses { get; internal set; }
        public List<Function> AllFunctions { get; internal set; }

    }
}
