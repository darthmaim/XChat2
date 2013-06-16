using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database.Clients
{
    public class ClientInformation
    {
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
            set { _userID = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        
    }
}
