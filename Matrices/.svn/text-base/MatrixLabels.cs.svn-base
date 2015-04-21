using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Matrices
{
    public class MatrixLabels : IIndexable<string>
    {
        private string[] _labels;
        private Dictionary<string, int> _labelPos = null;

        public MatrixLabels(int count)
        {
            _labels = new string[count];
        }

        public MatrixLabels(MatrixLabels matrixLabels)
        {
            _labels = new string[matrixLabels.Count];
            matrixLabels._labels.CopyTo(_labels, 0);
        }

        public void SetLabels(string labelString)
        {
            if (labelString == null)
                throw new ArgumentNullException("s");

            SetLabels(labelString.Split(','));
        }

        public void SetLabels(string[] labels)
        {
            if (labels == null)
                throw new ArgumentNullException("labels");

            // Copy from the first nonempty label
            Array.Copy(labels,
                Array.FindIndex<string>(labels, delegate(string s) { return !string.IsNullOrEmpty(s); }),
                _labels, 0, Math.Min(labels.Length, _labels.Length));
        }

        public void SetLabels(IEnumerable<string> labels)
        {
            if (labels == null)
                throw new ArgumentNullException("labels");

            // Copy from the first nonempty label
            int i = 0;
            foreach (string label in labels) 
            {
                _labels[i] = label;
                ++i;
            }
        }

        public string this[int i]
        {
            get
            {
                return _labels[i];
            }
            set
            {
                _labels[i] = value;
            }
        }

        public int this[string s]
        {
            get
            {
                if (_labelPos == null)
                    InitializeLabelPos();

                return _labelPos[s];
            }
        }

        private void InitializeLabelPos()
        {
            _labelPos = new Dictionary<string, int>();
            for (int i = 0; i < _labels.Length; ++i)
                _labelPos[_labels[i]] = i;
        }

        public int Count
        {
            get { return _labels.Length; }
        }

        public int Length
        {
            get { return _labels.Length; }
        }

        public void CopyFrom(MatrixLabels labels)
        {
            if (labels == null)
                throw new ArgumentNullException("labels");

            if (labels.Count > Count)
                throw new ArgumentOutOfRangeException("labels");

            labels._labels.CopyTo(_labels, 0);
        }

        public override string ToString()
        {
            return string.Join(",", _labels);
        }
    }
}
