using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Core
{
    internal class CheckRevisionStream : Stream
    {
        private long _curPos, _len;
        private Dictionary<long, Stream> _sourceStreams;

        public CheckRevisionStream(IEnumerable<Stream> sourceStreams)
        {
            _sourceStreams = new Dictionary<long, Stream>();

            long curPos = 0;
            foreach (Stream stream in sourceStreams)
            {
                _sourceStreams.Add(curPos, stream);
                int remainder = CalculatePaddedBufferSize(stream.Length);
                curPos += stream.Length;
                if (remainder > 0)
                {
                    Stream paddingStream = CreatePaddingStream(remainder);
                    _sourceStreams.Add(curPos, paddingStream);
                    _curPos += remainder;
                }
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { return _len; }
        }

        public override long Position
        {
            get
            {
                return _curPos;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // TODO: Figure out
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
            throw new NotSupportedException();
        }


        private static int CalculatePaddedBufferSize(long baseFileSize)
        {
            int remainder = baseFileSize % 1024;
            
            if (remainder == 0) return 0;
            else return 1024 - remainder;

            //if (remainder == 0)
            //    return baseFileSize;
            //else
            //    return baseFileSize - remainder + 1024;
        }

        private static Stream CreatePaddingStream(int remainingSize)
        {
            MemoryStream stream = new MemoryStream(remainingSize);
            byte currentPaddingByte = 0xff;
            for (int i = 0; i < remainingSize; i++)
            {
                unchecked
                {
                    stream.WriteByte(currentPaddingByte--);
                }
            }
            return stream;
        }
    }
}
