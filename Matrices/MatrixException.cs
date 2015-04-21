using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Network.Matrices
{
    [Serializable]
    public class MatrixException : Exception
    {
        public MatrixException() { }
        public MatrixException(string message) : base(message) { }
        public MatrixException(string message, Exception inner) : base(message, inner) { }

        protected MatrixException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
