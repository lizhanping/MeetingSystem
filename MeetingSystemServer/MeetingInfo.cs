﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Data.OleDb;
using System.Xml;
using waitForm;
using DataService;

namespace MeetingSystemServer
{
    public partial class MeetingInfo : Form
    {
        int fileCount = 0;//记录要分发的所有文件的个数，用于日志显示
        Dictionary<string, int> fileSendDic = new Dictionary<string, int>();
        private List<TreeNode> localSelectedFile = new List<TreeNode>();//存放选中的文件
        private List<TreeNode> remoteSelectFile = new List<TreeNode>();//存放远程的文件
        private List<TreeNode> selectUser = new List<TreeNode>();//存放选中的用户
        /// <summary>
        /// 定义一个指令结构体，包含ip、cmd、data
        /// </summary>
        struct cmdObj
        {
            public string ip;
            public string cmd;
            public List<string> dataList;
        }
        /// <summary>
        /// 文件和IP
        /// </summary>
        struct fileAndIp
        {
            public TreeNode fileName;
            public IPAddress ip;
        }

        //ip,fileName
        private Dictionary<string, List<string>> hasSendFile = new Dictionary<string, List<string>>();//存放各IP已发送的文件列表，只存放文件
        private Dictionary<string, List<string>> sendFailFile = new Dictionary<string, List<string>>();//存放未发送成功的文件列表
        private int sendCnt = 0;//发送文件计数
        /// <summary>
        /// 构造
        /// </summary>
        public MeetingInfo()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗口关闭时，应用一同退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Application.Exit();
            //关闭端口
            if (GlobalInfo.clientSocket != null)
            {
                GlobalInfo.clientSocket.Close();
                GlobalInfo.clientSocket = null;
            }
        }
        /// <summary>
        /// 加载基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingInfo_Load(object sender, EventArgs e)
        {
            topic.Text = GlobalInfo.MeetingTopic;
            department.Text = GlobalInfo.MeetingDepart;
            creater.Text = GlobalInfo.MeetingCreater;
            GlobalInfo.waterMark = readValueFromConfigByNode("waterMark").ToString() == "True"; //读取是否兼容水印
            GlobalInfo.encrypt = readValueFromConfigByNode("encrypt").ToString() == "True";//读取是否对文件进行加密传输
            //创建会议文件夹
            string FoldPath = Application.StartupPath + @"\MeetingFold\"+GlobalInfo.MeetingCreatTime.ToString("yyyyMMddHHmmss");
            try
            {
                Directory.CreateDirectory(FoldPath);
                GlobalInfo.localSpace = FoldPath;//定位到会议空间，用于监控
                localFoldWatcher.Path = FoldPath;
                localFoldWatcher.IncludeSubdirectories = true;
            }
            catch
            {
                MessageBox.Show("创建会议空间失败，请重试！ ");
                this.Close();
                return;
            }
            #region 扫描整个局域网络
            GetLocalBaseInfo();
            Thread thread = new Thread(new ThreadStart(ScanEntireNet));
            thread.Start();
            #endregion
            #region 开启自己对各终端的监听
            StartListenForClient();
            #endregion
            #region 加载图片集合
            loadImageList(localImageList, Application.StartupPath + @"\config\smallIcon.xml");
            #endregion
        }
        /// <summary>
        /// 加载图标集合
        /// </summary>
        private void loadImageList(ImageList imageList,string filePath)
        {
            string configFile = filePath;
            if (!File.Exists(configFile))
            {
                return;//保持默认
            }
            //存在，就加载
            imageList.Images.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            XmlNode rootNode = xmlDoc.SelectSingleNode("config");
            foreach (XmlNode xn in rootNode.ChildNodes)
            {
                string imagePath = Application.StartupPath + "\\" + xn.InnerText;
                if (File.Exists(imagePath))
                {
                    Bitmap image = new Bitmap(imagePath);
                    imageList.Images.Add(xn.Attributes["name"].Value,image);
                }
            }
        }
        /// <summary>
        /// 获取本机基本信息
        /// </summary>
        private void GetLocalBaseInfo()
        {
            GlobalInfo.localUserName = DataService.GetSystemInfo.getUserName();
            GlobalInfo.localIP = DataService.GetSystemInfo.getIP();
        }
        /// <summary>
        /// 扫描整个网络
        /// </summary>
        private void ScanEntireNet()
        {
            //GlobalInfo.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将能建立连接的ip，和用户名存入remoteIPList中；
            GlobalInfo.remoteIPList.Clear();//先清空
            //可以根据自己所处的网段决定整个网络的初始扫描基值
            if (GlobalInfo.localIP == "")//为空，先获取
            {
                GetLocalBaseInfo();
            }
            int index = GlobalInfo.localIP.LastIndexOf('.');
            string baseAddress = GlobalInfo.localIP.Substring(0,index+1);
            //baseAddress = "169.254.11.";
            //可以通过ping获取可连接的主机，之后再通过connect获取其余信息
            for (int i = 0; i < 255; i++)
            {
                string ipStr = baseAddress + i.ToString();//初始ip
                if (ipStr == GlobalInfo.localIP)
                {
                #if DEBUG
                    ;
                #elif Release
                    continue;
                #endif 
                }              
                Ping ping = new Ping();
                ping.PingCompleted += Ping_PingCompleted; //绑定事件
                ping.SendAsync(ipStr, 500, null);
            }
        }
        /// <summary>
        /// ping结束完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            // throw new NotImplementedException();
            if (e.Reply.Status == IPStatus.Success)
            {
                connectRemoteByIP(e.Reply.Address);                
            }
        }
        /// <summary>
        /// 通过IP地址建立连接
        /// </summary>
        /// <param name="ip"></param>
        private  void connectRemoteByIP(object ip)
        {
            byte[] recv = new byte[1024];
            //byte[] send = new byte[1024];
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           // Console.WriteLine("发送区缓冲大小为:"+clientSocket.SendBufferSize);
           // Console.WriteLine("接受区缓冲大小为："+clientSocket.ReceiveBufferSize);
            try
            {
                clientSocket.Connect((IPAddress)ip, GlobalInfo.Port);//连接成功，获取数据，添加至已连接列表                
                object msg = ip.ToString() + " 连接成功！";
                Console.WriteLine(msg.ToString());
                updateLogInfo(msg);//日志显示

                /*   唤醒->发会议信息->要求返回客户信息->接受返回的客户端信息    */

                //唤醒客户端，弹出会议界面
                DataService.DataService.SendCommand(clientSocket, DataService.DataService.wakeUpClient);
                Thread.Sleep(200);
                //发送会议信息过去
                DataService.DataService.SendCommand(clientSocket, DataService.DataService.recvMeetingInfo, GlobalInfo.MeetingTopic + "\\" + GlobalInfo.MeetingDepart+"\\"+GlobalInfo.MeetingCreater+"\\"+ readValueFromConfigByNode("clearLevel")+"\\"+readValueFromConfigByNode("waterMark")+"\\"+readValueFromConfigByNode("encrypt"));
                Thread.Sleep(200);
                //获取用户信息：发指令，接数据
                DataService.DataService.SendCommand(clientSocket, DataService.DataService.getUserInfo);
                int recvLength = clientSocket.Receive(recv);
                string userName = Encoding.Unicode.GetString(recv, 0, recvLength);
                msg = ip.ToString()+" 用户名为："+userName;
                Console.WriteLine(msg);
                updateLogInfo(msg);
                //添加至远程终端列表中
                GlobalInfo.remoteIPList.Add(ip.ToString(), userName);
                /*
                Thread thread = new Thread(new ThreadStart(updateUserInfo));
                thread.Start();
                */
                updateUserInfo(); //更新用户信息
            }
            catch (Exception ex)
            {
                object msg = ip.ToString() + "连接失败！" + "\n" + "原因:" + ex.Message;
                Console.WriteLine(msg);
                updateLogInfo(msg);
            }
            finally
            {
                clientSocket.Close();                
            }
        }
        /// <summary>
        /// 当有连接时更新用户信息树
        /// </summary>
        private  void updateUserInfo()
        {
            //添加已连接用户
            try
            {
                if (userInfo.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        userInfo.Nodes.Clear();
                        selectUser.Clear();
                        lock(GlobalInfo.remoteIPList)
                        {
                            string[] keys = GlobalInfo.remoteIPList.Keys.ToArray();
                            for (int i = 0; i < keys.Length;i++)
                            {
                                string ip = keys[i];
                                TreeNode tn = new TreeNode();
                                tn.Text = GlobalInfo.remoteIPList[ip];
                                tn.Name = ip;
                                tn.Checked = true;
                                if (!selectUser.Contains(tn))
                                    selectUser.Add(tn);
                                userInfo.Nodes.Add(tn);

                            }
                            /*注释以下部分，解决程序因为异常闪退的情况，修改为上面部分*/
                            //foreach (string ip in GlobalInfo.remoteIPList.Keys)
                            //{
                            //    TreeNode tn = new TreeNode();
                            //    tn.Text = GlobalInfo.remoteIPList[ip];
                            //    tn.Name = ip;
                            //    tn.Checked = true;
                            //    if (!selectUser.Contains(tn))
                            //        selectUser.Add(tn);
                            //    userInfo.Nodes.Add(tn);
                            //}
                        }
                        
                        //userInfo.Nodes.Add(x);
                    }));
                }
                if (status.InvokeRequired)
                {
                    int cnt = GlobalInfo.remoteIPList.Count;
                    this.BeginInvoke(new Action<int>(x => status.Text = "已连接" + x + "台客户端"), cnt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
                return;
            }
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message"></param>
        private void updateLogInfo(object message)
        {
            string msg = "[" + DateTime.Now + "]:" + message.ToString() + "\r\n";
            if (logInfo.Enabled) //根据使能情况，决定是否能显示
            {
                if (logInfo.InvokeRequired)
                {
                    try
                    {
                        this.BeginInvoke(new Action<string>(x => logInfo.AppendText(x)), msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }    
                }
            }
            //并写入日志文件
            string logPath = Application.StartupPath + "//log.txt";
            //将内容写入
            DataService.LogManager.logInfo(logPath, msg);
        }
        /// <summary>
        /// 开启对客户端的监听
        /// </summary>
        private void StartListenForClient()
        {
            IPAddress ip = IPAddress.Parse(GlobalInfo.localIP);
            GlobalInfo.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            GlobalInfo.clientSocket.Bind(new IPEndPoint(ip, GlobalInfo.Port1));//对该socket绑定IP和端口            
            GlobalInfo.clientSocket.Listen(Int32.Parse(readValueFromConfigByNode("clientMaxNum").ToString()));//只监听40个连接
            Console.WriteLine("start listenning {0} success! " , GlobalInfo.clientSocket.LocalEndPoint.ToString());
            try
            {
                Thread myThread = new Thread(ListenClientConnect); //启动一个线程处理端口的监听事件，防止阻塞主界面
                myThread.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine("启动监听异常:" + ex.Message);
            }
            
        }
        /// <summary>
        /// 读取配置文件节点的值
        /// </summary>
        /// <param name="node">节点名称</param>
        /// <returns></returns>
        private object readValueFromConfigByNode(string node)
        {
            string path = Application.StartupPath + @"\config\config.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            return xmlDoc.SelectSingleNode("config").SelectSingleNode(node).InnerText;
        }
        /// <summary>
        /// 监听连接
        /// </summary>
        private void ListenClientConnect()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = GlobalInfo.clientSocket.Accept(); //接受到一个connet    
                    object msg = "已监听到" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + "的连接...";
                    Console.WriteLine(msg);
                    updateLogInfo(msg);
                    //Directory.CreateDirectory(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString()]);
                    Thread receiveThread = new Thread(new ParameterizedThreadStart(ReceiveMessage));
                    receiveThread.Start(clientSocket);  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
        /// <summary>
        /// 接收服务端软件发送的信息
        /// </summary>
        private void ReceiveMessage(object clientSocket)
        {
            byte[] recv = new byte[DataService.DataService.Buff_Size];
            Socket myClientSocket = (Socket)clientSocket;
            long fileSize = 0;
            string fileName = "";
            int hasReceBytes = 0;//已接收的数据字节
            byte[] fileData = new byte[0];
            long realFileSize = 0L;
            while (true)
            {
                try
                {
                    int receiveNumber = myClientSocket.Receive(recv);
                    if (receiveNumber > 0)
                    {
                        Console.WriteLine("have received client {0}  length {1} ", myClientSocket.RemoteEndPoint.ToString(), receiveNumber.ToString());
                        //解析数据
                        if (Encoding.Unicode.GetString(recv, 0, DataService.DataService.HeadLength) == DataService.DataService.cmdHeadStr) //指令头
                        {
                            byte[] recvByte = new byte[receiveNumber];
                            recvByte = recv.Skip(0).Take(receiveNumber).ToArray();
                            ProcessCmd(myClientSocket, recvByte);
                        }
                        else if (Encoding.Unicode.GetString(recv, 0, DataService.DataService.HeadLength) == DataService.DataService.dataHeadStr)//数据头
                        {
                            //接受数据,解析数据文件大小和文件名
                            fileSize = BitConverter.ToInt32(recv, DataService.DataService.HeadLength);//获取4节点文件大小
                            realFileSize = fileSize;
                            fileData = new byte[fileSize];
                            fileName = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + 8, receiveNumber - DataService.DataService.HeadLength - 8);
                            object msg = "检测到客户端"+((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()+"回传文件数据：文件名为：" + fileName + "文件大小为：" + fileSize;
                            DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvFileHeadSuccess);//答复
                            Console.WriteLine(msg);
                            updateLogInfo(msg);
                            //如果文件大小为0，则下次不会接收到数据，因此需要在此处立即建立空文件即可
#region
                            if (fileSize == 0)
                            {
                                File.WriteAllBytes(GlobalInfo.localSpace+GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()] + "\\" + fileName, DataService.SecurityTransmit.Decoding(fileData));
                                if (File.Exists(GlobalInfo.localSpace+"\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()] + "\\" + fileName))
                                {
                                    msg = "创建文件" + fileName + "成功！";
                                    Console.WriteLine(msg);
                                    updateLogInfo(msg);
                                }
                                else
                                {
                                    msg = "创建文件" + fileName + "失败！";
                                    Console.WriteLine(msg);
                                    updateLogInfo(msg);
                                }
                            }
#endregion
                        }
                        else //其余则为其他数据
                        {
                            Console.WriteLine("接收到新数据，当前数据量为:" + hasReceBytes + " byte");
                            if (hasReceBytes < fileSize)
                            {
                                byte[] decodeData = new byte[receiveNumber];
                                Array.Copy(recv, decodeData, receiveNumber);
                                decodeData.CopyTo(fileData, hasReceBytes);
                                hasReceBytes += decodeData.Length;
                            }
                            //文件接收完毕
                            if (hasReceBytes >= fileSize)
                            {
                                //保存文件
                                try
                                {
                                    //此处判断下是否存在加密的情况
                                    if (GlobalInfo.encrypt)
                                    {
                                        byte[] realFile = new byte[realFileSize];
                                        int i = 0;
                                        int times = (int)(fileSize / (DataService.DataService.Buff_Size + 16));
                                        Console.WriteLine("times:" + times);
                                        for (i = 0; i < times; i++)
                                        {
                                            Console.WriteLine("当前i:" + i);
                                            byte[] encryData = new byte[DataService.DataService.Buff_Size + 16];
                                            Array.Copy(fileData, encryData, encryData.Length);
                                            Console.WriteLine("encryData length:" + encryData.Length);
                                            byte[] test = SecurityTransmit.Decoding(encryData);
                                            Console.WriteLine("test length:" + test.Length);
                                            try
                                            {
                                                test.CopyTo(realFile, DataService.DataService.Buff_Size * i);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message.ToString());
                                            }

                                        }
                                        //处理剩下部分
                                        SecurityTransmit.Decoding(fileData.Skip(i * (DataService.DataService.Buff_Size + 16)).ToArray()).CopyTo(realFile, DataService.DataService.Buff_Size * i);
                                        //保存文件
                                        Directory.CreateDirectory(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()]);
                                        File.WriteAllBytes(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()] + "\\" + fileName, fileData);
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()]);
                                        File.WriteAllBytes(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()] + "\\" + fileName, fileData);
                                    }

                                    if (File.Exists(GlobalInfo.localSpace + "\\" + GlobalInfo.remoteIPList[((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString()] + "\\" + fileName))
                                    {
                                        Console.WriteLine("创建文件成功！");
                                     }
                                    else
                                    {
                                        Console.WriteLine("创建文件失败！");
                                       // DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvFileHeadFailed);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("此处捕获文件接收完毕时的异常情况:" + ex.Message);
                                }

                            }

                        }
                    }
                    else
                    {
                        //myClientSocket.Shutdown(SocketShutdown.Both);
                        //myClientSocket.Close();                        
                        break;
                    }
                }
                catch (Exception ex)
                {
                    object msg = "异常：" + ex.Message;
                    Console.WriteLine(msg);
                    updateLogInfo(msg);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
        /// <summary>
        /// 根据指令处理对应信息
        /// </summary>
        /// <param name="cmd"></param>
        private void ProcessCmd(Socket connectSocket, byte[] recv)
        {
            string cmd = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength, DataService.DataService.cmdLength);
            switch (cmd)
            {                
                case DataService.DataService.createFolder: //创建文件夹
                    {
                        string folderName = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, recv.Length - DataService.DataService.HeadLength - DataService.DataService.cmdLength);
                        if (Directory.Exists(GlobalInfo.localSpace +"\\"+ GlobalInfo.remoteIPList[((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString()] + @"\" + folderName))
                        {
                            DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdFailed, DataService.DataService.objectHasExist);
                            return;
                        }
                        try
                        {
                            Directory.CreateDirectory(GlobalInfo.localSpace+"\\" + GlobalInfo.remoteIPList[((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString()] + @"\" + folderName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("创建文件夹" + folderName + "失败！" + "\n原因：" + ex.Message);
                            DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdFailed, DataService.DataService.createError);
                            return;
                        }
                        DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdSucess);
                    }
                    break;
                case DataService.DataService.ExitMeeting: //退出会议
                    {
                        //先获取远程IP地址
                        string ip = ((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString();
                        GlobalInfo.remoteIPList.Remove(ip);
                        updateUserInfo();
                    }
                    break;
                default: break;
            }
        }
        /// <summary>
        /// 刷新并检测当前接入的客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientRefresh_Click(object sender, EventArgs e)
        {
            //先清除所有人信息，再重新扫描
            userInfo.Nodes.Clear();
            Thread thread = new Thread(new ThreadStart(ScanEntireNet));
            thread.Start();
        }
        /// <summary>
        /// 从当前列表中删除某客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientDeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除远程客户端？","提示！",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
                return;
            foreach (TreeNode tn in selectUser)
            {
                GlobalInfo.remoteIPList.Remove(tn.Name);
                userInfo.Nodes.Remove(tn);
            }
            selectUser.Clear();
            status.Text = "已连接" + GlobalInfo.remoteIPList.Count + "台客户端";
            //updateUserInfo(null);
        }
        /// <summary>
        /// 选择所有客户端，主要用于文件的群发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllClientBtn_Click(object sender, EventArgs e)
        {
            if (userInfo.Nodes.Count == 0)
                return;
            foreach (TreeNode tn in userInfo.Nodes)
            {
                if (tn.Checked == false)
                {
                    tn.Checked = true;
                }
            }
        }
        /// <summary>
        /// 新增文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.Filter = "所有文件(*.*)|*.*";
            ofd.Title = "请选择上传文件";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] pathList = ofd.FileNames;
                uploadInterface ui = new uploadFile();
                ui.setDes(GlobalInfo.localSpace);
                foreach (string path in pathList)
                {
                    ui.setSrc(path);
                    Thread upLoadThread = new Thread(new ParameterizedThreadStart(processUpload));
                    waitForm.waitForm wf = new waitForm.waitForm();
                    wf.setText("正在上传文件"+ui.getFileName()+"，请稍后...");
                    wf.setMonit(upLoadThread, ui);
                    wf.ShowDialog();
                }              
            }
        }
        /// 删除本地会议空间选择的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteLocalFileBtn_Click(object sender, EventArgs e)
        {
            //判断是否有被选中的
            if (localSelectedFile.Count == 0)
            {
                MessageBox.Show("请先选择文件！");
                return;
            }
            Thread deleteThread = new Thread(new ParameterizedThreadStart(processDelete));
            deleteThread.Start(localSelectedFile);                  
        }
        /// <summary>
        /// 处理删除操作
        /// </summary>
        /// <param name="obj"></param>
        private void processDelete(object obj)
        {
            List<TreeNode> list = (List<TreeNode>)obj;
            List<TreeNode> delList = new List<TreeNode>();
            object msg = "";
            foreach (TreeNode tn in list)
            {
                if (tn.Tag.ToString()=="0")//目录
                {
                    DirectoryInfo di = new DirectoryInfo(tn.Name);
                    try
                    {
                        di.Delete(true);
                        msg = "文件夹" + Path.GetFileName(tn.Name) + "删除成功！";
                    }
                    catch
                    {
                        msg = "文件夹" + Path.GetFileName(tn.Name) + "删除失败！";

                    }
                }
                else
                {
                    FileInfo fi = new FileInfo(tn.Name);
                    if (fi.Exists)
                    {
                        try
                        {
                            fi.Delete();
                            msg = "文件" + Path.GetFileName(tn.Name) + "删除成功！";
                        }
                        catch
                        {
                            msg = "文件" + Path.GetFileName(tn.Name) + "删除失败！";
                        }
                    }
                    else
                    {
                        msg = "文件" + Path.GetFileName(tn.Name) + "删除成功！";
                    }
                }
                updateStatus(msg);
                updateLogInfo(msg);
            }
            localSelectedFile.Clear();//clear
        }
        /// <summary>
        /// 获取某路径下所有文件个数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int getFilesCount(string path)
        {
            if (Directory.Exists(path))
            {
                int rel = 0;
                foreach (string x in Directory.GetFileSystemEntries(path))
                {
                    rel += getFilesCount(x);
                }
                return rel;
            }
            else
            {
                if (File.Exists(path))
                    return 1;
                else
                    return 0;
            }      
        }
        /// <summary>
        /// 文件分发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendFileBtn_Click(object sender, EventArgs e)
        {
            GlobalInfo.remoteIPList.Count();
            if (selectUser.Count == 0)
            {
                MessageBox.Show("请先选择分发客户端！");
                return;
            }
            if (localSelectedFile.Count == 0)
            {
                MessageBox.Show("请先选择要分发的文件！");
                return;
            }
            Thread countThread = new Thread(new ParameterizedThreadStart(processCount));
            countThread.Start(localSelectedFile);
            //初始化
            fileSendDic.Clear();
            foreach (TreeNode tn in selectUser)
            {
                fileSendDic.Add(tn.Name, 0);
            }
            sendCnt = 0;
            foreach (TreeNode tnFile in localSelectedFile)
            {
                foreach (TreeNode tnUser in selectUser)
                {
                    fileAndIp fi = new fileAndIp();
                    fi.fileName = tnFile;
                    fi.ip = IPAddress.Parse(tnUser.Name);                    
                    Thread thread = new Thread(new ParameterizedThreadStart(sendFileToClient));
                    thread.Start(fi);
                }
            }
        }
        /// <summary>
        /// 处理文件统计进程
        /// </summary>
        /// <param name="obj"></param>
        private void processCount(object obj)
        {
            List<TreeNode> list = (List<TreeNode>)obj;
            object msg = "正在统计待发送文件个数...";
            updateStatus(msg);
            updateLogInfo(msg);
            fileCount = 0;
            foreach (TreeNode tn in list)
            {
                if (tn.Tag.ToString() == "1")
                    fileCount++;
            }
            msg = "共" + fileCount + "个文件待发送...";          //当存在多个客户端时，文件的发送可能会乱，需要重新考虑下。
            updateLogInfo(msg);
            updateStatus(msg);
        }
        /// <summary>
        /// 将指定文件发送到某IP
        /// </summary>
        /// <param name="fi"></param>
        private void sendFileToClient(object fi)
        {
            fileAndIp fai = (fileAndIp)fi;
            TreeNode fileName = fai.fileName;
            IPAddress ip = fai.ip;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect((IPAddress)ip, GlobalInfo.Port);
                sendSelectFileByFileName(clientSocket, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nIP:" + ip.ToString());
            }
            finally
            {
                clientSocket.Close();
            }
        }

        /// <summary>
        /// 发送选中的文件
        /// </summary>
        /// <param name="clientSocket">选择的客户端</param>
        /// <param name="x">文件</param>
        private void sendSelectFileByFileName(Socket clientSocket,TreeNode x)
        {
            string ip = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
            string DirName = DataService.DataService.getDirNameByFullPath(GlobalInfo.localSpace,x.Name);//先获取中间部分
            if (x.Tag.ToString()=="0") //文件夹
            {
                createFolder(clientSocket, DirName + "\\" + x.Text);
            }
            else
            {
                sendCnt++;
                //文件：先建文件夹，再传数据
                if (DirName != "") //不为空，则创建，为空，说明是在根目录，不用新建了
                    createFolder(clientSocket, DirName);
                object msg = "正在向客户端 "+GlobalInfo.remoteIPList[ip] +"发送文件："+x.Text+"（当前发送第"+sendCnt+ "个，共"+fileCount+"个）";
                Console.WriteLine(fileSendDic.Count);
                updateStatus(msg);
                sendFileData(clientSocket, x.Name);
                //发送完毕+1
                fileSendDic[ip] = fileSendDic[ip] + 1;
                //所有发送完成
                if(fileSendDic[ip] ==fileCount)
                {
                    msg = "向客户端："+ GlobalInfo.remoteIPList[((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString()] + " 文件发送完毕！";
                    updateStatus(msg);
                    updateLogInfo(msg);
                }
            }
        }
        /// <summary>
        /// 更新状态栏
        /// </summary>
        /// <param name="msg"></param>
        private void updateStatus(object msg)
        {
            if (status.InvokeRequired)
            {
                status.BeginInvoke(new Action<object>(x =>
                {
                    status.Text = msg.ToString();
                }), msg);
            }
        }
        /// <summary>
        /// 发送文件数据
        /// </summary>
        /// <param name="connectSocket">建立连接socket</param>
        /// <param name="fileName">文件名</param>
        private void sendFileData(Socket connectSocket,string fileName)
        {
            string ip = ((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString();
            object msg = "向客户端：" + GlobalInfo.remoteIPList[ip] + "(" + ip + ")" + " 发送文件：" + Path.GetFileName(fileName) + "...";
            Console.WriteLine(msg.ToString());
            updateLogInfo(msg);
            updateStatus(msg);
            string basePath = GlobalInfo.localSpace;
            string transFileName = fileName;
            //判断是否需要进行水印处理
            if (GlobalInfo.waterMark)//兼容水印
            {
                DataService.DataService.deleteMark(fileName, out basePath, out transFileName);
            }
            //发送
            int rst=DataService.DataService.SendFile(connectSocket, basePath, transFileName,readValueFromConfigByNode("encrypt").ToString()=="True");
            if (rst == 0)
            {
                msg = "向客户端："+GlobalInfo.remoteIPList[ip]+"("+ip+")"+" 发送文件：" + Path.GetFileName(fileName) + " 成功！";
                Console.WriteLine(msg.ToString());
                updateLogInfo(msg);
                updateStatus(msg);
                //判断关键字是否在字典中，不存在，则添加
                if (hasSendFile.ContainsKey(ip))
                {
                    List<string> fileList = hasSendFile[ip];
                    if (!fileList.Contains(fileName))
                    {
                        fileList.Add(fileName);
                        hasSendFile[ip] = fileList;
                    }                  
                }
                else
                {
                    List<string> fileList = new List<string>();
                    fileList.Add(fileName);
                    hasSendFile.Add(ip, fileList);
                }
                //成功了，则从失败列表中删除
                if (sendFailFile.ContainsKey(ip))
                {
                    List<string> failList = sendFailFile[ip];
                    if (failList.Contains(fileName))
                    {
                        failList.Remove(fileName);
                    }
                    sendFailFile[ip] = failList;
                }
            }   
            else
            {
                if (sendFailFile.ContainsKey(ip))
                {
                    List<string> fileList = sendFailFile[ip];
                    if (!fileList.Contains(fileName))
                    {
                        fileList.Add(fileName);
                        sendFailFile[ip] = fileList;
                    }
                }
                else
                {
                    List<string> fileList = new List<string>();
                    fileList.Add(fileName);
                    sendFailFile.Add(ip, fileList);
                }
                //失败了，则从成功列表中删除
                if (hasSendFile.ContainsKey(ip))
                {
                    List<string> failList = hasSendFile[ip];
                    if (failList.Contains(fileName))
                    {
                        failList.Remove(fileName);
                    }
                    hasSendFile[ip] = failList;
                }
                msg = "向客户端：" + GlobalInfo.remoteIPList[ip] + "(" + ip + ")" + " 发送文件：" + Path.GetFileName(fileName) + " 失败！";
                Console.WriteLine(msg.ToString());
                updateLogInfo(msg);
                updateStatus(msg);
            }
        }
        /// <summary>
        /// 通过IP发送命令
        /// </summary>
        /// <param name="obj">命令结构体</param>
        private void sendCommandByIp(object obj)
        {
            cmdObj tmp = (cmdObj)obj;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string cmdStr = (string)tmp.cmd;
            if (tmp.dataList == null)
            {
                try
                {
                    clientSocket.Connect(IPAddress.Parse(tmp.ip), GlobalInfo.Port);
                    DataService.DataService.SendCommand(clientSocket, cmdStr);
                    Thread.Sleep(10);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

            }
            else
            {
                List<string> dataList = (List<string>)tmp.dataList;
                try
                {
                    clientSocket.Connect(IPAddress.Parse(tmp.ip), GlobalInfo.Port);
                    foreach (string x in dataList)
                    {
                        DataService.DataService.SendCommand(clientSocket, cmdStr, x);
                        Thread.Sleep(50);
                        string msg = "向客户端：" + tmp.ip + " 发送指令成功！";
                        updateLogInfo(msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="connectSocket"></param>
        /// <param name="foldName"></param>
        private void createFolder(Socket connectSocket, string foldName)
        {
            byte[] recv = new byte[1024];
            DataService.DataService.SendCommand(connectSocket, DataService.DataService.createFolder, foldName);
            Thread.Sleep(10);
            int recvNum = connectSocket.Receive(recv);
            if (recvNum > 0)
            {
                string response = DataService.DataService.ParseResponse(recv.Skip(0).Take(recvNum).ToArray());//获取返回指令
                if (response == DataService.DataService.executeCmdSucess)//指令执行正确
                {
                    Console.WriteLine("向客户端" + GlobalInfo.remoteIPList[((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString()] + "创建文件夹：" + foldName + "成功！");
                }
                else
                {
                    if (response == DataService.DataService.objectHasExist)
                    {
                        Console.WriteLine("客户端" + GlobalInfo.remoteIPList[((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString()] + "已存在相同文件夹！" );
                    }
                    else if (response == DataService.DataService.createError)
                    {
                        Console.WriteLine("客户端" + GlobalInfo.remoteIPList[((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString()] + "创建文件夹失败！");
                    }
                }
            }
            else
            {
                Console.WriteLine("未接受客户端返回数据！");
                
            }
        }
        /// <summary>
        /// 清空本地会议空间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trash_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清空会议空间吗？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            //先删除整个文件夹，并重新建即可
            Thread deleteThread = new Thread(new ThreadStart(deleteSpace));
            //deleteThread.Start();
            waitForm.waitForm wf = new waitForm.waitForm();
            wf.setText("正在安全清除会议空间，请稍后...");
            wf.setMonit(deleteThread);
            wf.ShowDialog();
            //并将选中的发送列表清除
            localSelectedFile.Clear();

                 
        }
        /// <summary>
        /// 删除会议空间
        /// </summary>
        private void deleteSpace()
        {
            DataService.SecurityDelete.DoSecurityDeleteFolder(GlobalInfo.localSpace, Int32.Parse(readValueFromConfigByNode("clearLevel").ToString()), false);
            object msg = "会议空间已安全清除！";
            updateStatus(msg);
            updateLogInfo(msg);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadFileBtn_Click(object sender, EventArgs e)
        {
            if (localSelectedFile.Count == 0)
            {
                MessageBox.Show("请先选择要下载的文件！");
                return;
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "请选择目标路径";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string desDirName = fbd.SelectedPath;
                uploadInterface ui;
                foreach (TreeNode tn in localSelectedFile)
                {
                    if (tn.Tag.ToString() == "1")
                    {
                        ui = new uploadFile();
                    }
                    else
                    {
                        ui = new uploadFolder();
                    }
                    ui.setSrc(tn.Name);
                    ui.setDes(fbd.SelectedPath);
                    ui.upload();
                }
                MessageBox.Show("文件下载完成！","提示！",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 删除远程会议空间选中的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteRemoteFileBtn_Click(object sender, EventArgs e)
        {
            if (selectUser.Count == 0)
            {
                MessageBox.Show("请先选择客户端！");
                return;
            }
            if (remoteSelectFile.Count == 0)
            {
                MessageBox.Show("请先选择要删除的远程文件！");
                return;
            }

            foreach (TreeNode fi in remoteSelectFile)
            {
                List<string> dataList = new List<string>();
                dataList.Add(DataService.DataService.getDirNameByFullPath(GlobalInfo.localSpace, fi.Name) + "\\" + Path.GetFileName(fi.Name));
                foreach (TreeNode tn in selectUser)
                {
                    cmdObj obj = new cmdObj();
                    string ip = tn.Name;
                    obj.ip = tn.Name;
                    obj.cmd = DataService.DataService.deleteFiles;
                    obj.dataList = dataList;
                    Thread thread = new Thread(new ParameterizedThreadStart(sendCommandByIp));
                    thread.Start(obj);
                    //清除已发列表
                    if (hasSendFile.Keys.Contains(ip) && hasSendFile[ip].Contains(fi.Name))
                    {
                        hasSendFile[ip].Remove(fi.Name);
                    }
                    if (sendFailFile.Keys.Contains(ip) && sendFailFile[ip].Contains(fi.Name))
                    {
                        sendFailFile[ip].Contains(fi.Name);
                    }
                    loadRemoteSpaceFile(ip);
                }
            }
            remoteSelectFile.Clear();//清除完以后把自己清空下
        }
        /// <summary>
        /// 打开选中文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileBtn_Click(object sender, EventArgs e)
        {
            if (selectUser.Count == 0)
            {
                MessageBox.Show("请先选择客户端！");
                return;
            }
            if (remoteSelectFile.Count == 0)
            {
                MessageBox.Show("请先选择要打开的文件！");
                return;
            }

            foreach (TreeNode x in remoteSelectFile)
            {
                List<string> dataList = new List<string>();
                dataList.Add(DataService.DataService.getDirNameByFullPath(GlobalInfo.localSpace, x.Name) + "\\" + Path.GetFileName(x.Name));
                foreach (TreeNode tn in selectUser)
                {
                    string ip = tn.Name;
                    cmdObj obj = new cmdObj();
                    obj.ip = ip;
                    obj.cmd = DataService.DataService.openFiles;
                    obj.dataList = dataList;
                    Thread thread = new Thread(new ParameterizedThreadStart(sendCommandByIp));
                    thread.Start(obj);
                }
            }

        }
        /// <summary>
        /// 结束会议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endMeeting_Click(object sender, EventArgs e)
        {
            string diag = "确定要结束会议并关闭远程终端吗?" + "\n" + "是：结束会议，并关闭远程终端" + "\n" + "否：结束会议，不关闭远程终端" + "\n" + "取消：取消退出会议操作";
            DialogResult dr = MessageBox.Show(diag, "提示！", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes|| dr==DialogResult.No)
            {
                //todo
                //1.记录日志
                OleDbConnection oc = GlobalInfo.GlobalConnection;
                oc.Open();
                string sql = "update meetingtable set endtime=@endtime where createtime=@createtime";
                OleDbCommand ocmd = new OleDbCommand(sql, oc);
                ocmd.Parameters.Add("endtime", OleDbType.Date);
                ocmd.Parameters.Add("createtime", OleDbType.Date);
                ocmd.Parameters["endtime"].Value = DateTime.Now;
                ocmd.Parameters["createtime"].Value=GlobalInfo.MeetingCreatTime;
                try
                {
                    ocmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新失败！" + ex.Message);
                }
                finally
                {
                    ocmd.Dispose();
                    oc.Close();
                }
                

                //2.发送清空客户端会议空间的指令，并关闭系统
                foreach (string ip in GlobalInfo.remoteIPList.Keys)
                {
                    cmdObj obj = new cmdObj();
                    obj.ip = ip;
                    if (dr == DialogResult.Yes)
                        obj.cmd = DataService.DataService.endMeetingAndPowerOff; //结束会议并关机
                    else
                        obj.cmd = DataService.DataService.endMeetingOnly;//只结束会议
                    obj.dataList = null;
                    Thread thread = new Thread(new ParameterizedThreadStart(sendCommandByIp));
                    thread.Start(obj);
                }
                //3.退出当前界面，回到主界面
                //Application.Exit();
            }
            else
            {
                return;
            }

        }
        /// <summary>
        /// 目录下新建文件时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localFoldWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //不用清空，先查看受影响的文件是哪个路径，根据路径去查找e.Name是否有匹配的，没有的话添加
            loadAllFile(e.FullPath,localSpace);

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localFoldWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //MessageBox.Show("test3");
            //string text = Path.GetFileName(e.FullPath);
            TreeNodeCollection tnc = localSpace.Nodes;
            TreeNode[] nodeList = tnc.Find(e.FullPath, true);
            foreach (TreeNode tn in nodeList)
            {
                if (tn.Name == e.FullPath)
                {
                    localSpace.Nodes.Remove(tn);
                }
            }
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="tv"></param>
        private void loadAllFile(string path, TreeView tv)
        {
            string text = Path.GetFileName(path);
            TreeNode[] nodeList = tv.Nodes.Find(text, true);
            foreach (TreeNode tn in nodeList)
            {
                if (tn.Name == path)//存在
                    return;
            }
            //没有返回说明不存在，则添加即可
            string parentPath = path.Substring(0, path.Length - text.Length - 1);
            nodeList = tv.Nodes.Find(parentPath,true);
            TreeNode parentNode = null;
            foreach (TreeNode tn in nodeList)
            {
                if (tn.Name == parentPath)
                {
                    parentNode = tn;
                    break;
                }
            }
            if (parentNode == null)
            {
                TreeNode tn = new TreeNode();
                tn.Name = path;
                tn.Text = text;
                if (Directory.Exists(path)) //是否是目录
                {
                    tn.ImageIndex = getIndexByFileExtention(".folder");
                    tn.SelectedImageIndex = tn.ImageIndex;
                    tn.Tag = 0; //目录
                }
                else
                {
                    //文件的话
                    tn.ImageIndex = getIndexByFileExtention(Path.GetExtension(path));
                    tn.SelectedImageIndex = tn.ImageIndex;
                    tn.Tag = 1; //文件
                }
                tv.Nodes.Add(tn);
                //tv.ExpandAll();
            }
            else
            {
                TreeNode tn = new TreeNode();
                tn.Name = path;
                tn.Text = text;
                if (Directory.Exists(path)) //是否是目录
                {
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                    tn.Tag = 0;
                }
                else
                {
                    //文件的话
                    tn.ImageIndex = getIndexByFileExtention(Path.GetExtension(path));
                    tn.SelectedImageIndex = tn.ImageIndex;
                    tn.Tag = 1;
                }
                parentNode.Nodes.Add(tn);
                //parentNode.ExpandAll();
            }
        }
        /// <summary>
        /// 根据文件后缀名获取图像index
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private int getIndexByFileExtention(string ext)
        {
            if (localImageList.Images.Keys.Contains(ext))
            {
                return localImageList.Images.IndexOfKey(ext);
            }
            return localImageList.Images.IndexOfKey(".unkown");
        }
        /// <summary>
        /// 双击节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.ImageIndex == 0)//说明是文件夹
            {
                if (e.Node.Nodes.Count != 0)
                {
                    return;
                }
                //读取该文件夹下的所有文件
                loadChildFileByNode(e.Node);
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = e.Node.Name;
                try
                {
                    process.Start();
                }
                catch
                {
                    MessageBox.Show("请先安装相应的打开软件！");
                }
            }
        }
        /// <summary>
        /// 根据节点值获取子目录
        /// </summary>
        /// <param name="node"></param>
        private void loadChildFileByNode(TreeNode node)
        {
            DirectoryInfo dir = new DirectoryInfo(node.Name);
            DirectoryInfo[] dirList = dir.GetDirectories();
            foreach (DirectoryInfo x in dirList)
            {
                TreeNode tn = new TreeNode();
                tn.Text = x.Name;
                tn.ImageIndex = 0;
                tn.SelectedImageIndex = 0;
                tn.Name = x.FullName;
                node.Nodes.Add(tn);
            }
            FileInfo[] fileList = dir.GetFiles();
            foreach (FileInfo x in fileList)
            {
                TreeNode tn = new TreeNode();
                tn.Text = x.Name;
                int imIndex = getIndexByFileExtention(x.Extension);
                tn.ImageIndex = imIndex;
                tn.SelectedImageIndex = imIndex;
                tn.Name = x.FullName;
                node.Nodes.Add(tn);
            }
            node.ExpandAll();
        }
        /// <summary>
        /// 根据路径查找节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private TreeNode FindNodeInTreeView(string path,TreeNode tn)
        {

            return null;
        }
        /// <summary>
        /// 名称发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null||e.Label=="")
            {
                MessageBox.Show("名称不能为空！");
                e.CancelEdit = true;
                return;
            }
            if (e.Label.Split('.')[0] == "")
            {
                MessageBox.Show("名称不能为空！");
                e.CancelEdit = true;
                return;
            }
            string oldText = Path.GetFileName(e.Node.Name);
            string dir = Path.GetDirectoryName(e.Node.Name);
            if (e.Label.Equals(oldText))//未发生变化
            {
                return;
            }
            string newPath = Path.Combine(dir, e.Label);
            FileInfo renameFile = new FileInfo(e.Node.Name);
            try
            {
                renameFile.MoveTo(newPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                e.CancelEdit = true;
                return;
            }
            e.Node.Name = newPath;
            if (File.Exists(e.Node.Name))
            {
                FileInfo fi = new FileInfo(e.Node.Name);
                e.Node.ImageIndex = getIndexByFileExtention(fi.Extension);
                e.Node.SelectedImageIndex = e.Node.ImageIndex;
            }
            if (Directory.Exists(e.Node.Name))
                updateFilePath(e.Node);
        }
        /// <summary>
        /// 根据给定的节点更新所有子节点的filepath
        /// </summary>
        /// <param name="tn"></param>
        private void updateFilePath(TreeNode tn)
        {
            if (tn.Nodes.Count == 0)
                return;
            foreach (TreeNode x in tn.Nodes)
            {
                if (Directory.Exists(x.Name))//目录，继续下一级
                {
                    updateFilePath(x);
                }
                else //文件的话直接改，用node.name替换以前的dir即可
                {
                    string newPath = Path.Combine(tn.Name, x.Text);
                    x.Name = newPath;
                }
            }
        }
        /// <summary>
        /// 节点被选中或取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                localSelectedFile.Add(e.Node);
            }
            else
            {
                localSelectedFile.Remove(e.Node);
            }
            if (e.Node.Nodes.Count > 0)
            {
                bool NoFalse = true;
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    if (tn.Checked == false)
                    {
                        NoFalse = false;
                    }
                }
                if (e.Node.Checked == true || NoFalse)
                {
                    foreach (TreeNode tn in e.Node.Nodes)
                    {
                        if (tn.Checked != e.Node.Checked)
                        {
                            tn.Checked = e.Node.Checked;
                        }
                    }
                }
            }
            if (e.Node.Parent != null && e.Node.Parent is TreeNode)
            {
                bool ParentNode = true;
                foreach (TreeNode tn in e.Node.Parent.Nodes)
                {
                    if (tn.Checked == false)
                    {
                        ParentNode = false;
                    }
                }
                if (e.Node.Parent.Checked != ParentNode && (e.Node.Checked == false || e.Node.Checked == true && e.Node.Parent.Checked == false))
                {
                    e.Node.Parent.Checked = ParentNode;
                }
            }  
        }
        /// <summary>
        /// 同步子节点
        /// </summary>
        /// <param name="pNode"></param>
        /// <param name="checkedState"></param>
        private void synChildNode(TreeNode pNode, Boolean checkedState)
        {
            if (pNode == null)
                return;
            foreach (TreeNode tn in pNode.Nodes)
            {
                tn.Checked = checkedState;
                synChildNode(tn, checkedState);
            }
        }
        /// <summary>
        /// 上传文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFolderBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请选择上传的文件夹";
            fbd.ShowNewFolderButton = true;            
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                uploadInterface up = new uploadFolder();
                up.setSrc(fbd.SelectedPath);
                up.setDes(GlobalInfo.localSpace);
                try
                {
                    Thread addFolderThread = new Thread(new ParameterizedThreadStart(processUpload));
                    waitForm.waitForm wf = new waitForm.waitForm();
                    wf.setText("正在上传文件夹，请稍后...");
                    wf.setMonit(addFolderThread, up);
                    wf.ShowDialog();
                }
                catch (Exception ex)
                {
                    object msg = ex.Message + "\n" + "文件夹：" + Path.GetFileName(fbd.SelectedPath) + "上传失败！";
                    updateLogInfo(msg);
                    updateStatus(msg);
                    MessageBox.Show(msg.ToString(),"警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
        }
        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_DragEnter(object sender, DragEventArgs e)
        {
            //MessageBox.Show("test1");
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        /// <summary>
        /// 将文件拖至空间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop);
            uploadInterface ui;
            foreach (string str in pathList)
            {
                if (File.Exists(str))
                {
                    ui = new uploadFile();
                }
                else
                {
                    ui = new uploadFolder();
                }
                ui.setSrc(str);
                ui.setDes(GlobalInfo.localSpace);
                Thread upLoadThread = new Thread(new ParameterizedThreadStart(processUpload));
                waitForm.waitForm wf = new waitForm.waitForm();
                wf.setText("正在上传文件夹，请稍后...");
                wf.setMonit(upLoadThread, ui);
                wf.ShowDialog();
            }
        }
        /// <summary>
        /// 处理上传操作
        /// </summary>
        /// <param name="ui"></param>
        private void processUpload(object ui)
        {
            uploadInterface up = (uploadInterface)ui;
            try
            {
                int rst=up.upload();
                object msg = ((up.getTag() == 0) ? "文件夹" : "文件") + up.getFileName() + "上传"+((rst==0)?"成功！":"失败！");
                updateStatus(msg);
                updateLogInfo(msg);
            }
            catch (Exception ex)
            {
                object msg = (up.getTag() == 0) ? "文件夹" : "文件" + up.getFileName() + "上传失败！";
                updateStatus(msg);
                updateLogInfo(msg+ex.Message);
            }

        }
        /// <summary>
        /// 选中所有文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllFileBtn_Click(object sender, EventArgs e)
        {
            if (localSpace.Nodes.Count == 0)
                return;
            foreach (TreeNode tn in localSpace.Nodes)
            {
                if(tn.Checked==false)
                   tn.Checked = true;
            }
        }
        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetAllFileBtn_Click(object sender, EventArgs e)
        {
            if (localSpace.Nodes.Count == 0)
                return;
            selectAllFileBtn_Click(null, null);//先全选以下，再释放
            foreach (TreeNode tn in localSpace.Nodes)
            {
                if(tn.Checked)
                  tn.Checked = false;
            }
        }
        /// <summary>
        /// 被选中后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userInfo_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                selectUser.Add(e.Node);
            }
            else
            {
                selectUser.Remove(e.Node);
            }
        }
        /// <summary>
        /// 取消人员全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetAllPerson_Click(object sender, EventArgs e)
        {
            foreach (TreeNode tn in userInfo.Nodes)
            {
                if (tn.Checked)
                {
                    tn.Checked = false;
                }
            }
            selectUser.Clear();//再清除一次
        }
        /// <summary>
        /// 当鼠标移动到的效果展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endMeeting_MouseEnter(object sender, EventArgs e)
        {
            //修改效果
            //endMeeting.BorderStyle = BorderStyle.Fixed3D;
            endMeeting.BackColor = System.Drawing.SystemColors.ControlDark;
            endMeeting.Refresh();
        }
        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endMeeting_MouseLeave(object sender, EventArgs e)
        {
            //修改效果
            //endMeeting.BorderStyle = BorderStyle.None;
            endMeeting.BackColor= System.Drawing.SystemColors.Control;
            endMeeting.Refresh();
        }
        /// <summary>
        /// 点击某节点时，显示该节点的已发送列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userInfo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //获取该节点的ip地址
            string ip = e.Node.Name;
            loadRemoteSpaceFile(ip);
        }
        /// <summary>
        /// 加载远程空间
        /// </summary>
        /// <param name="ip"></param>
        private void loadRemoteSpaceFile(string ip)
        {
            //获取该节点的ip地址
            remoteSpace.Nodes.Clear();
            //string ip = e.Node.Name;
            if (hasSendFile.ContainsKey(ip))//成功存在
            {
                foreach (string fileName in hasSendFile[ip])
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = fileName;
                    tn.Text = "..\\" + DataService.DataService.getDirNameByFullPath(GlobalInfo.localSpace, fileName) + "\\" + Path.GetFileName(fileName);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                    remoteSpace.Nodes.Add(tn);
                }
            }
            if (sendFailFile.ContainsKey(ip))
            {
                foreach (string fileName in sendFailFile[ip])
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = fileName;
                    tn.Text = "..\\" + DataService.DataService.getDirNameByFullPath(GlobalInfo.localSpace, fileName) + "\\" + Path.GetFileName(fileName);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                    remoteSpace.Nodes.Add(tn);
                }
            }
        }
        /// <summary>
        /// 选中文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteSpace_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                if(!remoteSelectFile.Contains(e.Node))
                    remoteSelectFile.Add(e.Node);
            }
            else
            {
                remoteSelectFile.Remove(e.Node);
            }
        }
        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reverseSelectBtn_Click(object sender, EventArgs e)
        {
            foreach (TreeNode tn in userInfo.Nodes)
            {
                if (tn.Checked)
                {
                    tn.Checked = false;
                }
                else
                {
                    tn.Checked = true;
                }
            }
        }
        /// <summary>
        /// 清除日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearInfo_Click(object sender, EventArgs e)
        {
            logInfo.Clear();
        }
        /// <summary>
        /// 客户端重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定重启远程客户端？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            if (selectUser.Count == 0)
            {
                MessageBox.Show("请选择欲重启的客户端！");
                return;
            }
            foreach (TreeNode tn in selectUser)
            {
                string ip = tn.Name;
                cmdObj cmd = new cmdObj();
                cmd.dataList = null;
                cmd.ip = ip;
                cmd.cmd = DataService.DataService.reboot;
                Thread thread = new Thread(new ParameterizedThreadStart(sendCommandByIp));
                thread.Start(cmd);
            }
        }
        /// <summary>
        /// 日志开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logSwitch_Click(object sender, EventArgs e)
        {
            if (DisplayLogSwitch.ToolTipText=="关闭日志显示")
            {
                DisplayLogSwitch.ToolTipText = "打开日志显示";
                DisplayLogSwitch.Image = switchImage.Images[0];
                logInfo.Clear();
                logInfo.Enabled = false;
            }
            else
            {
                DisplayLogSwitch.ToolTipText = "关闭日志显示";
                DisplayLogSwitch.Image = switchImage.Images[1];
                logInfo.Enabled = true;
            }
        }
    }
}
