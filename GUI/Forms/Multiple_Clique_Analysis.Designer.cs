namespace NetworkGUI.Forms
{
    partial class Multiple_Clique_Analysis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.minExtraction = new System.Windows.Forms.RadioButton();
            this.lowerExtraction = new System.Windows.Forms.RadioButton();
            this.upperExtraction = new System.Windows.Forms.RadioButton();
            this.maxExtraction = new System.Windows.Forms.RadioButton();
            this.useCutoffFileButton = new System.Windows.Forms.Button();
            this.binaryCutoff = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.AddFiles = new System.Windows.Forms.Button();
            this.cliqueAffiliationMatrix = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Closee = new System.Windows.Forms.Button();
            this.CliqueChara = new System.Windows.Forms.Button();
            this.dyadicFile = new System.Windows.Forms.Button();
            this.cliqueByCliqueOverlap = new System.Windows.Forms.Button();
            this.cliqueMemberShipOverLap = new System.Windows.Forms.Button();
            this.WCOC = new System.Windows.Forms.Button();
            this.wcmo = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.NetworkChara = new System.Windows.Forms.Button();
            this.NCoutputoptions = new System.Windows.Forms.Button();
            this.useweightfilebutton = new System.Windows.Forms.Button();
            this.sameweightbutton = new System.Windows.Forms.RadioButton();
            this.weightfilebutton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.filenamelabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.weightFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.minExtraction);
            this.groupBox4.Controls.Add(this.lowerExtraction);
            this.groupBox4.Controls.Add(this.upperExtraction);
            this.groupBox4.Controls.Add(this.maxExtraction);
            this.groupBox4.Location = new System.Drawing.Point(12, 70);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(134, 113);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clique Extraction";
            // 
            // minExtraction
            // 
            this.minExtraction.AutoSize = true;
            this.minExtraction.Location = new System.Drawing.Point(6, 88);
            this.minExtraction.Name = "minExtraction";
            this.minExtraction.Size = new System.Drawing.Size(90, 17);
            this.minExtraction.TabIndex = 3;
            this.minExtraction.TabStop = true;
            this.minExtraction.Text = "Option 4 (min)";
            this.minExtraction.UseVisualStyleBackColor = true;
            this.minExtraction.CheckedChanged += new System.EventHandler(this.minExtraction_CheckedChanged);
            // 
            // lowerExtraction
            // 
            this.lowerExtraction.AutoSize = true;
            this.lowerExtraction.Location = new System.Drawing.Point(6, 65);
            this.lowerExtraction.Name = "lowerExtraction";
            this.lowerExtraction.Size = new System.Drawing.Size(99, 17);
            this.lowerExtraction.TabIndex = 2;
            this.lowerExtraction.TabStop = true;
            this.lowerExtraction.Text = "Option 3 (lower)";
            this.lowerExtraction.UseVisualStyleBackColor = true;
            this.lowerExtraction.CheckedChanged += new System.EventHandler(this.lowerExtraction_CheckedChanged);
            // 
            // upperExtraction
            // 
            this.upperExtraction.AutoSize = true;
            this.upperExtraction.Location = new System.Drawing.Point(6, 42);
            this.upperExtraction.Name = "upperExtraction";
            this.upperExtraction.Size = new System.Drawing.Size(101, 17);
            this.upperExtraction.TabIndex = 1;
            this.upperExtraction.TabStop = true;
            this.upperExtraction.Text = "Option 2 (upper)";
            this.upperExtraction.UseVisualStyleBackColor = true;
            this.upperExtraction.CheckedChanged += new System.EventHandler(this.upperExtraction_CheckedChanged);
            // 
            // maxExtraction
            // 
            this.maxExtraction.AutoSize = true;
            this.maxExtraction.Checked = true;
            this.maxExtraction.Location = new System.Drawing.Point(6, 19);
            this.maxExtraction.Name = "maxExtraction";
            this.maxExtraction.Size = new System.Drawing.Size(93, 17);
            this.maxExtraction.TabIndex = 0;
            this.maxExtraction.TabStop = true;
            this.maxExtraction.Text = "Option 1 (max)";
            this.maxExtraction.UseVisualStyleBackColor = true;
            this.maxExtraction.CheckedChanged += new System.EventHandler(this.maxExtraction_CheckedChanged);
            // 
            // useCutoffFileButton
            // 
            this.useCutoffFileButton.Location = new System.Drawing.Point(198, 112);
            this.useCutoffFileButton.Name = "useCutoffFileButton";
            this.useCutoffFileButton.Size = new System.Drawing.Size(96, 20);
            this.useCutoffFileButton.TabIndex = 20;
            this.useCutoffFileButton.Text = "Use Cutoff File...";
            this.useCutoffFileButton.UseVisualStyleBackColor = true;
            this.useCutoffFileButton.Click += new System.EventHandler(this.useCutoffFileButton_Click);
            // 
            // binaryCutoff
            // 
            this.binaryCutoff.Location = new System.Drawing.Point(156, 111);
            this.binaryCutoff.Name = "binaryCutoff";
            this.binaryCutoff.Size = new System.Drawing.Size(35, 20);
            this.binaryCutoff.TabIndex = 19;
            this.binaryCutoff.Text = "0.0";
            this.binaryCutoff.TextChanged += new System.EventHandler(this.binaryCutoff_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(153, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Cutoff for Binary Conversion:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(38, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(213, 21);
            this.comboBox1.TabIndex = 21;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Files";
            // 
            // AddFiles
            // 
            this.AddFiles.Location = new System.Drawing.Point(257, 12);
            this.AddFiles.Name = "AddFiles";
            this.AddFiles.Size = new System.Drawing.Size(75, 23);
            this.AddFiles.TabIndex = 23;
            this.AddFiles.Text = "Add Files";
            this.AddFiles.UseVisualStyleBackColor = true;
            this.AddFiles.Click += new System.EventHandler(this.AddFiles_Click);
            // 
            // cliqueAffiliationMatrix
            // 
            this.cliqueAffiliationMatrix.Location = new System.Drawing.Point(7, 189);
            this.cliqueAffiliationMatrix.Name = "cliqueAffiliationMatrix";
            this.cliqueAffiliationMatrix.Size = new System.Drawing.Size(150, 23);
            this.cliqueAffiliationMatrix.TabIndex = 25;
            this.cliqueAffiliationMatrix.Text = "Clique Affiliation Matrix";
            this.cliqueAffiliationMatrix.UseVisualStyleBackColor = true;
            this.cliqueAffiliationMatrix.Click += new System.EventHandler(this.cliqueAffiliationMatrix_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(249, 70);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 17);
            this.checkBox1.TabIndex = 26;
            this.checkBox1.Text = "Dyadic Files";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Closee
            // 
            this.Closee.Location = new System.Drawing.Point(257, 411);
            this.Closee.Name = "Closee";
            this.Closee.Size = new System.Drawing.Size(75, 23);
            this.Closee.TabIndex = 27;
            this.Closee.Text = "Close";
            this.Closee.UseVisualStyleBackColor = true;
            this.Closee.Click += new System.EventHandler(this.Close_Click);
            // 
            // CliqueChara
            // 
            this.CliqueChara.Location = new System.Drawing.Point(175, 189);
            this.CliqueChara.Name = "CliqueChara";
            this.CliqueChara.Size = new System.Drawing.Size(150, 23);
            this.CliqueChara.TabIndex = 28;
            this.CliqueChara.Text = "Clique Characteristics Matrix";
            this.CliqueChara.UseVisualStyleBackColor = true;
            this.CliqueChara.Click += new System.EventHandler(this.CliqueChara_Click);
            // 
            // dyadicFile
            // 
            this.dyadicFile.Location = new System.Drawing.Point(156, 41);
            this.dyadicFile.Name = "dyadicFile";
            this.dyadicFile.Size = new System.Drawing.Size(176, 23);
            this.dyadicFile.TabIndex = 29;
            this.dyadicFile.Text = "Use Multivariable Dyadic File";
            this.dyadicFile.UseVisualStyleBackColor = true;
            this.dyadicFile.Click += new System.EventHandler(this.dyadicFile_Click);
            // 
            // cliqueByCliqueOverlap
            // 
            this.cliqueByCliqueOverlap.Location = new System.Drawing.Point(175, 218);
            this.cliqueByCliqueOverlap.Name = "cliqueByCliqueOverlap";
            this.cliqueByCliqueOverlap.Size = new System.Drawing.Size(150, 23);
            this.cliqueByCliqueOverlap.TabIndex = 30;
            this.cliqueByCliqueOverlap.Text = "Clique by Clique Overlap";
            this.cliqueByCliqueOverlap.UseVisualStyleBackColor = true;
            this.cliqueByCliqueOverlap.Click += new System.EventHandler(this.cliqueByCliqueOverlap_Click);
            // 
            // cliqueMemberShipOverLap
            // 
            this.cliqueMemberShipOverLap.Location = new System.Drawing.Point(7, 218);
            this.cliqueMemberShipOverLap.Name = "cliqueMemberShipOverLap";
            this.cliqueMemberShipOverLap.Size = new System.Drawing.Size(150, 23);
            this.cliqueMemberShipOverLap.TabIndex = 31;
            this.cliqueMemberShipOverLap.Text = "Clique Membership Overlap";
            this.cliqueMemberShipOverLap.UseVisualStyleBackColor = true;
            this.cliqueMemberShipOverLap.Click += new System.EventHandler(this.cliqueMemberShipOverLap_Click);
            // 
            // WCOC
            // 
            this.WCOC.Location = new System.Drawing.Point(175, 247);
            this.WCOC.Name = "WCOC";
            this.WCOC.Size = new System.Drawing.Size(150, 23);
            this.WCOC.TabIndex = 32;
            this.WCOC.Text = "Weighted COC";
            this.WCOC.UseVisualStyleBackColor = true;
            this.WCOC.Click += new System.EventHandler(this.WCOC_Click);
            // 
            // wcmo
            // 
            this.wcmo.Location = new System.Drawing.Point(7, 247);
            this.wcmo.Name = "wcmo";
            this.wcmo.Size = new System.Drawing.Size(150, 23);
            this.wcmo.TabIndex = 33;
            this.wcmo.Text = "Weighted CMO";
            this.wcmo.UseVisualStyleBackColor = true;
            this.wcmo.Click += new System.EventHandler(this.wcmo_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(175, 305);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 23);
            this.button1.TabIndex = 34;
            this.button1.Text = "Save as Affilication Matrix";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(175, 276);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 23);
            this.button2.TabIndex = 35;
            this.button2.Text = "Save as Multiple Dyadic File";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // NetworkChara
            // 
            this.NetworkChara.Location = new System.Drawing.Point(7, 276);
            this.NetworkChara.Name = "NetworkChara";
            this.NetworkChara.Size = new System.Drawing.Size(150, 23);
            this.NetworkChara.TabIndex = 36;
            this.NetworkChara.Text = "Network Characteristics";
            this.NetworkChara.UseVisualStyleBackColor = true;
            this.NetworkChara.Click += new System.EventHandler(this.NetworkChara_Click);
            // 
            // NCoutputoptions
            // 
            this.NCoutputoptions.Location = new System.Drawing.Point(7, 305);
            this.NCoutputoptions.Name = "NCoutputoptions";
            this.NCoutputoptions.Size = new System.Drawing.Size(150, 23);
            this.NCoutputoptions.TabIndex = 37;
            this.NCoutputoptions.Text = "NC Output Options";
            this.NCoutputoptions.UseVisualStyleBackColor = true;
            this.NCoutputoptions.Click += new System.EventHandler(this.NCoutputoptions_Click);
            // 
            // useweightfilebutton
            // 
            this.useweightfilebutton.Enabled = false;
            this.useweightfilebutton.Location = new System.Drawing.Point(26, 38);
            this.useweightfilebutton.Name = "useweightfilebutton";
            this.useweightfilebutton.Size = new System.Drawing.Size(136, 23);
            this.useweightfilebutton.TabIndex = 23;
            this.useweightfilebutton.Text = "Use Weight File...";
            this.useweightfilebutton.UseVisualStyleBackColor = true;
            this.useweightfilebutton.Click += new System.EventHandler(this.useweightfilebutton_Click);
            // 
            // sameweightbutton
            // 
            this.sameweightbutton.AutoSize = true;
            this.sameweightbutton.Checked = true;
            this.sameweightbutton.Location = new System.Drawing.Point(7, 20);
            this.sameweightbutton.Name = "sameweightbutton";
            this.sameweightbutton.Size = new System.Drawing.Size(180, 17);
            this.sameweightbutton.TabIndex = 40;
            this.sameweightbutton.TabStop = true;
            this.sameweightbutton.Text = "Use same weight for all networks";
            this.sameweightbutton.UseVisualStyleBackColor = true;
            this.sameweightbutton.CheckedChanged += new System.EventHandler(this.sameweightbutton_CheckedChanged);
            // 
            // weightfilebutton
            // 
            this.weightfilebutton.AutoSize = true;
            this.weightfilebutton.Location = new System.Drawing.Point(7, 43);
            this.weightfilebutton.Name = "weightfilebutton";
            this.weightfilebutton.Size = new System.Drawing.Size(14, 13);
            this.weightfilebutton.TabIndex = 41;
            this.weightfilebutton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.filenamelabel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sameweightbutton);
            this.groupBox1.Controls.Add(this.useweightfilebutton);
            this.groupBox1.Controls.Add(this.weightfilebutton);
            this.groupBox1.Location = new System.Drawing.Point(12, 347);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(189, 87);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calcuate Interdependence";
            // 
            // filenamelabel
            // 
            this.filenamelabel.AutoSize = true;
            this.filenamelabel.Location = new System.Drawing.Point(81, 64);
            this.filenamelabel.Name = "filenamelabel";
            this.filenamelabel.Size = new System.Drawing.Size(68, 13);
            this.filenamelabel.TabIndex = 43;
            this.filenamelabel.Text = "filenamelabel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Filename:";
            // 
            // weightFileDialog
            // 
            this.weightFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
            // 
            // Multiple_Clique_Analysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 445);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.NCoutputoptions);
            this.Controls.Add(this.NetworkChara);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wcmo);
            this.Controls.Add(this.WCOC);
            this.Controls.Add(this.cliqueMemberShipOverLap);
            this.Controls.Add(this.cliqueByCliqueOverlap);
            this.Controls.Add(this.dyadicFile);
            this.Controls.Add(this.CliqueChara);
            this.Controls.Add(this.Closee);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.cliqueAffiliationMatrix);
            this.Controls.Add(this.AddFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.useCutoffFileButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.binaryCutoff);
            this.Controls.Add(this.label1);
            this.Name = "Multiple_Clique_Analysis";
            this.Text = "Multiple Clique Analysis";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton minExtraction;
        private System.Windows.Forms.RadioButton lowerExtraction;
        private System.Windows.Forms.RadioButton upperExtraction;
        private System.Windows.Forms.RadioButton maxExtraction;
        private System.Windows.Forms.Button useCutoffFileButton;
        private System.Windows.Forms.TextBox binaryCutoff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AddFiles;
        private System.Windows.Forms.Button cliqueAffiliationMatrix;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button Closee;
        private System.Windows.Forms.Button CliqueChara;
        private System.Windows.Forms.Button dyadicFile;
        private System.Windows.Forms.Button cliqueByCliqueOverlap;
        private System.Windows.Forms.Button cliqueMemberShipOverLap;
        private System.Windows.Forms.Button WCOC;
        private System.Windows.Forms.Button wcmo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button NetworkChara;
        private System.Windows.Forms.Button NCoutputoptions;
        private System.Windows.Forms.Button useweightfilebutton;
        private System.Windows.Forms.RadioButton sameweightbutton;
        private System.Windows.Forms.RadioButton weightfilebutton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label filenamelabel;
        private System.Windows.Forms.OpenFileDialog weightFileDialog;
    }
}