namespace NetworkGUI.Forms
{
    partial class DichotomizeForm
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
            this.binaryCutoff = new System.Windows.Forms.TextBox();
            this.useCutoffFileButton = new System.Windows.Forms.Button();
            this.cutoffFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Cutoff for Binary Conversion:";
            // 
            // binaryCutoff
            // 
            this.binaryCutoff.Location = new System.Drawing.Point(162, 10);
            this.binaryCutoff.Name = "binaryCutoff";
            this.binaryCutoff.Size = new System.Drawing.Size(44, 20);
            this.binaryCutoff.TabIndex = 8;
            this.binaryCutoff.TextChanged += new System.EventHandler(this.binaryCutoff_TextChanged);
            // 
            // useCutoffFileButton
            // 
            this.useCutoffFileButton.Location = new System.Drawing.Point(12, 37);
            this.useCutoffFileButton.Name = "useCutoffFileButton";
            this.useCutoffFileButton.Size = new System.Drawing.Size(94, 22);
            this.useCutoffFileButton.TabIndex = 18;
            this.useCutoffFileButton.Text = "Use Cutoff File...";
            this.useCutoffFileButton.UseVisualStyleBackColor = true;
            this.useCutoffFileButton.Click += new System.EventHandler(this.useCutoffFileButton_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(112, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 22);
            this.button1.TabIndex = 19;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DichotomizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 73);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.useCutoffFileButton);
            this.Controls.Add(this.binaryCutoff);
            this.Controls.Add(this.label1);
            this.Name = "DichotomizeForm";
            this.Text = "Dichotomization";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox binaryCutoff;
        private System.Windows.Forms.Button useCutoffFileButton;

        private System.Windows.Forms.OpenFileDialog cutoffFileDialog;
        private System.Windows.Forms.Button button1;
    }
}