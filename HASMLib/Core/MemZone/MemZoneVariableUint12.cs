namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt12 : MemZoneVariable
    {
        public MemZoneVariableUInt12(MemZone zone, string name, UInt12 value)
        {
            Length = 1;
            Name = name;
            Zone = zone;

            RAMOffster = zone.offset;

            SetValue(zone, value);
        }

        public UInt12 GetValue(MemZone zone)
        {
            return zone.RAMGetRange(RAMOffster, Length)[0];
        }

        public void SetValue(MemZone zone, UInt12 value)
        {
            zone.RAMSetRange(RAMOffster, new UInt12[1] { value });
        }

        public override string ToString()
        {
            return GetValue(Zone).ToString();
        }
    }
}