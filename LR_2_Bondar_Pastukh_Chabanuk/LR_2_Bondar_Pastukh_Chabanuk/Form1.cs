using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LR_2_Bondar_Pastukh_Chabanuk
{
    public partial class Form1 : Form
    {
        int?[,] arr = new int?[9, 9];
        TextBox[,] tbArr;
       
        bool initializing;

        public Form1()
        {
            InitializeComponent();

            tbArr = new TextBox[9, 9]
            {
                { textBox1, textBox2, textBox3,   textBox18, textBox17, textBox16,   textBox27, textBox26, textBox25 },
                { textBox4, textBox5, textBox6,   textBox15, textBox14, textBox13,   textBox24, textBox23, textBox22 },
                { textBox7, textBox8, textBox9,   textBox12, textBox11, textBox10,   textBox21, textBox20, textBox19 },

                { textBox36, textBox35, textBox34,   textBox45, textBox44, textBox43,   textBox54, textBox53, textBox52 },
                { textBox33, textBox32, textBox31,   textBox42, textBox41, textBox40,   textBox51, textBox50, textBox49 },
                { textBox30, textBox29, textBox28,   textBox39, textBox38, textBox37,   textBox48, textBox47, textBox46 },

                { textBox63, textBox62, textBox61,   textBox72, textBox71, textBox70,   textBox81, textBox80, textBox79 },
                { textBox60, textBox59, textBox58,   textBox69, textBox68, textBox67,   textBox78, textBox77, textBox76 },
                { textBox57, textBox56, textBox55,   textBox66, textBox65, textBox64,   textBox75, textBox74, textBox73 }
            };
            foreach (TextBox tb in tbArr)
                tb.ForeColor = SystemColors.Highlight;
            InitBoard();
            foreach (TextBox tb in tbArr)
            {
                tb.TextChanged += textBox_TextChanged;
                tb.KeyPress += textBox_KeyPress;
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;

            if (char.IsDigit(e.KeyChar))
            {
                if (e.KeyChar == '0')
                    e.Handled = true;

                if (!char.IsControl(e.KeyChar) && tb.Text.Length >= 1)
                    e.Handled = true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (initializing)
                return;

            bool win = true;
            bool[,] errors = new bool[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int value;
                    if (!int.TryParse(tbArr[i, j].Text, out value) || value < 1 || value > 9)
                    {
                        arr[i, j] = null;
                        errors[i, j] = tbArr[i, j].Text != "";
                        win = false;
                    }
                    else
                    {
                        arr[i, j] = value;
                        errors[i, j] = false;
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j1 = 0; j1 < 9; j1++)
                {
                    if (arr[i, j1] == null) continue;
                    for (int j2 = j1 + 1; j2 < 9; j2++)
                    {
                        if (arr[i, j2] != null && arr[i, j1] == arr[i, j2])
                        {
                            errors[i, j1] = true;
                            errors[i, j2] = true;
                            win = false;
                        }
                    }
                }
            }

            for (int j = 0; j < 9; j++)
            {
                for (int i1 = 0; i1 < 9; i1++)
                {
                    if (arr[i1, j] == null) continue;
                    for (int i2 = i1 + 1; i2 < 9; i2++)
                    {
                        if (arr[i2, j] != null && arr[i1, j] == arr[i2, j])
                        {
                            errors[i1, j] = true;
                            errors[i2, j] = true;
                            win = false;
                        }
                    }
                }
            }

            for (int blockRow = 0; blockRow < 3; blockRow++)
            {
                for (int blockCol = 0; blockCol < 3; blockCol++)
                {
                    for (int i1 = 0; i1 < 3; i1++)
                    {
                        for (int j1 = 0; j1 < 3; j1++)
                        {
                            int r1 = blockRow * 3 + i1;
                            int c1 = blockCol * 3 + j1;
                            if (arr[r1, c1] == null) continue;

                            for (int i2 = 0; i2 < 3; i2++)
                            {
                                for (int j2 = 0; j2 < 3; j2++)
                                {
                                    int r2 = blockRow * 3 + i2;
                                    int c2 = blockCol * 3 + j2;
                                    if ((r1 == r2 && c1 == c2) || arr[r2, c2] == null) continue;
                                    if (arr[r1, c1] == arr[r2, c2])
                                    {
                                        errors[r1, c1] = true;
                                        errors[r2, c2] = true;
                                        win = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    tbArr[i, j].BackColor = errors[i, j] ? Color.IndianRed : SystemColors.Window;

            if (win)
            {
                foreach (TextBox tb in tbArr)
                {
                    tb.BackColor = Color.MediumSeaGreen;
                    tb.ReadOnly = true;
                }
            }
        }

        void InitBoard()
        {
            initializing = true;

            arr = new int?[9, 9];

            SolveSudoku();

            Random rnd = new Random();
            int hiddenCells = 40;

            while (hiddenCells > 0)
            {
                int i = rnd.Next(0, 9);
                int j = rnd.Next(0, 9);
                if (arr[i, j] != null)
                {
                    arr[i, j] = null;
                    hiddenCells--;
                }
            }


            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (arr[i, j] != null)
                    {
                        tbArr[i, j].Text = arr[i, j].ToString();
                        tbArr[i, j].ReadOnly = true;
                        tbArr[i, j].ForeColor = SystemColors.WindowText;
                        tbArr[i, j].BackColor = SystemColors.Window;
                    }
                }
            }

            initializing = false;

        }

        bool SolveSudoku(int row = 0, int col = 0)
        {
            if (row == 9)
                return true;

            int nextRow = (col == 8) ? row + 1 : row;
            int nextCol = (col + 1) % 9;

            List<int> numbers = Enumerable.Range(1, 9).OrderBy(n => Guid.NewGuid()).ToList();

            foreach (int num in numbers)
            {
                if (IsSafe(row, col, num, arr))
                {
                    arr[row, col] = num;
                    if (SolveSudoku(nextRow, nextCol))
                        return true;
                    arr[row, col] = null;
                }
            }

            return false;
        }

        bool IsSafe(int row, int col, int num, int?[,] board)
        {
            for (int i = 0; i < 9; i++)
                if (board[row, i] == num || board[i, col] == num)
                    return false;

            int boxRow = row / 3 * 3;
            int boxCol = col / 3 * 3;
            for (int i = boxRow; i < boxRow + 3; i++)
                for (int j = boxCol; j < boxCol + 3; j++)
                    if (board[i, j] == num)
                        return false;

            return true;
        }

        private void новаГраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            arr = new int?[9, 9];

            foreach (TextBox tb in tbArr)
            {
                tb.Text = "";
                tb.BackColor = SystemColors.Window;
                tb.ForeColor = SystemColors.Highlight;
                tb.ReadOnly = false;
            }

            InitBoard();
        }

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryReader file = null;

                try
                {
                    file = new BinaryReader(new FileStream(openFileDialog.FileName, FileMode.Open));

                    arr = new int?[9, 9];
                    for (int i = 0; i < 9; ++i)
                    {
                        for (int j = 0; j < 9; ++j)
                        {
                            int value = file.ReadInt32();
                            arr[i, j] = (value == 0 ? (int?)null : (int?)value);
                            tbArr[i, j].Text = (value == 0 ? "" : value.ToString());
                        }
                    }

                    bool win = true;
                    for (int i = 0; i < 9; ++i)
                    {
                        for (int j = 0; j < 9; ++j)
                        {
                            bool value = file.ReadBoolean();
                            tbArr[i, j].ReadOnly = value;
                            tbArr[i, j].ForeColor = (value ? SystemColors.WindowText : SystemColors.Highlight);
                            tbArr[i, j].BackColor = SystemColors.Window;

                            if (!value)
                                win = false;
                        }
                    }

                    if (win)
                        foreach (TextBox tb in tbArr)
                            tb.BackColor = Color.MediumSeaGreen;
                }
                catch
                { }
                finally
                {
                    file?.Close();
                }
            }
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter file = null;

                try
                {
                    file = new BinaryWriter(new FileStream(saveFileDialog.FileName, FileMode.Create));

                    foreach (int? value in arr)
                        if (value == null)
                            file.Write(0);
                        else
                            file.Write((int)value);

                    foreach (TextBox tb in tbArr)
                        file.Write(tb.ReadOnly);
                }
                catch
                { }
                finally
                {
                    file?.Close();
                }
            }
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
