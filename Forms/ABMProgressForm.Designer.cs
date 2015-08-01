namespace NetworkGUI.Forms
{
    partial class ABMProgressForm
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
            this.runnobar = new System.Windows.Forms.ProgressBar();
            this.iterbar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // runnobar
            // 
            this.runnobar.Location = new System.Drawing.Point(12, 60);
            this.runnobar.Name = "runnobar";
            this.runnobar.Size = new System.Drawing.Size(260, 23);
            this.runnobar.TabIndex = 1;
            // 
            // iterbar
            // 
            this.iterbar.Location = new System.Drawing.Point(12, 89);
            this.iterbar.Name = "iterbar";
            this.iterbar.Size = new System.Drawing.Size(260, 23);
            this.iterbar.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Progress";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 146);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.iterbar);
            this.Controls.Add(this.runnobar);
            this.Name = "Form2";
            this.Text = "Operation Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar runnobar;
        private System.Windows.Forms.ProgressBar iterbar;
        private System.Windows.Forms.Label label1;

    }
}