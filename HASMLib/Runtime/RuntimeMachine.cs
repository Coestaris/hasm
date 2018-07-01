using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using HASMLib.Parser;
using System.Linq.Expressions;

namespace HASMLib.Runtime
{

    public class RuntimeMachine
    {
        internal delegate void RuntimeMachineIOHandler();
        internal delegate void RuntimeMachineIOBufferHandler(List<UInt12> data);

        public TimeSpan TimeOfRunning { get; private set; }

        public int Ticks { get; internal set; }

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

		public bool IsRunning { get; private set; }

		internal int GetGlobalInstructionIndexByLocalOne(int localIndex)
        {
            return _source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Instruction)
                .Select(p => (MemZoneFlashElementInstruction)p)
                .First(p => p.ProgramIndex == localIndex).RuntimeAbsoluteIndex;
        }

        private string _constantFormat = "_constant{0}";
        private string _variableFormat = "_var{0}";

        public RuntimeOutputCode Run()
        {
            DateTime startTime = DateTime.Now;

			IsRunning = true;

            Ticks = 0;
            InBuffer = new List<UInt12>();
            OnBufferFlushed?.Invoke();

            var result = RunInternal();

            OnBufferClosed?.Invoke();

            TimeOfRunning = TimeSpan.FromMilliseconds((DateTime.Now - startTime).TotalMilliseconds);

			IsRunning = false;

            return result;
        }

        internal UInt24 CurrentPosition;

        private RuntimeOutputCode RunInternal()
        {
            CurrentPosition = 0;
            List<MemZoneFlashElement> data = new List<MemZoneFlashElement>();
            data.AddRange(_source.ParseResult);

            List<NamedConstant> constants = data.FindAll(p => p.Type == MemZoneFlashElementType.Constant).Select(p =>
            {
                var constant = (MemZoneFlashElementConstant)p;
                return new NamedConstant(
                    string.Format(_constantFormat, constant.Index),
                    (UInt24)constant.Index,
                    constant.ToConstant());
            }).ToList();

            List<MemZoneFlashElementExpression> expressions = data.FindAll(p => p.Type == MemZoneFlashElementType.Expression).Select(p => (MemZoneFlashElementExpression)p).ToList();

            //Удаляем их из коллекции
            data.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);

            //Удаляем их из коллекции
            data.RemoveAll(p => p.Type == MemZoneFlashElementType.Expression);

            UInt24 globalIndex = 0;

            data.ForEach(p =>
            {
                if (p.Type == MemZoneFlashElementType.Instruction)
                {
                    ((MemZoneFlashElementInstruction)p).RuntimeAbsoluteIndex = globalIndex;
                }
                globalIndex += 1;
            });

            for (; CurrentPosition < data.Count; CurrentPosition += 1)
            {
                Ticks++;

                if(data[CurrentPosition].Type == MemZoneFlashElementType.Variable)
                {
                    var var = ((MemZoneFlashElementVariable)data[CurrentPosition]);
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
                    continue;
                }

                var instruction = (MemZoneFlashElementInstruction)data[CurrentPosition];

                //Проверяем валидность ссылок
                foreach (var parameter in instruction.Parameters)
                {
                    switch (parameter.Type)
                    {
                        case ReferenceType.Constant:
                            if (!constants.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;

                        case ReferenceType.Variable:
                            if (!_machine.MemZone.RAM.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;

                        case ReferenceType.Expression:
                            if(!expressions.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;

                    }
                }

                if (Ticks == int.MaxValue)
                    return RuntimeOutputCode.StackOverFlow;

                //Если все ОК, то запускаем нашу инструкцию
                RuntimeOutputCode output = SourceLineInstruction.Instructions[instruction.InstructionNumber].Apply(
                    _machine.MemZone, constants, expressions, instruction.Parameters, this);

                if (output != RuntimeOutputCode.OK)
                    return output;
            }
                      
            return RuntimeOutputCode.OK;
        }
    }
}