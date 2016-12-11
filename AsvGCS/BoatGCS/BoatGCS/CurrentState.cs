using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using System.Collections;
using BoatGCS;


namespace BoatGCS
{
    /// <summary>
    /// 整个工程中传递状态的变量
    /// </summary>
    public struct GBL_VAR
    {
        public bool bAutoPilot;//控制模式，false：手动游戏杆控制；true：自驾
        public bool bEStop;//紧急停车，=true时向AP发送的推进和转向控制输出全为0

        public bool bJoystickInstalled;//已检测到游戏杆

        /// <summary>
        /// 模拟状态.true:模拟状态;false:正常状态
        /// </summary>
        public bool bSimuState;
        /// <summary>
        /// 模拟飞行状态，此时可动态显示飞行器
        /// </summary>
        public bool bSimuSailing;

        public int wHeartbeatCnt;
        public int wHeartbeatCntOld;
        
        /// <summary>
        /// 航路规划状态切换。true:处于航路规划状态;false:处于实时显示状态
        /// </summary>
        public bool bRoutePlan;

        public bool markerentered;
        public bool markerselected;
        /// <summary>
        /// 用于给参数设置画面传递要设置的参数ID
        /// 1:航点高度
        /// </summary>
        public int paramset_id;

    };

    public struct ComPortPar
    {
        public string PortName;
        public int BaudRate;
        public int DataBits;
        public float StopBits;
        public string Parity;
    };

    public struct CONTROL_DATA
    { 
        public byte throttle;
        public byte yaw;
        public byte roll;
        public byte pitch;
    }

    // 下面定义GCS和AP之间的传输数据结构，分别为：
    //GCS->AP: 控制调整参数包GCS2AP_PARA; 操作杆包GCS2AP_RC; 控制命令包: GCS2AP_CONTROL; 航点包: GCS2AP_WAYPOINTS
    //AP->GCS: 实时数据包:
    /// <summary>
    /// 控制调整参数包
    /// </summary>
    public struct GCS2AP_PARA
    {
        public byte mainMotor_gain;//0-255,主推进电机控制增益,暂无意义
        public byte turnMotor_gain;//0-255,转向电机控制增益,
    }

    /// <summary>
    /// 操作杆遥控包
    /// </summary>
    public struct GCS2AP_RC
    {
        public byte mainMotor_output;//0-255,主推进电机控制输出，0-80:反转;80-160:停车,160以上:正转
        public byte turnMotor_output;//0-255,转向电机控制输出
    }
    /// <summary>
    /// 控制命令包
    /// </summary>
    public struct GCS2AP_CONTROL
    {
        public byte cmd1;//命令字节1，由8位命令构成。2(1下一航点)-1(1返航)-00(高0:无效1有效;0手动1自动)
        public byte cmd2;//命令字节2，由8位命令构成
        //public double wp_lng;//修改当前目标点的经度值
        //public double wp_lat;//修改当前目标点的纬度值
    }
    /// <summary>
    /// 航点任务结构，在没有下水前发送
    /// </summary>
    public struct GCS2AP_WAYPOINTS
    {
        public byte total_num;
        public double lng0;
        public double lat0;
        public double lng1;
        public double lat1;
        public double lng2;
        public double lat2;
        public double lng3;
        public double lat3;
    }

    /// <summary>
    /// AP发给地面站的快速数据，每5秒或更大间隔发送一个包
    /// 当接收到GCS发送的数据时，会立刻返回一次此数据包，来向GCS说明接收数据是否正确。
    /// ack=0：非确认包;=0xff:接收错误;=0xf:接收正确
    /// </summary>
    public struct AP2GCS_FASTDATA
    {
        //自驾仪 auto pilot
        public short ap_pitch;//[degree]
        public short ap_roll;//[degree]
        public short ap_yaw;//[degree]
        public short ap_speed;
        //public short ap_alt;//
        public int ap_lng;//
        public int ap_lat;//
        public byte state1;
        public byte state2;
        /*
        public byte ws_temp;
        public byte ws_airpress;
        public byte ws_winddir;
        public byte ws_windspd;
        public byte state1;
        public byte state2;*/

        //public byte rudder;//方向舵PWM值
        //public byte ap_main_startstop;
        //public byte ap_state1;//
        //public byte ap_state2;//
        //public byte state;//状态
        //public byte ack;//用于反馈是否正确接收了GCS的数据包.
    };
    /// <summary>
    /// 慢速数据，每分钟发送一个
    /// </summary>
    public struct AP2GCS_SLOWDATA
    {
        //气象站数据 weather station
        //public double ws_lng;//
        //public double ws_lat;//
        public short ws_temp;//[C]
        public short ws_dewtemp;//[C]
        public short ws_humi;//[%]
        public short ws_airpress;//
        public short ws_winddir;//[]
        public short ws_windspd;//[]
    };

    /// <summary>
    /// 气象站发给AP的数据
    /// </summary>
    public struct AirStation
    {
        public int _ddmmnn;//日月年
        public int _hhmmss;//时分秒
        public int _lat;//纬度[0.01度]
        public int _lng;//经度[0.01度]
        public ushort _temp;//温度,=收到的数据*100
        public ushort _dewtemp;//露点温度,=收到的数据*100
        public ushort _humi;//湿度=收到的数据*100
        public ushort _airpress;//气压,=收到的数据*100
        public short _winddir;//风向, =收到的数据*100
        public ushort _windspd;//风速,=收到的数据*100
    }

    public struct RECV_PACKET
    {
        public byte len;
        public byte cnt;
        public byte sysid;
        public byte type;
        public byte checksum;
        public byte state;
//        public byte 
    }
    /// <summary>
    /// 调试测试用
    /// </summary>
    public struct AP2GCS_RC
    {
        public byte mainMotor_output;//主推进电机控制输出
        public byte turnMotor_output;//转向电机控制输出
    }


}
