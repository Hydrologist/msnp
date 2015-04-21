namespace NetworkGUI.Forms
{
    partial class ABMForm
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
            this.nText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.netText = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.netIdent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.minbox = new System.Windows.Forms.TextBox();
            this.stepbox = new System.Windows.Forms.TextBox();
            this.maxbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.paramsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nText
            // 
            this.nText.Enabled = false;
            this.nText.Location = new System.Drawing.Point(116, 33);
            this.nText.Name = "nText";
            this.nText.Size = new System.Drawing.Size(115, 20);
            this.nText.TabIndex = 1;
            this.nText.Text = "10";
            this.nText.Visible = false;
            this.nText.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "# of Nodes";
            this.label1.Visible = false;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "# of Networks";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // netText
            // 
            this.netText.Location = new System.Drawing.Point(116, 33);
            this.netText.Name = "netText";
            this.netText.Size = new System.Drawing.Size(115, 20);
            this.netText.TabIndex = 2;
            this.netText.Text = "10";
            this.netText.TextChanged += new System.EventHandler(this.netText_TextChanged);
            // 
            // goButton
            // 
            this.goButton.Enabled = false;
            this.goButton.Location = new System.Drawing.Point(86, 61);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(75, 23);
            this.goButton.TabIndex = 3;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Visible = false;
            this.goButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // netIdent
            // 
            this.netIdent.Location = new System.Drawing.Point(116, 5);
            this.netIdent.Name = "netIdent";
            this.netIdent.Size = new System.Drawing.Size(115, 20);
            this.netIdent.TabIndex = 0;
            this.netIdent.Text = "1";
            this.netIdent.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Starting Network ID";
            // 
            // minbox
            // 
            this.minbox.Location = new System.Drawing.Point(31, 77);
            this.minbox.Name = "minbox";
            this.minbox.Size = new System.Drawing.Size(56, 20);
            this.minbox.TabIndex = 4;
            this.minbox.Text = "10";
            this.minbox.TextChanged += new System.EventHandler(this.minbox_TextChanged);
            // 
            // stepbox
            // 
            this.stepbox.Location = new System.Drawing.Point(93, 78);
            this.stepbox.Name = "stepbox";
            this.stepbox.Size = new System.Drawing.Size(56, 20);
            this.stepbox.TabIndex = 5;
            this.stepbox.Text = "10";
            this.stepbox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // maxbox
            // 
            this.maxbox.Location = new System.Drawing.Point(155, 77);
            this.maxbox.Name = "maxbox";
            this.maxbox.Size = new System.Drawing.Size(56, 20);
            this.maxbox.TabIndex = 6;
            this.maxbox.Text = "100";
            this.maxbox.TextChanged += new System.EventHandler(this.maxbox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Min";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Step";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(165, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Network Size Parameters";
            // 
            // paramsButton
            // 
            this.paramsButton.Location = new System.Drawing.Point(86, 117);
            this.paramsButton.Name = "paramsButton";
            this.paramsButton.Size = new System.Drawing.Size(75, 23);
            this.paramsButton.TabIndex = 7;
            this.paramsButton.Text = "Go";
            this.paramsButton.UseVisualStyleBackColor = true;
            this.paramsButton.Click += new System.EventHandler(this.paramsButton_Click);
            // 
            // ABMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 204);
            this.Controls.Add(this.paramsButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxbox);
            this.Controls.Add(this.stepbox);
            this.Controls.Add(this.minbox);
            this.Controls.Add(this.netIdent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.netText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nText);
            this.Name = "ABMForm";
            this.Text = "ABM Model";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox netText;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.TextBox netIdent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox minbox;
        private System.Windows.Forms.TextBox stepbox;
        private System.Windows.Forms.TextBox maxbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button paramsButton;
    }
}