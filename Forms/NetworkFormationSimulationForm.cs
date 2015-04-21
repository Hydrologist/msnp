using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network;
using Network.Matrices;
using RandomUtilities;
using System.IO;
using Network.IO;

namespace NetworkGUI
{
    public partial class NetworkFormationSimulationForm : Form
    {
        private enum Status
        {
            Ready, Running, Cancelled
        }

        private MainForm parent = null;
        private Status _status;
        private bool _overwrite;
        private int _type; //realist = 1, liberal = 2, simplified realist = 3, simplified liberal = 4, NAPT simulation = 5

        public bool Overwrite
        {
            get { return _overwrite; }
            set { _overwrite = value; }
        }

        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (_type==1)
                {
                    this.Height = 340;
                    label6.Top = 212;
                    tMaxTextBox.Top = 212;
                    label12.Top = 212;
                    maxStageTextBox.Top = 212;
                    label7.Top = 212;
                    nTextBox.Top = 212;
                    runButton.Top = 212;
                    label18.Top = 212;
                    brTextBox.Top = 212;
                    closeButton.Top = 212;
                    multipleNCheckBox.Top = 237;
                    label14.Top = label15.Top = label16.Top = minNbox.Top = maxNbox.Top = intNbox.Top = 262;
                    label2.Visible = true;
                    contminbox.Visible = true;
                    contmaxbox.Visible = true;
                    contminlabel.Visible = true;
                    contmaxlabel.Visible = true;
                    contiguityCheckBox.Visible = true;
                    contiguityDCheckBox.Visible = true;
                    contiguityMatrixFile.Visible = true;
                    openContiguityFile.Visible = true;
                    label3.Visible = true;
                    majminbox.Visible = true;
                    majmaxbox.Visible = true;
                    majminlabel.Visible = true;
                    majmaxlabel.Visible = true;
                    majorPowersCheckBox.Visible = true;
                    majorPowersDCheckBox.Visible = true;
                    majorPowersVectorFile.Visible = true;
                    openMajorPowersFile.Visible = true;
                    label4.Visible = true;
                    confminbox.Visible = true;
                    confmaxbox.Visible = true;
                    confminlabel.Visible = true;
                    confmaxlabel.Visible = true;
                    conflictCheckBox.Visible = true;
                    conflictDCheckBox.Visible = true;
                    conflictMatrixFile.Visible = true;
                    openConflictFile.Visible = true;
                    demominbox.Visible = false;
                    demominlabel.Visible = false;
                    demomaxbox.Visible = false;
                    demomaxlabel.Visible = false;
                    demoCheckBox.Visible = false;
                    demoDCheckBox.Visible = false;
                    demoMatrixFile.Visible = false;
                    label9.Visible = false;
                    openDemoFile.Visible = false;
                    jcminbox.Visible = false;
                    jcmaxbox.Visible = false;
                    jcminlabel.Visible = false;
                    jcmaxlabel.Visible = false;
                    jcCheckBox.Visible = false;
                    jcDCheckBox.Visible = false;
                    jcMatrixFile.Visible = false;
                    label10.Visible = false;
                    jcButton.Visible = false;
                    midminbox.Visible = false;
                    midmaxbox.Visible = false;
                    midminlabel.Visible = false;
                    midmaxlabel.Visible = false;
                    midButton.Visible = false;
                    label11.Visible = false;
                    midMatrixFile.Visible = false;
                    midCheckBox.Visible = false;
                    midDCheckBox.Visible = false;
                    label5.Top = 178;
                    outputFile.Top = 178;
                    openOutputFile.Top = 178;
                    outputDyadicCheckBox.Top = 178;

                    // Cultural Similarity Matrix
                    label21.Visible = false;
                    csMatrixFile.Visible = false;
                    csButton.Visible = false;
                    csCheckBox.Visible = false;
                    csDCheckbox.Visible = false;
                    csMinLabel.Visible = false;
                    csMinBox.Visible = false;
                    csMaxLabel.Visible = false;
                    csMaxBox.Visible = false;


                    // Reliability Matrix
                    label24.Visible = false;
                    reliabilityMatrixFile.Visible = false;
                    reliabilityButton.Visible = false;
                    reliabilityCheckBox.Visible = false;
                    reliabilityDCheckBox.Visible = false;
                    reliabilityMinLabel.Visible = false;
                    reliabilityMinBox.Visible = false;
                    reliabilityMaxLabel.Visible = false;
                    reliabilityMaxBox.Visible = false;

                    // Expected Alliance Output File
                    label25.Visible = false;
                    expectedAllianceFile.Visible = false;
                    openExpectedAlliance.Visible = false;
                    expectedAllianceDCheckBox.Visible = false;
                }
                else if(_type==2)
                {
                    this.Height = 399;
                    label6.Top = 267;
                    tMaxTextBox.Top = 267;
                    label12.Top = 267;
                    maxStageTextBox.Top = 267;
                    label7.Top = 267;
                    nTextBox.Top = 267;
                    runButton.Top = 267;
                    label18.Top = 267;
                    brTextBox.Top = 267;
                    closeButton.Top = 267;
                    multipleNCheckBox.Top = 292;
                    label14.Top = label15.Top = label16.Top = minNbox.Top = maxNbox.Top = intNbox.Top = 317;
                    demominbox.Visible = true;
                    demominlabel.Visible = true;
                    demomaxbox.Visible = true;
                    demomaxlabel.Visible = true;
                    demoCheckBox.Visible = true;
                    demoDCheckBox.Visible = true;
                    demoMatrixFile.Visible = true;
                    label9.Visible = true;
                    openDemoFile.Visible = true;
                    jcminbox.Visible = true;
                    jcmaxbox.Visible = true;
                    jcminlabel.Visible = true;
                    jcmaxlabel.Visible = true;
                    jcCheckBox.Visible = true;
                    jcDCheckBox.Visible = true;
                    jcMatrixFile.Visible = true;
                    label10.Visible = true;
                    jcButton.Visible = true;
                    label2.Visible = true;
                    contminbox.Visible = true;
                    contmaxbox.Visible = true;
                    contminlabel.Visible = true;
                    contmaxlabel.Visible = true;
                    contiguityCheckBox.Visible = true;
                    contiguityDCheckBox.Visible = true;
                    contiguityMatrixFile.Visible = true;
                    openContiguityFile.Visible = true;
                    label3.Visible = true;
                    majminbox.Visible = true;
                    majmaxbox.Visible = true;
                    majminlabel.Visible = true;
                    majmaxlabel.Visible = true;
                    majorPowersCheckBox.Visible = true;
                    majorPowersDCheckBox.Visible = true;
                    majorPowersVectorFile.Visible = true;
                    openMajorPowersFile.Visible = true;
                    label4.Visible = true;
                    confminbox.Visible = true;
                    confmaxbox.Visible = true;
                    confminlabel.Visible = true;
                    confmaxlabel.Visible = true;
                    conflictCheckBox.Visible = true;
                    conflictDCheckBox.Visible = true;
                    conflictMatrixFile.Visible = true;
                    openConflictFile.Visible = true;
                    midButton.Visible = false;
                    label11.Visible = false;
                    midminbox.Visible = false;
                    midmaxbox.Visible = false;
                    midminlabel.Visible = false;
                    midmaxlabel.Visible = false;
                    midMatrixFile.Visible = false;
                    midCheckBox.Visible = false;
                    midDCheckBox.Visible = false;
                    label5.Top = 178;
                    outputFile.Top = 178;
                    openOutputFile.Top = 178;
                    outputDyadicCheckBox.Top = 178;

                    // Cultural Similarity Matrix
                    label21.Visible = false;
                    csMatrixFile.Visible = false;
                    csButton.Visible = false;
                    csCheckBox.Visible = false;
                    csDCheckbox.Visible = false;
                    csMinLabel.Visible = false;
                    csMinBox.Visible = false;
                    csMaxLabel.Visible = false;
                    csMaxBox.Visible = false;


                    // Reliability Matrix
                    label24.Visible = false;
                    reliabilityMatrixFile.Visible = false;
                    reliabilityButton.Visible = false;
                    reliabilityCheckBox.Visible = false;
                    reliabilityDCheckBox.Visible = false;
                    reliabilityMinLabel.Visible = false;
                    reliabilityMinBox.Visible = false;
                    reliabilityMaxLabel.Visible = false;
                    reliabilityMaxBox.Visible = false;

                    // Expected Alliance Output File
                    label25.Visible = false;
                    expectedAllianceFile.Visible = false;
                    openExpectedAlliance.Visible = false;
                    expectedAllianceDCheckBox.Visible = false;

                }
                else if (_type == 3)
                {
                    this.Height = 295 ;
                    label6.Top = 167;
                    tMaxTextBox.Top =167;
                    label12.Top = 167;
                    maxStageTextBox.Top = 167;
                    label7.Top = 167;
                    nTextBox.Top = 167;
                    runButton.Top = 167;
                    label18.Top = 167;
                    brTextBox.Top = 167;
                    closeButton.Top = 167;
                    multipleNCheckBox.Top = 192;
                    label14.Top = label15.Top = label16.Top = minNbox.Top = maxNbox.Top = intNbox.Top = 217;
                    label5.Top = 121;
                    outputFile.Top = 121;
                    openOutputFile.Top = 121;
                    outputDyadicCheckBox.Top = 121;
                    midminbox.Top = 93;
                    midmaxbox.Top = 93;
                    midminlabel.Top = 93;
                    midmaxlabel.Top = 93;
                    midButton.Top = 93;
                    midCheckBox.Top = 93;
                    midDCheckBox.Top = 93;
                    midMatrixFile.Top = 93;
                    label11.Top = 93;

                    demoCheckBox.Visible = false;
                    demoDCheckBox.Visible = false;
                    demoMatrixFile.Visible = false;
                    label9.Visible = false;
                    openDemoFile.Visible = false;
                    jcCheckBox.Visible = false;
                    jcDCheckBox.Visible = false;
                    jcMatrixFile.Visible = false;
                    label10.Visible = false;
                    jcButton.Visible = false;
                    label2.Visible = false;
                    contiguityCheckBox.Visible = false;
                    contiguityDCheckBox.Visible = false;
                    contiguityMatrixFile.Visible = false;
                    openContiguityFile.Visible = false;
                    label3.Visible = false;
                    majorPowersCheckBox.Visible = false;
                    majorPowersDCheckBox.Visible = false;
                    majorPowersVectorFile.Visible = false;
                    openMajorPowersFile.Visible = false;
                    label4.Visible = false;
                    conflictCheckBox.Visible = false;
                    conflictDCheckBox.Visible = false;
                    conflictMatrixFile.Visible = false;
                    openConflictFile.Visible = false;
                    midMatrixFile.Visible = true;
                    label11.Visible = true;
                    midCheckBox.Visible = true;
                    midDCheckBox.Visible = true;
                    midButton.Visible = true;
                    contminbox.Visible = false;
                    contmaxbox.Visible = false;
                    contminlabel.Visible = false;
                    contmaxlabel.Visible = false;
                    majminbox.Visible = false;
                    majmaxbox.Visible = false;
                    majminlabel.Visible = false;
                    majmaxlabel.Visible = false;
                    confminbox.Visible = false;
                    confmaxbox.Visible = false;
                    confminlabel.Visible = false;
                    confmaxlabel.Visible = false;
                    demominbox.Visible = false;
                    demominlabel.Visible = false;
                    demomaxbox.Visible = false;
                    demomaxlabel.Visible = false;
                    jcminbox.Visible = false;
                    jcmaxbox.Visible = false;
                    jcminlabel.Visible = false;
                    jcmaxlabel.Visible = false;
                    midminbox.Visible = true;
                    midmaxbox.Visible = true;
                    midminlabel.Visible = true;
                    midmaxlabel.Visible = true;

                    // Cultural Similarity Matrix
                    label21.Visible = false;
                    csMatrixFile.Visible = false;
                    csButton.Visible = false;
                    csCheckBox.Visible = false;
                    csDCheckbox.Visible = false;
                    csMinLabel.Visible = false;
                    csMinBox.Visible = false;
                    csMaxLabel.Visible = false;
                    csMaxBox.Visible = false;


                    // Reliability Matrix
                    label24.Visible = false;
                    reliabilityMatrixFile.Visible = false;
                    reliabilityButton.Visible = false;
                    reliabilityCheckBox.Visible = false;
                    reliabilityDCheckBox.Visible = false;
                    reliabilityMinLabel.Visible = false;
                    reliabilityMinBox.Visible = false;
                    reliabilityMaxLabel.Visible = false;
                    reliabilityMaxBox.Visible = false;

                    // Expected Alliance Output File
                    label25.Visible = false;
                    expectedAllianceFile.Visible = false;
                    openExpectedAlliance.Visible = false;
                    expectedAllianceDCheckBox.Visible = false;
                }
                else if (_type == 4)
                {
                    this.Height = 359 ;
                    label6.Top = 229;
                    tMaxTextBox.Top = 229;
                    label7.Top = 229;
                    nTextBox.Top = 229;
                    label12.Top = 229;
                    maxStageTextBox.Top = 229;
                    runButton.Top = 229;
                    closeButton.Top = 229;
                    label18.Top = 229;
                    brTextBox.Top = 229;
                    multipleNCheckBox.Top = 254;
                    label14.Top = label15.Top = label16.Top = minNbox.Top = maxNbox.Top = intNbox.Top = 279;
                    label5.Top = 179;
                    outputFile.Top = 179;
                    openOutputFile.Top = 179;
                    outputDyadicCheckBox.Top = 179;
                    midminbox.Top = 92;
                    midmaxbox.Top = 92;
                    midminlabel.Top = 92;
                    midmaxlabel.Top = 92;
                    midButton.Top = 92;
                    midCheckBox.Top = 92;
                    midDCheckBox.Top = 92;
                    midMatrixFile.Top = 92;
                    label11.Top = 92;
                    demominbox.Top = 121;
                    demomaxbox.Top = 121;
                    demominlabel.Top = 121;
                    demomaxlabel.Top = 121;
                    demoCheckBox.Top = 121;
                    demoDCheckBox.Top = 121;
                    demoMatrixFile.Top = 121;
                    label9.Top = 121;
                    openDemoFile.Top = 121;
                    jcminbox.Top = 150;
                    jcmaxbox.Top = 150;
                    jcminlabel.Top = 150;
                    jcmaxlabel.Top = 150;
                    jcCheckBox.Top = 150;
                    jcDCheckBox.Top = 150;
                    jcMatrixFile.Top = 150;
                    label10.Top = 150;
                    jcButton.Top = 150;

                    demoCheckBox.Visible = true;
                    demoDCheckBox.Visible = true;
                    demoMatrixFile.Visible = true;
                    label9.Visible = true;
                    openDemoFile.Visible = true;
                    jcCheckBox.Visible = true;
                    jcDCheckBox.Visible = true;
                    jcMatrixFile.Visible = true;
                    label10.Visible = true;
                    jcButton.Visible = true;
                    label2.Visible = false;
                    contiguityCheckBox.Visible = false;
                    contiguityDCheckBox.Visible = false;
                    contiguityMatrixFile.Visible = false;
                    openContiguityFile.Visible = false;
                    label3.Visible = false;
                    majorPowersCheckBox.Visible = false;
                    majorPowersDCheckBox.Visible = false;
                    majorPowersVectorFile.Visible = false;
                    openMajorPowersFile.Visible = false;
                    label4.Visible = false;
                    conflictCheckBox.Visible = false;
                    conflictDCheckBox.Visible = false;
                    conflictMatrixFile.Visible = false;
                    openConflictFile.Visible = false;
                    midMatrixFile.Visible = true;
                    label11.Visible = true;
                    midCheckBox.Visible = true;
                    midDCheckBox.Visible = true;
                    midButton.Visible = true;
                    contminbox.Visible = false;
                    contmaxbox.Visible = false;
                    contminlabel.Visible = false;
                    contmaxlabel.Visible = false;
                    majminbox.Visible = false;
                    majmaxbox.Visible = false;
                    majminlabel.Visible = false;
                    majmaxlabel.Visible = false;
                    confminbox.Visible = false;
                    confmaxbox.Visible = false;
                    confminlabel.Visible = false;
                    confmaxlabel.Visible = false;
                    demominbox.Visible = true;
                    demominlabel.Visible = true;
                    demomaxbox.Visible = true;
                    demomaxlabel.Visible = true;
                    jcminbox.Visible = true;
                    jcmaxbox.Visible = true;
                    jcminlabel.Visible = true;
                    jcmaxlabel.Visible = true;
                    midminbox.Visible = true;
                    midmaxbox.Visible = true;
                    midminlabel.Visible = true;
                    midmaxlabel.Visible = true;

                    // Cultural Similarity Matrix
                    label21.Visible = false;
                    csMatrixFile.Visible = false;
                    csButton.Visible = false;
                    csCheckBox.Visible = false;
                    csDCheckbox.Visible = false;
                    csMinLabel.Visible = false;
                    csMinBox.Visible = false;
                    csMaxLabel.Visible = false;
                    csMaxBox.Visible = false;


                    // Reliability Matrix
                    label24.Visible = false;
                    reliabilityMatrixFile.Visible = false;
                    reliabilityButton.Visible = false;
                    reliabilityCheckBox.Visible = false;
                    reliabilityDCheckBox.Visible = false;
                    reliabilityMinLabel.Visible = false;
                    reliabilityMinBox.Visible = false;
                    reliabilityMaxLabel.Visible = false;
                    reliabilityMaxBox.Visible = false;

                    // Expected Alliance Output File
                    label25.Visible = false;
                    expectedAllianceFile.Visible = false;
                    openExpectedAlliance.Visible = false;
                    expectedAllianceDCheckBox.Visible = false;
                }
                else if (_type == 5)
                {

                    this.Height = 464;

                    // MID Matrix row
                    label11.Top = midMatrixFile.Top = midButton.Top = midCheckBox.Top
                                = midDCheckBox.Top = midminlabel.Top = midminbox.Top
                                = midmaxlabel.Top = midmaxbox.Top = 121;

                    // Democracy Matrix row
                    label9.Top = demoMatrixFile.Top = openDemoFile.Top = demoCheckBox.Top
                               = demoDCheckBox.Top = demominlabel.Top = demominbox.Top 
                               = demomaxlabel.Top = demomaxbox.Top = 150;

                    // JC Matrix row
                    label10.Top = jcMatrixFile.Top = jcButton.Top = jcCheckBox.Top 
                                = jcDCheckBox.Top = jcminlabel.Top = jcminbox.Top 
                                = jcmaxlabel.Top = jcmaxbox.Top = 179;

                    // Cultural Similarity Matrix row
                    label21.Top = csMatrixFile.Top = csButton.Top = csCheckBox.Top 
                                = csDCheckbox.Top = csMinLabel.Top = csMinBox.Top 
                                = csMaxLabel.Top = csMaxBox.Top = 208;

                    // Reliability Vector row
                    label24.Top = reliabilityMatrixFile.Top = reliabilityButton.Top
                                = reliabilityCheckBox.Top = reliabilityDCheckBox.Top
                                = reliabilityMinLabel.Top = reliabilityMinBox.Top
                                = reliabilityMaxLabel.Top = reliabilityMaxBox.Top = 237;

                    // Output File row
                    label5.Top = outputFile.Top = openOutputFile.Top 
                               = outputDyadicCheckBox.Top = 266;

                    // Expected Alliance File row
                    label25.Top = expectedAllianceFile.Top = openExpectedAlliance.Top
                                = expectedAllianceDCheckBox.Top = 295;

                    // No. of Cases row
                    label6.Top = tMaxTextBox.Top = label12.Top = maxStageTextBox.Top 
                               = label7.Top = nTextBox.Top = runButton.Top = label18.Top 
                               = brTextBox.Top = closeButton.Top = 329;

                    // Random N iterations row
                    multipleNCheckBox.Top = 358;
                    
                    // Range of N row
                    label14.Top = label15.Top = label16.Top = minNbox.Top = maxNbox.Top
                                = intNbox.Top = 387;

                    // Democracy Matrix
                    label9.Visible = true;
                    demominbox.Visible = true;
                    demominlabel.Visible = true;
                    demomaxbox.Visible = true;
                    demomaxlabel.Visible = true;
                    demoCheckBox.Visible = true;
                    demoDCheckBox.Visible = true;
                    demoMatrixFile.Visible = true;
                    openDemoFile.Visible = true;
                    
                    // JC Matrix
                    label10.Visible = true;
                    jcminbox.Visible = true;
                    jcmaxbox.Visible = true;
                    jcminlabel.Visible = true;
                    jcmaxlabel.Visible = true;
                    jcCheckBox.Visible = true;
                    jcDCheckBox.Visible = true;
                    jcMatrixFile.Visible = true;
                    jcButton.Visible = true;

                    // Contiguity Matrix
                    label2.Visible = true;
                    contminbox.Visible = true;
                    contmaxbox.Visible = true;
                    contminlabel.Visible = true;
                    contmaxlabel.Visible = true;
                    contiguityCheckBox.Visible = true;
                    contiguityDCheckBox.Visible = true;
                    contiguityMatrixFile.Visible = true;
                    openContiguityFile.Visible = true;
                    /*
                    label2.Visible = false ;
                    contminbox.Visible = false;
                    contmaxbox.Visible = false;
                    contminlabel.Visible = false;
                    contmaxlabel.Visible = false;
                    contiguityCheckBox.Visible = false;
                    contiguityDCheckBox.Visible = false;
                    contiguityMatrixFile.Visible = false;
                    openContiguityFile.Visible = false;
                    */

                    // Major Powers Vector
                    /*
                    label3.Visible = true;
                    majminbox.Visible = true;
                    majmaxbox.Visible = true;
                    majminlabel.Visible = true;
                    majmaxlabel.Visible = true;
                    majorPowersCheckBox.Visible = true;
                    majorPowersDCheckBox.Visible = true;
                    majorPowersVectorFile.Visible = true;
                    openMajorPowersFile.Visible = true;
                    */
                    label3.Visible = false;
                    majminbox.Visible = false;
                    majmaxbox.Visible = false;
                    majminlabel.Visible = false;
                    majmaxlabel.Visible = false;
                    majorPowersCheckBox.Visible = false;
                    majorPowersDCheckBox.Visible = false;
                    majorPowersVectorFile.Visible = false;
                    openMajorPowersFile.Visible = false;

                    // Conflict Matrix
                    /*
                    label4.Visible = true;
                    confminbox.Visible = true;
                    confmaxbox.Visible = true;
                    confminlabel.Visible = true;
                    confmaxlabel.Visible = true;
                    conflictCheckBox.Visible = true;
                    conflictDCheckBox.Visible = true;
                    conflictMatrixFile.Visible = true;
                    openConflictFile.Visible = true;
                    */
                    label4.Visible = false;
                    confminbox.Visible = false;
                    confmaxbox.Visible = false;
                    confminlabel.Visible = false;
                    confmaxlabel.Visible = false;
                    conflictCheckBox.Visible = false;
                    conflictDCheckBox.Visible = false;
                    conflictMatrixFile.Visible = false;
                    openConflictFile.Visible = false;

                    // MID Matrix
                    /*
                    label11.Visible = false;
                    midButton.Visible = false;
                    midminbox.Visible = false;
                    midmaxbox.Visible = false;
                    midminlabel.Visible = false;
                    midmaxlabel.Visible = false;
                    midMatrixFile.Visible = false;
                    midCheckBox.Visible = false;
                    midDCheckBox.Visible = false;
                    */
                    label11.Visible = true;
                    midButton.Visible = true;
                    midminbox.Visible = true;
                    midmaxbox.Visible = true;
                    midminlabel.Visible = true;
                    midmaxlabel.Visible = true;
                    midMatrixFile.Visible = true;
                    midCheckBox.Visible = true;
                    midDCheckBox.Visible = true;

                    // Don't need; it's already in that position
                    /*
                    label5.Top = 178;
                    outputFile.Top = 178;
                    openOutputFile.Top = 178;
                    outputDyadicCheckBox.Top = 178;
                    */

                    // Cultural Similarity Matrix
                    label21.Visible = true;
                    csMatrixFile.Visible = true;
                    csButton.Visible = true;
                    csCheckBox.Visible = true;
                    csDCheckbox.Visible = true;
                    csMinLabel.Visible = true;
                    csMinBox.Visible = true;
                    csMaxLabel.Visible = true;
                    csMaxBox.Visible = true;


                    // Reliability Matrix
                    label24.Visible = true;
                    reliabilityMatrixFile.Visible = true;
                    csButton.Visible = true;
                    reliabilityCheckBox.Visible = true;
                    reliabilityDCheckBox.Visible = true;
                    reliabilityMinLabel.Visible = true;
                    reliabilityMinBox.Visible = true;
                    reliabilityMaxLabel.Visible = true;
                    reliabilityMaxBox.Visible = true;

                    // Expected Alliance Output File
                    label25.Visible = true;
                    expectedAllianceFile.Visible = true;
                    openExpectedAlliance.Visible = true;
                    expectedAllianceDCheckBox.Visible = true;
                }
            }
        }

        public NetworkFormationSimulationForm(NetworkGUI.MainForm p)
        {
            parent = p;
            InitializeComponent();

            _status = Status.Ready;
        }

        private void openCababilityFile_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(capabilityMatrixFile, capabilitiesCheckBox.Checked);
        }

        private void openContiguityFile_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(contiguityMatrixFile, contiguityCheckBox.Checked);
        }

        private void openMajorPowersFile_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(majorPowersVectorFile, majorPowersCheckBox.Checked);
        }

        private void openConflictFile_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(conflictMatrixFile, conflictCheckBox.Checked);
        }

        private void openSrgFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                srgMatrixFile.Text = openFileDialog.FileName;

            CheckFiles();
            if (srgMatrixFile.Text != "")
            {
                saveSrgFile.Enabled = true;
                srgSaveMatrixFile.Enabled = true;
            }
            else
            { 
                saveSrgFile.Enabled = true;
                srgSaveMatrixFile.Enabled = true;
            }
        }

        private void midButton_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(midMatrixFile, midCheckBox.Checked);
        }

        private void csButton_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(csMatrixFile, csCheckBox.Checked);
        }

        private void reliabilityButton_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(reliabilityMatrixFile, reliabilityCheckBox.Checked);
        }

        private void LoadFilenameIntoTextBox(TextBox textbox, bool random)
        {
            if (random && saveFileDialog.ShowDialog() == DialogResult.OK)
                textbox.Text = saveFileDialog.FileName;
            else if (!random && openFileDialog.ShowDialog() == DialogResult.OK)
                textbox.Text = openFileDialog.FileName;

            CheckFiles();
        }

        private bool CheckFile(string filename, bool isChecked)
        {
            if (!isChecked)
                return !string.IsNullOrEmpty(filename) && File.Exists(filename);

            return true;
        }

        private bool CheckFile(string filename, bool isChecked, bool isVisible)
        {
            if (!isChecked && isVisible)
                return !string.IsNullOrEmpty(filename) && File.Exists(filename);

            return true;
        }

        private void runButton_Click(object sender, EventArgs e)
        { 
            if (!(srgminbox.Text == "" && srgmaxbox.Text == "") && srgCheckBox.Checked && (srgminbox.Text == "" || srgmaxbox.Text =="" || double.Parse(srgminbox.Text) > double.Parse(srgmaxbox.Text) || double.Parse(srgminbox.Text) < 0 || double.Parse(srgmaxbox.Text) > 1) ) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(midminbox.Text == "" && midmaxbox.Text == "") && midCheckBox.Checked && (midminbox.Text == "" || midmaxbox.Text == "" || double.Parse(midminbox.Text) > double.Parse(midmaxbox.Text) || double.Parse(midminbox.Text) < 0 || double.Parse(midmaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(demominbox.Text == "" && demomaxbox.Text == "") && demoCheckBox.Checked && (demominbox.Text == "" || demomaxbox.Text == "" || double.Parse(demominbox.Text) > double.Parse(demomaxbox.Text) || double.Parse(demominbox.Text) < 0 || double.Parse(demomaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(contminbox.Text == "" && contmaxbox.Text == "") && contiguityCheckBox.Checked && (contminbox.Text == "" || contmaxbox.Text == "" || double.Parse(contminbox.Text) > double.Parse(contmaxbox.Text) || double.Parse(contminbox.Text) < 0 || double.Parse(contmaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(confminbox.Text == "" && confmaxbox.Text == "") && conflictCheckBox.Checked && (confminbox.Text == "" || confmaxbox.Text == "" || double.Parse(confminbox.Text) > double.Parse(confmaxbox.Text) || double.Parse(confminbox.Text) < 0 || double.Parse(confmaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(majminbox.Text == "" && majmaxbox.Text == "") && majorPowersCheckBox.Checked && (majminbox.Text == "" || majmaxbox.Text == "" || double.Parse(majminbox.Text) > double.Parse(majmaxbox.Text) || double.Parse(majminbox.Text) < 0 || double.Parse(majmaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }           
            if (!(jcminbox.Text == "" && jcmaxbox.Text == "") && jcCheckBox.Checked && (jcminbox.Text == "" || jcmaxbox.Text == "" || double.Parse(jcminbox.Text) > double.Parse(jcmaxbox.Text) || double.Parse(jcminbox.Text) < 0 || double.Parse(jcmaxbox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(csMinBox.Text == "" && csMaxBox.Text == "") && csCheckBox.Checked && (csMinBox.Text == "" || csMaxBox.Text == "" || double.Parse(csMinBox.Text) > double.Parse(csMaxBox.Text) || double.Parse(csMinBox.Text) < 0 || double.Parse(csMaxBox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }
            if (!(reliabilityMinBox.Text == "" && reliabilityMaxBox.Text == "") && reliabilityCheckBox.Checked && (reliabilityMinBox.Text == "" || reliabilityMaxBox.Text == "" || double.Parse(reliabilityMinBox.Text) > double.Parse(reliabilityMaxBox.Text) || double.Parse(reliabilityMinBox.Text) < 0 || double.Parse(reliabilityMaxBox.Text) > 1)) { MessageBox.Show("Please enter appropriate probability range or leave both boxes blank!", "Error!"); return; }


            if (intNbox.Enabled)
            {
                try
                {
                    if (int.Parse(intNbox.Text) < 1)
                    {
                        MessageBox.Show("Please enter appropriate interval value");
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please enter appropriate interval value", "Error!");
                    return;
                }
            }
            if (_status == Status.Ready)
            {
                _status = Status.Running;
                runButton.Enabled = false;
                runButton.Text = "Running";
                closeButton.Text = "Cancel";
                progressBar.Value = 0;
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{ 
                int N = int.Parse(nTextBox.Text);
                //int numberOfIterations = int.Parse(nTextBox.Text);
                int iterations = int.Parse(tMaxTextBox.Text);
                int maxIterations;
                if(maxStageTextBox.Text=="")
                    maxIterations = int.MaxValue;
                else 
                    maxIterations = int.Parse(maxStageTextBox.Text);
                bool rangeN = multipleNCheckBox.Checked && multipleNCheckBox.Enabled;
                int rangeNmin = int.Parse(minNbox.Text);
                int rangeNmax = int.Parse(maxNbox.Text);
                int rangeNint = int.Parse(intNbox.Text);

                bool useSrg = CheckFile(srgMatrixFile.Text, srgCheckBox.Checked);
                MatrixProvider capabilProvider = new MatrixProvider(capabilityMatrixFile.Text,
                            capabilitiesCheckBox.Checked ? capabilityMatrixFile.Text : null,
                            capabilitiesCheckBox.Checked ? MatrixProvider.Type.RandomDiagonal : MatrixProvider.Type.MatrixFile,
                            N,
                            N, 
                            false);
                /*
                MatrixProvider contigProvider = new MatrixProvider(contiguityMatrixFile.Text,
                            contiguityCheckBox.Checked ? contiguityMatrixFile.Text : null,
                            !useSrg ? MatrixProvider.Type.NullFile :
                            (contiguityCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile),
                            N,
                            N,
                            contiguityDCheckBox.Checked);
                */
                
                MatrixProvider contigProvider = new MatrixProvider(contiguityMatrixFile.Text,
                            contiguityCheckBox.Checked ? contiguityMatrixFile.Text : null,
                            contiguityMatrixFile.Visible ? 
                            (contiguityCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            contiguityDCheckBox.Checked);
                
                /*
                MatrixProvider contigProvider = new MatrixProvider(contiguityMatrixFile.Text,
                                contiguityCheckBox.Checked ? contiguityMatrixFile.Text : null,
                                contiguityMatrixFile.Text == "" ? (contiguityCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.NullFile) :
                                MatrixProvider.Type.MatrixFile,
                                N,
                                N,
                                contiguityDCheckBox.Checked);
                */
                MatrixProvider majPowProvider = new MatrixProvider(majorPowersVectorFile.Text,
                            majorPowersCheckBox.Checked ? majorPowersVectorFile.Text : null,
                            useSrg ? MatrixProvider.Type.NullFile :
                            (majorPowersCheckBox.Checked ? MatrixProvider.Type.RandomVector : MatrixProvider.Type.VectorFile),
                            N,
                            N,
                            majorPowersDCheckBox.Checked);
                MatrixProvider conflictProvider = new MatrixProvider(conflictMatrixFile.Text,
                            null, //conflictCheckBox.Checked ? conflictMatrixFile.Text : null,
                            conflictCheckBox.Visible ? 
                            (conflictCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            conflictDCheckBox.Checked);
                MatrixProvider srgProvider = new MatrixProvider(srgMatrixFile.Text,
                            srgCheckBox.Checked ? srgMatrixFile.Text : null,
                            useSrg ?
                            (srgCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            srgDCheckBox.Checked);
                MatrixProvider demoProvider = new MatrixProvider(demoMatrixFile.Text,
                            demoCheckBox.Checked ? demoMatrixFile.Text : null,
                            demoCheckBox.Visible ?
                            (demoCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            demoDCheckBox.Checked);
                MatrixProvider jcProvider = new MatrixProvider(jcMatrixFile.Text,
                            jcCheckBox.Checked ? jcMatrixFile.Text : null,
                            jcCheckBox.Visible ?
                            (jcCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            jcDCheckBox.Checked);
                MatrixProvider midProvider = new MatrixProvider(midMatrixFile.Text,
                            midCheckBox.Checked ? midMatrixFile.Text : null,
                            midCheckBox.Visible ?
                            (midCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            midDCheckBox.Checked);
                MatrixProvider csProvider = new MatrixProvider(csMatrixFile.Text,
                            csCheckBox.Checked ? csMatrixFile.Text : null,
                            csCheckBox.Visible ?
                            (csCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            csDCheckbox.Checked);
                MatrixProvider relProvider = new MatrixProvider(reliabilityMatrixFile.Text,
                            reliabilityCheckBox.Checked ? reliabilityMatrixFile.Text : null,
                            reliabilityCheckBox.Visible ?
                            (reliabilityCheckBox.Checked ? MatrixProvider.Type.RandomSymmetric : MatrixProvider.Type.MatrixFile) : MatrixProvider.Type.NullFile,
                            N,
                            N,
                            reliabilityDCheckBox.Checked);

                //if (demoCheckBox.Checked)
                //{
                //    demoProvider.Min = 0.15;
                //    demoProvider.Max = 0.35;
                //}
                //if (jcCheckBox.Checked)
                //{
                //    jcProvider.Min = 0.3;
                //    jcProvider.Max = 0.6;
                //}

                if (demoCheckBox.Checked)
                {
                    if (demominbox.Text == "" && demomaxbox.Text == "")
                        demoProvider.Prange = false;
                    else
                    {
                        demoProvider.Prange = true;
                        demoProvider.Pmin = double.Parse(demominbox.Text);
                        demoProvider.Pmax = double.Parse(demomaxbox.Text);
                    }
                }
                if (srgCheckBox.Checked)
                {
                    if (srgminbox.Text == "" && srgmaxbox.Text == "")
                        srgProvider.Prange = false;
                    else
                    {
                        srgProvider.Prange = true;
                        srgProvider.Pmin = double.Parse(srgminbox.Text);
                        srgProvider.Pmax = double.Parse(srgmaxbox.Text);
                    }
                }
                if (jcCheckBox.Checked)
                {
                    if (jcminbox.Text == "" && jcmaxbox.Text == "")
                        jcProvider.Prange = false;
                    else
                    {
                        jcProvider.Prange = true;
                        jcProvider.Pmin = double.Parse(jcminbox.Text);
                        jcProvider.Pmax = double.Parse(jcmaxbox.Text);
                    }
                }
                if (midCheckBox.Checked)
                {
                    if (midminbox.Text == "" && midmaxbox.Text == "")
                        midProvider.Prange = false;
                    else
                    {
                        midProvider.Prange = true;
                        midProvider.Pmin = double.Parse(midminbox.Text);
                        midProvider.Pmax = double.Parse(midmaxbox.Text);
                    }
                }
                if (conflictCheckBox.Checked)
                {
                    if (confminbox.Text == "" && confmaxbox.Text == "")
                        conflictProvider.Prange = false;
                    else
                    {
                        conflictProvider.Prange = true;
                        conflictProvider.Pmin = double.Parse(confminbox.Text);
                        conflictProvider.Pmax = double.Parse(confmaxbox.Text);
                    }
                }
                if (majorPowersCheckBox.Checked)
                {
                    if (majminbox.Text == "" && majmaxbox.Text == "")
                        majPowProvider.Prange = false;
                    else
                    {
                        majPowProvider.Prange = true;
                        majPowProvider.Pmin = double.Parse(majminbox.Text);
                        majPowProvider.Pmax = double.Parse(majmaxbox.Text);
                    }
                }
                if (contiguityCheckBox.Checked)
                {
                    if (contminbox.Text == "" && contmaxbox.Text == "")
                        contigProvider.Prange = false;
                    else
                    {
                        contigProvider.Prange = true;
                        contigProvider.Pmin = double.Parse(contminbox.Text);
                        contigProvider.Pmax = double.Parse(contmaxbox.Text);
                    }
                }
                if (csCheckBox.Checked)
                {
                    if (csMinBox.Text == "" && csMaxBox.Text == "")
                        csProvider.Prange = false;
                    else
                    {
                        csProvider.Prange = true;
                        csProvider.Pmin = double.Parse(csMinBox.Text);
                        csProvider.Pmax = double.Parse(csMaxBox.Text);
                    }
                }
                if (reliabilityCheckBox.Checked)
                {
                    if (reliabilityMinBox.Text == "" && reliabilityMaxBox.Text == "")
                        relProvider.Prange = false;
                    else
                    {
                        relProvider.Prange = true;
                        relProvider.Pmin = double.Parse(reliabilityMinBox.Text);
                        relProvider.Pmax = double.Parse(reliabilityMaxBox.Text);
                    }
                }

                capabilProvider.ForceVector = capabilitiesVectorCheckBox.Checked;
                capabilProvider.WriteRepeatCount = 3;

                if (File.Exists(conflictMatrixFile.Text) && _overwrite && conflictCheckBox.Checked)
                    File.Delete(conflictMatrixFile.Text);

                if (File.Exists(outputFile.Text) && _overwrite)
                    File.Delete(outputFile.Text);


                if (outputDyadicCheckBox.Checked) MatrixWriter.WriteDyadicHeader(outputFile.Text);

                bool overwrite = parent.GetSaveOverwrite();

                if (!rangeN)
                {
                    for (int i = 1; i <= iterations; ++i) // changed from iterations to N
                    {
                        if (!capabilitiesCheckBox.Checked) N = capabilProvider.GetNextMatrixRows();
                        else if (!useSrg && !contiguityCheckBox.Checked) N = contigProvider.GetNextMatrixRows();
                        else if (!useSrg && !majorPowersCheckBox.Checked) N = majPowProvider.GetNextMatrixRows();
                        else if (!conflictCheckBox.Checked) N = conflictProvider.GetNextMatrixRows();
                        else if (useSrg && !srgCheckBox.Checked) N = srgProvider.GetNextMatrixRows();
                        else if (!demoCheckBox.Checked) N = demoProvider.GetNextMatrixRows();
                        else if (!jcCheckBox.Checked) N = jcProvider.GetNextMatrixRows();
                        else if (!midCheckBox.Checked) N = midProvider.GetNextMatrixRows();
                        else if (!csCheckBox.Checked) N = csProvider.GetNextMatrixRows();
                        else if (!reliabilityCheckBox.Checked) N = relProvider.GetNextMatrixRows();

                        if (N == -1)
                            N = int.Parse(nTextBox.Text);

                        if (capabilitiesCheckBox.Checked) capabilProvider.Rows = capabilProvider.Cols = N;
                        if (!useSrg && contiguityCheckBox.Checked) contigProvider.Rows = contigProvider.Cols = N;
                        if (!useSrg && majorPowersCheckBox.Checked) majPowProvider.Rows = majPowProvider.Cols = N;
                        if (conflictCheckBox.Checked) conflictProvider.Rows = conflictProvider.Cols = N;
                        if (useSrg && srgCheckBox.Checked) srgProvider.Rows = srgProvider.Cols = N;
                        if (demoCheckBox.Checked) demoProvider.Rows = demoProvider.Cols = N;
                        if (jcCheckBox.Checked) jcProvider.Rows = jcProvider.Cols = N;
                        if (midCheckBox.Checked) midProvider.Rows = midProvider.Cols = N;
                        if (csCheckBox.Checked) csProvider.Rows = csProvider.Cols = N;
                        if (reliabilityCheckBox.Checked) relProvider.Rows = relProvider.Cols = N;




                        NetworkFormationSimulation.SimulateToFile(
                            capabilProvider,
                            contigProvider,
                            majPowProvider,
                            conflictProvider,
                            outputFile.Text,
                            expectedAllianceFile.Text,
                            conflictMatrixFile.Text,
                            srgSaveMatrixFile.Text,
                            i,
                            conflictCheckBox.Checked,
                            contiguityCheckBox.Checked,
                            majorPowersCheckBox.Checked,
                            outputDyadicCheckBox.Checked,
                            expectedAllianceDCheckBox.Checked,
                            srgDCheckBox.Checked,
                            srgProvider,
                            demoProvider,
                            jcProvider,
                            midProvider,
                            csProvider,
                            relProvider,
                            maxIterations,
                            overwrite,
                            double.Parse(brTextBox.Text));
                        overwrite = false;
                        (sender as BackgroundWorker).ReportProgress((int)(0.5 + Math.Ceiling(100.0 * i / (double)iterations)));
                        if (e.Cancel)
                            break;
                    }
                }
                else
                { 
                    for (int i = 1; i <= iterations; ++i) // changed from iterations to N
                    {
                        N = RNG.RandomInt((rangeNmax - rangeNmin) / rangeNint) * rangeNint + rangeNmin; 
                        if (capabilitiesCheckBox.Checked) capabilProvider.Rows = capabilProvider.Cols = N;
                        if (useSrg && contiguityCheckBox.Checked) contigProvider.Rows = contigProvider.Cols = N; // usrSrg is now True as of 8/31/2012
                        if (!useSrg && majorPowersCheckBox.Checked) majPowProvider.Rows = majPowProvider.Cols = N;
                        if (conflictCheckBox.Checked) conflictProvider.Rows = conflictProvider.Cols = N;
                        if (useSrg && srgCheckBox.Checked) srgProvider.Rows = srgProvider.Cols = N;
                        if (demoCheckBox.Checked) demoProvider.Rows = demoProvider.Cols = N;
                        if (jcCheckBox.Checked) jcProvider.Rows = jcProvider.Cols = N;
                        if (midCheckBox.Checked) midProvider.Rows = midProvider.Cols = N;
                        if (csCheckBox.Checked) csProvider.Rows = csProvider.Cols = N;
                        if (reliabilityCheckBox.Checked) relProvider.Rows = relProvider.Cols = N;


                        NetworkFormationSimulation.SimulateToFile(
                            capabilProvider,
                            contigProvider,
                            majPowProvider,
                            conflictProvider,
                            outputFile.Text,
                            expectedAllianceFile.Text,
                            conflictMatrixFile.Text,
                            srgSaveMatrixFile.Text,
                            i,
                            conflictCheckBox.Checked,
                            contiguityCheckBox.Checked,
                            majorPowersCheckBox.Checked,
                            outputDyadicCheckBox.Checked,
                            expectedAllianceDCheckBox.Checked,
                            srgDCheckBox.Checked,
                            srgProvider,
                            demoProvider,
                            jcProvider,
                            midProvider,
                            csProvider,
                            relProvider,
                            maxIterations,
                            overwrite,
                            double.Parse(brTextBox.Text));
                        overwrite = false;
                        (sender as BackgroundWorker).ReportProgress((int)(0.5 + Math.Ceiling(100.0 * i / (double)iterations)));
                        if (e.Cancel)
                            break;
                    }
                

                }
            //catch (Exception exception)
            {
             //   MessageBox.Show("Error: " + exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _status = Status.Ready;
            runButton.Text = "Run";
            closeButton.Text = "Close";
            runButton.Enabled = true;
            closeButton.Enabled = true;
        }

        private void openOutputFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                outputFile.Text = saveFileDialog.FileName;
            }
        }

        private void openExpectedAlliance_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                expectedAllianceFile.Text = saveFileDialog.FileName;
            }
        }

        private void capabilityMatrixFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void CheckAllRandom()
        {
            multipleNCheckBox.Enabled = ((capabilitiesCheckBox.Checked || !capabilitiesCheckBox.Visible) && (srgCheckBox.Checked || !srgCheckBox.Visible) && (contiguityCheckBox.Checked || !contiguityCheckBox.Visible) &&
                          (majorPowersCheckBox.Checked || !majorPowersCheckBox.Visible) && (conflictCheckBox.Checked || !conflictCheckBox.Visible) && (demoCheckBox.Checked || !demoCheckBox.Visible) &&
                          (jcCheckBox.Checked || !jcCheckBox.Visible) && (midCheckBox.Checked || !midCheckBox.Visible) && (csCheckBox.Checked || !csCheckBox.Visible) && (reliabilityCheckBox.Checked || !reliabilityCheckBox.Visible));

            minNbox.Enabled = maxNbox.Enabled = intNbox.Enabled = (multipleNCheckBox.Checked && multipleNCheckBox.Enabled);

            nTextBox.Enabled = !minNbox.Enabled;
        }

        private void CheckFiles()
        {
            int tmp;
            if (CheckFile(capabilityMatrixFile.Text, capabilitiesCheckBox.Checked)
                && (CheckFile(srgMatrixFile.Text, srgCheckBox.Checked)
                || (CheckFile(contiguityMatrixFile.Text, contiguityCheckBox.Checked) && CheckFile(majorPowersVectorFile.Text, majorPowersCheckBox.Checked)))
                && CheckFile(conflictMatrixFile.Text, conflictCheckBox.Checked, conflictMatrixFile.Visible)
                && CheckFile(demoMatrixFile.Text, demoCheckBox.Checked, demoMatrixFile.Visible)
                && CheckFile(jcMatrixFile.Text, jcCheckBox.Checked, jcMatrixFile.Visible)
                && CheckFile(midMatrixFile.Text, midCheckBox.Checked, midMatrixFile.Visible)
                && CheckFile(csMatrixFile.Text, csCheckBox.Checked, csMatrixFile.Visible)
                && CheckFile(reliabilityMatrixFile.Text, reliabilityCheckBox.Checked, reliabilityMatrixFile.Visible)
                && !string.IsNullOrEmpty(outputFile.Text)
                && int.TryParse(tMaxTextBox.Text, out tmp)
                && int.TryParse(nTextBox.Text, out tmp))
            {
                runButton.Enabled = true;

            }
            else
                runButton.Enabled = false;
        }

        private void contiguityMatrixFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void majorPowersVectorFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void conflictMatrixFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void outputFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void midMatrixFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (_status == Status.Ready)
            {
                Close();
            }
            else if (_status == Status.Running)
            {
                _status = Status.Cancelled;
                runButton.Text = "Cancelling";
                closeButton.Enabled = false;
                backgroundWorker.CancelAsync();
            }
            progressBar.Value = 0;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void tMaxTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void nTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void capabilitiesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (capabilitiesCheckBox.Checked)
                openCababilityFile.Text = "Save File...";
            else
                openCababilityFile.Text = "Open File...";
 
            //capabilityMatrixFile.Text = string.Empty;
            capabilityMatrixFile.Text = "";

            CheckFiles();
            CheckAllRandom();
        }

        private void contiguityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (contiguityCheckBox.Checked)
               openContiguityFile.Text = "Save File...";
            else
               openContiguityFile.Text = "Open File...";

            contiguityDCheckBox.Enabled = contminbox.Enabled = contmaxbox.Enabled = contiguityCheckBox.Checked;
           //contiguityMatrixFile.Text = string.Empty;
           contiguityMatrixFile.Text = "";

           CheckFiles();
           CheckAllRandom();
        }

        private void majorPowersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (majorPowersCheckBox.Checked)
                openMajorPowersFile.Text = "Save File...";
            else
                openMajorPowersFile.Text = "Open File...";

            majorPowersDCheckBox.Enabled = majminbox.Enabled = majmaxbox.Enabled = majorPowersCheckBox.Checked;
            majorPowersVectorFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void conflictCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (conflictCheckBox.Checked)
                openConflictFile.Text = "Save File...";
            else
                openConflictFile.Text = "Open File...";

            conflictDCheckBox.Enabled = confminbox.Enabled = confmaxbox.Enabled = conflictCheckBox.Checked;
            conflictMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void demoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (demoCheckBox.Checked)
                openDemoFile.Text = "Save File...";
            else
                openDemoFile.Text = "Open File...";

            demoDCheckBox.Enabled = demominbox.Enabled = demomaxbox.Enabled = demoCheckBox.Checked;
            demoMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(jcMatrixFile, jcCheckBox.Checked);
        }

        private void jcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (jcCheckBox.Checked)
                jcButton.Text = "Save File...";
            else
                jcButton.Text = "Open File...";

            jcDCheckBox.Enabled = jcminbox.Enabled = jcmaxbox.Enabled = jcCheckBox.Checked;
            jcMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void midCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (midCheckBox.Checked)
                midButton.Text = "Save File...";
            else
                midButton.Text = "Open File...";

            midDCheckBox.Enabled = midminbox.Enabled = midmaxbox.Enabled = midCheckBox.Checked;
            midMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        } 

        private void srgCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (srgCheckBox.Checked || srgMatrixFile.Text != "")
            {
                srgSaveMatrixFile.Enabled = true;
                saveSrgFile.Enabled = true;
            }
            else
            {
                saveSrgFile.Enabled = false;
                srgSaveMatrixFile.Enabled = false;
            }

            srgDCheckBox.Enabled = srgminbox.Enabled = srgmaxbox.Enabled = srgCheckBox.Checked;
            srgMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void saveSrgFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                srgSaveMatrixFile.Text = saveFileDialog.FileName;   
        } 

        private void multipleNCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            minNbox.Enabled = maxNbox.Enabled = intNbox.Enabled = multipleNCheckBox.Checked;
            nTextBox.Enabled = !multipleNCheckBox.Checked;
        }

        private void demoMatrixFile_TextChanged(object sender, EventArgs e)
        {
            CheckFiles();
        }

        private void maxStageTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openDemoFile_Click(object sender, EventArgs e)
        {
            LoadFilenameIntoTextBox(demoMatrixFile, demoCheckBox.Checked);
        }

        private void outputDyadicCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void csCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (csCheckBox.Checked)
                csButton.Text = "Save File...";
            else
                csButton.Text = "Open File...";

            csDCheckbox.Enabled = csMinBox.Enabled = csMaxBox.Enabled = csCheckBox.Checked;
            csMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        private void reliabilityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (reliabilityCheckBox.Checked)
                reliabilityButton.Text = "Save File...";
            else
                reliabilityButton.Text = "Open File...";

            reliabilityDCheckBox.Enabled = reliabilityMinBox.Enabled = reliabilityMaxBox.Enabled = reliabilityCheckBox.Checked;
            reliabilityMatrixFile.Text = string.Empty;

            CheckFiles();
            CheckAllRandom();
        }

        
    }
}