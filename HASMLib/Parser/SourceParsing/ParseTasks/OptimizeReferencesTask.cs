using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class OptimizeReferencesTask : ParseTask
    {
        public override string Name => "Optimizing references in flash-elemetnts";

        protected override void InnerReset() { }

        private struct ConstnantGrouping
        {
            public Integer Value;
            public Integer NewIndex;
            public List<Integer> Indexes;

            public ConstnantGrouping(Integer value, List<Integer> indexes, Integer ind)
            {
                NewIndex = ind;
                Value = value;
                Indexes = indexes;
            }
        }

        private List<FlashElementInstruction> Instructions;
        private List<FlashElementExpression> Expressions;
        private List<ConstnantGrouping> Constnants;

        private int _retInstructionIndex = SourceLineInstruction.Instructions.Find(p => p.NameString == "ret").Index;
        private int _passInstructionIndex = SourceLineInstruction.Instructions.Find(p => p.NameString == "pass").Index;


        private ParseError GetComponents(Runtime.Structures.Units.Function function)
        {
            Instructions = function.CompileCache.Compiled
                .FindAll(p => p.Type == FlashElementType.Instruction)
                .Select(p => p as FlashElementInstruction)
                .ToList();

            Expressions = function.CompileCache.Compiled
                .FindAll(p => p.Type == FlashElementType.Expression)
                .Select(p => p as FlashElementExpression)
                .ToList();

            //Удаляем их из коллекции
            function.CompileCache.Compiled.RemoveAll(p => p.Type == FlashElementType.Expression);

            //Удаляем их из коллекции
            function.CompileCache.Compiled.RemoveAll(p => p.Type == FlashElementType.Instruction);

            return null;
        }

        private ParseError ResolveExpressions(Runtime.Structures.Units.Function function)
        {
            Dictionary<Integer, Integer> plainExpIndexes = new Dictionary<Integer, Integer>();

            for (int i = Expressions.Count - 1; i >= 0; i--)
            {
                var flashElement = Expressions[i];
                flashElement.Expression.Calculate(null, false, true);

                if (flashElement.Expression.TokenTree.Value != null)
                {
                    //Integer constIndex = (Integer)(++source._constIndex);

                    Integer constIndex = (Integer)0;

                    function.CompileCache.Compiled.Add(new FlashElementConstant(
                        flashElement.Expression.TokenTree.Value.Value,
                        constIndex));

                    plainExpIndexes.Add(flashElement.Index, constIndex);

                    Expressions.RemoveAt(i);
                }
            }

            foreach (FlashElementInstruction instruction in Instructions)
                if(instruction.Parameters != null)
                    foreach (ObjectReference reference in instruction.Parameters)
                        if (reference.Type == ReferenceType.Expression && plainExpIndexes.ContainsKey(reference.Index))
                        {
                            reference.Index = plainExpIndexes[reference.Index];
                            reference.Type = ReferenceType.Constant;
                        }

            return null;
        }

        private ParseError ResolveConsts(Runtime.Structures.Units.Function function)
        {
            Dictionary<Integer, Integer> plainConstIndexes = new Dictionary<Integer, Integer>();
            Integer ind = (Integer)0;

            //Помещаем все константы в начало флеша для удобства дебага
            //Выбираем с массива константы
            Constnants = function.CompileCache.Compiled
                .FindAll(p => p.Type == FlashElementType.Constant)
                .Select(p => p as FlashElementConstant)
                .GroupBy(p => p.Value)
                .Select(p =>
                {
                    List<Integer> indexes = new List<Integer>();

                    foreach (var item in p)
                    {
                        indexes.Add(item.Index);
                        plainConstIndexes.Add(item.Index, ind);
                    }

                    ConstnantGrouping grouping = new ConstnantGrouping(p.Key, indexes, ind);
                    ind += (Integer)1;

                    return grouping;
                })
                .ToList();

            //Удаляем их из коллекции
            function.CompileCache.Compiled.RemoveAll(p => p.Type == FlashElementType.Constant);

            foreach (FlashElementInstruction instruction in Instructions)
                if (instruction.Parameters != null)
                    foreach (ObjectReference reference in instruction.Parameters)
                        if (reference.Type == ReferenceType.Constant)
                            reference.Index = plainConstIndexes[reference.Index];

            return null;
        }

        private ParseError JoinComponents(Runtime.Structures.Units.Function function)
        {
            //Пихаем в ее начало
            function.CompileCache.Compiled.InsertRange(0, Constnants.Select(
                p => new FlashElementConstant(p.Value, p.NewIndex)));

            //Пихаем в ее начало
            function.CompileCache.Compiled.InsertRange(0, Expressions);

            function.CompileCache.Compiled.AddRange(Instructions);

            return null;
        }

        private ParseError Other(Runtime.Structures.Units.Function function)
        {
            //Если размер программы превышает максимально допустимый для этой машины
            int totalFlashSize = function.CompileCache.Compiled.Sum(p => p.FixedSize);
            if (totalFlashSize > source.Machine.Flash)
            {
                return new ParseError(ParseErrorType.Other_OutOfFlash, function.Directive);
            }
            return null;
        }

        private ParseError CheckRetEnding(Runtime.Structures.Units.Function function)
        {
            var last = Instructions.Last();
            
            if(last.InstructionNumber != (Integer)_retInstructionIndex && last.InstructionNumber != (Integer)_passInstructionIndex)
            {
                return new ParseError(ParseErrorType.Other_FunctionMustEndsWithPassOrRet, function.Directive);
            }

            return null;
        }

        protected override void InnerRun()
        {
            foreach (var function in source.Assembly.AllFunctions)
            {
                ParseError err = GetComponents(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }

                err = ResolveExpressions(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }

                err = ResolveConsts(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }

                err = JoinComponents(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }

                err = Other(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }

                err = CheckRetEnding(function);
                if (err != null)
                {
                    InnerEnd(err);
                    return;
                }
            }

            InnerEnd();
        }
    }
}
