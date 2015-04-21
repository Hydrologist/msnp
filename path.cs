using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class path : IComparable
    {
        public double cost;
        public int prevNode;
        public int curNode;

        public int CompareTo(object rhs)
        {
            return (int)((cost - ((path)rhs).cost)*10000);
        }

        public path(double Cost, int Prev, int CUR)
        {
            cost = Cost;
            prevNode = Prev;
            curNode = CUR;
        }
    }
}
