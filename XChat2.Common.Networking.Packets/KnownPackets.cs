using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Networking.Packets
{
    public enum KnownPackets : byte
    {
        KeepAlive = 0x00,
        //Default packets sent on initial connection
        Handshake                     = 0x01,
        HandshakeResponse             = 0x02,
        GetAccountInformation         = 0x03,
        GetAccountInformationResponse = 0x04,
        Option                        = 0x05,

        Exit                          = 0x0F,

        //contact information packets
        ContactList                   = 0x10,
        OnlineStatusChanged           = 0x11,

        //Chatting packets
        SendMessage                   = 0x20,

        //Adding contact packets
        SearchUser                    = 0x30,
        SearchUserResponse            = 0x31,
        SendContactRequest            = 0x32,
        IncomingContactRequest        = 0x33,
        AcceptContactRequest          = 0x34,
        OutstandingContactRequest     = 0x35,

        //FileTransfer packets
        FileTransferRequest           = 0x40,
        FileTransferResponse          = 0x41,
        FileTransferInfo              = 0x42,
        FileTransferStatus            = 0x43,
        FileTransferStart             = 0x44,
    }
}
