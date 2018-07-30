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

        private void GetComponents(Runtime.Structures.Units.Function function)
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
        }

        private void ResolveExpressions(Runtime.Structures.Units.Function function)
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
        }

        private void ResolveConsts(Runtime.Structures.Units.Function function)
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
        }

        private void JoinComponents(Runtime.Structures.Units.Function function)
        {
            //Пихаем в ее начало
            function.CompileCache.Compiled.InsertRange(0, Constnants.Select(
                p => new FlashElementConstant(p.Value, p.NewIndex)));

            //Пихаем в ее начало
            function.CompileCache.Compiled.InsertRange(0, Expressions);

            function.CompileCache.Compiled.AddRange(Instructions);
        }

        private void Other(Runtime.Structures.Units.Function function)
        {
            //Если размер программы превышает максимально допустимый для этой машины
            int totalFlashSize = function.CompileCache.Compiled.Sum(p => p.FixedSize);
            if (totalFlashSize > source.Machine.Flash)
            {
                var parseError = new ParseError(ParseErrorType.Other_OutOfFlash);
                InnerEnd(parseError);
                return;
            }
        }

        protected override void InnerRun()
        {
            foreach (var function in source.Assembly.AllFunctions)
            {
                GetComponents(function);
                ResolveExpressions(function);
                ResolveConsts(function);
                JoinComponents(function);
                Other(function);
            }

            InnerEnd();
        }
    }
}
