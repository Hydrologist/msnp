using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using CH.Combinations;
using Network.Matrices;
using Network.IO;
using Network.Cliques;
using Network.Blocks;
using Network.Communities;
using System.Threading;
using RandomUtilities;
using Community;
using NetworkGUI.Sets;
using NetworkGUI.Triads;
using NetworkGUI;
using DotNumerics.FortranLibrary;
using DotNumerics.LinearAlgebra.CSLapack;
using NetworkGUI.Forms;



namespace Network
{
    public enum CliqueExtractionType
    {
        Max,
        Min,
        Upper,
        Lower
    }

    public enum CommunityType
    {
        Affil,
        newAffil,
        ovAffil,
        Density,
        newDensity,
        ovDensity,
        RelativeDensity,
        newRelativeDensity,
        ovRelativeDensity,
        Cohesion,
        newCohesion,
        ovCohesion,
        Char,
        newChar,
        ovChar,
        Cluster,
        newCluster,
        ovCluster,
        Coefficients,
        Separation,
        newCoefficients,
        ovCoefficients
        
    }

    public enum ElementwiseFormat { Matrix, Dyadic, Monadic };

    //public struct tuple
    //{
    //    double m;					// stored value
    //    int i;					// row index
    //    int j;					// column index
    //    int k;					// heap index
    //};

    public class Network
    {
        protected List<List<int>> Blocks;
        protected List<int[]> communities;
        protected List<int[]> maxSeparationcommunities;
        protected int comNum; // number of communities
        protected List<List<int>> comList;
        protected bool firstCommunityIteration;

        protected List<double> comm_SC_list;
        protected List<double> comm_GC_list;

        protected List<double> maxcomm_SC_list;
        protected List<double> maxcomm_GC_list;

        protected List<double> comm_SE_den_list;
        protected List<double> comm_SE_coh_list;
        protected List<double> comm_T_den_list;
        protected List<double> comm_T_coh_list;

        protected List<double> maxcomm_SE_den_list;
        protected List<double> maxcomm_SE_coh_list;
        protected List<double> maxcomm_T_den_list;
        protected List<double> maxcomm_T_coh_list;
        protected List<double> maxcomm_null_SC_list;
        protected List<double> maxcomm_null_GC_list;
        protected List<double> maxcomm_T_SC_list;
        protected List<double> maxcomm_T_GC_list;

        protected List<double> comm_null_SC_list;
        protected List<double> comm_null_GC_list;
        protected List<double> comm_T_SC_list;
        protected List<double> comm_T_GC_list;
        public double optionsDensity;

        protected bool firstModWrite;

        protected int maxSepComNum;
        protected double maxSC = -1;

        protected int comSize; // number of nodes in a community
        // newly added matrices
        protected Matrix commAffil = null;
        protected Matrix commDensity = null;

        protected int cliqueSize;
        protected Matrix cliqueDensity = null;

        protected string cohesionFilename = null;
        protected Matrix _cohesionMatrix = null;

        protected int blockSize;

        protected Matrix SESEmatrix = null;

        protected List<Clique> overlapComm = null;

        protected bool isDichotomized;
        protected double dichotomizedCutoff;

        protected bool isRecoded;
        protected List<RecodeForm.Row> recodeRangeList = null;

        //
        // these matrices are used to maintain a copy of the actual matrices
        // so that the originals are not deleted when clearPreviousData() is 
        // called, which makes a new instance of mTable
        //
        protected Matrix tempDataMatrix = null;
        protected Matrix tempDataEventMatrix = null;
        protected Matrix tempAffilCorrelationMatrix = null;
        protected Matrix tempAffilCorrelationEventMatrix = null;
        protected Matrix tempAffilEuclideanMatrix = null;
        protected Matrix tempAffilEuclideanEventMatrix = null;
        protected Matrix tempAffilMatrix = null;

        //
        // List of matrices for multiple matrix files
        // newly added (4/26/13)
        //
        protected List<Matrix> multipleMatrixList;

        protected Matrix blocOverlap = null;
        public Matrix commOverlap = null;
        protected string prevTransitivityType = "";
        protected List<double> NPOLA_Sums;
        protected CliqueCollection _cliques = null;
        protected BlockCollection _blocks = null;
        protected Matrix _blockDensity = null;
        protected CommCollection _communities = null;
        protected CliqueCollection comcliques = null;
        protected BlockCollection comblocks = null;
        protected CommCollection comComm = null;

        protected const int cocCutoff = 5000; // Max K to use for FULL COC matrix

        protected enum StdValues { None, Row, Column, Diagonal };
        protected StdValues Standardization;

        public MatrixTable mTable = null;
        public List<Matrix> mList = null;
        public Community.Community com;

        public Random RNGen = null;

        protected Dictionary<int, double> densityVector;
        protected string densityVectorFile = "";

        protected Dictionary<int, int> reachNumVector;
        protected string reachNumVectorFile = "";

        protected Dictionary<int, double> viableCoalitionVector;
        protected string viableCoalitionVectorFile = "";

        protected Dictionary<int, int> cliqueMinVector;
        protected string cliqueMinVectorFile = "";

        protected Dictionary<int, int> kCliqueVector;
        protected string kCliqueVectorFile = "";

        public List<int> mcayear = new List<int>();
        public List<int> mcanetwork = new List<int>();
        public List<double> mcaweight = new List<double>();
        public string weightVectorFile = "";
        public List<int> npnetwork = new List<int>();
        public List<int> npnode = new List<int>();
        public List<double> npattribute = new List<double>();
        public string attributeVectorFile = "";

        public List<Matrix> data;

        public int _minCliqueSize;

        public CliqueExtractionType cet;

        #region Variables used to store calculations

        protected double npol = -1.0;
        protected double coi = -1.0;
        protected double density = -1.0;
        protected double transitivity = -1.0;
        protected double averageCliqueSize = -1.0;
        protected double averageCliqueMembers = -1.0;
        protected double coc = -1.0;
        protected double npolstar = -1.0;

        protected int ReachabilityN = -1;

        #endregion

        public Network()
        {
            mTable = new MatrixTable();
            RNGen = new Random();
            _cliques = null;

            _minCliqueSize = 1;

            mTable["Data"] = null;
            cet = CliqueExtractionType.Max;
        }


        public void Initialize(int rows, int cols)
        {
            // This function should be used with the default constructor above.
            // It will initialize all the same things
            // It should also be used when loading a new matrix

            mTable["Data"] = new Matrix(rows, cols);

            npol = coi = density = transitivity = averageCliqueMembers = averageCliqueSize = coc = npolstar = -1.0;
            ReachabilityN = -1;

            cet = CliqueExtractionType.Max;
        }

        public void Clear()
        {
            // Reset all the values
            mTable.Clear();

            // Clear all the cache variables
            npol = coi = density = transitivity = averageCliqueSize = averageCliqueMembers = npolstar = coc = -1.0;
        }

        public Matrix GetMatrix(string name)
        {
            return mTable[name];
        }

        public bool IsDichotomized
        {
            get { return isDichotomized; }
            set { isDichotomized = value; }
        }

        public double DichotomizedCutoff
        {
            get { return dichotomizedCutoff; }
            set { dichotomizedCutoff = value; }
        }

        public void setDictomoization(bool usage, double value)
        {
            isDichotomized = usage;
            dichotomizedCutoff = value;
        }

        public bool IsRecoded
        {
            get { return isRecoded; }
            set { isRecoded = value; }
        }

        public List<RecodeForm.Row> RecodeRangeList
        {
            get { return recodeRangeList; }
            set { recodeRangeList = value; }
        }

        public void setRecode(bool usage, List<RecodeForm.Row> rangeList)
        {
            isRecoded = usage;
            recodeRangeList = rangeList;
        }

        public int CliqueCount
        {
            get
            {
                return _cliques.Count;
            }
        }

        public int BlockCount
        {
            get
            {
                return _blocks.Count;
            }
        }

        public int CommCount
        {
            get
            {
                return comNum;
            }
        }

        public string CohesionFilename
        {
            get { return cohesionFilename; }
            set { cohesionFilename = value; }
        }

        public Matrix CohesionMatrix
        {
            get { return _cohesionMatrix; }
            set { _cohesionMatrix = value; }
        }

        // Calculations 
        public double AverageCliqueSize
        {
            get { return CliqueCalculations.AverageCliqueSize(_cliques); }
        }
        public double AverageBlockSize
        {
            get { return BlockCalculations.AverageBlockSize(_blocks); }
        }
        public double AverageCommSize
        {
            get { return CommCalculations.AverageBlockSize(_communities); }
        }
        public double AverageCliqueMembers
        {
            get { return CliqueCalculations.AverageCliqueMembers(_cliques); }
        }
        public double AverageBlockMembers
        {
            get { return BlockCalculations.AverageBlockMembers(_blocks); }
        }
        public double AverageCommMembers
        {
            get { return CommCalculations.AverageCommMembers(_communities); }
        }
        public double cNPOL
        {
            get { return CliqueCalculations.NPOL(_cliques, mTable["Data"]); }
        }

        public double bNPOL
        {
            get { return BlockCalculations.NPOL(_blocks, mTable["Data"]); }
        }

        // modified to accomodate new communities
        public double comNPOL
        {
            get
            {
                // old code
                //return CommCalculations.NPOL(_communities, mTable["Data"]); 

                // new code
                int total_sum = 0;
                for (int i = 0; i < comNum; i++)
                {
                    int Size = 0;
                    for (int j = 0; j < comSize; j++)
                        Size += communities[i][j];
                    total_sum += (Size * (comSize - Size));
                }
                double first_term = 4.0 * total_sum;
                double divisor = (double)(comNum * comSize * comSize);
                return first_term / divisor;
            }
        }

        public double ViableNPOL
        {
            get
            {
                int total_sum = 0, col_sum;

                for (int i = 0; i < mTable["ViableCoalitions"].Cols; i++)
                {
                    col_sum = 0;
                    for (int j = 0; j < mTable["Data"].Rows; j++)
                    {
                        if (mTable["ViableCoalitions"][j, i] == 1)
                            col_sum++;
                    }
                    total_sum += col_sum * (mTable["Data"].Rows - col_sum);
                }

                double first_term;
                if (mTable["Data"].Rows % 2 == 0)
                    first_term = 4.0 / (double)(mTable["ViableCoalitions"].Cols * mTable["Data"].Rows * mTable["Data"].Rows);
                else
                    first_term = 4.0 / (double)(mTable["ViableCoalitions"].Cols * (mTable["Data"].Rows * mTable["Data"].Rows - 1));

                npol = first_term * total_sum;
                return npol;
            }
        }

        public double cliqueGMOI
        {
            get
            {
                Matrix GA = new Matrix(mTable["Data"].Rows, CliqueCount);
                // create the GA matrix from the cliques
                for (int i = 0; i < CliqueCount; i++)
                {
                    for (int j = 0; j < mTable["Data"].Rows; j++)
                    {
                        GA[j, i] = _cliques[i].IntContains(j);
                    }
                }
                Matrix GMO = GA * GA.GetTranspose();
                double sum = 0.0;
                for (int i = 0; i < GMO.Rows; i++)
                {
                    for (int j = 0; j < GMO.Cols; j++)
                    {
                        sum += GMO[i, j] / GMO[j, j];
                    }
                }
                return sum / (GMO.Rows * (GMO.Rows - 1));
            }
        }

        public double blockGMOI
        {
            get
            {
                Matrix GA = new Matrix(mTable["Data"].Rows, BlockCount);
                // create the GA matrix from the blocks
                for (int i = 0; i < BlockCount; i++)
                {
                    for (int j = 0; j < mTable["Data"].Rows; j++)
                    {
                        GA[j, i] = _blocks[i].IntContains(j);
                    }
                }
                Matrix GMO = GA * GA.GetTranspose();
                double sum = 0.0;
                for (int i = 0; i < GMO.Rows; i++)
                {
                    for (int j = 0; j < GMO.Cols; j++)
                    {
                        sum += GMO[i, j] / GMO[j, j];
                    }
                }
                return sum / (GMO.Rows * (GMO.Rows - 1));
            }
        }

        public double comGMOI
        {
            get
            {
                Matrix GA = new Matrix(mTable["Data"].Rows, comNum);
                // create the GA matrix from the communities
                for (int i = 0; i < comNum; i++)
                {
                    for (int j = 0; j < mTable["Data"].Rows; j++)
                    {
                        GA[j, i] = communities[i][j];
                    }
                }
                Matrix GMO = GA * GA.GetTranspose();
                double sum = 0.0;
                for (int i = 0; i < GMO.Rows; i++)
                {
                    for (int j = 0; j < GMO.Cols; j++)
                    {
                        sum += GMO[i, j] / GMO[j, j];
                    }
                }
                return sum / (GMO.Rows * (GMO.Rows - 1));
            }
        }

        public double COI
        {
            get
            {
                // old code; possibly wrong
                //return CliqueCalculations.CMOI(_cliques);

                // new code
                /*if (cliqueDensity == null)
                    cliqueDensity = CalculateCliqueDensity(_cliques);
                double sum = 0;
                for (int i = 0; i < cliqueDensity.Rows; i++)
                {
                    for (int j = 0; j < cliqueDensity.Rows; j++)
                    {
                        if (j == i)
                            continue;
                        sum += cliqueDensity[i, j];
                    }
                }
                sum /= cliqueDensity.Rows * (cliqueDensity.Rows - 1);
                return sum; */

                double separation = CalculateCliqueSC(_cliques);
                if (separation >= 0)
                    return separation;
                else
                    return 0;
            }
        }

        public double bCOI
        {
            get
            {
                // old code; possibly wrong
                //return BlockCalculations.CMOI(_blocks);
                double separation = CalculateBlockSC(Blocks);
                if (separation >= 0)
                    return separation;
                else
                    return 0;
                /*
                double sum = 0;
                for (int i = 0; i < _blockDensity.Rows; i++)
                {
                    for (int j = 0; j < _blockDensity.Rows; j++)
                    {
                        if (j == i)
                            continue;
                        sum += _blockDensity[i, j];
                    }
                }
                sum /= _blockDensity.Rows * (_blockDensity.Rows - 1);
                return sum;*/

            }
        }

        public double comCOI
        {
            get
            {
                // old code; possibly wrong
                //return CommCalculations.CMOI(_communities); 

                // new code
                commDensity = CalculateCommDensity(communities);
                double separation = CalculateCommSC(communities);
                if (separation >= 0)
                    return separation;
                else
                    return 0;
                /*
                double sum = 0;
                for (int i = 0; i < commDensity.Rows; i++)
                {
                    for (int j = 0; j < commDensity.Rows; j++)
                    {
                        if (j == i)
                            continue;
                        sum += commDensity[i, j];
                    }
                }
                sum /= commDensity.Rows * (commDensity.Rows - 1);
                return sum;
                 */
                //return CommCalculations.ComGOI(commDensity);
            }
        }
        public double ViableCOI
        {
            get
            {
                double total_sum = 0;
                Matrix TempO = new Matrix(mTable["Overlap"].Rows);
                TempO.Clear();
                for (int v = 0; v < mTable["ViableCoalitions"].Cols; ++v)
                {
                    for (int i = 0; i < mTable["Overlap"].Rows; ++i)
                    {
                        for (int j = 0; j < mTable["Overlap"].Rows; ++j)
                        {
                            if (mTable["ViableCoalitions"][i, v] == 0 || mTable["ViableCoalitions"][j, v] == 0)
                                continue;
                            ++TempO[i, j];
                        }
                    }
                }
                for (int i = 0; i < TempO.Rows; i++)
                {
                    for (int j = i + 1; j < TempO.Rows; j++)
                    {
                        if (TempO[j, j] == 0)
                            continue;
                        total_sum += (double)(TempO[i, j]) / TempO[j, j];
                    }
                }

                coi = (double)(TempO.Rows * TempO.Rows - TempO.Rows);
                if (coi != 0)
                    coi = (total_sum * 2.0) / coi;
                return coi;
            }
        }
        public double NPI
        {
            get
            {
                return cNPOL * COI;
            }
        }

        public double bNPI
        {
            get
            {
                return bNPOL * bCOI;
            }
        }

        public double comNPI
        {
            get
            {
                return comNPOL * comCOI;//(1 - comGMOI);
            }
        }
        public double ViableNPI
        {
            get
            {
                if (mTable["ViableCoalitions"].Cols == 1 && mTable["ViableCoalitions"].GetColVector(0).IsZeroVector)
                    return 0.0;

                return ViableNPOL * (1 - ViableCOI);
            }
        }
        public int NumberOfCliques
        {
            get
            {
                return _cliques.Count;
            }
        }

        public Pair<double, double> CliqueOverlap
        {
            get { return CliqueCalculations.COC(_cliques); }
        }

        public Pair<double, double> BlocOverlap
        {
            get { return BlockCalculations.COC(_blocks); }
        }

        public Pair<double, double> CommOverlap
        {
            get { return CommCalculations.COC(_communities); }
        }



        /*
         * New implemented code to remove the matrix after it is loaded
         * into the viewing grid
         */

        public void ClearPreviousData(string m, string loadFrom)
        {
            if (m == "Data")
            {
                if (IsDichotomized)
                {
                    for (int i = 0; i < mTable["Data"].Rows; i++)
                    {
                        for (int j = 0; j < mTable["Data"].Rows; j++)
                        {
                            if (mTable["Data"][i, j] <= dichotomizedCutoff)
                                mTable["Data"][i, j] = 0;
                            else
                                mTable["Data"][i, j] = 1;
                        }
                    }
                }
                else if (isRecoded)
                {
                    for (int i = 0; i < mTable["Data"].Rows; i++)
                    {
                        for (int j = 0; j < mTable["Data"].Rows; j++)
                        {
                            double else_value = 0.0;
                            bool is_else_value = false;
                            bool else_value_assigned = false;
                            foreach (RecodeForm.Row row in recodeRangeList)
                            {
                                // check if the else value has already been assigned
                                if (!is_else_value)
                                {
                                    if (row.from == double.Epsilon)
                                    {
                                        else_value = row.value;
                                        is_else_value = true;
                                        break;
                                    }
                                }

                                if (mTable["Data"][i, j] >= row.from && mTable["Data"][i, j] < row.to)
                                {
                                    mTable["Data"][i, j] = row.value;
                                    else_value_assigned = false;
                                    break;
                                }
                                else
                                {
                                    else_value_assigned = true;
                                }
                            }
                            if (else_value_assigned)
                                mTable["Data"][i, j] = else_value;
                        }
                    }
                }
                else
                {
                    tempDataMatrix = mTable["Data"];
                    mTable = new MatrixTable();
                    mTable["Data"] = new Matrix(tempDataMatrix);
                    if (loadFrom == "Affil")
                    {
                        if (tempAffilMatrix != null)
                            mTable["Affil"] = new Matrix(tempAffilMatrix);
                        if (tempDataEventMatrix != null)
                            mTable["DataEvent"] = new Matrix(tempDataEventMatrix);
                        if (tempAffilCorrelationMatrix != null)
                            mTable["AffilCorrelation"] = new Matrix(tempAffilCorrelationMatrix);
                        if (tempAffilCorrelationEventMatrix != null)
                            mTable["AffilCorrelationEvent"] = new Matrix(tempAffilCorrelationEventMatrix);
                        if (tempAffilEuclideanMatrix != null)
                            mTable["AffilEuclidean"] = new Matrix(tempAffilEuclideanMatrix);
                        if (tempAffilEuclideanEventMatrix != null)
                            mTable["AffilEuclideanEvent"] = new Matrix(tempAffilEuclideanEventMatrix);
                    }
                }
            }
            else if (m == "Affil")
            {
                // do nothing
            }
            else
            {
                Matrix tempMatrix = new Matrix(mTable["Data"]);
                mTable = new MatrixTable();
                mTable["Data"] = tempMatrix;
                if (loadFrom == "Affil")
                {
                    if (tempAffilMatrix != null)
                        mTable["Affil"] = new Matrix(tempAffilMatrix);
                    if (tempDataEventMatrix != null)
                        mTable["DataEvent"] = new Matrix(tempDataEventMatrix);
                    if (tempAffilCorrelationMatrix != null)
                        mTable["AffilCorrelation"] = new Matrix(tempAffilCorrelationMatrix);
                    if (tempAffilCorrelationEventMatrix != null)
                        mTable["AffilCorrelationEvent"] = new Matrix(tempAffilCorrelationEventMatrix);
                    if (tempAffilEuclideanMatrix != null)
                        mTable["AffilEuclidean"] = new Matrix(tempAffilEuclideanMatrix);
                    if (tempAffilEuclideanEventMatrix != null)
                        mTable["AffilEuclideanEvent"] = new Matrix(tempAffilEuclideanEventMatrix);
                }

            }

            // May not need this part
            npol = coi = density = transitivity = averageCliqueMembers = averageCliqueSize = coc = npolstar = -1.0;
            ReachabilityN = -1;

            cet = CliqueExtractionType.Max;

            Blocks = null;
            communities = null;
            comList = null;
            comSize = 0;
            commAffil = null;
            commDensity = null;
            cliqueDensity = null;
            SESEmatrix = null;
            blocOverlap = null;
            commOverlap = null;
            _cliques = null;
            _blocks = null;
            _blockDensity = null;
            _communities = null;
            comcliques = null;
            comblocks = null;
            comComm = null;
            //System.IO.Directory.Delete(Constants.tempDir, true); // to delete recursively
        }

        /* 
         * New implemented code from the C code of
         * the community extraction program
         */

        /* Constants */

        const int NAME_LENGTH = 100;
        const int LINE_LENGTH = 100000;
        const int MAX_VERTICES = 25000;
        const bool TRUE = true;
        const bool FALSE = false;
        const double LARGE = 1000000000;
        const double EPSILON = 0.00001;

        const int CHECK_INT = 10;

        /* Types */

        // typedef char BOOLEAN;

        /* Globals */

        int nvertices;                               /* Size of graph */
        int nedges;                                       /* Number of edges */
        int[] indegree = new int[MAX_VERTICES];           /* In-degrees of vertices */
        int[] outdegree = new int[MAX_VERTICES];          /* Out-degrees of vertices */
        int[] outdegreeBuild = new int[MAX_VERTICES];     /* Out-degrees of vertices */
        int[][] list_edge;                                  /* Lists of edges */
        int[][] list_edgeTranspose = new int[MAX_VERTICES][];  /* Transpose lists of edges */
        //string label;                                     /* Names of vertices */

        double[] b;
        double[] u;
        protected int[] group;                                      /* Array of group indices for each vertex */
        int[] map;                                        /* Forward map for submatrices */
        int[] backmap;                                    /* Backward map for submatrices */
        int gsize;                                        /* Size of current group */

        int[] indiv;                                        /* Indivisible flags */
        double[] new_w;                                     /* Spectrum of eigenvalues */
        int ngroups;                                  /* Number of groups */
        double q;                                         /* Modularity */

        protected int q_index;
        protected double[] q_array;
        protected int q_size;

        protected double[] maxSepq_array;
        protected int maxSepq_size;

        public double calculateNullCoefficientFromNullModel(Matrix nullmodel, int groupCount, string type)
        {
            /*List<double> sumlist = new List<double>();
            for (int i = 0; i < nullmodel.Rows; i++)
            {
                sumlist.Add(nullmodel[i, i]);
            }*/
            double sum = 0;
            for (int i = 0; i < nullmodel.Rows; i++)
            {
                for (int j = 0; j < nullmodel.Cols; j++)
                {
                    sum += nullmodel[i, i] - nullmodel[i, j];
                }
            }
            double result;
            result = sum / (groupCount * (groupCount - 1));
            return result;
        }

        public List<double> calculateNullCoefficients(string type)
        {
            List<double> output = new List<double>();
            Matrix cohesionMatrix;
            if (CohesionMatrix != null)
            {
                cohesionMatrix = CohesionMatrix;
            }
            else
            {
                cohesionMatrix = MatrixComputations.StructuralEquivalenceStandardizedEuclidean(mTable["Data"], optionsDensity);
            }

            List<int> partitionedNodeList = new List<int>();
            int index = 0;
            int groupCount = 0;
            switch (type)
            {
                case "Communities":
                    partitionedNodeList.Capacity = mTable["Data"].Rows;
                    for (int i = 0; i < communities.Count; i++)
                    {
                        for (int j = 0; j < communities[i].Length; j++)
                        {
                            if (communities[i][j] == 1)
                            {
                                partitionedNodeList.Insert(index, j);
                                index += 1;
                            }
                        }
                    }
                    groupCount = communities.Count;
                    break;
                case "Cliques":
                    for (int i = 0; i < _cliques.Count; i++)
                    {
                        for (int j = 0; j < _cliques[i].Count; j++)
                        {
                            partitionedNodeList.Add(_cliques[i][j]);
                        }
                    }
                    groupCount = _cliques.Count;
                    break;
                case "Blocks":
                    for (int i = 0; i < Blocks.Count; i++)
                    {
                        for (int j = 0; j < Blocks[i].Count; j++)
                        {
                            partitionedNodeList.Add(Blocks[i][j]);
                        }
                    }
                    groupCount = Blocks.Count;
                    break;
            }
            Matrix partitioned = new Matrix(partitionedNodeList.Count, partitionedNodeList.Count);
            Matrix partitionedCohesion = new Matrix(partitionedNodeList.Count, partitionedNodeList.Count);
            for (int i = 0; i < partitionedNodeList.Count; i++)
            {
                for (int j = 0; j < partitionedNodeList.Count; j++)
                {
                    partitioned[i, j] = mTable["Data"][partitionedNodeList[i], partitionedNodeList[j]];
                    if (cohesionMatrix != null)
                    {
                        partitionedCohesion[i, j] = cohesionMatrix[partitionedNodeList[i], partitionedNodeList[j]];
                    }
                }
            }

            List<double> sepindegrees = new List<double>();
            List<double> sepoutdegrees = new List<double>();
            List<double> cohindegrees = new List<double>();
            List<double> cohoutdegrees = new List<double>();
            List<int> communitysizes = new List<int>();
            List<int> commcoords = new List<int>();

            switch (type)
            {
                case "Communities":
                    for (int i = 0; i < communities.Count; i++)
                    {
                        int sum = 0;
                        for (int j = 0; j < communities[i].Length; j++)
                        {
                            if (communities[i][j] == 1)
                                sum++;
                        }
                        communitysizes.Add(sum);
                    }
                    for (int i = 0; i < communities.Count; i++)
                    {
                        if (i > 0)
                        {
                            int sum = 0;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                sum += communitysizes[j];
                            }
                            commcoords.Add(sum);
                        }
                        else
                        {
                            commcoords.Add(0);
                        }
                    }
                    break;
                case "Cliques":
                    for (int i = 0; i < _cliques.Count; i++)
                    {
                        communitysizes.Add(_cliques[i].Count);
                    }
                    for (int i = 0; i < _cliques.Count; i++)
                    {
                        if (i > 0)
                        {
                            int sum = 0;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                sum += communitysizes[j];
                            }
                            commcoords.Add(sum);
                        }
                        else
                        {
                            commcoords.Add(0);
                        }
                    }
                    break;
                case "Blocks":
                    for (int i = 0; i < Blocks.Count; i++)
                    {
                        communitysizes.Add(Blocks[i].Count);
                    }
                    for (int i = 0; i < Blocks.Count; i++)
                    {
                        if (i > 0)
                        {
                            int sum = 0;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                sum += communitysizes[j];
                            }
                            commcoords.Add(sum);
                        }
                        else
                        {
                            commcoords.Add(0);
                        }
                    }
                    break;
            }

            double septotaldegree = 0;
            double cohtotaldegree = 0;

            for (int i = 0; i < partitioned.Rows; i++)//calculate outdegrees
            {
                double sum = 0;
                double cohsum = 0;
                for (int j = 0; j < partitioned.Cols; j++)
                {
                    sum += partitioned[i, j];
                    if (cohesionMatrix != null)
                    {
                        cohsum += partitionedCohesion[i, j];
                    }
                }
                sepoutdegrees.Add(sum);
                cohoutdegrees.Add(cohsum);
            }

            for (int i = 0; i < partitioned.Cols; i++)//Calculate indegrees
            {
                double sum = 0;
                double cohsum = 0;
                for (int j = 0; j < partitioned.Rows; j++)
                {
                    sum += partitioned[j, i];
                    if (cohesionMatrix != null)
                    {
                        cohsum += partitionedCohesion[j, i];
                    }

                }
                sepindegrees.Add(sum);
                cohindegrees.Add(cohsum);
            }

            foreach (double d in sepoutdegrees)
            {
                septotaldegree += d;
            }
            foreach (double d in cohoutdegrees)
            {
                cohtotaldegree += d;
            }

            Matrix seprandpart = new Matrix(partitionedNodeList.Count, partitionedNodeList.Count);
            Matrix cohrandpart = new Matrix(partitionedNodeList.Count, partitionedNodeList.Count);

            for (int i = 0; i < sepoutdegrees.Count; i++)
            {
                for (int j = 0; j < sepindegrees.Count; j++)
                {
                    seprandpart[i, j] = (sepoutdegrees[i] * sepindegrees[j]) / septotaldegree;
                    if (cohesionMatrix != null)
                    {
                        cohrandpart[i, j] = (cohoutdegrees[i] * cohindegrees[j]) / cohtotaldegree;
                    }
                }
            }
            Matrix sepadjrandpart = new Matrix(seprandpart);
            Matrix cohadjrandpart = new Matrix(cohrandpart);

            for (int i = 0; i < sepadjrandpart.Rows; i++)
            {
                int j = i;
                sepadjrandpart[i, j] = 0;
                if (cohesionMatrix != null)
                {
                    cohadjrandpart[i, j] = 0;
                }
            }

            Matrix nulldensity = new Matrix(groupCount, groupCount);
            Matrix nullcohesion = new Matrix(groupCount, groupCount);

            for (int q = 0; q < groupCount; q++)
            {
                for (int r = 0; r < groupCount; r++)
                {
                    double sum = 0;
                    double cohsum = 0;
                    double result = 0;
                    double cohresult = 0;
                    for (int i = commcoords[q]; i < commcoords[q] + communitysizes[q]; i++)
                    {
                        for (int j = commcoords[r]; j < commcoords[r] + communitysizes[r]; j++)
                        {
                            sum += sepadjrandpart[i, j];
                            if (cohesionMatrix != null)
                            {
                                cohsum += cohadjrandpart[i, j];
                            }
                        }
                    }
                    if (q == r)
                    {
                        result = sum / (communitysizes[q] * (communitysizes[q] - 1));
                        cohresult = cohsum / (communitysizes[q] * (communitysizes[q] - 1));
                    }
                    else if (type != "Cliques")
                    {
                        result = sum / (communitysizes[q] * communitysizes[r]);
                        cohresult = cohsum / (communitysizes[q] * communitysizes[r]);
                    }
                    else
                    {
                        _cliques.LoadCliqueByCliqueOverlap();
                        Matrix cliqueOverlap = new Matrix(_cliques.Count, _cliques.Count);
                        for (int i = 0; i < _cliques.Count; i++)
                        {
                            for (int j = 0; j < _cliques.Count; j++)
                            {
                                cliqueOverlap[i, j] = _cliques.GetCliqueByCliqueOverlap(i, j);
                            }
                        }
                        cliqueOverlap[0, 0] = 0;

                        result = sum / ((communitysizes[q] * communitysizes[r]) - cliqueOverlap[q, r]);
                        cohresult = cohsum / ((communitysizes[q] * communitysizes[r]) - cliqueOverlap[q, r]);
                    }
                    if (communitysizes[q] == 1 && communitysizes[r] == 1)
                    {
                        result = sum;
                        cohresult = cohsum;
                    }
                    nulldensity[q, r] = result;
                    nullcohesion[q, r] = cohresult;

                }
            }
            double nullSeparation = 0;
            double nullCohesion = 0;
            double TCohesion = 0;
            double TSeparation = 0;
            nullSeparation = calculateNullCoefficientFromNullModel(nulldensity, groupCount, type);
            nullCohesion = calculateNullCoefficientFromNullModel(nullcohesion, groupCount, type);
            switch (type)
            {
                case "Communities":
                    TCohesion = CalculateCommGC() / Math.Abs(nullCohesion);
                    TSeparation = CalculateCommSC(communities) / Math.Abs(nullSeparation);
                    break;
                case "Cliques":
                    TCohesion = CalculateCliqueGC() / Math.Abs(nullCohesion);
                    TSeparation = CalculateCliqueSC(_cliques) / Math.Abs(nullSeparation);
                    break;
                case "Blocks":
                    TCohesion = CalculateBlockGC() / Math.Abs(nullCohesion);
                    TSeparation = CalculateBlockSC(Blocks) / Math.Abs(nullCohesion);
                    break;
            }
            output.Add(nullSeparation);
            output.Add(TSeparation);
            output.Add(nullCohesion);
            output.Add(TCohesion);
            return output;
        }
        // Function to initialize the group array, putting all vertices in group 0
        public void initgroups()
        {
            group = new int[nvertices];
            indiv = new int[nvertices];
            q_array = new double[nvertices];
            for (int i = 0; i < nvertices; i++)
            {
                q_array[i] = -1;
            }
            q_size = 0;

            ngroups = 1;

            // Used as a substitute for the C calloc function
            for (int i = 0; i < nvertices; i++)
            {
                group[i] = indiv[i] = 0;
            }
            q = 0.0;
        }

        // Function to make the matrix B for a particular group g.  Returns 1 if
        // the group only has one vertex

        public int makebm(int g)
        {
            int i, j;
            int v, nn;
            double sum;

            // First count the number of vertices in group g

            for (i = 0, gsize = 0; i < nvertices; i++)
            {
                if (group[i] == g)
                    gsize++;
            }
            if (gsize == 1)
            {
                // stderr, "Group %i contains only one vertex and hence is indivisible\n"
                return 1;
            }

            // Allocate and clear space for matrix B and the vertex map

            map = new int[gsize];
            backmap = new int[nvertices];
            b = new double[gsize * gsize];
            u = new double[gsize * gsize];

            // Used as a substitute for the C calloc function
            for (i = 0; i < gsize * gsize; i++)
            {
                b[i] = u[i] = 0;
            }

            // Make the vertex maps
            for (i = 0, j = 0; i < nvertices; i++)
            {
                if (group[i] == g)
                {
                    map[j] = i;
                    backmap[i] = j;
                    j++;
                }
            }

            /* Set elements of the adjacency matrix to zero */

            // This is done so that the adjacency matrix and its
            // transpose can be use to create the B matrix
            // printf("Bus error fault occurs right after this line.\n");
            for (i = 0; i < gsize; i++)
            {
                for (j = 0; j < gsize; j++)
                {
                    b[i + gsize * j] = 0.0;
                }
            }

            // "Buss error by now.\n"

            // Build the adjacency matrix
            for (i = 0; i < gsize; i++)
            {
                v = map[i];
                for (j = 0; j < indegree[v]; j++)
                {
                    nn = list_edge[v][j];
                    if (group[nn] == g)
                    {
                        // Add real edge from matrix A to B matrix
                        b[i + gsize * backmap[nn]] += 1.0;
                        // Add real edge from matrix A^T to B matrix
                        b[backmap[nn] + gsize * i] += 1.0;
                    }
                }
            }

            // Subtract the outer product of the in/out degrees
            /* We need to subtract both (K^i_in * K^j_out / m) and (K^j_in * K^i_out / m),
             * to get out matrix B (which is really B + B^T) so we need to account
             * for the transpose as well */
            for (i = 0; i < gsize; i++)
            {
                for (j = 0; j < gsize; j++)
                {
                    b[i + gsize * j] -= (double)indegree[map[i]] * outdegree[map[j]] / nedges;
                    b[i + gsize * j] -= (double)outdegree[map[i]] * indegree[map[j]] / nedges;
                }
            }

            // Compute the sum of each row and subtract it from the diagonal
            for (i = 0; i < gsize; i++)
            {
                sum = 0;
                for (j = 0; j < gsize; j++)
                {
                    sum += b[i + gsize * j];
                }
                b[i + gsize * i] -= sum;
            }

            // Copy b[] to u[]
            for (i = 0; i < gsize; i++)
            {
                for (j = 0; j < gsize; j++)
                {
                    u[i + gsize * j] = b[i + gsize * j];
                }
            }

            return 0;
        }

        // Function to calculate the eigenvalues and eigenvectors of B
        void spectrum()
        {
            //int i;
            int lwork, liwork;
            int info = 0; // 0 is a temporary value
            int[] iwork;
            string jobz, uplo;
            double[] work;

            // Make space

            lwork = 1 + 6 * gsize + 2 * gsize * gsize;
            liwork = 3 + 5 * gsize;

            new_w = new double[gsize];
            work = new double[lwork];
            iwork = new int[liwork];

            // Use LAPACK to compute the eigenvalues and vectors
            //
            // Arguments:
            //   jobz: compute values only
            //   uplo: upper triangle is stored (actually both are, so no effect)
            //   gsize: order of matrix
            //   u: matrix -- overwritten by routine
            //   gsize: stride of array
            //   w: eigenvalues output
            //   work,lwork,iwork,liwork: work arrays and size of work arrays
            //   info: return status

            DSYEVD dsyevd = new DSYEVD();

            jobz = "V";
            uplo = "L";
            dsyevd.Run(jobz, uplo, gsize, ref u, 0, gsize, ref new_w, 0, ref work, 0, lwork, ref iwork, 0, liwork, ref info);
            //dsyevd_(&jobz,&uplo,&gsize,u,&gsize,w,work,&lwork,iwork,&liwork,&info);

            // Free space
        }

        // Function to divide vertices according to the leading eigenvector, or
        // not if no positive eigenvalue exists.  Returns 0 if the group was
        // divided, 1 otherwise.

        int divide(int g, bool calcStats, bool newDiscrete)
        {
            int i, j, n;
            int c, kout, kin, v;
            int dout0, din0, dout1, din1;
            int same, other;
            int maxvert = 0;
            int maxsplit;
            int count;
            int[] ind;
            int[] oldind;
            int[] done;
            int[] order;
            double maxrdq;
            double dq, maxdq;
            double spectraldq;
            double refinedq;
            double totaldq;
            double SE_coh = 0, SE_den = 0;


            // Check if the subgraph is divisible.  If not, mark it as indivisible.

            if (new_w[gsize - 1] < EPSILON)
            {
                for (i = 0; i < gsize; i++)
                {
                    indiv[map[i]] = 1;
                }

                q_index = g;
                // increment q_size only if there an unused spot in q_array[index]
                if (q_array[q_index] == -1)
                {
                    q_size++;
                    //
                    // this is where there is a new iterations...I think
                    //
                    if (calcStats && !newDiscrete)
                    {
                        createCommunitiesFromGroup();
                        double SC = CalculateCommSC(communities);
                        comm_SC_list.Add(SC);
                        double GC = CalculateCommGC();
                        comm_GC_list.Add(GC);
                        List<double> nullcoefficients = calculateNullCoefficients("Communities");

                        comm_null_SC_list.Add(nullcoefficients[0]);
                        comm_T_SC_list.Add(nullcoefficients[1]);
                        comm_null_GC_list.Add(nullcoefficients[2]);
                        comm_T_GC_list.Add(nullcoefficients[3]);
                    }

                    if (newDiscrete)
                    {
                        double SC;
                        createCommunitiesFromGroup();
                        if (communities.Count > 1)
                        {
                            SC = CalculateCommSC(communities);
                        }
                        else
                        {
                            SC = 0;
                        }
                        comm_SC_list.Add(SC);

                        if (calcStats)
                        {
                            double GC = CalculateCommGC();
                            comm_GC_list.Add(GC);
                            // Calculate the Separation (Density)
                            Matrix D = CalculateCommDensity(communities);
                            for (int k = 0; k < D.Rows; k++)
                                for (int l = 0; l < D.Cols; l++)
                                    SE_den += Math.Pow((D[k, l] - D.GetRowAverage(k)), 2);
                            SE_den = Math.Sqrt(SE_den / (D.Cols * D.Cols));


                            // Calculate the Separation (Cohesion)
                            Matrix C = computeCommCohesiveMatrix();
                            for (int k = 0; k < C.Rows; k++)
                                for (int l = 0; l < C.Cols; l++)
                                    SE_coh += Math.Pow((C[k, l] - C.GetRowAverage(k)), 2);
                            SE_coh = Math.Sqrt(SE_coh / (C.Cols * C.Cols));
                            comm_SE_coh_list.Add(SE_coh);
                            comm_SE_den_list.Add(SE_den);
                            comm_T_den_list.Add(SC / SE_den);
                            comm_T_coh_list.Add(GC / SE_coh);

                            List<double> nullcoefficients = calculateNullCoefficients("Communities");

                            comm_null_SC_list.Add(nullcoefficients[0]);
                            comm_T_SC_list.Add(nullcoefficients[1]);
                            comm_null_GC_list.Add(nullcoefficients[2]);
                            comm_T_GC_list.Add(nullcoefficients[3]);

                        }



                        if (SC > maxSC || firstCommunityIteration)
                        {
                            if (firstCommunityIteration)
                            {
                                firstCommunityIteration = false;
                            }
                            maxSeparationcommunities = new List<int[]>(communities);
                            maxSepComNum = maxSeparationcommunities.Count;


                            maxcomm_SC_list = new List<double>(comm_SC_list);
                            if (calcStats)
                            {
                                maxcomm_GC_list = new List<double>(comm_GC_list);
                                maxcomm_SE_coh_list = new List<double>(comm_SE_coh_list);
                                maxcomm_SE_den_list = new List<double>(comm_SE_den_list);
                                maxcomm_T_coh_list = new List<double>(comm_T_coh_list);
                                maxcomm_T_den_list = new List<double>(comm_T_den_list);
                                maxcomm_null_SC_list = new List<double>(comm_null_SC_list);
                                maxcomm_null_GC_list = new List<double>(comm_null_GC_list);
                                maxcomm_T_SC_list = new List<double>(comm_T_SC_list);
                                maxcomm_T_GC_list = new List<double>(comm_T_GC_list);
                            }
                            /*maxSepq_array = new double[nvertices];
                            for (int k = 0; k < nvertices; k++)
                                maxSepq_array[i] = q_array[i];
                            maxSepq_size = q_size;*/
                            maxSC = SC;
                        }
                        for (int k = 0; k < comm_SC_list.Count; k++)
                        {
                            if (comm_SC_list[k] > maxSC)
                                maxSC = comm_SC_list[k];
                        }
                    }
                }
                q_array[q_index] = q;

                return 1;
            }

            // If divisible, create a new community from the members with positive
            // corresponding elements in the eigenvector
            ind = new int[gsize];
            oldind = new int[gsize];
            done = new int[gsize];
            order = new int[gsize];

            for (i = 0; i < gsize; i++)
            {
                if (u[i + gsize * (gsize - 1)] > 0.0)
                {
                    ind[i] = 1;
                }
                else
                {
                    ind[i] = 0;
                }
            }

            // Calculate the modularity contribution for this split
            spectraldq = 0.0;
            for (i = 0; i < gsize; i++)
            {
                for (j = 0; j < gsize; j++)
                {
                    if (ind[i] == ind[j])
                    {
                        // keep the 1/(2 * nedges) factor instead of the 1/nedges
                        // one would expect, except that here B is actually B + B^T
                        spectraldq += b[i + gsize * j] / (2 * nedges);
                    }
                }
            }

            // Now perform a KL-type optimization starting from this as the initial
            // state
            totaldq = spectraldq;
            do
            {
                // Save the old ind[] array and clear the done[] array
                for (i = 0; i < gsize; i++)
                {
                    oldind[i] = ind[i];
                    done[i] = 0;
                }

                // Calculate the total indegrees and outdegrees
                // of each of the two communities
                din0 = din1 = dout0 = dout1 = 0;
                for (i = 0; i < gsize; i++)
                {
                    if (ind[i] == 0)
                    {
                        din0 += indegree[map[i]];
                        dout0 += outdegree[map[i]];
                    }
                    else
                    {
                        din1 += indegree[map[i]];
                        dout1 += outdegree[map[i]];
                    }
                }

                // Move each vertex exactly once
                refinedq = maxrdq = 0.0;
                maxsplit = 0;
                for (n = 0; n < gsize; n++)
                {
                    // Find which vertex to move this time
                    maxdq = -LARGE;
                    for (i = 0; i < gsize; i++)
                    {
                        if (done[i] == 0)
                        {
                            // Calculate the change in subgraph modularity if we move this vertex
                            v = map[i];
                            c = ind[i];
                            kin = indegree[v];
                            kout = outdegree[v];

                            // Calculate number of neighbors (but differentiate the ones
                            // pointing to out vertex and those pointed to by our vertices)
                            // that are in same or other group.

                            // Note that neighbors must be in overall group g first to be
                            // counted

                            same = other = 0;

                            // first look over those vertices which point to our
                            // vertex n (those in the regular adjacency list "edge")
                            // these are same and other b/c we are looking at the
                            // in edges on vertex v
                            for (j = 0; j < kin; j++)
                            {
                                if (group[list_edge[v][j]] == g)
                                {
                                    if (ind[backmap[list_edge[v][j]]] == c)
                                        same++;
                                    else
                                        other++;
                                }
                            }

                            // now look over those vertices which are pointed to by our
                            // vertex n (those in the transpose adjacency list "edgeTspose")
                            // these are same and other b/c we are looking at the 
                            // in edges on vertex v
                            for (j = 0; j < kout; j++)
                            {
                                if (group[list_edgeTranspose[v][j]] == g)
                                {
                                    if (ind[backmap[list_edgeTranspose[v][j]]] == c)
                                        same++;
                                    else
                                        other++;
                                }
                            }

                            // Calculate change in modularity
                            if (c == 0)
                                dq = other - same + ((double)kin * (dout0 - dout1 - kout) / (nedges)) + ((double)kout * (din0 - din1 - kin) / (nedges));
                            else
                                dq = other - same + ((double)kin * (dout1 - dout0 - kout) / (nedges)) + ((double)kout * (din1 - din0 - kin) / (nedges));
                            dq /= nedges;

                            // Check if this is largest so far
                            if (dq > maxdq)
                            {
                                maxdq = dq;
                                maxvert = i;
                            }
                        }
                    }

                    // Move the vertex that scored the highest and mark as done
                    ind[maxvert] = 1 - ind[maxvert];
                    done[maxvert] = 1;

                    // Update the d variables and the modularity change
                    if (ind[maxvert] == 0)
                    {
                        dout0 += outdegree[map[maxvert]];
                        din0 += indegree[map[maxvert]];
                        dout1 -= outdegree[map[maxvert]];
                        din1 -= indegree[map[maxvert]];
                    }
                    else
                    {
                        dout0 -= outdegree[map[maxvert]];
                        din0 -= indegree[map[maxvert]];
                        dout1 += outdegree[map[maxvert]];
                        din1 += indegree[map[maxvert]];
                    }

                    refinedq += maxdq;

                    // Make a note of which vertex was moved
                    order[n] = maxvert;

                    // Check to see if this is the largest Q we have seen so far
                    if (refinedq > maxrdq)
                    {
                        maxrdq = refinedq;
                        maxsplit = n + 1;
                    }
                }

                // Check to see if best split is for all vertices moved.  In that case,
                // it's exactly as if no vertices were moved, so we move none of them
                if (maxsplit == gsize)
                {
                    maxrdq = 0.0;
                    maxsplit = 0;
                }
                totaldq += maxrdq;

                // Update the ind[] array to the best configuration
                for (i = 0; i < gsize; i++)
                    ind[i] = oldind[i];
                for (i = 0; i < maxsplit; i++)
                    ind[order[i]] = 1 - ind[order[i]];

            } while (maxrdq > EPSILON);

            // Confirm this split if the best dQ > 0 and the split doesn't put all
            // vertices in a single group
            if (totaldq > EPSILON)
            {
                // Count the total number in group 1 as a check
                for (i = 0, count = 0; i < gsize; i++)
                {
                    if (ind[i] != 0) // if false?
                        count++;
                }

                if ((count != gsize) && (count != 0))
                {
                    // Copy the results to the group tables
                    for (i = 0; i < gsize; i++)
                    {
                        if (ind[i] != 0)
                            group[map[i]] = ngroups;
                    }

                    // And increase the number of groups
                    ngroups++;

                    // Update the modularity
                    q += totaldq;
                    return 0;
                }
                else
                {
                    // To avoid error from the compiler
                    return 0;
                }
            }
            else
            {
                // dQ <= 0 so this group is indivisible
                for (i = 0; i < gsize; i++)
                    indiv[map[i]] = 1;
                return 1;
            }
        }

        public void convertMatrixToProperInput()
        {
            list_edge = new int[mTable["Data"].Rows][];
            nvertices = mTable["Data"].Rows;
            for (int i = 0; i < mTable["Data"].Rows; ++i)
            {
                int indegree_count = 0;
                for (int j = 0; j < mTable["Data"].Cols; ++j)
                {
                    if (mTable["Data"][i, j] > 0.0)
                        indegree_count++;
                }
                indegree[i] = indegree_count;
                list_edge[i] = new int[indegree[i]];
            }

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                int edge_pos = 0;
                for (int j = 0; j < mTable["Data"].Cols; j++)
                {
                    if (mTable["Data"][i, j] > 0.0)
                    {
                        list_edge[i][edge_pos] = j;
                        edge_pos++;
                    }
                }
            }

            // Determine the out-degree of each vertex

            // Set out-degree for each vertex to 0;

            // Set outdegreeBuild will be a marker that allows us to build
            // the transpose edge list
            for (int i = 0; i < nvertices; i++)
            {
                outdegree[i] = 0;
                outdegreeBuild[i] = 0;
            }

            // Loop over edges counting every out edge from every vertex
            for (int n = 0; n < nvertices; n++)
            {
                for (int i = 0; i < indegree[n]; i++)
                {
                    outdegree[list_edge[n][i]]++;
                }
            }

            // Now make space for a transpose edge list
            for (int i = 0; i < nvertices; i++)
            {
                int tempSize = outdegree[i];
                if (tempSize != 0)
                {
                    list_edgeTranspose[i] = new int[tempSize];
                }
            }

            // Now create the reverse or transpose adjacency list

            int temp_j;
            for (int n = 0; n < nvertices; n++)
            {
                for (int i = 0; i < indegree[n]; i++)
                {
                    // Let temp_j be the source of the edge in the read adjacency list
                    // temp_j will become the target in the transpose edge list
                    temp_j = list_edge[n][i];
                    list_edgeTranspose[temp_j][outdegreeBuild[temp_j]] = n;
                    outdegreeBuild[temp_j]++;
                }
            }

            // Count the edges
            nedges = 0;
            for (int i = 0; i < nvertices; i++)
            {
                nedges += indegree[i];
            }
        }

        public void mainCommunityExtractionMethod(bool calcStats, bool newDiscrete, bool overlapping)
        {
            // Performs the actual algorithm for the community extraction
            firstCommunityIteration = true;
            int num_i = 0;
            initgroups();
            while (num_i < nvertices)
            {
                if (indiv[num_i] != 0)
                {
                    num_i++;
                }
                else
                {
                    if (makebm(group[num_i]) == 1)
                    {
                        indiv[num_i] = 1;
                    }
                    else
                    {
                        spectrum();
                        divide(group[num_i], calcStats, newDiscrete);
                    }
                }



            }
            // add Final value of modularity coefficient q to the q_array

            /*q_array = new double[nvertices];
            for (int i = 0; i < nvertices; i++)
                q_array[i] = maxSepq_array[i];
            q_size = maxSepq_size;*/

            q_array[q_size] = q;

        }

        public void createCommunitiesFromGroup()
        {
            // find the max integer, comNum, of the values in group
            // there will be comNum + 1 communities
            comSize = group.Length;
            comNum = 0;
            comNum = group[0];
            for (int i = 1; i < group.Length; i++)
                if (comNum < group[i])
                    comNum = group[i];

            comNum += 1;

            // create the communities
            communities = new List<int[]>();
            for (int i = 0; i < comNum; i++)
            {
                int[] tempComm = new int[group.Length];
                for (int j = 0; j < group.Length; j++)
                {
                    if (group[j] == i)
                        tempComm[j] = 1;
                    else
                        tempComm[j] = 0;
                }
                communities.Add(tempComm);
            }
        }


        // Class to implement IComparer for Blocks
        protected class BlockComparer : IComparer<List<int>>
        {
            public bool Equals(List<int> x, List<int> y)
            {
                return Compare(x, y) == 0;
            }

            public int Compare(List<int> x, List<int> y)
            {
                if (x.Count != y.Count)
                    return y.Count - x.Count;

                return x[0] - y[0];
            }

            public int GetHashCode(List<int> obj)
            {
                return obj.GetHashCode();
            }
        }

        protected double GetRealDensity(double density, int year, string m)
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

        protected int GetRealReachNum(int reachNum, int year)
        {
            if (reachNum == -1)
                if (!reachNumVector.ContainsKey(year))
                    throw new Exception("Cannot use reachability num vector for year " + year.ToString());
                else
                    return reachNumVector[year];
            return reachNum;
        }

        protected double GetRealViableCoalitionCutoff(double cutoff, int year)
        {
            if (cutoff == -1.0)
                if (!viableCoalitionVector.ContainsKey(year))
                    throw new Exception("Cannot use cutoff vector for year " + year.ToString());
                else
                    return viableCoalitionVector[year];
            return cutoff;

        }

        protected int GetRealCMinMember(int cmin, int year)
        {
            if (cmin == -1)
                if (!cliqueMinVector.ContainsKey(year))
                    throw new Exception("Cannot use clique min member vector for year " + year.ToString());
                else
                    return cliqueMinVector[year];
            return cmin;
        }

        protected int GetRealKCliqueValue(int kclique, int year)
        {
            if (kclique == -1)
                if (!kCliqueVector.ContainsKey(year))
                    throw new Exception("Cannot use clique min member vector for year " + year.ToString());
                else
                    return kCliqueVector[year];
            return kclique;
        }

        public CliqueCollection LoadComponents(double cutoff, bool calcSE, double maxik, int year, int cmin, int n, bool doSum, bool zeroDiagonal, bool useCliq)
        {

            mTable["Temp"] = new Matrix(mTable["Data"]);
            n = mTable["Data"].Rows;
            Matrix diag = mTable["Data"].GetDiagonalMatrix();
            mTable["Temp"].Binarize();

            Matrix mat = new Matrix(mTable["Temp"]);

            for (int i = 0; i < mTable["Temp"].Rows; ++i)
                mat[i, i] = 0.0;

            // Generate R (dependency) matrix:
            Matrix sum = new Matrix(mTable["Temp"]);
            Matrix power = new Matrix(mat);


            for (int i = 2; i <= n; ++i)
            {
                power.Binarize();
                power *= mat;
                power.ZeroDiagonal();
                sum += power;

                Application.DoEvents();
            }

            sum.CloneTo(mTable["Temp"]);
            ReachabilityN = mTable["Temp"].Rows - 1;
            mTable["Temp"].Binarize();

            if (calcSE)
            {
                LoadStructEquiv(maxik, year, "Data");
            }

            maxik = GetRealDensity(maxik, year, "Data");

            // Make sure that the binary matrix exists
            if (!mTable["Data"].IsSquareMatrix)
                throw new MatrixException("Cannot find cliques for non-square matrix.");

            SymmetricBinaryMatrix MBinary;
            if (useCliq)
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff, cet);
            else
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff);

            comcliques = new CliqueCollection(MBinary, cmin);

            if (calcSE)
            {
                foreach (Clique c in comcliques)
                {
                    c.ComputeCohesion(mTable["SESE"]);
                }
            }
            return comcliques;
        }

        public BlockCollection LoadbComponents(double cutoff, bool calcSE, double maxik, int year, int cmin, int n, bool doSum, bool zeroDiagonal, bool useCliq)
        {
            FindBlocks(cutoff, Blocks, _minCliqueSize);


            mTable["Temp"] = new Matrix(mTable["Data"]);
            n = mTable["Data"].Rows;
            Matrix diag = mTable["Data"].GetDiagonalMatrix();
            mTable["Temp"].Binarize();

            Matrix mat = new Matrix(mTable["Temp"]);

            for (int i = 0; i < mTable["Temp"].Rows; ++i)
                mat[i, i] = 0.0;

            // Generate R (dependency) matrix:
            Matrix sum = new Matrix(mTable["Temp"]);
            Matrix power = new Matrix(mat);


            for (int i = 2; i <= n; ++i)
            {
                power.Binarize();
                power *= mat;
                power.ZeroDiagonal();
                sum += power;

                Application.DoEvents();
            }

            sum.CloneTo(mTable["Temp"]);

            ReachabilityN = mTable["Temp"].Rows - 1;

            mTable["Temp"].Binarize();

            if (calcSE)
            {
                LoadStructEquiv(maxik, year, "Data");
            }

            maxik = GetRealDensity(maxik, year, "Data");

            // Make sure that the binary matrix exists
            if (!mTable["Data"].IsSquareMatrix)
                throw new MatrixException("Cannot find cliques for non-square matrix.");

            SymmetricBinaryMatrix MBinary;
            if (useCliq)
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff, cet);
            else
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff);
            List<Block> blocList = new List<Block>();
            blocList = _blocks._blocks;
            comblocks = new BlockCollection(MBinary, cmin, blocList);

            if (calcSE)
            {
                foreach (Block b in comblocks)
                {
                    b.ComputeCohesion(mTable["SESE"]);
                }
            }

            return comblocks;
        }

        public CommCollection LoadcomComponents(double cutoff, bool calcSE, double maxik, int year, int cmin, int n, bool doSum, bool zeroDiagonal, bool useCliq)
        {
            FindComm(cutoff, communities, _minCliqueSize);

            mTable["Temp"] = new Matrix(mTable["Data"]);
            n = mTable["Data"].Rows;
            Matrix diag = mTable["Data"].GetDiagonalMatrix();
            mTable["Temp"].Binarize();

            Matrix mat = new Matrix(mTable["Temp"]);

            for (int i = 0; i < mTable["Temp"].Rows; ++i)
                mat[i, i] = 0.0;

            // Generate R (dependency) matrix:
            Matrix sum = new Matrix(mTable["Temp"]);
            Matrix power = new Matrix(mat);


            for (int i = 2; i <= n; ++i)
            {
                power.Binarize();
                power *= mat;
                power.ZeroDiagonal();
                sum += power;

                Application.DoEvents();
            }

            sum.CloneTo(mTable["Temp"]);

            ReachabilityN = mTable["Temp"].Rows - 1;

            mTable["Temp"].Binarize();

            if (calcSE)
            {
                LoadStructEquiv(maxik, year, "Data");
            }

            maxik = GetRealDensity(maxik, year, "Data");

            // Make sure that the binary matrix exists
            if (!mTable["Data"].IsSquareMatrix)
                throw new MatrixException("Cannot find cliques for non-square matrix.");

            SymmetricBinaryMatrix MBinary;
            if (useCliq)
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff, cet);
            else
                MBinary = new SymmetricBinaryMatrix(mTable["Temp"], cutoff);
            List<Comm> commList = new List<Comm>();
            commList = _communities._communities;
            comComm = new CommCollection(MBinary, cmin, commList);


            if (calcSE)
            {
                foreach (Comm c in comComm)
                {
                    c.ComputeCohesion(mTable["SESE"]);
                }
            }

            return comComm;
        }

        //
        // Calling the Density Functions
        //
        public void LoadCliqueDensity()
        {
            mTable["CliqueDensity"] = CalculateCliqueDensity(_cliques);
            for (int i = 0; i < mTable["CliqueDensity"].Rows; ++i)
                mTable["CliqueDensity"].RowLabels[i] = mTable["CliqueDensity"].ColLabels[i] = (i + 1).ToString();
        }

        // can be used for either blocks or clusters
        public void LoadBlockDensity(string m_name)
        {
            mTable[m_name] = CalculateBlockDensity(Blocks);
            for (int i = 0; i < mTable[m_name].Rows; i++)
                mTable[m_name].RowLabels[i] = mTable[m_name].ColLabels[i] = (i + 1).ToString();
        }

        public void LoadOverlapCommDensity()
        {
            mTable["OverlapCommDensity"] = CalculateOverlapCommDensity(overlapComm);
            for (int i = 0; i < mTable["OverlapCommDensity"].Rows; i++)
                mTable["OverlapCommDensity"].RowLabels[i] = mTable["OverlapCommDensity"].ColLabels[i] = (i + 1).ToString();
        }

        //
        // Calling the Relative Density Matrices
        //

        public void LoadCliqueRelativeDensity(double density)
        {
            mTable["CliqueRelativeDensity"] = CalculateCliqueDensity(_cliques);
            double d = MatrixComputations.Density(mTable["Data"], density);

            for (int i = 0; i < mTable["CliqueRelativeDensity"].Rows; i++)
            {
                for (int j = 0; j < mTable["CliqueRelativeDensity"].Cols; j++)
                {
                    mTable["CliqueRelativeDensity"][i, j] /= d;
                }
                mTable["CliqueRelativeDensity"].RowLabels[i] = mTable["CliqueRelativeDensity"].ColLabels[i] = (i + 1).ToString();
            }
        }

        // can be used for either blocks or clusters
        public void LoadRelativeBlockDensity(double density, string m_name)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            //LoadBlockPartitionMatrix();
            //LoadStructEquiv(density, year, "Data");

            int B = Blocks.Count;
            mTable[m_name] = CalculateBlockDensity(Blocks);
            double d = MatrixComputations.Density(mTable["Data"], density);

            for (int i = 0; i < B; ++i)
            {
                for (int j = 0; j < B; ++j)
                {
                    mTable[m_name][i, j] /= d;
                }
                mTable[m_name].RowLabels[i] = mTable[m_name].ColLabels[i] = (i + 1).ToString();
            }
        }

        public void LoadOverlapCommRelativeDensity(double density)
        {
            mTable["OverlapCommRelativeDensity"] = CalculateOverlapCommDensity(overlapComm);
            double d = MatrixComputations.Density(mTable["Data"], density);

            for (int i = 0; i < mTable["OverlapCommRelativeDensity"].Rows; i++)
            {
                for (int j = 0; j < mTable["OverlapCommRelativeDensity"].Cols; j++)
                {
                    mTable["OverlapCommRelativeDensity"][i, j] /= d;
                }
                mTable["OverlapCommRelativeDensity"].RowLabels[i] = mTable["OverlapCommRelativeDensity"].ColLabels[i] = (i + 1).ToString();
            }
        }

        //
        // Calling the Cohesion Matrices
        //

        public void LoadCliqueCohesionMatrix(double density, int year)
        {
            LoadStructEquiv(density, year, "Data");
            mTable["CliqueCohesionMatrix"] = computeCliqueCohesiveMatrix();
            for (int i = 0; i < mTable["CliqueCohesionMatrix"].Rows; i++)
                mTable["CliqueCohesionMatrix"].RowLabels[i] = mTable["CliqueCohesionMatrix"].ColLabels[i] = (i + 1).ToString();
        }

        // can be used for either blocks or clusters
        public void LoadBlockCohesionMatrix(double cutoff, double density, int year, string m_name)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");
            FindBlocks(cutoff, Blocks, _minCliqueSize);
            LoadStructEquiv(density, year, "Data");

            mTable[m_name] = computeBlockCohesiveMatrix();
            for (int i = 0; i < mTable[m_name].Rows; i++)
                mTable[m_name].RowLabels[i] = mTable[m_name].ColLabels[i] = (i + 1).ToString();
        }

        public void LoadOverlapCommCohesiveMatrix(double density, int year)
        {
            LoadStructEquiv(density, year, "Data");
            mTable["OverlapCommCohesiveMatrix"] = computeOverlapCommCohesiveMatrix(overlapComm);
            for (int i = 0; i < mTable["OverlapCommCohesiveMatrix"].Rows; i++)
                mTable["OverlapCommCohesiveMatrix"].RowLabels[i] = mTable["OverlapCommCohesiveMatrix"].ColLabels[i] = (i + 1).ToString();
        }

        //
        // Calling the function to calculate the Separation Coefficients
        //

        public void LoadCliqueSC(string m_name)
        {
            double SC = CalculateCliqueSC(_cliques);
            Matrix SC_vector = mTable.AddMatrix(m_name, 1, 1);

            SC_vector[0, 0] = SC;
            SC_vector.ColLabels[0] = "Separation Coefficient";
        }



        //
        // will not need later
        //
        public void LoadBlockSC(string m_name)
        {
            double SC = CalculateBlockSC(Blocks);
            Matrix SC_vector = mTable.AddMatrix(m_name, 1, 1);

            SC_vector[0, 0] = SC;
            SC_vector.ColLabels[0] = "Separation Coefficient";
        }


        //
        // Calling the functions to calculate cohesion statistic
        //
        public void LoadCliqueCoefficients(double density, int year, string m_name)
        {
            LoadStructEquiv(density, year, "Data");

            double SC = CalculateCliqueSC(_cliques);
            double GC = CalculateCliqueGC();
            List<double> nullCoefficients = calculateNullCoefficients("Cliques");

            // Calculate the Separation (Density)
            double SE_den = 0;
            Matrix D = CalculateCliqueDensity(_cliques);
            for (int i = 0; i < D.Rows; i++)
                for (int j = 0; j < D.Cols; j++)
                    SE_den += Math.Pow((D[i, j] - D.GetRowAverage(i)), 2);
            SE_den = Math.Sqrt(SE_den / (D.Cols * D.Cols));

            // Calculate the Separation (Cohesion)
            double SE_coh = 0;
            Matrix C = computeCliqueCohesiveMatrix();
            for (int i = 0; i < C.Rows; i++)
                for (int j = 0; j < C.Cols; j++)
                    SE_coh += Math.Pow((C[i, j] - C.GetRowAverage(i)), 2);
            SE_coh = Math.Sqrt(SE_coh / (C.Cols * C.Cols));

            double T_den = SC / SE_den;
            double T_coh = GC / SE_coh;

            Matrix Stats_vector = mTable.AddMatrix(m_name, 1, 7);

            Stats_vector[0, 0] = year;
            Stats_vector[0, 1] = SC;
            Stats_vector[0, 2] = nullCoefficients[0];
            Stats_vector[0, 3] = nullCoefficients[1];
            Stats_vector[0, 4] = GC;
            Stats_vector[0, 5] = nullCoefficients[2];
            Stats_vector[0, 6] = nullCoefficients[3];

            Stats_vector.ColLabels[0] = "Year";
            Stats_vector.ColLabels[1] = "Separation Coefficient";
            Stats_vector.ColLabels[2] = "Null Sep. Coeff.";
            Stats_vector.ColLabels[3] = "T-Separation";
            Stats_vector.ColLabels[4] = "Cohesion Coefficient";
            Stats_vector.ColLabels[5] = "Null Coh. Coeff.";
            Stats_vector.ColLabels[6] = "T-Cohesion";

        }

        public void LoadBlockCoefficients(double cutoff, double density, int year, string m_name)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");
            FindBlocks(cutoff, Blocks, _minCliqueSize);
            LoadStructEquiv(density, year, "Data");

            double SC = CalculateBlockSC(Blocks);
            double GC = CalculateBlockGC();
            List<double> nullCoefficients = calculateNullCoefficients("Blocks");

            // Calculate the Separation (Density)
            double SE_den = 0;
            Matrix D = CalculateBlockDensity(Blocks);
            for (int i = 0; i < D.Rows; i++)
                for (int j = 0; j < D.Cols; j++)
                    SE_den += Math.Pow((D[i, j] - D.GetRowAverage(i)), 2);
            SE_den = Math.Sqrt(SE_den / (D.Cols * D.Cols));

            // Calculate the Separation (Cohesion)
            double SE_coh = 0;
            Matrix C = computeBlockCohesiveMatrix();
            for (int i = 0; i < C.Rows; i++)
                for (int j = 0; j < C.Cols; j++)
                    SE_coh += Math.Pow((C[i, j] - C.GetRowAverage(i)), 2);
            SE_coh = Math.Sqrt(SE_coh / (C.Cols * C.Cols));

            double T_den = SC / SE_den;
            double T_coh = GC / SE_coh;

            Matrix Stats_vector = mTable.AddMatrix(m_name, 1, 7);

            Stats_vector[0, 0] = year;
            Stats_vector[0, 1] = SC;
            Stats_vector[0, 2] = nullCoefficients[0];
            Stats_vector[0, 3] = nullCoefficients[1];
            Stats_vector[0, 4] = GC;
            Stats_vector[0, 5] = nullCoefficients[2];
            Stats_vector[0, 6] = nullCoefficients[3];

            Stats_vector.ColLabels[0] = "Year";
            Stats_vector.ColLabels[1] = "Separation Coefficient";
            Stats_vector.ColLabels[2] = "Null Sep. Coeff.";
            Stats_vector.ColLabels[3] = "T-Separation";
            Stats_vector.ColLabels[4] = "Cohesion Coefficient";
            Stats_vector.ColLabels[5] = "Null Coh. Coeff.";
            Stats_vector.ColLabels[6] = "T-Cohesion";
        }

        //
        // Calculation of statistics for Overlapping Communities is at bottom of code
        //


        //
        // Calculating the Density Matrices
        //
        public Matrix CalculateCliqueDensity(CliqueCollection cliques)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found");

            // Check if main diagonal of sociomatrix is all 0s
            bool isAllZeros = true;
            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                if (mTable["Data"][i, i] > 0)
                {
                    isAllZeros = false;
                    break;
                }
            }

            int cliqueNum = cliques.Count;
            Matrix Density = new Matrix(cliqueNum, cliqueNum);

            for (int i = 0; i < cliqueNum; i++)
            {
                for (int j = 0; j < cliqueNum; j++)
                {
                    double numNodes_m = 0;
                    double numNodes_k = 0;
                    double summationValue = 0;
                    double same_nodes = 0;
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (cliques[i].IntContains(k) == 0)
                            continue;
                        if (cliques[i].IntContains(k) == cliques[j].IntContains(k))
                            same_nodes++;
                        numNodes_m++;
                        numNodes_k = 0;
                        for (int z = 0; z < mTable["Data"].Rows; z++)
                        {
                            if (cliques[j].IntContains(z) == 0)
                                continue;
                            numNodes_k++;
                            summationValue += mTable["Data"][k, z];
                        }
                    }
                    Density[i, j] = summationValue;

                    double denominator;
                    if (isAllZeros)
                        denominator = (numNodes_m * numNodes_k) - same_nodes;
                    else
                        denominator = (numNodes_m * numNodes_k);

                    if (numNodes_m == 1 && numNodes_k == 1)
                        denominator = 1;
                    //Density[i, j] /= ((numNodes_m * numNodes_k) - same_nodes);
                    Density[i, j] = (denominator == 0) ? 0 : Density[i, j] / denominator;
                }
            }
            return Density;
        }



        public Matrix CalculateBlockDensity(List<List<int>> Blocks)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            // Check if main diagonal of sociomatrix is all 0s
            bool isAllZeros = true;
            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                if (mTable["Data"][i, i] > 0)
                {
                    isAllZeros = false;
                    break;
                }
            }

            int B = Blocks.Count;
            Matrix Density = new Matrix(B, B);

            for (int i = 0; i < B; ++i)
            {
                for (int j = 0; j < B; ++j)
                {
                    double numNodes_m = 0;
                    double numNodes_k = 0;
                    double same_nodes = 0;
                    double summationValue = 0;
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (!Blocks[i].Contains(k))
                            continue;
                        if (Blocks[i].Contains(k) && Blocks[j].Contains(k))
                            same_nodes++;
                        numNodes_m++;
                        numNodes_k = 0;
                        for (int z = 0; z < mTable["Data"].Rows; z++)
                        {
                            if (!Blocks[j].Contains(z))
                                continue;
                            numNodes_k++;
                            summationValue += mTable["Data"][k, z];
                        }
                    }
                    Density[i, j] = summationValue;

                    double denominator;

                    if (isAllZeros)
                        denominator = (numNodes_m * numNodes_k) - same_nodes;
                    else
                        denominator = (numNodes_m * numNodes_k);

                    if (numNodes_m == 1 && numNodes_k == 1)
                        denominator = 1;
                    //Density[i, j] /= ((numNodes_m * numNodes_k) - same_nodes);
                    Density[i, j] = (denominator == 0) ? 0 : Density[i, j] / denominator;
                }
            }
            return Density;
        }


        public Matrix CalculateCommDensity(List<int[]> communities)
        {
            if (communities == null)
                throw new Exception("Communities have not been extracted");

            // Check if main diagonal of sociomatrix is all 0s
            bool isAllZeros = true;
            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                if (mTable["Data"][i, i] > 0)
                {
                    isAllZeros = false;
                    break;
                }
            }

            Matrix Density = new Matrix(comNum, comNum);
            for (int i = 0; i < comNum; i++)
            {
                for (int j = 0; j < comNum; j++)
                {
                    double numNodes_m = 0;
                    double numNodes_k = 0;
                    double summationValue = 0;
                    double same_nodes = 0;
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (communities[i][k] == 0)
                            continue;
                        if (communities[i][k] == communities[j][k])
                            same_nodes++;
                        numNodes_m++;
                        numNodes_k = 0;
                        for (int z = 0; z < mTable["Data"].Rows; z++)
                        {
                            if (communities[j][z] == 0)
                                continue;
                            numNodes_k++;
                            summationValue += mTable["Data"][k, z];
                        }
                    }
                    Density[i, j] = summationValue;

                    double denominator;
                    if (isAllZeros)
                        denominator = (numNodes_m * numNodes_k) - same_nodes;
                    else
                        denominator = (numNodes_m * numNodes_k);

                    if (numNodes_m == 1 && numNodes_k == 1)
                        denominator = 1;
                    //Density[i, j] /= ((numNodes_m * numNodes_k) - same_nodes);
                    Density[i, j] = (denominator == 0) ? 0 : Density[i, j] / denominator;
                }
            }
            return Density;
        }


        public Matrix CalculateOverlapCommDensity(List<Clique> overlapComm)
        {
            if (_cliques == null)
                throw new Exception("No overlapping communities have been found");

            // Check if main diagonal of sociomatrix is all 0s
            bool isAllZeros = true;
            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                if (mTable["Data"][i, i] > 0)
                {
                    isAllZeros = false;
                    break;
                }
            }

            int cliqueNum = overlapComm.Count;
            Matrix Density = new Matrix(cliqueNum, cliqueNum);

            for (int i = 0; i < cliqueNum; i++)
            {
                for (int j = 0; j < cliqueNum; j++)
                {
                    double numNodes_m = 0;
                    double numNodes_k = 0;
                    double summationValue = 0;
                    double same_nodes = 0;
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (overlapComm[i].IntContains(k) == 0)
                            continue;
                        if (overlapComm[i].IntContains(k) == overlapComm[j].IntContains(k))
                            same_nodes++;
                        numNodes_m++;
                        numNodes_k = 0;
                        for (int z = 0; z < mTable["Data"].Rows; z++)
                        {
                            if (overlapComm[j].IntContains(z) == 0)
                                continue;
                            numNodes_k++;
                            summationValue += mTable["Data"][k, z];
                        }
                    }
                    Density[i, j] = summationValue;

                    double denominator;
                    if (isAllZeros)
                        denominator = (numNodes_m * numNodes_k) - same_nodes;
                    else
                        denominator = (numNodes_m * numNodes_k);

                    if (numNodes_m == 1 && numNodes_k == 1)
                        denominator = 1;
                    //Density[i, j] /= ((numNodes_m * numNodes_k) - same_nodes);
                    Density[i, j] = (denominator == 0) ? 0 : Density[i, j] / denominator;
                }
            }
            return Density;
        }

        //
        // Calculate Separation Coefficients
        //

        public double CalculateCliqueSC(CliqueCollection _cliques)
        {
            Matrix Density = CalculateCliqueDensity(_cliques);

            double sum = 0.0;
            for (int i = 0; i < Density.Rows; i++)
            {
                for (int j = 0; j < Density.Cols; j++)
                {
                    sum += (Density[i, i] - Density[i, j]);
                }
            }
            sum /= Density.Rows * (Density.Rows - 1);
            return sum;
        }

        public double CalculateBlockSC(List<List<int>> Blocks)
        {
            if (Blocks == null)
                throw new Exception("No blocks have been found");

            Matrix Density = CalculateBlockDensity(Blocks);

            double sum = 0.0;
            for (int i = 0; i < Density.Rows; i++)
            {
                for (int j = 0; j < Density.Cols; j++)
                {
                    sum += (Density[i, i] - Density[i, j]);
                }
            }
            sum /= Density.Rows * (Density.Rows - 1);
            return sum;
        }

        public double CalculateCommSC(List<int[]> communities)
        {
            if (communities == null)
                throw new Exception("No communities have been found");
            if (communities.Count == 1)
                return 0;

            Matrix Density = CalculateCommDensity(communities);

            double sum = 0.0;
            for (int i = 0; i < Density.Rows; i++)
            {
                for (int j = 0; j < Density.Cols; j++)
                {
                    sum += (Density[i, i] - Density[i, j]);
                }
            }
            sum /= Density.Rows * (Density.Rows - 1);
            return sum;
        }

        public double CalculateOverlapCommSC(List<Clique> overlapComm)
        {
            if (overlapComm == null)
                throw new Exception("No overlap communities have been found");

            Matrix Density = CalculateOverlapCommDensity(overlapComm);
            double sum = 0.0;
            for (int i = 0; i < Density.Rows; i++)
            {
                for (int j = 0; j < Density.Cols; j++)
                {
                    sum += (Density[i, i] - Density[i, j]);
                }
            }
            sum /= Density.Rows * (Density.Rows - 1);
            return sum;
        }


        //
        // Calculate Cohesion Matrices
        //
        public Matrix computeCliqueCohesiveMatrix()
        {
            Matrix cohesiveMatrix = new Matrix(_cliques.Count, _cliques.Count);

            // for external files
            if (cohesionFilename != null)
            {
                // Read matrix file
                Matrix ext = CohesionMatrix;

                for (int i = 0; i < _cliques.Count; i++)
                {
                    for (int j = 0; j < _cliques.Count; j++)
                    {
                        if (_cliques[i].Size == _cliques[j].Size && _cliques[i].Size == 1)
                        {
                            int member = _cliques[i].Members[0];
                            cohesiveMatrix[i, j] = ext[member, member];
                        }
                        else
                        {
                            double sum = 0.0;
                            foreach (int node_i in _cliques[i].Members)
                            {
                                foreach (int node_j in _cliques[j].Members)
                                {
                                    sum += ext[node_i, node_j];
                                }
                            }
                            sum /= (_cliques[i].Size * _cliques[j].Size);
                            cohesiveMatrix[i, j] = sum;
                        }
                    }
                }
            }


            // for non-external files
            else
            {
                double numNodes_m = 0;
                double numNodes_k = 0;
                double summationValue = 0;

                for (int i = 0; i < _cliques.Count; i++)
                {
                    for (int j = 0; j < _cliques.Count; j++)
                    {
                        numNodes_m = 0;
                        summationValue = 0;
                        for (int k = 0; k < mTable["SESE"].Rows; k++)
                        {
                            if (_cliques[i].IntContains(k) == 0)
                                continue;
                            numNodes_m++;
                            numNodes_k = 0;
                            for (int z = 0; z < mTable["SESE"].Rows; z++)
                            {
                                if (_cliques[j].IntContains(z) == 0)
                                    continue;
                                numNodes_k++;
                                summationValue += mTable["SESE"][k, z];
                            }
                        }

                        double divisor = numNodes_m * numNodes_k;
                        cohesiveMatrix[i, j] = summationValue;
                        cohesiveMatrix[i, j] /= divisor;
                    }
                }
            }

            return cohesiveMatrix;
        }

        public Matrix computeBlockCohesiveMatrix()
        {
            Matrix cohesiveMatrix = new Matrix(_blocks.Count, _blocks.Count);

            // for external files
            if (cohesionFilename != null)
            {
                // Read matrix file
                Matrix ext = CohesionMatrix;
                for (int i = 0; i < _blocks.Count; i++)
                {
                    for (int j = 0; j < _blocks.Count; j++)
                    {
                        if (_blocks[i].Size == _blocks[j].Size && _blocks[i].Size == 1)
                        {
                            int member = _blocks[i].Members[0];
                            cohesiveMatrix[i, j] = ext[member, member];
                        }
                        else
                        {
                            double sum = 0.0;
                            foreach (int node_i in _blocks[i].Members)
                            {
                                foreach (int node_j in _blocks[j].Members)
                                {
                                    sum += ext[node_i, node_j];
                                }
                            }
                            sum /= (_blocks[i].Size * _blocks[j].Size);
                            cohesiveMatrix[i, j] = sum;
                        }
                    }
                }
            }
            else
            {
                double numNodes_m = 0;
                double numNodes_k = 0;
                double summationValue = 0;

                for (int i = 0; i < _blocks.Count; i++)
                {
                    for (int j = 0; j < _blocks.Count; j++)
                    {
                        numNodes_m = 0;
                        summationValue = 0;
                        for (int k = 0; k < mTable["SESE"].Rows; k++)
                        {
                            if (_blocks[i].IntContains(k) == 0)
                                continue;
                            numNodes_m++;
                            numNodes_k = 0;
                            for (int z = 0; z < mTable["SESE"].Rows; z++)
                            {
                                if (_blocks[j].IntContains(z) == 0)
                                    continue;
                                numNodes_k++;
                                summationValue += mTable["SESE"][k, z];
                            }
                        }
                        double divisor = numNodes_m * numNodes_k;
                        cohesiveMatrix[i, j] = summationValue;
                        cohesiveMatrix[i, j] /= divisor;
                    }
                }
            }

            return cohesiveMatrix;
        }


        public Matrix computeCommCohesiveMatrix()
        {
            Matrix cohesiveMatrix = new Matrix(comNum, comNum);

            // for external files
            if (cohesionFilename != null)
            {
                // Read matrix file                
                Matrix ext = CohesionMatrix;

                for (int i = 0; i < comNum; i++)
                {
                    int size_i = 0;
                    for (int count = 0; count < communities[i].Length; count++)
                    {
                        if (communities[i][count] == 1)
                            size_i++;
                    }

                    for (int j = 0; j < comNum; j++)
                    {
                        int size_j = 0;
                        for (int count = 0; count < communities[j].Length; count++)
                        {
                            if (communities[j][count] == 1)
                                size_j++;
                        }

                        if (size_i == size_j && size_i == 1)
                        {
                            for (int temp = 0; temp < communities[i].Length; temp++)
                            {
                                int member;
                                if (communities[i][temp] == 1)
                                {
                                    member = temp;
                                    cohesiveMatrix[i, j] = ext[member, member];
                                }
                            }
                        }
                        else
                        {
                            double sum = 0.0;
                            for (int k = 0; k < communities[i].Length; k++)
                            {
                                if (communities[i][k] != 1)
                                    continue;
                                for (int l = 0; l < communities[j].Length; l++)
                                {
                                    if (communities[j][l] == 1)
                                        sum += ext[k, l];
                                }
                            }
                            sum /= (size_i * size_j);
                            cohesiveMatrix[i, j] = sum;
                        }
                    }
                }
            }
            else
            {
                double numNodes_m = 0;
                double numNodes_k = 0;
                double summationValue = 0;

                for (int i = 0; i < comNum; i++)
                {
                    for (int j = 0; j < comNum; j++)
                    {
                        numNodes_m = 0;
                        summationValue = 0;
                        for (int k = 0; k < mTable["SESE"].Rows; k++)
                        {
                            if (communities[i][k] == 0)
                                continue;
                            numNodes_m++;
                            numNodes_k = 0;
                            for (int z = 0; z < mTable["SESE"].Rows; z++)
                            {
                                if (communities[j][z] == 0)
                                    continue;
                                numNodes_k++;
                                summationValue += mTable["SESE"][k, z];
                            }
                        }

                        double divisor = numNodes_m * numNodes_k;
                        cohesiveMatrix[i, j] = summationValue;
                        cohesiveMatrix[i, j] /= divisor;
                    }
                }
            }

            return cohesiveMatrix;
        }

        public Matrix computeOverlapCommCohesiveMatrix(List<Clique> overlapComm)
        {
            Matrix cohesiveMatrix = new Matrix(overlapComm.Count, overlapComm.Count);

            // for external files
            if (cohesionFilename != null)
            {
                // Read matrix file
                Matrix ext = CohesionMatrix;

                for (int i = 0; i < overlapComm.Count; i++)
                {
                    for (int j = 0; j < overlapComm.Count; j++)
                    {
                        if (overlapComm[i].Size == overlapComm[j].Size && overlapComm[i].Size == 1)
                        {
                            int member = overlapComm[i].Members[0];
                            cohesiveMatrix[i, j] = ext[member, member];
                        }
                        else
                        {
                            double sum = 0.0;
                            foreach (int node_i in overlapComm[i].Members)
                            {
                                foreach (int node_j in overlapComm[j].Members)
                                {
                                    sum += ext[node_i, node_j];
                                }
                            }
                            sum /= (overlapComm[i].Size * overlapComm[j].Size);
                            cohesiveMatrix[i, j] = sum;
                        }
                    }
                }
            }
            else
            {
                double numNodes_m = 0;
                double numNodes_k = 0;
                double summationValue = 0;

                for (int i = 0; i < overlapComm.Count; i++)
                {
                    for (int j = 0; j < overlapComm.Count; j++)
                    {
                        numNodes_m = 0;
                        summationValue = 0;
                        for (int k = 0; k < mTable["SESE"].Rows; k++)
                        {
                            if (overlapComm[i].IntContains(k) == 0)
                                continue;
                            numNodes_m++;
                            numNodes_k = 0;
                            for (int z = 0; z < mTable["SESE"].Rows; z++)
                            {
                                if (overlapComm[j].IntContains(z) == 0)
                                    continue;
                                numNodes_k++;
                                summationValue += mTable["SESE"][k, z];
                            }
                        }

                        double divisor = numNodes_m * numNodes_k;
                        cohesiveMatrix[i, j] = summationValue;
                        cohesiveMatrix[i, j] /= divisor;
                    }
                }
            }

            return cohesiveMatrix;
        }



        public double CalculateCliqueGC()
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found");

            Matrix GC = computeCliqueCohesiveMatrix();

            double sum = 0.0;
            for (int i = 0; i < GC.Rows; i++)
            {
                for (int j = 0; j < GC.Cols; j++)
                {
                    sum += (GC[i, i] - GC[i, j]);
                }
            }
            sum /= GC.Rows * (GC.Rows - 1);
            return sum;
        }

        public double CalculateBlockGC()
        {
            if (Blocks == null)
                throw new Exception("No blocks have been found");

            Matrix GC = computeBlockCohesiveMatrix();

            double sum = 0.0;
            for (int i = 0; i < GC.Rows; i++)
            {
                for (int j = 0; j < GC.Cols; j++)
                {
                    sum += (GC[i, i] - GC[i, j]);
                }
            }
            sum /= GC.Rows * (GC.Rows - 1);
            return sum;
        }

        public double CalculateCommGC()
        {
            if (communities == null)
                throw new Exception("No communities have been found");
            if (communities.Count == 1)
                return 0;

            Matrix GC = computeCommCohesiveMatrix();

            double sum = 0.0;
            for (int i = 0; i < GC.Rows; i++)
            {
                for (int j = 0; j < GC.Cols; j++)
                {
                    sum += (GC[i, i] - GC[i, j]);
                }
            }
            sum /= GC.Rows * (GC.Rows - 1);
            return sum;
        }

        public double CalculateOverlapCommGC(List<Clique> overlapComm)
        {
            if (overlapComm == null)
                throw new Exception("No overlap communities have been found");

            Matrix GC = computeOverlapCommCohesiveMatrix(overlapComm);

            double sum = 0.0;
            for (int i = 0; i < GC.Rows; i++)
            {
                for (int j = 0; j < GC.Cols; j++)
                {
                    sum += (GC[i, i] - GC[i, j]);
                }
            }
            sum /= GC.Rows * (GC.Rows - 1);
            return sum;
        }




        //    public delegate CliqueCollection ParameterizedThreadStart(object obj, object obj);
        public CliqueCollection FindCliques(double cutoff, bool calcSE, double maxik, int year, int cmin, bool loadCOC, int cliqueOrder, bool kCliqueDiag)
        {
            maxik = GetRealDensity(maxik, year, "Data");

            if (calcSE)
            {
                LoadStructEquiv(maxik, year, "Data");
            }

            cmin = GetRealCMinMember(cmin, year);

            // Make sure that the binary matrix exists
            if (!mTable["Data"].IsSquareMatrix)
                throw new MatrixException("Cannot find cliques for non-square matrix.");

            //     Thread temp = new Thread(new ThreadStart( _cliques.LoadCliqueByCliqueOverlap));
            //temp.Start();
            //FindCliques(m);
            //while (!temp.IsAlive) ;
            //Thread.Sleep(100);
            //temp.Join();

            cliqueOrder = GetRealKCliqueValue(cliqueOrder, year);

            if (cliqueOrder > mTable["Data"].Rows - 1 || cliqueOrder < 1)
            {
                MessageBox.Show("K value in K-clique order cannot be less than 1 or greater than rows - 1. Now assume k is 1.");
                cliqueOrder = 1;
            }

            if (cliqueOrder == 1)
            {
                SymmetricBinaryMatrix MBinary = new SymmetricBinaryMatrix(mTable["Data"], cutoff, cet);
                _cliques = new CliqueCollection(MBinary, cmin);
            }
            else
            {

                mTable["Temp"] = new Matrix(mTable["Data"]);
                Matrix mat = new Matrix(mTable["Data"]);
                Matrix sum = new Matrix(mTable["Data"]);
                Matrix power = new Matrix(mat);

                if (!kCliqueDiag)
                    for (int i = 2; i <= cliqueOrder; ++i)
                    {
                        power *= mat;

                        sum += power;

                        Application.DoEvents();
                    }

                if (kCliqueDiag)
                    for (int i = 2; i <= cliqueOrder; ++i)
                    {
                        power *= mat;
                        power.ZeroDiagonal();
                        sum += power;

                        Application.DoEvents();
                    }

                sum.CloneTo(mTable["Temp"]);

                SymmetricBinaryMatrix MBinary = new SymmetricBinaryMatrix(sum, cutoff, cet);
                _cliques = new CliqueCollection(MBinary, cmin);

            }//else if k-clique order

            if (loadCOC)
            {
                //Thread s = new Thread(_cliques.LoadCliqueByCliqueOverlap);
                //s.Start();
                _cliques.LoadCliqueByCliqueOverlap();
            }

            mTable["Overlap"] = _cliques.CliqueOverlap;
            mTable["OverlapDiag"] = new Matrix(mTable["Overlap"]);
            for (int i = 0; i < mTable["OverlapDiag"].Rows; ++i)
            {
                for (int j = 0; j < mTable["OverlapDiag"].Cols; ++j)
                {
                    if (i == j) continue;
                    mTable["OverlapDiag"][i, j] /= mTable["OverlapDiag"][i, i];
                }
                mTable["OverlapDiag"][i, i] = 1;
            }


            if (calcSE)
            {
                foreach (Clique c in _cliques)
                {
                    c.ComputeCohesion(mTable["SESE"]);
                }
            }

            cliqueSize = mTable["Data"].Rows;
            return _cliques;
        }

        public BlockCollection FindBlocks(double cutoff, List<List<int>> Blocks, int bmin)
        {
            Block newBloc;
            List<Block> blocList = new List<Block>();
            foreach (List<int> bloc in Blocks)
            {
                newBloc = new Block(bloc, mTable["Data"].Rows);
                blocList.Add(newBloc);
            }
            SymmetricBinaryMatrix MBinary = new SymmetricBinaryMatrix(mTable["Data"], cutoff);
            _blocks = new BlockCollection(MBinary, bmin, blocList);
            blockSize = mTable["Data"].Rows;
            return _blocks;
        }

        public CommCollection FindComm(double cutoff, List<int[]> Comms, int cmin)  // TODO: Convert array to list and finish up with comms
        {

            if (Comms == null)
                throw new Exception("No Communities found!");

            SymmetricBinaryMatrix MBinary = new SymmetricBinaryMatrix(mTable["Data"], cutoff);
            List<Comm> commList = new List<Comm>();
            Comm newComm;

            List<int> tempComm = new List<int>();
            for (int i = 0; i < comNum; ++i)
            {
                tempComm.Clear();
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    if (Comms[i][j] != 0)
                        tempComm.Add(Comms[i][j] - 1);
                }
                tempComm.Sort();
                newComm = new Comm(tempComm, mTable["Data"].Rows);
                commList.Add(newComm);
            }

            commList.Sort();
            _communities = new CommCollection(MBinary, cmin, commList);

            // compute actual communities
            // should already be computed if (Comms != null)
            /*
            convertMatrixToProperInput();
            mainCommunityExtractionMethod();
            createCommunitiesFromGroup();
            */
            for (int i = 0; i < comNum; i++)
            {
                List<int> tempList = new List<int>();
                for (int j = 0; j < comSize; j++)
                {
                    tempList.Add(communities[i][j]);
                }
                Comm tempCommunity = new Comm(tempList, group.Length);
                _communities.new_communities.Add(tempCommunity);
            }

            return _communities;
        }

        public void LoadClosenessDistance(string m)
        {
            if (m == "ClosenessDistance")
            {
                m = "Temp";
                mTable[m] = new Matrix(mTable["ClosenessDistance"].Rows);
                mTable["ClosenessDistance"].CloneTo(mTable[m]);
                for (int i = 0; i < mTable[m].Rows; ++i)
                    mTable[m].RowLabels[i] = mTable[m].ColLabels[i] = mTable["ClosenessDistance"].ColLabels[i];
            }

            if (!(mTable[m].IsSquareMatrix))
                throw new Exception("Cannot calculation Closeness Distance on non-square matrix!");

            bool isBinary = mTable[m].IsBinaryMatrix;

            mTable["ClosenessDistance"] = new Matrix(mTable[m].Rows);
            mTable["ClosenessDistance"].Clear();

            Queue<int> q = new Queue<int>();
            Queue<int> q2 = new Queue<int>();

            // Add every possible node to the list
            for (int i = 0; i < mTable[m].Rows; ++i)
                for (int j = 0; j < mTable[m].Rows; ++j)
                    q.Enqueue(i * mTable[m].Rows + j);

            // Go until each one has been assigned a value or we run out
            Matrix Power = new Matrix(mTable[m]);
            for (int i = 1; i < mTable[m].Rows && q.Count > 0; ++i)
            {
                while (q.Count > 0)
                {
                    int current = q.Dequeue();
                    int row = current / mTable[m].Rows;
                    int col = current % mTable[m].Rows;
                    if (Power[row, col] > 0)
                    {
                        if (isBinary)
                            mTable["ClosenessDistance"][row, col] = i;
                        else
                            mTable["ClosenessDistance"][row, col] = i - 1 + Power[row, col];
                    }
                    else
                    {
                        // Enqueue it to try again next time
                        q2.Enqueue(current);
                    }
                }
                Algorithms.Swap(ref q, ref q2);
                Power *= mTable[m];
            }

            double maxValue = Algorithms.MaxValue(Power);
            for (int i = 0; i < mTable["ClosenessDistance"].Rows; ++i)
                for (int j = i; j < mTable["ClosenessDistance"].Cols; ++j)
                    if (mTable["ClosenessDistance"][i, j] == 0)
                        if (isBinary)
                            mTable["ClosenessDistance"][i, j] = mTable[m].Rows;
                        else
                            mTable["ClosenessDistance"][i, j] = mTable[m].Rows * maxValue;

            // Fix up the diagonal entires to be 0
            for (int i = 0; i < mTable["ClosenessDistance"].Rows; ++i)
                mTable["ClosenessDistance"][i, i] = 1;

            // And make it symmetrical
            for (int i = 0; i < mTable["ClosenessDistance"].Rows; ++i)
                for (int j = i; j < mTable["ClosenessDistance"].Cols; ++j)
                    if (mTable["ClosenessDistance"][i, j] < mTable["ClosenessDistance"][j, i])
                        mTable["ClosenessDistance"][j, i] = mTable["ClosenessDistance"][i, j];
                    else
                        mTable["ClosenessDistance"][i, j] = mTable["ClosenessDistance"][j, i];
        }

        public void LoadTriadic(string m, int year)
        {
            int n = mTable[m].Rows;
            int triadCount = n * (n - 1) * (n - 2) / 6;

            mTable["Triadic"] = new Matrix(triadCount, 14); // year, triad id, ab, ac, bc
            mTable["Triadic"].ColLabels[0] = "Year";
            mTable["Triadic"].ColLabels[1] = "Triad ID";
            mTable["Triadic"].ColLabels[2] = "A --> B";
            mTable["Triadic"].ColLabels[3] = "A --> C";
            mTable["Triadic"].ColLabels[4] = "B --> A";
            mTable["Triadic"].ColLabels[5] = "B --> C";
            mTable["Triadic"].ColLabels[6] = "C --> A";
            mTable["Triadic"].ColLabels[7] = "C --> B";
            mTable["Triadic"].ColLabels[8] = "ABC";
            mTable["Triadic"].ColLabels[9] = "ACB";
            mTable["Triadic"].ColLabels[10] = "BAC";
            mTable["Triadic"].ColLabels[11] = "BCA";
            mTable["Triadic"].ColLabels[12] = "CAB";
            mTable["Triadic"].ColLabels[13] = "CBA";

            int row = 0;
            for (int i = 0; i < n; ++i)
            {
                for (int j = i + 1; j < n; ++j)
                {
                    for (int k = j + 1; k < n; ++k)
                    {
                        mTable["Triadic"][row, 0] = year;
                        mTable["Triadic"][row, 1] = double.Parse((i + 1).ToString() + "," + (j + 1).ToString() + "," + (k + 1).ToString());
                        mTable["Triadic"][row, 2] = mTable[m][i, j] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 3] = mTable[m][i, k] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 4] = mTable[m][j, i] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 5] = mTable[m][j, k] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 6] = mTable[m][k, i] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 7] = mTable[m][k, j] > 0 ? 1 : 0;
                        mTable["Triadic"][row, 8] = GetRoleEquivalenceType(GetRelationshipType(i, j, m), GetRelationshipType(i, k, m), GetRelationshipType(j, k, m));
                        mTable["Triadic"][row, 9] = GetRoleEquivalenceType(GetRelationshipType(i, k, m), GetRelationshipType(i, j, m), GetRelationshipType(k, j, m));
                        mTable["Triadic"][row, 10] = GetRoleEquivalenceType(GetRelationshipType(j, i, m), GetRelationshipType(j, k, m), GetRelationshipType(i, k, m));
                        mTable["Triadic"][row, 11] = GetRoleEquivalenceType(GetRelationshipType(j, k, m), GetRelationshipType(j, i, m), GetRelationshipType(k, i, m));
                        mTable["Triadic"][row, 12] = GetRoleEquivalenceType(GetRelationshipType(k, i, m), GetRelationshipType(k, j, m), GetRelationshipType(i, j, m));
                        mTable["Triadic"][row, 13] = GetRoleEquivalenceType(GetRelationshipType(k, j, m), GetRelationshipType(k, i, m), GetRelationshipType(j, i, m));

                        ++row;
                    }
                }
            }
        }

        protected int GetRelationshipType(int i, int j, string m)
        {
            if (mTable[m][i, j] > 0 && mTable[m][j, i] > 0)
                return 3;
            else if (mTable[m][i, j] > 0)
                return 1;
            else if (mTable[m][j, i] > 0)
                return 2;

            return 0;
        }

        protected int GetRelationshipType(int i, int j, Matrix M)
        {
            if (M[i, j] > 0 && M[j, i] > 0)
                return 3;
            else if (M[i, j] > 0)
                return 1;
            else if (M[j, i] > 0)
                return 2;

            return 0;
        }

        protected int GetRoleEquivalenceType(int AB, int AC, int BC)
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

        public void LoadRoleEquivalence(string m)
        {
            /*
            mTable["Temp"] = new Matrix(mTable[m].Rows);
            mTable[m].CloneTo(mTable["Temp"]);
            m = "Temp";
            */
            mTable["RoleEquiv"] = MatrixComputations.RoleEquivalence(mTable["Data"]);
        }


        public void LoadStructEquiv(double maxik, int year, string m)
        {
            // may not need
            /*
            if (m == "SEC" || m == "SEE" || m == "SESE")
            {
                mTable["Temp"] = new Matrix(mTable[m].Rows);
                mTable[m].CloneTo(mTable["Temp"]);
                m = "Temp";
            }
            */
            m = "Data"; // may need to make m = "Data"
            maxik = GetRealDensity(maxik, year, m);

            mTable["SEC"] = MatrixComputations.StructuralEquivalenceCorrelation(mTable[m]);
            mTable["SEE"] = MatrixComputations.StructuralEquivalenceEuclidean(mTable[m]);

            if (maxik >= 0.0)
            {
                mTable["SESE"] = MatrixComputations.StructuralEquivalenceStandardizedEuclidean(mTable[m], maxik);
                SESEmatrix = new Matrix(mTable["SESE"]);
            }
        }

        public delegate Matrix CONCORConvergenceFunction(Matrix m);

        public List<List<string>> RecursiveCONCOR(double posCutoff, Matrix SEC, CONCORConvergenceFunction f, int maxNoSteps)
        {
            const int ConvIterations = 25;

            List<List<string>> Blocks = new List<List<string>>();

            Matrix M = new Matrix(SEC);

            for (int count = 0; count < ConvIterations; ++count)
                SEC = f(SEC);

            List<Matrix> Submatrices = new List<Matrix>();

            bool[] used = new bool[SEC.Rows];
            Array.Clear(used, 0, used.Length);

            List<string> prospectiveBloc;
            for (int i = 0; i < SEC.Rows; ++i)
            {
                if (used[i])
                    continue;

                // Test this row against its friends (then test each of them in turn)
                prospectiveBloc = new List<string>();
                List<int> subMatrixRemoval = new List<int>();
                prospectiveBloc.Add(SEC.RowLabels[i]);
                subMatrixRemoval.Add(i);
                for (int j = i + 1; j < SEC.Cols; ++j)
                {
                    if (used[j])
                        continue;

                    if (SEC[i, j] > posCutoff)
                    {
                        prospectiveBloc.Add(SEC.RowLabels[j]);
                        subMatrixRemoval.Add(j);
                    }
                }

                // Now test this bloc for consistency among the remaining members
                if (prospectiveBloc.Count > 2)
                {
                    for (int j = 1; j < prospectiveBloc.Count; ++j)
                    {
                        for (int k = j + 1; k < prospectiveBloc.Count; ++k)
                        {
                            if (SEC[subMatrixRemoval[j], subMatrixRemoval[k]] < posCutoff)
                            {
                                // This pair can't be in the bloc--remove them
                                prospectiveBloc.RemoveAt(k);
                                subMatrixRemoval.RemoveAt(k);
                                --j;
                                break;
                            }
                        }
                    }
                }

                // All right, we passed the bloc inspection, so add it
                if (prospectiveBloc.Count >= 2)
                {
                    foreach (int k in subMatrixRemoval)
                        used[k] = true;

                    Blocks.Add(prospectiveBloc);
                    Submatrices.Add(M.SubmatrixWithRows(subMatrixRemoval));
                }
            }

            prospectiveBloc = new List<string>();
            for (int i = 0; i < SEC.Rows; ++i)
                if (!used[i])
                    prospectiveBloc.Add(SEC.RowLabels[i]);

            if (prospectiveBloc.Count > 0)
                Blocks.Add(prospectiveBloc);

            if (Submatrices.Count <= 1 || maxNoSteps <= 1)
                return Blocks;
            else
            {
                Blocks.Clear();
                for (int i = 0; i < Submatrices.Count; ++i)
                    Blocks.AddRange(RecursiveCONCOR(posCutoff, Submatrices[i], f, maxNoSteps - 1));

                if (prospectiveBloc.Count > 0)
                    Blocks.Add(prospectiveBloc);

                return Blocks;
            }
        }

        public void CONCOR(double posCutoff, bool multiple, CONCORConvergenceFunction f, int maxNoSteps)
        {
            // Use SE correlation to find blocks 
            Matrix SEC = new Matrix(multiple ? mTable["SEC"] : mTable["Data"]);

            List<List<string>> tmpBlocks = RecursiveCONCOR(posCutoff, SEC, f, maxNoSteps);
            Blocks = new List<List<int>>();
            foreach (List<string> tmpBlock in tmpBlocks)
            {
                List<int> block = new List<int>(tmpBlock.Count);
                foreach (string s in tmpBlock)
                    block.Add(SEC.RowLabels[s]);
                Blocks.Add(block);
            }
            Blocks.Sort(new BlockComparer());
        }

        public void LoadBlockPartitionMatrix()
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            Matrix BPI = mTable.AddMatrix("BlockPartitionI", mTable["Data"].Rows, mTable["Data"].Cols);
            Matrix BPS = mTable.AddMatrix("BlockPartitionS", mTable["Data"].Rows, mTable["Data"].Cols);

            int rowAdd = 0, colAdd = 0;
            for (int row = 0; row < Blocks.Count; ++row)
            {
                colAdd = 0;
                for (int col = 0; col < Blocks.Count; ++col)
                {
                    for (int i = 0; i < Blocks[row].Count; ++i)
                    {
                        for (int j = 0; j < Blocks[col].Count; ++j)
                        {
                            BPS[i + rowAdd, j + colAdd] = mTable["Data"][Blocks[row][i], Blocks[col][j]];
                            BPI[i + rowAdd, j + colAdd] = double.Parse(((row + 1).ToString() + (col + 1).ToString()));
                            // this is not efficient
                            BPI.ColLabels[j + colAdd] = mTable["Data"].ColLabels[Blocks[col][j]];
                            BPS.ColLabels[j + colAdd] = mTable["Data"].ColLabels[Blocks[col][j]];
                        }
                        BPI.RowLabels[i + rowAdd] = mTable["Data"].ColLabels[Blocks[row][i]];
                        BPS.RowLabels[i + rowAdd] = mTable["Data"].ColLabels[Blocks[row][i]];

                    }
                    colAdd += Blocks[col].Count;
                }
                rowAdd += Blocks[row].Count;
            }

        }

        public void LoadClusterPartitionMatrix()
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            Matrix CP = mTable.AddMatrix("ClusterPartition", mTable["Data"].Rows, mTable["Data"].Cols);

            int rowAdd = 0, colAdd = 0;
            for (int row = 0; row < Blocks.Count; ++row)
            {
                colAdd = 0;
                for (int col = 0; col < Blocks.Count; ++col)
                {
                    for (int i = 0; i < Blocks[row].Count; ++i)
                    {
                        for (int j = 0; j < Blocks[col].Count; ++j)
                        {
                            CP[i + rowAdd, j + colAdd] = mTable["Data"][Blocks[row][i], Blocks[col][j]];
                            // this is not efficient 
                            CP.ColLabels[j + colAdd] = mTable["Data"].ColLabels[Blocks[col][j]];
                        }
                        CP.RowLabels[i + rowAdd] = mTable["Data"].ColLabels[Blocks[row][i]];

                    }
                    colAdd += Blocks[col].Count;
                }
                rowAdd += Blocks[row].Count;
            }

        }

        public void LoadClustering(string method, int maxcluster, int year, double maxik)
        {
            Blocks = new List<List<int>>();
            string m = "Data";
            maxik = GetRealDensity(maxik, year, m);

            int n = mTable[m].Rows;

            if (method == "Correlation")
                mTable["Temp"] = MatrixComputations.StructuralEquivalenceCorrelation(mTable[m]);
            else if (method == "ED")
                mTable["Temp"] = MatrixComputations.StructuralEquivalenceEuclidean(mTable[m]);
            else
                mTable["Temp"] = MatrixComputations.StructuralEquivalenceStandardizedEuclidean(mTable[m], maxik);


            if (method != "ED")
            {
                for (int cluster = n; cluster > maxcluster; cluster--)
                {
                    int firstnode = 0, secondnode = 1;
                    double max = mTable["Temp"][0, 1];
                    for (int i = 0; i < n; i++)
                        for (int j = i + 1; j < n; j++)
                            if (mTable["Temp"][i, j] > max)
                            {
                                max = mTable["Temp"][i, j];
                                firstnode = i;
                                secondnode = j;
                            }
                            else if (mTable["Temp"][i, j] == max && i == firstnode)
                            {
                                int blockSizeOfJ = -1; //if not found, return negative value
                                int blockSizeOfSecondNode = -1;
                                foreach (List<int> block in Blocks)
                                {
                                    if (block.Contains(j))
                                        blockSizeOfJ = block.Count;
                                    if (block.Contains(secondnode))
                                        blockSizeOfSecondNode = block.Count;
                                }
                                if (blockSizeOfJ < blockSizeOfSecondNode)
                                    secondnode = j;
                            }

                    if (Blocks.Count == 0)
                    {
                        List<int> newBlock = new List<int>();
                        newBlock.Add(firstnode);
                        newBlock.Add(secondnode);
                        Blocks.Add(newBlock);
                    }
                    else
                    {
                        int found1 = -1; //block index of firstnode
                        int found2 = -1; //block index of secondnode
                        for (int i = 0; i < Blocks.Count && (found1 == -1 || found2 == -1); i++)// loop until firstnode and second node are found
                        {
                            if (Blocks[i].Contains(firstnode))
                                found1 = i;
                            if (Blocks[i].Contains(secondnode))
                                found2 = i;
                        }

                        if (found1 != -1 && found2 != -1)
                        {
                            //   MessageBox.Show("firstnode = " + firstnode + "secondenode = " + secondnode + "found1 = " + found1 + "found2 = " + found2);
                            Blocks[found1].AddRange(Blocks[found2]);
                            Blocks.RemoveAt(found2);
                        }
                        else if (found1 != -1 && found2 == -1)
                        {
                            Blocks[found1].Add(secondnode);
                        }
                        else if (found1 == -1 && found2 != -1)
                        {
                            Blocks[found2].Add(firstnode);
                        }
                        else if (found1 == -1 && found2 == -1) //none of first or second node found
                        {
                            List<int> newBlock = new List<int>();
                            newBlock.Add(firstnode);
                            newBlock.Add(secondnode);
                            Blocks.Add(newBlock);
                        }
                    }

                    //update Temp Matrix
                    if (secondnode != 0) //it should not be zero
                    {
                        for (int i = 0; i < n; i++)
                        {
                            if (mTable["Temp"][firstnode, i] < mTable["Temp"][secondnode, i])
                            {
                                mTable["Temp"][firstnode, i] = mTable["Temp"][secondnode, i];
                                mTable["Temp"][i, firstnode] = mTable["Temp"][firstnode, i];
                            }
                        }

                        for (int i = 0; i < n; i++)
                        {
                            if (i == secondnode) continue;
                            mTable["Temp"][i, secondnode] = -int.MaxValue;
                            mTable["Temp"][secondnode, i] = -int.MaxValue;
                        }
                    }

                }//for each cluster


            }
            else //if (method == "ED")
            {
                for (int cluster = n; cluster > maxcluster; cluster--)
                {
                    int firstnode = 0, secondnode = 1;
                    double min = mTable["Temp"][0, 1];
                    for (int i = 0; i < n; i++)
                        for (int j = i + 1; j < n; j++)
                            if (mTable["Temp"][i, j] < min)
                            {
                                min = mTable["Temp"][i, j];
                                firstnode = i;
                                secondnode = j;
                            }
                            else if (mTable["Temp"][i, j] == min && i == firstnode)
                            {
                                int blockSizeOfJ = -1; //if not found, return negative value
                                int blockSizeOfSecondNode = -1;
                                foreach (List<int> block in Blocks)
                                {
                                    if (block.Contains(j))
                                        blockSizeOfJ = block.Count;
                                    if (block.Contains(secondnode))
                                        blockSizeOfSecondNode = block.Count;
                                }
                                if (blockSizeOfJ < blockSizeOfSecondNode)
                                    secondnode = j;
                            }

                    if (Blocks.Count == 0)
                    {
                        List<int> newBlock = new List<int>();
                        newBlock.Add(firstnode);
                        newBlock.Add(secondnode);
                        Blocks.Add(newBlock);
                    }
                    else
                    {
                        int found1 = -1; //block index of firstnode
                        int found2 = -1; //block index of secondnode
                        for (int i = 0; i < Blocks.Count && (found1 == -1 || found2 == -1); i++)// loop until firstnode and second node are found
                        {
                            if (Blocks[i].Contains(firstnode))
                                found1 = i;
                            if (Blocks[i].Contains(secondnode))
                                found2 = i;
                        }

                        if (found1 != -1 && found2 != -1)
                        {
                            //   MessageBox.Show("firstnode = " + firstnode + "secondenode = " + secondnode + "found1 = " + found1 + "found2 = " + found2);
                            Blocks[found1].AddRange(Blocks[found2]);
                            Blocks.RemoveAt(found2);
                        }
                        else if (found1 != -1 && found2 == -1)
                        {
                            Blocks[found1].Add(secondnode);
                        }
                        else if (found1 == -1 && found2 != -1)
                        {
                            Blocks[found2].Add(firstnode);
                        }
                        else if (found1 == -1 && found2 == -1) //none of first or second node found
                        {
                            List<int> newBlock = new List<int>();
                            newBlock.Add(firstnode);
                            newBlock.Add(secondnode);
                            Blocks.Add(newBlock);
                        }
                    }

                    //update Temp Matrix
                    if (secondnode != 0) //it should not be zero
                    {
                        for (int i = 0; i < n; i++)
                        {
                            if (mTable["Temp"][firstnode, i] < mTable["Temp"][secondnode, i])
                            {
                                mTable["Temp"][firstnode, i] = mTable["Temp"][secondnode, i];
                                mTable["Temp"][i, firstnode] = mTable["Temp"][firstnode, i];
                            }
                        }

                        for (int i = 0; i < n; i++)
                        {
                            if (i == secondnode) continue;
                            mTable["Temp"][i, secondnode] = -int.MaxValue;
                            mTable["Temp"][secondnode, i] = -int.MaxValue;
                        }
                    }

                }//for each cluster


            }


            //add cluster for leftout node
            for (int i = 0; i < n; ++i)
            {
                bool found = false;
                foreach (List<int> block in Blocks)
                    if (block.Contains(i))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                {
                    List<int> newBlock = new List<int>();
                    newBlock.Add(i);
                    Blocks.Add(newBlock);
                }
            }

            Blocks.Sort(new BlockComparer());
        }



        public void LoadBlockMatrices(double density, int year)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            //FindBlocks(cutoff, Blocks, _minCliqueSize);
            LoadBlockPartitionMatrix();
            LoadStructEquiv(density, year, "Data");

            int B = Blocks.Count;

            Matrix D = mTable.AddMatrix("DensityBlockMatrix", B, B);
            Matrix RD = mTable.AddMatrix("RelativeDensityBlockMatrix", B, B);
            Matrix BC = mTable.AddMatrix("BlockCohesivenessMatrix", B, B);

            double d = MatrixComputations.Density(mTable["Data"], density);

            int rowCount = 0;
            for (int i = 0; i < B; ++i)
            {
                int colCount = 0;
                for (int j = 0; j < B; ++j)
                {
                    double numNodes_m = 0;
                    double numNodes_k = 0;
                    double same_nodes = 0;
                    double summationValue = 0;
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (!Blocks[i].Contains(k))
                            continue;
                        if (Blocks[i].Contains(k) && Blocks[j].Contains(k))
                            same_nodes++;
                        numNodes_m++;
                        numNodes_k = 0;
                        for (int z = 0; z < mTable["Data"].Rows; z++)
                        {
                            if (!Blocks[j].Contains(z))
                                continue;
                            numNodes_k++;
                            summationValue += mTable["Data"][k, z];
                        }
                    }
                    D[i, j] = summationValue;
                    double denominator = (numNodes_m * numNodes_k) - same_nodes;
                    if (numNodes_m == 1 && numNodes_k == 1)
                        denominator = 1;
                    //Density[i, j] /= ((numNodes_m * numNodes_k) - same_nodes);
                    D[i, j] = (denominator == 0) ? 0 : D[i, j] / denominator;

                    //D[i, j] = MatrixComputations.SubmatrixDensity(mTable["BlockPartitionS"], rowCount, rowCount + Blocks[i].Count, colCount, colCount + Blocks[j].Count , density, i == j ? true : false);
                    RD[i, j] = D[i, j] / d;
                    BC[i, j] = MatrixComputations.SubmatrixDensity(mTable["SESE"], rowCount, rowCount + Blocks[i].Count, colCount, colCount + Blocks[j].Count, density, i == j ? true : false);
                    colCount += Blocks[j].Count;
                }
                rowCount += Blocks[i].Count;
                D.RowLabels[i] = D.ColLabels[i] = RD.RowLabels[i] = RD.ColLabels[i] = BC.RowLabels[i] = BC.ColLabels[i] = (i + 1).ToString();
            }

        }

        public void LoadClusterMatrices(double density, int year)
        {
            if (Blocks == null)
                throw new Exception("You must find the blocks before you can partition them!");

            LoadClusterPartitionMatrix();
            LoadStructEquiv(density, year, "Data");

            int B = Blocks.Count;

            Matrix CD = mTable.AddMatrix("DensityClusterMatrix", B, B);
            Matrix CRD = mTable.AddMatrix("RelativeDensityClusterMatrix", B, B);
            Matrix CC = mTable.AddMatrix("ClusterCohesivenessMatrix", B, B);

            double d = MatrixComputations.Density(mTable["Data"], density);
            int rowCount = 0;
            for (int i = 0; i < B; ++i)
            {
                int colCount = 0;
                for (int j = 0; j < B; ++j)
                {
                    CD[i, j] = MatrixComputations.SubmatrixDensity(mTable["ClusterPartition"], rowCount, rowCount + Blocks[i].Count, colCount, colCount + Blocks[j].Count, density, i == j ? true : false);
                    CRD[i, j] = CD[i, j] / d;
                    CC[i, j] = MatrixComputations.SubmatrixDensity(mTable["SESE"], rowCount, rowCount + Blocks[i].Count, colCount, colCount + Blocks[j].Count, density, i == j ? true : false);
                    colCount += Blocks[j].Count;
                }
                rowCount += Blocks[i].Count;
                CD.RowLabels[i] = CD.ColLabels[i] = CRD.RowLabels[i] = CRD.ColLabels[i] = CC.RowLabels[i] = CC.ColLabels[i] = (i + 1).ToString();
            }
        }

        public void LoadIntercliqueDistance(double maxik, int year)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");

            maxik = GetRealDensity(maxik, year, "Data");

            LoadStructEquiv(maxik, year, "Data");
            mTable["ICD"] = new Matrix(_cliques.Count);

            for (int i = 1; i <= _cliques.Count; ++i)
                mTable["ICD"].RowLabels[i - 1] = mTable["ICD"].ColLabels[i - 1] = i.ToString();

            for (int l = 0; l < _cliques.Count; ++l)
            {
                for (int m = 0; m < _cliques.Count; ++m)
                {
                    if (l == m)
                    {
                        mTable["ICD"][l, m] = 1.0;
                        continue;
                    }
                    double coisum = 0.0, sesum = 0.0;
                    for (int i = 0; i < mTable["Data"].Rows; ++i)
                    {
                        for (int j = 0; j < mTable["Data"].Rows; ++j)
                        {
                            if (_cliques[l].Contains(i) && _cliques[m].Contains(j)
                                || _cliques[l].Contains(j) && _cliques[m].Contains(i))
                            {
                                coisum += 1.0;
                                sesum += mTable["SESE"][i, j];
                            }
                        }
                    }
                    mTable["ICD"][l, m] = sesum / coisum;
                }
            }
        }


        public void LoadDependency(string m, int maxN, double maxik, int year, bool zeroDiagonal, bool reachSum)
        {
            LoadReachability(maxN, reachSum, zeroDiagonal, "Data", year, false);
            //if (m == "Dependency")
            //{
            //    m = "Temp";
            //    mTable[m] = new Matrix(mTable["Dependency"].Rows - 3);
            //    mTable["Dependency"].CloneTo(mTable[m]);
            //    for (int i = 0; i < mTable[m].Rows; ++i)
            //        mTable[m].RowLabels[i] = mTable[m].ColLabels[i] = mTable["Dependency"].ColLabels[i];
            //}


            maxik = GetRealDensity(maxik, year, "Data");


            if (zeroDiagonal)
            {
                // Code for getting new maximum k value ignoring the diagonal
                String temp = "Temp";
                mTable[temp] = new Matrix(mTable["Data"]);
                mTable[temp].ZeroDiagonal();
                maxik = GetRealDensity(-2.0, year, temp);


                // Set up final matrix
                int N = mTable["Data"].Rows;
                mTable["Dependency"] = new Matrix(mTable["Data"].Rows + 1);
                for (int i = 0; i < mTable["Data"].Rows; ++i)
                    for (int j = 0; j < mTable["Data"].Cols; ++j)
                        mTable["Dependency"][i, j] = (N * maxik - 1) * mTable["Reachability"][i, j] /
                                                        (maxik * Math.Pow((N * maxik), maxN));

                // Now do the calculations

                // This is for OUTDi
                for (int row = 0; row < mTable["Data"].Rows; ++row)
                {
                    double rowSum = 0.0;
                    for (int col = 0; col < mTable["Data"].Cols; ++col)
                    {
                        rowSum += mTable["Reachability"][row, col];
                    }
                    rowSum *= (1 - maxik * (N - 1));
                    rowSum /= (maxik * (N - 1) - Math.Pow(maxik * (N - 1), maxN + 1));

                    mTable["Dependency"][row, mTable["Data"].Rows] = rowSum;
                }

                // This is for ONDj
                for (int col = 0; col < mTable["Data"].Rows; ++col)
                {
                    double colSum = 0.0;
                    for (int row = 0; row < mTable["Data"].Rows; ++row)
                    {
                        colSum += mTable["Reachability"][row, col];
                    }
                    colSum *= (1 - maxik * (N - 1));
                    colSum /= (maxik * (N - 1) - Math.Pow(maxik * (N - 1), maxN + 1));
                    mTable["Dependency"][mTable["Data"].Rows, col] = colSum;
                }
            }

            else
            {

                // Set up final matrix
                int N = mTable["Data"].Rows;
                mTable["Dependency"] = new Matrix(mTable["Data"].Rows + 1);
                for (int i = 0; i < mTable["Data"].Rows; ++i)
                    for (int j = 0; j < mTable["Data"].Cols; ++j)
                        mTable["Dependency"][i, j] = (N * maxik - 1) * mTable["Reachability"][i, j] /
                                                        (maxik * Math.Pow((N * maxik), maxN));

                // This is for OUTDi
                for (int row = 0; row < mTable["Data"].Rows; ++row)
                {
                    double rowSum = 0.0;
                    for (int col = 0; col < mTable["Data"].Cols; ++col)
                    {
                        rowSum += mTable["Reachability"][row, col];
                    }
                    rowSum *= (N * (1 - maxik * N));
                    rowSum /= (maxik * N * (1 - Math.Pow((maxik * N), maxN + 1)));
                    mTable["Dependency"][row, mTable["Data"].Rows] = rowSum;
                }

                // This is for ONDj
                for (int col = 0; col < mTable["Data"].Rows; ++col)
                {
                    double colSum = 0.0;
                    for (int row = 0; row < mTable["Data"].Rows; ++row)
                    {
                        colSum += mTable["Reachability"][row, col];
                    }
                    colSum *= (N * (1 - maxik * N));
                    colSum /= (maxik * N * (1 - Math.Pow((maxik * N), maxN + 1)));
                    mTable["Dependency"][mTable["Data"].Rows, col] = colSum;
                }

            }

            //mTable[m].Unstandardize();



            // Setup labels
            mTable["Dependency"].RowLabels.CopyFrom(mTable["Data"].RowLabels);
            mTable["Dependency"].ColLabels.CopyFrom(mTable["Data"].ColLabels);

            mTable["Dependency"].ColLabels[mTable["Dependency"].Cols - 1] = "OUTDi";

            mTable["Dependency"].RowLabels[mTable["Dependency"].Rows - 1] = "ONDj";



            // SYSIN
            mTable["Dependency"][mTable["Dependency"].Rows - 1, mTable["Dependency"].Cols - 1] = GetSYSIN(zeroDiagonal, maxik, year, maxN);

        }

        public void LoadUnitDependency(int year)
        {
            if (mTable["Dependency"] == null)
                throw new IOException("Cannot save dependency data before it has been loaded!");

            String m = "NatDep";
            int numNodes = mTable["Data"].Rows;
            mTable[m] = new Matrix(numNodes, 6);

            double OUT;
            double ON;
            double Dii;
            double db;
            double node = 0;
            for (int i = 0; i < numNodes; ++i)
            {
                OUT = mTable["Dependency"][i, numNodes];
                ON = mTable["Dependency"][numNodes, i];
                Dii = mTable["Dependency"][i, i];
                db = (OUT - ON) / (OUT + ON + Dii);
                try
                {
                    node = Convert.ToDouble(mTable["Data"].ColLabels[i]);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Input string is not a sequence of digits.");
                }

                mTable[m][i, 0] = year;
                mTable[m][i, 1] = node;
                mTable[m][i, 2] = OUT;
                mTable[m][i, 3] = ON;
                mTable[m][i, 4] = Dii;
                mTable[m][i, 5] = db;
            }

            mTable[m].ColLabels[0] = "Year";
            mTable[m].ColLabels[1] = "Node";
            mTable[m].ColLabels[2] = "OUTD";
            mTable[m].ColLabels[3] = "OND";
            mTable[m].ColLabels[4] = "Self-Dep";
            mTable[m].ColLabels[5] = "Dep. Balance";

            //mTable[m].RowLabels.CopyFrom(mTable["Data"].ColLabels);

        }

        public double Dependency
        {
            get
            {
                return mTable["Dependency"][mTable["Dependency"].Rows - 2, mTable["Dependency"].Cols - 2];
            }
        }

        public double GetDIG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 3] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double dig = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    dig -= mTable["Centrality"][i, 3] * (mTable["Centrality"].Rows - 1);
                dig = dig / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return dig;
            }
        }

        public double GetDOG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 2] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double dog = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    dog -= mTable["Centrality"][i, 2] * (mTable["Centrality"].Rows - 1);
                dog = dog / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return dog;
            }
        }

        public double GetCIG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 5] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double cig = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    cig -= mTable["Centrality"][i, 5] * (mTable["Centrality"].Rows - 1);
                cig = cig * (2 * mTable["Centrality"].Rows - 3) / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return cig;
            }
        }

        public double GetCOG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 4] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double cog = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    cog -= mTable["Centrality"][i, 4] * (mTable["Centrality"].Rows - 1);
                cog = cog * (2 * mTable["Centrality"].Rows - 3) / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return cog;
            }
        }

        public double GetBIG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 7] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double big = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    big -= mTable["Centrality"][i, 7] * (mTable["Centrality"].Rows - 1);
                big = big * 2 / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return big;
            }
        }

        public double GetBOG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 6] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double bog = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    bog -= mTable["Centrality"][i, 6] * (mTable["Centrality"].Rows - 1);
                bog = bog * 2 / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return bog;
            }
        }

        public double GetEIG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 9] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double eig = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    eig -= mTable["Centrality"][i, 9] * (mTable["Centrality"].Rows - 1);
                eig = eig / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return eig;
            }
        }

        public double GetEOG
        {
            get
            {
                List<double> values = new List<double>();
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    values.Add(mTable["Centrality"][i, 8] * (mTable["Centrality"].Rows - 1));

                double max = Algorithms.MaxValue(values);
                double eog = max * mTable["Centrality"].Rows;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    eog -= mTable["Centrality"][i, 8] * (mTable["Centrality"].Rows - 1);
                eog = eog / ((mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 1) * (mTable["Centrality"].Rows - 2));
                return eog;
            }
        }

        public double GetNocom
        {
            get
            {

                return comcliques.Count;
            }
        }

        public double GetNoBcom
        {
            get
            {
                //return comblocks.Count;
                return comcliques.Count;
            }
        }

        public double GetNoComcom
        {
            get
            {
                // old code
                //return comComm.Count;

                // new code
                //return comNum;
                return comcliques.Count;
            }
        }

        public double GetGn
        {
            get
            {
                Vector v = new Vector(comcliques.Count);
                for (int i = 0; i < comcliques.Count; ++i)
                    v[i] = comcliques[i].Size;
                return Algorithms.MaxValue(v) / GetMatrix("Data").Rows;
            }
        }

        public double GetbGn
        {
            get
            {
                Vector v = new Vector(comcliques.Count);
                for (int i = 0; i < comcliques.Count; ++i)
                    v[i] = comcliques[i].Size;
                return Algorithms.MaxValue(v) / GetMatrix("Data").Rows;
                /*
                Vector v = new Vector(comblocks.Count);
                for (int i = 0; i < comblocks.Count; ++i)
                    v[i] = comblocks[i].Size;
                return Algorithms.MaxValue(v) / GetMatrix("Data").Rows;
                */
            }
        }

        public double GetcomGn
        {
            get
            {
                /*
                Vector v = new Vector(comComm.Count);
                for (int i = 0; i < comComm.Count; ++i)
                    v[i] = comComm[i].Size;
                return Algorithms.MaxValue(v) / GetMatrix("Data").Rows;
                */
                Vector v = new Vector(comcliques.Count);
                for (int i = 0; i < comcliques.Count; ++i)
                    v[i] = comcliques[i].Size;
                return Algorithms.MaxValue(v) / GetMatrix("Data").Rows;
            }
        }


        public double GetAvgDO
        {
            get
            {
                double avgdo = 0.0;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    avgdo += mTable["Centrality"][i, 2];
                return avgdo / mTable["Centrality"].Rows;

            }
        }

        public double GetAvgDI
        {
            get
            {
                double avgdi = 0.0;
                for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                    avgdi += mTable["Centrality"][i, 3];
                return avgdi / mTable["Centrality"].Rows;

            }
        }


        public double SysDep
        {
            get
            {
                double sysdep = 0.0;
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    sysdep -= mTable["Dependency"][j, j];
                    for (int I = 0; I < mTable["Data"].Rows; ++I)
                    {
                        sysdep += mTable["Dependency"][I, j];
                    }
                }
                return (sysdep / (mTable["Data"].Rows * (mTable["Data"].Rows - 1)));
            }
        }

        public double GetDensity(double cutoff)
        {
            return MatrixComputations.Density(mTable["Data"], cutoff);
        }

        public double GetTransitivity(MatrixComputations.TransitivityType transitivityType)
        {
            return MatrixComputations.Transitivity(mTable["Data"], transitivityType);
        }


        public void LoadReachability(int n, bool doSum, string m, int year, bool reachBinary)
        {
            LoadReachability(n, doSum, true, m, year, reachBinary);
        }
        public void LoadReachability(int n, bool doSum, bool zeroDiagonal, string m, int year, bool reachBinary)
        {
            n = GetRealReachNum(n, year);
            //if (m == "Reachability")
            //{
            //    m = "Temp";
            //    mTable[m] = new Matrix(mTable["Reachability"]);
            //    for (int i = 0; i < mTable[m].Rows; ++i)
            //        mTable[m].RowLabels[i] = mTable[m].ColLabels[i] = mTable["Reachability"].ColLabels[i];
            //}

            mTable["Reachability"] = new Matrix(mTable["Data"]);
            Matrix diag = mTable["Data"].GetDiagonalMatrix();

            if (!doSum)
            {
                for (int i = 0; i < n - 1; ++i)
                {
                    if (zeroDiagonal)
                        mTable["Reachability"].ZeroDiagonal();
                    mTable["Reachability"] *= mTable["Data"];
                }
                return;
            }

            if (n < 1)
                return;

            Matrix mat = new Matrix(mTable["Data"]);

            if (zeroDiagonal)
            {
                mat.ZeroDiagonal();
            }

            // Generate R (dependency) matrix:
            Matrix sum = new Matrix(mat);
            Matrix power = new Matrix(mat);

            for (int i = 2; i <= n; ++i)
            {
                power *= mat;
                sum += power;
                Application.DoEvents();
            }
            //power.ZeroDiagonal();

            if (reachBinary)
                for (int i = 0; i < sum.Length; i++)
                    if (sum[i] > 1)
                        sum[i] = 1;

            sum.CloneTo(mTable["Reachability"]);

            //if (zeroDiagonal)
            //{
            //    for (int i = 0; i < mTable["Reachability"].Rows; ++i)
            //        mTable["Reachability"][i, i] = diag[i, i];
            //}
            ReachabilityN = n;
        }


        public void LoadCognitiveReachability(int n, bool doSum, bool zeroDiagonal, string m, int year, bool reachBinary)
        {
            n = GetRealReachNum(n, year);
            //if (m == "Reachability")
            //{
            //    m = "Temp";
            //    mTable[m] = new Matrix(mTable["Reachability"]);
            //    for (int i = 0; i < mTable[m].Rows; ++i)
            //        mTable[m].RowLabels[i] = mTable[m].ColLabels[i] = mTable["Reachability"].ColLabels[i];
            //}
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required!");

            mTable["CognitiveReachability"] = new Matrix(mTable["Data"]);
            Matrix diag = mTable["Data"].GetDiagonalMatrix();

            if (!doSum)
            {
                for (int i = 0; i < n - 1; ++i)
                {
                    if (zeroDiagonal)
                        mTable["CognitiveReachability"].ZeroDiagonal();
                    mTable["CognitiveReachability"] = CognitiveAlgebra.Multiply(mTable["CognitiveReachability"], mTable["Data"]);
                    //mTable["CognitiveReachability"] *= mTable["Data"];
                }
                return;
            }

            if (n < 1)
                return;

            Matrix mat = new Matrix(mTable["Data"]);

            if (zeroDiagonal)
            {
                mat.ZeroDiagonal();
            }

            // Generate R (dependency) matrix:
            Matrix sum = new Matrix(mat);
            Matrix power = new Matrix(mat);

            for (int i = 2; i <= n; ++i)
            {
                power = CognitiveAlgebra.Multiply(power, mat);
                sum = CognitiveAlgebra.Add(sum, power);
                //power *= mat;
                //sum += power;
                Application.DoEvents();
            }
            //power.ZeroDiagonal();

            if (reachBinary)
                for (int i = 0; i < sum.Length; i++)
                    if (sum[i] > 1)
                        sum[i] = 1;

            sum.CloneTo(mTable["CognitiveReachability"]);

            //if (zeroDiagonal)
            //{
            //    for (int i = 0; i < mTable["Reachability"].Rows; ++i)
            //        mTable["Reachability"][i, i] = diag[i, i];
            //}
            ReachabilityN = n;
        }



        public void StandardizeByRow(string m)
        {
            mTable[m].Standardization = Matrix.StandardizationType.Row;
        }
        public void StandardizeByColumn(string m)
        {
            mTable[m].Standardization = Matrix.StandardizationType.Column;
        }
        public void StandardizeByDiagonalRow(string m)
        {
            if (mTable[m].IsSquareMatrix)
                mTable[m].Standardization = Matrix.StandardizationType.DiagonalRow;
            else
                throw new Exception("Cannot standardize non-square matrix by diagonal!");
        }
        public void StandardizeByDiagonalColumn(string m)
        {
            if (mTable[m].IsSquareMatrix)
                mTable[m].Standardization = Matrix.StandardizationType.DiagonalColumn;
            else
                throw new Exception("Cannot standardize non-square matrix by diagonal!");
        }
        public void StandardizeByDiagonalMinimum(string m)
        {
            if (mTable[m].IsSquareMatrix)
                mTable[m].Standardization = Matrix.StandardizationType.DiagonalMinimum;
            else
                throw new Exception("Cannot standardize non-square matrix by diagonal!");
        }
        public void StandardizeByDiagonalMaximum(string m)
        {
            if (mTable[m].IsSquareMatrix)
                mTable[m].Standardization = Matrix.StandardizationType.DiagonalMaximum;
            else
                throw new Exception("Cannot standardize non-square matrix by diagonal!");
        }

        public void Unstandardize(string m)
        {
            mTable[m].Standardization = Matrix.StandardizationType.None;
        }

        // deprecated
        protected double ComputeER1(bool useCohesion, bool useSize, int clique)
        {

            double p = 1.0 + (useCohesion ? _cliques[clique].Cohesion : 1.0);
            double d = useSize ? NPOLA_Sums[clique] : _cliques[clique].Size / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);

            //double d = useSize ? NPOLA_Sums[clique] : _cliques[clique].Size / (double)mTable["Data"].Rows;
            //return useCohesion ? _cliques[clique].Cohesion : 1.0;

        }
        // deprecated
        protected double bComputeER1(bool useCohesion, bool useSize, int block)
        {
            double p = 1.0 + (useCohesion ? _blocks[block].Cohesion : 1.0);
            double d = useSize ? NPOLA_Sums[block] : _blocks[block].Size / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }

        // deprecated
        protected double comComputeER1(bool useCohesion, bool useSize, int com)
        {
            double p = 1.0 + (useCohesion ? _communities[com].Cohesion : 1.0);
            double d = useSize ? NPOLA_Sums[com] : _communities[com].Count / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }

        // deprecated
        protected double cComputealphaER1(double alpha, bool useSize, int clique)
        {
            double p = 1.0 + alpha;
            double d = useSize ? NPOLA_Sums[clique] : _cliques[clique].Size / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }

        // deprecated
        protected double bComputealphaER1(double alpha, bool useSize, int block)
        {
            double p = 1.0 + alpha;
            double d = useSize ? NPOLA_Sums[block] : _blocks[block].Size / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }

        // deprecated
        protected double comComputealphaER1(double alpha, bool useSize, int com)
        {
            double p = 1.0 + alpha;
            double d = useSize ? NPOLA_Sums[com] : _communities[com].Count / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }

        // deprecated
        protected double ComputecomER1(bool useCohesion, bool useSize, int clique)
        {
            double p = 1.0 + (useCohesion ? comcliques[clique].Cohesion : 1.0);
            double d = useSize ? NPOLA_Sums[clique] : comcliques[clique].Size / (double)mTable["Data"].Rows;
            return Math.Pow(d, p);
        }


        // new function to calculate Cohesion for communities
        protected double NewComputeCohesion(int[] community)
        {
            // find size of community
            /*
            int Size = 0;
            for (int i = 0; i < comSize; i++)
            {
                if (community[i] == 1)
                    Size++;
            }


            double cohesion = 0.0;
            for (int i = 0; i < Size; ++i)
                for (int j = i + 1; j < Size; ++j)
                    cohesion += mTable["SESE"][i, j];

            if (Size == 1)
                cohesion = 1.0;
            else
                cohesion = (2.0 * cohesion) / (Size * (Size - 1));
            */


            int Size = 0;
            for (int i = 0; i < comSize; i++)
            {
                if (community[i] == 1)
                    Size++;
            }
            double cohesion = 0.0;
            List<double> cohesionList = new List<double>();
            for (int i = 0; i < comSize; ++i)
            {
                if (community[i] != 1)
                    continue;
                for (int j = i + 1; j < comSize; ++j)
                {
                    if (community[j] == 1)
                        cohesionList.Add(mTable["SESE"][i, j]);
                }
            }
            if (Size == 1)
                cohesion = 1.0;
            else
            {
                cohesion = Algorithms.Mean(cohesionList);
            }

            return cohesion;
        }



        public double cGetERPOL(bool useCohesion, bool useSize, string erpolType, double alpha)
        {
            double erpol = 0.0;
            double[] erpolTerms = new double[_cliques.Count];

            for (int i = 0; i < _cliques.Count; i++)
            {
                double firstTerm = 0.0, secondTerm = 0.0;
                double expTerm = 0.0, sum = 0.0;

                for (int j = 0; j < cliqueSize; j++)
                {
                    sum += _cliques[i].IntContains(j);
                }
                firstTerm = useSize ? NPOLA_Sums[i] : sum / cliqueSize;

                if (useCohesion)
                    expTerm = 1 + ((erpolType == "Cohesion") ? _cliques[i].Cohesion : alpha);
                else
                    expTerm = 1 + ((erpolType == "Cohesion") ? 1.0 : alpha);

                firstTerm = Math.Pow(firstTerm, expTerm);
                secondTerm = useSize ? (1 - NPOLA_Sums[i]) : (cliqueSize - sum) / cliqueSize;
                erpolTerms[i] = firstTerm * secondTerm;
            }
            // generate the GOI term from the density matrix
            if (cliqueDensity == null)
                cliqueDensity = CalculateCliqueDensity(_cliques);
            for (int i = 0; i < cliqueDensity.Rows; i++)
            {
                double sum = 0.0, GOIterm = 0.0;
                for (int j = 0; j < cliqueDensity.Rows; j++)
                {
                    if (j == i)
                        continue;
                    sum += cliqueDensity[i, j];
                }

                sum /= (_cliques.Count - 1);
                GOIterm = (1 - sum);
                erpol += erpolTerms[i] * GOIterm;
            }
            return erpol;
        }

        public double bGetERPOL(bool useCohesion, bool useSize, string erpolType, double alpha)
        {
            double erpol = 0.0;
            double[] erpolTerms = new double[_blocks.Count];

            for (int i = 0; i < _blocks.Count; i++)
            {
                double firstTerm = 0.0, secondTerm = 0.0;
                double expTerm = 0.0, sum = 0.0;

                for (int j = 0; j < blockSize; j++)
                {
                    sum += _blocks[i].IntContains(j);
                }
                firstTerm = useSize ? NPOLA_Sums[i] : sum / blockSize;

                if (useCohesion)
                    expTerm = 1 + ((erpolType == "Cohesion") ? _blocks[i].Cohesion : alpha);
                else
                    expTerm = 1 + ((erpolType == "Cohesion") ? 1.0 : alpha);

                firstTerm = Math.Pow(firstTerm, expTerm);
                secondTerm = useSize ? (1 - NPOLA_Sums[i]) : (blockSize - sum) / blockSize;
                erpolTerms[i] = firstTerm * secondTerm;
            }
            // generate the GOI term from the density matrix
            for (int i = 0; i < _blockDensity.Rows; i++)
            {
                double sum = 0.0, GOIterm = 0.0;
                for (int j = 0; j < _blockDensity.Rows; j++)
                {
                    if (j == i)
                        continue;
                    sum += _blockDensity[i, j];
                }

                sum /= (_blocks.Count - 1);
                GOIterm = (1 - sum);
                erpol += erpolTerms[i] * GOIterm;
            }
            return erpol;
        }


        public double comGetERPOL(bool useCohesion, bool useSize, string erpolType, double alpha)
        {

            double erpol = 0.0;
            double[] erpolTerms = new double[comNum];

            for (int i = 0; i < comNum; i++)
            {
                double firstTerm = 0.0, secondTerm = 0.0;
                double expTerm = 0.0, sum = 0.0;

                for (int j = 0; j < comSize; j++)
                {
                    sum += communities[i][j];
                }
                firstTerm = useSize ? NPOLA_Sums[i] : sum / comSize;

                if (useCohesion)
                {
                    //expTerm = 1 + ((erpolType == "Cohesion") ? _communities[i].Cohesion : alpha);
                    expTerm = 1 + ((erpolType == "Cohesion") ? NewComputeCohesion(communities[i]) : alpha);
                }
                else
                    expTerm = 1 + ((erpolType == "Cohesion") ? 1.0 : alpha);

                firstTerm = Math.Pow(firstTerm, expTerm);
                secondTerm = useSize ? (1 - NPOLA_Sums[i]) : (comSize - sum) / comSize;
                erpolTerms[i] = firstTerm * secondTerm;
            }
            // generate the GOI term from the density matrix
            //for (int i = 0; i < commDensity.Rows; i++)

            if (commDensity == null)
                commDensity = CalculateCommDensity(communities);
            for (int i = 0; i < comNum; i++)
            {
                double sum = 0.0, GOIterm = 0.0;
                for (int j = 0; j < commDensity.Rows; j++)
                {
                    if (j == i)
                        continue;
                    sum += commDensity[i, j];
                }

                sum /= (comNum - 1);
                GOIterm = (1 - sum);
                erpol += erpolTerms[i] * GOIterm;
            }
            return erpol;
        }










        public double GetSYSIN(bool zeroDiagonal, double maxik, int year, int reachMatrixCount)
        {
            maxik = GetRealDensity(maxik, year, "Data");
            double SYSIN = 0.0;

            int N = mTable["Data"].Rows;


            if (!mTable.ContainsKey("Dependency"))
                return 0.0;

            if (zeroDiagonal)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += mTable["Reachability"][i, j];
                    }
                }
                SYSIN *= (1 - maxik * (N - 1));
                SYSIN /= (N * (maxik * (N - 1) - Math.Pow((maxik * (N - 1)), reachMatrixCount + 1)));
            }
            else
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        SYSIN += mTable["Reachability"][i, j];
                    }
                }
                SYSIN *= (1 - maxik * N);
                SYSIN /= (N * (maxik * N - Math.Pow((maxik * N), reachMatrixCount + 1)));
            }

            return SYSIN;
        }

        public void merge(List<clique> cliques, List<clique> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                MergeOne(cliques, list[i]);
            }
        }

        private bool MergeOne(List<clique> cliques, clique p)
        {
            for (int i = 0; i < cliques.Count; i++) //Go though each element in cliques
            {
                bool pIsSmaller = false;
                bool pIsBigger = false;

                for (int j = 0; j < p.Count; j++) //foreach element in each element in that particular clique
                {
                    if (p[j] > cliques[i][j])
                        pIsBigger = true;
                    else if (cliques[i][j] > p[j])
                        pIsSmaller = true;
                }
                //Note that if p is not bigger in any way, then p is a subset or equal
                if (!pIsBigger)
                {
                    cliques[i].num_networks++;
                    return true;
                }
                //Note that if pIsBigger in some ways, but not smaller in any way, then that element of clique is a subset of p
                //So we replace it
                if (!pIsSmaller)
                {
                    cliques[i] = p;
                    //also, we can be sure that p is not a subset of anything that is in cliques
                    //but we do need to check for the rest of stuff that is in cliques to be  in p
                    for (int j = i + 1; j < cliques.Count; j++)
                    {
                        if (p.is_subset(cliques[j]))
                        {
                            cliques.RemoveAt(j);
                            j--;
                        }
                    }

                    return false;
                }
            }
            cliques.Add(p);
            return false;
        }

        /// <summary>
        /// Generate the lowest cost matrix, assuming that zero means no connet, and putting it in cheapest
        /// </summary>
        public void generateLowestCostMatrix()
        {
            if (mTable.ContainsKey("Cheapest"))
                return;
            /*
        	Matrices.Matrix cur=new Matrices.Matrix(mTable["Data"]);
        	Matrices.Matrix best=new Matrices.Matrix(mTable["Data"]);
        	Matrices.Matrix start=mTable["Data"];
        	int counter=0;
        	
        	for(counter=0; counter<start.Cols-1; counter++)
        	{
        		if(best.complete())
        			break;
        		else
        		{
        			best.ElementWiseOr(cur,counter);
        			cur=cur*start;
        		}
        	}
            */
            Matrices.Matrix best = new Matrices.Matrix(mTable["Data"]);
            for (int i = 0; i < best.Rows; i++)
            {
                best.SetRowVector(i, new Matrices.Vector(Dijkstra(i)));
            }

            mTable.Add("Cheapest", best);
        }

        /// <summary>
        /// return the distance from the inputed node to all other nodes, in a weighted graph
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        //public List<double> Dijkstra(int row)
        public double[] Dijkstra(int row)
        {
            double[] best = new double[mTable["Data"].Cols];
            //List<double> best = new List<double>(mTable["Data"].Cols);
            //for (int i = 0; i < best.Capacity; i++)
            for (int i = 0; i < best.Length; i++)
            {
                best[i] = 1000000.0; //setting best to be a really high number so it will be really easy to beat
            }

            int totalNodes = mTable["Data"].Cols;
            int knownNodes = 1;
            path cur;

            LiyeLib.heap<path> Heap = new LiyeLib.heap<path>();
            //inserting the root
            best[row] = 0;
            insertPaths(row, best, 0, Heap, mTable["Data"]);

            while (knownNodes < totalNodes)
            {
                if (Heap.isEmpty())
                    break; //If we run of things in the heap, that means that the graph is unconnected
                cur = Heap.findMin();
                Heap.deleteMin();
                if (cur.cost < best[cur.curNode]) //if the thing that we just popped off is a improvement
                {
                    //mark it down
                    best[cur.curNode] = cur.cost;
                    //insert everything around it
                    insertPaths(cur.curNode, best, cur.cost, Heap, mTable["Data"]);
                    knownNodes++;
                }
            }

            return best;
        }

        //private void insertPaths(int node, List<double> best, double cost, LiyeLib.heap<path> Heap, Matrices.Matrix map)
        private void insertPaths(int node, double[] best, double cost, LiyeLib.heap<path> Heap, Matrices.Matrix map)
        {
            //for (int i = 0; i < best.Capacity; i++)
            for (int i = 0; i < best.Length; i++)
            {
                //inserting everything that is near
                if (map[node, i] != 0.0) //If there is a edge
                {
                    if (best[i] != 0xFFFFFFF) //If we have never been here
                    {
                        Heap.insert(new path(cost + map[node, i], node, i));
                    }
                }
            }
        }

        public void generateStrengthMatrix()
        {
            if (mTable.ContainsKey("Strength"))
                return;
            Matrices.Matrix best = new Matrices.Matrix(mTable["Data"]);
            for (int i = 0; i < best.Rows; i++)
            {
                best.SetRowVector(i, new Matrices.Vector(findStrengthRow(i)));
            }

            mTable.Add("Strength", best);
        }

        //private List<double> findStrengthRow(int row)
        private double[] findStrengthRow(int row)
        {
            //List<double> best = new List<double>(mTable["Data"].Cols);
            double[] best = new double[mTable["Data"].Cols];
            LiyeLib.Maxheap<int_double> heap = new LiyeLib.Maxheap<int_double>();
            int count = 1;
            best[row] = 10000;

            //inserting everything around the starting point
            //for (int i = 0; i < best.Capacity; i++)
            for (int i = 0; i < best.Length; i++)
            {
                if (mTable["Data"][row, i] != 0)
                    heap.insert(new int_double(i, mTable["Data"][row, i]));
            }

            while (true)
            {
                if (heap.isEmpty())
                {
                    //if it is empty, that means that we are done
                    best[row] = 0;
                    break;
                }
                if (best[heap.findMax().x] < heap.findMax().value) //if it is a improvement
                {
                    //insert everything around it
                    //for (int i = 0; i < best.Capacity; i++)
                    for (int i = 0; i < best.Length; i++)
                    {
                        if (mTable["Data"][heap.findMax().x, i] != 0)
                            heap.insert(new int_double(i, Math.Min(heap.findMax().value, mTable["Data"][heap.findMax().x, i])));
                    }

                    //update best
                    best[heap.findMax().x] = heap.findMax().value;

                    heap.deleteMax();
                    count++;
                }
                else //if we have been here before
                {
                    heap.deleteMax();
                }
            }
            return best;
        }

        public List<int> getSequence(int nodecount)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < nodecount; i++)
            {
                list.Add(i);
            }
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = RNGen.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public void dropEdge(ref Matrix modeldata, ref Matrix utilitytable, ref List<int[]> tiecapacity, int droppingnode, int droppednode, int nodecount)
        {
            utilitytable[droppingnode * nodecount + droppednode, 7] = 0;//offer
            modeldata[droppednode * nodecount + droppednode, 22] = 0;//accc
            modeldata[droppednode * nodecount + droppingnode, 21] = 0;//accr
            modeldata[droppingnode * nodecount + droppednode, 4] = 0;//remove edge
            modeldata[droppingnode * nodecount + droppednode, 23] = 1;//droppedr
            utilitytable[droppingnode * nodecount + droppednode, 6] = 0;//edge
            utilitytable[droppingnode * nodecount + droppednode, 8] = 0;//accept
            modeldata[droppednode * nodecount + droppingnode, 4] = 0;//remove edge
            modeldata[droppednode * nodecount + droppingnode, 24] = 1;//droppedc
            utilitytable[droppednode * nodecount + droppingnode, 6] = 0;//edge
            utilitytable[droppednode * nodecount + droppingnode, 8] = 0;//accept
            tiecapacity[1][droppednode] += 1;
            tiecapacity[2][droppednode] -= 1;
            tiecapacity[1][droppingnode] += 1;
            tiecapacity[2][droppingnode] -= 1;
        }

        public void addEdge(ref Matrix modeldata, ref Matrix utilitytable, ref List<int[]> tiecapacity, int addingnode, int addednode, int nodecount)
        {
            utilitytable[addingnode * nodecount + addednode, 7] = 1; //offer
            modeldata[addingnode * nodecount + addednode, 22] = 1; //accc
            modeldata[addednode * nodecount + addingnode, 21] = 1; //accr
            modeldata[addingnode * nodecount + addednode, 4] = 1; //make edge
            utilitytable[addingnode * nodecount + addednode, 6] = 1;//edge
            utilitytable[addingnode * nodecount + addednode, 8] = 1;//accept
            tiecapacity[1][addingnode] -= 1; //decrement tie capacity
            tiecapacity[2][addingnode] += 1; //increment degree
            modeldata[addednode * nodecount + addingnode, 4] = 1; //make edge
            utilitytable[addednode * nodecount + addingnode, 6] = 1;
            utilitytable[addednode * nodecount + addingnode, 8] = 1;
            tiecapacity[1][addednode] -= 1; //decrement tie capacity
            tiecapacity[2][addednode] += 1; //increment degree
        }

        public int getDegree(int node, int nodecount, Matrix modeldata)
        {
            int degree = 0;
            for (int i = node * nodecount; i < node * nodecount + nodecount; ++i)
            {
                if (modeldata[i, 4] == 1)
                {
                    degree += 1;
                }
            }
            return degree;
        }

        public Matrix shockHomophily(ref Matrix modeldata, ref Matrix utilitytable, ref List<int[]> tiecapacity, int nodecount, ref int netedges, ref int netnodes, int runno, string[] words, bool enemy, bool democracy, bool cultism)
        {
            if (enemy)
            {
                double shockSpread = RNGen.NextDouble();              
                int affected = 0;
                List<int> shockedNodes = new List<int>();
                int candidate;
                double shocksum = 0;
                double shockpernode = 1.0 / nodecount;
                while (shocksum < shockSpread)
                {
                    candidate = RNGen.Next(nodecount);
                    if (!shockedNodes.Contains(candidate))
                    {
                        shocksum += shockpernode;
                        shockedNodes.Add(candidate);
                    }
                }

                for (int i = 0; i < shockedNodes.Count; ++i)
                {
                    for (int j = 0; j < nodecount * nodecount; ++j)
                    {
                        if (j % nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 11] = 0;
                            else
                                modeldata[j, 11] = 1;
                        }
                        if (j / nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 8] = 0;
                            else
                                modeldata[j, 8] = 1;
                        }
                    }
                }
            }
            if (democracy)
            {
                double shockSpread = RNGen.NextDouble();
                int affected = 0;
                List<int> shockedNodes = new List<int>();
                int candidate;
                double shocksum = 0;
                double shockpernode = 1.0 / nodecount;
                while (shocksum < shockSpread)
                {
                    candidate = RNGen.Next(nodecount);
                    if (!shockedNodes.Contains(candidate))
                    {
                        shocksum += shockpernode;
                        shockedNodes.Add(candidate);
                    }
                }

                for (int i = 0; i < shockedNodes.Count; ++i)
                {
                    for (int j = 0; j < nodecount * nodecount; ++j)
                    {
                        if (j % nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 10] = 0;
                            else
                                modeldata[j, 10] = 1;
                        }
                        if (j / nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 7] = 0;
                            else
                                modeldata[j, 7] = 1;
                        }
                    }
                }

            }
            if (cultism)
            {
                double shockSpread = RNGen.NextDouble();
                int affected = 0;
                List<int> shockedNodes = new List<int>();
                int candidate;
                double shocksum = 0;
                double shockpernode = 1.0 / nodecount;
                while (shocksum < shockSpread)
                {
                    candidate = RNGen.Next(nodecount);
                    if (!shockedNodes.Contains(candidate))
                    {
                        shocksum += shockpernode;
                        shockedNodes.Add(candidate);
                    }
                }

                for (int i = 0; i < shockedNodes.Count; ++i)
                {
                    for (int j = 0; j < nodecount * nodecount; ++j)
                    {
                        if (j % nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 12] = 0;
                            else
                                modeldata[j, 12] = 1;
                        }
                        if (j / nodecount == shockedNodes[i])
                        {
                            if (runno % 2 == 1)
                                modeldata[j, 9] = 0;
                            else
                                modeldata[j, 9] = 1;
                        }
                    }
                }
            }

            if (!democracy && !cultism && !enemy)
            {
                double shockSpread = RNGen.NextDouble();
                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "Network with runno " + runno + "'s shock spread is: " + (shockSpread * 100) + "%." + Environment.NewLine);
                int affected = 0;
                List<int> shockedNodes = new List<int>();
                int candidate;
                double shocksum = 0;
                double shockpernode = 1.0 / nodecount;
                while (shocksum < shockSpread)
                {
                    candidate = RNGen.Next(nodecount);
                    if (!shockedNodes.Contains(candidate))
                    {
                        shocksum += shockpernode;
                        shockedNodes.Add(candidate);
                    }
                }
                double shockSize;
                int oldcap;
                for (int i = 0; i < nodecount * nodecount; i++)
                {
                    modeldata[i, 15] = modeldata[i, 5];
                    modeldata[i, 16] = modeldata[i, 6];
                }
                for (int i = 0; i < shockedNodes.Count; i++)
                {
                    shockSize = RNGen.NextDouble();
                    oldcap = tiecapacity[0][shockedNodes[i]];
                    //tiecapacity[0][shockedNodes[i]] = tiecapacity[0][shockedNodes[i]];
                    tiecapacity[0][shockedNodes[i]] = (int)(tiecapacity[0][shockedNodes[i]] * shockSize);
                    //tiecapacity[0][shockedNodes[i]] = tiecapacity[0][shockedNodes[i]];
                    for (int j = 0; j < nodecount * nodecount; j++)
                    {
                        if (j % nodecount == shockedNodes[i])
                        {
                            utilitytable[j, 3] = tiecapacity[0][shockedNodes[i]];
                            modeldata[j, 16] = tiecapacity[0][shockedNodes[i]];
                        }
                        if (j / nodecount == shockedNodes[i])
                        {
                            modeldata[j, 15] = tiecapacity[0][shockedNodes[i]];
                        }
                    }
                    System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Node " + (shockedNodes[i] + 1) + " in network with runno " + runno + "'s tie capacity was reduced by " + ((1 - shockSize) * 100) + "% (" + (oldcap - tiecapacity[0][shockedNodes[i]]) + ") due to a shock. New tie capacity: " + tiecapacity[0][shockedNodes[i]] + Environment.NewLine);
                }
                while (shockedNodes.Count > 0)
                {
                    int shockNode = shockedNodes[0];
                    int continuecount = 0;
                    //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Active Node: " + shockNode + Environment.NewLine);
                    int shockNodeCapacity = tiecapacity[0][shockNode] - tiecapacity[2][shockNode];
                    tiecapacity[1][shockNode] = shockNodeCapacity;
                    while (tiecapacity[0][shockNode] - tiecapacity[2][shockNode] < 0)//shockNodeCapacity < 0)
                    {
                        List<int> minUtil = minUtility(utilitytable, modeldata, shockNode, nodecount, words);
                        if (minUtil.Count == 0)
                        {
                            if (continuecount < 5)
                            {
                                continuecount++;
                                continue;
                            }
                            else
                            {
                                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + shockNode + " in network with runno " + runno + " failed to drop nodes when shocked." + Environment.NewLine);
                                break;
                            }
                        }
                        int selectedNode = minUtil[RNGen.Next(minUtil.Count)];

                        //drop edge with node offering least utility
                        dropEdge(ref modeldata, ref utilitytable, ref tiecapacity, shockNode, selectedNode, nodecount);
                        if (tiecapacity[2][selectedNode] <= 0)
                            netnodes++;
                        if (tiecapacity[2][shockNode] <= 0)
                            netnodes++;
                        netedges--;
                        System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Node " + (shockNode + 1) + " in network with runno " + runno + " dropped node " + (selectedNode + 1)/*
                                         + " ShockNodeCap = " + (shockNodeCapacity + 1)  */+ "." + Environment.NewLine);

                        utilitytable = updateUtility(utilitytable, ref modeldata, tiecapacity, nodecount, netedges, netnodes, true); //check netedges and netnodes
                        shockNodeCapacity++;
                    }
                    shockedNodes.Remove(shockNode);

                }
            }
            return modeldata;
        }

        public List<List<Matrix>> ABMShocksNetworkFormation(int nodecount, int network, string filename, int runno, bool homophily, bool enemy, bool cultism, bool democracy, ref ABMProgressForm apform)
        {
            Matrix modeldata = new Matrix(nodecount * nodecount, 29);
            Matrix utilitytable = new Matrix(nodecount * nodecount, 9);
            List<int[]> tiecapacity = new List<int[]>();
            List<int> initialnodes = new List<int>();
            //System.IO.File.AppendAllText(filename, "NETWORK " + network + " OF SIZE " + nodecount + Environment.NewLine);

            List<List<Matrix>> output = new List<List<Matrix>>();
            List<Matrix> temp;
            Matrix tempout;

            temp = new List<Matrix>();
            output.Add(temp);
            

            tiecapacity.Add(new int[nodecount]);
            tiecapacity.Add(new int[nodecount]);
            tiecapacity.Add(new int[nodecount]);
            tiecapacity.Add(new int[nodecount]);
            List<int> democracyNodes = getSequence(nodecount).GetRange(0, (int)((RNGen.NextDouble() * .2 + .2) * nodecount));
            List<int> enmyenmyNodes = getSequence(nodecount).GetRange(0, (int)((RNGen.NextDouble() * .25 + .1) * nodecount));
            List<int> cultismNodes = getSequence(nodecount).GetRange(0, (int)((RNGen.NextDouble() * .6 + .2) * nodecount));

            for (int i = 0; i < nodecount; i++)
            {
                tiecapacity[0][i] = RNGen.Next(0, (int)(nodecount * .7));
                tiecapacity[1][i] = tiecapacity[0][i];//tie capacity
                for (int j = i * nodecount; j < i * nodecount + nodecount; j++)
                {
                    modeldata[j, 5] = tiecapacity[1][i];
                }
                tiecapacity[2][i] = 0;// degree
                tiecapacity[3][i] = 0;
            }

            for (int i = 0; i < nodecount * nodecount; i++)
            {
                modeldata[i, 7] = 0;
                modeldata[i, 8] = 0;
                modeldata[i, 9] = 0;
                if (democracyNodes.Contains(i % nodecount))
                    modeldata[i, 10] = 1;
                if (democracyNodes.Contains((int)i / nodecount))
                    modeldata[i, 7] = 1;
                if (enmyenmyNodes.Contains(i % nodecount))
                    modeldata[i, 11] = 1;
                if (enmyenmyNodes.Contains((int)i / nodecount))
                    modeldata[i, 8] = 1;
                if (cultismNodes.Contains(i % nodecount))
                    modeldata[i, 12] = 1;
                if (cultismNodes.Contains((int)i / nodecount))
                    modeldata[i, 9] = 1;
                modeldata[i, 0] = runno;
                modeldata[i, 2] = i / nodecount + 1;
                modeldata[i, 3] = i % nodecount + 1;
                modeldata[i, 6] = tiecapacity[1][i % nodecount];
                utilitytable[i, 0] = i / nodecount + 1; //i
                utilitytable[i, 1] = i % nodecount + 1; //j
                utilitytable[i, 3] = tiecapacity[1][i % nodecount]; //tie capacity
                utilitytable[i, 5] = tiecapacity[1][i % nodecount]; //c_t
                utilitytable[i, 4] = utilitytable[i, 3] - utilitytable[i, 5]; //degree
                utilitytable[i, 6] = 0; //edge
                utilitytable[i, 7] = 0; //offer
                utilitytable[i, 8] = 0; //accept

            }
            double randomdouble;
            List<int> networknodes = new List<int>();
            int count = 0;
            int randomint;
            while ((count < .2 * nodecount && nodecount < 100) || (count < .1 * nodecount && nodecount >= 100))
            {
                randomint = RNGen.Next(0, nodecount);
                if (!networknodes.Contains(randomint))
                {
                    networknodes.Add(randomint);
                    count++;
                }

            }
            int netedges, netnodes;
            netedges = 0;
            netnodes = 0;
            int x, y;
            for (int i = 0; i < networknodes.Count; i++) //nodecount ^ 2 iterations
            {
                x = networknodes[i];
                for (int j = 0; j < networknodes.Count; j++)
                {
                    y = networknodes[j];
                    if (x != y && modeldata[nodecount * x + y, 4] == 0)
                    {
                        randomdouble = RNGen.NextDouble();
                        modeldata[nodecount * x + y, 19] = 1;
                        modeldata[nodecount * y + x, 20] = 1;

                        if (randomdouble >= .4 && tiecapacity[1][x] > 0 && tiecapacity[1][y] > 0)
                        {
                            if (tiecapacity[2][x] == 0)
                                netnodes++;
                            if (tiecapacity[2][y] == 0)
                                netnodes++;
                            modeldata[nodecount * x + y, 4] = 1;
                            modeldata[nodecount * x + y, 22] = 1;
                            modeldata[nodecount * y + x, 21] = 1;
                            utilitytable[nodecount * x + y, 6] = 1;
                            tiecapacity[1][x] -= 1;
                            tiecapacity[2][x] += 1;
                            modeldata[nodecount * y + x, 4] = 1;
                            utilitytable[nodecount * y + x, 6] = 1;
                            tiecapacity[1][y] -= 1;
                            tiecapacity[2][y] += 1;
                            netedges++;
                            initialnodes.Add(x);
                            initialnodes.Add(y);
                        }
                    }
                }
            }
            for (int i = 0; i < nodecount; i++)
            {
                if (initialnodes.Contains(i))
                {
                    for (int j = 0; j < nodecount; j++)
                    {
                        modeldata[i * nodecount + j, 25] = 1;
                    }
                }
            }
            //System.IO.File.AppendAllText(filename, "INITAL NETWORK" + Environment.NewLine + "runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,dropped,initial" + Environment.NewLine);
            //System.IO.File.AppendAllText(filename, modeldata.ToCSV(initialnodes, nodecount) + Environment.NewLine);
            //modeldata = updateModel(modeldata, tiecapacity, nodecount);
            utilitytable = updateUtility(utilitytable, ref modeldata, tiecapacity, nodecount, netedges, netnodes, homophily);
            int netnodesize;
            if (nodecount >= 100)
                netnodesize = (int)nodecount / 10;
            else
                netnodesize = (int)nodecount / 5;
            List<int> offeringnodes = new List<int>();
            for (int i = 0; i < nodecount; i++)
            {
                if (tiecapacity[1][i] > 0 && !initialnodes.Contains(i))
                    offeringnodes.Add(i);
            }
            int sequence = 1;
            bool offermade = false;
            while (outofNetwork(tiecapacity) > 0 && offeringnodes.Count > 0)
            {
                offermade = false;
                int newnode = offeringnodes[RNGen.Next(0, offeringnodes.Count)];
                do
                {
                    if (offeringnodes.Count == 0)
                        break;
                    newnode = offeringnodes[RNGen.Next(0, offeringnodes.Count)];
                    if (tiecapacity[2][newnode] > 0 || tiecapacity[1][newnode] <= 0)
                    {
                        offeringnodes.Remove(newnode);
                        continue;
                    }
                    else
                        break;
                } while (tiecapacity[2][newnode] > 0 || tiecapacity[1][newnode] <= 0);
                List<int> offernodes = new List<int>();
                for (int i = 0; i < nodecount; i++)
                {
                    if (i != newnode && tiecapacity[2][i] > 0)
                        offernodes.Add(i);
                }
                int potentialnode;
                do
                {
                    List<int> maxutil = maxUtility(utilitytable, newnode, nodecount);

                    for (int i = 0; i < maxutil.Count; i++)
                    {
                        if (offernodes.Contains(maxutil[i]))
                            offernodes.Remove(maxutil[i]);
                    }
                    if (maxutil.Count == 0)
                    {
                        break;
                    }
                    while (maxutil.Count > 0)
                    {
                        potentialnode = maxutil[RNGen.Next(0, maxutil.Count)];
                        utilitytable[newnode * nodecount + potentialnode, 7] = 1;
                        utilitytable[potentialnode * nodecount + newnode, 7] = 1;
                        if (potentialnode == newnode)
                        {
                            maxutil.Remove(potentialnode);
                            if (maxutil.Count == 0)
                            {
                                offeringnodes.Remove(newnode);
                                break;
                            }
                            continue;
                        }

                        if (tiecapacity[2][potentialnode] > 0 && tiecapacity[1][newnode] > 0 && tiecapacity[1][potentialnode] > 0) //tie capacity
                        {
                            modeldata[newnode * nodecount + potentialnode, 19] = 1; //offerr
                            modeldata[potentialnode * nodecount + newnode, 20] = 1;  //offerc

                            offermade = true;
                            //if(tiecapacity[1][potentialnode] > 0)
                            //{
                            if (tiecapacity[2][newnode] == 0) //if degree is 0, increment network node count
                            {
                                netnodes++;
                                for (int i = 0; i < nodecount * nodecount; i++)
                                {
                                    if (((int)i / nodecount) == newnode)
                                    {
                                        modeldata[i, 17] = sequence;
                                    }
                                    if (((int)i % nodecount) == newnode)
                                        modeldata[i, 18] = sequence;
                                }
                                sequence++;
                            }
                            if (tiecapacity[2][newnode] == 0)
                                netnodes++;

                            addEdge(ref modeldata, ref utilitytable, ref tiecapacity, newnode, potentialnode, nodecount);
                            /*
                                utilitytable[newnode * nodecount + potentialnode, 7] = 1; //offer
                                modeldata[newnode * nodecount + potentialnode, 22] = 1; //accc
                                modeldata[potentialnode * nodecount + newnode, 21] = 1; //accr
                                modeldata[newnode * nodecount + potentialnode, 4] = 1; //make edge
                                utilitytable[newnode * nodecount + potentialnode, 6] = 1;//edge
                                utilitytable[newnode * nodecount + potentialnode, 8] = 1;//accept
                                tiecapacity[1][newnode] -= 1; //decrement tie capacity
                                tiecapacity[2][newnode] += 1; //increment degree
                                modeldata[potentialnode * nodecount + newnode, 4] = 1; //make edge
                                utilitytable[potentialnode * nodecount + newnode, 6] = 1;
                                utilitytable[potentialnode * nodecount + newnode, 8] = 1;
                                tiecapacity[1][potentialnode] -= 1; //decrement tie capacity
                                tiecapacity[2][potentialnode] += 1; //increment degree
                              */
                            netedges++;
                            //}
                        }
                        maxutil.Remove(potentialnode);
                        utilitytable = updateUtility(utilitytable, ref modeldata, tiecapacity, nodecount, netedges, netnodes, homophily);
                    }
                    if (tiecapacity[1][newnode] <= 0 || offernodes.Count == 0 || maxutil.Count == 0)
                    {
                        offeringnodes.Remove(newnode);
                        break;
                    }

                } while (tiecapacity[1][newnode] > 0 && offernodes.Count > 0);
                /*if (offermade)
                {
                    System.IO.File.AppendAllText(filename, "NODE " + (newnode + 1) + " FINISHED OFFERS" + Environment.NewLine + "runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,dropped,initial" + Environment.NewLine);
                    System.IO.File.AppendAllText(filename, "runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,dropped,initial" + Environment.NewLine + modeldata.ToCSV(initialnodes, nodecount) + Environment.NewLine);
                }*/

            }
            string[] words = filename.Split('.');

            /*List<int> randlist = getSequence(nodecount);
            System.IO.File.AppendAllText(words[0] + "-list.txt", "BEGIN LIST:" + Environment.NewLine);
            for (int i = 0; i < randlist.Count; i++)
            {
                System.IO.File.AppendAllText(words[0] + "-list.txt", " " + randlist[i]);
            }
            System.IO.File.AppendAllText(words[0] + "-list.txt", Environment.NewLine);*/

            //System.IO.File.AppendAllText(words[0] + "-netform" + "~" + runno + "." + words[1], /*"runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,droppedr,droppedc,initial" + Environment.NewLine + */modeldata.ToCSV(initialnodes, nodecount));
            tempout = new Matrix(modeldata);
            output[0].Add(tempout);

            //Begin rewiring loop
            List<int> rewiringSequence = getSequence(nodecount);
            for (int i = 0; i < 5; i++)
            {
                rewiringSequence.AddRange(getSequence(nodecount));
            }
            output.Add(networkRewiring(ref modeldata, nodecount, ref utilitytable, ref tiecapacity, initialnodes, words, runno, network, ref netnodes, ref netedges, "", rewiringSequence, homophily));

            Matrix controlMatrix = new Matrix(modeldata);
            Matrix controlUtilityTable = new Matrix(utilitytable);
            List<int[]> controlTieCapacity = new List<int[]>();
            controlTieCapacity.Add(new int[nodecount]);
            controlTieCapacity.Add(new int[nodecount]);
            controlTieCapacity.Add(new int[nodecount]);
            controlTieCapacity.Add(new int[nodecount]);

            for (int i = 0; i < 4; i++)
            {
                controlTieCapacity[i] = new int[nodecount];
                for (int j = 0; j < nodecount; j++)
                {
                    controlTieCapacity[i][j] = tiecapacity[i][j];
                }
            }


            //Begin shock loop
            modeldata = shockHomophily(ref modeldata, ref utilitytable, ref tiecapacity, nodecount, ref netedges, ref netnodes, runno, words, enemy, democracy, cultism);
            /*double shockSpread = RNGen.NextDouble();
            System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "Network with runno " + runno + "'s shock spread is: " + (shockSpread * 100) + "%." + Environment.NewLine);
            int affected = 0;
            List<int> shockedNodes = new List<int>();
            int candidate;
            double shocksum = 0;
            double shockpernode = 1.0 / nodecount;
            while (shocksum < shockSpread)
            {
                candidate = RNGen.Next(nodecount);
                if (!shockedNodes.Contains(candidate))
                {
                    shocksum += shockpernode;
                    shockedNodes.Add(candidate);
                }
            }
            double shockSize;
            int oldcap;
            for (int i = 0; i < nodecount * nodecount; i++)
            {
                modeldata[i, 15] = modeldata[i, 5];
                modeldata[i, 16] = modeldata[i, 6];
            }
            for (int i = 0; i < shockedNodes.Count; i++)
            {
                shockSize = RNGen.NextDouble();
                oldcap = tiecapacity[0][shockedNodes[i]];
                //tiecapacity[0][shockedNodes[i]] = tiecapacity[0][shockedNodes[i]];
                tiecapacity[0][shockedNodes[i]] = (int)(tiecapacity[0][shockedNodes[i]] * shockSize);
                //tiecapacity[0][shockedNodes[i]] = tiecapacity[0][shockedNodes[i]];
                for (int j = 0; j < nodecount * nodecount; j++)
                {
                    if (j % nodecount == shockedNodes[i])
                    {
                        utilitytable[j, 3] = tiecapacity[0][shockedNodes[i]];
                        modeldata[j, 16] = tiecapacity[0][shockedNodes[i]];
                    }
                    if (j / nodecount == shockedNodes[i])
                    {
                        modeldata[j, 15] = tiecapacity[0][shockedNodes[i]];
                    }
                }
                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Node " + (shockedNodes[i] + 1) + " in network with runno " + runno + "'s tie capacity was reduced by " + ((1 - shockSize) * 100) + "% (" + (oldcap - tiecapacity[0][shockedNodes[i]]) + ") due to a shock. New tie capacity: " + tiecapacity[0][shockedNodes[i]] + Environment.NewLine);
            }
            /*System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: list = {" );
            for (int i = 0; i < shockedNodes.Count; i++)
            {
                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", shockedNodes[i] + " ");
            }
            System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "}" + Environment.NewLine);*/
            /*while (shockedNodes.Count > 0)
            {
                int shockNode = shockedNodes[0];
                int continuecount = 0;
                //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Active Node: " + shockNode + Environment.NewLine);
                int shockNodeCapacity = tiecapacity[0][shockNode] - tiecapacity[2][shockNode];
                tiecapacity[1][shockNode] = shockNodeCapacity;
                while (tiecapacity[0][shockNode] - tiecapacity[2][shockNode] < 0)//shockNodeCapacity < 0)
                {
                    List<int> minUtil = minUtility(utilitytable, modeldata, shockNode, nodecount, words);
                    if (minUtil.Count == 0)
                    {
                        if (continuecount < 5)
                        {
                            continuecount++;
                            continue;
                        }
                        else
                        {
                            System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + shockNode + " in network with runno " + runno + " failed to drop nodes when shocked." + Environment.NewLine);
                            break;
                        }
                    }
                    int selectedNode = minUtil[RNGen.Next(minUtil.Count)];

                    //drop edge with node offering least utility
                    dropEdge(ref modeldata, ref utilitytable, ref tiecapacity, shockNode, selectedNode, nodecount);
                    /*
                     utilitytable[shockNode * nodecount + selectedNode, 7] = 0;//offer
                     modeldata[selectedNode * nodecount + selectedNode, 22] = 0;//accc
                     modeldata[selectedNode * nodecount + shockNode, 21] = 0;//accr
                     modeldata[shockNode * nodecount + selectedNode, 4] = 0;//remove edge
                     modeldata[shockNode * nodecount + selectedNode, 23] = 1;//droppedr
                     utilitytable[shockNode * nodecount + selectedNode, 6] = 0;//edge
                     utilitytable[shockNode * nodecount + selectedNode, 8] = 0;//accept
                     modeldata[selectedNode * nodecount + shockNode, 4] = 0;//remove edge
                     modeldata[selectedNode * nodecount + shockNode, 24] = 1;//droppedc
                     utilitytable[selectedNode * nodecount + shockNode, 6] = 0;//edge
                     utilitytable[selectedNode * nodecount + shockNode, 8] = 0;//accept
                     tiecapacity[1][selectedNode] += 1;
                     tiecapacity[2][selectedNode] -= 1;
                     tiecapacity[1][shockNode] += 1;
                     tiecapacity[2][shockNode] -= 1;
                     */
                    /*if (tiecapacity[2][selectedNode] <= 0)
                        netnodes++;
                    if (tiecapacity[2][shockNode] <= 0)
                        netnodes++;
                    netedges--;
                    System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "SHOCK: Node " + (shockNode + 1) + " in network with runno " + runno + " dropped node " + (selectedNode + 1)/*
                                         + " ShockNodeCap = " + (shockNodeCapacity + 1)  + "." + Environment.NewLine);

                    utilitytable = updateUtility(utilitytable, ref modeldata, tiecapacity, nodecount, netedges, netnodes, homophily); //check netedges and netnodes
                    shockNodeCapacity++;
                }
                shockedNodes.Remove(shockNode);

            }*/
            //System.IO.File.AppendAllText(words[0] + "-shock" + "~" + runno + "." + words[1], /*"runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,droppedr,droppedc,initial" + Environment.NewLine + */modeldata.ToCSV(initialnodes, nodecount));
            tempout = new Matrix(modeldata);
            output[0].Add(tempout);
            rewiringSequence = getSequence(nodecount);
            for (int i = 0; i < 5; ++i)
            {
                rewiringSequence.AddRange(getSequence(nodecount));
            }
            output.Add(networkRewiring(ref controlMatrix, nodecount, ref controlUtilityTable, ref controlTieCapacity, initialnodes, words, runno, network, ref netnodes, ref netedges, "C", rewiringSequence, homophily));
            output.Add(networkRewiring(ref modeldata, nodecount, ref utilitytable, ref tiecapacity, initialnodes, words, runno, network, ref netnodes, ref netedges, "T", rewiringSequence, homophily));



            /*int actualdegree, tcap;//, dropped;
            for (int i = 0; i < nodecount; i++)
            {
                tcap = (int)modeldata[i * nodecount, 15];
                actualdegree = 0;
                //dropped = 0;
                for (int j = 0; j < nodecount; j++)
                {
                    actualdegree += (int)modeldata[i * nodecount + j, 4];
                    //dropped += (int)modeldata[i * nodecount + j, 17];
                }
                if (actualdegree > tcap)
                    System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + (i + 1) + " in network with runno " + runno + " exceeded its tie capacity." + Environment.NewLine);
            }*/
            //System.IO.File.AppendAllText(words[0] + "-preshock." + words[1], "runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,droppedr,droppedc,initial" + Environment.NewLine + modeldata.ToCSV(initialnodes, nodecount) + Environment.NewLine);
            //apform.Invoke(apform.abmdelegate);
            return output;
        }
        public int outofNetwork(List<int[]> tiecap)
        {
            int count = 0;
            for (int i = 0; i < tiecap[2].Length; i++)
            {
                if (tiecap[2][i] == 0)
                {
                    count++;
                }
            }
            return count;

        }
        public List<int> minUtility(Matrix utilitytable, Matrix modeldata, int node, int nodecount, string[] words)
        {
            double minutil = 9999999999;
            for (int i = node * nodecount; i < node * nodecount + nodecount; i++)
            {
                if (i % nodecount != node && utilitytable[i, 2] <= minutil && modeldata[i, 4] == 1)
                    minutil = utilitytable[i, 2];
            }
            List<int> minutillist = new List<int>();
            int degree = 0;
            for (int i = node * nodecount; i < node * nodecount + nodecount; i++)
            {
                if (i % nodecount != node && utilitytable[i, 2] == minutil && modeldata[i, 4] == 1)
                    minutillist.Add(i % nodecount);
                if (modeldata[i, 4] == 1)
                    degree++;
            }
            //if (degree != 0 && minutillist.Count == 0)
                //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + node + "had nonzero degree but no minimum utility edges. Minimum utility = " + minutil + " Degree = " + degree + Environment.NewLine);
            return minutillist;
        }

        public List<int> maxUtility(Matrix utilitytable, int node, int nodecount)
        {
            double maxutil = 0;
            //System.IO.File.AppendAllText("C:\\Users\\Mike\\Desktop\\Testing\\log.txt", "UTILITY {");
            for (int i = node * nodecount; i < node * nodecount + nodecount; i++)
            {
                //System.IO.File.AppendAllText("C:\\Users\\Mike\\Desktop\\Testing\\log.txt", utilitytable[i, 2] + " ");
                if (i % nodecount != node && utilitytable[i, 2] > maxutil && utilitytable[i, 6] != 1 && utilitytable[i, 7] != 1)
                    maxutil = utilitytable[i, 2];
            }
            List<int> maxutillist = new List<int>();
            //System.IO.File.AppendAllText("C:\\Users\\Mike\\Desktop\\Testing\\log.txt", "Node " + node + " 's Max utility was " + maxutil + ".");
            for (int i = node * nodecount; i < node * nodecount + nodecount; i++)
            {
                if (utilitytable[i, 2] == maxutil && utilitytable[i, 6] != 1 && i % nodecount != node && utilitytable[i, 7] != 1)
                    maxutillist.Add(i % nodecount);

            }
            //System.IO.File.AppendAllText("C:\\Users\\Mike\\Desktop\\Testing\\log.txt", " " + maxutillist.Count + " }" + Environment.NewLine);
            return maxutillist;
        }

        public double calcUtility(int node1, int node2, int nodecount,/*int degj,*/ int netedges, int netnodes, ref Matrix modeldata, List<int[]> tiecapacity)
        {
            //return (double)(degj + 1) / (netedges + netnodes);
            List<int> firstordermodellist = new List<int>();
            for (int i = 0; i < nodecount; ++i)
            {
                if (i != node1)
                {
                    firstordermodellist.Add((int)modeldata[node2 * nodecount + i, 4]);
                }
                else
                {
                    firstordermodellist.Add(0);
                }
            }
            List<double> netutilityvector = new List<double>();
            double utility, cost, netutility;
            double firstutil = 0, firstcost = 0;
            for (int i = 0; i < nodecount; ++i)
            {
                utility = netedges + netnodes > 0 ? (double) (tiecapacity[2][i] + 1) / (netedges + netnodes) : 0;
                firstutil += utility;
                cost = netnodes > 0 ? utility / netnodes : 0;
                firstcost += cost;
                netutility = utility - cost > 0 ? utility - cost : 0;
                netutilityvector.Add(netutility);
            }
            modeldata[node1 * nodecount + node2, 26] = firstutil;
            modeldata[node1 * nodecount + node2, 27] = firstcost;
            double secondorderutility = 0;
            for (int i = 0; i < nodecount; ++i)
            {
                secondorderutility += netutilityvector[i] * firstordermodellist[i];
            }
            modeldata[node1 * nodecount + node2, 28] = secondorderutility * secondorderutility;
            return netutilityvector[node1] * firstordermodellist[node1] + secondorderutility * secondorderutility;

            /*
            Matrix firstordermodel = new Matrix(nodecount, nodecount);
            for (int i = 0; i < nodecount * nodecount; ++i)
            {
                if (i % nodecount == node1 || i / nodecount == node1)
                {
                    firstordermodel[i / nodecount, i % nodecount] = 0;
                }
                else
                {
                    firstordermodel[i / nodecount, i % nodecount] = modeldata[i, 4];
                }
            }

            double utility;// = (getDegree(node2, nodecount, modeldata) + 1) / (netedges + netnodes);
            double cost, netutil;
            Matrix utilitymatrix = new Matrix(nodecount, nodecount);
            for (int i = 0; i < nodecount; ++i)
            {
                if (netedges == 0 || netnodes == 0)
                    utility = 0.0;
                else
                    utility = (double) (getDegree(i, nodecount, modeldata) - 1) / (netedges + netnodes);
                cost = utility / netnodes;
                netutil = utility - cost > 0 ? utility - cost : 0;
                for (int j = 0; j < nodecount; ++j)
                {
                    utilitymatrix[j, i] = i != node1 && j != node1 ? netutil : 0;
                }
            }
            for(int i = 0; i < nodecount; ++i)
            {
                for(int j = 0; j < nodecount; ++j)
                {
                    utilitymatrix[i, j] = utilitymatrix[i, j] * firstordermodel[i, j];
                }
            }
            double firstorderutility = utilitymatrix[1, node2];
            double secondorderutility = 0;
            for (int i = 0; i < nodecount; ++i)
            {
                secondorderutility += utilitymatrix[node2, i];
            }
            return firstorderutility + secondorderutility * secondorderutility;
             */
        }

        public double calculateHomophilyUtility(int row, int col, int nodecount, int netnodes, ref Matrix modeldata)
        {
            List<double> firstorderutility = new List<double>();
            double utility, cost, netutility;
            for (int i = 0; i < nodecount; ++i)
            {
                if (modeldata[row, 7] == 1)
                {
                    
                    utility = (.5 * modeldata[row * nodecount + i, 7] * modeldata[row * nodecount + i, 10]) + (.3 * modeldata[row * nodecount + i, 8] * modeldata[row * nodecount + i, 11]) + (.2 * modeldata[row * nodecount + i, 9] * modeldata[row * nodecount + i, 12]);
                    if (row == col)
                        utility = 0;
                    cost = netnodes > 0 ? utility / netnodes : 0;
                    netutility = utility - cost > 0 ? utility - cost : 0;
                    modeldata[row * nodecount + i, 26] = utility;
                    modeldata[row * nodecount + i, 27] = cost;
                    firstorderutility.Add(netutility);
                }
                else
                {

                    utility = (.5 * modeldata[row * nodecount + i, 8] * modeldata[row * nodecount + i, 11]) + (.3 * modeldata[row * nodecount + i, 9] * modeldata[row * nodecount + i, 12]) + (.2 * modeldata[row * nodecount + i, 10]);
                    if (row == col)
                        utility = 0;
                    cost = netnodes > 0 ? utility / netnodes : 0;
                    netutility = utility - cost > 0 ? utility - cost : 0;
                    modeldata[row * nodecount + i, 26] = utility;
                    modeldata[row * nodecount + i, 27] = cost;
                    firstorderutility.Add(netutility);
                }
            }
            //List<double> secondorderutility = new List<double();
            double secondutility = 0.0;
            for (int i = 0; i < nodecount; ++i)
            {
                if(i != row && col != row && modeldata[row * nodecount + i] == 1)
                {
                    secondutility += firstorderutility[i];
                }
            }
            modeldata[row * nodecount + col, 28] = secondutility * secondutility;

            return firstorderutility[col] + secondutility * secondutility;
                       
            /*
            double firstorderutility;
            if (modeldata[node1, 7] == 1)
                firstorderutility = 0.5 * modeldata[node1, 10] * modeldata[node2, 10] + .3 * modeldata[node1, 11] * modeldata[node2, 11] + .2 * modeldata[node1, 12] * modeldata[node2, 12];
            else
                firstorderutility = 0.2 * modeldata[node2, 10] + .5 * modeldata[node1, 11] * modeldata[node2, 11] + .3 * modeldata[node1, 12] * modeldata[node2, 12];

            double cost = netnodes > 0 ? firstorderutility / netnodes : 0;
            */
        }

        public Matrix updateModel(Matrix modeldata, List<int[]> tiecapacity, int nodecount)
        {
            /*for (int i = 0; i < nodecount * nodecount; i++)
            {
                //modeldata[i, 5] = tiecapacity[1][(int)i / 10];
                //modeldata[i, 6] = tiecapacity[1][i % 10];

            }*/
            return modeldata;
        }

        public Matrix updateUtility(Matrix utilitytable, ref Matrix modeldata, List<int[]> tiecapacity, int nodecount, int nete, int netn, bool homophily)
        {
            if (homophily)
            {
                /*
                List<double> utilityvector;
                for (int i = 0; i < nodecount; ++i)
                {
                    utilityvector = calculateHomophilyUtility(i, nodecount, nete, ref modeldata);
                    for (int j = 0; j < nodecount; ++j)
                    {
                        utilitytable[i * nodecount + j, 5] = tiecapacity[1][j];
                        utilitytable[i * nodecount + j, 4] = utilitytable[i * nodecount + j, 3] - utilitytable[i * nodecount + j, 5];
                        utilitytable[i * nodecount + j, 2] = utilityvector[j];
                    }
                }*/
                for (int i = 0; i < nodecount * nodecount; ++i)
                {
                    utilitytable[i, 5] = tiecapacity[1][i % nodecount];//update tie capacity
                    utilitytable[i, 4] = utilitytable[i, 3] - utilitytable[i, 5];//update degree
                    utilitytable[i, 2] = calculateHomophilyUtility(i / nodecount, i % nodecount, nodecount, netn, ref modeldata);//update utility
                }

            }
            else
            {
                for (int i = 0; i < nodecount * nodecount; i++)
                {
                    utilitytable[i, 5] = tiecapacity[1][i % nodecount];//update tie capacity
                    utilitytable[i, 4] = utilitytable[i, 3] - utilitytable[i, 5];//update degree
                    utilitytable[i, 2] = calcUtility(i / nodecount, i % nodecount, nodecount, /*tiecapacity[2][i % nodecount],*/ nete, netn, ref modeldata, tiecapacity);//update utility
                    modeldata[i, 26] = utilitytable[i, 2];
                }
            }
            return utilitytable;
        }



        public List<Matrix> networkRewiring(ref Matrix modeldata, int nodecount, ref Matrix utilitytable, ref List<int[]> tiecapacity, List<int> initialnodes, string[] words, int runno, int network, ref int netnodes, ref int netedges, string type, List<int> nodeselection, bool homophily)
        {
            System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "REWIRING STAGE FOR NETWORK OF TYPE \"" + type + "\" BEGUN." + Environment.NewLine);
            int rewireloopcount = 0;
            int rewiringnode;
            bool rewired = false;
            int iteration = 1;
            int rewseq = 0;
            //List<int> selectednodes = new List<int>();

            List<Matrix> matrixlist = new List<Matrix>();
            Matrix temp;

            
            int sequence = 1;
            int lastoutput = 0;
            List<int> edges = new List<int>(nodecount * nodecount);
            for (int i = 0; i < nodecount * nodecount; ++i)
            {
                edges.Add(-1);
            }
            for (int k = 0; k < nodeselection.Count; ++k)
            {
                rewired = false;
                if (rewireloopcount % nodecount == 0)
                {
                    sequence = 1;
                    for (int i = 0; i < nodecount * nodecount; i++)
                    {
                        modeldata[i, 17] = 0;
                        modeldata[i, 18] = 0;
                        modeldata[i, 19] = 0; //offerr
                        modeldata[i, 20] = 0;  //offerc
                        modeldata[i, 22] = 0;//accc
                        modeldata[i, 21] = 0;//accr
                        modeldata[i, 23] = 0;//droppedc
                        modeldata[i, 24] = 0;//droppedr
                        modeldata[i, 26] = 0;
                        modeldata[i, 27] = 0;
                        modeldata[i, 28] = 0;
                        utilitytable[i, 7] = 0;
                        utilitytable[i, 8] = 0;
                    }
                }
                rewiringnode = nodeselection[k];
                for (int i = 0; i < nodecount; i++)
                {
                    modeldata[rewiringnode * nodecount + i, 1] = iteration;
                    modeldata[rewiringnode * nodecount + i, 17] = sequence;
                    modeldata[i * nodecount + rewiringnode, 18] = sequence;
                }
                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " was selected for rewiring with sequence number " + sequence + ". File: " + (rewireloopcount / nodecount + 1) + "N" + type + Environment.NewLine);
                iteration++;
                sequence++;

                int offercount = 0;
                while (offercount < nodecount)
                {
                    List<int> maxutil = maxUtility(utilitytable, rewiringnode, nodecount);
                    //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "  REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " had a MaxUtil of length " + maxutil.Count + ". File: " + (rewireloopcount / nodecount + 1) + "N" + type + Environment.NewLine);
                    if (maxutil.Count == 0)
                        break;
                    while (maxutil.Count > 0)
                    {
                        int offerednode = maxutil[RNGen.Next(maxutil.Count)];
                        //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "    REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " attempted to rewire with node " + offerednode + ". File: " + (rewireloopcount / nodecount + 1) + "N" + type + Environment.NewLine);
                        offercount++;
                        //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "      REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " had edge stats of " + utilitytable[rewiringnode * nodecount + offerednode, 6] + " and " + modeldata[rewiringnode * nodecount + offerednode, 4] + ". File: " + (rewireloopcount / nodecount + 1) + "N" + type + Environment.NewLine);
                        if (utilitytable[rewiringnode * nodecount + offerednode, 6] == 0 || modeldata[rewiringnode * nodecount + offerednode, 4] == 0)
                        {
                            utilitytable[rewiringnode * nodecount + offerednode, 7] = 1;
                            utilitytable[offerednode * nodecount + rewiringnode, 7] = 1;
                            modeldata[rewiringnode * nodecount + offerednode, 19] = 1;
                            modeldata[offerednode * nodecount + rewiringnode, 20] = 1;

                            List<int> minUtil = minUtility(utilitytable, modeldata, offerednode, nodecount, words);
                            if (minUtil.Count == 0)
                            {
                                maxutil.Remove(offerednode);
                                break;
                            }
                            int dropnode = minUtil[RNGen.Next(minUtil.Count)];
                            //System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "      REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " had " + tiecapacity[1][rewiringnode] + " tie capacity and a degree of " + tiecapacity[2][rewiringnode] + ". Offered node had " + tiecapacity[1][offerednode] + " tie capacity. File: " + (rewireloopcount / nodecount + 1) + "N" + type + Environment.NewLine);
                            if (tiecapacity[1][rewiringnode] > 0 && tiecapacity[1][offerednode] > 0)// && tiecapacity[2][rewiringnode] > 0)
                            {
                                //if (rewseq < 10)
                                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "      REWIRE: Node " + (rewiringnode + 1) + " in network with runno " + runno + " formed a new edge with node " + (offerednode + 1) +
                                    ". Remaining tie Capacity of node " + (rewiringnode + 1) + ": " + tiecapacity[1][rewiringnode] + ". Remaining tie capacity of node " + (offerednode + 1) + ": "
                                    + tiecapacity[1][offerednode] + ". Iteration: " + rewireloopcount + " File: " + ((rewireloopcount + 1) / nodecount + 1) + "N" + type + Environment.NewLine);
                                rewseq++;
                                addEdge(ref modeldata, ref utilitytable, ref tiecapacity, rewiringnode, offerednode, nodecount);
                                /*
                                modeldata[rewiringnode * nodecount + offerednode, 19] = 1; //offerr
                                modeldata[offerednode * nodecount + rewiringnode, 20] = 1;  //offerc
                                utilitytable[rewiringnode * nodecount + offerednode, 7] = 1;
                                modeldata[rewiringnode * nodecount + offerednode, 22] = 1;
                                modeldata[offerednode * nodecount + rewiringnode, 21] = 1;
                                modeldata[rewiringnode * nodecount + offerednode, 4] = 1;
                                utilitytable[rewiringnode * nodecount + offerednode, 6] = 1;
                                utilitytable[rewiringnode * nodecount + offerednode, 8] = 1;
                                tiecapacity[1][rewiringnode] -= 1;
                                tiecapacity[2][rewiringnode] += 1;
                                modeldata[offerednode * nodecount + rewiringnode, 4] = 1;
                                utilitytable[offerednode * nodecount + rewiringnode, 6] = 1;
                                utilitytable[offerednode * nodecount + rewiringnode, 8] = 1;
                                tiecapacity[1][offerednode] -= 1;
                                tiecapacity[2][offerednode] += 1;
                                */
                                rewired = true;
                                netedges++;
                            }
                            else if (modeldata[offerednode * nodecount + dropnode, 4] == 1 && utilitytable[offerednode * nodecount + dropnode, 2] < utilitytable[offerednode * nodecount + rewiringnode, 2] && tiecapacity[1][rewiringnode] > 0)
                            {

                                modeldata[rewiringnode * nodecount + offerednode, 19] = 1; //offerr
                                modeldata[offerednode * nodecount + rewiringnode, 20] = 1;  //offerc

                                //drop edge with node offering least utility
                                dropEdge(ref modeldata, ref utilitytable, ref tiecapacity, offerednode, dropnode, nodecount);
                                /*
                                utilitytable[offerednode * nodecount + dropnode, 7] = 0;//offer
                                modeldata[offerednode * nodecount + dropnode, 22] = 0;//accc
                                modeldata[dropnode * nodecount + offerednode, 21] = 0;//accr
                                modeldata[offerednode * nodecount + dropnode, 4] = 0;//remove edge
                                modeldata[offerednode * nodecount + dropnode, 24] = 1;//droppedc
                                utilitytable[offerednode * nodecount + dropnode, 6] = 0;//edge
                                utilitytable[offerednode * nodecount + dropnode, 8] = 0;//accept
                                modeldata[dropnode * nodecount + offerednode, 4] = 0;//remove edge
                                modeldata[dropnode * nodecount + offerednode, 23] = 1;//droppedr
                                utilitytable[dropnode * nodecount + offerednode, 6] = 0;//edge
                                utilitytable[dropnode * nodecount + offerednode, 8] = 0;//accept
                                tiecapacity[1][dropnode] += 1;
                                tiecapacity[2][dropnode] -= 1;
                                */

                                //add edge with new node offering greater utility
                                addEdge(ref modeldata, ref utilitytable, ref tiecapacity, rewiringnode, offerednode, nodecount);
                                /*
                                utilitytable[rewiringnode * nodecount + offerednode, 7] = 1;
                                modeldata[rewiringnode * nodecount + offerednode, 22] = 1;
                                modeldata[offerednode * nodecount + rewiringnode, 21] = 1;
                                modeldata[rewiringnode * nodecount + offerednode, 4] = 1;
                                utilitytable[rewiringnode * nodecount + offerednode, 6] = 1;
                                utilitytable[rewiringnode * nodecount + offerednode, 8] = 1;
                                tiecapacity[1][rewiringnode] -= 1;
                                tiecapacity[2][rewiringnode] += 1;
                                modeldata[offerednode * nodecount + rewiringnode, 4] = 1;
                                utilitytable[offerednode * nodecount + rewiringnode, 6] = 1;
                                utilitytable[offerednode * nodecount + rewiringnode, 8] = 1;
                                */
                                //if (rewseq < 10)
                                rewired = true;
                                System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "      REWIRE: Node " + (offerednode + 1) + " in network with runno " + runno + " added node " + (rewiringnode + 1) + " and dropped node " + (dropnode + 1)
                                    + ". Remaining tie Capacity of node " + (rewiringnode + 1) + ": " + tiecapacity[1][rewiringnode] + ". Remaining tie capacity of node " + (offerednode + 1) + ": "
                                    + tiecapacity[1][offerednode] + ". Iteration: " + rewireloopcount + " File: " + ((rewireloopcount + 1) / nodecount + 1) + "N" + type + Environment.NewLine);
                                rewseq++;
                                if (modeldata[offerednode * nodecount + dropnode, 4] == 1 || modeldata[dropnode * nodecount + offerednode, 4] == 1)
                                    System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + (offerednode + 1) + " in network with runno " + runno + " did not drop node " + (dropnode + 1) + "." + Environment.NewLine);
                                if (modeldata[rewiringnode * nodecount + offerednode, 4] != 1 || modeldata[offerednode * nodecount + rewiringnode, 4] != 1)
                                    System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "ERROR: Node " + (offerednode + 1) + " in network with runno " + runno + " did not gain an edge with " + (rewiringnode + 1) + "." + Environment.NewLine);
                            }
                        }
                        maxutil.Remove(offerednode);
                        utilitytable = updateUtility(utilitytable, ref modeldata, tiecapacity, nodecount, netedges, netnodes, homophily);
                    }
                }
                rewireloopcount++;
                if (rewireloopcount % nodecount == 0)
                {
                    //System.IO.File.AppendAllText(words[0] + "-" + (rewireloopcount / nodecount) + "N" + type +/* "-" + network +*/ "~" + runno + "." + words[1], /*"runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,droppedr,droppedc,initial" + Environment.NewLine + */modeldata.ToCSV(initialnodes, nodecount));
                    temp = new Matrix(modeldata);
                    matrixlist.Add(temp);
                    lastoutput++;
                    bool quit = true;
                    for (int i = 0; i < nodecount * nodecount; ++i)
                    {
                        if ((int)modeldata[i, 4] != edges[i])
                        {
                            edges[i] = (int)modeldata[i, 4];
                            quit = false;
                        }
                    }
                    if (quit)
                    {
                        //apform.Invoke(apform.quitdelegate);
                        break;
                    }
                }
                
                //apform.Invoke(apform.abmdelegate);
            }



            //}
            //for (int i = lastoutput; i < 6; i++)
            //{
            //System.IO.File.AppendAllText(words[0] + "-" + (i + 1) + "N" + type + /*"-" + network +*/ "." + words[1], /*"runno,iteration,row,col,edge,C0r,C0c,kr,kc,Csr,Csc,Seqr,Seqc,Offerr,Offerc,Accr,Accc,droppedr,droppedc,initial" + Environment.NewLine + */modeldata.ToCSV(initialnodes, nodecount));
            //}
            System.IO.File.AppendAllText(words[0] + "-log" + "~" + runno + ".txt", "REWIRING: Rewiring stage finished after " + rewireloopcount + "iterations." + Environment.NewLine);
            return matrixlist;
        }

    }



    // Implements additional IO functions for Network class
    public class NetworkIO : Network
    {

        // We provide two constructors to match those of our parent
        public NetworkIO()
            : base()
        {
        }

        public NetworkIO(NetworkGUI viableSource)
            : base()
        {
            // make a data matrix out of the viable coalitions matrix
            Matrix viable = viableSource.GetMatrix("ViableCoalitions");
            Matrix srcData = viableSource.GetMatrix("Data");
            if (viable.GetColVector(0).IsZeroVector)
            {
                mTable["Data"] = null;
                return;
            }

            Matrix newData = new Matrix(srcData.Rows);
            srcData.CloneTo(newData);
            newData.Clear();

            // count the number of set elements
            bool[] setElements = new bool[srcData.Rows];
            Algorithms.Fill<bool>(setElements, false);
            for (int i = 0; i < viable.Cols; ++i)
            {
                for (int j = 0; j < viable.Rows - 3; ++j)
                {
                    if (viable[j, i] == 1)
                        setElements[j] = true;
                    else continue;

                    for (int k = j + 1; k < viable.Rows - 3; ++k)
                    {
                        if (viable[k, i] != 1) continue;

                        newData[j, k] = newData[k, j] = 1;
                    }
                }
            }

            List<int> keepRows = new List<int>();
            for (int i = 0; i < setElements.Length; ++i)
                if (setElements[i]) keepRows.Add(i);

            mTable["Data"] = newData.SubmatrixWithRows(keepRows);
        }

        public enum CharacteristicType
        {
            Sum, Mean, StdDev, Min, Max
        }

        public double GO1()
        {
            if (_blockDensity == null)
                throw new Exception("Density not loaded");
            double sum = 0.0;
            int index = 0;
            double GO1 = 0;
            double totalSum = 0;            
           
            while (index < _blockDensity.Rows)
            {
                for (int j = 0; j < _blockDensity.Rows; j++)
                    sum += _blockDensity[j + index * _blockDensity.Rows];
                sum -= _blockDensity[index * _blockDensity.Rows + index];
                totalSum += sum;
                sum = 0;
                index++;
            }
            GO1 = totalSum / (_blockDensity.Rows * (_blockDensity.Rows - 1));
            return GO1;
        }

        

        public void LoadCliqueCharacteristics(List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix, int year, double binaryCutoff)
        {
            LoadCliqueCharacteristics(svc, dvc, attrMatrix, year, 0.000001, binaryCutoff);
        }

        public void LoadNetworkpower(bool file, int year, bool useclique, bool useComm, bool calcSP, string inputType, string cohesionfile, double maxik)
        {
            if (!useclique && !useComm)
            {
                if (Blocks == null)
                    throw new Exception("Must run CONCOR before loading block afilliation");
            }
            else if (useComm)
            {
                if (communities == null)
                    throw new Exception("No communities have been found!");
            }
            else
            {
                if (_cliques == null)
                    throw new Exception("No cliques have been found!");
            }


            Matrix Temp;
            if (inputType == "StructEquiv" || inputType == "None")
                Temp = new Matrix(mTable["SESE"]);
            else
            {
                Matrix CohesionMatrix = MatrixReader.ReadMatrixFromFile(cohesionfile, year);
                Temp = new Matrix(CohesionMatrix);
            }

            int n = mTable["Data"].Rows;
            List<double> tempattribute = new List<double>();
            if (file)
            {
                for (int i = 0; i < npnetwork.Count; i++)
                {
                    if (npnetwork[i] == year)
                    {
                        tempattribute.Add(npattribute[i]);
                        if (i == npnetwork.Count - 1 || npnetwork[i + 1] != year)
                            break;
                    }
                }
            }

            int count = 0;
            int winning = 0;
            if (useclique) count = _cliques.Count;
            else if (useComm) count = comNum;
            else count = Blocks.Count;

            Matrix WCA = new Matrix(n + 2, count);
            Matrix CCA = new Matrix(n + 2, count);
            ComputeWcliqueSum(WCA, CCA, n, Blocks, _cliques, useclique, useComm, Temp, tempattribute, file);
            double npsum = 0.0, npsum2 = 0.0;
            for (int i = 0; i < count; i++)
                if (WCA[n + 1, i] >= CCA[n + 1, i]) winning++;

            mTable["NetworkPower"] = new Matrix(mTable["Data"].Rows, calcSP ? 12 : 8);

            for (int node = 0; node < n; node++)
            {
                NPNode npNode = new NPNode();
                npNode.ComputeNP(WCA, CCA, node, n, Blocks, _cliques, communities, comNum, useComm, useclique, Temp, tempattribute, file);

                mTable["NetworkPower"][node, 0] = year;
                mTable["NetworkPower"][node, 1] = node + 1;
                if (file) mTable["NetworkPower"][node, 2] = tempattribute[node];

                else mTable["NetworkPower"][node, 2] = 1.0 / n;
                mTable["NetworkPower"][node, 3] = npNode.NP / count;
                mTable["NetworkPower"][node, 5] = npNode.NP / winning;
                mTable["NetworkPower"][node, 7] = npNode.NPabs / count;
                npsum += npNode.NP / count;
                if (winning == 0)
                    npsum2 += 0;
                else
                    npsum2 += npNode.NP / winning;
            }


            double spsum = 0.0;
            double sp2sum = 0.0;
            if (calcSP)
            {
                Matrix SP = new Matrix(mTable["Data"]);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        SP[i, j] = -1;

                Matrix SP2 = new Matrix(mTable["Data"]);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        SP[i, j] = -1;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        //if (SP[i, j] != -1) continue;
                        Matrix DataNode = new Matrix(mTable["Data"]);
                        DataNode[i, j] = 0;
                        DataNode[j, i] = 0;

                        Temp = MatrixComputations.StructuralEquivalenceStandardizedEuclidean(DataNode, maxik);

                        SymmetricBinaryMatrix MBinary = new SymmetricBinaryMatrix(DataNode, 0, CliqueExtractionType.Min);
                        CliqueCollection spCliques = new CliqueCollection(MBinary, 1);

                        int cCount = spCliques.Count;

                        Matrix WCAsp = new Matrix(n + 2, cCount);
                        Matrix CCAsp = new Matrix(n + 2, cCount);

                        // may not be false for useComm
                        ComputeWcliqueSum(WCAsp, CCAsp, n, null, spCliques, true, useComm, Temp, tempattribute, file);

                        int winningsp = 0;
                        for (int k = 0; k < cCount; k++)
                            if (WCAsp[n + 1, k] >= CCAsp[n + 1, k]) winningsp++;

                        NPNode npNode = new NPNode();
                        npNode.ComputeNP(WCAsp, CCAsp, j, n, null, spCliques, communities, comNum, useComm, true, Temp, tempattribute, file);

                        if (npNode.NP / cCount < mTable["NetworkPower"][j, 3]) //if it is smaller than NP1
                        {
                            SP[i, j] = mTable["NetworkPower"][j, 3] - npNode.NP / cCount; 
                        }
                        else
                        {
                            SP[i, j] = 0; 
                        }

                        if (npNode.NP / winningsp < mTable["NetworkPower"][j, 5]) //if it is smaller than NP2
                        {
                            SP2[i, j] = mTable["NetworkPower"][j, 5] - npNode.NP / winningsp;
                        }
                        else
                        {
                            SP2[i, j] = 0;
                        }

                        //if (npNode.NP / winningsp < mTable["NetworkPower"][i, 5]) //if it is smaller than NP2
                        //{
                        //    SP[j, i] = mTable["NetworkPower"][i, 5] - npNode.NP / winningsp; 
                        //}
                        //else
                        //{ 
                        //    SP[j, i] = 0;
                        //}

                        /*
                        if (i == 1)
                        {
                            MessageBox.Show("j is now " + j);
                            MessageBox.Show(mTable["NetworkPower"][j, 3] + ""); 
                            MessageBox.Show(npNode.NP / cCount + "");
                            MessageBox.Show(SP[i, j] + "");
                        }
                        */
                    }
                    Matrix tmpData = new Matrix(mTable["Data"]);
                    tmpData.Binarize();
                    mTable["NetworkPower"][i, 8] = (SP.GetRowSum(i) - SP[i, i]) / (tmpData.GetRowSum(i) - tmpData[i, i]);
                    spsum += mTable["NetworkPower"][i, 8];
                    mTable["NetworkPower"][i, 10] = (SP2.GetRowSum(i) - SP2[i, i]) / (tmpData.GetRowSum(i) - tmpData[i, i]);
                    sp2sum += mTable["NetworkPower"][i, 10];
                }
            }
                
            for (int row = 0; row < n; row++)
            {
                mTable["NetworkPower"][row, 4] = mTable["NetworkPower"][row, 3] / npsum;
                mTable["NetworkPower"][row, 6] = mTable["NetworkPower"][row, 5] / npsum2;
                if (calcSP) mTable["NetworkPower"][row, 9] = mTable["NetworkPower"][row, 8] / spsum;
                if (calcSP) mTable["NetworkPower"][row, 11] = mTable["NetworkPower"][row, 10] / sp2sum;
                mTable["NetworkPower"].RowLabels[row] = mTable["Data"].RowLabels[row];
            }

            mTable["NetworkPower"].ColLabels[0] = "Network Identifier";
            mTable["NetworkPower"].ColLabels[1] = "Node #";
            mTable["NetworkPower"].ColLabels[2] = "Attribute";
            mTable["NetworkPower"].ColLabels[3] = "NP1";
            mTable["NetworkPower"].ColLabels[4] = "Normalized NP1";
            mTable["NetworkPower"].ColLabels[5] = "NP2";
            mTable["NetworkPower"].ColLabels[6] = "Normalized NP2";
            mTable["NetworkPower"].ColLabels[7] = "NPabs";
            if (calcSP)
            {
                mTable["NetworkPower"].ColLabels[8] = "Spoiling Power";
                mTable["NetworkPower"].ColLabels[9] = "Normalized SP";
                mTable["NetworkPower"].ColLabels[10] = "Spoiling Power 2";
                mTable["NetworkPower"].ColLabels[11] = "Normalized SP2";
            }
        }


        // An added function for Communities to calculate the Cohesion of the group
        public double ComputeCommCohesion(Matrix SESE, int[] community)
        {
            double cohesion = 0.0;
            int Size = 0;
            for (int i = 0; i < comSize; i++)
            {
                if (community[i] != 0)
                    Size++;
            }
            if (Size == 1)
                return 1.0;

            for (int i = 0; i < comSize; i++)
            {
                if (community[i] == 0)
                    continue;
                for (int j = 0; j < comSize; j++)
                {
                    if (community[j] != 0)
                        cohesion += SESE[i, j];
                }
            }          
            cohesion = (2.0 * cohesion) / (Size * (Size - 1));
            return cohesion;
        }

        // An added function for Communities to calculate the Wsum of the group
        public double ComputeCommWsum(int[] community, List<double> attribute, bool file, double cohesion)
        {
            double wsum = 0.0;
            if (file)
            {
                for (int i = 0; i < comSize; ++i)
                    if (community[i] != 0)
                        wsum += attribute[i];
            }
            else
            {
                for (int i = 0; i < comSize; ++i)
                    if (community[i] != 0)
                        wsum += 1.0 / comSize;
            }
            wsum *= cohesion;
            return wsum;
        }

        public void ComputeWcliqueSum(Matrix WCA, Matrix CCA, int n, List<List<int> > Blocks, CliqueCollection _cliques, bool isClique, bool isComm, Matrix Cohesion, List<double> tempattribute, bool file)
        {
            if (!isClique && !isComm)
            { 
                for (int col = 0; col < Blocks.Count; col++)
                {
                    List<int> blocklist = new List<int>();
                    for (int row = 0; row < n; row++)
                    {
                        if (Blocks[col].Contains(row))
                        {
                            WCA[row, col] = file ? tempattribute[row] : 1/n;
                            blocklist.Add(row);
                        }
                    }
                    Clique block = new Clique(blocklist, n);
                    block.ComputeCohesion(Cohesion);
                    WCA[n, col] = block.Cohesion;
                    WCA[n + 1, col] = block.ComputeWsum(tempattribute, file);
                } 

                for (int col = 0; col < Blocks.Count; col++)
                {
                    List<int> comblocklist = new List<int>();
                    for (int row = 0; row < n; row++)
                    {
                        if (!Blocks[col].Contains(row))
                        {
                            comblocklist.Add(row);
                            CCA[row, col] = file ? tempattribute[row] : 1/n;
                        }
                    }
                    Clique comblock = new Clique(comblocklist, n);
                    comblock.ComputeCohesion(Cohesion);
                    CCA[n, col] = comblock.Cohesion;
                    CCA[n + 1, col] = comblock.ComputeWsum(tempattribute, file);
                }
            }
            else if (isComm)
            {
                for (int col = 0; col < comNum; col++)
                {
                    for (int row = 0; row < n; row++)
                    {
                        if (communities[col][row] != 0)
                            WCA[row, col] = file ? tempattribute[row] : 1 / n;
                    }
                    
                    //double cohesion = ComputeCommCohesion(Cohesion, communities[col]);
                    double cohesion = NewComputeCohesion(communities[col]);
                    WCA[n, col] = cohesion;
                    WCA[n + 1, col] = ComputeCommWsum(communities[col], tempattribute, file, cohesion);
                }

                for (int col = 0; col < comNum; col++)
                {
                    List<int> comlist = new List<int>();
                    for (int row = 0; row < n; row++)
                    {
                        if (communities[col][row] == 0)
                        {
                            comlist.Add(row);
                            CCA[row, col] = file ? tempattribute[row] : 1 / n;
                        }
                    }
                    Clique comm = new Clique(comlist, n);
                    comm.ComputeCohesion(Cohesion);
                    CCA[n, col] = comm.Cohesion;
                    CCA[n + 1, col] = comm.ComputeWsum(tempattribute, file);
                }
            }
            else //if use clique
            {

                for (int col = 0; col < _cliques.Count; col++)
                {
                    for (int row = 0; row < n; row++)
                    {
                        if (_cliques[col].Contains(row))
                            WCA[row, col] = file ? tempattribute[row] : 1 / n;
                    }
                    _cliques[col].ComputeCohesion(Cohesion);
                    WCA[n, col] = _cliques[col].Cohesion;
                    WCA[n + 1, col] = _cliques[col].ComputeWsum(tempattribute, file);
                }

                for (int col = 0; col < _cliques.Count; col++)
                {
                    List<int> comlist = new List<int>();
                    for (int row = 0; row < n; row++)
                    {
                        if (!_cliques[col].Contains(row))
                        {
                            comlist.Add(row);
                            CCA[row, col] = file ? tempattribute[row] : 1 / n;
                        }
                    }
                    Clique comclique = new Clique(comlist, n);
                    comclique.ComputeCohesion(Cohesion);
                    CCA[n, col] = comclique.Cohesion;
                    CCA[n + 1, col] = comclique.ComputeWsum(tempattribute, file);
                }
            }
        }


        /*
         * Algorithms to compute the NGT of each group
         */
        public double computeCliqueNGT(Triads triad, Clique _clique)
        {
            double sum = 0;
            for (int i = 0; i < _clique.MemberRangeSize; i++)
            {
                if (_clique.IntContains(i) == 1)
                {
                    sum += triad.getLocalTransitivity(i);
                }
            }
            sum /= _clique.Size;
            return sum;
        }

        public double computeBlockNGT(Triads triad, Block _block)
        {
            double sum = 0;
            for (int i = 0; i < _block.MemberRangeSize; i++)
            {
                if (_block.IntContains(i) == 1)
                {
                    sum += triad.getLocalTransitivity(i);
                }
            }
            sum /= _block.Size;
            return sum;
        }

        public double computeCommNGT(Triads triad, int[] _comm, int Size)
        {
            double sum = 0;
            for (int i = 0; i < comSize; i++)
            {
                if (_comm[i] == 1)
                {
                    sum += triad.getLocalTransitivity(i);
                }
            }
            sum /= Size;
            return sum;
        }

        public double computeOverlapCommNGT(Triads triad, Clique _overlapComm)
        {
            double sum = 0;
            for (int i = 0; i < cliqueSize; i++)
            {
                if (_overlapComm.IntContains(i) == 1)
                {
                    sum += triad.getLocalTransitivity(i);
                }
            }
            sum /= _overlapComm.Size;
            return sum;
        }


        /*
         * Algorithms to compute the GMT of each group
         */
        public double computeCliqueGMT(Triads triad, Clique _clique)
        {
            // make an initial list of all of the triads
            List<int[]> triadCombo = new List<int[]>();
            for (int i = 0; i < _clique.MemberRangeSize; i++)
                for (int j = 0; j < triad.getTriadListCount(i); j++)
                    triadCombo.Add(triad[i][j]);

            double sum = 0;
            double denominator = 0;
            for (int i = 0; i < _clique.MemberRangeSize; i++)
            {
                if (_clique.IntContains(i) != 1)
                    continue; // no need to check for instances where node[i] = 0
                for (int j = i + 1; j < _clique.MemberRangeSize; j++)
                {
                    if (_clique.IntContains(j) == 1)
                    {
                        for (int k = 0; k < triadCombo.Count; k++)
                        {
                            bool i_in_node = false;
                            bool j_in_node = false;
                            // check if both i and j are in the triadCombo
                            foreach (int node in triadCombo[k])
                            {
                                if (i == node)
                                    i_in_node = true;
                                if (j == node)
                                    j_in_node = true;
                            }

                            if (i_in_node == true && j_in_node == true)
                            {
                                denominator++; // increase denom for every time i and j are in triad
                                // check for transitivity
                                int I = triadCombo[k][0];
                                int J = triadCombo[k][1];
                                int K = triadCombo[k][2];
                                
                                if ((mTable["Data"][I, J] > triad.Cutoff) && (mTable["Data"][J, K] > triad.Cutoff) &&
                                    (mTable["Data"][I, K] > triad.Cutoff))
                                {
                                    sum++;
                                }
                                int[] temp = { -1, -1, -1 };
                                triadCombo[k] = temp; // to mark that the triad has been checked
                            }
                        }
                    }
                }
            }

            if (denominator == 0)
                return 0.0;
            else
                return sum / denominator;
            
        }

        public double computeBlockGMT(Triads triad, Block _block)
        {
            // make an initial list of all of the triads
            List<int[]> triadCombo = new List<int[]>();
            for (int i = 0; i < _block.MemberRangeSize; i++)
                for (int j = 0; j < triad.getTriadListCount(i); j++)
                    triadCombo.Add(triad[i][j]);

            double sum = 0;
            double denominator = 0;
            for (int i = 0; i < _block.MemberRangeSize; i++)
            {
                if (_block.IntContains(i) != 1)
                    continue; // no need to check for instances where node[i] = 0
                for (int j = i + 1; j < _block.MemberRangeSize; j++)
                {
                    if (_block.IntContains(j) == 1)
                    {
                        for (int k = 0; k < triadCombo.Count; k++)
                        {
                            bool i_in_node = false;
                            bool j_in_node = false;
                            // check if both i and j are in the triadCombo
                            foreach (int node in triadCombo[k])
                            {
                                if (i == node)
                                    i_in_node = true;
                                if (j == node)
                                    j_in_node = true;
                            }

                            if (i_in_node == true && j_in_node == true)
                            {
                                denominator++; // increase denom for every time i and j are in triad
                                // check for transitivity
                                int I = triadCombo[k][0];
                                int J = triadCombo[k][1];
                                int K = triadCombo[k][2];

                                if ((mTable["Data"][I, J] > triad.Cutoff) && (mTable["Data"][J, K] > triad.Cutoff) &&
                                    (mTable["Data"][I, K] > triad.Cutoff))
                                {
                                    sum++;
                                }
                                int[] temp = { -1, -1, -1 };
                                triadCombo[k] = temp; // to mark that the triad has been checked
                            }
                        }
                    }
                }
            }
            if (denominator == 0)
                return 0.0;
            return sum / denominator;
        }

        public double computeCommGMT(Triads triad, int[] _comm)
        {
            // make an initial list of all of the triads
            List<int[]> triadCombo = new List<int[]>();
            for (int i = 0; i < comSize; i++)
                for (int j = 0; j < triad.getTriadListCount(i); j++)
                    triadCombo.Add(triad[i][j]);

            double sum = 0;
            double denominator = 0;
            for (int i = 0; i < comSize; i++)
            {
                if (_comm[i] != 1)
                    continue; // no need to check for instances where node[i] = 0
                for (int j = i + 1; j < comSize; j++)
                {
                    if (_comm[j] == 1)
                    {
                        for (int k = 0; k < triadCombo.Count; k++)
                        {
                            bool i_in_node = false;
                            bool j_in_node = false;
                            // check if both i and j are in the triadCombo
                            foreach (int node in triadCombo[k])
                            {
                                if (i == node)
                                    i_in_node = true;
                                if (j == node)
                                    j_in_node = true;
                            }

                            if (i_in_node == true && j_in_node == true)
                            {
                                denominator++; // increase denom for every time i and j are in triad
                                // check for transitivity
                                int I = triadCombo[k][0];
                                int J = triadCombo[k][1];
                                int K = triadCombo[k][2];

                                if ((mTable["Data"][I, J] > triad.Cutoff) && (mTable["Data"][J, K] > triad.Cutoff) &&
                                    (mTable["Data"][I, K] > triad.Cutoff))
                                {
                                    sum++;
                                }
                                int[] temp = { -1, -1, -1 };
                                triadCombo[k] = temp; // to mark that the triad has been checked
                            }
                        }
                    }
                }
            }
            if (denominator == 0)
                return 0.0;
            return sum / denominator;
        }

        public double computeOverlapCommGMT(Triads triad, Clique _overlapComm)
        {
            // make an initial list of all of the triads
            List<int[]> triadCombo = new List<int[]>();
            for (int i = 0; i < cliqueSize; i++)
                for (int j = 0; j < triad.getTriadListCount(i); j++)
                    triadCombo.Add(triad[i][j]);

            double sum = 0;
            double denominator = 0;
            for (int i = 0; i < cliqueSize; i++)
            {
                if (_overlapComm.IntContains(i) != 1)
                    continue; // no need to check for instances where node[i] = 0
                for (int j = i + 1; j < cliqueSize; j++)
                {
                    if (_overlapComm.IntContains(j) == 1)
                    {
                        for (int k = 0; k < triadCombo.Count; k++)
                        {
                            bool i_in_node = false;
                            bool j_in_node = false;
                            // check if both i and j are in the triadCombo
                            foreach (int node in triadCombo[k])
                            {
                                if (i == node)
                                    i_in_node = true;
                                if (j == node)
                                    j_in_node = true;
                            }

                            if (i_in_node == true && j_in_node == true)
                            {
                                denominator++; // increase denom for every time i and j are in triad
                                // check for transitivity
                                int I = triadCombo[k][0];
                                int J = triadCombo[k][1];
                                int K = triadCombo[k][2];

                                if ((mTable["Data"][I, J] > triad.Cutoff) && (mTable["Data"][J, K] > triad.Cutoff) &&
                                    (mTable["Data"][I, K] > triad.Cutoff))
                                {
                                    sum++;
                                }
                                int[] temp = { -1, -1, -1 };
                                triadCombo[k] = temp; // to mark that the triad has been checked
                            }
                        }
                    }
                }
            }
            if (denominator == 0)
                return 0.0;
            return sum / denominator;
        }





        public void LoadCliqueCharacteristics(List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix,
            int year, double cutoff, double binaryCutoff)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");


            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance, binaryCutoff);
            //Read in Attribute matrix if there is one.
            string s = "";
            
            List<Matrix>[] multipleDyadicMatrices = new List<Matrix>[dvc.Count];
            int totalAttrCount = 0;
            for (int i = 0; i < dvc.Count; i++)
            {
                multipleDyadicMatrices[i] = MatrixReader.ReadMatrixFromMultipleDyadicFile(dvc[i].First, year);
                totalAttrCount += multipleDyadicMatrices[i].Count;
            }

            mTable["Characteristics"] = new Matrix(_cliques.Count, 4 + svc.Count + totalAttrCount + attrMatrix.Count);

            // Column labels
            mTable["Characteristics"].ColLabels[0] = "Year";
            mTable["Characteristics"].ColLabels[1] = "Clique Members";
            // newly added
            mTable["Characteristics"].ColLabels[2] = "NGT";
            mTable["Characteristics"].ColLabels[3] = "GMT";
            int I = 4;
            
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in svc)
            {
                string t = p.First;
                mTable["Characteristics"].ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            
            foreach (List<Matrix> mList in multipleDyadicMatrices)
            {
                foreach (Matrix m in mList)
                {
                    mTable["Characteristics"].ColLabels[I++] = m.Name;
                }
            }
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in attrMatrix)
            {
                string t = p.First;
                mTable["Characteristics"].ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }

            for (int i = 0; i < mTable["Characteristics"].Rows; ++i)
                mTable["Characteristics"].RowLabels[i] = "Clique " + (i + 1).ToString();

            s = null;

            // 0. Year
            for (int i = 0; i < mTable["Characteristics"].Rows; ++i)
                mTable["Characteristics"][i, 0] = year;

            // 1. Clique size
            for (int i = 0; i < mTable["Characteristics"].Rows; ++i)
            {
                mTable["Characteristics"][i, 1] = _cliques[i].Size;
            }

            // 2. NGT
            for (int i = 0; i < _cliques.Count; ++i)
            {
                mTable["Characteristics"][i, 2] = computeCliqueNGT(triad, _cliques[i]);
            }

            // 3. GMT
            for (int i = 0; i < _cliques.Count; ++i)
                mTable["Characteristics"][i, 3] = computeCliqueGMT(triad, _cliques[i]);



            // Vector characteristics 
            for (int i = 0; i < svc.Count; ++i)
            {
                BufferedFileTable.GetFile(svc[i].First).JumpToNetworkId(year);

                // Now read in the lines and generate av
                Matrix av = new Matrix(mTable["Data"].Rows, 1);

                //Skip first line of vector
                //s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                    /*
                    string[] parts = s.Split(',');
                    av[j, 0] = double.Parse(parts[2]);
                    */
                    if (s != null)
                    {
                        string[] parts = s.Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }
                }

                for (int j = 0; j < _cliques.Count; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                    {
                        if (_cliques[j].IntContains(k) != 0)
                        {
                            values.Add(av[k, 0]);
                        }
                        else
                        {
                            values.Add(0.0);
                        }
                    }

                    switch (svc[i].Second)
                    {
                        case CharacteristicType.Sum:
                            mTable["Characteristics"][j, 4 + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            mTable["Characteristics"][j, 4 + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            mTable["Characteristics"][j, 4 + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            mTable["Characteristics"][j, 4 + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            //mTable["Characteristics"][j, 2 + i] = Math.Sqrt(Algorithms.Variance(values));
                            mTable["Characteristics"][j, 4 + i] = Algorithms.Stdev(values);
                            break;
                    }

                }
            }

            int currentCount = 0;
            // Dyadic vector characteristics
            for (int i = 0; i < dvc.Count; ++i)
            {
                //Matrix NC = MatrixReader.ReadMatrixFromFile(dvc[i].First, year);
                for (int var = 0; var < multipleDyadicMatrices[i].Count; var++)
                {
                    // For each clique
                    for (int j = 0; j < _cliques.Count; ++j)
                    {
                        // Generate Q and CA
                        Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                            Q[k, 0] = _cliques[j].IntContains(k);

                        Matrix CA = Q * (Matrix)Q.GetTranspose();
                        for (int k = 0; k < CA.Rows; ++k)
                            CA[k, k] = 0;

                        // Now we can do DVCj
                        List<double> values = new List<double>();
                        for (int l = 0; l < CA.Rows; ++l)
                        {
                            for (int m = 0; m < CA.Rows; ++m)
                            {
                                //values.Add(CA[l, m] * NC[l, m]);
                                values.Add(CA[l, m] * multipleDyadicMatrices[i][var][l, m]);
                            }
                        }
                        switch (dvc[i].Second)
                        {
                            case CharacteristicType.Sum:
                                mTable["Characteristics"][j, 4 + svc.Count + currentCount] = Algorithms.Accumulate(values, 0.0);
                                break;
                            case CharacteristicType.Mean:
                                mTable["Characteristics"][j, 4 + svc.Count + currentCount] = Algorithms.Mean(values);
                                break;
                            case CharacteristicType.Max:
                                mTable["Characteristics"][j, 4 + svc.Count + currentCount] = Algorithms.MaxValue(values);
                                break;
                            case CharacteristicType.Min:
                                mTable["Characteristics"][j, 4 + svc.Count + currentCount] = Algorithms.MinValue(values);
                                break;
                            case CharacteristicType.StdDev:
                                mTable["Characteristics"][j, 4 + svc.Count + currentCount] = Math.Sqrt(Algorithms.Variance(values));
                                break;
                        }
                    }
                    currentCount++;
                }
            }

            for (int i = 0; i < attrMatrix.Count; i++)
            {
                Matrix attrMatr = MatrixReader.ReadMatrixFromFile(attrMatrix[0].First, year);

                // for each community
                for (int j = 0; j < _cliques.Count; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (_cliques[j].IntContains(k) != 0)
                        {
                            for (int l = 0; l < attrMatr.Rows; l++)
                            {
                                if (_cliques[j].IntContains(l) != 0 && k != l)
                                {
                                    values.Add(attrMatr[k, l]);
                                }
                            }
                        }
                    }

                    switch (attrMatrix[i].Second)
                    {
                        case CharacteristicType.Sum:
                            mTable["Characteristics"][j, 4 + svc.Count + dvc.Count + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            mTable["Characteristics"][j, 4 + svc.Count + dvc.Count + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            mTable["Characteristics"][j, 4 + svc.Count + dvc.Count + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            mTable["Characteristics"][j, 4 + svc.Count + dvc.Count + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            mTable["Characteristics"][j, 4 + svc.Count + dvc.Count + i] = Math.Sqrt(Algorithms.Variance(values));
                            break;
                    }
                }
            }
        }     

        protected double GetSubsetSize(int[] subset, Vector v)
        {
            double s = 0.0;
            for (int i = 0; i < subset.Length; ++i)
                s += v[subset[i]];
            return s;
        }

        protected double GetSubsetCohesion(int[] subset)
        {
            double cohesion = 0.0;

            if (subset.Length == 1)
                return 1.0;

            for (int i = 0; i < subset.Length; ++i)
            {
                for (int j = i + 1; j < subset.Length; ++j)
                {
                    cohesion += mTable["SESE"][subset[i], subset[j]];
                }
            }

            return (2.0 * cohesion) / (subset.Length * (subset.Length - 1));
        }

        public void LoadViableCoalitions(double cutoff, int year, string svcFile)
        {
            cutoff = GetRealViableCoalitionCutoff(cutoff, year);

            List<int[]> allSets = new List<int[]>();

            BufferedFileTable.GetFile(svcFile).JumpToNetworkId(year);

            Vector v = new Vector(mTable["Data"].Rows);

            // Now read in the lines and generate av
            for (int j = 0; j < mTable["Data"].Rows; ++j)
            {
                string s = BufferedFileTable.GetFile(svcFile).ReadLine();
                string[] parts = s.Split(',');
                v[j] = double.Parse(parts[parts.Length - 1]);
            }

            for (int i = 0; i < _cliques.Count; ++i)
            {
                //if (NPOLA_Sums[i] < cutoff)
                //    continue;

                // Load all members of this clique
                // Start with groups of size 1, 2, ... until you find one that satisfies the requirements
                // i.e. size >= cutoff and weighted size >= cutoff

                List<int> nums = new List<int>();
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                    if (_cliques[i].Contains(j))
                        nums.Add(j);

                List<int[]> sets = new List<int[]>();

                ElementSet<int> es = new ElementSet<int>(nums.ToArray());
                for (int j = 1; j <= nums.Count; ++j)
                {
                    Combinations<int> c = new Combinations<int>(es, j);
                    foreach (int[] comb in c)
                    {
                        if (Algorithms.IsSupersetOf(comb, allSets))
                            continue;

                        // test size threshhold
                        double size = GetSubsetSize(comb, v);
                        double cohesion = GetSubsetCohesion(comb);
                        if (size < cutoff || (size * cohesion) < cutoff)
                            continue;

                        sets.Add(comb);
                        allSets.Add(comb);
                    }
                }
            }

            // got all sets, now make the matrix
            mTable["CoalitionStructure"] = new Matrix(Math.Max(1, allSets.Count), 6);
            mTable["CoalitionStructure"].ColLabels[0] = "Matrix identifier";
            mTable["CoalitionStructure"].ColLabels[1] = "Coalition No.";
            mTable["CoalitionStructure"].ColLabels[2] = "No. of Units";
            mTable["CoalitionStructure"].ColLabels[3] = "Size";
            mTable["CoalitionStructure"].ColLabels[4] = "Cohesion";
            mTable["CoalitionStructure"].ColLabels[5] = "Weighted Size";

            mTable["ViableCoalitions"] = new Matrix(mTable["Data"].Rows + 3, Math.Max(1, allSets.Count));
            mTable["ViableCoalitions"].RowLabels.CopyFrom(mTable["Data"].RowLabels);
            mTable["ViableCoalitions"].RowLabels[mTable["ViableCoalitions"].RowLabels.Length - 3] = "Size";
            mTable["ViableCoalitions"].RowLabels[mTable["ViableCoalitions"].RowLabels.Length - 2] = "Cohesion";
            mTable["ViableCoalitions"].RowLabels[mTable["ViableCoalitions"].RowLabels.Length - 1] = "Weighted Size";

            for (int i = 0; i < allSets.Count; ++i)
            {
                mTable["ViableCoalitions"].ColLabels[i] = (i + 1).ToString();
                for (int j = 0; j < allSets[i].Length; ++j)
                {
                    mTable["ViableCoalitions"][allSets[i][j], i] = 1;
                }
                mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 3, i] = GetSubsetSize(allSets[i], v);
                mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 2, i] = GetSubsetCohesion(allSets[i]);
                mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 1, i] = mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 3, i] * mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 2, i];

                mTable["CoalitionStructure"][i, 0] = year;
                mTable["CoalitionStructure"][i, 1] = i + 1;
                mTable["CoalitionStructure"][i, 2] = allSets[i].Length;
                mTable["CoalitionStructure"][i, 3] = mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 3, i];
                mTable["CoalitionStructure"][i, 4] = mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 2, i];
                mTable["CoalitionStructure"][i, 5] = mTable["ViableCoalitions"][mTable["ViableCoalitions"].Rows - 1, i];
            }
            if (allSets.Count == 0)
            {
                //mTable["CoalitionStructure"][1, 0] = year;
            }

            mTable["ViableNPI"] = new Matrix(1, 1);
            mTable["ViableNPI"].ColLabels[0] = "NPI";
            mTable["ViableNPI"].RowLabels[0] = year.ToString();
            mTable["ViableNPI"][0, 0] = ViableNPI;
        }
             

        public void LoadBlockCharacteristics(List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix,  
            int year, bool cluster)
        { 
            if (Blocks == null)
                throw new Exception("No blocks have been found!");
            //if (_blocks == null)
            //{
                FindBlocks(year, Blocks, _minCliqueSize);
            //}

            // count the number of attr vectors from each dyadic file
            List<Matrix>[] multipleDyadicMatrices = new List<Matrix>[dvc.Count];

            //dyadicLabels = new string[dvc.Count][];
            int totalAttrCount = 0;
            for (int i = 0; i < dvc.Count; i++)
            {
                multipleDyadicMatrices[i] = MatrixReader.ReadMatrixFromMultipleDyadicFile(dvc[i].First, year);
                totalAttrCount += multipleDyadicMatrices[i].Count;
                //dyadicLabels[i] = new string[multipleDyadicMatrices[i].Count];
            }


            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance);
            Matrix BC = mTable.AddMatrix("BlockCharacteristics", _blocks.Count, svc.Count + totalAttrCount + attrMatrix.Count + 4);


            //Read in Attribute matrix if there is one.
            //string s = null;
            string s = "";

            // Row labels
            BC.ColLabels[0] = "Year";
            BC.ColLabels[1] = "Block Size";
            // newly added
            BC.ColLabels[2] = "NGT";
            BC.ColLabels[3] = "GMT";
            int I = 4;
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in svc)
            {
                string t = p.First;
                BC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            
            foreach (List<Matrix> mList in multipleDyadicMatrices)
            {
                foreach (Matrix m in mList)
                {
                    BC.ColLabels[I++] = m.Name;
                }
            }
            
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in attrMatrix)
            {
                string t = p.First;
                BC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }

            for (int i = 0; i < _blocks.Count; ++i) 
                BC.RowLabels[i] = "Block " + (i + 1).ToString();
 
            //s = null;

            // 0. Year
            for (int i = 0; i < BC.Rows; ++i)
                BC[i, 0] = year;

            // 1. Clique size
            for (int i = 0; i < BC.Rows; ++i)
            {
                BC[i, 1] = Blocks[i].Count;
            }

            // 2. NGT
            for (int i = 0; i < _blocks.Count; ++i)
            {
                BC[i, 2] = computeBlockNGT(triad, _blocks[i]);
            }

            // 3. GMT
            for (int i = 0; i < _blocks.Count; ++i)
                BC[i, 3] = computeBlockGMT(triad, _blocks[i]);

            // Vector characteristics 
            for (int i = 0; i < svc.Count; ++i)
            {   
                BufferedFileTable.GetFile(svc[i].First).JumpToNetworkId(year);

                // Now read in the lines and generate av
                Matrix av = new Matrix(mTable["Data"].Rows, 1);

                //skip first line
                //s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    
                    s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                    /*
                    string[] parts = s.Split(',');
                    av[j, 0] = double.Parse(parts[2]);
                    */
                    if (s != null)
                    {
                        string[] parts = s.Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }
                }

                for (int j = 0; j < _blocks.Count; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                    {
                        if (_blocks[j].IntContains(k) != 0)
                        {
                            values.Add(av[k, 0]);
                        }
                        else
                        {
                            values.Add(0.0);
                        }
                    }
                    
                    switch (svc[i].Second)
                    {
                        case CharacteristicType.Sum:
                            BC[j, 4 + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            BC[j, 4 + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            BC[j, 4 + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            BC[j, 4 + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            BC[j, 4 + i] = Algorithms.Stdev(values);
                            break;
                    }
                }
            }


            int currentCount = 0;
            // Dyadic vector characteristics
            for (int i = 0; i < dvc.Count; ++i)
            {
                //Matrix NC = MatrixReader.ReadMatrixFromFile(dvc[i].First, year);
                for (int var = 0; var < multipleDyadicMatrices[i].Count; var++)
                {
                    //dyadicLabels[i][var] = multipleDyadicMatrices[i][var].Name;
                    // For each clique
                    for (int j = 0; j < Blocks.Count; ++j)
                    {
                        // Generate Q and CA
                        Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                            Q[k, 0] = Blocks[j].Contains(k) ? 1 : 0;

                        Matrix CA = Q * (Matrix)Q.GetTranspose();
                        for (int k = 0; k < CA.Rows; ++k)
                            CA[k, k] = 0;

                        // Now we can do DVCj
                        List<double> values = new List<double>();
                        for (int l = 0; l < CA.Rows; ++l)
                        {
                            for (int m = 0; m < CA.Rows; ++m)
                            {
                                //values.Add(CA[l, m] * NC[l, m]);
                                values.Add(CA[l, m] * multipleDyadicMatrices[i][var][l, m]);
                            }
                        }
                        
                        switch (dvc[i].Second)
                        {
                            case CharacteristicType.Sum:
                                BC[j, 4 + svc.Count + currentCount] = Algorithms.Accumulate(values, 0.0);
                                break;
                            case CharacteristicType.Mean:
                                BC[j, 4 + svc.Count + currentCount] = Algorithms.Mean(values);
                                break;
                            case CharacteristicType.Max:
                                BC[j, 4 + svc.Count + currentCount] = Algorithms.MaxValue(values);
                                break;
                            case CharacteristicType.Min:
                                BC[j, 4 + svc.Count + currentCount] = Algorithms.MinValue(values);
                                break;
                            case CharacteristicType.StdDev:
                                BC[j, 4 + svc.Count + currentCount] = Math.Sqrt(Algorithms.Variance(values));
                                break;
                        }
                    }
                    currentCount++;
                }
            }

            for (int i = 0; i < attrMatrix.Count; i++)
            {
                Matrix attrMatr = MatrixReader.ReadMatrixFromFile(attrMatrix[0].First, year);

                // for each community
                for (int j = 0; j < _blocks.Count; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (_blocks[j].IntContains(k) != 0)
                        {
                            for (int l = 0; l < attrMatr.Rows; l++)
                            {
                                if (_blocks[j].IntContains(l) != 0 && k != l)
                                {
                                    values.Add(attrMatr[k, l]);
                                }
                            }
                        }
                    }

                    switch (attrMatrix[i].Second)
                    {
                        case CharacteristicType.Sum:
                            BC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            BC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            BC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            BC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            BC[j, 4 + svc.Count + dvc.Count + i] = Math.Sqrt(Algorithms.Variance(values));
                            break;
                    }
                }
            }
        }

        public Matrix LoadComCharacteristics(List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix,
            int year, int numCom, List<int[]> communities, List<int> comSize, double binaryCutoff, ref string[][] dyadicLabels)
        {
            //string ms = "ComCharacteristics";
            string ms = "Community";

            // count the number of attr vectors from each dyadic file
            List<Matrix>[] multipleDyadicMatrices = new List<Matrix>[dvc.Count];
            
            dyadicLabels = new string[dvc.Count][];
            int totalAttrCount = 0;
            for (int i = 0; i < dvc.Count; i++)
            {
                multipleDyadicMatrices[i] = MatrixReader.ReadMatrixFromMultipleDyadicFile(dvc[i].First, year);
                totalAttrCount += multipleDyadicMatrices[i].Count;
                dyadicLabels[i] = new string[multipleDyadicMatrices[i].Count];
            }
            
            Matrix ComC = mTable.AddMatrix(ms, numCom, svc.Count + totalAttrCount + attrMatrix.Count + 4);

            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance, binaryCutoff);
            //Read in Attribute matrix if there is one.
            string s = "";      
        
            // Row labels
            ComC.ColLabels[0] = "Year";
            ComC.ColLabels[1] = "Comm Size";
            // newly added
            ComC.ColLabels[2] = "NGT";
            ComC.ColLabels[3] = "GMT";
            int I = 4;
        
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in svc)
            {
                string t = p.First;
                ComC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in dvc)
            {
                string t = p.First;
                ComC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in attrMatrix)
            {
                string t = p.First;
                ComC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            
            for (int i = 0; i < numCom; ++i)
                ComC.RowLabels[i] = "Community " + (i + 1).ToString();

            // 0. Year
            for (int i = 0; i < ComC.Rows; ++i)
                ComC[i, 0] = year;

            // 1. Clique size
            for (int j = 0; j < numCom; ++j)
                 ComC[j, 1] = comSize[j];

            // 2. NGT
            for (int i = 0; i < numCom; ++i)
            {
                ComC[i, 2] = computeCommNGT(triad, communities[i], comSize[i]);
            }
            
            // 3. GMT
            for (int i = 0; i < numCom; ++i)
                ComC[i, 3] = computeCommGMT(triad, communities[i]);
      
            
            // Vector characteristics 
            for (int i = 0; i < svc.Count; ++i)
            {
                BufferedFileTable.GetFile(svc[i].First).JumpToNetworkId(year);

                // Now read in the lines and generate av
                Matrix av = new Matrix(mTable["Data"].Rows, 1);
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                    if (s != null)
                    {
                        string[] parts = s.Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }
                }
                

                for (int j = 0; j < numCom; ++j)
                {
                    List<double> values = new List<double>();

                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                    {
                        if (communities[j][k] != 0)
                        {
                            values.Add(av[k, 0]);
                        }
                        else
                        {
                            values.Add(0.0);
                        }
                    }

                    switch (svc[i].Second)
                    {
                        case CharacteristicType.Sum:
                            ComC[j, 4 + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            ComC[j, 4 + i] = Algorithms.Mean(values); // values2?
                            break;
                        case CharacteristicType.Max:
                            ComC[j, 4 + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            ComC[j, 4 + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            ComC[j, 4 + i] = Algorithms.Stdev(values);
                            break;
                    }
                }
            }
            int currentCount = 0;
            // Dyadic vector characteristics
            for (int i = 0; i < dvc.Count; ++i)
            {
                for (int var = 0; var < multipleDyadicMatrices[i].Count; var++)
                {
                    dyadicLabels[i][var] = multipleDyadicMatrices[i][var].Name;
                    // For each clique
                    for (int j = 0; j < numCom; ++j)
                    {

                        // Generate Q and CA
                        Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                        {
                            Q[k, 0] = communities[j][k] != 0 ? 1 : 0;
                        }

                        Matrix CA = Q * (Matrix)Q.GetTranspose();
                        for (int k = 0; k < CA.Rows; ++k)
                            CA[k, k] = 0;

                        // Now we can do DVCj
                        List<double> values = new List<double>();
                        for (int l = 0; l < CA.Rows; ++l)
                            for (int m = 0; m < CA.Rows; ++m)
                                values.Add(CA[l, m] * multipleDyadicMatrices[i][var][l, m]);

                        switch (dvc[i].Second)
                        {
                            case CharacteristicType.Sum:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.Accumulate(values, 0.0);
                                break;
                            case CharacteristicType.Mean:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.Mean(values);
                                break;
                            case CharacteristicType.Max:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.MaxValue(values);
                                break;
                            case CharacteristicType.Min:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.MinValue(values);
                                break;
                            case CharacteristicType.StdDev:
                                ComC[j, 4 + svc.Count + currentCount] = Math.Sqrt(Algorithms.Variance(values));
                                break;
                        }
                    }
                    currentCount++;
                }
            }

            for (int i = 0; i < attrMatrix.Count; i++)
            {
                Matrix attrMatr = MatrixReader.ReadMatrixFromFile(attrMatrix[i].First, year);

                // for each community
                for (int j = 0; j < numCom; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (communities[j][k] != 0)
                        {
                            for (int l = 0; l < attrMatr.Rows; l++)
                            {
                                if (communities[j][l] != 0 && k != l)
                                {
                                    values.Add(attrMatr[k, l]);
                                }
                            }
                        }
                    }

                    switch (attrMatrix[i].Second)
                    {
                        case CharacteristicType.Sum:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Math.Sqrt(Algorithms.Variance(values));
                            break;
                    }
                }
            }   
            return ComC;
        }

        // Overlap Community Characteristics
        public Matrix LoadOverlapCommCharacteristics(List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix,
            int year, double binaryCutoff)
        {
            string ms = "OverlapCommCharacteristics";

            List<Matrix>[] multipleDyadicMatrices = new List<Matrix>[dvc.Count];

            int totalAttrCount = 0;
            for (int i = 0; i < dvc.Count; i++)
            {
                multipleDyadicMatrices[i] = MatrixReader.ReadMatrixFromMultipleDyadicFile(dvc[i].First, year);
                totalAttrCount += multipleDyadicMatrices[i].Count;
            }

            Matrix ComC = mTable.AddMatrix(ms, overlapComm.Count, svc.Count + totalAttrCount + attrMatrix.Count + 4);

            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance, binaryCutoff);
            //Read in Attribute matrix if there is one.
            string s = "";

            // Row labels
            ComC.ColLabels[0] = "Year";
            ComC.ColLabels[1] = "OverlapComm Size";
            // newly added
            ComC.ColLabels[2] = "NGT";
            ComC.ColLabels[3] = "GMT";
            int I = 4;

            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in svc)
            {
                string t = p.First;
                ComC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }
            
            foreach (List<Matrix> mList in multipleDyadicMatrices)
            {
                foreach (Matrix m in mList)
                {
                    ComC.ColLabels[I++] = m.Name;
                }
            }
            foreach (UnordererdPair<string, NetworkIO.CharacteristicType> p in attrMatrix)
            {
                string t = p.First;
                ComC.ColLabels[I++] = "(" + p.Second + ")       " + t.Split('\\')[t.Split('\\').Length - 1];
            }

            for (int i = 0; i < overlapComm.Count; ++i)
                ComC.RowLabels[i] = (i + 1).ToString();

            // 0. Year
            for (int i = 0; i < ComC.Rows; ++i)
                ComC[i, 0] = year;

            // 1. Clique size
            for (int j = 0; j < overlapComm.Count; ++j)
                ComC[j, 1] = overlapComm[j].Size;

            // 2. NGT
            for (int i = 0; i < overlapComm.Count; ++i)
            {
                ComC[i, 2] = computeOverlapCommNGT(triad, overlapComm[i]);
            }

            // 3. GMT
            for (int i = 0; i < overlapComm.Count; ++i)
                ComC[i, 3] = computeOverlapCommGMT(triad, overlapComm[i]);

            // Vector characteristics 
            for (int i = 0; i < svc.Count; ++i)
            {
                BufferedFileTable.GetFile(svc[i].First).JumpToNetworkId(year);

                // Now read in the lines and generate av
                Matrix av = new Matrix(mTable["Data"].Rows, 1);
                for (int j = 0; j < mTable["Data"].Rows; ++j)
                {
                    s = BufferedFileTable.GetFile(svc[i].First).ReadLine();
                    if (s != null)
                    {
                        string[] parts = s.Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }
                }
             
                for (int j = 0; j < overlapComm.Count; ++j)
                {
                    List<double> values = new List<double>();

                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                    {
                        if (overlapComm[j].IntContains(k) != 0)
                        {
                            values.Add(av[k, 0]);
                        }
                        else
                        {
                            values.Add(0.0);
                        }
                    }

                    switch (svc[i].Second)
                    {
                        case CharacteristicType.Sum:
                            ComC[j, 4 + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            ComC[j, 4 + i] = Algorithms.Mean(values); // values2?
                            break;
                        case CharacteristicType.Max:
                            ComC[j, 4 + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            ComC[j, 4 + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            ComC[j, 4 + i] = Algorithms.Stdev(values);
                            break;
                    }
                }
            }

            int currentCount = 0;
            // Dyadic vector characteristics
            for (int i = 0; i < dvc.Count; ++i)
            {
                for (int var = 0; var < multipleDyadicMatrices[i].Count; var++)
                {
                    // For each clique
                    for (int j = 0; j < overlapComm.Count; ++j)
                    {

                        // Generate Q and CA
                        Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                        {
                            Q[k, 0] = overlapComm[j].Contains(k) ? 1 : 0;
                        }       
                        Matrix CA = Q * (Matrix)Q.GetTranspose();
                        for (int k = 0; k < CA.Rows; ++k)
                            CA[k, k] = 0;

                        // Now we can do DVCj
                        List<double> values = new List<double>();
                        for (int l = 0; l < CA.Rows; ++l)
                        {
                            for (int m = 0; m < CA.Rows; ++m)
                            {
                                values.Add(CA[l, m] * multipleDyadicMatrices[i][var][l, m]);
                            }
                        }
                        switch (dvc[i].Second)
                        {
                            case CharacteristicType.Sum:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.Accumulate(values, 0.0);
                                break;
                            case CharacteristicType.Mean:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.Mean(values);
                                break;
                            case CharacteristicType.Max:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.MaxValue(values);
                                break;
                            case CharacteristicType.Min:
                                ComC[j, 4 + svc.Count + currentCount] = Algorithms.MinValue(values);
                                break;
                            case CharacteristicType.StdDev:
                                ComC[j, 4 + svc.Count + currentCount] = Math.Sqrt(Algorithms.Variance(values));
                                break;
                        }
                    }
                    currentCount++;
                }
            }

            for (int i = 0; i < attrMatrix.Count; i++)
            {
                Matrix attrMatr = MatrixReader.ReadMatrixFromFile(attrMatrix[i].First, year);

                // for each community
                for (int j = 0; j < overlapComm.Count; ++j)
                {
                    List<double> values = new List<double>();
                    for (int k = 0; k < mTable["Data"].Rows; k++)
                    {
                        if (overlapComm[j].IntContains(k) != 0)
                        {
                            for (int l = 0; l < attrMatr.Rows; l++)
                            {
                                if (overlapComm[j].IntContains(l) != 0 && k != l)
                                {
                                    values.Add(attrMatr[k, l]);
                                }
                            }
                        }
                    }

                    switch (attrMatrix[i].Second)
                    {
                        case CharacteristicType.Sum:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Accumulate(values, 0.0);
                            break;
                        case CharacteristicType.Mean:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.Mean(values);
                            break;
                        case CharacteristicType.Max:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MaxValue(values);
                            break;
                        case CharacteristicType.Min:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Algorithms.MinValue(values);
                            break;
                        case CharacteristicType.StdDev:
                            ComC[j, 4 + svc.Count + dvc.Count + i] = Math.Sqrt(Algorithms.Variance(values));
                            break;
                    }
                }
            }

            return ComC;
        }



        public double NPOLStar(bool useStructEquiv, string fileName, double cutoff, int year, double maxik, int cliqueOrder, bool kCliqueDiag, string selMeth)
        {
            if (selMeth == "Cliq")
                return NPOLStar(useStructEquiv, fileName, cutoff, year, maxik, true, cliqueOrder, kCliqueDiag);
            else if (selMeth == "Bloc" || selMeth == "Clus")
                return bNPOLStar(useStructEquiv, fileName, cutoff, year, maxik, true, cliqueOrder, kCliqueDiag);
            else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                return comNPOLStar(useStructEquiv, fileName, cutoff, year, maxik, true, cliqueOrder, kCliqueDiag);
               
            else 
                return 0.0;
        }

        public double NPOLStar(bool useStructEquiv, string fileName, double cutoff, int year, double maxik, bool calculate, int cliqueOrder, bool kCliqueDiag)
        {
            //Matrix cohesiveMatrix = null;
            if (_cliques == null)
                FindCliques(cutoff, true, maxik, year, _minCliqueSize, false, cliqueOrder, kCliqueDiag);

            if (!useStructEquiv) // Read in from a file
            {

                BufferedFileTable.AddFile(fileName);

                Matrix NC = MatrixReader.ReadMatrixFromFile(fileName, year);

                // For each clique
                for (int j = 0; j < _cliques.Count; ++j)
                {
                    // Generate Q and CA
                    Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                        Q[k, 0] = _cliques[j].IntContains(k);

                    Matrix CA = Q * (Matrix)Q.GetTranspose();
                    for (int k = 0; k < CA.Rows; ++k)
                        CA[k, k] = 0;

                    // Now we multiply by the external matrix
                    _cliques[j].Cohesion = 0.0;
                    for (int l = 0; l < CA.Rows; ++l)
                        for (int m = 0; m < CA.Rows; ++m)
                            _cliques[j].Cohesion += CA[l, m] * NC[l, m];

                    // Average it
                    double q = Q.GetColSum(0);
                    if (q == 1.0)
                        _cliques[j].Cohesion = 1.0;
                    else
                        _cliques[j].Cohesion /= q * q;
                }

            }
            // new code added for GPOL cohesion

            // Now we have the cohesion, quit if we don't actually need the NPOL*
            if (!calculate)
                return -1.0;

            //calculate GPOL with cohesion
            double total_sum = 0.0;

            for (int i = 0; i < _cliques.Count; i++)
            {
                //total_sum += _cliques[i].Size * (mTable["Data"].Rows - _cliques[i].Size) * cohesiveMatrix[i, i];
                total_sum += _cliques[i].Size * (mTable["Data"].Rows - _cliques[i].Size) * _cliques[i].Cohesion;
            }

            double first_term = 4.0 / (double)(_cliques.Count * mTable["Data"].Rows * mTable["Data"].Rows);

            return first_term * total_sum;
        }

        public double bNPOLStar(bool useStructEquiv, string fileName, double cutoff, int year, double maxik, bool calculate, int cliqueOrder, bool kCliqueDiag)
        {
            //Matrix cohesiveMatrix = null;
            if (_blocks == null)
                FindBlocks(cutoff, Blocks, _minCliqueSize);

            if (!useStructEquiv) // Read in from a file
            {
                BufferedFileTable.AddFile(fileName);

                Matrix NC = MatrixReader.ReadMatrixFromFile(fileName, year);

                // For each block
                for (int j = 0; j < _blocks.Count; ++j)
                {
                    // Generate Q and CA
                    Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                        Q[k, 0] = _blocks[j].IntContains(k);

                    Matrix CA = Q * (Matrix)Q.GetTranspose();
                    for (int k = 0; k < CA.Rows; ++k)
                        CA[k, k] = 0;

                    // Now we multiply by the external matrix
                    _blocks[j].Cohesion = 0.0;
                    for (int l = 0; l < CA.Rows; ++l)
                        for (int m = 0; m < CA.Rows; ++m)
                            _blocks[j].Cohesion += CA[l, m] * NC[l, m];

                    // Average it
                    double q = Q.GetColSum(0);
                    if (q == 1.0)
                        _blocks[j].Cohesion = 1.0;
                    else
                        _blocks[j].Cohesion /= q * q;
                }

            }
            
            // Now we have the cohesion, quit if we don't actually need the NPOL*
            if (!calculate)
                return -1.0;
            
            //calculate GPOL with cohesion
            double total_sum = 0.0;

            for (int i = 0; i < _blocks.Count; i++)
            {
                //total_sum += _blocks[i].Size * (mTable["Data"].Rows - _blocks[i].Size) * cohesiveMatrix[i, i];
                //total_sum += _blocks[i].Size * (mTable["Data"].Rows - _blocks[i].Size) * _blocks[i].Cohesion;
                _blocks[i].ComputeCohesion(mTable["SESE"]);
                total_sum += _blocks[i].Size * _blocks[i].Cohesion * (mTable["Data"].Rows - _blocks[i].Size);
            }

            double first_term = 4.0 / (double)(_blocks.Count * mTable["Data"].Rows * mTable["Data"].Rows);

            return first_term * total_sum;
            //return cohesiveMatrix[0, 0];
        }

        public double comNPOLStar(bool useStructEquiv, string fileName, double cutoff, int year, double maxik, bool calculate, int cliqueOrder, bool kCliqueDiag)
        {
            //Matrix cohesiveMatrix = null;
            if (_communities == null)
                FindComm(cutoff, communities, _minCliqueSize);

            if (!useStructEquiv) // Read in from a file
            {
                BufferedFileTable.AddFile(fileName);

                Matrix NC = MatrixReader.ReadMatrixFromFile(fileName, year);

                // For each clique
                for (int j = 0; j < _communities.Count; ++j)
                {
                    // Generate Q and CA
                    Matrix Q = new Matrix(mTable["Data"].Rows, 1);
                    for (int k = 0; k < mTable["Data"].Rows; ++k)
                        Q[k, 0] = _communities[j].IntContains(k);

                    Matrix CA = Q * (Matrix)Q.GetTranspose();
                    for (int k = 0; k < CA.Rows; ++k)
                        CA[k, k] = 0;

                    // Now we multiply by the external matrix
                    _communities[j].Cohesion = 0.0;
                    for (int l = 0; l < CA.Rows; ++l)
                        for (int m = 0; m < CA.Rows; ++m)
                            _communities[j].Cohesion += CA[l, m] * NC[l, m];

                    // Average it
                    double q = Q.GetColSum(0);
                    if (q == 1.0)
                        _communities[j].Cohesion = 1.0;
                    else
                        _communities[j].Cohesion /= q * q;
                }
            }

            // Now we have the cohesion, quit if we don't actually need the NPOL*
            if (!calculate)
                return -1.0;
            
            double total_sum = 0.0;
            Matrix cohesiveMatrix = computeCommCohesiveMatrix();
            for (int i = 0; i < comNum; i++)
            {
                int Size = 0;
                for (int j = 0; j < comSize; j++)
                {
                    Size += communities[i][j];
                }
                total_sum += Size * (mTable["Data"].Rows - Size) *cohesiveMatrix[i, i];
                //total_sum += Size * (mTable["Data"].Rows - Size) * NewComputeCohesion(communities[i]);
            }

            double first_term = 4.0 / (double)(comNum * mTable["Data"].Rows * mTable["Data"].Rows);
            return first_term * total_sum;
        }
        

        public double NPOLA(int year, string svcFile, bool useCohesion, double cutoff, double maxik, int cliqueOrder, bool kCliqueDiag)
        {
            NPOLA_Sums = new List<double>();
            // First, do we need cohesion?
            if (useCohesion)
            {
                NPOLStar(true, null, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag); // Calculates Cohesion
            }
            
            // old code
            //Vector vv = MatrixReader.ReadVectorFromFile(svcFile, year);

            // new code
            BufferedFileTable.GetFile(svcFile).JumpToNetworkId(year); // may not need

            // Now read in the lines and generate av
            Vector av = new Vector(mTable["Data"].Rows);
            //string s = BufferedFileTable.GetFile(svcFile).ReadLine();
            for (int j = 0; j < mTable["Data"].Rows; ++j)
            {
                string s;
                s = BufferedFileTable.GetFile(svcFile).ReadLine();
                if (s != null)
                {
                    string[] parts = s.Split(',');
                    av[j] = double.Parse(parts[2]);
                }
            }
            BufferedFileTable.RemoveFile(svcFile);
            
            // Now multiply across CA and sum
            double n = 0.0;
            for (int j = 0; j < _cliques.Count; ++j)
            {
                double d = 0.0;
                NPOLA_Sums.Add(0.0);
                for (int k = 0; k < mTable["Data"].Rows; ++k)
                {
                    d += av[k] * _cliques[j].IntContains(k);
                }
                NPOLA_Sums[j] = d;
                Matrix cohesiveMatrix = null;
                if (useCohesion)
                {
                    cohesiveMatrix = computeCliqueCohesiveMatrix();
                    n += (d * (1 - d) * cohesiveMatrix[j, j]);
                }
                else
                {
                    n += (d * (1 - d));
                }
            }
            return n / ((double)_cliques.Count / 4.0);
        }

        public double bNPOLA(int year, string svcFile, bool useCohesion, double cutoff, double maxik, int cliqueOrder, bool kCliqueDiag)
        {
            NPOLA_Sums = new List<double>();

            // First, do we need cohesion?
            if (useCohesion)
            {
                bNPOLStar(true, null, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag); // Calculates Cohesion
            }

            // old code
            //Vector av = MatrixReader.ReadVectorFromFile(svcFile, year);

            // new code
            BufferedFileTable.GetFile(svcFile).JumpToNetworkId(year); // may not need

            // Now read in the lines and generate av
            Vector av = new Vector(mTable["Data"].Rows);
            //string s = BufferedFileTable.GetFile(svcFile).ReadLine();
            for (int j = 0; j < mTable["Data"].Rows; ++j)
            {
                string s;
                s = BufferedFileTable.GetFile(svcFile).ReadLine();
                if (s != null)
                {
                    string[] parts = s.Split(',');
                    av[j] = double.Parse(parts[2]);
                }
            }
            BufferedFileTable.RemoveFile(svcFile);

            // Now multiply across CA and sum
            double n = 0.0;
            for (int j = 0; j < _blocks.Count; ++j)
            {
                double d = 0.0;
                NPOLA_Sums.Add(0.0);
                for (int k = 0; k < mTable["Data"].Rows; ++k)
                {
                    d += av[k] * _blocks[j].IntContains(k);
                }
                NPOLA_Sums[j] = d;
                Matrix cohesiveMatrix = null;
                if (useCohesion)
                {
                    cohesiveMatrix = computeBlockCohesiveMatrix();
                    n += (d * (1 - d) * cohesiveMatrix[j, j]);
                }
                else
                {
                    n += (d * (1 - d));
                }
            }
            return n / ((double)_blocks.Count / 4.0);

        }

        public double comNPOLA(int year, string svcFile, bool useCohesion, double cutoff, double maxik, int cliqueOrder, bool kCliqueDiag)
        {
            NPOLA_Sums = new List<double>();

            // First, do we need cohesion?
            if (useCohesion)
            {
                comNPOLStar(true, null, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag); // Calculates Cohesion
            }
            
            // old code
            //Vector av = MatrixReader.ReadVectorFromFile(svcFile, year);

            // new code
            BufferedFileTable.GetFile(svcFile).JumpToNetworkId(year); // may not need

            // Now read in the lines and generate av
            Vector av = new Vector(mTable["Data"].Rows);
            //string s = BufferedFileTable.GetFile(svcFile).ReadLine();
            for (int j = 0; j < mTable["Data"].Rows; ++j)
            {
                string s;
                s = BufferedFileTable.GetFile(svcFile).ReadLine();
                if (s != null)
                {
                    string[] parts = s.Split(',');
                    av[j] = double.Parse(parts[2]);
                }
            }
            BufferedFileTable.RemoveFile(svcFile);

            // Now multiply across CA and sum
            double n = 0.0;
            for (int j = 0; j < comNum; ++j)
            {
                double d = 0.0;
                NPOLA_Sums.Add(0.0);
                for (int k = 0; k < comSize; ++k)
                {
                    //d += av[k] * _communities[j].IntContains(k);
                    d += av[k] * communities[j][k];
                }
                NPOLA_Sums[j] = d;
                Matrix cohesiveMatrix = null;
                if (useCohesion)
                {
                    cohesiveMatrix = computeCommCohesiveMatrix();
                    n += (d * (1 - d) * cohesiveMatrix[j, j]);
                }
                else
                {
                    n += (d * (1 - d));
                }
            }
            return n / ((double)comNum * .25);

        }

        /// <summary>
        /// Correctly loads any (dyadic/matrix) type of file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="fileType">Will be set to the type of file (dyadic/matrix)</param>
        /// <returns>Year that was loaded</returns>
        public int SmartLoad(string filename, out string fileType)
        {
            Reset();

            if (!BufferedFileTable.ContainsFile(filename))
                BufferedFileTable.AddFile(filename);


            if (BufferedFileTable.GetFile(filename).FileType == BufferedFileReader.Type.Matrix)
            {
                fileType = "Matrix";
            }
            else if (BufferedFileTable.GetFile(filename).FileType == BufferedFileReader.Type.Dyadic)
                fileType = "Dyadic";
            else
                throw new IOException("This file is not of matrix OR dyadic type!");

            mTable["Data"] = MatrixReader.ReadMatrixFromFile(filename);
            tempDataMatrix = new Matrix(mTable["Data"]);
            return mTable["Data"].NetworkId;
        }

        public int LoadFromDyadicFile(string fileName, int year)
        {
            // Reset various matrices which depend on the data matrix
            Reset();

            mTable["Data"] = MatrixReader.ReadMatrixFromFile(fileName, year);
            return mTable["Data"].NetworkId;
        }

        public int LoadFromMatrixFileIntoMatrix(string filename, int year, string m)
        {
            mTable[m] = MatrixReader.ReadMatrixFromFile(filename, year);
            return mTable[m].NetworkId;
        }

        public int LoadFromMatrixFile(string fileName, int year)
        {
            // Reset various matrices which depend on the data matrix
            Reset();

            // Simply load from the file into the data matrix
            mTable["Data"] = MatrixReader.ReadMatrixFromFile(fileName, year);
            return mTable["Data"].NetworkId;
        }

        public void Reset()
        {
            mTable["Dependency"] = mTable["Temp"] = mTable["RoleEquiv"] = mTable["SEC"] =
            mTable["SEE"] = mTable["SESE"] = mTable["Reachability"] = mTable["CognitiveReachability"] = 
            mTable["Centrality"] = mTable["Components"] = mTable["OverlapDiag"] = mTable["Overlap"] = 
            mTable["ClosenessDistance"] = mTable["Affil"] = null;
            mTable["Data"] = null;
            //communities = null;
            //mTable.Clear();
            _cliques = null;
            comcliques = null;

            SESEmatrix = null;
        }

        protected int LoadFromMonadicFileIntoMatrix(string filename, int year, string m)
        {
            // Obtain a file buffer
            BufferedFileTable.AddFile(filename);

            // Jump to the correct year
            int actualYear = BufferedFileTable.GetFile(filename).JumpToNetworkId(year, true);

            string s = BufferedFileTable.GetFile(filename).ReadLine();

            // Calculate the size of this file
            Dictionary<string, int> labels = BufferedFileTable.GetFile(filename).GetDyadicLabels(actualYear);
            int lines = labels.Count;

            // Setup the matrix

            mTable[m] = new Matrix(lines);

            mTable[m].RowLabels.SetLabels(labels.Keys);
            mTable[m].ColLabels.SetLabels(labels.Keys);

            int totalLines = BufferedFileTable.GetFile(filename).CountLines(actualYear);
            for (int i = 0; i < totalLines; ++i)
            {
                s = BufferedFileTable.GetFile(filename).ReadLine();
                string[] parts = s.Split(',');

                double value;
                if (!double.TryParse(parts[2], out value))
                    throw new IOException("Couldn't read value for monadic data at year " + actualYear.ToString() + " row " + parts[1] + " column " + parts[1]);

                mTable[m][labels[parts[1]], labels[parts[1]]] = value;
            }

            return actualYear;
        }

        public int LoadFromMonadicFile(string fileName, int year)
        {
            // Reset various matrices which depend on the data matrix
            Reset();

            // Simply load from the file into the data matrix
            return LoadFromMonadicFileIntoMatrix(fileName, year, "Data");
        }

        public void SaveMatrixToMatrixFile(string fileName, int year, string m,
            bool writeYear, bool writeColLabels, bool Overwrite)
        {
            if (!mTable.ContainsKey(m) || mTable[m] == null)
                throw new IOException("Attempting to write null matrix " + m);

            mTable[m].NetworkId = year;
            MatrixWriter.WriteMatrixToMatrixFile(mTable[m], fileName, Overwrite, writeYear, writeColLabels);
        }

        public void SaveCBCOverlapToFile(string fileName, int year, bool writeYear, bool writeColLabels, bool Overwrite, bool diag)
        {
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            if (writeYear)
                sw.WriteLine(year.ToString());

            // Write out colLabels
            if (writeColLabels)
            {
                for (int i = 0; i < _cliques.Count; ++i)
                {
                    sw.Write("," + (i + 1).ToString());
                }
                sw.WriteLine("");
            }

            for (int row = 0; row < _cliques.Count; ++row)
            {
                double r = 1.0;
                if (diag) 
                    r = _cliques.GetCliqueByCliqueOverlap(row, row);
                string line = (row + 1).ToString() + ",";
                for (int col = 0; col < _cliques.Count; ++col)
                {
                    line += (_cliques.GetCliqueByCliqueOverlap(row, col)/r).ToString();
                    if (col + 1 != _cliques.Count)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
        }

        public void SaveCentralityToFile(string fileName, int year, bool writeYear, bool writeColLabels, bool Overwrite)
        {
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            if (writeYear)
                sw.WriteLine(year.ToString());

            // Write out colLabels
            if (writeColLabels)
            {
                for (int i = 0; i < mTable["Centrality"].Cols; ++i)
                {
                    sw.Write("," + mTable["Centrality"].ColLabels[i]);
                }
                sw.WriteLine();
            }

            for (int row = 0; row < _cliques.Count; ++row)
            {
                string line = mTable["Centrality"].RowLabels[row] + ",";
                for (int col = 0; col < _cliques.Count; ++col)
                {
                    if (col == 1)
                        line += mTable["Centrality"].RowLabels[row];
                    else
                        line += mTable["Centrality"][row, col].ToString();

                    if (col + 1 != _cliques.Count)
                        line += ",";
                }
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
        }



        // Special format for affiliation files
        public void SaveAffiliationMatrixToMatrixFile(string fileName, int year, bool Overwrite)
        {
            bool topLabels = true;
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                {
                    sw = File.AppendText(fileName);
                    topLabels = false;
                }
            }
            if (topLabels)
            {
                sw.Write("year,state");
                for (int i = 0; i < mTable["Affil"].Cols; ++i)
                {
                    sw.Write(",R" + (i + 1).ToString());
                }
                sw.WriteLine("");
            }
            for (int row = 0; row < mTable["Affil"].Rows; ++row)
            {
                string line = year.ToString() + "," + mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < mTable["Affil"].Cols; ++col)
                {
                    line += mTable["Affil"][row, col].ToString();
                    if (col + 1 != mTable["Affil"].Cols)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
        }

        public void SaveAffiliationToMatrixFile(string fileName, int year, double cutoff, bool saveCohesion, bool useStructEquiv, string srcFile,
            bool dyadic, double maxik, string sumMean, string sumMeanFilename, string SF, bool Overwrite,
            bool useCliqueSize, bool useCliqueCohesion, bool useER, int cliqueOrder, bool kCliqueDiag)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");

            if (saveCohesion)
            {
                NPOLStar(useStructEquiv, srcFile, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag);
            }

            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }
            sw.WriteLine(year.ToString());
            // Column labels should be just numbers
            string s = ",";
            for (int j = 0; j < _cliques.Count; ++j)
            {
                s += (j + 1).ToString();
                if (j + 1 != _cliques.Count)
                    s += ",";
            }
            sw.WriteLine(s);

            for (int row = 0; row < mTable["Data"].Rows; ++row)
            {
                string line = mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < _cliques.Count; ++col)
                {
                    line += _cliques[col].IntContains(row).ToString();
                    if (col + 1 != _cliques.Count)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }

            if (SF != null && useCliqueSize)
            {
                NPOLA(year, SF, saveCohesion, cutoff, maxik, cliqueOrder, kCliqueDiag);
                string s1 = "Size,";
                for (int i = 0; i < _cliques.Count; ++i)
                    s1 += NPOLA_Sums[i] + (i < _cliques.Count - 1 ? "," : "");
                sw.WriteLine(s1);
            }

            if (saveCohesion && useCliqueCohesion)
            {
                string s1 = "Cohesion,";
                for (int i = 0; i < _cliques.Count; ++i)
                    s1 += _cliques[i].Cohesion.ToString() + (i < _cliques.Count - 1 ? "," : "");
                sw.WriteLine(s1);
            }

            if (SF != null && saveCohesion && useCliqueCohesion && useCliqueSize)
            {
                string newLine = "Weighted Size,";
                for (int col = 0; col < _cliques.Count; ++col)
                    newLine += (NPOLA_Sums[col] * _cliques[col].Cohesion).ToString() + (col < _cliques.Count ? "," : "");
                sw.WriteLine(newLine);
            }

            // Add the last DVC/SVC Sum/Mean row
            if (sumMean != "None")
            {
                double[] rowVals = new double[_cliques.Count];
                if (sumMean.Contains("DVC"))
                {
                    mTable["D"] = MatrixReader.ReadMatrixFromFile(sumMeanFilename, year);

                    // Now generate clique dyads for each clique
                    for (int i = 0; i < _cliques.Count; ++i)
                    {
                        Vector Q = new Vector(_cliques[i].MemberSet);
                        Matrix Dyads = (Matrix)Q.GetTranspose() * Q;

                        // Multiply elementwise and SUM
                        for (int j = 0; j < Dyads.Rows; ++j)
                            for (int k = 0; k < Dyads.Cols; ++k)
                                rowVals[i] += Dyads[j, k] * mTable["D"][j, k];

                        // Should we do the mean?
                        if (sumMean == "DVCMean")
                            rowVals[i] /= Dyads.Rows * Dyads.Cols;
                    }
                }
                else // SVC
                {
                    BufferedFileTable.AddFile(sumMeanFilename);
                    BufferedFileTable.GetFile(sumMeanFilename).JumpToNetworkId(year);

                    // Now read in the lines and generate av
                    Matrix av = new Matrix(mTable["Data"].Rows, 1);
                    for (int j = 0; j < mTable["Data"].Rows; ++j)
                    {
                        string[] parts = BufferedFileTable.GetFile(sumMeanFilename).ReadLine().Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }

                    for (int j = 0; j < _cliques.Count; ++j)
                    {
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                        {
                            rowVals[j] += av[k, 0] * _cliques[k].IntContains(k);
                        }
                        if (sumMean == "SVCMean")
                            rowVals[j] /= mTable["Data"].Rows;
                    }

                }
                string newline = sumMean + ",";
                for (int i = 0; i < _cliques.Count; ++i)
                {
                    newline += rowVals[i].ToString();
                    if (i + 1 < _cliques.Count)
                        newline += ",";
                }
                sw.WriteLine(newline);
            }

            // add the ER1 row
            if (useER)
            {
                string[] newRow = new string[_cliques.Count];
                for (int col = 0; col < _cliques.Count; ++col)
                {
                    double val = ComputeER1(saveCohesion, SF != null, col);
                    newRow[col] = val.ToString();
                }
                sw.Write("ER1," + string.Join(",", newRow));
            }

            sw.Flush();
            sw.Close();
        }
        public void SaveBlocAffiliationToMatrixFile(string fileName, int year, double p, bool Overwrite, bool multiple)
        {
            if (Blocks == null)
                throw new Exception("Must run CONCOR before saving block affiliation.");

            StreamWriter sw;
            bool topLabels = true;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                {
                    sw = File.AppendText(fileName);
                    //topLabels = false;
                }
            }

            if (topLabels)
            {
                sw.WriteLine(year.ToString());
                // Column labels should be just numbers
                string s = ",";
                for (int j = 0; j < Blocks.Count; ++j)
                {
                    s += (j + 1).ToString();
                    if (j + 1 != Blocks.Count)
                        s += ",";
                }
                sw.WriteLine(s);
            }

            for (int row = 0; row < mTable["Data"].Rows; ++row)
            {
                string line = mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < Blocks.Count; ++col)
                {
                    int l = 0;
                    if (!int.TryParse(mTable["Data"].RowLabels[row], out l))
                        l = row + 1;
                    if (Blocks[col].Contains(l))
                        line += "1";
                    else
                        line += "0";
                    if (col + 1 != Blocks.Count)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }

            sw.Flush();
            sw.Close();
        }

        public void SaveCommAffiliationToAffiliationFile(string fileName, int year, bool label, bool Overwrite)
        {

            bool topLabels = true;
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                {
                    sw = File.AppendText(fileName);
                    topLabels = false;
                }
            }
            if (topLabels)
            {
                sw.Write("year,state");
                for (int i = 0; i < mTable["Community"].Cols; ++i)
                {
                    sw.Write(",R" + (i + 1).ToString());
                }
                sw.WriteLine("");
            }
            for (int row = 0; row < mTable["Community"].Rows; ++row)
            {
                string line = year.ToString() + "," + mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < mTable["Community"].Cols; ++col)
                {
                    line += mTable["Community"][row, col].ToString();
                    if (col + 1 != mTable["Community"].Cols)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();

        }

        public void SaveOverlapCommAffiliationToAffiliationFile(string fileName, int year, bool label, bool Overwrite)
        {

            bool topLabels = true;
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                {
                    sw = File.AppendText(fileName);
                    topLabels = false;
                }
            }
            if (topLabels)
            {
                sw.Write("year,state");
                for (int i = 0; i < mTable["OverlappingCommunity"].Cols; ++i)
                {
                    sw.Write(",R" + (i + 1).ToString());
                }
                sw.WriteLine("");
            }
            for (int row = 0; row < mTable["OverlappingCommunity"].Rows; ++row)
            {
                string line = year.ToString() + "," + mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < mTable["OverlappingCommunity"].Cols; ++col)
                {
                    line += mTable["OverlappingCommunity"][row, col].ToString();
                    if (col + 1 != mTable["OverlappingCommunity"].Cols)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();

        }

        public void SaveBlocAffiliationToAffiliationFile(string fileName, int year, double p, bool Overwrite, bool multiple, bool cluster)
        {
            if (Blocks == null)
                throw new Exception("Must run CONCOR before saving block affiliation.");

            StreamWriter sw;
            bool topLabels = true;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                {
                    sw = File.AppendText(fileName);
                    topLabels = false;
                }
            }

            if (topLabels)
            {
                sw.Write("\"year\",\"state\"");
                // Column labels should be just numbers
                for (int j = 0; j < Blocks.Count; ++j)
                {
                    if (cluster) sw.Write(",Cluster ");
                    else sw.Write(",Block ");
                    sw.Write(j + 1);
                }
                sw.WriteLine("");
            }

            for (int row = 0; row < mTable["Data"].Rows; ++row)
            {
                string line = year.ToString() + "," + mTable["Data"].RowLabels[row] + ",";
                for (int col = 0; col < Blocks.Count; ++col)
                {
                    if (Blocks[col].Contains(row))
                        line += "1";
                    else
                        line += "0";
                    if (col + 1 != Blocks.Count)
                        line += ",";
                }
                Application.DoEvents();
                sw.WriteLine(line);
            }

            sw.Flush();
            sw.Close();
        }

        public void SaveNationalDependencyToMatrixFile(string fileName, int year, bool Overwrite)
        {
            LoadUnitDependency(year);

            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
              //  sw.WriteLine(year.ToString());
                sw.WriteLine("Year,Node,OUTD,OND,Self-Dep,Dep Balance");
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
               //     sw.WriteLine(year.ToString());
                    sw.WriteLine("Year,Node,OUTD,OND,Self-Dep,Dep Balance");
                }
                else
                    sw = File.AppendText(fileName);
            }
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
               // sw.WriteLine(year.ToString());
                sw.WriteLine("Year,Node,OUTD,OND,Self-Dep,Dep Balance");
            }

            for (int row = 0; row < mTable["Data"].Rows; ++row)
            {
                sw.WriteLine( year.ToString() + "," + mTable["Data"].RowLabels[row] + "," + mTable["NatDep"][row, 2].ToString() + "," + mTable["NatDep"][row, 3].ToString()
                    + "," + mTable["NatDep"][row, 4].ToString() + "," + mTable["NatDep"][row, 5].ToString());
                Application.DoEvents();

            }
            sw.Flush();
            sw.Close();
        }

        public string MakeDefaultDyadicLabel(string value)
        {
            return "\"year\",\"statei\",\"statej\",\"" + value + "\"";
        }

        public void SaveMatrixToDyadicFile(string fileName, int year, string m, string label, bool Overwrite)
        {
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            // Should we print the label?
            if (label != null)
            {
                sw.WriteLine(label);
            }
            else if (label == "")
            {
                sw.WriteLine("\"year\",\"statei\",\"statej\",\"value\""); // Some decent default?
            }


            //for (int row = 0; row < temp; row++)
            //{
            //     generate an array to hold this row
            //    string[] newrow = new string[temp];

            //    double r = 1.0;
            //    if (stddiag)
            //        r = _cliques.getcliquebycliqueoverlap(row, row);
            //    for (int col = 0; col < temp; col++)
            //        newrow[col] = (_cliques.getcliquebycliqueoverlap(row, col) / r).tostring();

            //    data.rows.add(newrow);

            //    data.rows[row].headercell.value = data.columns[row].headercell.value;
            //}
            
            if (m == "CBCO" || m == "CBCODiag")
            {
                _cliques.LoadCliqueByCliqueOverlap();
                double clique_value = 0;
                int temp = _cliques.Count;
                string tempLine;


                for (int row = 0; row < temp; ++row)
                {
                    double r = 1.0;
                    if (m == "CBCODiag")
                        r = _cliques.GetCliqueByCliqueOverlap(row, row);                  
                    string line = year.ToString() + "," + (row + 1).ToString() + ",";
                    for (int col = 0; col < temp; col++)
                    {
                        clique_value = _cliques.GetCliqueByCliqueOverlap(row, col) / r;
                        tempLine = line + (col + 1).ToString() + "," + clique_value.ToString();

                        sw.WriteLine(tempLine);
                    }
                }
            }

            else
            {
                for (int row = 0; row < mTable[m].Rows; ++row)
                {
                    Application.DoEvents();
                    for (int col = 0; col < mTable[m].Cols; ++col)
                    {
                        string line = year.ToString() + "," + mTable[m].RowLabels[row].ToString() + ",";
                        line += mTable[m].RowLabels[col].ToString() + "," + mTable[m][row, col].ToString();
                        sw.WriteLine(line);
                    }

                }
            }
            sw.Flush();
            sw.Close();
        }

        public void LoadMultiplicationMatrix(string filename, bool dyadic, int year, string targetMatrix, string sourceMatrix)
        {
            mTable[targetMatrix] = MatrixReader.ReadMatrixFromFile(filename, year);
            try
            {
                mTable[targetMatrix] = mTable[sourceMatrix] * mTable[targetMatrix];
                mTable[targetMatrix].CopyLabelsFrom(mTable[sourceMatrix]);
            }
            catch (Exception)
            {
                throw new Exception("Matrix sizes do not match for multiplication!");
            }
        }

        // Leave this one for now? (because it is sort of special??)
        // Might change later
        public void SaveAffiliationToDyadicFile(string fileName, int year, bool label, bool Overwrite)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");

            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            // Should we print the label?
            if (label)
            {
                sw.Write("\"year\",\"state\"");
                for (int i = 0; i < _cliques.Count; ++i)
                    sw.Write(",R" + (i + 1).ToString());
                sw.WriteLine("");
            }

            for (int col = 0; col < mTable["Data"].Cols; ++col)
            {
                string line = year.ToString() + "," + mTable["Data"].ColLabels[col];
                for (int i = 0; i < _cliques.Count; ++i)
                    line += "," + _cliques[i].IntContains(col).ToString();
                sw.WriteLine(line);
            }

            sw.Flush();
            sw.Close();
        }

        public void SaveAffiliationMatrixToFile(string fileName, List<clique> c)
        {
            int year = 1;
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                File.Delete(fileName);
                sw = File.CreateText(fileName);
            }

            //Print the lable
            sw.Write("\"year\",\"state\"");
            for (int i = 0; i < _cliques.Count; ++i)
                sw.Write(",R" + (i + 1).ToString());
            sw.WriteLine("");

            for (int col = 0; col < mTable["Data"].Cols; ++col)
            {
                string line = year.ToString() + "," + mTable["Data"].ColLabels[col];
                for (int i = 0; i < c.Count; ++i)
                    line += "," + c[i][col].ToString();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
        }


        public void SaveCounterToFile(string fileName, int year, bool label, string sep, double cutoff, double density, string inputType, string externalFile,
            string svcFile, bool useCohesion, MatrixComputations.TransitivityType transitivityType, bool[] options, bool Overwrite, bool zeroDiagonal, bool reachSum, int reachMatrixCount, string erpolType, double alpha, int cliqueOrder,
            bool kCliqueDiag, string selectionMethod)
        {
           // density = GetRealDensity(density, year, "Data");

            if (inputType == "StructEquiv")
                LoadStructEquiv(density, year, "Data");

            bool useCliq = false;
            if (selectionMethod == "Cliq")
                useCliq = true;
            

            //bool loadCOC = false;
            //if (options[10] || options[11] || options[14] || options[18] || options[24])
            //    loadCOC = true;            

            if (options[23])
                LoadReachability(reachMatrixCount, reachSum, zeroDiagonal, "Data", year, false);
            if (options[5] || options[6])
            {
                if (selectionMethod == "Cliq")
                    LoadComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
                else if (selectionMethod == "Bloc" || selectionMethod == "Clus")
                    LoadbComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
                else if (selectionMethod == "Comm")
                    LoadcomComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
            }
            if (options[19] || options[20] || options[23])
                LoadDependency("Data", reachMatrixCount, density, year, zeroDiagonal, reachSum);
            if (options[25] || options[26] || options[27] || options[28] || options[29] || options[30] || options[31] || options[32] || options[33] || options[34])
                LoadCentralityIndices("Data", year, 1, false, false);
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }
            NetworkCharacteristics nc = new NetworkCharacteristics(this, year, inputType, externalFile, svcFile, transitivityType,
                cutoff, density, zeroDiagonal, reachMatrixCount, erpolType, alpha, cliqueOrder, kCliqueDiag);
            nc.selMeth = selectionMethod;

            for (int i = 0; i < options.Length; ++i)
                nc.SetLabel(i, options[i]);

            if (label)
                sw.WriteLine(nc.Label);

            sw.WriteLine(string.Join(",", nc.Line));

            nc = null;

            sw.Flush();
            sw.Close();
            sw = null;
        }


        public void SaveCounterToFile(string fileName, bool label, bool Overwrite)
        {
            if (mTable["Counter"] == null)
                throw new Exception("Counter Matrix does not exist");
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            string[] labels = new string[mTable["Counter"].Cols];
            for (int i = 0; i < mTable["Counter"].Cols; i++)
                labels[i] = mTable["Counter"].ColLabels[i];

            if (label)
                sw.WriteLine(string.Join(",", labels));

            string[] values = new string[mTable["Counter"].Cols];
            for (int col = 0; col < mTable["Counter"].Cols; col++)
                values[col] = mTable["Counter"][0, col].ToString();
            sw.WriteLine(string.Join(",", values));

            sw.Flush();
            sw.Close();
            sw = null;

        }


        public void SaveSignedNetworkToFile(string fileName, bool label, bool Overwrite)
        {
            if (mTable["SignedNetwork"] == null)
                throw new Exception("Signed Network Characteristics Matrix does not exist");
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            string[] labels = new string[mTable["SignedNetwork"].Cols];
            for (int i = 0; i < mTable["SignedNetwork"].Cols; i++)
                labels[i] = mTable["SignedNetwork"].ColLabels[i];

            if (label)
                sw.WriteLine(string.Join(",", labels));

            string[] values = new string[mTable["SignedNetwork"].Cols];
            for (int col = 0; col < mTable["SignedNetwork"].Cols; col++)
                values[col] = mTable["SignedNetwork"][0, col].ToString();
            sw.WriteLine(string.Join(",", values));

            sw.Flush();
            sw.Close();
            sw = null;
        }

        public void SaveAsTableToFile(string fileName, bool label, bool Overwrite, string m, CommunityType commType)
        {
            bool modularityLabels = false;
            if (m == "ClusterCharacteristics")
                m = "BlockCharacteristics";
            if (mTable[m] == null)
                throw new Exception(m.ToString() + " matrix does not exist");
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
                modularityLabels = true;
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                    modularityLabels = true;

                }
                else
                {
                    sw = File.AppendText(fileName);
                    modularityLabels = false;
                }
            }
            if (commType == CommunityType.Cluster)
            {
                string networkID = mTable[m].NetworkId.ToString();
                string value;
                if (modularityLabels)
                    sw.WriteLine("Network ID, Iteration, Modularity Coefficient,");
                for (int row = 0; row < mTable[m].Rows; row++)
                {
                    value = mTable[m][row, 0].ToString();
                    sw.Write(networkID);
                    sw.Write(",");
                    sw.Write(row.ToString());
                    sw.Write(",");
                    sw.Write(value);
                    sw.WriteLine(",");
                }
                sw.Flush();
                sw.Close();
                sw = null;
            }
            else
            {
                string[] labels = new string[mTable[m].Cols];
                for (int i = 0; i < mTable[m].Cols; i++)
                    labels[i] = mTable[m].ColLabels[i];


                if (label)
                    sw.WriteLine(string.Join(",", labels));
                for (int row = 0; row < mTable[m].Rows; row++)
                {
                    string[] values = new string[mTable[m].Cols];
                    for (int col = 0; col < mTable[m].Cols; col++)
                    {
                        if (mTable[m].ColIsNonInteger && col == mTable[m].ColOfNonInteger)
                            values[col] = mTable[m].ActualCol[row];
                        else
                            values[col] = mTable[m][row, col].ToString();
                    }
                    sw.WriteLine(string.Join(",", values));
                }


                sw.Flush();
                sw.Close();
                sw = null;
            }
        }







        // This is for the multiple file correlation measure
        public int LoadFromMultipleFiles(string[] files, int year)
        {
            // New code to create a list of multiple matrices for
            // multiple matrix input files

            multipleMatrixList = new List<Matrix>();
            foreach (string file in files)
            {
                multipleMatrixList.Add(MatrixReader.ReadMatrixFromFile(file, year));
            }

            // Not sure what the other code does for multiple matrix
            // input files

            mTable["Temp"] = MatrixReader.ReadMatrixFromFile(files[0], year);

            mTable["SEC"] = new Matrix(mTable["Temp"].Rows);
            mTable["SEE"] = new Matrix(mTable["Temp"].Rows);
            mTable["SESE"] = new Matrix(mTable["Temp"].Rows);
            mTable["RoleEquiv"] = new Matrix(mTable["Temp"].Rows);

            // We want three temp matrices to store the values for the three sums
            Matrix topSums = new Matrix(mTable["Temp"].Rows);
            Matrix sqrtLeftSums = new Matrix(mTable["Temp"].Rows);
            Matrix sqrtRightSums = new Matrix(mTable["Temp"].Rows);

            int count = 0;

            // For the first run do the Transpose
            foreach (string file in files)
            {
                if (count++ > 0)
                    mTable["Temp"] = MatrixReader.ReadMatrixFromFile(file, year);

                Matrix M = mTable["Temp"].GetTranspose();

                // Now that we have the year loaded, make the temp sums for each entry in this year
                for (int i = 0; i < M.Rows; ++i)
                {
                    for (int j = 0; j < M.Cols; ++j)
                    {
                        for (int k = 0; k < M.Rows; ++k)
                        {
                            topSums[i, j] += (M[i, k] - M.GetRowAverage(i)) * (M[j, k] - M.GetRowAverage(j));
                            sqrtLeftSums[i, j] += Math.Pow(M[i, k] - M.GetRowAverage(i), 2);
                            sqrtRightSums[i, j] += Math.Pow(M[j, k] - M.GetRowAverage(j), 2);

                            // We can do the euclidean right away
                            mTable["SEE"][i, j] += Math.Pow(M[i, k] - M[j, k], 2);
                            mTable["SEE"][i, j] += Math.Pow(M[k, i] - M[k, j], 2);
                        }
                    }
                }

                mTable["Temp"] = null;
            }

            double maxMatrixValue = double.MinValue;
            // Do the nontranspose 
            // And do role equiv as well

            
            foreach (string file in files)
            {
                Matrix M = MatrixReader.ReadMatrixFromFile(file, year);

                // Generate temporary triad matrix
                mTable["TempTriad"] = new Matrix(M.Rows, 36);
                mTable["TempTriad"].Clear();

                for (int i = 0; i < M.Rows; ++i)
                {
                    for (int j = 0; j < M.Rows; ++j)
                    {
                        if (j == i)
                            continue;
                        for (int k = j + 1; k < M.Rows; ++k)
                        {
                            if (k == i || k == j)
                                continue;

                            int type = GetRoleEquivalenceType(GetRelationshipType(i, j, M),
                                                              GetRelationshipType(i, k, M),
                                                              GetRelationshipType(j, k, M));

                            ++mTable["TempTriad"][i, type - 1];
                        }
                    }
                }

                for (int i = 0; i < mTable["RoleEquiv"].Rows; ++i)
                {
                    for (int j = 0; j < mTable["RoleEquiv"].Cols; ++j)
                    {
                        double requiv = 0.0;
                        for (int k = 0; k < mTable["TempTriad"].Cols; ++k)
                            requiv += Math.Pow(mTable["TempTriad"][i, k] - mTable["TempTriad"][j, k], 2);
                        mTable["RoleEquiv"][i, j] += Math.Sqrt(requiv);
                    }
                }

                // Now that we have the year loaded, make the temp sums for each entry in this year
                for (int i = 0; i < M.Rows; ++i)
                {
                    for (int j = 0; j < M.Cols; ++j)
                    {
                        double sese = 0.0;
                        for (int k = 0; k < M.Rows; ++k)
                        {
                            topSums[i, j] += (M[i, k] - M.GetRowAverage(i)) * (M[j, k] - M.GetRowAverage(j));
                            sqrtLeftSums[i, j] += Math.Pow(M[i, k] - M.GetRowAverage(i), 2);
                            sqrtRightSums[i, j] += Math.Pow(M[j, k] - M.GetRowAverage(j), 2);

                            // We can do the euclidean right away
                            mTable["SEE"][i, j] += Math.Pow(M[i, k] - M[j, k], 2);
                            mTable["SEE"][i, j] += Math.Pow(M[k, i] - M[k, j], 2);

                            double d = Math.Pow(M[i, k] - M[j, k], 2);

                            if (d > maxMatrixValue)
                                maxMatrixValue = d;

                            sese += d;
                            sese += Math.Pow(M[k, i] - M[k, j], 2);
                        }
                        mTable["SESE"][i, j] = sese;
                    }
                }
                mTable["Temp"] = null;
            }

            

            // Now set up the two struct equiv matrices
            for (int i = 0; i < mTable["SEE"].Rows; ++i)
            {
                for (int j = 0; j < mTable["SEE"].Cols; ++j)
                {
                    mTable["SEC"][i, j] = topSums[i, j] / (Math.Sqrt(sqrtLeftSums[i, j]) * Math.Sqrt(sqrtRightSums[i, j]));
                    mTable["SEE"][i, j] = Math.Sqrt(mTable["SEE"][i, j] / 2.0);
                    mTable["SESE"][i, j] = 1.0 - mTable["SEE"][i, j] / (mTable["SEE"].Rows * Math.Sqrt(files.Length * maxMatrixValue)); // should be maxik not 1.0

                    mTable["RoleEquiv"][i, j] = 1.0 - (mTable["RoleEquiv"][i, j] / (files.Length * (mTable["SEE"].Rows - 1) * (mTable["SEE"].Rows - 2)));
                }
            }

            // Now just set up the last matrix
            mTable["Data"] = MatrixReader.ReadMatrixFromFile(files[files.Length - 1], year);

            mTable["SEC"].CopyLabelsFrom(mTable["Data"]);
            mTable["SEE"].CopyLabelsFrom(mTable["Data"]);
            mTable["SESE"].CopyLabelsFrom(mTable["Data"]);

            SESEmatrix = new Matrix(mTable["SESE"]);

            return mTable["Data"].NetworkId;
        }

        public int LoadFromMultivariableDyadicFile(string fileName, int year)
        {
            Reset();

            int variableCount = BufferedFileTable.GetFile(fileName).CountVarsInDyadicFile() - 1;

            multipleMatrixList = new List<Matrix>();
            for (int i = 0; i <= variableCount; i++)
            {
                multipleMatrixList.Add(MatrixReader.ReadMatrixFromFile(fileName, year, i));
            }

            // Load the first variable last so it remains
            mTable["Data"] = MatrixReader.ReadMatrixFromFile(fileName, year, variableCount);
            year = mTable["Data"].NetworkId;

            mTable["SEC"] = new Matrix(mTable["Data"].Rows);
            mTable["SEE"] = new Matrix(mTable["Data"].Rows);
            mTable["SESE"] = new Matrix(mTable["Data"].Rows);

            // We want three temp matrices to store the values for the three sums
            Matrix topSums = new Matrix(mTable["Data"].Rows);
            Matrix sqrtLeftSums = new Matrix(mTable["Data"].Rows);
            Matrix sqrtRightSums = new Matrix(mTable["Data"].Rows);

            // For the first run do the Transpose
            for (int curVar = variableCount; curVar >= 0; --curVar)
            {
                if (curVar != variableCount) // Skip first load
                    mTable["Data"] = MatrixReader.ReadMatrixFromFile(fileName, year, variableCount);

                Matrix M = mTable["Data"].GetTranspose() as Matrix;

                // Now that we have the year loaded, make the temp sums for each entry in this year
                for (int i = 0; i < M.Rows; ++i)
                {
                    for (int j = 0; j < M.Cols; ++j)
                    {
                        for (int k = 0; k < M.Rows; ++k)
                        {
                            topSums[i, j] += (M[i, k] - M.GetRowAverage(i)) * (M[j, k] - M.GetRowAverage(j));
                            sqrtLeftSums[i, j] += Math.Pow(M[i, k] - M.GetRowAverage(i), 2);
                            sqrtRightSums[i, j] += Math.Pow(M[j, k] - M.GetRowAverage(j), 2);

                            // We can do the euclidean right away
                            mTable["SEE"][i, j] += Math.Pow(M[i, k] - M[j, k], 2);
                            mTable["SEE"][i, j] += Math.Pow(M[k, i] - M[k, j], 2);
                        }
                    }
                }
            }

            // Do the nontranspose
            for (int curVar = variableCount; curVar >= 0; --curVar)
            {
                mTable["Data"] = MatrixReader.ReadMatrixFromFile(fileName, year, variableCount);

                Matrix M = mTable["Data"];

                // Now that we have the year loaded, make the temp sums for each entry in this year
                for (int i = 0; i < M.Rows; ++i)
                {
                    for (int j = 0; j < M.Cols; ++j)
                    {
                        for (int k = 0; k < M.Rows; ++k)
                        {
                            topSums[i, j] += (M[i, k] - M.GetRowAverage(i)) * (M[j, k] - M.GetRowAverage(j));
                            sqrtLeftSums[i, j] += Math.Pow(M[i, k] - M.GetRowAverage(i), 2);
                            sqrtRightSums[i, j] += Math.Pow(M[j, k] - M.GetRowAverage(j), 2);

                            // We can do the euclidean right away
                            mTable["SEE"][i, j] += Math.Pow(M[i, k] - M[j, k], 2);
                            mTable["SEE"][i, j] += Math.Pow(M[k, i] - M[k, j], 2);
                        }
                    }
                }
            }

            // Now set up the two struct equiv matrices
            for (int i = 0; i < mTable["Data"].Rows; ++i)
            {
                for (int j = 0; j < mTable["Data"].Cols; ++j)
                {
                    mTable["SEC"][i, j] = topSums[i, j] / (Math.Sqrt(sqrtLeftSums[i, j]) * Math.Sqrt(sqrtRightSums[i, j]));
                    mTable["SEE"][i, j] = Math.Sqrt(mTable["SEE"][i, j]);
                    mTable["SESE"][i, j] = 1.0 - Math.Sqrt(mTable["SEE"][i, j] / (2 * mTable["SEE"].Rows * 1.0)); // should be maxik not 1.0
                }
            }

            mTable["SEC"].CopyLabelsFrom(mTable["Data"]);
            mTable["SEE"].CopyLabelsFrom(mTable["Data"]);
            mTable["SESE"].CopyLabelsFrom(mTable["Data"]);

            SESEmatrix = new Matrix(mTable["SESE"]);

            return year;
        }

        // Needs to be here because it uses the row/col labels
        public void LoadCentralityIndices(string m, int year, double sijmax, bool countMember, bool zeroDiagonal)
        {
            mTable["Centrality"] = new Matrix(mTable["Data"].Rows, 10);
            mTable["Centrality"].ColLabels.SetLabels("Year,State,DO,DI,CO,CI,BO,BI,EO,EI");
            mTable["Centrality"].RowLabels.CopyFrom(mTable["Data"].RowLabels);

            LoadClosenessDistance("Data");

            // This consists of 9 separate columns
            for (int i = 0; i < mTable["Data"].Rows; ++i)
            {
                // 0.   Year
                mTable["Centrality"][i, 0] = year;

                // 1.   State ID
                double tmp;
                if (double.TryParse(mTable["Data"].RowLabels[i], out tmp))
                    mTable["Centrality"][i, 1] = tmp;
                else
                    mTable["Centrality"][i, 1] = i;

                // 2.   Outgoing Degree Centrality
                double DO = mTable["Data"].GetRowSum(i);
                DO -= mTable["Data"][i, i];
                DO /= mTable["Data"].Rows - 1;
                mTable["Centrality"][i, 2] = DO;

                // 3.   Incoming Degree Centrality
                double DI = mTable["Data"].GetColSum(i);
                DI -= mTable["Data"][i, i];
                DI /= mTable["Data"].Rows - 1;
                mTable["Centrality"][i, 3] = DI;

                // 4.   Outgoing Closeness Centrality
                double CO = mTable["ClosenessDistance"].GetRowSum(i) - mTable["ClosenessDistance"][i, i];

                mTable["Centrality"][i, 4] = sijmax * (mTable["Data"].Rows - 1) / CO;

                // 5.   Incoming Closeness Centrality
                double CI = mTable["ClosenessDistance"].GetColSum(i) - mTable["ClosenessDistance"][i, i];

                mTable["Centrality"][i, 5] = sijmax * (mTable["Data"].Rows - 1) / CI;
            }

            Vector BOVector = MatrixComputations.BetweennessCentrality(mTable["Data"]);
            mTable["Centrality"].SetColVector(6, BOVector);
            mTable["Centrality"].SetColVector(7, BOVector);

            Vector EOVector = MatrixComputations.EigenvectorCentrality(mTable["Data"]);
            mTable["Centrality"].SetColVector(8, EOVector);
            mTable["Centrality"].SetColVector(9, EOVector);
        }

        // Random data load
        public void LoadRandom(int N, string m, bool symmetric, bool range, double pmin, double pmax, bool randomN, int randomMin, int randomMax, int randomInt)
        {
            if (randomN)
                N = RNG.RandomInt((randomMax - randomMin) / randomInt) * randomInt + randomMin; 

            if (symmetric)
                mTable[m] = RandomMatrix.LoadSymmetric(N, range, pmin, pmax);
            else
                mTable[m] = RandomMatrix.LoadNonSymmetric(N, range, pmin, pmax);
            
            tempDataMatrix = new Matrix(mTable[m]);
        }

        public void LoadValuedRandom(int N, string m, bool symmetric, double vmin, double vmax, bool datatype, bool zerodiagonalized, bool range, double pmin, double pmax, bool randomN, int randomMin, int randomMax, int randomInt)
        {
            if (randomN)
                N = RNG.RandomInt((randomMax - randomMin) / randomInt) * randomInt + randomMin; 

            if (symmetric)
                mTable[m] = RandomMatrix.LoadValuedSymmetric(N,vmin,vmax, datatype,zerodiagonalized, range, pmin, pmax);
            else
                mTable[m] = RandomMatrix.LoadValuedNonSymmetric(N,vmin,vmax,datatype,zerodiagonalized, range, pmin, pmax);

            tempDataMatrix = mTable[m];
        }

        // The multiple/multivar saving routines
        public void SaveToMultipleMatrixFiles(string fromFile, string[] toFiles, int year, bool Overwrite)
        {
            // Convert the multivar dyadic into several matrix files
            for (int skip = 0; skip < toFiles.Length; ++skip)
            {
                // Load the file
                mTable["Temp"] = MatrixReader.ReadMatrixFromFile(fromFile, year, skip);
                year = mTable["Temp"].NetworkId;

                // And save it
                SaveMatrixToMatrixFile(toFiles[skip], year, "Temp", true, true, Overwrite);
            }
        }

        public int SaveToMultivariableDyadicFile(string[] fromFiles, string fileName, int year, int prevYear, bool Overwrite)
        {
            // Load the first one
            year = LoadFromMatrixFileIntoMatrix(fromFiles[0], year, "Temp");

            if (year == prevYear)
                return year;

            Matrix M = mTable["Temp"];

            // Now setup a matrix to store the dyadic values
            Matrix m = new Matrix(M.Rows * M.Cols, fromFiles.Length);

            try
            {
                for (int i = 0; i < fromFiles.Length; ++i)
                {
                    if (i > 0)
                        LoadFromMatrixFileIntoMatrix(fromFiles[i], year, "Temp");

                    M = mTable["Temp"];

                    // Add to our matrix
                    for (int r = 0; r < M.Rows; ++r)
                    {
                        for (int c = 0; c < M.Cols; ++c)
                        {
                            m[r * M.Cols + c, i] = M[r, c];
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("The files you are converting to do not have identical sizes over the year range requested."
                    + " The conversion therefore cannot be completed. Please check the files and try again.");
            }

            // Now just save the matrix
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (Overwrite)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                else
                    sw = File.AppendText(fileName);
            }

            sw.Write("year,statei,statej,");
            for (int i = 0; i < fromFiles.Length; ++i)
                if (i != fromFiles.Length - 1)
                    sw.Write("var" + (i + 1) + ",");
                else
                    sw.WriteLine("var" + (i + 1));

            M = mTable["Temp"];
            for (int i = 0; i < m.Rows; ++i)
            {
                sw.Write(year + ",");

                // Figure out the row and column labels
                int r = i / M.Cols;
                int c = i % M.Cols;

                if (r >= M.RowLabels.Length || c >= M.RowLabels.Length)
                    continue;

                sw.Write(M.RowLabels[r] + "," + M.RowLabels[c] + ",");
                for (int j = 0; j < m.Cols; ++j)
                {
                    if (j + 1 < m.Cols)
                        sw.Write(m[i, j] + ",");
                    else
                        sw.WriteLine(m[i, j]);
                }
                Application.DoEvents();
            }

            sw.Flush();
            sw.Close();

            return year;
        }

        public int SaveToMultivariableDyadicFile(Matrix[] ms, string fileName)
        {
            Matrix M = ms[0];
            // Now setup a matrix to store the dyadic values
            Matrix m = new Matrix(M.Rows * M.Cols, ms.Length);

            try
            {
                for (int i = 0; i < ms.Length; ++i)
                {
                    M = ms[i];

                    // Add to our matrix
                    for (int r = 0; r < M.Rows; ++r)
                    {
                        for (int c = 0; c < M.Cols; ++c)
                        {
                            m[r * M.Cols + c, i] = M[r, c];
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("The files you are converting to do not have identical sizes over the year range requested."
                    + " The conversion therefore cannot be completed. Please check the files and try again.");
            }

            // Now just save the matrix
            StreamWriter sw;
            if (!File.Exists(fileName))
            {
                sw = File.CreateText(fileName);
            }
            else
            {
                if (true)
                {
                    File.Delete(fileName);
                    sw = File.CreateText(fileName);
                }
                //else
                //    sw = File.AppendText(fileName);
            }

            sw.Write("year,statei,statej,");
            for (int i = 0; i < ms.Length; ++i)
                if (i != ms.Length - 1)
                    sw.Write("var" + (i + 1) + ",");
                else
                    sw.WriteLine("var" + (i + 1));

            M = ms[0];
            for (int i = 0; i < m.Rows; ++i)
            {
                sw.Write("0" + ",");

                // Figure out the row and column labels
                int r = i / M.Cols;
                int c = i % M.Cols;

                if (r >= M.RowLabels.Length || c >= M.RowLabels.Length)
                    continue;

                sw.Write(M.RowLabels[r] + "," + M.RowLabels[c] + ",");
                for (int j = 0; j < m.Cols; ++j)
                {
                    if (j + 1 < m.Cols)
                        sw.Write(m[i, j] + ",");
                    else
                        sw.WriteLine(m[i, j]);
                }
                Application.DoEvents();
            }

            sw.Flush();
            sw.Close();

            return 0;
        }

        public int LoadFromAffiliationFile(string fileName, int year)
        {
            Reset();

            // Obtain a file buffer
            BufferedFileTable.AddFile(fileName);

            // New way: Read in the first line of this year to get the number of columns

            // Jump to the correct year
            int actualYear = BufferedFileTable.GetFile(fileName).JumpToNetworkId(year, true);

            // Calculate the size of this file
            int lines = BufferedFileTable.GetFile(fileName).CountLines(actualYear);
            int cols = BufferedFileTable.GetFile(fileName).ReadLine().Split(',').Length;

            BufferedFileTable.GetFile(fileName).JumpToNetworkId(year, true);

            Matrix Affil = mTable.AddMatrix("Affil", lines, cols - 2);
            mTable["Affil"].Clear();
            mTable["Affil"].NetworkId = actualYear;

            for (int i = 1; i < cols - 1; ++i)
                Affil.ColLabels[i - 1] = "R" + i.ToString();

            string s = null;
            for (int line = 0; line < lines; ++line)
            {
                s = BufferedFileTable.GetFile(fileName).ReadLine();

                string[] parts = s.Split(',');

                if (parts.Length - 2 > Affil.Cols)
                    throw new IOException("Affiliation file has too many entries for year " + actualYear.ToString());

                // Add this row's label
                Affil.RowLabels[line] = parts[1];

                // Read in this row
                for (int i = 2; i < parts.Length; ++i)
                {
                    double tmp;
                    if (!double.TryParse(parts[i], out tmp))
                        throw new IOException("Expecting floating point value: " + parts[i]);
                    Affil[line, i - 2] = tmp;
                }
            }

            tempAffilMatrix = new Matrix(mTable["Affil"]); // need to keep track of this

            // Set up affiliation stuff
            mTable["Temp"] = mTable["Affil"].GetTranspose();
            mTable["Data"] = (mTable["Affil"] * mTable["Temp"]);
            mTable["EventOverlap"] = (mTable["Temp"] * mTable["Affil"]);
            mTable["DataEvent"] = mTable["EventOverlap"];

            // The two new affiliation measures
            mTable["AffilCorrelation"] = new Matrix(mTable["Affil"].Rows);
            mTable["AffilCorrelation"].RowLabels.CopyFrom(mTable["Affil"].RowLabels);
            mTable["AffilCorrelation"].ColLabels.CopyFrom(mTable["Affil"].RowLabels);
            for (int i = 0; i < mTable["Affil"].Rows; ++i)
            {
                for (int j = 0; j < mTable["Affil"].Rows; ++j)
                {
                    double EXY = 0.0;
                    for (int m = 0; m < mTable["Affil"].Cols; ++m)
                    {
                        EXY += mTable["Affil"][i, m] * mTable["Affil"][j, m];
                    }
                    EXY /= mTable["Affil"].Cols;
                    double EX = mTable["Affil"].GetRowAverage(i);
                    double EY = mTable["Affil"].GetRowAverage(j);
                    double EXS = mTable["Affil"].GetRowSquareAverage(i);
                    double EYS = mTable["Affil"].GetRowSquareAverage(j);
                    mTable["AffilCorrelation"][i, j] = (EXY - EX * EY) / (Math.Sqrt(EXS - EX * EX) * Math.Sqrt(EYS - EY * EY));
                }
            }

            mTable["AffilCorrelationEvent"] = new Matrix(mTable["Affil"].Cols);
            mTable["AffilCorrelationEvent"].RowLabels.CopyFrom(mTable["Affil"].ColLabels);
            mTable["AffilCorrelationEvent"].ColLabels.CopyFrom(mTable["Affil"].ColLabels);
            for (int i = 0; i < mTable["Affil"].Cols; ++i)
            {
                for (int j = 0; j < mTable["Affil"].Cols; ++j)
                {
                    double EXY = 0.0;
                    for (int m = 0; m < mTable["Affil"].Rows; ++m)
                    {
                        EXY += mTable["Affil"][m, i] * mTable["Affil"][m, j];
                    }
                    EXY /= mTable["Affil"].Rows;
                    double EX = mTable["Affil"].GetColAverage(i);
                    double EY = mTable["Affil"].GetColAverage(j);
                    double EXS = mTable["Affil"].GetColSquareAverage(i);
                    double EYS = mTable["Affil"].GetColSquareAverage(j);
                    mTable["AffilCorrelationEvent"][i, j] = (EXY - EX * EY) / (Math.Sqrt(EXS - EX * EX) * Math.Sqrt(EYS - EY * EY));
                }
            }


            mTable["AffilEuclidean"] = new Matrix(mTable["Affil"].Rows);
            mTable["AffilEuclidean"].RowLabels.CopyFrom(mTable["Affil"].RowLabels);
            mTable["AffilEuclidean"].ColLabels.CopyFrom(mTable["Affil"].RowLabels);
            double kmax = mTable["Affil"].Cols * Math.Pow(Algorithms.MaxValue<double>(mTable["Affil"]) - Algorithms.MinValue<double>(mTable["Affil"]), 2);
            for (int i = 0; i < mTable["Affil"].Rows; ++i)
            {
                for (int j = 0; j < mTable["Affil"].Rows; ++j)
                {
                    double sum = 0.0;
                    for (int m = 0; m < mTable["Affil"].Cols; ++m)
                    {
                        sum += Math.Pow(mTable["Affil"][i, m] - mTable["Affil"][j, m], 2);
                    }
                    mTable["AffilEuclidean"][i, j] = 1.0 - sum / kmax;
                }
            }


            mTable["AffilEuclideanEvent"] = new Matrix(mTable["Affil"].Cols);
            mTable["AffilEuclideanEvent"].RowLabels.CopyFrom(mTable["Affil"].ColLabels);
            mTable["AffilEuclideanEvent"].ColLabels.CopyFrom(mTable["Affil"].ColLabels);
            double nmax = mTable["Affil"].Rows * Math.Pow(Algorithms.MaxValue<double>(mTable["Affil"]) - Algorithms.MinValue<double>(mTable["Affil"]), 2);
            for (int i = 0; i < mTable["Affil"].Cols; ++i)
            {
                for (int j = 0; j < mTable["Affil"].Cols; ++j)
                {
                    double sum = 0.0;
                    for (int m = 0; m < mTable["Affil"].Rows; ++m)
                    {
                        sum += Math.Pow(mTable["Affil"][m, i] - mTable["Affil"][m, j], 2);
                    }
                    mTable["AffilEuclideanEvent"][i, j] = 1.0 - sum / nmax;
                }
            }

            // Make a global copy of each of the matrices to be used for
            // clearPreviousData()

            tempDataMatrix = new Matrix(mTable["Data"]);
            tempDataEventMatrix = new Matrix(mTable["DataEvent"]);
            tempAffilCorrelationMatrix = new Matrix(mTable["AffilCorrelation"]);
            tempAffilCorrelationEventMatrix = new Matrix(mTable["AffilCorrelationEvent"]);
            tempAffilEuclideanMatrix = new Matrix(mTable["AffilEuclidean"]);
            tempAffilEuclideanEventMatrix = new Matrix(mTable["AffilEuclideanEvent"]);

            return mTable["Affil"].NetworkId;
        }

        public int CountVarsInMultivariableDyadicFile(string filename)
        {
            return BufferedFileTable.GetFile(filename).CountVarsInDyadicFile();
        }

        // Loads elementwise multiplication matrix
        // Assumes a data matrix has been loaded
        public void LoadElementwiseMultiplication(string filename, int year, string m, ElementwiseFormat ef)
        {
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required before elementwise multiplication is valid!");

            if (ef == ElementwiseFormat.Matrix || ef == ElementwiseFormat.Dyadic)
            {
                mTable["Temp"] = MatrixReader.ReadMatrixFromFile(filename, year);
            }
            else
            {
                LoadFromMonadicFileIntoMatrix(filename, year, "Temp");
            }

            mTable[m] = new Matrix(mTable["Temp"].Rows, mTable["Temp"].Cols);

            mTable[m].CopyLabelsFrom(mTable["Data"]);

            if (ef != ElementwiseFormat.Monadic)
            {
                for (int i = 0; i < mTable[m].Rows; ++i)
                    for (int j = 0; j < mTable[m].Cols; ++j)
                        mTable[m][i, j] = mTable["Data"][i, j] * mTable["Temp"][i, j];
            }
            else
            {
                for (int i = 0; i < mTable[m].Rows; ++i)
                    for (int j = 0; j < mTable[m].Cols; ++j)
                        mTable[m][i, j] = mTable["Data"][i, j] * mTable["Temp"][i, i];
            }
        }

        // Loads binary complement matrix
        // Assumes a data matrix has been loaded
        public void LoadBinaryComplement(string M) // M is source matrix
        {
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required before binary complement is valid!");

            mTable[M] = new Matrix(mTable["Data"]);

            for (int i = 0; i < mTable[M].Rows; ++i)
            {
                for (int j = 0; j < mTable[M].Cols; ++j)
                {
                    if (mTable[M][i, j] > 0.0)
                        mTable[M][i, j] = 0;
                    else
                        mTable[M][i, j] = 1;
                }
            }
        }     
        

        // DEPRECATED

        /* 
         * Loads Clique Affiliation Matrix
         * Assumes a data matrix has been loaded
         */
        public void LoadCliqueAffiliationMatrix(string M)
        {
            string m = "Temp";
            if (mTable[M] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            if (!mTable[M].IsSquareMatrix)
            {
                
                Dictionary<string, Matrix>.ValueCollection values = mTable.Values;
                

                // throw Exception
            }

            mTable[m] = new Matrix(mTable[M]); // This is the previous matrix to be converted to CAmatrix

            int size = mTable[m].Rows;
            List<Set> cliqueSets = new List<Set>();
            List<Set> adjacentVertices = new List<Set>(size);

            for (int i = 0; i < size; i++)
            {
                Set tempSet = new Set();
                for (int j = 0; j < size; j++)
                {
                    if (mTable[m][i, j] == 1)
                    {
                        tempSet.insert(j + 1);
                    }
                }
                adjacentVertices.Add(tempSet);
            }

            Set R = new Set();
            Set P = new Set();
            Set X = new Set();

            for (int i = 0; i < size; i++)
                P.insert(i + 1);

            // may need to may a call by reference for cliqueSets
            Bronkerbosch.findMaximalCliques(R, P, X, ref cliqueSets, adjacentVertices, size);

            Matrix tempMatrix = new Matrix(size, cliqueSets.Count);
            for (int i = 0; i < cliqueSets.Count; i++)
            {
                int[] column = new int[size];
                for (int j = 0; j < size; j++)
                {
                    column[j] = cliqueSets[i][j];
                }
                Vector v = new Vector(column);
                
                tempMatrix.SetColVector(i, v);
            }
            
            
            /*
             * sort the cliques
             */

            //mTable["CliqueAffiliation"] = mTable["Temp"];
            mTable["CliqueAffiliation"] = tempMatrix;
        }

        // DEPRECATED
        public void LoadJointCliqueAffiliationMatrix(string M)
        {
            string m = "Temp";
            if (mTable[M] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            /*
             * need code to get the multiple matrices
             */
            int count = 0;
            foreach (KeyValuePair<string, Matrix> keyValuepair in mTable)
            {
                count++;
                //Console.WriteLine(count);
            }

            
            mTable[m] = new Matrix(mTable[M]); // This is the previous matrix to be converted to CAmatrix

            Matrix blah = new Matrix(mTable[m].Rows, mTable[m].Cols);
            for (int i = 0; i < mTable[m].Rows; i++)
            {
                for (int j = 0; j < mTable[m].Cols; j++)
                {
                    blah[i, j] = count;
                }
            }
            mTable["JointCliqueAffiliation"] = blah;
        }

        // DEPRECATED
        public void formCliquesForJCAmatrix(ref Matrix JCAmatrix, int totalCliques)
        {
            int numOfMatrices = 3;
            Matrix[] tempMatrices = new Matrix[numOfMatrices];
            List<Vector> tempVectors = new List<Vector>();
            for (int i = 0; i < numOfMatrices; i++)
            {
                tempMatrices[i] = new Matrix(7, 7);
            }
            for (int i = 0; i < numOfMatrices; i++)
            {
                for (int j = 0; j < tempMatrices[i].Cols; j++)
                {
                    tempVectors.Add(tempMatrices[i].GetColVector(j));
                }
            }
            createJCAmatrix(ref JCAmatrix, tempVectors, totalCliques);
        }

        // DEPRECATED
        public void createJCAmatrix(ref Matrix JCAmatrix, List<Vector> tempVectors, int totalCliques)
        {
            int[] net = new int[totalCliques];
            for (int i = 0; i < totalCliques; i++)
                net[i] = 1;

            for (int i = 0; i < totalCliques; i++)
            {
                if (net[i] == 0)
                    continue;
                for (int j = i + 1; j < totalCliques; j++)
                {
                    if (Vector.isEqual(tempVectors[i], tempVectors[j]))
                    {
                        net[i] = net[i] + 1;
                        net[j] = 0;
                    }
                }
            }

            for (int i = 0; i < totalCliques; i++)
            {
                if (net[i] == 0)
                    continue;
                for (int j = i + 1; j < totalCliques; j++)
                {
                    if (net[j] == 0)
                        continue;
                    int notTaken = 0;
                    if (Vector.isSubset(tempVectors[i], tempVectors[j]))
                    {
                        int pos = Vector.getSubsetByPos(tempVectors[i], i, tempVectors[j], j, ref notTaken);
                        net[pos] = net[pos] + 1;
                        net[notTaken] = 0;
                    }
                }
            }

            List<Vector> CAvectors = new List<Vector>();
            List<int> netVector = new List<int>();
            for (int i = 0; i < totalCliques; i++)
            {
                if (net[i] != 0)
                {
                    CAvectors.Add(tempVectors[i]);
                    netVector.Add(net[i]);
                }
            }

            int rowSize = tempVectors[0].Size;
            JCAmatrix = new Matrix(rowSize + 1, CAvectors.Count);
            for (int i = 0; i < CAvectors.Count; i++)
            {
                Vector newVector = new Vector(rowSize + 1);
                for (int j = 0; j < rowSize; j++)
                    newVector[j] = CAvectors[i][j];
                newVector[rowSize] = netVector[i];
                JCAmatrix.SetColVector(i, newVector);
            }
            // sort cliques
        }

        // DEPRECATED
        public void LoadCliqueMembershipOverlapMatrix(string M)
        {
            string m = "Temp";
            if (mTable[M] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            /*
             * need code to get the multiple matrices
             */

            mTable[m] = new Matrix(mTable[M]); // This is the previous matrix to be converted to CAmatrix

            int totalCliques = 21; // temp variable
            Matrix JCAmatrix = new Matrix(7, totalCliques); // temp matrix
            formCliquesForJCAmatrix(ref JCAmatrix, totalCliques);

            // form CMOmatrix
            int rowSize = JCAmatrix.Rows - 1;
            int colSize = JCAmatrix.Cols;
            Matrix tempJCAmatrix = new Matrix(rowSize, colSize);
            Vector tempVector = new Vector(rowSize);
            for (int i = 0; i < colSize; i++)
            {
                for (int j = 0; j < rowSize; j++)
                    tempVector[j] = JCAmatrix.GetColVector(i)[j];
                tempJCAmatrix.SetColVector(i, tempVector);
            }
            Matrix CMOmatrix = JCAmatrix * JCAmatrix.GetTranspose();

            /*
             * display the CMOmatrix
             */

            mTable["CliqueMembershipOverlap"] = CMOmatrix;
        }

        // DEPRECATED
        public void LoadCliqueByCliqueOverlapMatrix(string M)
        {
            string m = "Temp";
            if (mTable[M] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            /*
             * need code to get the multiple matrices
             */

            mTable[m] = new Matrix(mTable[M]); // This is the previous matrix to be converted to CAmatrix

            int totalCliques = 21; // temp variable
            Matrix JCAmatrix = new Matrix(7, totalCliques); // temp matrix
            formCliquesForJCAmatrix(ref JCAmatrix, totalCliques);

            // form CMOmatrix
            int rowSize = JCAmatrix.Rows - 1;
            int colSize = JCAmatrix.Cols;
            Matrix tempJCAmatrix = new Matrix(rowSize, colSize);
            Vector tempVector = new Vector(rowSize);
            for (int i = 0; i < colSize; i++)
            {
                for (int j = 0; j < rowSize; j++)
                    tempVector[j] = JCAmatrix.GetColVector(i)[j];
                tempJCAmatrix.SetColVector(i, tempVector);
            }
            Matrix COmatrix = JCAmatrix.GetTranspose() * JCAmatrix;

            /*
             * display the COmatrix
             */

            mTable["CliqueByCliqueOverlap"] = COmatrix;
        }



        public int GetPreviousYear(string filename, int year)
        {
            return BufferedFileTable.GetFile(filename).GetPreviousNetworkId(year);
        }

        public int GetFirstYear(string filename)
        {
            return BufferedFileTable.GetFile(filename).MinNetworkId;
        }

        public int GetLastYear(string filename)
        {
            return BufferedFileTable.GetFile(filename).MaxNetworkId;
        } 

        public void LoadDensityVector(string filename)
        {
            if (densityVectorFile == filename)
                return;
            else
            {
                densityVector = new Dictionary<int, double>();
                densityVectorFile = filename;
            }

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                double val;
                int year;
                string[] parts = s.Split(',');
                if (double.TryParse(parts[1], out val) && int.TryParse(parts[0], out year))
                    densityVector[year] = val;
            }
        }

        public void LoadReachNumVector(string filename)
        { 
            if (reachNumVectorFile == filename)
                return;
            else
            {
                reachNumVector = new Dictionary<int, int>();
                reachNumVectorFile = filename;
            }
 
            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                int val;
                int year;
                string[] parts = s.Split(',');
                if (int.TryParse(parts[1], out val) && int.TryParse(parts[0], out year))
                    reachNumVector[year] = val;
            }  
        }

        public void LoadViableCutoffVector(string filename)
        {
            if (viableCoalitionVectorFile == filename)
                return;
            else
            {
                viableCoalitionVector = new Dictionary<int, double>();
                viableCoalitionVectorFile = filename;
            } 

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                double val;
                int year;
                string[] parts = s.Split(',');
                if (double.TryParse(parts[1], out val) && int.TryParse(parts[0], out year))
                    viableCoalitionVector[year] = val;
            }
        }

        public void LoadcliqueMinVector(string filename)
        {
            if (cliqueMinVectorFile == filename)
                return;
            else
            {
                cliqueMinVector = new Dictionary<int, int>();
                cliqueMinVectorFile = filename;
            }

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                int val;
                int year;
                string[] parts = s.Split(',');
                if (int.TryParse(parts[1], out val) && int.TryParse(parts[0], out year))
                    cliqueMinVector[year] = val;
            }
        }

        public void LoadKCliqueVector(string filename)
        {
            if (kCliqueVectorFile == filename)
                return;
            else
            {
                kCliqueVector = new Dictionary<int, int>();
                kCliqueVectorFile = filename;
            }

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                int val;
                int year;
                string[] parts = s.Split(',');
                if (int.TryParse(parts[1], out val) && int.TryParse(parts[0], out year))
                    kCliqueVector[year] = val;
            }
        }

        public void LoadweightVector(string filename)
        {
         if (weightVectorFile == filename)
                return;

            else
            {
                mcaweight.Clear();
                mcanetwork.Clear();
                mcayear.Clear();
            }

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                double val;
                int year;
                int networkid;
                string[] parts = s.Split(',');
                if (double.TryParse(parts[2], out val) && int.TryParse(parts[1], out networkid) && int.TryParse(parts[0], out year))
                {
                    mcaweight.Add(val);
                    mcanetwork.Add(networkid);
                    mcayear.Add(year);
                }
            }

            weightVectorFile = filename;
        }

        public void LoadattributeVector(string filename)
        {
            if (attributeVectorFile == filename)
                return;

            else
            {
                npnetwork.Clear();
                npnode.Clear();
                npattribute.Clear();
            }

            StreamReader sr = new StreamReader(filename);
            string s;

            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                double val;
                int node;
                int networkid;
                string[] parts = s.Split(',');
                if (double.TryParse(parts[2], out val) && int.TryParse(parts[1], out node) && int.TryParse(parts[0], out networkid))
                {
                    npattribute.Add(val);
                    npnetwork.Add(networkid);
                    npnode.Add(node);
                }
            }

            attributeVectorFile = filename;
        }



        

    }

    /*****************************************************************/

    // Implements the GUI output functions
    public class NetworkGUI : NetworkIO
    {

        public NetworkGUI()
            : base()
        {
        }

        public NetworkGUI(NetworkGUI viableSource)
            : base(viableSource)
        {
        }

        public void LoadMatrixIntoDataGridView(DataGridView data, string m)
        {
            if (m == "ClusterCharacteristics")
                m = "BlockCharacteristics";
            GUI.DataGridViewLoader.LoadMatrixIntoDataGridView(data, mTable[m]);

            if (m == "Dependency")
            {
                data.Rows[data.Rows.Count - 1].Height *= 2;
                data.Rows[data.Rows.Count - 2].Height *= 2;
                data.Rows[data.Rows.Count - 3].Height *= 2;
            }
        }

        public void LoadCentralityIntoDataGridView(DataGridView data, bool avg)
        {
            GUI.DataGridViewLoader.LoadMatrixIntoDataGridView(data, mTable["Centrality"]);

            for (int i = 0; i < mTable["Centrality"].Rows; ++i)
                data.Rows[i].Cells[1].Value = mTable["Centrality"].RowLabels[i];

            if (avg)
            {
                string[] newRow = new string[10];
                for (int col = 0; col < 10; ++col)
                {
                    double val = mTable["Centrality"].GetColAverage(col);
                    newRow[col] = val.ToString();
                }
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Average Centrality";
            }
        }

        public void LoadMCACounterIntoDataGridView(DataGridView data, MatrixTable matrixtable, List<clique> cliques, bool[] options, MatrixComputations.TransitivityType TT, List<Matrix> list, bool useweight/*, List<int>mcaYear, List<int>mcaNetworkid, List<double> mcaWeight*/)
        {


            data.Columns.Clear();

            MCA_Characteristics mcanc = new MCA_Characteristics(TT);

            for (int i = 0; i < options.Length; ++i)
                mcanc.SetLabel(i, options[i]);

            string[] labelParts = mcanc.Label.Split(',');
            for (int i = 0; i < labelParts.Length; ++i)
                data.Columns.Add(labelParts[i], labelParts[i]);

            mcanc.Calculate(matrixtable, cliques, list, mcayear, mcanetwork, mcaweight, useweight);


            data.Rows.Add(mcanc.MCALine);
        }


        public double calculateSignedDensity()
        {
            double sum = 0.0;
            for (int i = 0; i < mTable["Data"].Rows; i++)
                for (int j = 0; j < mTable["Data"].Cols; j++)
                    if (mTable["Data"][i, j] != 0)
                        sum += 1;

            // only need to check for the diagonals of the matrix
            for (int i = 0; i < mTable["Data"].Rows; i++)
                if (mTable["Data"][i, i] != 0)
                    return sum / (mTable["Data"].Rows * mTable["Data"].Rows);
            
            // if b(S_ii) == 0 for all x_ii that exist in S^1
            return sum / (mTable["Data"].Rows * (mTable["Data"].Rows - 1));
        }

        public double calculateSignedCyclicality()
        {
            double sum = 0.0;
            // only need to check for the diagonals of the matrix
            for (int i = 0; i < mTable["CognitiveReachability"].Rows; i++)
                if (mTable["CognitiveReachability"][i, i] != 0)
                    sum += 1;

            return sum / mTable["CognitiveReachability"].Rows;
        }

        public double calculateImbalanceCyclicality()
        {
            double sum = 0.0;
            // only need to check for the diagonals of the matrix
            for (int i = 0; i < mTable["CognitiveReachability"].Rows; i++)
                if (mTable["CognitiveReachability"][i, i] >= CognitiveAlgebra.NON_ZERO_RELATIONSHIP)
                    sum += 1;

            return sum / mTable["CognitiveReachability"].Rows;
        }

        public double calculateReciprocity_1()
        {
            double sum = 0.0;
            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                double innersum = 0.0;
                for (int j = 0; j < mTable["Data"].Cols; j++)
                {
                    if ((mTable["Data"][i, j] == mTable["Data"][j, i]) &&
                        (mTable["Data"][i, j] != 0) && (mTable["Data"][j, i] != 0))
                        innersum += 1;
                }
                double r;
                if (mTable["Data"][i, i] == 0)
                    r = 0;
                else
                    r = 1;
                sum += innersum - r;        
            }
            return sum / (mTable["Data"].Rows * (mTable["Data"].Rows - 1));
            //return (2 * sum) / (mTable["Data"].Rows * (mTable["Data"].Rows - 1));
        }

        public double calculateReciprocity_R()
        {
            double sum = 0.0;
            for (int i = 0; i < mTable["CognitiveReachability"].Rows; i++)
            {
                double innersum = 0.0;
                for (int j = 0; j < mTable["CognitiveReachability"].Cols; j++)
                {
                    if ((mTable["CognitiveReachability"][i, j] == mTable["CognitiveReachability"][j, i]) &&
                        (mTable["CognitiveReachability"][i, j] != 0) && (mTable["CognitiveReachability"][j, i] != 0))
                        innersum += 1;
                }
                double r = 0.0;
                if (mTable["CognitiveReachability"][i, i] == 0)
                    r = 0;
                else
                    r = 1;
                sum += innersum - r;
            }
            //return (2 * sum) / (mTable["CognitiveReachability"].Rows * (mTable["CognitiveReachability"].Rows - 1));
            return sum / (mTable["CognitiveReachability"].Rows * (mTable["CognitiveReachability"].Rows - 1));
        }

        public double calculateImbalance()
        {
            double sum = 0.0;
            for (int i = 0; i < mTable["CognitiveReachability"].Rows; i++)
                for (int j = 0; j < mTable["CognitiveReachability"].Cols; j++)
                    if (mTable["CognitiveReachability"][i, j] == CognitiveAlgebra.NON_ZERO_RELATIONSHIP)
                        sum += 1;

            return sum / (mTable["CognitiveReachability"].Rows * mTable["CognitiveReachability"].Rows);
        }

        public double calculateTransitiveBalance()
        {
            double balanceTriads = 0.0;
            double transitiveTriads = 0.0;
            Triads TB = new Triads(mTable["Data"], Triads.TriadType.Balance);

            for (int i = 0; i < TB.ColCount; i++)
            {
                for (int j = 0; j < TB.getTriadListCount(i); j++)
                {
                    int I = TB[i][j][0];
                    int J = TB[i][j][1];
                    int K = TB[i][j][2];

                    /*
                    if ((I == -2) || (J == -2) || (K == -2))
                        break;
                    */
                    // count how many negative diads
                    double negativeCount = 0.0;
                    if (mTable["Data"][I, J] == -1)
                        negativeCount++;
                    if (mTable["Data"][J, K] == -1)
                        negativeCount++;
                    if (mTable["Data"][I, K] == -1)
                        negativeCount++;

                    if (negativeCount == 0 || negativeCount == 2) // if positive
                        balanceTriads++;

                    transitiveTriads++;
                }
            }
            if (transitiveTriads == 0)
                return 0.0;
            else
                return balanceTriads / transitiveTriads;
        }

        public void LoadSignedNetworkCharacteristics(DataGridView data, int n, bool doSum, bool zeroDiagonal, string m, int year, bool reachBinary)
        {
            data.Columns.Clear();
            string[] labelParts = { "Network ID", "N", "Density", "Cyclicality", "Imbalanced Cyclicality",
                                    "Reciprocity 1", "Reciprocity R", "Imbalance", "Transitive Balance" };
            foreach (string parts in labelParts)
                data.Columns.Add(parts, parts);

            mTable["SignedNetwork"] = new Matrix(1, labelParts.Length);
            for (int col = 0; col < labelParts.Length; col++)
            {
                mTable["SignedNetwork"].ColLabels[col] = labelParts[col];
            }

            //if (mTable["CognitiveReachability"] == null)
            LoadCognitiveReachability(n, doSum, zeroDiagonal, m, year, reachBinary);
            int position = 0;

            // get the Network ID of the matrix
            mTable["SignedNetwork"][0, position++] = year;
            //mTable["SignedNetwork"][0, position++] = mTable["Data"].NetworkId;

            // get the number of Nodes N of the matrix
            mTable["SignedNetwork"][0, position++] = mTable["Data"].Rows;

            // get the density of matrix S^1
            mTable["SignedNetwork"][0, position++] = calculateSignedDensity();

            // get the Cyclicality (CY) of Reachability matrix R
            mTable["SignedNetwork"][0, position++] = calculateSignedCyclicality();

            // get the Imbalance Cyclicality (ICY) of Reachability matrix R
            mTable["SignedNetwork"][0, position++] = calculateImbalanceCyclicality();            

            // get the Reciprocity 1 of the data matrix S^1
            mTable["SignedNetwork"][0, position++] = calculateReciprocity_1();

            // get the Reciprocity R of the Reachability matrix R
            mTable["SignedNetwork"][0, position++] = calculateReciprocity_R();

            // get the Imbalance of the Reachability matrix R
            mTable["SignedNetwork"][0, position++] = calculateImbalance();

            // get the Transitive Balance of the Sociomatrix S^1
            mTable["SignedNetwork"][0, position++] = calculateTransitiveBalance();

            string[] labels = new string[labelParts.Length];
            for (int col = 0; col < labels.Length; col++)
                labels[col] = mTable["SignedNetwork"][0, col].ToString();
            data.Rows.Add(labels);
            data.Rows[0].HeaderCell.Value = year;
        }


        public void LoadCounterIntoDataGridView(DataGridView data, int year, double cutoff, double density, string inputType, string externalFile, string svcFile,
            bool useCohesion, MatrixComputations.TransitivityType transitivityType, bool[] options, bool reachSum, bool zeroDiagonal, int reachMatrixCount, string erpolType, double alpha, int cliqueOrder,
            bool kCliqueDiag, string selectionMethod, int _maxStepSize)
        {
            data.Columns.Clear();

            if (mTable["Data"] != null)
                density = GetRealDensity(density, year, "Data");

            NetworkCharacteristics nc = new NetworkCharacteristics(this, year, inputType, externalFile, svcFile,
                transitivityType, cutoff, density, zeroDiagonal, reachMatrixCount, erpolType, alpha, cliqueOrder, kCliqueDiag);

            for (int i = 0; i < options.Length; ++i)
                nc.SetLabel(i, options[i]);

            string[] labelParts = nc.Label.Split(',');
            mTable["Counter"] = new Matrix(1, labelParts.Length);
            for (int i = 0; i < labelParts.Length; ++i)
            {
                mTable["Counter"].ColLabels[i] = labelParts[i];
                data.Columns.Add(labelParts[i], labelParts[i]);
            }

            if (mTable["Data"] == null)
            {
                string[] newLine = new string[data.Columns.Count];
                Algorithms.Fill<string>(newLine, "NA");
                newLine[0] = year.ToString();
                newLine[1] = newLine[2] = "0";
                data.Rows.Add(newLine);
                data.Rows[0].HeaderCell.Value = year;
                return;
            }

            if (inputType == "StructEquiv")
                LoadStructEquiv(density, year, "Data");

            bool loadCOC = false;
            if (options[10] || options[11] || options[14] || options[18] || options[24])
                loadCOC = true;
            if (options[2] || options[7] || options[6] || options[5] || options[4] || options[3] ||
                options[8] || options[9] || options[11] || options[10] || options[14] || options[15] ||
                options[17] || options[16] || options[18] || options[24])
            {
                if (selectionMethod == "Cliq")
                    FindCliques(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, loadCOC, cliqueOrder, kCliqueDiag);
                else if (selectionMethod == "Bloc")
                {
                    FindBlocks(cutoff, Blocks,_minCliqueSize);
                    _blockDensity = mTable["DensityBlockMatrix"];
                }
                else if (selectionMethod == "Clus")
                {
                    FindBlocks(cutoff, Blocks, _minCliqueSize);
                    _blockDensity = mTable["DensityBlockMatrix"];
                }
                else if (selectionMethod == "Comm" || selectionMethod == "NewDisc" || selectionMethod == "NewOv")
                {
                    FindComm(cutoff, communities, _minCliqueSize);
                    //FindComm();
                }


            }
            //LoadReachability(reachMatrixCount, reachSum, zeroDiagonal, "Data");

            bool useCliq = false;
            if (selectionMethod == "Cliq")
                useCliq = true;

            if (options[23])
                LoadReachability(reachMatrixCount, reachSum, zeroDiagonal, "Data", year, false);
            if (options[5] || options[6])
            {
                /*
                if (selectionMethod == "Cliq")
                    LoadComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
                else if (selectionMethod == "Bloc" || selectionMethod == "Clus")
                    LoadbComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
                else if (selectionMethod == "Comm")
                    LoadcomComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
                */
                LoadComponents(cutoff, inputType == "StructEquiv", density, year, _minCliqueSize, reachMatrixCount, reachSum, zeroDiagonal, useCliq);
            }
            if (options[19] || options[20] || options[23])
                LoadDependency("Data", reachMatrixCount, density, year, zeroDiagonal, reachSum);
            
            if (options[25] || options[26] || options[27] || options[28] || options[29] || options[30] || options[31] || options[32] || options[33] || options[34])
                LoadCentralityIndices("Data", year, 1, false, false);

            nc.selMeth = selectionMethod;
            string[] temp = nc.Line; // more efficient since don't have to call all the functions from nc.Line
            for (int i = 0; i < mTable["Counter"].Cols; i++)
                mTable["Counter"][0, i] = Convert.ToDouble(temp[i]);
            data.Rows.Add(temp);
            data.Rows[0].HeaderCell.Value = year;
            nc = null;
            _cliques = null;
            _blocks = null;
            _blockDensity = null;
            _communities = null;
            commDensity = null;
            
           
        }



        public void LoadBlocAffiliationIntoDataGridView(DataGridView data, bool cluster)
        {
            if (Blocks == null)
                throw new Exception("Must run CONCOR before loading block afilliation");

            data.Columns.Clear();

            for (int i = 0; i < Blocks.Count; ++i)
                data.Columns.Add((i + 1).ToString(), cluster ? "Cluster " + (i + 1).ToString() : "Block " + (i + 1).ToString());

            // Now add the rows
            for (int row = 0; row < mTable["Data"].Rows; ++row)
            {
                string[] newRow = new string[Blocks.Count];
                for (int col = 0; col < Blocks.Count; ++col)
                    if (Blocks[col].Contains(row))
                        newRow[col] = "1";
                    else
                        newRow[col] = "0";
                data.Rows.Add(newRow);
                data.Rows[row].HeaderCell.Value = mTable["Data"].ColLabels[row];
            }
        }


        public void LoadAffiliationIntoDataGridView(DataGridView data, double cutoff, bool useStructEquiv, string fileName, bool dyadic, int year,
            double maxik, bool useCohesion, string sumMean, string sumMeanFilename, string SF, string displayclique,
            bool useCliqueSize, bool useCliqueCohesion, bool useER, int cliqueOrder, bool kCliqueDiag)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");
            if (_cliques.Count == 0)
                return;

            // Load cohesion
            if (useCohesion)
                NPOLStar(useStructEquiv, fileName, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag);

            // if using size
            if (SF != null)
            {
                NPOLA(year, SF, false, cutoff, density, cliqueOrder, kCliqueDiag);
            }
            data.Columns.Clear();

            float fillweight = 0.0f;
            if (displayclique == "Display500")
                fillweight = 65536f / 500;
            else
                fillweight = 1.0f;

            // Loads the matrix into the provided DataGridView
            try
            {
                for (int i = 0; i < _cliques.Count; i++)
                {
                    // Generate labels sequentially
                    data.Columns.Add((i + 1).ToString(), (i + 1).ToString());
                    data.Columns[i].FillWeight = fillweight;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("More than 500 cliques! Try to change display clique option or save file to view for faster access. ", "Error!");
            }

            // Now add the rows
            for (int row = 0; row < mTable["Data"].Rows; row++)
            {
                // Generate an array to hold this row
                string[] newRow = new string[_cliques.Count];
                for (int col = 0; col < _cliques.Count; col++)
                    newRow[col] = _cliques[col].IntContains(row).ToString();

                data.Rows.Add(newRow);

                data.Rows[row].HeaderCell.Value = mTable["Data"].RowLabels[row];
            }


            // Add the clique "size" row
            if (SF != null && useCliqueSize)
            {
                NPOLA(year, SF, useCohesion, cutoff, maxik, cliqueOrder, kCliqueDiag);
                {
                    string[] newRow = new string[_cliques.Count];
                    for (int col = 0; col < _cliques.Count; ++col)
                        newRow[col] = NPOLA_Sums[col].ToString();
                    data.Rows.Add(newRow);
                    data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Size";
                }
            }


            // add the weighted size row
            if (SF != null && useCohesion && useCliqueCohesion)
            {
                string[] newRow = new string[_cliques.Count];
                for (int col = 0; col < _cliques.Count; ++col)
                    newRow[col] = (NPOLA_Sums[col] * _cliques[col].Cohesion).ToString();
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Weighted Size";
            }

            if (useCliqueSize)  //combine cliquesize option with summean option
            {
                // Add the last DVC/SVC Sum/Mean row
                if (sumMean != "None")
                {
                    double[] rowVals = new double[_cliques.Count];
                    if (sumMean.Contains("DVC"))
                    {
                        LoadFromMatrixFileIntoMatrix(sumMeanFilename, year, "D");

                        // Now generate clique dyads for each clique
                        for (int i = 0; i < _cliques.Count; ++i)
                        {
                            Vector Q = new Vector(_cliques[i].MemberSet);
                            Matrix Dyads = (Matrix)Q.GetTranspose() * Q;

                            // Multiply elementwise and SUM
                            for (int j = 0; j < Dyads.Rows; ++j)
                                for (int k = 0; k < Dyads.Cols; ++k)
                                    rowVals[i] += Dyads[j, k] * mTable["D"][j, k];

                            // Should we do the mean?
                            if (sumMean == "DVCMean")
                                rowVals[i] /= Dyads.Rows * Dyads.Cols;
                        }
                    }
                    else // SVC
                    {
                        BufferedFileTable.GetFile(sumMeanFilename).JumpToNetworkId(year);
                        BufferedFileTable.GetFile(sumMeanFilename).JumpToNetworkId(year);

                        // Now read in the lines and generate av
                        Matrix av = new Matrix(mTable["Data"].Rows, 1);
                        for (int j = 0; j < mTable["Data"].Rows; ++j)
                        {
                            string s = BufferedFileTable.GetFile(sumMeanFilename).ReadLine();
                            string[] parts = s.Split(',');
                            av[j, 0] = double.Parse(parts[2]);
                        }

                        for (int j = 0; j < _cliques.Count; ++j)
                        {
                            for (int k = 0; k < mTable["Data"].Rows; ++k)
                            {
                                rowVals[j] += av[k, 0] * _cliques[j].IntContains(k);
                            }
                            if (sumMean == "SVCMean")
                                rowVals[j] /= mTable["Data"].Rows;
                        }

                    }
                    string[] newr = new string[_cliques.Count];
                    for (int i = 0; i < _cliques.Count; ++i)
                        newr[i] = rowVals[i].ToString();
                    data.Rows.Add(newr);
                    data.Rows[data.Rows.Count - 1].HeaderCell.Value = sumMean;
                }
            }

            // Add the last (cohesion) row
            if (useCohesion && useCliqueCohesion)
            {
                string[] newr = new string[_cliques.Count];
                for (int col = 0; col < _cliques.Count; ++col)
                    newr[col] = _cliques[col].Cohesion.ToString();

                data.Rows.Add(newr);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Cohesion";
            }

            // add the ER1 row
            if (useER)
            {
                string[] newRow = new string[_cliques.Count];
                for (int col = 0; col < _cliques.Count; ++col)
                {
                    double val = ComputeER1(useCohesion, SF != null, col);
                    newRow[col] = val.ToString();
                }
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "ER1";
            }


        }


        public void LoadComponentsIntoDataGridView(DataGridView data, double cutoff, bool useStructEquiv, string fileName, bool dyadic, int year,
          double maxik, bool useCohesion, string sumMean, string sumMeanFilename, string SF,
          bool useCliqueSize, bool useCliqueCohesion, bool useER, int cliqueOrder, bool kCliqueDiag)
        {
            if (comcliques == null)
                throw new Exception("No cliques have been found!");
            if (comcliques.Count == 0)
                return;

            // Load cohesion
            if (useCohesion)
                NPOLStar(useStructEquiv, fileName, cutoff, year, maxik, false, cliqueOrder, kCliqueDiag);

            data.Columns.Clear();

            // Loads the matrix into the provided DataGridView
            for (int i = 0; i < comcliques.Count; i++)
            {
                // Generate labels sequentially
                data.Columns.Add((i + 1).ToString(), (i + 1).ToString());
            }

            // Now add the rows
            for (int row = 0; row < mTable["Data"].Rows; row++)
            {
                // Generate an array to hold this row
                string[] newRow = new string[comcliques.Count];
                for (int col = 0; col < comcliques.Count; col++)
                    newRow[col] = comcliques[col].IntContains(row).ToString();

                data.Rows.Add(newRow);

                data.Rows[row].HeaderCell.Value = mTable["Data"].RowLabels[row];
            }

            // Add the clique "size" row
            if (SF != null && useCliqueSize)
            {
                NPOLA(year, SF, useCohesion, cutoff, maxik, cliqueOrder, kCliqueDiag);
                string[] newRow = new string[comcliques.Count];
                for (int col = 0; col < comcliques.Count; ++col)
                    newRow[col] = NPOLA_Sums[col].ToString();
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Size";
            }

            // Add the last (cohesion) row
            if (useCohesion && useCliqueCohesion)
            {
                string[] newr = new string[comcliques.Count];
                for (int col = 0; col < comcliques.Count; ++col)
                    newr[col] = comcliques[col].Cohesion.ToString();

                data.Rows.Add(newr);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Cohesion";
            }

            // add the weighted size row
            if (SF != null && useCohesion && useCliqueCohesion)
            {
                string[] newRow = new string[comcliques.Count];
                for (int col = 0; col < comcliques.Count; ++col)
                    newRow[col] = (NPOLA_Sums[col] * comcliques[col].Cohesion).ToString();
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "Weighted Size";
            }

            // Add the last DVC/SVC Sum/Mean row
            if (sumMean != "None")
            {
                double[] rowVals = new double[comcliques.Count];
                if (sumMean.Contains("DVC"))
                {
                    LoadFromMatrixFileIntoMatrix(sumMeanFilename, year, "D");

                    // Now generate clique dyads for each clique
                    for (int i = 0; i < _cliques.Count; ++i)
                    {
                        Vector Q = new Vector(_cliques[i].MemberSet);
                        Matrix Dyads = (Matrix)Q.GetTranspose() * Q;

                        // Multiply elementwise and SUM
                        for (int j = 0; j < Dyads.Rows; ++j)
                            for (int k = 0; k < Dyads.Cols; ++k)
                                rowVals[i] += Dyads[j, k] * mTable["D"][j, k];

                        // Should we do the mean?
                        if (sumMean == "DVCMean")
                            rowVals[i] /= Dyads.Rows * Dyads.Cols;
                    }
                }
                else // SVC
                {
                    BufferedFileTable.GetFile(sumMeanFilename).JumpToNetworkId(year);
                    BufferedFileTable.GetFile(sumMeanFilename).JumpToNetworkId(year);

                    // Now read in the lines and generate av
                    Matrix av = new Matrix(mTable["Data"].Rows, 1);
                    for (int j = 0; j < mTable["Data"].Rows; ++j)
                    {
                        string s = BufferedFileTable.GetFile(sumMeanFilename).ReadLine();
                        string[] parts = s.Split(',');
                        av[j, 0] = double.Parse(parts[2]);
                    }

                    for (int j = 0; j < comcliques.Count; ++j)
                    {
                        for (int k = 0; k < mTable["Data"].Rows; ++k)
                        {
                            rowVals[j] += av[k, 0] * comcliques[j].IntContains(k);
                        }
                        if (sumMean == "SVCMean")
                            rowVals[j] /= mTable["Data"].Rows;
                    }

                }
                string[] newr = new string[comcliques.Count];
                for (int i = 0; i < comcliques.Count; ++i)
                    newr[i] = rowVals[i].ToString();
                data.Rows.Add(newr);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = sumMean;
            }

            // add the ER1 row
            if (useER)
            {
                string[] newRow = new string[comcliques.Count];
                for (int col = 0; col < comcliques.Count; ++col)
                {
                    double val = ComputecomER1(useCohesion, SF != null, col);
                    newRow[col] = val.ToString();
                }
                data.Rows.Add(newRow);
                data.Rows[data.Rows.Count - 1].HeaderCell.Value = "ER1";
            }

        }

        public void LoadCBCOverlapIntoDataGridView(DataGridView data, string displayclique)
        {
            LoadCBCOverlapIntoDataGridView(data, false, displayclique);
        }
        public void LoadCBCOverlapIntoDataGridView(DataGridView data, bool stdDiag, string displayclique)
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found!");

            data.Columns.Clear();
            // Loads the matrix into the provided DataGridView

            float fillweight = 0.0f;
            if (displayclique == "Display500")
                fillweight = 65536f / 500;
            else
                fillweight = 1.0f;

            int temp = _cliques.Count;
            try
            {
                for (int i = 0; i < temp; ++i)
                {
                    data.Columns.Add((i + 1).ToString(), (i + 1).ToString());
                    data.Columns[i].FillWeight = fillweight;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("More than 500 cliques! Try to change display clique option or save file to view for faster access. ", "Error!");
                temp = 500;
            }
            // Now add the rows
            for (int row = 0; row < temp; row++)
            {
                // Generate an array to hold this row
                string[] newRow = new string[temp];

                double r = 1.0;
                if (stdDiag)
                    r = _cliques.GetCliqueByCliqueOverlap(row, row);
                for (int col = 0; col < temp; col++)
                    newRow[col] = (_cliques.GetCliqueByCliqueOverlap(row, col) / r).ToString();

                data.Rows.Add(newRow);

                data.Rows[row].HeaderCell.Value = data.Columns[row].HeaderCell.Value;
            }
        }

        public void LoadNationalDependencyIntoDataGridView(int reachMatrixCount, double density, int year, bool zeroDiagonal, bool reachSum)
        {
            LoadUnitDependency(year);       
        }

        public void LoadDistanceMatrix(double cutoff)
        {
            mTable["Distance"] = new Matrix(mTable["Data"]);
            Algorithms.Fill<double>(mTable["Distance"], 0.0);
            Matrix prod = new Matrix(mTable["Data"]);

            for (int n = 1; n < mTable["Data"].Rows; ++n)
            {
                for (int i = 0; i < mTable["Distance"].Rows; ++i)
                {
                    for (int j = 0; j < mTable["Distance"].Cols; ++j)
                    {
                        if (mTable["Distance"][i, j] == 0.0 && prod[i, j] > cutoff)
                            mTable["Distance"][i, j] = n;
                    }
                }

                prod *= mTable["Data"];
            }
        }

        public List<int[]> copyCommunities(List<int[]> communities)
        {
            List<int[]> templist = new List<int[]>();
            for (int i = 0; i < communities.Count; i++)
            {
                int[] temparray = new int[communities[i].Length];
                for (int j = 0; j < communities[i].Length; j++)
                {
                    temparray[j] = communities[i][j];
                }
                templist.Add(temparray);
            }
            return templist;
        }




        public void calculateCommunities(DataGridView data, CommunityType commType, int year, List<UnordererdPair<string, NetworkIO.CharacteristicType>> svc,
            List<UnordererdPair<string, NetworkIO.CharacteristicType>> dvc, List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix, double cutoff, double density)
        {
            bool newDiscrete = (commType == CommunityType.newAffil || commType == CommunityType.newChar || commType == CommunityType.newCluster ||
                commType == CommunityType.newCohesion || commType == CommunityType.newDensity || commType == CommunityType.newDensity || commType == CommunityType.newCoefficients);
            maxSC = -1;
            bool overlapping = (commType == CommunityType.ovAffil || commType == CommunityType.ovCoefficients || commType == CommunityType.ovCohesion || commType == CommunityType.ovDensity || commType == CommunityType.ovRelativeDensity);
            if (commType == CommunityType.Coefficients || commType == CommunityType.newCoefficients || commType == CommunityType.ovCoefficients)
            {
                maxcomm_GC_list = new List<double>();
                maxcomm_SC_list = new List<double>();
                maxcomm_SE_coh_list = new List<double>();
                maxcomm_SE_den_list = new List<double>();
                maxcomm_T_coh_list = new List<double>();
                maxcomm_T_den_list = new List<double>();
                maxcomm_null_SC_list = new List<double>();
                maxcomm_null_GC_list = new List<double>();
                maxcomm_T_SC_list = new List<double>();
                maxcomm_T_GC_list = new List<double>();
                comm_SE_coh_list = new List<double>();
                comm_SE_den_list = new List<double>();
                comm_T_coh_list = new List<double>();
                comm_T_den_list = new List<double>();
                comm_null_SC_list = new List<double>();
                comm_null_GC_list = new List<double>();
                comm_T_SC_list = new List<double>();
                comm_T_GC_list = new List<double>();

            }
            LoadStructEquiv(density, year, "Data");
            comm_SC_list = new List<double>();
            comm_GC_list = new List<double>();
            comm_null_SC_list = new List<double>();
            comm_null_GC_list = new List<double>();
            comm_T_SC_list = new List<double>();
            comm_T_GC_list = new List<double>();

            convertMatrixToProperInput();
            mainCommunityExtractionMethod(commType == CommunityType.newCoefficients || commType == CommunityType.Coefficients || commType == CommunityType.ovCoefficients, newDiscrete, overlapping);
            createCommunitiesFromGroup();
            communities.Sort((x, y) =>
            {
                int x_count = 0;
                int y_count = 0;
                for (int i = 0; i < x.Length; i++)
                    x_count += x[i];
                for (int i = 0; i < y.Length; i++)
                    y_count += y[i];
                return y_count.CompareTo(x_count);
            });
            bool done = false;
            double prevSeparation = CalculateCommSC(communities);
            List<int[]> prevCommunities = communities;
            while(overlapping && !done && prevSeparation != 0)
            {
                    Matrix densityMatrix = CalculateCommDensity(communities);
                    prevSeparation = CalculateCommSC(communities);
                    prevCommunities = copyCommunities(communities);
                    List<int> partitionedNodeList = new List<int>();
                    int index = 0;
                    int i, j;

                    partitionedNodeList.Capacity = mTable["Data"].Rows;
                    for (i = 0; i < communities.Count; i++)
                    {
                        for (j = 0; j < communities[i].Length; j++)
                        {
                            if (communities[i][j] == 1)
                            {
                                partitionedNodeList.Insert(index, j);
                                index++;
                            }
                        }
                    }
                    Matrix partitionedMatrix = new Matrix(partitionedNodeList.Count, partitionedNodeList.Count);
                    for (i = 0; i < partitionedNodeList.Count; i++)
                    {
                        for (j = 0; j < partitionedNodeList.Count; j++)
                        {
                            partitionedMatrix[i, j] = mTable["Data"][partitionedNodeList[i], partitionedNodeList[j]];
                        }
                    }
                    List<int> communitysizes = new List<int>();
                    List<int> commcoords = new List<int>();
                    for (i = 0; i < communities.Count; i++)
                    {
                        int sum = 0;
                        for (j = 0; j < communities[i].Length; j++)
                        {
                            if (communities[i][j] == 1)
                                sum++;
                        }
                        communitysizes.Add(sum);
                    }
                    for (i = 0; i < communities.Count; i++)
                    {
                        if (i > 0)
                        {
                            int sum = 0;
                            for (j = i - 1; j >= 0; j--)
                            {
                                sum += communitysizes[j];
                            }
                            commcoords.Add(sum);
                        }
                        else
                        {
                            commcoords.Add(0);
                        }
                    }
                    int k, l;
                    List<List<double>> indensities = new List<List<double>>();
                    List<List<double>> outdensities = new List<List<double>>();
                    indensities.Capacity = communities.Count;
                    outdensities.Capacity = communities.Count;
                    double insum, outsum;
                    for (i = 0; i < commcoords.Count; i++)
                    {
                        List<double> commindensities = new List<double>();
                        List<double> commoutdensities = new List<double>();
                        for (k = 0; k < communities[i].Length; k++)
                        {
                            outsum = 0;
                            insum = 0;
                            for (l = commcoords[i]; l < commcoords[i] + communitysizes[i]; l++)
                            {
                                outsum += partitionedMatrix[k, l];
                                insum += partitionedMatrix[l, k];
                            }
                            bool contains = k < commcoords[i] + communitysizes[i] && k >= commcoords[i];
                            if (contains)
                            {
                                outsum /= communitysizes[i] - 1;
                                insum /= communitysizes[i] - 1;
                            }
                            else
                            {
                                outsum /= communitysizes[i];
                                insum /= communitysizes[i];
                            }
                            commoutdensities.Add(outsum);
                            commindensities.Add(insum);
                        }
                        outdensities.Add(commoutdensities);
                        indensities.Add(commindensities);
                    }
                    List<double> withinCommDen = new List<double>();
                    for (i = 0; i < communities.Count; i++)
                    {
                        withinCommDen.Add(densityMatrix[i, i]);
                    }
                    List<List<double>> densityDifferences = new List<List<double>>();
                    List<double> MCD = new List<double>();
                    for (i = 0; i < communities.Count; i++)
                    {
                        List<double> communityDensityNodeDifferences = new List<double>();
                        for (j = 0; j < communities[i].Length; j++)
                        {
                            double minimum = Math.Min(outdensities[i][j] - withinCommDen[i],indensities[i][j] - withinCommDen[i]);
                            communityDensityNodeDifferences.Add(minimum);
                        }
                        densityDifferences.Add(communityDensityNodeDifferences);                        
                    }
                    for (i = 0; i < communities[0].Length; i++)
                    {
                        double max = -999999999;

                        for (j = 0; j < communities.Count; j++)
                        {
                            if (communities[j][partitionedNodeList[i]] == 0)
                            {
                                max = Math.Max(max, densityDifferences[j][i]);
                            }
                        }
                        MCD.Add(max);                       
                    }
                    List<int> nodesToAdd = new List<int>();
                    for (i = 0; i < MCD.Count; i++)
                    {
                        int MCDComm = -1;
                        if (MCD[i] > 0)
                        {
                            for (j = 0; j < communities.Count; j++)
                            {
                                if (communities[j][partitionedNodeList[i]] == 0)
                                {
                                    if (densityDifferences[j][i] == MCD[i])
                                        MCDComm = j;
                                }
                            }
                            if(MCDComm != -1)
                            {
                                if (communities[MCDComm][partitionedNodeList[i]] == 0)
                                    communities[MCDComm][partitionedNodeList[i]] = 1;
                            }
                        }
                        
                    }
                    double separation = CalculateCommSC(communities);
                    if (separation <= prevSeparation)
                    {
                        done = true;
                        communities = prevCommunities;
                        continue;
                    }
            }         



            // Sort the communities using an anonymous function
            /*communities.Sort((x, y) => {
                int x_count = 0;
                int y_count = 0;
                for (int i = 0; i < x.Length; i++)
                    x_count += x[i];
                for (int i = 0; i < y.Length; i++)
                    y_count += y[i];
                return y_count.CompareTo(x_count);
            });*/

            double SE_den = 0;
            double SE_coh = 0;
            double T_den = 0;
            double T_coh = 0;
            double GC, SC;

            // for the final iteration 
            if (commType == CommunityType.Coefficients || commType == CommunityType.newCoefficients || commType == CommunityType.ovCoefficients)
            {
                LoadStructEquiv(density, year, "Data");
                if (newDiscrete && maxSeparationcommunities != null)
                {
                    if (maxSeparationcommunities.Count == mTable["Data"].Rows)
                    {
                        communities = new List<int[]>(maxSeparationcommunities);
                        comNum = maxSepComNum;
                    }
                    comm_SC_list = new List<double>(maxcomm_SC_list);
                    comm_GC_list = new List<double>(maxcomm_GC_list);
                    comm_SE_den_list = new List<double>(maxcomm_SE_den_list);
                    comm_SE_coh_list = new List<double>(maxcomm_SE_coh_list);
                    comm_T_coh_list = new List<double>(maxcomm_T_coh_list);
                    comm_T_den_list = new List<double>(maxcomm_T_den_list);
                    comm_null_SC_list = new List<double>(maxcomm_null_SC_list);
                    comm_null_GC_list = new List<double>(maxcomm_null_GC_list);
                    comm_T_SC_list = new List<double>(maxcomm_T_SC_list);
                    comm_T_GC_list = new List<double>(maxcomm_T_GC_list);
                }
                            
                SC = CalculateCommSC(communities);
                GC = CalculateCommGC();
                List<double> nullCoefficients = calculateNullCoefficients("Communities");
                if (!newDiscrete || comm_SC_list.Count == 0)
                {
                    comm_SC_list.Add(SC);
                    comm_GC_list.Add(GC);
                    comm_null_SC_list.Add(nullCoefficients[0]);
                    comm_T_SC_list.Add(nullCoefficients[1]);
                    comm_null_GC_list.Add(nullCoefficients[2]);
                    comm_T_GC_list.Add(nullCoefficients[3]);
                }
                else
                {
                    comm_SC_list[comm_SC_list.Count - 1] = SC;
                    comm_GC_list[comm_GC_list.Count - 1] = GC;
                    comm_null_SC_list[comm_null_SC_list.Count - 1] = nullCoefficients[0];
                    comm_null_GC_list[comm_null_GC_list.Count - 1] = nullCoefficients[2];
                    comm_T_SC_list[comm_T_SC_list.Count - 1] = nullCoefficients[1];
                    comm_T_GC_list[comm_T_GC_list.Count - 1] = nullCoefficients[3];

                }
     

                // Calculate the Separation (Density)
                Matrix D = CalculateCommDensity(communities);
                for (int i = 0; i < D.Rows; i++)
                    for (int j = 0; j < D.Cols; j++)
                        SE_den += Math.Pow((D[i, j] - D.GetRowAverage(i)), 2);
                SE_den = Math.Sqrt(SE_den / (D.Cols * D.Cols));
                comm_SE_den_list.Add(SE_den);

                // Calculate the Separation (Cohesion)
                Matrix C = computeCommCohesiveMatrix();
                for (int i = 0; i < C.Rows; i++)
                    for (int j = 0; j < C.Cols; j++)
                        SE_coh += Math.Pow((C[i, j] - C.GetRowAverage(i)), 2);
                SE_coh = Math.Sqrt(SE_coh / (C.Cols * C.Cols));
                comm_SE_coh_list.Add(SE_coh);

                T_den = SC / SE_den;
                T_coh = GC / SE_coh;

                comm_T_den_list.Add(T_den);
                comm_T_coh_list.Add(T_coh);

            }
           
            string m_name = "Community";
            ///////////////////// Preliminary Calculations finished, time to specialize //////////////////
            if (commType == CommunityType.Affil || commType == CommunityType.newAffil || commType == CommunityType.ovAffil)
            {
                // group is the vector that produces the community affiliaton matrix
                
                data.Columns.Clear(); // Clear data grid and prep for affliation matrix
                
                mTable[m_name] = new Matrix(group.Length, comNum); // may not need
                //mTable["Community"].CloneTo(mTable[m_name]);

                // add the labels to the columns of the grid
                for (int i = 0; i < comNum; i++)
                {
                    data.Columns.Add((i).ToString(), "Comm " + (i + 1).ToString());
                    mTable[m_name].ColLabels[i] = "Comm " + (i + 1).ToString();
                }
                
                for (int i = 0; i < comNum; i++)
                    for (int j = 0; j < group.Length; j++)
                        mTable[m_name][j, i] = communities[i][j];

                for (int row = 0; row < group.Length; row++)
                {
                    // Generate an array to hold this row
                    string[] newRow = new string[comNum];     //Label this bad boy
                    for (int col = 0; col < comNum; col++)
                        newRow[col] = mTable[m_name][row, col].ToString();

                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = mTable["Data"].RowLabels[row];
                    mTable[m_name].RowLabels[row] = mTable["Data"].RowLabels[row];
                }
                commAffil = new Matrix(mTable[m_name]);
            }

            else if (commType == CommunityType.Density || commType == CommunityType.newDensity || commType == CommunityType.ovDensity)
            {
                data.Rows.Clear();
                data.Columns.Clear();

                mTable[m_name] = CalculateCommDensity(communities);
                //mTable[m_name] = new Matrix(comNum, comNum);
                for (int i = 0; i < comNum; ++i)
                {
                    data.Columns.Add((i + 1).ToString(), "Comm " + (i + 1).ToString());
                    mTable[m_name].ColLabels[i] = "Comm " + (i + 1).ToString();
                }

                for (int row = 0; row < comNum; row++)
                {
                    // Generate an array to hold this row
                    string[] newRow = new string[comNum];
                    for (int col = 0; col < comNum; col++)
                        newRow[col] = mTable[m_name][row, col].ToString();

                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = "Comm " + (row + 1).ToString();
                    mTable[m_name].RowLabels[row] = "Comm " + (row + 1).ToString();
                }
                //commDensity = new Matrix(mTable[m_name]);
            }
            else if (commType == CommunityType.Separation)
            {
                data.Rows.Clear();
                data.Columns.Clear();
                SC = CalculateCommSC(communities);

                data.Columns.Add("1", "Separation Coefficient");
                data.Rows.Add(SC);
                
            }

            else if (commType == CommunityType.RelativeDensity || commType == CommunityType.newRelativeDensity || commType == CommunityType.ovRelativeDensity)
            {
                data.Rows.Clear();
                data.Columns.Clear();

                double d = MatrixComputations.Density(mTable["Data"], density);
                mTable[m_name] = CalculateCommDensity(communities);

                for (int i = 0; i < mTable[m_name].Rows; i++)
                    for (int j = 0; j < mTable[m_name].Cols; j++)
                        mTable[m_name] /= d;

                for (int i = 0; i < comNum; ++i)
                {
                    data.Columns.Add((i + 1).ToString(), "Comm " + (i + 1).ToString());
                    mTable[m_name].ColLabels[i] = "Comm " + (i + 1).ToString();
                }

                for (int row = 0; row < comNum; row++)
                {
                    // Generate an array to hold this row
                    string[] newRow = new string[comNum];
                    for (int col = 0; col < comNum; col++)
                        newRow[col] = mTable[m_name][row, col].ToString();

                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = "Comm " + (row + 1).ToString();
                    mTable[m_name].RowLabels[row] = "Comm " + (row + 1).ToString();
                }
            }

            else if (commType == CommunityType.Cohesion || commType == CommunityType.newCohesion || commType == CommunityType.ovCohesion)
            {
                data.Rows.Clear();
                data.Columns.Clear();

                LoadStructEquiv(density, year, "Data");
                mTable[m_name] = computeCommCohesiveMatrix();

                for (int i = 0; i < mTable[m_name].Rows; ++i)
                {
                    data.Columns.Add((i + 1).ToString(), "Comm " + (i + 1).ToString());
                    mTable[m_name].ColLabels[i] = "Comm " + (i + 1).ToString();
                }

                for (int row = 0; row < mTable[m_name].Rows; row++)
                {
                    // Generate an array to hold this row
                    string[] newRow = new string[comNum];
                    for (int col = 0; col < mTable[m_name].Cols; col++)
                        newRow[col] = mTable[m_name][row, col].ToString();

                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = "Comm " + (row + 1).ToString();
                    mTable[m_name].RowLabels[row] = "Comm " + (row + 1).ToString();
                }
            }

            else if (commType == CommunityType.Char || commType == CommunityType.newChar || commType == CommunityType.ovChar)
            {

                List<int> comSizeArray = new List<int>(comNum);

                for (int i = 0; i < comNum; ++i)
                {
                    int comSize = 0;
                    for (int w = 0; w < group.Length; ++w)
                        if (communities[i][w] != 0)
                            comSize++; // determine community size for all communities
                    comSizeArray.Add(comSize);
                }

                Matrix ComC;
                //int dvcCount = 0;
                string[][] dyadicLabels = null;
                ComC = this.LoadComCharacteristics(svc, dvc, attrMatrix, year, comNum, communities, comSizeArray, cutoff, ref dyadicLabels);

                data.Rows.Clear();
                data.Columns.Clear();

                int priorCount = 3;
                data.Columns.Add("1", "NetId");
                data.Columns.Add("2", "Size");

                // new columns to be added for NGT and GMT
                data.Columns.Add("3", "NGT");
                data.Columns.Add("4", "GMT");
                for (int i = 0; i < svc.Count; ++i)
                {
                    data.Columns.Add((i + 5).ToString(), "SVC " + svc[i].Second.ToString());
                    priorCount++;
                }
                for (int i = 0; i < dvc.Count; ++i)
                //for (int i = 0; i < dyadicLabels.Length; ++i)
                {
                    for (int j = 0; j < dyadicLabels[i].Length; j++)
                    {
                        //data.Columns.Add((priorCount + i + 5).ToString(), "DVC " + dvc[i].Second.ToString());
                        data.Columns.Add(priorCount.ToString(), dyadicLabels[i][j]);
                        priorCount++;
                    }
                }
                for (int i = 0; i < attrMatrix.Count; ++i)
                {
                    data.Columns.Add((priorCount + i + 5).ToString(), "ATTRMATR " + attrMatrix[i].Second.ToString());
                }


                for (int row = 0; row < ComC.Rows; row++)
                {
                    // Generate an array to hold this row
                    string[] newRow = new string[ComC.Cols];
                    for (int col = 0; col < ComC.Cols; col++)
                        newRow[col] = ComC[row, col].ToString();

                    data.Rows.Add(newRow);

                    //data.Rows[row].HeaderCell.Value = "Comm " + mTable["Data"].RowLabels[row];
                    data.Rows[row].HeaderCell.Value = "Comm " + (row + 1).ToString();
                }
            }

            else if (commType == CommunityType.Cluster || commType == CommunityType.newCluster || commType == CommunityType.ovCluster)
            {
                data.Columns.Clear(); // Clear data grid and prep for affliation matrix
                data.Columns.Add("1", "Community Modularity");

                // matrix needs comNum + 1 rows since it needs an extra row for the final q value
                mTable[m_name] = new Matrix(q_size + 1, 1);
                mTable["Community"].CloneTo(mTable[m_name]); // may not need

                // create the communities
                for (int i = 0; i < q_size + 1; i++)
                {
                    mTable[m_name][i, 0] = q_array[i];
                }

                string newRow = "";     //Label this bad boy
                for (int row = 0; row < q_size; row++)
                {
                    // Generate an array to hold this row
                    newRow = mTable[m_name][row, 0].ToString();
                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = "Comm " + row.ToString();
                }
                newRow = mTable[m_name][q_size, 0].ToString();
                data.Rows.Add(newRow);
                data.Rows[q_size].HeaderCell.Value = "Final ";
            }

            else if (commType == CommunityType.Coefficients || commType == CommunityType.newCoefficients || commType == CommunityType.ovCoefficients)
            {
                data.Rows.Clear();
                data.Columns.Clear();
                if (communities.Count == 1)
                {
                    for (int i = 0; i < q_array.Length; i++)
                    {
                        q_array[i] = 0;
                    }
                }

                LoadStructEquiv(density, year, "Data");

                if (true)//commType == CommunityType.Coefficients)
                {
                    mTable[m_name] = new Matrix(comm_SC_list.Count, 9);
                }
                else if (commType == CommunityType.newCoefficients)
                {
                    mTable[m_name] = new Matrix(comm_SC_list.Count, 9);
                }
                
                for (int i = 0; i < comm_SC_list.Count - 1; i++)
                {
                    mTable[m_name][i, 0] = year;
                    mTable[m_name][i, 1] = i + 1;
                    mTable[m_name][i, 2] = comm_SC_list[i];
                    mTable[m_name][i, 5] = comm_GC_list[i];

                    if (true)//commType == CommunityType.Coefficients)
                    {
                        mTable[m_name][i, 8] = q_array[i];
                        mTable[m_name][i, 6] = comm_null_GC_list[i];
                        mTable[m_name][i, 7] = comm_T_GC_list[i];
                        mTable[m_name][i, 3] = comm_null_SC_list[i];
                        mTable[m_name][i, 4] = comm_T_SC_list[i];
                    }
                    
                    if (false)//commType == CommunityType.newCoefficients)
                    {
                        mTable[m_name][i, 8] = q_array[i];
                        mTable[m_name][i, 6] = comm_SE_coh_list[i];
                        mTable[m_name][i, 7] = comm_T_coh_list[i];
                        mTable[m_name][i, 3] = comm_SE_den_list[i];
                        mTable[m_name][i, 4] = comm_T_den_list[i];
                    }
                    mTable[m_name].RowLabels[i] = (i + 1).ToString();          
                }

                //mTable[m_name] = new Matrix(1, 6);
                int lastRow = comm_SC_list.Count - 1;
                if (true)//commType == CommunityType.Coefficients)
                {
                    mTable[m_name][lastRow, 0] = year;
                    mTable[m_name][lastRow, 1] = lastRow + 1;
                    mTable[m_name][lastRow, 2] = comm_SC_list[lastRow];
                    mTable[m_name][lastRow, 3] = comm_null_SC_list[lastRow];
                    mTable[m_name][lastRow, 4] = comm_T_SC_list[lastRow];
                    mTable[m_name][lastRow, 5] = comm_GC_list[comm_GC_list.Count - 1];
                    mTable[m_name][lastRow, 6] = comm_null_GC_list[lastRow];
                    mTable[m_name][lastRow, 7] = comm_T_GC_list[lastRow];
                }
                else
                {
                    mTable[m_name][lastRow, 0] = year;
                    mTable[m_name][lastRow, 1] = lastRow + 1;
                    mTable[m_name][lastRow, 2] = comm_SC_list[lastRow];
                    mTable[m_name][lastRow, 3] = SE_den;
                    mTable[m_name][lastRow, 4] = T_den;
                    mTable[m_name][lastRow, 5] = comm_GC_list[comm_GC_list.Count - 1];
                    mTable[m_name][lastRow, 6] = SE_coh;
                    mTable[m_name][lastRow, 7] = T_coh;
                }
                //if (commType == CommunityType.newCoefficients)
                //{
                    mTable[m_name][lastRow, 8] = q_array[q_size];
                //}


                mTable[m_name].RowLabels[lastRow] = (lastRow + 1).ToString();
                if (true)//commType == CommunityType.Coefficients)
                {
                    mTable[m_name].ColLabels[0] = "Year";
                    mTable[m_name].ColLabels[1] = "Row";
                    mTable[m_name].ColLabels[2] = "Separation Coefficient";
                    mTable[m_name].ColLabels[3] = "Null Sep. Coeff.";
                    mTable[m_name].ColLabels[4] = "T-Separation";
                    mTable[m_name].ColLabels[5] = "Cohesion Coefficient";
                    mTable[m_name].ColLabels[6] = "Null Coh. Coeff.";
                    mTable[m_name].ColLabels[7] = "T-Cohesion";
                }
                else
                {
                    mTable[m_name].ColLabels[0] = "Year";
                    mTable[m_name].ColLabels[1] = "Row";
                    mTable[m_name].ColLabels[2] = "Separation Coefficient";
                    mTable[m_name].ColLabels[3] = "SE Separation";
                    mTable[m_name].ColLabels[4] = "T-stat Separation";
                    mTable[m_name].ColLabels[5] = "Cohesion Coefficient";
                    mTable[m_name].ColLabels[6] = "SE Cohesion";
                    mTable[m_name].ColLabels[7] = "T-stat Cohesion";
                }
                //if (commType == CommunityType.newCoefficients)
                //{
                    mTable[m_name].ColLabels[8] = "Modularity Coefficient";
                //}
                    if (true)//commType == CommunityType.Coefficients)
                    {
                        data.Columns.Add("Year", "Year");
                        data.Columns.Add("Row", "Row");
                        data.Columns.Add("Separation Coefficient", "Separation Coefficient");
                        data.Columns.Add("Null Sep. Coeff.", "Null Sep. Coeff.");
                        data.Columns.Add("T-Separation", "T-Separation");
                        data.Columns.Add("Cohesion Coefficient", "Cohesion Coefficient");
                        data.Columns.Add("Null Coh. Coeff.", "Null Coh. Coeff.");
                        data.Columns.Add("T-Cohesion", "T-Cohesion");
                    }
                    else
                    {
                        data.Columns.Add("Year", "Year");
                        data.Columns.Add("Row", "Row");
                        data.Columns.Add("Separation Coefficient", "Separation Coefficient");
                        data.Columns.Add("SE Separation", "SE Separation");
                        data.Columns.Add("T-stat Separation", "T-stat Separation");
                        data.Columns.Add("Cohesion Coefficient", "Cohesion Coefficient");
                        data.Columns.Add("SE Cohesion", "SE Cohesion");
                        data.Columns.Add("T-stat Cohesion", "T-stat Cohesion");
                    }
                //if (commType == CommunityType.newCoefficients)
                //{
                    data.Columns.Add("Modularity Coefficient", "Modularity Coefficient");
                //}


                for (int row = 0; row < mTable[m_name].Rows; row++)
                {
                    string[] newRow = new string[mTable[m_name].Cols];
                    for (int col = 0; col < mTable[m_name].Cols; col++)
                    {
                        newRow[col] = mTable[m_name][row, col].ToString();
                    }
                    data.Rows.Add(newRow);
                    data.Rows[row].HeaderCell.Value = (row + 1).ToString();
                }
            }
        }

        protected void calcComm(int maxSize, ref Community.returnValues returnVal, int[] used, ref int[] tempCommunity, ref int position, ref int comPos, ref bool unique)
        {
            for (int i = 0; i < maxSize; ++i)
            {
                tempCommunity[0] = i + 1;   // Add original node to community
                for (int j = 0; j < maxSize; ++j)
                {
                    if (j < returnVal.join.Length && returnVal.join[j].y == i + 1)   // If node was joined with originating node, add it to community
                    {
                        tempCommunity[position] = returnVal.join[j].x;
                        // used[returnVal.join[j].x] = 1;
                        position++;
                    }
                    else if (returnVal.communities[i + 1].v != null && returnVal.communities[i + 1].v.findItem(j + 1) != null
                        /*&& returnVal.communities[i + 1].v.support >= size*/)
                    {                                   // If an edge exists....
                        tempCommunity[position] = j + 1;
                        position++;
                        used[j + 1] = 1;

                    }
                }
                position = 1;

                bool[] theTruth = new bool[comPos];
                int[] dummy;
                int[] tryAgain = new int[maxSize];

                bool switcharoo = false;

                for (int k = 0; k < comPos; ++k)
                {
                    dummy = communities[k];
                    // reset assuming its unique
                    foreach (int j in tempCommunity)
                    {
                        if (j == 0)
                            break;

                        unique = true;
                        for (int l = 0; l < maxSize; ++l)
                        {
                            if (dummy[l] == 0) break;
                            if (j == dummy[l])
                            {
                                unique = false;         // if element already present in another community, move on to next element                                   
                            }
                        }
                        if (unique)
                        {
                            break;                // if element not found in any other community                                          
                        }
                    }
                    theTruth[k] = unique;
                }

                for (int j = 0; j < comPos; ++j)
                    if (!theTruth[j]) tempCommunity[1] = 0;

                switcharoo = true;
                for (int k = 0; k < comPos; ++k)
                {

                    foreach (int h in communities[k])
                    {
                        if (h == 0) break;
                        switcharoo = true;
                        foreach (int r in tempCommunity)
                        {
                            if (r == 0) break;
                            if (h == r)
                            {
                                switcharoo = false;
                                break;
                            }

                        }
                        if (switcharoo)
                        {
                            break;
                        }
                    }
                    if (!switcharoo)
                    {
                        communities[k] = tempCommunity;
                        break;
                    }
                }

                if (tempCommunity[1] != 0 && switcharoo)
                {

                    communities[comPos] = tempCommunity;
                    comPos++;

                }

                tempCommunity = new int[maxSize + 1];
                unique = true;
                switcharoo = false;
            }
        }


        public void LoadLocalTransitivityMatrix(DataGridView data, int networkID, double binaryCutoff)
        {
            data.Columns.Clear();
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            string ms = "LocalTransitivity";
            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance, binaryCutoff);

            //string[] labelParts = { "Network ID", "Node ID", "d", "lt" };
            string[] labelParts = { "Network ID", "Node", "Degree", "Closure Triangles", "Local Closure", "Relevant Triads", "Local Transitivity" };
            for (int i = 0; i < labelParts.Length; ++i)
                data.Columns.Add(labelParts[i], labelParts[i]);

            int colCount = labelParts.Length; // there are 6 columns in this grid

            Matrix lt = mTable.AddMatrix(ms, mTable["Data"].Rows, colCount);
            // Col labels
            for (int i = 0; i < colCount; i++)
                lt.ColLabels[i] = labelParts[i];

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                lt.RowLabels[i] = mTable["Data"].RowLabels[i];
            }

            lt.ColIsNonInteger = true;
            lt.ColOfNonInteger = 1;

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                lt[i, 0] = networkID;
                lt[i, 1] = i + 1; // not used
                lt.ActualCol.Add(mTable["Data"].RowLabels[i]);
                lt[i, 2] = triad.getDegree(i);
                lt[i, 3] = triad.getClosureTriangle(i);
                lt[i, 4] = triad.getLocalClosure(i);
                lt[i, 5] = triad.getRelevantTriad(i);
                lt[i, 6] = triad.getLocalTransitivity(i);
            }

            for (int row = 0; row < lt.Rows; row++)
            {
                // Generate an array to hold this row
                string[] newRow = new string[lt.Cols];
                for (int col = 0; col < lt.Cols; col++)
                {
                    if (col == 1) // column of the nodes
                        newRow[col] = mTable["Data"].RowLabels[row];
                    else
                        newRow[col] = lt[row, col].ToString();
                }
                data.Rows.Add(newRow);
                data.Rows[row].HeaderCell.Value = mTable["Data"].RowLabels[row];
            }   
        }

        public void LoadDyadicTransitiviyMatrix(DataGridView data, int networkID, double binaryCutoff)
        {
            data.Columns.Clear();
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");
            
            string ms = "DyadicTransitivity";
            Triads triad = new Triads(mTable["Data"], Triads.TriadType.NonBalance);
            mTable[ms] = triad.calcDyadicTransitivity(mTable["Data"]);
        }

        public void LoadLocalBalanceMatrix(DataGridView data, int networkID)
        {
            data.Columns.Clear();
            if (mTable["Data"] == null)
                throw new Exception("Data matrix required before Clique Affiliation Matrix is valid!");

            string ms = "LocalBalance";
            Triads triad = new Triads(mTable["Data"], Triads.TriadType.Balance);

            string[] labelParts = { "Network ID", "Node ID", "d", "LB" };
            for (int i = 0; i < labelParts.Length; ++i)
                data.Columns.Add(labelParts[i], labelParts[i]);

            int colCount = 4; // there are 4 columns in this grid

            Matrix lt = mTable.AddMatrix(ms, mTable["Data"].Rows, colCount);
            // Col labels
            for (int i = 0; i < colCount; i++)
                lt.ColLabels[i] = labelParts[i];

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                lt.RowLabels[i] = mTable["Data"].RowLabels[i];
            }

            lt.ColIsNonInteger = true;
            lt.ColOfNonInteger = 1;

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                lt[i, 0] = networkID;
                lt[i, 1] = (i + 1); // these are not the actual values
                lt.ActualCol.Add(mTable["Data"].RowLabels[i]);
                lt[i, 2] = triad.getTriadListCount(i);
                lt[i, 3] = triad.getLocalBalance(i);
            }

            for (int row = 0; row < lt.Rows; row++)
            {
                // Generate an array to hold this row
                string[] newRow = new string[lt.Cols];
                for (int col = 0; col < lt.Cols; col++)
                {
                    if (col == 1)
                        newRow[col] = mTable["Data"].RowLabels[row];
                    else
                        newRow[col] = lt[row, col].ToString();
                }
                data.Rows.Add(newRow);
                data.Rows[row].HeaderCell.Value = mTable["Data"].RowLabels[row];
            }
        }

        private void CombineCliques(List<Clique> cliques, int index1, int index2)
        {
            Set clique1 = new Set();
            Set clique2 = new Set();
            clique1.SetArray = cliques[index1].Members;
            clique2.SetArray = cliques[index2].Members;

            // Combine the cliques by using a Set Union
            Set newSet = Set.unionSets(clique1, clique2);
            Clique newClique = new Clique(newSet.SetArray, cliqueSize);

            // remove the cliques used for combining
            // always should be 2 iterations since only should be combining 2 cliques
            // need to iterate backwards since items at the end of the list will be removed

            cliques.RemoveAt(index2);
            cliques.RemoveAt(index1);

            // add the new combined clique back to the collection
            cliques.Add(newClique);
            cliques.Sort();
            //return cliques;
        }

        private double CalculateEQ(Matrix CA)
        {
            // Calculate the value of EQ
            double sum = 0.0;
            for (int c = 0; c < CA.Cols; c++)
            {
                for (int i = 0; i < CA.Rows; i++)
                {
                    if (CA[i, c] == 0)
                        continue;
                    for (int j = i + 1; j < CA.Rows; j++)
                    {
                        if (CA[j, c] == 0)
                            continue;
                        double oi = CA.GetRowSum(i);
                        double oj = CA.GetRowSum(j);
                        double bsij = mTable["Data"][i, j];
                        double ici = mTable["Data"].GetRowSum(i);
                        double ocj = mTable["Data"].GetRowSum(j);

                        sum += (bsij - ((ici * ocj) / (2 * mTable["Data"].Rows))) / (oi * oj);
                    }
                }
            }
            return sum / (2 * mTable["Data"].Rows);
        }

        private double FindCorrectCombinedCliques(List<Clique> cliques, List<int> combineList)
        {
            //List<Clique> tempCliques = new List<Clique>();
            List<double> EQ_list = new List<double>();
            //Dictionary<double, List<int>> cliquesToCombine = new Dictionary<double, List<int>>();
            Dictionary<double, List<Clique>> newCliques = new Dictionary<double, List<Clique>>();
            double EQ;

            for (int i = 0; i < combineList.Count; i++)
            {
                for (int j = i + 1; j < combineList.Count; j++)
                {
                    // Copy the contents of prevCliques to cliques to restore original
                    //tempCliques.Clear();
                    List<Clique> tempCliques = new List<Clique>();
                    foreach (Clique clique in cliques)
                        tempCliques.Add(clique);

                    // Combine the cliques specified by indices i and j
                    CombineCliques(tempCliques, combineList[i], combineList[j]);
                    
                    // Convert cliques into a matrix
                    Matrix CA = new Matrix(cliqueSize, tempCliques.Count);
                    for (int r = 0; r < tempCliques.Count; r++)
                        for (int s = 0; s < cliqueSize; s++)
                            CA[s, r] = tempCliques[r].IntContains(s);

                    // Calculate the new EQ value and add the corresponding indices
                    // to the cliquesToCombine dictionary
                    EQ = CalculateEQ(CA);
                    newCliques[EQ] = tempCliques;
                    EQ_list.Add(EQ);
                }
            }

            double max = Algorithms.MaxValue(EQ_list);

            // Set the original cliques to the new clique that corresponds to the
            // max value
            cliques.Clear();
            foreach (Clique clique in newCliques[max])
                cliques.Add(clique);

            return max;              
        }

        private void CalculateOverlapComm(List<Clique> cliques, List<Clique> prevCliques, double prevEQ,
            List<double> EQ_list, bool calcStats, List<double> SC_list, List<double> GC_list)
        {
            // create the CA matrix from the _cliques
            Matrix CA = new Matrix(cliqueSize, cliques.Count);
            for (int i = 0; i < cliques.Count; i++)
                for (int j = 0; j < cliqueSize; j++)
                    CA[j, i] = cliques[i].IntContains(j);

            
            double EQ = CalculateEQ(CA);

            double SC = 0, GC = 0;
            if (calcStats)
            {
                SC = CalculateOverlapCommSC(cliques);
                GC = CalculateOverlapCommGC(cliques);
            }
            

            // break case
            // if the new calculated EQ is less than the previous EQ
            if (EQ < prevEQ)
            {
                return;
            }

            // copy contents of cliques to prevCliques
            prevCliques.Clear();
            foreach (Clique clique in cliques)
                prevCliques.Add(clique);

            EQ_list.Add(EQ);

            if (calcStats)
            {
                SC_list.Add(SC);
                GC_list.Add(GC);
            }
            // Now need to check if need to combine cliques

            // create the CA transpose matrix and multiply the two to
            // create the CO matrix
            Matrix CO = CA.GetTranspose() * CA;

            // Diagonally standardize the CO matrix
            Matrix CO_diag = new Matrix(CO.Rows, CO.Cols);
            for (int i = 0; i < CO.Rows; i++)
                for (int j = 0; j < CO.Cols; j++)
                    CO_diag[i, j] = CO[i, j] / CO[i, i];


            // calculate the average of each entry between CO_diag and CO_diag transpose
            Matrix CO_avg = new Matrix(CO_diag.Rows, CO_diag.Cols);
            Matrix CO_transpose = CO_diag.GetTranspose();
            for (int i = 0; i < CO_diag.Rows; i++)
                for (int j = 0; j < CO_diag.Cols; j++)
                    CO_avg[i, j] = (CO_diag[i, j] + CO_transpose[i, j]) / 2;

            // Find the maximum non-diagonal entry of the CO_avg matrix
            List<double> tempMax = new List<double>();
            for (int i = 0; i < CO_avg.Rows; i++)
            {
                for (int j = 0; j < CO_avg.Cols; j++)
                {
                    if (i == j)
                        continue;
                    tempMax.Add(CO_avg[i, j]);
                }
            }
            List<int> cliquesToCombine = new List<int>();
            
            // Find all cliques that contain the maximum value for non-diagonal entries
            double max = Algorithms.MaxValue(tempMax);
            for (int i = 0; i < CO_avg.Cols; i++)
            {
                for (int j = 0; j < CO_avg.Rows; j++)
                {
                    if (CO_avg[j, i] == max)
                    {
                        cliquesToCombine.Add(i);
                        break;
                    }
                }
            }

            if (cliquesToCombine.Count > 1) // there are cliques to combine
            {
                FindCorrectCombinedCliques(cliques, cliquesToCombine);
                CalculateOverlapComm(cliques, prevCliques, EQ, EQ_list, calcStats, SC_list, GC_list);
            }            
        }

        public void CalculateOverlapComm()
        {
            if (_cliques == null)
                throw new Exception("No cliques have been found");

            List<Clique> cliques = new List<Clique>();
            List<Clique> prevCliques = new List<Clique>();

            foreach (Clique clique in _cliques)
            {
                cliques.Add(clique);
                prevCliques.Add(clique);
            }

            List<double> EQ_list = new List<double>();

            List<double> SC_list = new List<double>();
            List<double> GC_list = new List<double>();
            // prevCliques is changed within the function call
            // prevCliques will then become the list of Cliques for the overlapComm
            CalculateOverlapComm(cliques, prevCliques, double.MinValue, EQ_list, false, SC_list, GC_list);
            overlapComm = prevCliques;
        }

        public void LoadOverlapCommAffilMatrix()
        {
            // Convert overlapComm into a matrix to be able to show in GUI
            mTable.AddMatrix("OverlappingCommunity", cliqueSize, overlapComm.Count);
            for (int i = 0; i < overlapComm.Count; i++)
                for (int j = 0; j < cliqueSize; j++)
                    mTable["OverlappingCommunity"][j, i] = overlapComm[i].IntContains(j);

            for (int i = 0; i < mTable["Data"].Rows; i++)
            {
                mTable["OverlappingCommunity"].RowLabels[i] = mTable["Data"].RowLabels[i];
            }

            for (int i = 0; i < mTable["OverlappingCommunity"].Cols; i++)
            {
                mTable["OverlappingCommunity"].ColLabels[i] = (i + 1).ToString();
            }
        }

        public void LoadOverlapModifiedModularity()
        {
            List<Clique> cliques = new List<Clique>();
            List<Clique> prevCliques = new List<Clique>();

            foreach (Clique clique in _cliques)
            {
                cliques.Add(clique);
                prevCliques.Add(clique);
            }

            List<double> EQ_list = new List<double>();

            List<double> SC_list = new List<double>();
            List<double> GC_list = new List<double>();
            // prevCliques is changed within the function call
            // prevCliques will then become the list of Cliques for the overlapComm
            CalculateOverlapComm(cliques, prevCliques, double.MinValue, EQ_list, false, SC_list, GC_list);

            Matrix EQ_vector = mTable.AddMatrix("OverlapCommModularityEQ", EQ_list.Count, 1);
            for (int i = 0; i < EQ_vector.Rows; i++)
                EQ_vector[i, 0] = EQ_list[i];

            for (int i = 0; i < EQ_vector.Rows - 1; i++)
                EQ_vector.RowLabels[i] = (i + 1).ToString();

            EQ_vector.RowLabels[EQ_vector.Rows - 1] = "Final";
            EQ_vector.ColLabels[0] = "Modified Modularity";
        }

        public void LoadOverlapCommCoefficients(double density, int year, string m_name)
        {
            LoadStructEquiv(density, year, "Data");
            List<Clique> cliques = new List<Clique>();
            List<Clique> prevCliques = new List<Clique>();

            foreach (Clique clique in _cliques)
            {
                cliques.Add(clique);
                prevCliques.Add(clique);
            }
            
            List<double> EQ_list = new List<double>();

            List<double> SC_list = new List<double>();
            List<double> GC_list = new List<double>();
            // prevCliques is changed within the function call
            // prevCliques will then become the list of Cliques for the overlapComm
            CalculateOverlapComm(cliques, prevCliques, double.MinValue, EQ_list, true, SC_list, GC_list);

            // Calculate the Separation (Density)
            double SE_den = 0;
            Matrix D = CalculateOverlapCommDensity(prevCliques);
            for (int i = 0; i < D.Rows; i++)
                for (int j = 0; j < D.Cols; j++)
                    SE_den += Math.Pow((D[i, j] - D.GetRowAverage(i)), 2);
            SE_den = Math.Sqrt(SE_den / (D.Cols * D.Cols));

            // Calculate the Separation (Cohesion)
            double SE_coh = 0;
            Matrix C = computeOverlapCommCohesiveMatrix(prevCliques);
            for (int i = 0; i < C.Rows; i++)
                for (int j = 0; j < C.Cols; j++)
                    SE_coh += Math.Pow((C[i, j] - C.GetRowAverage(i)), 2);
            SE_coh = Math.Sqrt(SE_coh / (C.Cols * C.Cols));

            double T_den = SC_list[SC_list.Count - 1] / SE_den;
            double T_coh = GC_list[SC_list.Count - 1] / SE_coh;


            Matrix Stats_vector = mTable.AddMatrix(m_name, SC_list.Count, 8);

            for (int i = 0; i < SC_list.Count - 1; i++)
            {
                Stats_vector[i, 0] = year;
                Stats_vector[i, 1] = i + 1;
                Stats_vector[i, 2] = SC_list[i];
                Stats_vector[i, 5] = GC_list[i];
                Stats_vector.RowLabels[i] = (i + 1).ToString();
            }

            int lastRow = SC_list.Count - 1;

            Stats_vector[lastRow, 0] = year;
            Stats_vector[lastRow, 1] = lastRow + 1;
            Stats_vector[lastRow, 2] = SC_list[lastRow];
            Stats_vector[lastRow, 3] = SE_den;
            Stats_vector[lastRow, 4] = T_den;
            Stats_vector[lastRow, 5] = GC_list[lastRow];
            Stats_vector[lastRow, 6] = SE_coh;
            Stats_vector[lastRow, 7] = T_coh;

            Stats_vector.RowLabels[lastRow] = (lastRow + 1).ToString();
            Stats_vector.ColLabels[0] = "Year";
            Stats_vector.ColLabels[1] = "Row";
            Stats_vector.ColLabels[2] = "Separation Coefficient";
            Stats_vector.ColLabels[3] = "SE Separation";
            Stats_vector.ColLabels[4] = "T-stat Separation";
            Stats_vector.ColLabels[5] = "Cohesion Coefficient";
            Stats_vector.ColLabels[6] = "SE Cohesion";
            Stats_vector.ColLabels[7] = "T-stat Cohesion";
        }


        // Helper function to generate the expected value matrix
        public Matrix GenExpectedValueMatrix(Matrix mat)
        {
            // Calculate the out-degrees
            List<double> outDegrees = new List<double>();
            for (int i = 0; i < mat.Rows; i++)
            {
                double rowSum = mat.GetRowSum(i);
                if (rowSum == 0)
                    rowSum = 0.0001;
                outDegrees.Add(rowSum);
            }

            // Calculate the in-degrees
            List<double> inDegrees = new List<double>();
            for (int i = 0; i < mat.Cols; i++)
            {
                double colSum = mat.GetColSum(i);
                if (colSum == 0)
                    colSum = 0.0001;
                inDegrees.Add(colSum);
            }

            // Calculate the sum of the rows
            double sumOfRows = 0.0;
            for (int i = 0; i < outDegrees.Count; i++)
            {
                sumOfRows += outDegrees[i];
            }

            // Generate an Expected Value Matrix ES
            Matrix es = new Matrix(outDegrees.Count, inDegrees.Count);
            for (int i = 0; i < outDegrees.Count; i++)
            {
                for (int j = 0; j < inDegrees.Count; j++)
                {
                    es[i, j] = (outDegrees[i] * inDegrees[j]) / sumOfRows;
                }
            }
            return es;
        }

        public Matrix GenChiSquareMatrix(Matrix s, Matrix es)
        {
            // Calculate Chi-Square Matrix C
            Matrix C = new Matrix(es.Rows, es.Cols);
            for (int i = 0; i < es.Rows; i++)
            {
                for (int j = 0; j < es.Cols; j++)
                {
                    C[i, j] = Math.Pow((s[i, j] - es[i, j]), 2);
                    C[i, j] /= es[i, j];
                }
            }
            return C;
        }

        public double calcMbStat(Matrix s, Matrix es, Matrix C, double chisq_score)
        {
            // Calculate a difference matrix D
            Matrix D = new Matrix(C.Rows, C.Cols);
            for (int i = 0; i < s.Rows; i++)
            {
                for (int j = 0; j < s.Cols; j++)
                {
                    D[i, j] = s[i, j] >= es[i, j] ? C[i, j] : C[i, j] * -1;
                }
            }

            // Calculate mb statistic
            double m_stat = 0.0;
            for (int i = 0; i < D.Rows; i++)
            {
                for (int j = 0; j < D.Cols; j++)
                {
                    m_stat += D[i, j];
                }
            }
            m_stat /= chisq_score;
            return m_stat;
        }

        public void LoadSingleNetworkExpectations(DataGridView data, int year)
        {
            string ms = "SingleNetworkExpectations";

            Matrix es = GenExpectedValueMatrix(mTable["Data"]);
            Matrix C = GenChiSquareMatrix(mTable["Data"], es);

            // Calculate Chi-Square Score X
            double chisq_score = C.sum();

            // Calculate degrees of freedom
            double df = Math.Pow(C.Rows - 2, 2);

            // Calculate mb statistic
            double m_stat = calcMbStat(mTable["Data"], es, C, chisq_score);

            data.Columns.Clear();
            Matrix output = mTable.AddMatrix(ms, 1, 4);
            output[0, 0] = year;
            output[0, 1] = chisq_score;
            output[0, 2] = df;
            output[0, 3] = m_stat;

            output.ColLabels[0] = "Network ID";
            output.ColLabels[1] = "Chi-Square";
            output.ColLabels[2] = "Degrees of Freedom";
            output.ColLabels[3] = "m Statistic";


            //string[] labelParts = { "Network ID", "Node ID", "d", "lt" };
            
            for (int i = 0; i < output.ColLabels.Length; i++)
                data.Columns.Add(output.ColLabels[i], output.ColLabels[i]);

            string[] row = new string[output.Cols];
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = output[i].ToString();
            }
            data.Rows.Add(row);
        }

        public void LoadNetworkSpilloverStatistics(DataGridView data, int year, List<int> indices)
        {
            string ms = "NetworkSpilloverStatistics";

            // Generate an actual spillover matrix s12

            int size = multipleMatrixList[indices[0] - 1].Rows;
            Matrix s_all = new Matrix(size, size);
            for (int i = 0; i < s_all.Rows; i++)
            {
                for (int j = 0; j < s_all.Cols; j++)
                {
                    double product = 1.0;
                    foreach (int index in indices)
                    {
                        product *= multipleMatrixList[index - 1][i, j];
                    }
                    s_all[i, j] = product;
                }
            }

            // Generate an Expected Spillover Matrix es12
            List<Matrix> listESMatrices = new List<Matrix>();
            foreach (int index in indices)
            {
                listESMatrices.Add(GenExpectedValueMatrix(multipleMatrixList[index - 1]));
            }

            Matrix es_all = new Matrix(s_all.Rows, s_all.Cols);
            for (int i = 0; i < es_all.Rows; i++)
            {
                for (int j = 0; j < es_all.Cols; j++)
                {
                    double product = 1.0;
                    foreach (Matrix es in listESMatrices)
                    {
                        product *= es[i, j];
                    }
                    es_all[i, j] = product;
                }
            }

            // Generate a Chi-Square Matrix C12
            Matrix C12 = GenChiSquareMatrix(s_all, es_all);
            
            // Calculate a spillover Chi-Square score X12^2
            double chisq_score = C12.sum();

            // Calculate degrees of freedom
            double df = Math.Pow(C12.Rows - 2, 2);
            
            // Calculate overlap statistic
            double overlap_stat = calcMbStat(s_all, es_all, C12, chisq_score);



            data.Columns.Clear();
            Matrix output = mTable.AddMatrix(ms, 1, (2 * indices.Count) + 4);
            output[0, 0] = year;
            output[0, 1] = df;
            for (int i = 0; i < indices.Count; i++)
            {
                Matrix s = multipleMatrixList[indices[i] - 1];
                Matrix es = GenExpectedValueMatrix(s);
                Matrix C = GenChiSquareMatrix(s, es);
                double chisq = C.sum();
                output[0, 2 * i + 2] = chisq;
                output[0, 2 * i + 3] = calcMbStat(s, es, C, chisq);
            }
            output[0, output.Cols - 2] = chisq_score;
            output[0, output.Cols - 1] = overlap_stat;

            output.ColLabels[0] = "Network ID";
            output.ColLabels[1] = "Degrees of Freedom";

            string chisq_label = "";
            for (int i = 0; i < indices.Count; i++)
            {
                output.ColLabels[2 * i + 2] = "Chi-Square " + (indices[i]).ToString();
                output.ColLabels[2 * i + 3] = "m Stat " + (indices[i]).ToString();
                chisq_label += (indices[i]).ToString();
            }
            output.ColLabels[output.Cols - 2] = "Chi-Square " + chisq_label;
            output.ColLabels[output.Cols - 1] = "Overlap Statistic";

            for (int i = 0; i < output.ColLabels.Length; i++)
                data.Columns.Add(output.ColLabels[i], output.ColLabels[i]);

            string[] row = new string[output.Cols];
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = output[i].ToString();
            }
            data.Rows.Add(row);
        }
    }
}
