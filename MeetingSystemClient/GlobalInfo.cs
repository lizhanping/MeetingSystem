using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MeetingSystemClient
{
    class GlobalInfo
    {
        public static double softVersion = 2017.7;//软件版本
        public static string softMaker = "北京京航计算通讯研究所";
        public static string softUser = "军办认证测评中心";

        public static string MeetingFold = "";

        public static string localIP = "";//IP地址
        public static string localUserName = "";//用户名
        public static string serverIP = "";//服务端IP地址
        public static int Port = 2001;//设置通信端口 发送数据
        public static int Port1 = 2002;//发送回传数据端口

        public static Socket serverSocket = null;
        public static List<string> FilterList = new List<string>() { ".doc",".docx",".xls",".xlsx",".pdf"};
    }
}
