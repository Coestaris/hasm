using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime
{
    public class IOStream : Stream
    {
        private RuntimeMachine _runtimeMachine;

        private List<byte> _buffer;

        private void UpdateBuffer()
        {
            _buffer.AddRange(_runtimeMachine.OutBuffer);
        }

        public IOStream()
        {

        }

        internal void Init(RuntimeMachine runtimeMachine)
        {
            _isInited = true;
            _runtimeMachine = runtimeMachine;
            _runtimeMachine.OutBufferUpdated += UpdateBuffer;
            Flush();
        }

        private bool _isInited;

        public bool IsOpened
        {
            get
            {
                return _isInited && _runtimeMachine.IsRunning;
            }
        }

        public override bool CanRead => _isInited && _buffer.Count != 0;

        public override bool CanSeek => false;

        public override bool CanWrite => _isInited && IsOpened;

        public override long Length => _buffer.Count;

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            _buffer = new List<byte>();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > _buffer.Count)
                throw new ArgumentException();

            Buffer.BlockCopy(_buffer.ToArray(), 0, buffer, offset, count);
            _buffer.RemoveRange(0, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            List<byte> inBuffer = buffer.Skip(offset).Take(count).ToList();
            _runtimeMachine.InbufferRecieved(inBuffer);
        }
    }
}
