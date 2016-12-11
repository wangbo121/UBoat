namespace BoatGCS
{
    partial class FrmSingleParamSet
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
            this.tBoxParamValue = new System.Windows.Forms.TextBox();
            this.labelParamName = new System.Windows.Forms.Label();
            this.btnParamSetOk = new System.Windows.Forms.Button();
            this.btnParamSetCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tBoxParamValue
            // 
            this.tBoxParamValue.Location = new System.Drawing.Point(133, 15);
            this.tBoxParamValue.Name = "tBoxParamValue";
            this.tBoxParamValue.Size = new System.Drawing.Size(100, 21);
            this.tBoxParamValue.TabIndex = 0;
            // 
            // labelParamName
            // 
            this.labelParamName.Location = new System.Drawing.Point(12, 18);
            this.labelParamName.Name = "labelParamName";
            this.labelParamName.Size = new System.Drawing.Size(100, 23);
            this.labelParamName.TabIndex = 1;
            this.labelParamName.Text = "参数名称[单位]";
            // 
            // btnParamSetOk
            // 
            this.btnParamSetOk.Location = new System.Drawing.Point(70, 53);
            this.btnParamSetOk.Name = "btnParamSetOk";
            this.btnParamSetOk.Size = new System.Drawing.Size(75, 23);
            this.btnParamSetOk.TabIndex = 2;
            this.btnParamSetOk.Text = "确定";
            this.btnParamSetOk.UseVisualStyleBackColor = true;
            this.btnParamSetOk.Click += new System.EventHandler(this.btnParamSetOk_Click);
            // 
            // btnParamSetCancel
            // 
            this.btnParamSetCancel.Location = new System.Drawing.Point(158, 53);
            this.btnParamSetCancel.Name = "btnParamSetCancel";
            this.btnParamSetCancel.Size = new System.Drawing.Size(75, 23);
            this.btnParamSetCancel.TabIndex = 3;
            this.btnParamSetCancel.Text = "取消";
            this.btnParamSetCancel.UseVisualStyleBackColor = true;
            this.btnParamSetCancel.Click += new System.EventHandler(this.btnParamSetCancel_Click);
            // 
            // FrmSingleParamSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 91);
            this.Controls.Add(this.btnParamSetCancel);
            this.Controls.Add(this.btnParamSetOk);
            this.Controls.Add(this.labelParamName);
            this.Controls.Add(this.tBoxParamValue);
            this.Name = "FrmSingleParamSet";
            this.Text = "设置参数";
            this.Load += new System.EventHandler(this.FrmSingleParamSet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tBoxParamValue;
        private System.Windows.Forms.Label labelParamName;
        private System.Windows.Forms.Button btnParamSetOk;
        private System.Windows.Forms.Button btnParamSetCancel;
    }
}