using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Matrices
{
    public class SymmetricBinaryMatrix : Matrix
    {
        private double _cutoff;

        public SymmetricBinaryMatrix(int rows) : this(rows, double.Epsilon) { }
        public SymmetricBinaryMatrix(int rows, double cutoff)
            : base(rows, rows)
        {
            _cutoff = cutoff;
        }
        public SymmetricBinaryMatrix(Matrix m) : this(m, double.Epsilon, CliqueExtractionType.Upper) { }
        public SymmetricBinaryMatrix(Matrix m, double cutoff) : this(m, cutoff, CliqueExtractionType.Upper) { }
        public SymmetricBinaryMatrix(Matrix m, double cutoff, CliqueExtractionType cet)
            : base(m)
        {
            _cutoff = cutoff;

            for (int i = 0; i < _rows; i++)
            {
                for (int j = i; j < _cols; j++)
                {
                    double val = 0.0;
                    switch (cet)
                    {
                        case CliqueExtractionType.Max:
                            val = Math.Max(m[i, j], m[j, i]);
                            break;
                        case CliqueExtractionType.Min:
                            val = Math.Min(m[i, j], m[j, i]);
                            break;
                        case CliqueExtractionType.Upper:
                            val = m[i, j];
                            break;
                        case CliqueExtractionType.Lower:
                            val = m[j, i];
                            break;
                    }

                    this[i, j] = val;
                }
            }
        }
        public SymmetricBinaryMatrix(SymmetricBinaryMatrix m) : this(m, m._cutoff, CliqueExtractionType.Upper) { }

        public bool GetValue(int r, int c)
        {
            if (r == c)
                return true;

            return this[r, c] > 0;
        }
        public void SetValue(int r, int c, bool value)
        {
            this[c, r] = this[r, c] = value ? 1 : 0;
        }

        public override double this[int r, int c]
        {
            get
            {
                if (r == c)
                    return 1;

                return base[r, c] > 0 ? 1 : 0;
            }
            set
            {
                base[r, c] = base[c, r] = value > _cutoff ? 1 : 0; // keep matrix symmetric
            }
        }

        
    }
}
