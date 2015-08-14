using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace NetworkGUI.Forms
{
    public partial class ABMProgressForm : Form
    {
        List<int> runnos;
        int currentno;
        List<int> iters;

        public delegate void updateDelegate();
        public updateDelegate abmdelegate;
        public delegate void quitDelegate();
        public quitDelegate quitdelegate;

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
            abmdelegate = new updateDelegate(updateProgress);
            quitdelegate = new quitDelegate(quit);
        }

        public void updateProgress()
        {
            iterbar.Value += 1;
            if (currentno == runnos.Count - 1 && iterbar.Value == iterbar.Maximum)
            {
                this.Close();
            }
            if (iterbar.Value == iterbar.Maximum && runnobar.Value < runnobar.Maximum)
            {
                currentno++;
                runnobar.Value += 1;
                runnobar.Update();
                label1.Text = "Currently simulating runno " + runnos[currentno] + " of " + runnos.Count;
                label1.Update();
                iterbar.Maximum = iters[currentno];
                iterbar.Value = 0;
                return;
            }
        }

        public void quit()
        {
            iterbar.Value = ((iterbar.Value / (iterbar.Maximum / 3) + 1) * (iterbar.Maximum / 3)) - 1;
            updateProgress();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
