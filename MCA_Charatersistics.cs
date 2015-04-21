using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using Network.IO;

namespace Network
{
    public class MCA_Characteristics
    {
        protected string[] labels = { "N", "No. Networks", "No. Clques", "Clque Members", "Weight Clique Members", "Simple CPOL", "CMOI", "WCMOI", "Simple NPI", "Simple Clq. Overlap", "Wght. Simple Clq. Overlap", "Complex Clique Overlap", "Weighted COC", "Density", "Transitivity", "Interdependence" };
        public int N;
        public int No_Networks;
        public int No_cliques;
        public double clq_mem;
        public double Weighted_Clique_Membership;
        public double CPOL;
        public double CMOI;
        public double WCMOI;
        public double SimpleNPI;
        public double Weighted_Simple_Clique_Overlap;
        public double Density;
        public double Transitivity;
        public MatrixComputations.TransitivityType transitivitytype;
        public double Interdependence;
        public double K;
        public double COCI;
        public double WCOCI;
        public double COCm;
        public double WCOCm;
        private Dictionary<string, bool> labelSettings;

        public MCA_Characteristics(MatrixComputations.TransitivityType TT) {
            labelSettings = new Dictionary<string, bool>(labels.Length);
            foreach (string s in labels)
            {
                labelSettings[s] = true;
            }
            transitivitytype = TT;

        }

        public string Label
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in labels)
                {
                    if (UseLabel(s))
                    {
                        sb.Append(s);
                        sb.Append(',');
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }

        protected  bool UseLabel(string label)
        {
            return labelSettings[label];
        }

        public void SetLabel(string s, bool val)
        {
            labelSettings[s] = val;
        }
        public void SetLabel(int i, bool val)
        {
            labelSettings[labels[i]] = val;
        }

        public string[] MCALine
        {
            get
            {
                string[] line = new string[labels.Length];
                int curLine = -1;


                if (labelSettings["N"]) line[++curLine] = N.ToString();
                if (labelSettings["No. Networks"]) line[++curLine] = No_Networks.ToString();
                if (labelSettings["No. Clques"]) line[++curLine] = No_cliques.ToString();
                if (labelSettings["Clque Members"]) line[++curLine] = clq_mem.ToString();
                if (labelSettings["Weight Clique Members"]) line[++curLine] = Weighted_Clique_Membership.ToString();
                if (labelSettings["Simple CPOL"]) line[++curLine] = CPOL.ToString();
                if (labelSettings["CMOI"]) line[++curLine] = CMOI.ToString();
                if (labelSettings["WCMOI"]) line[++curLine] = WCMOI.ToString();
                if (labelSettings["Simple NPI"]) line[++curLine] = (CPOL*(1-CMOI)).ToString();
                if (labelSettings["Simple Clq. Overlap"]) line[++curLine] = COCI.ToString();
                if (labelSettings["Wght. Simple Clq. Overlap"]) line[++curLine] = WCOCI.ToString();
                if (labelSettings["Complex Clique Overlap"]) line[++curLine] = COCm.ToString();
                if (labelSettings["Weighted COC"]) line[++curLine] = WCOCm.ToString();
                if (labelSettings["Density"]) line[++curLine] = Density.ToString();
                if (labelSettings["Transitivity"]) line[++curLine] = Transitivity.ToString();
                if (labelSettings["Interdependence"]) line[++curLine] = Interdependence.ToString();

                return line;
            }
        }

        public void Calculate(global::Network.Matrices.MatrixTable matrixTable, List<clique> cliques, List<global::Network.Matrices.Matrix> list, List<int> year, List<int> networkid, List<double> weight, bool useweight)
        {
            K = cliques.Count;
            N = matrixTable["Data"].Cols;
            No_Networks = list.Count;
            No_cliques = cliques.Count;

            clq_mem = calculate_clq_mem(cliques);

            Weighted_Clique_Membership = calculate_Weighted_Clique_Membership(cliques);
           WCMOI= CalculateWCMOI(matrixTable["wcmo"]);
            CMOI = CalculateWCMOI(matrixTable["cmo_mat"]);
            CPOL = calculate_NPOL(matrixTable["Data"], cliques);
            WCOCI = calculate_WCOC(matrixTable["wcoc"]);
            COCI = calculate_WCOC(matrixTable["coc_mat"]);
            WCOCm = calculate_WCOCm(matrixTable["wcoc"]);
            COCm = calculate_WCOCm(matrixTable["coc_mat"]);
            Density = calculate_Density(list);

            if(!useweight)
                Interdependence = calculate_InterdependenceSameWeight(list);
            else
                Interdependence = calculate_InterdependenceWeightFile(list, year, networkid, weight);
            if (transitivitytype == MatrixComputations.TransitivityType.Simple) Transitivity = calculate_Simple_Transitivity(list);
            if (transitivitytype == MatrixComputations.TransitivityType.Weak) Transitivity = calculate_Weak_Transitivity(list);
            if (transitivitytype == MatrixComputations.TransitivityType.Strong) Transitivity = calculate_Strong_Transitivity(list);



        }

        private double CalculateWCMOI(global::Network.Matrices.Matrix matrix)
        {
            double sum=0;
            for (int i = 0; i < N - 1; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    sum += matrix[i, j] / matrix[j, j];
                }
            }
            return sum / (N * (N - 1));
        }

        private double calculate_Weighted_Clique_Membership(List<clique> cliques)
        {
            double ans = 0;
            foreach (clique i in cliques)
            {
                ans += i.num_elements()*i.num_networks;
            }
            return ans / cliques.Count;
        }

        private double calculate_clq_mem(List<clique> cliques)
        {
            double ans = 0;
            foreach (clique i in cliques)
            {
                ans += i.num_elements();
            }
            return ans / cliques.Count;
        }

        private double calculate_WCOC(global::Network.Matrices.Matrix matrix)
        {
            double sum = 0;
            for (int i = 0; i < K - 1; i++)
            {
                for (int j = i + 1; j < K; j++)
                {
                    sum += matrix[i, j]/(matrix[j,j]);
                }
            }
            return sum / (K * (K - 1));
        }

        private double calculate_WCOCm(global::Network.Matrices.Matrix matrix)
        {
            double sum = 0;
            for (int i = 0; i < K - 1; i++)
            {
                for (int j = i + 1; j < K; j++)
                {
                    sum += matrix[i, j] / (matrix[j, j] - 1);
                }
            }
            return sum / (K * (K - 1));
        }

        private double calculate_Density(List<Matrices.Matrix> list)
        {
            double sum = 0;
            foreach( Matrices.Matrix M in list)
            {
                sum += M.sum()/(Algorithms.MaxValue<double>(M)*N*N) ;
            }
            return sum / list.Count;
        }

        private double calculate_Strong_Transitivity(List<Matrices.Matrix> list)
        {
            double sum = 0;
            foreach (Matrices.Matrix i in list)
            {
                sum += Matrices.MatrixComputations.Transitivity(i, Matrices.MatrixComputations.TransitivityType.Strong);
            }
            return sum / list.Count;
        }

        private double calculate_Weak_Transitivity(List<Matrices.Matrix> list)
        {
            double sum = 0;
            foreach (Matrices.Matrix i in list)
            {
                sum += Matrices.MatrixComputations.Transitivity(i, Matrices.MatrixComputations.TransitivityType.Weak);
            }
            return sum / list.Count;
        }

        private double calculate_Simple_Transitivity(List<Matrices.Matrix> list)
        {
            double sum = 0;
            foreach (Matrices.Matrix i in list)
            {
                sum += Matrices.MatrixComputations.Transitivity(i, Matrices.MatrixComputations.TransitivityType.Simple);
            }
            return sum / list.Count;
        }


        private double calculate_InterdependenceSameWeight(List<Matrices.Matrix> list)  //only for first-order dependency, e.g. reachmatrixcount = 1; otherwise for third or more order, need to run dependency matrix alg
        {
            Matrices.Matrix temp = new Matrices.Matrix(list[0]);
            foreach (Matrices.Matrix i in list)
            {
                temp += i;
            }

            double maxik = Algorithms.MaxValue<double>(temp);

            double SYSIN = 0.0;
            int reachMatrixCount = 1;

            int N = temp.Rows;

            bool zeroDiagonal = false;

            for (int i = 0; i < N; i++)
            {
                if (temp[i, i] != 0)
                {
                    zeroDiagonal = false;
                    break;
                }
                zeroDiagonal = true;
            }

            if (!zeroDiagonal)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += temp[i, j];
                    }
                    SYSIN -= temp[i, i];
                }
                SYSIN *= (1 - maxik * N);
                SYSIN /= maxik;
                SYSIN /= (1 - Math.Pow(maxik * N, reachMatrixCount));
            }
            else
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += temp[i, j];
                    }
                    
                }
                SYSIN *= (1 - maxik * (N - 1));
                SYSIN /= maxik * (N - 1);
                SYSIN /= (1 - Math.Pow(maxik * (N - 1), reachMatrixCount));
            }

            return SYSIN;

        }


        private double calculate_InterdependenceWeightFile(List<Matrices.Matrix> list, List<int> year, List<int> networkid, List<double> weight)
        {
            Matrices.Matrix temp = new Matrices.Matrix(list[0]);

            for (int i = 0; i < list.Count; i++)
            {
                for (int row = 0; row < list[0].Rows; row++)
                    for (int col = 0; col < list[0].Cols; col++)
                        list[i][row, col] *= weight[i];
            }

            foreach (Matrices.Matrix i in list)
            {
                temp += i;
            }

            double maxik = Algorithms.MaxValue<double>(temp);

            double SYSIN = 0.0;
            int reachMatrixCount = 1;

            int N = temp.Rows;

            bool zeroDiagonal = false;

            for (int i = 0; i < N; i++)
            {
                if (temp[i, i] != 0)
                {
                    zeroDiagonal = false;
                    break;
                }
                zeroDiagonal = true;
            }

            if (!zeroDiagonal)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += temp[i, j];
                    }
                    SYSIN -= temp[i, i];
                }
                SYSIN *= (1 - maxik * N);
                SYSIN /= maxik;
                SYSIN /= (1 - Math.Pow(maxik * N, reachMatrixCount));
            }
            else
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += temp[i, j];
                    }

                }
                SYSIN *= (1 - maxik * (N - 1));
                SYSIN /= maxik * (N - 1);
                SYSIN /= (1 - Math.Pow(maxik * (N - 1), reachMatrixCount));
            }

            return SYSIN;

        }


        
            //quoted from options.cs for future use

      /*     protected double GetRealDensity(double density, int year, string m)
        {
            if (density == -2.0)
                return Algorithms.MaxValue<double>(mTable[m]);
            if (density == -1.0)
                if (!densityVector.ContainsKey(year))
                    throw new Exception("Cannot use density vector for year " + year.ToString());
                else
                    return densityVector[year];
            return density;
        }


         public double GetSYSIN(bool zeroDiagonal, double maxik, int year, int reachMatrixCount)
        {
            maxik = GetRealDensity(maxik, year, "Data");
            double SYSIN = 0.0;

            int N = mTable["Data"].Rows;

            if (!mTable.ContainsKey("Dependency"))
                return 0.0;

            if (!zeroDiagonal)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += mTable["Dependency"][i, j];
                    }
                }
                SYSIN *= (1 - maxik * N);
                SYSIN /= maxik;
                SYSIN /= (1 - Math.Pow(maxik * N, reachMatrixCount));
            }
            else
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += mTable["Dependency"][i, j];
                    }
                    SYSIN -= mTable["Dependency"][i, i];
                }
                SYSIN *= (1 - maxik * (N - 1));
                SYSIN /= maxik * (N - 1);
                SYSIN /= (1 - Math.Pow(maxik * (N - 1), reachMatrixCount));
            }

            return SYSIN;
        }
           */

        private double calculate_NPOL(Matrices.Matrix M, List<clique> Cliques)
        {
            double npol;
            int total_sum = 0, col_sum;

            for (int i = 0; i < Cliques.Count; i++)
            {
                col_sum = 0;
                for (int j = 0; j < M.Rows; j++)
                {
                    if (Cliques[i][j] == 1)
                        col_sum++;
                }
                total_sum += col_sum * (M.Rows - col_sum);
            }

            double first_term;
            if (M.Rows % 2 == 0)
                first_term = 4.0 / (double)(Cliques.Count * M.Rows * M.Rows);
            else
                first_term = 4.0 / (double)(Cliques.Count * (M.Rows * M.Rows - 1));

            npol = first_term * total_sum;
            return npol;
        }


    }
}
