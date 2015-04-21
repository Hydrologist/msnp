using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Network.Cliques
{
    public sealed class CliqueCollection  : IEnumerable<Clique>
    {
        private List<Clique> _cliques;
        private Matrix _cliqueOverlap;
        private Matrix _cliqueByCliqueOverlap;
        private int _minCliqueSize;
        //public delegate void ParameterizedThreadStart(SymmetricBinaryMatrix obj);
        public CliqueCollection(SymmetricBinaryMatrix m,int cmin)
        {
            _cliques = new List<Clique>();
            _cliqueOverlap = new Matrix(m.Rows);
            _cliqueOverlap.CopyLabelsFrom(m);
           _minCliqueSize = cmin;

           Vector Z = Vector.Zero(m.Rows);
          //Thread t = new Thread(delegate() { FindCliques(m); });
          // t.Start();
          //  while (!t.IsAlive) ;
          //  Thread.Sleep(100);
          //  t.Join();
            FindCliques(m);
            LoadCliqueOverlap();

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

            _cliques.Sort();

            //Thread temp = new Thread(LoadCliqueByCliqueOverlap);
            //temp.Start();
            //FindCliques(m);
            //while (!temp.IsAlive) ;
            //Thread.Sleep(100);
            //temp.Join();

            
            //LoadCliqueByCliqueOverlap();
        }

        public void LoadCliqueOverlap()
        {
            _cliqueOverlap.Clear();
            foreach (Clique clique in _cliques)
                foreach (int i in clique)
                    foreach (int j in clique)
                        ++_cliqueOverlap[i, j];
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
            get { return _cliques.Count; }
        }

        public Matrix CliqueOverlap
        {
            get { return _cliqueOverlap; }
        }

        public Matrix CliqueByCliqueOverlap
        {
            get { return _cliqueByCliqueOverlap; }
        }

        public int MemberRangeSize
        {
            get
            {
                if (_cliques.Count > 0)
                    return _cliques[0].MemberRangeSize;
                return -1;
            }
        }

        public double GetCliqueByCliqueOverlap(int i, int j)
        {
            if (_cliques.Count < Constants.CliqueByCliqueOverlapCutoff)
                return _cliqueByCliqueOverlap[i, j];
            else if (i == j)
                return _cliqueByCliqueOverlap[0, i];
            else
                return CalculateCliqueByCliqueOverlap(i, j);
        }

        public Clique this[int i]
        {
            get { return _cliques[i]; }
        }

        public Matrix ToCliqueAffiliationMatrix()
        {
            Matrix m = new Matrix(MemberRangeSize, Count);

            for (int i = 0; i < m.Rows; ++i)
                for (int j = 0; j < m.Cols; ++j)
                    m[i, j] = _cliques[j].Contains(i) ? 1 : 0;

            return m;
        }

        private int CalculateCliqueByCliqueOverlap(int i, int j)
        {
            if (i == j)
                return _cliques[i].Size;

            if (_cliques[i].Size > _cliques[j].Size)
                Algorithms.Swap(ref i, ref j);

            int matches = 0;
            foreach (int k in _cliques[i])
                if (_cliques[j].Contains(k))
                    ++matches;
            return matches;
        }

        public void LoadCliqueByCliqueOverlap()
        {
            // We can only have the matrix be _so_ large, otherwise calculate as needed
            if (_cliques.Count < Constants.CliqueByCliqueOverlapCutoff)
            {
                _cliqueByCliqueOverlap = new Matrix(_cliques.Count);
                _cliqueByCliqueOverlap.Clear();

                Algorithms.Iota(_cliqueByCliqueOverlap.RowLabels, 1);
                Algorithms.Iota(_cliqueByCliqueOverlap.ColLabels, 1);

               for (int c1 = 0; c1 < _cliques.Count; ++c1)
                    for (int c2 = 0; c2 < _cliques.Count; ++c2)
                        _cliqueByCliqueOverlap[c1, c2] = CalculateCliqueByCliqueOverlap(c1, c2);
            }
            else
            {
                _cliqueByCliqueOverlap = new Vector(_cliques.Count);
                Vector v = _cliqueByCliqueOverlap as Vector;
                Algorithms.Iota(v.Labels, 1);
                for (int i = 0; i < _cliques.Count; ++i)    
                    v[i] = _cliques[i].Size;
            }
        }

        private void FindCliques(SymmetricBinaryMatrix m)
        {
            Vector v = new Vector(m.Rows);
            for (int i = 0; i < m.Rows; i++)
            {
                v[i] = m[i, i];
                m[i, i] = 1;
            }

            List<int> Compsub = new List<int>();
            List<int> All = new List<int>();

            for (int c = 0; c < m.Rows; c++)
                All.Add(c);

            BkExtend(All, 0, m.Rows, Compsub, m);

            for (int i = 0; i < m.Rows; i++)
                m[i, i] = v[i];

        }

        private void BkExtend(List<int> old, int ne, int ce, List<int> Compsub, SymmetricBinaryMatrix MBinary)
        {
            int nod = 0, fixp = -1;
            int newne, newce, i, j, count, pos = 0, p, s = -1, sel, minnod = ce;
            List<int> new_ = new List<int>();
            for (i = 0; i < ce; i++)
                new_.Add(0);

            // Determine each counter value and look for minimum
            for (i = 0; i < ce && minnod != 0; i++)
            {
                p = old[i];
                count = 0;

                // Count disconnections
                for (j = ne; j < ce && count < minnod; j++)
                {
                    if (!MBinary.GetValue(p, old[j]))
                    {
                        ++count;

                        // Save position of potential candidate
                        pos = j;
                    }
                }

                // Test new minimum
                if (count < minnod)
                {
                    fixp = p;
                    minnod = count;
                    if (i < ne)
                    {
                        s = pos;
                    }
                    else
                    {
                        s = i;
                        nod = 1;
                    }
                }
            }

            // If fixed point initially chosen from candidates then
            // number of disconnections will be preincreased by one

            // Backtrackcycle
            for (nod = minnod + nod; nod >= 1; nod--)
            {
                // Interchange
                p = old[s];
                old[s] = old[ne];
                sel = old[ne] = p;

                // Fill new set "not"
                newne = 0;
                for (i = 0; i < ne; i++)
                {
                    if (MBinary.GetValue(sel, old[i]))
                        new_[newne++] = old[i];
                }

                // Fill new set "cand"
                newce = newne;
                for (i = ne + 1; i < ce; i++)
                {
                    if (MBinary.GetValue(sel, old[i]))
                        new_[newce++] = old[i];
                }

                // Add to Compsub
                Compsub.Add(sel);

                if (newce == 0)
                {
                    StoreClique(new Clique(Compsub, MBinary.Rows));
                }
                else
                {
                    if (newne < newce)
                    {
                        BkExtend(new_, newne, newce, Compsub, MBinary);
                    }
                }

                // Remove from Compsub
                if (Compsub.Count > 0)
                    Compsub.RemoveAt(Compsub.Count - 1);

                // Add to "nod"
                ne++;
                if (nod > 1)
                {
                    // Select a candidate disconnected to the fixed point
                    for (s = ne; MBinary.GetValue(fixp, old[s]); s++)
                    {
                        // Nothing
                    }
                }
            }
        }
        
        private void StoreClique(Clique clique)
        {
           
            if (clique.Size < _minCliqueSize)
                return;

            if (_cliques.Count >= Constants.MaximumNumberOfCliques)
            {
                _cliques.Sort();
              _minCliqueSize = _cliques[0].Size;
                if (clique.Size > _minCliqueSize)
                    _cliques[0] = clique;
            }
            else 
            {
                _cliques.Add(clique);
            }
        }

        #region IEnumerable<Clique> Members

        public IEnumerator<Clique> GetEnumerator()
        {
            foreach (Clique c in _cliques)
                yield return c;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _cliques.GetEnumerator();
        }

        #endregion
    }
}
