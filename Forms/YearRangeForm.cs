#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace NetworkGUI
{
    partial class YearRangeForm : Form
    {
        public YearRangeForm()
        {
            InitializeComponent();
        }

        private void YearRangeForm_Load(object sender, EventArgs e)
        {
            this.SetMode(false);
        }

        public void SetMode(bool big)
        {
            if (big)
            {
                this.Height = 84;
            }
            else
            {
                this.Height = 58;
            }
        }

        public bool Cohesion
        {
            get { return cohesionCheckBox.Checked; }
        }

        public int from
        {
            get
            {
                return int.Parse(fromText.Text);
            }
            set
            {
                fromText.Text = value.ToString();
            }
        }

        public int to
        {
            get
            {
                return int.Parse(toText.Text);
            }
            set
            {
                toText.Text = value.ToString();
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            int fromYear, toYear;
            try
            {
                fromYear = int.Parse(fromText.Text);
                toYear = int.Parse(toText.Text);
                if (toYear < fromYear)
                {
                    MessageBox.Show("The end year must be less than or equal to the start year!", "Error!");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The years entered are invalid!", "Error!");
                return;
            }
            this.Close();
        }

        private void cohesionCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}