using HASMLib.Core.MemoryZone;
using System.Linq;
using System.Threading;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    internal class OptimizeReferencesTask : ParseTask
    {
        public override string Name => "Optimizing references in flash-elemetnts";

        protected override void InnerReset() { }

        protected override void InnerRun()
        {
            //Помещаем все константы в начало флеша для удобства дебага
            //Выбираем с массива константы
            var constants = source.ParseResult.FindAll(p => p.Type == MemZoneFlashElementType.Constant).ToList();
            
            //Удаляем их из коллекции
            source.ParseResult.RemoveAll(p => p.Type == MemZoneFlashElementType.Constant);
            
            //Пихаем в ее начало
            source.ParseResult.InsertRange(0, constants);

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
