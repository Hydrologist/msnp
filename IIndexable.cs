using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public interface IIndexable<T>
    {
        T this[int i]
        {
            get;
            set;
        }

        int Length
        {
            get;
        }
    }
}
