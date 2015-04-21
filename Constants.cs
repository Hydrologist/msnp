using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public sealed class Constants
    {
        private Constants() { }

        //public const int BufferSize = 1048576;
        public const int BufferSize = int.MaxValue / 4;
        public const int CliqueByCliqueOverlapCutoff = 5000;

        public const string FileSelected = "File";

        public const string tempDir = "TempFolder";
        public const string TempMatrix = "TempMatrix";
        //public const int MaximumNumberOfCliques = 50000;
        //public const int MaximumNumberOfCliques = 16384;
        public const int MaximumNumberOfCliques = 500000;
        public const int MaxMatrixSize = 250000000;
        //public const int MaxMatrixSize = 20;

        public const double Epsilon = 0.0000000000001;

        public const int MinimumNumberOfConvergenceSteps = 20;
    }
}
