namespace NetworkGUI.Forms
{
    partial class ClusteringForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.clusterText = new System.Windows.Forms.TextBox();
            this.correlationButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StandardizedEuclideanDistanceButton = new System.Windows.Forms.RadioButton();
            this.EuclideanDistanceButton = new System.Windows.Forms.RadioButton();
            this.goButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specified No. of Clusters:";
            // 
            // clusterText
            // 
            this.clusterText.Location = new System.Drawing.Point(138, 16);
            this.clusterText.Name = "clusterText";
            this.clusterText.Size = new System.Drawing.Size(46, 20);
            this.clusterText.TabIndex = 2;
            this.clusterText.Text = "1";
            // 
            // correlationButton
            // 
            this.correlationButton.AutoSize = true;
            this.correlationButton.Checked = true;
            this.correlationButton.Location = new System.Drawing.Point(16, 19);
            this.correlationButton.Name = "correlationButton";
            this.correlationButton.Size = new System.Drawing.Size(75, 17);
            this.correlationButton.TabIndex = 3;
            this.correlationButton.TabStop = true;
            this.correlationButton.Text = "Correlation";
            this.correlationButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StandardizedEuclideanDistanceButton);
            this.groupBox1.Controls.Add(this.EuclideanDistanceButton);
            this.groupBox1.Controls.Add(this.correlationButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clustering Method";
            // 
            // StandardizedEuclideanDistanceButton
            // 
            this.StandardizedEuclideanDistanceButton.AutoSize = true;
            this.StandardizedEuclideanDistanceButton.Location = new System.Drawing.Point(16, 65);
            this.StandardizedEuclideanDistanceButton.Name = "StandardizedEuclideanDistanceButton";
            this.StandardizedEuclideanDistanceButton.Size = new System.Drawing.Size(182, 17);
            this.StandardizedEuclideanDistanceButton.TabIndex = 5;
            this.StandardizedEuclideanDistanceButton.TabStop = true;
            this.StandardizedEuclideanDistanceButton.Text = "Standardized Euclidean Distance";
            this.StandardizedEuclideanDistanceButton.UseVisualStyleBackColor = true;
            // 
            // EuclideanDistanceButton
            // 
            this.EuclideanDistanceButton.AutoSize = true;
            this.EuclideanDistanceButton.Location = new System.Drawing.Point(16, 42);
            this.EuclideanDistanceButton.Name = "EuclideanDistanceButton";
            this.EuclideanDistanceButton.Size = new System.Drawing.Size(117, 17);
            this.EuclideanDistanceButton.TabIndex = 4;
            this.EuclideanDistanceButton.TabStop = true;
            this.EuclideanDistanceButton.Text = "Euclidean Distance";
            this.EuclideanDistanceButton.UseVisualStyleBackColor = true;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(199, 12);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(77, 30);
            this.goButton.TabIndex = 5;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // ClusteringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 162);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.clusterText);
            this.Controls.Add(this.label1);
            this.Name = "ClusteringForm";
            this.Text = "ClusteringForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox clusterText;
        private System.Windows.Forms.RadioButton correlationButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton StandardizedEuclideanDistanceButton;
        private System.Windows.Forms.RadioButton EuclideanDistanceButton;
        private System.Windows.Forms.Button goButton;
    }
}