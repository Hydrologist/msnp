using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class ABMForm : Form
    {
        public ABMForm()
        {
            InitializeComponent();
        }

        public int N
        {
            get { return int.Parse(nText.Text); }
            set { nText.Text = value.ToString(); }
        }

        public int networks
        {
            get { return int.Parse(netText.Text); }
            set { netText.Text = value.ToString(); }
        }
        public int netID
        {
            get { return int.Parse(netIdent.Text); }
            set { netIdent.Text = value.ToString(); }
        }

        public int maxsize
        {
            get { return int.Parse(maxbox.Text); }
            set { maxbox.Text = value.ToString(); }
        }

        public int stepsize
        {
            get { return int.Parse(stepbox.Text); }
            set { stepbox.Text = value.ToString(); }
        }

        public int minsize
        {
            get { return int.Parse(minbox.Text); }
            set { minbox.Text = value.ToString(); }
        }

        public bool enemy
        {
            get { return shocks[0]; }
        }
        public bool democracy
        {
            get { return shocks[1]; }
        }
        public bool cultism
        {
            get { return shocks[2]; }
        }

        public bool[] shocks = new bool[3];

        public bool homophily;

        public bool useparams;
    

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(nText.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int.Parse(nText.Text);
                int.Parse(netText.Text);
                int.Parse(netIdent.Text);
                useparams = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Enter integers only!", "Error!");
                return;
            }
            this.Close();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(netIdent.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void netText_TextChanged(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(netText.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(stepbox.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void minbox_TextChanged(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(minbox.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void maxbox_TextChanged(object sender, EventArgs e)
        {
            int n;
            //goButton.Enabled = true;
            if (!int.TryParse(maxbox.Text, out n))
            {
                //goButton.Enabled = false;
                return;
            }
        }

        private void paramsButton_Click(object sender, EventArgs e)
        {
            try
            {
                int.Parse(stepbox.Text);
                int.Parse(minbox.Text);
                int.Parse(maxbox.Text);
                int.Parse(netText.Text);
                int.Parse(netIdent.Text);
                useparams = true;
                homophily = homophilyButton.Checked;

                shocks[0] = false;
                shocks[1] = false;
                shocks[2] = false;
                if (checkedListBox1.CheckedItems.Count > 0)
                {
                    foreach (object checkeditem in checkedListBox1.CheckedItems)
                    {
                        if (string.Compare(checkeditem.ToString(), "Enemy") == 0)
                        {
                            shocks[0] = true;
                        }
                        else if (string.Compare(checkeditem.ToString(), "Democracy") == 0)
                        {
                            shocks[1] = true;
                        }
                        else if (string.Compare(checkeditem.ToString(), "Cultism") == 0)
                        {
                            shocks[2] = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Enter integers only!", "Error!");
                return;
            }
            this.Close();
        }

        private void prefAttachButton_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = false;

        }

        private void homophilyButton_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = true;
        }
    }
}
