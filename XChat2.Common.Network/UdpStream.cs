using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace XChat2.Common.Networking
{
    public class UdpStream : Stream
    {
        private UdpClient _client;
        public UdpClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        Queue<byte> _sendBuffer;
        Queue<byte> _receiveBuffer;

        public int Available
        {
            get { return _client.Available; }
        }
        
        public UdpStream(UdpClient client) : this(client, true) { }
        public UdpStream(UdpClient client, bool useBuffer) : base()
        {
            _client = client;
            _sendBuffer = new Queue<byte>();
            _receiveBuffer = new Queue<byte>();
            _useBuffer = useBuffer;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Close()
        {
            _client.Close();    
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _client.Close();
            base.Dispose(disposing);
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int WriteTimeout
        {
            get { return _client.Client.SendTimeout; }
            set { _client.Client.SendTimeout = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        private bool _useBuffer = true;
        public bool UseBuffer
        {
            get { return _useBuffer; }
            set {
                if (_useBuffer != value)
                {
                    _useBuffer = value;
                    if (_useBuffer)
                    {
                        startThreads();
                    }
                }
            }
        }

        private void startThreads()
        {
            _sendThread = new Thread(sendThread);
            _sendThread.Name = "[UdpStreamSendThread(" + _client.Client.RemoteEndPoint.ToString() + ")]";
            _sendThread.Start();
        }

        private int _bufferFlushInterval = 100;
        public int BufferFlushInterval
        {
            get { return _bufferFlushInterval; }
            set { _bufferFlushInterval = value; }
        }

        #region Send
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_useBuffer)
            {
                lock (_sendLock)
                    for (int i = 0; i < count; i++)
                        _sendBuffer.Enqueue(buffer[i + offset]);
            }
            else
            {
                internalWrite(buffer, offset, count);
            }
        }

        private void internalWrite(byte[] buffer, int offset, int count)
        {
            if (offset == 0)
                _client.Send(buffer, count);
            else
            {
                _client.Send(buffer.Skip(offset).ToArray(), count);
            }
        }

        public override void WriteByte(byte value)
        {
            if (_useBuffer)
                _sendBuffer.Enqueue(value);
            else
                _client.Send(new byte[] { value }, 1);
        }

        private Thread _sendThread;
        private object _sendLock = new object();
        private void sendThread()
        {
            while (_useBuffer)
            {
                if (_sendBuffer.Count > 0 && _bufferFlushInterval >= 0)
                {
                    lock(_sendLock)
                        internalWrite(_sendBuffer.ToArray(), 0, _sendBuffer.Count);
                }

                if (_bufferFlushInterval >= 0)
                    Thread.Sleep(_bufferFlushInterval);
                else
                    Thread.Sleep(100);
            }
        }
        #endregion

        #region Read

        private Thread _readThread;
        private object _readLock = new object();
        private void readThread()
        {
            while (_useBuffer)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
