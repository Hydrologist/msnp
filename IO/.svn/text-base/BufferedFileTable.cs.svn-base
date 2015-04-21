using System;
using System.Collections.Generic;
using System.Text;
using Network;

namespace Network.IO
{
    public sealed class BufferedFileTable
    {
        // Uses Singleton pattern so we only have one table
        private readonly static BufferedFileTableImplementation _instance = new BufferedFileTableImplementation();
        private BufferedFileTable() { }

        public static BufferedFileReader GetFile(string filename)
        {
            return _instance.GetFile(filename);
        }

        public static bool ContainsFile(string filename)
        {
            return _instance.ContainsFile(filename);
        }

        public static void AddFile(string filename)
        {
            _instance.AddFile(filename);
        }

        public static void RemoveFile(string filename)
        {
            _instance.RemoveFile(filename);
        }

        public static void Clear()
        {
            _instance.Clear();
        }

        private class BufferedFileTableImplementation
        {
            private Dictionary<string, BufferedFileReader> _map;

            public BufferedFileTableImplementation()
            {
                _map = new Dictionary<string, BufferedFileReader>();
            }

            public BufferedFileReader this[string filename]
            {
                get { return GetFile(filename); }
            }

            public BufferedFileReader GetFile(string filename)
            {
                AddFile(filename);
                return _map[filename];
            }

            public bool ContainsFile(string filename)
            {
                return _map.ContainsKey(filename);
            }

            public void AddFile(string filename)
            {
                if (!_map.ContainsKey(filename))
                    _map[filename] = new BufferedFileReader(filename);
            }

            public void RemoveFile(string filename)
            {
                if (_map.ContainsKey(filename))
                {
                    _map[filename].Dispose();
                    _map.Remove(filename);
                }
            }

            // Removes all files
            public void Clear()
            {
                foreach (string key in _map.Keys)
                    _map[key].Dispose();

                _map.Clear();
            }
        }
    }
}
