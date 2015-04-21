namespace NetworkGUI.Forms
{
    partial class CliqueOptionForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sumMeanFileSelectButton = new System.Windows.Forms.Button();
            this.noneButton = new System.Windows.Forms.RadioButton();
            this.svcMeanButton = new System.Windows.Forms.RadioButton();
            this.svcSumButton = new System.Windows.Forms.RadioButton();
            this.dvcMeanButton = new System.Windows.Forms.RadioButton();
            this.dvcSumButton = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.minExtraction = new System.Windows.Forms.RadioButton();
            this.lowerExtraction = new System.Windows.Forms.RadioButton();
            this.upperExtraction = new System.Windows.Forms.RadioButton();
            this.maxExtraction = new System.Windows.Forms.RadioButton();
            this.cMinMembersFileButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cMinMembers = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.useCutoffFileButton = new System.Windows.Forms.Button();
            this.binaryCutoff = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.kCliqueValue = new System.Windows.Forms.TextBox();
            this.kCliqueFileButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.cutoffFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.cMinMembersFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.sumMeanFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.kCliqueFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.kCliqueDiagCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sumMeanFileSelectButton);
            this.groupBox1.Controls.Add(this.noneButton);
            this.groupBox1.Controls.Add(this.svcMeanButton);
            this.groupBox1.Controls.Add(this.svcSumButton);
            this.groupBox1.Controls.Add(this.dvcMeanButton);
            this.groupBox1.Controls.Add(this.dvcSumButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(161, 163);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clique Characteristics";
            // 
            // sumMeanFileSelectButton
            // 
            this.sumMeanFileSelectButton.Location = new System.Drawing.Point(17, 132);
            this.sumMeanFileSelectButton.Name = "sumMeanFileSelectButton";
            this.sumMeanFileSelectButton.Size = new System.Drawing.Size(96, 21);
            this.sumMeanFileSelectButton.TabIndex = 5;
            this.sumMeanFileSelectButton.Text = "Select File...";
            this.sumMeanFileSelectButton.UseVisualStyleBackColor = true;
            this.sumMeanFileSelectButton.Click += new System.EventHandler(this.sumMeanFileSelectButton_Click);
            // 
            // noneButton
            // 
            this.noneButton.AutoSize = true;
            this.noneButton.Checked = true;
            this.noneButton.Location = new System.Drawing.Point(19, 19);
            this.noneButton.Name = "noneButton";
            this.noneButton.Size = new System.Drawing.Size(51, 17);
            this.noneButton.TabIndex = 4;
            this.noneButton.TabStop = true;
            this.noneButton.Text = "None";
            this.noneButton.UseVisualStyleBackColor = true;
            // 
            // svcMeanButton
            // 
            this.svcMeanButton.AutoSize = true;
            this.svcMeanButton.Location = new System.Drawing.Point(19, 109);
            this.svcMeanButton.Name = "svcMeanButton";
            this.svcMeanButton.Size = new System.Drawing.Size(104, 17);
            this.svcMeanButton.TabIndex = 3;
            this.svcMeanButton.Text = "Vector file--mean";
            this.svcMeanButton.UseVisualStyleBackColor = true;
            // 
            // svcSumButton
            // 
            this.svcSumButton.AutoSize = true;
            this.svcSumButton.Location = new System.Drawing.Point(19, 86);
            this.svcSumButton.Name = "svcSumButton";
            this.svcSumButton.Size = new System.Drawing.Size(97, 17);
            this.svcSumButton.TabIndex = 2;
            this.svcSumButton.Text = "Vector file--sum";
            this.svcSumButton.UseVisualStyleBackColor = true;
            // 
            // dvcMeanButton
            // 
            this.dvcMeanButton.AutoSize = true;
            this.dvcMeanButton.Location = new System.Drawing.Point(19, 63);
            this.dvcMeanButton.Name = "dvcMeanButton";
            this.dvcMeanButton.Size = new System.Drawing.Size(101, 17);
            this.dvcMeanButton.TabIndex = 1;
            this.dvcMeanButton.Text = "Matrix file--mean";
            this.dvcMeanButton.UseVisualStyleBackColor = true;
            // 
            // dvcSumButton
            // 
            this.dvcSumButton.AutoSize = true;
            this.dvcSumButton.Location = new System.Drawing.Point(19, 40);
            this.dvcSumButton.Name = "dvcSumButton";
            this.dvcSumButton.Size = new System.Drawing.Size(94, 17);
            this.dvcSumButton.TabIndex = 0;
            this.dvcSumButton.Text = "Matrix file--sum";
            this.dvcSumButton.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.minExtraction);
            this.groupBox4.Controls.Add(this.lowerExtraction);
            this.groupBox4.Controls.Add(this.upperExtraction);
            this.groupBox4.Controls.Add(this.maxExtraction);
            this.groupBox4.Location = new System.Drawing.Point(185, 151);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(134, 117);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clique Extraction";
            // 
            // minExtraction
            // 
            this.minExtraction.AutoSize = true;
            this.minExtraction.Location = new System.Drawing.Point(13, 88);
            this.minExtraction.Name = "minExtraction";
            this.minExtraction.Size = new System.Drawing.Size(90, 17);
            this.minExtraction.TabIndex = 3;
            this.minExtraction.TabStop = true;
            this.minExtraction.Text = "Option 4 (min)";
            this.minExtraction.UseVisualStyleBackColor = true;
            // 
            // lowerExtraction
            // 
            this.lowerExtraction.AutoSize = true;
            this.lowerExtraction.Location = new System.Drawing.Point(13, 65);
            this.lowerExtraction.Name = "lowerExtraction";
            this.lowerExtraction.Size = new System.Drawing.Size(99, 17);
            this.lowerExtraction.TabIndex = 2;
            this.lowerExtraction.TabStop = true;
            this.lowerExtraction.Text = "Option 3 (lower)";
            this.lowerExtraction.UseVisualStyleBackColor = true;
            // 
            // upperExtraction
            // 
            this.upperExtraction.AutoSize = true;
            this.upperExtraction.Location = new System.Drawing.Point(13, 42);
            this.upperExtraction.Name = "upperExtraction";
            this.upperExtraction.Size = new System.Drawing.Size(101, 17);
            this.upperExtraction.TabIndex = 1;
            this.upperExtraction.TabStop = true;
            this.upperExtraction.Text = "Option 2 (upper)";
            this.upperExtraction.UseVisualStyleBackColor = true;
            // 
            // maxExtraction
            // 
            this.maxExtraction.AutoSize = true;
            this.maxExtraction.Checked = true;
            this.maxExtraction.Location = new System.Drawing.Point(13, 19);
            this.maxExtraction.Name = "maxExtraction";
            this.maxExtraction.Size = new System.Drawing.Size(93, 17);
            this.maxExtraction.TabIndex = 0;
            this.maxExtraction.TabStop = true;
            this.maxExtraction.Text = "Option 1 (max)";
            this.maxExtraction.UseVisualStyleBackColor = true;
            // 
            // cMinMembersFileButton
            // 
            this.cMinMembersFileButton.Location = new System.Drawing.Point(246, 53);
            this.cMinMembersFileButton.Name = "cMinMembersFileButton";
            this.cMinMembersFileButton.Size = new System.Drawing.Size(75, 22);
            this.cMinMembersFileButton.TabIndex = 36;
            this.cMinMembersFileButton.Text = "Use File...";
            this.cMinMembersFileButton.UseVisualStyleBackColor = true;
            this.cMinMembersFileButton.Click += new System.EventHandler(this.cMinMembersFileButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(191, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "members";
            // 
            // cMinMembers
            // 
            this.cMinMembers.Location = new System.Drawing.Point(155, 53);
            this.cMinMembers.Name = "cMinMembers";
            this.cMinMembers.Size = new System.Drawing.Size(31, 20);
            this.cMinMembers.TabIndex = 34;
            this.cMinMembers.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Display Cliques with minimum:";
            // 
            // useCutoffFileButton
            // 
            this.useCutoffFileButton.Location = new System.Drawing.Point(204, 17);
            this.useCutoffFileButton.Name = "useCutoffFileButton";
            this.useCutoffFileButton.Size = new System.Drawing.Size(96, 22);
            this.useCutoffFileButton.TabIndex = 32;
            this.useCutoffFileButton.Text = "Use Cutoff File...";
            this.useCutoffFileButton.UseVisualStyleBackColor = true;
            this.useCutoffFileButton.Click += new System.EventHandler(this.useCutoffFileButton_Click);
            // 
            // binaryCutoff
            // 
            this.binaryCutoff.Location = new System.Drawing.Point(154, 18);
            this.binaryCutoff.Name = "binaryCutoff";
            this.binaryCutoff.Size = new System.Drawing.Size(44, 20);
            this.binaryCutoff.TabIndex = 31;
            this.binaryCutoff.Text = "0.0";
            this.binaryCutoff.TextChanged += new System.EventHandler(this.binaryCutoff_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Cutoff for Binary Conversion:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Specify K-value of K-clique order:";
            // 
            // kCliqueValue
            // 
            this.kCliqueValue.Location = new System.Drawing.Point(179, 89);
            this.kCliqueValue.Name = "kCliqueValue";
            this.kCliqueValue.Size = new System.Drawing.Size(36, 20);
            this.kCliqueValue.TabIndex = 38;
            this.kCliqueValue.Text = "1";
            // 
            // kCliqueFileButton
            // 
            this.kCliqueFileButton.Location = new System.Drawing.Point(223, 87);
            this.kCliqueFileButton.Name = "kCliqueFileButton";
            this.kCliqueFileButton.Size = new System.Drawing.Size(75, 22);
            this.kCliqueFileButton.TabIndex = 39;
            this.kCliqueFileButton.Text = "Use File...";
            this.kCliqueFileButton.UseVisualStyleBackColor = true;
            this.kCliqueFileButton.Click += new System.EventHandler(this.kCliqueFileButton_Click);
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(237, 283);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(84, 31);
            this.goButton.TabIndex = 40;
            this.goButton.Text = "OK";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // cutoffFileDialog
            // 
            this.cutoffFileDialog.FileName = "openFileDialog1";
            // 
            // cMinMembersFileDialog
            // 
            this.cMinMembersFileDialog.FileName = "openFileDialog1";
            // 
            // sumMeanFileDialog
            // 
            this.sumMeanFileDialog.FileName = "openFileDialog1";
            // 
            // kCliqueFileDialog
            // 
            this.kCliqueFileDialog.FileName = "openFileDialog1";
            // 
            // kCliqueDiagCheckBox
            // 
            this.kCliqueDiagCheckBox.AutoSize = true;
            this.kCliqueDiagCheckBox.Checked = true;
            this.kCliqueDiagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.kCliqueDiagCheckBox.Location = new System.Drawing.Point(13, 117);
            this.kCliqueDiagCheckBox.Name = "kCliqueDiagCheckBox";
            this.kCliqueDiagCheckBox.Size = new System.Drawing.Size(173, 17);
            this.kCliqueDiagCheckBox.TabIndex = 41;
            this.kCliqueDiagCheckBox.Text = "Zero diagonal of Reach Matrix ";
            this.kCliqueDiagCheckBox.UseVisualStyleBackColor = true;
            // 
            // CliqueOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 321);
            this.Controls.Add(this.kCliqueDiagCheckBox);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.kCliqueFileButton);
            this.Controls.Add(this.kCliqueValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cMinMembersFileButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cMinMembers);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.useCutoffFileButton);
            this.Controls.Add(this.binaryCutoff);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Name = "CliqueOptionForm";
            this.Text = "Cliques Option";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button sumMeanFileSelectButton;
        private System.Windows.Forms.RadioButton noneButton;
        private System.Windows.Forms.RadioButton svcMeanButton;
        private System.Windows.Forms.RadioButton svcSumButton;
        private System.Windows.Forms.RadioButton dvcMeanButton;
        private System.Windows.Forms.RadioButton dvcSumButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton minExtraction;
        private System.Windows.Forms.RadioButton lowerExtraction;
        private System.Windows.Forms.RadioButton upperExtraction;
        private System.Windows.Forms.RadioButton maxExtraction;
        private System.Windows.Forms.Button cMinMembersFileButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox cMinMembers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button useCutoffFileButton;
        private System.Windows.Forms.TextBox binaryCutoff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox kCliqueValue;
        private System.Windows.Forms.Button kCliqueFileButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.OpenFileDialog cutoffFileDialog;
        private System.Windows.Forms.OpenFileDialog cMinMembersFileDialog;
        private System.Windows.Forms.OpenFileDialog sumMeanFileDialog;
        private System.Windows.Forms.OpenFileDialog kCliqueFileDialog;
        private System.Windows.Forms.CheckBox kCliqueDiagCheckBox;
    }
}