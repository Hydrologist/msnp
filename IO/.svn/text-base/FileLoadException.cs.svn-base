using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace NetworkGUI.IO
{
    [Serializable]
    class FileLoadException :  IOException
    {
        public FileLoadException() : base() { }
        public FileLoadException(string message) : base(message) { }
        public FileLoadException(string message, Exception inner) : base(message, inner) { }
        protected FileLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
