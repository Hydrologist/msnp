using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class BlocForm : Form
    {
        public BlocForm()
        {
            InitializeComponent();
        }

        private void EnableButton()
        {
            goButton.Enabled = false;
            try
            {
                double p = double.Parse(posText.Text);
                int n = int.Parse(negText.Text);
                if (p < 0 || p > 1.0 || n < 1)
                    return;
            }
            catch (Exception)
            {
                return;
            }
            goButton.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void posText_TextChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        private void negTex_TextChanged(object sender, EventArgs e)
        {
            EnableButton();
        }

        public double pos
        {
            get { return double.Parse(posText.Text); }
        }

        public int MaxNoSteps
        {
            get { return int.Parse(negText.Text); }
        }


    }
}