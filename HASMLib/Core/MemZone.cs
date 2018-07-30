using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;

namespace HASMLib.Core
{
    public class MemZone
    {
        public Stack<Integer> Stack { get; private set; }

        public List<Variable> RAM { get; private set; }

        public List<FlashElement> Flash { get; internal set; }

        public int FreeRAM => _ram - Stack.Count - RAM.Count;
        public int FreeFlash => _flash - Flash.Count;

        private int _flash;
        private int _ram;

        public void Clear()
        {
            Flash = new List<FlashElement>();
            RAM = new List<Variable>();
            Stack = new Stack<Integer>();
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