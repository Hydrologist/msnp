using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using Network.Cliques;

namespace Network
{
    class NPNode
    {
        private double np = 0.0;
        private double npabs = 0.0;

        public NPNode()
        {
        }

        public double NP
        {
            get { return np; }
        }

        public double NPabs
        {
            get { return npabs; }
        }

        public void ComputeNP(Matrix WCA, Matrix CCA, int node, int n, List<List<int>> Blocks, CliqueCollection _cliques, List<int[]> communities, int comNum, bool isComm, bool isClique, Matrix Cohesion, List<double> tempattribute, bool file)
        {
            if (!isClique && !isComm)
            {
                for (int col = 0; col < Blocks.Count; col++)
                {
                    if (WCA[node, col] != 0 && (WCA[n + 1, col] >= CCA[n + 1, col]))
                    {
                        List<int> rcalist = new List<int>();
                        List<int> rccalist = new List<int>();
                        List<int> rccalist2 = new List<int>();
                        for (int row = 0; row < n; row++)
                            if (Blocks[col].Contains(row) && row != node)
                                rcalist.Add(row);
                        Clique rcaclique = new Clique(rcalist, n);
                        rcaclique.ComputeCohesion(Cohesion);
                        double rcawsum = rcaclique.ComputeWsum(tempattribute, file);
                        for (int row = 0; row < n; row++)
                        {
                            if (!Blocks[col].Contains(row) || row == node)
                                rccalist.Add(row);
                            if (!Blocks[col].Contains(row))
                                rccalist2.Add(row);
                        }
                        Clique rccaclique = new Clique(rccalist, n);
                        rccaclique.ComputeCohesion(Cohesion);
                        double rccawsum = rccaclique.ComputeWsum(tempattribute, file);
                        Clique rccaclique2 = new Clique(rccalist2, n);
                        rccaclique2.ComputeCohesion(Cohesion);
                        double rccawsum2 = rccaclique2.ComputeWsum(tempattribute, file);

                        if (rcawsum < rccawsum) np += 1;
                        else if (rcawsum == rccawsum) np += 0.5;
                        if (rcawsum < rccawsum2) npabs += 1;
                        else if (rcawsum == rccawsum2) npabs += 0.5;

                    }

                }
            }//if block
            else if (isComm)
            {
                for (int col = 0; col < comNum; col++)
                {
                    if (WCA[node, col] != 0 && (WCA[n + 1, col] >= CCA[n + 1, col]))
                    {
                        List<int> rcalist = new List<int>();
                        List<int> rccalist = new List<int>();
                        List<int> rccalist2 = new List<int>();
                        // recreate the WCA matrix without the members of the node
                        for (int row = 0; row < n; row++)
                            if ((communities[col][row] != 0) && (row != node))
                                rcalist.Add(row);
                        Clique rcaclique = new Clique(rcalist, n);
                        rcaclique.ComputeCohesion(Cohesion);

                        double rcawsum = rcaclique.ComputeWsum(tempattribute, file);
                        // recreate the CCA matrix with the members that were removed
                        // from the WCA matrix previously
                        for (int row = 0; row < n; row++)
                        {
                            if ((communities[col][row] == 0) || (row == node))
                                rccalist.Add(row);
                            if (communities[col][row] == 0)
                                rccalist2.Add(row);
                        }
                        Clique rccaclique = new Clique(rccalist, n);
                        rccaclique.ComputeCohesion(Cohesion);
                        double rccawsum = rccaclique.ComputeWsum(tempattribute, file);
                        Clique rccaclique2 = new Clique(rccalist2, n);
                        rccaclique2.ComputeCohesion(Cohesion);
                        double rccawsum2 = rccaclique2.ComputeWsum(tempattribute, file);

                        if (rcawsum < rccawsum) np += 1;
                        else if (rcawsum == rccawsum) np += 0.5;
                        if (rcawsum < rccawsum2) npabs += 1;
                        else if (rcawsum == rccawsum2) npabs += 0.5;

                    }
                }

            }
            else //if use clique
            {
                for (int col = 0; col < _cliques.Count; col++)
                {
                    if (WCA[node, col] != 0 && (WCA[n + 1, col] >= CCA[n + 1, col]))
                    {
                        List<int> rcalist = new List<int>();
                        List<int> rccalist = new List<int>();
                        List<int> rccalist2 = new List<int>();
                        for (int row = 0; row < n; row++)
                            if (_cliques[col].Contains(row) && row != node)
                                rcalist.Add(row);
                        Clique rcaclique = new Clique(rcalist, n);
                        rcaclique.ComputeCohesion(Cohesion);
                        double rcawsum = rcaclique.ComputeWsum(tempattribute, file);
                        for (int row = 0; row < n; row++)
                        {
                            if (!_cliques[col].Contains(row) || row == node)
                                rccalist.Add(row);
                            if (!_cliques[col].Contains(row))
                                rccalist.Add(row);
                        }
                        Clique rccaclique = new Clique(rccalist, n);
                        rccaclique.ComputeCohesion(Cohesion);
                        double rccawsum = rccaclique.ComputeWsum(tempattribute, file);
                        Clique rccaclique2 = new Clique(rccalist2, n);
                        rccaclique2.ComputeCohesion(Cohesion);
                        double rccawsum2 = rccaclique2.ComputeWsum(tempattribute, file);

                        if (rcawsum < rccawsum) np += 1;
                        else if (rcawsum == rccawsum) np += 0.5;
                        if (rcawsum < rccawsum2) npabs += 1;
                        else if (rcawsum == rccawsum2) npabs += 0.5;

                    }
                }
            }
        }

    }
}

 
