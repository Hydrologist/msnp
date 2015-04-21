using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Network.Blocks
{
    public sealed class BlockCollection : IEnumerable<Block>
    {
        public List<Block> _blocks;
        private Matrix _blocksOverlap;
        private Matrix _blocksByBlockOverlap;
        private int _minBlockSize;
        //public delegate void ParameterizedThreadStart(SymmetricBinaryMatrix obj);
        public BlockCollection(SymmetricBinaryMatrix m, int bmin, List<Block> Blocks)
        {
            _blocks = new List<Block>();
            _blocks = Blocks;
            _blocksOverlap = new Matrix(m.Rows);
            _blocksOverlap.CopyLabelsFrom(m);
            _minBlockSize = bmin;

            Vector Z = Vector.Zero(m.Rows);
            //Thread t = new Thread(delegate() { FindCliques(m); });
            // t.Start();
            //  while (!t.IsAlive) ;
            //  Thread.Sleep(100);
            //  t.Join();            
            LoadBlockOverlap();

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

            _blocks.Sort();

            //Thread temp = new Thread(LoadCliqueByCliqueOverlap);
            //temp.Start();
            //FindCliques(m);
            //while (!temp.IsAlive) ;
            //Thread.Sleep(100);
            //temp.Join();


            LoadBlockByBlockOverlap();
        }

        public void LoadBlockOverlap()
        {
            _blocksOverlap.Clear();
            foreach (Block block in _blocks)
                foreach (int i in block)
                    foreach (int j in block)
                        ++_blocksOverlap[i, j];
        }

        public int Count
        {
            get { return _blocks.Count; }
        }

        public Matrix BlockOverlap
        {
            get { return _blocksOverlap; }
        }

        public Matrix BlockByBlockOverlap
        {
            get { return _blocksByBlockOverlap; }
        }

        public int MemberRangeSize
        {
            get
            {
                if (_blocks.Count > 0)
                    return _blocks[0].MemberRangeSize;
                return -1;
            }
        }

        public double GetBlockByBlockOverlap(int i, int j)
        {
            if (_blocks.Count < Constants.CliqueByCliqueOverlapCutoff)
                return _blocksByBlockOverlap[i, j];
            else if (i == j)
                return _blocksByBlockOverlap[0, i];
            else
                return CalculateBlockbyBlockOverlap(i, j);
        }

        public Block this[int i]
        {
            get { return _blocks[i]; }
        }

        public Matrix ToBlockAffiliationMatrix()
        {
            Matrix m = new Matrix(MemberRangeSize, Count);

            for (int i = 0; i < m.Rows; ++i)
                for (int j = 0; j < m.Cols; ++j)
                    m[i, j] = _blocks[j].Contains(i) ? 1 : 0;

            return m;
        }

        private int CalculateBlockbyBlockOverlap(int i, int j)
        {
            if (i == j)
                return _blocks[i].Size;

            if (_blocks[i].Size > _blocks[j].Size)
                Algorithms.Swap(ref i, ref j);

            int matches = 0;
            foreach (int k in _blocks[i])
                if (_blocks[j].Contains(k))
                    ++matches;
            return matches;
        }

        public void LoadBlockByBlockOverlap()
        {
            // We can only have the matrix be _so_ large, otherwise calculate as needed
            if (_blocks.Count < Constants.CliqueByCliqueOverlapCutoff)
            {
                _blocksByBlockOverlap = new Matrix(_blocks.Count);
                _blocksByBlockOverlap.Clear();

                Algorithms.Iota(_blocksByBlockOverlap.RowLabels, 1);
                Algorithms.Iota(_blocksByBlockOverlap.ColLabels, 1);

                for (int c1 = 0; c1 < _blocks.Count; ++c1)
                    for (int c2 = 0; c2 < _blocks.Count; ++c2)
                        _blocksByBlockOverlap[c1, c2] = CalculateBlockbyBlockOverlap(c1, c2);
            }
            else
            {
                _blocksByBlockOverlap = new Vector(_blocks.Count);
                Vector v = _blocksByBlockOverlap as Vector;
                Algorithms.Iota(v.Labels, 1);
                for (int i = 0; i < _blocks.Count; ++i)
                    v[i] = _blocks[i].Size;
            }
        }



       #region IEnumerable<Block> Members

        public IEnumerator<Block> GetEnumerator()
        {
            foreach (Block b in _blocks)
                yield return b;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        #endregion
    }
}
