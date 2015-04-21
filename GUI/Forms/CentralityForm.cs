using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI
{
    public partial class CentralityForm : Form
    {
        private Label SijLabel;
        private TextBox sijmax;
        private CheckBox zeroDiagonal;
        private CheckBox countMember;
        private CheckBox avg; private Button goButton;
    
        public double Sijmax
        {
            get
            {
                return double.Parse(sijmax.Text);
            }
            set
            {
                sijmax.Text = value.ToString();
            }
        }

        public bool CountMember
        {
            get { return countMember.Checked; }
            set { countMember.Checked = value; }
        }

        public bool ZeroDiagonal
        {
            get { return zeroDiagonal.Checked; }
            set { zeroDiagonal.Checked = value; }
        }

        public bool Avg
        {
            get { return avg.Checked; }
            set { avg.Checked = value; }
        }

        public CentralityForm()
        {
            InitializeComponent();
        }

        private void reach_TextChanged(object sender, EventArgs e)
        {

        }

        private void goButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                double.Parse(sijmax.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Sij(max) is not a valid number!", "Error!");
                return;
            }
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SijLabel = new System.Windows.Forms.Label();
            this.sijmax = new System.Windows.Forms.TextBox();
            this.zeroDiagonal = new System.Windows.Forms.CheckBox();
            this.countMember = new System.Windows.Forms.CheckBox();
            this.goButton = new System.Windows.Forms.Button();
            this.avg = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SijLabel
            // 
            this.SijLabel.AutoSize = true;
            this.SijLabel.Location = new System.Drawing.Point(12, 9);
            this.SijLabel.Name = "SijLabel";
            this.SijLabel.Size = new System.Drawing.Size(46, 13);
            this.SijLabel.TabIndex = 0;
            this.SijLabel.Text = "Sij(max):";
            // 
            // sijmax
            // 
            this.sijmax.Location = new System.Drawing.Point(64, 6);
            this.sijmax.Name = "sijmax";
            this.sijmax.Size = new System.Drawing.Size(74, 20);
            this.sijmax.TabIndex = 1;
            this.sijmax.Text = "1.0";
            // 
            // zeroDiagonal
            // 
            this.zeroDiagonal.AutoSize = true;
            this.zeroDiagonal.Location = new System.Drawing.Point(12, 55);
            this.zeroDiagonal.Name = "zeroDiagonal";
            this.zeroDiagonal.Size = new System.Drawing.Size(120, 17);
            this.zeroDiagonal.TabIndex = 2;
            this.zeroDiagonal.Text = "Zero Main Diagonal";
            this.zeroDiagonal.UseVisualStyleBackColor = true;
            // 
            // countMember
            // 
            this.countMember.AutoSize = true;
            this.countMember.Location = new System.Drawing.Point(12, 32);
            this.countMember.Name = "countMember";
            this.countMember.Size = new System.Drawing.Size(216, 17);
            this.countMember.TabIndex = 3;
            this.countMember.Text = "Count Member\'s Betweenness With Self";
            this.countMember.UseVisualStyleBackColor = true;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(64, 109);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(97, 23);
            this.goButton.TabIndex = 4;
            this.goButton.Text = "&Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click_1);
            // 
            // avg
            // 
            this.avg.AutoSize = true;
            this.avg.Location = new System.Drawing.Point(12, 78);
            this.avg.Name = "avg";
            this.avg.Size = new System.Drawing.Size(113, 17);
            this.avg.TabIndex = 5;
            this.avg.Text = "Average Centrality";
            this.avg.UseVisualStyleBackColor = true;
            // 
            // CentralityForm
            // 
            this.AcceptButton = this.goButton;
            this.ClientSize = new System.Drawing.Size(232, 144);
            this.Controls.Add(this.avg);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.countMember);
            this.Controls.Add(this.zeroDiagonal);
            this.Controls.Add(this.sijmax);
            this.Controls.Add(this.SijLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CentralityForm";
            this.Text = "Centrality Indices";
            this.Load += new System.EventHandler(this.CentralityForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CentralityForm_Load(object sender, EventArgs e)
        {

        }

    }
}