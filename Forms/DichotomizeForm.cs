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
using NetworkGUI.Forms;

namespace NetworkGUI.Forms
{
    public partial class DichotomizeForm : Form
    {
        private VectorValueProvider _cutoffVector;

        public DichotomizeForm()
        {
            InitializeComponent();
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

        public string CutoffText
        {
            get { return binaryCutoff.Text; }
            set { binaryCutoff.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
