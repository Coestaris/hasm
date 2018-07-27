using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
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

        private List<MemZoneFlashElementInstruction> Instructions;
        private List<MemZoneFlashElementExpression> Expressions;
        private List<ConstnantGrouping> Constnants;

        private void GetComponents()
        {
            Instructions = source.ParseResult
                .FindAll(p => p.Type == MemZoneFlashElementType.Instruction)
                .Select(p => p as MemZoneFlashElementInstruction)
                .ToList();

            Expressions = source.ParseResult
                .FindAll(p => p.Type == MemZoneFlashElementType.Expression)
                .Select(p => p as MemZoneFlashElementExpression)
                .ToList();

            //Удаляем их из коллекции
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Expression);

            //Удаляем их из коллекции
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Instruction);
        }

        private void ResolveExpressions()
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

                    source.ParseResult.Add(new MemZoneFlashElementConstant(
                        flashElement.Expression.TokenTree.Value.Value,
                        constIndex));

                    plainExpIndexes.Add(flashElement.Index, constIndex);

                    Expressions.RemoveAt(i);
                }
            }

            foreach (MemZoneFlashElementInstruction instruction in Instructions)
                foreach (ObjectReference reference in instruction.Parameters)
                    if (reference.Type == ReferenceType.Expression && plainExpIndexes.ContainsKey(reference.Index))
                    {
                        reference.Index = plainExpIndexes[reference.Index];
                        reference.Type = ReferenceType.Constant;
                    }
        }

        private void ResolveConsts()
        {
            Dictionary<Integer, Integer> plainConstIndexes = new Dictionary<Integer, Integer>();
            Integer ind = (Integer)0;

            //Помещаем все константы в начало флеша для удобства дебага
            //Выбираем с массива константы
            Constnants = source.ParseResult
                .FindAll(p => p.Type == MemZoneFlashElementType.Constant)
                .Select(p => p as MemZoneFlashElementConstant)
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
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);

            foreach (MemZoneFlashElementInstruction instruction in Instructions)
                foreach (ObjectReference reference in instruction.Parameters)
                    if (reference.Type == ReferenceType.Constant)
                        reference.Index = plainConstIndexes[reference.Index];
        }

        private void JoinComponents()
        {
            //Пихаем в ее начало
            source.ParseResult.InsertRange(0, Constnants.Select(
                p => new MemZoneFlashElementConstant(p.Value, p.NewIndex)));

            //Пихаем в ее начало
            source.ParseResult.InsertRange(0, Expressions);

            source.ParseResult.AddRange(Instructions);
        }

        private void Other()
        {
            //Если размер программы превышает максимально допустимый для этой машины
            int totalFlashSize = source.ParseResult.Sum(p => p.FixedSize);
            if (totalFlashSize > source.Machine.Flash)
            {
                var parseError = new ParseError(ParseErrorType.Other_OutOfFlash);
                InnerEnd(parseError);
                return;
            }
        }

        protected override void InnerRun()
        {
            GetComponents();
            ResolveExpressions();
            ResolveConsts();
            JoinComponents();

            Other();

            InnerEnd();
        }
    }
}
