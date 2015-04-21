using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.Windows.Forms;
using System.IO;

namespace Network
{
    class NetworkCharacteristicsLabels
    {
        protected string[] labels = { "Net. Ident.", "N", "No. Clqs.", "Clq. Size", "Clq. Mem.", "No. Components", "G/N" , "GPOL0", "GMOI", "NPI", "GO", "GO1",
                                    "GPOL Cohesion", "NPI Cohesion GMO", "NPI Cohesion GO", "CPsz", "GPOLcsz", "NPIcszcmo", 
                                    "NPIcszgo","NoLongerNeeded","CodeWasntModular", "Density", "Transitivity", "SYSIN", "ERPOL", "Average indegree", "Average outdegree", "DIG", "DOG", "CIG", "COG", "BIG", "BOG", "EIG", "EOG"};

        public NetworkCharacteristicsLabels()
        {
        }

        protected virtual bool UseLabel(string label)
        {
            return true;
        }

        public string Label
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in labels)
                {
                    if (UseLabel(s))
                    {
                        sb.Append(s);
                        sb.Append(',');
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }
    }

    class NetworkCharacteristics : NetworkCharacteristicsLabels
    {
        private NetworkIO net;
        private int netID;
        private Dictionary<string, bool> labelSettings;
        private string inputType, externalFile, svcFile;
        MatrixComputations.TransitivityType transitivityType;
        private double cutoff, density;
        private bool zeroDiagonal;
        private int reachMatrixCount;
        private string erpolType;
        private double alpha;
        private int kCliqueValue;
        private bool kCliqueDiag;
        public string selMeth; //Selection method specified in options form

        public NetworkCharacteristics(NetworkIO n, int i, string _inputType, string _externalFile, string _svcFile, MatrixComputations.TransitivityType _transitivityType,
            double _cutoff, double _density, bool _zeroDiagonal, int _reachMatrixCount, string _erpolType, double _alpha, int _kCliqueValue, bool _kCliqueDiag)
        {
            net = n;
            netID = i;
            labelSettings = new Dictionary<string, bool>(labels.Length);
            foreach (string s in labels)
            {
                labelSettings[s] = true;
            }
            inputType = _inputType;
            externalFile = _externalFile;
            svcFile = _svcFile;
            transitivityType = _transitivityType;
            cutoff = _cutoff;
            density = _density;
            zeroDiagonal = _zeroDiagonal;
            reachMatrixCount = _reachMatrixCount;
            erpolType = _erpolType;
            alpha = _alpha;
            kCliqueDiag = _kCliqueDiag;
            kCliqueValue = _kCliqueValue;
        }

        public NetworkCharacteristics() 
        {
            net = null;
            labelSettings = null;
        }

        protected override bool UseLabel(string label)
        {
            return labelSettings[label];
        }

        public void SetLabel(string s, bool val)
        {
            labelSettings[s] = val;
        }
        public void SetLabel(int i, bool val)
        {
            labelSettings[labels[i]] = val;
        }

        private bool CohesionSet()
        {
            return labelSettings["GPOL Cohesion"] || labelSettings["NPI Cohesion GMO"] || labelSettings["NPI Cohesion GO"];
        }
        private bool SizeSet()
        {
            return labelSettings["CPsz"] || labelSettings["GPOLcsz"] || labelSettings["NPIcszcmo"] || labelSettings["NPIcszgo"];
        }

        public void Log(string text)
        {
            /*
            using (StreamWriter sw = File.AppendText("logfile.txt"))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
            */
        }


        public string[] Line
        {
            get
            {
                Log("Starting network characteristics on " + DateTime.Now.ToString());
                
                if (net == null)
                    return null;                    

                double npolStar = 0.0; // GPOL cohesion only ?
                double npolA = 0.0; // GPOL size only ?
                double npolAStar = 0.0; // GPOL cohesion and size ?

                if (CohesionSet())
                {
                    npolStar = net.NPOLStar(inputType == "StructEquiv", externalFile, cutoff, netID, density, kCliqueValue, kCliqueDiag, selMeth);
                    if (SizeSet())
                    {
                        if(selMeth == "Cliq")
                            npolAStar = net.NPOLA(netID, svcFile, true, cutoff, density, kCliqueValue, kCliqueDiag);
                        else if (selMeth == "Bloc" || selMeth == "Clus")
                            npolAStar = net.bNPOLA(netID, svcFile, true, cutoff, density, kCliqueValue, kCliqueDiag);
                        else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                            npolAStar = net.comNPOLA(netID, svcFile, true, cutoff, density, kCliqueValue, kCliqueDiag);
                    }
                }
                
                if (SizeSet())
                {
                    if (selMeth == "Cliq")
                        npolA = net.NPOLA(netID, svcFile, false, cutoff, density, kCliqueValue, kCliqueDiag);
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        npolA = net.bNPOLA(netID, svcFile, false, cutoff, density, kCliqueValue, kCliqueDiag);
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        npolA = net.comNPOLA(netID, svcFile, false, cutoff, density, kCliqueValue, kCliqueDiag);
                }
                string[] line = new string[labels.Length];
                int curLine = -1;

                Log("Computing COC (if necessary)");

                DateTime dt = DateTime.Now;
                Pair<double, double> coc = new Pair<double,double>();
                if (labelSettings["GO"] || labelSettings["GO1"] || labelSettings["NPIcszgo"] || labelSettings["NPI Cohesion GO"])
                {
                    if (selMeth == "Cliq")
                        coc = net.CliqueOverlap;
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                    {                       
                        coc = net.BlocOverlap;
                    }
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                    {                        
                        coc = net.CommOverlap;
                    }
                }
 

                TimeSpan ts = DateTime.Now - dt;
                //MessageBox.Show(ts.TotalSeconds.ToString());

                Log("Computing simple measures");

                if (labelSettings["Net. Ident."]) line[++curLine] = netID.ToString();
                if (labelSettings["N"]) line[++curLine] = net.GetMatrix("Data").Rows.ToString();
                if (labelSettings["No. Clqs."])
                {
                    if(selMeth == "Cliq")
                        line[++curLine] = net.CliqueCount.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.BlockCount.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.CommCount.ToString();
                        
                }
                if (labelSettings["Clq. Size"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.AverageCliqueSize.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.AverageBlockSize.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.AverageCommSize.ToString();
                }
                if (labelSettings["Clq. Mem."])
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.AverageCliqueMembers.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.AverageBlockMembers.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.AverageCommMembers.ToString();
                }
                if (labelSettings["No. Components"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.GetNocom.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.GetNoBcom.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.GetNoComcom.ToString();
                }
                if (labelSettings["G/N"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.GetGn.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.GetbGn.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.GetcomGn.ToString();
                }
                if (labelSettings["GPOL0"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.cNPOL.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.bNPOL.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.comNPOL.ToString();
                }
                if (labelSettings["GMOI"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.cliqueGMOI.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.blockGMOI.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.comGMOI.ToString();
                }

                Log("Computing Overlap measures");

                if (labelSettings["NPI"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.NPI.ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.bNPI.ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                    {
                        //line[++curLine] = net.comNPI.ToString();
                        line[++curLine] = net.comNPI.ToString();
                    }
                }
                if (labelSettings["GO"])
                {
                    //line[++curLine] = coc.First.ToString();
                    if (selMeth == "Bloc" || selMeth == "Clus")
                    {
                        line[++curLine] = net.bCOI.ToString();
                    }
                    else if (selMeth == "Cliq") // use to be just an else statement
                    {
                        //line[++curLine] = coc.Second.ToString();
                        line[++curLine] = net.COI.ToString();
                    }
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.comCOI.ToString();
                }
                if (labelSettings["GO1"])
                {
                    if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.GO1().ToString();                    
                    else
                        line[++curLine] = coc.Second.ToString();
                }   

                Log("Computing NPOL/NPI");

                if (labelSettings["GPOL Cohesion"]) line[++curLine] = npolStar.ToString();
                if (labelSettings["NPI Cohesion GMO"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = (npolStar * (1 - net.cliqueGMOI)).ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = (npolStar * (1 - net.blockGMOI)).ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = (npolStar * (1 - net.comGMOI)).ToString();
                }
                if (labelSettings["NPI Cohesion GO"])
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = (npolStar * net.COI).ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = (npolStar * net.bCOI).ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = (npolStar * net.comCOI).ToString();
                }//line[++curLine] = (npolStar * (1 - coc.First)).ToString();

                if (labelSettings["CPsz"]) line[++curLine] = npolA.ToString();
                if (labelSettings["GPOLcsz"]) line[++curLine] = npolAStar.ToString();
                if (labelSettings["NPIcszcmo"]) 
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = (npolAStar * net.COI).ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = (npolAStar * net.bCOI).ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = (npolAStar * net.comCOI).ToString();
                }
                if (labelSettings["NPIcszgo"])//
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = (npolAStar * (1 - net.cliqueGMOI)).ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = (npolAStar * (1 - net.blockGMOI)).ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = (npolAStar * (1 - net.comGMOI)).ToString();
                } //line[++curLine] = (npolAStar * (1 - coc.First)).ToString();

                Log("Computing other measures");

                if (labelSettings["Density"]) line[++curLine] = net.GetDensity(density).ToString();
                if (labelSettings["Transitivity"]) line[++curLine] = net.GetTransitivity(transitivityType).ToString();
                if (labelSettings["SYSIN"]) line[++curLine] = net.GetSYSIN(zeroDiagonal, density, netID, reachMatrixCount).ToString();
                if (labelSettings["ERPOL"])
                {
                    if (selMeth == "Cliq")
                        line[++curLine] = net.cGetERPOL(CohesionSet(), SizeSet(), erpolType, alpha).ToString();
                    else if (selMeth == "Bloc" || selMeth == "Clus")
                        line[++curLine] = net.bGetERPOL(CohesionSet(), SizeSet(), erpolType, alpha).ToString();
                    else if (selMeth == "Comm" || selMeth == "NewDisc" || selMeth == "NewOv")
                        line[++curLine] = net.comGetERPOL(CohesionSet(), SizeSet(), erpolType, alpha).ToString();
                }
                if (labelSettings["Average indegree"]) line[++curLine] = net.GetAvgDI.ToString();
                if (labelSettings["Average outdegree"]) line[++curLine] = net.GetAvgDO.ToString();

                if (labelSettings["DIG"]) line[++curLine] = net.GetDIG.ToString();
                if (labelSettings["DOG"]) line[++curLine] = net.GetDOG.ToString();
                if (labelSettings["CIG"]) line[++curLine] = net.GetCIG.ToString();
                if (labelSettings["COG"]) line[++curLine] = net.GetCOG.ToString();
                if (labelSettings["BIG"]) line[++curLine] = net.GetBIG.ToString();
                if (labelSettings["BOG"]) line[++curLine] = net.GetBOG.ToString();
                if (labelSettings["EIG"]) line[++curLine] = net.GetEIG.ToString();
                if (labelSettings["EOG"]) line[++curLine] = net.GetEOG.ToString();
             

                Log("Done with network characteristics on " + DateTime.Now.ToString());

                return line;
            }
        }
    }
            
}
