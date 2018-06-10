namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt24 : MemZoneVariable
    {
        public MemZoneVariableUInt24(MemZone zone, string name, UInt24 value)
        {
            Length = 2;
            Name = name;
            Zone = zone;

            RAMOffster = zone.offset;

            SetValue(zone, value);
        }

        public UInt12 GetValue(MemZone zone)
        {
            return UInt24.FromUInt12(zone.RAMGetRange(RAMOffster, Length));
        }

        public void SetValue(MemZone zone, UInt24 value)
        {
            zone.RAMSetRange(RAMOffster, value.ToUInt12());
        }

        public override string ToString()
        {
            return GetValue(Zone).ToString();
        }
    }
}