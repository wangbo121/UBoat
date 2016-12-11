using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

using System.Security.Cryptography.X509Certificates;

using System.Net;
using System.Net.Sockets;
using System.Xml; // config file
using System.Runtime.InteropServices; // dll imports
//using log4net;
//using ZedGraph; // Graphs
using System.Reflection;

using System.IO;

using System.Drawing.Drawing2D;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace BoatGCS
{

    /// <summary>
    /// used to override the drawing of the waypoint box bounding
    /// </summary>
    [Serializable]
    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 2);

        public Color Color { get { return Pen.Color; } set { if (!initcolor.HasValue) initcolor = value; Pen.Color = value; } }

        Color? initcolor = null;

        public GMapMarker InnerMarker;

        public int wprad = 0;//=0:不画任何东西;>0:画虚线圆圈

        public void ResetColor()
        {
            if (initcolor.HasValue) Color = initcolor.Value;
            else Color = Color.White;
        }

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2 - 20);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

            // undo autochange in mouse over
            //if (Pen.Color == Color.Blue)
            //  Pen.Color = Color.White;
            //获取Overlay实际距离与像素点之间的转换系数，包括宽度方向和高度方向的
            double width = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
            double height = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0)) * 1000.0);
            double m2pixelwidth = Overlay.Control.Width / width;//像素点/实际距离[m]
            double m2pixelheight = Overlay.Control.Height / height;//像素点/实际距离[m]

            GPoint loc = new GPoint((int)(LocalPosition.X - (m2pixelwidth * wprad * 2)), LocalPosition.Y);// MainMap.FromLatLngToLocal(wpradposition);
            //当图放大到一定比例后，在坐标点周围画出一个虚线圆圈，圆圈的半径为wprad
            if (m2pixelheight > 0.5)
                g.DrawArc(Pen, new System.Drawing.Rectangle(LocalPosition.X - Offset.X - (int)(Math.Abs(loc.X - LocalPosition.X) / 2), LocalPosition.Y - Offset.Y - (int)Math.Abs(loc.X - LocalPosition.X) / 2, (int)Math.Abs(loc.X - LocalPosition.X), (int)Math.Abs(loc.X - LocalPosition.X)), 0, 360);

        }
    }

    [Serializable]
    public class GMapMarkerWP : GMarkerGoogle //航点图标，图标上可带航点编号
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        string wpno = "";//航点编号
        public bool selected = false;

        public GMapMarkerWP(PointLatLng p, string wpno)
            : base(p, GMarkerGoogleType.green)
        {
            this.wpno = wpno;
        }

        public override void OnRender(Graphics g)
        {        
            if (selected)//如果选中了该航点，则在航点周围画一个以图标尺寸为直径的虚线圆圈
            {
                Pen Pen = new Pen(Brushes.Red, 2);
                Pen.DashStyle = DashStyle.Dash;
                //g.FillEllipse(Brushes.Red, new Rectangle(this.LocalPosition, this.Size));
                //g.DrawArc(Pens.Red,new Rectangle(this.LocalPosition,this.Size),0,360);
                g.DrawArc(Pen,new Rectangle(this.LocalPosition,this.Size),0,360);
            }
            base.OnRender(g);
            //标记航点编号
            var midw = LocalPosition.X +10;//图标中心点X
            var midh = LocalPosition.Y +3;//图标中心点Y
            var txtsize = TextRenderer.MeasureText(wpno, SystemFonts.DefaultFont);//指定文本的尺寸（像素点）
            if (txtsize.Width > 15) midw -= 4;
            g.DrawString(wpno, SystemFonts.DefaultFont, Brushes.Black , new PointF(midw, midh));//航点号,字体,笔色,坐标
        }
    }

    [Serializable]
    public class GMapMarkerPlane : GMapMarker //飞机飞行图标，包括飞行中的一些指示线
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        private readonly Bitmap icon = global::BoatGCS.Properties.Resources.planetracker;

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;

        public GMapMarkerPlane(PointLatLng p, float heading, float cog, float nav_bearing, float target)//坐标,机头朝向,
            : base(p)
        {
            this.heading = heading;//机头朝向,0:Up;180:Down
            this.cog = cog;//course over ground，地速方向
            this.target = target;//目标航点朝向
            this.nav_bearing = nav_bearing;//当前导航航向
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                //画出机头朝向直线
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch { }
            //画出导航方向直线
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            //画出地速方向直线
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //画出目标点朝向直线
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
 
            // anti NaN
            try
            {
                float desired_lead_dist = 100;
                //Overlay层宽度方向对应的经度距离[m]
                double width = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                //Overlay层宽度方向像素点数/对应的经度距离[m] = pixel / meter: 变换系数
                double m2pixelwidth = Overlay.Control.Width / width;

                float radius = 100;//转弯半径？MainV2.comPort.MAV.cs.radius
                float alpha = ((desired_lead_dist * (float)m2pixelwidth) / radius) * rad2deg;

                if (radius < -1)
                {
                    // fixme 
                    float p1 = (float)Math.Cos((cog) * deg2rad) * radius + radius;
                    float p2 = (float)Math.Sin((cog) * deg2rad) * radius + radius;
                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(radius) * 2, Math.Abs(radius) * 2, cog, alpha);//画出转弯弧
                }
                else if (radius > 1)
                {
                    // correct
                    float p1 = (float)Math.Cos((cog - 180) * deg2rad) * radius + radius;
                    float p2 = (float)Math.Sin((cog - 180) * deg2rad) * radius + radius;
                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, radius * 2, radius * 2, cog - 180, alpha);//画出转弯弧
                }
            }
            catch { }

            try
            {
                g.RotateTransform(heading);//按机头朝向旋转图标
            }
            catch { }
            g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2);//按图标原尺寸画出图标

            g.Transform = temp;
        }
    }

    public class GMapMarkerBoat : GMapMarker //船航行图标，包括航行中的一些指示线。与飞机的一样
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        private readonly Bitmap icon = global::BoatGCS.Properties.Resources.boattracker;

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;

        public GMapMarkerBoat(PointLatLng p, float heading, float cog, float nav_bearing, float target)//坐标,船头朝向,
            : base(p)
        {
            if ((heading < 0.0) || (heading > 360.0)) heading = (float)0.0;
            this.heading = heading;//机头朝向,0:Up;180:Down
            if ((cog < 0.0) || (cog > 360.0)) cog = (float)0.0;
            this.cog = cog;//地速方向
            if ((target < 0.0) || (target > 360.0)) target = (float)0.0;
            this.target = target;//目标点朝向
            if ((nav_bearing < 0.0) || (nav_bearing > 360.0)) nav_bearing = (float)0.0;
            this.nav_bearing = nav_bearing;//导航航向
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                //画出船头朝向直线
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch { }
            //画出导航方向直线
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            //画出地速方向直线
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //画出目标点朝向直线
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
/*
            // anti NaN
            try
            {
                float desired_lead_dist = 100;
                //Overlay层宽度方向对应的经度距离[m]
                double width = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                //Overlay层宽度方向像素点数/对应的经度距离[m] = pixel / meter: 变换系数
                double m2pixelwidth = Overlay.Control.Width / width;

                float radius = 100;//转弯半径？MainV2.comPort.MAV.cs.radius
                float alpha = ((desired_lead_dist * (float)m2pixelwidth) / radius) * rad2deg;

                if (radius < -1)
                {
                    // fixme 
                    float p1 = (float)Math.Cos((cog) * deg2rad) * radius + radius;
                    float p2 = (float)Math.Sin((cog) * deg2rad) * radius + radius;
                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(radius) * 2, Math.Abs(radius) * 2, cog, alpha);//画出转弯弧
                }
                else if (radius > 1)
                {
                    // correct
                    float p1 = (float)Math.Cos((cog - 180) * deg2rad) * radius + radius;
                    float p2 = (float)Math.Sin((cog - 180) * deg2rad) * radius + radius;
                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, radius * 2, radius * 2, cog - 180, alpha);//画出转弯弧
                }
            }
            catch { }
*/
            try
            {
                g.RotateTransform(heading);//按船头朝向旋转图标
            }
            catch { }
            g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2);//按图标原尺寸画出图标

            g.Transform = temp;
        }
    }

}
