using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Exceptions;
using XChat2.Common.Hashes;
using Packets = XChat2.Common.Networking.Packets.Packets;
using XChat2.Common.Collections;
using XChat2.Common.Configuration;
using System.Runtime.CompilerServices;
using System.IO;
using XChat2.Server.Database.Clients;

[assembly: InternalsVisibleTo("XChat2.Tests.Server.Clients")]

namespace XChat2.Server.Clients
{
    public class Client
    {
        private static Dictionary<uint, Client> _clients;
        
        private static IClientDataSource _dataSource;
        private static Config _cfg;
        private static OnlineStatusChangedHandler _onlineStatusChanged;

        public static void Init(Config cfg, IClientDataSource clientDataSource, OnlineStatusChangedHandler onlineStatusChanged)
        {
            _cfg = cfg;
            _dataSource = clientDataSource;
            _clients = new Dictionary<uint, Client>();
            _onlineStatusChanged = onlineStatusChanged;
        }

        public static Client GetClient(string username)
        {
            username = username.ToLower();
            if(!ClientExists(username))
                throw new UnknownUserException("Unknown user '" + username + "'");
            if (_clients.Count(c => c.Value.Username.ToLower() == username) == 0)
                LoadClient(username);
            return _clients.Single((c) => c.Value.Username.ToLower() == username).Value;
        }

        public static Client GetClient(uint id)
        {
            if(!ClientExists(id))
                throw new UnknownUserException("Unknown user '" + id + "'");
            if (!_clients.ContainsKey(id))
                LoadClient(id);
            return _clients[id];
        }

        public static bool ClientExists(string username)
        {
            username = username.ToLower();
            if (_clients.Count(c => c.Value.Username.ToLower() == username) == 1)
                return true;
            else
            {
                using (ClientRepository cr = new ClientRepository(_dataSource))
                {
                    return cr.Exists(username);
                }
            }
        }

        public static bool ClientExists(uint id)
        {
            if (_clients.ContainsKey(id))
                return true;
            else
            {
                using (ClientRepository cr = new ClientRepository(_dataSource))
                {
                    return cr.Exists(id);
                }
            }
        }

        public static Dictionary<uint, string> SearchClients(string username)
        {
            Dictionary<uint, string> result = new Dictionary<uint,string>();
            foreach(Client c in _clients.Values)
            {
                if(c.Username.ToLower().Contains(username.ToLower()))
                    result.Add(c.ID, c.Username);
            }
            return result;
        }

        public static void SaveAll()
        {
            foreach (Client c in _clients.Values)
                c.Save();
        }

        internal static Client LoadClient(uint id) 
        {
            System.Diagnostics.Debug.WriteLine("[Client] Load client id=" + id);

            using (ClientRepository cr = new ClientRepository(_dataSource))
            {
                var clientInfo = cr.GetByID(id);
                Client c = new Client(clientInfo.UserID, clientInfo.Username, clientInfo.Password, clientInfo.Email);
                _clients.Add(c.ID, c);

                foreach (uint contactID in cr.GetContacts(c.ID).ToList())
                {
                    c._contacts.Add(GetClient(contactID));
                }

                c.OnlineStatusChanged += _onlineStatusChanged;

                return c;
            }
        }

        internal static Client LoadClient(string username)
        {
            System.Diagnostics.Debug.WriteLine("[Client] Load client username=" + username);

            using (ClientRepository cr = new ClientRepository(_dataSource))
            {
                var clientInfo = cr.GetByUsername(username);
                Client c = new Client(clientInfo.UserID, clientInfo.Username, clientInfo.Password, clientInfo.Email);
                _clients.Add(c.ID, c);

                foreach (uint contactID in cr.GetContacts(c.ID).ToList())
                {
                    c._contacts.Add(GetClient(contactID));
                }

                c.OnlineStatusChanged += _onlineStatusChanged;

                return c;
            }
        }

        public static IEnumerable<Client> Clients
        {
            get { return _clients.Values.AsEnumerable<Client>(); }
        }

        private List<Client> _contacts;
        public Client[] Contacts
        {
            get
            {
                return _contacts.ToArray();
            }
        }

        public Client AddContact(Client contact)
        {
            if(!_contacts.Contains(contact))
            {
                _contacts.Add(contact);
                if (!contact.Contacts.Contains(this))
                {
                    contact.AddContact(this);

                    using (ClientRepository cr = new ClientRepository(_dataSource))
                    {
                        cr.AddContact(this.ID, contact.ID);
                    }
                }
            }
            return this;
        }

        private SyncQueue<Packets.SendMessagePacket> _messageQueue;
        public void EnqueueMessagePacket(Packets.SendMessagePacket smp)
        {
            if(Online)
            {
                foreach(Connection c in _connections)
                {
                    c.EnqueuePacket(smp);
                }
            }
            else
            {
                _messageQueue.Enqueue(smp);
            }
        }
        public bool MessageQueueContainsPackets
        {
            get { return _messageQueue.Count > 0; }
        }

        internal Client(uint id, string username, string pwMD5, string email)
        {
            _username = username;
            _id = id;

            _contacts = new List<Client>();
            _messageQueue = new SyncQueue<Packets.SendMessagePacket>();
            _connections = new List<Connection>();

            if(!string.IsNullOrEmpty(pwMD5))
                _passwordHash = HashGenerator.HexStringToByteArray(pwMD5);

            _optionPath = Path.Combine(_cfg["clients"].get<string>("UserOptionPath").Value, ID + ".xchatOptions");
            if (Options.Exists(_optionPath))
                _options = Options.LoadFromFile(_optionPath);
            else
                _options = Options.CreateNew();
        }

        private Client(string username, string password, uint id) : this(id, username, "", "")
        {
            _passwordHash = HashGenerator.GetMD5Hash(password);
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Username, this.ID);    
        }

        private string _optionPath;
        private Options _options;
        internal Options Options
        {
            get { return _options; }
            set { _options = value; }
        }

        private uint _id;
        public uint ID
        {
            get { return _id; }
            set { _id = value; }
        }
        

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private byte[] _passwordHash;
        public byte[] PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }

        /// <summary>
        /// A list of all outstanding contact requests
        /// </summary>
        List<uint> _outstandingContactRequests = new List<uint>();

        /// <summary>
        /// Add an outstanding contact request
        /// </summary>
        /// <param name="id">the userID of the invited user</param>
        public void AddOutstandingContactRequest(uint id)
        {
            if (_outstandingContactRequests.Contains(id))
                return;
            _outstandingContactRequests.Add(id);
        }

        /// <summary>
        /// Removes an outstanding contact request (request was accepted or declined)
        /// </summary>
        /// <param name="id">the id of the user of the outstanding contact request</param>
        public void RemoveOutstandingContactRequest(uint id)
        {
            if (!_outstandingContactRequests.Contains(id))
                return;
            _outstandingContactRequests.Remove(id);
        }

        /// <summary>
        /// A list of all Contact yet to answer contact requests
        /// </summary>
        List<uint> _yetToAnswerContactRequests = new List<uint>();

        /// <summary>
        /// Add a yet to be answered contact request from another user
        /// </summary>
        /// <param name="id">the user who send the contact request</param>
        public void AddContactRequest(uint id)
        {
            if(_yetToAnswerContactRequests.Contains(id)) return;
            _yetToAnswerContactRequests.Add(id);
            if(this.Online)
            {
                _connections.ForEach(c => { c.EnqueuePacket(new Packets.IncomingContactRequestPacket(id, GetClient(id).Username)); });
            }
        }

        public void AcceptContactRequest(uint id)
        {
            if(!_yetToAnswerContactRequests.Contains(id)) 
                return;
            _yetToAnswerContactRequests.Remove(id);
            this.AddContact(GetClient(id));
            _connections.ForEach(c => { c.SendContactlist(); });
            GetClient(id)._connections.ForEach(c => { c.SendContactlist(); });
        }

        public void DeclineContactRequest(uint id)
        {
            if (!_yetToAnswerContactRequests.Contains(id))
                return;
            _yetToAnswerContactRequests.Remove(id);
        }

        private List<Connection> _connections;
        public void AddConnection(Connection connection)
        {
            _connections.Add(connection);
            connection.OptionsUpdated += OptionsUpdated;

            SendOutstandingContactRequests(connection);

            if(_connections.Count == 1)
                OnOnlineStatusChanged(true);
        }

        public void RemoveConnection(Connection connection)
        {
            _connections.Remove(connection);
            connection.OptionsUpdated -= OptionsUpdated;

            if(_connections.Count == 0)
                OnOnlineStatusChanged(false);
        }

        void OptionsUpdated(Connection sender)
        {
            _options.SaveToFile(_optionPath);
            _connections.ForEach(c => { c.SendOptions(); });
        }

        /// <summary>
        /// Saves the client to the db
        /// </summary>
        public void Save()
        {
        
        }
        
        public bool Online
        {
            get { return _connections.Count > 0; }
        }

        private void OnOnlineStatusChanged(bool online)
        {
            foreach(Client c in _contacts)
            {
                if(c.Online)
                    c.SendOnlineStatusChanged(this, online);
            }

            if (Online)
            {
                while (MessageQueueContainsPackets)
                {
                    Packets.SendMessagePacket smp = _messageQueue.Dequeue();
                    foreach (Connection c in _connections)
                        c.EnqueuePacket(smp);
                }
                foreach (uint cr in _yetToAnswerContactRequests)
                    _connections.ForEach(c => { c.EnqueuePacket(new Packets.IncomingContactRequestPacket(cr, GetClient(cr).Username)); });
            }

            if(OnlineStatusChanged != null)
                OnlineStatusChanged.Invoke(this, online);
        }

        private void SendOnlineStatusChanged(Client client, bool online)
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                Connection c = _connections[i];
                if (c.Connected)
                    c.EnqueuePacket(new Packets.OnlineStatusChangedPacket(client.ID, online));
            }
        }

        public delegate void OnlineStatusChangedHandler(Client client, bool online);
        public event OnlineStatusChangedHandler OnlineStatusChanged;

        internal void SendMessage(Connection connection, Packets.SendMessagePacket smp)
        {
            foreach (Connection c in _connections)
            {
                if (c != connection && c.Connected)
                {
                    c.EnqueuePacket(smp);
                }
            }
            Client receiver = GetClient(smp.Receiver);
            receiver.EnqueueMessagePacket(smp);
        }

        /// <summary>
        /// Sends all outstanding contact requests to all connections
        /// </summary>
        internal void SendOutstandingContactRequests()
        {
            _connections.ForEach(c => SendOutstandingContactRequests(c));
        }

        /// <summary>
        /// Sends all outstanding contact requests to the specified connection
        /// </summary>
        /// <param name="connection">The connection to send the outstanding contact requests to</param>
        internal void SendOutstandingContactRequests(Connection connection)
        {
            foreach (uint uid in _outstandingContactRequests)
            {
                Client outstandingContact = Client.GetClient(uid);
                connection.EnqueuePacket(new Packets.OutstandingContactRequestPacket(outstandingContact.ID, outstandingContact.Username));
            }
        }


    }
}
