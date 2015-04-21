using System;
using System.Collections.Generic;
using System.Text;

namespace LiyeLib
{
    //A binaryheap Data struture. Adapted from Weiss's C++ code by Liye Zhang
    class Maxheap<T>
    {
        public int currentSize;
        List<IComparable> array;

        public Maxheap()
        {
            array = new List<IComparable>();
            array.Add(null);
            currentSize = 0;
        }

        public void insert(IComparable x)
        {
            if (currentSize + 1 >= array.Count) //if we are out of room
            {
                array.Add(x);
            }

            // Percolate up
            int hole = ++currentSize;
            for (; hole > 1 && x.CompareTo(array[hole / 2]) > 0; hole /= 2)
                array[hole] = array[hole / 2];
            array[hole] = x;
        }

        public IComparable deleteMax()
        {
            IComparable temp = array[1];
            array[1] = array[currentSize--];
            percolateDown(1);
            return temp;
        }

        public void percolateDown(int hole)
        {
            /* 1*/
            int child;
            /* 2*/
            IComparable tmp = array[hole];

            /* 3*/
            for (; hole * 2 <= currentSize; hole = child)
            {
                /* 4*/
                child = hole * 2;
                /* 5*/
                if (child != currentSize && array[child + 1].CompareTo(array[child]) > 0)
                    /* 6*/
                    child++;
                /* 7*/
                if (array[child].CompareTo(tmp) > 0)
                    /* 8*/
                    array[hole] = array[child];
                else
                    /* 9*/
                    break;
            }
            /*10*/
            array[hole] = tmp;
        }

        public bool isEmpty()
        {
            return currentSize == 0;
        }

        public T findMax()
        {
            return (T)array[1];
        }
    }
}
