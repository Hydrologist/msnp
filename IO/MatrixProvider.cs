using System;
using System.Collections.Generic;
using System.Text;
using Network.IO; 
using System.Windows.Forms;

namespace Network.Matrices
{
    public class MatrixProvider
    {
        public enum Type
        {
            MatrixFile, VectorFile, RandomSymmetric, RandomNonSymmetric, RandomDiagonal, 
            RandomVector, RandomWithProbRange, NullFile
        }

        private string _sourceFile, _outputFile;
        private bool _isdyadic;
        private Type _type;
        private int _networkId, _rows, _cols;

        public int Cols
        {
            get { return _cols; }
            set { _cols = value; }
        }

        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
        private bool _forceVector;
        private bool range;
        private int _writeRepeatCount;
        private double _min, _max;
        private double pmin, pmax;

        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public double Pmax
        {
            get { return pmax; }
            set { pmax = value; }
        }

        public double Pmin
        {
            get { return pmin; }
            set { pmin = value; }
        }

        public bool Prange
        {
            get { return range; }
            set { range = value; }
        }

        public int NetworkID
        {
            get { return _networkId; }
            set { _networkId = value; }
        }

        public int WriteRepeatCount
        {
            get { return _writeRepeatCount; }
            set { _writeRepeatCount = value; }
        }

        public bool ForceVector
        {
            get { return _forceVector; }
            set { _forceVector = value; }
        }

        public MatrixProvider(string sourceFile, string outputFile, Type type, int rows, int cols, bool isdyadic)
        {
            _networkId = 1;
            _sourceFile = sourceFile;
            _outputFile = outputFile;
            _type = type;
            _rows = rows;
            _cols = cols;
            _isdyadic = isdyadic;
            _forceVector = false;
            _writeRepeatCount = 1;
        }

        public bool IsFromFile
        {
            get { return _type == Type.MatrixFile || _type == Type.VectorFile; }
        }

        public Matrix ReadNext(bool overwrite)
        {
            Matrix m = null;
            switch (_type)
            {
                case Type.MatrixFile:
                    m = MatrixReader.ReadMatrixFromFile(_sourceFile, _networkId);
                    break;

                case Type.VectorFile:
                    m = MatrixReader.ReadVectorFromFile(_sourceFile, _networkId);
                    break;

                case Type.RandomDiagonal:
                    m = RandomMatrix.LoadDiagonal(_rows, true);
                    m.NetworkId = _networkId;
                    break;

                case Type.RandomSymmetric:
                    m = RandomMatrix.LoadSymmetric(_rows, range, pmin, pmax);
                    m.NetworkId = _networkId;
                    break;
                    
                case Type.RandomNonSymmetric:
                    m = RandomMatrix.LoadNonSymmetric(_rows, range, pmin, pmax);
                    m.NetworkId = _networkId;
                    break;

                case Type.RandomVector:
                    m = RandomMatrix.LoadVector(_rows);
                    m.NetworkId = _networkId;
                    break;

                case Type.RandomWithProbRange:
                    m = RandomMatrix.LoadWithProbabilisticRange(_rows, _cols, _min, _max);
                    m.NetworkId = _networkId;
                    break;

                case Type.NullFile:
                    return null;
            }

            _networkId = m.NetworkId + 1;
            
            if (!string.IsNullOrEmpty(_outputFile) && _type != Type.NullFile)
                WriteMatrixToFile(m, _isdyadic, overwrite);

            /*
            if (m.NetworkId < 1000)
                m.NetworkId = int.Parse("1" + m.NetworkId);
            */
            //else
              //  m.NetworkId = int.Parse("2" + m.NetworkId);
            //m.NetworkId = int.Parse("1" + m.NetworkId);
            return m;
        }

        public int GetNextMatrixRows()
        {
            switch (_type)
            {
                case Type.MatrixFile:
                    return MatrixReader.ReadMatrixFromFile(_sourceFile, _networkId).Rows;

                case Type.VectorFile:
                    return MatrixReader.ReadVectorFromFile(_sourceFile, _networkId).Rows;

                case Type.RandomDiagonal:
                    return _rows;

                case Type.RandomSymmetric:
                    return _rows;

                case Type.RandomNonSymmetric:
                    return _rows;

                case Type.RandomVector:
                    return _rows;

                case Type.RandomWithProbRange:
                    return _rows;

                case Type.NullFile:
                    return -1;

                default:
                    return 0;
            }
        }

        private void WriteMatrixToFile(Matrix m, bool isDyadic, bool overwrite)
        { 
            int oldID = m.NetworkId;
            for (int i = 0; i < _writeRepeatCount; ++i)
            {
                if (_writeRepeatCount > 1)
                    m.NetworkId = int.Parse(oldID.ToString() + (i + 1).ToString());
                if (m is Vector)
                    MatrixWriter.WriteVectorToVectorFile(m as Vector, _outputFile, overwrite);
                else if (_forceVector)
                    MatrixWriter.WriteVectorToVectorFile(m.GetDiagonalVector(), _outputFile, overwrite);
                else if (!isDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(m, _outputFile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(m, _outputFile, overwrite);

            }
            m.NetworkId = oldID;
        }
    }
}
