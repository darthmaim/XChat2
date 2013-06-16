using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using XChat2.Common.Exceptions;

namespace XChat2.Common.Networking.P2P.Server
{
    public class HolePunchServer : IDisposable
    {
        private IPAddress _bind;
        public IPAddress BindAddress
        {
            get { return _bind; }
        }

        List<HolePunchClient> _registeredClients;

        private int _port;
        public int Port
        {
            get { return _port; }
        }

        Thread _runThread;
        bool _run = true;

        public HolePunchServer(IPAddress bind, int port)
        {
            _bind = bind;
            _port = port;

            _registeredClients = new List<HolePunchClient>();

            _runThread = new Thread(run);
            _runThread.IsBackground = true;
            _runThread.Name = "[HolePunchServerThread]";
            _runThread.Start();
        }

        public void Stop() {
            _run = false;
            if (_runThread != null && _runThread.IsAlive)
            {
                _runThread.Join(500);
                if (_runThread.IsAlive)
                    _runThread.Abort();
            }
        }

        public bool Running { get { return _run; } }

        private UdpClient _udp;

        private void run()
        {
            _udp = new UdpClient(new IPEndPoint(_bind, _port));

            while (_run)
            {
                if (_udp.Available >= 0)
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _udp.Receive(ref ep);
                    try
                    {
                        HandleIncoming(ep, data);
                    }
                    catch (P2PUnexpectedPacketException upe)
                    {
                        Console.WriteLine("[P2P Error] " + upe.ToString());
                    }
                }
                else
                    Thread.Sleep(100);
            }
        }

        private void HandleIncoming(IPEndPoint ep, byte[] data)
        {
            if (data[0] != (byte)KnownP2PPackets.Handshake)
            {
                throw new P2PUnexpectedPacketException(string.Format("Expected Handshake (0x{0}) but got 0x{1}", data[0].ToString("X2"), ((byte)KnownP2PPackets.Handshake).ToString("X2")));
            }
            byte[] authKey = new byte[data.Length - 1];
            Array.Copy(data, 1, authKey, 0, authKey.Length);

            HolePunchClient client;
            if (!TryGetClient(out client, authKey))
            {
                byte[] unauthenticatedPacket = P2PPacketCreator.CreateAuthenticationFailedPacket();
                _udp.Send(unauthenticatedPacket, unauthenticatedPacket.Length);
                return;
            }
            else
            {
                client.RemoteEP = ep;
                if (client.Partner.HasAuthenticated)
                {
                    //Send both authenticated packet...
                    byte[] authPacket1 = P2PPacketCreator.CreateAuthenticatedPacket(client.Partner.RemoteEP.Address, client.Partner.RemoteEP.Port);
                    byte[] authPacket2 = P2PPacketCreator.CreateAuthenticatedPacket(client.RemoteEP.Address, client.RemoteEP.Port);

                    _udp.Send(authPacket1, authPacket1.Length, client.RemoteEP);
                    _udp.Send(authPacket2, authPacket2.Length, client.Partner.RemoteEP);

                    //That was all the server has todo -> remove clients from list
                    _registeredClients.Remove(client);
                    _registeredClients.Remove(client.Partner);
                }
                else
                {
                    client.HasAuthenticated = true;
                }
            }
        }

        public void RegisterClients(byte[] authKey1, byte[] authKey2)
        {
            if (AuthKeyExists(authKey1, authKey2))
                throw new Exception("Those authentication keys are already registered.");
        
            HolePunchClient client1 = new HolePunchClient(authKey1);
            HolePunchClient client2 = new HolePunchClient(authKey2);
            client1.Partner = client2;
            client2.Partner = client1;

            _registeredClients.Add(client1);
            _registeredClients.Add(client2);
        }

        /// <summary>
        /// Returns true if any of the given authKeys exists.
        /// </summary>
        public bool AuthKeyExists(params byte[][] authKeys)
        {
            foreach (HolePunchClient c in _registeredClients)
                foreach (byte[] key in authKeys)
                    if (c.AuthenticationKey.SequenceEqual(key))
                        return true;

            return false;
        }

        internal bool TryGetClient(out HolePunchClient client, byte[] authKey)
        {
            foreach (HolePunchClient c in _registeredClients)
                if (c.AuthenticationKey.SequenceEqual(authKey))
                {
                    client = c;
                    return true;
                }
            client = null;
            return false;
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
