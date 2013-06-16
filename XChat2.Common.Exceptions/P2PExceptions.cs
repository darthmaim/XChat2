using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Exceptions
{
    [Serializable]
    public class P2PException : Exception
    {
        public P2PException() { }
        public P2PException(string message) : base(message) { }
        public P2PException(string message, Exception inner) : base(message, inner) { }
        protected P2PException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class P2PAuthenticationFailedException : P2PException
    {
        public P2PAuthenticationFailedException() { }
        public P2PAuthenticationFailedException(string message) : base(message) { }
        public P2PAuthenticationFailedException(string message, Exception inner) : base(message, inner) { }
        protected P2PAuthenticationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class P2PUnexpectedPacketException : P2PException
    {
        public P2PUnexpectedPacketException() { }
        public P2PUnexpectedPacketException(string message) : base(message) { }
        public P2PUnexpectedPacketException(string message, Exception inner) : base(message, inner) { }
        protected P2PUnexpectedPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
