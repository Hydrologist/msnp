

namespace NetworkGUI
{
    partial class ValuedRandomForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.vmintext = new System.Windows.Forms.TextBox();
            this.vmaxtext = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.integerdatatype = new System.Windows.Forms.RadioButton();
            this.realnumberdatatype = new System.Windows.Forms.RadioButton();
            this.zerodiagonal = new System.Windows.Forms.RadioButton();
            this.randomdiagonal = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.minNbox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.intNbox = new System.Windows.Forms.TextBox();
            this.maxNbox = new System.Windows.Forms.TextBox();
            this.randomNCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(151, 8);
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
            this.nText.Size = new System.Drawing.Size(106, 20);
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
            this.label1.Location = new System.Drawing.Point(12, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "&Network Identifier (NI):";
            // 
            // yearTextBox
            // 
            this.yearTextBox.Location = new System.Drawing.Point(128, 222);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(98, 20);
            this.yearTextBox.TabIndex = 10;
            this.yearTextBox.Text = "1820";
            this.yearTextBox.TextChanged += new System.EventHandler(this.yearTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Range:   Min:";
            // 
            // vmintext
            // 
            this.vmintext.Location = new System.Drawing.Point(89, 114);
            this.vmintext.Name = "vmintext";
            this.vmintext.Size = new System.Drawing.Size(36, 20);
            this.vmintext.TabIndex = 12;
            this.vmintext.Text = "0";
            // 
            // vmaxtext
            // 
            this.vmaxtext.Location = new System.Drawing.Point(179, 114);
            this.vmaxtext.Name = "vmaxtext";
            this.vmaxtext.Size = new System.Drawing.Size(36, 20);
            this.vmaxtext.TabIndex = 13;
            this.vmaxtext.Text = "100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "to  Max:";
            // 
            // integerdatatype
            // 
            this.integerdatatype.AutoSize = true;
            this.integerdatatype.Checked = true;
            this.integerdatatype.Location = new System.Drawing.Point(6, 19);
            this.integerdatatype.Name = "integerdatatype";
            this.integerdatatype.Size = new System.Drawing.Size(58, 17);
            this.integerdatatype.TabIndex = 15;
            this.integerdatatype.TabStop = true;
            this.integerdatatype.Text = "Integer";
            this.integerdatatype.UseVisualStyleBackColor = true;
            // 
            // realnumberdatatype
            // 
            this.realnumberdatatype.AutoSize = true;
            this.realnumberdatatype.Location = new System.Drawing.Point(6, 42);
            this.realnumberdatatype.Name = "realnumberdatatype";
            this.realnumberdatatype.Size = new System.Drawing.Size(87, 17);
            this.realnumberdatatype.TabIndex = 16;
            this.realnumberdatatype.Text = "Real Number";
            this.realnumberdatatype.UseVisualStyleBackColor = true;
            // 
            // zerodiagonal
            // 
            this.zerodiagonal.AutoSize = true;
            this.zerodiagonal.Checked = true;
            this.zerodiagonal.Location = new System.Drawing.Point(15, 19);
            this.zerodiagonal.Name = "zerodiagonal";
            this.zerodiagonal.Size = new System.Drawing.Size(47, 17);
            this.zerodiagonal.TabIndex = 17;
            this.zerodiagonal.TabStop = true;
            this.zerodiagonal.Text = "Zero";
            this.zerodiagonal.UseVisualStyleBackColor = true;
            // 
            // randomdiagonal
            // 
            this.randomdiagonal.AutoSize = true;
            this.randomdiagonal.Location = new System.Drawing.Point(15, 42);
            this.randomdiagonal.Name = "randomdiagonal";
            this.randomdiagonal.Size = new System.Drawing.Size(84, 17);
            this.randomdiagonal.TabIndex = 18;
            this.randomdiagonal.Text = "Randomized";
            this.randomdiagonal.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.integerdatatype);
            this.groupBox1.Controls.Add(this.realnumberdatatype);
            this.groupBox1.Location = new System.Drawing.Point(12, 140);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(93, 72);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.zerodiagonal);
            this.groupBox2.Controls.Add(this.randomdiagonal);
            this.groupBox2.Location = new System.Drawing.Point(111, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(115, 72);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fill in Diagonal with: ";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(132, 306);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(42, 20);
            this.textBox2.TabIndex = 26;
            this.textBox2.Text = "1.0";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(48, 306);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(42, 20);
            this.textBox1.TabIndex = 25;
            this.textBox1.Text = "0.0";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(96, 309);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Max:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 309);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Min:";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(15, 281);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(162, 17);
            this.radioButton2.TabIndex = 22;
            this.radioButton2.Text = "Probabiliy range for nonzero: ";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(15, 258);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(190, 17);
            this.radioButton1.TabIndex = 21;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "No probability for nonzero specified";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // minNbox
            // 
            this.minNbox.Enabled = false;
            this.minNbox.Location = new System.Drawing.Point(39, 85);
            this.minNbox.Name = "minNbox";
            this.minNbox.Size = new System.Drawing.Size(36, 20);
            this.minNbox.TabIndex = 100;
            this.minNbox.Text = "10";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 64);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(77, 13);
            this.label16.TabIndex = 99;
            this.label16.Text = "Range of N:    ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(137, 88);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 98;
            this.label15.Text = "at interval:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(76, 88);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(19, 13);
            this.label14.TabIndex = 97;
            this.label14.Text = "to:";
            // 
            // intNbox
            // 
            this.intNbox.Enabled = false;
            this.intNbox.Location = new System.Drawing.Point(195, 85);
            this.intNbox.Name = "intNbox";
            this.intNbox.Size = new System.Drawing.Size(37, 20);
            this.intNbox.TabIndex = 96;
            this.intNbox.Text = "10";
            // 
            // maxNbox
            // 
            this.maxNbox.Enabled = false;
            this.maxNbox.Location = new System.Drawing.Point(97, 85);
            this.maxNbox.Name = "maxNbox";
            this.maxNbox.Size = new System.Drawing.Size(37, 20);
            this.maxNbox.TabIndex = 95;
            this.maxNbox.Text = "100";
            // 
            // randomNCheckBox
            // 
            this.randomNCheckBox.AutoSize = true;
            this.randomNCheckBox.Location = new System.Drawing.Point(18, 41);
            this.randomNCheckBox.Name = "randomNCheckBox";
            this.randomNCheckBox.Size = new System.Drawing.Size(123, 17);
            this.randomNCheckBox.TabIndex = 94;
            this.randomNCheckBox.Text = "Random N generate";
            this.randomNCheckBox.UseVisualStyleBackColor = true;
            this.randomNCheckBox.CheckedChanged += new System.EventHandler(this.randomNCheckBox_CheckedChanged_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 101;
            this.label6.Text = "from";
            // 
            // ValuedRandomForm
            // 
            this.AcceptButton = this.goButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 335);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.minNbox);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.intNbox);
            this.Controls.Add(this.maxNbox);
            this.Controls.Add(this.randomNCheckBox);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.vmaxtext);
            this.Controls.Add(this.vmintext);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.nText);
            this.Controls.Add(this.reachLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ValuedRandomForm";
            this.Text = "Valued Random Matrix";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.TextBox nText;
        private System.Windows.Forms.Label reachLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox vmintext;
        private System.Windows.Forms.TextBox vmaxtext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton integerdatatype;
        private System.Windows.Forms.RadioButton realnumberdatatype;
        private System.Windows.Forms.RadioButton zerodiagonal;
        private System.Windows.Forms.RadioButton randomdiagonal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox minNbox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox intNbox;
        private System.Windows.Forms.TextBox maxNbox;
        private System.Windows.Forms.CheckBox randomNCheckBox;
        private System.Windows.Forms.Label label6;

    }
}