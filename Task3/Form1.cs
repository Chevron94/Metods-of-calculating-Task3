using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Task3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Result.RowCount = 12;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result.Rows.Clear();
            Result.RowCount = 12;
            int[] dimention = { 10, 30, 50 };
            int[] range = { 2, 50 };
            int[] eps = { -5, -8 };
            double avg_lambda=0, avg_vec=0, r=0;
            int iter=0;
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j<2; j++)
                {
                    for(int k = 0; k<2; k++)
                    {
                        Solver slv = new Solver(dimention[i], range[j], 1000, eps[k]);
                        slv.FormAnswer(50, ref avg_lambda, ref avg_vec, ref r, ref iter);
                        Result.Rows[count].Cells[4].Value = avg_lambda.ToString("E2");
                        Result.Rows[count].Cells[5].Value = avg_vec.ToString("E2");
                        Result.Rows[count].Cells[6].Value = r.ToString("E2");
                        Result.Rows[count].Cells[7].Value = iter;
                        Result.Rows[count].Cells[0].Value = count+1;
                        Result.Rows[count].Cells[1].Value = dimention[i];
                        Result.Rows[count].Cells[2].Value = "[ -"+range[j].ToString()+" ; "+ range[j].ToString() + " ]";
                        Result.Rows[count].Cells[3].Value = Math.Pow(10, eps[k]).ToString("E2"); 
                        count++;
                        Application.DoEvents();

                    }
                }
                
            }

        }
    }
}
