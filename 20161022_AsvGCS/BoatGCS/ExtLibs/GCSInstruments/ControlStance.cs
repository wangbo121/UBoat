using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GCSInstruments
{
    public partial class ControlStance : UserControl
    {
        public ControlStance()
        {
            InitializeComponent();
            SetValue(0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pitchAngle">俯仰角</param>
        /// <param name="rollAngle">横滚角</param>
        /// <param name="deflecionAngle">航向角</param>
        /// <returns></returns>
        public Bitmap DrawAppearance(float pitchAngle, float rollAngle, float deflecionAngle)
        {
           
            //rollAngle = -rollAngle;
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            pitchAngle = pitchAngle % 360;
            Bitmap bmp = new Bitmap(width, height);
            PointF center = new PointF(width / 2, height / 2);
            Graphics gra = Graphics.FromImage(bmp);
            gra.Clear(Color.DimGray);
            gra.SmoothingMode = SmoothingMode.HighQuality;

            gra.TranslateTransform(center.X, center.Y);
            gra.RotateTransform(rollAngle);  //roll
            gra.TranslateTransform(-center.X, -center.Y);
            Pen drawPen = new Pen(Color.Black, 2);
            gra.DrawEllipse(drawPen, 2, 2, width - 4, height - 4);
            GraphicsPath gPath = new GraphicsPath();
            Rectangle rect = new Rectangle(4, 4, width - 8, height - 8);
            gPath.StartFigure();
            gPath.AddArc(rect, pitchAngle - 180, 180 - 2 * pitchAngle);
            gra.FillPath(new SolidBrush(Color.LightSkyBlue), gPath);
            gPath.Dispose();
            gPath = new GraphicsPath();
            gPath.StartFigure();
            gPath.AddArc(rect, -pitchAngle, 180 + 2 * pitchAngle);
            gPath.CloseFigure();
            gra.FillPath(new SolidBrush(Color.DarkGoldenrod), gPath);
            //gra.FillPath(linear, gPath);


            //画正交基线
            drawPen.Color = Color.FromArgb(180, Color.White);
            drawPen.Width = 2.5f;
            gra.DrawLine(drawPen, new PointF(4, center.Y), new PointF(width - 4, center.Y));
            gra.DrawLine(drawPen, new PointF(center.X, 4), new PointF(center.X, height - 4));

            //画刻度
            double scaleRadian;
            int radius1 = (int)(center.X - 2);
            int radius2 = (int)(center.Y - 7);
            drawPen.Width = 2;
            drawPen.Color = Color.Black;
            for (int i = -60; i <= 60; i += 15)
            {

                scaleRadian = DegToRad(i);
                if (i == 0)
                {
                    gra.DrawLine(drawPen, (float)(center.X + radius1 * Math.Sin(scaleRadian)), (float)(center.Y -
                radius1 * Math.Cos(scaleRadian)), (float)(center.X + (radius2 - 3) * Math.Sin(scaleRadian)), (float)(center.Y -
                (radius2 - 3) * Math.Cos(scaleRadian)));
                    continue;
                }

                gra.DrawLine(drawPen, (float)(center.X + radius1 * Math.Sin(scaleRadian)), (float)(center.Y -
                radius1 * Math.Cos(scaleRadian)), (float)(center.X + radius2 * Math.Sin(scaleRadian)), (float)(center.Y -
                radius2 * Math.Cos(scaleRadian)));
            }

            for (int i = 20; i < center.X; i += 7)
            {
                gra.DrawLine(new Pen(new SolidBrush(Color.Yellow), 2), center.X - 5, i, center.Y + 5, i);
            }

            double rollRadian = DegToRad(0); //

            gra.DrawArc(new Pen(Color.FromArgb(150, Color.Red), 5), new Rectangle(width / 8, height / 8, width * 3 / 4, height * 3 / 4), 130, 280);
            gra.FillEllipse(new SolidBrush(Color.Red), new RectangleF(center.X - 8, center.Y - 8, 16, 16));

            gra.DrawLine(new Pen(Color.Red, 4), center,
                new PointF((float)(center.X + width * 3 / 8 * Math.Sin(rollRadian)), (float)(center.Y - height * 3 / 8 * Math.Cos(rollRadian))));
            string strPitch = (pitchAngle > 0 ? "俯" : "仰");
            strPitch += Math.Abs(Math.Round(pitchAngle)).ToString();
            string strRoll = (rollAngle > 0 ? "左" : "右");
            strRoll += Math.Abs(Math.Round(rollAngle)).ToString();
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            gra.DrawString(string.Format("{0}° {1}°", strRoll, strPitch), new Font("微软雅黑", 12), new SolidBrush(Color.Black),
                new RectangleF(center.X - 50, center.Y + 30, 100, 40), strFormat);
            gra.ResetTransform();

            return bmp;
        }

        public void SetValue(float pitch, float roll)
        {
            pictureBox1.Image = DrawAppearance(pitch, roll, 0);
        }

        private double DegToRad(double d)
        {
            return d * Math.PI / 180;
        }

        private void ControlStance_Resize(object sender, EventArgs e)
        {
            this.Height = this.Width;
            SetValue(0, 0);
        }
    }


}
