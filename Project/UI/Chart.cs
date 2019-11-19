
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulate;
using Window;
using test.Scripts;

namespace test.UI
{
    public partial class Chart : Form
    {
        public Chart()
        {
            InitializeComponent();
        }

        private void Chart_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.btnHistogram.Checked = true;
            this.panel1.BackColor = Color.FromArgb(227, 168, 105);
        }

        private void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            //this.Hide();
            (this.Owner as newUI).chBoxChart.Checked = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Instance._out != null&&this.Owner!=null)
            {
                string s1 = "出口ID\r\n\r\n";
                string s2 = "已疏散人数\r\n\r\n";
                string s3 = "所占比例\r\n\r\n";
                for (int i = 0; i < Instance._out.Length; i++)
                {
                    this.chart1.Series[0].Points.AddXY((i + 1), Instance._outAgentCount[i]);
                    //this.chart2.Series[0].Points.Add((i + 1), Instance._outAgentCount[i]);
                    //s += "  " + (i + 1).ToString() + "      " + Instance._outAgentCount[i] + "      " + (Instance._outAgentCount[i] * 100 / Sample.numsPeople).ToString() + "%\r\n";
                    s1 += " " + (i + 1).ToString()+"\r\n";
                    s2 +="   "+ Instance._outAgentCount[i] + "\r\n";
                    s3 += "   "+(Instance._outAgentCount[i] * 100 / (Sample.numsPeople-(this.Owner as newUI).agentsOutCount)).ToString() + "%\r\n";
                }
                this.label1.Text = s1;
                this.label2.Text = s2;
                this.label3.Text = s3;
            }
        }

        private void btnMessage_CheckedChanged(object sender, EventArgs e)
        {
            if(btnMessage.Checked==true)
            {
                this.panel1.Show();
            }
            else
            {
                this.panel1.Hide();
            }
        }

        private void btnHistogram_CheckedChanged(object sender, EventArgs e)
        {
            if(btnHistogram.Checked==true)
            {
                this.chart1.Show();
            }
            else
            {
                this.chart1.Hide();
            }
        }

      
    }
}
