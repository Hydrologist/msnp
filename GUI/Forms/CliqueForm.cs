using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network;

namespace NetworkGUI
{
    public partial class CliqueForm : Form
    {
        List<UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>>> pairs;

        public List<UnordererdPair<string, NetworkIO.CharacteristicType>> SVC
        {
            get
            {
                List<UnordererdPair<string, NetworkIO.CharacteristicType>> response = new List<UnordererdPair<string, NetworkIO.CharacteristicType>>();
                foreach (UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p in pairs)
                {
                    if (p.First == "SVC")
                        response.Add(p.Second);
                }
                return response;
            }
        }

        public List<UnordererdPair<string, NetworkIO.CharacteristicType>> DVC
        {
            get
            {
                List<UnordererdPair<string, NetworkIO.CharacteristicType>> response = new List<UnordererdPair<string, NetworkIO.CharacteristicType>>();
                foreach (UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p in pairs)
                {
                    if (p.First == "DVC")
                        response.Add(p.Second);
                }
                return response;
            }
        }

        public List<UnordererdPair<string, NetworkIO.CharacteristicType>> attrMatrix
        {
            get
            {
                List<UnordererdPair<string, NetworkIO.CharacteristicType>> response = new List<UnordererdPair<string, NetworkIO.CharacteristicType>>();
                foreach (UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p in pairs)
                {
                    if (p.First == "attrMatrix")
                        response.Add(p.Second);
                }
                return response;
            }
        }

        public string[] SVCArray
        {
            get
            {
                List<string> response = new List<string>();
                foreach (UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p in pairs)
                {
                    if (p.First == "SVC")
                        response.Add(p.Second.First);
                }
                return response.ToArray();
            }
        }

        public string[] DVCArray
        {
            get
            {
                List<string> response = new List<string>();
                foreach (UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p in pairs)
                {
                    if (p.First == "DVC")
                        response.Add(p.Second.First);
                }
                return response.ToArray();
            }
        }

        public string ButtonText
        {
            get { return button2.Text; }
            set { button2.Text = value; }
        }

        public CliqueForm()
        {
            InitializeComponent();
            pairs = new List<UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>>>();
        }

        public void ClearList()
        {
            list.Items.Clear();
            pairs.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void svcButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                UnordererdPair<string, NetworkIO.CharacteristicType> p2 = 
                    new UnordererdPair<string, NetworkIO.CharacteristicType>(openFileDialog.FileName, GetCurrentType());
                UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p = new UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>>("SVC", p2);
                pairs.Add(p);
                list.Items.Add(p.First + " (" + Enum.GetName(typeof(NetworkIO.CharacteristicType), p.Second.Second) + "): " + p.Second.First);
            }
        }

        private NetworkIO.CharacteristicType GetCurrentType()
        {
            if (radioButton1.Checked)
                return NetworkIO.CharacteristicType.Sum;
            if (radioButton2.Checked)
                return NetworkIO.CharacteristicType.Max;
            if (radioButton3.Checked)
                return NetworkIO.CharacteristicType.Min;
            if (radioButton4.Checked)
                return NetworkIO.CharacteristicType.StdDev;

            return NetworkIO.CharacteristicType.Mean;
        }

        private void dvcButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                UnordererdPair<string, NetworkIO.CharacteristicType> p2 = 
                    new UnordererdPair<string, NetworkIO.CharacteristicType>(openFileDialog.FileName, GetCurrentType());
                UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p = new UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>>("DVC", p2);
                pairs.Add(p);
                list.Items.Add(p.First + " (" + Enum.GetName(typeof(NetworkIO.CharacteristicType), p.Second.Second) + "): " + p.Second.First);
            }
        }

        private void CliqueForm_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearList();
        }

        private void attrMatrixButton_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                UnordererdPair<string, NetworkIO.CharacteristicType> p2 =
                    new UnordererdPair<string, NetworkIO.CharacteristicType>(openFileDialog.FileName, GetCurrentType());
                UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>> p = new UnordererdPair<string, UnordererdPair<string, NetworkIO.CharacteristicType>>("attrMatrix", p2);
                pairs.Add(p);
                list.Items.Add(p.First + " (" + Enum.GetName(typeof(NetworkIO.CharacteristicType), p.Second.Second) + "): " + p.Second.First);
            }
        }
    }
}