using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class MultiplicationForm : Form
    {
        public MultiplicationForm()
        {
            InitializeComponent();
            multiplyButton.Enabled = false;
        }

        public string fileName
        {
            get { return openFileDialog.FileName; }
        }

        public bool dyadic
        {
            get { return dyadicFileButton.Checked == true; }
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                    multiplyButton.Enabled = true;
            }
        }

        private void multiplyButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}