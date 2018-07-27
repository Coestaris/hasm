using System;
using System.Threading;

namespace HASMLib.Parser.SourceParsing
{
    public abstract class ParseTask
    {
        public abstract string Name { get; }
        public TimeSpan Length { get; private set; }

        public ParseError Error { get; protected set; }
        public ParseTaskStatus Status { get; private set; } = ParseTaskStatus.Waiting;

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        protected Thread workingThread;
        protected HASMSource source;

        protected abstract void InnerReset();
        protected abstract void InnerRun();

        public void Reset()
        {
            Length = TimeSpan.Zero;
            Status = ParseTaskStatus.Waiting;
            Error = null;

            InnerReset();
        }

        public void RunAsync(HASMSource source)
        {
            workingThread = new Thread(p => Run(source));
            workingThread.Start();
        }

        public void Run(HASMSource source)
        {
            this.source = source;
            StartTime = DateTime.Now;
            Status = ParseTaskStatus.Running;
            InnerRun();
        }

        protected void InnerEnd()
        {
            EndTime = DateTime.Now;
            Length = TimeSpan.FromMilliseconds((EndTime - StartTime).TotalMilliseconds);
            Status = ParseTaskStatus.Ok;
        }

        protected void InnerEnd(ParseError error)
        {
            EndTime = DateTime.Now;
            Length = TimeSpan.FromMilliseconds((EndTime - StartTime).TotalMilliseconds);
            Status = ParseTaskStatus.Failed;
            Error = error;
        }

        public virtual void Abort()
        {
            workingThread?.Abort();
            Status = ParseTaskStatus.Aborted;
        }
    }
}
