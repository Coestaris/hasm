using HASMLib.Core.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASMLib.Runtime
{
    public class IOStream
    {
        private RuntimeMachine _runtimeMachine;
        private bool _isOpened;
        private bool _isInited;
        private List<Integer> _buffer;

        public string StreamName;
        public StreamDirection Direction;

        private void UpdateBuffer(string streamName, List<Integer> inBuffer)
        {
            if(Direction == StreamDirection.Out && streamName == StreamName)
                _buffer.AddRange(inBuffer);
        }

        public IOStream(string name, StreamDirection direction)
        {
            Direction = direction;
            StreamName = name;
        }

        internal void Init(RuntimeMachine runtimeMachine)
        {
            _isInited = true;
            _runtimeMachine = runtimeMachine;
            _runtimeMachine.OutBufferUpdated += UpdateBuffer;
            _runtimeMachine.OnBufferFlushed += Flushed;
            _runtimeMachine.OnBufferClosed += Closed;
            Flush();
        }

        private void Closed()
        {
            _isOpened = false;
        }

        private void Flushed()
        {
            _isOpened = true;
            Flush();
        }

        public bool IsOpened
        {
            get
            {
                return _isInited && _isOpened;
            }
        }

        public bool CanRead => _isInited && _buffer.Count != 0;

        public bool CanSeek => false;

        public bool CanWrite => IsOpened;

        public long Length => _buffer.Count;

        public long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Flush()
        {
            _buffer = new List<Integer>();
        }

        public List<Integer> ReadAll()
        {
            if (Direction != StreamDirection.Out)
                throw new InvalidOperationException();

            Integer[] buffer = new Integer[Length];
            Read(buffer, 0, (int)Length);
            return buffer.ToList();
        }

        public int Read(Integer[] buffer, int offset, int count)
        {
            if (Direction != StreamDirection.Out)
                throw new InvalidOperationException();

            if (count > _buffer.Count)
                throw new ArgumentException();

            _buffer.CopyTo(0, buffer, offset, count);
            _buffer.RemoveRange(0, count);
            return count;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public void Write(Integer[] buffer, int offset, int count)
        {
            if (Direction != StreamDirection.In)
                throw new InvalidOperationException();

            List<Integer> inBuffer = buffer.Skip(offset).Take(count).ToList();
            _runtimeMachine.InbufferRecieved(inBuffer);
        }
    }
}
