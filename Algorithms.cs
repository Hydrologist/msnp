using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Network
{
    // Some generic algorithsm, a la the C++ STL
    public sealed class Algorithms
    {
        public delegate T VoidFunctor<T>();
        public delegate T Functor2<T>(double a);
        public delegate T Functor<T>(double a,double b);

        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static void Fill<T>(IIndexable<T> obj, T value)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = value;
        } 
        public static void Fill<T>(IIndexable<T> obj, VoidFunctor<T> f)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = f();
        }
        public static void Fill<T>(IIndexable<T> obj, Functor2<T> f, double a)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = f(a);
        }
        public static void Fill<T>(IIndexable<T> obj, Functor<T> f, double a, double b)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = f(a,b);
        }
        public static void Fill<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = value;
        }
        public static void Fill<T>(T[] array, VoidFunctor<T> f)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = f();
        }

        public static void Iota(IIndexable<int> obj, int startValue)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = startValue + i;
        }

        public static void Iota(IIndexable<string> obj, int startValue)
        {
            for (int i = 0; i < obj.Length; ++i)
                obj[i] = (startValue + i).ToString();
        }

        public static int Accumulate(IEnumerable<int> array, int startValue)
        {
            foreach (int i in array)
                startValue += i;
            return startValue;
        }

        public static double Accumulate(IEnumerable<double> array, double startValue)
        {
            foreach (double i in array)
                startValue += i;
            return startValue;
        }

        public static T MaxValue<T>(IEnumerable<T> obj) where T: IComparable
        {
            T max = default(T);
            foreach (T i in obj)
            {
                max = i;
                break; // only want to traverse through obj once to get first index
            }
            foreach (T i in obj)
            {
                //if (max .CompareTo(default(T)) == 0 || i.CompareTo(max) > 0)
                if (i.CompareTo(max) > 0)
                    max = i;
            }
            return max;
        }

        public static T MinValue<T>(IEnumerable<T> obj) where T : IComparable
        {
            T max = default(T);
            foreach (T i in obj)
            {
                max = i;
                break; // only want to traverse through obj once to get first index
            }
            foreach (T i in obj)
            {
                //if (max.CompareTo(default(T)) == 0 || i.CompareTo(max) < 0)
                if (i.CompareTo(max) < 0)
                    max = i;
            }
            return max;
        }

        public static double Mean(params double[] values)
        {
            double sum = 0;
            foreach (double val in values)
                sum += val;
            return sum / values.Length;
        }

        public static double Mean(IEnumerable<double> values)
        {
            double sum = 0;
            int count = 0;
            foreach (double val in values)
            {
                sum += val;
                ++count;
            }
            return sum / count;
        }

        public static double Variance(params double[] values)
        {
            double mean = Mean(values);
            double M2 = 0.0;
            int n = 0;
            foreach (double x in values)
            {
                ++n;
                //double delta = x - mean;
                //mean += delta / n;
                //M2 += delta * (x - mean);
                double delta = (x - mean) * (x - mean);
                M2 += delta;
            }
            return M2 / n;
        }

        public static double Variance(IEnumerable<double> values)
        {
            double mean = Mean(values);
            double M2 = 0.0;
            int n = 0;
            foreach (double x in values)
            {
                ++n;
                double delta = (x - mean) * (x - mean);
                M2 += delta;
            }
            return M2 / n;
        }

        // Returns the standard deviation of a sample
        public static double Stdev(IEnumerable<double> values)
        {
            double mean = Mean(values);
            double M2 = 0.0;
            int n = 0;
            foreach (double x in values)
            {
                ++n;
                double delta = (x - mean) * (x - mean);
                M2 += delta;
            }
            double sample = M2 / (n - 1);
            return Math.Sqrt(sample);
        }

        // Returns the standard deviation of a population
        public static double Stdevp(IEnumerable<double> values)
        {
            return Math.Sqrt(Variance(values));
        }

        public static T[] Subarray<T>(T[] a, int start, int end)
        {
            T[] subarray = new T[end - start];
            Array.Copy(a, start, subarray, 0, subarray.Length);
            return subarray;
        }

        public static bool IsSupersetOf<T>(T[] superset, T[] subset)
        {
            if (superset.Length < subset.Length)
                return false;

            for (int i = 0; i < subset.Length; ++i)
            {
                if (Array.IndexOf(superset, subset[i]) < 0)
                    return false;
            }
            return true;
        }

        public static bool IsSupersetOf<T>(T[] subset, List<T[]> sets)
        {
            foreach (T[] set in sets)
                if (IsSupersetOf(subset, set))
                    return true;

            return false;
        }

        public static int GetMinNumMultiples(int multiple, double whole)
        {
            if (whole % multiple == 0)
            {
                return (int)(whole / multiple);
            }
            else
            {
                return (int)(whole / multiple) + 1;
            }
        }
    }
}
