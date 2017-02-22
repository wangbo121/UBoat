using System;
using System.Collections;//队列等
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices; 

using GeoUtility;
using GeoUtility.GeoSystem;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.IO.Ports;//串口
using System.Threading;//串口线程用
using BoatGCS;

namespace BoatGCS
{
    public partial class FrmGCSMain : Form
    {
        //定义一些常量
        // RS232 task
        public const byte RECV_HEAD1=0;
        public const byte RECV_HEAD2 = 1;
        public const byte RECV_LEN = 2;
        public const byte RECV_CNT=3;
        public const byte RECV_SYSID=4;
        public const byte RECV_TYPE = 5;
        public const byte RECV_DATA=6;
        public const byte RECV_CHECKSUM=7;

        /*参数类型宏定义*/
        public const byte PARAMETER_CRUISE_THROTTLE = 0;
        public const byte PARAMETER_GET_CRUISE_THROTTLE = 1;

        public const byte PARAMETER_GET_WP = 2;

        public const byte PARAMETER_SET_ARRIVE_RADIUS = 3;
        public const byte PARAMETER_GET_ARRIVE_RADIUS = 4;
        /*偏航距所能矫正的最大角度*/
        public const byte PARAMETER_SET_CTE_MAX_CORRECT = 5;
        public const byte PARAMETER_GET_CTE_MAX_CORRECT = 6;

        public const byte PARAMETER_SET_REVERSE = 7;
        public const byte PARAMETER_GET_REVERSE = 8;
        public const byte RUDDER_REVERSE = 1;
        public const byte THROTTLE_REVERSE = 2;

        public const byte PARAMETER_SET_ROTARY = 9;
        public const byte ROTARY_AS_CENTER = 1;//设置目前码盘的位置为中心位置，目前中心位置是180度，也就是实际上码盘会从145转到215，因为舵机最多转左右各35度
        public const byte ROTARY_CLOCKWISE = 2;//设置顺时针转动时，码盘数值增加
        public const byte ROTARY_COUNTER_CLOCKWISE = 3;//设置逆时针转动时，码盘数值增加

        public const byte PARAMETER_GET_ROTARY_POSITION = 10;

        public const byte PARAMETER_SET_POWER_DISCHARGE = 11;//给充电机和切换器发送命令
        public const byte POWER_CHARGE1 = 1;
        public const byte POWER_CHARGE2 = 2;
        public const byte POWER_CHARGE3 = 3;
        public const byte POWER_STOP_CHARGE = 4;
        public const byte DISCHARGE1 = 5;
        public const byte DISCHARGE2 = 6;
        public const byte DISCHARGE_STOP = 7;

        public const byte PARAMETER_SET_GENERATOR = 12;//打开关闭发电机
        public const byte GENERATOR_ON = 1;
        public const byte GENERATOR_OFF = 0;

        public const byte PARAMETER_SET_MOTOR_DIRECTION = 13;//电机正反转
        public const byte MOTOR_FORWARD = 0;
        public const byte MOTOR_BACKWARD = 1;
        public const byte MOTOR_OFF = 2;
        public const byte MOTOR_ON = 3;

        public const byte PARAMETER_SET_MOTOR_LEFT_INCREASE = 14;//左电机增加百分之x
        public const byte PARAMETER_SET_MOTOR_RIGHT_INCREASE = 15;//右电机增加百分之x

        public const byte PARAMETER_SET_ROCKET = 16;
        public const byte PARAMETER_SET_ROCKET_CLOSE = 0;
        public const byte PARAMETER_SET_ROCKET_LAUNCH = 1;

        public const byte PARAMETER_SET_CTE_I = 17;
        public const byte PARAMETER_SET_CTE_D = 18;

        public const byte PARAMETER_SET_THROTTLE_PERCENT_TIME = 19;

        public const byte PARAMETER_SET_USE_DIFFERENTIAL_CONTROL = 20;
        public const byte RUDDER_CONTROL = 0;
        public const byte DIFFERENTIAL_CONTROL = 1;

        public const byte PARAMETER_SET_USE_TRAJECTORY_NAVIGATION = 21;
        public const byte BOAT_HEAD_NAVIGATION = 0;
        public const byte TRAJECTORY_NAVIGATION = 1;

        public const byte PARAMETER_SET_SWITCH_CHANNEL = 22;
        public const byte PARAMETER_SET_SWITCH_LOW_LIMIT = 23;
        public const byte PARAMETER_SET_SWITCH_HIGH_LIMIT = 24;
        public const byte PARAMETER_SET_SWITCH_START_ON = 25;

        public const byte PARAMETER_SET_CHARGE_CHANNEL = 26;
        public const byte PARAMETER_SET_CHARGE_VOLTAGE = 27;
        public const byte PARAMETER_SET_CHARGE_CURRENT = 28;
        public const byte PARAMETER_SET_CHARGE_START_ON = 29;
        public const byte PARAMETER_SET_CHARGE_TURN_ON = 1;
        public const byte PARAMETER_SET_CHARGE_TURN_OFF = 0;

        



        


        //wangbo 测试发送指定航点
        public const byte specific_wp = (byte)0x80;

        //just for test receive ascii data
        public const byte RECV_NUM = 0;
        public const byte RECV_DOT = 1;
        public const byte RECV_ABC = 2;

        //通信数据包ID
        public const byte ID_GCS2AP_CMD = 0x01;
        public const byte ID_GCS2AP_WP = 0x02;
        public const byte ID_GCS2AP_LNK = 0x03;
        public const byte ID_GCS2AP_CTE = 0x04;
        public const byte ID_GCS2AP_PARAMETER = 0x05;


        public const byte ID_AP2GCS_REAL = 0x11;
        public const byte ID_AP2GCS_CMD = 0x12;
        public const byte ID_AP2GCS_WP = 0x13;
        public const byte ID_AP2GCS_ACK = 0x14;
        public const byte ID_AP2GCS_PARAMETER = 0x15;

        public const byte ID_AP2GCS_AWS = 0x18;

        //设备ID
        public const byte SYSID_AP = 0x1;
        public const byte SYSID_GCS = 0xf;

        public const double TIMER_DISP=2000;//[ms] 

        public const double BEIJING_LAT = 32.6843585;
        public const double BEIJING_LNG = 117.0552518;
        //public const double HUAINAN_LAT = 32.56765;
        //public const double HUAINAN_LNG = 117.00853;
        public const double HUAINAN_LAT = 32.6843585;
        public const double HUAINAN_LNG = 117.0552518;

        //public double onestep = 0.1;

        /// <summary>
        /// 默认航行速度
        /// </summary>
        public const int default_WP_spd = 9;//[knot]  

        //游戏杆相关
        private JoystickInterface.Joystick joystick;

        //串行通信接口相关
        //开两个串口，一个(comA)用于电台通信，一个(comB)用于铱星通信
        public static ComPortPar comAPara;
        public static ComPortPar comBPara;
        SerialPort comPortA = new SerialPort();//电台通信
        SerialPort comPortB = new SerialPort();//铱星通信
        /// <summary>
        /// used as a serial port write lock
        /// </summary>
        volatile object comASendlock = new object();
        // used for a readlock on readpacket
        volatile object comARecvlock = new object();

        //以下几个变量仅用于调试，调试完成后可取消
        int recvErrorCnt;//接收错误计数
        int errorCnt;//错误计数
        int error1, error2, error3, error4, error5, error6, error7, error8, error9, error10;

        int comAsendPacketCnt;//串口A发送计数 发送了多少个数据包
        int comArecvCount;//串口A接收计数
        int comAsendCount;//串口A发送计数 发送了多少个字节

        private string[] ports;//可用串口数组
        Thread comARecvThread;//串口读线程
        Queue comArecQueue = new Queue();//接收数据线程与数据处理线程直接传递的队列
        //invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，
        //并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html
        private bool comAListening = false;//是否没有执行完invoke相关操作
        private bool comAClosing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke

        //Thread comBRecvThread;
        int comBsendPacketCnt;//串口A发送计数 发送了多少个数据包
        int comBrecvCount;
        int comBsendCount;
        Thread comBRecvThread;//串口读线程
        Queue comBrecQueue = new Queue();//接收数据线程与数据处理线程直接传递的队列
        //private bool comBWaitClose = false;//invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，
        //并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html
        private bool comBListening = false;//是否没有执行完invoke相关操作
        private bool comBClosing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke
        volatile object comBSendlock = new object();
        // used for a readlock on readpacket
        volatile object comBRecvlock = new object();


        //声音相关
        System.Media.SoundPlayer _alarmSound = new System.Media.SoundPlayer(Properties.Resources.alarm);
        System.Media.SoundPlayer _didiSound = new System.Media.SoundPlayer(Properties.Resources.didi);

        //地图相关
        /// <summary>
        /// 航路规划层，用于标记设定航点和预定航线
        /// </summary>
        GMapOverlay markersOverlay;
        /// <summary>
        /// 实际飞行路径层
        /// </summary>
        GMapOverlay realRouteOverlay;
        /// <summary>
        /// 飞行器飞行动画层
        /// </summary>
        GMapOverlay realVehicleOverlay;
        /// <summary>
        /// 规划航点队列，包含了全部规划航点信息。航点0为家
        /// </summary>
        public static List<PointLatLngAlt> totalWPlist = new List<PointLatLngAlt>();

        /// <summary>
        /// 家坐标
        /// </summary>
        PointLatLng homePos = new PointLatLng();
        /// <summary>
        /// 生效航点标记
        /// </summary>
        GMapMarker actWP_marker;
        /// <summary>
        /// 航点索引
        /// </summary>
        public static int actWP_index;
        /// <summary>
        /// 当前在地图上鼠标所指的位置
        /// </summary>
        PointLatLng currentPosition = new PointLatLng();

        //List<int> groupmarkers = new List<int>();

        /// <summary>
        /// 用于计算地图上两点距离的临时坐标变量
        /// </summary>
        PointLatLngAlt tmp_point_4_calc = new PointLatLngAlt();

        /// <summary>
        /// 全局状态结构，其中包含了全部用于在整个程序中传递的状态变量或标志
        /// </summary>
        public static GBL_VAR gbl_var;

        //public static byte wpNum; 
       // WAYPOINT wp

        /// <summary>
        /// 要发送给AP的航点
        /// </summary>
        public static GCS_AP_WP gcs2ap_wp;
        /// <summary>
        /// 从AP返回的航点
        /// </summary>
        public static GCS_AP_WP ap2gcs_wp_back;
        /// <summary>
        /// 发送给AP的命令
        /// </summary>
        public static GCS_AP_CMD gcs2ap_cmd;
        /// <summary>
        /// 正在更新的命令，等更新完毕后变为gcs2ap_cmd_new发给AP
        /// </summary>
        public static GCS_AP_CMD gcs2ap_cmd_new;
        /// <summary>
        /// AP返回的GCS命令
        /// </summary>
        public static GCS_AP_CMD ap2gcs_cmd_back;
        /// <summary>
        /// 发送给AP的链路确认包
        /// </summary>
        public static GCS2AP_LNK gcs2ap_lnk;

        public static GCS2AP_CTE gcs2ap_cte;

        public static GCS_AP_PARAMETER gcs2ap_parameter;

    
        /// <summary>
        /// 接收到的AP实时包
        /// </summary>
        public static AP2GCS_REAL ap2gcs_real;
        /// <summary>
        /// 接收到的AP确认包
        /// </summary>
        public static AP2GCS_ACK ap2gcs_ack;
        /// <summary>
        /// 接收到的AP气象站包
        /// </summary>
        public static AP2GCS_AWS ap2gcs_aws;

        /// <summary>
        /// 接收到的AP参数包
        /// </summary>
        public static GCS_AP_PARAMETER ap2gcs_parameter;

        /// <summary>
        /// comA 接收到的数据包
        /// </summary>
        public static RECV_PACKET comA_recvpacket;

        //wangbo 20170118
        /// <summary>
        /// comA 接收到的数据包
        /// </summary>
        public static RECV_PACKET comB_recvpacket;


        /// <summary>
        /// 定时器定时计数
        /// </summary>
        public static int timerCnt;

        //wangbo 20170118
        /// <summary>
        /// 北斗 地面站-->自驾仪
        /// </summary>
        public static BD_GCS2AP bd_gcs2ap;
        

        /// <summary>
        /// 北斗 自驾仪-->地面站
        /// </summary>
       

        public FrmGCSMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 实时数据记录文件名,按时间写文件名，保存格式为逗号分割ASCII码
        /// </summary>
        FileStream file_real = new FileStream(DateTime.Now.ToString("yyyy-MM-dd-hhmmss")+".txt", FileMode.Create);//创建一个1.txt
        //文件指针
        StreamWriter file_real_w;
        /// <summary>
        /// 写字串
        /// </summary>
        string file_real_str = "";     
   

        //wangbo 
        PointLatLng waypoint_test = new PointLatLng();


        private void FrmGCSMain_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MaximumSize;// .Size;
            this.MinimumSize = this.MinimumSize;//.Size;
            this.MaximizeBox = true;

            gbl_var.main_timer_interval = main_timer.Interval;

            //创建实时数据的保存文件
            file_real_w = new StreamWriter(file_real);

            //wangbo 20170118 初始化北斗发送数据数组
            bd_gcs2ap.data = new char[72];


            //wangbo
            //本来计划保存为航点文本文件，但是后来考虑做成地面站一个个获取航点数据，再重新画
#if false
            if (System.IO.File.Exists(Path.GetFullPath(".\\waypoint.txt")))
            {
                 MessageBox.Show("航点文件存在");
                //打开航点文件，载入航点
            }
            else
            {
                MessageBox.Show("航点文件不存在");
                //创建航点文件
            } 
#endif

            // 查找并初始化游戏杆
            joystick = new JoystickInterface.Joystick(this.Handle);
            string[] sticks = joystick.FindJoysticks();
            if (sticks == null)
            {
                MessageBox.Show("未连接摇杆!");
                //Process.GetCurrentProcess().Kill();
                gbl_var.bJoystickInstalled = false;
            }
            else
            {
                gbl_var.bJoystickInstalled = true;
            }

            if(gbl_var.bJoystickInstalled) joystick.AcquireJoystick(sticks[0]);
        

            //地图初始化 费老师一开始使用的，无法进行缓存的地图
            //gMapControl.SetPositionByKeywords("Beijing,China");//先随意指定一个地图中心，等GPS定位后自动更新到实际坐标点
            ////gMap.MapProvider = GMap.NET.MapProviders.AMapSateliteProvider.Instance;//初始默认采用高德卫星地图
            //gMapControl.MapProvider = GMap.NET.MapProviders.AMapProvider.Instance;//初始默认采用高德地图
            //statusMapProvider.Text = "高德地图";
            ////gMapControl.CacheLocation = @"D:\GMap.NET";//@"G:\GMAP";//好像不管用???
            //gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;

            //可以用googlemap的
            /*
            gMapControl.CacheLocation = @"D:\\gmap";
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleChinaMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            //gMapControl.Position = new PointLatLng(39.958436, 116.309175);//此为定初始位置的另一种方式  
            //gMapControl.SetPositionByKeywords("china,Beijing");//设置初始中心为china harbin  
            //GMaps.Instance.ImportFromGMDB("D:\\Data.gmdb");
            
            statusMapProvider.Text = "googleMap China地图";
            //gMapControl.ReloadMap();
            */

            gMapControl.SetPositionByKeywords("Beijing,China");
            gMapControl.Position = new PointLatLng(39.95940, 116.31583);//此为定初始位置的另一种方式  
            gMapControl.CacheLocation=(Environment.CurrentDirectory + "\\GMapCache_AMap");//这个语句，可以在bin/debug文件件下建立相应缓存文件
            gMapControl.MapProvider = GMap.NET.MapProviders.AMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            statusMapProvider.Text = "高德地图";         

            //创建一个marker层，用于标记航点
            markersOverlay = new GMapOverlay("markers");
            gMapControl.Overlays.Add(markersOverlay);
            //创建一个飞行动画层，用于动态显示飞行器的飞行状态
            realVehicleOverlay = new GMapOverlay("realvehicle");
            gMapControl.Overlays.Add(realVehicleOverlay);
            //创建实时航路层，用于显示实时航线
            realRouteOverlay = new GMapOverlay("realroute");
            gMapControl.Overlays.Add(realRouteOverlay);

            //初始显示位置和地图尺寸，后面当选择"设置为家"按钮时，将直接将AP上返回的GPS坐标作为家的位置
            homePos.Lat = HUAINAN_LAT;
            homePos.Lng = HUAINAN_LNG;
            gMapControl.Position = homePos;
            gMapControl.Zoom = 16;

            //航点表中的0号航点为家坐标
            totalWPlist.Add(new PointLatLngAlt(homePos.Lat, homePos.Lng, 0.0, "H"));
            addpolygonmarker("H", homePos.Lat, homePos.Lng, 0.0, null);
            //totalWPlist[0].color = Color.Pink;
            //addpolygonmarker("H", homePos.Lat, homePos.Lng, 0.0, totalWPlist[0].color);

            //启动主定时器，定时间隔100ms
            main_timer.Enabled = true;

            //串口初始化，但不打开
            ports = SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//有串口可用
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    cbxACOMPort.Items.Add(ports[i]);//下拉控件里添加可用串口
                    cbxBCOMPort.Items.Add(ports[i]);//下拉控件里添加可用串口
                }
                cbxACOMPort.SelectedIndex = 0;
                cbxBCOMPort.SelectedIndex = 0;
            }
            else//未检测到串口
            {
                statusPortAName.Text = "串口A: ";
                statusPortBName.Text = "串口B: ";
            }

            //列出常用的波特率
            cbxABaudRate.Items.Add("9600");
            cbxABaudRate.Items.Add("19200");
            cbxABaudRate.Items.Add("38400");
            cbxABaudRate.Items.Add("57600");
            cbxABaudRate.Items.Add("115200");
            cbxABaudRate.SelectedIndex = 0;

            cbxBBaudRate.Items.Add("9600");
            cbxBBaudRate.Items.Add("19200");
            cbxBBaudRate.Items.Add("38400");
            cbxBBaudRate.Items.Add("57600");
            cbxBBaudRate.Items.Add("115200");
            cbxBBaudRate.SelectedIndex = 0;

            btnPortAOpenClose.Text = "打开";
            comPortA.DataReceived += new SerialDataReceivedEventHandler(comAReceived);

            btnPortBOpenClose.Text = "打开";
            comPortB.DataReceived += new SerialDataReceivedEventHandler(comBReceived);
           
            //初始位置所在城市设置，用于在没有GPS采集数据时设置地图位置
            cbxBoxHomeCity.Items.Add("北京");
            cbxBoxHomeCity.Items.Add("淮南");

            //初始化参数

            //GCS2AP_CMD初始化
            //以后需要改为读参数文件进行初始化
            gcs2ap_cmd.cmd_state = 0x3;//开机自动进入测试状态
            gcs2ap_cmd.cmd_test = 0x4;//方向舵舵机输出有效
            gcs2ap_cmd.cmd_manu = 0x0;//主推进器停车
            gcs2ap_cmd.cmd_auto = 0x0;//完全自驾状态
            gcs2ap_cmd.cmd_rkt = 0x0;
            gcs2ap_cmd.cmd_mot = 0x0;
            gcs2ap_cmd.cmd_aws = 0x0;
            gcs2ap_cmd.cmd_flag = 0x1;//电台和铱星同时工作
            gcs2ap_cmd.moo_pwm = 127;//中位
            gcs2ap_cmd.mbf_pwm = 127;
            gcs2ap_cmd.rud_pwm = 127;
            gcs2ap_cmd.rud_p = 1;
            gcs2ap_cmd.rud_i = 0;
            gcs2ap_cmd.rud_d = 0;//20161205 wangbo
            gcs2ap_cmd.wpno = 0xff;//航点数据无效
            //系统启动时两个数据相同，后面通过对比来确定数据是否变化
            gcs2ap_cmd_new = gcs2ap_cmd;



        }
        /// <summary>
        /// 接收缓冲区尺寸
        /// </summary>
        public static int packbuflen;

        //wangbo
        public static int firstWp;

        byte[] com_recv_buf = new byte[500];//接收缓冲区

        public static int com_recv_cnt=0;

        private void comAReceived(object sender, SerialDataReceivedEventArgs e)//接收数据 中断只标志有数据需要读取，读取操作在中断外进行
        {
            int recvLen;
            int i = 0;
            uint recv_tmp1,recv_tmp2;
            byte c = 0;
            byte[] packbuf = new byte[500];//接收缓冲区
            for (i = 0; i < 500; i++) packbuf[i] = 0;

            if (comAClosing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            Thread.Sleep(50);
            if (false)
            {
                comPortA.DiscardInBuffer();//清接收缓存
            }
            else
            {
                byte[] recvBuffer;//接收缓冲区
                try
                {
                    comAListening = true;//设置标记，说明我已经开始处理数据，一会儿要使用系统UI的。
                    /*
                    recvBuffer = new byte[comPortA.BytesToRead];//接收数据缓存大小
                    recvLen = comPortA.Read(recvBuffer, 0, recvBuffer.Length);//读取数据
                    comArecvCount += recvLen;
                    */
                    
                    recvBuffer = new byte[comPortA.BytesToRead];//接收数据缓存大小
                    recvLen = comPortA.Read(recvBuffer, 0, recvBuffer.Length);//读取数据
                    comArecvCount += recvLen;
                    Array.Copy(recvBuffer, 0, com_recv_buf, com_recv_cnt, recvLen);
                    /*
                    for (i = 0; i < recvLen; i++)
                    {
                        com_recv_buf[com_recv_cnt+i] = recvBuffer[i];
                    }
                     * */
                    com_recv_cnt += recvLen;

                    if (com_recv_cnt >= 79)
                    {

                        //此处加数据处理
                        //for (i = 0; i < recvLen; i++)
                        for (i = 0; i < com_recv_cnt; i++)
                        {
                            //c = recvBuffer[i];
                            c = com_recv_buf[i];
                            switch (comA_recvpacket.state)
                            {
                                case RECV_HEAD1:
                                    if (c == 0xaa)
                                    {
                                        comA_recvpacket.state = RECV_HEAD2;
                                        comA_recvpacket.checksum = c;
                                    }
                                    else
                                    {
                                        error1++;
                                    }
                                    packbuflen = 0;
                                    break;
                                case RECV_HEAD2:
                                    if (c == 0x55)
                                    {
                                        comA_recvpacket.state = RECV_LEN;
                                        comA_recvpacket.checksum += c;
                                    }
                                    else
                                    {
                                        comA_recvpacket.state = RECV_HEAD1;
                                        error2++;
                                    }
                                    packbuflen = 0;
                                    break;
                                case RECV_LEN:
                                    comA_recvpacket.len = c;
                                    comA_recvpacket.state = RECV_CNT;
                                    comA_recvpacket.checksum += c;
                                    packbuflen = 0;
                                    break;
                                case RECV_CNT:
                                    comA_recvpacket.cnt = c;
                                    comA_recvpacket.state = RECV_SYSID;
                                    comA_recvpacket.checksum += c;
                                    packbuflen = 0;
                                    break;
                                case RECV_SYSID:
                                    comA_recvpacket.sysid = c;
                                    comA_recvpacket.state = RECV_TYPE;
                                    comA_recvpacket.checksum += c;
                                    packbuflen = 0;
                                    break;
                                case RECV_TYPE:
                                    comA_recvpacket.type = c;
                                    comA_recvpacket.state = RECV_DATA;
                                    comA_recvpacket.checksum += c;
                                    packbuflen = 0;
                                    break;
                                case RECV_DATA:
                                    packbuf[packbuflen] = c;
                                    comA_recvpacket.checksum += c;
                                    packbuflen++;

                                    if (packbuflen >= comA_recvpacket.len)
                                    {
                                        comA_recvpacket.state = RECV_CHECKSUM;
                                    }
                                    break;
                                case RECV_CHECKSUM:
                                    if (comA_recvpacket.checksum == c)
                                    {
                                        if (comA_recvpacket.type == ID_AP2GCS_REAL)//AP-->GCS的实时数据
                                        {
                                            //ap2gcs_real.lng = BitConverter.ToUInt32(packbuf,0);//[度*0.00001]
                                            //ap2gcs_real.lat = BitConverter.ToUInt32(packbuf,4);//[度*0.00001]
                                            //ap2gcs_real.spd = BitConverter.ToInt16(packbuf,8);//[Knot*0.01]，实时航速
                                            //ap2gcs_real.dir = BitConverter.ToInt16(packbuf,10);//[度*0.01]，航向
                                            //ap2gcs_real.pitch = BitConverter.ToInt16(packbuf,12);//[度*0.01]，俯仰
                                            //ap2gcs_real.roll = BitConverter.ToInt16(packbuf,14);//[度*0.01]，滚转
                                            //ap2gcs_real.yaw = BitConverter.ToInt16(packbuf,16);//[度*0.01]，偏航
                                            //ap2gcs_real.moo_pwm = packbuf[18];//主电机启停舵机PWM 0-255
                                            //ap2gcs_real.mbf_pwm = packbuf[19];//主电机前进后退舵机PWM 0-255
                                            //ap2gcs_real.rud_pwm = packbuf[20];//方向舵舵机PWM 0-255
                                            //ap2gcs_real.mm_state = packbuf[21];//主推进电机状态
                                            //ap2gcs_real.rud_p = packbuf[22];//方向舵机控制P增益
                                            //ap2gcs_real.rud_i = packbuf[23];//方向舵机控制I增益
                                            //ap2gcs_real.spare1 = packbuf[24];//预留
                                            //ap2gcs_real.boat_temp1 = packbuf[25];//[度]，艇内1号点温度
                                            //ap2gcs_real.boat_temp2 = packbuf[26];//[度]，艇内2号点温度
                                            //ap2gcs_real.boat_humi = packbuf[27];//[%]，艇内湿度
                                            //ap2gcs_real.wpno = packbuf[28];//下一航点编号，0xff表示是GCS发送的新航点
                                            //ap2gcs_real.spare2 = packbuf[29];





                                            //wangbo
                                            //ap2gcs_real.lng = BitConverter.ToUInt32(packbuf, 0);//[度*0.00001]
                                            //ap2gcs_real.lat = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                            recv_tmp1 = BitConverter.ToUInt32(packbuf, 0);//[度*0.00001]
                                            recv_tmp2 = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                            if ((recv_tmp1 < 1000000) || (recv_tmp2 < 1000000))//取消通信错误数据
                                            {

                                                //also display when no GPS position ...FEIQING20161023
                                                ap2gcs_real.lng = recv_tmp1;
                                                ap2gcs_real.lat = recv_tmp2;
                                                ap2gcs_real.spd = BitConverter.ToUInt32(packbuf, 8);//[Knot*0.01]，实时航速
                                                ap2gcs_real.dir_gps = BitConverter.ToInt16(packbuf, 12);//[度*0.01]，GPS航向
                                                ap2gcs_real.dir_heading = BitConverter.ToInt16(packbuf, 14);//[度*0.01]，机头朝向
                                                ap2gcs_real.dir_target = BitConverter.ToInt16(packbuf, 16);//[度*0.01]，目标点方向
                                                ap2gcs_real.dir_nav = BitConverter.ToInt16(packbuf, 18);//[度*0.01]，导航航向
                                                ap2gcs_real.pitch = BitConverter.ToInt16(packbuf, 20);//[度*0.01]，俯仰
                                                ap2gcs_real.roll = BitConverter.ToInt16(packbuf, 22);//[度*0.01]，滚转
                                                ap2gcs_real.yaw = BitConverter.ToInt16(packbuf, 24);//[度*0.01]，偏航
                                                ap2gcs_real.moo_pwm = packbuf[26];//主电机启停舵机PWM 0-255
                                                ap2gcs_real.mbf_pwm = packbuf[27];//主电机前进后退舵机PWM 0-255
                                                ap2gcs_real.rud_pwm = packbuf[28];//方向舵舵机PWM 0-255
                                                ap2gcs_real.mm_state = packbuf[29];//主推进电机状态
                                                ap2gcs_real.rud_p = packbuf[30];//方向舵机控制P增益
                                                ap2gcs_real.rud_i = packbuf[31];//方向舵机控制I增益
                                                ap2gcs_real.boat_temp0 = packbuf[32];//预留
                                                ap2gcs_real.boat_temp1 = packbuf[33];//[度]，艇内1号点温度
                                                ap2gcs_real.boat_temp2 = packbuf[34];//[度]，艇内2号点温度
                                                ap2gcs_real.wp_load_cnt = packbuf[35];//[%]，艇内湿度
                                                ap2gcs_real.wpno = packbuf[36];//下一航点编号，0xff表示是GCS发送的新航点

                                                ap2gcs_real.generator_onoff_req = packbuf[37];
                                                ap2gcs_real.voltage_bat1= packbuf[38];
                                                ap2gcs_real.voltage_bat2 = packbuf[39];
                                                ap2gcs_real.toggle_state= packbuf[40];
                                                ap2gcs_real.charge_state= packbuf[41];
                                                ap2gcs_real.temp= packbuf[42];
                                                ap2gcs_real.humi = packbuf[43];
                                                ap2gcs_real.windspeed = packbuf[44];
                                                ap2gcs_real.winddir = packbuf[45];
                                                ap2gcs_real.airpress = packbuf[46];
                                                ap2gcs_real.seasault = packbuf[47];
                                                ap2gcs_real.elec_cond = packbuf[48];
                                                ap2gcs_real.seatemp1 = packbuf[49];
                                                ap2gcs_real.launch_req_ack = packbuf[50];
                                                ap2gcs_real.rocket_state= packbuf[51];
                                                ap2gcs_real.rktnumber= packbuf[52];
                                                ap2gcs_real.spare1 = packbuf[53];
                                                ap2gcs_real.alt = BitConverter.ToInt16(packbuf,54 );
                                                
                                                ap2gcs_real.spare10 = BitConverter.ToUInt32(packbuf, 56);
                                                ap2gcs_real.spare11 = BitConverter.ToUInt32(packbuf, 60);
                                                ap2gcs_real.spare12 = BitConverter.ToUInt32(packbuf, 64);
                                                ap2gcs_real.spare13 = BitConverter.ToUInt32(packbuf, 68);

                                            }
                                            else
                                            {
                                                ap2gcs_real.lng = recv_tmp1;
                                                ap2gcs_real.lat = recv_tmp2;
                                                ap2gcs_real.spd = BitConverter.ToUInt32(packbuf, 8);//[Knot*0.01]，实时航速
                                                ap2gcs_real.dir_gps = BitConverter.ToInt16(packbuf, 12);//[度*0.01]，GPS航向
                                                ap2gcs_real.dir_heading = BitConverter.ToInt16(packbuf, 14);//[度*0.01]，机头朝向
                                                ap2gcs_real.dir_target = BitConverter.ToInt16(packbuf, 16);//[度*0.01]，目标点方向
                                                ap2gcs_real.dir_nav = BitConverter.ToInt16(packbuf, 18);//[度*0.01]，导航航向
                                                ap2gcs_real.pitch = BitConverter.ToInt16(packbuf, 20);//[度*0.01]，俯仰
                                                ap2gcs_real.roll = BitConverter.ToInt16(packbuf, 22);//[度*0.01]，滚转
                                                ap2gcs_real.yaw = BitConverter.ToInt16(packbuf, 24);//[度*0.01]，偏航
                                                ap2gcs_real.moo_pwm = packbuf[26];//主电机启停舵机PWM 0-255
                                                ap2gcs_real.mbf_pwm = packbuf[27];//主电机前进后退舵机PWM 0-255
                                                ap2gcs_real.rud_pwm = packbuf[28];//方向舵舵机PWM 0-255
                                                ap2gcs_real.mm_state = packbuf[29];//主推进电机状态
                                                ap2gcs_real.rud_p = packbuf[30];//方向舵机控制P增益
                                                ap2gcs_real.rud_i = packbuf[31];//方向舵机控制I增益
                                                ap2gcs_real.spare1 = packbuf[32];//预留
                                                ap2gcs_real.boat_temp1 = packbuf[33];//[度]，艇内1号点温度
                                                ap2gcs_real.boat_temp2 = packbuf[34];//[度]，艇内2号点温度
                                                ap2gcs_real.wp_load_cnt = packbuf[35];//[%]，艇内湿度
                                                ap2gcs_real.wpno = packbuf[36];//下一航点编号，0xff表示是GCS发送的新航点


                                                ap2gcs_real.generator_onoff_req = packbuf[37];
                                                ap2gcs_real.voltage_bat1 = packbuf[38];
                                                ap2gcs_real.voltage_bat2 = packbuf[39];
                                                ap2gcs_real.toggle_state = packbuf[40];
                                                ap2gcs_real.charge_state = packbuf[41];
                                                ap2gcs_real.temp = packbuf[42];
                                                ap2gcs_real.humi = packbuf[43];
                                                ap2gcs_real.windspeed = packbuf[44];
                                                ap2gcs_real.winddir = packbuf[45];
                                                ap2gcs_real.airpress = packbuf[46];
                                                ap2gcs_real.seasault = packbuf[47];
                                                ap2gcs_real.elec_cond = packbuf[48];
                                                ap2gcs_real.seatemp1 = packbuf[49];
                                                ap2gcs_real.launch_req_ack = packbuf[50];
                                                ap2gcs_real.rocket_state = packbuf[51];
                                                ap2gcs_real.rktnumber = packbuf[52];
                                                ap2gcs_real.spare1 = packbuf[53];
                                                ap2gcs_real.alt = BitConverter.ToInt16(packbuf, 54);

                                                ap2gcs_real.spare10 = BitConverter.ToUInt32(packbuf, 56);
                                                ap2gcs_real.spare11 = BitConverter.ToUInt32(packbuf, 60);
                                                ap2gcs_real.spare12 = BitConverter.ToUInt32(packbuf, 64);
                                                ap2gcs_real.spare13 = BitConverter.ToUInt32(packbuf, 68);
                                            }


                                            //实时数据接收计数
                                            gbl_var.ap2gcs_real_cnt++;
                                        }
                                        else if (comA_recvpacket.type == ID_AP2GCS_CMD)//AP-->GCS命令回传包
                                        {
                                            ap2gcs_cmd_back.cmd_state = packbuf[0];
                                            ap2gcs_cmd_back.cmd_test = packbuf[1];
                                            ap2gcs_cmd_back.cmd_manu = packbuf[2];
                                            ap2gcs_cmd_back.cmd_auto = packbuf[3];
                                            ap2gcs_cmd_back.cmd_rkt = packbuf[4];
                                            ap2gcs_cmd_back.cmd_aws = packbuf[5];
                                            ap2gcs_cmd_back.cmd_mot = packbuf[6];
                                            ap2gcs_cmd_back.cmd_flag = packbuf[7];
                                            ap2gcs_cmd_back.moo_pwm = packbuf[8];
                                            ap2gcs_cmd_back.mbf_pwm = packbuf[9];
                                            ap2gcs_cmd_back.rud_pwm = packbuf[10];
                                            ap2gcs_cmd_back.rud_p = packbuf[11];
                                            ap2gcs_cmd_back.rud_i = packbuf[12];
                                            ap2gcs_cmd_back.wpno = packbuf[13];
                                            if ((ap2gcs_cmd_back.cmd_state == gcs2ap_cmd.cmd_state)
                                                && (ap2gcs_cmd_back.cmd_state == 0x3))//测试状态时清限位设置命令
                                            {
                                                //如果D2-D0不为零，且D5-D3不为零，则表明上次发送的是舵机位置标定值，
                                                //此时，记录该位置并将该指令清除
                                                if (((ap2gcs_cmd_back.cmd_test & 0x7) != 0) && ((ap2gcs_cmd_back.cmd_test & 0x38) != 0))
                                                {
                                                    if (ap2gcs_cmd_back.cmd_test == 0x9)
                                                    {
                                                        gbl_var.motor_on_pwm = ap2gcs_cmd_back.moo_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x11)
                                                    {
                                                        gbl_var.motor_off_pwm = ap2gcs_cmd_back.moo_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x1a)
                                                    {
                                                        gbl_var.motor_fwd_pwm = ap2gcs_cmd_back.mbf_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x22)
                                                    {
                                                        gbl_var.motor_bwd_pwm = ap2gcs_cmd_back.mbf_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x2c)
                                                    {
                                                        gbl_var.rud_left_pwm = ap2gcs_cmd_back.rud_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x34)
                                                    {
                                                        gbl_var.rud_right_pwm = ap2gcs_cmd_back.rud_pwm;
                                                    }
                                                    if (ap2gcs_cmd_back.cmd_test == 0x3c)
                                                    {
                                                        gbl_var.rud_mid_pwm = ap2gcs_cmd_back.rud_pwm;
                                                    }
                                                    gcs2ap_cmd_new.cmd_test &= 0xc7;//将位置设置命令清除
                                                    gcs2ap_cmd_new.cmd_flag &= 0xf7;//清回传命令
                                                    gbl_var.send_req_cnt++;
                                                    gbl_var.send_cmd_req = true;
                                                }
                                            }

                                        }
                                        else if (comA_recvpacket.type == ID_AP2GCS_WP)//AP-->GCS航点回传包
                                        {

                                            ap2gcs_wp_back.type = packbuf[0];
                                            ap2gcs_wp_back.total = packbuf[1];
                                            ap2gcs_wp_back.no = packbuf[2];
                                            ap2gcs_wp_back.spd = packbuf[3];
                                            ap2gcs_wp_back.lng = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                            ap2gcs_wp_back.lat = BitConverter.ToUInt32(packbuf, 8);//[度*0.00001]
                                            gbl_var.ap2gcs_wp_cnt++;
                                            error6++;

                                            gbl_var.all_wp_num = ap2gcs_wp_back.total;

                                            //下面的不知道为什么放在这里不对
                                            //textBox_all_wp_num.Text = ap2gcs_wp_back.total.ToString();
                                            //textBox_all_wp_num.Text = Convert.ToString(ap2gcs_wp_back.total);

                                            if (gbl_var.parameter_set_get_wp)
                                            {

                                                //把航点存储在totalWPlist中去
                                                //totalWPlist[ap2gcs_wp_back.no].Lng = ap2gcs_wp_back.lng;
                                                //totalWPlist[ap2gcs_wp_back.no].Lat = ap2gcs_wp_back.lat;
                                                waypoint_test.Lng = ap2gcs_wp_back.lng * 0.00001;
                                                waypoint_test.Lat = ap2gcs_wp_back.lat * 0.00001;

                                                //totalWPlist.Add(new PointLatLngAlt(homePos.Lat, homePos.Lng, 0.0, "H"));
                                                totalWPlist.Add(new PointLatLngAlt(waypoint_test.Lat, waypoint_test.Lng, 0.0, Convert.ToString(ap2gcs_wp_back.no)));

                                                if (gcs2ap_parameter.value == 127)
                                                {

                                                    if (ap2gcs_wp_back.no == ap2gcs_wp_back.total - 1)
                                                    {
                                                        //接收所有的航点接收完全，就重新航点
                                                        gbl_var.parameter_set_get_wp = false;



                                                        ReDrawAllWP();//重画全部航点
                                                        ReDrawAllRoute();//重画全部路径
                                                    }
                                                }
                                                else
                                                {
                                                    //每次接收1个航点，就重新画图
                                                    gbl_var.parameter_set_get_wp = false;
                                                    ReDrawAllWP();//重画全部航点
                                                    ReDrawAllRoute();//重画全部路径


                                                }

                                                if (ap2gcs_wp_back.no == ap2gcs_wp_back.total - 1)
                                                {
                                                    //接收航点接收完全，就重新航点
                                                    gbl_var.parameter_set_get_wp = false;



                                                    ReDrawAllWP();//重画全部航点
                                                    ReDrawAllRoute();//重画全部路径
                                                }

                                            }
                                            else
                                            {
                                                if ((gcs2ap_wp.no == ap2gcs_wp_back.no) && (gcs2ap_wp.type == ap2gcs_wp_back.type)
                                                && (gcs2ap_wp.total == ap2gcs_wp_back.total) && (gcs2ap_wp.lat == ap2gcs_wp_back.lat)
                                                && (gcs2ap_wp.lng == ap2gcs_wp_back.lng) && (gcs2ap_wp.spd == ap2gcs_wp_back.spd))
                                                {
                                                    error7++;
                                                    if ((gcs2ap_wp.type == 1) && ((gcs2ap_wp.no + 1) < gcs2ap_wp.total))//全部航点还没有都发送完
                                                    //if ((gcs2ap_wp.type == 1) && (() <= gcs2ap_wp.total))//全部航点还没有都发送完
                                                    {
                                                        //firstWp++;
                                                        gcs2ap_wp.no++;
                                                        gcs2ap_wp.spd = Convert.ToByte(totalWPlist[gcs2ap_wp.no].Alt * 10);
                                                        gcs2ap_wp.lng = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lng * 100000);
                                                        gcs2ap_wp.lat = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lat * 100000);
                                                        gbl_var.send_req_cnt++;
                                                        gbl_var.send_wp_req = true;
                                                        error8++;
                                                    }
                                                    else
                                                    {
                                                        gcs2ap_wp.type = 0x0;//不是要发送全部航点，或者已经全部发送完毕，则不再发送  
                                                        error4++;
                                                    }

                                                }
                                                else//接收错误，需要重发
                                                {
                                                    //to be done
                                                    error5++;
                                                }
                                            }

#if false
                                        //王博 这是之前的，现在加个限制就是地面站重启后获取航点
                                        //接收正确，可发送下一个航点或者结束发送
                                        if ((gcs2ap_wp.no == ap2gcs_wp_back.no) && (gcs2ap_wp.type == ap2gcs_wp_back.type)
                                            && (gcs2ap_wp.total == ap2gcs_wp_back.total) && (gcs2ap_wp.lat == ap2gcs_wp_back.lat)
                                            && (gcs2ap_wp.lng == ap2gcs_wp_back.lng) && (gcs2ap_wp.spd == ap2gcs_wp_back.spd))
                                        {
                                            error7++;
                                            if ((gcs2ap_wp.type == 1) && ((gcs2ap_wp.no + 1) < gcs2ap_wp.total))//全部航点还没有都发送完
                                            //if ((gcs2ap_wp.type == 1) && (() <= gcs2ap_wp.total))//全部航点还没有都发送完
                                            {
                                                //firstWp++;
                                                gcs2ap_wp.no++;
                                                gcs2ap_wp.spd = Convert.ToByte(totalWPlist[gcs2ap_wp.no].Alt * 10);
                                                gcs2ap_wp.lng = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lng * 100000);
                                                gcs2ap_wp.lat = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lat * 100000);
                                                gbl_var.send_req_cnt++;
                                                gbl_var.send_wp_req = true;
                                                error8++;
                                            }
                                            else
                                            {
                                                gcs2ap_wp.type = 0x0;//不是要发送全部航点，或者已经全部发送完毕，则不再发送  
                                                error4++;
                                            }

                                        }
                                        else//接收错误，需要重发
                                        {
                                            //to be done
                                            error5++;
                                        }
#endif

                                        }
                                        else if (comA_recvpacket.type == ID_AP2GCS_ACK)//AP-->GCS接收确认包
                                        {
                                            ap2gcs_ack.type = packbuf[0];//接收数据包类型
                                            ap2gcs_ack.cnt = packbuf[1];//接收数据包编号
                                            ap2gcs_ack.state = packbuf[2];//接收数据包状态
                                            ap2gcs_ack.spare = packbuf[3];
                                            ap2gcs_ack.hhmmss = BitConverter.ToUInt32(packbuf, 4);//接收数据包时间
                                            gbl_var.ap2gcs_ack_cnt++;
                                        }
                                        else if (comA_recvpacket.type == ID_AP2GCS_AWS)//AP-->GCS气象站数据包
                                        {
                                            ap2gcs_aws.hhmmss = BitConverter.ToUInt32(packbuf, 0);//发送数据包时间
                                            ap2gcs_aws.lng = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                            ap2gcs_aws.lat = BitConverter.ToUInt32(packbuf, 8);//[度*0.00001]
                                            ap2gcs_aws.temp = BitConverter.ToInt16(packbuf, 12);//[度*0.01]
                                            ap2gcs_aws.dewtemp = BitConverter.ToInt16(packbuf, 14);//[度*0.01]
                                            ap2gcs_aws.humi = BitConverter.ToUInt16(packbuf, 16);//[*0.01]
                                            ap2gcs_aws.airpress = BitConverter.ToUInt16(packbuf, 18);//[*0.01]
                                            ap2gcs_aws.winddir = BitConverter.ToInt16(packbuf, 20);//[*0.01]
                                            ap2gcs_aws.windspd = BitConverter.ToInt16(packbuf, 22);//[*0.01]
                                            gbl_var.ap2gcs_aws_cnt++;
                                        }
                                        else if (comA_recvpacket.type == ID_AP2GCS_PARAMETER)
                                        {
                                            ap2gcs_parameter.type = packbuf[0];
                                            ap2gcs_parameter.value = packbuf[1];

                                            switch (Convert.ToInt32(ap2gcs_parameter.type))
                                            {
                                                case PARAMETER_GET_CRUISE_THROTTLE:
                                                    //王博wangbo这里很奇怪，为什么这里赋值就会跳到catch那里，到底哪里有溢出错误呢
                                                    //可能是因为多线程，这里无法访问，错误显示是text=函数求值需要运行所有线程
                                                    //textBox_get_throttle.Text = Convert.ToString(ap2gcs_parameter.value);
                                                    gbl_var.parameter_get_cruise_throttle = true;
                                                    break;
                                                case PARAMETER_GET_ARRIVE_RADIUS:
                                                    //textBox_get_arrive_radius.Text = Convert.ToString(ap2gcs_parameter.value);
                                                    gbl_var.parameter_get_arrive_radius = true;
                                                    break;
                                                case PARAMETER_GET_CTE_MAX_CORRECT:
                                                    gbl_var.parameter_get_max_head_error_angle = true;
                                                    break;
                                                case PARAMETER_GET_ROTARY_POSITION:
                                                    gbl_var.parameter_get_rotary_position = true;
                                                    break;

                                                default:
                                                    break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        error3++;
                                    }
                                    comA_recvpacket.state = RECV_HEAD1;
                                    packbuflen = 0;

                                    break;
                            }
                        }
                        com_recv_cnt = 0;//Add 20161119
                    }
                }
                catch
                {
                    if (!comPortA.IsOpen)
                    {
                        while (comAListening) Application.DoEvents();//打开时点击，则关闭串口
                        comAClosing = true;
                        comPortA.Close();
                        cbxACOMPort.Enabled = true;
                        cbxABaudRate.Enabled = true;
                        btnPortAOpenClose.Text = "打开";
                        statusPortAName.Text = "串口A: ";
                    }
                }
                finally
                {
                    comAListening = false;//可以关闭串口
                }
            }
        }

        //wangbo20170118 北斗串口接收从自驾仪来的实时数据
        /// <summary>
        /// 接收缓冲区尺寸
        /// </summary>
        public static int packbuflen_bd;

        //wangbo
        //public static int firstWp;

        byte[] com_recv_buf_bd = new byte[500];//接收缓冲区

        public static int com_recv_cnt_bd = 0;

        //char[] rcvQ=new char[1024];
        byte[] rcvQ = new byte[1024];
        int rcvQTail;
        byte [] recvpackbuf=new byte[200];

        String STR_GLZK = "$GLZK";
        String STR_TXXX = "$TXXX";
        String STR_ICXX = "$ICCX";
        String STR_FKXX = "$FKXX";

        private void comBReceived(object sender, SerialDataReceivedEventArgs e)//接收数据 中断只标志有数据需要读取，读取操作在中断外进行
        {
            int recvLen;
            UInt16 data_len;
            int i = 0;
            uint recv_tmp1, recv_tmp2;
            byte c = 0;
            byte[] packbuf = new byte[500];//接收缓冲区
            for (i = 0; i < 500; i++) packbuf[i] = 0;

            int switch2beidou_data=0;

            if (comBClosing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            Thread.Sleep(50);
            if (false)
            {
                comPortB.DiscardInBuffer();//清接收缓存
            }
            else
            {
                byte[] recvBuffer;//接收缓冲区
                try
                {
                    comBListening = true;//设置标记，说明我已经开始处理数据，一会儿要使用系统UI的。
                    /*
                    recvBuffer = new byte[comPortA.BytesToRead];//接收数据缓存大小
                    recvLen = comPortA.Read(recvBuffer, 0, recvBuffer.Length);//读取数据
                    comArecvCount += recvLen;
                    */

                    recvBuffer = new byte[comPortB.BytesToRead];//接收数据缓存大小
                    recvLen = comPortB.Read(recvBuffer, 0, recvBuffer.Length);//读取数据
                    //memcpy(&rcvQ[rcvQTail], buf, nread);
                    Array.Copy(recvBuffer, 0, rcvQ, rcvQTail, recvLen);

                    rcvQTail += recvLen;

                    /*********************/
                    /*********************/
                    //recvBuffer是通过PortB读取的全部数据，要进行北斗的枕头解析，去掉后然后再解析我们自己定义的数据包

                    int nread=0;
                    //int i = 0;
                    int j=0;
                    char[] buf=new char[500];
                    int pack_len;
                    byte checksum;

                    rcvQTail = 0;

                    if (rcvQTail >= 12)//大于最小包的长度，可以进行处理
                    {
                        for (i = 0; i < (rcvQTail - 7); i++) //rcvQTail-7:除去头部ID的5个字节和长度的2个字节
                        {
                            String str_temp;
                            str_temp = System.Text.Encoding.Default.GetString(rcvQ,0,5);

                            if ((string.Compare(str_temp,0, STR_GLZK, 0,5) == 0)
                               || (string.Compare(str_temp, 0, STR_TXXX,0, 5) == 0)//通信信息
                               || (string.Compare(str_temp, 0, STR_ICXX,0, 5) == 0)//IC信息
                               || (string.Compare(str_temp, 0, STR_FKXX,0, 5) == 0)//反馈信息
                               )//包头正确
                            {
                                //memcpy(&pack_len,&rcvQ[i+5],2);

                                pack_len = (((rcvQ[i + 5] << 8) & 0xff00) | (rcvQ[i + 6] & 0xff));
                                pack_len = ((rcvQ[i + 5] << 8) & 0xff00) | (rcvQ[i + 6] & 0xff);
  
                                if ((rcvQTail - i) >= pack_len)//缓冲区内数据长度超过包长度
                                {
                                    Array.Copy(rcvQ, i, recvpackbuf, 0, pack_len);

                                    checksum = 0;
                                    for (j = 0; j < (pack_len - 1); j++)
                                    {
                                        checksum ^= recvpackbuf[j];
                                    }
                                    //if (checksum == recvpackbuf[pack_len - 1])//包校验正确，表示收到正确数据包，可以进行解析
                                    if ((checksum == recvpackbuf[pack_len - 1]) && (string.Compare(str_temp, 0, STR_TXXX, 0, 5) == 0))
                                    {
                                        //解析完，删除已解析的数据
                                        Array.Copy(rcvQ, i + pack_len, rcvQ, 0, (rcvQTail - (i + pack_len)));
                                        rcvQTail = rcvQTail - (i + pack_len);

                                        //处理数据 数据是recvpackbuf 长度是pack_len
                                        //bd_recv_process(pack_len, recvpackbuf);

                                        data_len = BitConverter.ToUInt16(recvpackbuf, 16);

                                        Array.Copy(recvpackbuf, 18, com_recv_buf_bd, 0, data_len);

                                        com_recv_cnt_bd = data_len;

                                        //if (com_recv_cnt_bd >= 79)
                                        if (true)
                                        {
                                            for (i = 0; i < com_recv_cnt_bd; i++)
                                            {
                                                c = com_recv_buf_bd[i];
                                                switch (comB_recvpacket.state)
                                                {
                                                    case RECV_HEAD1:
                                                        if (c == 0xaa)
                                                        {
                                                            comB_recvpacket.state = RECV_HEAD2;
                                                            comB_recvpacket.checksum = c;
                                                        }
                                                        else
                                                        {
                                                            error1++;
                                                        }
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_HEAD2:
                                                        if (c == 0x55)
                                                        {
                                                            comB_recvpacket.state = RECV_LEN;
                                                            comB_recvpacket.checksum += c;
                                                        }
                                                        else
                                                        {
                                                            comB_recvpacket.state = RECV_HEAD1;
                                                            error2++;
                                                        }
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_LEN:
                                                        comB_recvpacket.len = c;
                                                        comB_recvpacket.state = RECV_CNT;
                                                        comB_recvpacket.checksum += c;
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_CNT:
                                                        comB_recvpacket.cnt = c;
                                                        comB_recvpacket.state = RECV_SYSID;
                                                        comB_recvpacket.checksum += c;
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_SYSID:
                                                        comB_recvpacket.sysid = c;
                                                        comB_recvpacket.state = RECV_TYPE;
                                                        comB_recvpacket.checksum += c;
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_TYPE:
                                                        comB_recvpacket.type = c;
                                                        comB_recvpacket.state = RECV_DATA;
                                                        comB_recvpacket.checksum += c;
                                                        packbuflen_bd = 0;
                                                        break;
                                                    case RECV_DATA:
                                                        packbuf[packbuflen_bd] = c;
                                                        comB_recvpacket.checksum += c;
                                                        packbuflen_bd++;

                                                        if (packbuflen_bd >= comB_recvpacket.len)
                                                        {
                                                            comB_recvpacket.state = RECV_CHECKSUM;
                                                        }
                                                        break;
                                                    case RECV_CHECKSUM:
                                                        if (comB_recvpacket.checksum == c)
                                                        {
                                                            if (comB_recvpacket.type == ID_AP2GCS_REAL)//AP-->GCS的实时数据
                                                            {
                                                                //wangbo
                                                                //ap2gcs_real.lng = BitConverter.ToUInt32(packbuf, 0);//[度*0.00001]
                                                                //ap2gcs_real.lat = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                                                recv_tmp1 = BitConverter.ToUInt32(packbuf, 0);//[度*0.00001]
                                                                recv_tmp2 = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                                                if ((recv_tmp1 < 1000000) || (recv_tmp2 < 1000000))//取消通信错误数据
                                                                {
                                                                    ap2gcs_real.boat_temp2 = packbuf[26];//先把主电机启停舵机PWM显示在温度2中 测试能不能收到北斗数据
                                                                    if (switch2beidou_data==1)
                                                                    {
                                                                        //also display when no GPS position ...FEIQING20161023
                                                                        ap2gcs_real.lng = recv_tmp1;
                                                                        ap2gcs_real.lat = recv_tmp2;
                                                                        ap2gcs_real.spd = BitConverter.ToUInt32(packbuf, 8);//[Knot*0.01]，实时航速
                                                                        ap2gcs_real.dir_gps = BitConverter.ToInt16(packbuf, 12);//[度*0.01]，GPS航向
                                                                        ap2gcs_real.dir_heading = BitConverter.ToInt16(packbuf, 14);//[度*0.01]，机头朝向
                                                                        ap2gcs_real.dir_target = BitConverter.ToInt16(packbuf, 16);//[度*0.01]，目标点方向
                                                                        ap2gcs_real.dir_nav = BitConverter.ToInt16(packbuf, 18);//[度*0.01]，导航航向
                                                                        ap2gcs_real.pitch = BitConverter.ToInt16(packbuf, 20);//[度*0.01]，俯仰
                                                                        ap2gcs_real.roll = BitConverter.ToInt16(packbuf, 22);//[度*0.01]，滚转
                                                                        ap2gcs_real.yaw = BitConverter.ToInt16(packbuf, 24);//[度*0.01]，偏航
                                                                        ap2gcs_real.moo_pwm = packbuf[26];//主电机启停舵机PWM 0-255
                                                                        ap2gcs_real.mbf_pwm = packbuf[27];//主电机前进后退舵机PWM 0-255
                                                                        ap2gcs_real.rud_pwm = packbuf[28];//方向舵舵机PWM 0-255
                                                                        ap2gcs_real.mm_state = packbuf[29];//主推进电机状态
                                                                        ap2gcs_real.rud_p = packbuf[30];//方向舵机控制P增益
                                                                        ap2gcs_real.rud_i = packbuf[31];//方向舵机控制I增益
                                                                        ap2gcs_real.boat_temp0 = packbuf[32];//预留
                                                                        ap2gcs_real.boat_temp1 = packbuf[33];//[度]，艇内1号点温度
                                                                        ap2gcs_real.boat_temp2 = packbuf[34];//[度]，艇内2号点温度
                                                                        ap2gcs_real.wp_load_cnt = packbuf[35];//[%]，艇内湿度
                                                                        ap2gcs_real.wpno = packbuf[36];//下一航点编号，0xff表示是GCS发送的新航点

                                                                        ap2gcs_real.generator_onoff_req = packbuf[37];
                                                                        ap2gcs_real.voltage_bat1 = packbuf[38];
                                                                        ap2gcs_real.voltage_bat2 = packbuf[39];
                                                                        ap2gcs_real.toggle_state = packbuf[40];
                                                                        ap2gcs_real.charge_state = packbuf[41];
                                                                        ap2gcs_real.temp = packbuf[42];
                                                                        ap2gcs_real.humi = packbuf[43];
                                                                        ap2gcs_real.windspeed = packbuf[44];
                                                                        ap2gcs_real.winddir = packbuf[45];
                                                                        ap2gcs_real.airpress = packbuf[46];
                                                                        ap2gcs_real.seasault = packbuf[47];
                                                                        ap2gcs_real.elec_cond = packbuf[48];
                                                                        ap2gcs_real.seatemp1 = packbuf[49];
                                                                        ap2gcs_real.launch_req_ack = packbuf[50];
                                                                        ap2gcs_real.rocket_state = packbuf[51];
                                                                        ap2gcs_real.rktnumber = packbuf[52];
                                                                        ap2gcs_real.spare1 = packbuf[53];
                                                                        ap2gcs_real.alt = BitConverter.ToInt16(packbuf, 54);

                                                                        ap2gcs_real.spare10 = BitConverter.ToUInt32(packbuf, 56);
                                                                        ap2gcs_real.spare11 = BitConverter.ToUInt32(packbuf, 60);
                                                                        ap2gcs_real.spare12 = BitConverter.ToUInt32(packbuf, 64);
                                                                        ap2gcs_real.spare13 = BitConverter.ToUInt32(packbuf, 68);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (switch2beidou_data == 1)
                                                                    {
                                                                        ap2gcs_real.lng = recv_tmp1;
                                                                        ap2gcs_real.lat = recv_tmp2;
                                                                        ap2gcs_real.spd = BitConverter.ToUInt32(packbuf, 8);//[Knot*0.01]，实时航速
                                                                        ap2gcs_real.dir_gps = BitConverter.ToInt16(packbuf, 12);//[度*0.01]，GPS航向
                                                                        ap2gcs_real.dir_heading = BitConverter.ToInt16(packbuf, 14);//[度*0.01]，机头朝向
                                                                        ap2gcs_real.dir_target = BitConverter.ToInt16(packbuf, 16);//[度*0.01]，目标点方向
                                                                        ap2gcs_real.dir_nav = BitConverter.ToInt16(packbuf, 18);//[度*0.01]，导航航向
                                                                        ap2gcs_real.pitch = BitConverter.ToInt16(packbuf, 20);//[度*0.01]，俯仰
                                                                        ap2gcs_real.roll = BitConverter.ToInt16(packbuf, 22);//[度*0.01]，滚转
                                                                        ap2gcs_real.yaw = BitConverter.ToInt16(packbuf, 24);//[度*0.01]，偏航
                                                                        ap2gcs_real.moo_pwm = packbuf[26];//主电机启停舵机PWM 0-255
                                                                        ap2gcs_real.mbf_pwm = packbuf[27];//主电机前进后退舵机PWM 0-255
                                                                        ap2gcs_real.rud_pwm = packbuf[28];//方向舵舵机PWM 0-255
                                                                        ap2gcs_real.mm_state = packbuf[29];//主推进电机状态
                                                                        ap2gcs_real.rud_p = packbuf[30];//方向舵机控制P增益
                                                                        ap2gcs_real.rud_i = packbuf[31];//方向舵机控制I增益
                                                                        ap2gcs_real.spare1 = packbuf[32];//预留
                                                                        ap2gcs_real.boat_temp1 = packbuf[33];//[度]，艇内1号点温度
                                                                        ap2gcs_real.boat_temp2 = packbuf[34];//[度]，艇内2号点温度
                                                                        ap2gcs_real.wp_load_cnt = packbuf[35];//[%]，艇内湿度
                                                                        ap2gcs_real.wpno = packbuf[36];//下一航点编号，0xff表示是GCS发送的新航点


                                                                        ap2gcs_real.generator_onoff_req = packbuf[37];
                                                                        ap2gcs_real.voltage_bat1 = packbuf[38];
                                                                        ap2gcs_real.voltage_bat2 = packbuf[39];
                                                                        ap2gcs_real.toggle_state = packbuf[40];
                                                                        ap2gcs_real.charge_state = packbuf[41];
                                                                        ap2gcs_real.temp = packbuf[42];
                                                                        ap2gcs_real.humi = packbuf[43];
                                                                        ap2gcs_real.windspeed = packbuf[44];
                                                                        ap2gcs_real.winddir = packbuf[45];
                                                                        ap2gcs_real.airpress = packbuf[46];
                                                                        ap2gcs_real.seasault = packbuf[47];
                                                                        ap2gcs_real.elec_cond = packbuf[48];
                                                                        ap2gcs_real.seatemp1 = packbuf[49];
                                                                        ap2gcs_real.launch_req_ack = packbuf[50];
                                                                        ap2gcs_real.rocket_state = packbuf[51];
                                                                        ap2gcs_real.rktnumber = packbuf[52];
                                                                        ap2gcs_real.spare1 = packbuf[53];
                                                                        ap2gcs_real.alt = BitConverter.ToInt16(packbuf, 54);

                                                                        ap2gcs_real.spare10 = BitConverter.ToUInt32(packbuf, 56);
                                                                        ap2gcs_real.spare11 = BitConverter.ToUInt32(packbuf, 60);
                                                                        ap2gcs_real.spare12 = BitConverter.ToUInt32(packbuf, 64);
                                                                        ap2gcs_real.spare13 = BitConverter.ToUInt32(packbuf, 68);

                                                                    }                                                                
                                                                }


                                                                //实时数据接收计数
                                                                gbl_var.ap2gcs_real_cnt++;
                                                            }
                                                            else if (comB_recvpacket.type == ID_AP2GCS_CMD)//AP-->GCS命令回传包
                                                            {

                                                                if (switch2beidou_data == 1)
                                                                {
                                                                    ap2gcs_cmd_back.cmd_state = packbuf[0];
                                                                    ap2gcs_cmd_back.cmd_test = packbuf[1];
                                                                    ap2gcs_cmd_back.cmd_manu = packbuf[2];
                                                                    ap2gcs_cmd_back.cmd_auto = packbuf[3];
                                                                    ap2gcs_cmd_back.cmd_rkt = packbuf[4];
                                                                    ap2gcs_cmd_back.cmd_aws = packbuf[5];
                                                                    ap2gcs_cmd_back.cmd_mot = packbuf[6];
                                                                    ap2gcs_cmd_back.cmd_flag = packbuf[7];
                                                                    ap2gcs_cmd_back.moo_pwm = packbuf[8];
                                                                    ap2gcs_cmd_back.mbf_pwm = packbuf[9];
                                                                    ap2gcs_cmd_back.rud_pwm = packbuf[10];
                                                                    ap2gcs_cmd_back.rud_p = packbuf[11];
                                                                    ap2gcs_cmd_back.rud_i = packbuf[12];
                                                                    ap2gcs_cmd_back.wpno = packbuf[13];
                                                                    if ((ap2gcs_cmd_back.cmd_state == gcs2ap_cmd.cmd_state)
                                                                        && (ap2gcs_cmd_back.cmd_state == 0x3))//测试状态时清限位设置命令
                                                                    {
                                                                        //如果D2-D0不为零，且D5-D3不为零，则表明上次发送的是舵机位置标定值，
                                                                        //此时，记录该位置并将该指令清除
                                                                        if (((ap2gcs_cmd_back.cmd_test & 0x7) != 0) && ((ap2gcs_cmd_back.cmd_test & 0x38) != 0))
                                                                        {
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x9)
                                                                            {
                                                                                gbl_var.motor_on_pwm = ap2gcs_cmd_back.moo_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x11)
                                                                            {
                                                                                gbl_var.motor_off_pwm = ap2gcs_cmd_back.moo_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x1a)
                                                                            {
                                                                                gbl_var.motor_fwd_pwm = ap2gcs_cmd_back.mbf_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x22)
                                                                            {
                                                                                gbl_var.motor_bwd_pwm = ap2gcs_cmd_back.mbf_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x2c)
                                                                            {
                                                                                gbl_var.rud_left_pwm = ap2gcs_cmd_back.rud_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x34)
                                                                            {
                                                                                gbl_var.rud_right_pwm = ap2gcs_cmd_back.rud_pwm;
                                                                            }
                                                                            if (ap2gcs_cmd_back.cmd_test == 0x3c)
                                                                            {
                                                                                gbl_var.rud_mid_pwm = ap2gcs_cmd_back.rud_pwm;
                                                                            }
                                                                            gcs2ap_cmd_new.cmd_test &= 0xc7;//将位置设置命令清除
                                                                            gcs2ap_cmd_new.cmd_flag &= 0xf7;//清回传命令
                                                                            gbl_var.send_req_cnt++;
                                                                            gbl_var.send_cmd_req = true;
                                                                        }
                                                                    }
                                                                }

                                                                

                                                            }
                                                            else if (comB_recvpacket.type == ID_AP2GCS_WP)//AP-->GCS航点回传包
                                                            {
                                                                if (switch2beidou_data == 1)
                                                                {


                                                                    ap2gcs_wp_back.type = packbuf[0];
                                                                    ap2gcs_wp_back.total = packbuf[1];
                                                                    ap2gcs_wp_back.no = packbuf[2];
                                                                    ap2gcs_wp_back.spd = packbuf[3];
                                                                    ap2gcs_wp_back.lng = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                                                    ap2gcs_wp_back.lat = BitConverter.ToUInt32(packbuf, 8);//[度*0.00001]
                                                                    gbl_var.ap2gcs_wp_cnt++;
                                                                    error6++;

                                                                    gbl_var.all_wp_num = ap2gcs_wp_back.total;

                                                                    //下面的不知道为什么放在这里不对
                                                                    //textBox_all_wp_num.Text = ap2gcs_wp_back.total.ToString();
                                                                    //textBox_all_wp_num.Text = Convert.ToString(ap2gcs_wp_back.total);

                                                                    if (gbl_var.parameter_set_get_wp)
                                                                    {

                                                                        //把航点存储在totalWPlist中去
                                                                        //totalWPlist[ap2gcs_wp_back.no].Lng = ap2gcs_wp_back.lng;
                                                                        //totalWPlist[ap2gcs_wp_back.no].Lat = ap2gcs_wp_back.lat;
                                                                        waypoint_test.Lng = ap2gcs_wp_back.lng * 0.00001;
                                                                        waypoint_test.Lat = ap2gcs_wp_back.lat * 0.00001;

                                                                        //totalWPlist.Add(new PointLatLngAlt(homePos.Lat, homePos.Lng, 0.0, "H"));
                                                                        totalWPlist.Add(new PointLatLngAlt(waypoint_test.Lat, waypoint_test.Lng, 0.0, Convert.ToString(ap2gcs_wp_back.no)));

                                                                        if (gcs2ap_parameter.value == 127)
                                                                        {

                                                                            if (ap2gcs_wp_back.no == ap2gcs_wp_back.total - 1)
                                                                            {
                                                                                //接收所有的航点接收完全，就重新航点
                                                                                gbl_var.parameter_set_get_wp = false;



                                                                                ReDrawAllWP();//重画全部航点
                                                                                ReDrawAllRoute();//重画全部路径
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            //每次接收1个航点，就重新画图
                                                                            gbl_var.parameter_set_get_wp = false;
                                                                            ReDrawAllWP();//重画全部航点
                                                                            ReDrawAllRoute();//重画全部路径


                                                                        }

                                                                        if (ap2gcs_wp_back.no == ap2gcs_wp_back.total - 1)
                                                                        {
                                                                            //接收航点接收完全，就重新航点
                                                                            gbl_var.parameter_set_get_wp = false;



                                                                            ReDrawAllWP();//重画全部航点
                                                                            ReDrawAllRoute();//重画全部路径
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        if ((gcs2ap_wp.no == ap2gcs_wp_back.no) && (gcs2ap_wp.type == ap2gcs_wp_back.type)
                                                                        && (gcs2ap_wp.total == ap2gcs_wp_back.total) && (gcs2ap_wp.lat == ap2gcs_wp_back.lat)
                                                                        && (gcs2ap_wp.lng == ap2gcs_wp_back.lng) && (gcs2ap_wp.spd == ap2gcs_wp_back.spd))
                                                                        {
                                                                            error7++;
                                                                            if ((gcs2ap_wp.type == 1) && ((gcs2ap_wp.no + 1) < gcs2ap_wp.total))//全部航点还没有都发送完
                                                                            //if ((gcs2ap_wp.type == 1) && (() <= gcs2ap_wp.total))//全部航点还没有都发送完
                                                                            {
                                                                                //firstWp++;
                                                                                gcs2ap_wp.no++;
                                                                                gcs2ap_wp.spd = Convert.ToByte(totalWPlist[gcs2ap_wp.no].Alt * 10);
                                                                                gcs2ap_wp.lng = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lng * 100000);
                                                                                gcs2ap_wp.lat = Convert.ToUInt32(totalWPlist[gcs2ap_wp.no].Lat * 100000);
                                                                                gbl_var.send_req_cnt++;
                                                                                gbl_var.send_wp_req = true;
                                                                                error8++;
                                                                            }
                                                                            else
                                                                            {
                                                                                gcs2ap_wp.type = 0x0;//不是要发送全部航点，或者已经全部发送完毕，则不再发送  
                                                                                error4++;
                                                                            }

                                                                        }
                                                                        else//接收错误，需要重发
                                                                        {
                                                                            //to be done
                                                                            error5++;
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                            else if (comB_recvpacket.type == ID_AP2GCS_ACK)//AP-->GCS接收确认包
                                                            {
                                                                if (switch2beidou_data == 1)
                                                                {
                                                                    ap2gcs_ack.type = packbuf[0];//接收数据包类型
                                                                    ap2gcs_ack.cnt = packbuf[1];//接收数据包编号
                                                                    ap2gcs_ack.state = packbuf[2];//接收数据包状态
                                                                    ap2gcs_ack.spare = packbuf[3];
                                                                    ap2gcs_ack.hhmmss = BitConverter.ToUInt32(packbuf, 4);//接收数据包时间
                                                                    gbl_var.ap2gcs_ack_cnt++;
                                                                }

                                                              
                                                            }
                                                            else if (comB_recvpacket.type == ID_AP2GCS_AWS)//AP-->GCS气象站数据包
                                                            {
                                                                if (switch2beidou_data == 1)
                                                                {
                                                                    ap2gcs_aws.hhmmss = BitConverter.ToUInt32(packbuf, 0);//发送数据包时间
                                                                    ap2gcs_aws.lng = BitConverter.ToUInt32(packbuf, 4);//[度*0.00001]
                                                                    ap2gcs_aws.lat = BitConverter.ToUInt32(packbuf, 8);//[度*0.00001]
                                                                    ap2gcs_aws.temp = BitConverter.ToInt16(packbuf, 12);//[度*0.01]
                                                                    ap2gcs_aws.dewtemp = BitConverter.ToInt16(packbuf, 14);//[度*0.01]
                                                                    ap2gcs_aws.humi = BitConverter.ToUInt16(packbuf, 16);//[*0.01]
                                                                    ap2gcs_aws.airpress = BitConverter.ToUInt16(packbuf, 18);//[*0.01]
                                                                    ap2gcs_aws.winddir = BitConverter.ToInt16(packbuf, 20);//[*0.01]
                                                                    ap2gcs_aws.windspd = BitConverter.ToInt16(packbuf, 22);//[*0.01]
                                                                    gbl_var.ap2gcs_aws_cnt++;
                                                                }

                                                               
                                                            }
                                                            else if (comB_recvpacket.type == ID_AP2GCS_PARAMETER)
                                                            {

                                                                if (switch2beidou_data == 1)
                                                                {
                                                                    ap2gcs_parameter.type = packbuf[0];
                                                                    ap2gcs_parameter.value = packbuf[1];

                                                                    switch (Convert.ToInt32(ap2gcs_parameter.type))
                                                                    {
                                                                        case PARAMETER_GET_CRUISE_THROTTLE:
                                                                            //王博wangbo这里很奇怪，为什么这里赋值就会跳到catch那里，到底哪里有溢出错误呢
                                                                            //可能是因为多线程，这里无法访问，错误显示是text=函数求值需要运行所有线程
                                                                            //textBox_get_throttle.Text = Convert.ToString(ap2gcs_parameter.value);
                                                                            gbl_var.parameter_get_cruise_throttle = true;
                                                                            break;
                                                                        case PARAMETER_GET_ARRIVE_RADIUS:
                                                                            //textBox_get_arrive_radius.Text = Convert.ToString(ap2gcs_parameter.value);
                                                                            gbl_var.parameter_get_arrive_radius = true;
                                                                            break;
                                                                        case PARAMETER_GET_CTE_MAX_CORRECT:
                                                                            gbl_var.parameter_get_max_head_error_angle = true;
                                                                            break;
                                                                        case PARAMETER_GET_ROTARY_POSITION:
                                                                            gbl_var.parameter_get_rotary_position = true;
                                                                            break;

                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                            }//ID_AP2GCS_PARAMETER   结束
                                                        }
                                                        else
                                                        {
                                                            error3++;
                                                        }
                                                        comB_recvpacket.state = RECV_HEAD1;
                                                        packbuflen_bd = 0;

                                                        break;
                                                }
                                            }
                                            com_recv_cnt_bd = 0;//Add 20161119
                                        }

                                    }
                                }//if((rcvQTail-i)>=pack_len)
                            }
                        }//for
                    }// if(rcvQTail>=12)
                    if (rcvQTail > 500)
                    {
                        rcvQTail = 0;
                    }                   
                }//try 括号结束
                catch
                {
                    if (!comPortB.IsOpen)
                    {
                        while (comBListening) Application.DoEvents();//打开时点击，则关闭串口
                        comBClosing = true;
                        comPortB.Close();
                        cbxBCOMPort.Enabled = true;
                        cbxBBaudRate.Enabled = true;
                        btnPortBOpenClose.Text = "打开";
                        statusPortBName.Text = "串口A: ";
                    }
                }
                finally
                {
                    comBListening = false;//可以关闭串口
                }
            }
        }

        /// <summary>
        /// 生成数据包并通过串口发送。数据包格式:
        /// 0xAA | 0x55 | len(0-255) | cnt(0-255) | sysid(255) | type(0-255) | data(length = len) | checksum 
        /// sysid: GCS:0xf, AP: 0x1.
        /// type: GCS->AP: x1: GCS2AP_WP; x2: GCS2AP_CMD; x3: GCS2AP_LNK; x4:GCS2AP_CTE;
        ///       AP->GCS: x11: AP2GCS_REAL; x12: AP2GCS_CMD; x13: AP2GCS_WP; x14: AP2GCS_ACK; x18:AP2GCS_AWS
        /// </summary>
        void generatePacket(byte messageType, byte[] buf, int len)
        {
            if (!comPortA.IsOpen) return;

            lock (comASendlock)
            {
                byte[] packet = new byte[6+len+1];//数据包长度
                packet[0] = 0xaa;
                packet[1] = 0x55;
                packet[2] = (byte)len;
                packet[3] = (byte)comAsendPacketCnt;
                comAsendPacketCnt++;
                packet[4] = SYSID_GCS;
                packet[5] = messageType;

                //把indata中的数据写到Mavlink包的payload区
                int i = 6;
                foreach (byte b in buf)
                {
                    packet[i] = b;
                    i++;
                }

                //改为校验和
                i = 0;
                byte checksum = 0;
                for (i = 0; i < len + 6; i++)
                {
                    checksum += packet[i];
                }
                i = len + 6;
                packet[i] = (byte)(checksum & 0xff);
                i += 1;

                if (comPortA.IsOpen)
                {
                    comPortA.Write(packet, 0, i);//发送数据包，从0字节开始，长度为i,即包长度
                    comAsendCount += len;
                }
            }
        }


        //将Byte转换为结构体类型
        public static byte[] StructToBytes(object structObj, int size)
        {
            //StructDemo sd;
            //int num = 2;
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }

        //将Byte转换为结构体类型
        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            //分配结构体内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        //wangbo 20170118增加铱星或者北斗发送数据
        void generatePacket_beidou(byte messageType, byte[] buf, int len)
        {
            if (!comPortB.IsOpen) return;

            lock (comBSendlock)
            {
                byte[] packet = new byte[6 + len + 1];//数据包长度
                packet[0] = 0xaa;
                packet[1] = 0x55;
                packet[2] = (byte)len;
                packet[3] = (byte)comBsendPacketCnt;
                comBsendPacketCnt++;
                packet[4] = SYSID_GCS;
                packet[5] = messageType;

                //把indata中的数据写到Mavlink包的payload区
                int i = 6;
                foreach (byte b in buf)
                {
                    packet[i] = b;
                    i++;
                }

                //改为校验和
                i = 0;
                byte checksum = 0;
                for (i = 0; i < len + 6; i++)
                {
                    checksum += packet[i];
                }
                i = len + 6;
                packet[i] = (byte)(checksum & 0xff);
                i += 1;

                //北斗通信不能直接发送，需要北斗的协议桢头等
                /*
                if (comPortB.IsOpen)
                {
                    comPortB.Write(packet, 0, i);//发送数据包，从0字节开始，长度为i,即包长度
                    comBsendCount += len;
                }
                */

                //下面的本来是用来把某个结构转为数组的组成北斗包发送的，但是我觉得为了跟电台统一，不用北斗包，直接还是以前的命令包等
                //byte[] struct2byte=new byte[72];
                //struct2byte=StructToBytes(bd_gcs2ap,72);
                //Array.Copy(packet, 0,struct2byte,2, i);

                beidou_send_data(packet, i);
            }
        }
        void beidou_send_data(byte[] buf, int len)
        {
            int pack_len = 0;//包长度
            //short msg_len_bits = 0;
            int msg_len_bits = 0;
            int _addr;
            //char sbuf[200];//包信息缓冲区
            char checksum;//校验和
            int i = 0;
            int j = 0;
            char[] sbuf = new char[500];
            String beidou_str="$TXSQ";

            pack_len = 18 + len;
            //strcpy_s(sbuf, "$TXSQ", 5);//包ID
            beidou_str.CopyTo(0,sbuf,0,5);

            sbuf[5] = Convert.ToChar((pack_len >> 8) & 0xff);//包长度
            sbuf[6] = Convert.ToChar((pack_len)& 0xff);
            _addr = 0x032ec2;//_BD_ID_AP
            sbuf[7] = Convert.ToChar((_addr >> 16) & 0xff);//本机地址
            sbuf[8] = Convert.ToChar((_addr >> 8) & 0xff);
            sbuf[9] = Convert.ToChar(_addr & 0xff);
            sbuf[10] = Convert.ToChar(0x46);//信息类别为0x46
            _addr = 0x032ec4;
            sbuf[11] = Convert.ToChar((_addr >> 16) & 0xff);//对方地址
            sbuf[12] = Convert.ToChar((_addr >> 8) & 0xff);
            sbuf[13] = Convert.ToChar((_addr)& 0xff);

            msg_len_bits = len * 8;
            sbuf[14] = Convert.ToChar((msg_len_bits >> 8) & 0xff);//包长度
            sbuf[15] = Convert.ToChar((msg_len_bits)& 0xff);

            sbuf[16] = Convert.ToChar(0);//不需要应答
            Array.Copy(buf,0,sbuf,17,pack_len);
            //memcpy(&sbuf[17], buf, pack_len);

            checksum = Convert.ToChar(0);
            for (i = 0; i<(pack_len - 1); i++)
            {
                checksum ^= sbuf[i];
            }
            sbuf[pack_len - 1] = checksum;

            //write(fd_bd, sbuf, pack_len);
            if (comPortB.IsOpen)
            {
                comPortB.Write(sbuf, 0, pack_len);//发送数据包，从0字节开始，长度为i,即包长度
                comBsendCount += len;
            }        
        }


        /*****************************************************************************************/
        /*********下面几个是发送各个类型的数据包的相关函数*********/
        void Send_GCS2AP_CMD()
        {
            int i = 0;
            bool cmp_equal = true;
            int _size = Marshal.SizeOf(gcs2ap_cmd);
            //先比较已经发送给AP的数据和当前修改后的数据是否一致，若不一致则说明有新命令，需要发送数据包
            byte[] _cur = MyConverter.StructToByte(gcs2ap_cmd, _size);
            byte[] _new = MyConverter.StructToByte(gcs2ap_cmd_new, _size);
            for (i = 0; i < _size; i++)
            {
                if (_cur[i] != _new[i])
                {
                    cmp_equal = false;
                    break;
                    
                };
            }
            //两个包不一致，发送新数据包，并用新数据包替换老数据包
            //if (!cmp_equal) //不检查，每次都触发发送
            {
                gcs2ap_cmd = gcs2ap_cmd_new;
                generatePacket(ID_GCS2AP_CMD, _new, _size);//输出
                //generatePacket_beidou(ID_GCS2AP_CMD, _new, _size);//通过北斗 输出

            }
        }

        void Send_GCS2AP_WP()
        {
            int _size = Marshal.SizeOf(gcs2ap_wp);
            byte[] _wp = MyConverter.StructToByte(gcs2ap_wp, _size);
            generatePacket(ID_GCS2AP_WP, _wp, _size);//输出
            //generatePacket(ID_GCS2AP_WP, _wp, _size);//通过北斗 输出
        }

        void Send_GCS2AP_CTE()
        {
            int _size = Marshal.SizeOf(gcs2ap_cte);
            byte[] _cte = MyConverter.StructToByte(gcs2ap_cte, _size);
            generatePacket(ID_GCS2AP_CTE, _cte, _size);//输出
            //generatePacket(ID_GCS2AP_CTE, _cte, _size);//通过北斗 输出
        }

        void Send_GCS2AP_parameter()
        {
            int _size = Marshal.SizeOf(gcs2ap_parameter);
            byte[] _parameter = MyConverter.StructToByte(gcs2ap_parameter, _size);
            generatePacket(ID_GCS2AP_PARAMETER, _parameter, _size);//输出
            //generatePacket(ID_GCS2AP_PARAMETER, _parameter, _size);//通过北斗 输出
        }

        /*****************************************************************************************/
        /*********下面几个是gmap的相关函数*********/
        /// <summary>
        /// 在地图上添加一个航点标志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        private void addpolygonmarker(string tag, double lat, double lng, double alt, Color? color)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarkerWP m = new GMapMarkerWP(point, tag);
                m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                m.ToolTipText = "Alt: " + alt.ToString("0");
                m.Tag = tag;
                markersOverlay.Markers.Add(m);//添加标志
            }
            catch (Exception) { }
        }

        private void MMItemMPlan_Click(object sender, EventArgs e)
        {
            gbl_var.bRoutePlan = !gbl_var.bRoutePlan;
        }

        private void gMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = gMapControl.FromLocalToLatLng(e.X, e.Y);
            double current2home_lng_m;
            double current2home_lat_m;
            double current2home_m;
            double current2home_m2;
            double current2home_m3;
            string NS;
            string WE;
            //鼠标移动时，在地图左上角显示当前坐标和距离HOME的距离
            labelLocLatLng.Text = point.Lat.ToString("0.00000") + "," + point.Lng.ToString("0.00000");
            //若有航点，则在鼠标移动时显示当前鼠标位置与第一个航点之间的距离
            if (totalWPlist.Count > 0)
            {
                //纬度上的距离（即东西距离）
                tmp_point_4_calc.Lng = totalWPlist[0].Lng;
                tmp_point_4_calc.Lat = point.Lat;
                current2home_lat_m = totalWPlist[0].GetDistance(tmp_point_4_calc);
                if (point.Lat > totalWPlist[0].Lat) NS = "N";
                else NS = "S";
                //经度上的距离（即南北距离）
                tmp_point_4_calc.Lng = point.Lng;
                tmp_point_4_calc.Lat = totalWPlist[0].Lat;
                current2home_lng_m = totalWPlist[0].GetDistance(tmp_point_4_calc);
                if (point.Lng > totalWPlist[0].Lng) WE = "E";
                else WE = "W";
                //直线距离
                tmp_point_4_calc.Lat = point.Lat;
                current2home_m = totalWPlist[0].GetDistance(tmp_point_4_calc);
                current2home_m2 = totalWPlist[0].GetDistance2(tmp_point_4_calc);
                current2home_m3 = totalWPlist[0].GetBearing(tmp_point_4_calc);
                //显示
                labelDist2Home.Text = current2home_m.ToString("0") + " (" + NS + current2home_lat_m.ToString("0")
                    + "," + WE + current2home_lng_m.ToString("0") + ")";
                label3.Text = current2home_m2.ToString("0");
                label4.Text = current2home_m3.ToString("0");
            }

            //如果是在航点规划状态，则若某个航点被选(即在某个航点上点击了鼠标左键)，则可以被拖动
            if (gbl_var.bRoutePlan)
            {
                if (gbl_var.markerselected)//航点被选
                {
                    //该航点的GPS坐标随着鼠标位置而变化
                    totalWPlist[actWP_index].Lat = gMapControl.FromLocalToLatLng(e.X, e.Y).Lat;
                    totalWPlist[actWP_index].Lng = gMapControl.FromLocalToLatLng(e.X, e.Y).Lng;
                    actWP_marker.Position = gMapControl.FromLocalToLatLng(e.X, e.Y);
                }
                else
                {
                }
            }
        }

        private void gMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//点击鼠标左键
            {
                if (gbl_var.bRoutePlan)//若是航点规划状态，则可以修改航点。以后要考虑在线修改
                {
                    //如果在航点处，则标记航点被选，且给出航点在航点表中的位置
                    if (gbl_var.markerentered)
                    {
                        gbl_var.markerselected = true;
                        //先找到航点链表中的相应航点下标
                        actWP_index = getWPindex(actWP_marker.Tag.ToString());//feiqing 总报错需检查
                    }
                }
            }
            if (e.Button == MouseButtons.Right)//鼠标右键出菜单
            {
                currentPosition = gMapControl.FromLocalToLatLng(e.X, e.Y);
            }
        }

        private void gMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//点击鼠标左键
            {
                if (gbl_var.bRoutePlan)//航点规划状态时：加航点
                {
                    if (gbl_var.markerselected)//如果是移动航点，则不需要做增加航点的工作，只是重新画航线即可
                    {
                    }
                    else//如果是增加航点，则
                    {
                        //将本点追加入航点链表
                        if (totalWPlist.Count == 0)
                        {
                            totalWPlist.Add(new PointLatLngAlt(gMapControl.FromLocalToLatLng(e.X, e.Y).Lat,
                                gMapControl.FromLocalToLatLng(e.X, e.Y).Lng, default_WP_spd, "H"));
                        }
                        else
                        {
                            totalWPlist.Add(new PointLatLngAlt(gMapControl.FromLocalToLatLng(e.X, e.Y).Lat,
                                gMapControl.FromLocalToLatLng(e.X, e.Y).Lng, default_WP_spd, totalWPlist.Count.ToString()));
                        }
                        //在地图上标出本航点
                        addpolygonmarker((totalWPlist.Count - 1).ToString(), gMapControl.FromLocalToLatLng(e.X, e.Y).Lat,
                            gMapControl.FromLocalToLatLng(e.X, e.Y).Lng, default_WP_spd, null);
                        //重新画出全部航点路径。因为添加新航点后，需要删除原返航路径，改为从新航点返航，所以干脆全部重画
                        //以后也可以改为仅删除原返航路径，然后画新航点的两条路径（到达和返航）
                    }
                    ReDrawAllRoute();
                }
            }
            if (e.Button == MouseButtons.Right)//鼠标右键出菜单
            {
            }

            gbl_var.markerselected = false;
        }

        /// <summary>
        /// 在两个规划航点之间画一条直线路径
        /// </summary>
        /// <param name="point_start"></param>
        /// <param name="point_end"></param>
        private void DrawLineLinkTwoWP(PointLatLng point_start, PointLatLng point_end)
        {
            List<PointLatLng> points;
            GMapPolygon line;
            points = new List<PointLatLng>();
            points.Add(point_start);
            points.Add(point_end);
            line = new GMapPolygon(points, "");
            line.Stroke = new Pen(Color.Yellow, 4);
            markersOverlay.Polygons.Add(line);
        }
        /// <summary>
        /// 在两个实时飞行点之间画一条直线，用于画出实时航路
        /// </summary>
        /// <param name="point_start"></param>
        /// <param name="point_end"></param>
        private void DrawLineLinkTwoRealFlyPoint(PointLatLng point_start, PointLatLng point_end)
        {
            List<PointLatLng> points;
            GMapPolygon line;
            points = new List<PointLatLng>();
            points.Add(point_start);
            points.Add(point_end);
            line = new GMapPolygon(points, "");
            line.Stroke = new Pen(Color.Green, 2);
            realRouteOverlay.Polygons.Add(line);
        }
        /// <summary>
        /// 画出连接全部航点的航路（直线型）
        /// </summary>
        private void ReDrawAllRoute()
        {
            //清除多边形
            markersOverlay.Polygons.Clear();
            //然后重新画航点间直线
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (i == totalWPlist.Count - 1)
                    //画最后一个航点到第一个航点的直线
                    DrawLineLinkTwoWP(totalWPlist[totalWPlist.Count - 1], totalWPlist[0]);
                else DrawLineLinkTwoWP(totalWPlist[i], totalWPlist[i + 1]);
                //else DrawLineLinkTwoRealFlyPoint(totalWPlist[i], totalWPlist[i + 1]);
            }
        }

        /// <summary>
        /// 重画全部航点
        /// </summary>
        private void ReDrawAllWP()
        {
            //重新将各航点做标记，第一个航点为H，其它航点从1顺序编号
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (i == 0) totalWPlist[i].Tag = "H";
                else totalWPlist[i].Tag = i.ToString();
            }
            //清除地图上的全部航点标志
            markersOverlay.Markers.Clear();
            //重画全部航点
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                //addpolygonmarker(totalWPlist[i].Tag, totalWPlist[i].Lat, totalWPlist[i].Lng, totalWPlist[i].Alt, null);


                //totalWPlist[i].Lat = totalWPlist[i].Lat - 10;
                //totalWPlist[i].Lng = totalWPlist[i].Lng - 10;
                addpolygonmarker(totalWPlist[i].Tag, totalWPlist[i].Lat, totalWPlist[i].Lng, totalWPlist[i].Alt, null);
            }
        }
        /// <summary>
        /// 根据航点的tag标志找到该航点的链表位置下标
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private int getWPindex(string tag)
        {
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (totalWPlist[i].Tag == tag) return i;
                if (((totalWPlist[i].Tag == "0") || (totalWPlist[i].Tag == "H"))
                    && ((tag == "0") || (tag == "H")))
                    return 0;
            }
            return -1;
        }
        /// <summary>
        /// 插入航点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tSMenuItemInsertWP_Click(object sender, EventArgs e)
        {
            if (!gbl_var.bRoutePlan) return;
            PointLatLngAlt newWP = new PointLatLngAlt();
            //向参数设置画面传递要设置的参数id。id=2表示要设置新插入航点位置
            gbl_var.paramset_id = 2;
            //打开参数设置窗口
            Form FrmSingleParamSet = new FrmSingleParamSet();
            FrmSingleParamSet.Text = "设置插入位置";
            FrmSingleParamSet.ShowDialog();
            //插入操作
            newWP.Lat = currentPosition.Lat;
            newWP.Lng = currentPosition.Lng;
            newWP.Alt = default_WP_spd;
            newWP.Tag = (actWP_index + 1).ToString();
            totalWPlist.Insert(actWP_index + 1, newWP);//改为在之前插入
            ReDrawAllWP();//重画全部航点
            ReDrawAllRoute();//重画全部路径
            gbl_var.markerselected = false;
        }
        /// <summary>
        /// 删除指定航点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tSMenuItemDeleteWP_Click(object sender, EventArgs e)
        {
            //找到指定航点，并删除
            if (!gbl_var.bRoutePlan) return;
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (totalWPlist[i].Tag == actWP_marker.Tag.ToString())
                {
                    totalWPlist.RemoveAt(i);
                    break;
                }
            }
            ReDrawAllWP();//重画全部航点
            ReDrawAllRoute();//重画全部路径
        }

        /// <summary>
        /// 显示航路规划列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tSMenuItemGrid_Click(object sender, EventArgs e)
        {
            Form FrmFlightPlanList = new FrmFlightPlanList();
            FrmFlightPlanList.ShowDialog();
            ReDrawAllWP();
        }
        /// <summary>
        /// 清空航路规划任务数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tSMenuItemClearTask_Click(object sender, EventArgs e)
        {
            totalWPlist.Clear();
            ReDrawAllWP();
            ReDrawAllRoute();
            actWP_marker = null;
            gbl_var.markerentered = false;
            gbl_var.markerselected = false;
        }

        private void MMItemConfig_MouseHover(object sender, EventArgs e)
        {
            if (gbl_var.bRoutePlan) MMItemMPlan.Text = "退出航点规划";
            else MMItemMPlan.Text = "进入航点规划";
        }

        private void gMapControl_OnMarkerLeave(GMapMarker item)
        {
            //如果鼠标没有在某个航点的上方，则点击鼠标右键时进制删除航点和修改高度
            tSMenuItemDeleteWP.Enabled = false;
            tSMenuItemInsertWP.Enabled = true;
            //没有进入航点局域
            gbl_var.markerentered = false;
        }

        private void gMapControl_OnMarkerEnter(GMapMarker item)
        {
            tSMenuItemDeleteWP.Enabled = true;
            tSMenuItemInsertWP.Enabled = false;
            //进入航点区域
            gbl_var.markerentered = true;
            //记录进入的航点，作为当前活动航点
            actWP_marker = item;
        }

        private void MMItemMPortCfg_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void 高德地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                                
            gMapControl.MapProvider = GMap.NET.MapProviders.AMapProvider.Instance;
            this.gMapControl.Manager.Mode = AccessMode.ServerAndCache;          
            this.gMapControl.ReloadMap(); 

            statusMapProvider.Text = "高德地图";
        }

        private void 高德卫星地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gMapControl.MapProvider = GMap.NET.MapProviders.AMapSateliteProvider.Instance;
            this.gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            //this.gMapControl.Manager.Mode = AccessMode.ServerOnly;
            this.gMapControl.ReloadMap();

            statusMapProvider.Text = "高德卫星地图";

        }

        /// <summary>
        /// 将当前GPS坐标作为回家的坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetHome_Click(object sender, EventArgs e)
        {
            if ((ap2gcs_real.lat > 20.0) && (ap2gcs_real.lng > 100.0))
            {
                homePos.Lat = Convert.ToDouble(ap2gcs_real.lat) * 0.00001;
                homePos.Lng = Convert.ToDouble(ap2gcs_real.lng) * 0.00001;
            }
            else
            {
                homePos.Lat = HUAINAN_LAT;
                homePos.Lng = HUAINAN_LNG;
                ap2gcs_real.lat = Convert.ToUInt32(homePos.Lat * 1000000.0);
                ap2gcs_real.lng = Convert.ToUInt32(homePos.Lng * 1000000.0);
            }
            gMapControl.Position = homePos;
            gMapControl.Zoom = 16;
            totalWPlist[0].Lng = homePos.Lng;
            totalWPlist[0].Lat = homePos.Lat;
            ReDrawAllWP();//重画全部航点
            ReDrawAllRoute();//重画全部路径
        }

        private void FrmGCSMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /*****************************************************************************************/
        /*********下面几个刷新打开通信串口的相关函数*********/
        private void btnPortARefresh_Click(object sender, EventArgs e)
        {
            if (comPortA.IsOpen)
            {
            }
            else//串口没有打开的情况下允许重新搜索串口
            {
                cbxACOMPort.Items.Clear();
                ports = SerialPort.GetPortNames();//获取可用串口
                if (ports.Length > 0)//有串口可用
                {
                    for (int i = 0; i < ports.Length; i++)
                    {
                        cbxACOMPort.Items.Add(ports[i]);//下拉控件里添加可用串口
                    }
                    cbxACOMPort.SelectedIndex = 0;
                }
                else//未检测到串口
                {
                    cbxACOMPort.Text = "None";
                }
            }
        }

        private void btnPortAOpenClose_Click(object sender, EventArgs e)
        {
            if (comPortA.IsOpen)
            {
                comAClosing = true;
                while (comAListening) Application.DoEvents();//打开时点击，则关闭串口
                comPortA.Close();
                cbxACOMPort.Enabled = true;
                cbxABaudRate.Enabled = true;
                btnPortAOpenClose.Text = "打开";
            }
            else
            {
                if ((comPortA.PortName != "None") && (comPortA.PortName != null))
                {
                    comPortA.PortName = cbxACOMPort.Text.Trim();//设置串口名
                    comPortA.BaudRate = Convert.ToInt32(cbxABaudRate.Text.Trim());//设置串口的波特率
                    comPortA.DataBits = 8;//设置数据位
                    comPortA.StopBits = StopBits.One;
                    comPortA.Parity = Parity.None;
                    comPortA.ReadTimeout = 1000;//[ms]
                    comPortA.WriteTimeout = 1000;//[ms]
                    comPortA.ReadBufferSize = 1024;
                    comPortA.WriteBufferSize = 1024;

                    comPortA.Open();//打开串口
                }
                else
                {
                    MessageBox.Show("请检查串口配置", "提示");
                }
                if (comPortA.IsOpen)
                {
                    cbxACOMPort.Enabled = false;
                    cbxABaudRate.Enabled = false;
                    comAClosing = false;//等待关闭串口状态改为false 
                    btnPortAOpenClose.Text = "关闭";
                }
            }
        }

        private void trBarTurnMotor_Scroll(object sender, EventArgs e)
        {
            teBoxTurnMotor.Text = Convert.ToString(trBarTurnMotor.Value);
        }

        /*****************************************************************************************/
        /*********下面几个实用的相关函数*********/
        public static byte[] ConvertDoubleToByteArray(double d)
        {
            return BitConverter.GetBytes(d);
        }

        public static double ConvertByteArrayToDouble(byte[] b)
        {
            return BitConverter.ToDouble(b,0);
        }

        private void btnSimuSail_Click(object sender, EventArgs e)
        {
            if(gbl_var.bSimuState)//进入模拟飞行状态，用于程序调试和测试
            {
                gbl_var.bSimuSailing = true;
            }
        }

        /// <summary>
        /// 飞机当前的位置
        /// </summary>
        PointLatLngAlt realpos_now = new PointLatLngAlt();
        /// <summary>
        /// 上一次显示时的飞行位置
        /// </summary>
        PointLatLngAlt realpos_prev = new PointLatLngAlt();
        /// <summary>
        /// 本时段实时飞行路径起点
        /// </summary>
        PointLatLng realroute_start;
        /// <summary>
        /// 本时段实时飞行路径终点
        /// </summary>
        PointLatLng realroute_end;
        /// <summary>
        /// 当前实时飞行目标航点
        /// </summary>
        int real_tar_WP = 0;//默认当前目标航点
        GMapMarker realVehicle;

        /// <summary>
        /// 飞行器在地图上的实时显示
        /// </summary>        
        private void SailDisplay()
        {
            float target_angle;
            float cog_angle;
            float heading_angle;
            float nav_angle;
            if (totalWPlist.Count < 1) return;
            if (gbl_var.bSimuState)//模拟飞行显示
            {
                if (totalWPlist.Count == 1)//仅有一个设定航点，则飞机位于航点处
                {
                    realpos_now.Lat = totalWPlist[0].Lat;
                    realpos_now.Lng = totalWPlist[0].Lng;
                    realpos_now.Alt = totalWPlist[0].Alt;
                    realroute_start = realpos_now;
                    real_tar_WP = 0;
                }
                else if (!gbl_var.bSimuSailing)//非模拟飞行状态时，将飞机的当前位置设置为在航点0，下一个航点为1
                {
                    realpos_now.Lat = totalWPlist[0].Lat;
                    realpos_now.Lng = totalWPlist[0].Lng;
                    realpos_now.Alt = totalWPlist[0].Alt;
                    realroute_start = realpos_now;
                    real_tar_WP = 1;
                }
                else //航行状态
                {
                    //查找目标航点
                    for (int i = real_tar_WP; i < totalWPlist.Count; i++)
                    {
                        if (totalWPlist[i].GetDistance(realpos_now) < 10.0)//[m]如果离某个航点非常近，则目标是下一个航点
                        {
                            if (i == (totalWPlist.Count - 1)) real_tar_WP = 0;
                            else real_tar_WP = i + 1;
                            break;
                        }
                    }
                    double tmp_angle = realpos_now.GetBearing(totalWPlist[real_tar_WP]);
                    double tmp_delta_lat = 0.0001 * Math.Abs(Math.Cos(tmp_angle * Math.PI / 180));//每次移动0.0001(默认飞行速度约=10.0m/s)
                    double tmp_delta_lng = 0.0001 * Math.Abs(Math.Sin(tmp_angle * Math.PI / 180));

                    if (realpos_now.Lat > (totalWPlist[real_tar_WP].Lat + tmp_delta_lat)) realpos_now.Lat -= tmp_delta_lat;
                    else if (realpos_now.Lat < (totalWPlist[real_tar_WP].Lat - tmp_delta_lat)) realpos_now.Lat += tmp_delta_lat;
                    if (realpos_now.Lng > (totalWPlist[real_tar_WP].Lng + tmp_delta_lng)) realpos_now.Lng -= tmp_delta_lng;
                    else if (realpos_now.Lng < (totalWPlist[real_tar_WP].Lng - tmp_delta_lng)) realpos_now.Lng += tmp_delta_lng;
                    if (realpos_now.Alt > (totalWPlist[real_tar_WP].Alt + 1.0)) realpos_now.Alt -= 1.0;//[m/s]默认爬升率=1m/s
                    else if (realpos_now.Alt < (totalWPlist[real_tar_WP].Alt - 1.0)) realpos_now.Alt += 1.0;//[m/s]爬升率 
                }
                //方向
                target_angle = (float)realpos_now.GetBearing(totalWPlist[real_tar_WP]);//计算目标点方向
                cog_angle = (float)realpos_prev.GetBearing(realpos_now);//计算地速方向
                heading_angle = target_angle;//机头朝向，应由自驾仪传感器给出
                nav_angle = target_angle;//导航方向，应由自驾仪计算给出
            }
            else//真实飞行状态
            {
                if ((ap2gcs_real.lat > 20.0) && (ap2gcs_real.lng > 100.0))
                {
                    realpos_now.Lat = Convert.ToDouble(ap2gcs_real.lat) * 0.00001;
                    realpos_now.Lng = Convert.ToDouble(ap2gcs_real.lng) * 0.00001;
                }
                else
                {
                    realpos_now.Lat = homePos.Lat;
                    realpos_now.Lng = homePos.Lng;
                }
                //方向
                cog_angle = (float)(ap2gcs_real.dir_gps * 0.01);//地速方向
                heading_angle = (float)(ap2gcs_real.dir_heading * 0.01);//船头朝向
                target_angle = (float)(ap2gcs_real.dir_target * 0.01);//目标航点朝向
                nav_angle = (float)(ap2gcs_real.dir_nav * 0.01);//导航航向
            }
            
            //画出飞行动画
            realVehicleOverlay.Clear();//先清除上次画的动画

            realVehicle = new GMapMarkerBoat(realpos_now, heading_angle, cog_angle, nav_angle, target_angle);//画动画
            realVehicleOverlay.Markers.Add(realVehicle);
            //画出实时飞行航路
            realroute_end = realpos_now;
            DrawLineLinkTwoRealFlyPoint(realroute_start, realroute_end);//画实时飞行航路
            realroute_start = realroute_end;//记录当前航点
            realpos_prev.Lat = realroute_end.Lat;//用于计算地速方向
            realpos_prev.Lng = realroute_end.Lng;
            
        }

        private void FrmGCSMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            file_real_w.Close();
            file_real.Close();
        }

        private void ckBoxSimu_Click(object sender, EventArgs e)
        {
            if (ckBoxSimu.Checked) gbl_var.bSimuState = true;
            else gbl_var.bSimuState = false;
        }

        /*****************************************************************************************/
        /***************************************************************************************/
        /************************************************************************************/
        /**********************************************************************************/
        /********************************************************************************/
        /******************************************************************************/
        /*********这可能是最重要的定时器 定时触发函数了 王博wangbo*********/
        private void main_timer_Tick(object sender, EventArgs e)
        {
            //手柄输出
            byte _pitch, _roll, _yaw, _throttle;
            _pitch = 0;
            _roll = 0;
            _yaw = 0;
            _throttle = 0;
            if (gbl_var.bJoystickInstalled)
            {
                joystick.UpdateStatus();
                /*
                _pitch = Convert.ToByte(((65535 - joystick.AxisD) >> 8) & 0xff);
                _roll = Convert.ToByte(((65535 - joystick.AxisC) >> 8) & 0xff);
                _yaw = Convert.ToByte(((65535 - joystick.AxisA) >> 8) & 0xff);
                _throttle = Convert.ToByte(((65535 - joystick.AxisE) >> 8) & 0xff);
                */
                _pitch = Convert.ToByte(((65535 - joystick.AxisE) >> 8) & 0xff);
                _roll = Convert.ToByte(((65535 - joystick.AxisA) >> 8) & 0xff);
                _yaw = Convert.ToByte(((65535 - joystick.AxisD) >> 8) & 0xff);
                _throttle = Convert.ToByte(((65535 - joystick.AxisC) >> 8) & 0xff);
            }

            //系统配置栏中操作杆位置显示
            teBox_jsk_pitch.Text = Convert.ToString(_pitch);
            teBox_jsk_throttle.Text = Convert.ToString(_throttle);
            teBox_jsk_yaw.Text = Convert.ToString(_yaw);
            teBox_jsk_roll.Text = Convert.ToString(_roll);
            prBar_jsk_yaw.Value = _yaw;
            prBar_jsk_roll.Value = _roll;
            prBar_jsk_pitch.Value = _pitch;
            prBar_jsk_throttle.Value = _throttle;

            //在主控数据栏的舵机标定位置显示从AP接收到的舵机标定位置
            teBox_jsk_on.Text = Convert.ToString(gbl_var.motor_on_pwm);
            teBox_jsk_off.Text = Convert.ToString(gbl_var.motor_off_pwm);
            teBox_jsk_fwd.Text = Convert.ToString(gbl_var.motor_fwd_pwm);
            teBox_jsk_bwd.Text = Convert.ToString(gbl_var.motor_bwd_pwm);
            teBox_jsk_rudllmt.Text = Convert.ToString(gbl_var.rud_left_pwm);
            teBox_jsk_rudrlmt.Text = Convert.ToString(gbl_var.rud_right_pwm);
            teBox_jsk_rudmid.Text = Convert.ToString(gbl_var.rud_mid_pwm);

            //在主控数据栏的舵机标定位置显示摇杆的数据输入
            prBar_jsk_onoff.Value = _throttle;//用滚转来标定启停舵机
            teBox_jsk_onoff.Text = Convert.ToString(prBar_jsk_onoff.Value);
            prBar_jsk_bwdfwd.Value = _throttle;//用俯仰来标定前进后退舵机
            teBox_jsk_bwdfwd.Text = Convert.ToString(prBar_jsk_bwdfwd.Value);
            prBar_jsk_rudlmt.Value = _yaw;//用偏航来标定方向舵舵机
            teBox_jsk_rudlmt.Text = Convert.ToString(prBar_jsk_rudlmt.Value);

            //在主控数据栏的手动遥控位置显示摇杆的数据输入
            //wangbo20160903 这里把油门往下拉时，定义为低油门
            prBar_jsk_throttle2.Value = _throttle;
            teBox_jsk_throttle2.Text = Convert.ToString(_throttle);
            //prBar_jsk_throttle2.Value = 255-_throttle;
            //teBox_jsk_throttle2.Text = Convert.ToString(prBar_jsk_throttle2.Value);



            prBar_jsk_yaw2.Value = _yaw;
            teBox_jsk_yaw2.Text = Convert.ToString(_yaw);
            //prBar_jsk_yaw2.Value = _throttle;
            //teBox_jsk_yaw2.Text = Convert.ToString(_throttle);

            //AP反馈的舵机当前位置值
            teBox_fb_oo_pwm.Text = Convert.ToString(ap2gcs_real.moo_pwm);
            teBox_fb_bf_pwm.Text = Convert.ToString(ap2gcs_real.mbf_pwm);
            teBox_fb_yaw_pwm.Text = Convert.ToString(ap2gcs_real.rud_pwm);

            //摇杆给出的方向舵位置增益值
            teBoxTurnMotor.Text = Convert.ToString(trBarTurnMotor.Value);
            textBoxSteerIntegral.Text = Convert.ToString(trBarTurnMotorIntegral.Value);
            textBoxSteerDeviate.Text = Convert.ToString(trBarTurnMotorDeviate.Value);
            //AP反馈的方向舵位置增益值
            teBoxTurnMotorFb.Text = Convert.ToString(ap2gcs_real.rud_p);
/*
            //根据油门舵机的位置判断主电机的工作状态，并在手动遥控栏中显示
            if ((_throttle >= 0) && (_throttle < 50))
            {
                gbl_var.mmotor_state = 0x2;
                labelMotorState.Text = "反向"; 
            }
            else if ((_throttle > 180) && (_throttle <= 255))
            {
                gbl_var.mmotor_state = 0x1;
                labelMotorState.Text = "正向"; 
            }
            else
            {
                gbl_var.mmotor_state = 0x0;
                labelMotorState.Text = "停车"; 
            }
*/
            statusPortARecvCnt.Text = comArecvCount.ToString();//接收数据字节数
            statusPortASendCnt.Text = comAsendCount.ToString();//发送数据字节数
            //statusPortBRecvCnt.Text = 0;
            //statusPortASendCnt.Text = 0;
            status1.Text = error1.ToString();
            status2.Text = error2.ToString();
            status3.Text = error3.ToString();
            status4.Text = error4.ToString();
            status5.Text = error5.ToString();
            status6.Text = error6.ToString();
            status7.Text = error7.ToString();
            status8.Text = error8.ToString();


            //在手动遥控情况下，当遥控状态或数值变化时，通知发送命令包
            if (gbl_var.run_state == 0x3)//测试状态
            {
                if (checkBoxOnoffAct.Checked)
                {
                    gcs2ap_cmd_new.moo_pwm = Convert.ToByte(prBar_jsk_onoff.Value);
                    gbl_var.send_req_cnt++;
                    gbl_var.send_cmd_req = true;
                }
                if (checkBoxBwdfwdAct.Checked)
                {
                    gcs2ap_cmd_new.mbf_pwm = Convert.ToByte(prBar_jsk_bwdfwd.Value);
                    gbl_var.send_req_cnt++;
                    gbl_var.send_cmd_req = true;
                }
                if (checkBoxRudLmtAct.Checked)
                {
                    gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_rudlmt.Value);
                    gbl_var.send_req_cnt++;
                    gbl_var.send_cmd_req = true;
                }
            }
            else if (gbl_var.run_state == 0x6)//人工遥控状态
            {
                if ((gcs2ap_cmd_new.rud_pwm != Convert.ToByte(prBar_jsk_yaw2.Value))
                    || (gbl_var.mmotor_pwm_out != Convert.ToByte(prBar_jsk_throttle2.Value)))
                {
                    //gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_yaw2.Value);
                    gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_yaw2.Value);
                    gcs2ap_cmd_new.moo_pwm = Convert.ToByte(prBar_jsk_throttle2.Value);
                    gcs2ap_cmd_new.mbf_pwm = Convert.ToByte(prBar_jsk_throttle2.Value);
                    gcs2ap_cmd_new.cmd_manu = gbl_var.mmotor_state;
                    //gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                    gbl_var.send_req_cnt++;
                    gbl_var.send_cmd_req = true;
                }

                
            }
            else if (gbl_var.run_state == 0x5)//自动状态
            {
                gcs2ap_cmd_new.rud_p = Convert.ToByte(trBarTurnMotor.Value);
                //wangbo
                gcs2ap_cmd_new.rud_i = Convert.ToByte(trBarTurnMotorIntegral.Value);
                gcs2ap_cmd_new.rud_d = Convert.ToByte(trBarTurnMotorDeviate.Value);


                gcs2ap_cmd_new.cte_p = Convert.ToByte(trackBar_CTE_P.Value);

                /*发送特定指定目标航点*/
                if(gbl_var.send_specify_wp)
                {
                    /*这个是设置航点命令，把cmd数据包wpno的最高位置1，表明要设置目标航点*/
                    gbl_var.send_specify_wp = false;
                    gcs2ap_cmd_new.wpno = Convert.ToByte(target_wp_num.Text);
                    gcs2ap_cmd_new.wpno = Convert.ToByte(gcs2ap_cmd_new.wpno | 0x80);
                }
                else
                {
                    gcs2ap_cmd_new.wpno = Convert.ToByte(gcs2ap_cmd_new.wpno & 0x00);//没有发送目标航点指令，最高位就置0
                }

                /*发送航点的到达半径*/
                /*
                if(gbl_var.send_arrive_radius)
                {
                    gbl_var.send_arrive_radius = false;
                    //gcs2ap_cmd_new.arrive_ardius = Convert.ToByte(arrive_radius_value.Text);
                }
                */
#if false
                /*发送偏航距 CTE数据包
                 *因为只需要在自动驾驶的时候才发送变化，所以放在自动状态这个判断if语句下
                 *又因为cte的比例积分和微分值，与方向舵的pid数值都是用滑杆设置，没有按钮，
                 *所以放在了这里而不是跟设置参数的数据包发送放在一起*/
                gcs2ap_cte.cte_p = Convert.ToByte(trackBar_CTE_P.Value);
                if (gbl_var.send_max_head_error_angle)
                {
                    gbl_var.send_max_head_error_angle = false;
                    gcs2ap_cte.cte_max_correct_angle = Convert.ToByte(textBoxCTE_max.Text);
                }
                gbl_var.send_cte_req = true;
#endif        
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
                
                
            }

            /*无论测试、手动遥控、自动驾驶都可以发送的数据包在这里添加*/
            //wangbo
#if false
            gcs2ap_cmd_new.reverse = 0x00;
            if(gbl_var.steer_reverse)
            {
                gbl_var.steer_reverse = false;
                gcs2ap_cmd_new.reverse = 0x01;
            }
            if(gbl_var.throttle_reverse)
            {
                gbl_var.throttle_reverse = false;
                gcs2ap_cmd_new.reverse = 0x02;
            }
#endif


            //在自动驾驶状态下，当AP控制参数发生变化时，通知发送命令包
            //发送数据包的准备和处理
            if (gbl_var.send_req_cnt != gbl_var.send_req_cnt_lst)//当有发送请求,即发送请求累加器变化时，处理发送工作
            {
                //若需要发送命令
                if (gbl_var.send_cmd_req)
                {
                    /*
                    //根据工作状态来确定要发送的数据
                    switch (gbl_var.run_state)
                    {
                        case 0x3://测试状态
                            if (checkBoxOnoffAct.Checked)
                            {
                                gcs2ap_cmd_new.moo_pwm = Convert.ToByte(prBar_jsk_onoff.Value);
                            }
                            if (checkBoxBwdfwdAct.Checked)
                            {
                                gcs2ap_cmd_new.mbf_pwm = Convert.ToByte(prBar_jsk_bwdfwd.Value);
                            }
                            if (checkBoxRudLmtAct.Checked)
                            {
                                gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_rudlmt.Value);
                            }
                            break;
                        case 0x6://人工状态
                            gcs2ap_cmd_new.cmd_manu = gbl_var.mmotor_state;
                            gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_yaw2.Value);
                            break;
                        case 0x5://自动状态
                            gcs2ap_cmd_new.rud_p = Convert.ToByte(trBarTurnMotor.Value);
                            gcs2ap_cmd_new.rud_i = 0;
                            break;
                        case 0xa0://紧急停车
                            //不做任何处理，只是把紧急停车要求发给AP，由AP来完成相关工作
                            break;
                    }
                    */
                    Send_GCS2AP_CMD();
                    gbl_var.send_cmd_req = false;
                }
                
                if (gbl_var.send_wp_req)
                {
                    Send_GCS2AP_WP();
                    gbl_var.send_wp_req = false;
                }

                //发送偏航距数据包
                if(gbl_var.send_cte_req)
                {
                    Send_GCS2AP_CTE();
                    gbl_var.send_cte_req = false;
                }

                //发送参数设置包
                if (gbl_var.send_parameter_set)
                {   
                    Send_GCS2AP_parameter();
                    gbl_var.send_parameter_set = false;

                    //为什么放在这里就能显示出总航点数呢？
                    //this.textBox_all_wp_num.Text=ap2gcs_wp_back.total.ToString();
                }
            
            }
            //wangbo 按理说这个应该放在收到第0个航点后就应该显示总航点数，但是实际上却出现了问题，就暂时放在这里先
            //接收线程中收到的数据，不能直接在text文本框中显示
            this.textBox_all_wp_num.Text = gbl_var.all_wp_num.ToString();

            //wangbo下面处理显示接收线程收到的参数数据
            if (gbl_var.parameter_get_cruise_throttle)
            {
                textBox_get_throttle.Text = Convert.ToString(ap2gcs_parameter.value);
                gbl_var.parameter_get_cruise_throttle = false;
            }
            if (gbl_var.parameter_get_arrive_radius)
            {
                textBox_get_arrive_radius.Text = Convert.ToString(ap2gcs_parameter.value);
                gbl_var.parameter_get_arrive_radius = false;
            }
            if(gbl_var.parameter_get_max_head_error_angle)
            {
                textBox_get_cte_max.Text = Convert.ToString(ap2gcs_parameter.value);
                gbl_var.parameter_get_max_head_error_angle = false;
            }
            if(gbl_var.parameter_get_rotary_position)
            {
                textBox_get_rotary.Text = Convert.ToString(ap2gcs_parameter.value*2-180);
                textBox_rotary_value.Text = Convert.ToString(ap2gcs_parameter.value * 2);
                gbl_var.parameter_get_rotary_position = false;
            }

            

            
            /*记录请求发送的计数*/
            gbl_var.send_req_cnt_lst = gbl_var.send_req_cnt;

            //时间间隔为TIMER_DISP
            //包括：飞行路径及飞行动画显示、基本状态(如：心跳，通信状态，地面站状态)显示
            if (gbl_var.main_timer_interval < 50) gbl_var.main_timer_interval = 50;
            if (gbl_var.main_timer_interval >2000) gbl_var.main_timer_interval = 2000;

            //当收到新实时数据包时，更新显示和保存
            if (gbl_var.ap2gcs_real_cnt != gbl_var.ap2gcs_real_cnt_lst)
            {
                //左侧文本框的数据显示
                teBox_ap_lng.Text = Convert.ToString(ap2gcs_real.lng * 0.00001);
                teBox_ap_lat.Text = Convert.ToString(ap2gcs_real.lat * 0.00001);
                teBox_ap_pitch.Text = Convert.ToString(ap2gcs_real.pitch * 0.01);
                teBox_ap_roll.Text = Convert.ToString(ap2gcs_real.roll * 0.01);
                teBox_ap_yaw.Text = Convert.ToString(ap2gcs_real.yaw * 0.01);
                teBox_ap_dir_gps.Text = Convert.ToString(ap2gcs_real.dir_gps * 0.01);
                teBox_ap_dir_heading.Text = Convert.ToString(ap2gcs_real.dir_heading * 0.01);
                teBox_ap_dir_target.Text = Convert.ToString(ap2gcs_real.dir_target * 0.01);
                teBox_ap_dir_nav.Text = Convert.ToString(ap2gcs_real.dir_nav * 0.01);
                teBox_ap_spd.Text = Convert.ToString(ap2gcs_real.spd * 0.01);
                teBox_ap_nextwp.Text = Convert.ToString(ap2gcs_real.wpno);
                teBox_ap_moo.Text = Convert.ToString(ap2gcs_real.moo_pwm);
                teBox_ap_mbf.Text = Convert.ToString(ap2gcs_real.mbf_pwm);
                teBox_ap_rud.Text = Convert.ToString(ap2gcs_real.rud_pwm);
                teBox_ap_rud_p.Text = Convert.ToString(ap2gcs_real.rud_p);
                teBox_ap_rud_i.Text = Convert.ToString(ap2gcs_real.rud_i);
                teBox_inboat_t1.Text = Convert.ToString(ap2gcs_real.boat_temp1);
                teBox_inboat_t2.Text = Convert.ToString(ap2gcs_real.boat_temp2);
                teBox_wp_load_cnt.Text = Convert.ToString(ap2gcs_real.wp_load_cnt);

                textBox_toggle_voltage0.Text = Convert.ToString(ap2gcs_real.voltage_bat1);
                textBox_toggle_voltage1.Text = Convert.ToString(ap2gcs_real.voltage_bat2);

                if (ap2gcs_real.generator_onoff_req==0)
                {
                    text_generator.Text = "关闭中";
                }
                else{
                    text_generator.Text = "发电中";
                }

                teBox_ws_winddir.Text = Convert.ToString(ap2gcs_real.winddir);
                teBox_ws_windspd.Text = Convert.ToString(ap2gcs_real.windspeed);
                textBox_rocket_alt.Text = Convert.ToString(ap2gcs_real.alt);

                /*气象站*/
                teBox_ws_temp.Text = Convert.ToString(ap2gcs_real.temp);
                teBox_ws_airpress.Text = Convert.ToString(ap2gcs_real.airpress);
                teBox_ws_seatemp.Text = Convert.ToString(ap2gcs_real.seatemp1);
                teBox_ws_humi.Text = Convert.ToString(ap2gcs_real.humi);

                textBox_seasault.Text = Convert.ToString(ap2gcs_real.seasault);






                //右侧仪表和地图上文本数据显示
                panelInstruments1.SetValue((float)(ap2gcs_real.spd * 0.01), 0, (float)(ap2gcs_real.pitch * 0.01),
                    (float)(ap2gcs_real.roll * 0.01));
                labelRealLat.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lat) * 0.00001);
                labelRealLng.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lng) * 0.00001);
                /*
                teBox_ap_lng.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lng) * 0.00001);
                teBox_ap_lat.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lat) * 0.00001);
                teBox_ap_pitch.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.pitch) * 0.01);
                teBox_ap_roll.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.roll) * 0.01);
                teBox_ap_yaw.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.yaw) * 0.01);
                 */

                //wangbo 这里以前因为某种原因把实时数据的保存注释掉了，但是现在又打开，因为我们需要保存实时数据
                //保存实时数据
                file_real_str = Convert.ToString(ap2gcs_real.lng * 0.00001) + "," + Convert.ToString(ap2gcs_real.lat * 0.00001)
                                          +"," + Convert.ToString(ap2gcs_real.spd * 0.01)
                                          + "," + Convert.ToString(ap2gcs_real.dir_gps * 0.01) + "," + Convert.ToString(ap2gcs_real.dir_heading * 0.01)
                                          + "," + Convert.ToString(ap2gcs_real.dir_target * 0.01) + "," + Convert.ToString(ap2gcs_real.dir_nav * 0.01)
                                          + "," + Convert.ToString(ap2gcs_real.pitch * 0.01) + "," + Convert.ToString(ap2gcs_real.roll * 0.01)
                                          + "," + Convert.ToString(ap2gcs_real.yaw * 0.01)
                                          + "," + Convert.ToString(ap2gcs_real.moo_pwm) + "," + Convert.ToString(ap2gcs_real.mbf_pwm)
                                          + "," + Convert.ToString(ap2gcs_real.rud_pwm)
                                          /*这里空了个mm_state*/
                                          ///发现问题，下面这一句就会报错,索引超出了范围，也就是说totalWPlist这个数组可能没有wpno这个数值，而且在地面站启动的时候totalWPlist肯定是还没有任何航点呢，所以放在这里不合适
                                          //+ "," + Convert.ToString(totalWPlist[ap2gcs_real.wpno].Lng) + "," + Convert.ToString(totalWPlist[ap2gcs_real.wpno].Lat)
                                          + "," + Convert.ToString(ap2gcs_real.rud_p) + "," + Convert.ToString(ap2gcs_real.rud_i)
                                          /*这里空了个spare1*/
                                          + "," + Convert.ToString(ap2gcs_real.boat_temp1) + "," + Convert.ToString(ap2gcs_real.boat_temp2)
                                          + "," + Convert.ToString(ap2gcs_real.wp_load_cnt)
                                          + "," + Convert.ToString(ap2gcs_real.wpno)
                                          /*下面的不是实时数据包中的数据*/
                                          + "," + Convert.ToString(prBar_jsk_onoff.Value) + "," + Convert.ToString(prBar_jsk_bwdfwd.Value)
                                          + "," + Convert.ToString(prBar_jsk_rudlmt.Value)
                                          + "," + Convert.ToString(gcs2ap_cmd.cmd_state) + "," + Convert.ToString(gcs2ap_cmd.cmd_test)
                                          ;
                file_real_w.WriteLine(file_real_str);
            }

            //当收到新气象站数据包时，更新显示和保存
            if (gbl_var.ap2gcs_aws_cnt != gbl_var.ap2gcs_aws_cnt_lst)
            {
                teBox_ws_lng.Text = Convert.ToString(ap2gcs_aws.lng * 0.00001);
                teBox_ws_lat.Text = Convert.ToString(ap2gcs_aws.lat * 0.00001);
                teBox_ws_temp.Text = Convert.ToString(ap2gcs_aws.temp * 0.01);
                teBox_ws_seatemp.Text = Convert.ToString(ap2gcs_aws.dewtemp * 0.01);
                teBox_ws_humi.Text = Convert.ToString(ap2gcs_aws.humi * 0.01);
                teBox_ws_airpress.Text = Convert.ToString(ap2gcs_aws.airpress * 0.01);
                teBox_ws_winddir.Text = Convert.ToString(ap2gcs_aws.winddir * 0.01);
                teBox_ws_windspd.Text = Convert.ToString(ap2gcs_aws.windspd * 0.01);            
            }
            //不需要快速显示的数据，延长显示周期
            if ((timerCnt % Convert.ToInt32(TIMER_DISP / gbl_var.main_timer_interval)) == 0)
            {
                if (comPortA.IsOpen)
                {
                    statusPortAName.Text = "串口A: " + comPortA.PortName;
                }
                else
                {
                    statusPortAName.Text = "串口A: ";
                }

                if (gbl_var.ap2gcs_real_cnt != gbl_var.ap2gcs_real_cnt_lst)
                {
                    _alarmSound.Play();//放在此处仅用于测试。应放在通信处，当收到数据包时给出提示音
                    statusHeartBeatCnt.Text = "心跳: " + gbl_var.ap2gcs_real_cnt.ToString();
                }
                statusStateDisp.Text = "航路规划:" + gbl_var.bRoutePlan.ToString() + " 模拟飞行:" + gbl_var.bSimuState.ToString();
                //显示航行动画
                SailDisplay();
            }

            labelRealLat.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lat) * 0.00001);
            labelRealLng.Text = Convert.ToString(Convert.ToDouble(ap2gcs_real.lng) * 0.00001);

            //更新状态确认，以保证下一次的有效更新
            gbl_var.ap2gcs_real_cnt_lst = gbl_var.ap2gcs_real_cnt;
            gbl_var.ap2gcs_aws_cnt_lst = gbl_var.ap2gcs_aws_cnt;

            timerCnt++;
            if (timerCnt > 999999) timerCnt = 0;
        }
        /// <summary>
        /// 测试模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestMode_Click(object sender, EventArgs e)
        {
            gbl_var.run_state = 0x3;//测试状态
            panelTest.BackColor = tabPage3.BackColor;
            panelManu.BackColor = panel1.BackColor;
            panelAuto.BackColor = panel1.BackColor;
            panelStop.BackColor = panel1.BackColor;
            checkBoxOnoffAct.Checked = false;
            checkBoxBwdfwdAct.Checked = false;
            checkBoxRudLmtAct.Checked = false;
            gcs2ap_cmd_new.cmd_state = 0x3;
            gbl_var.send_req_cnt++;
            gbl_var.send_cmd_req = true;
        }
        /// <summary>
        /// 手动遥控模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManuMode_Click(object sender, EventArgs e)
        {
            gbl_var.run_state = 0x6;//手操状态
            panelTest.BackColor = panel1.BackColor;
            panelManu.BackColor = tabPage3.BackColor;
            panelAuto.BackColor = panel1.BackColor;
            panelStop.BackColor = panel1.BackColor;
            gcs2ap_cmd_new.cmd_state = 0x6;
            gbl_var.send_req_cnt++;
            gbl_var.send_cmd_req = true;
        }

        /// <summary>
        /// 自动驾驶模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoMode_Click(object sender, EventArgs e)
        {
            gbl_var.run_state = 0x5;//自驾状态
            panelTest.BackColor = panel1.BackColor;
            panelManu.BackColor = panel1.BackColor;
            panelAuto.BackColor = tabPage3.BackColor;
            panelStop.BackColor = panel1.BackColor;
            gcs2ap_cmd_new.cmd_state = 0x5;
            gbl_var.send_req_cnt++;
            gbl_var.send_cmd_req = true;
        }

        /// <summary>
        /// 紧急停车模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            gbl_var.run_state = 0xa0;//停车状态
            panelTest.BackColor = panel1.BackColor;
            panelManu.BackColor = panel1.BackColor;
            panelAuto.BackColor = panel1.BackColor;
            panelStop.BackColor = tabPage3.BackColor;
            gcs2ap_cmd_new.cmd_state = 0xa0;
            gbl_var.send_req_cnt++;
            gbl_var.send_cmd_req = true;
        }

        /// <summary>
        /// 主电机启动PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMMStart_Click(object sender, EventArgs e)
        {
            if (checkBoxOnoffAct.Checked)
            {
                gcs2ap_cmd_new.moo_pwm = Convert.ToByte(prBar_jsk_onoff.Value);
                gcs2ap_cmd_new.cmd_test = 0x9;//启动位:test = 00 001 001
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }
        }
        /// <summary>
        /// 主电机停止PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMMStop_Click(object sender, EventArgs e)
        {
            if (checkBoxOnoffAct.Checked)
            {
                gcs2ap_cmd_new.moo_pwm = Convert.ToByte(prBar_jsk_onoff.Value);
                gcs2ap_cmd_new.cmd_test = 0x11;//停止位:test = 00 010 001
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }
        }
        /// <summary>
        /// 主电机前进PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMMFwd_Click(object sender, EventArgs e)
        {
            if (checkBoxBwdfwdAct.Checked)
            {
                gcs2ap_cmd_new.mbf_pwm = Convert.ToByte(prBar_jsk_bwdfwd.Value);
                //gcs2ap_cmd_new.cmd_test = 0x1c;//前进位:test = 00 011 010
                //wangbo
                gcs2ap_cmd_new.cmd_test = 0x1a;//前进位:test = 00 011 010
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;


                
            }
        }
        /// <summary>
        /// 主电机后退PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMMBwd_Click(object sender, EventArgs e)
        {
            if (checkBoxBwdfwdAct.Checked)
            {
                gcs2ap_cmd_new.mbf_pwm = Convert.ToByte(prBar_jsk_bwdfwd.Value);
                gcs2ap_cmd_new.cmd_test = 0x22;//后退前进位:test = 00 100 010
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;


            }
        }
        /// <summary>
        /// 方向舵左满舵PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRudLMax_Click(object sender, EventArgs e)
        {
            if (checkBoxRudLmtAct.Checked)
            {
                gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_rudlmt.Value);
                gcs2ap_cmd_new.cmd_test = 0x2c;//左满位:test = 00 101 100
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;

            }
        }
        /// <summary>
        /// 方向舵右满舵PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRudRMax_Click(object sender, EventArgs e)
        {
            if (checkBoxRudLmtAct.Checked)
            {
                gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_rudlmt.Value);
                gcs2ap_cmd_new.cmd_test = 0x34;//右满位:test = 00 110 100
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }
        }
        /// <summary>
        /// 方向舵中位PWM设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRudLMid_Click(object sender, EventArgs e)
        {
            if ((gcs2ap_cmd_new.cmd_state == 0x3) && (checkBoxRudLmtAct.Checked))
            {
                gcs2ap_cmd_new.rud_pwm = Convert.ToByte(prBar_jsk_rudlmt.Value);
                gcs2ap_cmd_new.cmd_test = 0x3c;//中位:test = 00 111 100
                gcs2ap_cmd_new.cmd_flag |= 0x8;//回传命令
            }
            gbl_var.send_req_cnt++;
            gbl_var.send_cmd_req = true;

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            gcs2ap_wp.total = 0xff;
            gcs2ap_wp.type = 0;
            gcs2ap_wp.no = 0xff;
            gcs2ap_wp.lat = Convert.ToUInt32(homePos.Lat * 100000.0);
            gcs2ap_wp.lat = Convert.ToUInt32(homePos.Lng * 100000.0);
            //发送数据包

        }

        private void tSMenuItemDownCurWP_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (totalWPlist[i].Tag == actWP_marker.Tag.ToString())
                {
                    gcs2ap_wp.no = Convert.ToByte(i);
                    gcs2ap_wp.total = Convert.ToByte(totalWPlist.Count);
                    gcs2ap_wp.type = 0x0;//只发送一个航点
                    gcs2ap_wp.lat = Convert.ToUInt32(totalWPlist[i].Lat*100000);
                    gcs2ap_wp.lng = Convert.ToUInt32(totalWPlist[i].Lng*100000);
                    gcs2ap_wp.spd = Convert.ToByte(totalWPlist[i].Alt*10);
                    gbl_var.send_req_cnt++;
                    gbl_var.send_wp_req = true;
                    break;
                }
            }
        }

        private void tSMenuItemDownAllWP_Click(object sender, EventArgs e)
        {
            gcs2ap_wp.no = Convert.ToByte(0);
            gcs2ap_wp.total = Convert.ToByte(totalWPlist.Count);
            gcs2ap_wp.type = 0x1;//发送全部航点
            gcs2ap_wp.lat = Convert.ToUInt32(totalWPlist[0].Lat * 100000);
            gcs2ap_wp.lng = Convert.ToUInt32(totalWPlist[0].Lng * 100000);
            gcs2ap_wp.spd = Convert.ToByte(totalWPlist[0].Alt * 10);
            gbl_var.send_req_cnt++;
            gbl_var.send_wp_req = true;
        }

        private void prBar_jsk_throttle2_Click(object sender, EventArgs e)
        {

        }

        private void gMapControl_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOneStep_Click(object sender, EventArgs e)
        {
            ap2gcs_real.lat = ap2gcs_real.lat + 10;
            ap2gcs_real.lng = ap2gcs_real.lng + 10;
        }

        private void btnOneLeft_Click(object sender, EventArgs e)
        {
            ap2gcs_real.lat = ap2gcs_real.lat + 10;
            ap2gcs_real.lng = ap2gcs_real.lng - 10;
 
        }

        private void btnFwd_Click(object sender, EventArgs e)
        {
            if (gbl_var.run_state == 0x6)//人工遥控状态
            {
                gbl_var.mmotor_state = 0x1;
                labelMotorState.Text = "反向";
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }

            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_DIRECTION;
            gcs2ap_parameter.value = MOTOR_BACKWARD;

            gbl_var.send_req_cnt++;

        }

        private void btnBwd_Click(object sender, EventArgs e)
        {
            if (gbl_var.run_state == 0x6)//人工遥控状态
            {
                gbl_var.mmotor_state = 0x2;
                labelMotorState.Text = "正向";
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }

            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_DIRECTION;
            gcs2ap_parameter.value = MOTOR_FORWARD;

            gbl_var.send_req_cnt++;

        }

        private void btnMotStop_Click(object sender, EventArgs e)
        {
            if (gbl_var.run_state == 0x6)//人工遥控状态
            {
                gbl_var.mmotor_state = 0x0;
                labelMotorState.Text = "停车";
                gbl_var.send_req_cnt++;
                gbl_var.send_cmd_req = true;
            }

            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_DIRECTION;
            gcs2ap_parameter.value = MOTOR_OFF;

            gbl_var.send_req_cnt++;
        }

        private void trBarTurnMotor_Scroll_1(object sender, EventArgs e)
        {

        }

        private void trBarTurnMotorIntegral_Scroll(object sender, EventArgs e)
        {
            textBoxSteerIntegral.Text = Convert.ToString(trBarTurnMotorIntegral.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gbl_var.send_specify_wp = true;
        }

        private void arrive_radius_Click(object sender, EventArgs e)
        {
            

            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ARRIVE_RADIUS;
            gcs2ap_parameter.value = Convert.ToByte(arrive_radius_value.Text);
            gbl_var.parameter_set_arrive_radius = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //gbl_var.steer_reverse = true;
        }



        private void 离线地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          /*
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleChinaMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl.Position = new PointLatLng(39.958436, 116.309175);//此为定初始位置的另一种方式  
            gMapControl.CacheLocation = @"D:\\gmap";
            GMaps.Instance.ImportFromGMDB("d:\\gmap\\TileDBv5\\en\\Data.gmdb");
            gMapControl.ReloadMap();
            //GMap.NET.MapProviders.
           * */

        }

        private void 高德离线地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.CacheOnly; 
            gMapControl.MapProvider = GMap.NET.MapProviders.AMapProvider.Instance;
            //gMapControl.CacheLocation = @"D:\\gmap";
            gMapControl.CacheLocation = (Environment.CurrentDirectory + "\\GMapCache_AMap");
            //GMaps.Instance.ImportFromGMDB("d:\\gmap\\TileDBv5\\en\\Data.gmdb");
            GMaps.Instance.ImportFromGMDB(gMapControl.CacheLocation+"\\TileDBv5\\en\\Data.gmdb");
            gMapControl.ReloadMap();

            statusMapProvider.Text = "高德离线地图";
        }


        private void 谷歌中国离线地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {         
            //gMapControl.Position = new PointLatLng(39.958436, 116.309175);//此为定初始位置的另一种方式
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.CacheOnly;
            gMapControl.CacheLocation = (Environment.CurrentDirectory + "\\GMapMapCache_GoogleChinaMap");
            GMaps.Instance.ImportFromGMDB("d:\\gmap\\TileDBv5\\en\\Data.gmdb");           
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleChinaMapProvider.Instance;
            gMapControl.ReloadMap();

            statusMapProvider.Text = "谷歌中国离线地图";

        }

        private void 谷歌中国地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            //gMapControl.Position = new PointLatLng(39.958436, 116.309175);//此为定初始位置的另一种方式 
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleChinaMapProvider.Instance;           
            gMapControl.ReloadMap();

            statusMapProvider.Text = "谷歌中国地图";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar_CTE_P_Scroll(object sender, EventArgs e)
        {
            textBoxCTE_P.Text = Convert.ToString(trackBar_CTE_P.Value);
        }

        private void cmbBox_parameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //这里当参数过多时，应该用switch语句进行判断
            if(0==cmbBox_parameter.SelectedIndex)
            {
                //gcs2ap_parameter.type = 0;//暂时定义type=0为巡航油门
                //gcs2ap_parameter.value = Convert.ToByte(txtBox_parameter.Text);
            }
        }

        private void bttn_send_para_Click(object sender, EventArgs e)
        {
            /*这个暂时作为发送全部参数的保留*/
            /*因为海上通信效果较差，所以参数就一个一个的传输，如果一次性传输太多参数会有问题*/
            /*现在把这个参数设置改了，全部的参数都用按钮形式发送，并且1个是发送该参数的数值，另1个是是让自动驾驶仪回发该参数值*/
            /*
            gcs2ap_parameter.value = Convert.ToByte(txtBox_parameter.Text);
            gbl_var.parameter_set = true;
            gbl_var.send_req_cnt++;
             * */
        }

  

        private void button_set_throttle_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_CRUISE_THROTTLE;
            gcs2ap_parameter.value = Convert.ToByte(textBox_set_throttle.Text);

            gbl_var.parameter_set_cruise_throttle = true;

            gbl_var.send_req_cnt++;
        }

        private void button_get_cruise_throttle_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_GET_CRUISE_THROTTLE;
            gcs2ap_parameter.value = Convert.ToByte(0);

            //gbl_var.parameter_get_cruise_throttle = true;//这个标志位的设置放在接收线程中

            gbl_var.send_req_cnt++;
        }

        private void get_wp_from_pilot_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_GET_WP;
            gcs2ap_parameter.value = Convert.ToByte(textBox_wp_cnt.Text);

            gbl_var.parameter_set_get_wp = true;

            /*如果请求的是航点0或者127，那么就先清除掉任务，然后再重新画，目前只能先请求第0个航点，然后按顺序请求*/
            if (gcs2ap_parameter.value == 0 || gcs2ap_parameter.value == 127)
            {
                totalWPlist.Clear();
            }

            gbl_var.send_req_cnt++;
        }

        private void set_arrive_radius_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ARRIVE_RADIUS;
            //gcs2ap_parameter.value = Convert.ToByte(0);//一开始复制成这个了，导致一直收到的半径为0
            gcs2ap_parameter.value = Convert.ToByte(arrive_radius_value.Text);

            gbl_var.parameter_set_arrive_radius = true;

            gbl_var.send_req_cnt++;
        }

        private void button_get_arrive_radius_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_GET_ARRIVE_RADIUS;
            //gcs2ap_parameter.value = Convert.ToByte(textBox_wp_cnt.Text);
            gcs2ap_parameter.value = Convert.ToByte(0);
            //gbl_var.parameter_get_arrive_radius = true;//这个放在接收线程中

            gbl_var.send_req_cnt++;
        }

        private void btnmax_error_head_track_Click(object sender, EventArgs e)
        {
            //gbl_var.send_max_head_error_angle = true;

            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CTE_MAX_CORRECT;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = Convert.ToByte(textBoxCTE_max.Text);

            gbl_var.parameter_set_max_head_error_angle = true;

            gbl_var.send_req_cnt++;
        }

        private void button_get_cte_max_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_GET_CTE_MAX_CORRECT;
            gcs2ap_parameter.value = Convert.ToByte(0);
            //gcs2ap_parameter.value = Convert.ToByte(textBoxCTE_max.Text);

           //gbl_var.parameter_get_max_head_error_angle = true;//放在线程接收

            gbl_var.send_req_cnt++;
        }

        private void steer_reverse_btn_Click(object sender, EventArgs e)
        {
             /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_REVERSE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = RUDDER_REVERSE;

            gbl_var.send_req_cnt++;

            gbl_var.steer_reverse = true;
            checkBox_rudder_reverse.Checked = !checkBox_rudder_reverse.Checked;
        }

        private void throttle_reverse_btn_Click(object sender, EventArgs e)
        {
            gcs2ap_parameter.type = PARAMETER_SET_REVERSE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = THROTTLE_REVERSE;

            gbl_var.send_req_cnt++;

            gbl_var.throttle_reverse = true;
            checkBox_throttle_reverse.Checked = !checkBox_throttle_reverse.Checked;
        }

        private void button_set_rotary_as_center_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ROTARY;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = ROTARY_AS_CENTER;

            gbl_var.send_req_cnt++;

            checkBox_set_rotary_center.Checked = !checkBox_set_rotary_center.Checked;

        }

        private void button_rotary_clockwise_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ROTARY;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = ROTARY_CLOCKWISE;

            gbl_var.send_req_cnt++;

            checkBox_rotary_clockwise.Checked = !checkBox_rotary_clockwise.Checked;

        }

        private void button_rotary_counter_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ROTARY;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = ROTARY_COUNTER_CLOCKWISE;

            gbl_var.send_req_cnt++;

            checkBox_rotary_counter.Checked = !checkBox_rotary_counter.Checked;
        }

        private void button_get_rotary_position_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_GET_ROTARY_POSITION;
            gcs2ap_parameter.value = Convert.ToByte(0);
            //gcs2ap_parameter.value = ROTARY_COUNTER_CLOCKWISE;

            gbl_var.parameter_get_rotary_position = false;

            gbl_var.send_req_cnt++;

        }

        private void button_charge1_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_POWER_DISCHARGE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = POWER_CHARGE1;

            gbl_var.send_req_cnt++;

            checkBox_charge1.Checked = !checkBox_charge1.Checked;

        }

        private void button_charge2_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_POWER_DISCHARGE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = POWER_CHARGE2;

            gbl_var.send_req_cnt++;

            checkBox_charge2.Checked = !checkBox_charge2.Checked;

        }

        private void button_charge3_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_POWER_DISCHARGE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = POWER_CHARGE3;

            gbl_var.send_req_cnt++;

            checkBox_charge3.Checked = !checkBox_charge3.Checked;

        }

        private void button_discharge1_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_POWER_DISCHARGE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = DISCHARGE1;

            gbl_var.send_req_cnt++;

            checkBox_discharge1.Checked = !checkBox_discharge1.Checked;

        }

        private void button_discharge2_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_POWER_DISCHARGE;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = DISCHARGE2;

            gbl_var.send_req_cnt++;

            checkBox_discharge2.Checked = !checkBox_discharge2.Checked;
        }

        private void button_generator_onoff_Click(object sender, EventArgs e)
        {

        }

        private void button_generator_on_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_GENERATOR;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = GENERATOR_ON;

            gbl_var.send_req_cnt++;

            //checkBox_discharge2.Checked = !checkBox_discharge2.Checked;
        }

        private void button_generator_off_Click(object sender, EventArgs e)
        {
        
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_GENERATOR;
            //gcs2ap_parameter.value = Convert.ToByte(0);
            gcs2ap_parameter.value = GENERATOR_OFF;

            gbl_var.send_req_cnt++;

            //checkBox_discharge2.Checked = !checkBox_discharge2.Checked;
        }

        private void teBox_ap_lng_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_DIRECTION;
            gcs2ap_parameter.value = MOTOR_ON;

            gbl_var.send_req_cnt++;
        }

        private void left_motor_increase_ValueChanged(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_LEFT_INCREASE;
            gcs2ap_parameter.value = Convert.ToByte(left_motor_increase.Value);

            gbl_var.send_req_cnt++;
        }

        private void right_motor_increase_ValueChanged(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_MOTOR_RIGHT_INCREASE;
            gcs2ap_parameter.value = Convert.ToByte(right_motor_increase.Value);

            gbl_var.send_req_cnt++;
        }

        private void button_lanuch_rocket_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ROCKET;
            gcs2ap_parameter.value = PARAMETER_SET_ROCKET_LAUNCH;

            gbl_var.send_req_cnt++;

        }

        private void trBarTurnMotorDeviate_Scroll(object sender, EventArgs e)
        {
            textBoxSteerDeviate.Text = Convert.ToString(trBarTurnMotorDeviate.Value);
        }

        private void button_cte_I_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CTE_I;
            gcs2ap_parameter.value = Convert.ToByte(textBox_cte_I.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_cte_D_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CTE_D;
            gcs2ap_parameter.value = Convert.ToByte(textBox_cte_D.Text);

            gbl_var.send_req_cnt++;

        }

        private void button_throttle_change_time_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_THROTTLE_PERCENT_TIME;
            gcs2ap_parameter.value = Convert.ToByte(textBox_throttle_change_time.Text);

            gbl_var.send_req_cnt++;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button_differential_control_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_USE_DIFFERENTIAL_CONTROL;
            gcs2ap_parameter.value = DIFFERENTIAL_CONTROL;

            gbl_var.send_req_cnt++;
        }

        private void button_rudder_control_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_USE_DIFFERENTIAL_CONTROL;
            gcs2ap_parameter.value = RUDDER_CONTROL;

            gbl_var.send_req_cnt++;
        }

        private void button_boat_head_navigation_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_USE_TRAJECTORY_NAVIGATION;
            gcs2ap_parameter.value = BOAT_HEAD_NAVIGATION;

            gbl_var.send_req_cnt++;
        }

        private void button_trajectory_navigation_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_USE_TRAJECTORY_NAVIGATION;
            gcs2ap_parameter.value = TRAJECTORY_NAVIGATION;

            gbl_var.send_req_cnt++;
        }

        private void button_set_switch_channel_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_SWITCH_CHANNEL;
            gcs2ap_parameter.value = Convert.ToByte(textBox_switch_channel.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_set_switch_low_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_SWITCH_LOW_LIMIT;
            gcs2ap_parameter.value = Convert.ToByte(textBox_switch_low.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_set_switch_high_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_SWITCH_HIGH_LIMIT;
            gcs2ap_parameter.value = Convert.ToByte(textBox_switch_high.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_switch_start_on_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_SWITCH_START_ON;
            //gcs2ap_parameter.value = Convert.ToByte(textBox_switch_high.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_set_charge_channel_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CHARGE_CHANNEL;
            gcs2ap_parameter.value = Convert.ToByte(textBox_charge_channel.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_set_charge_voltage_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CHARGE_VOLTAGE;
            gcs2ap_parameter.value = Convert.ToByte(textBox_charge_voltage.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_set_charge_current_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CHARGE_CURRENT;
            gcs2ap_parameter.value = Convert.ToByte(textBox_charge_current.Text);

            gbl_var.send_req_cnt++;
        }

        private void button_charge_start_on_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CHARGE_START_ON;
            gcs2ap_parameter.value = PARAMETER_SET_CHARGE_TURN_ON;

            gbl_var.send_req_cnt++;
        }

        private void button_charge_turn_off_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_CHARGE_START_ON;
            gcs2ap_parameter.value = PARAMETER_SET_CHARGE_TURN_OFF;

            gbl_var.send_req_cnt++;
        }

        private void btnPortBOpenClose_Click(object sender, EventArgs e)
        {
            if (comPortB.IsOpen)
            {
                comBClosing = true;
                while (comBListening) Application.DoEvents();//打开时点击，则关闭串口
                comPortB.Close();
                cbxBCOMPort.Enabled = true;
                cbxBBaudRate.Enabled = true;
                btnPortBOpenClose.Text = "打开";
            }
            else
            {
                if ((comPortB.PortName != "None") && (comPortB.PortName != null))
                {
                    comPortB.PortName = cbxBCOMPort.Text.Trim();//设置串口名
                    comPortB.BaudRate = Convert.ToInt32(cbxBBaudRate.Text.Trim());//设置串口的波特率
                    comPortB.DataBits = 8;//设置数据位
                    comPortB.StopBits = StopBits.One;
                    comPortB.Parity = Parity.None;
                    comPortB.ReadTimeout = 1000;//[ms]
                    comPortB.WriteTimeout = 1000;//[ms]
                    comPortB.ReadBufferSize = 1024;
                    comPortB.WriteBufferSize = 1024;

                    comPortB.Open();//打开串口
                }
                else
                {
                    MessageBox.Show("请检查串口配置", "提示");
                }
                if (comPortB.IsOpen)
                {
                    cbxBCOMPort.Enabled = false;
                    cbxBBaudRate.Enabled = false;
                    comBClosing = false;//等待关闭串口状态改为false 
                    btnPortBOpenClose.Text = "关闭";
                }
            }
        }

        private void btnPortBRefresh_Click(object sender, EventArgs e)
        {
            if (comPortB.IsOpen)
            {
            }
            else//串口没有打开的情况下允许重新搜索串口
            {
                cbxBCOMPort.Items.Clear();
                ports = SerialPort.GetPortNames();//获取可用串口
                if (ports.Length > 0)//有串口可用
                {
                    for (int i = 0; i < ports.Length; i++)
                    {
                        cbxBCOMPort.Items.Add(ports[i]);//下拉控件里添加可用串口
                    }
                    cbxBCOMPort.SelectedIndex = 0;
                }
                else//未检测到串口
                {
                    cbxBCOMPort.Text = "None";
                }
            }
        }

        private void button_rocket_close_Click(object sender, EventArgs e)
        {
            /*这个是所有的参数设置都要把这个置为true*/
            gbl_var.send_parameter_set = true;

            gcs2ap_parameter.type = PARAMETER_SET_ROCKET;
            gcs2ap_parameter.value = PARAMETER_SET_ROCKET_CLOSE;

            gbl_var.send_req_cnt++;
        }

    }
}
