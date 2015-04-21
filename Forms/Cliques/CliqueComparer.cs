using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Cliques
{
    class CliqueComparer : IComparer<Clique>
    {
        public bool Equals(Clique x, Clique y)
        {
            return Compare(x, y) == 0;
        }

        public int Compare(Clique x, Clique y)
        {
            int ret = 0;
            for (int i = 0; i < x.Size - 1; ++i)
                ret += y[i] - x[i];
            return ret;
        }

        public int GetHashCode(int[] obj)
        {
            return obj.GetHashCode();
        }
    }
}
