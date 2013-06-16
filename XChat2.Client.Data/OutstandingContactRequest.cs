using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Client.Data
{
    public class OutstandingContactRequest
    {
        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
            internal set { _userID = value; }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            internal set { _username = value; }
        }

        public OutstandingContactRequest(uint userID, string username)
        {
            _userID = userID;
            _username = username;
        }
    }
}
