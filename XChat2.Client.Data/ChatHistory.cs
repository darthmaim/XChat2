using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Client.Data.ChatMessages;

namespace XChat2.Client.Data
{
    public class ChatHistory : IEnumerable<IChatMessage>
    {
        private List<IChatMessage> _chatMessages = new List<IChatMessage>();

        public ChatHistory(Contact c)
        {
            _contact = c;
        }

        private Contact _contact;
        public Contact Contact
        {
            get { return _contact; }
        }

        public void Add(IChatMessage message)
        {
            _chatMessages.Add(message);
            OnChatMessageAdded(message);
        }

        private void OnChatMessageAdded(IChatMessage added)
        {
            if (ChatMessageAdded != null)
                ChatMessageAdded(this, added);
        }
        public delegate void ChatMessageAddedHandler(ChatHistory sender, IChatMessage added);
        public event ChatMessageAddedHandler ChatMessageAdded;

        public IEnumerator<IChatMessage> GetEnumerator()
        {
            return _chatMessages.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _chatMessages.GetEnumerator();
        }
    }
}
