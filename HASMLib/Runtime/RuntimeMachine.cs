using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using static HASMLib.Parser.HASMParser;

using System.Collections.Generic;
using HASMLib.Parser.SyntaxTokens;
using System.Linq;
using HASMLib.Parser;

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

        private string _constantFormat = "_constant{0}";
        private string _variableFormat = "_var{0}";

        public enum RunOutput
        {
            UnknownConstantReference,
            UnknownVariableReference,

            OK
        }

        public RunOutput Run()
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
                                        return RunOutput.UnknownConstantReference;
                                    break;

                                case ReferenceType.Variable:
                                    if (!_machine.MemZone.RAM.Exists(p => p.Index == parameter.Index))
                                        return RunOutput.UnknownConstantReference;
                                    break;
                            }
                        }

                        //Если все ОК, то запускаем нашу инструкцию
                        instructions[instruction.InstructionNumber].Apply(
                            _machine.MemZone, constants, instruction.Parameters, this);

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

            return RunOutput.OK;
        }
    }
}