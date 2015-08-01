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
        ABMForm _ABMForm = new ABMForm();
        ABMConfirmForm _ABMConfirm = new ABMConfirmForm();
        ValuedRandomForm _vrandomForm = new ValuedRandomForm();
        CentralityForm _centralityForm = new CentralityForm();
        CliqueForm _cliqueForm = new CliqueForm();
        CliqueForm _blockForm = new CliqueForm();
        CliqueForm _comForm = new CliqueForm();
        CliqueForm _overlapCommForm = new CliqueForm(); // new for overlap communities
        DichotomizeForm _dichotomizeForm = new DichotomizeForm(); // new for dichotomize
        RecodeForm _recodeForm = new RecodeForm(); // new for recode

        public OptionsForm _optionsForm = new OptionsForm();
        MultiplicationForm _multiplicationForm = new MultiplicationForm();
        BlocForm _blocForm = new BlocForm();
        BlocForm2 _blocForm2 = new BlocForm2();
        NetworkFormationSimulationForm _nfsForm;
        NetworkPowerForm _npForm = new NetworkPowerForm();
        CliqueOptionForm _cliqueOptionForm = new CliqueOptionForm();
        Multiple_Clique_Analysis _MCA_form;
        ClusteringForm _clusterForm = new ClusteringForm();

        NetworkSpilloverForm _spilloverForm = new NetworkSpilloverForm();

        bool _randomSymmetric = false;

        Network.NetworkGUI net = new Network.NetworkGUI();
        int startYear;
        int currentYear;
        bool reachBinary;
        bool diag;
        string loadFrom;
        string displayMatrix, prevDisplayMatrix;
        CommunityType communityType;


        bool[] MCAcounteroptionlist;
        bool MCAuseweight;
        string MCAweightfilename;

        List<clique> cliques = new List<clique>();

        string[] fileNames; // File names for multiple files 

        Network.ElementwiseFormat ef = Network.ElementwiseFormat.Matrix;

        private const string versionString = "3.76";

        //protected ScrollableControl scroll_ctl = new ScrollableControl();

        bool useMultipleFiles = false;

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

            _comForm.Text = "Community Characteristics";
            _comForm.ButtonText = "Generate Community Characteristics";

            _overlapCommForm.Text = "Overlap Community Characteristics";
            _overlapCommForm.ButtonText = "Generate Overlap Community Characteristics";

            _MCA_form = new NetworkGUI.Forms.Multiple_Clique_Analysis(this);
            _nfsForm = new NetworkFormationSimulationForm(this);

           // initialmcacounter();
        }


        private void DetermineCounter(int currentYear)
        {
            switch (_optionsForm.SelectionMethod)
            {
                case "Cliq":
                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                    break;
                case "Bloc":
                    _blocForm2 = _optionsForm._blocForm;

                    if (_optionsForm.counterOptions[2] || _optionsForm.counterOptions[7] || _optionsForm.counterOptions[6] || _optionsForm.counterOptions[5] || _optionsForm.counterOptions[4] || _optionsForm.counterOptions[3] ||
                        _optionsForm.counterOptions[8] || _optionsForm.counterOptions[9] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[10] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[15] ||
                        _optionsForm.counterOptions[17] || _optionsForm.counterOptions[16] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                    {
                        net.CONCOR(_blocForm2.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm2.MaxNoSteps);
                        net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                    }
                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _blocForm2.pos, _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, _blocForm2.MaxNoSteps);
                    break;
                case "Clus":
                    _clusterForm = _optionsForm._clusterForm;
                    if (_optionsForm.counterOptions[2] || _optionsForm.counterOptions[7] || _optionsForm.counterOptions[6] || _optionsForm.counterOptions[5] || _optionsForm.counterOptions[4] || _optionsForm.counterOptions[3] ||
                        _optionsForm.counterOptions[8] || _optionsForm.counterOptions[9] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[10] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[15] ||
                        _optionsForm.counterOptions[17] || _optionsForm.counterOptions[16] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                    {
                        net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                        net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                    }
                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                    break;
                case "NewDisc":
                    if (_optionsForm.counterOptions[2] || _optionsForm.counterOptions[7] || _optionsForm.counterOptions[6] || _optionsForm.counterOptions[5] || _optionsForm.counterOptions[4] || _optionsForm.counterOptions[3] ||
                        _optionsForm.counterOptions[8] || _optionsForm.counterOptions[9] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[10] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[15] ||
                        _optionsForm.counterOptions[17] || _optionsForm.counterOptions[16] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                    {
                        net.calculateCommunities(dataGrid, CommunityType.newAffil, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    }

                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                    break;
                case "NewOv":
                    if (_optionsForm.counterOptions[2] || _optionsForm.counterOptions[7] || _optionsForm.counterOptions[6] || _optionsForm.counterOptions[5] || _optionsForm.counterOptions[4] || _optionsForm.counterOptions[3] ||
                        _optionsForm.counterOptions[8] || _optionsForm.counterOptions[9] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[10] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[15] ||
                        _optionsForm.counterOptions[17] || _optionsForm.counterOptions[16] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                    {
                        net.calculateCommunities(dataGrid, CommunityType.ovAffil, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    }

                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                    break;
                case "Comm":
                    if (_optionsForm.counterOptions[2] || _optionsForm.counterOptions[7] || _optionsForm.counterOptions[6] || _optionsForm.counterOptions[5] || _optionsForm.counterOptions[4] || _optionsForm.counterOptions[3] ||
                        _optionsForm.counterOptions[8] || _optionsForm.counterOptions[9] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[10] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[15] ||
                        _optionsForm.counterOptions[17] || _optionsForm.counterOptions[16] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                    {
                        net.calculateCommunities(dataGrid, CommunityType.Affil, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    }

                    net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                    _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                    _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                    _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                    _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                    break;
                default:
                    break;
            }
        }



        private void LoadData()
        {
            net.ClearPreviousData(displayMatrix, loadFrom);
            // Load matrix into GUI
            DoStandardize();
            // Thread t = new Thread(delegate() { DoLoadCorrect(currentYear);  });
            // t.Start();
            //while (!t.IsAlive) ;
            // Thread.Sleep(100);
            // t.Join();
            DoLoadCorrect(currentYear);
            switch (displayMatrix)
            {
                case "Affiliation":
                    if (_optionsForm.DisplayCliqueOption == "Save")
                        SaveAffiliationWithoutDisplay();
                    else
                        net.LoadAffiliationIntoDataGridView(dataGrid, _optionsForm.Cutoff[currentYear], _optionsForm.InputType == "StructEquiv",
                                                            _optionsForm.FileName, _optionsForm.InputType == "Dyadic", currentYear, _optionsForm.Density, _optionsForm.InputType != "None",
                                                            _optionsForm.SumMean, _optionsForm.SumMeanFilename, _optionsForm.svcFile, _optionsForm.DisplayCliqueOption,
                                                            cliqueSizeToolStripMenuItem1.Checked, cliqueCohesionToolStripMenuItem1.Checked, estebanRayIndexToolStripMenuItem1.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "ViableCounter":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadViableCoalitions(_optionsForm.ViableCoalitionCutoff, currentYear, _optionsForm.svcCoalitionFile);

                    Network.NetworkGUI viableNet = new Network.NetworkGUI(net);
                    viableNet.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                        _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                        _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod,net._minCliqueSize);
                    break;
                case "CONCOR":
                    net.LoadBlocAffiliationIntoDataGridView(dataGrid, false);
                    break;
                case "Clustering":
                    net.LoadBlocAffiliationIntoDataGridView(dataGrid, true);
                    break;
                case "NatDep":
                    net.LoadNationalDependencyIntoDataGridView(_optionsForm.ReachNumMatrices, _optionsForm.Density, 
                                                                currentYear, _optionsForm.reachZero, _optionsForm.reachSum);
                    net.LoadMatrixIntoDataGridView(dataGrid, displayMatrix);
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
                                                        cliqueSizeToolStripMenuItem1.Checked, cliqueCohesionToolStripMenuItem1.Checked, estebanRayIndexToolStripMenuItem1.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "Counter":
                    break;
                case "Community":
                    break;
                // new code for local Transitivity
                case "LocalTransitivity":
                    break;
                case "LocalBalance":
                    break;

                case "SignedNetwork":
                    //net.LoadSignedNetworkCharacteristics(dataGrid, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, currentYear, reachBinary);
                    break;

                case "SingleNetworkExpectations":
                    break;
                case "NetworkSpilloverStatistics":
                    break;

                

                default:
                    net.LoadMatrixIntoDataGridView(dataGrid, displayMatrix);
                    break;
            }
            //net.ClearPreviousData(displayMatrix);

        }

        private void DoLoadCorrect(int currentYear)
        {
            switch (displayMatrix)
            {
                case "ABMModel":
                    net.LoadMatrixIntoDataGridView(dataGrid, "Data");
                    break;
                case "Counter":
                    DetermineCounter(currentYear);
                    break;
                case "Affiliation":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "Cheapest":
                    net.generateLowestCostMatrix();
                    net.LoadMatrixIntoDataGridView(dataGrid, "Cheapest");
                    break;
                case "Strength":
                    net.generateStrengthMatrix();
                    net.LoadMatrixIntoDataGridView(dataGrid, "Strength");
                    break;
                case "CliqueDensity":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadCliqueDensity();
                    break;
                case "CliqueRelativeDensity":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadCliqueRelativeDensity(_optionsForm.Density);
                    break;
                case "Overlap":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "OverlapDiag":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "CBCO":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, true, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "CBCODiag":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, true, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    break;
                case "Community":
                    net.calculateCommunities(dataGrid, communityType, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    break;
                case "Dependency":
                    net.LoadDependency(prevDisplayMatrix, _optionsForm.ReachNumMatrices, _optionsForm.Density, currentYear, _optionsForm.reachZero, _optionsForm.reachSum);
                    break;
                case "Reachability": 
                    net.LoadReachability(_optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, currentYear, reachBinary);
                    break;
                case "CognitiveReachability":
                    net.LoadCognitiveReachability(_optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, currentYear, reachBinary);
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
                    net.LoadCliqueCharacteristics(_cliqueForm.SVC, _cliqueForm.DVC, _cliqueForm.attrMatrix, currentYear, _optionsForm.CutoffValue);
                    break;
                case "NatDep":
                    net.LoadDependency(prevDisplayMatrix, _optionsForm.ReachNumMatrices, _optionsForm.Density, currentYear, _optionsForm.reachZero, _optionsForm.reachSum);
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
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    break;
                case "ICD":
                    net.LoadIntercliqueDistance(_optionsForm.Density, currentYear);
                    break;
                case "Elementwise":
                    net.LoadElementwiseMultiplication(openFileDialog2.FileName, currentYear, "Elementwise", ef);
                    break;
                case "BinaryComplement":
                    net.LoadBinaryComplement(displayMatrix);
                    break;
                case "Triadic":
                    net.LoadTriadic("Data", currentYear);
                    break;
                case "RoleEquiv":
                    if (!openFileDialog.Multiselect)
                        net.LoadRoleEquivalence(displayMatrix);
                    break;
                case "BlockPartitionI":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockPartitionMatrix();
                    break;
                case "BlockPartitionS":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps); ;
                    net.LoadBlockPartitionMatrix();
                    break;
                case "DensityBlockMatrix":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockDensity(displayMatrix);
                    break;
                case "RelativeDensityBlockMatrix":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadRelativeBlockDensity(_optionsForm.Density, displayMatrix);
                    break;
                case "BlockCohesionMatrix":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockCohesionMatrix(_blocForm2.pos, _optionsForm.Density, currentYear, displayMatrix);
                    break;
                
                case "BlockCharacteristics":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);

                    //
                    // may not need 
                    //
                    net.LoadBlockMatrices(_optionsForm.Density, currentYear);

                    net.LoadBlockCharacteristics(_blockForm.SVC, _blockForm.DVC, _blockForm.attrMatrix, currentYear, false);
                    break;
                case "ClusterPartition":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadClusterPartitionMatrix();
                    break;
                case "DensityClusterMatrix":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadBlockDensity(displayMatrix);
                    break;
                case "RelativeDensityClusterMatrix":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadRelativeBlockDensity(_optionsForm.Density, displayMatrix);
                    break;
                case "ClusterCohesivenessMatrix":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadBlockCohesionMatrix(_blocForm2.pos, _optionsForm.Density, currentYear, displayMatrix);
                    break;
                case "ClusterCharacteristics":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadBlockCharacteristics(_blockForm.SVC, _blockForm.DVC, _blockForm.attrMatrix, currentYear, true);
                    break;
                case "CommunityCharacteristics":
                    net.LoadBlockCharacteristics(_comForm.SVC, _comForm.DVC, _comForm.attrMatrix, currentYear, false);
                    break;
                case "ViableCoalitions":
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
                    net.LoadComponents(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero,true);
                    break;
                case "NetworkPower":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType == "None" || _optionsForm.InputType == "StructEquiv", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    if(!_npForm.CliquePower && !_npForm.CommPower)
                        net.CONCOR(_npForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunctionnp(), _npForm.MaxNoSteps);
                    else if (_npForm.CommPower)
                        net.calculateCommunities(dataGrid, communityType, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    if(_npForm.useattributefile)
                        net.LoadattributeVector(_npForm.attributefilename);
                    net.LoadNetworkpower(_npForm.useattributefile, currentYear, _npForm.CliquePower, _npForm.CommPower, _npForm.showSP, _optionsForm.InputType, _optionsForm.FileName, _optionsForm.Density);
                    break;
                case "Clustering":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    break;


                case "LocalTransitivity":
                    net.LoadLocalTransitivityMatrix(dataGrid, currentYear, _optionsForm.CutoffValue);
                    break;
                case "DyadicTransitivity":
                    net.LoadDyadicTransitiviyMatrix(dataGrid, currentYear, _optionsForm.CutoffValue);
                    break;
                case "LocalBalance":
                    net.LoadLocalBalanceMatrix(dataGrid, currentYear);
                    break;
                case "SignedNetwork":
                    net.LoadSignedNetworkCharacteristics(dataGrid, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, currentYear, reachBinary);
                    break;

                // Deprecated code
                case "CliqueAffiliation":
                    net.LoadCliqueAffiliationMatrix(prevDisplayMatrix);
                    break;
                case "JointCliqueAffiliation":
                    net.LoadJointCliqueAffiliationMatrix(prevDisplayMatrix);
                    break;
                case "CliqueMembershipOverlap":
                    net.LoadCliqueMembershipOverlapMatrix(prevDisplayMatrix);
                    break;
                case "CliqueByCliqueOverlap":
                    net.LoadCliqueByCliqueOverlapMatrix(prevDisplayMatrix);
                    break;

                case "NewOverlappingCommunity":
                    net.calculateCommunities(dataGrid, communityType, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                    break;

                    
                // For Overlapping Communities
                case "OverlappingCommunity":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CalculateOverlapComm();
                    net.LoadOverlapCommAffilMatrix();
                    break;
                case "OverlapCommDensity":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CalculateOverlapComm();
                    net.LoadOverlapCommDensity();
                    break;
                case "OverlapCommRelativeDensity":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CalculateOverlapComm();
                    net.LoadOverlapCommRelativeDensity(_optionsForm.Density);
                    break;
                case "OverlapCommCohesiveMatrix":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CalculateOverlapComm();
                    net.LoadOverlapCommCohesiveMatrix(_optionsForm.Density, currentYear);
                    break;
                case "OverlapCommCharacteristics":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.CalculateOverlapComm();
                    net.LoadOverlapCommCharacteristics(_overlapCommForm.SVC, _overlapCommForm.DVC, _overlapCommForm.attrMatrix, currentYear, _optionsForm.getCutOff(currentYear));
                    break;
                case "OverlapCommModularityEQ":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadOverlapModifiedModularity();
                    break;


                // For Cohesion (only cliques)
                case "CliqueCohesionMatrix":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadCliqueCohesionMatrix(_optionsForm.Density, currentYear);
                    break;

                // For statistics
                case "CliqueCoefficients":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadCliqueCoefficients(_optionsForm.Density, currentYear, displayMatrix);
                    break;
                case "BlockCoefficients":
                    net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                    net.LoadBlockCoefficients(_blocForm2.pos, _optionsForm.Density, currentYear, displayMatrix);
                    break;
                case "ClusterCoefficients":
                    net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                    net.LoadBlockCoefficients(_blocForm2.pos, _optionsForm.Density, currentYear, displayMatrix);
                    break;
                case "OverlapCommCoefficients":
                    net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    net.LoadOverlapCommCoefficients(_optionsForm.Density, currentYear, displayMatrix);
                    break;
                
                // For Single Network Expectations
                case "SingleNetworkExpectations":
                    net.LoadSingleNetworkExpectations(dataGrid, currentYear);
                    break;
                case "NetworkSpilloverStatistics":
                    net.LoadNetworkSpilloverStatistics(dataGrid, currentYear, _spilloverForm.Indices);
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
            if (loadFrom == "ABMModel")
            {
                this.Text = "Matrix Manipulator v" + versionString + " - Agent-Based Shocks Model Simulation";
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
            bool b = correlationToolStripMenuItem.Checked;

            bool useCS = cliqueSizeToolStripMenuItem1.Checked;
            bool useCC = cliqueCohesionToolStripMenuItem1.Checked;
            bool estRay = estebanRayIndexToolStripMenuItem1.Checked;

            UncheckAllItems(menuStrip);

            // moved items
            cliqueSizeToolStripMenuItem1.Checked = useCS;
            cliqueCohesionToolStripMenuItem1.Checked = useCC;
            estebanRayIndexToolStripMenuItem1.Checked = estRay;


            correlationToolStripMenuItem.Checked = b;
            stdEuclideanDistanceToolStripMenuItem.Checked = !b;

            if (displayMatrix != "Elementwise")
            {
                matrixToolStripMenuItem.Checked = false;
                dyadicFileToolStripMenuItem.Checked = false;
                monadicFileToolStripMenuItem1.Checked = false;
            }

            if (loadFrom == "Affil" && displayMatrix == "Data")
                unitBasedConversionToolStripMenuItem3.Checked = true;
            else
            {
                switch (displayMatrix)
                {
                    case "Data": 
                        dataMatrixToolStripMenuItem.Checked = true;
                        dataMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "Affiliation": 
                        cliqueAffiliationMatrixToolStripMenuItem1.Checked = true; // new
                        break;
                    case "CliqueDensity":
                        cliqueDensityMatrixToolStripMenuItem1.Checked = true; // new
                        break;
                    case "CliqueRelativeDensity":
                        relativeDensityToolStripMenuItem.Checked = true; // new
                        break;
                    case "Overlap":
                        cliqueMembershipOverlapMatrixToolStripMenuItem1.Checked = true; // new
                        break;
                    case "OverlapDiag":
                        diagonallyStandardizedToolStripMenuItem2.Checked = true; // new
                        break;
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
                    case "CBCO":
                        cliqueOverlapMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "CBCODiag":
                        diagonallyStandardizedToolStripMenuItem3.Checked = true;
                        break;
                    case "Dependency": dependencyMatrixToolStripMenuItem.Checked = true; break;
                    case "Reachability": reachabilityMatrixToolStripMenuItem.Checked = true; break;
                    case "CognitiveReachability": reachabilityMatrixToolStripMenuItem1.Checked = true; break;
                    case "Components": componentsMatrixToolStripMenuItem.Checked = true; break;
                    case "Centrality": centralityIndicesMatrixToolStripMenuItem.Checked = true; break;
                    case "Characteristics":
                        cliqueCharacteristicsMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "EventOverlap": eventOverlapMatrixToolStripMenuItem.Checked = true; break;
                    case "NatDep": nationalDependencyMatrixToolStripMenuItem.Checked = true; break;
                    case "Counter": counterDataToolStripMenuItem.Checked = true; break;
                    case "Multiplication": 
                        matrixMultiplicationToolStripMenuItem1.Checked = true; break;
                    case "CONCOR":
                        cONCORBlockAffiliationToolStripMenuItem1.Checked = true;
                        break;
                    case "ICD":
                        interCliqueDistanceToolStripMenuItem1.Checked = true;
                        break;
                    case "Elementwise": elementwiseMultiplicationToolStripMenuItem1.Checked = true; break;
                    case "BinaryComplement": binaryComplementToolStripMenuItem.Checked = true; break;
                    case "Triadic": triadicMatrixToolStripMenuItem.Checked = true; break;
                    case "RoleEquiv": roleEquivalenceMatrixToolStripMenuItem.Checked = true; break;
                    case "DataEvent": eventBasedConversionToolStripMenuItem3.Checked = true; break;
                    case "AffilEuclidean": unitBasedConversionToolStripMenuItem5.Checked = true; break;
                    case "AffilCorrelation": unitBasedConversionToolStripMenuItem4.Checked = true; break;
                    case "AffilCorrelationEvent": eventBasedConversionToolStripMenuItem4.Checked = true; break;
                    case "AffilEuclideanEvent": eventBasedConversionToolStripMenuItem5.Checked = true; break;
                    case "BlockPartitionS":
                        sociomatrixEntriesToolStripMenuItem.Checked = true;
                        break;
                    case "BlockPartitionI":
                        blockIdentityEntriesToolStripMenuItem.Checked = true;
                        break;
                    case "DensityBlockMatrix":
                        blockDensityMatrixToolStripMenuItem.Checked = true;
                        break;
                    case "RelativeDensityBlockMatrix":
                        relativeDensityToolStripMenuItem2.Checked = true;
                        break;
                    // May not need                    
                    case "BlockCohesionMatrix":
                        blockCohesionToolStripMenuItem.Checked = true;
                        break;

                    case "ClusterPartition":
                        clusterPartitionMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "DensityClusterMatrix":
                        clusterDensityMatrixToolStripMenuItem.Checked = true;
                        break;
                    case "RelativeDensityClusterMatrix":
                        relativeDensityToolStripMenuItem3.Checked = true;
                        break;
                    case "ClusterCohesivenessMatrix":
                        clusterCohesionToolStripMenuItem.Checked = true;
                        break;
                    case "BlockCharacteristics":
                        blockCharacteristicsToolStripMenuItem.Checked = true;
                        break;
                    case "ClusterCharacteristics": clusterCharacteristicsToolStripMenuItem1.Checked = true;
                        break;
                    case "Community":
                        if (communityType == CommunityType.Affil) communityAffiliationMatrixToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.Density) communityDensityMatrixToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.RelativeDensity) relativeDensityToolStripMenuItem4.Checked = true;
                        else if (communityType == CommunityType.Cohesion) communityCohesionToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.Char) communityCharacteristicsToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.Cluster) modularityCoefficientToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.Coefficients) communityCoefficientsToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.newAffil) communityAffiliationMatrixToolStripMenuItem1.Checked = true;
                        else if (communityType == CommunityType.newDensity) communityDensityMatrixToolStripMenuItem1.Checked = true;
                        else if (communityType == CommunityType.newRelativeDensity) relativeDensityToolStripMenuItem1.Checked = true;
                        else if (communityType == CommunityType.newCohesion) communityCohesionMatrixToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.newChar) communityCharacteristicsToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.Separation) separationCoefficientToolStripMenuItem.Checked = true;
                        else if (communityType == CommunityType.newCoefficients) communityCoefficientsToolStripMenuItem1.Checked = true;
                        else if (communityType == CommunityType.ovAffil) overlappingCommunityAffiliationMatrixToolStripMenuItem1.Checked = true;
                        break;

                    case "Distance":
                        distanceMatrixToolStripMenuItem.Checked = true;
                        distanceMatrixToolStripMenuItem1.Checked = true;
                        break;
                    case "Cheapest":
                        distanceMatrixToolStripMenuItem.Checked = true;
                        cheapestCostMatrixToolStripMenuItem.Checked = true;
                        break;
                    case "Strength":
                        distanceMatrixToolStripMenuItem.Checked = true;
                        strengthMatrixToolStripMenuItem.Checked = true;
                        break;

                    case "NetworkPower": networkPowerMatrixToolStripMenuItem.Checked = true; break;
                    case "Clustering": clusterAffiliationMatrixToolStripMenuItem.Checked = true; break;

                    case "LocalTransitivity": localTransitivityToolStripMenuItem.Checked = true; break;
                    case "DyadicTransitivity": dyadicTransitivityToolStripMenuItem.Checked = true; break;
                    case "LocalBalance": localBalanceMatrixToolStripMenuItem.Checked = true; break;

                    // Deprecated
                    case "JointCliqueAffiliation": jointCliqueAffiliationMatrixToolStripMenuItem1.Checked = true; break;
                    case "CliqueMembershipOverlap": cliqueMembershipOverlapMatrixToolStripMenuItem2.Checked = true; break;
                    case "CliqueByCliqueOverlap": jointCliqueOverlapMatrixToolStripMenuItem.Checked = true; break;


                    case "SignedNetwork": signedNetworkCharacteristicsToolStripMenuItem.Checked = true; break;
                    case "OverlappingCommunity": overlappingCommunityAffiliationMatrixToolStripMenuItem.Checked = true; break;
                    case "OverlapCommDensity": overlappingCommunityDensityMatrixToolStripMenuItem.Checked = true; break;
                    case "OverlapCommRelativeDensity": relativeDensityToolStripMenuItem5.Checked = true; break;
                    case "OverlapCommCohesiveMatrix": overlappingCommunityCohesionToolStripMenuItem.Checked = true; break;
                    case "OverlapCommCharacteristics": overlappingCommunityCharacteristicsToolStripMenuItem.Checked = true; break;
                    case "OverlapCommModularityEQ": overlappingCommunityToolStripMenuItem.Checked = true; break;
                    case "CliqueCohesionMatrix": cliqueCohesionToolStripMenuItem.Checked = true; break;
                    case "CliqueCoefficients": cliqueCoefficientsToolStripMenuItem.Checked = true; break;
                    case "BlockCoefficients": blockCoefficientsToolStripMenuItem.Checked = true; break;
                    case "ClusterCoefficients": clusterCoefficientsToolStripMenuItem.Checked = true; break;
                    case "OverlapCommCoefficients": overlappingCommunityCoefficientsToolStripMenuItem.Checked = true; break;
                    case "SingleNetworkExpectations": singleNetworkToolStripMenuItem.Checked = true; break;
                    case "NetworkSpilloverStatistics": networkSpilloverToolStripMenuItem.Checked = true; break;
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
            try
            {
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
                else if (loadFrom == "ABMModel")
                {
                    net.mTable["Data"] = net.mList[currentYear - _ABMForm.netID];
                }
            }
            catch (Exception E)
            {
                --currentYear;
                MessageBox.Show("Unable to advance to next year: " + E.Message, "Error!");
                return;
            }
            if (net.CohesionFilename != null)
                net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, currentYear);

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
                else if (loadFrom == "ABMModel")
                {
                    net.mTable["Data"] = net.mList[currentYear - _ABMForm.netID];
                }
            }
            catch (Exception E)
            {
                ++currentYear;
                MessageBox.Show("Unable to advance to previous year: " + E.Message, "Error!");
                return;
            }

            if (net.CohesionFilename != null)
                net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, currentYear);

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
                    else if (loadFrom == "ABMModel")
                    {
                        newYear = jump.year;
                        net.mTable["Data"] = net.mList[jump.year - _ABMForm.netID];
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
                if (net.CohesionFilename != null)
                    net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, currentYear);
                
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
                else if (loadFrom == "ABMModel")
                {
                    currentYear = _ABMForm.netID;
                    net.mTable["Data"] = net.mList[0];
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
            else if (loadFrom == "ABMModel")
            {
                currentYear = _ABMForm.netID + _ABMForm.networks - 1;
                net.mTable["Data"] = net.mList[currentYear - _ABMForm.netID];
            }

            if (net.CohesionFilename != null)
                net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, currentYear);
            
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
            else if (loadFrom == "ABMModel")
            {
                currentYear = _ABMForm.netID;
                net.mTable["Data"] = net.mList[0];
            }

            if (net.CohesionFilename != null)
                net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, currentYear);

            DoStandardize();
            LoadData();
            SetFormTitle();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void dataMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            EnableStandardizedChecks();
            if (loadFrom != "Affil")
                displayMatrix = "Data";
            else
                displayMatrix = "Affil";
            net.setDictomoization(false, 0.0);
            net.setRecode(false, null);
            LoadData();
            SetChecked();
        }

        private void SetNewDisplayMatrix(string s)
        {
            prevDisplayMatrix = displayMatrix;
            displayMatrix = s;
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
                                                        cliqueSizeToolStripMenuItem1.Checked, cliqueCohesionToolStripMenuItem1.Checked, estebanRayIndexToolStripMenuItem1.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                    else if (displayMatrix == "Affil" && loadFrom == "Affil")
                        net.SaveAffiliationMatrixToMatrixFile(saveFileDialog.FileName, year, _optionsForm.SaveOverwrite && year == startYear);
                    else if (displayMatrix == "NatDep")
                        net.SaveNationalDependencyToMatrixFile(saveFileDialog.FileName, year, _optionsForm.SaveOverwrite && year == startYear);
                    else if (displayMatrix == "CONCOR")
                        net.SaveBlocAffiliationToMatrixFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect);
                    else if (displayMatrix == "Clustering")
                        net.SaveBlocAffiliationToMatrixFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect);
                    else if (displayMatrix == "CBCO" || displayMatrix == "CBCODiag")
                        net.SaveCBCOverlapToFile(saveFileDialog.FileName, year, displayMatrix != "Characteristics",
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear, diag);
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

                        if (net.CohesionFilename != null)
                            net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, year);
                        //DoLoadCorrect(year);
                        currentYear = year;
                        LoadData();
                    }
                    else
                        break;
                }

                // display the last matrix in program
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
                                                         cliqueSizeToolStripMenuItem1.Checked, cliqueCohesionToolStripMenuItem1.Checked, estebanRayIndexToolStripMenuItem1.Checked, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
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
                            displayMatrix != "Characteristics" || year == startYear, _optionsForm.SaveOverwrite && year == startYear, diag);
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
                        DoLoadCorrect(year);
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
            string type;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetMode(false);
                    //                    Matrix m = MatrixReader.ReadMatrixFromFile(openFileDialog.FileName);
                    startYear = currentYear = net.SmartLoad(openFileDialog.FileName, out loadFrom);//type);
                    //loadFrom = "Dyadic";
                    //loadFrom = type;
                    
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
                        if (net.CohesionFilename != null)
                            net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, year);
                        //DoLoadCorrect(year);
                        currentYear = year;
                        LoadData();
                        
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
                for (int year = startYear; year <= endYear; ++year) // used to be year = startYear
                {
                    

                    if (year != previousYear && year <= endYear)
                    {
                        DetermineCounter(year);
                        net.SaveCounterToFile(saveFileDialog.FileName, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                    }

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
            useMultipleFiles = true;
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
                    useMultipleFiles = true;
                    currentYear = net.LoadFromMultivariableDyadicFile(fileNames[0], -1);
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
                affiliationToSociomatrixConversionToolStripMenuItem.Enabled = eventOverlapMatrixToolStripMenuItem.Enabled = true;
            else
                affiliationToSociomatrixConversionToolStripMenuItem.Enabled = eventOverlapMatrixToolStripMenuItem.Enabled = false;
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

        private void reachabilityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Reachability");
            LoadData();
            SetChecked();
        }
     
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


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Text, "About");
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Matrix Manipulator v" + versionString, "About");
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
                    {
                        net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                        net.SaveAffiliationToDyadicFile(saveFileDialog.FileName, year, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                    }
                    else if (displayMatrix == "CONCOR")
                    {
                        net.CONCOR(_blocForm.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm.MaxNoSteps);
                        net.SaveBlocAffiliationToAffiliationFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect, false);
                    }
                    else if (displayMatrix == "Cluster")
                    {
                        net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                        net.SaveBlocAffiliationToAffiliationFile(saveFileDialog.FileName, year, _blocForm.pos, _optionsForm.SaveOverwrite && year == startYear, openFileDialog.Multiselect, true);
                    }
                    else if (displayMatrix == "Community")
                    {
                        net.calculateCommunities(dataGrid, communityType, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                        net.SaveCommAffiliationToAffiliationFile(saveFileDialog.FileName, year, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                    }
                    else if (displayMatrix == "OverlappingCommunity")
                    {
                        net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType != "None", _optionsForm.Density, currentYear, _optionsForm.CMinMembers, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag);
                        net.CalculateOverlapComm();
                        net.LoadOverlapCommAffilMatrix();
                        net.SaveOverlapCommAffiliationToAffiliationFile(saveFileDialog.FileName, year, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
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

                        //DoLoadCorrect(year);
                    }
                    else
                        break;
                }
            }
        }

        private void cONCORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blocForm.ShowDialog();
            SetNewDisplayMatrix("CONCOR");
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

            useMultipleFiles = false;
            multipleMatrixFilesToolStripMenuItem.Enabled = true;
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

        private void dichotomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dichotomizeForm.ShowDialog() == DialogResult.OK)
                {
                    if (_dichotomizeForm.CutoffText == "")
                        throw new FormException("No Cutoff value has been defined");

                    if (_dichotomizeForm.CutoffValue != -1.0)
                        net.setDictomoization(true, _dichotomizeForm.CutoffValue);
                    else if (_dichotomizeForm.CutoffValue == -1.0)
                        net.setDictomoization(true, _dichotomizeForm.Cutoff[currentYear]);
                    
                    _dichotomizeForm.CutoffText = "";
                    SetNewDisplayMatrix("Data");
                    LoadData();
                    SetChecked();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        private void recodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = _recodeForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    if (_recodeForm.isValid())
                        net.setRecode(true, _recodeForm.ValidRows);
                    else
                    {
                        //net.setRecode(false, null);
                        throw new Exception("Invalid format for recoding");
                    }
                    _recodeForm.ClearTextBoxes();
                    SetNewDisplayMatrix("Data");
                    LoadData();
                    SetChecked();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
            else if (result == DialogResult.Cancel)
            {
                // do nothing for now
            }

            
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_optionsForm.net == null)
            {
                _optionsForm.net = net;
                _optionsForm.year = currentYear;
            }

            _optionsForm.ShowDialog();

            if (_optionsForm.Density == -1.0)
                net.LoadDensityVector(_optionsForm.densityFile);
            else
            {
                net.optionsDensity = _optionsForm.Density;
            }
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

            cliqueSizeToolStripMenuItem1.Enabled = _optionsForm.svcFile != null || _optionsForm.SumMeanFilename != null;
            cliqueCohesionToolStripMenuItem1.Enabled = _optionsForm.InputType != "None";
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
            SetNewDisplayMatrix("Cheapest");
            LoadData();
            SetChecked();
            //net.generateLowestCostMatrix();
            //net.LoadMatrixIntoDataGridView(dataGrid, "Cheapest");
        }

        private void strengthMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Strength");
            LoadData();
            SetChecked();
            /*net.generateStrengthMatrix();
            net.LoadMatrixIntoDataGridView(dataGrid, "Strength");*/
        }

        private void nPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ViableCounter");
            LoadData();
            SetChecked();
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
                            _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero, _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);

                        switch (_optionsForm.SelectionMethod)
                        {
                            case "Cliq":
                                bool temp = false;
                                if (_optionsForm.counterOptions[10] || _optionsForm.counterOptions[11] || _optionsForm.counterOptions[14] || _optionsForm.counterOptions[18] || _optionsForm.counterOptions[24])
                                    temp = true;
                                net.FindCliques(_optionsForm.Cutoff[currentYear], _optionsForm.InputType == "StructEquiv",
                                    _optionsForm.Density, currentYear, net._minCliqueSize, temp, _optionsForm.KCliqueValue,
                                    _optionsForm.KCliqueDiag);
                                net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                                _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                                _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                                _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                                _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                                break;
                            case "Bloc":
                                _blocForm2.ShowDialog();
                                net.CONCOR(_blocForm2.pos, openFileDialog.Multiselect, GetCONCORConvergenceFunction(), _blocForm2.MaxNoSteps);
                                net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                                net.LoadCounterIntoDataGridView(dataGrid, currentYear, _blocForm2.pos, _optionsForm.Density,
                                                                _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                                _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                                _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                                _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, _blocForm2.MaxNoSteps);
                                break;
                            case "Clus":
                                _clusterForm.ShowDialog();
                                net.LoadClustering(_clusterForm.ClusteringMethod, _clusterForm.MaxNoClusters, currentYear, _optionsForm.Density);
                                net.LoadBlockMatrices(_optionsForm.Density, currentYear);
                                net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                                 _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                                 _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                                 _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                                 _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                                break;
                            case "Comm":
                                net.calculateCommunities(dataGrid, CommunityType.Affil, currentYear, _comForm.SVC, _comForm.DVC, _comForm.attrMatrix, _optionsForm.getCutOff(currentYear), _optionsForm.Density);
                                net.LoadCounterIntoDataGridView(dataGrid, currentYear, _optionsForm.Cutoff[currentYear], _optionsForm.Density,
                                                                _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion,
                                                                _optionsForm.transitivityType, _optionsForm.counterOptions, _optionsForm.reachSum, _optionsForm.reachZero,
                                                                _optionsForm.ReachNumMatrices, _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue,
                                                                _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod, net._minCliqueSize);
                                break;
                            default:
                                break;
                        }
                        viableNet.SaveCounterToFile(saveFileDialog.FileName, year, year == startYear, ",", _optionsForm.Cutoff[year], _optionsForm.Density,
                            _optionsForm.InputType, _optionsForm.FileName, _optionsForm.svcFile, _optionsForm.useCohesion, _optionsForm.transitivityType,
                            _optionsForm.counterOptions, _optionsForm.SaveOverwrite && year == startYear, _optionsForm.reachZero, _optionsForm.reachSum, _optionsForm.ReachNumMatrices,
                            _optionsForm.ERPOLType, _optionsForm.Alpha, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag, _optionsForm.SelectionMethod);
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


        private NetworkIO.CONCORConvergenceFunction GetCONCORConvergenceFunction()
        {
            if (correlationToolStripMenuItem.Checked)
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

        private void nAPTSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nfsForm.Type != 5)
                _nfsForm = new NetworkFormationSimulationForm(this);
            _nfsForm.Type = 5;
            _nfsForm.ShowDialog();
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
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        net.SmartLoad(files[i].fileName, out Null);
                        net.cet = files[i].option;
                        temp = clique.convertClique(net.FindCliques(files[i].cutOff, false, 0.0, 0xFFFF, 0, false, _optionsForm.KCliqueValue, _optionsForm.KCliqueDiag));
                        net.merge(cliques, temp);
                        if (useweight)
                            net.LoadweightVector(weightfile);
                        cliqueList.Add(temp);
                        net.data.Add(net.mTable["Data"]);
                    }
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

        private void binarizedMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reachBinary = true;
            SetNewDisplayMatrix("Reachability");
            LoadData();
            SetChecked();
        }

        private void valuedMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reachBinary = false;
            SetNewDisplayMatrix("Reachability");
            LoadData();
            SetChecked();
        }

        
        // Deprecated Functions
        /*
        private void cliqueAffiliationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueAffiliation");
            LoadData();
            SetChecked();
        }
        
        
        private void jointCliqueAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("JointCliqueAffiliation");
            LoadData();
            SetChecked();
        }

        
        private void cliqueMembershipOverlapMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueMembershipOverlap");
            LoadData();
            SetChecked();
        }

        private void cliquebyCliqueOverlapMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueByCliqueOverlap");
            LoadData();
            SetChecked();
        }
        */
        
        private void dataMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EnableStandardizedChecks();
            if (loadFrom != "Affil")
                displayMatrix = "Data";
            else
                displayMatrix = "Affil";
            LoadData();
            SetChecked();
        }

        private void reachabilityMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CognitiveReachability");
            LoadData();
            SetChecked();
        }

        private void binarizedMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            reachBinary = true;
            SetNewDisplayMatrix("CognitiveReachability");
            LoadData();
            SetChecked();
        }

        private void valuedMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            reachBinary = false;
            SetNewDisplayMatrix("CognitiveReachability");
            LoadData();
            SetChecked();
        }

        private void signedNetworkCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SignedNetwork");
            LoadData();
            SetChecked();
        }

        private void localBalanceMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("LocalBalance");
            LoadData();
            SetChecked();
        }

        private void signedNetworkCharacteristicsFileToolStripMenuItem_Click(object sender, EventArgs e)
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
                    if (year != previousYear && year <= endYear)
                    {
                        net.LoadSignedNetworkCharacteristics(dataGrid, _optionsForm.ReachNumMatrices, _optionsForm.reachSum, _optionsForm.reachZero, prevDisplayMatrix, year, reachBinary);
                        net.SaveSignedNetworkToFile(saveFileDialog.FileName, year == startYear, _optionsForm.SaveOverwrite && year == startYear);
                    }

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


        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
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
                        if (useMultipleFiles)
                        {
                            net.LoadFromMultipleFiles(fileNames, year);
                        }
                        else
                        {
                            net.LoadFromMatrixFile(openFileDialog.FileName, year);
                        }
                    }
                    else if (loadFrom == "Dyadic")
                    {
                        if (useMultipleFiles)
                        {
                            net.LoadFromMultivariableDyadicFile(fileNames[0], year);
                        }
                        else
                        {
                            net.LoadFromDyadicFile(openFileDialog.FileName, year);
                        }
                    }
                    else if (loadFrom == "Random")
                    {
                        net.LoadRandom(_randomForm.N, "Data", _randomSymmetric, _randomForm.ProbRange, _randomForm.MinProb, _randomForm.MaxProb, _randomForm.RandomN, _randomForm.RandomMinN, _randomForm.RandomMaxN, _randomForm.RandomIntN);
                    }
                    else if (loadFrom == "ValuedRandom")
                    {
                        net.LoadValuedRandom(_vrandomForm.N, "Data", _randomSymmetric, _vrandomForm.vmin, _vrandomForm.vmax, _vrandomForm.datatype, _vrandomForm.zerodiagonalized, _vrandomForm.ProbRange, _vrandomForm.MinProb, _vrandomForm.MaxProb, _vrandomForm.RandomN, _vrandomForm.RandomMinN, _vrandomForm.RandomMaxN, _vrandomForm.RandomIntN);
                    }
                    progress.curYear = year;
                    Application.DoEvents();
                    previousYear = year;

                    if (displayMatrix == "Counter" || displayMatrix == "SignedNetwork" || displayMatrix == "Centrality" ||
                        displayMatrix == "NetworkPower" || displayMatrix == "LocalTransitivity" || displayMatrix == "NatDep" ||
                        displayMatrix == "Triadic" || displayMatrix == "Characteristics" || displayMatrix == "ClusterCharacteristics" ||
                        displayMatrix == "BlockCharacteristics" || displayMatrix == "Community" || displayMatrix == "OverlapCommCharacteristics" ||
                        displayMatrix == "LocalBalance" || displayMatrix == "CliqueCoefficients" || displayMatrix == "BlockCoefficients" ||
                        displayMatrix == "ClusterCoefficients" || displayMatrix == "OverlapCommCoefficients" || displayMatrix == "SingleNetworkExpectations" ||
                        displayMatrix == "NetworkSpilloverStatistics")
                    {
                        if (net.CohesionFilename != null)
                            net.CohesionMatrix = MatrixReader.ReadMatrixFromFile(net.CohesionFilename, year);
                        //DoLoadCorrect(year);
                        currentYear = year;
                        LoadData();
                        if (displayMatrix == "NatDep")
                        {
                            net.LoadUnitDependency(year);
                        }
                        else if (displayMatrix == "Community")
                        {
                            if (communityType == CommunityType.Char || communityType == CommunityType.Cluster || communityType == CommunityType.Coefficients || communityType == CommunityType.ovCoefficients || communityType == CommunityType.newCoefficients || communityType == CommunityType.newChar)
                            {
                                // do nothing; just avoiding a nested if statement
                            }
                            else
                            {
                                throw new Exception("Cannot save matrix as a Table format");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot save " + displayMatrix.ToString() + " matrix as a Table format");
                    }
                        
                    net.SaveAsTableToFile(saveFileDialog.FileName, year == startYear, _optionsForm.SaveOverwrite && year == startYear, displayMatrix, communityType);
                }
            }
        }

        /**************************************************************************
         **************************************************************************
         ************************* Moved Subgroups Menu ***************************
         **************************************************************************
         *************************************************************************/
        
        private void cliqueAffiliationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void cliqueSizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cliqueSizeToolStripMenuItem1.Checked = !cliqueSizeToolStripMenuItem1.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void cliqueCohesionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cliqueCohesionToolStripMenuItem1.Checked = !cliqueCohesionToolStripMenuItem1.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void estebanRayIndexToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            estebanRayIndexToolStripMenuItem1.Checked = !estebanRayIndexToolStripMenuItem1.Checked;
            displayMatrix = "Affiliation";
            DisableStandardizedChecks();
            LoadData();
            SetChecked();
        }

        private void cliqueMembershipOverlapMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Overlap");
            LoadData();
            SetChecked();
        }

        private void diagonallyStandardizedToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapDiag");
            LoadData();
            SetChecked();
        }

        private void cliqueOverlapMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CBCO");
            diag = false;
            LoadData();
            SetChecked();
        }

        private void diagonallyStandardizedToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CBCODiag");
            diag = true;
            LoadData();
            SetChecked();
        }

        private void cliqueCharacteristicsMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _cliqueForm.ShowDialog();
            displayMatrix = "Characteristics";
            LoadData();
            SetChecked();
        }

        private void interCliqueDistanceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ICD");
            LoadData();
            SetChecked();
        }

        private void jointCliqueAffiliationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("JointCliqueAffiliation");
            LoadData();
            SetChecked();
        }

        private void cliqueMembershipOverlapMatrixToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueMembershipOverlap");
            LoadData();
            SetChecked();
        }

        private void jointCliqueOverlapMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueByCliqueOverlap");
            LoadData();
            SetChecked();
        }

        private void cliqueDensityMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueDensity");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueRelativeDensity");
            LoadData();
            SetChecked();
        }


        private void cONCORBlockAffiliationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _blocForm.ShowDialog();
            SetNewDisplayMatrix("CONCOR");
            LoadData();
            SetChecked();
        }

        private void correlationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            correlationToolStripMenuItem.Checked = true;
            stdEuclideanDistanceToolStripMenuItem.Checked = false;
            
            cONCORBlockAffiliationToolStripMenuItem1_Click(sender, e);
        }

        private void stdEuclideanDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stdEuclideanDistanceToolStripMenuItem.Checked = true;
            correlationToolStripMenuItem.Checked = false;

            cONCORBlockAffiliationToolStripMenuItem1_Click(sender, e);
        }

        private void sociomatrixEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockPartitionS");
            LoadData();
            SetChecked();
        }

        private void blockIdentityEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockPartitionI");
            LoadData();
            SetChecked();
        }

        private void blockDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DensityBlockMatrix");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("RelativeDensityBlockMatrix");
            LoadData();
            SetChecked();
        }

        // may not need
        
        private void blockCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockCohesionMatrix");
            LoadData();
            SetChecked();
        }
        

        private void blockCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _blockForm.ShowDialog();
            if (_blockForm.SVC.Count + _blockForm.DVC.Count >= 0)
            {
                SetNewDisplayMatrix("BlockCharacteristics");
                LoadData();
                SetChecked();
            }
        }

        

        private void clusterAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _clusterForm.ShowDialog();
            SetNewDisplayMatrix("Clustering");
            LoadData();
            SetChecked();
        }

        private void clusterPartitionMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ClusterPartition");
            LoadData();
            SetChecked(); 
        }

        private void clusterDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DensityClusterMatrix");
            LoadData();
            SetChecked();
        }


        private void relativeDensityToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("RelativeDensityClusterMatrix");
            LoadData();
            SetChecked(); 
        }

        private void clusterCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ClusterCohesivenessMatrix");
            LoadData();
            SetChecked();
        }

        private void clusterCharacteristicsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _blockForm.ShowDialog();
            if (_blockForm.SVC.Count + _blockForm.DVC.Count >= 0)
            {
                SetNewDisplayMatrix("ClusterCharacteristics");
                LoadData();
                SetChecked();
            }
        }

        private void communityAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Affil;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Density;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.RelativeDensity;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Cohesion;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _comForm.ShowDialog();
            if (_comForm.SVC.Count + _comForm.DVC.Count /*+ _comForm.attrMatrix.Count*/ >= 0)
            {
                communityType = CommunityType.Char;
                SetNewDisplayMatrix("Community");
                LoadData();
                SetChecked();
            }
        }

        private void modularityCoefficientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Cluster;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void overlappingCommunityAffiliationMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlappingCommunity");
            LoadData();
            SetChecked();
        }

        private void overlappingCommunityDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapCommDensity");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapCommRelativeDensity");
            LoadData();
            SetChecked();
        }

        private void overlappingCommunityCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapCommCohesiveMatrix");
            LoadData();
            SetChecked();
        }

        private void overlappingCommunityCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _overlapCommForm.ShowDialog();
            if (_overlapCommForm.SVC.Count + _overlapCommForm.DVC.Count >= 0)
            {
                SetNewDisplayMatrix("OverlapCommCharacteristics");
                LoadData();
                SetChecked();
            }
        }

        private void overlappingCommunityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapCommModularityEQ");
            LoadData();
            SetChecked();
        }


        // Clique cohesion
        private void cliqueCohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueCohesionMatrix");
            LoadData();
            SetChecked();
        }


        // Coefficients
        private void cliqueCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("CliqueCoefficients");
            LoadData();
            SetChecked();
        }

        private void blockCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("BlockCoefficients");
            LoadData();
            SetChecked();
        }

        private void clusterCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("ClusterCoefficients");
            LoadData();
            SetChecked();
        }

        private void communityCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Coefficients;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void overlappingCommunityCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("OverlapCommCoefficients");
            LoadData();
            SetChecked();
        }

        private void localTransitivityToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("LocalTransitivity");
            LoadData();
            SetChecked();
        }

        private void matrixMultiplicationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Multiplication");
            _multiplicationForm.ShowDialog();

            LoadData();
            SetChecked();
        }

        private void unitBasedConversionToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("Data");
            LoadData();
            SetChecked();
        }

        private void eventBasedConversionToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DataEvent");
            LoadData();
            SetChecked();
        }

        private void unitBasedConversionToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilCorrelation");
            LoadData();
            SetChecked();
        }

        private void eventBasedConversionToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilCorrelationEvent");
            LoadData();
            SetChecked();
        }

        private void unitBasedConversionToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilEuclidean");
            LoadData();
            SetChecked();
        }

        private void eventBasedConversionToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("AffilEuclideanEvent");
            LoadData();
            SetChecked();
        }

        private void affiliationToSociomatrixConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // need this to created an instance of the ToolStripMenuItem
        }

        private void matrixFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            matrixToolStripMenuItem.Checked = true;
            dyadicFileToolStripMenuItem2.Checked = false;
            monadicFileToolStripMenuItem1.Checked = false;
            ef = Network.ElementwiseFormat.Matrix;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");
                LoadData();
                SetChecked();
            }
        }

        private void dyadicFileToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            matrixToolStripMenuItem.Checked = false;
            dyadicFileToolStripMenuItem2.Checked = true;
            monadicFileToolStripMenuItem1.Checked = false;
            ef = Network.ElementwiseFormat.Matrix;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");
                LoadData();
                SetChecked();
            }
        }

        private void monadicFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            matrixToolStripMenuItem.Checked = false;
            dyadicFileToolStripMenuItem2.Checked = false;
            monadicFileToolStripMenuItem1.Checked = true;
            ef = Network.ElementwiseFormat.Matrix;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("Elementwise");
                LoadData();
                SetChecked();
            }
        }

        private void elementwiseMultiplicationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // need this to created an instance of the ToolStripMenuItem
        }

        private void dyadicTransitivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("DyadicTransitivity");
            LoadData();
            SetChecked();
        }

        private void singleNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDisplayMatrix("SingleNetworkExpectations");
            LoadData();
            SetChecked();
        }

        private void networkSpilloverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_spilloverForm.ShowDialog() == DialogResult.OK)
            {
                SetNewDisplayMatrix("NetworkSpilloverStatistics");
                LoadData();
                SetChecked();
            }
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void communityAffiliationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.newAffil;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityDensityMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.newDensity;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityCohesionMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.newCohesion;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.newRelativeDensity;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityCharacteristicsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _comForm.ShowDialog();
            if (_comForm.SVC.Count + _comForm.DVC.Count /*+ _comForm.attrMatrix.Count*/ >= 0)
            {
                communityType = CommunityType.newChar;
                SetNewDisplayMatrix("Community");
                LoadData();
                SetChecked();
            }
        }

        private void separationCoefficientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.Separation;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void communityCoefficientsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.newCoefficients;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void overlappingCommunityAffiliationMatrixToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovAffil;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void newOverlappingCommunityCoefficientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovCoefficients;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void newOverlappingCommunityCohesionMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovCohesion;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void newOverlappingCommunityCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _comForm.ShowDialog();
            if (_comForm.SVC.Count + _comForm.DVC.Count /*+ _comForm.attrMatrix.Count*/ >= 0)
            {
                communityType = CommunityType.ovChar;
                SetNewDisplayMatrix("Community");
                LoadData();
                SetChecked();
            }
        }

        private void newOverlappingDensityMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovDensity;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void relativeDensityToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovRelativeDensity;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void modularityCoefficientToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            communityType = CommunityType.ovCluster;
            SetNewDisplayMatrix("Community");
            LoadData();
            SetChecked();
        }

        private void loadFromToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void agentShocksModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            openFileDialog.Multiselect = false;
            SetMode(false);
            _ABMForm.ShowDialog();
            loadFrom = "ABMModel";
            int nodecount = 10;
            int netcount = 1;
            int min = 10;
            int max = 100;
            int step = 10;
            if (!_ABMForm.useparams)
            {
                nodecount = _ABMForm.N;
                netcount = _ABMForm.networks;
            }
            else
            {
                min = _ABMForm.minsize;
                max = _ABMForm.maxsize;
                step = _ABMForm.stepsize;
                netcount = _ABMForm.networks;
            }
            SetFormTitle();
            displayMatrix = "Data";
            currentYear = _ABMForm.netID;
            _optionsForm.ReachNumMatrices = _ABMForm.N - 1;
            string[] labels = new string[20] { "runno", "iteration", "row", "col", "edge", "C0r", "C0c", "kr", "kc", "Csr", "Csc", "Seqr", "Seqc", "Offerr", "Offerc", "Accr", "Accc", "droppedr", "droppedc", "initial" };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                net.mList = new List<Matrix>();
                Matrix m;
                int runno = _ABMForm.netID;
                for (int i = min; i <= max; i += step)
                {
                    for (int j = 0; j < netcount; j++)
                    {
                        m = net.ABMShocksNetworkFormation(i, _ABMForm.netID + j, saveFileDialog.FileName, runno++, false);
                        m.ColLabels.SetLabels(labels);
                        net.mList.Add(m);
                    }
                }
                //}
                net.mTable["Data"] = net.mList[0];
                LoadData();
            }
            else
            {
                MessageBox.Show("Invalid file name.");
            }
             */

        }

        private void randomMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void agentBasedModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //used
            openFileDialog.Multiselect = false;
            SetMode(false);
            _ABMForm.ShowDialog();
            loadFrom = "ABMModel";
            int nodecount = 10;
            int netcount = 1;
            int min = 10;
            int max = 100;
            int step = 10;
            if (!_ABMForm.useparams)
            {
                nodecount = _ABMForm.N;
                netcount = _ABMForm.networks;
            }
            else
            {
                min = _ABMForm.minsize;
                max = _ABMForm.maxsize;
                step = _ABMForm.stepsize;
                netcount = _ABMForm.networks;
            }
            SetFormTitle();
            displayMatrix = "Data";
            currentYear = _ABMForm.netID;
            _optionsForm.ReachNumMatrices = _ABMForm.N - 1;
            string[] labels = new string[27] { "runno", "iteration", "row", "col", "edge", "C0r", "C0c", "demrow", "enmyenmyrow", "cultismrow", "demcol", "enmyenmycol", "cultismcol", "kr", "kc", "Csr", "Csc", "Seqr", "Seqc", "Offerr", "Offerc", "Accr", "Accc", "droppedr", "droppedc", "initial", "uij"};
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                string[] words = saveFileDialog.FileName.Split('.');
                string[] filenames = new string[4];
                filenames[0] = words[0] + "-netform." + words[1];
                filenames[1] = words[0] + "-preshock." + words[1];
                filenames[2] = words[0] + "-log." + words[1];
                if (File.Exists(filenames[0]) || File.Exists(filenames[1]) || File.Exists(filenames[2]))
                {
                    _ABMConfirm.text = "One or more of the files " + Environment.NewLine + Environment.NewLine + filenames[0] + ", " + Environment.NewLine + filenames[1] + ", " + Environment.NewLine + filenames[2] + Environment.NewLine + Environment.NewLine + " already exist. Overwrite?";
                    _ABMConfirm.confirmation = false;
                    _ABMConfirm.ShowDialog();
                    if(_ABMConfirm.confirmation == true)
                    {
                        try
                        {
                            File.Delete(filenames[0]);
                            File.Delete(filenames[1]);
                            File.Delete(filenames[2]);
                        }
                        catch
                        {
                            MessageBox.Show("Error occurred while removing files.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to use chosen file name.");
                        return;
                    }
                }
                net.mList = new List<Matrix>();
                Matrix m;
                /*if (false)
                {
                    for (int i = 0; i < netcount; i++)
                    {
                        m = net.ABMShocksNetworkFormation(nodecount, 0, saveFileDialog.FileName);
                        m.ColLabels.SetLabels(labels);
                        net.mList.Add(m);
                    }
                }
                else
                {*/
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(labels[0]);
                for (int i = 1; i < labels.Length; i++)
                {
                    sb.Append("," + labels[i]);
                }
                System.IO.File.WriteAllText(words[0] + "-netform." + words[1], sb.ToString() + Environment.NewLine);
                System.IO.File.WriteAllText(words[0] + "-shock." + words[1], sb.ToString() + Environment.NewLine);
                
                for (int i = 1; i <= 6; i++)
                {
                    System.IO.File.WriteAllText(words[0] + "-" + i + "N." + words[1], sb.ToString() + Environment.NewLine);
                    System.IO.File.WriteAllText(words[0] + "-" + i + "NC." + words[1], sb.ToString() + Environment.NewLine);
                    System.IO.File.WriteAllText(words[0] + "-" + i + "NT." + words[1], sb.ToString() + Environment.NewLine);
                    
                }
                int runno = 1, runno2 = 1;
                List<int> runnos = new List<int>(), iterations = new List<int>();
                for (int i = min; i <= max; i += step)
                {
                    for (int j = 0; j < netcount; j++)
                    {
                        runnos.Add(runno2++);
                        iterations.Add(18 * i);
                    }
                }

                runnos[0] = 1;
                iterations[0] = iterations[0];

                ABMProgressForm apform = new ABMProgressForm(runnos, iterations);
                apform.Show();
                for (int i = min; i <= max; i += step)
                {
                    for (int j = 0; j < netcount; j++)
                    {
                        m = net.ABMShocksNetworkFormation(i, _ABMForm.netID + j, saveFileDialog.FileName, runno++, _ABMForm.homophily, ref apform);
                        m.ColLabels.SetLabels(labels);
                        net.mList.Add(m);
                    }
                }
                    
                //}
                net.mTable["Data"] = net.mList[0];
                LoadData();
            }
            else
            {
                MessageBox.Show("Invalid file name.");
            }
        }
        
    }
}