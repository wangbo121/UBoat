namespace BoatGCS
{
    partial class FrmFlightPlanList
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabelWPCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tSStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelDistTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelDistMax = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGViewFlightPlan = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGViewFlightPlan)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelWPCount,
            this.tSStatusLabel2,
            this.statusLabelDistTotal,
            this.statusLabelDistMax});
            this.statusStrip1.Location = new System.Drawing.Point(0, 240);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(525, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelWPCount
            // 
            this.statusLabelWPCount.AutoSize = false;
            this.statusLabelWPCount.BackColor = System.Drawing.SystemColors.Desktop;
            this.statusLabelWPCount.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.statusLabelWPCount.Name = "statusLabelWPCount";
            this.statusLabelWPCount.Size = new System.Drawing.Size(80, 17);
            this.statusLabelWPCount.Text = "航点数: 10";
            this.statusLabelWPCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tSStatusLabel2
            // 
            this.tSStatusLabel2.Name = "tSStatusLabel2";
            this.tSStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // statusLabelDistTotal
            // 
            this.statusLabelDistTotal.AutoSize = false;
            this.statusLabelDistTotal.BackColor = System.Drawing.SystemColors.Desktop;
            this.statusLabelDistTotal.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.statusLabelDistTotal.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.statusLabelDistTotal.Name = "statusLabelDistTotal";
            this.statusLabelDistTotal.Size = new System.Drawing.Size(120, 17);
            this.statusLabelDistTotal.Text = "总航程[m]: 2300";
            this.statusLabelDistTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelDistMax
            // 
            this.statusLabelDistMax.AutoSize = false;
            this.statusLabelDistMax.BackColor = System.Drawing.SystemColors.Desktop;
            this.statusLabelDistMax.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.statusLabelDistMax.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.statusLabelDistMax.Name = "statusLabelDistMax";
            this.statusLabelDistMax.Size = new System.Drawing.Size(150, 17);
            this.statusLabelDistMax.Text = "离家最远距离[m]: 100";
            this.statusLabelDistMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 190);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(525, 50);
            this.panel1.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(357, 15);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(438, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGViewFlightPlan);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(525, 190);
            this.panel2.TabIndex = 3;
            // 
            // dataGViewFlightPlan
            // 
            this.dataGViewFlightPlan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGViewFlightPlan.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            this.dataGViewFlightPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGViewFlightPlan.Location = new System.Drawing.Point(0, 0);
            this.dataGViewFlightPlan.Name = "dataGViewFlightPlan";
            this.dataGViewFlightPlan.RowTemplate.Height = 23;
            this.dataGViewFlightPlan.Size = new System.Drawing.Size(525, 190);
            this.dataGViewFlightPlan.TabIndex = 1;
            this.dataGViewFlightPlan.MouseLeave += new System.EventHandler(this.dataGViewFlightPlan_MouseLeave);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "序号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 40;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "标志";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 40;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "经度";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "纬度";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "航点距离[m]";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "家距离[m]";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // FrmFlightPlanList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 262);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "FrmFlightPlanList";
            this.Text = "规划航点列表";
            this.Load += new System.EventHandler(this.FrmFlightPlanList_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGViewFlightPlan)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelWPCount;
        private System.Windows.Forms.ToolStripStatusLabel tSStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelDistMax;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGViewFlightPlan;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelDistTotal;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}