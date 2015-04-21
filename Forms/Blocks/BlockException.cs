using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Blocks
{
    [Serializable]
    public class BlockException : Exception
    {
        public BlockException() { }
        public BlockException(string message) : base(message) { }
        public BlockException(string message, Exception inner) : base(message, inner) { }
        protected BlockException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
