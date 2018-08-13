using HASMLib.Core;
using HASMLib.Core.BaseTypes;
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

        public bool ContainsVarialbe(Integer index)
        {
            if (index >= (Integer)CallStackItem.Locals.Count)
                return MemZone.Globals.Exists(p => p.Index == index);
            else return CallStackItem.Locals.Exists(p => p.Index == index);
        }

        internal Variable GetVariable(Integer index)
        {
            if (index >= (Integer)CallStackItem.Locals.Count)
                return MemZone.Globals.Find(p => p.Index == index);
            else return CallStackItem.Locals.Find(p => p.Index == index);
        }
    }
}
