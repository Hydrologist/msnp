namespace NetworkGUI
{
    partial class BlocForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.posText = new System.Windows.Forms.TextBox();
            this.negText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(231, 3);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(49, 49);
            this.goButton.TabIndex = 0;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Positive Correlation Cutoff:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max Number of Steps:";
            // 
            // posText
            // 
            this.posText.Location = new System.Drawing.Point(149, 6);
            this.posText.Name = "posText";
            this.posText.Size = new System.Drawing.Size(65, 20);
            this.posText.TabIndex = 3;
            this.posText.Text = "0.9";
            this.posText.TextChanged += new System.EventHandler(this.posText_TextChanged);
            // 
            // negText
            // 
            this.negText.Location = new System.Drawing.Point(149, 32);
            this.negText.Name = "negText";
            this.negText.Size = new System.Drawing.Size(65, 20);
            this.negText.TabIndex = 4;
            this.negText.Text = "2";
            this.negText.TextChanged += new System.EventHandler(this.negTex_TextChanged);
            // 
            // BlocForm
            // 
            this.AcceptButton = this.goButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 59);
            this.Controls.Add(this.negText);
            this.Controls.Add(this.posText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.goButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlocForm";
            this.Text = "CONCOR Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox posText;
        private System.Windows.Forms.TextBox negText;
    }
}