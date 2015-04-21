using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class NetworkSpilloverForm : Form
    {
        private string inputText;
        private List<int> indices;
        //private bool allIndices;

        public NetworkSpilloverForm()
        {
            InitializeComponent();
        }

        private void NetworkSpilloverForm_Load(object sender, EventArgs e)
        {

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            inputText = SpilloverTextBox.Text;
            if (inputText == "")
                throw new Exception("Input was empty string");

            /*
            if (inputText == "all")
            {
                allIndices = true;
            }
            */
            string[] inputIndices = inputText.Split(',');

            indices = new List<int>();
            foreach (string index in inputIndices)
            {
                int num = 0;
                try
                {
                    num = Convert.ToInt32(index);
                }
                catch
                {
                    throw new Exception(index + " is not a valid number");
                }
                indices.Add(num);
            }
            Close();
            SpilloverTextBox.Text = "";
            DialogResult = DialogResult.OK;
        }

        public List<int> Indices
        {
            get { return indices; }
        }


    }
}
