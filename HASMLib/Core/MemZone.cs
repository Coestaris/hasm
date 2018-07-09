using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;

namespace HASMLib.Core
{
    public class MemZone
    {
        public Stack<FSingle> Stack { get; private set; }

        public List<MemZoneVariable> RAM { get; private set; }

        public List<MemZoneFlashElement> Flash { get; internal set; }
    
        public int FreeRAM => _ram - Stack.Count - RAM.Count;
        public int FreeFlash => _flash - Flash.Count;
        
        private int _flash;
        private int _ram;

        public void Clear()
        {
            Flash = new List<MemZoneFlashElement>();
            RAM = new List<MemZoneVariable>();
            Stack = new Stack<FSingle>();
        }

        public MemZone(int flash_len, int ram, List<MemZoneFlashElement> flash)
        {
            _flash = flash_len;
            _ram = ram;

            Clear();

            Flash = flash;
        }

        
    }
}