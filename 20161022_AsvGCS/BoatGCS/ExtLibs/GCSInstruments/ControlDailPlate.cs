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
    public partial class ControlDialPlate : UserControl
    {
        public enum FingerStyles
        {
            style1,
            style2,
            style3,
        };
        private int _maxValue = 200;
        private string _tiltle = "速度表";
        private string _unit = "km/s";
        private Bitmap _bmpFace = null;
        private int _alarmValue = 100;
        private bool _alarmState = false;
        private Color _colorAlarm = Color.Red;
        private Color _colorScaleValue = Color.Teal;
        private Color[] _colorsLinear = new Color[2] { Color.FromArgb(50, Color.Black), Color.White };
        private Color _colorValue = Color.Red;
        private Font _fontScaleValue = new Font("Arial", 10, FontStyle.Bold);
        private FingerStyles _fingerStyle = FingerStyles.style1;
        public ControlDialPlate()
        {
            InitializeComponent();
            _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
            pictureBox1.Image = _bmpFace;
        }
        /// <summary>
        /// 获取或设置仪表的最大值
        /// </summary>
        public int MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
                pictureBox1.Image = _bmpFace;
                SetValue(0);
            }
        }

        /// <summary>
        /// 获取或设置仪表名
        /// </summary>
        public string Title
        {
            get
            {
                return _tiltle;
            }
            set
            {
                _tiltle = value;
                SetValue(0);
            }
        }
        /// <summary>
        /// 获取或设置所仪表值的单位
        /// </summary>
        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
                SetValue(0);
            }
        }
        /// <summary>
        ///获取或设置报警值
        /// </summary>
        public int AlarmValue
        {
            get
            {
                return _alarmValue;
            }
            set
            {
                _alarmValue = value;
            }
        }

        /// <summary>
        /// 获取或设置刻度字体的颜色
        /// </summary>
        public Color ScaleValueColor
        {
            get
            {
                return _colorScaleValue;
            }
            set
            {
                _colorScaleValue = value;
                if (_colorScaleValue == null)
                {
                    throw new Exception("空值异常---属性值无效");
                }
                else
                {
                    _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
                    pictureBox1.Image = _bmpFace;
                    SetValue(0);
                }
            }
        }

        /// <summary>
        /// 获取或设置显示值的颜色
        /// </summary>
        public Color ValueColor
        {
            get
            {
                return _colorValue;
            }
            set
            {
                _colorValue = value;
                if (_colorValue == null)
                {
                    throw new Exception("空值异常---属性值无效");
                }
                else
                {
                    SetValue(0);
                }
            }
        }

        /// <summary>
        /// 获取或设置仪表盘渐变的起始色和结束色(一个由两个 Color 结构组成的数组，表示渐变的起始色和结束色)
        /// </summary>
        public Color[] LinearColors
        {
            get
            {
                return _colorsLinear;
            }
            set
            {
                _colorsLinear = value;
                if (_colorsLinear == null || _colorsLinear.Length != 2)
                {
                    throw new Exception("属性值无效");
                    //_colorsLinear = new Color[2] { Color.FromArgb(50, Color.Black), Color.White };
                }
                else
                {
                    _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
                    pictureBox1.Image = _bmpFace;
                    SetValue(0);
                }
            }
        }

        public Font ScaleValueFont
        {
            get
            {
                return _fontScaleValue;
            }
            set
            {
                _fontScaleValue = value;
                if (_fontScaleValue == null)
                {
                    throw new Exception("空值异常---属性值无效");
                }
                else
                {
                    _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
                    pictureBox1.Image = _bmpFace;
                    SetValue(0);
                }
            }
        }
        public FingerStyles FingerStyle
        {
            get
            {
                return _fingerStyle;
            }
            set
            {
                _fingerStyle = value;
                SetValue(0);
            }
        }
        private Bitmap DrawFace(int maxValue, Color colorScaleValue, Color[] colorsLinear)
        {
            Point pntStart = new Point(2, 2);
            int width = this.pictureBox1.Width - pntStart.X * 2;
            int height = this.pictureBox1.Height - pntStart.Y * 2;
            Bitmap bmpFace = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            Graphics gra = Graphics.FromImage(bmpFace);
            //gra.Clear(Control.DefaultBackColor);
            gra.Clear(Color.DimGray);
            Rectangle recFace = new Rectangle(pntStart.X, pntStart.Y, width, height);
            LinearGradientBrush linear = new LinearGradientBrush(recFace, colorsLinear[0], colorsLinear[1], 270f);
            PointF center = new PointF(bmpFace.Width / 2, bmpFace.Height / 2);
            gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen drawPen = new Pen(new SolidBrush(Color.Black), 1);
            int offset = 8;
            Rectangle recInnerFace = new Rectangle(pntStart.X + offset, pntStart.Y + offset,
                width - offset * 2, height - offset * 2);

            gra.FillEllipse(linear, recFace);
            GraphicsPath gpath = new GraphicsPath();
            gpath.AddEllipse(recFace);
            gpath.AddEllipse(recInnerFace);
            gra.FillPath(new SolidBrush(Color.Snow), gpath);
            drawPen.Width = 2.5f;
            gra.DrawEllipse(drawPen, recFace);
            drawPen.Width = 1;
            gra.DrawEllipse(drawPen, recInnerFace);

            double radian;
            float radius1 = center.X - 10;
            float radius2 = center.X - 17;
            float radius3 = center.X - 15;
            int scaleValue = 0;
            RectangleF strRegion;
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            drawPen.Width = 3f;
            //drawPen.Color = Color.FromArgb(255, 159, 113);
            drawPen.Color = Color.Orange;



            //float angle = 0;
            for (int i = -120; i <= 120; i += 30)
            {
                radian = DegToRad(i);
                gra.DrawLine(drawPen, (float)(center.X + radius1 * Math.Sin(radian)), (float)(center.Y -
                radius1 * Math.Cos(radian)), (float)(center.X + radius2 * Math.Sin(radian)), (float)(center.Y -
                radius2 * Math.Cos(radian)));
                //strRegion = new RectangleF(new PointF((float)(
                //   center.X + (radius3 - 20) * Math.Sin(radian) - 12), (float)(center.Y - (radius3 - 14) * Math.Cos(radian)) - 7), new SizeF(width / 4, heigth / 4));
                strRegion = new RectangleF(center.X - width / 6, center.Y - radius1 + 8, width / 3, width / 3);

                gra.TranslateTransform(center.X, center.Y);
                gra.RotateTransform(i);
                gra.TranslateTransform(-center.X, -center.Y);
                gra.DrawString(scaleValue.ToString(), _fontScaleValue, new SolidBrush(colorScaleValue), strRegion, strFormat);
                scaleValue += (int)(maxValue / 8);
                gra.ResetTransform();
            }
            drawPen.Width = 2f;
            //drawPen.Color = Color.FromArgb(103, 95, 96);
            drawPen.Color = Color.SlateGray;

            for (int i = -105; i < 120; i += 30)
            {
                radian = DegToRad(i);
                gra.DrawLine(drawPen, (float)(center.X + radius1 * Math.Sin(radian)), (float)(center.Y -
                radius1 * Math.Cos(radian)), (float)(center.X + radius3 * Math.Sin(radian)), (float)(center.Y -
                radius3 * Math.Cos(radian)));
            }

            RectangleF arc = new RectangleF(center.X - width * 3 / 11, center.Y - height * 3 / 11, width * 6 / 11, height * 6 / 11);
            gra.DrawArc(new Pen(Color.LimeGreen, width / 20), arc, 150, 60);
            gra.DrawArc(new Pen(Color.Gold, width / 20), arc, 210, 120);
            gra.DrawArc(new Pen(Color.Red, width / 20), arc, 330, 60);
            gra.Dispose();

            return bmpFace;
        }

        private Bitmap DrawFinger(float value)
        {
            Point pntStart = new Point(2, 2);
            int offsetY = 2;
            Graphics gra;
            int fingerValue = (int)value % _maxValue;
            _alarmState = value < _alarmValue ? false : true;
            float angle = 240 * fingerValue / _maxValue - 120;
            float width = this.pictureBox1.Width * 3 / 4;
            float height = this.pictureBox1.Height * 3 / 4;
            Bitmap bmp = new Bitmap((int)width, (int)height);
            PointF center = new PointF(width / 2, height / 2);
            gra = Graphics.FromImage(bmp);
            gra.SmoothingMode = SmoothingMode.HighQuality;
            gra.TranslateTransform(center.X, center.Y);
            gra.RotateTransform(angle);
            gra.TranslateTransform(-center.X, -center.Y);

            PointF[] finger = new PointF[] { 
                new PointF(center.X, 15 - offsetY), 
                new PointF(center.X - width / 24f, center.Y), 
                new PointF(center.X + height / 24f, center.Y),};
            gra.FillPolygon(new SolidBrush(Color.Black), finger);
            //gra.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new PointF(width * 9 / 20f, height * 9 / 20f), new SizeF(width / 10f, height / 10f)));
            gra.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new PointF(center.X - width / 16f, center.Y - height / 16f), new SizeF(width / 8f, height / 8f)));

            gra.ResetTransform();
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            //gra.DrawString(string.Format("{0}", (int)Math.Round(value)), new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(_colorValue),
            //    new RectangleF(center.X - 40, center.Y + 10, 80, 40), strFormat);
            gra.DrawString(string.Format("{0}", (int)Math.Round(value)), new Font("Times New Roman", width / 8, FontStyle.Bold), new SolidBrush(_colorValue),
                new RectangleF(center.X - width / 3f, center.Y + height / 8, width * 2 / 3f, height / 3), strFormat);
            if (_alarmState)
            {
                if (_colorAlarm == Color.Red)
                {
                    _colorAlarm = Color.Black;
                }
                else if (_colorAlarm == Color.Black)
                {
                    _colorAlarm = Color.Red;
                }
            }
            else
            {
                _colorAlarm = Color.Black;
            }
            //gra.DrawString(string.Format(string.Format("{0}", _tiltle)), new Font("微软雅黑", 11, FontStyle.Regular), new SolidBrush(_colorAlarm),
            //    new RectangleF(center.X - 50, center.Y + 35, 100, 40), strFormat);
            gra.DrawString(string.Format(string.Format("{0}", _tiltle)), new Font("微软雅黑", width / 10 - 2, FontStyle.Regular), new SolidBrush(_colorAlarm),
                new RectangleF(center.X - width / 3, center.Y + height / 3, width * 2 / 3, height / 4), strFormat);
            gra.DrawString(string.Format(string.Format("{0}", _unit)), new Font("Arial", width / 10 - 1, FontStyle.Regular), new SolidBrush(_colorAlarm),
                new RectangleF(center.X - width / 3, height / 4, width * 2 / 3, height / 4), strFormat);
            gra.Dispose();
            return bmp;
        }


        private Bitmap DrawFinger(float value, FingerStyles fingerStyle)
        {
            Point pntStart = new Point(2, 2);
            int offsetY = 2;
            Graphics gra;
            int fingerValue = (int)value % _maxValue;
            _alarmState = value < _alarmValue ? false : true;
            float angle = 240 * fingerValue / _maxValue - 120;
            float width = this.pictureBox1.Width * 3 / 4;
            float height = this.pictureBox1.Height * 3 / 4;
            Bitmap bmp = new Bitmap((int)width, (int)height);
            PointF center = new PointF(width / 2, height / 2);
            gra = Graphics.FromImage(bmp);
            gra.SmoothingMode = SmoothingMode.HighQuality;
            gra.TranslateTransform(center.X, center.Y);
            gra.RotateTransform(angle);
            gra.TranslateTransform(-center.X, -center.Y);

            if (fingerStyle == FingerStyles.style1)
            {
                PointF[] finger = new PointF[] { 
                new PointF(center.X, 15 - offsetY), 
                new PointF(center.X - width / 24f, center.Y), 
                new PointF(center.X + height / 24f, center.Y),};
                gra.FillPolygon(new SolidBrush(Color.Black), finger);
                //gra.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new PointF(width * 9 / 20f, height * 9 / 20f), new SizeF(width / 10f, height / 10f)));
                gra.FillEllipse(new SolidBrush(Color.DarkGray), new RectangleF(new PointF(center.X - width / 16f, center.Y - height / 16f), new SizeF(width / 8f, height / 8f)));

            }
            else if (fingerStyle == FingerStyles.style2)
            {
                PointF[] finger = new PointF[] { 
                new PointF(center.X, 10 - offsetY), 
                new PointF(center.X - width / 24f + 2, 25 - offsetY),
                new PointF(center.X - width / 24f, center.Y + width / 8f), 
                new PointF(center.X + height / 24f, center.Y + height / 8f),
                new PointF(center.X + width / 24f - 2, 25 - offsetY),
                };
                gra.FillPolygon(new SolidBrush(Color.FromArgb(200, Color.Black)), finger);
                gra.FillEllipse(new SolidBrush(Color.DarkGray), new RectangleF(new PointF(center.X - width / 16f, center.Y - height / 16f), new SizeF(width / 8f, height / 8f)));

            }
            else if (fingerStyle == FingerStyles.style3)
            {
                PointF[] finger = new PointF[] { 
                new PointF(center.X, 10 - offsetY), 
                new PointF(center.X - width / 24f + 2, 25 - offsetY),
                new PointF(center.X - width / 24f, center.Y), 
                new PointF(center.X + height / 24f, center.Y),
                new PointF(center.X + width / 24f - 2, 25 - offsetY),
                };
                gra.FillPolygon(new SolidBrush(Color.FromArgb(200, Color.Black)), finger);
                gra.FillEllipse(new SolidBrush(Color.DarkGray), new RectangleF(new PointF(center.X - width / 16f, center.Y - height / 16f), new SizeF(width / 8f, height / 8f)));

            }
            gra.ResetTransform();
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            //gra.DrawString(string.Format("{0}", (int)Math.Round(value)), new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(_colorValue),
            //    new RectangleF(center.X - 40, center.Y + 10, 80, 40), strFormat);
            gra.DrawString(string.Format("{0}", (int)Math.Round(value)), new Font("Times New Roman", width / 8, FontStyle.Bold), new SolidBrush(_colorValue),
                new RectangleF(center.X - width / 3f, center.Y + height / 8, width * 2 / 3f, height / 3), strFormat);
            if (_alarmState)
            {
                if (_colorAlarm == Color.Red)
                {
                    _colorAlarm = Color.Black;
                }
                else if (_colorAlarm == Color.Black)
                {
                    _colorAlarm = Color.Red;
                }
            }
            else
            {
                _colorAlarm = Color.Black;
            }
            //gra.DrawString(string.Format(string.Format("{0}", _tiltle)), new Font("微软雅黑", 11, FontStyle.Regular), new SolidBrush(_colorAlarm),
            //    new RectangleF(center.X - 50, center.Y + 35, 100, 40), strFormat);
            gra.DrawString(string.Format(string.Format("{0}", _tiltle)), new Font("微软雅黑", width / 10 - 2, FontStyle.Regular), new SolidBrush(_colorAlarm),
                new RectangleF(center.X - width / 3, center.Y + height / 3, width * 2 / 3, height / 4), strFormat);
            gra.DrawString(string.Format(string.Format("{0}", _unit)), new Font("Arial", width / 10 - 1, FontStyle.Regular), new SolidBrush(_colorAlarm),
                new RectangleF(center.X - width / 3, height / 4, width * 2 / 3, height / 4), strFormat);
            gra.Dispose();
            return bmp;
        }

        private double DegToRad(double d)
        {
            return d * Math.PI / 180;
        }

        private void ControlDialPlate_Resize(object sender, EventArgs e)
        {
            try
            {
                this.DoubleBuffered = true;
                this.Height = this.Width;
                this.pictureBox1.Width = this.pictureBox1.Height;
                _bmpFace = DrawFace(_maxValue, _colorScaleValue, _colorsLinear);
                this.pictureBox1.Image = _bmpFace;
                SetValue(0);
            }
            catch
            {
            }
        }

        public void SetValue(float speedVaule)
        {
            Bitmap tbmp = (Bitmap)_bmpFace.Clone();
            Graphics gra = Graphics.FromImage(tbmp);
            RectangleF rct = new RectangleF(this.pictureBox1.Width / 8, this.pictureBox1.Height / 8,
                this.pictureBox1.Width * 3 / 4, this.pictureBox1.Height * 3 / 4);
            gra.DrawImage(DrawFinger(speedVaule, _fingerStyle), rct);
            pictureBox1.Image = tbmp;
        }
    }
}
