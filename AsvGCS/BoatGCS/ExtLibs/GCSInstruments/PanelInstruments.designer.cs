namespace GCSInstruments
{
    partial class PanelInstruments
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this._ctlStance = new GCSInstruments.ControlStance();
            this._ctlPlateHeight = new GCSInstruments.ControlDialPlate();
            this._ctlPlateSpeed = new GCSInstruments.ControlDialPlate();
            this.SuspendLayout();
            // 
            // _ctlStance
            // 
            this._ctlStance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ctlStance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._ctlStance.Location = new System.Drawing.Point(331, 3);
            this._ctlStance.Name = "_ctlStance";
            this._ctlStance.Size = new System.Drawing.Size(160, 160);
            this._ctlStance.TabIndex = 2;
            // 
            // _ctlPlateHeight
            // 
            this._ctlPlateHeight.AlarmValue = 100;
            this._ctlPlateHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ctlPlateHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._ctlPlateHeight.FingerStyle = GCSInstruments.ControlDialPlate.FingerStyles.style3;
            this._ctlPlateHeight.LinearColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))),
        System.Drawing.Color.White};
            this._ctlPlateHeight.Location = new System.Drawing.Point(167, 3);
            this._ctlPlateHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._ctlPlateHeight.MaxValue = 80;
            this._ctlPlateHeight.Name = "_ctlPlateHeight";
            this._ctlPlateHeight.ScaleValueColor = System.Drawing.Color.MidnightBlue;
            this._ctlPlateHeight.ScaleValueFont = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ctlPlateHeight.Size = new System.Drawing.Size(160, 160);
            this._ctlPlateHeight.TabIndex = 1;
            this._ctlPlateHeight.Title = "高度";
            this._ctlPlateHeight.Unit = "m";
            this._ctlPlateHeight.ValueColor = System.Drawing.Color.Black;
            // 
            // _ctlPlateSpeed
            // 
            this._ctlPlateSpeed.AlarmValue = 100;
            this._ctlPlateSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ctlPlateSpeed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._ctlPlateSpeed.FingerStyle = GCSInstruments.ControlDialPlate.FingerStyles.style1;
            this._ctlPlateSpeed.LinearColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))),
        System.Drawing.Color.White};
            this._ctlPlateSpeed.Location = new System.Drawing.Point(3, 3);
            this._ctlPlateSpeed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._ctlPlateSpeed.MaxValue = 32;
            this._ctlPlateSpeed.Name = "_ctlPlateSpeed";
            this._ctlPlateSpeed.ScaleValueColor = System.Drawing.Color.Teal;
            this._ctlPlateSpeed.ScaleValueFont = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ctlPlateSpeed.Size = new System.Drawing.Size(160, 160);
            this._ctlPlateSpeed.TabIndex = 0;
            this._ctlPlateSpeed.Title = "航速";
            this._ctlPlateSpeed.Unit = "knots";
            this._ctlPlateSpeed.ValueColor = System.Drawing.Color.Blue;
            // 
            // PanelInstruments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ctlStance);
            this.Controls.Add(this._ctlPlateHeight);
            this.Controls.Add(this._ctlPlateSpeed);
            this.Name = "PanelInstruments";
            this.Size = new System.Drawing.Size(495, 167);
            this.ResumeLayout(false);

        }

        #endregion

        private ControlStance _ctlStance;
        private ControlDialPlate _ctlPlateHeight;
        private ControlDialPlate _ctlPlateSpeed;

    }
}
