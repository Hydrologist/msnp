namespace NetworkGUI.Forms
{
    partial class NetworkSpilloverForm
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
            this.MatchedSpilloverRadioButton = new System.Windows.Forms.RadioButton();
            this.ReciprocatedSpilloverRadioButton = new System.Windows.Forms.RadioButton();
            this.InverseSpilloverRadioButton = new System.Windows.Forms.RadioButton();
            this.SpilloverTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MatchedSpilloverRadioButton
            // 
            this.MatchedSpilloverRadioButton.AutoSize = true;
            this.MatchedSpilloverRadioButton.Location = new System.Drawing.Point(12, 12);
            this.MatchedSpilloverRadioButton.Name = "MatchedSpilloverRadioButton";
            this.MatchedSpilloverRadioButton.Size = new System.Drawing.Size(110, 17);
            this.MatchedSpilloverRadioButton.TabIndex = 0;
            this.MatchedSpilloverRadioButton.TabStop = true;
            this.MatchedSpilloverRadioButton.Text = "Matched Spillover";
            this.MatchedSpilloverRadioButton.UseVisualStyleBackColor = true;
            // 
            // ReciprocatedSpilloverRadioButton
            // 
            this.ReciprocatedSpilloverRadioButton.AutoSize = true;
            this.ReciprocatedSpilloverRadioButton.Location = new System.Drawing.Point(12, 35);
            this.ReciprocatedSpilloverRadioButton.Name = "ReciprocatedSpilloverRadioButton";
            this.ReciprocatedSpilloverRadioButton.Size = new System.Drawing.Size(132, 17);
            this.ReciprocatedSpilloverRadioButton.TabIndex = 1;
            this.ReciprocatedSpilloverRadioButton.TabStop = true;
            this.ReciprocatedSpilloverRadioButton.Text = "Reciprocated Spillover";
            this.ReciprocatedSpilloverRadioButton.UseVisualStyleBackColor = true;
            // 
            // InverseSpilloverRadioButton
            // 
            this.InverseSpilloverRadioButton.AutoSize = true;
            this.InverseSpilloverRadioButton.Location = new System.Drawing.Point(12, 58);
            this.InverseSpilloverRadioButton.Name = "InverseSpilloverRadioButton";
            this.InverseSpilloverRadioButton.Size = new System.Drawing.Size(103, 17);
            this.InverseSpilloverRadioButton.TabIndex = 2;
            this.InverseSpilloverRadioButton.TabStop = true;
            this.InverseSpilloverRadioButton.Text = "Inverse Spillover";
            this.InverseSpilloverRadioButton.UseVisualStyleBackColor = true;
            // 
            // SpilloverTextBox
            // 
            this.SpilloverTextBox.Location = new System.Drawing.Point(9, 106);
            this.SpilloverTextBox.Name = "SpilloverTextBox";
            this.SpilloverTextBox.Size = new System.Drawing.Size(106, 20);
            this.SpilloverTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Matrices included in overlap calculation";
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(130, 106);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(72, 20);
            this.SubmitButton.TabIndex = 5;
            this.SubmitButton.Text = "OK";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // NetworkSpilloverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 134);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SpilloverTextBox);
            this.Controls.Add(this.InverseSpilloverRadioButton);
            this.Controls.Add(this.ReciprocatedSpilloverRadioButton);
            this.Controls.Add(this.MatchedSpilloverRadioButton);
            this.Name = "NetworkSpilloverForm";
            this.Text = "NetworkSpilloverForm";
            this.Load += new System.EventHandler(this.NetworkSpilloverForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton MatchedSpilloverRadioButton;
        private System.Windows.Forms.RadioButton ReciprocatedSpilloverRadioButton;
        private System.Windows.Forms.RadioButton InverseSpilloverRadioButton;
        private System.Windows.Forms.TextBox SpilloverTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SubmitButton;
    }
}