using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.IO;

namespace Network.IO
{
    public class VectorValueProvider
    {
        private Dictionary<int, double> _valueMap = null;
        private double _value;

        public VectorValueProvider(string filename)
        {
            _valueMap = new Dictionary<int, double>();
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    double val;
                    int networkId;
                    string[] parts = s.Split(',');
                    if (parts.Length > 1 && double.TryParse(parts[1], out val) && int.TryParse(parts[0], out networkId))
                        _valueMap[networkId] = val;
                }
            }
        }

        public VectorValueProvider(double value)
        {
            _value = value;
        }

        public double GetValue(int networkId)
        {
            if (_valueMap == null)
                return _value;

            return _valueMap[networkId];
        }

        public double this[int networkID]
        {
            get { return GetValue(networkID); }
        }
    }
}
