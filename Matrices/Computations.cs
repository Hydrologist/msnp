using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Network.Matrices
{
    public sealed class MatrixComputations
    {
        public static double Density(Matrix m)
        {
            return Density(m, Algorithms.MaxValue<double>(m));
        }
        public static double Density(Matrix m, double maxMatrixValue)
        {
            maxMatrixValue = Algorithms.MaxValue<double>(m);
            if (Math.Abs(maxMatrixValue) < double.Epsilon)
                return 0.0;

            double total = 0;

            foreach (double d in m)
                total += d;

            total = m.sum();

            return Math.Abs(total / (maxMatrixValue * m.Rows * (m.Rows - 1)));
        }

        public static double SubmatrixDensity(Matrix m, int rowStart, int rowEnd, int colStart, int colEnd, bool diagonalBlock)
        {
            return SubmatrixDensity(m, rowStart, rowEnd, colStart, colEnd, Algorithms.MaxValue<double>(m), diagonalBlock);
        }
        public static double SubmatrixDensity(Matrix m, int rowStart, int rowEnd, int colStart, int colEnd, double maxMatrixValue, bool diagonalBlock)
        {
            
            double max = Algorithms.MaxValue<double>(m);
            double total = 0;
            for (int i = rowStart; i < rowEnd; ++i)
                for (int j = colStart; j < colEnd; ++j)
                    total += m[i, j];

            if (total == 0) return 0;

            return Math.Abs(total / ( max * (rowEnd - rowStart) * (diagonalBlock ? (colEnd - colStart - 1) : (colEnd - colStart))));
            
            
        }

        public enum TransitivityType
        {
            Simple, Weak, Strong
        }

        public static double Transitivity(Matrix m, TransitivityType type)
        {
            int total = 0, matching = 0;

            if (!m.IsSquareMatrix)
                throw new ComputationException("Can only compute transitivity for square matrices.");

            switch (type)
            {
                case TransitivityType.Simple:
                    for (int i = 0; i < m.Rows; i++)
                    {
                        for (int j = 0; j < m.Rows; j++)
                        {
                            if (m[i, j] <= 0.0 || i == j)
                                continue;

                            for (int k = 0; k < m.Rows; k++)
                            {
                                if (m[j, k] <= 0.0 || j == k || k == i)
                                    continue;

                                ++total;
                                if (m[i, k] > 0)
                                    ++matching;
                            }
                        }
                    }
                    break;
                case TransitivityType.Weak:

                    for (int i = 0; i < m.Rows; i++)
                    {
                        for (int j = 0; j < m.Rows; j++)
                        {
                            if (m[i, j] <= 0.0 || i == j)
                                continue;

                            for (int k = 0; k < m.Rows; k++)
                            {
                                if (m[i, k] <= 0.0 || j == k || k == i)
                                    continue;

                                ++total;
                                if (m[j, k] > Math.Min(m[i, j], m[i, k]))
                                    ++matching;
                            }
                        }
                    }
                    break;
                case TransitivityType.Strong:

                    for (int i = 0; i < m.Rows; i++)
                    {
                        for (int j = 0; j < m.Rows; j++)
                        {
                            if (m[i, j] <= 0.0 || i == j)
                                continue;

                            for (int k = 0; k < m.Rows; k++)
                            {
                                if (m[i, k] <= 0.0 || j == k || k == i)
                                    continue;

                                ++total;
                                if (m[j, k] > Math.Max(m[i, j], m[i, k]))
                                    ++matching;
                            }
                        }
                    }
                    break;
            }

            if (total == 0)
                return 0.0;

            return (double)matching / (double)total;
        }

        public static Matrix StructuralEquivalenceCorrelation(Matrix m)
        {
            Matrix SEC = new Matrix(m);

            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    double sec = 0.0;
                    double sqrtSum1 = 0.0, sqrtSum2 = 0.0;
                    for (int k = 0; k < m.Rows; ++k)
                    {
                        sec += ((m[i, k] - m.GetRowAverage(i)) * (m[j, k] - m.GetRowAverage(j)));
                        sec += ((m[k, i] - m.GetColAverage(i)) * (m[k, j] - m.GetColAverage(j)));

                        sqrtSum1 += (m[i, k] - m.GetRowAverage(i)) * (m[i, k] - m.GetRowAverage(i));
                        sqrtSum1 += (m[k, i] - m.GetColAverage(i)) * (m[k, i] - m.GetColAverage(i));

                        sqrtSum2 += (m[j, k] - m.GetRowAverage(j)) * (m[j, k] - m.GetRowAverage(j));
                        sqrtSum2 += (m[k, j] - m.GetColAverage(j)) * (m[k, j] - m.GetColAverage(j));
                    }
                    sqrtSum1 = Math.Sqrt(sqrtSum1);
                    sqrtSum2 = Math.Sqrt(sqrtSum2);
                    if (Math.Abs(sqrtSum1 * sqrtSum2) > 0.0)
                        sec /= (sqrtSum1 * sqrtSum2);
                    else
                        sec = 0.0;

                    SEC[i, j] = sec;
                }
            }

            return SEC;
        }

        public static Matrix StructuralEquivalenceEuclidean(Matrix m)
        {
            Matrix SEE = new Matrix(m);

            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    double see = 0.0;
                    double sum = 0.0;
                    for (int k = 0; k < m.Rows; ++k)
                    {
                        see += Math.Pow(m[i, k] - m[j, k], 2);
                        see += Math.Pow(m[k, i] - m[k, j], 2);

                        sum += m[i, k] + m[k, j];
                    }
                    if (sum > 0.0)
                        SEE[i, j] = Math.Sqrt(see);
                    else
                        SEE[i, j] = 1.0;
                }
            }

            return SEE;
        }


        public static Matrix RoleEquivalence(Matrix m)
        {
            Matrix RoleEquiv = new Matrix(m);

            // Generate temporary triad matrix
            Matrix TempTriad = new Matrix(m.Rows, 36);
            TempTriad.Clear();
            
       

            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = i + 1; j < m.Rows; ++j)
                {
                    //   if (j == i)
                    //     continue;
                    for (int k = j + 1; k < m.Rows; ++k)
                    {
                        //   if (k == i || k == j)
                        //     continue;

                        int type = GetRoleEquivalenceType(GetRelationshipType(i, j, m), GetRelationshipType(i, k, m), GetRelationshipType(j, k, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];
                        type = GetRoleEquivalenceType(GetRelationshipType(i, k, m), GetRelationshipType(i, j, m), GetRelationshipType(k, j, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];
                        type = GetRoleEquivalenceType(GetRelationshipType(j, i, m), GetRelationshipType(j, k, m), GetRelationshipType(i, k, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];
                        type = GetRoleEquivalenceType(GetRelationshipType(j, k, m), GetRelationshipType(j, i, m), GetRelationshipType(k, i, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];
                        type = GetRoleEquivalenceType(GetRelationshipType(k, i, m), GetRelationshipType(k, j, m), GetRelationshipType(i, j, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];
                        type = GetRoleEquivalenceType(GetRelationshipType(k, j, m), GetRelationshipType(k, i, m), GetRelationshipType(j, i, m));

                        ++TempTriad[i, type - 1];
                        ++TempTriad[j, type - 1];
                        ++TempTriad[k, type - 1];


                    }
                }
            }


            for (int i = 0; i < RoleEquiv.Rows; ++i)
            {
                for (int j = 0; j < RoleEquiv.Cols; ++j)
                {
                    double requiv = 0.0;
                    for (int k = 0; k < TempTriad.Cols; ++k)
                        requiv += Math.Pow(TempTriad[i, k] - TempTriad[j, k], 2);
                    //       mTable["RoleEquiv"][i, j] = 1.0 - (1.0 * Math.Sqrt(requiv) / ((mTable[m].Rows - 1 ) * (mTable[m].Rows - 2)));
                    RoleEquiv[i, j] = 1.0 - (Math.Sqrt(requiv) / (3 * (RoleEquiv.Rows - 1) * (RoleEquiv.Rows - 2)));
                }
            }

            return RoleEquiv;

        }


        /* Old Method SESE
        public static Matrix StructuralEquivalenceStandardizedEuclidean(Matrix m, double maxMatrixValue)
        {
            Matrix SESE = new Matrix(m);

            // Go through and find SE for each space
            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    double sese = 0.0;
                    for (int k = 0; k < m.Rows; ++k)
                    {
                        sese += Math.Pow(m[i, k] - m[j, k], 2);
                        sese += Math.Pow(m[k, i] - m[k, j], 2);
                    }
                    SESE[i, j] = 1.0 - Math.Sqrt(sese) / (m.Rows * m.Rows * maxMatrixValue);
                }
            }

            return SESE;
        }
         * */

        public static Matrix StructuralEquivalenceStandardizedEuclidean(Matrix m, double maxMatrixValue)
        {
            Matrix SESE = new Matrix(m);

            maxMatrixValue = double.MinValue;

            // Go through and find SE for each space
            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    double sese = 0.0;
                    for (int k = 0; k < m.Rows; ++k)
                    {
                        double d = Math.Pow(m[i, k] - m[j, k], 2);
                        double d2 = Math.Pow(m[k, i] - m[k, j], 2);

                        if (d > maxMatrixValue) 
                            maxMatrixValue = d;

                        if (d2 > maxMatrixValue)
                            maxMatrixValue = d2;

                        sese += d;
                        sese += d2;
                    }
                    SESE[i, j] = sese;
                }
            }

            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    SESE[i, j] /=  2 * m.Rows * maxMatrixValue;
                    SESE[i, j] = 1 - SESE[i, j];
                }
            }

            return SESE;
        }

        public static Matrix StructuralEquivalenceStandardizedEuclidean(Matrix m)
        {
            return StructuralEquivalenceStandardizedEuclidean(m, Algorithms.MaxValue<double>(m));
        }

        public static Matrix GeodesicDistance(Matrix m)
        {
            Matrix geodesic = new Matrix(m.Rows);

            Queue<Pair<int, int>> q = new Queue<Pair<int, int>>();

            // Add every possible node to the list
            for (int i = 0; i < m.Rows; ++i)
                for (int j = 0; j < m.Rows; ++j)
                    q.Enqueue(new Pair<int, int>(i, j));

            // Go until each one has been assigned a value or we run out
            Matrix Power = new Matrix(m);
            for (int i = 1; i < m.Rows && q.Count > 0; ++i)
            {
                for (int j = 0; j < q.Count; ++j)
                {
                    Pair<int, int> current = q.Dequeue();
                    int row = current.First;
                    int col = current.Second;
                    if (Power[row, col] > 0)
                    {
                        geodesic[row, col] = i;
                        // Don't enqueue this again
                        ++j;
                    }
                    else
                    {
                        // Enqueue it to try again next time
                        q.Enqueue(current);
                    }
                }
                Power *= m;
            }

            // Fix up the diagonal entires to be 0
            for (int i = 0; i < geodesic.Rows; ++i)
                geodesic[i, i] = 1;

            // And make it symmetrical
            for (int i = 0; i < geodesic.Rows; ++i)
                for (int j = i + 1; j < geodesic.Cols; ++j)
                    geodesic[i, j] = geodesic[j, i] = Math.Min(geodesic[i, j], geodesic[j, i]);

            return geodesic;
        }

        public static Vector BetweennessCentrality(Matrix m)
        {
            int N = m.Rows;
            Vector BV = new Vector(N);
            BV.Clear();

            for (int s = 0; s < N; ++s)
            {
                Queue<int> Q = new Queue<int>();
                Q.Enqueue(s);

                Stack<int> S = new Stack<int>();

                List<List<int>> P = new List<List<int>>(N);
                for (int i = 0; i < N; ++i)
                    P.Add(new List<int>());

                Vector d = new Vector(N);
                Algorithms.Fill<double>(d, -1);
                d[s] = 0;

                Vector sigma = new Vector(N);
                sigma[s] = 1;

                while (Q.Count > 0)
                {
                    int v = Q.Dequeue();

                    S.Push(v);

                    for (int w = 0; w < N; ++w)
                    {
                        if (m[v, w] > 0)
                        {
                            if (d[w] < 0)
                            {
                                Q.Enqueue(w);
                                d[w] = d[v] + 1;
                            }

                            if (d[w] == d[v] + 1)
                            {
                                sigma[w] = sigma[w] + sigma[v];
                                P[w].Add(v);
                            }
                        }
                    }
                }

                Vector delta = new Vector(N);
                delta.Clear();

                while (S.Count > 0)
                {
                    int w = S.Pop();
                    foreach (int v in P[w]) 
                    {
                        delta[v] += (sigma[v] / sigma[w]) * (1 + delta[w]);
                    }

                    if (w != s)
                    {
                        BV[w] += delta[w];
                    }
                }
            }

            for (int i = 0; i < BV.Size; ++i)
                BV[i] *= 100.0 / (((double)N - 1) * ((double)N - 2));

            return BV;
        }

        public static Vector EigenvectorCentrality(Matrix m)
        {
            Vector v = RandomMatrix.LoadRealUnitVector(m.Rows);
            v.Labels.CopyFrom(m.RowLabels);

            int numIter = Math.Max(Constants.MinimumNumberOfConvergenceSteps, m.Rows);

            while (numIter-- > 0)
            {
                v = m * v;
                v.Normalize();
            }

            for (int i = 0; i < v.Size; ++i)
                v[i] *= 100.0 * Math.Sqrt(2);

            return v;
        }

        private static int GetRoleEquivalenceType(int AB, int AC, int BC)
        {
            int[, ,] T = new int[4, 4, 4];

            T[0, 0, 0] = 1;
            T[0, 0, 1] = 21;
            T[0, 0, 2] = 21;
            T[0, 0, 3] = 11;
            T[0, 1, 0] = 2;
            T[0, 1, 1] = 22;
            T[0, 1, 2] = 31;
            T[0, 1, 3] = 12;
            T[0, 2, 0] = 4;
            T[0, 2, 1] = 24;
            T[0, 2, 2] = 32;
            T[0, 2, 3] = 14;
            T[0, 3, 0] = 6;
            T[0, 3, 1] = 33;
            T[0, 3, 2] = 26;
            T[0, 3, 3] = 16;
            T[1, 0, 0] = 2;
            T[1, 0, 1] = 22;
            T[1, 0, 2] = 31;
            T[1, 0, 3] = 12;
            T[1, 1, 0] = 3;
            T[1, 1, 1] = 23;
            T[1, 1, 2] = 23;
            T[1, 1, 3] = 13;
            T[1, 2, 0] = 8;
            T[1, 2, 1] = 34;
            T[1, 2, 2] = 28;
            T[1, 2, 3] = 18;
            T[1, 3, 0] = 9;
            T[1, 3, 1] = 35;
            T[1, 3, 2] = 29;
            T[1, 3, 3] = 19;
            T[2, 0, 0] = 4;
            T[2, 0, 1] = 24;
            T[2, 0, 2] = 32;
            T[2, 0, 3] = 14;
            T[2, 1, 0] = 8;
            T[2, 1, 1] = 28;
            T[2, 1, 2] = 34;
            T[2, 1, 3] = 18;
            T[2, 2, 0] = 5;
            T[2, 2, 1] = 25;
            T[2, 2, 2] = 25;
            T[2, 2, 3] = 15;
            T[2, 3, 0] = 10;
            T[2, 3, 1] = 36;
            T[2, 3, 2] = 30;
            T[2, 3, 3] = 20;
            T[3, 0, 0] = 6;
            T[3, 0, 1] = 26;
            T[3, 0, 2] = 33;
            T[3, 0, 3] = 16;
            T[3, 1, 0] = 9;
            T[3, 1, 1] = 29;
            T[3, 1, 2] = 35;
            T[3, 1, 3] = 19;
            T[3, 2, 0] = 10;
            T[3, 2, 1] = 30;
            T[3, 2, 2] = 36;
            T[3, 2, 3] = 20;
            T[3, 3, 0] = 7;
            T[3, 3, 1] = 27;
            T[3, 3, 2] = 27;
            T[3, 3, 3] = 17;

            return T[AB, AC, BC];
        }

        private static int GetRelationshipType(int i, int j, Matrix m)
        {
            if (m[i, j] > 0 && m[j, i] > 0)
                return 3;
            else if (m[i, j] > 0)
                return 1;
            else if (m[j, i] > 0)
                return 2;

            return 0;
        }

    }
}
