using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions
{
    public abstract class Instruction
    {
        public string NameString { get; protected set; }
        public int Index { get; protected set; }
        public Regex Name { get; protected set; }
        public int ParameterCount { get; protected set; }
        public List<InstructionParameterType> ParameterTypes { get; protected set; }

        protected Variable GetVar(Integer index, RuntimeDataPackage package)
        {
            if (index < (Integer)package.CallStackItem.Locals.Count)
                return package.CallStackItem.Locals[(int)index];

            return package.MemZone.Globals[(int)index + package.CallStackItem.Locals.Count];
        }

        protected ConstantMark GetConst(Integer index, RuntimeDataPackage package)
        {
            return package.Constants[(int)index];
        }

        public void RuntimeMachineJump(Integer position, RuntimeMachine runtimeMachine)
        {
            throw new NotImplementedException();

            //var localIndex = position;
            //var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);
            //runtimeMachine.ProgramCounter = globalIndex - (Integer)1;
        }

        public Constant GetNumericValue(ObjectReference reference, RuntimeDataPackage package)
        {
            switch (reference.Type)
            {
                case (ReferenceType.Constant):
                    return GetConst(reference.Index, package).Constant;
                case (ReferenceType.Variable):
                    {
                        var var = GetVar(reference.Index, package);
                        if (var.Value.Type.Type != Structures.TypeReferenceType.Integer)
                            return null;

                        return new Constant(var);
                    }
                case (ReferenceType.Expression):
                    {
                        Expression expression = package.Expressions.Find(p => p.Index == reference.Index).Expression;
                        Constant constnant = expression.Calculate(package.MemZone, true);

                        package.RuntimeMachine.Ticks += expression.Steps;
                        return constnant;
                    }
            }

            return null;
        }

        public abstract RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters);
    }
}
