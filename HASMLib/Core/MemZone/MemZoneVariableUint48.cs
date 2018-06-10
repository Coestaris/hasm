namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt48 : MemZoneVariable
    {
        public MemZoneVariableUInt48(MemZone zone, string name, UInt48 value)
        {
            Length = 4;
            Name = name;
            Zone = zone;

            RAMOffster = zone.offset;

            SetValue(zone, value);
        }

        public UInt48 GetValue(MemZone zone)
        {
            return UInt48.FromUInt12(zone.RAMGetRange(RAMOffster, Length));
        }

        public void SetValue(MemZone zone, UInt48 value)
        {
            zone.RAMSetRange(RAMOffster, value.ToUInt12());
        }

        public override string ToString()
        {
            return GetValue(Zone).ToString();
        }
    }
}