using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;

namespace HASMLib.Runtime
{
    public class RuntimeMachine
    {
        internal delegate void RuntimeMachineOutBufferUpdated();

        private HASMMachine _machine;

        private HASMSource _source;

        public RuntimeMachine(HASMMachine machine, HASMSource source)
        {
            _machine = machine;
            _source = source;
        }

        internal void InbufferRecieved(List<byte> inBuffer)
        {
            InBuffer.AddRange(inBuffer);
        }

        private void OutBytes(List<byte> bytes)
        {
            OutBuffer.AddRange(bytes);
            OutBufferUpdated?.Invoke();
        }

        internal event RuntimeMachineOutBufferUpdated OutBufferUpdated;

        internal List<byte> OutBuffer;
        internal List<byte>  InBuffer;

        public bool IsRunning
        {
            get => false;
        }

        public void Run()
        {
            var constants = new List<UInt24, > 

            foreach (var item in _source.ParseResult)
            {
                switch (item.Type)
                {
                    case MemZoneFlashElementType.Variable:
                        var var = ((MemZoneFlashElementVariable)item);
                        switch (var.VariableType)
                        {
                            case MemZoneVariableLength.Single:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt12(0, var.Index));
                                break;
                            case MemZoneVariableLength.Double:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt24(0, var.Index));
                                break;
                            case MemZoneVariableLength.Quad:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt48(0, var.Index));
                                break;
                        }
                        break;


                    case MemZoneFlashElementType.Instruction:
                        break;


                    case MemZoneFlashElementType.Constant:
                        
                        break;


                    case MemZoneFlashElementType.Undefined:
                        break;
                }
            }
        }
    }
}