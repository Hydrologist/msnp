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
    partial class JumpToForm : Form
    {
        public JumpToForm()
        {
            InitializeComponent();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (yearTextBox.Text == "")
            {
                MessageBox.Show("Please enter a year to jump to!", "Error!");
            }
            else
            {
                try
                {
                    int.Parse(yearTextBox.Text);
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid year entered!", "Error!");
                }

            }
        }

        public int year
        {
            get
            {
                return int.Parse(yearTextBox.Text);
            }
            set
            {
                yearTextBox.Text = value.ToString();
            }
        }
    }
}