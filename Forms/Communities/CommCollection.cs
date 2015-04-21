using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Network.Communities
{
    public sealed class CommCollection : IEnumerable<Comm>
    {
        public List<Comm> _communities;

        public List<Comm> new_communities;

        private Matrix _communitiesOverlap;
        private Matrix _commByCommOverlap;
        private int _minCommSize;
        //public delegate void ParameterizedThreadStart(SymmetricBinaryMatrix obj);
        public CommCollection(SymmetricBinaryMatrix m, int bmin, List<Comm> Comms)
        {
            new_communities = new List<Comm>();
            new_communities = Comms;

            _communities = new List<Comm>();
            _communities = Comms;
            _communitiesOverlap = new Matrix(m.Rows);
            _communitiesOverlap.CopyLabelsFrom(m);
            _minCommSize = bmin;

            Vector Z = Vector.Zero(m.Rows);
            //Thread t = new Thread(delegate() { FindCliques(m); });
            // t.Start();
            //  while (!t.IsAlive) ;
            //  Thread.Sleep(100);
            //  t.Join();
   
            LoadCommunitiesOverlap();

            //bool addMore = false;
            //for (int i = 0; i < m.Rows; ++i)
            //    if (_cliqueOverlap[i, i] != 0)
            //    {
            //        m.SetRowVector(i, Z);
            //    }
            //    else
            //        addMore = true;

            //if (addMore)
            //{
            //   
            //    CliqueCollection cc = new CliqueCollection(m, _minCliqueSize);
            //    foreach (Clique c in cc)
            //    {
            //        if (_cliqueOverlap[c[0], c[0]] == 0)
            //            _cliques.Add(c);
            //    }
            //    LoadCliqueOverlap();
            //}

            _communities.Sort();

            //Thread temp = new Thread(LoadCliqueByCliqueOverlap);
            //temp.Start();
            //FindCliques(m);
            //while (!temp.IsAlive) ;
            //Thread.Sleep(100);
            //temp.Join();


            LoadCommByCommOverlap();
        }

        public void LoadCommunitiesOverlap()
        {
            _communitiesOverlap.Clear();
            foreach (Comm comm in _communities)
                foreach (int i in comm)
                    foreach (int j in comm)
                        ++_communitiesOverlap[i, j];
        }



        /* private void LoadCliqueByCliqueOverlap(int n)
         {
             _cliqueByCliqueOverlap = new Matrix(_cliques.Count);
             _cliqueByCliqueOverlap.Clear();
             for (int i = 0; i < _cliques.Count; ++i)
             {
                 for (int k = 0; k < n; ++k){
                 if (_cliques[i].Contains(k)) ++_cliqueByCliqueOverlap[i, i];}
                 for (int j = i+1; j < _cliques.Count; ++j)
                 {
                     for (int k = 0; k < n; ++k)
                     {


                         if (_cliques[i].Contains(k) && _cliques[j].Contains(k))
                         {
                            
                                 ++_cliqueByCliqueOverlap[i, j];
                                 ++_cliqueByCliqueOverlap[j, i];
                             }
                         }
                     }
                 }
             }*/








        public int Count
        {
            get { return _communities.Count; }
        }

        public Matrix CommOverlap
        {
            get { return _communitiesOverlap; }
        }

        public Matrix CommByCommOverlap
        {
            get { return _commByCommOverlap; }
        }

        public int MemberRangeSize
        {
            get
            {
                if (_communities.Count > 0)
                    return _communities[0].MemberRangeSize;
                return -1;
            }
        }

        public double GetCommByCommOverlap(int i, int j)
        {
            if (_communities.Count < Constants.CliqueByCliqueOverlapCutoff)
                return _commByCommOverlap[i, j];
            else if (i == j)
                return _commByCommOverlap[0, i];
            else
                return CalculateCommbyCommOverlap(i, j);
        }

        public Comm this[int i]
        {
            get { return _communities[i]; }
        }
        
        public Comm getNewCommunity(int i)
        {
            return new_communities[i];
        }
        

        public Matrix ToCommAffiliationMatrix()
        {
            Matrix m = new Matrix(MemberRangeSize, Count);

            for (int i = 0; i < m.Rows; ++i)
                for (int j = 0; j < m.Cols; ++j)
                    m[i, j] = _communities[j].Contains(i) ? 1 : 0;

            return m;
        }

        private int CalculateCommbyCommOverlap(int i, int j)
        {
            if (i == j)
                return _communities[i].Size;

            if (_communities[i].Size > _communities[j].Size)
                Algorithms.Swap(ref i, ref j);

            int matches = 0;
            foreach (int k in _communities[i])
                if (_communities[j].Contains(k))
                    ++matches;
            return matches;
        }

        public void LoadCommByCommOverlap()
        {
            // We can only have the matrix be _so_ large, otherwise calculate as needed
            if (_communities.Count < Constants.CliqueByCliqueOverlapCutoff)
            {
                _commByCommOverlap = new Matrix(_communities.Count);
                _commByCommOverlap.Clear();

                Algorithms.Iota(_commByCommOverlap.RowLabels, 1);
                Algorithms.Iota(_commByCommOverlap.ColLabels, 1);

                for (int c1 = 0; c1 < _communities.Count; ++c1)
                    for (int c2 = 0; c2 < _communities.Count; ++c2)
                        _commByCommOverlap[c1, c2] = CalculateCommbyCommOverlap(c1, c2);
            }
            else
            {
                _commByCommOverlap = new Vector(_communities.Count);
                Vector v = _commByCommOverlap as Vector;
                Algorithms.Iota(v.Labels, 1);
                for (int i = 0; i < _communities.Count; ++i)
                    v[i] = _communities[i].Size;
            }
        }
       
        #region IEnumerable<Comm> Members

        public IEnumerator<Comm> GetEnumerator()
        {
            foreach (Comm c in _communities)
                yield return c;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _communities.GetEnumerator();
        }

        #endregion
    }
}
