namespace HASMLib.Core.MemoryZone
{
	public abstract class MemZoneFlashElement
    {
		protected const byte Element_Const = 1;
		protected const byte Element_Var = 2;
		protected const byte Element_Instruction = 3;

        public virtual int FixedSize => 0;
        public virtual MemZoneFlashElementType Type => MemZoneFlashElementType.Undefined;
       
		public abstract byte[] ToBytes();
    }
}