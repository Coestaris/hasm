using HASMLib.Runtime.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    public class StructureRule
    {
        private List<AccessModifier> _availableAccsessModifiers;

        public Type AssignedType;
        public List<AccessModifier> AvailableAccsessModifiers
        {
            set
            {
                AvailableAccsessModifiersStrings = value.Select(p => p.ToString().ToLower()).ToList();
                _availableAccsessModifiers = value;
            }
            get => _availableAccsessModifiers;
        }
        public List<Modifier> Modifiers;
        public RuleTarget Target;
        public List<RuleTarget> AllowedChilds;
        public bool AllowedInnerInstructions;

        public List<string> AvailableAccsessModifiersStrings;
    }
}
