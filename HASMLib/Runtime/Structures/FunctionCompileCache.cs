using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures
{
    public struct FunctionCompileCache
    {
        internal List<ConstantErrorMark> UnknownLabelNameErrorList;
        internal List<VariableMark> Variables;
        internal List<ConstantMark> NamedConsts;
        internal int ConstIndex;
        internal int ExpressionIndex;
        internal int VarIndex;
        internal int InstructionIndex;

        public List<FlashElement> Compiled;
    }
}
