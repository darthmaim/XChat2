using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace XChat2.Common.Networking.P2P.Server
{
    internal class HolePunchClient
    {
        private byte[] _authKey;
        public byte[] AuthenticationKey
        {
            get { return _authKey; }
            set { _authKey = value; }
        }

        public HolePunchClient(byte[] authKey)
        {
            _authKey = authKey;
        }

        private IPEndPoint _remoteEP;
        public IPEndPoint RemoteEP
        {
            get { return _remoteEP; }
            set { _remoteEP = value; }
        }

        private HolePunchClient _partner;
        public HolePunchClient Partner
        {
            get { return _partner; }
            set { _partner = value; }
        }

        private bool _hasAuthenticated;
        public bool HasAuthenticated
        {
            get { return _hasAuthenticated; }
            set { _hasAuthenticated = value; }
        }
        
    }
}
