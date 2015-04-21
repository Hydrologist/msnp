using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class ABMConfirmForm : Form
    {
        public ABMConfirmForm()
        {
            InitializeComponent();
        }

        public bool confirmation
        {
            get { return answer; }
            set { answer = value; }
        }

        private bool answer;
        
        public string text
        {
            set { label1.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            answer = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            answer = false;
            this.Close();
        }
    }
}
