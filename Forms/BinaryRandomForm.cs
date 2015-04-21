using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class RandomForm : Form
    {
        public RandomForm()
        {
            InitializeComponent();
        }


        public int N
        {
            get { return int.Parse(nText.Text); }
            set { nText.Text = value.ToString(); }
        }

        public bool RandomN
        {
            get { return randomNCheckBox.Checked; }
        }

        public int RandomMinN
        {
            get { return int.Parse(minNbox.Text); }
        }

        public int RandomMaxN
        {
            get { return int.Parse(maxNbox.Text); }
        }

        public int RandomIntN
        {
            get { return int.Parse(intNbox.Text); }
        }

        public int Year
        {
            get { return int.Parse(yearTextBox.Text); }
            set { yearTextBox.Text = value.ToString(); }
        }

        public bool ProbRange
        {
            get { return radioButton2.Checked; }
        }
	
        public double MinProb
        {
            get { return double.Parse(textBox1.Text); }
            set { textBox1.Text = value.ToString(); }
        }

        public double MaxProb
        {
            get { return double.Parse(textBox2.Text); }
            set { textBox2.Text = value.ToString(); }
        }

        private void reach_TextChanged(object sender, EventArgs e)
        {
            int n;
            goButton.Enabled = true;
            if (!int.TryParse(nText.Text, out n))
            {
                goButton.Enabled = false;
                return;
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            try
            {
                int.Parse(nText.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("N must be an integer!", "Error!");
                return;
            }
            if (radioButton2.Checked)
            {
                if (MinProb < 0 || MaxProb > 1)
                {
                    MessageBox.Show("Probability range has to be in between 0 to 1!", "Error!");
                    return;
                }
                if (MinProb > MaxProb)
                {
                    MessageBox.Show("Min range has to be equal or smaller than Max range!", "Error!");
                    return;
                }
            }
            this.Close();
        }

        private void yearTextBox_TextChanged(object sender, EventArgs e)
        {
            int n;
            goButton.Enabled = true;
            if (!int.TryParse(yearTextBox.Text, out n))
            {
                goButton.Enabled = false;
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double n;
            goButton.Enabled = true;
            if (!double.TryParse(textBox2.Text, out n))
            {
                goButton.Enabled = false;
                return;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double n;
            goButton.Enabled = true;
            if (!double.TryParse(textBox1.Text, out n))
            {
                goButton.Enabled = false;
                return;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = textBox2.Enabled  = radioButton2.Checked;
        } 

        private void randomNCheckBox_CheckedChanged_1(object sender, EventArgs e)
        {
            minNbox.Enabled = maxNbox.Enabled = intNbox.Enabled = randomNCheckBox.Checked;
            nText.Enabled = !randomNCheckBox.Checked; 
        }

        private void RandomForm_Load(object sender, EventArgs e)
        {

        }
    }
}