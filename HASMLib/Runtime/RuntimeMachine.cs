using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using HASMLib.Parser;

namespace HASMLib.Runtime
{

    public class RuntimeMachine
    {
        internal delegate void RuntimeMachineIOHandler();
        internal delegate void RuntimeMachineIOBufferHandler(List<Integer> data);

        private const string _constantFormat = "_constant{0}";
        private const string _variableFormat = "_var{0}";

        public TimeSpan TimeOfRunning { get; private set; }
        public int Ticks { get; internal set; }
		public bool IsRunning { get; private set; }

        internal List<Integer> InBuffer;
        internal Integer ProgramCounter;

        private HASMMachine _machine;
        private HASMSource _source;

        internal event RuntimeMachineIOHandler OnBufferFlushed;
        internal event RuntimeMachineIOHandler OnBufferClosed;
        internal event RuntimeMachineIOBufferHandler OutBufferUpdated;

        public RuntimeMachine(HASMMachine machine, HASMSource source)
        {
            _machine = machine;
            _source = source;
        }

        internal void InbufferRecieved(List<Integer> inBuffer)
        {
            InBuffer.AddRange(inBuffer);
        }

        internal void OutBytes(List<Integer> bytes)
        {
            OutBufferUpdated?.Invoke(bytes);
        }

		internal int GetGlobalInstructionIndexByLocalOne(int localIndex)
        {
            return _source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Instruction)
                .Select(p => (MemZoneFlashElementInstruction)p)
                .First(p => p.ProgramIndex == localIndex).RuntimeAbsoluteIndex;
        }
        
        public RuntimeOutputCode Run()
        {
            DateTime startTime = DateTime.Now;

			IsRunning = true;

            Ticks = 0;
            InBuffer = new List<Integer>();
            OnBufferFlushed?.Invoke();

            var result = RunInternal();

            OnBufferClosed?.Invoke();

            TimeOfRunning = TimeSpan.FromMilliseconds((DateTime.Now - startTime).TotalMilliseconds);

			IsRunning = false;

            return result;
        }

        private RuntimeOutputCode RunInternal()
        {
            ProgramCounter = 0;

            List<MemZoneFlashElement> data = new List<MemZoneFlashElement>();
            data.AddRange(_source.ParseResult);

            List<NamedConstant> constants = data.FindAll(p => p.Type == MemZoneFlashElementType.Constant).Select(p =>
            {
                var constant = (MemZoneFlashElementConstant)p;
                return new NamedConstant(
                    string.Format(_constantFormat, constant.Index),
                    constant.Index,
                    constant.ToConstant());
            }).ToList();

            List<MemZoneFlashElementExpression> expressions = data.FindAll(p => p.Type == MemZoneFlashElementType.Expression).Select(p => (MemZoneFlashElementExpression)p).ToList();

            //Удаляем их из коллекции
            data.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);

            //Удаляем их из коллекции
            data.RemoveAll(p => p.Type == MemZoneFlashElementType.Expression);

            Integer globalIndex = 0;

            data.ForEach(p =>
            {
                if (p.Type == MemZoneFlashElementType.Instruction)
                {
                    ((MemZoneFlashElementInstruction)p).RuntimeAbsoluteIndex = globalIndex;
                }
                globalIndex += 1;
            });

            //Проверяем валидность ссылок
            foreach(MemZoneFlashElementInstruction instruction in data.FindAll(p => p.Type == MemZoneFlashElementType.Instruction))
            {
                //Проверяем валидность ссылок
                foreach (var parameter in instruction.Parameters)
                {
                    switch (parameter.Type)
                    {
                        case ReferenceType.Constant:
                            if (!constants.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;

                        /*case ReferenceType.Variable:
                            if (!_machine.MemZone.RAM.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;*/

                        case ReferenceType.Expression:
                            if (!expressions.Exists(p => p.Index == parameter.Index))
                                return RuntimeOutputCode.UnknownConstantReference;
                            break;
                    }
                }
            }

            for (; ProgramCounter < data.Count; ProgramCounter += 1)
            {
                Ticks++;
                if(data[ProgramCounter].Type == MemZoneFlashElementType.Variable)
                {
                    var var = ((MemZoneFlashElementVariable)data[ProgramCounter]);
                    _machine.MemZone.RAM.Add(new MemZoneVariable(var.VariableType, var.Index));
                    continue;
                }

                var instruction = (MemZoneFlashElementInstruction)data[ProgramCounter];
                
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