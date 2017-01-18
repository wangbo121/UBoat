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
        public bool bJoystickInstalled;//已检测到游戏杆
        public byte run_state;//工作状态,0x3: 测试; 0x6: 手控; 0x5: 自驾; 0xa0: 停车
        public int main_timer_interval;//主定时器的时间间隔，从控件中获取

        /// <summary>
        /// 总航点数
        /// </summary>
        public byte wp_total;
        /// <summary>
        /// 要发送航点类型，1：发送全部航点；0：发送指定航点
        /// </summary>
        public byte send_total_wp;
        /// <summary>
        /// 发送请求累加器
        /// </summary>
        public int send_req_cnt;
        /// <summary>
        /// 上一次的发送请求累加器
        /// </summary>
        public int send_req_cnt_lst;

        public bool send_cmd_req;
        public bool send_wp_req;
        public bool send_lnk_req;
        public bool send_cte_req;
        /// <summary>
        /// 主电机状态，=0停车; =1:正转; =2:反转
        /// </summary>
        public byte mmotor_state;


        //舵机限位标定值（从AP反馈回来的数据）
        /// <summary>
        /// AP反馈主电机ON位置
        /// </summary>
        public byte motor_on_pwm;
        /// <summary>
        /// AP反馈主电机OFF位置
        /// </summary>
        public byte motor_off_pwm;
        /// <summary>
        /// AP反馈主电机正向位置
        /// </summary>
        public byte motor_fwd_pwm;
        /// <summary>
        /// AP反馈主电机反向位置
        /// </summary>
        public byte motor_bwd_pwm;
        /// <summary>
        /// AP反馈方向舵左满位置
        /// </summary>
        public byte rud_left_pwm;
        /// <summary>
        /// AP反馈方向舵右满位置
        /// </summary>
        public byte rud_right_pwm;
        /// <summary>
        /// AP反馈方向舵中位
        /// </summary>
        public byte rud_mid_pwm;

        public byte mmotor_pwm_out;

        //数据包接收计数
        public UInt32 ap2gcs_real_cnt;
        public UInt32 ap2gcs_real_cnt_lst;
        public UInt32 ap2gcs_aws_cnt;
        public UInt32 ap2gcs_aws_cnt_lst;
        public UInt32 ap2gcs_wp_cnt;
        public UInt32 ap2gcs_wp_cnt_lst;
        public UInt32 ap2gcs_ack_cnt;
        public UInt32 ap2gcs_ack_cnt_lst;

        //above is write after 9.16 

        /// <summary>
        /// 模拟状态.true:模拟状态;false:正常状态
        /// </summary>
        public bool bSimuState;
        /// <summary>
        /// 模拟飞行状态，此时可动态显示飞行器
        /// </summary>
        public bool bSimuSailing;

     
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
        //王博wangbo
        public bool send_specify_wp;
        //public bool send_arrive_radius;
        
       

        public bool steer_reverse;
        public bool throttle_reverse;

        /*发送设置参数命令*/
        public bool send_parameter_set;
        /*设置参数标志*/
        public bool parameter_set_get_wp;

        public bool parameter_set_cruise_throttle;//0
        public bool parameter_get_cruise_throttle;//1
        /*获取自驾仪的航点总数，并且显示*/
        public byte all_wp_num;//2
        public bool parameter_set_arrive_radius;//3
        public bool parameter_get_arrive_radius;//4
        public bool parameter_set_max_head_error_angle;//5
        public bool parameter_get_max_head_error_angle;//6
        public bool parameter_get_rotary_position;//10

        

    };

    public struct ComPortPar
    {
        public string PortName;
        public int BaudRate;
        public int DataBits;
        public float StopBits;
        public string Parity;
    };
    /// <summary>
    /// 发送的单航点结构，12字节。双向
    /// </summary>
    public struct GCS_AP_WP
    {
        public byte type;//航点类型
        public byte total;//总航点数
        public byte no;//本航点号
        public byte spd; //[knot]，第1个航点处的设定航速，单位为节，大约0.5m/s
        public UInt32 lng;//[度*0.00001]，第1个航点的经度坐标，整型，精确到米
        public UInt32 lat;//[度*0.00001]，第1个航点的纬度标，整型，精确到米
    }
    /// <summary>
    /// 发送的GCS-->AP的命令，16字节。双向，GCS发命令，AP回传
    /// </summary>
    public struct GCS_AP_CMD
    {
        public byte cmd_state;//工作状态命令
        public byte cmd_test;//调试命令，仅在cmd_state的D2D1D0=011时有效。
        public byte cmd_manu;//手柄控制命令，仅在Cmd_STATE的D2D1D0=110时有效。
        public byte cmd_auto;//自动状态控制命令，仅在cmd_STATE的D2D1D0=101时有效。
        public byte cmd_rkt;//火箭工作命令，预留
        public byte cmd_aws;//气象站工作命令，预留
        public byte cmd_mot;//主电机各环节工作命令，预留
        public byte cmd_flag;//各设备的工作状态设置,D2D1D0: 通信链路设置位
        public byte moo_pwm;//主电机启停舵机PWM输出 0-255，仅在测试状态下有效
        public byte mbf_pwm;//主电机前进后退舵机PWM输出 0-255，仅在测试状态下有效
        public byte rud_pwm;//方向舵舵机PWM输出 0-255，在测试、手柄控制和自动控制下均有效
        public byte rud_p;//方向舵控制P增益（任意状态均可修改，修改后即生效）
        public byte rud_i;//方向舵控制I增益（任意状态均可修改，修改后即生效）
        public byte wpno;//航点编号,0：无效;1-254:航点号;255:返航
        //public byte spare1;//备用
        //public byte spare2;//备用

        public byte cte_p;//偏航距 的比例系数
        //wangbo
        //public byte reverse;
        public byte rud_d;//20161205 wangbo
       
    }
    /// <summary>
    /// GCS发给AP的链路确认包，8字节。单向
    /// </summary>
    public struct GCS2AP_LNK
    {
        public UInt32 hhmmss;//发送数据包时间，按hhmmss格式
        public UInt16 time_interval;//[秒]预计下次发送本数据包的时间间隔
        public UInt16 spare;//备用
    }

    public struct GCS2AP_CTE
    {
	    public byte cte_p;
	    public byte  cte_i;
	    public byte  cte_d;
        public byte cte_max_correct_angle;
    }

    public struct GCS_AP_PARAMETER
    {
        public byte type;
        public byte value;
    }

    /// <summary>
    /// AP发送给GCS的实时数据包，30字节。单向
    /// </summary>
    public struct AP2GCS_REAL
    {
        public UInt32 lng;//[度*0.00001]，GPS经度坐标，整型，精确到米
        public UInt32 lat;//[度*0.00001]，GPS纬度坐标，整型，精确到米
        //public Int16 spd;//[Knot*0.01]，实时航速。注意，精度比设定航速高
        public UInt32 spd;//[Knot*0.01]，实时航速。注意，精度比设定航速高
        public Int16 dir_gps;//[度*0.01]，地速航向，GPS航向
        public Int16 dir_heading;//[度*0.01]，机头朝向
        public Int16 dir_target;//[度*0.01]，目标点朝向
        public Int16 dir_nav;//[度*0.01]，导航航向
        public Int16 pitch;//[度*0.1]，俯仰
        public Int16 roll;//[度*0.1]，滚转
        public Int16 yaw;//[度*0.1]，偏航
        public byte moo_pwm;//主电机启停舵机PWM 0-255
        public byte mbf_pwm;//主电机前进后退舵机PWM 0-255
        public byte rud_pwm;//方向舵舵机PWM 0-255
        public byte mm_state;//主推进电机状态
        public byte rud_p;//方向舵机控制P增益
        public byte rud_i;//方向舵机控制I增益
        public byte boat_temp0;//预留 可能用于pid的微分环节
        public byte boat_temp1;//[度]，艇内1号点温度
        public byte boat_temp2;//[度]，艇内2号点温度
        //public byte boat_humi;//[%]，艇内湿度
        public byte wp_load_cnt;//[%]，艇内湿度
        public byte wpno;//下一航点编号，0xff表示是GCS发送的新航点
        /*20161211改变下面几个实时数据*/
        public byte generator_onoff_req;
        public byte voltage_bat1;
        public byte voltage_bat2;

        /*s上面是40个字节*/
        public byte toggle_state;
        public byte charge_state;
        public byte temp;
        public byte humi;//湿度
        public byte windspeed;
        public byte winddir;
        public byte airpress;
        public byte seasault;

        public byte elec_cond;
        public byte seatemp1;
        public byte launch_req_ack;
        public byte rocket_state;
        public byte rktnumber;
        public byte spare1;
        public Int16 alt;
    
        public UInt32 spare10;
        public UInt32 spare11;
        public UInt32 spare12;
        public UInt32 spare13;
        public UInt32 spare14;
        public UInt32 spare15;
    }
    /// <summary>
    /// AP发送给GCS的接收确认包，8字节。单向
    /// 只在不需要回传命令或航点时才发送该包，否则回传的包即可作为确认包
    /// </summary>
    public struct AP2GCS_ACK
    {
        public byte type;//接收数据包类型，即接收到的数据包所包含的类型
        public byte cnt;//接收数据包编号，即接收到的数据包所包含的计数
        public byte state;//接收数据包状态
        public byte spare;//备用
        public UInt32 hhmmss;//接收数据包时间，按hhmmss格式
    }
    /// <summary>
    /// AP发送给GCS的气象站数据包，24字节。单向
    /// </summary>
    public struct AP2GCS_AWS
    {
        public UInt32 hhmmss;//发送数据包时间，按hhmmss格式
        public UInt32 lng;//[度*0.00001]，当前GPS经度坐标，整型，精确到米
        public UInt32 lat;//[度*0.00001]，当前GPS纬度坐标，整型，精确到米
        public Int16 temp;//[度*0.01]，温度
        public Int16 dewtemp;//[度*0.01]，露点温度
        public UInt16 humi;//[*0.01]，湿度
        public UInt16 airpress;//[*0.01]，气压
        public Int16 winddir;//[*0.01]，风向
        public Int16 windspd;//[*0.01]，风速
    }
    /// <summary>
    /// 处理接收数据包使用的结构
    /// </summary>
    public struct RECV_PACKET
    {
        public byte len;
        public byte cnt;
        public byte sysid;
        public byte type;
        public byte checksum;
        public byte state;
    }

    /// <summary>
    /// 北斗数据从地面站发送给自驾仪的包
    /// </summary>
    public struct BD_GCS2AP
    {
        public byte msgid;
        public byte cnt;
        public char[] data;
    }
}
