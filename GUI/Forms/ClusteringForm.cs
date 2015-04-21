using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class ClusteringForm : Form
    {
        public ClusteringForm()
        {
            InitializeComponent();
        }
 
        private void EnableButton()
        {
            goButton.Enabled = false;
            try
            {
                int n = int.Parse(clusterText.Text);
                if ( n < 1)
                    return;
            }
            catch (Exception)
            {
                return;
            }
            goButton.Enabled = true;
        }

        public string ClusteringMethod
        {
            get
            {
                if (correlationButton.Checked)
                    return "Correlation";
                else if (EuclideanDistanceButton.Checked)
                    return "ED";
                else
                    return "SED";
            }
        }

        public int MaxNoClusters
        {
            get { return int.Parse(clusterText.Text); }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
 
    }
}
