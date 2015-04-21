using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class ProgressForm : Form
    {
        int start, end, cur;
        public ProgressForm()
        {
            InitializeComponent();
        }
        public ProgressForm(int _start, int _end, int _cur)
        {
            InitializeComponent();

            start = _start;
            end = _end;
            cur = _cur;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            yearSaveLabel.Text = "Saving year " + cur.ToString() + " from " + start.ToString()
                + "-" + end.ToString();

            yearSaveLabel.Update();

            // Update the progress bar
            progressBar.Minimum = start;
            progressBar.Maximum = end;
            progressBar.Value = cur >= start && cur <= end ? cur : start;

            if (cur == end)
                this.Close();
        }

        public int startYear
        {
            set
            {
                start = value;
                UpdateLabel();
            }
        }

        public int endYear
        {
            set
            {
                end = value;
                UpdateLabel();
            }
        }

        public int curYear
        {
            set
            {
                cur = value;
                UpdateLabel();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {
            
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}