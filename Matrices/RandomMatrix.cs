using System;
using System.Collections.Generic;
using System.Text;
using RandomUtilities;
using System.Windows.Forms;

namespace Network.Matrices
{
    public sealed class RandomMatrix
    {
        private RandomMatrix() { }

        public static Matrix LoadNonSymmetric(int n, bool range, double pmin, double pmax)
        {
            if (!range)
            {
                Matrix m = LoadNonSymmetric(n, n);

                for (int i = 0; i < m.Rows; ++i)
                    m[i, i] = 0.0;

                return m;
            }
            else
            {

                double probno = RNG.RandomFloat(pmin, pmax);
                double newprob = probno;
                int counter = 0;
                Matrix m = new Matrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j) m[i, j] = 0;
                 //       else if (((counter + 1) / (double)(n - 1)) > newprob) m[i, j] = 0;
                        else
                        {
                            m[i, j] = RNG.RandomBinary(newprob);
                            if (m[i, j] == 1) counter++;
                        }
                    } 
                    newprob = probno + newprob - (counter / (double)(n - 1));
                    counter = 0;
                }

 
                Algorithms.Iota(m.ColLabels, 1);
                Algorithms.Iota(m.RowLabels, 1);
            return m;
            }

        }

        public static Matrix LoadNonSymmetric(int rows, int cols)
        {
            Matrix m = new Matrix(rows, cols);

            Algorithms.Fill<double>(m, RNG.RandomBinary);
            Algorithms.Iota(m.ColLabels, 1);
            Algorithms.Iota(m.RowLabels, 1);

            return m;
        }

        public static Matrix LoadValuedNonSymmetric(int n, double vmin, double vmax, bool datatype, bool zerodiagonalized, bool range, double pmin, double pmax)
        {

            if (!range)
            {
                Matrix m = LoadValuedNonSymmetric(n, n, vmin, vmax, datatype, zerodiagonalized);

                return m;
            }
            else
            {

                double probno = RNG.RandomFloat(pmin, pmax);
                double newprob = probno;
                int counter = 0;
                Matrix m = new Matrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j && zerodiagonalized) m[i, j] = 0;
                        //       else if (((counter + 1) / (double)(n - 1)) > newprob) m[i, j] = 0;
                        else
                        {
                            m[i, j] = RNG.RandomBinary(newprob);
                            if (m[i, j] == 1)
                            {
                                m[i, j] = datatype ? RNG.RandomInt(vmin, vmax) : RNG.RandomFloat(vmin, vmax);
                                counter++;
                            }
                        }
                    }
                    newprob = probno + newprob - (counter / (double)(zerodiagonalized?(n - 1):n));
                    counter = 0;
                }

                Algorithms.Iota(m.ColLabels, 1);
                Algorithms.Iota(m.RowLabels, 1);
                return m;
            }


        }

        public static Matrix LoadValuedNonSymmetric(int rows, int cols, double vmin, double vmax, bool datatype, bool zerodiagonalized)
        {
            Matrix m = new Matrix(rows, cols);
            if(datatype)
                Algorithms.Fill<double>(m, RNG.RandomInt, vmin, vmax);
            else
                Algorithms.Fill<double>(m, RNG.RandomFloat, vmin, vmax);

            if (zerodiagonalized)
            {
                for (int i = 0; i < m.Rows; ++i)
                    m[i, i] = 0.0;
            }
            Algorithms.Iota(m.ColLabels, 1);
            Algorithms.Iota(m.RowLabels, 1);

            return m;
        }

        public static Matrix LoadWithProbabilisticRange(int rows, int cols, double min, double max)
        {
            Matrix m = new Matrix(rows, cols);
            Algorithms.Iota(m.ColLabels, 1);
            Algorithms.Iota(m.RowLabels, 1);

            for (int i = 0; i < rows; ++i)
                for (int j = i + 1; j < cols; ++j)
                    m[i, j] = m[j, i] = RNG.RandomBinary(RNG.RandomFloat(min, max));

            return m;
        }

        public static Matrix LoadSymmetric(int n, bool range, double pmin, double pmax)
        {
            if (!range)
            {
                Matrix m = LoadNonSymmetric(n, range, pmin, pmax);

                for (int i = 0; i < m.Rows; ++i)
                    for (int j = i + 1; j < m.Rows; ++j)
                        m[j, i] = m[i, j];

                return m;
            }
            else
            {

                double probno = RNG.RandomFloat(pmin, pmax);
                double newprob = probno;
                int counter = 0;
                Matrix m = new Matrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j) m[i, j] = 0;
                        else if (i > j)
                        {
                            m[i, j] = m[j, i];
                            if (m[i, j] == 1)
                                counter++;
                        }
                    //    else if (((counter + 1) / (double)(n - 1)) > newprob) m[i, j] = 0;
                        else
                        {
                            m[i, j] = RNG.RandomBinary(newprob);
                            if (m[i, j] == 1) counter++;
                        }
                    }
                    newprob = probno + newprob - (counter / (double)(n - 1));
                    counter = 0;
                }


                Algorithms.Iota(m.ColLabels, 1);
                Algorithms.Iota(m.RowLabels, 1); 

                return m;
            }
        }
        public static Matrix LoadBlank(int N)
        {
            Matrix m = new Matrix(N);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    m[i, j] = 0;
                }
            }
            return m;
        }

        public static Matrix LoadValuedSymmetric(int n, double vmin, double vmax, bool datatype, bool zerodiagonalized, bool range, double pmin, double pmax)
        {


            if (!range)
            {
                Matrix m = LoadValuedNonSymmetric(n, vmin, vmax, datatype, zerodiagonalized, range, pmin, pmax);

                for (int i = 0; i < m.Rows; ++i)
                    for (int j = i + 1; j < m.Rows; ++j)
                        m[j, i] = m[i, j];

                return m;
            }
            else
            {

                double probno = RNG.RandomFloat(pmin, pmax);
                double newprob = probno;
                int counter = 0;
                Matrix m = new Matrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j && zerodiagonalized) m[i, j] = 0;
                        else if (i > j)
                        {
                            m[i, j] = m[j, i];
                            if (m[i, j] != 0)
                                counter++;
                        }
                        //       else if (((counter + 1) / (double)(n - 1)) > newprob) m[i, j] = 0;
                        else
                        {
                            m[i, j] = RNG.RandomBinary(newprob);
                            if (m[i, j] == 1)
                            {
                                m[i, j] = datatype ? RNG.RandomInt(vmin, vmax) : RNG.RandomFloat(vmin, vmax);
                                counter++;
                            }
                        }
                    }
                    newprob = probno + newprob - (counter / (double)(zerodiagonalized ? (n - 1) : n));
                    counter = 0;
                }

                //int hi = 0;
                //for (int i = 0; i < n; i++)
                //    for (int j = 0; j < n; j++)
                //        if (m[i, j] != 0) hi++;

                //MessageBox.Show(hi / (n * (double)(zerodiagonalized ? (n - 1) : n)) + "");

                Algorithms.Iota(m.ColLabels, 1);
                Algorithms.Iota(m.RowLabels, 1);
                return m;
            }

        }

        public static Matrix LoadDiagonal(int n, bool normalize)
        {
            Matrix m = new Matrix(n);
            Algorithms.Iota(m.ColLabels, 1);
            Algorithms.Iota(m.RowLabels, 1);

            double sum = 0.0;
            for (int i = 0; i < m.Rows; ++i)
            {
                m[i, i] = RNG.RandomFloat();
                sum += m[i, i];
            }

            if (normalize)
                for (int i = 0; i < m.Rows; ++i)
                    m[i, i] /= sum;

            return m;
        }

        public static Vector LoadVector(int n)
        {
            Vector v = new Vector(n);
            Algorithms.Iota(v.Labels, 1);
            Algorithms.Fill<double>(v, RNG.RandomBinary);

            return v;
        }

        public static Vector LoadRealUnitVector(int n)
        {
            Vector v = new Vector(n);
            Algorithms.Iota(v.Labels, 1);
            Algorithms.Fill<double>(v, RNG.RandomFloat);
            v.Normalize();

            return v;
        }
    }
}
