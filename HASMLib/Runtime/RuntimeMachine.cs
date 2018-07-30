using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using HASMLib.Parser;
using HASMLib.Runtime.Structures.Units;

namespace HASMLib.Runtime
{

    public class RuntimeMachine
    {
        internal delegate void RuntimeMachineIOHandler();
        internal delegate void RuntimeMachineIOBufferHandler(string streamName, List<Integer> data);

        private const string _constantFormat = "_constant{0}";
        private const string _variableFormat = "_var{0}";

        public TimeSpan TimeOfRunning { get; private set; }
        public int Ticks { get; internal set; }
        public bool IsRunning { get; private set; }

        internal List<Integer> InBuffer;

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

        internal void OutBytes(string streamName, List<Integer> bytes)
        {
            OutBufferUpdated?.Invoke(streamName, bytes);
        }

        internal Integer GetGlobalInstructionIndexByLocalOne(Integer localIndex)
        {
            throw new NotImplementedException();
            /*return _source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Instruction)
                .Select(p => (MemZoneFlashElementInstruction)p)
                .First(p => p.ProgramIndex == localIndex).RuntimeAbsoluteIndex;*/
        }

        private List<CallStackItem> CallStack;

        public RuntimeError Run()
        {
            DateTime startTime = DateTime.Now;
            IsRunning = true;

            Ticks = 0;
            InBuffer = new List<Integer>();
            CallStack = new List<CallStackItem>();

            OnBufferFlushed?.Invoke();

            foreach (var function in _source.Assembly.AllFunctions)
            {
                List<ConstantMark> constants = function.CompileCache.Compiled.FindAll(p => p.Type == FlashElementType.Constant).Select(p =>
                {
                    var constant = (FlashElementConstant)p;
                    return new ConstantMark(
                        string.Format(_constantFormat, constant.Index),
                        constant.Index,
                        constant.ToConstant());
                }).ToList();

                List<FlashElementExpression> expressions = function.CompileCache.Compiled
                    .FindAll(p => p.Type == FlashElementType.Expression)
                    .Select(p => (FlashElementExpression)p).ToList();

                List<FlashElementInstruction> instructions = function.CompileCache.Compiled
                    .FindAll(p => p.Type == FlashElementType.Instruction)
                    .Select(p => (FlashElementInstruction)p).ToList();

                List<FlashElementVariable> variables = function.CompileCache.Compiled
                    .FindAll(p => p.Type == FlashElementType.Variable)
                    .Select(p => (FlashElementVariable)p).ToList();


                Integer globalIndex = (Integer)0;

                instructions.ForEach(p =>
                {
                    p.RuntimeAbsoluteIndex = globalIndex;
                    globalIndex += (Integer)1;
                });

                function.RuntimeCache.Constants = constants;
                function.RuntimeCache.Variables = variables;
                function.RuntimeCache.Expressions = expressions;
                function.RuntimeCache.Instructions= instructions;

            }

            var result = CallFunction(_source.Assembly._entryPoint);

            OnBufferClosed?.Invoke();
            TimeOfRunning = TimeSpan.FromMilliseconds((DateTime.Now - startTime).TotalMilliseconds);
            IsRunning = false;
            return result;
        }

        private RuntimeError CallFunction(Function function)
        {
            CallStackItem csi = new CallStackItem(function, (Integer)0);
            RuntimeDataPackage package = new RuntimeDataPackage()
            {
                Constants = function.RuntimeCache.Constants,
                Expressions = function.RuntimeCache.Expressions,
                MemZone = _machine.MemZone,
                RuntimeMachine = this,
                Assembly = _source.Assembly
            };

            CallStack.Add(csi);

            foreach (var variable in function.RuntimeCache.Variables)
            {
                _machine.MemZone.RAM.Add(new Variable(variable.VariableType, variable.Index));
            }

            for (; (int)csi.ProgramCounter < function.RuntimeCache.Instructions.Count; csi.ProgramCounter += (Integer)1)
            {
                Ticks++;
                
                var instruction = function.RuntimeCache.Instructions[(int)csi.ProgramCounter];

                if (Ticks == int.MaxValue)
                    return new RuntimeError(RuntimeOutputCode.StackOverFlow);

                //Если все ОК, то запускаем нашу инструкцию
                RuntimeOutputCode output = SourceLineInstruction.Instructions[(int)instruction.InstructionNumber].Apply(package, instruction.Parameters);

                if (output != RuntimeOutputCode.OK)
                    return new RuntimeError(output, instruction.Line);
            }

            return null;
        }
    }
}