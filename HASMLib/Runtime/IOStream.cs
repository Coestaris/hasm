using HASMLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime
{
    public class IOStream
    {
        private RuntimeMachine _runtimeMachine;

        private List<UIntSingle> _buffer;

        private void UpdateBuffer(List<UIntSingle> inBuffer)
        {
            _buffer.AddRange(inBuffer);
        }

        public IOStream()
        {

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

        private bool _isOpened;
        private bool _isInited;

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
            _buffer = new List<UIntSingle>();
        }

        public List<UIntSingle> ReadAll()
        {
            UIntSingle[] buffer = new UIntSingle[Length];
            Read(buffer, 0, (int)Length);
            return buffer.ToList();
        }

        public int Read(UIntSingle[] buffer, int offset, int count)
        {
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

        public void Write(UIntSingle[] buffer, int offset, int count)
        {
            List<UIntSingle> inBuffer = buffer.Skip(offset).Take(count).ToList();
            _runtimeMachine.InbufferRecieved(inBuffer);
        }
    }
}
