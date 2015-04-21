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
            this.correlationButton = new System.Windows.Forms.RadioButton();
            this.EuclideanDistanceButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // posText
            // 
            this.posText.Location = new System.Drawing.Point(149, 61);
            this.posText.Name = "posText";
            this.posText.Size = new System.Drawing.Size(65, 20);
            this.posText.TabIndex = 4;
            this.posText.Text = "0.9";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Positive Correlation Cutoff:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Max Number of Steps:";
            // 
            // negText
            // 
            this.negText.Location = new System.Drawing.Point(149, 89);
            this.negText.Name = "negText";
            this.negText.Size = new System.Drawing.Size(65, 20);
            this.negText.TabIndex = 7;
            this.negText.Text = "2";
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(237, 56);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(49, 49);
            this.goButton.TabIndex = 8;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // correlationButton
            // 
            this.correlationButton.AutoSize = true;
            this.correlationButton.Checked = true;
            this.correlationButton.Location = new System.Drawing.Point(108, 11);
            this.correlationButton.Name = "correlationButton";
            this.correlationButton.Size = new System.Drawing.Size(75, 17);
            this.correlationButton.TabIndex = 9;
            this.correlationButton.TabStop = true;
            this.correlationButton.Text = "Correlation";
            this.correlationButton.UseVisualStyleBackColor = true;
            // 
            // EuclideanDistanceButton
            // 
            this.EuclideanDistanceButton.AutoSize = true;
            this.EuclideanDistanceButton.Location = new System.Drawing.Point(108, 34);
            this.EuclideanDistanceButton.Name = "EuclideanDistanceButton";
            this.EuclideanDistanceButton.Size = new System.Drawing.Size(139, 17);
            this.EuclideanDistanceButton.TabIndex = 10;
            this.EuclideanDistanceButton.Text = "Std. Euclidean Distance";
            this.EuclideanDistanceButton.UseVisualStyleBackColor = true;
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
            this.ClientSize = new System.Drawing.Size(291, 116);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EuclideanDistanceButton);
            this.Controls.Add(this.correlationButton);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.negText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.posText);
            this.Name = "BlocForm2";
            this.Text = "Block Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox posText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox negText;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.RadioButton correlationButton;
        private System.Windows.Forms.RadioButton EuclideanDistanceButton;
        private System.Windows.Forms.Label label3;
    }
}