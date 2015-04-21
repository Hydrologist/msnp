using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.Windows.Forms;

namespace Network.Communities
{
    public class Comm : IEnumerable<int>, IComparable
    {
        private List<int> _members;
        private byte[] _memberSet;
        private double _cohesion;

        

        public double Cohesion
        {
            get { return _cohesion; }
            set { _cohesion = value; }
        }

        public Comm(List<int> members, int n)
        {
            _members = new List<int>(members); // list of indexes of members in the group
            _memberSet = new byte[n];

            Array.Clear(_memberSet, 0, _memberSet.Length);

            for (int i = 0; i < members.Count; ++i)
                _memberSet[members[i]] = 1;

        }

        public int Size
        {
            get { return _members.Count; }
        }

        public int Count
        {
            get { return _members.Count; }
        }

        public int MemberRangeSize
        {
            get { return _memberSet.Length; }
        }

        public List<int> Members
        {
            get { return _members; }
        }

        public byte[] MemberSet
        {
            get { return _memberSet; }
        }

        public bool Contains(int member)
        {
            //  if (member < 0 || member >= _memberSet.Length)
            //    return false;

            return _memberSet[member] != 0;
        }

        public int IntContains(int member)
        {
            return _memberSet[member];
        }

        public IEnumerator GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            foreach (int i in _members)
                yield return i;
        }

        public int CompareTo(object obj)
        {
            Comm c = obj as Comm;
            if (c == null)
                throw new CommException("Cannot compare to null Block or non-Block object.");

            if (Size != c.Size)
                return -1 * Size.CompareTo(c.Size);

            for (int i = 0; i < _memberSet.Length; ++i)
                if (_memberSet[i] - c._memberSet[i] != 0)
                    return _memberSet[i] - c._memberSet[i];

            return 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in _members)
            {
                sb.Append(i);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public int this[int i]
        {
            get
            {
                return _members[i];
            }
        }

        public void ComputeCohesion(Matrix SESE)
        {
            _cohesion = 0.0;
            for (int i = 0; i < Size; ++i)
                for (int j = i + 1; j < Size; ++j)
                    _cohesion += SESE[_members[i], _members[j]];

            if (Size == 1)
                _cohesion = 1.0;
            else
            {
                _cohesion = (2.0 * _cohesion) / (Size * (Size - 1));
            }
        }

        public double ComputeWsum(List<double> attribute, bool file)
        {
            double wsum = 0.0;
            if (file)
            {
                for (int i = 0; i < _memberSet.Length; ++i)
                    if (this.Contains(i))
                        wsum += attribute[i];
            }
            else
            {
                for (int i = 0; i < _memberSet.Length; ++i)
                    if (this.Contains(i))
                        wsum += 1.0 / _memberSet.Length;
            }
            wsum *= _cohesion;
            return wsum;
        }

    }
}
