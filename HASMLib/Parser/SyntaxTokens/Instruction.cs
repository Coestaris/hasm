using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class Instruction
    {
        public string NameString { get; protected set; }
        public int Index { get; protected set; }
        public Regex Name { get; protected set; }
        public int ParameterCount { get; protected set; }
        public List<InstructionParameterType> ParameterTypes { get; protected set; }

        protected MemZoneVariable GetVar(MemZone mz, UIntDouble index)
        {
            return mz.RAM.Find(p => p.Index == index);
        }

        protected NamedConstant GetConst(List<NamedConstant> constants, UIntDouble index)
        {
            return constants.Find(p => p.Index == index);
        }

        public void RuntimeMachineJump(UIntDouble position, RuntimeMachine runtimeMachine)
        {
            var localIndex = position;
            var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);
            runtimeMachine.CurrentPosition = (UIntDouble)(globalIndex - 1);
        }

        public Constant GetNumericValue(int index, MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            switch (parameters[index].Type)
            {
                case (ReferenceType.Constant):
                    return GetConst(constants, parameters[index].Index).Constant;
                case (ReferenceType.Variable):
                    return new Constant(GetVar(memZone, parameters[index].Index));
                case (ReferenceType.Expression):
                    {
                        Expression expression = expressions.Find(p => p.Index == parameters[index].Index).Expression;
                        Constant constnant = expression.Calculate(memZone, true);

                        runtimeMachine.Ticks += expression.Steps;
                        return constnant;
                    }
            }

            return null;
        }

        public abstract RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine);
    }
}
