using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Core
{
    public class MemZone
    {
        private Dictionary<string, MemZoneVariable> Variables;

        internal int offset;

        public Stack<UInt12> Stack;

        public List<MemZoneFlashElement> Flash;

        private UInt12[] RAM;

        public void RAMSetVariableValue(string Name, UInt12 value)
        {
            ((MemZoneVariableUInt12)Variables[Name]).SetValue(this, value);
        }

        public void RAMSetVariableValue(int index, UInt12 value)
        {
            ((MemZoneVariableUInt12)Variables.Values.ElementAt(index)).SetValue(this, value);
        }

        public void RAMSetVariableValue(string Name, UInt24 value)
        {
            ((MemZoneVariableUInt24)Variables[Name]).SetValue(this, value);
        }

        public void RAMSetVariableValue(int index, UInt24 value)
        {
            ((MemZoneVariableUInt24)Variables.Values.ElementAt(index)).SetValue(this, value);
        }

        public void RAMSetVariableValue(string Name, UInt48 value)
        {
            ((MemZoneVariableUInt48)Variables[Name]).SetValue(this, value);
        }

        public void RAMSetVariableValue(int index, UInt48 value)
        {
            ((MemZoneVariableUInt48)Variables.Values.ElementAt(index)).SetValue(this, value);
        }

        public MemZoneVariable RAMGetVariable(int index)
        {
            return Variables.Values.ElementAt(index);
        }

        public MemZoneVariable RAMGetVariable(string Name)
        {
            return Variables[Name];
        }

        public void RAMAllocate(UInt12 value, string Name)
        {
            Variables.Add(Name, new MemZoneVariableUInt12(this, Name, value));
            offset += 1;
        }

        public void RAMAllocate(UInt24 value, string Name)
        {
            Variables.Add(Name, new MemZoneVariableUInt24(this, Name, value));
            offset += 2;
        }

        public void RAMAllocate(UInt48 value, string Name)
        {
            Variables.Add(Name, new MemZoneVariableUInt48(this, Name, value));
            offset += 4;
        }

        internal UInt12[] RAMGetRange(int offset, int length)
        {
            if (offset + length > RAM.Length)
                return null;

            return RAM.Skip(offset).Take(length).ToArray();
        }

        internal bool RAMSetRange(int offset, UInt12[] data)
        {
            if (data.Length + offset > RAM.Length)
                return false;

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            data.CopyTo(RAM, offset);
            return true;
        }

        public MemZone(int ramLength)
        {
            Variables = new Dictionary<string, MemZoneVariable>();

            Stack = new Stack<UInt12>();
            RAM = new UInt12[ramLength];
            Flash = new List<MemZoneFlashElement>();
        }

        
    }
}