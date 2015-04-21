using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.Windows.Forms;

namespace Network.Cliques
{
    public sealed class CliqueCalculations
    {
        private CliqueCalculations() { }

        public static double AverageCliqueSize(CliqueCollection cc)
        {
                int sum = 0;
                for (int i = 0; i < cc.Count; ++i)
                    sum += cc[i].Size;

                return (double)sum / (double)cc.Count;
        }

        public static double AverageCliqueMembers(CliqueCollection cc)
        {
            double sum = 0;
            for (int i = 0; i < cc.CliqueOverlap.Rows; i++)
                sum += cc.CliqueOverlap[i, i];

            return (double)sum / (double)cc.CliqueOverlap.Rows;            
        }

        public static double CMOI(CliqueCollection cc)
        {
            Matrix O = cc.CliqueOverlap;
            double total_sum = 0;
            for (int i = 0; i < O.Rows; i++)
                for (int j = i + 1; j < O.Rows; j++)
                {
                    if (O[j, j] != 0) 
                    total_sum += (double)(O[i, j]) / O[j, j];
                }

            double coi = (double)(O.Rows * O.Rows - O.Rows);
            if (coi != 0)
                coi = (total_sum * 2.0) / coi;
            return coi;
        }

        public static Pair<double, double> COC(CliqueCollection cc)
        {
            double simple_sum = 0, complex_sum = 0;   

            Matrix CBCO = cc.CliqueByCliqueOverlap;
            Vector v = CBCO as Vector;

            if (CBCO.Cols == 1)
                return new Pair<double, double>(0.0, 0.0);

            double denominator = (double)(((double)CBCO.Cols * ((double)CBCO.Cols - 1)) / 2.0);

            
            if (v != null)
            {
                Vector w = new Vector(v.Size);
                w.Clear();
                for (int i = 0; i < v.Size; ++i)
                {
                    for (int j = i + 1; j < v.Size; ++j)
                        w[j] += cc.GetCliqueByCliqueOverlap(i, j);
                }

                for (int j = 1; j < v.Size; ++j)
                {
                    double sum = w[j];
                    simple_sum += (sum / v[j]);
                    if (v[j] > 1)
                        complex_sum += (sum / (v[j] - 1));
                }
            }
            else
            {
                for (int j = 1; j < CBCO.Rows; ++j)
                {
                    double sum = 0.0;

                    for (int i = 0; i < j; ++i)
                        sum += CBCO[i, j];

                    simple_sum += sum / CBCO[j, j];
                    if (CBCO[j, j] > 1)
                        complex_sum += sum / (CBCO[j, j] - 1);
                }

            }

            return new Pair<double, double>(simple_sum / denominator, complex_sum / denominator);
        }

        public static double NPOL(CliqueCollection cc, Matrix data)
        {
            int total_sum = 0;

            for (int i = 0; i < cc.Count; i++)
            {
                total_sum += cc[i].Size * (data.Rows - cc[i].Size);
            }

            double first_term = 4.0 * total_sum;
            double divisor = (double)(cc.Count * data.Rows * data.Rows);

            return first_term / divisor;
        }
    }
}
