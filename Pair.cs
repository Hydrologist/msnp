using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public struct Pair<K, V> : IComparable<Pair<K, V>> where V : IComparable<V>
    {
        K _first;
        V _second;

        public K First
        {
            get { return _first; }
            set { _first = value; }
        }

        public V Second
        {
            get { return _second; }
            set { _second = value; }
        }

        public Pair(K first, V second)
        {
            _first = first;
            _second = second;
        }
        public int CompareTo(Pair<K, V> other)
        {
            return _second.CompareTo(other._second);
        }

        public static Pair<K, V> MakePair(K first, V second)
        {
            return new Pair<K, V>(first, second);
        }
    }

    public struct UnordererdPair<K, V>
    {
        K _first;
        V _second;

        public K First
        {
            get { return _first; }
            set { _first = value; }
        }

        public V Second
        {
            get { return _second; }
            set { _second = value; }
        }

        public UnordererdPair(K first, V second)
        {
            _first = first;
            _second = second;
        }

        public static UnordererdPair<K, V> MakePair(K first, V second)
        {
            return new UnordererdPair<K, V>(first, second);
        }
    }
}
