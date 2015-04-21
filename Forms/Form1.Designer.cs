namespace NetworkGUI
{
    partial class BlocForm2
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
            this.posText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.negText = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.saveOverwrite = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // posText
            // 
            this.posText.Location = new System.Drawing.Point(149, 70);
            this.posText.Name = "posText";
            this.posText.Size = new System.Drawing.Size(65, 20);
            this.posText.TabIndex = 4;
            this.posText.Text = "0.9";
            this.posText.TextChanged += new System.EventHandler(this.posText_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Positive Correlation Cutoff:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Max Number of Steps:";
            // 
            // negText
            // 
            this.negText.Location = new System.Drawing.Point(149, 108);
            this.negText.Name = "negText";
            this.negText.Size = new System.Drawing.Size(65, 20);
            this.negText.TabIndex = 7;
            this.negText.Text = "2";
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(234, 69);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(49, 49);
            this.goButton.TabIndex = 8;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            // 
            // saveOverwrite
            // 
            this.saveOverwrite.AutoSize = true;
            this.saveOverwrite.Checked = true;
            this.saveOverwrite.Location = new System.Drawing.Point(108, 11);
            this.saveOverwrite.Name = "saveOverwrite";
            this.saveOverwrite.Size = new System.Drawing.Size(75, 17);
            this.saveOverwrite.TabIndex = 9;
            this.saveOverwrite.TabStop = true;
            this.saveOverwrite.Text = "Correlation";
            this.saveOverwrite.UseVisualStyleBackColor = true;
            this.saveOverwrite.CheckedChanged += new System.EventHandler(this.saveOverwrite_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(108, 34);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(139, 17);
            this.radioButton1.TabIndex = 10;
            this.radioButton1.Text = "Std. Euclidean Distance";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Calculate using:";
            // 
            // BlocForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 130);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.saveOverwrite);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.negText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.posText);
            this.Name = "BlocForm2";
            this.Text = "Block Options";
            this.Load += new System.EventHandler(this.BlocForm2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox posText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox negText;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.RadioButton saveOverwrite;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label3;
    }
}