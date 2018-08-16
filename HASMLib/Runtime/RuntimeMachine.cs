using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using HASMLib.Parser;
using HASMLib.Runtime.Structures.Units;
using HASMLib.Runtime.Structures;

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

        private bool _funcReturned;
        private HASMMachine _machine;
        private HASMSource _source;
        private Stack<CallStackItem> _callStack;

        internal event RuntimeMachineIOHandler OnBufferFlushed;
        internal event RuntimeMachineIOHandler OnBufferClosed;
        internal event RuntimeMachineIOBufferHandler OutBufferUpdated;

        public RuntimeMachine(HASMMachine machine, HASMSource source)
        {
            _machine = machine;
            _source = source;
        }

        private RuntimeError CreateError(RuntimeOutputCode code, Function function)
        {
            return new RuntimeError(code, function.Directive);
        }

        private RuntimeError CreateError(RuntimeOutputCode code, FlashElementInstruction instruction)
        {
            return new RuntimeError(code, instruction.Line);
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


        public RuntimeError Run()
        {
            DateTime startTime = DateTime.Now;
            IsRunning = true;
            _funcReturned = false;

            Ticks = 0;
            InBuffer = new List<Integer>();
            _callStack = new Stack<CallStackItem>();

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

        internal void Return()
        {
            _funcReturned = true;
        }

        internal RuntimeError CallFunction(Function function)
        {
            Integer localVarCounter = (Integer)1;
            CallStackItem csi = new CallStackItem(function, (Integer)0);

            if (!function.IsStatic && !function.IsEntryPoint)
                csi.Locals.Add(new Variable(_source.Machine.MemZone.ObjectStackItem, localVarCounter += (Integer)1));

            foreach (var parameter in function.Parameters)
            {
                if (_machine.MemZone.ParamStack.Count == 0)
                    return CreateError(RuntimeOutputCode.ArgumentsAreExpected, function);

                Structures.Object value = _machine.MemZone.ParamStack.Pop();
                TypeReference type = parameter.Type;

                if(value.Type != type)
                {
                    return CreateError(RuntimeOutputCode.ExpectedOtherType, function);
                }

                csi.Locals.Add(new Variable(value, localVarCounter));
                localVarCounter += (Integer)1;
            }

            RuntimeDataPackage package = new RuntimeDataPackage()
            {
                Constants = function.RuntimeCache.Constants,
                Expressions = function.RuntimeCache.Expressions,
                MemZone = _machine.MemZone,
                RuntimeMachine = this,
                Assembly = _source.Assembly,
                CallStackItem = csi
            };

            _callStack.Push(csi);

            //foreach (var variable in function.RuntimeCache.Variables)
            //{
                //_machine.MemZone.Globals.Add(new Variable(variable.VariableType, variable.Index));
            //}

            for (; (int)csi.ProgramCounter < function.RuntimeCache.Instructions.Count; csi.ProgramCounter += (Integer)1)
            {
                Ticks++;
                
                var instruction = function.RuntimeCache.Instructions[(int)csi.ProgramCounter];

                if (Ticks == int.MaxValue)
                    return new RuntimeError(RuntimeOutputCode.StackOverFlow);

                //Если все ОК, то запускаем нашу инструкцию
                RuntimeOutputCode output = SourceLineInstruction.Instructions[(int)instruction.InstructionNumber].Apply(package, instruction.Parameters);

                if (output != RuntimeOutputCode.OK)
                    return CreateError(output, instruction);

                if (_funcReturned)
                {
                    _funcReturned = false;
                    _callStack.Pop();
                    return null;
                }
            }

            return null;
        }
    }
}