using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Network.Matrices
{
    public class Matrix : ICloneable, IEnumerable<double>, IIndexable<double>
    {
        protected int _rows;
        protected int _cols;
        protected int _networkId;
        protected double[] _data;

        // used for outOfMemory matrices
        protected bool outOfMemory;
        protected int splitRowSize;
        protected int splitCount;
        protected int beginRow;
        protected int endRow;
        protected int currentFileUsed;

        public static int count_id = 0;
        protected string _count_id;

        // used to check if any cols are non integers
        protected bool colIsNonInteger;
        protected int colOfNonInteger;
        protected List<string> actualCol; // the list of actual values in the matrix as strings

        // need a name member for the multiple dyadic matrices
        protected string _name;

        //protected double[][] _tempdata;
        //protected List<double> _data;
        protected double[] _rowAvg, _colAvg, _rowSqrAvg, _colSqrAvg;
        protected MatrixLabels _rowLabels, _colLabels;
        private StandardizationType _standardization;

        public enum StandardizationType 
        { 
            Row, Column, DiagonalColumn, DiagonalRow, DiagonalMaximum, DiagonalMinimum, None 
        }

        public Matrix(int rows, int cols, int networkId)
        {
            if (rows <= 0)
                throw new ArgumentOutOfRangeException("rows");
            if (cols <= 0)
                throw new ArgumentOutOfRangeException("cols");

            colIsNonInteger = false;
            actualCol = new List<string>();
            
            _rows = rows;
            _cols = cols;
            _rowLabels = new MatrixLabels(_rows);
            _colLabels = new MatrixLabels(_cols);

            /*
            // testing stream writer
            System.IO.Directory.CreateDirectory(tempDir);
            TextWriter tw = new StreamWriter(tempDir + "/date.txt");

            // write a line of text to the file
            string text = "Blah Blah Blah";
            tw.WriteLine(text);
            tw.WriteLine(text);
            tw.Close();
            System.IO.Directory.Delete(tempDir, true); // to delete recursively
            //File.Delete("date.txt");
            */


            // need a double representation for the matrix size that can't be 
            // represented by a 32-bit interger
            double tempRow = _rows;
            double tempCol = _cols;
            double matrixSize = tempRow * tempCol;
            outOfMemory = (matrixSize > Constants.MaxMatrixSize) ? true : false;

            if (outOfMemory)
            {
                this._count_id = (Matrix.count_id++).ToString() + "_";

                splitCount = Algorithms.GetMinNumMultiples(Constants.MaxMatrixSize, matrixSize);
                splitRowSize = _rows / splitCount;
                if (rows % splitCount != 0)
                    splitCount += 1;

                _data = new double[splitRowSize * _cols];
                //currentFileUsed = 0;
                System.IO.Directory.CreateDirectory(Constants.tempDir);

                // create initialized matrices
                // need to check if file exists during a read

                for (int i = 0; i < splitCount; i++)
                {
                    TextWriter tw = new StreamWriter(Constants.tempDir + "/" + 
                        Constants.TempMatrix + _count_id + i.ToString() + ".txt");
                    for (int row = 0; row < splitRowSize; row++)
                    {
                        string tempRows = "";
                        int begin = i * splitRowSize;
                        tempRows += (begin + row).ToString();
                        for (int col = 0; col < _cols; col++)
                        {
                            tempRows += " 0";
                        }
                        tw.WriteLine(tempRows);
                    }
                    tw.Close();
                }
                //currentFileUsed = 0;
                //_data = new double[_rows * _cols];
            }
            else
            {
                _data = new double[_rows * _cols];
            }

            /*
            _data = new List<double>(_rows * _cols);
            for (int i = 0; i < _data.Capacity; i++)
                _data.Add(0);
            */
            _standardization = StandardizationType.None;
            _networkId = networkId;

            SetUpAverageArrays();
            currentFileUsed = 0;
        }
        public Matrix(int rows, int cols) : this(rows, cols, 0) { }
        public Matrix(int rows) : this(rows, rows, 0) { }
        public Matrix(Matrix m)
            : this(m._rows, m._cols)
        {
            m.CloneTo(this);
        }

        private void SetUpAverageArrays()
        {
            _rowAvg = new double[_rows];
            _rowSqrAvg = new double[_rows];
            _colAvg = new double[_cols];
            _colSqrAvg = new double[_cols];
        
            Algorithms.Fill<double>(_rowAvg, double.NaN);
            Algorithms.Fill<double>(_rowSqrAvg, double.NaN);
            Algorithms.Fill<double>(_colAvg, double.NaN);
            Algorithms.Fill<double>(_colSqrAvg, double.NaN);
        }

        public StandardizationType Standardization
        {
            get { return _standardization; }
            set { _standardization = value; }
        }

        public int Rows
        {
            get { return _rows; }
        }

        public int Cols
        {
            get { return _cols; }
        }

        public MatrixLabels ColLabels
        {
            get { return _colLabels; }
        }

        public MatrixLabels RowLabels
        {
            get { return _rowLabels; }
        }

        public bool ColIsNonInteger
        {
            get { return colIsNonInteger; }
            set { colIsNonInteger = value; }
        }

        public int ColOfNonInteger
        {
            get { return colOfNonInteger; }
            set { colOfNonInteger = value; }
        }

        public List<string> ActualCol
        {
            get { return actualCol; }
        }

        public bool IsSquareMatrix
        {
            get { return _rows == _cols; }
        }

        public bool IsAllZero
        {
            get
            {
                 for (int i = 0; i < _rows; ++i)
                    for (int j = 0; j < _cols; ++j)
                         if (this[i, j] != 0) return false;

                return true;
            }
        }

        public bool IsAllOnes
        {
            get
            {
                for (int i = 0; i < _rows; ++i)
                    for (int j = 0; j < _cols; ++j)
                        if (this[i, j] != 1) return false;

                return true;
            }
        }

        public bool IsSameAs(object target)
        {
            Matrix m = target as Matrix;
            for (int i = 0; i < _rows; ++i)
                for (int j = 0; j < _cols; ++j)
                    if (this[i, j] != m[i, j]) return false;

            return true;
        }
            

        public int NetworkId
        {
            get { return _networkId; }
            set { _networkId = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsBinaryMatrix
        {
            get
            {
                foreach (double d in this)
                {
                    if (Math.Abs(d) >= double.Epsilon && Math.Abs(d - 1.0) >= double.Epsilon)
                        return false;
                }
                return true;
            }
        }


        public void Clear()
        {
            Array.Clear(_data, 0, _rows * _cols);
            /*
            for (int i = 0; i < _data.Capacity; i++)
                _data[i] = 0.0;
            */
        }

        public virtual Matrix GetTranspose()
        {
            Matrix temp = new Matrix(_cols, _rows);

            temp._rowLabels.CopyFrom(_colLabels);
            temp._colLabels.CopyFrom(_rowLabels);

            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _cols; j++)
                    temp[j, i] = this[i, j];

            return temp;
        }

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs._rows != rhs._rows || lhs._cols != rhs._cols)
                throw new MatrixException("Cannot add matrices of different dimensions.");

            Matrix result = new Matrix(lhs);
            for (int i = 0; i < lhs._rows * lhs._cols; ++i)
                result._data[i] += rhs._data[i];

            return result;
        }

        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if (lhs._rows != rhs._rows || lhs._cols != rhs._cols)
                throw new MatrixException("Cannot add matrices of different dimensions.");

            Matrix result = new Matrix(lhs);
            for (int i = 0; i < lhs._rows * lhs._cols; ++i)
                result._data[i] -= rhs._data[i];

            return result;
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs._cols != rhs._rows)
                throw new MatrixException("Dimensions of matrices do not match for multiplication.");

            Matrix result = new Matrix(lhs._rows, rhs._cols);
            result.RowLabels.CopyFrom(lhs.RowLabels);
            result.ColLabels.CopyFrom(rhs.ColLabels);
            result.Clear();
            for (int r = 0; r < lhs._rows; r++)
                for (int c = 0; c < rhs._cols; c++)
                    for (int i = 0; i < lhs._cols; i++)
                        result[r, c] += lhs[r, i] * rhs[i, c];
            return result;
        }

        public static Matrix operator /(Matrix lhs, double rhs)
        {
            Matrix result = new Matrix(lhs);
            for (int i = 0; i < lhs._rows * lhs._cols; ++i)
                result._data[i] /= rhs;

            return result;
        }

        public Vector GetRowVector(int row)
        {
            Vector v = new Vector(_cols);
            for (int i = 0; i < _cols; ++i)
                v[i] = this[row, i];
            return v;
        }
        public Vector GetColVector(int col)
        {
            Vector v = new Vector(_rows);
            for (int i = 0; i < _rows; ++i)
                v[i] = this[i, col];
            return v;
        }
        public void SetRowVector(int row, Vector v)
        {
            for (int i = 0; i < _cols; ++i)
                this[row, i] = v[i];
        }
        public void SetColVector(int col, Vector v)
        {
            for (int i = 0; i < _rows; ++i)
                this[i, col] = v[i];
        }

        public virtual double this[string r, string c]
        {
            get
            {
                return this[_rowLabels[r], _colLabels[c]];
            }
            set
            {
                this[_rowLabels[r], _colLabels[c]] = value;
            }
        }

        public virtual double this[int r, int c]
        {
            get
            {
                if (r < 0 || r >= _rows || c < 0 || c >= _cols)
                    throw new IndexOutOfRangeException();

                if (outOfMemory)
                {
                    int fileToRead = DetermineFileToUse(r);
                    if (fileToRead != currentFileUsed)
                    {
                        // Write the previous matrix data to a file
                        TextWriter tw = new StreamWriter(Constants.tempDir + "/" +
                            Constants.TempMatrix + _count_id + currentFileUsed.ToString() + ".txt");
                        for (int row = 0; row < splitRowSize; row++)
                        {
                            string tempRow = "";
                            int begin = currentFileUsed * splitRowSize;
                            tempRow += (begin + row).ToString();
                            for (int col = 0; col < _cols; col++)
                            {
                                tempRow += " " + _data[row * _cols + col].ToString();
                            }
                            tw.WriteLine(tempRow);
                        }
                        tw.Close();
                        //System.IO.Directory.Delete(Constants.tempDir, true); // to delete recursively
                        currentFileUsed = fileToRead;

                        // Read new matrix data from new file
                        string[] lines = System.IO.File.ReadAllLines(Constants.tempDir + "/" +
                            Constants.TempMatrix + _count_id + fileToRead.ToString() + ".txt");

                        /*
                        string[] beginning = lines[0].Split();
                        string[] end = lines[lines.Length - 1].Split();
                        beginRow = int.Parse(beginning[0]); // may not need
                        endRow = int.Parse(end[0]); // may not need
                        */

                        _data = new double[splitRowSize * _cols];
                        for (int row = 0; row < lines.Length; row++)
                        {
                            string[] newRow = lines[row].Split();
                            for (int col = 1; col < newRow.Length; col++)
                            {
                                _data[row * _cols + (col - 1)] = double.Parse(newRow[col]);
                            }
                        }

                    }

                    int modifiedRow = r % splitRowSize;
                    return _data[modifiedRow * _cols + c];
                }
                else
                {
                    double div = 1.0;
                    if (_standardization == StandardizationType.Row)
                        div = GetRowSum(r);
                    else if (_standardization == StandardizationType.Column)
                        div = GetColSum(c);
                    else if (_standardization == StandardizationType.DiagonalMaximum)
                        div = Math.Max(_data[r * _cols + r], _data[c * _cols + c]);
                    else if (_standardization == StandardizationType.DiagonalMinimum)
                        div = Math.Max(_data[r * _cols + r], _data[c * _cols + c]);
                    else if (_standardization == StandardizationType.DiagonalRow)
                        div = _data[r * _cols + r];
                    else if (_standardization == StandardizationType.DiagonalColumn)
                        div = _data[c * _cols + c];

                    return _data[r * _cols + c] / div;
                }
            }
            set
            {
                if (r < 0 || r >= _rows || c < 0 || c >= _cols)
                    throw new IndexOutOfRangeException();

                if (outOfMemory)
                {
                    int fileToWrite = DetermineFileToUse(r);
                    if (fileToWrite != currentFileUsed)
                    {                       
                        TextWriter tw = new StreamWriter(Constants.tempDir + "/" +
                            Constants.TempMatrix + _count_id + currentFileUsed.ToString() + ".txt");
                        for (int row = 0; row < splitRowSize; row++)
                        {
                            string tempRow = "";
                            int begin = currentFileUsed * splitRowSize;
                            tempRow += (begin + row).ToString();
                            for (int col = 0; col < _cols; col++)
                            {
                                tempRow += " " + _data[row * _cols + col].ToString(); 
                            }
                            tw.WriteLine(tempRow);
                        }
                        tw.Close();
                        //System.IO.Directory.Delete(Constants.tempDir, true); // to delete recursively
                        currentFileUsed = fileToWrite;

                        // need to read in the new data before writing to it
                        // Read new matrix data from new file
                        string[] lines = System.IO.File.ReadAllLines(Constants.tempDir + "/" +
                            Constants.TempMatrix + _count_id + currentFileUsed.ToString() + ".txt");

                        /*
                        string[] beginning = lines[0].Split();
                        string[] end = lines[lines.Length - 1].Split();
                        beginRow = int.Parse(beginning[0]); // may not need
                        endRow = int.Parse(end[0]); // may not need
                        */
                        
                        _data = new double[splitRowSize * _cols];
                        for (int row = 0; row < lines.Length; row++)
                        {
                            string[] newRow = lines[row].Split();
                            for (int col = 1; col < newRow.Length; col++)
                            {
                                _data[row * _cols + (col - 1)] = double.Parse(newRow[col]);
                            }
                        }

                    }

                    int modifiedRow = r % splitRowSize;
                    _data[modifiedRow * _cols + c] = value;
                }
                else
                {
                    _data[r * _cols + c] = value;

                    _rowAvg[r] = _rowSqrAvg[r] = double.NaN;
                    _colAvg[c] = _colSqrAvg[c] = double.NaN;
                }
            }
        }

        public double GetRowAverage(int row)
        {
            if (row >= _rows)
                throw new ArgumentOutOfRangeException("row");

            if (double.IsNaN(_rowAvg[row]))
            {
                _rowAvg[row] = 0.0;
                for (int i = 0; i < _cols; ++i)
                    _rowAvg[row] += _data[row * _cols + i];

                _rowAvg[row] /= _cols;
            }

            return _rowAvg[row];
        }

        public double GetColAverage(int col)
        {
            if (col >= _cols)
                throw new ArgumentOutOfRangeException("col");

            if (double.IsNaN(_colAvg[col]))
            {
                _colAvg[col] = 0.0;
                for (int i = 0; i < _rows; ++i)
                    _colAvg[col] += _data[i * _cols + col];

                _colAvg[col] /= _rows;
            }

            return _colAvg[col];
        }

        public double GetRowSquareAverage(int row)
        {
            if (row >= _rows)
                throw new ArgumentOutOfRangeException("row");

            if (double.IsNaN(_rowSqrAvg[row]))
            {
                _rowSqrAvg[row] = 0.0;
                for (int i = 0; i < _cols; ++i)
                    _rowSqrAvg[row] += _data[row * _cols + i] * _data[row * _cols + i];

                _rowSqrAvg[row] /= _cols;
            }
            return _rowSqrAvg[row];
        }

        public double GetColSquareAverage(int col)
        {
            if (col >= _cols)
                throw new ArgumentOutOfRangeException("col");

            if (double.IsNaN(_colSqrAvg[col]))
            {
                _colSqrAvg[col] = 0.0;
                for (int i = 0; i < _rows; ++i)
                    _colSqrAvg[col] += _data[i * _cols + col] * _data[i * _cols + col];

                _colSqrAvg[col] /= _rows;
            }
            return _colSqrAvg[col];
        }

        public double GetRowSum(int row)
        {
            return GetRowAverage(row) * _cols;
        }

        public double GetColSum(int col)
        {
            return GetColAverage(col) * _rows;
        }

        public IEnumerable<double> GetRowEnumerator(int r)
        {
            for (int c = 0; c < _cols; ++c)
                yield return this[r, c];
        }

        public IEnumerable<double> GetColEnumerator(int c)
        {
            for (int r = 0; r < _rows; ++r)
                yield return this[r, c];
        }

        public double GetMaxInRow(int row)
        {
            if (row < 0 || row >= _rows)
                throw new IndexOutOfRangeException();

            double[] row_list = new double[Cols];
            for (int i = 0; i < Cols; ++i)
                row_list[i] = this[row, i];
            return Algorithms.MaxValue(row_list);
        }

        public double GetMaxInCol(int col)
        {
            if (col < 0 || col >= _rows)
                throw new IndexOutOfRangeException();

            double[] col_list = new double[Rows];
            for (int i = 0; i < Rows; ++i)
                col_list[i] = this[i, col];
            return Algorithms.MaxValue(col_list);
        }

        public int CountOnesInRow(int row)
        {
            if (row < 0 || row >= _rows)
                throw new IndexOutOfRangeException();

            int sum = 0;
            for (int i = 0; i < _cols; ++i)
                if (Math.Abs(this[row, i]) > 0.0)
                    ++sum;

            return sum;
        }

        public int CountOnesInCol(int col)
        {
            if (col < 0 || col >= _rows)
                throw new IndexOutOfRangeException();

            int sum = 0;
            for (int i = 0; i < _rows; ++i)
                if (Math.Abs(this[i, col]) > 0.0)
                    ++sum;

            return sum;
        }

        public object Clone()
        {
            return new Matrix(this);
        }

        public void CloneTo(object target)
        {
            Matrix m = target as Matrix;
            if (m != null)
            {
                if (m._rows == _rows && m._cols == _cols)
                {
                    _data.CopyTo(m._data, 0);
                    /*
                    // Copy the data from m._data to this._data
                    for (int i = 0; i < m._data.Capacity; i++)
                        m._data[i] = _data[i];
                    */
                    _rowAvg.CopyTo(m._rowAvg, 0);
                    _colAvg.CopyTo(m._colAvg, 0);
                    _rowSqrAvg.CopyTo(m._rowSqrAvg, 0);
                    _colSqrAvg.CopyTo(m._colSqrAvg, 0);
                    m._standardization = _standardization;
                    m._networkId = _networkId;
                    m._rowLabels.CopyFrom(_rowLabels);
                    m._colLabels.CopyFrom(_colLabels);
                }
                else
                    throw new MatrixException("Cannot clone to matrix of different dimensions.");
            }
            else
                throw new MatrixException("Target is not a matrix.");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _cols; ++j)
                {
                    sb.Append(this[i, j]);
                    sb.Append(' ');
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
        public string ToCSV()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _cols; ++j)
                {
                    
                    sb.Append(this[i, j]);
                    sb.Append(',');
                }                
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public void CopyLabelsFrom(Matrix m)
        {
            _rowLabels.CopyFrom(m._rowLabels);
            _colLabels.CopyFrom(m._colLabels);
        }

        public Matrix SubmatrixWithRows(List<int> rows)
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Can't make submatrix from non-square matrix.");

            if (rows == null)
                throw new ArgumentNullException("rows");

            if (rows.Count == 0)
                return this;

            int n = rows.Count;

            Matrix m = new Matrix(n);

            int r = 0, c = 0;
            for (int i = 0; i < _rows; ++i)
            {
                if (!rows.Contains(i))
                    continue;

                m.RowLabels[r] = this.RowLabels[i];

                c = 0;
                for (int j = 0; j < _rows; ++j)
                {
                    if (!rows.Contains(j))
                        continue;

                    m[r, c] = this[i, j];

                    ++c;
                }
                ++r;
            }

            m.ColLabels.CopyFrom(m.RowLabels);

            return m;
        }

        public Matrix SubmatrixWithRemovedRows(List<int> rows)
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Can't make submatrix from non-square matrix.");

            if (rows == null)
                throw new ArgumentNullException("rows");

            if (rows.Count == 0)
                return this;

            int n = _rows - rows.Count;

            Matrix m = new Matrix(n);

            int r = 0, c = 0;
            for (int i = 0; i < _rows; ++i)
            {
                if (rows.Contains(i)) 
                    continue;

                m.RowLabels[r] = this.RowLabels[i];

                c = 0;
                for (int j = 0; j < _rows; ++j)
                {
                    if (rows.Contains(j))
                        continue;

                    m[r, c] = this[i, j];

                    ++c;
                }
                ++r;
            }

            m.ColLabels.CopyFrom(m.RowLabels);

            return m;
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            foreach (double d in _data)
                yield return d;
        }

        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public void Normalize()
        {
            double d = 0.0;
            //for (int i = 0; i < _data.Capacity; ++i)
            for (int i = 0; i < _data.Length; ++i)
                d += _data[i] * _data[i];

            d = Math.Sqrt(d);
            //for (int i = 0; i < _data.Capacity; ++i)
            for (int i = 0; i < _data.Length; ++i)
                _data[i] /= d;
        }

        public void Binarize()
        { 
                for (int i = 0; i < _rows; ++i)
                    for (int j = 0; j < _cols; ++j)
                        if (this[i, j] != 0) this[i, j] = 1;
            }

        // Static methods to create useful matrices
        public static Matrix Zero(int rows, int cols)
        {
            Matrix m = new Matrix(rows, cols);
            m.Clear();
            return m;
        }

        public static Matrix Identity(int n)
        {
            Matrix m = Zero(n, n);
            for (int i = 0; i < n; ++i)
                m[i, i] = 1;
            return m;
        }

        public static Matrix Ones(int rows, int cols)
        {
            Matrix m = new Matrix(rows, cols);
            //for (int i = 0; i < m._data.Capacity; ++i)
            for (int i = 0; i < m._data.Length; ++i)
                m._data[i] = 1;
            return m;
        }

        #region IIndexable<double> Members

        public double this[int i]
        {
            get
            {
                return _data[i];
            }
            set
            {
                _data[i] = value;
            }
        }

        public int Length
        {
            get { return _data.Length; }
            //get { return _data.Capacity; }
        }

        #endregion

        public void ZeroDiagonal()
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Cannot zero diagonal of non-square matrix.");

            for (int i = 0; i < _rows; ++i)
                this[i, i] = 0.0;
        }

        public Matrix GetDiagonalMatrix()
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Cannot get diagonal matrix from non-square matrix.");
            Matrix tmp = new Matrix(this.Rows);
            tmp.Clear();

            for (int i = 0; i < _rows; ++i)
                tmp[i, i] = this[i, i];

            return tmp;
        }

        public Vector GetDiagonalVector()
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Cannot get diagonal matrix from non-square matrix.");
            Vector tmp = new Vector(this.Rows);
            tmp.Clear();

            for (int i = 0; i < _rows; ++i)
                tmp[i] = this[i, i];

            tmp.Labels.CopyFrom(_rowLabels);
            tmp.NetworkId = NetworkId;

            return tmp;
        }

        public void SetDiagonalFromVector(Vector v)
        {
            if (!IsSquareMatrix)
                throw new MatrixException("Cannot get diagonal matrix from non-square matrix.");
            if (Rows != v.Size)
                throw new MatrixException("Cannot set diagonal. Vector is wrong size.");

            for (int i = 0; i < _rows; ++i)
                this[i, i] = v[i];
        }

        internal double sum()
        {
            double sum = 0;
            foreach( double i in _data)
            {
                sum+=i;
            }
            return sum;
        }

        private void WriteMemoryToDisk()
        {

        }

        // Determine which file contains the necessary rows required
        // row: the actual row number of the matrix
        private int DetermineFileToUse(int row)
        {
            return (row / splitRowSize);
        }
    }
}
