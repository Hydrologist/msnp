using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Matrices
{
    public class Vector : Matrix
    {
        public Vector(int size) : base(1, size) { }
        public Vector(Vector v) : this(v._data) { }

        /*
        public Vector(List<double> array)
            : this(array.Count)
        */
        public Vector(double[] array)
            : this(array.Length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            array.CopyTo(_data, 0);
            /*
            for (int i = 0; i < _data.Count; i++)
                array[i] = _data[i];
            */
        }

        public Vector(int[] array)
            : this(array.Length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = 0; i < array.Length; ++i)
                _data[i] = array[i];
        }

        public Vector(byte[] array)
            : this(array.Length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = 0; i < array.Length; ++i)
                _data[i] = array[i];
        }

        public int Size
        {
            get { return _cols; }
        }

        public MatrixLabels Labels
        {
            get { return _colLabels; }
        }

        public new MatrixLabels ColLabels
        {
            get { return Labels; }
        }

        public new MatrixLabels RowLabels
        {
            get { return Labels; } 
        }

        new public double this[int n]
        {
            get { return base[0, n]; }
            set { base[0, n] = value; }
        }

        public Vector DotProduct(Vector v)
        {
            Vector result = new Vector(Size);
            for (int i = 0; i < Size; ++i)
                result[i] = this[i] * v[i];
            return result;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            Vector v = new Vector(Math.Min(a.Size, b.Size));
            for (int i = 0; i < a.Size && i < b.Size; ++i)
                v[i] = a[i] + b[i];
            return v;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            Vector v = new Vector(Math.Min(a.Size, b.Size));
            for (int i = 0; i < a.Size && i < b.Size; ++i)
                v[i] = a[i] - b[i];
            return v;
        }

        public static Vector operator /(Vector a, double d)
        {
            Vector v = new Vector(a.Size);
            for (int i = 0; i < a.Size; ++i)
                v[i] = a[i] / d;
            return v;
        }

        public static Vector operator *(Matrix lhs, Vector rhs)
        {
            if (lhs.Cols != rhs.Size)
                throw new MatrixException("Dimensions of matrices do not match for multiplication.");

            Vector result = new Vector(rhs.Size);
            result.Labels.CopyFrom(rhs.Labels);
            result.Clear();
            for (int r = 0; r < lhs.Rows; r++)
                    for (int i = 0; i < lhs.Cols; i++)
                        result[r] += lhs[r, i] * rhs[i];

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Size; ++i)
            {
                sb.Append(this[i]);
                sb.Append(' ');
            }

            return sb.ToString();
        }

        public bool IsZeroVector
        {
            get
            {
                foreach (double d in this)
                {
                    if (d != 0)
                        return false;
                }
                return true;
            }
        }

        public static Vector Zero(int p)
        {
            Vector v = new Vector(p);
            Array.Clear(v._data, 0, p);
            /*
            for (int i = 0; i < v._data.Count; i++)
                v._data[i] = 0.0;
            */
            return v;
        }

        public static bool isEqual(Vector a, Vector b)
        {
            if (a.Size != b.Size)
                return false;

            int size = a.Size;
            bool equal = true;
            for (int i = 0; i < size; i++)
            {
                if (a[i] != b[i])
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }

        public static bool isSubset(Vector a, Vector b)
        {
            if (a.Size != b.Size)
                return false;

            int size = a.Size;
            int notEqualCount = 0;
            bool isProper = true;
            for (int i = 0; i < size; i++)
            {
                if (a[i] != b[i])
                    notEqualCount++;
            }
            if (notEqualCount >= 2)
                isProper = false;
            return isProper;
        }

        /*
         * int notTaken: parameter is used to return the position of
         * the vector that is not taken
         */
        public static int getSubsetByPos(Vector a, int pos1, Vector b, int pos2, ref int notTaken)
        {
            int size = a.Size;
            double sum1 = 0;
            double sum2 = 0;
            for (int i = 0; i < size; i++)
            {
                sum1 += a[i];
                sum2 += b[i];
            }

            if (sum1 > sum2)
            {
                notTaken = pos2;
                return pos1;
            }
            else
            {
                notTaken = pos1;
                return pos2;
            }
        }
    }
}
