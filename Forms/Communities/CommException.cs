using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Communities
{
    [Serializable]
    public class CommException : Exception
    {
        public CommException() { }
        public CommException(string message) : base(message) { }
        public CommException(string message, Exception inner) : base(message, inner) { }
        protected CommException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
