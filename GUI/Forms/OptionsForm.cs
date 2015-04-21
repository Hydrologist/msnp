using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network.Matrices;
using Network.IO;
using Network;

namespace NetworkGUI
{
    public partial class OptionsForm : Form
    {
        private VectorValueProvider _cutoffVector;

        public OptionsForm()
        {
            InitializeComponent();
            EnableButton();
            filenamelabel.Text = "";
            d = 1.0;
            v = 1.0;
            r = 0;
            m = 1;
            k = 1;
            a = 0.0;
            net = null;

            UpdateCheckBoxes();
        }

        public Network.NetworkGUI net;
        string CohesionFileName;
        string ShortCohesionFileName;
        double d, v, a;
        int r,m, k;

        public string FileName
        {
            get { return openFileDialog.FileName; }
        }

        public string InputType
        {
            get
            {
                if (inputTypeDyadic.Checked)
                    return "Dyadic";
                else if (inputTypeMatrix.Checked)
                    return "Matrix";
                else if (inputTypeStructEquiv.Checked)
                    return "StructEquiv";
                else
                    return "None";
            }
        }

        public string DisplayCliqueOption
        {
            get
            {
                if (display500cliquesbutton.Checked)
                    return "Display500";
                else if (displayallcliquesbutton.Checked)
                    return "DisplayAll";
                else
                    return "Save";
            }
        }

        public string ERPOLType
        {
            get
            {
                if (erpoltypeCohesion.Checked)
                    return "Cohesion";
                else
                    return "Alpha";
            }
        }

        public bool SaveOverwrite
        {
            get { return saveOverwrite.Checked; }
        }

        public string SumMean
        {
            get
            {
                if (noneButton.Checked)
                    return "None";
                else if (dvcMeanButton.Checked)
                    return "DVCMean";
                else if (dvcSumButton.Checked)
                    return "DVCSum";
                else if (svcMeanButton.Checked)
                    return "SVCMean";
                else if (svcSumButton.Checked)
                    return "SVCSum";
                else
                    return "None";
            }
            set
            {
                if (value == "None") noneButton.Checked = true;
                else if (value == "DVSMean") dvcMeanButton.Checked = true;
                else if (value == "DVCSum") dvcSumButton.Checked = true;
                else if (value == "SVCMean") svcMeanButton.Checked = true;
                else if (value == "SVCSum") svcSumButton.Checked = true;
            }
        }

        public string SumMeanFilename
        {
            get { return sumMeanFileDialog.FileName; }
            set { sumMeanFileDialog.FileName = value; }
        }

        public double Density
        {
            get
            {
                if (calcAutoDensity.Checked)
                    return -2.0;
                else
                    return d;
            }
        }
        public VectorValueProvider Cutoff
        {
            get
            {
                if (_cutoffVector == null)
                {
                    if (binaryCutoff.Text == Constants.FileSelected)
                        _cutoffVector = new VectorValueProvider(cutoffFileDialog.FileName);
                    else
                        _cutoffVector = new VectorValueProvider(double.Parse(binaryCutoff.Text));
                }
 
                return _cutoffVector;
            }
            set
            {
                _cutoffVector = value; 
            }
        }
        public double CutoffValue
        {
            get { return (binaryCutoff.Text == Constants.FileSelected? -1.0 : double.Parse(binaryCutoff.Text)); }
            set { binaryCutoff.Text = (value == -1.0 ? Constants.FileSelected : value.ToString() ); }
        }

        public double Alpha
        {
            get { return a; }
            set { a = value; alphaValue.Text = value.ToString(); }
        }

        public double ViableCoalitionCutoff
        {
            get { return v; }
        }

        public int ReachNumMatrices
        {
            get { return r; }
            set { r = value; reachNumMatrices.Text = value.ToString();  }
        }

        public int CMinMembers
        {
            get { return m; }
            set { m = value; cMinMembers.Text = value.ToString(); }
        }

        public int KCliqueValue
        {
            get { return k; }
            set { k = value; kCliqueValue.Text = value.ToString(); }
        }

        public string svcFile
        {
            get
            {
                if (!NPOLACheckBox.Checked)
                    return null;
                else
                    return svcFileDialog.FileName;
            }
        }

        public string svcCoalitionFile
        {
            get { return svcFileDialog.FileName; }
        }

        public bool useCohesion
        {
            get
            {
                return NPOLACohesion.Checked;
            }
        }

        public string densityFile
        {
            get
            {
                return densityFileDialog.FileName;
            }
        }

        public string viableCoalitionFile
        {
            get
            {
                return viableCoalitionFileDialog.FileName;
            }
        }

        public string reachFile
        {
            get
            {
                return reachFileDialog.FileName;
            }
        }

        public string cMinMembersFile
        {
            get
            {
                return cMinMembersFileDialog.FileName;
            }
        }

        public bool KCliqueDiag
        {
            get { return kCliqueDiagCheckBox.Checked; }
        }

        public string KCliqueFileName
        {
            get { return kCliqueFileDialog.FileName; }
        }

        public bool reachSum
        {
            get { return sumBox.Checked; }
        }
        public bool reachZero
        {
            get { return zeroDiagonal.Checked; }
        }

        private void EnableButton()
        {
            goButton.Enabled = false;
            if (inputTypeNone.Checked || inputTypeStructEquiv.Checked)
                goButton.Enabled = true;
            else if (openFileDialog.FileName != "")
                goButton.Enabled = true;

            if (NPOLACheckBox.Checked && svcFileDialog.FileName == "")
                goButton.Enabled = false;

            if (noneButton.Checked == false && sumMeanFileDialog.FileName == "")
                goButton.Enabled = false;
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            EnableButton();
        }

        public CliqueExtractionType CETType
        {
            get
            {
                if (maxExtraction.Checked)
                    return Network.CliqueExtractionType.Max;
                else if (minExtraction.Checked)
                    return Network.CliqueExtractionType.Min;
                else if (upperExtraction.Checked)
                    return Network.CliqueExtractionType.Upper;
                else //if (lowerExtraction.Checked)
                    return Network.CliqueExtractionType.Lower;
            }
            set
            {
                maxExtraction.Checked = (value == CliqueExtractionType.Max ? true : false);
                minExtraction.Checked = (value == CliqueExtractionType.Min ? true : false);
                upperExtraction.Checked = (value == CliqueExtractionType.Upper ? true : false);
                lowerExtraction.Checked = (value == CliqueExtractionType.Lower ? true : false);
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (maxExtraction.Checked)
                net.cet = Network.CliqueExtractionType.Max;
            else if (minExtraction.Checked)
                net.cet = Network.CliqueExtractionType.Min;
            else if (upperExtraction.Checked)
                net.cet = Network.CliqueExtractionType.Upper;
            else if (lowerExtraction.Checked)
                net.cet = Network.CliqueExtractionType.Lower;

            double tmp;
            if ((binaryCutoff.Text != Constants.FileSelected && !double.TryParse(binaryCutoff.Text, out tmp)
                || (densityMax.Text != Constants.FileSelected && !double.TryParse(densityMax.Text, out d)))
                || (reachNumMatrices.Text != Constants.FileSelected && !int.TryParse(reachNumMatrices.Text, out r))
                || (viableCoalitionValue.Text != Constants.FileSelected && !double.TryParse(viableCoalitionValue.Text, out v))
                || (cMinMembers.Text!= Constants.FileSelected && !int.TryParse(cMinMembers.Text,out m))
                || (kCliqueValue.Text != Constants.FileSelected && !int.TryParse(kCliqueValue.Text, out k))
                ||(!double.TryParse(alphaValue.Text, out a)))
            {
                MessageBox.Show("You have not entered proper real numbers!", "Error!");
            }
            else
            {
                if (densityMax.Text == Constants.FileSelected)
                    d = -1.0;
                if (reachNumMatrices.Text == Constants.FileSelected)
                    r = -1;
                if (viableCoalitionValue.Text == Constants.FileSelected)
                    v = -1;
                if (cMinMembers.Text == Constants.FileSelected)
                    m = -1;
                if (kCliqueValue.Text == Constants.FileSelected)
                    k = -1;
                this.Close();
            }
        }

        private void inputTypeDyadic_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void inputTypeStructEquiv_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        public MatrixComputations.TransitivityType transitivityType
        {
            get
            {
                if (simpleTransitivity.Checked)
                    return MatrixComputations.TransitivityType.Simple;
                else if (weakTransitivity.Checked)
                    return MatrixComputations.TransitivityType.Weak;
                else
                    return MatrixComputations.TransitivityType.Strong;
            }
        }

        private bool TCB(CheckBox cb)
        {
            return cb.Enabled && cb.Checked;
        }

        public bool[] counterOptions
        {
            get
            {
                bool[] options = new bool[35];
                options[0] = TCB(netIdentCheck);
                options[1] = TCB(nCheck);
                options[2] = TCB(noClqCheck);
                options[3] = TCB(csCheck);
                options[4] = TCB(cmCheck);
                options[5] = TCB(nocomCheck);
                options[6] = TCB(gnCheck);
                options[7] = TCB(npolCheck);
                options[8] = TCB(cmoiCheck);
                options[9] = TCB(npiCheck);
                options[10] = TCB(clqOverlpCheck);
                options[11] = TCB(complexClqOverlpCheck);
                options[12] = TCB(npolStarCheck);
                options[13] = TCB(npiStarCheck);
                options[14] = TCB(npiOneStarCheck);
                options[15] = TCB(npolSizeCheck);
                options[16] = TCB(npolCohesionSize);
                options[17] = TCB(npiTwoStarCheck);
                options[18] = TCB(npiThreeStarCheck);
                options[19] = TCB(depCheck);
                options[20] = TCB(sysdepCheck);
                options[21] = TCB(densityCheck);
                options[22] = TCB(transCheck);
                options[23] = TCB(sysinCheck);
                options[24] = TCB(erpolCheck);
                options[25] = TCB(avgdiCheck);
                options[26] = TCB(avgdoCheck);
                options[27] = TCB(digCheck);
                options[28] = TCB(dogCheck);
                options[29] = TCB(cigCheck);
                options[30] = TCB(cogCheck);
                options[31] = TCB(bigCheck);
                options[32] = TCB(bogCheck);
                options[33] = TCB(eigCheck);
                options[34] = TCB(eogCheck);
       
                return options;
            }
        }

        private void inputTypeNone_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void inputTypeMatrix_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void NPOLACheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void svcButton_Click(object sender, EventArgs e)
        {
            svcFileDialog.ShowDialog();
            EnableButton();
        }

        private void CounterForm_Load(object sender, EventArgs e)
        {

        }

        private void NPOLACohesion_CheckedChanged(object sender, EventArgs e)
        {
            NPOLACheckBox.Checked = true;
            EnableButton();
        }

        private void sumMeanFileSelectButton_Click(object sender, EventArgs e)
        {
            sumMeanFileDialog.ShowDialog();
            EnableButton();
        }

        private void dvcSumButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void svcSumButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void noneButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void dvcMeanButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void svcMeanButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cutoffFileDialog.ShowDialog() == DialogResult.OK)
            {
                binaryCutoff.Text = Constants.FileSelected;
                _cutoffVector = null;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (densityFileDialog.ShowDialog() == DialogResult.OK)
                densityMax.Text = Constants.FileSelected;
        }

        private void npolStarCheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void inputTypeNone_CheckedChanged_1(object sender, EventArgs e)
        {
            fileSelectButton.Enabled = !inputTypeNone.Checked;
            UpdateCheckBoxes();
        }

        private void  SetCohesion(bool val)
        {
            npolStarCheck.Enabled = npiStarCheck.Enabled = npiOneStarCheck.Enabled = npolCohesionSize.Enabled = npiTwoStarCheck.Enabled = npiThreeStarCheck.Enabled = val;
        }
        private void SetSize(bool val)
        {
            npolSizeCheck.Enabled = npolCohesionSize.Enabled = npiTwoStarCheck.Enabled = npiThreeStarCheck.Enabled = val;
        }

        private void UpdateRadioButton()
        {
            bool erpoltype = erpolCheck.Checked;

            if (erpoltype)
            {
                groupBox11.Enabled = erpoltypeCohesion.Enabled = erpoltypeAlpha.Enabled = true;
            }
            else
            {
                groupBox11.Enabled = erpoltypeCohesion.Enabled = erpoltypeAlpha.Enabled = false;
            }


        }

        private void UpdateCheckBoxes()
        {
            bool cohesion = !inputTypeNone.Checked;
            bool size = NPOLACheckBox.Checked;

            if (cohesion && size)
            {
                SetCohesion(cohesion);
                SetSize(size);
            }
            else if (cohesion)
            {
                SetCohesion(cohesion);
                SetSize(size);
            }
            else
            {
                SetSize(size);
                SetCohesion(cohesion);
            }
            
        }

        private void NPOLACheckBox_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }

        private void NPOLACohesion_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }

        private void inputTypeStructEquiv_CheckedChanged_1(object sender, EventArgs e)
        {
            fileSelectButton.Enabled = !inputTypeStructEquiv.Checked;
            UpdateCheckBoxes();
        }

        private void inputTypeDyadic_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }

        private void inputTypeMatrix_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }

        private void fileSelectButton_Click_1(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            EnableButton();
            CohesionFileName = openFileDialog.FileName;
            string[] parts = CohesionFileName.Split('\\');
            ShortCohesionFileName = parts[parts.Length - 1];
            filenamelabel.Text = ShortCohesionFileName;
        }

        private void svcButton_Click_1(object sender, EventArgs e)
        {
            svcFileDialog.ShowDialog();
            EnableButton();
        }

        private void reachabilityUseFile_Click(object sender, EventArgs e)
        {
            if (reachFileDialog.ShowDialog() == DialogResult.OK)
                reachNumMatrices.Text = Constants.FileSelected;
        }

        private void densityMax_TextChanged(object sender, EventArgs e)
        {

        }

        private void densityFileButton_Click(object sender, EventArgs e)
        {
            if (densityFileDialog.ShowDialog() == DialogResult.OK)
            {
                densityMax.Text = Constants.FileSelected;
                calcAutoDensity.Checked = false;
            }

        }

        private void calcAutoDensity_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void useViableCoalitionFileButton_Click(object sender, EventArgs e)
        {
            if (viableCoalitionFileDialog.ShowDialog() == DialogResult.OK)
            {
                viableCoalitionValue.Text = Constants.FileSelected;
            }
        }

        private void binaryCutoff_TextChanged(object sender, EventArgs e)
        {
            _cutoffVector = null;
        }

        private void cMinMembersFileButton_Click(object sender, EventArgs e)
        {
            if (cMinMembersFileDialog.ShowDialog() == DialogResult.OK)
            {
                cMinMembers.Text = Constants.FileSelected;
            }

        } 

        private void erpolCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRadioButton();
        }

        private void erpoltypeAlpha_CheckedChanged(object sender, EventArgs e)
        {
            bool erpoltype = erpolCheck.Checked;
            bool alphatype = erpoltypeAlpha.Checked;

            if (erpoltype && alphatype)
            {
                alphaValue.Enabled = true;
            }

            else
            {
                alphaValue.Enabled = false;
            }
        }

        private void setalloptions(bool set)
        {
                netIdentCheck.Checked = set;
				nCheck.Checked = set;
				noClqCheck.Checked = set;
				csCheck.Checked = set;
				cmCheck.Checked = set;
				nocomCheck.Checked = set;
				gnCheck.Checked = set;
				npolCheck.Checked = set;
				cmoiCheck.Checked = set;
				npiCheck.Checked = set;
				clqOverlpCheck.Checked = set;
				complexClqOverlpCheck.Checked = set;
				npolStarCheck.Checked = set;
				npiStarCheck.Checked = set;
				npiOneStarCheck.Checked = set;
				npolSizeCheck.Checked = set;
				npolCohesionSize.Checked = set;
				npiTwoStarCheck.Checked = set;
				npiThreeStarCheck.Checked = set;
				depCheck.Checked = set;
				sysdepCheck.Checked = set;
				densityCheck.Checked = set;
				transCheck.Checked = set;
				sysinCheck.Checked = set;
				erpolCheck.Checked = set;
				avgdiCheck.Checked = set;
				avgdoCheck.Checked = set;
				digCheck.Checked = set;
				dogCheck.Checked = set;
				cigCheck.Checked = set;
				cogCheck.Checked = set;
				bigCheck.Checked = set;
				bogCheck.Checked = set;
				eigCheck.Checked = set;
				eogCheck.Checked = set;
            }

        private void selectallbutton_Click(object sender, EventArgs e)
        {
               this.setalloptions(true);
        }

        private void deselectallbutton_Click(object sender, EventArgs e)
        {
                 this.setalloptions(false);
        }

        private void kCliqueFileButton_Click(object sender, EventArgs e)
        {
            if (kCliqueFileDialog.ShowDialog() == DialogResult.OK)
            {
                kCliqueValue.Text = Constants.FileSelected;
            }
        }
 

       

        

    }
}
        



     

        

        
    
