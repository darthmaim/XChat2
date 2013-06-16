using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Packets = XChat2.Common.Networking.Packets.Packets;
using XChat2.Common.Collections;
using System.Collections.ObjectModel;
using XChat2.Common.Networking.Packets;
using XChat2.Common.Exceptions;
using XChat2.Common.Hashes;
using XChat2.Client.Data;
using XChat2.Client.Data.ChatMessages;
using XChat2.Common.Helper;
using XChat2.Client.Communication.PacketHandlers;

namespace XChat2.Client.Communication
{
    public class ClientConnection : IConnection
    {
        public enum States
        {
            Disconnected,
            Connecting,
            Connected,
            Disconnecting
        }

        #region State
        private States _state = States.Disconnected;
        public States State
        {
            get { return _state; }
            private set
            {
                States oldState = _state;
                _state = value;

                OnStateChanged(_state, oldState);
            }
        }
        public delegate void StateChangedHandler(States newState, States oldState);
        public event StateChangedHandler StateChanged;
        private void OnStateChanged(States newState, States oldState)
        {
            if (StateChanged != null)
                StateChanged.Invoke(newState, oldState);
        }
        #endregion

        #region Server/Port
        private string _server;
        public string Server
        {
            get { return _server; }
            private set { _server = value; }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            private set { _port = value; }
        } 
        #endregion

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
            private set { _userID = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private EventList<Contact> _contacts;
        public EventList<Contact> Contacts
        {
            get { return _contacts; }
        }

        private EventList<OutstandingContactRequest> _outstandingContactRequests;
        public EventList<OutstandingContactRequest> OutstandingContactRequests
        {
            get { return _outstandingContactRequests; }
        }
        
        private bool _run = true;
        private Thread _tcpClientThread;
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private SyncQueue<IClientPacket> _sendPacketQueue;
        private SyncQueue<IServerPacket> _receivePacketQueue;

        private Dictionary<Type, List<IPacketHandler>> _incomingPacketHandler;
        private Dictionary<uint, List<IPacketHandler>> _responsePacketHandler;

        public ClientConnection(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void StartConnect(string server, int port)
        {
            if (_state != States.Disconnected)
                throw new ConnectException("Can't start connect if state isn't disconnected, state was " + _state.ToString());

            State = States.Connecting;
            _server = server;
            _port = port;

            _sendPacketQueue = new SyncQueue<IClientPacket>();
            _receivePacketQueue = new SyncQueue<IServerPacket>();

            _incomingPacketHandler = new Dictionary<Type, List<IPacketHandler>>();
            _responsePacketHandler = new Dictionary<uint, List<IPacketHandler>>();

            _contacts = new EventList<Contact>();

            _outstandingContactRequests = new EventList<OutstandingContactRequest>();

            _tcpClient = new TcpClient();

            _tcpClientThread = new Thread(new ThreadStart(Run));
            _tcpClientThread.Name = "Network Thread";
            //_tcpClientThread.IsBackground = true;
            _tcpClientThread.Start();
        }

        private int _serverKeepAlive = 0;

        private void Run()
        {
            try
            {
                _tcpClient.Connect(_server, _port);
            }
            catch
            {
                State = States.Disconnected;
                return;
            }
            try
            {

            _stream = _tcpClient.GetStream();

            uint uid = SendPacket(new Packets.HandshakePacket(_username.ToLower(), HashGenerator.GetMD5Hash(_password))).UID;
            Packets.HandshakeResponsePacket handshakeResponse = (Packets.HandshakeResponsePacket)WaitForResponse<IServerResponsePacket>(uid);

            Console.WriteLine("Got handshake back :P");

            if(handshakeResponse.Success)
                Console.WriteLine("authenticated!");
            else
            {
                Console.WriteLine("authentication failed :(");
                _run = false;
                State = States.Disconnected;
                return;
            }

            this.State = States.Connected;

            EnqueuePacket(new Packets.GetAccountInformationPacket());

            byte keepAliveCounter = 0;
                while (_run)
                {
                    _serverKeepAlive++;
                    if (keepAliveCounter++ >= 50)
                    {
                        keepAliveCounter = 0;
                        EnqueuePacket(new Packets.KeepAlivePacket());
                    }
                    if (_serverKeepAlive >= 100)
                    {
                        throw new ConnectionLostException("The server is not responding");
                    }

                    while (_stream.DataAvailable)
                    {
                        Stopwatch sw;
                        IServerPacket received;
                        using (sw = new Stopwatch())
                        {
                            received = ReceivePacket();
                        }
                        System.Diagnostics.Debug.WriteLine(string.Format("{0}ms for receiving {1}", sw.ElapsedMilliseconds, received.ToString()));
                    }

                    while (_sendPacketQueue.Count > 0)
                    {
                        IClientPacket outgoing = SendPacket(_sendPacketQueue.Dequeue());

                        ProcessOutgoingPacket(outgoing);
                    }

                    while (_receivePacketQueue.Count > 0)
                    {
                        IServerPacket incoming = _receivePacketQueue.Dequeue();

                        ProcessIncomingPacket(incoming);

                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            finally
            {
                _run = false;
                State = States.Disconnected;
            }
        }

        private void ProcessOutgoingPacket(IClientPacket outgoing)
        {
            switch(outgoing.Type)
            {
                case KnownPackets.GetAccountInformation:
                    Packets.GetAccountInformationResponsePacket response = WaitForResponseOrEnqueue<Packets.GetAccountInformationResponsePacket>(outgoing.UID);
                    _username = response.Username;
                    _userID = response.UserID;
                    Console.WriteLine("User: " + _username);
                    break;
                case KnownPackets.Exit:
                    _run = false;
                    break;
            }
        }

        private void ProcessIncomingPacket(IServerPacket incoming)
        {
            _serverKeepAlive = 0;
            switch (incoming.Type)
            {
                case KnownPackets.KeepAlive:
                    _serverKeepAlive = 0;
                    break;
                case KnownPackets.ContactList:
                    {
                        Packets.ContactListPacket clp = (Packets.ContactListPacket)incoming;
                        _contacts.Clear();
                        //OnContactListReload();
                        foreach (Packets.ContactListPacket.ContactInformation ci in clp.ContactInformations)
                        {
                            Contact new_c = new Contact(ci.ID, ci.Name);
                            if (this._outstandingContactRequests.Count(oc => oc.UserID == new_c.ID) > 0)
                                this._outstandingContactRequests.Remove(this.OutstandingContactRequests.Single(oc => oc.UserID == new_c.ID));
                            this._contacts.Add(new_c);
                            //OnContactAdded(new_c);
                            Console.WriteLine("Contact: {0} [{1}]", ci.Name, ci.ID);
                        }
                        break;
                    }
                case KnownPackets.OnlineStatusChanged:
                    {
                        Packets.OnlineStatusChangedPacket oscp = (Packets.OnlineStatusChangedPacket)incoming;
                        GetContact(oscp.ClientID).Online = oscp.Online;
                        Console.WriteLine(GetContact(oscp.ClientID).Name + " is now " + (oscp.Online ? "online" : "offline"));
                        break;
                    }
                case KnownPackets.SendMessage:
                    {
                        Packets.SendMessagePacket smp = (Packets.SendMessagePacket)incoming;
                        Contact c;
                        if (smp.Sender == _userID)
                            c = GetContact(smp.Receiver);
                        else
                            c = GetContact(smp.Sender);
                        IChatMessage cm = CreateChatMessage(smp, c, smp.Sender == _userID ? Directions.Outgoing : Directions.Incoming);
                        c.History.Add(cm);
                        OnMessageReceived(c, cm);
                        break;
                    }
                case KnownPackets.FileTransferRequest:
                    {
                        Packets.FileTransfer.FileTransferRequestPacket ftrp = (Packets.FileTransfer.FileTransferRequestPacket)incoming;
                        Contact c = GetContact(ftrp.Contact);
                        OnIncomingFile(c, ftrp.FileName, ftrp.FileTransferID);
                        break;
                    }
                case KnownPackets.SearchUserResponse:
                    {
                        Packets.SearchUserResponsePacket surp = (Packets.SearchUserResponsePacket)incoming;
                        if (!_searchUserRequests.ContainsKey(surp.ResponseTo))
                            throw new UnexpectedPacketException(string.Format("Got SearchUserResponsePacket (UID: {0}), but no SearchUser request was pending.", surp.UID));
                        else
                        {
                            _searchUserRequests[surp.ResponseTo].Invoke(surp.Users);
                            _searchUserRequests.Remove(surp.ResponseTo);
                        }
                        break;
                    }
                case KnownPackets.OutstandingContactRequest:
                    {
                        Packets.OutstandingContactRequestPacket ocrp = (Packets.OutstandingContactRequestPacket)incoming;
                        if (_outstandingContactRequests.Count(ocr => ocr.UserID == ocrp.UserID) == 0)
                            _outstandingContactRequests.Add(new OutstandingContactRequest(ocrp.UserID, ocrp.Username));
                        break;
                    }
                case KnownPackets.IncomingContactRequest:
                    {
                        Packets.IncomingContactRequestPacket icrp = (Packets.IncomingContactRequestPacket)incoming;
                        OnIncomingContactRequest(icrp.UserID, icrp.Username);
                        break;
                    }
                case KnownPackets.Option:
                    {
                        Packets.OptionPacket optionPacket = (Packets.OptionPacket)incoming;
                        OnIncomingOptions(optionPacket.Version, optionPacket.Data);
                        break;
                    }
                case KnownPackets.Exit:
                    _run = false;
                    break;
            }

            Type type = incoming.GetType();
            if (_incomingPacketHandler.ContainsKey(type))
            {
                foreach (var handler in _incomingPacketHandler[type])
                    handler.Invoke(this, incoming);
            }

            if (incoming.IsResponse && _responsePacketHandler.ContainsKey(incoming.UID))
            {
                foreach (var handler in _responsePacketHandler[incoming.UID])
                    handler.Invoke(this, incoming);
            }
        }

        private IChatMessage CreateChatMessage(Packets.SendMessagePacket smp, Contact c, Directions direction)
        {
            switch (smp.MessageType)
            {
                case Packets.SendMessagePacket.MessageTypes.Text:
                    return new TextChatMessage(smp.Message, direction == Directions.Incoming ? c.Name : Username, c, direction, smp.SendTime);
                case Packets.SendMessagePacket.MessageTypes.Image:
                    return new ImageChatMessage(smp.ImageBuffer, direction == Directions.Incoming ? c.Name : Username, c, direction, smp.SendTime);
                default:
                    return null;
            }
        }

        private IClientPacket SendPacket(IClientPacket packet)
        {
            try
            {
                packet.Send(_stream);
                _stream.Flush();
            }
            catch(System.IO.IOException iox)
            {
                _run = false;
            }
            return packet;
        }

        public void EnqueuePacket(IClientPacket packet)
        {
            if(_state != States.Connected && _state != States.Disconnecting)
                throw new SendPacketException("Not connected");
            _sendPacketQueue.Enqueue(packet);
        }

        private T WaitForResponseOrEnqueue<T>(uint uid) where T : IServerResponsePacket
        {
            if(_state == States.Disconnected)
                throw new GetResponseException("Not connected");

            while(!_stream.DataAvailable)
                Thread.Sleep(3);

            IServerPacket p = PacketHandler.ReadPacket<IServerPacket>(_stream);
            while(!p.IsResponse || p.UID != uid)
            {
                _receivePacketQueue.Enqueue(p);

                while(!_stream.DataAvailable)
                    Thread.Sleep(3);

                p = PacketHandler.ReadPacket<IServerPacket>(_stream);
            }
            
            if(!typeof(T).IsAssignableFrom(p.GetType()))
                throw new UnexpectedPacketException("Got response but expected " + typeof(T).ToString() + " and get " + p.ToString());
            return (T)p;
            
        }

        /// <param name="timeout">in seconds</param>
        private T WaitForResponse<T>(uint uid, int timeout = 120) where T : IServerResponsePacket
        {
            timeout /= 3;
            var timeoutCounter = 0;
            while (!_stream.DataAvailable && timeoutCounter < timeout)
            {
                Thread.Sleep(3);
                timeoutCounter++;
            }
            IServerPacket p = PacketHandler.ReadPacket<IServerPacket>(_stream);
            if(!typeof(T).IsAssignableFrom(p.GetType()))
                throw new UnexpectedPacketException("Expect " + typeof(T).ToString() + " but get " + p.ToString());
            if(p.UID != uid)
                throw new UnexpectedPacketException("Expect UID " + uid + " but get " + p.UID);
            return (T)p;
        }

        private IServerPacket ReceivePacket()
        {
            IServerPacket p = PacketHandler.ReadPacket<IServerPacket>(_stream);
            _receivePacketQueue.Enqueue(p);
            return p;
        }

        public void Disconnect()
        {
            this.State = States.Disconnecting;
            EnqueuePacket(new Packets.ExitPacket("disconnect"));
        }

        public void SendMessage(Contact receiver, string m)
        {
            Packets.SendMessagePacket smp = new Packets.SendMessagePacket(receiver.ID, _userID, m);
            EnqueuePacket(smp);
            receiver.History.Add(CreateChatMessage(smp, receiver, Directions.Outgoing));
        }

        public void SendMessage(Contact receiver, byte[] imageBuffer)
        {
            Packets.SendMessagePacket smp = new Packets.SendMessagePacket(receiver.ID, _userID, imageBuffer);
            EnqueuePacket(smp);
            receiver.History.Add(CreateChatMessage(smp, receiver, Directions.Outgoing));
        }
        
        public Contact GetContact(uint id)
        {
            return _contacts.Where(c => c.ID == id).First();
        }

        // ======= INCOMING MESSAGES ======

        private void OnMessageReceived(Contact c, IChatMessage cm)
        {
            if (MessageReceived != null)
                MessageReceived(c, cm);
            c.OnMessageReceived(cm);
        }
        public event Contact.MessageReceivedHandler MessageReceived;

        // ======= INCOMING FILE ========

        private void OnIncomingFile(Contact c, string filename, uint fileTransferID)
        {
            if (IncomingFile != null)
                IncomingFile(c, filename, fileTransferID);
            c.OnIncomingFile(filename, fileTransferID);
        }
        public event Contact.IncomingFileHandler IncomingFile;

        // ======= CONTACT REQUESTS ========

        public bool SendContactRequest(uint id)
        {
            if(_contacts.Count(c => c.ID == id) > 0)
                return false;

            EnqueuePacket(new Packets.SendContactRequestPacket(id));
            return true;
        }

        public delegate void SearchCompleteDelegate(Dictionary<uint, string> result);
        private Dictionary<uint, SearchCompleteDelegate> _searchUserRequests = new Dictionary<uint, SearchCompleteDelegate>();
        public void StartSearchContact(string username, SearchCompleteDelegate searchComplete)
        {
            Packets.SearchUserPacket sup = new Packets.SearchUserPacket(username);
            _searchUserRequests.Add(sup.UID, searchComplete);
            EnqueuePacket(sup);
        }

        public void AcceptContact(uint id)
        {
            EnqueuePacket(new Packets.AcceptContactRequestPacket(id, true));
        }

        public void DeclineContact(uint id)
        {
            EnqueuePacket(new Packets.AcceptContactRequestPacket(id, false));
        }

        private void OnIncomingContactRequest(uint id, string username)
        {
            if(IncomingContactRequest != null)
                IncomingContactRequest.Invoke(id, username);
        }

        public delegate void IncomingContactRequestHandler(uint id, string username);
        public event IncomingContactRequestHandler IncomingContactRequest;

        // ======= OPTIONS ========

        public void SendOptions(Options options)
        {
            EnqueuePacket(new Packets.OptionPacket(Options.Version, options.getData()));
        }

        private void OnIncomingOptions(int version, byte[] data)
        {
            if (IncomingOptions != null)
                IncomingOptions.Invoke(this, version, data);
        }

        public delegate void IncomingOptionsHandler(ClientConnection sender, int version, byte[] data);
        public event IncomingOptionsHandler IncomingOptions;

        // ======= EXCEPTIONS ========

        private void OnException(Exception x)
        {
            if (Exception != null)
                Exception(this, x);
        }

        public delegate void ExceptionHandler(ClientConnection c, Exception x);
        public event ExceptionHandler Exception;

        // ======= ICONNECTION ========

        public void RegisterIncomingPacketHandler<T>(IncomingPacketHandler<T> handler) where T:IServerPacket
        {
            Type packetType = typeof(T);
            if(!_incomingPacketHandler.ContainsKey(packetType))
                _incomingPacketHandler.Add(packetType, new List<IPacketHandler>());
            _incomingPacketHandler[packetType].Add(handler);
        }

        public void RegisterForResponsePacket<T>(T packet, ResponsePacketHandler<IServerResponsePacketFor<T>> handler) where T : IClientPacket
        {
            if (!_responsePacketHandler.ContainsKey(packet.UID))
                _responsePacketHandler.Add(packet.UID, new List<IPacketHandler>());
            _responsePacketHandler[packet.UID].Add(handler);
        }
    }
}
