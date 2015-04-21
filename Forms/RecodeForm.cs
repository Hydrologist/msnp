using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetworkGUI.Forms
{
    public partial class RecodeForm : Form
    {
        public enum RecodeType
        {
            AllEmpty, IncompleteRow, Valid, Invalid
        }

        public struct Row
        {
            public double from;
            public double to;
            public double value;
        }

        private const string MIN = "min";
        private const string MAX = "max";
        private const string ELSE = "else";

        private RecodeType recodeType;
        private Row row1;
        private Row row2;
        private Row row3;
        private Row row4;

        private List<Row> validRows;

        public RecodeForm()
        {
            InitializeComponent();
            row1 = row2 = row3 = row4 = new Row();
            validRows = new List<Row>();
        }

        public bool isValid()
        {
            RecodeType row1, row2, row3, row4;
            if (textBox1.Text == "" && textBox2.Text == "" && textBox3.Text == "")
                row1 = RecodeType.AllEmpty;
            else if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                if (textBox1.Text.ToLower() == "else" && textBox3.Text != "")
                {
                    row1 = RecodeType.Valid;
                    validRows.Add(Row1);
                }
                else
                    row1 = RecodeType.IncompleteRow;
            }
            else
            {
                row1 = RecodeType.Valid;
                validRows.Add(Row1);
            }

            if (textBox4.Text == "" && textBox5.Text == "" && textBox6.Text == "")
                row2 = RecodeType.AllEmpty;
            else if (textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
            {
                if (textBox4.Text.ToLower() == "else" && textBox6.Text != "")
                {
                    row2 = RecodeType.Valid;
                    validRows.Add(Row2);
                }
                else
                    row2 = RecodeType.IncompleteRow;
            }
            else
            {
                row2 = RecodeType.Valid;
                validRows.Add(Row2);
            }

            if (textBox7.Text == "" && textBox8.Text == "" && textBox9.Text == "")
                row3 = RecodeType.AllEmpty;
            else if (textBox7.Text == "" || textBox8.Text == "" || textBox9.Text == "")
            {
                if (textBox7.Text.ToLower() == "else" && textBox9.Text != "")
                {
                    row3 = RecodeType.Valid;
                    validRows.Add(Row3);
                }
                else
                    row3 = RecodeType.IncompleteRow;
            }
            else
            {
                row3 = RecodeType.Valid;
                validRows.Add(Row3);
            }
            if (textBox10.Text == "" && textBox11.Text == "" && textBox12.Text == "")
                row4 = RecodeType.AllEmpty;
            else if (textBox10.Text == "" || textBox11.Text == "" || textBox12.Text == "")
            {
                if (textBox10.Text.ToLower() == "else" && textBox12.Text != "")
                {
                    row4 = RecodeType.Valid;
                    validRows.Add(Row4);
                }
                else
                    row4 = RecodeType.IncompleteRow;
            }                
            else
            {
                row4 = RecodeType.Valid;
                validRows.Add(Row4);
            }

            if (row1 == RecodeType.AllEmpty && row2 == RecodeType.AllEmpty &&
                row3 == RecodeType.AllEmpty && row4 == RecodeType.AllEmpty)
                return false;
            else if (row1 == RecodeType.IncompleteRow || row2 == RecodeType.IncompleteRow ||
                row3 == RecodeType.IncompleteRow || row4 == RecodeType.IncompleteRow)
                return false;
            else
                return true;
            
        }

        public void ClearTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
        }

        private double parseTextBox(string text)
        {
            double value = 0.0;
            if (text.ToLower() == MIN)
            {
                value = double.MinValue;
            }
            else if (text.ToLower() == MAX)
            {
                value = double.MaxValue;
            }
            else if (text.ToLower() == ELSE)
            {
                value = double.Epsilon;
            }
            else
            {
                try
                {
                    value = double.Parse(text);
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
            return value;
        }

        public Row Row1
        {   
            get 
            {
                row1.from = parseTextBox(textBox1.Text);
                if (row1.from != double.Epsilon)
                    row1.to = parseTextBox(textBox2.Text);
                row1.value = parseTextBox(textBox3.Text);
                return row1;
            }
        }

        public Row Row2
        {
            get
            {
                row2.from = parseTextBox(textBox4.Text);
                if (row2.from != double.Epsilon)
                    row2.to = parseTextBox(textBox5.Text);
                row2.value = parseTextBox(textBox6.Text);
                return row2;
            }
        }

        public Row Row3
        {
            get
            {
                row3.from = parseTextBox(textBox7.Text);
                if (row3.from != double.Epsilon)
                    row3.to = parseTextBox(textBox8.Text);
                row3.value = parseTextBox(textBox9.Text);
                return row3;
            }
        }

        public Row Row4
        {
            get
            {
                row4.from = parseTextBox(textBox10.Text);
                if (row4.from != double.Epsilon)
                    row4.to = parseTextBox(textBox11.Text);
                row4.value = parseTextBox(textBox12.Text);
                return row4;
            }
        }

        public List<Row> ValidRows
        {
            get { return validRows; }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
