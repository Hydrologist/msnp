using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class ABMProgressForm : Form
    {
        List<int> runnos;
        int currentno;
        List<int> iters;

        public ABMProgressForm(List<int> _runnos, List<int> _iters)
        {
            InitializeComponent();
            runnos = _runnos;
            iters = _iters;
            currentno = 0;
            runnobar.Minimum = runnos[0];
            runnobar.Maximum = runnos[runnos.Count - 1];
            runnobar.Value = runnos[0];
            iterbar.Minimum = 0;
            iterbar.Maximum = iters[currentno];
            iterbar.Value = 0;
            label1.Text = "Currently simulating runno " + runnos[currentno] + " of " + runnos.Count;
            label1.Update();
            label1.Text = "Currently simulating runno " + runnos[currentno] + " of " + runnos.Count;
            label1.Update();
        }

        public void updateProgress()
        {
            iterbar.Value += 1;
            if (currentno == runnos.Count - 1 && iterbar.Value == iterbar.Maximum)
            {
                this.Close();
            }
            if (iterbar.Value == iterbar.Maximum)
            {
                currentno++;
                runnobar.Value += 1;
                label1.Text = "Currently simulating runno " + currentno + " of " + runnos.Count;
                label1.Update();
                iterbar.Maximum = iters[currentno];
                iterbar.Value = 0;
                return;
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
