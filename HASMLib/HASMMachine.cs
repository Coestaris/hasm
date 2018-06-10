using System;

using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib
{
    public class HASMMachine
    {
        public HASMMachine(uint ram, uint eeprom, uint flash)
        {
            RAM = ram;
            EEPROM = eeprom;
            Flash = flash;

            MemZone = new MemZone((int)RAM);
        }

        public void SetRegisters(string NameFormat, uint count)
        {
            for (int i = 0; i < count; i++)
                MemZone.RAMAllocate((UInt12)0, string.Format(NameFormat, i));

            RegisterCount = count;
        }

        public void ClearRegisters()
        {
            for (int i = 0; i < RegisterCount; i++)
            {
                var a = (MemZoneVariableUInt12)MemZone.RAMGetVariable(i);
                a.SetValue(MemZone, 0);
            }
        }

        public UInt32 RegisterCount;

        public UInt32 RAM { get; set; }
        public UInt32 EEPROM { get; set; }
        public UInt32 Flash { get; set; }

        public MemZone MemZone;
        
        public HASMMachineBannedFeatures BannedFeatures { get; set; }

        public UInt12 Run(HASMSource source)
        {
            return 0;
        }
    }
}
