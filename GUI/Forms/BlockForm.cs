using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class BlockForm : Form
    {
        public class Pair
        {
            public string Type, File;
            public Pair(string t, string f)
            {
                Type = t;
                File = f;
            }
            public override string ToString()
            {
                return Type + ": " + File;
            }
        }

        List<Pair> pairs;

        public List<string> SVC
        {
            get
            {
                List<string> response  = new List<string>();
                foreach (Pair p in pairs)
                {
                    if (p.Type == "SVC")
                        response.Add(p.File);
                }
                return response;
            }
        }

        public List<string> DVC
        {
            get
            {
                List<string> response = new List<string>();
                foreach (Pair p in pairs)
                {
                    if (p.Type == "DVC")
                        response.Add(p.File);
                }
                return response;
            }
        }

        public BlockForm()
        {
            InitializeComponent();
            pairs = new List<Pair>();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void svcButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Pair p = new Pair("SVC", openFileDialog.FileName);
                pairs.Add(p);
                list.Items.Add(p.ToString());
            }
        }

        private void dvcButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Pair p = new Pair("DVC", openFileDialog.FileName);
                pairs.Add(p);
                list.Items.Add(p.ToString());
            }
        }

        private void CliqueForm_Load(object sender, EventArgs e)
        {

        }
    }
}