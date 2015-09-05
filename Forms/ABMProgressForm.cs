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

        public delegate void updateDelegate();
        public updateDelegate abmdelegate;

        public ABMProgressForm(int networks)
        {
            InitializeComponent();
            runnobar.Minimum = 0;
            runnobar.Maximum = networks;
            runnobar.Value = 0;
            abmdelegate = new updateDelegate(updateProgress);
        }

        public void updateProgress()
        {
            if (runnobar.Value < runnobar.Maximum)
                runnobar.Value += 1;
            if (runnobar.Value == runnobar.Maximum)
                this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
