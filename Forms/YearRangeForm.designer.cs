namespace NetworkGUI
{
    partial class YearRangeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.firstLabel = new System.Windows.Forms.Label();
            this.goButton = new System.Windows.Forms.Button();
            this.secondLabel = new System.Windows.Forms.Label();
            this.fromText = new System.Windows.Forms.TextBox();
            this.toText = new System.Windows.Forms.TextBox();
            this.cohesionCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // firstLabel
            // 
            this.firstLabel.AutoSize = true;
            this.firstLabel.Location = new System.Drawing.Point(9, 10);
            this.firstLabel.Name = "firstLabel";
            this.firstLabel.Size = new System.Drawing.Size(83, 13);
            this.firstLabel.TabIndex = 0;
            this.firstLabel.Text = "Save years from";
            this.firstLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(261, 5);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(41, 22);
            this.goButton.TabIndex = 1;
            this.goButton.Text = "&Save";
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // secondLabel
            // 
            this.secondLabel.AutoSize = true;
            this.secondLabel.Location = new System.Drawing.Point(166, 10);
            this.secondLabel.Name = "secondLabel";
            this.secondLabel.Size = new System.Drawing.Size(16, 13);
            this.secondLabel.TabIndex = 2;
            this.secondLabel.Text = "to";
            // 
            // fromText
            // 
            this.fromText.Location = new System.Drawing.Point(102, 7);
            this.fromText.Multiline = true;
            this.fromText.Name = "fromText";
            this.fromText.Size = new System.Drawing.Size(57, 20);
            this.fromText.TabIndex = 3;
            // 
            // toText
            // 
            this.toText.Location = new System.Drawing.Point(187, 7);
            this.toText.Multiline = true;
            this.toText.Name = "toText";
            this.toText.Size = new System.Drawing.Size(57, 20);
            this.toText.TabIndex = 4;
            // 
            // cohesionCheckBox
            // 
            this.cohesionCheckBox.AutoSize = true;
            this.cohesionCheckBox.Location = new System.Drawing.Point(13, 37);
            this.cohesionCheckBox.Name = "cohesionCheckBox";
            this.cohesionCheckBox.Size = new System.Drawing.Size(121, 17);
            this.cohesionCheckBox.TabIndex = 5;
            this.cohesionCheckBox.Text = "Save cohesion data";
            this.cohesionCheckBox.UseVisualStyleBackColor = true;
            this.cohesionCheckBox.CheckedChanged += new System.EventHandler(this.cohesionCheckBox_CheckedChanged);
            // 
            // YearRangeForm
            // 
            this.AcceptButton = this.goButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(309, 59);
            this.Controls.Add(this.cohesionCheckBox);
            this.Controls.Add(this.toText);
            this.Controls.Add(this.fromText);
            this.Controls.Add(this.secondLabel);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.firstLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "YearRangeForm";
            this.Text = "Year Range to Save";
            this.Load += new System.EventHandler(this.YearRangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label firstLabel;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label secondLabel;
        private System.Windows.Forms.TextBox fromText;
        private System.Windows.Forms.TextBox toText;
        private System.Windows.Forms.CheckBox cohesionCheckBox;
    }
}