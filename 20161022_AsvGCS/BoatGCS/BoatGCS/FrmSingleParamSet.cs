using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;


namespace BoatGCS
{
    public partial class FrmSingleParamSet : Form
    {
        public FrmSingleParamSet()
        {
            InitializeComponent();
        }

        private void FrmSingleParamSet_Load(object sender, EventArgs e)
        {
            switch (FrmGCSMain.gbl_var.paramset_id)
            { 
                case 1://设置航点高度
                    labelParamName.Text = "航点高度[m]";
                    tBoxParamValue.Text = FrmGCSMain.totalWPlist[FrmGCSMain.actWP_index].Alt.ToString();
                    break;
                case 2://设置插入航点位置
                    labelParamName.Text = "在哪个航点后";
                    tBoxParamValue.Text = "";
                    break;
            }
        }

        private void btnParamSetOk_Click(object sender, EventArgs e)
        {
            switch (FrmGCSMain.gbl_var.paramset_id)
            {
                case 1://设置航点高度
                    FrmGCSMain.totalWPlist[FrmGCSMain.actWP_index].Alt = int.Parse(tBoxParamValue.Text);
                    break;
                case 2://设置航点插入位置
                    if (tBoxParamValue.Text.ToString() == "H")//若输入为H
                    {
                        FrmGCSMain.actWP_index = 0;
                    }
                    else if ((int.Parse(tBoxParamValue.Text) >= FrmGCSMain.totalWPlist.Count)
                        || (int.Parse(tBoxParamValue.Text)<0))
                    {
                        FrmGCSMain.actWP_index = FrmGCSMain.totalWPlist.Count-1;
                    }
                    else
                    {
                        FrmGCSMain.actWP_index = int.Parse(tBoxParamValue.Text);
                    }
                    break;
            }
            Close();
        }

        private void btnParamSetCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
