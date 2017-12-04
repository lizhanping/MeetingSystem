using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace DataService
{
    /// <summary>
    /// 基本数据服务
    /// </summary>
    public class DataService
    {
        //todo
        //需要定义一些共用的指令格式和数据传输方面的方法
        public static int Buff_Size = 1024 * 63;//定义缓冲区为63K; 1048576*4;//定义缓冲区大小1M
        public const int HeadLength= 16;//数据头长度
        public const int cmdLength = 8;//命令头长度
        public static string cmdHeadStr = "55AA55AA";//指令
        public static string dataHeadStr = "55BB55BB";//数据
        public static byte[] cmdHead = Encoding.Unicode.GetBytes(cmdHeadStr);//命令头
        public static byte[] dataHead = Encoding.Unicode.GetBytes(dataHeadStr);//数据头
        //以下为定义的指令集
        public const string getUserInfo = "0001"; //获取用户信息 4个字符，Unicode下为8个字节
        public const string recvMeetingInfo = "0002";//接收会议基本信息
        public const string reboot = "0003";//客户端重启
        public const string createFolder = "0004";//新建文件夹
        public const string wakeUpClient = "0005";//唤醒客户端
        public const string endMeetingAndPowerOff = "0006";//结束会议并关机
        public const string openFiles = "0007";//打开文件
        public const string deleteFiles = "0008";//删除文件
        public const string endMeetingOnly = "0009";//结束会议不关机
        public const string ExitMeeting = "0010";//告知主机本机退出会议



        //以下定义响应符号
        public const string executeCmdSucess = "1000";//执行指令正常
        public const string executeCmdFailed = "1001";//执行指令失败
        public const string recvFileHeadSuccess = "1002";//接受文件头成功
        public const string recvFileHeadFailed = "1003";//接收文件头失败
        public const string recvAllFileSuccess = "1004";//接受并创建文件成功
        public const string recvAllFileFailed = "1005";//创建文件失败

        //以下定义失败原因
        public const string objectHasExist = "2000";//对象已存在 ，主要用于创建文件或文件夹时
        public const string createError = "2001";//创建失败。

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="connectSocket">已连接的Socket</param>
        /// <param name="sendDate">欲发送的数据</param>
        /// <returns></returns>
        public static void sendBytes(Socket connectSocket, byte[] sendData)
        {
            try
            {
                int byteSend = sendData.Length;                
                connectSocket.Send(sendData, 0, byteSend, SocketFlags.None);
                Console.WriteLine("已向" + ((IPEndPoint)connectSocket.RemoteEndPoint).Address.ToString() + "发送" + byteSend + "数据！");
                Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        /// <summary>
        /// 发送文件数据
        /// </summary>
        /// <param name="connectSocket">已建立的连接</param>
        /// <param name="basePath">基础路径</param>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static int SendFile(Socket connectSocket, string basePath, string fileName)
        {
            return SendFile(connectSocket, basePath, fileName, false);
        }
        /// <summary>
        /// 发送文件数据
        /// </summary>
        /// <param name="connectSocket">已建立的连接</param>
        /// <param name="basePath">基础路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="encrypt">是否加密传输</param>
        /// <returns></returns>
        public static int SendFile(Socket connectSocket, string basePath,string fileName,bool encrypt)
        {         
            /*思路：先发送文件名和长度，等待回应后再发数据，可以处理数据与头混乱的情况*/
            try
            {
                long fileLength = (new FileInfo(fileName)).Length;              
                Console.WriteLine("文件发送大小为："+fileLength);
                byte[] byteLength = BitConverter.GetBytes(fileLength);//long:8byte,int:4byte
                byte[] byteFileName = Encoding.Unicode.GetBytes(getDirNameByFullPath(basePath,fileName)+"\\"+Path.GetFileName(fileName));
                byte[] sendData = new byte[byteLength.Length + dataHead.Length+byteFileName.Length];
                dataHead.CopyTo(sendData, 0);  //  16字节
                byteLength.CopyTo(sendData, dataHead.Length); //合并  8字节
                byteFileName.CopyTo(sendData, dataHead.Length + byteLength.Length);//合并

                //先发送数据头+文件大小
                sendBytes(connectSocket, sendData);
                Console.WriteLine("发送文件头大小为："+ sendData.Length);

                #region 等待答复后再次发送数据
                byte[] recv = new byte[1024];
                int hasrecv = 0;
                try
                {
                    hasrecv = connectSocket.Receive(recv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (hasrecv > 0)
                {
                    string response = ParseResponse(recv.Skip(0).Take(hasrecv).ToArray());//获取返回指令
                    if (response == DataService.recvFileHeadSuccess)
                    {
                        Console.WriteLine("接收文件头成功，继续发送文件数据");
                    }
                    else
                    {
                        Console.WriteLine("接收文件头失败，请尝试重新发送！");
                        return -1;
                    }
                }
                else
                {
                    Console.WriteLine("未接受到响应！");
                    return -1;
                }
                #endregion

                //Thread.Sleep(200);
                #region 最后发送文件数据
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[Buff_Size ];
                    long numBytesToRead = fs.Length;
                    int num = 0;
                    while ((num=fs.Read(fileData,0,Buff_Size))!=0)
                    {
                        Console.WriteLine("本次读取："+num);
                        byte[] sData = new byte[num];
                        Array.Copy(fileData, sData, num);
                        if (encrypt)
                        {
                            byte[] encodeByte = SecurityTransmit.Encoding(sData);
                            Console.WriteLine("本次发送数据长度为:"+encodeByte.Length+" 已加密");
                            //byte[] encodeByte = SecurityTransmit.Encrypt(sData);
                            sendBytes(connectSocket, encodeByte);
                        }
                        else
                        {
                            Console.WriteLine("本次发送数据长度为："+sData.Length+" 未加密");
                            sendBytes(connectSocket, sData);
                        }
                        //numBytesToRead -= num;
                    }
                    return 0;
                }     
                #endregion
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return -1;//发送失败
            }
        }
        /// <summary>
        /// 解析指令获取返回的响应
        /// </summary>
        /// <param name="recv"></param>
        /// <returns></returns>
        public static string ParseResponse(byte[] recv)
        {
            string resp = Encoding.Unicode.GetString(recv, DataService.HeadLength, DataService.cmdLength);//先获取指令字
            if (resp == DataService.executeCmdSucess)//成功
                return resp;
            if (resp == DataService.recvFileHeadSuccess)//接收文件头成功
                return resp;
            if (resp == DataService.executeCmdFailed)//失败
            {
                resp = Encoding.Unicode.GetString(recv, DataService.HeadLength + DataService.cmdLength, DataService.cmdLength);
                return resp;
            }
            return null;
        }
        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static string getDirNameByFullPath(string basePath,string fullpath)
        {
            string DirName = Path.GetDirectoryName(fullpath);
            DirName = DirName.Substring(basePath.Length);
            return DirName;
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="connectSocket">连接</param>
        /// <param name="cmd">命令字，4个字符</param>
        /// <returns></returns>
        public static int SendCommand(Socket connectSocket,string cmd)
        {
            byte[] cmdByte = Encoding.Unicode.GetBytes(cmd);
            byte[] sendData = new byte[cmdHead.Length + cmdByte.Length];
            cmdHead.CopyTo(sendData, 0);
            cmdByte.CopyTo(sendData, cmdHead.Length);
            try
            {
                connectSocket.Send(sendData, sendData.Length, SocketFlags.None);
            }
            catch
            {
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 发送带数据字的命令
        /// </summary>
        /// <param name="connectSocket"></param>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int SendCommand(Socket connectSocket, string cmd, string data)
        {
            byte[] cmdByte = Encoding.Unicode.GetBytes(cmd);
            byte[] dataByte = Encoding.Unicode.GetBytes(data);
            byte[] sendData = new byte[cmdHead.Length + cmdByte.Length+dataByte.Length];
            cmdHead.CopyTo(sendData, 0);
            cmdByte.CopyTo(sendData, cmdHead.Length);
            dataByte.CopyTo(sendData, cmdHead.Length + cmdByte.Length);
            try
            {
                connectSocket.Send(sendData, sendData.Length, SocketFlags.None);
            }
            catch
            {
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 生成uuid
        /// </summary>
        /// <returns></returns>
        public static string generateUUID()
        {
            string ret = "";
            string allString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] rtn = new char[14];
            Guid guid = Guid.NewGuid();
            var ba = guid.ToByteArray();
            for (var i = 0; i < 14; i++)
            {
                rtn[i] = allString[((ba[i] + ba[i + 1]) % 35)];
            }
            foreach (char r in rtn)
                ret = ret + r;
            return ret;
        }
        /// <summary>
        /// 删除水印
        /// </summary>
        /// <param name="srcbaseName"></param>
        /// <param name="srcFullName"></param>
        /// <param name="desbaseName"></param>
        /// <param name="desFullName"></param>
        public static void deleteMark(string srcFullName, out string desbaseName, out string desFullName)
        {
            //改成后缀为.dat文件可以解决水印文件大小不一致问题，能够获取到正确的文件大小
            string destFileName = Path.GetTempPath() + Path.GetFileName(srcFullName); //
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            File.Copy(srcFullName, destFileName);
            File.Move(destFileName, destFileName+".dat");
            desbaseName = Path.GetTempPath().Substring(0, Path.GetTempPath().Length - 1);
            desFullName = destFileName;
        }
    }
    /// <summary>
    /// 数据安全删除帮助类
    /// </summary>
    public class SecurityDelete
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DoSecurityDeleteFile(object filePath,int level)
        {
            try
            {
                string filename = filePath.ToString();
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
                if (File.Exists(filename))
                {
                    for (int i = 0; i < level + 1; i++)  //删除level+1遍
                    {
                        WriteToFile(filename, 0);
                        WriteToFile(filename, 1);
                    }                    
                    File.SetAttributes(filename, FileAttributes.Normal);
                    double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                    byte[] dummyBuffer = new byte[512];
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    FileStream inputStream = new FileStream(filename, FileMode.Open);
                    inputStream.Position = 0;
                    for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                    {
                        rng.GetBytes(dummyBuffer);
                        inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                        sectorsWritten++;
                    }
                    inputStream.SetLength(0);
                    inputStream.Close();
                    DateTime dt = new DateTime(2049, 1, 1, 0, 0, 0);
                    File.SetCreationTime(filename, dt);
                    File.SetLastAccessTime(filename, dt);
                    File.SetLastWriteTime(filename, dt);
                    File.Delete(filename);    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="folder">目录名</param>
        /// <param name="flag">是否删除自己本身</param>
        public static void DoSecurityDeleteFolder(object folder,int level,bool flag)
        {
            string folderPath = folder.ToString();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
            DirectoryInfo direct = new DirectoryInfo(folderPath);
            FileSystemInfo[] filesystem = direct.GetFileSystemInfos();
            if ((filesystem == null || filesystem.Length == 0) && flag)//删除自己
            {
                direct.Delete();
                return;
            }
            else if (filesystem == null || filesystem.Length == 0)//不删除自身
            {
                return;
            }
            else
            {
                foreach (FileSystemInfo fileItem in filesystem)
                {
                    if (fileItem is FileInfo)
                    {
                        DoSecurityDeleteFile(fileItem.FullName,level);
                    }
                    else
                    {
                        DoSecurityDeleteFolder(fileItem.FullName,level,true);

                    }
                }
                if(flag)
                    direct.Delete();
            }


        }
        /// <summary>
        /// 写value值向filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        private static void WriteToFile(string fileName, int value)
        {
            File.SetAttributes(fileName, FileAttributes.Normal);
            double sectors = Math.Ceiling(new FileInfo(fileName).Length / 512.0);
            FileStream fs = new FileStream(fileName,FileMode.Open);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < sectors; i++)
            {
                for(int j=0;j<512;j++)
                    sw.Write(value);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
    /// <summary>
    /// 数据安全传输帮助类
    /// </summary>
    public class SecurityTransmit
    {
        private static byte[] key = { 24, 55, 102, 24, 98, 26, 67, 29, 84, 19, 37, 118, 104, 85, 121, 27, 93, 86, 24, 55, 102, 24, 98, 26, 67, 29, 9, 2, 49, 69, 73, 92 };
        private static byte[] IV = { 22, 56, 82, 77, 84, 31, 74, 24, 55, 102, 24, 98, 26, 67, 29, 99 };
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="origin">原始数据</param>
        /// <param name="lengh">长度</param>
        /// <returns></returns>
        public static byte[] Encoding(byte[] origin)
        {
            RijndaelManaged myRijndael = new RijndaelManaged();//实例化RijndaelManaged对象
           // myRijndael.Padding = PaddingMode.None;
            try
            {
                ICryptoTransform ic = myRijndael.CreateEncryptor(key, IV);               
                byte[] data = ic.TransformFinalBlock(origin, 0, origin.Length);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encode error!"+ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="origin">原始数据</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] Decoding(byte[] origin)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())//实例化RijndaelManaged对象
            {
                
                try
                {
                    ICryptoTransform ic = myRijndael.CreateDecryptor(key, IV);                  
                    byte[] data = ic.TransformFinalBlock(origin, 0, origin.Length);
                    ic.Dispose();
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Decode error! "+ex.Message);
                    return null;
                }
            }
        }
    }
    /// <summary>
    /// 文件系统控制类
    /// </summary>
    public class FileSystemControll
    {
        private static List<FileSystemWatcher> watchers=new List<FileSystemWatcher>();

        public  static List<FileSystemWatcher> Watchers
        {
            get
            {
                if (watchers.Count!=0)
                    return watchers;
                else
                {
                    getLocalWatchers();
                    return watchers;
                }
            }
        }
        /// <summary>
        /// 根据磁盘获取监视
        /// </summary>
        private static void getLocalWatchers()
        {
            string[] drivers = Directory.GetLogicalDrives();//先获取磁盘
            //构建watcher
            foreach (string str in drivers)
            {
                try
                {
                    watchers.Add(new FileSystemWatcher { Path=str});
                }
                catch
                {
                    Console.WriteLine("get watchers error at "+str+"!");
                }
            }
        }

    }
    /// <summary>
    /// 日志记录帮助类
    /// </summary>
    public class LogManager
    {
        static object locker = new object();
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="path">日志文件路径</param>
        /// <param name="msg"></param>
        public static void logInfo(string path, string msg)
        {
            lock(locker)
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }
    }
    /// <summary>
    /// 获取系统信息帮助类
    /// </summary>
    public class GetSystemInfo
    {
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string getIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipe = Dns.GetHostEntry(hostName);
            foreach (IPAddress ip in ipe.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <returns></returns>
        public static string getUserName()
        {
            return Environment.UserName;
        }

    }

}
