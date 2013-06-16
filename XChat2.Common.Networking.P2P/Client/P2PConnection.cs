using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using XChat2.Common.Helper;
using XChat2.Common.Exceptions;
using System.Threading;

namespace XChat2.Common.Networking.P2P.Client
{
    public class P2PConnection : IDisposable
    {
        private byte[] _serverAuthKey;
        public byte[] ServerAuthenticationKey
        {
            get { return _serverAuthKey; }
        }

        private string _serverHostname;
        public string ServerHostname
        {
            get { return _serverHostname; }
        }

        private int _serverPort;
        public int ServerPort
        {
            get { return _serverPort; }
        }

        private IPEndPoint _server;
        public IPEndPoint Server
        {
            get { return _server; }
        }

        private bool _established = false;
        public bool Established
        {
            get { return _established; }
        }
        
        private UdpClient _client; 

        public P2PConnection(string serverHostname, int serverPort, byte[] serverAuthKey)
        {
            _serverAuthKey = serverAuthKey;

            _serverHostname = serverHostname;
            _serverPort = serverPort;

            _server = NetworkHelper.GetIPEndPointFromHostName(_serverHostname, _serverPort, false);

            _client = new UdpClient();
        }

        public void StartAsync()
        {
            ThreadPool.QueueUserWorkItem((o) => Start());
        }

        public bool Start()
        {
            //Send Handshake
            byte[] serverHandshake = P2PPacketCreator.CreateServerHandshake(_serverAuthKey);
            _client.Send(serverHandshake, serverHandshake.Length, _server);

            if (!Misc.WaitFor<UdpClient>(_client, c => c.Available > 0, 10000, 100))
            {
                throw new TimeoutException("The server hasn't responded in time");
            }

            //Receive response
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] response = _client.Receive(ref remoteEndPoint);

            switch (response[0])
            {
                case (byte)KnownP2PPackets.Authenticated:
                    return ContinueAfterAuthentication(response);
                case (byte)KnownP2PPackets.AuthenticationFailed:
                    throw new P2PAuthenticationFailedException("Authentication failed");
                default:
                    throw new P2PUnexpectedPacketException(string.Format("Expected 0x{0} or 0x{1}, but got 0x{2}",
                        ((byte)KnownP2PPackets.Authenticated).ToString("X2"), ((byte)KnownP2PPackets.Authenticated).ToString("X2"),response[0].ToString("X2")));
            }
        }

        private IPAddress _address;
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private bool ContinueAfterAuthentication(byte[] response)
        {
            P2PPacketCreator.AuthenticatedPacket authPacket = P2PPacketCreator.ReadAuthenticatedPacket(response);
            _address = authPacket.IPAddress;
            _port = authPacket.Port;
            _client.Connect(_address, _port);

            int tries = 0;
            do
            {
                Holepunch();
                Thread.Sleep(100);
                tries++;
            } while (_client.Available == 0 && tries < 30);

            if (_client.Available == 0) {
                Console.WriteLine("[Holepunch] No data received after {0} tries", tries);
                return false;
            }

            Console.WriteLine("[Holepunch] Punched hole after {0} tries.", tries);

            ClearHolepunchMessages();
            
            _established = true;
            ConnectionEstablished.Raise(this, new P2PConnectionEstablishedEventArgs(true));
            return true;
        }

        private void ClearHolepunchMessages()
        {
            byte[] dataStartPacket = P2PPacketCreator.CreateStartPacket();
            _client.Send(dataStartPacket, dataStartPacket.Length);

            byte[] packet;
            do
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                packet = _client.Receive(ref ep);
            } while (!packet.SequenceEqual(dataStartPacket));
        }

        private void Holepunch()
        {
            _client.Send(new byte[] { 0x00 }, 1);
        }

        public void Dispose()
        { }

        public void Send(byte[] data)
        {
            if (!_established)
                throw new Exception("P2P Connection not established yet");
            _client.Send(data, data.Length);
        }

        public byte[] Read()
        {
            if (!_established)
                throw new Exception("P2P Connection not established yet");
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            return _client.Receive(ref ep);
        }

        public bool DataAvailable
        {
            get { return _established && _client.Available > 0; }
        }

        public event EventHandler<P2PConnectionEstablishedEventArgs> ConnectionEstablished;
    }

    public class P2PConnectionEstablishedEventArgs : EventArgs
    {
        public P2PConnectionEstablishedEventArgs(bool success) : this(success, null) { }
        public P2PConnectionEstablishedEventArgs(bool success, P2PException exception)
        {
            _success = success;
            _exception = exception;
        }
        private bool _success;
        public bool Success
        {
            get { return _success; }
            private set { _success = value; }
        }

        private P2PException _exception;
        /// <summary>
        /// Currently not used.
        /// </summary>
        public P2PException Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
        
        
    }
}
