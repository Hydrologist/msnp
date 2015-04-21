using System;
using System.Collections.Generic;
using System.Text;
using Network.Cliques;
using Network.Matrices;
using Network.IO;
using System.IO;
using System.Windows.Forms;

namespace Network
{
    public class clique
    {
        private List<byte> data;
        public int CompareTo(clique rhs)
        {
            if (rhs.size - size != 0)
                return rhs.size - size;
            else
                return rhs.num_networks - num_networks;
        }
        public short num_networks;
        public clique()
        {
        }
        public clique(int[] L)
        {
            num_networks=1;
            data=new List<byte>(L.Length);
            for(int i=0; i<L.Length; i++)
            {
                data.Add((byte)L[i]);
                size += L[i];
            }
        }
        public clique(Clique L)
        {
            num_networks = 1;
            data = new List<byte>(L.MemberRangeSize);
            for (int i = 0; i < L.MemberRangeSize; i++)
            {
                data.Add((byte)(L.Contains(i) ? 1 : 0));
                //size += L[i];
                size += (L.Contains(i) ? 1 : 0);
            }
        }
        private int size = 0;

        public int num_elements()
        {
            return size;
        }

        public bool is_subset(clique rhs)
        {
            for (int i = 0; i < rhs.Count; i++)
            {
                if (rhs[i] > data[i])
                    return false;
            }
            return true;
        }

        internal void find_num_networks(List<List<clique>> cliqueList)
        {
            int ans=0;
            foreach (List<clique> list in cliqueList)
            {
                foreach (clique C in list)
                {
                    if (is_subset(C))
                    {
                        ans++;
                        break;
                    }
                }
            }
            num_networks = (short)ans;
        }

        public Matrices.Vector ToVector()
        //generate a vector, assuming that the last element of this array is invalid
        {
            Matrices.Vector ans = new global::Network.Matrices.Vector(Count - 1);
            for (int i = 0; i < Count - 1; i++)
            {
                ans[i] = this[i];
            }
            return ans;
        }

        static public Matrices.Matrix ToMatrix(List<clique> list)
        {
            Matrices.Matrix answer = new global::Network.Matrices.Matrix(list.Count, list[0].Count - 1);
            for (int c = 0; c < list[0].Count - 1; c++)
            {
                for (int r = 0; r < list.Count; r++)
                    answer[r, c] = list[r][c];
            }
            return answer;
        }

        static public List<clique> convertClique(CliqueCollection list)
        {
            List<clique> temp = new List<clique>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                temp.Add(new clique(list[i]));
            }
            return temp;
        }


        public int this[int i]
        {
            get
            {
                return data[i];
            }
        }

        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        #region Clique Transformation Functions
        static public Matrices.Matrix GenerateAffiliationMatrixTemp(List<clique> list)
        {

            Matrices.Matrix result = new global::Network.Matrices.Matrix(list[0].Count, list.Count); //The number of elements in each clique is inflated by one, so it works out fine
            for (int c = 0; c < list.Count; c++)
            {
                for (int r = 0; r < list[0].Count; r++)
                {
                    result[r, c] = list[c][r];
                }
            }

            for (int i = 1; i <= list[0].Count; i++)
            {
                result.RowLabels[i - 1] = i.ToString();
            }

            for (int i = 1; i <= list.Count; i++)
            {
                result.ColLabels[i - 1] = i.ToString();
            }
            return result;
        }
        
        static public Matrices.Matrix GenerateAffiliationMatrix(List<clique> list)
        {

            Matrices.Matrix result = new global::Network.Matrices.Matrix(list[0].Count+1, list.Count); //The number of elements in each clique is inflated by one, so it works out fine
            for (int c = 0; c < list.Count; c++)
            {
                result[0, c] = list[c].num_networks;
                for (int r = 0; r < list[0].Count; r++)
                {
                    result[r + 1, c] = list[c][r];
                }
            }

            result.RowLabels[0] = "#networks";
            for (int i = 1; i <= list[0].Count; i++)
            {
                result.RowLabels[i] = i.ToString();
            }

            for (int i = 1; i <= list.Count; i++)
            {
                result.ColLabels[i-1] = i.ToString();
            }
            return result;
        }

        internal static global::Network.Matrices.Matrix GenerateCharacteristicsMatrix(List<clique> cliques)
        {
            Matrices.Matrix result = new global::Network.Matrices.Matrix(cliques.Count,4);
            for (int i = 0; i < cliques.Count; i++)
            {
                result[i, 0] = i + 1;
                result[i, 1] = 1820;
                result[i, 2] = cliques[i].num_elements();
                result[i, 3] = cliques[i].num_networks;
                result.RowLabels[i] = (i + 1).ToString();
            }
            result.ColLabels[0] = "Clique Number";
            result.ColLabels[1] = "Year";
            result.ColLabels[2] = "No. Members";
            result.ColLabels[3] = "#Networks";
            return result;
        }

         





        public static Matrices.Matrix GenerateOverLapTable(List<clique> list) // problematic
        {
            Matrices.Matrix result = new global::Network.Matrices.Matrix(list[0].Count - 1, list[0].Count - 1);
            foreach (clique C in list)
            {
                for (int i = 0; i < list[0].Count - 1; i++)
                {
                    if (C[i] == 1)
                    {
                        result[i, i]++;

                        for (int j = i + 1; j < list[0].Count - 1; j++)
                        {
                            if (C[j] == 1)
                            {
                                result[i, j]++;
                                result[j, i]++;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < result.Cols; i++)
            {
                result.ColLabels[i] = (i + 1).ToString();
                result.RowLabels[i] = (i + 1).ToString();
            }
            return result;
        }

        static public Matrices.Matrix GenerateWeightedAffiliationMatrixTemp(List<clique> list)
        {

            Matrices.Matrix result = new global::Network.Matrices.Matrix(list[0].Count, list.Count); //The number of elements in each clique is inflated by one, so it works out fine
            for (int c = 0; c < list.Count; c++)
            {
                //result[0, c] = list[c].num_networks;
                for (int r = 0; r < list[0].Count; r++)
                {
                    result[r, c] = list[c][r] * list[c].num_networks;
                }
            }

            for (int i = 1; i <= list[0].Count; i++)
            {
                result.RowLabels[i - 1] = i.ToString();
            }

            for (int i = 1; i <= list.Count; i++)
            {
                result.ColLabels[i - 1] = i.ToString();
            }
            return result;
        }

        static public Matrices.Matrix GenerateWeightedAffiliationMatrix(List<clique> list)
        {

            Matrices.Matrix result = new global::Network.Matrices.Matrix(list[0].Count-1, list.Count); //The number of elements in each clique is inflated by one, so it works out fine
            for (int c = 0; c < list.Count; c++)
            {
                result[0, c] = list[c].num_networks;
                for (int r = 0; r < list[0].Count - 1; r++)
                {
                    result[r, c] = list[c][r]*list[c].num_networks;
                }
            }

            for (int i = 1; i < list[0].Count; i++)
            {
                result.RowLabels[i-1] = i.ToString();
            }

            for (int i = 1; i <= list.Count; i++)
            {
                result.ColLabels[i - 1] = i.ToString();
            }
            return result;
        }

        internal static global::Network.Matrices.Matrix GenerateWeightedCOCMatrix(List<clique> cliques, global::Network.Matrices.Matrix matrix)
        {
            Matrices.Matrix result = new Matrices.Matrix(matrix);
            for (int c = 0; c < matrix.Cols; c++)
            {
                for (int r = 0; r < matrix.Rows; r++)
                {
                    result[r, c] *= cliques[c].num_networks;
                }
            }
            return result;
        }
        #endregion
    }
}
