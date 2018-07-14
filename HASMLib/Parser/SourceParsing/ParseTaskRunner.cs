using HASMLib.Parser.SourceParsing.ParseTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HASMLib.Parser.SourceParsing
{
    public class ParseTaskRunner
    {
        public ParseTaskStatus Status { get; private set; } =  ParseTaskStatus.Waiting;
        private Thread workingThread;

        public int FailedTaskIndex;

        public List<ParseTask> Tasks = new List<ParseTask>()
        {
            new PrepareTask(),
            new PreprocessorTask(),
            new InstructionsTask(),
            new OptimizeReferencesTask()
        };

        public delegate void AsyncParseEndDelegate(ParseTaskRunner runner, HASMSource Source);
        public delegate void AsyncTaskChangedDelegate(ParseTaskRunner runner, HASMSource Source);

        public event AsyncParseEndDelegate AsyncParseEnd;
        public event AsyncTaskChangedDelegate AsyncTaskСhanged;

        private HASMSource Source;

        public ParseTaskRunner(HASMSource source)
        {
            Source = source;
        }

        public void Reset()
        {
            Status = ParseTaskStatus.Waiting;
            foreach (var task in Tasks)
                task.Reset();
        }

        public void AbortAsync()
        {
            workingThread?.Abort();
            Status = ParseTaskStatus.Aborted;
        }


        public void Run()
        {
            Status = ParseTaskStatus.Running;
            int index = 0;
            foreach (var task in Tasks)
            {
                index++;
                AsyncTaskСhanged?.Invoke(this, Source);
                task.Run(Source);
                AsyncTaskСhanged?.Invoke(this, Source);

                if (task.Status != ParseTaskStatus.Ok)
                {
                    FailedTaskIndex = index - 1;
                    Status = ParseTaskStatus.Failed;
                    AsyncParseEnd?.Invoke(this, Source);

                    return;
                }
            }
            Status = ParseTaskStatus.Ok;

            Source.ParseTime = TimeSpan.Zero;
            Tasks.ForEach(p => Source.ParseTime += p.Length);

            AsyncParseEnd?.Invoke(this, Source);
        }

        public void RunAsync()
        {
            workingThread = new Thread(Run);
            workingThread.Start();
        }
    }
}
