using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime.Structures;
using System.Collections.Generic;

namespace HASMLib.Core
{
    public class MemZone
    {
        public List<Variable> Globals { get; private set; }
        public Stack<Object> ResultStack { get; private set; }
        public Stack<Object> ParamStack { get; private set; }
        
        public List<FlashElement> Flash { get; internal set; }

        public int FreeRAM => throw new System.NotImplementedException();//_ram - Stack.Count - Globals.Count;
        public int FreeFlash => _flash - Flash.Count;

        private int _flash;
        private int _ram;

        public void Clear()
        {
            Flash = new List<FlashElement>();

            ResultStack = new Stack<Object>();
            ParamStack = new Stack<Object>();
            Globals = new List<Variable>();
            //Stack = new Stack<Integer>();
        }

        public MemZone(int flash_len, int ram, List<FlashElement> flash)
        {
            _flash = flash_len;
            _ram = ram;

            Clear();

            Flash = flash;
        }


    }
}