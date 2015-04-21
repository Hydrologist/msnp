using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;

namespace NetworkGUI.Triads
{
    class Triad
    {
        public enum TriadType
        {
            Balance, NonBalance
        }

        public class Dyad
        {
            private int _firstNode;
            private int _lastNode;

            public Dyad(int first, int last)
            {
                _firstNode = first;
                _lastNode = last;
            }

            public int First { get { return _firstNode; } }
            public int Last { get { return _lastNode; } }


        }

        private double R;
        private TriadType _type;

        // the identity of the triad node
        private int _id;

        // list of all dyads related to triad node
        private List<Dyad> _dyads;

        public Triad(int id)
        {
            _id = id;
            _dyads = new List<Dyad>();
        }

        public void AddDyad(int first, int last)
        {
            Dyad d = new Dyad(first, last);
            _dyads.Add(d);
        }

        public Dyad this[int i]
        {
            get { return _dyads[i]; }
        }

        public int Count
        {
            get { return _dyads.Count; }
        }
    }
}
