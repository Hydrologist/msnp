using System;
using System.Collections.Generic;
using System.Text;

namespace Network.IO
{
    class NetworkIdTable
    {
        private Dictionary<int, int> _table;
        private int _minNetworkId, _maxNetworkId;

        public int MinNetworkId
        {
            get { return _minNetworkId; }
        }

        public int MaxNetworkId
        {
            get { return _maxNetworkId; }
        }

        public NetworkIdTable()
        {
            _table = new Dictionary<int, int>();
            _minNetworkId = int.MaxValue;
            _maxNetworkId = int.MinValue;
        }

        public bool ContainsNetworkId(int networkId)
        {
            return _table.ContainsKey(networkId);
        }

        public int GetNextNetworkId(int networkId)
        {
            if (networkId >= _maxNetworkId)
                return -1;
            if (networkId < _minNetworkId)
                return _minNetworkId;

            while (!ContainsNetworkId(++networkId))
                ;

            return networkId;
        }

        public int GetPreviousNetworkId(int networkId)
        {
            if (networkId <= _minNetworkId)
                return -1;
            if (networkId > _maxNetworkId)
                return _maxNetworkId;

            while (!ContainsNetworkId(--networkId))
                ;

            return networkId;
        }

        public int this[int networkId]
        {
            get
            {
                return _table[networkId];
            }
            set
            {
                _table[networkId] = value;

                if (networkId < _minNetworkId)
                    _minNetworkId = networkId;
                if (networkId > _maxNetworkId)
                    _maxNetworkId = networkId;
            }
        }
    }
}
