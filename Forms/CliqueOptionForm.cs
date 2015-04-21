using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network.IO;
using Network;

namespace NetworkGUI.Forms
{
    public partial class CliqueOptionForm : Form
    {
        private VectorValueProvider _cutoffVector;

        public CliqueOptionForm()
        {
            InitializeComponent();
            m = 1;
            k = 1;
            net = null;
        }

        public Network.NetworkGUI net;
        int m, k;

        public string KCliqueFileName
        {
            get { return kCliqueFileDialog.FileName; }
        }

        public string CMinMembersFileName
        {
            get { return cMinMembersFileDialog.FileName; }
        }

        public string SumMeanFilename
        {
            get { return sumMeanFileDialog.FileName; }
            set { sumMeanFileDialog.FileName = value; }
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

        public bool KCliqueDiag
        {
            get { return kCliqueDiagCheckBox.Checked; }
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
            get { return (binaryCutoff.Text == Constants.FileSelected ? -1.0 : double.Parse(binaryCutoff.Text)); }
            set { binaryCutoff.Text = (value == -1.0 ? Constants.FileSelected : value.ToString()); }
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
                || (kCliqueValue.Text != Constants.FileSelected && !int.TryParse(kCliqueValue.Text, out k))) 
                || (cMinMembers.Text != Constants.FileSelected && !int.TryParse(cMinMembers.Text, out m))) 
            {
                MessageBox.Show("You have not entered proper real numbers!", "Error!");
            }
            else
            { 
                if (cMinMembers.Text == Constants.FileSelected)
                    m = -1;
                if (kCliqueValue.Text == Constants.FileSelected)
                    k = -1;
                this.Close();
            }
        }

        private void sumMeanFileSelectButton_Click(object sender, EventArgs e)
        {
            sumMeanFileDialog.ShowDialog(); 
        }

        private void useCutoffFileButton_Click(object sender, EventArgs e)
        {
            if (cutoffFileDialog.ShowDialog() == DialogResult.OK)
            {
                binaryCutoff.Text = Constants.FileSelected;
                _cutoffVector = null;
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

        private void kCliqueFileButton_Click(object sender, EventArgs e)
        {
            if (kCliqueFileDialog.ShowDialog() == DialogResult.OK)
            {
                kCliqueValue.Text = Constants.FileSelected;
            }
        } 

    }
}
