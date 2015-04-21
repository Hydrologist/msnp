using System;
using System.Collections.Generic;
using System.Text;
using Network;
using System.IO;

namespace Network
{
    // This wraps dyadic/matrix files and presents a common interface
    public class DataFileWrapper : BufferedFile
    {
        // We will store all the numbers in a data list
        protected List<double> data;
        protected int curData;
        protected int curYear;
        private const double EOY = 123456789.987654321; // end of year marker

        public double NextNumber()
        {
            if (++curData < data.Count && data[curData] != EOY)
                return data[curData];
            else
                throw new ArgumentOutOfRangeException();
        }
        public double PrevNumber()
        {
            if (--curData >= 0 && data[curData] != EOY)
                return data[curData];
            else
                throw new ArgumentOutOfRangeException();
        }
        public int NextYear()
        {
            do
            {
                ++curYear;
            } while (!yearTable.ContainsKey(curYear));
            curData = yearTable[curYear];
            return curYear;
        }
        public int PrevYear()
        {
            do
            {
                --curYear;
            } while (!yearTable.ContainsKey(curYear));
            curData = yearTable[curYear];
            return curYear;
        }

       /* public virtual void LoadFile()
        {
            if (fileName == null)
            {
                throw new FileLoadException();
            }

            Clear();
            // Read in entire file
            FileInfo f = new FileInfo(fileName);
            FileStream fin = f.OpenRead();

            byte[] data = new byte[f.Length];
            fin.Read(data, 0, (int)f.Length);

            // Let's go through loading into the data array
            for (int i = 0; i < (int)f.Length; ++i)
            {
                // Add this number
                if ((char)data[i] == '\r' || (char)data[i] == ',')
                {
                    ++i;
                    if (s.ToString() != "") // Empty lines are useless
                    {
                        lines.Add(s.ToString());
                        s.Remove(0, s.Length);
                    }
                }
                else
                {
                    if ((char)data[i] != ',' || (char)data[i - 1] != ',')
                        s.Append((char)data[i]);
                }

                // Check for a year to store
                int year;
                if (int.TryParse(s.ToString(), out year))
                {
                    if (year > 1500 && year < 10000 && !yearTable.ContainsKey(year))
                        yearTable[year] = lines.Count - 1;
                }
            }
        }*/


    }
}
