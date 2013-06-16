using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Networking.P2P
{
    public enum KnownP2PPackets : byte
    {
        /// <summary>
        /// Send from the client to the server to authenticate
        /// </summary>
        Handshake = 0x01,

        /// <summary>
        /// Send from the server to the client as response to the handshake, if the authentication was successfull
        /// </summary>
        Authenticated = 0x02,

        /// <summary>
        /// Send from the server to the client as response to the handshake, if the authentication failed
        /// </summary>
        AuthenticationFailed = 0x03,
        
        /// <summary>
        /// After all the holepunches, send this :)
        /// </summary>
        DataStart = 0x04
    }
}
