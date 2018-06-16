using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Runtime
{

    public class RuntimeMachine
    {
        internal delegate void RuntimeMachineIOHandler();
        internal delegate void RuntimeMachineIOBufferHandler(List<UInt12> data);


        private HASMMachine _machine;

        private HASMSource _source;

        public RuntimeMachine(HASMMachine machine, HASMSource source)
        {
            _machine = machine;
            _source = source;
        }

        internal void InbufferRecieved(List<UInt12> inBuffer)
        {
            InBuffer.AddRange(inBuffer);
        }

        internal void OutBytes(List<UInt12> bytes)
        {
            OutBufferUpdated?.Invoke(bytes);
        }

        internal event RuntimeMachineIOHandler OnBufferFlushed;
        internal event RuntimeMachineIOHandler OnBufferClosed;
        internal event RuntimeMachineIOBufferHandler OutBufferUpdated;

        internal List<UInt12> InBuffer;

        public bool IsRunning
        {
            get => false;
        }

        private string _constantFormat = "_constant{0}";
        private string _variableFormat = "_var{0}";

        public RuntimeOutputCode Run()
        {
            InBuffer = new List<UInt12>();
            OnBufferFlushed?.Invoke();

            var result = RunInternal();

            OnBufferClosed?.Invoke();

            return result;
        }

        private RuntimeOutputCode RunInternal()
        {
            var constants = new List<NamedConstant>(); 

            foreach (var item in _source.ParseResult)
            {
                switch (item.Type)
                {
                    case MemZoneFlashElementType.Variable:
                        var var = ((MemZoneFlashElementVariable)item);
                        switch (var.VariableType)
                        {
                            case LengthQualifier.Single:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt12(0, var.Index, string.Format(_variableFormat, var.Index)));
                                break;
                            case LengthQualifier.Double:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt24(0, var.Index, string.Format(_variableFormat, var.Index)));
                                break;
                            case LengthQualifier.Quad:
                                _machine.MemZone.RAM.Add(new MemZoneVariableUInt48(0, var.Index, string.Format(_variableFormat, var.Index)));
                                break;
                        }
                        break;


                    case MemZoneFlashElementType.Instruction:
                        var instruction = (MemZoneFlashElementInstruction)item;

                        //Проверяем валидность ссылок
                        foreach (var parameter in instruction.Parameters)
                        {
                            switch (parameter.Type)
                            {
                                case ReferenceType.Constant:
                                    if(!constants.Exists( p => p.Index == parameter.Index ))
                                        return RuntimeOutputCode.UnknownConstantReference;
                                    break;

                                case ReferenceType.Variable:
                                    if (!_machine.MemZone.RAM.Exists(p => p.Index == parameter.Index))
                                        return RuntimeOutputCode.UnknownConstantReference;
                                    break;
                            }
                        }

                        //Если все ОК, то запускаем нашу инструкцию
                        RuntimeOutputCode output = instructions[instruction.InstructionNumber].Apply(
                            _machine.MemZone, constants, instruction.Parameters, this);

                        if (output != RuntimeOutputCode.OK)
                            return output;

                        break;


                    case MemZoneFlashElementType.Constant:
                        var constant = (MemZoneFlashElementConstant)item;

                        constants.Add(new NamedConstant(
                            string.Format(_constantFormat, constant.Index),
                            (UInt24)constant.Index,
                            constant.ToConstant()));
                        break;


                    case MemZoneFlashElementType.Undefined:
                        break;
                }
            }

            return RuntimeOutputCode.OK;
        }
    }
}