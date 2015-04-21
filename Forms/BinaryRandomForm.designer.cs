namespace NetworkGUI
{
    partial class RandomForm
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
            this.goButton = new System.Windows.Forms.Button();
            this.nText = new System.Windows.Forms.TextBox();
            this.reachLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.yearTextBox = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.minNbox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.intNbox = new System.Windows.Forms.TextBox();
            this.maxNbox = new System.Windows.Forms.TextBox();
            this.randomNCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(147, 9);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(75, 24);
            this.goButton.TabIndex = 8;
            this.goButton.Text = "&Go";
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // nText
            // 
            this.nText.Location = new System.Drawing.Point(34, 12);
            this.nText.Name = "nText";
            this.nText.Size = new System.Drawing.Size(107, 20);
            this.nText.TabIndex = 7;
            this.nText.TextChanged += new System.EventHandler(this.reach_TextChanged);
            // 
            // reachLabel
            // 
            this.reachLabel.AutoSize = true;
            this.reachLabel.Location = new System.Drawing.Point(12, 15);
            this.reachLabel.Name = "reachLabel";
            this.reachLabel.Size = new System.Drawing.Size(18, 13);
            this.reachLabel.TabIndex = 6;
            this.reachLabel.Text = "&N:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "&Network Identifier (NI):";
            // 
            // yearTextBox
            // 
            this.yearTextBox.Location = new System.Drawing.Point(131, 119);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(87, 20);
            this.yearTextBox.TabIndex = 10;
            this.yearTextBox.Text = "1820";
            this.yearTextBox.TextChanged += new System.EventHandler(this.yearTextBox_TextChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(15, 152);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(200, 17);
            this.radioButton1.TabIndex = 11;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Individual equal chance for nonzero: ";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(15, 175);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(165, 17);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.Text = "Probability range for nonzero: ";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 203);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Min:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(96, 203);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Max:";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(48, 200);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(42, 20);
            this.textBox1.TabIndex = 15;
            this.textBox1.Text = "0.0";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(132, 200);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(42, 20);
            this.textBox2.TabIndex = 16;
            this.textBox2.Text = "1.0";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 108;
            this.label6.Text = "from";
            // 
            // minNbox
            // 
            this.minNbox.Enabled = false;
            this.minNbox.Location = new System.Drawing.Point(36, 86);
            this.minNbox.Name = "minNbox";
            this.minNbox.Size = new System.Drawing.Size(36, 20);
            this.minNbox.TabIndex = 107;
            this.minNbox.Text = "10";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(77, 13);
            this.label16.TabIndex = 106;
            this.label16.Text = "Range of N:    ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(134, 89);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 105;
            this.label15.Text = "at interval:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(73, 89);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(19, 13);
            this.label14.TabIndex = 104;
            this.label14.Text = "to:";
            // 
            // intNbox
            // 
            this.intNbox.Enabled = false;
            this.intNbox.Location = new System.Drawing.Point(192, 86);
            this.intNbox.Name = "intNbox";
            this.intNbox.Size = new System.Drawing.Size(37, 20);
            this.intNbox.TabIndex = 103;
            this.intNbox.Text = "10";
            // 
            // maxNbox
            // 
            this.maxNbox.Enabled = false;
            this.maxNbox.Location = new System.Drawing.Point(94, 86);
            this.maxNbox.Name = "maxNbox";
            this.maxNbox.Size = new System.Drawing.Size(37, 20);
            this.maxNbox.TabIndex = 102;
            this.maxNbox.Text = "100";
            // 
            // randomNCheckBox
            // 
            this.randomNCheckBox.AutoSize = true;
            this.randomNCheckBox.Location = new System.Drawing.Point(15, 40);
            this.randomNCheckBox.Name = "randomNCheckBox";
            this.randomNCheckBox.Size = new System.Drawing.Size(122, 17);
            this.randomNCheckBox.TabIndex = 109;
            this.randomNCheckBox.Text = "Random N generate";
            this.randomNCheckBox.UseVisualStyleBackColor = true;
            this.randomNCheckBox.CheckedChanged += new System.EventHandler(this.randomNCheckBox_CheckedChanged_1);
            // 
            // RandomForm
            // 
            this.AcceptButton = this.goButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 231);
            this.Controls.Add(this.randomNCheckBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.minNbox);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.intNbox);
            this.Controls.Add(this.maxNbox);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.nText);
            this.Controls.Add(this.reachLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RandomForm";
            this.Text = "Binary Random Matrix";
            this.Load += new System.EventHandler(this.RandomForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.TextBox nText;
        private System.Windows.Forms.Label reachLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox minNbox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox intNbox;
        private System.Windows.Forms.TextBox maxNbox;
        private System.Windows.Forms.CheckBox randomNCheckBox;

    }
}