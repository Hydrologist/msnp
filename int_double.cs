using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    class int_double : IComparable
    {
        public int x;
        public double value;

        public int CompareTo(object rhs)
        {
            return (int)Math.Ceiling(value - ((int_double)rhs).value);
        }

        public int_double(int X,double Value)
        {
            x=X;
            value=Value;
        }
    }
}
