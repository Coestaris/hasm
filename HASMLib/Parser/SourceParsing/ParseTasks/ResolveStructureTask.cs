using HASMLib.Parser.SyntaxTokens.Structure;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class ResolveStructureTask : ParseTask
    {
        public override string Name => "Resolving structure";

        public static List<StructureRule> Rules = new List<StructureRule>()
        {
            new StructureRule()
            {
                Target = RuleTarget.Class,
                AvailableAccsessModifiers = new List<AccessModifier>()
                {
                    AccessModifier.Default,
                    AccessModifier.Inner,
                    AccessModifier.Public
                },
                AllowedChilds = new List<RuleTarget>()
                {
                    RuleTarget.Class,
                    RuleTarget.Field,
                    RuleTarget.Method,
                    RuleTarget.Constructor
                },
                AllowedInnerInstructions = false,
                Modifiers = new List<Modifier>()
                {
                    new Modifier(false, false, Class.AbstractKeyword),
                    new Modifier(false, false, Class.SealedKeyword),
                }
            },
            new StructureRule()
            {
                Target = RuleTarget.Method,
                AvailableAccsessModifiers = new List<AccessModifier>()
                {
                    AccessModifier.Private,
                    AccessModifier.Default,
                    AccessModifier.Inner,
                    AccessModifier.Public
                },
                AllowedChilds = null,
                AllowedInnerInstructions = true,
                Modifiers = new List<Modifier>()
                {
                    new Modifier(false, false, Function.StaticKeyword),
                    new Modifier(false, false, Function.EntryPointKeyword),
                    new Modifier(true, true, Function.ReturnKeyword),
                    new Modifier(false, true, Function.ParameterKeyword)
                },
            },
            new StructureRule()
            {
                Target = RuleTarget.Field,
                AvailableAccsessModifiers = new List<AccessModifier>()
                {
                    AccessModifier.Private,
                    AccessModifier.Default,
                    AccessModifier.Inner,
                    AccessModifier.Public
                },
                AllowedChilds = null,
                AllowedInnerInstructions = false,
                Modifiers = new List<Modifier>()
                {
                    new Modifier(false, false, Function.StaticKeyword),
                    new Modifier(true, true, Field.TypeKeyword)
                }
            },
            new StructureRule()
            {
                Target = RuleTarget.Assembly,
                AllowedChilds = new List<RuleTarget>()
                {
                    RuleTarget.Class,
                },
                AllowedInnerInstructions = false,
                AvailableAccsessModifiers = null,
                Modifiers = null,
            },
            new StructureRule()
            {
                Target = RuleTarget.Constructor,
                AllowedChilds = null,
                AllowedInnerInstructions = true,
                AvailableAccsessModifiers = new List<AccessModifier>()
                {
                    AccessModifier.Private,
                    AccessModifier.Default,
                    AccessModifier.Inner,
                    AccessModifier.Public
                },
                Modifiers = new List<Modifier>()
                {
                    new Modifier(false, true, Function.ParameterKeyword)
                }
            }
        };

        private List<string> AccessModifiers = Enum.GetNames(typeof(AccessModifier))
            .Select(p => p.ToLower()).ToList();

        public BaseStructure GetStructures(CodeBlock block, out ParseError error)
        {
            StructureRule rule = Rules.Find(p => p.Target == block.ParentDirective.Target);
            
            if(!rule.AllowedInnerInstructions && block.RawLines.Count != 0)
            {
                error = new ParseError(ParseErrorType.Directives_InnerInstructionsAreNotAllowed,
                    block.ParentDirective.LineIndex, block.ParentDirective.FileName);
                return null;
            }

            AccessModifier accessModifier = AccessModifier.Default;
            List<Modifier> modifiers = new List<Modifier>();
            List<BaseStructure> childs = null;

            if(block.ParentDirective.Parameters != null) foreach (var modifier in block.ParentDirective.Parameters)
            {
                var modifierParts = modifier.Split(':');

                var modifierName = modifierParts[0];
                var modifierValue = string.Join(":", modifierParts.Skip(1));

                Modifier Modifier = rule.Modifiers.Find(p => p.Name == modifierName);

                if(Modifier == null && !AccessModifiers.Contains(modifier))
                {
                    error = new ParseError(ParseErrorType.Directives_UnknownModifier,
                                block.ParentDirective.LineIndex, block.ParentDirective.FileName);
                    return null;
                }

                if(AccessModifiers.Contains(modifier))
                {
                    if (rule.AvailableAccsessModifiersStrings.Contains(modifier))
                    {
                        accessModifier = rule.AvailableAccsessModifiers.Find(
                            p => p.ToString().ToLower() == modifier);
                    }
                    else
                    {
                        error = new ParseError(ParseErrorType.Directives_NotAllowedAccessModifier,
                                block.ParentDirective.LineIndex, block.ParentDirective.FileName);
                        return null;
                    }

                    continue;
                }

                if(Modifier.ValueRequired && string.IsNullOrEmpty(modifierValue))
                {
                    error = new ParseError(ParseErrorType.Directives_ModifierValueRequired,
                                block.ParentDirective.LineIndex, block.ParentDirective.FileName);
                    return null;
                }

                modifiers.Add(new Modifier(Modifier, modifierValue));
            }

            if (!rule.Modifiers.FindAll(p => p.IsRequired).All(p => modifiers.Exists(j => j.Name == p.Name)))
            {
                error = new ParseError(ParseErrorType.Directives_SomeRequiredModifiersAreMissing,
                               block.ParentDirective.LineIndex, block.ParentDirective.FileName);
                return null;
            }

            childs = new List<BaseStructure>();
            foreach (var child in block.ChildBlocks)
            {
                if(!rule.AllowedChilds.Contains(child.ParentDirective.Target))
                {
                    error = new ParseError(ParseErrorType.Directives_WrongChildTarget,
                               child.ParentDirective.LineIndex, child.ParentDirective.FileName);
                    return null;
                }

                childs.Add(GetStructures(child, out error));
                if (error != null) return null;
            }

            error = null;
            return new BaseStructure()
            {
                Name = block.ParentDirective.Name,
                AccessModifier = accessModifier,
                Modifiers = modifiers,
                Childs = childs,
                RawLines = block.RawLines,
                Target = rule.Target,
                Directive = block.ParentDirective
            }.Cast();
        }

        protected override void InnerReset() { }

        protected override void InnerRun()
        {
            CodeBlock block = source._parentBlock;

            List<BaseStructure> structures = new List<BaseStructure>();
            foreach (var child in block.ChildBlocks)
            {
                structures.Add(GetStructures(child, out ParseError error));
                if(error != null)
                {
                    InnerEnd(error);
                    return;
                }
            }

            source._parentBlock = null;
            if(structures != null && structures.Count != 0)
                source.Assembly =  new Assembly(structures[0]);

            InnerEnd();
        }
    }
}
