using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataService;
using System.Diagnostics;
using System.Xml;
using waitForm;

namespace MeetingSystemClient
{
    public partial class MainForm : Form
    {
        private List<string> pullBackFile = new List<string>();//存放回传文件列表，全路径
        private string currentFold = "";//当前路径
        /*以下三个变量通过初次交互后从服务端获取*/
        private int deleteNumb = 1;//默认删除三次
        private bool waterMark = false;//默认不兼容水印
        private bool encrypt = false;//加密传输

        List<FileSystemWatcher> watchers; //定义一个监控集合
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 点击图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //左键点击时，显示窗口,右键显示“退出，关于”，已属性设置
            if (e.Button == MouseButtons.Left)
            {
                if (WindowState != FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Minimized;
                    notifyIcon1.ShowBalloonTip(100);
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }
        }
        /// <summary>
        /// 退出客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出系统?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //TODO 
                //清理会议空间

                this.Close();
            }
            else
                return;
        }
        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("-软件名称："+this.Text+"\n"+"-软件版本："+GlobalInfo.softVersion.ToString()+"\n"+"-研发单位："+GlobalInfo.softMaker+"\n"+"-使用单位："+GlobalInfo.softUser);
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            loadImageList(smallImageList, Application.StartupPath + @"\config\smallIcon.xml");
            loadImageList(largeImageList, Application.StartupPath + @"\config\largeIcon.xml");
            #region 初始化会议空间
            if (Directory.Exists(Application.StartupPath + @"\MeetingFold"))
            {
                //存在的话先删除
                Directory.Delete(Application.StartupPath + @"\MeetingFold", true);
            }
            try
            {
                Directory.CreateDirectory(Application.StartupPath + @"\MeetingFold");
                GlobalInfo.MeetingFold = Application.StartupPath + @"\MeetingFold";
                //并创建2个文件：1.raw文件，记录原始日期；2.log文件，记录文件是否存在其他地方
                CreateLogFile(GlobalInfo.MeetingFold + @"\raw.txt");
                CreateLogFile(GlobalInfo.MeetingFold + @"\log.txt");
                clientSpaceWatcher.Path = GlobalInfo.MeetingFold;
            }
            catch (Exception ex)
            {
                MessageBox.Show("会议空间创建失败！" + "\n" + "原因：" + ex.Message);
                return;
            }
            #endregion
            #region 监听端口并响应
            //1.先获取本机基本信息
            GetLocalBaseInfo();
            //2.创建socket，监听
            IPAddress ip = IPAddress.Parse(GlobalInfo.localIP);
            GlobalInfo.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            GlobalInfo.serverSocket.Bind(new IPEndPoint(ip, GlobalInfo.Port));//对该socket绑定IP和端口
            GlobalInfo.serverSocket.Listen(1);//只监听1个连接
            Console.WriteLine("start listenning {0} success!", GlobalInfo.serverSocket.LocalEndPoint.ToString());
            Thread myThread = new Thread(ListenClientConnect); //启动一个线程处理端口的监听事件，防止阻塞主界面
            myThread.Start();
            //Console.ReadLine();
            #endregion
        }
        /// <summary>
        /// 加载图标集合
        /// </summary>
        private void loadImageList(ImageList imageList, string filePath)
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
                    imageList.Images.Add(xn.Attributes["name"].Value, image);
                }
            }
        }
        /// <summary>
        /// 创建日志文件
        /// </summary>
        private void CreateLogFile(string fileName)
        {
            File.WriteAllText(fileName, null);
            File.SetAttributes(fileName, FileAttributes.Hidden);
        }
        /// <summary>
        /// 文件系统监控初始化
        /// </summary>
        private void FileSystemListenInit()
        {
            watchers = DataService.FileSystemControll.Watchers;//获取监视器
            //为每个监视器添加监控事件
            foreach (FileSystemWatcher fsw in watchers)
            {
                fsw.IncludeSubdirectories = true;
                fsw.Filter = "*.*";
                //fsw.EnableRaisingEvents = true;
                fsw.Changed += Fsw_Changed;
                fsw.Created += Fsw_Changed;
                fsw.Deleted += Fsw_Changed;

            }
        }
        /// <summary>
        /// 启动对整个文件系统的监控
        /// </summary>
        private void StartListenFileSystem()
        {
            foreach (FileSystemWatcher fsw in watchers)
            {
                fsw.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// 文件系统变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
            string typeInfo = e.ChangeType.ToString();
            //只有当文件满足过滤条件且存在于其他非会议空间的情况下，记录；在会议空间的情况下，肯定会被清除
            if(FilterPath(e.FullPath)&&!e.FullPath.Contains(GlobalInfo.MeetingFold))
            {
                string msg = typeInfo + ":" + e.FullPath;
                string fileName = GlobalInfo.MeetingFold + @"\log.txt";
                if (!File.Exists(fileName))
                    CreateLogFile(fileName);
                Console.WriteLine(msg);
                DataService.LogManager.logInfo(fileName, msg);
            }
                
        }
        /// <summary>
        /// 信息过滤，去除掉文件目录以及非doc、docx、xls、xlsx、pdf、jpg等情况
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool FilterPath(string path)
        {
            if (Directory.Exists(path))
            {
                return false;
            }
            else
            {
                //获取后缀
                string ext = Path.GetExtension(path);
                if (!GlobalInfo.FilterList.Contains(ext))
                {
                    return false;
                }
                return true;
            }
            
        }
        /// <summary>
        /// 结束对整个文件系统的监控
        /// </summary>
        private void EndListenFileSystem()
        {
            foreach (FileSystemWatcher fsw in watchers)
            {
                fsw.EnableRaisingEvents = false;
            }
        }
        /// <summary>
        /// 获取本机基本信息
        /// </summary>
        private void GetLocalBaseInfo()
        {
            GlobalInfo.localIP = GetSystemInfo.getIP();
            GlobalInfo.localUserName = GetSystemInfo.getUserName();
        }
        /// <summary>
        /// 监听客户端的连接
        /// </summary>
        private  void ListenClientConnect()
        {
            while (true)
            {
                //Console.WriteLine(i+++"");
                Socket clientSocket = GlobalInfo.serverSocket.Accept(); //接受到一个connet             
                GlobalInfo.serverIP = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();//获取远程地址
                Console.WriteLine("ServerIP:"+GlobalInfo.serverIP);
                //clientSocket.Send(Encoding.ASCII.GetBytes(GlobalInfo.localUserName));//发送本机用户名至服务端软件
                Thread receiveThread = new Thread(new ParameterizedThreadStart(ReceiveMessage));
                receiveThread.Start(clientSocket);
                //clientSocket.Close();
            }
        }
        /// <summary>
        /// 唤醒客户端界面
        /// </summary>
        private void WakeUpCLient()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(x =>
                {
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        return;
                    }
                }),0);

            }
        }
        /// <summary>
        /// 接收服务端软件发送的信息
        /// </summary>
        private  void ReceiveMessage(object clientSocket)
        {
            byte[] recv = new byte[DataService.DataService.Buff_Size+16];
            Socket myClientSocket = (Socket)clientSocket;
            long fileSize = 0;
            long realFileSize = 0;
            string fileName = "";
            int hasReceBytes = 0;//已接收的数据字节
            byte[] fileData=new byte[0];
            long encryLength = 0L;
            while (true)
            {
                try
                {
                    int receiveNumber = myClientSocket.Receive(recv);
                    if (receiveNumber > 0)
                    {
                        Console.WriteLine("已接受客户端: {0}数据，长度为: {1}", myClientSocket.RemoteEndPoint.ToString(),receiveNumber.ToString());
                        if (Encoding.Unicode.GetString(recv, 0, DataService.DataService.HeadLength) == DataService.DataService.cmdHeadStr) //指令头
                        {
                            byte[] recvByte = new byte[receiveNumber];
                            recvByte = recv.Skip(0).Take(receiveNumber).ToArray();
                            ProcessCmd(myClientSocket, recvByte);
                        }
                        else if (Encoding.Unicode.GetString(recv, 0, DataService.DataService.HeadLength) == DataService.DataService.dataHeadStr)//数据头
                        {
                            //接受数据,解析数据文件大小和文件名
                            // fileSize = BitConverter.ToInt32(recv, DataService.DataService.HeadLength);//获取4节点文件大小
                            fileSize = BitConverter.ToInt64(recv, DataService.DataService.HeadLength);//获取8字节文件大小
                            realFileSize = fileSize;
                            fileName = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + 8, receiveNumber - DataService.DataService.HeadLength - 8);
                            Console.WriteLine("检测到文件数据：文件名为："+fileName+"文件大小为："+fileSize);
                            //如果文件大小为0，则下次不会接收到数据，因此需要在此处立即建立空文件即可
                            DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvFileHeadSuccess);//答复
                            hasReceBytes = 0;//将已接收数据量清0

                            #region 发送了一个空文件
                            if (fileSize == 0)
                            {
                                // File.WriteAllBytes(GlobalInfo.MeetingFold + "\\" + fileName, DataService.SecurityTransmit.Decoding(fileData));
                                File.WriteAllBytes(GlobalInfo.MeetingFold + "\\" + fileName, fileData);
                                if (File.Exists(GlobalInfo.MeetingFold + "\\" + fileName))
                                {
                                    Console.WriteLine("创建文件成功！");
                                    //获取文件时间
                                    string fileCreateTime = File.GetCreationTime(GlobalInfo.MeetingFold + "\\" + fileName).ToString();
                                    string msg = GlobalInfo.MeetingFold + "\\" + fileName.Substring(1) + " " + fileCreateTime + " " + fileSize;
                                    string path = GlobalInfo.MeetingFold + "\\" + "raw.txt";
                                    if (!File.Exists(path))
                                        CreateLogFile(path);
                                    DataService.LogManager.logInfo(path, msg);
                                    DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvAllFileSuccess);
                                }
                                else
                                {
                                    Console.WriteLine("创建文件失败！");
                                    DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvFileHeadFailed);
                                }
                            }
                            else
                            {
                                //需要根据实际接收的文件大小进行判断 ,加密情况
                                if (encrypt)
                                {
                                    if (fileSize % DataService.DataService.Buff_Size == 0)
                                    {
                                        encryLength = fileSize + (fileSize / DataService.DataService.Buff_Size) * 16;
                                    }
                                    else
                                    {
                                        encryLength = fileSize + (fileSize / DataService.DataService.Buff_Size) * 16 + (16 - (fileSize % DataService.DataService.Buff_Size)%16);
                                    }
                                    fileSize = encryLength;
                                } 
                                fileData = new byte[fileSize];
                            }
                            #endregion
                        }
                        else //其余则为其他数据
                        {
                            Console.WriteLine("接收到新数据，当前数据量为:"+hasReceBytes+" byte");
                            if (hasReceBytes < fileSize)
                            {
                                byte[] decodeData = new byte[receiveNumber];
                                Array.Copy(recv, decodeData, receiveNumber);
                                decodeData.CopyTo(fileData, hasReceBytes);
                                hasReceBytes += decodeData.Length;                              
                            }
                            //文件接收完毕
                            if(hasReceBytes>=fileSize)
                            { 
                                //保存文件
                                try
                                {
                                    //此处判断下是否存在加密的情况
                                    if (encrypt)
                                    {
                                        byte[] realFile = new byte[realFileSize];
                                        int i = 0;
                                        int times = (int)(fileSize / (DataService.DataService.Buff_Size + 16));
                                        Console.WriteLine("times:"+times);
                                        for (i = 0; i < times; i++)
                                        {
                                            Console.WriteLine("当前i:"+i);
                                            byte[] encryData = new byte[DataService.DataService.Buff_Size+16];
                                            Array.Copy(fileData, encryData, encryData.Length);
                                            Console.WriteLine("encryData length:"+encryData.Length);
                                            byte[] test = SecurityTransmit.Decoding(encryData);
                                            Console.WriteLine("test length:"+test.Length);
                                            try
                                            {
                                                test.CopyTo(realFile, DataService.DataService.Buff_Size * i);
                                            }
                                            catch(Exception ex)
                                            {
                                                Console.WriteLine(ex.Message.ToString());
                                            }
                                            
                                        }
                                        //处理剩下部分
                                        SecurityTransmit.Decoding(fileData.Skip(i * (DataService.DataService.Buff_Size+16)).ToArray()).CopyTo(realFile, DataService.DataService.Buff_Size * i);
                                        File.WriteAllBytes(GlobalInfo.MeetingFold + "\\" + fileName, fileData);
                                    }
                                    else
                                    {
                                        File.WriteAllBytes(GlobalInfo.MeetingFold + "\\" + fileName, fileData);
                                    }
                                        
                                    if (File.Exists(GlobalInfo.MeetingFold + "\\" + fileName))
                                    {
                                        Console.WriteLine("创建文件成功！");                                        
                                        string fileCreateTime = File.GetCreationTime(GlobalInfo.MeetingFold + "\\" + fileName).ToString();
                                        string msg = GlobalInfo.MeetingFold + "\\" + fileName.Substring(1) + " " + fileCreateTime+" "+fileSize;
                                        string path = GlobalInfo.MeetingFold + "\\" + "raw.txt";
                                        if (!File.Exists(path))
                                            CreateLogFile(path);
                                        DataService.LogManager.logInfo(path, msg);
                                        DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvAllFileSuccess);
                                    }
                                    else
                                    {
                                        Console.WriteLine("创建文件失败！");
                                        DataService.DataService.SendCommand(myClientSocket, DataService.DataService.recvFileHeadFailed);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("此处捕获文件接收完毕时的异常情况:"+ex.Message);
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
                    Console.WriteLine("此处捕获所有接收数据部分的异常情况 :"+ex.Message);
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
        private void ProcessCmd(Socket connectSocket,byte[] recv)
        {
            string cmd = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength, DataService.DataService.cmdLength);
            switch (cmd)
            {
                case DataService.DataService.getUserInfo:
                    connectSocket.Send(Encoding.Unicode.GetBytes(GlobalInfo.localUserName));break;//发送本机用户名至服务端软件
                case DataService.DataService.wakeUpClient:
                    WakeUpCLient();break;  //唤醒客户端
                case DataService.DataService.recvMeetingInfo:   //接受会议基本信息
                    {
                        string info = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, recv.Length - DataService.DataService.HeadLength - DataService.DataService.cmdLength);
                        Thread thread = new Thread(new ParameterizedThreadStart(updateMeetingInfo));
                        thread.Start(info);
                        #region 启动对整个磁盘的监听
                        FileSystemListenInit();//初始化
                        StartListenFileSystem();//启动监听
                        #endregion
                    }
                    break;
                case DataService.DataService.reboot:
                    ControlPC.Reboot(); break;//电脑重启
                case DataService.DataService.createFolder: //创建文件夹
                    {
                        string folderName= Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, recv.Length - DataService.DataService.HeadLength - DataService.DataService.cmdLength);
                        if (Directory.Exists(GlobalInfo.MeetingFold + @"\" + folderName))
                        {
                            DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdFailed, DataService.DataService.objectHasExist);
                            return;
                        }
                        try
                        {
                            Directory.CreateDirectory(GlobalInfo.MeetingFold + @"\" + folderName);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("创建文件夹"+folderName+"失败！"+"\n原因："+ex.Message);
                            DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdFailed, DataService.DataService.createError);
                            return;
                        }
                        DataService.DataService.SendCommand(connectSocket, DataService.DataService.executeCmdSucess);
                    }
                    break;
                case DataService.DataService.endMeetingAndPowerOff:
                    {
                        clearMeetingSpace();
                        ControlPC.PowerOff();
                    } break;//结束会议，关闭 计算机
                case DataService.DataService.endMeetingOnly:
                    {
                        clearMeetingSpace();
                    }
                    break;//只结束会议
                case DataService.DataService.deleteFiles:   //删除文件
                    {
                        string fileName = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, recv.Length - DataService.DataService.HeadLength - DataService.DataService.cmdLength);
                        Console.WriteLine("接收到删除文件指令，删除文件名为："+fileName);
                        fileName = GlobalInfo.MeetingFold + "\\" + fileName;//最终的文件名
                        DataService.SecurityDelete.DoSecurityDeleteFile(fileName,deleteNumb);
                    }
                    break;
                case DataService.DataService.openFiles://打开文件
                    {
                        string fileName = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, recv.Length - DataService.DataService.HeadLength - DataService.DataService.cmdLength);
                        Console.WriteLine("接收到删除文件指令，删除文件名为：" + fileName);
                        fileName = GlobalInfo.MeetingFold + "\\" + fileName;//最终的文件名
                        Process process = new Process();
                        process.StartInfo.FileName = fileName;
                        try
                        {
                            process.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;
                default:break;
            }
        }
        /// <summary>
        /// 填充会议基本信息
        /// </summary>
        /// <param name="info"></param>
        private void updateMeetingInfo(object info)
        {
            string[] meetingInfo = ((string)info).Split('\\');
            if (meetingTopic.InvokeRequired)
            {
                meetingTopic.BeginInvoke(new Action<string>(x =>
                {
                    meetingTopic.Text = x;
                }), meetingInfo[0]);
            }
            if (meetingDepart.InvokeRequired)
            {
                meetingDepart.BeginInvoke(new Action<string>(x =>
                {
                    meetingDepart.Text = x;
                }), meetingInfo[1]);
            }
            if (creater.InvokeRequired)
            {
                creater.BeginInvoke(new Action<string>(x =>
                {
                    creater.Text = x;
                }), meetingInfo[2]);
            }
            deleteNumb = Int32.Parse(meetingInfo[3]);//获取配置的删除次数
            waterMark = meetingInfo[4] == "True";//获取是否兼容水印
            encrypt = meetingInfo[5] == "True";//获取是否加密传输
           // Console.WriteLine(deleteNumb+waterMark.ToString()+encrypt.ToString());
        }
        /// <summary>
        /// 退出会议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitSystemBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要退出会议吗？", "提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                //关闭会议监控
                EndListenFileSystem();
                //清除其余的临时文件
                clearOtherTempFile();
                //清除会议
                clearMeetingSpace();
                //并告知主控退出了会议
                sendExitMsgToHost();
                //this.Close();
                Application.Exit();
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 发送退出会议指令至主机
        /// </summary>
        private void sendExitMsgToHost()
        {
            Socket connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建socket
            try
            {
                connectSocket.Connect(IPAddress.Parse(GlobalInfo.serverIP), GlobalInfo.Port1);
                //连接成功，则发送数据
                DataService.DataService.SendCommand(connectSocket, DataService.DataService.ExitMeeting);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        /// <summary>
        /// 上传已选中文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadBtn_Click(object sender, EventArgs e)
        {
            //收集会议空间中变化的文件
            collectChangedFile(GlobalInfo.MeetingFold);
            sendBackFile();
            MessageBox.Show("回传完毕！");
        }
        /// <summary>
        /// 收集变更过的文件信息
        /// </summary>
        private void collectChangedFile(string path)
        {
           // pullBackFile.Add(GlobalInfo.MeetingFold + "\\12345");
            //查看当前会议空间中所有的文件，与原始raw中的文件大小先以及最后的写入日期进行比较，如果没什么变化，就可以排除
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach (FileSystemInfo fs in fsi)
            {
                if (fs is DirectoryInfo)
                {
                    collectChangedFile(fs.FullName);//递归
                }
                else
                {
                    if (new FileInfo(fs.FullName).Attributes.HasFlag(FileAttributes.Hidden)&&((fs.FullName==GlobalInfo.MeetingFold+"\\log.txt")||(fs.FullName==GlobalInfo.MeetingFold+"\\raw.txt")))
                    {
                        continue;//如果文件是隐藏的，且为log文件或者是raw文件
                    }
                    FileStream myFs = new FileStream(GlobalInfo.MeetingFold + "\\raw.txt", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(myFs);
                    string tmp = null;
                    //如果是文件的话，先获取文件的修改时间，如果和创建时间一致，可放弃，如果不一致，看文件大小，大小一样，放弃，不一样，则记录
                    string changeTime = fs.LastWriteTime.ToString();//获取最后的写入时间
                    int fileSize = (int)new FileInfo(fs.FullName).Length;
                    bool flag = false;
                    while ((tmp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(tmp);
                        if (tmp.Split(' ')[0] == fs.FullName)//存在相同的
                        {
                            flag = true;
                            if (tmp.Split(' ')[1] == changeTime)
                            {
                                break;
                            }
                            else
                            {
                                if (fileSize == Int32.Parse(tmp.Split(' ')[3]))//跳过时分秒
                                {
                                    break;
                                }
                                else
                                {
                                    pullBackFile.Add(fs.FullName);
                                    break;
                                }
                            }
                        }
                    }
                    sr.Close();
                    myFs.Close();
                
                    if (!flag)//一个也没有，说明是自己新加的
                    {
                        pullBackFile.Add(fs.FullName);
                    }
   
                }
            }
        }
        /// <summary>
        /// 回传数据
        /// </summary>
        private void sendBackFile()
        {
            //文件回传
            Socket connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建socket
            try
            {
                connectSocket.Connect(IPAddress.Parse(GlobalInfo.serverIP), GlobalInfo.Port1);
                //连接成功，则发送数据
                foreach (string path in pullBackFile)
                {
                    SendSelectFileByFileName(connectSocket, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        /// <summary>
        /// 发送选中的文件
        /// </summary>
        /// <param name="fileName"></param>
        private void SendSelectFileByFileName(Socket clientSocket, string fileName)
        {
            string basePath = GlobalInfo.MeetingFold;
            string transFileName = fileName;
            string DirName = DataService.DataService.getDirNameByFullPath(GlobalInfo.MeetingFold, fileName);//先获取中间部分
            //文件：先建文件夹，再传数据
            if (DirName != "") //不为空，则创建，为空，说明是在根目录，不用新建了
                CreateFolder(clientSocket, DirName);
            if (waterMark) //兼容水印处理
            {
                try
                {
                    //现将该文件copy至临时目录，然后进行水印处理，同时更新basepath、transfilename
                    string destFileName = Path.GetTempPath() + Path.GetFileName(fileName);
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }
                    File.Copy(fileName, destFileName);
                    File.Move(destFileName, destFileName);
                    //先加密处理

                    basePath = Path.GetTempPath().Substring(0, Path.GetTempPath().Length - 1);
                    transFileName = destFileName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (DataService.DataService.SendFile(clientSocket, basePath, transFileName, encrypt) == 0)
            {
                Console.WriteLine("文件：" + fileName + "发送成功！");
            }
            else
            {
                Console.WriteLine("文件：" + fileName + "发送失败！");
            }
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="connectSocket"></param>
        /// <param name="foldName"></param>
        private void CreateFolder(Socket connectSocket, string foldName)
        {
            byte[] recv = new byte[1024];
            DataService.DataService.SendCommand(connectSocket, DataService.DataService.createFolder, foldName);
            int recvNum = connectSocket.Receive(recv);
            if (recvNum > 0)
            {
                string response = ParseResponse(recv.Skip(0).Take(recvNum).ToArray());
                if (response == DataService.DataService.executeCmdSucess)//指令执行正确
                {
                    Console.WriteLine("向服务端" + GlobalInfo.serverIP + "创建文件夹：" + foldName + "成功！");
                }
                else
                {
                    if (response == DataService.DataService.objectHasExist)
                    {
                        Console.WriteLine("服务端" + GlobalInfo.serverIP + "已存在相同文件夹！");
                    }
                    else if (response == DataService.DataService.createError)
                    {
                        Console.WriteLine("服务端" + GlobalInfo.serverIP + "创建文件夹失败！");
                    }
                }
            }
            else
            {
                Console.WriteLine("未接受到数据！");
            }
        }
        /// <summary>
        /// 解析指令获取返回的响应
        /// </summary>
        /// <param name="recv"></param>
        /// <returns></returns>
        private string ParseResponse(byte[] recv)
        {
            string resp = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength, DataService.DataService.cmdLength);//先获取指令字
            if (resp == DataService.DataService.executeCmdSucess)//成功
                return resp;
            if (resp == DataService.DataService.executeCmdFailed)//失败
            {
                resp = Encoding.Unicode.GetString(recv, DataService.DataService.HeadLength + DataService.DataService.cmdLength, DataService.DataService.cmdLength);
                return resp;
            }

            return null;
        }
        /// <summary>
        /// 手动清除会议空间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLocalSpaceBtn_Click(object sender, EventArgs e)
        {
            string path = GlobalInfo.MeetingFold + @"\raw.txt";
            //文件不在，说明已被清除了一次
            if (!File.Exists(path))
            {
                MessageBox.Show("会议空间无东西可清除！");
                return;
            }
            if (MessageBox.Show("此操作将不可被恢复，确定要清空会议空间吗？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                EndListenFileSystem();
                clearMeetingSpace();
                StartListenFileSystem();
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 清除会议空间
        /// </summary>
        private void clearMeetingSpace()
        {
            Thread deleteThread = new Thread(new ThreadStart(processDelete));
            waitForm.waitForm wf = new waitForm.waitForm();
            wf.setText("正在清除...");
            wf.setMonit(deleteThread);
            wf.ShowDialog();
            //清空树节点和listview
            localSpace.Nodes.Clear();
            detailListView.Items.Clear();
        }
        /// <summary>
        /// 删除处理
        /// </summary>
        private void processDelete()
        {
            //再清除整个会议空间
            SecurityDelete.DoSecurityDeleteFolder(GlobalInfo.MeetingFold, deleteNumb, false);
        }
        /// <summary>
        /// 清除其余的临时文件
        /// </summary>
        private void clearOtherTempFile()
        {
            List<string> deleteFile = new List<string>();//存放需要删除的文件路径
            string path = GlobalInfo.MeetingFold + @"\log.txt";
            if (!File.Exists(path)) //log不存在，直接退出即可
                return;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string lineStr = "";
            while ((lineStr=sr.ReadLine()) != null)
            {
                int count = lineStr.IndexOf(':');
                string[] tmpStr = lineStr.Split(':');
                if (tmpStr[0] == "Created")
                {
                    deleteFile.Add(lineStr.Substring(count+1));
                    Console.WriteLine(lineStr.Substring(count + 1));
                }
            }
            sr.Close();
            fs.Close();
            foreach (string filePath in deleteFile)
            {
                DataService.SecurityDelete.DoSecurityDeleteFile(filePath,deleteNumb);
            }
        }
        /// <summary>
        /// 获取某节点下的所有文件夹
        /// </summary>
        /// <param name="tn"></param>
        private void loadFolderByNode(TreeNode tn)
        {
            tn.Nodes.Clear();
            //加载节点tn下的所有文件夹
            if (tn.Name==GlobalInfo.MeetingFold)//根节点
            {
                DirectoryInfo dir = new DirectoryInfo(GlobalInfo.MeetingFold);
                DirectoryInfo[] dirList = dir.GetDirectories();
                foreach (DirectoryInfo x in dirList)
                {
                    TreeNode node = new TreeNode();
                    node.Name = x.FullName;
                    node.Text = x.Name;
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    tn.Nodes.Add(node);
                }
                if (dirList.Count() == 0)
                {
                    displayChildFile(GlobalInfo.MeetingFold);
                }
                else
                {
                    localSpace.SelectedNode = localSpace.Nodes[0];
                }
            }
            else
            {
                string foldPath = tn.Name;
                //查找该节点下的所有文件添加至父节点并展开
                DirectoryInfo dir = new DirectoryInfo(foldPath);
                DirectoryInfo[] dirList = dir.GetDirectories();
                foreach (DirectoryInfo x in dirList)
                {
                    TreeNode node = new TreeNode();
                    node.Name = x.FullName;
                    node.Text = x.Name;
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    tn.Nodes.Add(node);
                }                
            }
        }
        /// <summary>
        /// 节点双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count != 0)
            {
                return;
            }
            loadFolderByNode(e.Node);
            e.Node.ExpandAll();
        }
        /// <summary>
        /// 选定某一节点，在listvie中显示当前folder下的所有文件或文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localSpace_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //读取该节点下的所有文件或文件夹信息
            currentFold = e.Node.Name;
            displayChildFile(e.Node.Name);
        }
        /// <summary>
        /// 显示某路径下的信息
        /// </summary>
        /// <param name="path"></param>
        private void displayChildFile(string path)
        {
            detailListView.Items.Clear();//先清空
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo x in dirs)
            {
                ListViewItem lvi = new ListViewItem(x.Name,getIndexByFileExtention(".folder"));
                lvi.SubItems.Add(x.LastWriteTimeUtc.ToString());
                lvi.SubItems.Add("");
                lvi.SubItems.Add(x.FullName);
                detailListView.Items.Add(lvi);
            }
            FileInfo[] fis = di.GetFiles();
            foreach (FileInfo f in fis)
            {
                Console.WriteLine(f.Name + ":" + f.Attributes.ToString());
                if (!f.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    ListViewItem lvi = new ListViewItem(f.Name, getIndexByFileExtention(f.Extension));
                    // lvi.SubItems.Add(f.LastWriteTimeUtc.ToString());
                    lvi.SubItems.Add(f.LastWriteTime.ToString());
                    if (f.Length / 1024 == 0)//小于1kb
                    {
                        lvi.SubItems.Add(f.Length.ToString() + " byte");
                    }
                    else if (f.Length / 1024 <= 1024)
                    {
                        lvi.SubItems.Add(f.Length / 1024 + "." + f.Length % 1024 + " KB");
                    }
                    else if (f.Length / 1048576 < 1024)
                    {
                        lvi.SubItems.Add(f.Length / 1048576 + "." + f.Length % 1048576 + " MB");
                    }

                    lvi.SubItems.Add(f.FullName);
                    detailListView.Items.Add(lvi);
                }
            }
        }
        /// <summary>
        /// 根据文件后缀名获取图像index
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private int getIndexByFileExtention(string ext)
        {
            if (detailListView.View == View.Details)
            {
                if (smallImageList.Images.Keys.Contains(ext))
                {
                    return smallImageList.Images.IndexOfKey(ext);
                }
                return smallImageList.Images.IndexOfKey(".unkown");
            }
            else
            {
                if (largeImageList.Images.Keys.Contains(ext))
                {
                    return largeImageList.Images.IndexOfKey(ext);
                }
                return largeImageList.Images.IndexOfKey(".unkown");
            }

        }
        /// <summary>
        /// 查看详细视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailListView.View = View.Details;
        }
        /// <summary>
        /// 查看大图标视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailListView.View = View.LargeIcon;
        }
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = detailListView.HitTest(e.X, e.Y);
            if (info.Item == null)
                return;
            else
            {
                //依据item类型处理
                ListViewItem lvi = info.Item as ListViewItem;
                string path = lvi.SubItems[3].ToString();//获取路径值
                path = lvi.SubItems[3].Text;
                //path = lvi.SubItems[2].Name;
                if (lvi.ImageIndex == 0)//文件夹
                {
                    currentFold = path;
                    displayChildFile(path);
                    //
                }
                else
                {
                    Process process = new Process();
                    process.StartInfo.FileName = path;
                    try
                    {
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                }

            }
        }
        /// <summary>
        /// 会议空间创建新文件时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientSpaceWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //MessageBox.Show("发现新文件！");
            //将文件夹添加到树，文件不用操作
            localSpace.Nodes.Clear();
            TreeNode tn = new TreeNode();
            tn.Name = GlobalInfo.MeetingFold;
            tn.Text = "本地会议空间";
            tn.SelectedImageIndex = 0;
            tn.ImageIndex = 0;
            localSpace.Nodes.Add(tn);
            loadFolderByNode(tn);
        }
        /// <summary>
        /// 当有文件被删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientSpaceWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileName = e.FullPath;
            if (detailListView.Items.Count == 0)
            {
                return;
            }
            foreach (ListViewItem lvi in detailListView.Items)
            {
                if (lvi.SubItems[3].Text == e.FullPath)
                {
                    detailListView.Items.Remove(lvi);
                    break;
                }
            }
        }
        /// <summary>
        /// 返回上一级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backUpBtn_Click(object sender, EventArgs e)
        {
            if (currentFold == "")
                return;
            if (Path.GetDirectoryName(currentFold) == Path.GetDirectoryName(GlobalInfo.MeetingFold))
            {
                return;
            }
            currentFold = Path.GetDirectoryName(currentFold);
            displayChildFile(currentFold);
        }
        /// <summary>
        /// 被选中时，背景颜色改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitSystemBtn_MouseEnter(object sender, EventArgs e)
        {
            exitSystemBtn.BackColor = SystemColors.ControlDark;
        }
        /// <summary>
        /// 离开时，背景颜色恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitSystemBtn_MouseLeave(object sender, EventArgs e)
        {
            exitSystemBtn.BackColor = SystemColors.Control;
        }
        /// <summary>
        /// 窗口关闭时，并不真正关闭，而是最小化到托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
