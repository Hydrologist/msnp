using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Network.Matrices;

namespace Network.GUI
{
    public sealed class DataGridViewLoader
    {
        private DataGridViewLoader() { }

        public static void LoadMatrixIntoDataGridView(DataGridView data, Matrix m)
        {
            data.Columns.Clear();
           

            for (int i = 0; i < m.ColLabels.Length; ++i)
            {
                data.Columns.Add(m.ColLabels[i], m.ColLabels[i]);

                data.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                data.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                data.Columns[i].Resizable = DataGridViewTriState.True;
                data.Columns[i].FillWeight = (float)100;
            }

            for (int row = 0; row < m.Rows; ++row)
            {
                string[] newRow = new string[m.Cols];

                for (int col = 0; col < m.Cols; ++col)
                    newRow[col] = m[row, col].ToString();
                data.Rows.Add(newRow);

                data.Rows[row].HeaderCell.Value = m.RowLabels[row];
            }
        }

        public static void LoadVectorIntoDataGridView(DataGridView data, Vector v)
        {
            data.Columns.Clear();

            data.Columns.Add("", "");
            data.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            data.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            data.Columns[0].Resizable = DataGridViewTriState.True;

            for (int row = 0; row < v.Size; ++row)
            {
                data.Rows.Add(v[row].ToString());

                data.Rows[row].HeaderCell.Value = v.Labels[row];
            }
        }
    }
}
