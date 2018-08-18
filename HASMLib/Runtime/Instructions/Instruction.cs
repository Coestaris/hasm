using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
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

        protected bool CheckObjectStackItem(RuntimeDataPackage package, Class baseClass, out RuntimeOutputCode error)
        {
            if (package.MemZone.ObjectStackItem == null)
            {
                error = RuntimeOutputCode.ObjectStackIsEmpty;
                return false;
            }
            if (package.MemZone.ObjectStackItem.Type.Type != TypeReferenceType.Class)
            {
                error = RuntimeOutputCode.ClassTypeExpected;
                return false;
            }

            if (package.MemZone.ObjectStackItem.Type.ClassType != baseClass)
            {
                error = RuntimeOutputCode.DifferentClasses;
                return false;
            }

            error = RuntimeOutputCode.OK;
            return true;
        }


        protected Variable GetVar(Integer index, RuntimeDataPackage package)
        {
            return package.GetVariable(index);
        }

        protected ConstantMark GetConst(Integer index, RuntimeDataPackage package)
        {
            return package.Constants[(int)index];
        }

        public void RuntimeMachineJump(Integer position, RuntimeMachine runtimeMachine)
        {
            throw new System.NotImplementedException();
            //TODO
            //var localIndex = position;
            //var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);
            //runtimeMachine.ProgramCounter = globalIndex - (Integer)1;
        }

        public Object GetObject(ObjectReference reference, RuntimeDataPackage package)
        {
            switch (reference.Type)
            {
                case ReferenceType.Variable:
                    return GetVar(reference.Index, package).Value;
                case ReferenceType.Constant:
                case ReferenceType.Expression:
                    return new Object(GetNumericValue(reference, package).IntValue, package.Assembly);
                case ReferenceType.Define:
                case ReferenceType.Type:
                case ReferenceType.Function:
                case ReferenceType.Field:
                default:
                    return null;
            }
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
                        if (var.Value.Type.Type != TypeReferenceType.Integer)
                            return null;

                        return new Constant(var);
                    }
                case (ReferenceType.Expression):
                    {
                        Expression expression = package.Expressions.Find(p => p.Index == reference.Index).Expression;
                        Constant constnant = expression.Calculate(package, true);

                        package.RuntimeMachine.Ticks += expression.Steps;
                        return constnant;
                    }
            }

            return null;
        }

        public abstract RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters);
    }
}
