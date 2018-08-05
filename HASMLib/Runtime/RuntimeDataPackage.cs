using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;

namespace HASMLib.Runtime
{
    public class RuntimeDataPackage
    {
        public CallStackItem CallStackItem;
        public MemZone MemZone;
        public List<ConstantMark> Constants;
        public List<FlashElementExpression> Expressions;
        public RuntimeMachine RuntimeMachine;
        public Assembly Assembly;
    }
}
