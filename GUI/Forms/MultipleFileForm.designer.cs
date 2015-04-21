namespace NetworkGUI
{
    partial class MultipleFileForm
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
            this.fileList = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
// 
// fileList
// 
            this.fileList.FormattingEnabled = true;
            this.fileList.HorizontalScrollbar = true;
            this.fileList.Location = new System.Drawing.Point(13, 13);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(553, 212);
            this.fileList.TabIndex = 0;
// 
// addButton
// 
            this.addButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.addButton.Location = new System.Drawing.Point(13, 232);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(133, 26);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "&Add Matrix File";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
// 
// removeButton
// 
            this.removeButton.Location = new System.Drawing.Point(153, 232);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(133, 26);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "&Remove Matrix File";
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
// 
// cancelButton
// 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(433, 232);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(133, 26);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
// 
// okButton
// 
            this.okButton.Location = new System.Drawing.Point(293, 232);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(133, 26);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
// 
// openFileDialog
// 
            this.openFileDialog.Filter = "CSV files|*.csv|Text files|*.txt|All files|*.*";
// 
// MultipleFileForm
// 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(579, 263);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.fileList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultipleFileForm";
            this.Text = "Load From Multiple Matrix Files";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox fileList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}