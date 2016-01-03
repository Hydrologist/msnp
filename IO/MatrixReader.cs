using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Network.Matrices;
using System.Threading;

namespace Network.IO
{
    public sealed class MatrixReader
    {
        private MatrixReader() { }

        public static Matrix ReadMatrixFromFile(string filename)
        {
            return ReadMatrixFromFile(filename, 0, 0);
        }
        public static Matrix ReadMatrixFromFile(string filename, int networkId)
        {
            return ReadMatrixFromFile(filename, networkId, 0);
        }
        public static Matrix ReadMatrixFromFile(string filename, int networkId, int dyadicVariable)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            BufferedFileReader reader = BufferedFileTable.GetFile(filename);
            lock (reader)
            {
                switch (reader.FileType)
                {
                    case BufferedFileReader.Type.Matrix:
                        return ReadMatrixFromMatrixFile(reader, networkId);
                    case BufferedFileReader.Type.Dyadic:
                        return ReadMatrixFromDyadicFile(reader, networkId, dyadicVariable);
                    case BufferedFileReader.Type.Vector:
                        return ReadMatrixFromVectorFile(reader, networkId);
                    default:
                        throw new FileLoadException("Invalid file type.");
                }
            }
        }

        public static Vector ReadVectorFromFile(string filename)
        {
            return ReadVectorFromFile(filename, -1);
        }
        public static Vector ReadVectorFromFile(string filename, int networkId)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            BufferedFileReader reader = BufferedFileTable.GetFile(filename);
            lock (reader)
            {
                switch (reader.FileType)
                {
                    case BufferedFileReader.Type.Vector:
                        return ReadVectorFromVectorFile(reader, networkId);
                    default:
                        throw new FileLoadException("Invalid file type.");
                }
            }
        }


        public static Matrix ReadAttributeVector(string filename, int netID) //working here
        {
            return createTransposeMatrix(filename, netID);
        }

        public static Matrix createTransposeMatrix(string filename, int netID) // working here - working for integers and random nodes
        {
            Dictionary <int, double> d = new Dictionary<int,double>();

            BufferedFileReader reader = BufferedFileTable.GetFile(filename);
            reader.GoToLine(0);

            while (!reader.EndOfStream) //reading file
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                string[] line_parts = line.Split(',');

                if (Int64.Parse(line_parts[0]) == netID)
                {
                    d.Add(ExtractNode(line_parts[1]), ExtractDouble(line_parts[2])); // need to work here for string case
                }
            }

            Matrix m = new Matrix(d.Count, d.Count);  //transpose matrix


            //calculation
            for (int i = 0; i < d.Count; i++)
            {
                for (int j = 0; j < d.Count; j++)
                {
                    m[i,j] = d[i + 1] * d[j + 1]; 
                }
            }
           
           return m;
        }
        
        private static Matrix ReadMatrixFromVectorFile(BufferedFileReader reader, int networkId)
        {
            networkId = reader.JumpToNetworkId(networkId, true);

            Matrix m = new Matrix(reader.CountLines(networkId));
            m.NetworkId = networkId;
            for (int i = 0; i < m.Rows; ++i)
            {
                string s = reader.ReadLine();
                string[] parts = s.Split(',');
                m[i, i] = ExtractDouble(parts[parts.Length - 1]);
                m.RowLabels[i] = m.ColLabels[i] = parts[parts.Length - 2];
            }

            return m;
        }

        private static Vector ReadVectorFromVectorFile(BufferedFileReader reader, int networkId)
        {
            networkId = reader.JumpToNetworkId(networkId, true);

            Vector v = new Vector(reader.CountLines(networkId));
            v.NetworkId = networkId;
            for (int i = 0; i < v.Size; ++i)
            {
                string s = reader.ReadLine();
                string[] parts = s.Split(',');
                v[i] = ExtractDouble(parts[parts.Length - 1]);
                v.Labels[i] = parts[parts.Length - 2];
            }

            return v;
        }

        private static Matrix ReadMatrixFromDyadicFile(BufferedFileReader reader, int networkId, int dyadicVariable)
        {
            networkId = reader.JumpToNetworkId(networkId, true);

            Dictionary<string, int> labels = reader.GetDyadicLabels(networkId);
            int rows = labels.Count;

            Matrix matrix = new Matrix(rows, rows);
            //matrix.NetworkId = networkId;
            //matrix.NetworkId = int.Parse("1" + matrix.NetworkId); // new
            matrix.RowLabels.SetLabels(labels.Keys);
            matrix.ColLabels.SetLabels(labels.Keys);

            int totalLines = reader.CountLines(networkId);
            for (int i = 0; i < totalLines; ++i)
            {
                string s = reader.ReadLine();

                string[] parts = s.Split(',');

                if (parts.Length < 3 + dyadicVariable)
                    throw new FileLoadException("Missing value for line: " + s);

                matrix[labels[parts[1]], labels[parts[2]]] = ExtractDouble(parts[3 + dyadicVariable]);
            }
            /*
            if (networkId < 1000)
                networkId = int.Parse("1" + networkId);
            else
                networkId = int.Parse("2" + networkId);
            */
            matrix.NetworkId = networkId;
            
            
            return matrix;
        }

        public static List<Matrix> ReadMatrixFromMultipleDyadicFile(string filename, int networkId)
        {
            BufferedFileReader reader = BufferedFileTable.GetFile(filename);
            lock (reader)
            {
                networkId = reader.JumpToNetworkId(networkId, true);

                Dictionary<string, int> labels = reader.GetDyadicLabels(networkId);
                int rows = labels.Count;

                // initialize the matrices
                List<Matrix> multipleMatrices = new List<Matrix>();
                
                string[] topLabels = reader.TopLine.Split(',');
                //string[] labels = new string[reader.CountVarsInDyadicFile()];

                for (int var = 0; var < reader.CountVarsInDyadicFile(); var++)
                {
                    Matrix matrix = new Matrix(rows, rows);
                    matrix.NetworkId = networkId;
                    matrix.Name = topLabels[var + 3];
                    matrix.RowLabels.SetLabels(labels.Keys);
                    matrix.ColLabels.SetLabels(labels.Keys);

                    multipleMatrices.Add(matrix);
                }

                int totalLines = reader.CountLines(networkId);

                for (int i = 0; i < totalLines; ++i)
                {
                    string s = reader.ReadLine();
                    string[] parts = s.Split(',');

                    //if (parts.Length < 3 + reader.CountVarsInDyadicFile())
                      //  throw new FileLoadException("Missing value for line: " + s);

                    for (int var = 0; var < reader.CountVarsInDyadicFile(); var++)
                    {
                        multipleMatrices[var][labels[parts[1]], labels[parts[2]]] = ExtractDouble(parts[3 + var]);
                    }
                }
                /*
                if (networkId < 1000)
                    networkId = int.Parse("1" + networkId);
                else
                    networkId = int.Parse("2" + networkId);
                */


                return multipleMatrices;
            }
        }


        private static Matrix ReadMatrixFromMatrixFile(BufferedFileReader reader, int networkId)
        {
            networkId = reader.JumpToNetworkId(networkId, true);
            reader.ReadLine(); // Skip first line with the network id
            /*
            string tempLabels = reader.ReadLine();
            if (tempLabels == null)
                return null;
            */  

            string[] colLabels = reader.ReadLine().Split(',');
            //string[] colLabels = tempLabels.Split(',');
            int rows = reader.CountLines(networkId) - 2; // Subtract off header columns
            int cols = colLabels.Length - 1;

            Matrix matrix = new Matrix(rows, cols);
            matrix.NetworkId = networkId;
            matrix.ColLabels.SetLabels(colLabels);

            for (int row = 0; row < rows; ++row)
            {
                string temp = reader.ReadLine();
                if (temp == null)
                    break;
                //string[] parts = reader.ReadLine().Split(',');
                string[] parts = temp.Split(',');

                if (parts.Length > cols + 1) // one extra for header
                    throw new FileLoadException("Matrix file has too many entries for network id: " + networkId.ToString() + ", row " + parts[0]);

                if (parts.Length == 0)
                    throw new FileLoadException("Matrix file has no entries for network id: " + networkId.ToString());

                matrix.RowLabels[row] = parts[0];

                for (int i = 1; i < parts.Length; ++i)
                    matrix[row, i - 1] = ExtractDouble(parts[i]);
            }
            
            reader.closeStream();
            reader.Dispose(); /* Change made 12/3/2010 - PM */
            
            return matrix; 
        }

        private static double ExtractDouble(string s)
        {
            double tmp;

            if (!double.TryParse(s, out tmp))
                throw new FileLoadException("Expecting floating point value: " + s);

            return tmp;
        }
        
        private static int ExtractNode(string s) //working
        {
            int node = 0;
            string number = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsDigit(s[i]))
                    number += s[i];
            }

            node = Int32.Parse(number);

            return node;
        }
    }
}
