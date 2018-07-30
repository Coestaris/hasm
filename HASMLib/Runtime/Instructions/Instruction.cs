using HASMLib.Core;
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

        protected Variable GetVar(MemZone mz, Integer index)
        {
            return mz.RAM[(int)index];
        }

        protected ConstantMark GetConst(List<ConstantMark> constants, Integer index)
        {
            return constants[(int)index];
        }

        public void RuntimeMachineJump(Integer position, RuntimeMachine runtimeMachine)
        {
            throw new NotImplementedException();

            //var localIndex = position;
            //var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);
            //runtimeMachine.ProgramCounter = globalIndex - (Integer)1;
        }

        public Constant GetNumericValue(int index, MemZone memZone, List<ConstantMark> constants, List<FlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
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

        public abstract RuntimeOutputCode Apply(MemZone memZone, List<ConstantMark> constants, List<FlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine);
    }
}
