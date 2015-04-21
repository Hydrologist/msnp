namespace NetworkGUI
{
    partial class JumpToForm
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
            this.yearLabel = new System.Windows.Forms.Label();
            this.yearTextBox = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
// 
// yearLabel
// 
            this.yearLabel.AutoSize = true;
            this.yearLabel.Location = new System.Drawing.Point(4, 13);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(31, 14);
            this.yearLabel.TabIndex = 0;
            this.yearLabel.Text = "&Year:";
// 
// yearTextBox
// 
            this.yearTextBox.Location = new System.Drawing.Point(42, 10);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(135, 20);
            this.yearTextBox.TabIndex = 1;
// 
// goButton
// 
            this.goButton.Location = new System.Drawing.Point(184, 10);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(50, 21);
            this.goButton.TabIndex = 2;
            this.goButton.Text = "&Go";
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
// 
// JumpToForm
// 
            this.AcceptButton = this.goButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(239, 39);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.yearLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JumpToForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Jump To Year";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.Button goButton;
    }
}