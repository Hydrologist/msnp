using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkGUI.Sets
{
    public class Set
    {
        private List<int> setArray;
        private int setSize;

        public Set()
        {
            setArray = new List<int>();
        }

        public Set(int size)
        {
            setSize = size;
            setArray = new List<int>();
            for (int i = 0; i < setSize; i++)
                setArray.Add(0);
        }

        public void insert(int value)
        {
            setArray.Add(value);
        }

        public void insert(int value, int pos)
        {
            setArray[pos] = value;
        }

        public bool isEmpty()
        {
            return Size == 0;
        }

        public void removeSet()
        {
            setArray.Clear();
        }

        public static Set unionSets(Set setA, Set setB)
        {
            Set newSet = new Set();
            for (int i = 0; i < setA.Size; i++)
            {
                bool inSet = false;
                for (int j = 0; j < setB.Size; j++)
                {
                    if (setA[i] == setB[j])
                    {
                        inSet = true;
                        break;
                    }
                }
                if (!inSet)
                    newSet.insert(setA[i]);
            }
            for (int i = 0; i < setB.Size; i++)
                newSet.insert(setB[i]);
            return newSet;
        }

        public static Set intersectionSets(Set setA, Set setB)
        {
            Set newSet = new Set();
            for (int i = 0; i < setA.Size; i++)
            {
                bool inSet = false;
                for (int j = 0; j < setB.Size; j++)
                {
                    if (setA[i] == setB[j])
                    {
                        inSet = true;
                        break;
                    }
                }
                if (inSet)
                    newSet.insert(setA[i]);
            }
            return newSet;
        }

        public Set relativeComplement(Set setB)
        {
            Set newSet = new Set();
            for (int i = 0; i < Size; i++)
            {
                bool inSet = false;
                for (int j = 0; j < setB.Size; j++)
                {
                    if (setArray[i] == setB[j])
                    {
                        inSet = true;
                        break;
                    }
                }
                if (!inSet)
                    newSet.insert(setArray[i]);
            }
            return newSet;
        }

        public static bool compareEqual(Set a, Set b)
        {
            bool isEqual = true;
            if (a.Size != b.Size)
                return false;

            for (int i = 0; i < a.Size; i++)
            {
                if (a[i] != b[i])
                    isEqual = false;
            }
            return isEqual;
        }

        public static Set getNeighbors(int vertex, List<Set> allSets)
        {
            Set neighbors = new Set();
            for (int i = 0; i < allSets[vertex - 1].Size; i++)
                neighbors.insert(allSets[vertex - 1][i]);
            return neighbors;
        }

        public List<int> SetArray
        {
            get { return setArray; }
            set { setArray = value; }
        }

        public int Size
        {
            get { return setArray.Count; }
        }

        public int this[int pos]
        {
            get
            {
                return setArray[pos];
            }
            set
            {
                setArray[pos] = value;
            }
        }
    }
}
