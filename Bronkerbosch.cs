using System;
using System.Collections.Generic;
using System.Text;
using NetworkGUI.Sets;

namespace NetworkGUI
{
    static class Bronkerbosch
    {
        //public static int numOfRowsAndCols;
        public static void findMaximalCliques(Set R, Set P, Set X, ref List<Set> cliqueSets, List<Set> adjacentVertices, int numOfRowsAndCols)
        {
            if (P.isEmpty() && X.isEmpty())
            {
                Set newSet = new Set(numOfRowsAndCols);
                for (int i = 0; i < R.Size; i++)
                    newSet.insert(1, R[i] - 1);
                if (cliqueSets.Count == 0) // if empty
                    cliqueSets.Add(newSet);

                bool inClique = false;
                for (int i = 0; i < cliqueSets.Count; i++)
                {
                    if (Set.compareEqual(cliqueSets[i], newSet))
                    {
                        inClique = true;
                        break;
                    }
                }
                if (!inClique)
                    cliqueSets.Add(newSet);
                return;
            }

            for (int i = 0; i < P.Size; i++)
            {
                int vertex = P[i];
                Set v = new Set();
                Set neighbors = Set.getNeighbors(vertex, adjacentVertices);
                v.insert(vertex);

                findMaximalCliques(Set.unionSets(R, v), Set.intersectionSets(P, neighbors),
                    Set.intersectionSets(X, neighbors), ref cliqueSets, adjacentVertices, numOfRowsAndCols);
                X = Set.unionSets(X, v);
            }
        }
    }
}
