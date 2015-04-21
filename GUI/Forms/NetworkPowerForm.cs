using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class NetworkPowerForm : Form
    {
        string AttributeFileName;
        string ShortAttributeFileName;
        

        public NetworkPowerForm()
        {
            InitializeComponent();
            filenamelabel.Text = "";
        }

        private void EnableButton()
        {
            gobutton.Enabled = false;
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
            gobutton.Enabled = true;
        }

        private void gobutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void blockpowerbutton_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = groupBox2.Enabled = blockpowerbutton.Checked;
        }

        private void usesameattributebutton_CheckedChanged(object sender, EventArgs e)
        {
            useattributefilebutton.Enabled = !usesameattributebutton.Checked;
        }

        private void useattributefilebutton_Click_1(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            AttributeFileName = openFileDialog.FileName;
            string[] parts = AttributeFileName.Split('\\');
            ShortAttributeFileName = parts[parts.Length - 1];
            filenamelabel.Text = ShortAttributeFileName;
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

        public bool CliquePower
        {
            get { return cliquepowerbutton.Checked; }
        }

        public bool RoleEqui
        {
            get { return roleeqbutton.Checked; }
        }

        public bool useattributefile
        {
            get
            {
                return attributefilebutton.Checked;
            }
        }

        public string attributefilename
        {
            get
            {
                return AttributeFileName;
            }
        }

        public string attributefile
        {
            get
            {
                return ShortAttributeFileName;
            }
        }

        public bool showSP
        {
            get { return SPCheckBox.Checked; }
        }
      

   /*     public void Setuseattributeoption(bool use, string filename)
        {
            attributefilebutton.Checked = use;
            ShortAttributeFileName = filename;
            filenamelabel.Text = filename;
        }
        */
    }
}
