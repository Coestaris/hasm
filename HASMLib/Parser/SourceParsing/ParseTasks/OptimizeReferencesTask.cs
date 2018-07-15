using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        protected override void InnerRun()
        {
            Dictionary<Integer, Integer> plainIndexes = new Dictionary<Integer, Integer>();
            Integer ind = (Integer)0;

            //Помещаем все константы в начало флеша для удобства дебага
            //Выбираем с массива константы
            var constants = source.ParseResult
                .FindAll(p => p.Type == MemZoneFlashElementType.Constant)
                .Select(p => p as MemZoneFlashElementConstant)
                .GroupBy(p => p.Value)
                .Select(p =>
                {
                    List<Integer> indexes = new List<Integer>();

                    foreach (var item in p)
                    {
                        indexes.Add(item.Index);
                        plainIndexes.Add(item.Index, ind);
                    }

                    ConstnantGrouping grouping = new ConstnantGrouping(p.Key, indexes, ind);
                    ind += (Integer)1;

                    return grouping;
                })
                .ToList();
            
            //Удаляем их из коллекции
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);

            var expressions = source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Expression);

            //Удаляем их из коллекции
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Expression);

            foreach (MemZoneFlashElementInstruction instruction in source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Instruction))
                foreach (ObjectReference reference in instruction.Parameters)
                    if(reference.Type == ReferenceType.Constant)
                        reference.Index = plainIndexes[reference.Index];

            //Пихаем в ее начало
            source.ParseResult.InsertRange(0, constants.Select(
                p => new MemZoneFlashElementConstant(p.Value, p.NewIndex)));

            //Пихаем в ее начало
            source.ParseResult.InsertRange(0, expressions);

            foreach (MemZoneFlashElementExpression expression in source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Expression))
            {
                expression.Expression.Calculate(null, false, true);
            }

            //Если размер программы превышает максимально допустимый для этой машины
            int totalFlashSize = source.ParseResult.Sum(p => p.FixedSize);
            if (totalFlashSize > source.Machine.Flash)
            {
                var parseError = new ParseError(ParseErrorType.Other_OutOfFlash);
                InnerEnd(true, parseError);
                return;
            }

            InnerEnd(false, null);
        }
    }
}
