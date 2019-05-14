using System;
using System.IO;

namespace Emignatik.NxFileViewer.Utils
{
    /// <summary>
    /// Disposing this stream will not dispose the underlying stream
    /// </summary>
    public class SubStream : Stream
    {
        private readonly Stream _originalStream;
        private readonly long _streamPosStart;
        private readonly long _streamLength;
        private readonly long _streamPosEnd;

        public event DisposedHandler Disposed;

        public SubStream(Stream originalStream, long streamPosStart, long streamLength)
        {
            _originalStream = originalStream ?? throw new ArgumentNullException(nameof(originalStream));
            _streamPosStart = streamPosStart;
            _streamLength = streamLength;
            _streamPosEnd = streamPosStart + streamLength;
            _originalStream.Position = streamPosStart;
        }

        protected override void Dispose(bool disposing)
        {
            var onDisposed = Disposed;
            onDisposed?.Invoke(this, new DisposedHandlerArgs());
            // Do not dispose the original stream.
        }

        public override void Flush()
        {
            throw new NotSupportedException($"\"{nameof(SubStream)}\" doesn't support \"{nameof(Flush)}\".");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException($"\"{nameof(SubStream)}\" doesn't support \"{nameof(Seek)}\".");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"\"{nameof(SubStream)}\" doesn't support \"{nameof(SetLength)}\".");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var remainingNumberOfBytes = _streamPosEnd - _originalStream.Position;
            var effectiveCount = Math.Min(count, remainingNumberOfBytes);

            var read = _originalStream.Read(buffer, offset, (int)effectiveCount);

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"\"{nameof(SubStream)}\" is readonly.");
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _streamLength;

        public override long Position
        {
            get
            {
                var relPos = _originalStream.Position - _streamPosStart;
                return relPos;
            }
            set
            {
                var realPos = _streamPosStart + value;
                if (realPos < _streamPosStart || realPos > _streamPosEnd) throw new Exception("Position is out of stream.");
                _originalStream.Position = realPos;
            }
        }
    }

    public class SubStream<T> : SubStream
    {
        public SubStream(Stream originalStream, long streamPosStart, long streamLength)
            : base(originalStream, streamPosStart, streamLength)
        {
        }
        public T AttachedData { get; set; }
    }

    public delegate void DisposedHandler(object sender, DisposedHandlerArgs args);

    public class DisposedHandlerArgs
    {
    }
}