using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using MSExcel = Microsoft.Office.Interop.Excel;
using System.Xml;
using System.Threading;


namespace MeetingSystemServer
{
    public partial class HistoryMeeting : Form
    {
        private string baseFold = "";//基础目录，不可再回退
        private string currentFold = "";//当前目录
        public HistoryMeeting()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryMeeting_Load(object sender, EventArgs e)
        {
            baseFold = Application.StartupPath + "\\MeetingFold";
            loadImageList(smallImageList, Application.StartupPath + @"\config\smallIcon.xml");
            loadImageList(largeImageList, Application.StartupPath + @"\config\largeIcon.xml");
            LoadHistory();
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
        /// 读取数据库，加载历史
        /// </summary>
        private void LoadHistory()
        {
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "select id,createtime,topic from meetingtable ";
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            try
            {
                OleDbDataReader odr = ocmd.ExecuteReader();
                while (odr.Read())
                {
                    string folder = Convert.ToDateTime(odr[1].ToString()).ToString("yyyyMMddHHmmss");
                    TreeNode tn = new TreeNode();
                    tn.Text = Convert.ToDateTime(odr[1].ToString()).ToString("yyyy年MM月dd日")+"-"+odr[2].ToString();
                    tn.Name = baseFold + "\\" + folder;//定位到文件夹
                    DirectoryInfo di = new DirectoryInfo(tn.Name);
                    
                    if (Directory.Exists(tn.Name)&&(di.GetFileSystemInfos().Count() != 0))//目录存在且内容不为空
                    {
                        tn.ImageIndex = 1;
                        tn.SelectedImageIndex = 1;
                    }
                    else
                    {
                        tn.ImageIndex = 0;
                        tn.SelectedImageIndex = 0;
                    }
                    tn.Tag = odr[0];
                    historyTree.Nodes.Add(tn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ocmd.Dispose();
                oc.Close();
            }

        }
        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <param name="tn"></param>
        private void readNodeBaseInfo(TreeNode tn)
        {
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "select * from meetingtable where id=" + Int32.Parse(tn.Tag.ToString());
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            try
            {
                OleDbDataReader odr = ocmd.ExecuteReader();
                while (odr.Read())
                {
                    topic.Text = odr["topic"].ToString();
                    department.Text = odr["department"].ToString();
                    creater.Text = odr["creater"].ToString();
                    statrttime.Text = odr["createtime"].ToString();
                    endtime.Text = odr["endtime"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ocmd.Dispose();
                oc.Close();
            }
        }
        /// <summary>
        /// 节点被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //读取节点信息
            readNodeBaseInfo(e.Node);
            //搜索当前目录显示到listview中
            currentFold = e.Node.Name; //每次点击后更新当前目录
            detailListView.Items.Clear();
            if (e.Node.ImageIndex == 1)
            {
                clearToolStripMenuItem.Enabled = true;
                readNodeFileInfo(e.Node.Name);
            }
            else
            {
                clearToolStripMenuItem.Enabled = false;
                return;
            }
                
        }
        /// <summary>
        /// 读取节点会议资料信息
        /// </summary>
        /// <param name="tn"></param>
        private void readNodeFileInfo(string fullPath)
        {
            
            DirectoryInfo di = new DirectoryInfo(fullPath);
            try
            {
                FileSystemInfo[] fis = di.GetFileSystemInfos();
                foreach (FileSystemInfo x in fis)
                {
                    if (x is DirectoryInfo)
                    {
                        ListViewItem lvi = new ListViewItem(x.Name, getIndexByFileExtention(".folder"));
                        lvi.SubItems.Add(x.LastWriteTimeUtc.ToString());
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add(x.FullName);
                        detailListView.Items.Add(lvi);
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(x.Name, getIndexByFileExtention(x.Extension));
                        // lvi.SubItems.Add(f.LastWriteTimeUtc.ToString());
                        lvi.SubItems.Add(x.LastWriteTime.ToString());
                        lvi.SubItems.Add(((FileInfo)x).Length.ToString() + " byte");
                        lvi.SubItems.Add(x.FullName);
                        detailListView.Items.Add(lvi);
                    }
                }
            }
            catch //没找着，已经被清过
            {
                return;
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
        /// 详细视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailListView.View = View.Details;
        }
        /// <summary>
        /// 大图标视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bigViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailListView.View = View.LargeIcon;
        }
        /// <summary>
        /// 双击打开
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
                    detailListView.Items.Clear();//先清除以前
                    readNodeFileInfo(path);
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
        /// 返回上一级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backUpLevelBtn_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(currentFold) == baseFold) //到根目录则无法回退
            {
                return;
            }
            //否则，打开上级目录，就是返回
            detailListView.Items.Clear();
            readNodeFileInfo(Path.GetDirectoryName(currentFold));
            currentFold = Path.GetDirectoryName(currentFold);
        }
        /// <summary>
        /// 清除空间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清除本次会议空间吗？（清除后不可恢复）","提示！",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Thread deleteThread = new Thread(new ParameterizedThreadStart(processDelete));
                waitForm.waitForm wf = new waitForm.waitForm();
                wf.setText("正在清理...");
                wf.setMonit(deleteThread,historyTree.SelectedNode.Name);
                wf.ShowDialog();
                historyTree.SelectedNode.ImageIndex = 0;
                historyTree.SelectedNode.SelectedImageIndex = 0;
                detailListView.Items.Clear();
                MessageBox.Show("清理成功！");
                return;
            }
        }
        /// <summary>
        /// 删除进程
        /// </summary>
        private void processDelete(object path)
        {
            string folder = (string)path;
            DataService.SecurityDelete.DoSecurityDeleteFolder(folder, Int32.Parse(readValueFromConfigByNode("clearLevel").ToString()), false);

        }
        /// <summary>
        /// 全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "select topic as 会议主题,department as 办会部门,creater as 办会人, createtime as 会议开始时间, endtime as 会议结束时间 from meetingtable";
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            OleDbDataAdapter oda = new OleDbDataAdapter(ocmd);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("未查到有效数据！");
                return;
            }
            GlobalInfo.DataTableToExcel(dt, "所有会议记录");
        }
        /// <summary>
        /// 选择导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectForm sf = new selectForm();
            sf.ShowDialog();
        }
        /// <summary>
        /// 历史记录的导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
    }
}
