using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Network.Matrices;
using Network.IO;

namespace NetworkGUI.Forms
{
    public partial class Multiple_Clique_Analysis : Form
    {
        FileForCliqueAnalysis[] files = new FileForCliqueAnalysis[1];
        string CutOffFileName;
        string WeightFileName;
        string ShortWeightFileName;
        NetworkGUI.MultipleFileForm add_files;
        int currentlySelected;
        bool dyadic_file=false;
        double binary_cutoff;
        MainForm parent=null; //if C# is not referenced based, this would be terrible
        public McaNCoptions ncForm;
       // Network.NetworkGUI net = new Network.NetworkGUI();

        public Multiple_Clique_Analysis(NetworkGUI.MainForm p)
        {
            parent = p;
            InitializeComponent();
            add_files = new MultipleFileForm(this);
            //weightFileDialog = new OpenFileDialog();
           // openFileDialog = new OpenFileDialog();
            ncForm = new McaNCoptions(this);
            filenamelabel.Text = "";
        }

        #region File Load
        private void dyadicFile_Click(object sender, EventArgs e) // add multiple dyadic files
        {
            dyadic_file = true;
            openFileDialog.ShowDialog();
            files[0] = new FileForCliqueAnalysis();
            files[0].fileName = new string(openFileDialog.FileName.ToCharArray());
            currentlySelected = 0;
            //update dyadic checkbox
            checkBox1.Checked = dyadic_file;
        }

        private void AddFiles_Click(object sender, EventArgs e) // add file dialog box
        {
            dyadic_file = false;
            add_files = new MultipleFileForm(this);
            openFileDialog.Multiselect = true;
            add_files.Show();
            add_files.Activate();
            //add_files calls updatefiles when it is done
            //updatefiles(add_files.FileList); EXTRA
        }

        public void updatefiles(string[] file)
        {
            string[] fileNames = file;
            files = new FileForCliqueAnalysis[file.Length];

            //recording all of the filenames that I just got back
            for (int i = 0; i < fileNames.Length; i++)
            {
                comboBox1.Items.Add(file[i]);
                if(files[i]==null)
                    files[i]=new FileForCliqueAnalysis();
                //A bit ineffient, but it should be unimportant
                files[i].fileName = new string(file[i].ToCharArray());
            }
        }
        #endregion

        #region Extraction Option
        private void maxExtraction_CheckedChanged(object sender, EventArgs e)
        {
            files[currentlySelected].option = Network.CliqueExtractionType.Max;
        }

        private void upperExtraction_CheckedChanged(object sender, EventArgs e)
        {
            files[currentlySelected].option = Network.CliqueExtractionType.Upper;
        }

        private void lowerExtraction_CheckedChanged(object sender, EventArgs e)
        {
            files[currentlySelected].option = Network.CliqueExtractionType.Lower;
        }

        private void minExtraction_CheckedChanged(object sender, EventArgs e)
        {
            files[currentlySelected].option = Network.CliqueExtractionType.Min;
        }
        #endregion

        private void binaryCutoff_TextChanged(object sender, EventArgs e)
        {
            files[currentlySelected].cutOff=Convert.ToDouble(binaryCutoff.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //dyadic_file = !(dyadic_file); //problem 
            dyadic_file = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //A bit ineffient, but it should be unimportant
            currentlySelected = comboBox1.SelectedIndex;
            if (files[currentlySelected].option == Network.CliqueExtractionType.Max)
                maxExtraction.Checked = true;
            if (files[currentlySelected].option == Network.CliqueExtractionType.Upper)
                upperExtraction.Checked = true;
            if (files[currentlySelected].option == Network.CliqueExtractionType.Lower)
                lowerExtraction.Checked = true;
            if (files[currentlySelected].option == Network.CliqueExtractionType.Min)
                minExtraction.Checked = true;
        }

        #region Button Events
        private void useCutoffFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            CutOffFileName=openFileDialog.FileName;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);

        }

        private void cliqueAffiliationMatrix_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 5, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void CliqueChara_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 4, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void cliqueByCliqueOverlap_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 3, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void cliqueMemberShipOverLap_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 2, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void WCOC_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 1, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void wcmo_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 0, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        #region Save File
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            saveFileDialog1.DefaultExt = ".csv";
            saveFileDialog1.ShowDialog();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 6, saveFileDialog1.FileName, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            saveFileDialog1.DefaultExt = ".csv";
            saveFileDialog1.ShowDialog();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 7, saveFileDialog1.FileName, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }
        #endregion

        private void NetworkChara_Click(object sender, EventArgs e)
        {
            this.Close();
            parent.DealWithMultipleCliques(files, CutOffFileName, binary_cutoff, dyadic_file, 8, null, WeightFileName, useweightfile);
            parent.LoadMCAoptionlist(GetMCAcounter);
            parent.LoadMCAuseweightoption(weightfilebutton.Checked, ShortWeightFileName);
        }

        private void NCoutputoptions_Click(object sender, EventArgs e)
        {
       
            ncForm.ShowDialog();
        }

#endregion

        public void UpdateInterdependnceRadioButton(bool interdep)
        {
           groupBox1.Enabled  = interdep;
        }

        private void sameweightbutton_CheckedChanged(object sender, EventArgs e)
        {
            useweightfilebutton.Enabled = !sameweightbutton.Checked;
        }

        private void useweightfilebutton_Click(object sender, EventArgs e)
        {
            weightFileDialog.ShowDialog();
            WeightFileName = weightFileDialog.FileName;
            string[] parts = WeightFileName.Split('\\');
            ShortWeightFileName = parts[parts.Length - 1];
            filenamelabel.Text = ShortWeightFileName;
        }

        public bool useweightfile
        {
            get
            {
                return weightfilebutton.Checked;
            }
        }

        public string weightfile
        {
            get
            {
                return ShortWeightFileName;
            }
        }

        public bool[] GetMCAcounter
        {
            get
            {
                return ncForm.MCAcounterOptions;
            }
        }

        public void SetMCAcounter(bool[] list)
        {
            ncForm.setoptions(list);
        }

        public void Setuseweightoption(bool use, string filename)
        {
            weightfilebutton.Checked = use;
            ShortWeightFileName = filename;
            filenamelabel.Text = filename;
        }

        public MatrixComputations.TransitivityType TT
        {
            get
            { 
                return ncForm.TT;
            }
        }


        

     

        
    }
    public class FileForCliqueAnalysis
    {
        public string fileName;
        public Network.CliqueExtractionType option;
        public double cutOff;
        public FileForCliqueAnalysis()
        { }
        /* modification of this class required to import from multiple clique file */
    }        
}
