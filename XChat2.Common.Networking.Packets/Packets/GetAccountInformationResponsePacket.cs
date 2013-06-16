using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class GetAccountInformationResponsePacket : BasePacket, IServerResponsePacketFor<Packets.GetAccountInformationPacket>
    {
        private string _username;
        public string Username
        {
            get { return _username; }
        }

        private uint _userID;

        public uint UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        
        
        //========================================================================
        public GetAccountInformationResponsePacket(uint uid, string username, uint userID)
            : base(0x04, uid) 
        {
            _username = username;
            _userID = userID;
        }

        public GetAccountInformationResponsePacket(NetworkStream stream, uint uid)
            : base(0x04, uid)
        {
            _username = StreamHelper.ReadString(stream);
            _userID = StreamHelper.ReadUInt(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteString(stream, _username);
            StreamHelper.WriteUInt(stream, _userID);
        }
    }
}
