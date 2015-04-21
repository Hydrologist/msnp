using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using Network.Cliques;
using Network.IO;
using System.ComponentModel;
using RandomUtilities;
using System.Windows.Forms;

namespace Network
{
    public sealed class NetworkFormationSimulation
    {
        private NetworkFormationSimulation() { }

        private static Vector UpdateEAMatrix(Vector AO, Matrix Q, Matrix EA)
        {
            int n = AO.Size; 

            Vector MQ = new Vector(n);
            Vector SEA = Vector.Zero(n);
            Vector RS = null;

            while (true)
            {
                for (int i = 0; i < SEA.Size; ++i)
                    SEA[i] = EA.GetRowSum(i);

                RS = AO - SEA;

                for (int i = 0; i < MQ.Size; ++i)
                    MQ[i] = Algorithms.MaxValue<double>(Q.GetRowEnumerator(i));

                for (int i = 0; i < EA.Rows; ++i)
                {
                    if (RS[i] < double.Epsilon)
                        continue;

                    for (int j = 0; j < EA.Cols; ++j)
                        if (EA[i, j] == 0 && Q[i, j] == MQ[i])
                            EA[i, j] = Q[i, j];
                }

                for (int i = 0; i < Q.Rows; ++i)
                    for (int j = 0; j < Q.Cols; ++j)
                        if (Q[i, j] == MQ[i])
                            Q[i, j] = 0;

                if (Algorithms.MaxValue<double>(RS) < double.Epsilon)
                    break;
                if (Algorithms.MaxValue<double>(MQ) < double.Epsilon)
                    break;
            }

            return RS;
        }




        /// <summary>
        /// Simulates stage one of the SIMPLIFIED realist network formation simulation.
        /// </summary>
        /// <param name="C">Capability matrix</param>
        /// <param name="R">Policy relevance matrix (SRG)</param>
        /// <param name="M">MID matrix</param>
        /// <returns>Expected alliance matrix</returns>
        private static Matrix SimulateSimplifiedRealistStageOne(Matrix C, Matrix R, Matrix M, string outputFile, double br)
        {
            int n = C.Rows;
            Matrix SRC = R * C;
            Vector SRGC = new Vector(n);
            for (int i = 0; i < n; ++i)
                SRGC[i] = SRC.GetRowSum(i) * br;
            Vector AOC = new Vector(n);
            for (int i = 0; i < n; ++i)
            {
                if (SRGC[i] <= C[i, i]) AOC[i] = 0;
                else AOC[i] = 1 - C[i, i] / SRGC[i];
            }

            Matrix F = Matrix.Ones(n, n);
            F.ZeroDiagonal();
            Matrix PAN = F - R;
            Matrix PAC = PAN * C;

            Matrix EE = M * M;
            EE.ZeroDiagonal();
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EE[i, j] != 0) EE[i, j] = 1;

            Matrix EEC = EE * C;
            Vector EEM = new Vector(n);

            for (int i = 0; i < n; ++i)
                EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

            Matrix EA = new Matrix(n, n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EEC[i, j] == EEM[i] && PAN[i, j] == 1 && AOC[i] != 0)
                        EA[i, j] = EEC[i, j];
                    else
                        EA[i, j] = 0;



            Vector SEA = new Vector(n);

            Matrix previousEA = new Matrix(n);

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (EEC[i, j] == EEM[i]) EEC[i, j] = 0;

                for (int i = 0; i < n; ++i)
                    EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        //if (AOC[i] != 0 && EEC[i, j] == EEM[i] && EA[i, j] == 0)
                        //    EA[i, j] = EEC[i, j];

                        if (AOC[i] == 0 || EA[i, j] != 0);
                        else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
                            EA[i, j] = EEC[i, j];
                        else EA[i, j] = 0;
                    }




            } while ((!previousEA.IsSameAs(EA)) && !EEC.IsAllZero && !AOC.IsZeroVector);

            if (!AOC.IsZeroVector)
            {

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (PAC[i, j] > 0 && EEC[i, j] != 0) PAC[i, j] = 0;

                Vector PAM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                            EA[i, j] = PAC[i, j];

                EA.CopyLabelsFrom(C);
                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (PAC[i, j] == PAM[i]) PAC[i, j] = 0;

                    for (int i = 0; i < n; ++i)
                        PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                                EA[i, j] = PAC[i, j];

                    EA.CopyLabelsFrom(C);
                } while ((!previousEA.IsSameAs(EA)) && !PAC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero

            Matrix EAT = EA.GetTranspose();

            Matrix EAF = new Matrix(n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    EAF[i, j] = EA[i, j] * EAT[i, j];

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (EAF[i, j] != 0) BEA[i, j] = 1;
                    else BEA[i, j] = 0;
                }


            return BEA;
        }

        /// <summary>
        /// Simulates stage two of the realist network formation simulation.
        /// </summary>
        /// <param name="C">Capability matrix for stage two</param>
        /// <param name="R">Policy relevance matrix (SRG)</param>
        /// <param name="EAF">EAF matrix from stage one</param>
        /// <param name="M">MID matrix</param>
        /// <returns>Expected alliance matrix</returns>

        /// <returns>EA matrix for stage two</returns>
        private static Matrix UpdateSimplifiedRealistStageTwo(Matrix C, Matrix R, Matrix M, Matrix BEA, string outputFile, double br)
        {
            int n = C.Rows;

            Matrix AE = M * BEA;

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (R[i, j] == 1 || AE[i, j] > 0 ) R[i, j] = 1;
                    else R[i, j] = 0;
                }
 

            R.ZeroDiagonal();
            Matrix SRC = R * C;
 

            Vector SRGC = new Vector(n);
            for (int i = 0; i < n; ++i)
                SRGC[i] = SRC.GetRowSum(i) * br;

            Matrix EAC = BEA * C;


            Vector AOC = new Vector(n);
            for (int i = 0; i < n; ++i)
            {
                if (EAC.GetRowSum(i) + C[i, i] >= SRGC[i]) AOC[i] = 0;
                else AOC[i] = 1 - ((EAC.GetRowSum(i) + C[i, i]) / SRGC[i]);
            }

            Matrix F = Matrix.Ones(n, n);
            F.ZeroDiagonal();
            Matrix PAN = F - R;
            Matrix PAC = PAN * C;

            Matrix EE = M * M;
            EE.ZeroDiagonal();
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EE[i, j] != 0) EE[i, j] = 1;

            Matrix EEC = EE * C;
 
            Vector EEM = new Vector(n);

            for (int i = 0; i < n; ++i)
                EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

            Matrix EA = new Matrix(n, n);
 

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EAC[i, j] > 0 || AOC[i] <= 0)
                        EA[i, j] = EAC[i, j];
                    else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
                        EA[i, j] = EEC[i, j];
                    else
                        EA[i, j] = 0;

            //Copied from Stage 1

            Vector SEA = new Vector(n);

            Matrix previousEA = new Matrix(n);


            Matrix TempEEC = new Matrix(n);

            EEC.CloneTo(TempEEC);

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }
 
                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (EEC[i, j] == EEM[i]) EEC[i, j] = 0;

 

                for (int i = 0; i < n; ++i)
                    EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    { 
                        if (AOC[i] == 0 || EA[i, j] != 0) ;
                        else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
                            EA[i, j] = EEC[i, j];
                        else EA[i, j] = 0;
                    }



                //for (int i = 0; i < n; ++i)
                //    for (int j = 0; j < n; ++j)
                //    {
                //        if (AOC[i] == 0 || EA[i, j] > 0) ;
                //        else if (EEC[i, j] == EEM[i]) EA[i, j] = EEC[i, j];
                //        else EA[i, j] = 0;
                //    }
            } while ((!previousEA.IsSameAs(EA)) && !EEC.IsAllZero && !AOC.IsZeroVector);


            if (!AOC.IsZeroVector)
            {

                //for (int i = 0; i < n; ++i)
                //    for (int j = 0; j < n; ++j)
                //        if (PAC[i, j] > 0 && TempEEC[i, j] != 0) PAC[i, j] = 0;

                //PAC.CopyLabelsFrom(C);
                //MatrixWriter.WriteMatrixToMatrixFile(PAC, outputFile);
                Vector PAM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                            EA[i, j] = PAC[i, j];


                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    //Matrix output = new Matrix(n, n + 4);
                    //for (int i = 0; i < n; i++)
                    //    for (int j = 0; j < n; j++)
                    //        output[i, j] = EA[i, j];

                    //for (int i = 0; i < n; ++i)
                    //    output[i, n] = SEA[i];
                    //for (int i = 0; i < n; ++i)
                    //    output[i, n + 1] = SRGC[i];
                    //for (int i = 0; i < n; ++i)
                    //    output[i, n + 2] = AOC[i];
                    //for (int i = 0; i < n; ++i)
                    //    output[i, n + 3] = 1111;


                    //output.CopyLabelsFrom(C);
                    //MatrixWriter.WriteMatrixToMatrixFile(output, outputFile);

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (PAC[i, j] == PAM[i]) PAC[i, j] = 0;

                    //PAC.CopyLabelsFrom(C);
                    //MatrixWriter.WriteMatrixToMatrixFile(PAC, outputFile);
                    for (int i = 0; i < n; ++i)
                        PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                                EA[i, j] = PAC[i, j];

                } while ((!previousEA.IsSameAs(EA)) && !PAC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero

            Matrix EAT = EA.GetTranspose();

            Matrix EAF = new Matrix(n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    EAF[i, j] = EA[i, j] * EAT[i, j];

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (EAF[i, j] != 0) BEA[i, j] = 1;
                    else BEA[i, j] = 0;
                }


            return BEA;

        }

        private static Matrix UpdateSimplifiedLiberalStageTwo(Matrix C, Matrix R, Matrix M, Matrix D, Matrix JC, Matrix BEA, string outputFile, double br)
        {
            int n = C.Rows;

            Matrix AE = M * BEA;

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (R[i, j] == 1 || AE[i, j] > 0 ) R[i, j] = 1;
                    else R[i, j] = 0;
                }



            R.ZeroDiagonal();
            Matrix SRC = R * C;


            //R.CopyLabelsFrom(C);
            //MatrixWriter.WriteMatrixToMatrixFile(R, outputFile);

            Vector SRGC = new Vector(n);
            for (int i = 0; i < n; ++i)
                SRGC[i] = SRC.GetRowSum(i) * br;

            Matrix EAC = BEA * C;


            Vector AOC = new Vector(n);
            for (int i = 0; i < n; ++i)
            {
                if (EAC.GetRowSum(i) + C[i, i] >= SRGC[i]) AOC[i] = 0;
                else AOC[i] = 1 - ((EAC.GetRowSum(i) + C[i, i]) / SRGC[i]);
            }

            Matrix F = Matrix.Ones(n, n);
            F.ZeroDiagonal();
            Matrix PAN = F - R;
            Matrix PAC = PAN * C;

            Matrix EE = M * M;
            EE.ZeroDiagonal();
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EE[i, j] != 0) EE[i, j] = 1;

            Matrix EEC = EE * C;

            //PAN.CopyLabelsFrom(C);
            //MatrixWriter.WriteMatrixToMatrixFile(PAN, outputFile);
            //EEC.CopyLabelsFrom(C);
            //MatrixWriter.WriteMatrixToMatrixFile(EEC, outputFile);
            Vector EEM = new Vector(n);

            for (int i = 0; i < n; ++i)
                EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

            Matrix EA = new Matrix(n, n);

            //for (int i = 0; i < n; ++i)
            //    for (int j = 0; j < n; ++j)
            //        if (EAC[i, j] > 0 || AOC[i] <= 0)
            //            EA[i, j] = EAC[i, j];
            //        else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
            //            EA[i, j] = EEC[i, j];
            //        else
            //            EA[i, j] = 0;

            //Copied from Stage 1



            Matrix TempEEC = new Matrix(n);

            EEC.CloneTo(TempEEC);

            Matrix DC = D * C;

            Vector DM = new Vector(n);
            for (int i = 0; i < n; ++i)
                DM[i] = Algorithms.MaxValue<double>(DC.GetRowEnumerator(i));

            //Matrix output = new Matrix(n, n + 2);
            //for (int i = 0; i < n; i++)
            //    for (int j = 0; j < n; j++)
            //        output[i, j] = EA[i, j];

            //for (int i = 0; i < n; ++i)
            //    output[i, n ] = SRGC[i];
            //for (int i = 0; i < n; ++i)
            //    output[i, n + 1] = AOC[i];

            //output.CopyLabelsFrom(C);
            //MatrixWriter.WriteMatrixToMatrixFile(output, outputFile);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EAC[i, j] > 0 || AOC[i] <= 0)
                        EA[i, j] = EAC[i, j];
                    else if (PAN[i, j] > 0 && DC[i, j] == DM[i])
                        EA[i, j] = DC[i, j];
                    else
                        EA[i, j] = 0;



            Vector SEA = new Vector(n);

            Matrix previousEA = new Matrix(n);

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }



                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (DC[i, j] == DM[i]) DC[i, j] = 0;

                for (int i = 0; i < n; ++i)
                    DM[i] = Algorithms.MaxValue<double>(DC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        //if (AOC[i] != 0 && EEC[i, j] == EEM[i] && EA[i, j] == 0)
                        //    EA[i, j] = EEC[i, j];

                        if (AOC[i] == 0 || EA[i, j] != 0) ;
                        else if (PAN[i, j] > 0 && DC[i, j] == DM[i])
                            EA[i, j] = DC[i, j];
                        else EA[i, j] = 0;
                    }



            } while ((!previousEA.IsSameAs(EA)) && !DC.IsAllZero && !AOC.IsZeroVector);

            //EA.CopyLabelsFrom(C);
            //MatrixWriter.WriteMatrixToMatrixFile(EA, outputFile);
            if (!AOC.IsZeroVector)
            {

                Matrix JCC = JC * C;

                Vector JCM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    JCM[i] = Algorithms.MaxValue<double>(JCC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] <= 0 || EA[i, j] > 0) ;
                        else if (JCC[i, j] == JCM[i] && PAN[i, j] > 0)
                            EA[i, j] = JCC[i, j];

                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (JCC[i, j] == JCM[i]) JCC[i, j] = 0;

                    for (int i = 0; i < n; ++i)
                        JCM[i] = Algorithms.MaxValue<double>(JCC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] == 0 || EA[i, j] != 0) ;
                            else if (PAN[i, j] > 0 && JCC[i, j] == JCM[i])
                                EA[i, j] = JCC[i, j];
                            else EA[i, j] = 0;


                } while ((!previousEA.IsSameAs(EA)) && !JCC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero


            for (int i = 0; i < n; ++i)
                EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EEC[i, j] == EEM[i] && PAN[i, j] == 1 && AOC[i] != 0)
                        EA[i, j] = EEC[i, j];

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (EEC[i, j] == EEM[i]) EEC[i, j] = 0;

                for (int i = 0; i < n; ++i)
                    EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        //if (AOC[i] != 0 && EEC[i, j] == EEM[i] && EA[i, j] == 0)
                        //    EA[i, j] = EEC[i, j];

                        if (AOC[i] == 0 || EA[i, j] != 0) ;
                        else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
                            EA[i, j] = EEC[i, j];
                        else EA[i, j] = 0;
                    }




            } while ((!previousEA.IsSameAs(EA)) && !EEC.IsAllZero && !AOC.IsZeroVector);





            if (!AOC.IsZeroVector)
            {

                //for (int i = 0; i < n; ++i)
                //    for (int j = 0; j < n; ++j)
                //        if (PAC[i, j] > 0 && EEC[i, j] != 0) PAC[i, j] = 0;

                //PAC.CopyLabelsFrom(C);
                //MatrixWriter.WriteMatrixToMatrixFile(PAC, outputFile);
                Vector PAM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                            EA[i, j] = PAC[i, j];

                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (PAC[i, j] == PAM[i]) PAC[i, j] = 0;

                    for (int i = 0; i < n; ++i)
                        PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                                EA[i, j] = PAC[i, j];

                } while ((!previousEA.IsSameAs(EA)) && !PAC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero

            Matrix EAT = EA.GetTranspose();

            Matrix EAF = new Matrix(n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    EAF[i, j] = EA[i, j] * EAT[i, j];

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (EAF[i, j] != 0) BEA[i, j] = 1;
                    else BEA[i, j] = 0;
                }


            return BEA;

        }


        private static Matrix SimulateSimplifiedRealistStageTwo(Matrix C, Matrix R, Matrix M, Matrix BEA, string outputFile, string srgOutputFile, int networkID, bool outputDyadic, bool srgDyadic, int maxIter, double br)
        {
            int stage = 2;
            Matrix Temp = new Matrix(C.Rows);
            BEA.CloneTo(Temp);
            //Matrix OriginalR = new Matrix(C.Rows);
            //R.CloneTo(OriginalR);
            Matrix BEA2 = UpdateSimplifiedRealistStageTwo(C, R, M, BEA, outputFile, br);
            BEA2.CopyLabelsFrom(C);
            BEA2.NetworkId = int.Parse(networkID + "02");
            R.NetworkId = BEA2.NetworkId;

            while (!BEA2.IsSameAs(Temp) && (stage <= maxIter))
            {
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(BEA2, outputFile, false);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(BEA2, outputFile, false);
                if (!srgDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(R, srgOutputFile, false);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(R, srgOutputFile, false);
                //Matrix OriginR = new Matrix(C.Rows);
                //R.CloneTo(OriginR);
                BEA.CloneTo(Temp);
                BEA2.CloneTo(BEA);
                BEA2 = UpdateSimplifiedRealistStageTwo(C, R, M, BEA, outputFile, br);
                stage++;
                BEA2.NetworkId = int.Parse(networkID + "" + (stage >= 10 ? stage + "" : "0" + stage + ""));
                R.NetworkId = BEA2.NetworkId;
                BEA2.CopyLabelsFrom(C);
            }
            return BEA2;
        }

        private static Matrix SimulateSimplifiedLiberalStageTwo(Matrix C, Matrix R, Matrix M, Matrix D, Matrix JCC, Matrix BEA, string outputFile, string srgOutputFile, int networkID, bool outputDyadic, bool srgDyadic, int maxIter, double br)
        { 
            int stage = 2;
            Matrix Temp = new Matrix(C.Rows);
            BEA.CloneTo(Temp);
            //Matrix OriginalR = new Matrix(C.Rows);
            //R.CloneTo(OriginalR);
            Matrix BEA2 = UpdateSimplifiedLiberalStageTwo(C, R, M, D, JCC, BEA, outputFile, br);
            BEA2.CopyLabelsFrom(C);
            BEA2.NetworkId = int.Parse(networkID + "02");
            R.NetworkId = BEA2.NetworkId;
            
            while (!BEA2.IsSameAs(Temp) && (stage <= maxIter))
            {
                
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(BEA2, outputFile, false);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(BEA2, outputFile, false);

                if (!srgDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(R, srgOutputFile, false);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(R, srgOutputFile, false);

                //Matrix OriginR = new Matrix(C.Rows);
                //R.CloneTo(OriginR);
                BEA.CloneTo(Temp);
                BEA2.CloneTo(BEA);
                BEA2 = UpdateSimplifiedLiberalStageTwo(C, R, M, D, JCC, BEA, outputFile, br);
                stage++;
                BEA2.NetworkId = int.Parse(networkID + "" + (stage >= 10 ? stage + "" : "0" + stage + ""));
                R.NetworkId = BEA2.NetworkId;
                BEA2.CopyLabelsFrom(C);

            }
            return BEA2;
        }

        /// <summary>
        /// Simulates stage one of the SIMPLIFIED liberal network formation simulation.
        /// </summary>
        /// <param name="C">Capability matrix</param>
        /// <param name="R">Policy relevance matrix (SRG)</param>
        /// <param name="M">MID matrix</param>
        /// <param name="D">deocracy matrix</param>
        /// <param name="JC">JC matrix</param> 
        /// <returns>Expected alliance matrix</returns>
        /// expectedAlliance = SimulateSimplifiedLiberalStageOne(capabilities, srg, MID, D, JCC, outputFile, br);
        private static Matrix SimulateSimplifiedLiberalStageOne(Matrix C, Matrix R, Matrix M, Matrix D, Matrix JC, string outputFile, double br)
        {
            int n = C.Rows;
            Matrix SRC = R * C;
            Vector SRGC = new Vector(n);
            for (int i = 0; i < n; ++i)
                SRGC[i] = SRC.GetRowSum(i) * br;
            Vector AOC = new Vector(n);
            for (int i = 0; i < n; ++i)
            {
                if (SRGC[i] <= C[i, i]) AOC[i] = 0;
                else AOC[i] = 1 - C[i, i] / SRGC[i];
            }

            Matrix F = Matrix.Ones(n, n);
            F.ZeroDiagonal();
            Matrix PAN = F - R;
            Matrix PAC = PAN * C;

            Matrix EE = M * M;
            EE.ZeroDiagonal();
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EE[i, j] != 0) EE[i, j] = 1;

            Matrix DC = D * C;

            Vector DM = new Vector(n);
            for (int i = 0; i < n; ++i)
                DM[i] = Algorithms.MaxValue<double>(DC.GetRowEnumerator(i));

            Matrix EA = new Matrix(n, n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (DC[i, j] == DM[i] && PAN[i, j] == 1 && AOC[i] != 0)
                        EA[i, j] = DC[i, j];
                    else
                        EA[i, j] = 0;



            Vector SEA = new Vector(n);

            Matrix previousEA = new Matrix(n);

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (DC[i, j] == DM[i]) DC[i, j] = 0;

                for (int i = 0; i < n; ++i)
                    DM[i] = Algorithms.MaxValue<double>(DC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        //if (AOC[i] != 0 && EEC[i, j] == EEM[i] && EA[i, j] == 0)
                        //    EA[i, j] = EEC[i, j];

                        if (AOC[i] == 0 || EA[i, j] != 0) ;
                        else if (PAN[i, j] > 0 && DC[i, j] == DM[i])
                            EA[i, j] = DC[i, j];
                        else EA[i, j] = 0;
                    }




            } while ((!previousEA.IsSameAs(EA)) && !DC.IsAllZero && !AOC.IsZeroVector);

            if (!AOC.IsZeroVector)
            {

                Matrix JCC = JC * C;

                Vector JCM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    JCM[i] = Algorithms.MaxValue<double>(JCC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] <= 0 || EA[i, j] > 0) ;
                        else if (JCC[i, j] == JCM[i] && PAN[i, j] > 0)
                            EA[i, j] = JCC[i, j];

                EA.CopyLabelsFrom(C);
                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (JCC[i, j] == JCM[i]) JCC[i, j] = 0;

                    for (int i = 0; i < n; ++i)
                        JCM[i] = Algorithms.MaxValue<double>(JCC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] != 0 && JCC[i, j] == JCM[i] && EA[i, j] == 0)
                                EA[i, j] = JCC[i, j];

                    EA.CopyLabelsFrom(C);
                } while ((!previousEA.IsSameAs(EA)) && !JCC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero

            Matrix EEC = EE * C;
            Vector EEM = new Vector(n);

            for (int i = 0; i < n; ++i)
                EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    if (EEC[i, j] == EEM[i] && PAN[i, j] == 1 && AOC[i] != 0)
                        EA[i, j] = EEC[i, j];

            do
            {

                EA.CloneTo(previousEA);

                for (int i = 0; i < n; ++i)
                    SEA[i] = EA.GetRowSum(i) + C[i, i];

                for (int i = 0; i < n; ++i)
                {
                    if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                    else AOC[i] = 1 - SEA[i] / SRGC[i];
                }

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (EEC[i, j] == EEM[i]) EEC[i, j] = 0;

                for (int i = 0; i < n; ++i)
                    EEM[i] = Algorithms.MaxValue<double>(EEC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        //if (AOC[i] != 0 && EEC[i, j] == EEM[i] && EA[i, j] == 0)
                        //    EA[i, j] = EEC[i, j];

                        if (AOC[i] == 0 || EA[i, j] != 0);
                        else if (PAN[i, j] > 0 && EEC[i, j] == EEM[i])
                            EA[i, j] = EEC[i, j];
                        else EA[i, j] = 0;
                    }




            } while ((!previousEA.IsSameAs(EA)) && !EEC.IsAllZero && !AOC.IsZeroVector);

            if (!AOC.IsZeroVector)
            {

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (PAC[i, j] > 0 && EEC[i, j] != 0) PAC[i, j] = 0;

                Vector PAM = new Vector(n);

                for (int i = 0; i < n; ++i)
                    PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                        if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                            EA[i, j] = PAC[i, j];

                EA.CopyLabelsFrom(C);
                do
                {

                    EA.CloneTo(previousEA);

                    for (int i = 0; i < n; ++i)
                        SEA[i] = EA.GetRowSum(i) + C[i, i];

                    for (int i = 0; i < n; ++i)
                    {
                        if (SRGC[i] <= SEA[i]) AOC[i] = 0;
                        else AOC[i] = 1 - SEA[i] / SRGC[i];
                    }

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (PAC[i, j] == PAM[i]) PAC[i, j] = 0;

                    for (int i = 0; i < n; ++i)
                        PAM[i] = Algorithms.MaxValue<double>(PAC.GetRowEnumerator(i));

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < n; ++j)
                            if (AOC[i] != 0 && PAC[i, j] == PAM[i] && EA[i, j] == 0)
                                EA[i, j] = PAC[i, j];

                    EA.CopyLabelsFrom(C);
                } while ((!previousEA.IsSameAs(EA)) && !PAC.IsAllZero && !AOC.IsZeroVector);


            }//if AOC is not all zero


            Matrix EAT = EA.GetTranspose();

            Matrix EAF = new Matrix(n);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    EAF[i, j] = EA[i, j] * EAT[i, j];

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    if (EAF[i, j] != 0) BEA[i, j] = 1;
                    else BEA[i, j] = 0;
                }


            return BEA;
        }


        /// <summary>
        /// Simulates stage one of the realist network formation simulation.
        /// </summary>
        /// <param name="G">Contiguity matrix</param>
        /// <param name="C">Capability matrix</param>
        /// <param name="P">Major powers vector</param>
        /// <param name="R">Policy relevance matrix (SRG)</param>
        /// <returns>Expected alliance matrix</returns>
        private static Matrix SimulateRealistStageOne(SymmetricBinaryMatrix G, Matrix C, Matrix R, Vector P, double br)
        {
            int n = C.Rows;

            if (R == null)
            {
                R = new Matrix(n);
                for (int i = 0; i < R.Rows; ++i)
                    for (int j = 0; j < R.Cols; ++j)
                        if (G.GetValue(i, j) || P[i] == 1 || P[j] == 1)
                            R[i, j] = 1;
                        else
                            R[i, j] = 0;
            }

            Matrix CR = R * C;

            Vector AO = new Vector(n);
            for (int i = 0; i < AO.Size; ++i)
            {
                if (Algorithms.MaxValue<double>(CR.GetRowEnumerator(i)) <= 0.0)
                    AO[i] = 0;
                else
                    AO[i] = br * (CR.GetRowSum(i) - C[i, i]);
            }

            Matrix F = Matrix.Ones(n, n);
            F.SetDiagonalFromVector(Vector.Zero(n));

            Matrix PAN = (F - R) * C;

            Matrix EA = Matrix.Zero(n, n);

            UpdateEAMatrix(AO, PAN, EA);

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    BEA[i, j] = EA[i, j] < double.Epsilon ? 0 : 1;
            Matrix BEAT = BEA.GetTranspose();

            return (BEA + BEAT) / 2;
        }

        /// <summary>
        /// Simulates stage two of the realist network formation simulation.
        /// </summary>
        /// <param name="M">Conflict matrix</param>
        /// <param name="EA">EA matrix from stage one</param>
        /// <param name="C">Capability matrix for stage two</param>
        /// <returns>EA matrix for stage two</returns>
        private static Matrix SimulateRealistStageTwo(Matrix M, Matrix EA1, Matrix C, double br)
        {
            int n = M.Rows;
            Matrix AM = M * EA1;
            Matrix R = new Matrix(n);
            for (int i = 0; i < R.Rows; ++i)
                for (int j = 0; j < R.Cols; ++j)
                    if (i != j && (M[i, j] > 0 || AM[i, j] > 0))
                        R[i, j] = 1;
                    else
                        R[i, j] = 0;

            Matrix EE = M * M;
            Matrix BEE = new Matrix(n);
            for (int i = 0; i < R.Rows; ++i)
                for (int j = 0; j < R.Cols; ++j)
                    if (i != j && EE[i, j] > 0)
                        BEE[i, j] = 1;
                    else
                        BEE[i, j] = 0;

            Matrix CR = R * C;
            Vector AO = new Vector(n);
            for (int i = 0; i < AO.Size; ++i)
            {
                double sum = br * (CR.GetRowSum(i) - C[i, i]);
                if (sum <= C[i, i])
                    AO[i] = 0;
                else
                    AO[i] = sum;
            }

            Matrix F = Matrix.Ones(n, n);
            F.SetDiagonalFromVector(Vector.Zero(n));

            Matrix PAN = (F - R) * C;
            Matrix PAC = BEE * C;

            Matrix EA = Matrix.Zero(n, n);

            UpdateEAMatrix(AO, PAC, EA);
            UpdateEAMatrix(AO, PAN, EA);

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    BEA[i, j] = EA[i, j] < double.Epsilon ? 0 : 1;
            Matrix BEAT = BEA.GetTranspose();

            return (BEA + BEAT) / 2;
        }

        private static Matrix SimulateLiberalStageOne(SymmetricBinaryMatrix G, Matrix C, Matrix R, Vector P, Matrix D, Matrix JC, double br)
        {
            Matrix DC = D * C;
            Matrix JCC = JC * C;

            int n = C.Rows;

            if (R == null)
            {
                R = new Matrix(n);
                for (int i = 0; i < R.Rows; ++i)
                    for (int j = 0; j < R.Cols; ++j)
                        if (G.GetValue(i, j) || P[i] == 1 || P[j] == 1)
                            R[i, j] = 1;
                        else
                            R[i, j] = 0;
            }

            Matrix CR = R * C;

            Vector AO = new Vector(n);
            for (int i = 0; i < AO.Size; ++i)
            {
                if (Algorithms.MaxValue<double>(CR.GetRowEnumerator(i)) <= 0.0)
                    AO[i] = 0;
                else
                    AO[i] = br * (CR.GetRowSum(i) - C[i, i] );
            }

            Matrix F = Matrix.Ones(n, n);
            F.SetDiagonalFromVector(Vector.Zero(n));

            Matrix PAN = (F - R) * C;

            Matrix EA = Matrix.Zero(n, n);

            UpdateEAMatrix(AO, DC, EA);
            UpdateEAMatrix(AO, PAN, EA);
            UpdateEAMatrix(AO, JCC, EA);

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    BEA[i, j] = EA[i, j] < double.Epsilon ? 0 : 1;
            Matrix BEAT = BEA.GetTranspose();

            return (BEA + BEAT) / 2;
        }

        private static Matrix SimulateLiberalStageTwo(Matrix M, Matrix EA1, Matrix C, Matrix D, Matrix JC, double br)
        {
            Matrix JCC = JC * C;
            Matrix DC = D * C;

            int n = M.Rows;
            Matrix AM = M * EA1;
            Matrix R = new Matrix(n);
            for (int i = 0; i < R.Rows; ++i)
                for (int j = 0; j < R.Cols; ++j)
                    if (i != j && (M[i, j] > 0 || AM[i, j] > 0))
                        R[i, j] = 1;
                    else
                        R[i, j] = 0;

            Matrix EE = M * M;
            Matrix BEE = new Matrix(n);
            for (int i = 0; i < R.Rows; ++i)
                for (int j = 0; j < R.Cols; ++j)
                    if (i != j && EE[i, j] > 0)
                        BEE[i, j] = 1;
                    else
                        BEE[i, j] = 0;

            Matrix CR = R * C;
            Vector AO = new Vector(n);
            for (int i = 0; i < AO.Size; ++i)
            {
                double sum = br * (CR.GetRowSum(i) - C[i, i]);
                if (sum <= C[i, i])
                    AO[i] = 0;
                else
                    AO[i] = sum;
            }

            Matrix F = Matrix.Ones(n, n);
            F.SetDiagonalFromVector(Vector.Zero(n));

            Matrix PAN = (F - R) * C;
            Matrix PAC = BEE * C;

            Matrix EA = Matrix.Zero(n, n);

            UpdateEAMatrix(AO, DC, EA);
            UpdateEAMatrix(AO, PAC, EA);
            UpdateEAMatrix(AO, PAN, EA);
            UpdateEAMatrix(AO, JCC, EA);

            Matrix BEA = new Matrix(n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    BEA[i, j] = EA[i, j] < double.Epsilon ? 0 : 1;
            Matrix BEAT = BEA.GetTranspose();

            AO.Clear();
            PAC.Clear();
            EA.Clear();
            DC.Clear();

            return (BEA + BEAT) / 2;
        }


        // for steps 13-24 in stage one
        private static void NAPTStageOneHelper1(Matrix CAP, Matrix SRG, Matrix SRGC, Matrix PAN, Matrix UA11, Matrix CUA11, Vector AOC,
            ref Matrix DP1, ref Matrix FA1, int stage, Matrix AC)
        {
            // 13. Calculate the expected alliance matrix A11
            // Sum across rows of A11?
            Matrix A11 = new Matrix(CUA11.Rows, CUA11.Cols);
            if (stage == 1)
            {
                for (int i = 0; i < CUA11.Rows; i++)
                {
                    for (int j = 0; j < CUA11.Cols; j++)
                    {
                        if (i == j)
                            A11[i, j] = CAP[i, i];
                        else if (CUA11[i, j] == CUA11.GetMaxInRow(i))
                            A11[i, j] = CAP[j, j];
                        else
                            A11[i, j] = 0;
                    }
                }
            }
            else // stage 2 is different
            {
                for (int i = 0; i < CUA11.Rows; i++)
                {
                    for (int j = 0; j < CUA11.Cols; j++)
                    {
                        if (AC[i, j] > 0)
                            A11[i, j] = AC[i, j];
                        else if (CUA11[i, j] > 0)
                            A11[i, j] = CUA11[i, j];
                        else
                            A11[i, j] = 0;
                    }
                }
            }

            /*
            // 14. Update the AOC vector to AOC11
            Vector AOC11 = new Vector(AOC.Size);
            for (int i = 0; i < A11.Rows; i++)
            {
                if (A11.GetRowSum(i) >= AOC[i])
                    AOC11[i] = 0;
                else
                    AOC11[i] = AOC[i] - A11.GetRowSum(i);   
            }
            */

            // 14. Update the AOC vector to AOC11
            //      (modified to compare to SRG CAP instead of AOC) -------- this one is correct ------
            Vector AOC11 = new Vector(AOC.Size);
            for (int i = 0; i < A11.Rows; i++)
            {
                if (A11.GetRowSum(i) >= SRGC.GetRowSum(i))
                    AOC11[i] = 0;
                else
                    AOC11[i] = SRGC.GetRowSum(i) - A11.GetRowSum(i);
            }



            // 17.  Subtract Sum(AOC12) - Sum(AOC11). If result is equal to zero, stop run and go back to step #x
            //      If the result is less than zero, repeat steps 11-16 by calculating UA13, CUA13, A13, AOC13
            //
            //      This method incorporates steps 15-16
            Matrix new_A = UpdateAOC(UA11, CUA11, AOC11, CAP, SRGC, A11, PAN, stage);

            // 18.  Generate an aliiance offer matrix AO1 which is a binarized version of A1x (where x is the number
            //      of the last A matrix

            //new_A.Binarize();

            // Recently updated as of 3:59AM 7/17/2012 (subject to change)

            Matrix AO1 = new Matrix(new_A.Rows, new_A.Cols);
            for (int i = 0; i < AO1.Rows; i++)
            {
                for (int j = 0; j < AO1.Cols; j++)
                {
                    if (i == j)
                        AO1[i, j] = 0;
                    else if (new_A[i, j] > 0)
                        AO1[i, j] = 1;
                    else
                        AO1[i, j] = 0;
                }
            }

            // 19. Transpose AO1
            Matrix AO1_T = AO1.GetTranspose();

            // 20. Generate a Defence Pacts matrix DP1 = AO1 * AO1'
            DP1 = new Matrix(AO1.Rows, AO1_T.Cols);
            for (int i = 0; i < AO1.Rows; i++)
                for (int j = 0; j < AO1_T.Cols; j++)
                    DP1[i, j] = AO1[i, j] * AO1_T[i, j];

            // 21. Generate an Allies of Enemies AE1 matrix AE1 = SRG x DP1
            Matrix AE1 = SRG * DP1;

            // 22. Generate a Prevention matrix PV1
            /*
            Matrix PV1 = new Matrix(AE1.Rows, AE1.Cols);
            for (int i = 0; i < AE1.Rows; i++)
            {
                for (int j = 0; j < AE1.Cols; j++)
                {
                    if (DP1[i, j] == 0 && AE1[i, j] == 0)
                        PV1[i, j] = 0;
                    else if (DP1[i, j] == 1 && AE1[i, j] == 0)
                        PV1[i, j] = 1;
                    else if (DP1[i, j] == 0 && AE1[i, j] > 0)
                        PV1[i, j] = 2;
                }
            }
            */

            // New implementation which uses excel formula instead of pdf
            Matrix PV1 = new Matrix(AE1.Rows, AE1.Cols);
            for (int i = 0; i < AE1.Rows; i++)
            {
                for (int j = 0; j < AE1.Cols; j++)
                {
                    if (i == j)
                        PV1[i, j] = 0;
                    else if (stage == 1)
                    {
                        if (AE1[i, j] == 1)
                            PV1[i, j] = 1;
                        else if (AE1[i, j] > 0 && SRG[i, j] == 0)
                            PV1[i, j] = 2;
                        else
                            PV1[i, j] = 0;
                    }
                    else
                    {
                        if (DP1[i, j] == 1)
                            PV1[i, j] = 1;
                        else if (AE1[i, j] > 0 && SRG[i, j] == 0)
                            PV1[i, j] = 2;
                        else
                            PV1[i, j] = 0;
                    }
                }
            }

            // 23. Transpose PV1
            Matrix PV1_T = PV1.GetTranspose();

            // 24. Generate a final alliance matrix at T1 (FA1)
            FA1 = new Matrix(PV1.Rows, PV1.Cols);
            for (int i = 0; i < PV1.Rows; i++)
            {
                for (int j = 0; j < PV1.Cols; j++)
                {
                    // PDF implementation
                    /*
                    if (PV1[i, j] == 0 && PV1_T[i, j] == 0)
                        FA1[i, j] = 0;
                    else
                        FA1[i, j] = (PV1[i, j] > PV1_T[i, j]) ? PV1[i, j] : PV1_T[i, j]; // max value of the two
                    */

                    // Excel Implementation
                    if (PV1[i, j] > 0 && PV1_T[i, j] > 0)
                        FA1[i, j] = (PV1[i, j] > PV1_T[i, j]) ? PV1[i, j] : PV1_T[i, j]; // max value of the two
                    else
                        FA1[i, j] = 0;

                }
            }
        }

        // For steps 25-33 in stage one
        private static Matrix NAPTStageOneHelper2(Matrix CAP, Matrix SRG, Matrix FA1, Matrix UA11, Matrix REL, Vector AOC, int stage, int networkid)
        {
            // 25. Generate a Non-Aggression matrix NAG
            Matrix NAG = new Matrix(FA1.Rows, FA1.Cols);
            for (int i = 0; i < FA1.Rows; i++)
            {
                for (int j = 0; j < FA1.Cols; j++)
                {
                    if (FA1[i, j] == 2)
                        NAG[i, j] = 1;
                    else
                        NAG[i, j] = 0;
                }
            }

            Vector NAG_vec = new Vector(NAG.Rows);
            for (int i = 0; i < NAG_vec.Size; i++)
            {
                NAG_vec[i] = NAG.GetRowSum(i);
            }

            // 26.  Generate an Allies Attractiveness matrix AAT = DP1 * UA11
            //      Calculate the sum row entry
            Matrix AAT = new Matrix(UA11.Rows, UA11.Cols);
            /*
            for (int i = 0; i < DP1.Rows; i++)
                for (int j = 0; j < UA11.Cols; j++)
                    AAT[i, j] = DP1[i, j] * UA11[i, j];
            */
            //FA1.Binarize();
            for (int i = 0; i < FA1.Rows; i++)
                for (int j = 0; j < FA1.Cols; j++)
                    if (FA1[i, j] != 1)
                        FA1[i, j] = 0;


            // Generate the vector DEF for Output Matrix
            Vector DEF = new Vector(FA1.Rows);
            for (int i = 0; i < DEF.Size; i++)
            {
                DEF[i] = FA1.GetRowSum(i);
            }

            // follows the excel instructions
            for (int i = 0; i < FA1.Rows; i++)
            {
                for (int j = 0; j < UA11.Cols; j++)
                {
                    //AAT[i, j] = (FA1[i, j] == 1 ? 1 : 0) * UA11[i, j];
                    AAT[i, j] = FA1[i, j] * UA11[i, j];
                }
            }

            // 27. Generate the attractiveness coefficient atc for each row
            /*
            double[] atc = new double[AAT.Rows];
            for (int i = 0; i < AAT.Rows; i++)
                atc[i] = AAT.GetRowSum(i) * DP1.GetRowSum(i);
            */
            // follow excel instructions
            //double[] ATC = new double[AAT.Rows];
            Vector ATC = new Vector(AAT.Rows);
            for (int i = 0; i < AAT.Rows; i++)
            {
                if (FA1.GetRowSum(i) > 0)
                    ATC[i] = AAT.GetRowSum(i) / FA1.GetRowSum(i);
                else
                    ATC[i] = 0;
            }

            // 28. Generate an Allies Reliability matrix ARE = BFA1 * REL (where BAF1 is the binarized FA1 matrix)
            //FA1.Binarize();
            Matrix ARE = new Matrix(FA1.Rows, REL.Cols);
            if (stage == 1)
            {
                for (int i = 0; i < FA1.Rows; i++)
                    for (int j = 0; j < REL.Cols; j++)
                        ARE[i, j] = FA1[i, j] * REL[i, i]; // may need to be changed to [j, j]
            }
            else
            {
                for (int i = 0; i < FA1.Rows; i++)
                    for (int j = 0; j < REL.Cols; j++)
                        ARE[i, j] = NAG[i, j] * REL[j, j];
            }

            Vector REC = new Vector(ARE.Rows);
            if (stage == 1)
            {
                for (int i = 0; i < REC.Size; i++)
                {
                    if ((DEF[i] + NAG_vec[i]) > 0)
                        REC[i] = ARE.GetRowSum(i) / (DEF[i] + NAG_vec[i]);
                    else
                        REC[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < REC.Size; i++)
                {
                    REC[i] = ARE.GetRowAverage(i);
                }
            }
            // 29. Generate an alliance capabilities matrix defined as AC1 = DP1 * CAP
            /*
            Matrix AC1 = new Matrix(DP1.Rows, CAP.Cols);
            for (int i = 0; i < DP1.Rows; i++)
                for (int j = 0; j < CAP.Cols; j++)
                    AC1[i, j] = DP1[i, j] * CAP[i, i];
            */
            // follow excel instructions
            Matrix AC1 = new Matrix(FA1.Rows, CAP.Cols);
            for (int i = 0; i < FA1.Rows; i++)
            {
                for (int j = 0; j < CAP.Cols; j++)
                {
                    if (i == j)
                        AC1[i, j] = CAP[j, j];
                    else
                        AC1[i, j] = FA1[i, j] * CAP[j, j];
                }
            }

            // 30. Generate a cumulative alliance capability vector CAL
            Vector CAL = new Vector(AC1.Rows);
            for (int i = 0; i < AC1.Rows; i++)
                CAL[i] = AC1.GetRowSum(i);

            // 31.  Generate a finals SRG capabilities matrix SRGC1
            //      Generate a SRG capability vector SRGC
            Matrix SRGC1 = new Matrix(FA1.Rows, FA1.Cols);
            for (int i = 0; i < FA1.Rows; i++)
            {
                for (int j = 0; j < FA1.Cols; j++)
                {
                    if (SRG[i, j] == 1 && FA1[i, j] == 0)
                        SRGC1[i, j] = CAP[j, j];
                    else
                        SRGC1[i, j] = 0;
                }
            }

            Vector SRGC_vec = new Vector(SRGC1.Rows);
            for (int i = 0; i < SRGC1.Rows; i++)
                SRGC_vec[i] = SRGC1.GetRowSum(i);

            // 32. Generate a revised AOC1 vector
            Vector AOC1 = new Vector(SRGC_vec.Size);
            for (int i = 0; i < SRGC_vec.Size; i++)
            {
                if (CAL[i] >= SRGC_vec[i])
                    AOC1[i] = 0;
                else
                    AOC1[i] = SRGC_vec[i] - CAL[i];
            }

            // 33. Save output
            Matrix Output = new Matrix(FA1.Rows, 9);
            string[] colNames = {"Node", "DEF", "NAG", "CAP", "CAL", "AOC0", "AOC1", "ATC", "REC"};
            //Output.NetworkId = 101 + stage;
            Output.NetworkId = int.Parse(networkid + "0" + stage);
            for (int i = 0; i < colNames.Length; i++)
                Output.ColLabels[i] = colNames[i];
            for (int i = 0; i < Output.Rows; i++)
            {
                //Output.RowLabels[i] = (101 + stage).ToString();
                Output.RowLabels[i] = (int.Parse(networkid + "0" + stage)).ToString();
                Output[i, 0] = i + 1;
            }
            Output.SetColVector(1, DEF);
            Output.SetColVector(2, NAG_vec);
            for (int i = 0; i < CAP.Rows; i++)
                Output[i, 3] = CAP[i, i];
            Output.SetColVector(4, CAL);
            Output.SetColVector(5, AOC);
            Output.SetColVector(6, AOC1);
            Output.SetColVector(7, ATC);
            Output.SetColVector(8, REC);
            
            return Output;
        }


        private static Matrix SimulateNAPTStageOne(Matrix CAP, Matrix SRG, Matrix CONT, Matrix MID, Matrix DEMO, Matrix JC, Matrix CS, Matrix REL, ref Matrix SRGOutput, ref Matrix EAOutput, string EAOutputFile, string outputfile, bool overwrite, bool EADyadic, bool outputDyadic, int networkid)
        {
            int stage = 1;
            //int networkid = SRG.NetworkId;
            // 1. Transpose vectors Cap, Demo, and Rel
            // No need to since the vectors are just the diagonals of the matrices
            
            // 2. Generate a SRG Capability matrix SRG CAP.  Sum the rows of this matrix
            Matrix SRGC = new Matrix(SRG);
            double[] srgSum = new double[SRGC.Rows];
            for (int i = 0; i < SRGC.Rows; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < SRGC.Cols; j++)
                {
                    SRGC[i, j] *= CAP[j, j];
                    sum += SRGC[i, j];
                }
                srgSum[i] = sum;
            }

            // 3. Generate an Alliance Opportunity Cost vector
            Vector AOC = new Vector(SRGC.Rows);
            for (int i = 0; i < AOC.Size; i++)
            {
                if (CAP[i, i] >= srgSum[i])
                    AOC[i] = 0;
                else
                    AOC[i] = srgSum[i] - CAP[i, i];
            }

            // 4. Generate an Enemy of Enemy matrix (EE) and binarize it
            Matrix EE = new Matrix(MID * MID);

            // 5. Generate a potential alliance network matrix PAN
            Matrix PAN = new Matrix(SRG.Rows, SRG.Cols);
            for (int i = 0; i < SRG.Rows; i++)
                for (int j = 0; j < SRG.Cols; j++)
                    PAN[i, j] = 1 - SRG[i, j];

            // 6. Generate a joint democracy matrix (DD)
            Matrix DD = new Matrix(DEMO.Rows, DEMO.Cols);
            for (int i = 0; i < DEMO.Rows; i++)
                for (int j = 0; j < DEMO.Cols; j++)
                    DD[i, j] = DEMO[i, i] * DEMO[j, j];

            // 7. Generate a contiguity to SRG matrix (SRGCT)
            Matrix SRGCT = SRG * CONT;

            // 8. Binarize the EE matrix
            // 9. Binarize the DD matrix
            // 10. Binarize the SRGCT matrix
            EE.Binarize();
            DD.Binarize();
            SRGCT.Binarize();

            // 11. Generate a Utility for Potential Ally matrix UA11
            Matrix UA11 = new Matrix(PAN.Rows, PAN.Cols);
            double[] maxRowUA11 = new double[UA11.Rows];
            for (int i = 0; i < PAN.Rows; i++)
            {
                double cur_max = double.MinValue; // find max value in row
                for (int j = 0; j < PAN.Cols; j++)
                {
                    if (i == j || PAN[i, j] == 0)
                        UA11[i, j] = 0;
                    else if (DEMO[i, i] == 1 && PAN[i, j] == 1)
                        UA11[i, j] = (0.2 * EE[i, j]) + (0.4 * DD[i, j]) + (0.1 * JC[i, j]) + (0.1 * CS[i, j]) + (0.2 * SRGCT[i, j]);
                    else if (DEMO[i, i] == 0 && PAN[i, j] == 1)
                        UA11[i, j] = (0.5 * EE[i, j]) + (0.1 * DD[i, j]) + (0.1 * JC[i, j]) + (0.1 * CS[i, j]) + (0.2 * SRGCT[i, j]);

                    if (UA11[i, j] > cur_max)
                        cur_max = UA11[i, j];
                }
                maxRowUA11[i] = cur_max;
            }

            // 12. Generate a matrix CUA11
            Matrix CUA11 = new Matrix(UA11.Rows, UA11.Cols);
            double[] maxRowCUA11 = new double[CUA11.Rows];
            for (int i = 0; i < UA11.Rows; i++)
            {
                double cur_max = double.MinValue; // find max value in row
                for (int j = 0; j < UA11.Cols; j++)
                {
                    if (UA11[i, j] != maxRowUA11[i])
                        CUA11[i, j] = 0;
                    else if (UA11[i, j] == maxRowUA11[i] && PAN[i, j] > 0 && i != j) // needs PAN and i!=j conditions
                        CUA11[i, j] = CAP[j, j];

                    if (CUA11[i, j] > cur_max)
                        cur_max = CUA11[i, j];
                }
                maxRowCUA11[i] = cur_max;
            }
            
            Matrix DP1 = new Matrix(UA11.Rows, UA11.Cols);
            Matrix FA1 = new Matrix(UA11.Rows, UA11.Cols);
            NAPTStageOneHelper1(CAP, SRG, SRGC, PAN, UA11, CUA11, AOC, ref DP1, ref FA1, stage, null);
            
            // write FA1 to matrix file
            FA1.NetworkId = int.Parse(networkid + "0" + stage);
            //FA1.NetworkId = SRG.NetworkId + 1;
            //int.Parse(editedNetworkId + "01");
            for (int i = 0; i < SRG.Rows; i++)
            {
                FA1.RowLabels[i] = SRG.RowLabels[i];
                FA1.ColLabels[i] = SRG.ColLabels[i];
            }
            if (!EADyadic)
                MatrixWriter.WriteMatrixToMatrixFile(FA1, EAOutputFile, overwrite);
            else
                MatrixWriter.WriteMatrixToDyadicFile(FA1, EAOutputFile, overwrite);
            //overwrite = false;

            Matrix Temp = new Matrix(FA1);
            Matrix Output = NAPTStageOneHelper2(CAP, SRG, FA1, UA11, REL, AOC, stage, networkid);

            // write Output to matrix file
            if (!outputDyadic)
                MatrixWriter.WriteMatrixToMatrixFile(Output, outputfile, overwrite);
            else
                MatrixWriter.WriteMatrixToDyadicFile(Output, outputfile, overwrite);
            overwrite = false;

            FA1.Clear();
            Temp.CloneTo(FA1);
            
            return SimulateNAPTStageTwo(FA1, SRG, DP1, CAP, REL, UA11, SRG.Rows, Output, ref SRGOutput, ref EAOutput, stage + 1, EAOutputFile, outputfile, overwrite, EADyadic, outputDyadic, networkid);

        }

        private static Matrix SimulateNAPTStageTwo(Matrix FA1, Matrix SRG1, Matrix DP1, Matrix CAP, Matrix REL, Matrix UA11, int rowColSize, Matrix output, ref Matrix SRGOutput, ref Matrix EAOutput, int stage, string EAOutputFile, string outputfile, bool overwrite, bool EADyadic, bool outputDyadic, int networkid)
        {
            //int networkid = SRG1.NetworkId;
            // 2. Generate Allies of Enemies matrix AE2 = SRG1 x DP1
            Matrix AE2 = SRG1 * DP1;
            
            // 3. Generate SRG2
            Matrix SRG2 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < SRG2.Rows; i++)
            {
                for (int j = 0; j < SRG2.Cols; j++)
                {
                    if (FA1[i, j] > 0)
                        SRG2[i, j] = 0;
                    else if (SRG1[i, j] == 1 || AE2[i, j] > 0)
                        SRG2[i, j] = 1;
                    else
                        SRG2[i, j] = 0;
                }
            }
            

            // 4. Generate an Allies Capabilities Matrix AC20 = DP1 * C.  Sum over rows of AC20
            // Method from excel sheet
            Matrix AC20 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                for (int j = 0; j < rowColSize; j++)
                {
                    if (i == j || FA1[i, j] == 1)
                        AC20[i, j] = CAP[j, j];
                    else
                        AC20[i, j] = 0;
                }
            }

            // 5. Generate an SRG Capabilities Matrix SRGC20 = SRGC * C.  Sum across rows
            Matrix SRGC20 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                for (int j = 0; j < rowColSize; j++)
                {
                    SRGC20[i, j] = SRG2[i, j] * CAP[j, j];
                }
            }

            // 6. Generate an Alliance Opportunity Cost Vector AOC20
            Vector AOC20 = new Vector(rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                if (AC20.GetRowSum(i) >= SRGC20.GetRowSum(i))
                    AOC20[i] = 0;
                else
                    AOC20[i] = SRGC20.GetRowSum(i) - AC20.GetRowSum(i);
            }

            // 7. Generate a Potential Alliance Network T2 PAN2
            // method from excel sheet
            Matrix PAN2 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                for (int j = 0; j < rowColSize; j++)
                {
                    if (i == j || FA1[i, j] > 0)
                        PAN2[i, j] = 0;
                    else
                        PAN2[i, j] = 1 - SRG2[i, j];
                }
            }

            // 8. Generate a Utility for Potential Ally matrix UA21 = PAN2 * UA11.  Calulate the max row for UA21
            /*
            Matrix UA21 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                for (int j = 0; j < rowColSize; j++)
                {
                    if (i != j)
                        UA21[i, j] = PAN2[i, j] * UA11[i, j];
                    else
                        UA21[i, j] = 0;
                }
            }
            */
            Matrix UA21 = new Matrix(UA11);

            // 9. Calculate the Potential Ally Capabilities matrix CUA21
            // method from excel sheet
            Matrix CUA21 = new Matrix(rowColSize, rowColSize);
            for (int i = 0; i < rowColSize; i++)
            {
                for (int j = 0; j < rowColSize; j++)
                {
                    if (i != j && UA21[i, j] == UA21.GetMaxInRow(i))
                        CUA21[i, j] = CAP[j, j];
                    else
                        CUA21[i, j] = 0;
                }
            }

            Matrix DP2 = new Matrix(rowColSize, rowColSize);
            Matrix FA2 = new Matrix(rowColSize, rowColSize);
            NAPTStageOneHelper1(CAP, SRG2, SRGC20, PAN2, UA21, CUA21, AOC20, ref DP2, ref FA2, stage, AC20);

            if (stage >= 10)
                FA2.NetworkId = int.Parse(networkid + "" + stage);
            else
                FA2.NetworkId = int.Parse(networkid + "0" + stage);
            for (int i = 0; i < FA1.Rows; i++)
            {
                FA2.RowLabels[i] = FA1.RowLabels[i];
                FA2.ColLabels[i] = FA1.ColLabels[i];
            }
            // write FA2 to matrix file
            // Set network ID and row and col labels first
            if (!EADyadic)
                MatrixWriter.WriteMatrixToMatrixFile(FA2, EAOutputFile, overwrite);
            else
                MatrixWriter.WriteMatrixToDyadicFile(FA2, EAOutputFile, overwrite);
            //overwrite = false;

            Matrix M2 = FA2 - FA1;
            if (M2.IsAllZero)
            {
                // done?
                EAOutput = FA2;
                for (int i = 0; i < output.Rows; i++)
                {
                    output.RowLabels[i] = (int.Parse(networkid + "0" + stage)).ToString();
                }
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(output, outputfile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(output, outputfile, overwrite);
                return output; // for now
            }
            else
            {
                // need to unbinarize FA2 first
                Matrix Temp = new Matrix(FA2);
                // argument needs to be UA11 and not UA12 (according to excel)
                Matrix new_output = NAPTStageOneHelper2(CAP, SRG2, FA2, UA11, REL, AOC20, stage, networkid);

                // write new_output to matrix file
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(new_output, outputfile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(new_output, outputfile, overwrite);

                // Copy the data from SRG2 to SRGOutput
                SRGOutput.Clear();
                for (int i = 0; i < SRG2.Rows; i++)
                    for (int j = 0; j < SRG2.Cols; j++)
                        SRGOutput[i, j] = SRG2[i, j];
                SRGOutput.NetworkId = new_output.NetworkId;
                FA2.Clear();
                Temp.CloneTo(FA2);
                return SimulateNAPTStageTwo(FA2, SRG1, DP2, CAP, REL, UA11, rowColSize, new_output, ref SRGOutput, ref EAOutput, stage + 1, EAOutputFile, outputfile, overwrite, EADyadic, outputDyadic, networkid);
            }
        }


        private static Matrix UpdateAOC(Matrix _UA, Matrix _CUA, Vector _AOC, Matrix CAP, Matrix SRGC, Matrix _A, Matrix PAN, int stage)
        {
            double sum1 = 0;
            double sum2 = 0;
            
            // Calculate sum for AOC11
            for (int i = 0; i < _AOC.Size; i++)
                sum1 += _AOC[i];

            // Update the UA11 matrix into UA12 matrix
            Matrix UA = new Matrix(_CUA.Rows, _CUA.Cols);
            
            for (int i = 0; i < _CUA.Rows; i++)
            {
                for (int j = 0; j < _CUA.Cols; j++)
                {
                    if (_CUA[i, j] == _CUA.GetMaxInRow(i))
                        UA[i, j] = 0;
                    else
                    {
                        // excel sheet assigns value from _UA instead of _CUA
                        //UA[i, j] = _CUA[i, j];
                        UA[i, j] = _UA[i, j];
                    }
                }
            }

            // Generate a matrix CUA11
            Matrix CUA = new Matrix(UA.Rows, UA.Cols);
            if (stage == 1)
            {
                for (int i = 0; i < UA.Rows; i++)
                {
                    for (int j = 0; j < UA.Cols; j++)
                    {
                        if (UA[i, j] != UA.GetMaxInRow(i))
                            CUA[i, j] = 0;
                        else if (UA[i, j] == UA.GetMaxInRow(i) && PAN[i, j] > 0 && i != j) // needs PAN and i!=j conditions
                            CUA[i, j] = CAP[j, j];
                    }
                }
            }
            else // for the other stages
            {
                for (int i = 0; i < UA.Rows; i++)
                {
                    for (int j = 0; j < UA.Cols; j++)
                    {
                        if (UA[i, j] != UA.GetMaxInRow(i))
                            CUA[i, j] = 0;
                        else if (UA[i, j] == UA.GetMaxInRow(i) && UA[i, j] > 0 && i != j) // needs PAN and i!=j conditions
                            CUA[i, j] = CAP[j, j];
                    }
                }
            }

            // Calculate the expected alliance matrix A11
            //
            // ****** Calculated completely different in excel sheet ******
            //
            /*
            Matrix A = new Matrix(CUA.Rows, CUA.Cols);
            for (int i = 0; i < CUA.Rows; i++)
            {
                for (int j = 0; j < CUA.Cols; j++)
                {
                    if (i == j)
                        A[i, j] = CAP[i, i];
                    else if (CUA[i, j] == CUA.GetMaxInRow(i))
                        A[i, j] = CAP[j, j];
                    else
                        A[i, j] = 0;
                }
            }
            */

            // This version may be wrong; might need to use above version
            Matrix A = new Matrix(CUA.Rows, CUA.Cols);
            if (stage == 1)
            {
                for (int i = 0; i < CUA.Rows; i++)
                {
                    for (int j = 0; j < CUA.Cols; j++)
                    {
                        if (_A[i, j] > 0 || _AOC[i] == 0)
                            A[i, j] = _A[i, j];
                        else
                        {
                            if (CUA[i, j] > 0)
                                A[i, j] = CUA[i, j];
                            else
                                A[i, j] = 0;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < CUA.Rows; i++)
                {
                    for (int j = 0; j < CUA.Cols; j++)
                    {
                        if (_A[i, j] > 0)
                            A[i, j] = _A[i, j];
                        else if (CUA[i, j] > 0)
                            A[i, j] = CUA[i, j];
                        else
                            A[i, j] = 0;
                    }
                }
            }
            /*
            // Update the AOC vector to AOC11
            Vector AOC = new Vector(_AOC.Rows);
            for (int i = 0; i < A.Rows; i++)
            {
                if (A.GetRowSum(i) >= _AOC[i])
                    AOC[i] = 0;
                else
                    AOC[i] = _AOC[i] - A.GetRowSum(i);
            }
            */

            // Update the AOC vector to AOC11 --------- this one is correct ---------
            Vector AOC = new Vector(_AOC.Size);
            for (int i = 0; i < A.Rows; i++)
            {
                if (A.GetRowSum(i) >= SRGC.GetRowSum(i))
                    AOC[i] = 0;
                else
                    AOC[i] = SRGC.GetRowSum(i) - A.GetRowSum(i);
            }

            // Calculate sum for AOC12
            for (int i = 0; i < AOC.Size; i++)
                sum2 += AOC[i];

            if ((sum2 - sum1) > 0)
                throw new Exception("The difference of sum cannot be greater than zero!");
            if ((sum2 - sum1) < 0)
                return UpdateAOC(UA, CUA, AOC, CAP, SRGC, A, PAN, stage);
            else
                return A;

        }



        public static void SimulateToFile(MatrixProvider capabilitiesProvider, MatrixProvider contiguityProvider, MatrixProvider majorPowersProvider,
            MatrixProvider conflictProvider, string outputFile, string expectedAllianceFile, string conflictOutputFile, string srgOutputFile, int networkID, bool randomConflict, bool randomContig, bool randomMajorPower, bool outputDyadic, bool expectedAllianceDyadic, bool srgDyadic,
            MatrixProvider srgProvider, MatrixProvider dProvider, MatrixProvider jccProvider, MatrixProvider midProvider, MatrixProvider csProvider, MatrixProvider relProvider, int maxIter, bool overwrite, double br)
        { 
            Matrix tmpContig = contiguityProvider.ReadNext(overwrite);
            SymmetricBinaryMatrix contiguity = tmpContig == null ? null : new SymmetricBinaryMatrix(tmpContig);
            Matrix initialCapabilities = capabilitiesProvider.ReadNext(overwrite);
            Vector majorPowers = majorPowersProvider.ReadNext(overwrite) as Vector;
            Matrix conflict = conflictProvider.ReadNext(overwrite);
            Matrix srg = srgProvider.ReadNext(overwrite);
            Matrix D = dProvider.ReadNext(overwrite);
            Matrix JCC = jccProvider.ReadNext(overwrite);
            Matrix MID = midProvider.ReadNext(overwrite);
            Matrix CS = csProvider.ReadNext(overwrite);
            Matrix REL = relProvider.ReadNext(overwrite);

            //int tryIntVar1 = srg.NetworkId;
            Matrix P = null;

            if (randomConflict)
            {
                P = Matrix.Zero(conflict.Rows, conflict.Cols);
                for (int i = 0; i < P.Rows; ++i)
                    for (int j = i + 1; j < P.Rows; ++j)
                        if (contiguity == null ? srg[i, j] > 0.0 : contiguity.GetValue(i, j))
                            P[i, j] = P[j, i] = RNG.RandomFloat(0.01, 0.03);
                        else
                            P[i, j] = P[j, i] = RNG.RandomFloat(0.002, 0.004);

                for (int i = 0; i < conflict.Rows; ++i)
                    for (int j = i + 1; j < conflict.Cols; ++j)
                        conflict[i, j] = conflict[j, i] = RNG.RandomBinary(P[i, j]);
            }

            if (randomContig && contiguity != null)
            {
                if (CS == null)
                {
                    contiguity.Clear();
                    int[] values = { 3, 4, 5 };
                    for (int row = 0; row < contiguity.Rows; ++row)
                    {
                        int count = RNG.Choose<int>(values);

                        count = Math.Min(count, contiguity.Cols);

                        for (int i = 0; i < count; ++i)
                        {
                            int which = RNG.RandomInt(contiguity.Cols);
                            if (contiguity[row, which] == 1)
                                --i;
                            contiguity[row, which] = 1;
                        }
                    }
                }
            }

            if (randomMajorPower)
            {
                int[] values = { 4, 5, 6 };
                int count = RNG.Choose<int>(values);

                count = Math.Min(count, majorPowers.Size);
                majorPowers.Clear();

                for (int i = 0; i < count; ++i)
                {
                    int which = RNG.RandomInt(majorPowers.Size);
                    if (majorPowers[which] == 1)
                        --i;
                    majorPowers[which] = 1;
                }
            }

            Matrix capabilities = initialCapabilities;
            if (initialCapabilities is Vector)
            {
                capabilities = Matrix.Zero(initialCapabilities.Rows, initialCapabilities.Cols);
                capabilities.SetDiagonalFromVector(initialCapabilities as Vector);
            }

            Matrix expectedAlliance = null;
            
            if (CS != null) // 5
            {
                Matrix SRGOutput = new Matrix(srg);
                Matrix EAOutput = null;
                networkID = srg.NetworkId;
                //expectedAlliance = SimulateNAPTStageOne(capabilities, srg, contiguity, MID, D, JCC, CS, REL, ref SRGOutput, ref EAOutput, expectedAllianceFile, outputFile, overwrite, expectedAllianceDyadic, outputDyadic, networkID);
                SimulateNAPTStageOne(capabilities, srg, contiguity, MID, D, JCC, CS, REL, ref SRGOutput, ref EAOutput, expectedAllianceFile, outputFile, overwrite, expectedAllianceDyadic, outputDyadic, networkID);
                
                /*
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(expectedAlliance, outputFile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(expectedAlliance, outputFile, overwrite);
                */ 
                if (srgOutputFile != "")
                {
                    if (!srgDyadic)
                        MatrixWriter.WriteMatrixToMatrixFile(SRGOutput, srgOutputFile, overwrite);
                    else
                        MatrixWriter.WriteMatrixToDyadicFile(SRGOutput, srgOutputFile, overwrite);
                }
                
                return;
            }
            else if (conflict == null && D != null) // 4
                expectedAlliance = SimulateSimplifiedLiberalStageOne(capabilities, srg, MID, D, JCC, outputFile, br);
            else if (MID != null) // 3
                expectedAlliance = SimulateSimplifiedRealistStageOne(capabilities, srg, MID, outputFile, br);
            else if (D == null) // 1
                expectedAlliance = SimulateRealistStageOne(contiguity, capabilities, srg, majorPowers, br);
            else // 2
                expectedAlliance = SimulateLiberalStageOne(contiguity, capabilities, srg, majorPowers, D, JCC, br);

            //if (outputDyadic) MatrixWriter.WriteDyadicHeader(outputFile);
            int editedNetworkId = srg.NetworkId;
            
            expectedAlliance.NetworkId = int.Parse(editedNetworkId + "01");   // Problem encountered 2/15/11
            expectedAlliance.CopyLabelsFrom(capabilities);
            srg.NetworkId = int.Parse(editedNetworkId + "01");
            srg.CopyLabelsFrom(capabilities);
            
            if (!outputDyadic)
                MatrixWriter.WriteMatrixToMatrixFile(expectedAlliance, outputFile, overwrite);
            else
                MatrixWriter.WriteMatrixToDyadicFile(expectedAlliance, outputFile, overwrite);
            if (srgOutputFile != "")
            {
                if (!srgDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(srg, srgOutputFile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(srg, srgOutputFile, overwrite);
            }
            if (conflict != null)
            {
                conflict.NetworkId = expectedAlliance.NetworkId;
                if (!string.IsNullOrEmpty(conflictOutputFile) && !conflictProvider.IsFromFile)
                {
                    
                    MatrixWriter.WriteMatrixToMatrixFile(conflict, conflictOutputFile, overwrite);

                }
            }

            if (CS != null) ; // 5
                //  expectedAlliance = SimulateNAPTStageTwo(...)
            else if (conflict == null && D != null) // 4
                expectedAlliance = SimulateSimplifiedLiberalStageTwo(capabilities, srg, MID, D, JCC, expectedAlliance, outputFile, srgOutputFile, editedNetworkId /*networkID*/, outputDyadic, srgDyadic, maxIter, br);
            else if (MID != null) // 3
                expectedAlliance = SimulateSimplifiedRealistStageTwo(capabilities, srg, MID, expectedAlliance, outputFile, srgOutputFile, networkID, outputDyadic, srgDyadic, maxIter, br);
            else if (D == null) // 1
                expectedAlliance = SimulateRealistStageTwo(conflict, expectedAlliance, capabilities, br);
            else // 2
                expectedAlliance = SimulateLiberalStageTwo(conflict, expectedAlliance, capabilities, D, JCC, br);

            if (MID == null) //if not simplified version
            {
                expectedAlliance.NetworkId = int.Parse(editedNetworkId + "02");
                expectedAlliance.CopyLabelsFrom(capabilities);
                
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(expectedAlliance, outputFile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(expectedAlliance, outputFile, overwrite);
            }

            if (conflict != null) //if conflict file exists
            {
                P = new Matrix(conflict);
                for (int i = 0; i < P.Rows; ++i)
                {
                    for (int j = i + 1; j < P.Cols; ++j)
                    {
                        P[j, i] = P[i, j] = RNG.RandomFloat(0.002, 0.004);
                        if (conflict[i, j] == 1)
                            P[j, i] = P[i, j] = RNG.RandomFloat(0.08, 0.25);
                        if (expectedAlliance[i, j] == 0.5)
                            P[j, i] = P[i, j] = 1 / ((double)P.Rows * (double)(P.Rows - 1));
                        else if (expectedAlliance[i, j] == 1)
                            P[j, i] = P[i, j] = 0.5 / ((double)P.Rows * (double)(P.Rows - 1));
                    }
                }

                conflict = new Matrix(P.Rows);
                conflict.RowLabels.CopyFrom(expectedAlliance.RowLabels);
                conflict.ColLabels.CopyFrom(expectedAlliance.ColLabels);
                for (int i = 0; i < conflict.Rows; ++i)
                    for (int j = i + 1; j < conflict.Cols; ++j)
                        conflict[i, j] = conflict[j, i] = RNG.RandomBinary(P[i, j]);


                if (D == null)
                    expectedAlliance = SimulateRealistStageTwo(conflict, expectedAlliance, capabilities, br);
                else
                    expectedAlliance = SimulateLiberalStageTwo(conflict, expectedAlliance, capabilities, D, JCC, br);

                expectedAlliance.NetworkId = int.Parse(editedNetworkId + "03");
                expectedAlliance.CopyLabelsFrom(capabilities);
                
                if (!outputDyadic)
                    MatrixWriter.WriteMatrixToMatrixFile(expectedAlliance, outputFile, overwrite);
                else
                    MatrixWriter.WriteMatrixToDyadicFile(expectedAlliance, outputFile, overwrite);

                conflict.NetworkId = expectedAlliance.NetworkId;
                if (!string.IsNullOrEmpty(conflictOutputFile) && !conflictProvider.IsFromFile)
                {
                    
                    if (!outputDyadic)
                        MatrixWriter.WriteMatrixToMatrixFile(expectedAlliance, outputFile, overwrite);
                    else
                        MatrixWriter.WriteMatrixToDyadicFile(expectedAlliance, outputFile, overwrite);
                }
            }//if conflict file exists

        }
    }
}

