using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Networking.Packets.Packets;

namespace XChat2.Client.Data.ChatMessages
{
    public interface IChatMessage
    {
        DateTime Time { get; }

        Contact Contact { get; }

        Directions Direction { get; }

        string Username { get; }
    }

    public enum Directions { Incoming, Outgoing }
}
