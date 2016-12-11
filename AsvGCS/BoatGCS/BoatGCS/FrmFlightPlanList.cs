using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoatGCS
{
    public partial class FrmFlightPlanList : Form
    {
        public FrmFlightPlanList()
        {
            InitializeComponent();
        }

        private void FrmFlightPlanList_Load(object sender, EventArgs e)
        {
            double dist_to_home=0.0;
            double dist_to_last = 0.0;
            double dist_total = 0.0;
            double dist_to_home_max=0.0;
            //将totalWPlist中数据显示在列表中
            if(FrmGCSMain.totalWPlist.Count>0)
            {
                for (int i = 0; i < FrmGCSMain.totalWPlist.Count; i++)
                {
                    dataGViewFlightPlan.Rows.Add();
                    dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[0].Value = i;
                    dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[1].Value = FrmGCSMain.totalWPlist[i].Tag;
                    dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[2].Value = FrmGCSMain.totalWPlist[i].Lat.ToString("0.000000");
                    dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[3].Value = FrmGCSMain.totalWPlist[i].Lng.ToString("0.000000");
                    if(i>0)
                    {
                        dist_to_last = FrmGCSMain.totalWPlist[i].GetDistance(FrmGCSMain.totalWPlist[i - 1]);
                        dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[4].Value = dist_to_last.ToString("0.0");
                        dist_to_home = FrmGCSMain.totalWPlist[i].GetDistance(FrmGCSMain.totalWPlist[0]);
                        dataGViewFlightPlan.Rows[dataGViewFlightPlan.RowCount - 2].Cells[5].Value = dist_to_home.ToString("0.0");
                        dist_total += dist_to_last;
                        if (dist_to_home_max < dist_to_home) dist_to_home_max = dist_to_home;
                        if (i == (FrmGCSMain.totalWPlist.Count - 1))
                        {
                            dist_total += dist_to_home;
                        }
                    }
                }
            }
            statusLabelWPCount.Text = "航点数： "+FrmGCSMain.totalWPlist.Count.ToString();
            statusLabelDistTotal.Text = "总航程[m]: " + dist_total.ToString("0.0");
            statusLabelDistMax.Text = "离家最远距离[m]: " + dist_to_home_max.ToString("0.0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGViewFlightPlan_MouseLeave(object sender, EventArgs e)
        {
        }
    }
}
