#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace NetworkGUI
{
    public partial class MultipleFileForm : Form
    {
        MainForm parent = null;

        NetworkGUI.Forms.Multiple_Clique_Analysis parent2 = null;
        string[] files;


        public MultipleFileForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        public MultipleFileForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        public MultipleFileForm(NetworkGUI.Forms.Multiple_Clique_Analysis parent)
        {
            this.parent2 = parent;
            InitializeComponent();
        }



        private void addButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog.FileNames)
                {
                    fileList.Items.Add(s);
                }
            }
            files = null;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedIndex == -1)
                return;

            fileList.Items.RemoveAt(fileList.SelectedIndex);
            files = null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (parent != null)
            {
                parent.loadFromMultipleFiles(this);
                this.Close();
            }
            if (parent2 != null)
            {
                parent2.updatefiles(FileList);
                this.Close();
            }

        }

   

        // Allows the parent to access our list
        public string[] FileList
        {
            get
            {
                if (files == null)
                {
                    files = new string[fileList.Items.Count];
                    for (int i = 0; i < fileList.Items.Count; ++i)
                    {
                        files[i] = fileList.Items[i].ToString();
                    }
                }
                return files;
            }
        }
    }
}