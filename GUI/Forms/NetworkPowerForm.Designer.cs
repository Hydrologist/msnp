namespace NetworkGUI.Forms
{
    partial class NetworkPowerForm
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
            this.blockpowerbutton = new System.Windows.Forms.RadioButton();
            this.cliquepowerbutton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.structuraleqbutton = new System.Windows.Forms.RadioButton();
            this.roleeqbutton = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.filenamelabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.useattributefilebutton = new System.Windows.Forms.Button();
            this.attributefilebutton = new System.Windows.Forms.RadioButton();
            this.usesameattributebutton = new System.Windows.Forms.RadioButton();
            this.gobutton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.negText = new System.Windows.Forms.TextBox();
            this.posText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SPCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.blockpowerbutton);
            this.groupBox1.Controls.Add(this.cliquepowerbutton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(106, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Power Type";
            // 
            // blockpowerbutton
            // 
            this.blockpowerbutton.AutoSize = true;
            this.blockpowerbutton.Location = new System.Drawing.Point(13, 42);
            this.blockpowerbutton.Name = "blockpowerbutton";
            this.blockpowerbutton.Size = new System.Drawing.Size(85, 17);
            this.blockpowerbutton.TabIndex = 1;
            this.blockpowerbutton.Text = "Block Power";
            this.blockpowerbutton.UseVisualStyleBackColor = true;
            this.blockpowerbutton.CheckedChanged += new System.EventHandler(this.blockpowerbutton_CheckedChanged);
            // 
            // cliquepowerbutton
            // 
            this.cliquepowerbutton.AutoSize = true;
            this.cliquepowerbutton.Checked = true;
            this.cliquepowerbutton.Location = new System.Drawing.Point(13, 19);
            this.cliquepowerbutton.Name = "cliquepowerbutton";
            this.cliquepowerbutton.Size = new System.Drawing.Size(87, 17);
            this.cliquepowerbutton.TabIndex = 0;
            this.cliquepowerbutton.TabStop = true;
            this.cliquepowerbutton.Text = "Clique Power";
            this.cliquepowerbutton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.structuraleqbutton);
            this.groupBox2.Controls.Add(this.roleeqbutton);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(124, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 71);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Block power option";
            // 
            // structuraleqbutton
            // 
            this.structuraleqbutton.AutoSize = true;
            this.structuraleqbutton.Location = new System.Drawing.Point(13, 42);
            this.structuraleqbutton.Name = "structuraleqbutton";
            this.structuraleqbutton.Size = new System.Drawing.Size(132, 17);
            this.structuraleqbutton.TabIndex = 1;
            this.structuraleqbutton.Text = "Structural Equivalence";
            this.structuraleqbutton.UseVisualStyleBackColor = true;
            // 
            // roleeqbutton
            // 
            this.roleeqbutton.AutoSize = true;
            this.roleeqbutton.Checked = true;
            this.roleeqbutton.Location = new System.Drawing.Point(13, 19);
            this.roleeqbutton.Name = "roleeqbutton";
            this.roleeqbutton.Size = new System.Drawing.Size(112, 17);
            this.roleeqbutton.TabIndex = 0;
            this.roleeqbutton.TabStop = true;
            this.roleeqbutton.Text = "Role Equivalence ";
            this.roleeqbutton.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.filenamelabel);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.useattributefilebutton);
            this.groupBox3.Controls.Add(this.attributefilebutton);
            this.groupBox3.Controls.Add(this.usesameattributebutton);
            this.groupBox3.Location = new System.Drawing.Point(12, 89);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(262, 94);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Nodes attribute";
            // 
            // filenamelabel
            // 
            this.filenamelabel.AutoSize = true;
            this.filenamelabel.Location = new System.Drawing.Point(81, 70);
            this.filenamelabel.Name = "filenamelabel";
            this.filenamelabel.Size = new System.Drawing.Size(68, 13);
            this.filenamelabel.TabIndex = 4;
            this.filenamelabel.Text = "filenamelabel";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filename:";
            // 
            // useattributefilebutton
            // 
            this.useattributefilebutton.Enabled = false;
            this.useattributefilebutton.Location = new System.Drawing.Point(33, 42);
            this.useattributefilebutton.Name = "useattributefilebutton";
            this.useattributefilebutton.Size = new System.Drawing.Size(111, 23);
            this.useattributefilebutton.TabIndex = 2;
            this.useattributefilebutton.Text = "Use attribute file...";
            this.useattributefilebutton.UseVisualStyleBackColor = true;
            this.useattributefilebutton.Click += new System.EventHandler(this.useattributefilebutton_Click_1);
            // 
            // attributefilebutton
            // 
            this.attributefilebutton.AutoSize = true;
            this.attributefilebutton.Location = new System.Drawing.Point(13, 46);
            this.attributefilebutton.Name = "attributefilebutton";
            this.attributefilebutton.Size = new System.Drawing.Size(14, 13);
            this.attributefilebutton.TabIndex = 1;
            this.attributefilebutton.UseVisualStyleBackColor = true;
            // 
            // usesameattributebutton
            // 
            this.usesameattributebutton.AutoSize = true;
            this.usesameattributebutton.Checked = true;
            this.usesameattributebutton.Location = new System.Drawing.Point(13, 19);
            this.usesameattributebutton.Name = "usesameattributebutton";
            this.usesameattributebutton.Size = new System.Drawing.Size(173, 17);
            this.usesameattributebutton.TabIndex = 0;
            this.usesameattributebutton.TabStop = true;
            this.usesameattributebutton.Text = "Use same attribute for all nodes";
            this.usesameattributebutton.UseVisualStyleBackColor = true;
            this.usesameattributebutton.CheckedChanged += new System.EventHandler(this.usesameattributebutton_CheckedChanged);
            // 
            // gobutton
            // 
            this.gobutton.Location = new System.Drawing.Point(217, 258);
            this.gobutton.Name = "gobutton";
            this.gobutton.Size = new System.Drawing.Size(64, 47);
            this.gobutton.TabIndex = 3;
            this.gobutton.Text = "Go";
            this.gobutton.UseVisualStyleBackColor = true;
            this.gobutton.Click += new System.EventHandler(this.gobutton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.negText);
            this.groupBox4.Controls.Add(this.posText);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(12, 189);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(199, 83);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "CONCOR option";
            // 
            // negText
            // 
            this.negText.Location = new System.Drawing.Point(138, 45);
            this.negText.Name = "negText";
            this.negText.Size = new System.Drawing.Size(50, 20);
            this.negText.TabIndex = 3;
            this.negText.Text = "2";
            // 
            // posText
            // 
            this.posText.Location = new System.Drawing.Point(138, 20);
            this.posText.Name = "posText";
            this.posText.Size = new System.Drawing.Size(50, 20);
            this.posText.TabIndex = 2;
            this.posText.Text = "0.9";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Max Number of Steps:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Positive Correlation Cutoff:";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
            // 
            // SPCheckBox
            // 
            this.SPCheckBox.AutoSize = true;
            this.SPCheckBox.Checked = true;
            this.SPCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SPCheckBox.Location = new System.Drawing.Point(12, 282);
            this.SPCheckBox.Name = "SPCheckBox";
            this.SPCheckBox.Size = new System.Drawing.Size(134, 17);
            this.SPCheckBox.TabIndex = 5;
            this.SPCheckBox.Text = "Display Spoiling Power";
            this.SPCheckBox.UseVisualStyleBackColor = true;
            // 
            // NetworkPowerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 317);
            this.Controls.Add(this.SPCheckBox);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gobutton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetworkPowerForm";
            this.Text = "Network Power Option";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton blockpowerbutton;
        private System.Windows.Forms.RadioButton cliquepowerbutton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton structuraleqbutton;
        private System.Windows.Forms.RadioButton roleeqbutton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton usesameattributebutton;
        private System.Windows.Forms.RadioButton attributefilebutton;
        private System.Windows.Forms.Button useattributefilebutton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label filenamelabel;
        private System.Windows.Forms.Button gobutton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox posText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox negText;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox SPCheckBox;
    }
}