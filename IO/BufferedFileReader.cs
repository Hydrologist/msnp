using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Network.IO
{
    public sealed class BufferedFileReader : IDisposable
    {
        public enum Type
        {
            Matrix, Dyadic, Vector
        }

        private readonly string _filename;
        private readonly int _bufferSize;
        private readonly List<string> _lines;
        private readonly NetworkIdTable _networkIdTable;
        private readonly BufferedStream _stream;
        private Type _type;

        private int _curLine = -1;
        private byte[] _data;

        private bool _isStreamLoaded;

        public BufferedFileReader(string filename, int bufferSize) 
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("File does not exist!");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");

            try
            {
                _stream = new BufferedStream(File.OpenRead(filename));
            }
            catch (IOException e)
            {
                MessageBox.Show("Could not open file: " + e.Message);
                throw e;
            }

            _filename = filename;
            _bufferSize = bufferSize;
            _lines = new List<string>();
            _networkIdTable = new NetworkIdTable();
            _data = new byte[bufferSize];
            _isStreamLoaded = false;

            GuessType();

            TryLoadFile();
        }

        public BufferedFileReader(string filename, Type type)
            : this(filename, Constants.BufferSize) { }
        public BufferedFileReader(string filename)
            : this(filename, Constants.BufferSize) { }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public string Filename
        {
            get { return _filename; }
        }

        public Type FileType
        {
            get { return _type; }
        }

        public bool TryLoadFile()
        {
            if (_isStreamLoaded)
                return false;

            int bytesRead = _stream.Read(_data, 0, _bufferSize);



            // Go through and do each line
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytesRead; ++i)
            {
                // Build this line
                char c = (char)_data[i];
                if (c != '\n' && c != '\r')
                {
                    if (c != ',' || (i > 0 && (char)_data[i - 1] != ',')) 
                        sb.Append(c);
                }
                else if (c == '\n' || c == '\r') // Changed 1/20/11
                {
                    //++i;
                    if (sb.Length > 0) // Empty lines are useless
                    {
                        ParseLine(sb);
                        sb.Remove(0, sb.Length);
                    }
                }
            }

            // Seek back to start of this line
            if (sb.Length > 0)
            {
                if (_stream.Position >= _stream.Length)
                    ParseLine(sb);
                else
                    _stream.Seek(-1 * sb.Length, SeekOrigin.Current);
            }

            //if (_stream.Position == _stream.Length)
            //{
            //    _isStreamLoaded = true;
            //    _stream.Close();
            //}
            if(_stream.Position == _stream.Length)
                closeStream();

            return true;
        }

        public void closeStream()
        {
            _isStreamLoaded = true;
            _stream.Close();
        }

        private void ParseLine(StringBuilder sb)
        {
            if (sb[sb.Length - 1] == ',') // Remove trailing commas
                sb.Remove(sb.Length - 1, 1);

            string curLine = sb.ToString();

            _lines.Add(curLine);
            

            // Add year to table
            ParseStringForNetworkId(curLine);
        }

        private void ParseStringForNetworkId(string curLine)
        {
            string firstPart = curLine;
            int commaPos = curLine.IndexOf(',');
            if (commaPos > 0)
                firstPart = curLine.Substring(0, curLine.IndexOf(','));
            int networkId;
            if (int.TryParse(firstPart, out networkId))
            {
                if (_type != Type.Matrix || curLine.LastIndexOf(',') <= commaPos)
                    AddNetworkIdToTable(networkId);
            }
        }

        private void AddNetworkIdToTable(int networkId)
        {
            if (!_networkIdTable.ContainsNetworkId(networkId))
            {
                _networkIdTable[networkId] = _lines.Count - 2;
            }
        }

        public void ResetLine()
        {
            _curLine = -1;
        }

        public void GoToLine(int line)
        {
            _curLine = line;
        }

        public string ReadLine()
        {
            if (_curLine < 0)
                _curLine = -1;

            ++_curLine;
            while (_curLine >= _lines.Count && TryLoadFile())
                ;

            if (_curLine >= _lines.Count)
                return null;

            return _lines[_curLine];
        }

        public bool EndOfStream
        {
            get
            { 
                return _curLine >= _lines.Count && _isStreamLoaded; 
            }
        }

        public bool JumpToNetworkId(int networkId)
        {
            while (!_networkIdTable.ContainsNetworkId(networkId) && networkId > _networkIdTable.MaxNetworkId && TryLoadFile())
                ;

            if (_networkIdTable.ContainsNetworkId(networkId))
            {
                _curLine = _networkIdTable[networkId];
                return true;
            }

            return false;
        }

        // If second param is true will keep trying forwards
        // Otherwise backwards
        // Returns successful year
        public int JumpToNetworkId(int networkId, bool forwards)
        {
            if (_networkIdTable.ContainsNetworkId(networkId))
            {
                _curLine = _networkIdTable[networkId];
                return networkId;
            }

            int newNetworkId;
            if (forwards)
            {
                while ((newNetworkId = _networkIdTable.GetNextNetworkId(networkId)) < 0 && TryLoadFile())
                    ;
            }
            else
            {
                while ((newNetworkId = _networkIdTable.GetPreviousNetworkId(networkId)) < 0 && TryLoadFile())
                    ;
            }

            if (_networkIdTable.ContainsNetworkId(newNetworkId))
            {
                _curLine = _networkIdTable[newNetworkId];
                return newNetworkId;
            }
            return -1;
        }


        // Returns the number of lines for this year
        public int CountLines(int networkId)
        {
            if (!_networkIdTable.ContainsNetworkId(networkId))
                return -1; // This year doesn't exist
            
            int start = _networkIdTable[networkId];
            int stop;
            while ((stop = _networkIdTable.GetNextNetworkId(networkId)) < 0 && TryLoadFile())
                ;

            if (stop < 0)
                stop = _lines.Count - 1;
            else
                stop = _networkIdTable[stop];

            return stop - start;
        }

        public Dictionary<string, int> GetDyadicLabels(int networkId)
        {
            if (_type != Type.Dyadic && _type != Type.Vector)
                throw new Exception("Attempting to read dyadic labels from non-dyadic file!");

            if (!_networkIdTable.ContainsNetworkId(networkId))
                throw new Exception("That year is not present in the file.");

            Dictionary<string, int> labels = new Dictionary<string, int>();

            int prevLine = _curLine;

            JumpToNetworkId(networkId);

            int lineCount = CountLines(networkId);

            for (int i = 0; i < lineCount; ++i)
            {
                string[] parts = ReadLine().Split(',');
                if (_type == Type.Dyadic)
                {
                    if (!labels.ContainsKey(parts[2]))
                        labels.Add(parts[2], i);
                    else
                        break;
                }
                else // Monadic file
                {
                    if (!labels.ContainsKey(parts[1]))
                        labels.Add(parts[1], i);
                    else
                        break;
                }
            }

            _curLine = prevLine;

            return labels;
        }

        public int CountVarsInDyadicFile()
        {
            if (_type == Type.Vector)
                return 1;

            if (_type != Type.Dyadic)
                throw new FileLoadException("Can only count number of vars in a dyadic file!");

            while (_lines.Count < 2 && TryLoadFile())
                ;

            return _lines[1].Split(',').Length - 3;
        }

        private void GuessType()
        {
            string s;
            using (StreamReader sr = new StreamReader(_filename))
            {
                s = sr.ReadLine();

                while (s.Contains(",,"))
                    s = s.Replace(",,", "");

                int splitLength = s.Split(',').Length;

                if (splitLength == 3)
                    _type = Type.Vector;
                else if (splitLength > 3)
                    /*
                    if (splitLength == 4)
                        _type = Type.Dyadic;
                    else
                        _type = Type.MultipleDyadic;
                    */
                    _type = Type.Dyadic;
                else
                {
                    s = sr.ReadLine();
                    splitLength = s.Split(',').Length;

                    if (splitLength == 2)
                        _type = Type.Vector;
                    else
                        _type = Type.Matrix;
                }
            }
        }

        public int GetPreviousNetworkId(int networkId)
        {
            return _networkIdTable.GetPreviousNetworkId(networkId);
        }

        public int MinNetworkId
        {
           get { return _networkIdTable.MinNetworkId; }
        }

        public int MaxNetworkId
        {
            get { return _networkIdTable.MaxNetworkId; }
        }

        // need a member to get the top line of the file
        public string TopLine
        {
            get { return _lines[0]; }
        }
    }
}
