using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.IO;

namespace MeetingSystemServer
{
    public partial class MainForm : Form
    {
        private string defaultMeetingTopic = DateTime.Now.Date.ToString("yyyy年MM月dd日")+"会议";
        private string defaultMeetingDepart = "";
        private string defaultMeetingCreater = "";
        public MainForm()
        {
            InitializeComponent();
            startMeeting.Select();
        }
        /// <summary>
        /// 设置为默认
        /// </summary>
        private void defaultMeetingSet()
        {
            if(MeetingTopic.Text=="")
            {
                MeetingTopic.Text = defaultMeetingTopic;
                MeetingTopic.ForeColor = Color.Gray;
            }
            if (MeetingDepartment.Text == "")
            {
                MeetingDepartment.Text = defaultMeetingDepart;
                MeetingDepartment.ForeColor = Color.Gray;
            }
            if(MeetingCreater.Text=="")
            {
                MeetingCreater.Text = defaultMeetingCreater;
                MeetingCreater.ForeColor = Color.Gray;
            }

        }
        /// <summary>
        /// 开始会议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startMeeting_Click(object sender, EventArgs e)
        {
            startMeeting.Select();
            #region 初始化会议空间
            //检查输入是否有误或者为空的情况
            if (MeetingTopic.Text.Trim().Equals(""))
            {
                MessageBox.Show("会议主题不能为空！");
                MeetingTopic.Focus();
                return;
            }
            if (MeetingDepartment.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写会议组办部门！");
                MeetingDepartment.Focus();
                return;
            }
            if (MeetingCreater.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写会议组办人！");
                MeetingCreater.Focus();
                return;
            }
            //进入会议，新增记录，access数据库
            GlobalInfo.MeetingTopic = MeetingTopic.Text.Trim();
            GlobalInfo.MeetingDepart = MeetingDepartment.Text.Trim();
            GlobalInfo.MeetingCreater = MeetingCreater.Text.Trim();
            GlobalInfo.MeetingCreatTime = DateTime.Now;
            //新数据库
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "insert into meetingtable(topic,department,creater,createtime) values(@topic,@deparment,@creater,@createtime)";
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            ocmd.Parameters.Add("topic", OleDbType.Char);
            ocmd.Parameters.Add("department", OleDbType.Char);
            ocmd.Parameters.Add("creater", OleDbType.Char);
            ocmd.Parameters.Add("createtime", OleDbType.Date);

            ocmd.Parameters["topic"].Value=GlobalInfo.MeetingTopic;
            ocmd.Parameters["department"].Value = GlobalInfo.MeetingDepart;
            ocmd.Parameters["creater"].Value = GlobalInfo.MeetingCreater;
            ocmd.Parameters["createtime"].Value = GlobalInfo.MeetingCreatTime;
            try
            {
                ocmd.ExecuteNonQuery();
                ocmd.Dispose();
                oc.Close();
                //写日志
                string msg = "["+GlobalInfo.MeetingCreatTime+"]:"+"创建会议成功";
                string path = Application.StartupPath + "//log.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                
                DataService.LogManager.logInfo(path, msg);
                //启动会议
                MeetingInfo mi = new MeetingInfo();
                this.Visible = false;
                mi.ShowDialog();
                this.Visible = true;
            }
            catch(Exception ex)
            {
                ocmd.Dispose();
                oc.Close();
                MessageBox.Show("会议创建失败！"+ex.Message);
                //写日志
                string msg = "/r/n" + GlobalInfo.MeetingCreatTime + "创建会议失败";
                string path = Application.StartupPath + "//log.txt";
                DataService.LogManager.logInfo(path, msg);
            }
            #endregion

        }
        /// <summary>
        /// 加载完毕后初始化数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            DataBaseInit();
            readConfigFile();
            readExtraConfig();
            defaultMeetingSet();
            readCurrentSpaceStorage();
       }
        /// <summary>
        /// 读取当前剩余空间容量
        /// </summary>
        private void readCurrentSpaceStorage()
        {
            string driver = Application.StartupPath.Split('/')[0];
            DriveInfo di = new DriveInfo(driver);
            long freeSpace = di.TotalFreeSpace / (1024 * 1024 * 1024);
            string warnStr = "";
            if (freeSpace < 1)
                warnStr = "  >>>请及时清理空间";
            else
                warnStr = "  >>>会议空间充足";
            capacity.Text = "剩余空间：" + freeSpace + " G"+warnStr;            
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        private void readConfigFile()
        {
            MeetingTopic.Items.Clear();
            MeetingDepartment.Items.Clear();
            MeetingCreater.Items.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            string cfgPath = Application.StartupPath + "//config.xml";
            xmlDoc.Load(cfgPath);
            XmlNode rootNode = xmlDoc.SelectSingleNode("config");

            if (rootNode.SelectSingleNode("createByDate").InnerText == "False")
            {
                XmlNode topicNode = rootNode.SelectSingleNode("defaultTopic");
                defaultMeetingTopic = topicNode.ChildNodes[Int32.Parse(topicNode.Attributes["default"].Value)].InnerText;
            }
            else
            {
                defaultMeetingTopic = DateTime.Now.Date.ToString("yyyy年MM月dd日") + "会议";
            }
            defaultMeetingDepart = rootNode.SelectSingleNode("defaultDepart").ChildNodes[Int32.Parse(rootNode.SelectSingleNode("defaultDepart").Attributes["default"].Value)].InnerText;
            defaultMeetingCreater = rootNode.SelectSingleNode("defaultCreater").ChildNodes[Int32.Parse(rootNode.SelectSingleNode("defaultCreater").Attributes["default"].Value)].InnerText;
            foreach (XmlNode xn in rootNode.SelectSingleNode("defaultTopic").ChildNodes)
            {
                MeetingTopic.Items.Add(xn.InnerText);
            }
            foreach (XmlNode xn in rootNode.SelectSingleNode("defaultDepart").ChildNodes)
            {
                MeetingDepartment.Items.Add(xn.InnerText);
            }
            foreach (XmlNode xn in rootNode.SelectSingleNode("defaultCreater").ChildNodes)
            {
                MeetingCreater.Items.Add(xn.InnerText);
            }
            this.WindowState = rootNode.SelectSingleNode("mainForm").ChildNodes[0].InnerText == "True" ? FormWindowState.Maximized : FormWindowState.Normal;
            this.FormBorderStyle = rootNode.SelectSingleNode("mainForm").ChildNodes[1].InnerText == "True" ? FormBorderStyle.Sizable : FormBorderStyle.None;
            string filepath = rootNode.SelectSingleNode("mainForm").ChildNodes[2].InnerText;
            if (filepath != ""&&File.Exists(filepath))
            {
                Image image = new Bitmap(filepath);
                this.BackgroundImage = image;
            }
        }
        /// <summary>
        /// 读取额外配置信息
        /// </summary>
        private void readExtraConfig()
        {
            if (!File.Exists(Application.StartupPath + "//extra.xml"))
            {
                return; //default
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Application.StartupPath + "//extra.xml");
                XmlNode rootNode = xmlDoc.SelectSingleNode("config");
                foreach (XmlNode xn in rootNode.ChildNodes)
                {
                    if (xn.Name == "name")
                    {
                        title.Text = xn.InnerText;
                    }
                    if (xn.Name == "label1")
                    {
                        label1.Text = xn.InnerText;
                    }
                    if (xn.Name == "label2")
                    {
                        label2.Text = xn.InnerText;
                    }
                    if (xn.Name == "label3")
                    {
                        label3.Text = xn.InnerText;
                    }
                }
            }
        }
        /// <summary>
        /// 数据库初始化
        /// </summary>
        private void DataBaseInit()
        {
            string filePath = "";
            filePath = Application.StartupPath + @"\database\meeting.accdb";//数据库路径
            string connectionStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", filePath);
            GlobalInfo.getOleDbConnection(connectionStr);
        }
        /// <summary>
        /// 当获得焦点时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingTopic_Enter(object sender, EventArgs e)
        {
            if (MeetingTopic.Text == defaultMeetingTopic && MeetingTopic.ForeColor == Color.Gray)
            {
                MeetingTopic.Text = "";
                MeetingTopic.ForeColor = Color.Black;  
            }
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingTopic_Leave(object sender, EventArgs e)
        {
            if (MeetingTopic.Text.Trim() == "")
                defaultMeetingSet();
        }
        /// <summary>
        /// 获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingDepartment_Enter(object sender, EventArgs e)
        {
            if (MeetingDepartment.Text == defaultMeetingDepart && MeetingDepartment.ForeColor == Color.Gray)
            {
                MeetingDepartment.Text = "";
                MeetingDepartment.ForeColor = Color.Black;
            }
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingDepartment_Leave(object sender, EventArgs e)
        {
            if (MeetingDepartment.Text.Trim() == "")
                defaultMeetingSet();
        }
        /// <summary>
        /// 获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingCreater_Enter(object sender, EventArgs e)
        {
            if (MeetingCreater.Text == defaultMeetingCreater && MeetingCreater.ForeColor == Color.Gray)
            {
                MeetingCreater.Text = "";
                MeetingCreater.ForeColor = Color.Black;
            }
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeetingCreater_Leave(object sender, EventArgs e)
        {
            if (MeetingCreater.Text.Trim() == "")
                defaultMeetingSet();
        }
        /// <summary>
        /// 清除会议内容
        /// </summary>
        private void clearMeetingInfo()
        {
            MeetingTopic.Text = "";
            MeetingDepartment.Text = "";
            MeetingCreater.Text = "";
        }
        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyBtn_Click(object sender, EventArgs e)
        {
            //进入历史界面，读取数据
            HistoryMeeting hm = new HistoryMeeting();
            hm.ShowDialog();
        }
        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingBtn_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
            readConfigFile();
            clearMeetingInfo();
            defaultMeetingSet();
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要退出会议系统吗?","提示！",MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == DialogResult.OK)
            {
                Application.Exit();
            }
            else
                return;   
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitBtn_MouseEnter(object sender, EventArgs e)
        {
            myToolTip.SetToolTip(exitBtn, "退出系统");
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingBtn_MouseEnter(object sender, EventArgs e)
        {
            myToolTip.SetToolTip(settingBtn, "系统设置");
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyBtn_MouseEnter(object sender, EventArgs e)
        {
            myToolTip.SetToolTip(historyBtn, "历史记录");
        }

        /// <summary>
        /// 解决卡顿问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}
