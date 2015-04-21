using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RandomUtilities;

using Network;
using Network.IO;
using System.IO;
using Network.Matrices;
using NetworkGUI.Forms;
using NetworkGUI;
using System.Threading;

namespace NetworkGUI
{
    public partial class MainForm : Form
    {
        RandomForm _randomForm = new RandomForm();
        ValuedRandomForm _vrandomForm = new ValuedRandomForm();
        CentralityForm _centralityForm = new CentralityForm();
        CliqueForm _cliqueForm = new CliqueForm();
        CliqueForm _blockForm = new CliqueForm();
        OptionsForm _optionsForm = new OptionsForm();
        MultiplicationForm _multiplicationForm = new MultiplicationForm();
        BlocForm _blocForm = new BlocForm();
        NetworkFormationSimulationForm _nfsForm;
        NetworkPowerForm _npForm = new NetworkPowerForm();
        CliqueOptionForm _cliqueOptionForm = new CliqueOptionForm();
         Multiple_Clique_Analysis _MCA_form;
         ClusteringForm _clusterForm = new ClusteringForm();

        bool _randomSymmetric = false;

        Network.NetworkGUI net = new Network.NetworkGUI();
        int startYear;
        int currentYear;
        int reachBinary = 0;
        string loadFrom;
        string displayMatrix, prevDisplayMatrix;

        bool[] MCAcounteroptionlist;
        bool MCAuseweight;
        string MCAweightfilename;

        List<clique> cliques = new List<clique>();

        string[] fileNames; // File names for multiple files 

        Network.ElementwiseFormat ef = Network.ElementwiseFormat.Matrix;

        private const string versionString = "3.76";

        private enum MatrixType
        {
            Data, Affiliation, Overlap, SEE, SEC, SESE, CBCO, Reachability, Dependency, Centrality, Components, Characteristics, EventOverlap,
            NationalDependency, Counter, Multiplication, CONCOR, IntercliqueDistance, Elementwise, BinaryComplement, Triadic,
            RoleEquivalence, AffilEuclidean, AffilCorrelation, AffilCorrelationEvent, AffilEucildeanEvent, DataEvent, BlockPartitionS, BlockPartitionI, DensityBlockMatrix,
            RelativeDensityBlockMatrix, BlockCohesivenessMatrix, BlockCharacteristics, ClusterPartition, DensityClusterMatrix, RelativeDensityClusterMatrix, ClusterCohesivenessMatrix
        }


        public MainForm()
        {
            InitializeComponent();

            startYear = -1;
            currentYear = -1;
            loadFrom = "";
            displayMatrix = "Data";

            SetChecked();

            _optionsForm.ReachNumMatrices = 1;


            _randomForm.N = 3;
            _vrandomForm.N = 3;

            _vrandomForm.vmin = 0;
            _vrandomForm.vmax = 100;


            Text = "Maoz Social Networks Program V. " + versionString;

            helpProvider.HelpNamespace = "Network.chm";

            _blockForm.Text = "Block Characteristics";
            _blockForm.ButtonText = "Generate Block Characteristics";

            _MCA_form = new NetworkGUI.Forms.Multiple_Clique_Analysis(this);
            _nfsForm = new NetworkFormationSimulationForm(this);

           // initialmcacounter();
        }

        private void LoadData()
        {
            // Load matrix into GUI
            DoStandardize();
           // Thread t = new Thread(delegate() { DoLoadCorrect(currentYear);  });
           // t.Start();
           //while (!t.IsAlive) ;
           // Thread.Sleep(100);
           // t.Join();
            DoLoadCorrect(currentYear, reachBinary);
            switch (displayMatrix)
            {
                case "Affiliation":
                    if (_optionsForm.DisplayCliqueOption == "Save")
                        SaveAffiliationWithoutDisplay();
                    else
                            net.LoadAffiliationIntoDataGridView(dataGrid, _optionsForm.Cutoff[currentYear], _optionsForm.InputType == "StructEquiv",
                                                            _optionsForm.FileName, _optionsForm.InputType == "Dyadic", currentYear, _optionsForm.Density, _optionsForm.InputType != "None",
                                                            _optionsForm.SumMean, _optionsForm.SumMeanFilename, _optionsForm.svcFile, _optionsForm.DisplayCliqueOption,
                                                            cliqueSizeToolStripMenuItem.Checked, cliqueCohesionToolStripMenuItem.Checked, estebanRayIndexToolStripMenuItem.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;

                case "Counter":
                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                        _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                        _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "ViableCounter":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);

                    Network.NetworkGUI viableNet = new Network.NetworkGUI(net);
                    viableNet.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                        _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                        _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "CONCOR":
                    net.LoadBlocAffiliationIntoDataGridView(dataGrid, false);
                    break;
                case "Clustering":
                    net.LoadBlocAffiliationIntoDataGridView(dataGrid, true);
                    break;
                case "NatDep":
                    net.LoadNationalDependencyIntoDataGridView(dataGrid, currentYear);
                    break;
                case "CBCO":
                    if (_optionsForm.DisplayCliqueOption == "Save")
                        SaveAffiliationWithoutDisplay();
                    else
                    {
                   //     Thread v = new Thread(delegate() { net.LoadCBCOverlapIntoDataGridView(dataGrid, _optionsForm.DisplayCliqueOption); });
                    //    v.Start();
                     //   while (!v.IsAlive) ;
                    //       Thread.Sleep(100);
                     //   v.Join();
                        net.LoadCBCOverlapIntoDataGridView(dataGrid, _optionsForm.DisplayCliqueOption);
                    }
                    break;
                case "CBCODiag":
                    if (_optionsForm.DisplayCliqueOption == "Save")
                        SaveAffiliationWithoutDisplay();
                    else
                       net.LoadCBCOverlapIntoDataGridView(dataGrid, true, _optionsForm.DisplayCliqueOption);
                    break;
                case "Centrality":
                    net.LoadCentralityIntoDataGridView(dataGrid, _centralityForm.Avg);
                    break;
                case "Components":
                    net.LoadComponentsIntoDataGridView(dataGrid, _optionsForm.Cutoff[currentYear], _optionsForm.InputType == "StructEquiv",
                                                        _optionsForm.FileName, _optionsForm.InputType == "Dyadic", currentYear, _optionsForm.Density, _optionsForm.InputType != "None",
                                                        _optionsForm.SumMean, _optionsForm.SumMeanFilename, _optionsForm.svcFile,
                                                        cliqueSizeToolStripMenuItem.Checked, cliqueCohesionToolStripMenuItem.Checked, estebanRayIndexToolStripMenuItem.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                default:
                    net.LoadMatrixIntoDataGridView(dataGrid, displayMatrix);
                    break;
            }

        }

        private void DoLoadCorrect(int currentYear, int options)
        {
            switch (displayMatrix)
            {
                case "Affiliation":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "Overlap":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "CBCO":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, true, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "Dependency":
                    net.LoadDependency(prevDisplayMatrix, _optionsForm.ReachNumMatrices, _optionsForm.Density, currentYear, _optionsForm.reachZero);
                    break;
                case "Reachability": 
                    net.LoadReachability(_optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, currentYear, reachBinary);
                    break;
                case "SEE":
                    if (!openFileDialog.Multiselect)
                        net.LoadStructEquiv(_optionsForm.Density, currentYear, prevDisplayMatrix);
                    break;
                case "SEC":
                    if (!openFileDialog.Multiselect)
                        net.LoadStructEquiv(_optionsForm.Density, currentYear, prevDisplayMatrix);
                    break;
                case "SESE":
                    if (!openFileDialog.Multiselect)
                        net.LoadStructEquiv(_optionsForm.Density, currentYear, prevDisplayMatrix);
                    break;
                case "Centrality":
                    net.LoadCentralityIndices(prevDisplayMatrix, currentYear, _centralityForm.Sijmax, _centralityForm.CountMember, _centralityForm.ZeroDiagonal);
                    break;
                case "Characteristics":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadCliqueCharacteristics(_cliqueForm.SVC, _cliqueForm.DVC, _cliqueForm.attrMatrix, currentYear);
                    break;
                case "NatDep":
                    net.LoadDependency(prevDisplayMatrix, _optionsForm.ReachNumMatrices, _optionsForm.Density, currentYear, _optionsForm.reachZero);
                    break;
                case "Multiplication":
                    try
                    {
                        net.LoadMultiplicationMatrix(_multiplicationForm.fileName, _multiplicationForm.dyadic, currentYear, "Multiplication", prevDisplayMatrix);
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Error loading multiplication matrix: " + E.Message, "Error!");
                        return;
                    }
                    break;
                case "CONCOR":
             //       net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    break;
                case "ICD":
                    net.LoadIntercliqueDistance(_optionsForm.Density, currentYear);
                    break;
                case "Elementwise":
                    net.LoadElementwiseMultiplication(openFileDialog2.FileName, currentYear, "Elementwise", ef);
                    break;
                case "BinaryComplement":
                    net.LoadBinaryComplement(prevDisplayMatrix);
                    break;
                case "Triadic":
                    net.LoadTriadic(prevDisplayMatrix, currentYear);
                    break;
                case "RoleEquiv":
                    if (!openFileDialog.Multiselect)
                        net.LoadRoleEquivalence(prevDisplayMatrix);
                    break;
                case "BlockPartitionI":
                   // net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockPartitionMatrix();
                    break;
                case "BlockPartitionS":
                    //net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps); ;
                    net.LoadBlockPartitionMatrix();
                    break;
                case "DensityBlockMatrix":
                   // net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                    break;
                case "RelativeDensityBlockMatrix":
                   // net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                    break;
                case "BlockCohesivenessMatrix":
                  //  net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                    break;
                case "BlockCharacteristics":
                    //net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockCharacteristics(_blockForm.SVC, _blockForm.DVC, _blockForm.attrMatrix, currentYear, false);
                    break;
                case "ClusterPartition":
                    //net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadClusterPartitionMatrix();
                    break;
                case "DensityClusterMatrix":
                    // net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadClusterMatrices(_optionsForm.Density, currentYear);
                    break;
                case "RelativeDensityClusterMatrix":
                    // net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadClusterMatrices(_optionsForm.Density, currentYear);
                    break;
                case "ClusterCohesivenessMatrix":
                    //  net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadClusterMatrices(_optionsForm.Density, currentYear);
                    break;
                case "ClusterCharacteristics":
                    //net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadBlockCharacteristics(_blockForm.SVC, _blockForm.DVC, _blockForm.attrMatrix, currentYear, true);
                    break;
                case "ViableCoalitions":
                    //net.LoadStructEquiv(_optionsForm.Density, currentYear, prevDisplayMatrix);
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);
                    break;
                case "CoalitionStructure":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);
                    break;
                case "ViableNPI":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);
                    break;
                case "Distance":
                    net.LoadDistanceMatrix(_optionsForm.Cutoff.GetValue(currentYear));
                    break;
                case "Components":
                    net.LoadComponents(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero);
                    break;
                case "NetworkPower":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType == "None" || _optionsForm.InputType == "StructEquiv", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    if(!_npForm.CliquePower)
                         net.CONCOR(_npForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunctionnp(), _npForm.MaxNoSteps);
                    if(_npForm.useattributefile)
                        net.LoadattributeVector(_npForm.attributefilename);
                    net.LoadNetworkpower(_npForm.useattributefile, currentYear, _npForm.CliquePower, _npForm.showSP, _optionsForm.InputType, _optionsForm.FileName, _optionsForm.Density);
                    break;
                case "Clustering":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    break;
                case "Community":
                    net.LoadCommunity(currentYear);
                    break;
            }
        }

        private void SetFormTitle()
        {
            string file;
            if (loadFrom == "Random")
            {
                this.Text = "Matrix Manipulator v" + versionString + " - Random Data";
                return;
            }
            if (loadFrom == "")
            {
                this.Text = "Matrix Manipulator v" + versionString;
                return;
            }

            if (openFileDialog.Multiselect)
                file = fileNames[fileNames.Length - 1];
            else
                file = openFileDialog.FileName;
            this.Text = string.Concat("Matrix Manipulator v" + versionString + " - ",
                    file.Substring(file.LastIndexOf('\\') + 1),
                    " - ", currentYear.ToString());
            if (openFileDialog.Multiselect)
                this.Text += " [Multiple Files]";
        }


        private uint[] RandomArray(int N)
        {
            uint[] a = new uint[N * N / 8 + 1];
            for (int i = 0; i < a.Length; ++i)
            {
                a[i] = (uint)RNG.RandomInt();
            }
            return a;
        }

        private void UncheckAllItems(MenuStrip menu)
        {
            foreach (ToolStripMenuItem item in menu.Items)
                UncheckAllItems(item);
        }

        private void UncheckAllItems(ToolStripMenuItem item)
        {
            item.Checked = false;
            foreach (ToolStripMenuItem subItem in item.DropDownItems)
                UncheckAllItems(subItem);
        }

        private void SetChecked()
        {
            bool b = sECToolStripMenuItem.Checked;

            bool useCS = cliqueSizeToolStripMenuItem.Checked;
            bool useCC = cliqueCohesionToolStripMenuItem.Checked;
            bool estRay = estebanRayIndexToolStripMenuItem.Checked;

            UncheckAllItems(menuStrip);

            cliqueSizeToolStripMenuItem.Checked = useCS;
            cliqueCohesionToolStripMenuItem.Checked = useCC;
            estebanRayIndexToolStripMenuItem.Checked = estRay;

            sECToolStripMenuItem.Checked = b;
            sESEToolStripMenuItem.Checked = !b;

            if (displayMatrix != "Elementwise")
            {
                matrixToolStripMenuItem1.Checked = false;
                dyadicToolStripMenuItem.Checked = false;
                monadicFileToolStripMenuItem.Checked = false;
            }

            if (loadFrom == "Affil" && displayMatrix == "Data")
                unitBasedConversionToolStripMenuItem.Checked = true;
            else
            {
                switch (displayMatrix)
                {
                    case "Data": dataMatrixToolStripMenuItem.Checked = true; break;
                    case "Affiliation": cliqueAffiliationMatrixToolStripMenuItem.Checked = true; break;
                    case "Overlap": cliqueOverlapMatrixToolStripMenuItem.Checked = true; break;
                    case "OverlapDiag": diagonallyStandardizedToolStripMenuItem.Checked = true; break;
                    case "SEE":
                        euclideanMatrixToolStripMenuItem.Checked = true;
                        euclideanMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "SEC":
                        correlationMatrixToolStripMenuItem.Checked = true;
                        correlationMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "SESE":
                        standardizedEuclideanDistanceMatrixToolStripMenuItem.Checked = true;
                        standardizedEuclideanDistanceMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "CBCO": cliquebyCliqueOverlapMatrixToolStripMenuItem.Checked = true; break;
                    case "CBCODiag": diagonallyStandardizedToolStripMenuItem1.Checked = true; break;
                    case "Dependency": dependencyMatrixToolStripMenuItem.Checked = true; break;
                    case "Reachability": reachabilityMatrixToolStripMenuItem.Checked = true; break;
                    case "Components": componentsMatrixToolStripMenuItem.Checked = true; break;
                    case "Centrality": centralityIndicesMatrixToolStripMenuItem.Checked = true; break;
                    case "Characteristics": cliqueCharacteristicsMatrixToolStripMenuItem.Checked = true; break;
                    case "EventOverlap": eventOverlapMatrixToolStripMenuItem.Checked = true; break;
                    case "NatDep": nationalDependencyMatrixToolStripMenuItem.Checked = true; break;
                    case "Counter": counterDataToolStripMenuItem.Checked = true; break;
                    case "Multiplication": matrixMultiplicationToolStripMenuItem.Checked = true; break;
                    case "CONCOR": cONCORBlockAffiliationToolStripMenuItem.Checked = true; break;
                    case "ICD": interCliqueDistanceToolStripMenuItem.Checked = true; break;
                    case "Elementwise": elementwiseMultiplicationToolStripMenuItem.Checked = true; break;
                    case "BinaryComplement": binaryComplementToolStripMenuItem.Checked = true; break;
                    case "Triadic": triadicMatrixToolStripMenuItem.Checked = true; break;
                    case "RoleEquiv": roleEquivalenceMatrixToolStripMenuItem.Checked = true; break;
                    case "DataEvent": eventBasedConversionToolStripMenuItem.Checked = true; break;
                    case "AffilEuclidean": unitbasedConversionToolStripMenuItem2.Checked = true; break;
                    case "AffilCorrelation": unitbasedConversionToolStripMenuItem1.Checked = true; break;
                    case "AffilCorrelationEvent": eventbasedConversionToolStripMenuItem1.Checked = true; break;
                    case "AffilEuclideanEvent": eventbasedConversionToolStripMenuItem2.Checked = true; break;
                    case "BlockPartitionS": sociomatrixEntiresToolStripMenuItem.Checked = true; break;
                    case "BlockPartitionI": toolStripMenuItem1.Checked = true; break;
                    case "DensityBlockMatrix": densityToolStripMenuItem.Checked = true; break;
                    case "RelativeDensityBlockMatrix": relativeDensityToolStripMenuItem.Checked = true; break;
                    case "BlockCohesivenessMatrix": blockCoheToolStripMenuItem.Checked = true; break;
                    case "ClusterPartition":  clusterPartitionMatrixToolStripMenuItem.Checked = true; break;
                    case "DensityClusterMatrix": densityToolStripMenuItem1.Checked = true; break;
                    case "RelativeDensityClusterMatrix": relativeDensityToolStripMenuItem1.Checked = true; break;
                    case "ClusterCohesivenessMatrix": cohesivenessToolStripMenuItem.Checked = true; break;
                    case "BlockCharacteristics": blockCharacteristicToolStripMenuItem.Checked = true; break;
                    case "Distance": distanceMatrixToolStripMenuItem.Checked = true; break;
                    case "NetworkPower": networkPowerMatrixToolStripMenuItem.Checked = true; break;
                    case "Clustering": hierarchicalClusteringToolStripMenuItem.Checked = true; break;
                }

            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void matrixFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetMode(false);
                    startYear = currentYear = net.SmartLoad(openFileDialog.FileName, out loadFrom);
                    //loadFrom = "Matrix";
                    SetFormTitle();

                    if (displayMatrix == "Affil")
                        displayMatrix = "Data";
                    LoadData();

                    if (displayMatrix == "Sociomatrix")
                        displayMatrix = "Matrix";
                    SetChecked();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error opening the file: " + E.Message, "Error!");
                loadFrom = "";
                dataGrid.Columns.Clear();
                SetFormTitle();
            }
        }

        private void nextYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            net.Reset(); 

            if (currentYear == -1)
                return;

            ++currentYear;

            if (loadFrom == "Matrix")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.LoadFromMultipleFiles(fileNames, currentYear);
                }
                else
                {
                    currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Dyadic")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear);
                }
                else
                {
                    currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Affil")
            {
                currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
            }
            else if (loadFrom == "Monadic")
            {
                currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, currentYear);
            }
            else if (loadFrom == "Random")
            {
                net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                --currentYear;
            }
            else if (loadFrom == "ValuedRandom")
            {
                net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                --currentYear;
            }


            DoStandardize();
            LoadData();
            SetFormTitle();
        }

        private void previousYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentYear == -1)
                return;

            --currentYear;

            try
            {
                if (loadFrom == "Matrix")
                {
                    if (openFileDialog.Multiselect)
                    {
                        currentYear = net.LoadFromMultipleFiles(fileNames, net.GetPreviousYear(fileNames[0], currentYear));
                    }
                    else
                    {
                        currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, net.GetPreviousYear(openFileDialog.FileName, currentYear));
                    }
                }
                else if (loadFrom == "Dyadic")
                {
                    if (openFileDialog.Multiselect)
                    {
                        currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, net.GetPreviousYear(openFileDialog.FileName, currentYear));
                    }
                    else
                    {

                        currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, net.GetPreviousYear(openFileDialog.FileName, currentYear));
                    }

                }
                else if (loadFrom == "Affil")
                {
                    currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, net.GetPreviousYear(openFileDialog.FileName, currentYear));
                }
                else if (loadFrom == "Monadic")
                {
                    currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, net.GetPreviousYear(openFileDialog.FileName, currentYear));
                }
                else if (loadFrom == "Random")
                {
                    net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    ++currentYear;
                }
                else if (loadFrom == "ValuedRandom")
                {
                    net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    ++currentYear;
                }
            }
            catch (Exception E)
            {
                ++currentYear;
                MessageBox.Show("Unable to advance to previous year: " + E.Message, "Error!");
                return;
            }
            DoStandardize();
            LoadData();
            SetFormTitle();
        }

        private void jumpToYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadFrom == "Random")
            {
                MessageBox.Show("Cannot jump to specific year with random data!", "Error!");
                return;
            }
            if (loadFrom == "ValuedRandom")
            {
                MessageBox.Show("Cannot jump to specific year with random data!", "Error!");
                return;
            }

            int newYear = 0;
            if (currentYear == -1)
                return;

            JumpToForm jump = new JumpToForm();
            jump.year = currentYear;
            jump.ShowDialog();
            try
            {
                if (jump.year != currentYear) // No need to be wasteful and reload unnecessarily
                {

                    if (loadFrom == "Matrix")
                    {
                        if (openFileDialog.Multiselect)
                        {
                            newYear = net.LoadFromMultipleFiles(fileNames, jump.year);
                        }
                        else
                        {
                            newYear = net.LoadFromMatrixFile(openFileDialog.FileName, jump.year);
                        }
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        if (openFileDialog.Multiselect)
                        {
                            newYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, jump.year);
                        }
                        else
                        {
                            newYear = net.LoadFromDyadicFile(openFileDialog.FileName, jump.year);
                        }
                    }
                    else if (loadFrom == "Affil")
                    {
                        newYear = net.LoadFromAffiliationFile(openFileDialog.FileName, jump.year);
                    }
                    else if (loadFrom == "Monadic")
                    {
                        newYear = net.LoadFromMonadicFile(openFileDialog.FileName, jump.year);
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("Unable to jump to year: " + E.Message, "Error!");
                return;
            }

            if (newYear != -1)
            {
                currentYear = newYear;
                DoStandardize();
                LoadData();
                SetFormTitle();
            }
            else
            {
                if (loadFrom == "Matrix")
                {
                    if (openFileDialog.Multiselect)
                    {
                        net.LoadFromMultipleFiles(fileNames, currentYear);
                    }
                    else
                    {
                        net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                    }
                }
                else if (loadFrom == "Dyadic")
                {
                    if (openFileDialog.Multiselect)
                    {
                        net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear);
                    }
                    else
                    {
                        net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                    }
                }
                else if (loadFrom == "Affil")
                {
                    currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Monadic")
                {
                    currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, currentYear);
                }
                MessageBox.Show("That year is not present in this file!", "Error!");
            }
        }

        private void lastToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            if (loadFrom == "Random")
            {
                MessageBox.Show("Cannot go to last year with random data!", "Error!");
                return;
            }
            if (loadFrom == "ValuedRandom")
            {
                MessageBox.Show("Cannot go to last year with random data!", "Error!");
                return;
            }

            if (loadFrom == "Matrix")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.GetLastYear(fileNames[0]);
                    currentYear = net.LoadFromMultipleFiles(fileNames, currentYear);  
                }
                else
                {
                    currentYear = net.GetLastYear(openFileDialog.FileName); 
                    currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Dyadic")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.GetLastYear(openFileDialog.FileName);
                    currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear); 
                }
                else
                {
                    currentYear = net.GetLastYear(openFileDialog.FileName); 
                    currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Affil")
            {
                currentYear = net.GetLastYear(openFileDialog.FileName); 
                currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
            }
            else if (loadFrom == "Monadic")
            {
                currentYear = net.GetLastYear(openFileDialog.FileName); 
                currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, currentYear);
            } 


            DoStandardize();
            LoadData();
            SetFormTitle();
        }

        private void firstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadFrom == "Random")
            {
                MessageBox.Show("Cannot go to first year with random data!", "Error!");
                return;
            }
            if (loadFrom == "ValuedRandom")
            {
                MessageBox.Show("Cannot go to first year with random data!", "Error!");
                return;
            }

            if (loadFrom == "Matrix")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.GetFirstYear(fileNames[0]);
                    currentYear = net.LoadFromMultipleFiles(fileNames, currentYear);  
                }
                else
                {
                    currentYear = net.GetFirstYear(openFileDialog.FileName);
                    currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Dyadic")
            {
                if (openFileDialog.Multiselect)
                {
                    currentYear = net.GetFirstYear(openFileDialog.FileName);
                    currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear); 
                }
                else
                {
                    currentYear = net.GetFirstYear(openFileDialog.FileName);
                    currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
            }
            else if (loadFrom == "Affil")
            {
                currentYear = net.GetFirstYear(openFileDialog.FileName);
                currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
            }
            else if (loadFrom == "Monadic")
            {
                currentYear = net.GetFirstYear(openFileDialog.FileName);
                currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, currentYear);
            }


            DoStandardize();
            LoadData();
            SetFormTitle();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cliqueAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void dataMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableStandardizedChecks();
            if (loadFrom != "Affil")
                displayMatrix = "Data";
            else
                displayMatrix = "Affil";
            LoadData();
            SetChecked();
        }

        private void SetNewDisplayMatrix(string s)
        {
            prevDisplayMatrix = displayMatrix;
            displayMatrix = s;
        }

        private void cliqueOverlapMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Overlap");
            LoadData();
            SetChecked();
        }

        private void cliquebyCliqueOverlapMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CBCO");
            LoadData();
            SetChecked();
        }

        private void dependencyMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Dependency");
            LoadData();
            SetChecked();
        }

        private void structuralEquivalenceMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void structuralEquivalenceEMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void matrixFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            if (displayMatrix == "Counter")
            {
                counterDataFileToolStripMenuItem_Click(sender, e);
                return;
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();

                if (displayMatrix == "Affiliation")
                    range.SetMode(true);

                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm progress = new ProgressForm();
                progress.endYear = endYear;
                progress.startYear = startYear;
                progress.curYear = 0;
                progress.Show();

                // Should we standardize?
                if (byRowToolStripMenuItem.Checked == true)
                    net.StandardizeByRow(displayMatrix);
                else if (byColumnToolStripMenuItem.Checked == true)
                    net.StandardizeByColumn(displayMatrix);
                else if (rowToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalRow(displayMatrix);
                else if (columnToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalColumn(displayMatrix);
                else if (minimumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMinimum(displayMatrix);
                else if (maximumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMaximum(displayMatrix);

                int year = startYear;
                while (true)
                {
                    progress.curYear = year;

                    if (displayMatrix == "Affiliation")
                        net.SaveAffiliationToMatrixFile(saveFileDialog.FileName, year, _optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.InputType == "StructEquiv",
                                                        _optionsForm.FileName, _optionsForm.InputType == "Dyadic", _optionsForm.Density, _optionsForm.SumMean, _optionsForm.SumMeanFilename,
                                                        _optionsForm.svcFile, _optionsForm.SaveOverwrite && year == startYear,
                                                        cliqueSizeToolStripMenuItem.Checked, cliqueCohesionToolStripMenuItem.Checked, estebanRayIndexToolStripMenuItem.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    else if (displayMatrix == "Affil" && loadFrom == "Affil")
                        net.SaveAffiliationMatrixToMatrixFile(saveFileDialog.FileName, year, _optionsForm.SaveOverwrite && year == startYear);
                    else if (displayMatrix == "NatDep")
                        net.SaveNationalDependencyToMatrixFile(saveFileDialog.FileName, year, _optionsForm.SaveOverwrite && year == startYear);
                    else if (displayMatrix == "CONCOR")
                        net.SaveBlocAffiliationToMatrixFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect);
                    else if (displayMatrix == "Clustering")
                        net.SaveBlocAffiliationToMatrixFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect);
                    else if (displayMatrix == "CBCO")
                        net.SaveCBCOverlapToFile(saveFileDialog.FileName, year, displayMatrix != "Characteristics",
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear,false);
                    else if (displayMatrix == "CBCODiag")
                        net.SaveCBCOverlapToFile(saveFileDialog.FileName, year, displayMatrix != "Characteristics",
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear,true);

                    else
                        net.SaveMatrixToMatrixFile(saveFileDialog.FileName, year, displayMatrix, displayMatrix != "Characteristics",
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear);



                    if (year < endYear)
                    {
                        if (loadFrom == "Matrix")
                        {
                            if (openFileDialog.Multiselect)
                                year = net.LoadFromMultipleFiles(fileNames, year + 1);
                            else
                                year = net.LoadFromMatrixFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Dyadic")
                        {
                            if (openFileDialog.Multiselect)
                                year = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, year + 1);
                            else
                                year = net.LoadFromDyadicFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Affil")
                        {
                            year = net.LoadFromAffiliationFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Random")
                        {
                            net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                            ++year;
                        }
                        else if (loadFrom == "ValuedRandom")
                        {
                            net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                            ++year;
                        }
                        else if (loadFrom == "Monadic")
                        {
                            year = net.LoadFromMonadicFile(openFileDialog.FileName, year + 1);
                        }

                        DoLoadCorrect(year, 0);
                    }
                    else
                        break;
                }


                if (loadFrom == "Matrix")
                {
                    if (openFileDialog.Multiselect)
                        currentYear = net.LoadFromMultipleFiles(fileNames, currentYear);
                    else
                        currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    if (openFileDialog.Multiselect)
                        currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear);
                    else
                        currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Affil")
                {
                    currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Random")
                {
                    net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                }
                else if (loadFrom == "ValuedRandom")
                {
                    net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                }
            }
        }

        private void SaveAffiliationWithoutDisplay() //copy most of code from previous method
        {
            if (loadFrom == "")
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();

                if (displayMatrix == "Affiliation")
                    range.SetMode(true);

                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm progress = new ProgressForm();
                progress.endYear = endYear;
                progress.startYear = startYear;
                progress.curYear = 0;
                progress.Show();

                // Should we standardize?
                if (byRowToolStripMenuItem.Checked == true)
                    net.StandardizeByRow(displayMatrix);
                else if (byColumnToolStripMenuItem.Checked == true)
                    net.StandardizeByColumn(displayMatrix);
                else if (rowToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalRow(displayMatrix);
                else if (columnToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalColumn(displayMatrix);
                else if (minimumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMinimum(displayMatrix);
                else if (maximumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMaximum(displayMatrix);

                int year = startYear;
                while (true)
                {
                    progress.curYear = year;
                    if (displayMatrix == "Affiliation")
                        net.SaveAffiliationToMatrixFile(saveFileDialog.FileName, year, _optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.InputType == "StructEquiv",
                                                         _optionsForm.FileName, _optionsForm.InputType == "Dyadic", _optionsForm.Density, _optionsForm.SumMean, _optionsForm.SumMeanFilename,
                                                         _optionsForm.svcFile, _optionsForm.SaveOverwrite && year == startYear,
                                                         cliqueSizeToolStripMenuItem.Checked, cliqueCohesionToolStripMenuItem.Checked, estebanRayIndexToolStripMenuItem.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    else
                    {
                        //Thread s = new Thread(delegate()
                        //{
                        //    net.SaveCBCOverlapToFile(saveFileDialog.FileName, year, displayMatrix != "Characteristics",
                        //        displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                        //});
                     //   s.Start();
                        //while (!s.IsAlive) ;
                        //    Thread.Sleep(1);
                          //s.Join();
                        net.SaveCBCOverlapToFile(saveFileDialog.FileName, year, displayMatrix != "Characteristics",
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear,false);
                    }
                    if (year < endYear)
                    {
                        if (loadFrom == "Matrix")
                        {
                            if (openFileDialog.Multiselect)
                                year = net.LoadFromMultipleFiles(fileNames, year + 1);
                            else
                                year = net.LoadFromMatrixFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Dyadic")
                        {
                            if (openFileDialog.Multiselect)
                                year = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, year + 1);
                            else
                                year = net.LoadFromDyadicFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Affil")
                        {
                            year = net.LoadFromAffiliationFile(openFileDialog.FileName, year + 1);
                        }
                        else if (loadFrom == "Random")
                        {
                            net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                            ++year;
                        }
                        else if (loadFrom == "ValuedRandom")
                        {
                            net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                            ++year;
                        }
                        else if (loadFrom == "Monadic")
                        {
                            year = net.LoadFromMonadicFile(openFileDialog.FileName, year + 1);
                        }
                        //Thread t = new Thread(delegate() { DoLoadCorrect(year); });
                      //  t.Start();
                        //while (!t.IsAlive) ;
                        //   Thread.Sleep(1);
                      //  t.Join();
                        DoLoadCorrect(year, 0);
                    }
                    else
                        break;
                }


                if (loadFrom == "Matrix")
                {
                    if (openFileDialog.Multiselect)
                        currentYear = net.LoadFromMultipleFiles(fileNames, currentYear);
                    else
                        currentYear = net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    if (openFileDialog.Multiselect)
                        currentYear = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, currentYear);
                    else
                        currentYear = net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Affil")
                {
                    currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Random")
                {
                    net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                }
                else if (loadFrom == "ValuedRandom")
                {
                    net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                }
            }
        }
        private void dyadicFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetMode(false);
                    //                    Matrix m = MatrixReader.ReadMatrixFromFile(openFileDialog.FileName);
                    startYear = currentYear = net.SmartLoad(openFileDialog.FileName, out loadFrom);
                    //loadFrom = "Dyadic";
                    SetFormTitle();
                    if (displayMatrix == "Affil")
                        displayMatrix = "Data";

                    LoadData();

                    if (displayMatrix == "Sociomatrix")
                        displayMatrix = "Matrix";
                    SetChecked();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error opening the file:" + E.Message, "Error!");
                loadFrom = "";
                dataGrid.Columns.Clear();
                SetFormTitle();
            }
        }

        private void dyadicFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();
                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm p = new ProgressForm(startYear, endYear, 0);
                p.Show();

                int previousYear = -1;
                for (int year = startYear; year <= endYear; ++year)
                {
                    if (loadFrom == "Matrix")
                    {
                        if (openFileDialog.Multiselect)
                            year = net.LoadFromMultipleFiles(fileNames, year);
                        else
                            year = net.LoadFromMatrixFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        if (openFileDialog.Multiselect)
                            year = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, year);
                        else
                            year = net.LoadFromDyadicFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Affil")
                    {
                        year = net.LoadFromAffiliationFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Random")
                    {
                        net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    }
                    else if (loadFrom == "ValuedRandom")
                    {
                        net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    }
                    else if (loadFrom == "Monadic")
                    {
                        year = net.LoadFromMonadicFile(openFileDialog.FileName, year );
                    }

                    // Should we standardize?
                    if (byRowToolStripMenuItem.Checked == true)
                        net.StandardizeByRow(displayMatrix);
                    else if (byColumnToolStripMenuItem.Checked == true)
                        net.StandardizeByColumn(displayMatrix);
                    else if (rowToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalRow(displayMatrix);
                    else if (columnToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalColumn(displayMatrix);
                    else if (minimumToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalMinimum(displayMatrix);
                    else if (maximumToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalMaximum(displayMatrix);

                    if (year != previousYear && year <= endYear)
                    {
                        // First load correct matrix
                        if (displayMatrix == "SEE" || displayMatrix == "SEC" || displayMatrix == "SESE")
                            net.LoadStructEquiv(_optionsForm.Density, year, prevDisplayMatrix);
                        else if (displayMatrix == "Dependency")
                            net.LoadDependency(prevDisplayMatrix, _optionsForm.ReachNumMatrices, _optionsForm.Density, year, _optionsForm.reachZero);
                        else if (displayMatrix == "Reachability")
                            net.LoadReachability(_optionsForm.ReachNumMatrices, _optionsForm.reachSum, prevDisplayMatrix, currentYear, reachBinary);
                        else if (displayMatrix == "Components")
                            net.LoadComponents(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero);
                        else if (displayMatrix == "Overlap" || displayMatrix == "OverlapDiag" || displayMatrix == "CBCO" || displayMatrix == "CBCODiag")
                            net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                        else if (displayMatrix == "Centrality")
                            net.LoadCentralityIndices(prevDisplayMatrix, year, _centralityForm.Sijmax, _centralityForm.CountMember, _centralityForm.ZeroDiagonal);
                        else if (displayMatrix == "Characteristics")
                            net.LoadCliqueCharacteristics(_cliqueForm.SVC, _cliqueForm.DVC, _cliqueForm.attrMatrix, year);
                        else if (displayMatrix == "Multiplication")
                            net.LoadMultiplicationMatrix(_multiplicationForm.fileName, _multiplicationForm.dyadic, year, "Multiplication", prevDisplayMatrix);
                        else if (displayMatrix == "Elementwise")
                            net.LoadElementwiseMultiplication(openFileDialog2.FileName, currentYear, "Elementwise", ef);
                        else if (displayMatrix == "BinaryComplement")
                            net.LoadBinaryComplement(prevDisplayMatrix);
                        else if (displayMatrix == "RoleEquiv")
                            net.LoadRoleEquivalence(prevDisplayMatrix);
                        else if (displayMatrix == "AffilCorrelation" || displayMatrix == "AffilEuclidean")
                            ;
                        else if (displayMatrix != "Data")
                        {
                            MessageBox.Show("The matrix type '" + displayMatrix + "' cannot be saved to a dyadic file!", "Error!");
                            return;
                        }
                        string s = net.MakeDefaultDyadicLabel(displayMatrix);
                        if (year != startYear)
                            s = null;
                        net.SaveMatrixToDyadicFile(saveFileDialog.FileName, year, displayMatrix, s, _optionsForm.SaveOverwrite && year == startYear);

                    }
                    p.curYear = year;
                    previousYear = year;
                }

                if (currentYear == endYear)
                    return;

                if (loadFrom == "Matrix")
                {
                    net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Affil")
                {
                    net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Monadic")
                {
                    net.LoadFromMonadicFile(openFileDialog.FileName, currentYear);
                }
            }
        }

        private void counterDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {


                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();
                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm progress = new ProgressForm();
                progress.endYear = endYear;
                progress.startYear = startYear;
                progress.curYear = 0;
                progress.Show();

                int previousYear = -1;
                for (int year = startYear; year <= endYear; ++year)
                {
                    if (loadFrom == "Matrix")
                    {
                        year = net.LoadFromMatrixFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        year = net.LoadFromDyadicFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Random")
                    {
                        net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    }
                    else if (loadFrom == "ValuedRandom")
                    {
                        net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    }
                    if (year != previousYear && year <= endYear)
                    {
                        net.SaveCounterToFile(saveFileDialog.FileName, year, year == startYear, ",", _optionsForm.Cutoff[year], _optionsForm.Density,
                            _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion, _optionsForm.transitivityType,
                            _optionsForm.counterOptions, _optionsForm.SaveOverwrite && year == startYear, _optionsForm.reachZero, _optionsForm.reachSum, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    }
                    progress.curYear = year;
                    Application.DoEvents();
                    previousYear = year;
                }

                if (currentYear == endYear)
                    return;

                if (loadFrom == "Matrix")
                {
                    net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
            }

        }

        private void multipleFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = true;

            MultipleFileForm fileForm = new MultipleFileForm(this);
            fileForm.Show();
        }

        // This function does the actual file loading
        // It is called by the fileForm
        public void loadFromMultipleFiles(MultipleFileForm fileForm)
        {
            SetMode(true, false);
            fileNames = fileForm.FileList;
            try
            {
                currentYear = net.LoadFromMultipleFiles(fileNames, -1);
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error loading from multiple files: " + e.Message, "Error!");
            }
            loadFrom = "Matrix";
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
        }

        private void correlationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SEC");
            LoadData();
            SetChecked();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void euclideanMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SEE");
            LoadData();
            SetChecked();
        }

        private void correlationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SEC");
            LoadData();
            SetChecked();
        }

        private void euclideanMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SEE");
            LoadData();
            SetChecked();
        }

        private void multivariableDyadicFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
           // try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetMode(true, true);
                    loadFrom = "Dyadic";
                    fileNames = openFileDialog.FileNames;
                    currentYear = net.LoadFromMultivariableDyadicFile(fileNames[0], 1);
                    SetFormTitle();
                    if (displayMatrix == "Affil")
                        displayMatrix = "Data";
                    LoadData();

                }
            }
           /* catch (Exception E)
            {
                MessageBox.Show("There was an error opening the multivariable dyadic file: " + E.Message, "Error!");
                loadFrom = "";
                dataGrid.Columns.Clear();
                SetFormTitle();
            }*/
        }

        private void SetMode(bool MultipleFiles) { SetMode(MultipleFiles, true); }
        private void SetMode(bool MultipleFiles, bool MultiVariable)
        {
            if (MultipleFiles)
            {
                openFileDialog.Multiselect = true;
                multivariableDyadicFileToolStripMenuItem1.Enabled = !MultiVariable;
                multipleMatrixFilesToolStripMenuItem.Enabled = MultiVariable;
                structuralEquivalenceToolStripMenuItem.Enabled = false;
                multipleStructuralEquivalenceToolStripMenuItem.Enabled = true;
            }
            else
            {
                openFileDialog.Multiselect = false;
                multivariableDyadicFileToolStripMenuItem1.Enabled = false;
                multipleMatrixFilesToolStripMenuItem.Enabled = false;
                structuralEquivalenceToolStripMenuItem.Enabled = true;
                multipleStructuralEquivalenceToolStripMenuItem.Enabled = false;
            }
            if (loadFrom == "Affil")
                sociomatrixToolStripMenuItem.Enabled = eventOverlapMatrixToolStripMenuItem.Enabled = true;
            else
                sociomatrixToolStripMenuItem.Enabled = eventOverlapMatrixToolStripMenuItem.Enabled = false;
        }

        private void multipleMatrixFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            // Get however many we will need
            int fileCount = net.CountVarsInMultivariableDyadicFile(openFileDialog.FileName);

            MessageBox.Show("The multivariable dyadic data will be saved to " + fileCount + " matrix files."
                + " Please select them in order when prompted. You will then be able to choose the year range.", "Please Note!");

            List<string> files = new List<string>();
            while (fileCount-- > 0)
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                files.Add(saveFileDialog.FileName);
            }

            int startYear, endYear;
            YearRangeForm range = new YearRangeForm();
            range.from = currentYear;
            range.to = currentYear;
            range.ShowDialog();
            startYear = range.from;
            endYear = range.to;

            for (int year = startYear; year <= endYear; ++year)
            {
                net.SaveToMultipleMatrixFiles(openFileDialog.FileName, files.ToArray(), year, _optionsForm.SaveOverwrite && year == startYear);
            }

        }

        private void multivariableDyadicFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();
                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                int prevYear = -1;
                for (int year = startYear; year <= endYear; ++year)
                {
                    prevYear = net.SaveToMultivariableDyadicFile(fileNames, saveFileDialog.FileName, year, prevYear, _optionsForm.SaveOverwrite);
                    if (prevYear == -1)
                        return;
                }

            }

        }

        private void ClearStandardizedChecks()
        {
            noneToolStripMenuItem.Checked = false;
            byRowToolStripMenuItem.Checked = false;
            byColumnToolStripMenuItem.Checked = false;
            byDiagonalToolStripMenuItem.Checked = false;
            rowToolStripMenuItem.Checked = false;
            columnToolStripMenuItem.Checked = false;
            minimumToolStripMenuItem.Checked = false;
            maximumToolStripMenuItem.Checked = false;
        }

        private void DisableStandardizedChecks()
        {
            if (displayMatrix != "Affiliation")
                net.Unstandardize(displayMatrix);
            noneToolStripMenuItem.Enabled = false;
            byRowToolStripMenuItem.Enabled = false;
            byColumnToolStripMenuItem.Enabled = false;
            byDiagonalToolStripMenuItem.Enabled = false;
        }

        private void EnableStandardizedChecks()
        {
            noneToolStripMenuItem.Enabled = true;
            byRowToolStripMenuItem.Enabled = true;
            byColumnToolStripMenuItem.Enabled = true;
            byDiagonalToolStripMenuItem.Enabled = true;
        }


        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            net.Unstandardize(displayMatrix);
            LoadData();
        }


        private void byRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            byRowToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void byColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            byColumnToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void byDiagonalToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void matrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DoStandardize()
        {
            try
            {
                if (byRowToolStripMenuItem.Checked == true)
                    net.StandardizeByRow(displayMatrix);
                else if (byColumnToolStripMenuItem.Checked == true)
                    net.StandardizeByColumn(displayMatrix);
                else if (rowToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalRow(displayMatrix);
                else if (columnToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalColumn(displayMatrix);
                else if (minimumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMinimum(displayMatrix);
                else if (maximumToolStripMenuItem.Checked == true)
                    net.StandardizeByDiagonalMaximum(displayMatrix);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to standardize by diagonal: " + e.Message, "Error!");
            }
        }

        private void affiliationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            //try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    loadFrom = "Affil";
                    SetMode(false);
                    startYear = currentYear = net.LoadFromAffiliationFile(openFileDialog.FileName, -1);
                    SetFormTitle();

                    if (displayMatrix == "Data")
                        displayMatrix = "Affil";

                    LoadData();

                }
            }
            /* catch (Exception E)
             {
                 MessageBox.Show("There was an error opening the affiliation file: " + E.Message, "Error!");
                 loadFrom = "";
                 dataGrid.Columns.Clear();
                 SetFormTitle();
             }*/

        }

        private void monadicDiagonalFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetMode(false);
                    startYear = currentYear = net.LoadFromMonadicFile(openFileDialog.FileName, -1);
                    loadFrom = "Monadic";
                    SetFormTitle();
                    if (displayMatrix == "Affil")
                        displayMatrix = "Data";
                    LoadData();

                }
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error opening the monadic file: " + E.Message, "Error!");
                loadFrom = "";
                dataGrid.Columns.Clear();
                SetFormTitle();
            }
        }

     /*   private void randomMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _randomForm.ShowDialog();
            loadFrom = "Random";
            try
            {
                net.LoadRandom(_randomForm.N, "Data", _randomSymmetric);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _randomForm.Year;
            _optionsForm.ReachNumMatrices = _randomForm.N - 1;
        }
        */
   /*     private void valuedRandomMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _vrandomForm.ShowDialog();
            loadFrom = "ValuedRandom";
            try
            {
                net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _randomForm.Year;
            _optionsForm.ReachNumMatrices = _vrandomForm.N - 1;
        }
        */
        private void centralityIndicesMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _centralityForm.ShowDialog();
            displayMatrix = "Centrality";

            LoadData();
            SetChecked();
        }

        private void componentsMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayMatrix = "Components";
            LoadData();
            SetChecked();
        }

        private void cliqueCharacteristicsMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _cliqueForm.ShowDialog();
            displayMatrix = "Characteristics";
            LoadData();
            SetChecked();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Text, "About");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Matrix Manipulator v" + versionString, "About");
        }

        private void sociomatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void eventOverlapMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("EventOverlap");
            LoadData();
            SetChecked();
        }

        private void nationalDependencyMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_optionsForm.ReachNumMatrices == -1)
                _optionsForm.ReachNumMatrices = dataGrid.Rows.Count - 1;
            SetNewDisplayMatrix("NatDep");
            LoadData();
            SetChecked();
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }

        private void standardizedEuclideanDistanceMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SetNewDisplayMatrix("SESE");

            LoadData();
            SetChecked();
        }

        private void counterDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string prevMatrix = displayMatrix;
            //try
            {
                displayMatrix = "Counter";
                LoadData();
                SetChecked();
            }
            /*catch (Exception E)
            {
                MessageBox.Show("There was a problem displaying the network characteristics file file: " + E.Message, "Error!");
                displayMatrix = prevMatrix;
                LoadData();
                SetChecked();
            }*/
        }

        private void affiliationFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();

                if (displayMatrix == "Affiliation")
                    range.SetMode(true);

                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm progress = new ProgressForm();
                progress.endYear = endYear;
                progress.startYear = startYear;
                progress.curYear = 0;
                progress.Show();

                int previousYear = -1;
                for (int year = startYear; year <= endYear; ++year)
                {
                    if (loadFrom == "Matrix")
                    {
                        if (openFileDialog.Multiselect)
                            year = net.LoadFromMultipleFiles(fileNames, year);
                        else
                            year = net.LoadFromMatrixFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        if (openFileDialog.Multiselect)
                            year = net.LoadFromMultivariableDyadicFile(openFileDialog.FileName, year);
                        else
                            year = net.LoadFromDyadicFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Affil")
                    {
                        year = net.LoadFromAffiliationFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Random")
                    {
                        net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    }
                    else if (loadFrom == "ValuedRandom")
                    {
                        net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    }
                    // Should we standardize?
                    if (byRowToolStripMenuItem.Checked == true)
                        net.StandardizeByRow(displayMatrix);
                    else if (byColumnToolStripMenuItem.Checked == true)
                        net.StandardizeByColumn(displayMatrix);
                    else if (rowToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalRow(displayMatrix);
                    else if (columnToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalColumn(displayMatrix);
                    else if (minimumToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalMinimum(displayMatrix);
                    else if (maximumToolStripMenuItem.Checked == true)
                        net.StandardizeByDiagonalMaximum(displayMatrix);

                    if (year != previousYear && year <= endYear)
                    {
                        if (displayMatrix != "CONCOR" && displayMatrix != "Clustering")
                        {
                            net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                            net.SaveAffiliationToDyadicFile(saveFileDialog.FileName, year, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                        }
                        else if (displayMatrix == "CONCOR")
                        {
                            net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps); 
                            net.SaveBlocAffiliationToAffiliationFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect, false);
                        }
                        else
                        {
                            net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                            net.SaveBlocAffiliationToAffiliationFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect, true);
                        }
                    }
                    progress.curYear = year;
                    previousYear = year;
                }

                if (currentYear == endYear)
                    return;

                if (loadFrom == "Matrix")
                {
                    net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Affil")
                {
                    net.LoadFromAffiliationFile(openFileDialog.FileName, currentYear);
                }
            }
        }

        private void matrixMultiplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Multiplication");
            _multiplicationForm.ShowDialog();

            LoadData();
            SetChecked();
        }

        private void cONCORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blocForm.ShowDialog();
            SetNewDisplayMatrix("CONCOR");

            LoadData();
            SetChecked();

        }

        private void standardizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void interCliqueDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ICD");


            LoadData();
            SetChecked();
        }

        private void standardizedEuclideanDistanceMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            SetNewDisplayMatrix("SESE");
            LoadData();
            SetChecked();
        }

        private void elementwiseMultiplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void binaryComplementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BinaryComplement");

            LoadData();
            SetChecked();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            net = new Network.NetworkGUI();
            startYear = -1;
            currentYear = -1;
            loadFrom = "";
            displayMatrix = "Data";
            SetChecked();
            _centralityForm = new CentralityForm();
            _cliqueForm = new CliqueForm();
            _optionsForm.ReachNumMatrices = -1;
            _optionsForm.CMinMembers = 1;
            _optionsForm.Alpha = 0.0;
            _randomForm = new RandomForm();
            _vrandomForm = new ValuedRandomForm();
            _optionsForm = new OptionsForm();
            _randomForm.N = 3;
            _vrandomForm.N = 3;
            _blocForm = new BlocForm();

            _multiplicationForm = new MultiplicationForm();
            dataGrid.Columns.Clear();
             _nfsForm = new NetworkFormationSimulationForm(this);

       
            BufferedFileTable.Clear();
        }

        private void reset()
        {
            //net = new Network.NetworkGUI();
            startYear = -1;
            currentYear = -1;
            loadFrom = "";
            displayMatrix = "Data";
            SetChecked();
            _centralityForm = new CentralityForm();
            _cliqueForm = new CliqueForm();
            _optionsForm.ReachNumMatrices = -1;
            _optionsForm.CMinMembers = 1;
            _optionsForm.Alpha = 0.0;
            _randomForm = new RandomForm();
            _vrandomForm = new ValuedRandomForm();
            _optionsForm = new OptionsForm();
            _randomForm.N = 3;
            _vrandomForm.N = 3;
            _vrandomForm.vmin = 0;
            _vrandomForm.vmin = 100;
         
            _blocForm = new BlocForm();

            _multiplicationForm = new MultiplicationForm();
            dataGrid.Columns.Clear();

            BufferedFileTable.Clear();
        }



        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_optionsForm.net == null)
                _optionsForm.net = net;

            _optionsForm.ShowDialog();

            if (_optionsForm.Density == -1.0)
                net.LoadDensityVector(_optionsForm.densityFile);
            if (_optionsForm.ReachNumMatrices == -1)
                net.LoadReachNumVector(_optionsForm.reachFile);
            if (_optionsForm.ViableCoalitionCutoff == -1.0)
                net.LoadViableCutoffVector(_optionsForm.viableCoalitionFile);
            if (_optionsForm.CMinMembers == -1)
                net.LoadcliqueMinVector(_optionsForm.cMinMembersFile);
            if (_optionsForm.KCliqueValue == -1)
                net.LoadKCliqueVector(_optionsForm.KCliqueFileName);
            saveFileDialog.OverwritePrompt = _optionsForm.SaveOverwrite;

            _nfsForm.Overwrite = _optionsForm.SaveOverwrite;
            //_cliqueOptionForm.SumMeanFilename = _optionsForm.SumMeanFilename;
            //_cliqueOptionForm.SumMean = _optionsForm.SumMean;
            //_cliqueOptionForm.CETType = _optionsForm.CETType;
            //_cliqueOptionForm.CutoffValue = _optionsForm.CutoffValue;
            //_cliqueOptionForm.Cutoff = _optionsForm.Cutoff;
            //_cliqueOptionForm.CMinMembers = _optionsForm.CMinMembers;  

            cliqueSizeToolStripMenuItem.Enabled = _optionsForm.svcFile != null || _optionsForm.SumMeanFilename != null;
            cliqueCohesionToolStripMenuItem.Enabled = _optionsForm.InputType != "None";
        }

        private void triadicMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Triadic");
            LoadData();
            SetChecked();
        }

        private void roleEquivalenceMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("RoleEquiv");
            LoadData();
            SetChecked();
        }

       

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider.HelpNamespace);
        }

        private void matrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            matrixToolStripMenuItem1.Checked = true;
            dyadicToolStripMenuItem.Checked = false;
            monadicFileToolStripMenuItem.Checked = false;
            ef = Network.ElementwiseFormat.Matrix;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");

                LoadData();
                SetChecked();
            }
        }

        private void dyadicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matrixToolStripMenuItem1.Checked = false;
            dyadicToolStripMenuItem.Checked = true;
            monadicFileToolStripMenuItem.Checked = false;
            ef = Network.ElementwiseFormat.Dyadic;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");

                LoadData();
                SetChecked();
            }
        }

        private void monadicFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matrixToolStripMenuItem1.Checked = false;
            dyadicToolStripMenuItem.Checked = false;
            monadicFileToolStripMenuItem.Checked = true;
            ef = Network.ElementwiseFormat.Monadic;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");

                LoadData();
                SetChecked();
            }
        }

        private void cONCORBlockAffiliationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blocForm.ShowDialog();
            SetNewDisplayMatrix("CONCOR");
            LoadData();
            SetChecked();

        }

        private void sociomatrixEntiresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockPartitionS");
            LoadData();
            SetChecked();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockPartitionI");
            LoadData();
            SetChecked();
        }

        private void densityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DensityBlockMatrix");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("RelativeDensityBlockMatrix");
            LoadData();
            SetChecked();
        }

        private void blockCoheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockCohesivenessMatrix");
            LoadData();
            SetChecked();
        }

        private void blockCharacteristicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blockForm.ShowDialog();
            if (_blockForm.SVC.Count + _blockForm.DVC.Count >= 0)
            {
                SetNewDisplayMatrix("BlockCharacteristics");
                LoadData();
                SetChecked();
            }
        }

        private void hierarchicalClusteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _clusterForm.ShowDialog();
            SetNewDisplayMatrix("Clustering");
            LoadData();
            SetChecked();
        }

        private void clusterPartitionMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ClusterPartition");
            LoadData();
            SetChecked(); 
        }

        private void densityToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DensityClusterMatrix");
            LoadData();
            SetChecked(); 
        }

        private void relativeDensityToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("RelativeDensityClusterMatrix");
            LoadData();
            SetChecked(); 
        }

        private void cohesivenessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ClusterCohesivenessMatrix");
            LoadData();
            SetChecked();
        }

        private void clusterCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blockForm.ShowDialog();
            if (_blockForm.SVC.Count + _blockForm.DVC.Count >= 0)
            {
                SetNewDisplayMatrix("ClusterCharacteristics");
                LoadData();
                SetChecked();
            }
        } 

        private void protoCoalitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Affiliation");
            LoadData();
            SetChecked();
        }

        private void viableCoalitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ViableCoalitions");
            LoadData();
            SetChecked();
        }

        private void coalitionStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CoalitionStructure");
            LoadData();
            SetChecked();
        }

        private void rowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            rowToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void columnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            columnToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void minimumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            minimumToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void maximumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStandardizedChecks();
            maximumToolStripMenuItem.Checked = true;
            DoStandardize();

            LoadData();
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BufferedFileTable.RemoveFile(openFileDialog.FileName);
        }

        private void networkFormationSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void distanceMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void symmetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _randomForm.ShowDialog();
            loadFrom = "Random";
            _randomSymmetric = true;
            try
            {
                net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _randomForm.Year;
            _optionsForm.ReachNumMatrices = _randomForm.N - 1;
        }

        private void vsymmetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _vrandomForm.ShowDialog();
            loadFrom = "ValuedRandom";
            _randomSymmetric = true;
            try
            {
                net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _vrandomForm.Year;
            _optionsForm.ReachNumMatrices = _vrandomForm.N - 1;
        }

        private void bnonsymmetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _randomForm.ShowDialog();
            loadFrom = "Random";
            _randomSymmetric = false;
            try
            {
                net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _randomForm.Year;
            _optionsForm.ReachNumMatrices = _randomForm.N - 1;
        }

        private void vnonsymmetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            SetMode(false);
            _vrandomForm.ShowDialog();
            loadFrom = "ValuedRandom";
            _randomSymmetric = false;
            try
            {
                net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
            }
            catch (Exception E)
            {
                MessageBox.Show("There was an error loading the random matrix: " + E.Message, "Error!");
            }
            SetFormTitle();
            if (displayMatrix == "Affil")
                displayMatrix = "Data";
            LoadData();
            currentYear = _vrandomForm.Year;
            _optionsForm.ReachNumMatrices = _vrandomForm.N - 1;
        }

        private void multipleCliqueAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _MCA_form = new NetworkGUI.Forms.Multiple_Clique_Analysis(this);
            if (MCAcounteroptionlist != null)
            {
                _MCA_form.SetMCAcounter(MCAcounteroptionlist);
                _MCA_form.UpdateInterdependnceRadioButton(MCAcounteroptionlist[15]);
                _MCA_form.Setuseweightoption(MCAuseweight, MCAweightfilename);
            }
             _MCA_form.Show();
        }

        private void distanceMatrixToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Distance");
            LoadData();
            SetChecked();
        }

        private void cheapestCostMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            net.generateLowestCostMatrix();
            net.LoadMatrixIntoDataGridView(dataGrid, "Cheapest");
        }

        private void strengthMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //net.generateStrengthMatrix();
            net.LoadMatrixIntoDataGridView(dataGrid, "Strength");
        }

        private void nPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ViableCounter");
            LoadData();
            SetChecked();
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (loadFrom == "")
                return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int startYear, endYear;
                YearRangeForm range = new YearRangeForm();
                range.from = currentYear;
                range.to = currentYear;
                range.ShowDialog();
                startYear = range.from;
                endYear = range.to;

                ProgressForm progress = new ProgressForm();
                progress.endYear = endYear;
                progress.startYear = startYear;
                progress.curYear = 0;
                progress.Show();

                int previousYear = -1;
                for (int year = startYear; year <= endYear; ++year)
                {
                    if (loadFrom == "Matrix")
                    {
                        year = net.LoadFromMatrixFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        year = net.LoadFromDyadicFile(openFileDialog.FileName, year);
                    }
                    else if (loadFrom == "Random")
                    {
                        net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    }
                    else if (loadFrom == "ValuedRandom")
                    {
                        net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    }
                    if (year != previousYear && year <= endYear)
                    {
                        net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                        net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);

                        Network.NetworkGUI viableNet = new Network.NetworkGUI(net);
                        viableNet.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                            _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                            _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);

                        viableNet.SaveCounterToFile(saveFileDialog.FileName, year, year == startYear, ",", _optionsForm.Cutoff[year], _optionsForm.Density,
                            _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion, _optionsForm.transitivityType,
                            _optionsForm.counterOptions, _optionsForm.SaveOverwrite && year == startYear, _optionsForm.reachZero, _optionsForm.reachSum, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    }
                    progress.curYear = year;
                    Application.DoEvents();
                    previousYear = year;
                }

                if (currentYear == endYear)
                    return;

                if (loadFrom == "Matrix")
                {
                    net.LoadFromMatrixFile(openFileDialog.FileName, currentYear);
                }
                else if (loadFrom == "Dyadic")
                {
                    net.LoadFromDyadicFile(openFileDialog.FileName, currentYear);
                }
            }
        }

        private void networkPowerMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _npForm.ShowDialog();
            SetNewDisplayMatrix("NetworkPower");
            LoadData();
            SetChecked();
        }

        private void sESEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sESEToolStripMenuItem.Checked = true;
            sECToolStripMenuItem.Checked = false;

            cONCORBlockAffiliationToolStripMenuItem_Click(sender, e);
        }

        private void sECToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sECToolStripMenuItem.Checked = true;
            sESEToolStripMenuItem.Checked = false;

            cONCORBlockAffiliationToolStripMenuItem_Click(sender, e);
        }

        private NetworkIO.CONCORConvergenceFunction GetCONCORConvergenceFunction()
        {
            if (sECToolStripMenuItem.Checked)
                return new Network.Network.CONCORConvergenceFunction(MatrixComputations.StructuralEquivalenceCorrelation);
            else
                return new Network.Network.CONCORConvergenceFunction(MatrixComputations.StructuralEquivalenceStandardizedEuclidean);
        }

        private NetworkIO.CONCORConvergenceFunction GetCONCORConvergenceFunctionnp()
        {
            if (_npForm.RoleEqui)
                return new Network.Network.CONCORConvergenceFunction(MatrixComputations.RoleEquivalence);
            else
                return new Network.Network.CONCORConvergenceFunction(MatrixComputations.StructuralEquivalenceCorrelation);
        }

        private void realistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nfsForm.Type!=1)
                _nfsForm = new NetworkFormationSimulationForm(this);
            _nfsForm.Type = 1;
            _nfsForm.ShowDialog();
        }

        private void liberalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nfsForm.Type!=2)
                _nfsForm = new NetworkFormationSimulationForm(this);
            _nfsForm.Type = 2;
            _nfsForm.ShowDialog();
        }

        private void simplifiedRealistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nfsForm.Type!=3)
                _nfsForm = new NetworkFormationSimulationForm(this);
            _nfsForm.Type = 3;
            _nfsForm.ShowDialog();
        }

        private void simplifiedLiberalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nfsForm.Type != 4)
                _nfsForm = new NetworkFormationSimulationForm(this);
            _nfsForm.Type = 4;
            _nfsForm.ShowDialog();
        }

        private void diagonallyStandardizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapDiag");
            LoadData();
            SetChecked();
        }

        private void diagonallyStandardizedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CBCODiag");
            LoadData();
            SetChecked();
        }

        private void estebanRayIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            estebanRayIndexToolStripMenuItem.Checked = !estebanRayIndexToolStripMenuItem.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void cliqueCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cliqueCohesionToolStripMenuItem.Checked = !cliqueCohesionToolStripMenuItem.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void cliqueSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cliqueSizeToolStripMenuItem.Checked = !cliqueSizeToolStripMenuItem.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void unitBasedConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Data");
            LoadData();
            SetChecked();
        }

        private void eventBasedConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DataEvent");
            LoadData();
            SetChecked();
        }

        private void unitbasedConversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilCorrelation");
            LoadData();
            SetChecked();
        }

        private void eventbasedConversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilCorrelationEvent");
            LoadData();
            SetChecked();
        }

        private void unitbasedConversionToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilEuclidean");
            LoadData();
            SetChecked();
        }

        private void eventbasedConversionToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilEuclideanEvent");
            LoadData();
            SetChecked();
        }

        public void DealWithMultipleCliques(NetworkGUI.Forms.FileForCliqueAnalysis[] files, String cutOffFileName, double binary_cut_off, bool dyadic, int opt, string fileName, string weightfile, bool useweight)
        {
            reset();

            

            if (files[0] != null)
            {

                net.data = /*new List<Matrix>();*/ new List<global::Network.Matrices.Matrix>();
                net.mTable.Clear();
                List<clique> temp;
                List<List<clique>> cliqueList = new List<List<clique>>();
                string Null; //Does nothing


                if (dyadic == false)
                    for (int i = 0; i < files.Length; i++)
                    {
                        net.SmartLoad(files[i].fileName, out Null);
                        net.cet = files[i].option;
                        temp = clique.convertClique(net.FindCliques(files[i].cutOff, false, 0.0, 0xFFFF, 0, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag));
                        net.merge(cliques, temp);
                        if(useweight)
                            net.LoadweightVector(weightfile);
                        cliqueList.Add(temp);
                        net.data.Add(net.mTable["Data"]);
                    }
                else
                {
                    int variableCount = BufferedFileTable.GetFile(files[0].fileName).CountVarsInDyadicFile() - 1;
                    for (int curVar = variableCount; curVar >= 0; --curVar)
                    {
                        net.cet = files[0].option;
                        net.mTable["Data"] = MatrixReader.ReadMatrixFromFile(files[0].fileName, -1, curVar);
                        temp = clique.convertClique(net.FindCliques(files[0].cutOff, false, 0.0, 0xFFFF, 0, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag));
                        net.merge(cliques, temp);
                        if (useweight)
                            net.LoadweightVector(weightfile);
                        cliqueList.Add(temp);
                        net.data.Add(net.mTable["Data"]);
                    }
                }

                foreach (clique C in cliques)
                {
                    C.find_num_networks(cliqueList);
                }

                cliques.Sort(delegate(clique p1, clique p2) { return p1.CompareTo(p2); });

                Matrix jaffil = clique.GenerateAffiliationMatrixTemp(cliques);
                Matrix affil = clique.GenerateAffiliationMatrix(cliques);
                net.mTable.Add("Affiliation", affil);
                Matrix chara = clique.GenerateCharacteristicsMatrix(cliques);
                net.mTable.Add("chara", chara);
                //Matrix nc = clique.GenerateNCMatrix(net.data, cliques, _mcancForm.MCAcounterOptions);
                //net.mTable.Add("nc", nc);

                //no need
                //Matrix ovrlap = clique.GenerateOverLapTable(cliques);
                //net.mTable.Add("overlap", ovrlap);

                Matrix w_affil = clique.GenerateWeightedAffiliationMatrixTemp(cliques);
                net.mTable.Add("w_affil", w_affil);
                Matrix coc_mat = (jaffil.GetTranspose()) * jaffil;
                net.mTable.Add("coc_mat", coc_mat);
                Matrix cmo_mat = jaffil * (jaffil.GetTranspose());
                net.mTable.Add("cmo_mat", cmo_mat);
                //Matrix wcoc = clique.GenerateWeightedCOCMatrix(cliques, net.mTable["coc_mat"]);
                Matrix wcoc = (w_affil.GetTranspose()) * w_affil;
                net.mTable.Add("wcoc", wcoc);
                Matrix wcmo = w_affil * (w_affil.GetTranspose());
                net.mTable.Add("wcmo", wcmo);
            }



            #region comment
            /*
            if (opt == 0)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "wcmo");
                return;
            }

            if (opt == 1)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "wcoc");
                return;
            }

            if (opt == 2)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "overlap");
                return;
            }

            if (opt == 3)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "CBCO");
                return;
            }

            if (opt == 5)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "Affiliation");
                SetChecked();
                return;
            }
            if (opt == 4)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "chara");
            }

            if (opt == 6)
            {
                net.SaveAffiliationMatrixToFile(fileName, cliques);
            }

            if (opt == 7)
            {
                net.SaveToMultivariableDyadicFile(net.data.ToArray(), fileName);
            }
            */
            #endregion

            if (opt == 0)
                net.LoadMatrixIntoDataGridView(dataGrid, "wcmo");
            else if (opt == 1)
                net.LoadMatrixIntoDataGridView(dataGrid, "wcoc");
            else if (opt == 2)
                net.LoadMatrixIntoDataGridView(dataGrid, "cmo_mat"); //"overlap");
            else if (opt == 3)
                net.LoadMatrixIntoDataGridView(dataGrid, "coc_mat"); //"CBCO");
            else if (opt == 5)
            {
                net.LoadMatrixIntoDataGridView(dataGrid, "Affiliation");
                SetChecked();
            }
            else if (opt == 4)
                net.LoadMatrixIntoDataGridView(dataGrid, "chara");
            else if (opt == 6)
                net.SaveAffiliationMatrixToFile(fileName, cliques);
            else if (opt == 7)
                net.SaveToMultivariableDyadicFile(net.data.ToArray(), fileName);
            else if (opt == 8)
                net.LoadMCACounterIntoDataGridView(dataGrid, net.mTable, cliques, _MCA_form.GetMCAcounter, _MCA_form.ncForm.TT, net.data, useweight/*, net.mcayear, net.mcanetwork, net.mcaweight*/);
            return;
        }

        public void LoadMCAoptionlist(bool[] optionlist)
        {      
                MCAcounteroptionlist = optionlist;
        }

        public void LoadMCAuseweightoption(bool use, string filename)
        {
            MCAuseweight = use;
            MCAweightfilename = filename;
        }

        public bool GetSaveOverwrite()
        {
            return _optionsForm.SaveOverwrite;
        }

        private void specifyCliquesOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_cliqueOptionForm.net == null)
                _cliqueOptionForm.net = net; 

            _cliqueOptionForm.ShowDialog();

            if (_cliqueOptionForm.CMinMembers == -1)
                net.LoadcliqueMinVector(_cliqueOptionForm.CMinMembersFileName);
            if (_cliqueOptionForm.KCliqueValue == -1)
                net.LoadKCliqueVector(_cliqueOptionForm.KCliqueFileName);

            _optionsForm.SumMeanFilename = _cliqueOptionForm.SumMeanFilename;
            _optionsForm.SumMean = _cliqueOptionForm.SumMean;
            _optionsForm.CETType = _cliqueOptionForm.CETType;
            _optionsForm.CMinMembers = _cliqueOptionForm.CMinMembers;
            _optionsForm.CutoffValue = _cliqueOptionForm.CutoffValue;
            _optionsForm.Cutoff = _cliqueOptionForm.Cutoff;

        }

        private void binarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reachBinary = 1;
            SetNewDisplayMatrix("Reachability");
            LoadData();
            SetChecked();
        }

        private void valuedMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reachBinary = 0;
            SetNewDisplayMatrix("Reachability");
            LoadData();
            SetChecked();
        }

        private void communityAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CommunityAffliation");
            LoadData();
            SetChecked();
        }

        private void communityDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CommunityDensity");
            LoadData();
            SetChecked();
        }

        private void communityCharacteristicsMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CommunityCharacteristics");
            LoadData();
            SetChecked();
        } 

 
 
 



     

       
    }
}