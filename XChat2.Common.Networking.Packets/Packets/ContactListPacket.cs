using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class ContactListPacket : BasePacket, IServerPacket
    {
        public struct ContactInformation
        {
            public uint ID { get; set; }
            public string Name { get; set; }
        }

        private ContactInformation[] _contactInformations;

        public ContactInformation[] ContactInformations
        {
            get { return _contactInformations; }
            set { _contactInformations = value; }
        }
        

        //========================================================================
        public ContactListPacket(ContactInformation[] ci)
            : base(0x10) 
        {
            _contactInformations = ci;
        }

        public ContactListPacket(NetworkStream stream, uint uid)
            : base(0x10, uid)
        {
            _contactInformations = new ContactInformation[StreamHelper.ReadInt(stream)];

            for(int i = 0; i< _contactInformations.Length; i++) {
                _contactInformations[i].ID = StreamHelper.ReadUInt(stream);
                _contactInformations[i].Name = StreamHelper.ReadString(stream);
            }
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteInt(stream, _contactInformations.Length);
            foreach(ContactInformation ci in _contactInformations)
            {
                StreamHelper.WriteUInt(stream, ci.ID);
                StreamHelper.WriteString(stream, ci.Name);
            }
        }
    }
}
