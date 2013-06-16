using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Exceptions
{
    [Serializable]
    public class SendPacketException : Exception
    {
        public SendPacketException() { }
        public SendPacketException(string message) : base(message) { }
        public SendPacketException(string message, Exception inner) : base(message, inner) { }
        protected SendPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class GetResponseException : Exception
    {
        public GetResponseException() { }
        public GetResponseException(string message) : base(message) { }
        public GetResponseException(string message, Exception inner) : base(message, inner) { }
        protected GetResponseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnknownPacketException : Exception
    {
        public UnknownPacketException() { }
        public UnknownPacketException(string message) : base(message) { }
        public UnknownPacketException(string message, Exception inner) : base(message, inner) { }
        protected UnknownPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnexpectedPacketException : Exception
    {
        public UnexpectedPacketException() { }
        public UnexpectedPacketException(string message) : base(message) { }
        public UnexpectedPacketException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnknownUserException : Exception
    {
        public UnknownUserException() { }
        public UnknownUserException(string message) : base(message) { }
        public UnknownUserException(string message, Exception inner) : base(message, inner) { }
        protected UnknownUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class DuplicateClientEntryException : Exception
    {
        public DuplicateClientEntryException() { }
        public DuplicateClientEntryException(string message) : base(message) { }
        public DuplicateClientEntryException(string message, Exception inner) : base(message, inner) { }
        protected DuplicateClientEntryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ConnectException : Exception
    {
        public ConnectException() { }
        public ConnectException(string message) : base(message) { }
        public ConnectException(string message, Exception inner) : base(message, inner) { }
        protected ConnectException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ConnectionLostException : Exception
    {
        public ConnectionLostException() { }
        public ConnectionLostException(string message) : base(message) { }
        public ConnectionLostException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionLostException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
