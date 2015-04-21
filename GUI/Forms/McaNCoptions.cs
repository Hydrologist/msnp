using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network.Matrices;
using Network.IO;
using Network;
using NetworkGUI.Forms;

namespace NetworkGUI
{
    public partial class McaNCoptions : Form
    {
        Multiple_Clique_Analysis parent = null;
        
        public McaNCoptions(Multiple_Clique_Analysis p)
        {
            parent = p;
            InitializeComponent();
        }
  

        private bool TCB(CheckBox cb)
        {
            return  cb.Enabled && cb.Checked;
        }

        public MatrixComputations.TransitivityType TT
        {
            get
            {
                if (simpleTT.Checked)
                    return MatrixComputations.TransitivityType.Simple;
                else if (weaklinkTT.Checked)
                    return MatrixComputations.TransitivityType.Weak;
                else
                    return MatrixComputations.TransitivityType.Strong;
            }
        }

        public void setoptions(bool[] list)
        {
            checkBox1.Checked = list[0];
            checkBox2.Checked = list[1];
            checkBox3.Checked = list[2];
            checkBox4.Checked = list[3];
            checkBox5.Checked = list[4];
            checkBox6.Checked = list[5];
            checkBox7.Checked = list[6];
            checkBox8.Checked = list[7];
            checkBox9.Checked = list[8];
            checkBox10.Checked = list[9];
            checkBox11.Checked = list[10];
            checkBox12.Checked = list[11];
            checkBox13.Checked = list[12];
            checkBox14.Checked = list[13];
            checkBox15.Checked = list[14];
            checkBox16.Checked = list[15];
        }

        public bool[] MCAcounterOptions
        {
            get
            {
                bool[] mcaoptions = new bool[16];
                mcaoptions[0] = TCB(checkBox1);
                mcaoptions[1] = TCB(checkBox2);
                mcaoptions[2] = TCB(checkBox3);
                mcaoptions[3] = TCB(checkBox4);
                mcaoptions[4] = TCB(checkBox5);
                mcaoptions[5] = TCB(checkBox6);
                mcaoptions[6] = TCB(checkBox7);
                mcaoptions[7] = TCB(checkBox8);
                mcaoptions[8] = TCB(checkBox9);
                mcaoptions[9] = TCB(checkBox10);
                mcaoptions[10] = TCB(checkBox11);
                mcaoptions[11] = TCB(checkBox12);
                mcaoptions[12] = TCB(checkBox13);
                mcaoptions[13] = TCB(checkBox14);
                mcaoptions[14] = TCB(checkBox15);
                mcaoptions[15] = TCB(checkBox16);
             

                return mcaoptions;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.UpdateInterdependnceRadioButton(checkBox16.Checked);
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = simpleTT.Enabled = weaklinkTT.Enabled = stronglinkTT.Enabled = checkBox15.Checked;
        }

   

      

    }
}
