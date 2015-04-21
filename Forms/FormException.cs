using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkGUI.Forms
{
    [Serializable]
    public class FormException : Exception
    {
        public FormException() { }
        public FormException(string message) : base(message) { }
        public FormException(string message, Exception inner) : base(message, inner) { }
        protected FormException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
    }
}
