using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;

namespace NetworkGUI.Triads
{
    public class Triads
    {
        public enum TriadType
        {
            Balance, NonBalance
        }

        /* Constants */
        private const int MAX_TRIAD_SIZE = 3;
        private const int MAX_TRIAD_LIST_SIZE = 10000;
        private const int I = 0;
        private const int J = 1;
        private const int K = 2;

        private double R;
        private TriadType _type;

        private int colCount = 0; // member variable to hold column size of matrix
        private int numNodes = 0;
        /* 
         * Triple array that contains the triads
         * The first dimension keeps track of which node in the group
         * The second dimension keeps track of all the triads in the corresponding node
         * The third dimension keeps track of each node in the triad
         */
        private int[][][] triadList; 

        //private int[][] triadListValue; // ???
        private int[] triadListCount; // a parallel array to triadList to keep track of how many triads in node
        private int[] diadCount; // an array that stores the number of diads for each node
        private double[] localTransitivity; // an array that stores the local transitivity for each node
        private double[] localBalance; // an array that stores the local balance for each node; only used for balance triads

        private Matrix transitiveTriads;

        // newly added arrays
        private int[] degrees;
        private List<int>[] triangleList;
        private int[] relevantTriads;
        private double[] localClosure;
        private int[] closureTriangles;

        private double[] tempCount;

        private int _count; // number of different triad nodes

        public Triads(Matrix matrix, TriadType type, double binaryCutoff = 0.0)
        {
            _type = type;
            R = binaryCutoff;
            colCount = matrix.Rows;
            numNodes = matrix.Rows;
            _count = numNodes;
            triadList = new int[colCount][][];
            //triadListValue = new int[colCount][];
            triadListCount = new int[colCount];

            transitiveTriads = new Matrix(numNodes);
            triangleList = new List<int>[numNodes]; // dyads to the node: 1 to 1 relationship

            // temp
            tempCount = new double[numNodes];


            degrees = new int[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                triadList[i] = new int[MAX_TRIAD_LIST_SIZE][];
                //triadListValue[i] = new int[MAX_TRIAD_LIST_SIZE];
                triadListCount[i] = 0;

                // create the triads list for each node
                triangleList[i] = new List<int>();
                for (int j = 0; j < numNodes; j++)
                {
                    if (matrix[i, j] > R)
                    {
                        transitiveTriads[i, j] = 1;
                        triangleList[i].Add(j);
                        // calculate the degree of each node
                        degrees[i]++;
                    }
                }
            }

            calcClosedTriangles();
            calcRelevantTriangles();
            newCalc(matrix);
            for (int i = 0; i < colCount; i++)
            {
                int triadCount = 0; // counter of how many nodes have been accumulated in the triad so far
                int[] list = new int[MAX_TRIAD_SIZE]; // a dynamic array used to contain the nodes in a triad
                bool[] markedList = new bool[colCount]; // an array to check which nodes have been used
                for (int j = 0; j < colCount; j++)
                    markedList[j] = false;
                list[triadCount++] = i; // add the first node to the triad
                markedList[i] = true;
                computeTriads(matrix, ref triadCount, list, i, ref markedList, ref triadList[i],
                    ref triadListCount[i]);
                
            }

            computeDiads(matrix);
            //computeLocalTransitivity();
            if (_type == TriadType.Balance)
                computeLocalBalance(matrix);
            
        }

        public bool verify(int first, int second, int last)
        {
            if (first == last)
                return false;

            if (triangleList[first].Contains(last))
                return true;
            else
                return false;
        }

        public Matrix calcDyadicTransitivity(Matrix s)
        {
            // initialize the dt matrix with s
            Matrix dt = new Matrix(s.Rows, s.Cols);
            dt.CopyLabelsFrom(s);
            dt.NetworkId = s.NetworkId;

            for (int i = 0; i < s.Rows; i++)
            {
                for (int j = 0; j < s.Cols; j++)
                {
                    if (i == j)
                    {
                        dt[i, j] = 0;
                        continue;
                    }
                    if (s[i, j] == 0)
                        dt[i, j] = -9;
                    else
                    {
                        int totalTriangles = 0;
                        int triangleCount = 0;
                        int secondNode = j;
                        for (int k = 0; k < triangleList[secondNode].Count; k++)
                        {
                            int lastNode = triangleList[secondNode][k];                            
                            
                            // check if last node is same as first
                            if (lastNode != i)
                            {
                                if (triangleList[i].Contains(lastNode))
                                {
                                    triangleCount++; // number of actual triangles that contribute to t_i_jk
                                }
                                totalTriangles++;
                            }
                        }
                        dt[i, j] = (double)triangleCount / (double)totalTriangles;
                    }
                }
            }

            return dt;
        }


        private List<Triad> triads;

        // helper function to calculate the Triads
        private void newerCalc(Matrix s) // as of 12/18/12
        {
            triads = new List<Triad>();
            for (int i = 0; i < _count; i++)
            {
                // create and initialize a marked list to keep track of
                // which nodes have already been computed
                List<int>[] markedList = new List<int>[_count];
                for (int j = 0; j < _count; j++)
                    markedList[j] = new List<int>();

                int totalTriangles = 0;
                Triad tri = new Triad(i);

                for (int j = 0; j < triangleList[i].Count; j++)
                {
                    // assign secondNode to the current jth node in the triangleList
                    int secondNode = triangleList[i][j];
                    for (int k = 0; k < triangleList[secondNode].Count; k++)
                    {
                        int lastNode = triangleList[secondNode][k];
                        // need to check if i == j and if triad is a multiple
                        if (lastNode != i && !(markedList[lastNode].Contains(secondNode)))
                        {
                            if (triangleList[i].Contains(lastNode))
                            {
                                // triad is found so corresponding dyad nodes are added
                                // to the current triad
                                tri.AddDyad(secondNode, lastNode);

                                // mark the secondNode as having a triad consisting of
                                // the last node to avoid multiple triads
                                markedList[secondNode].Add(lastNode);
                                //triangleCount++; // number of actual triangles that contribute to t_i_jk
                            }
                            totalTriangles++;
                        }
                    }
                }
                localTransitivity[i] = (double)tri.Count / (double)totalTriangles;
                triads.Add(tri);
            }
        }

        public void calcRelevantTriangles()
        {    
            localTransitivity = new double[numNodes];
            relevantTriads = new int[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                List<int>[] markedList = new List<int>[numNodes];
                for (int j = 0; j < numNodes; j++)
                    markedList[j] = new List<int>();

                int totalTriangles = 0;
                int triangleCount = 0;
                for (int j = 0; j < triangleList[i].Count; j++)
                {
                    int secondNode = triangleList[i][j];
                    for (int k = 0; k < triangleList[secondNode].Count; k++)
                    {
                        int lastNode = triangleList[secondNode][k];

                        // need to check if i == j and if triad is a multiple
                        if (lastNode != i && !(markedList[lastNode].Contains(secondNode)))
                        {
                            if (triangleList[i].Contains(lastNode))
                            {
                                markedList[secondNode].Add(lastNode);
                                triangleCount++; // number of actual triangles that contribute to t_i_jk
                            }
                            totalTriangles++;
                        }
                    }
                }
                relevantTriads[i] = totalTriangles;
                //localTransitivity[i] = (double)triangleCount / (double)totalTriangles;
                localTransitivity[i] = (double)tempCount[i] / (double)((numNodes - 1) * (numNodes - 2)) / 2;//(double)(totalTriangles);
                
            }
        }
        // New algorithms for Triads
        public void newCalc(Matrix s)
        {
            //List<int>[] ti_jk = new List<int>[numNodes]; // transitive triads
            List<Matrix> ti_jk = new List<Matrix>();
            for (int i = 0; i < numNodes; i++)
            {
                Matrix jk = new Matrix(numNodes);
                for (int j = 0; j < numNodes; j++)
                {
                    if (j == i) // a triad cannot contain two of the same nodes
                        continue;
                    for (int k = 0; k < numNodes; k++)
                    {
                        if (k == i || k == j) // a triad cannot contain two of the same nodes
                            continue;
                        double[] temp = { s[i, j], s[j, k], s[i, k] };
                        if (s[i, j] > R && s[j, k] > R && s[i, k] > R && Network.Algorithms.MinValue(temp) > R)
                        {
                            jk[j, k] = 1;
                        }
                    }
                }
                ti_jk.Add(jk);
            }

            double[] lt = new double[numNodes];
            // calculate sums temporarily
            for (int i = 0; i < numNodes; i++)
            {
                double sum = 0;
                for (int j = 0; j < ti_jk[i].Rows; j++)
                {
                    sum += ti_jk[i].GetRowSum(j);
                }
                //lt[i] = sum / relevantTriads[i];
                lt[i] = 2 * sum / (degrees[i] * (degrees[i] - 1));
            }

            Matrix dt = new Matrix(numNodes);
            for (int i = 0; i < numNodes; i++)
            {
                double sum = 0;
                for (int j = 0; j < ti_jk[i].Rows; j++)
                {
                    if (s[i, j] == 0)
                    {
                        dt[i, j] = -9;
                    }
                    else if (s[i, j] > 0)
                    {
                        for (int k = 0; k < ti_jk[i].Cols; k++)
                        {
                            if (s[j, k] > 0)
                                sum += ti_jk[i][j, k];
                            
                        }
                    }
                }
            }
        }


        public void calcClosedTriangles()
        {  
            localClosure = new double[numNodes];
            closureTriangles = new int[numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                int totalTriangles = 0;
                int triangleCount = 0;
                for (int j = 0; j < triangleList[i].Count; j++)
                {
                    int secondNode = triangleList[i][j];
                    for (int k = j + 1; k < triangleList[i].Count; k++)
                    {
                        int lastNode = triangleList[i][k];
                        if (triangleList[secondNode].Contains(lastNode))
                        {
                            triangleCount++;
                        }
                        totalTriangles++;
                    } 
                }
                closureTriangles[i] = totalTriangles;
                localClosure[i] = (double)triangleCount / (double)(totalTriangles);
                
                tempCount[i] = triangleCount;
            }
        }

        public void computeTriads(Matrix matrix, ref int triadCount, int[] list,
            int row, ref bool[] markedList, ref int[][] triad, ref int count)
        {
            if (triadCount >= MAX_TRIAD_SIZE) // greater than 2 since index started with 0
            {
                // need to make a new copy of list
                int[] tempList = new int[MAX_TRIAD_SIZE];
                for (int i = 0; i < MAX_TRIAD_SIZE; i++)
                {
                    tempList[i] = list[i];
                }

                // check to see if combination of the triad already exists
                bool equal = false;
                if (count > 0) // needs to be at least one element in triad
                {
                    for (int i = 0; i < count; i++)
                    {
                        int j;
                        for (j = 0; j < triadCount; j++)
                        {
                            int k;
                            for (k = 0; k < triadCount; k++)
                            {
                                if (tempList[k] == triad[i][j]) // element equal to triad[i] element
                                    break;
                            }
                            if (k == triadCount) // no element equal to triad[i] element
                            {
                                equal = false;
                                break;
                            }
                        }
                        if (j == triadCount) // list is equal to at least one triad
                        {
                            equal = true;
                            break;
                        }
                    }
                }
                if (!equal)
                    triad[count++] = tempList;

                triadCount--;
                markedList[row] = false;
                return;
            }

            for (int j = 0; j < colCount; j++)
            {
                if (markedList[j] == true)
                    continue;
                bool comparison_exp;
                if (_type == TriadType.NonBalance)
                    comparison_exp = (matrix[row, j] > R);
                else // _type == TriadType.Balance
                    comparison_exp = (matrix[row, j] != 0);
                //if (matrix[row, j] > R)
                if (comparison_exp)
                {
                    list[triadCount++] = j;
                    markedList[j] = true;
                    computeTriads(matrix, ref triadCount, list, j, ref markedList, ref triad, ref count);
                }

            }
            markedList[row] = false;
            triadCount--;
        }
        
        public void computeDiads(Matrix matrix)
        {
            diadCount = new int[colCount];
            for (int i = 0; i < colCount; i++)
            {
                diadCount[i] = 0;
            }
            
            for (int row = 0; row < colCount; row++)
            {
                for (int numTriads = 0; numTriads < triadListCount[row]; numTriads++)
                {
                    int i = triadList[row][numTriads][I];
                    int j = triadList[row][numTriads][J];
                    int k = triadList[row][numTriads][K];
                    if ((i == j) || (j == k) || (i == k))
                        continue;

                    bool comparison_exp;
                    if (_type == TriadType.NonBalance)
                        comparison_exp = (matrix[i, j] > R) && (matrix[j, k] > R) && (matrix[i, k] > R);
                    else
                        comparison_exp = (matrix[i, j] != 0) && (matrix[j, k] != 0) && (matrix[i, k] != 0);
                    //if ((matrix[i, j] > R) && (matrix[j, k] > R) && (matrix[i, k] > R))
                    if (comparison_exp)
                    {
                        //triadListValue[row][numTriads] = 1;
                        diadCount[row]++;
                    }
                    /*
                    else
                    {
                        //triadListValue[row][numTriads] = 0;
                    }
                    */
                }
            }
        }        

        /* This function is only used by triads that are balanced */
        public void computeLocalBalance(Matrix matrix)
        {
            localBalance = new double[colCount];
            for (int i = 0; i < colCount; i++)
            {
                double balanceTriads = 0.0;
                for (int j = 0; j < triadListCount[i]; j++)
                {   
                    int I = triadList[i][j][0];
                    int J = triadList[i][j][1];
                    int K = triadList[i][j][2];
                    /*
                    if ((I == -2) || (J == -2) || (K == -2))
                    {
                        localBalance[i] = 0.0;
                        break;
                    }
                    */
                    // count how many negative diads
                    double negativeCount = 0.0;
                    if (matrix[I, J] == -1)
                        negativeCount++;
                    if (matrix[J, K] == -1)
                        negativeCount++;
                    if (matrix[I, K] == -1)
                        negativeCount++;

                    if (negativeCount == 0 || negativeCount == 2) // if positive
                        balanceTriads++;
                    
                }
                if (triadListCount[i] == 0)
                    localBalance[i] = 0;
                else
                    localBalance[i] = balanceTriads / triadListCount[i];
            }
        }

        public void computeLocalTransitivity()
        {
            localTransitivity = new double[colCount];
            for (int i = 0; i < colCount; i++)
            {
                if (triadListCount[i] == 0)
                    localTransitivity[i] = 0;
                else
                    localTransitivity[i] = (double)diadCount[i] / (double)triadListCount[i];//((numNodes - 1) * (numNodes - 2))/2;
            }
        }

        /* Standard Getter functions */
        public int getTriadListCount(int group)
        {
            return triadListCount[group];
        }

        public int getDiadCount(int group)
        {
            return diadCount[group];
        }

        public double getLocalTransitivity(int node)
        {
            return localTransitivity[node];
        }

        public double getLocalBalance(int group)
        {
            return localBalance[group];
        }

        public int getDegree(int node)
        {
            return degrees[node];
        }

        public int getClosureTriangle(int node)
        {
            return closureTriangles[node];
        }

        public double getLocalClosure(int node)
        {
            return localClosure[node];
        }

        public int getRelevantTriad(int node)
        {
            return relevantTriads[node];
        }

        /* C# Getter functions */
        public double Cutoff
        {
            get { return R; }
        }

        public int ColCount
        {
            get { return colCount; }
        }

        public TriadType Type
        {
            get { return _type; }
        }

        public int[][] this[int i]
        {
            get { return triadList[i]; }
        }

        public Matrix TransitiveMatrix
        {
            get { return transitiveTriads; }
        }
    }
}
